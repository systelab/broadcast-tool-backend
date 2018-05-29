namespace Main.Models
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    public class ItemsContextSeedData
    {

        private readonly UserManager<UserManage> userManager;

        public ItemsContextSeedData(UserManager<UserManage> _userM)
        {
            this.userManager = _userM;
        }

        public async Task EnsureSeedData()
        {

            if (await this.userManager.FindByNameAsync("Systelab") == null)
            {
                var user = new UserManage
                {
                    UserName = "Systelab",
                    Name = "Systelab",
                    LastName = "Seed_Dotnet",
                    Email = "Systelab@werfen.com",
                    Admin = false
                };
                await this.userManager.CreateAsync(user, "Systelab");
            }

            if (await this.userManager.FindByNameAsync("admin") == null)
            {
                var user = new UserManage
                               {
                                   UserName = "admin",
                                   Name = "Administrator",
                                   LastName = "Seed_Dotnet",
                                   Email = "admin@werfen.com",
                                   Admin = true
                               };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }

           
        }
    }
}