using Microsoft.Extensions.Logging;
using PingerInfo.Core.Abstractions;
using PingerInfo.Core.DB;
using PingerInfo.Core.DB.Model;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PingerInfo.Core.Json;

public class JsonPinger : BaseJsonPinger
{
    public JsonPinger(int period, int timeout, string filename, ILogger<BaseJsonPinger>? logger) : base(period, timeout, filename, logger) { }

    protected override Task ReceivePacketAsync(PingObject pingObject, PingReply pingReply)
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
        
        return Task.CompletedTask;
    }

    protected override async Task PingDoneAsync()
    {
        _logger?.Log(LogLevel.Information, "saving changes to json file");
        await SwitchesJsonLoader.SavePingObjectsAsync(_filename, _pingObjects);
    }
}