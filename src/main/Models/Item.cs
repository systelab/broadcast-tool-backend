using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models
{
    public class Item
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Dob { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public bool Pinned { get; set; }
        public bool Deleted { get; set; }
        public bool Draft { get; set; }
        public DateTime ExpirationDate { get; set; }
        [ForeignKey("Category")]
        public int IdCategory { get; set; }

        [ForeignKey("Localization")]
        public int IdLocalization { get; set; }

    }
}