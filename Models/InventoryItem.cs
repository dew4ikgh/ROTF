using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROTF.Server.Models
{
    [Table("inventory")]
    public class InventoryItem
    {
        [Key]
        public int id { get; set; }

        public int playerprogressid { get; set; }

        public int itemid { get; set; }

        public int quantity { get; set; }
        [ForeignKey("itemid")]
        public Item? item { get; set; }
    }
}