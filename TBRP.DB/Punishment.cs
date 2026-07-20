using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TBRP.DB;

public class Punishment
{
    [Key]
    public int Id { get; set; }
    
    public PunishmentType Type { get; set; }
    public long RobloxId { get; set; }
    public string Reason { get; set; }
    public ulong CreatorId { get; set; }
    public bool ActionTaken { get; set; }
    public DateTime? Expiry { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    
    public Punishment()
    {
        UpdatedDate = DateTime.UtcNow;
        CreatedDate ??= UpdatedDate;
    }
}

public enum PunishmentType
{
    Warn,
    Kick,
    Ban
}