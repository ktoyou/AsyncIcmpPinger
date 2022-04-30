using System.Text;
using Newtonsoft.Json;
using PingerInfo.Core.DB.Model;

namespace PingerInfo.Core.Json;

public class SwitchesJsonLoader
{
    public static IEnumerable<PingObject> GetPingObjects(string filename)
    {
        if (!File.Exists(filename)) throw new FileNotFoundException("not found json", filename);
        using FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, buffer.Length);
        return JsonConvert.DeserializeObject<IEnumerable<PingObject>>(Encoding.UTF8.GetString(buffer));
    }

    public static async Task SavePingObjectsAsync(string filename, IEnumerable<PingObject> pingObjects)
    {
        if (!File.Exists(filename)) throw new FileNotFoundException("not found json", filename);
        using FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Write);
        byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pingObjects));
        await fs.WriteAsync(buffer, 0, buffer.Length);
    }
}