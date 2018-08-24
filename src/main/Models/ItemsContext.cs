namespace Main.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ItemsContext : IdentityDbContext<UserManage>
    {
        private readonly IConfigurationRoot config;

        public ItemsContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            this.config = config;
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Stats_Item> Stats_Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Localization> Localizations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(this.config["ConnectionStrings:seed_dotnetContextConnection"]);
        }
    }
}