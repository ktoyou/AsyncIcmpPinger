using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Extensions.Logging;
using PingerInfo.Core.DB.Model;
using PingerInfo.Core.Json;

namespace PingerInfo.Core.Abstractions
{
    /// <summary>
    /// Базовый абстрактный класс который пингует объекты с Json файла
    /// </summary>
    public abstract class BaseJsonPinger : BasePinger
    {
        protected BaseJsonPinger(int period, int timeout, string filename, ILogger<BaseJsonPinger>? logger) : base(period, timeout)
        {
            _logger = logger;
            _filename = filename;
        }

        protected override async Task UpdateAsync()
        {
            _pingObjects = new List<PingObject>(SwitchesJsonLoader.GetPingObjects(_filename));
            SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);
        
            var tasks = _pingObjects.Select(async item =>
            {
                await semaphore.WaitAsync();
                try
                {
                    Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(item.GetAddr(), 2000);
                    await ReceivePacketAsync(item, reply);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        
            await Task.WhenAll(tasks);
            await PingDoneAsync(_pingObjects);
        }

        protected List<PingObject> _pingObjects;

        protected ILogger<BaseJsonPinger> _logger;

        protected string _filename;
    }
}