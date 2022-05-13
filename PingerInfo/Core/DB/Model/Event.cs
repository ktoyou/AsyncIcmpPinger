using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("events")]
public class Event
{
    [Key] public int ID { get; set; }
    
    [Column("level")] public int Level { get; set; }
    
    [Column("event_id")] public string EventID { get; set; }
    
    [Column("message")] public string Message { get; set; }
    
    [Column("begin")] public int Begin { get; set; }
    
    [Column("end")] public int End { get; set; }
}