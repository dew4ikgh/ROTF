using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROTF.Server.Models
{
    [Table("buildings")]
    public class Building
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int serverid { get; set; }

        [Required]
        public int ownerid { get; set; }

        [Required]
        public string buildingtype { get; set; } 

        public float positionx { get; set; }
        public float positiony { get; set; }
        public float positionz { get; set; }

        public float health { get; set; } = 100.0f;
    }
}
