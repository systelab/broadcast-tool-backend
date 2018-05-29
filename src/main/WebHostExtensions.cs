namespace Main
{
    using Main.Models;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    public static class WebHostExtensions
    {
        public static IWebHost Seed(this IWebHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ItemsContextSeedData>();
                seeder.EnsureSeedData().Wait();
            }

            return webhost;
        }

    }
}