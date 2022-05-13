using Microsoft.Extensions.Logging;
using PingerInfo.Core.Abstractions;
using PingerInfo.Core.DB;
using PingerInfo.Core.DB.Model;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;

namespace PingerInfo.Core
{
    /// <summary>
    /// Пингует объекты из базы данных и обновляет онлайн объектов
    /// </summary>
    internal class MySqlPinger : BaseMySqlPinger
    {
        public MySqlPinger(int period, int timeout, DBConfiguration configuration, ILogger<BaseMySqlPinger>? logger = null) : base(period, timeout, configuration, logger) {}

        protected override async Task ReceivePacketAsync(PingObject pingObject, PingReply pingReply)
        {
            switch (pingReply.Status)
            {
                case IPStatus.Success:
                    pingObject.Online = true;
                    _logger?.Log(LogLevel.Information, $"object {pingObject.Address} online. ID={pingObject.ID}");
                    
                    break;
                default:
                    pingObject.Online = false;
                    _logger?.Log(LogLevel.Information, $"object {pingObject.Address} offline. ID={pingObject.ID}");
                    break;
            }
        }

        protected override async Task PingDoneAsync(List<PingObject> pingObjects)
        {
            pingObjects.ForEach(elem =>
            {
                var eventId = $"{elem.ID}_device-down";
                var e = _dbApplicationContext.Events.Where(elem => elem.EventID.Equals(eventId)).FirstOrDefault();

                switch (elem.Online)
                {
                    case true when e != null:
                        _dbApplicationContext.Events.Remove(e);
                        break;
                    case false when e == null:
                        _dbApplicationContext.Events.Add(new Event()
                        {
                            Message = $"Девайс с именем {elem.Title} недоступен. ID девайса - {elem.ID}",
                            Begin = (int) DateTimeOffset.Now.ToUnixTimeSeconds() + 3600 * 3,
                            Level = 4,
                            EventID = eventId
                        });
                        break;
                    case false:
                        e.Begin = (int) DateTimeOffset.Now.ToUnixTimeSeconds() + 3600 * 3;
                        break;
                }
            });
            
            
            _logger?.Log(LogLevel.Information, "saving changes to database");
            await _dbApplicationContext.SaveChangesAsync();
        }
        
    }
}
