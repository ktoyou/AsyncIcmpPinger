using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PingerInfo.Core.DB;
using PingerInfo.Core.DB.Model;
using System.Net;
using System.Net.NetworkInformation;

namespace PingerInfo.Core.Abstractions
{
    /// <summary>
    /// Базовый абстрактный класс который пингует объекты из базы данных MySql
    /// </summary>
    internal abstract class BaseMySqlPinger : BasePinger
    {
        public BaseMySqlPinger(int period, int timeout, DBConfiguration configuration,  ILogger<BaseMySqlPinger>? logger = null) : base(period, timeout)
        {
            _configuration = configuration;
            _logger = logger;
            _logger?.Log(LogLevel.Information, "pinger initialized");
        }

        /// <summary>
        /// Данный метод создает экземпляр базы и пингует объекты, после чего вызывает метод DisposeAsync у базы.
        /// </summary>
        protected override async Task UpdateAsync()
        {
            _dbApplicationContext = new DbApplicationContext(_configuration);
            List<PingObject> pingObjects = await _dbApplicationContext.PingObjects.Where(elem => elem.Enabled).ToListAsync();
            using SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);

            var tasks = Task.WhenAll(pingObjects.Select(async item =>
            {
                await semaphore.WaitAsync();
                try
                {
                    using Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(item.GetAddr(), 1000);
                    await ReceivePacketAsync(item, reply);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
            await Task.WhenAll(tasks);
            await PingDoneAsync(pingObjects);
            await _dbApplicationContext.DisposeAsync();
        }

        protected DbApplicationContext _dbApplicationContext;

        private readonly DBConfiguration _configuration;
        
        protected readonly ILogger<BaseMySqlPinger>? _logger;
    }
}
