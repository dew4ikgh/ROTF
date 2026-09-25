using System.ComponentModel.DataAnnotations;

public class PlayerProgress
{
    [Key]
    public int id { get; set; }

    [Required]
    public int userid { get; set; }

    [Required]
    public int serverid { get; set; }

    public float health { get; set; } = 100.0f;
    public float stamina { get; set; } = 100.0f;
    public float hunger { get; set; } = 100.0f;
    public float thirst { get; set; } = 100.0f;
    public float fatigue { get; set; } = 0.0f;
    public float temperature { get; set; } = 36.6f;

    public float positionx { get; set; }
    public float positiony { get; set; }
    public float positionz { get; set; }

    public DateTime lastsaved { get; set; } = DateTime.Now;
}