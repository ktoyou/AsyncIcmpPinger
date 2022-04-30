using Microsoft.Extensions.Logging;
using PingerInfo.Core.Abstractions;
using PingerInfo.Core.DB;
using PingerInfo.Core.DB.Model;
using System.Net.NetworkInformation;

namespace PingerInfo.Core
{
    /// <summary>
    /// Пингует объекты из базы данных и обновляет онлайн объектов
    /// </summary>
    internal class MySqlPinger : BaseMySqlPinger
    {
        public MySqlPinger(int period, int timeout, DbApplicationContext dbApplicationContext, ILogger<BaseMySqlPinger>? logger = null) : base(period, timeout, dbApplicationContext, logger) {}

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

        protected override async Task PingDoneAsync()
        {
            _logger?.Log(LogLevel.Information, "saving changes to database");
            await _dbApplicationContext.SaveChangesAsync();
        }
    }
}
