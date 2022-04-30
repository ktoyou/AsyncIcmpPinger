using Newtonsoft.Json;
using System.Text;

namespace PingerInfo.Core.DB
{
    /// <summary>
    /// Конфигурация базы данных
    /// </summary>
    internal class DBConfiguration
    {
        [JsonProperty("server")] public string? Server { get; set; }

        [JsonProperty("db")] public string? DataBase { get; set; }

        [JsonProperty("uid")] public string? UID { get; set; }

        [JsonProperty("password")] public string? Password { get; set; }

        public static DBConfiguration LoadConfiguration(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("config not found");

            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return JsonConvert.DeserializeObject<DBConfiguration>(Encoding.UTF8.GetString(buffer));
        }
    }
}
