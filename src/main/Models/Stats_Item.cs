using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models
{
    public class Stats_Item
    {
        public int Id { get; set; }
        public DateTime Dob { get; set; }

        [ForeignKey("Item")]
        public int IdItem { get; set; }
    }
}