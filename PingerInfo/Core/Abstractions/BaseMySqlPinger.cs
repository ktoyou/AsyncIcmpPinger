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
        public BaseMySqlPinger(int period, int timeout, DbApplicationContext dbApplicationContext, ILogger<BaseMySqlPinger>? logger = null) : base(period, timeout)
        {
            _dbApplicationContext = dbApplicationContext;
            _logger = logger;
            _logger?.Log(LogLevel.Information, "pinger initialized");
        }

        protected override async Task UpdateAsync()
        {
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
            await PingDoneAsync();
        }

        protected readonly DbApplicationContext _dbApplicationContext;

        protected readonly ILogger<BaseMySqlPinger>? _logger;
    }
}
