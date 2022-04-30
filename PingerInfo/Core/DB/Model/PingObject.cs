using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace PingerInfo.Core.DB.Model
{
    [Table("ping_objects")]
    public class PingObject
    {
        [Key] public int ID { get; set; }

        [JsonProperty("address")] [Column("address")] public string? Address { get; set; }

        [JsonProperty("title")] [Column("title")] public string? Title { get; set; }

        [JsonProperty("description")] [Column("description")] public string? Description { get; set; }

        [JsonProperty("online")] [Column("online")] public bool Online { get; set; }

        [JsonProperty("enabled")] [Column("enabled")] public bool Enabled { get; set; }

        public IPAddress GetAddr()
        {
            try
            {
                return IPAddress.Parse(Address);
            }
            catch (Exception e)
            {
                return IPAddress.None;
            }
        }
    }
}
