using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROTF.Server.Models
{
    public class WorldItem
    {
        [Key]
        public int id { get; set; }

        public int itemid { get; set; }
        public int serverid { get; set; }
        public int quantity { get; set; }
        public float positionx { get; set; }
        public float positiony { get; set; }
        public float positionz { get; set; }

        [ForeignKey("itemid")]
        public Item? item { get; set; }

        [ForeignKey("serverid")]
        public GameServer? server { get; set; }
    }
}