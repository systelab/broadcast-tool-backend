namespace Main.ViewModels
{
    using Main.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ItemViewModel
    {
        /// <summary>
        /// Email address
        /// </summary>

        public string Email { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Dob { get; set; }

        public string Description { get; set; }
        public int Type { get; set; }
        public string Path { get; set; }
        [Required]
        public string Title { get; set; }

        public int access { get; set; }
        public bool Pinned { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool Deleted { get; set; }

        public int IdCategory { get; set; }
        public string CategoryName { get; set; }

        public bool Draft { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string expDate { get; set; }
    }
}