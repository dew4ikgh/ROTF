using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROTF.Server.Models
{
    [Table("users_r")]
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string passwordhash { get; set; }

        public int roleid { get; set; } = 1;
    }
}