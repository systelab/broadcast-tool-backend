namespace Main.ViewModels
{
    using Main.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ItemImageViewModel
    {
        /// <summary>
        /// Email address
        /// </summary>

        public string content { get; set; }
        public int id { get; set; }
        public string name { get; set; }
      
        public string type { get; set; }

    }
}