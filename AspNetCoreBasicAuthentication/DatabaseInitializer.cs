using System.Threading.Tasks;
using AspNetCoreBasicAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreBasicAuthentication
{
    public class DatabaseInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private DatabaseInitializer(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public static async Task InitializeDatabase(IServiceScope serviceScope)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var databaseInitializer = new DatabaseInitializer(userManager);
            await databaseInitializer.InitializeDatabase();
        }
        
        private async Task InitializeDatabase()
        {
            var user = new ApplicationUser
            {
                UserName = AppConstants.USERNAME,
                Email = AppConstants.USERNAME
            };
            var userCreationResult = await _userManager.CreateAsync(user, AppConstants.PASSWORD);
            if (!userCreationResult.Succeeded)
            {
                throw new System.Exception("Ease the Asp.Net Core Identity password requirements, could not create initial user.");
            }
        }
    }
}
