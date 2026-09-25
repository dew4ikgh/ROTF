using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROTF.Server.Models
{
    public class GameServer
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public int adminid { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime createdat { get; set; }
    }
}