using Chatbot_in.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class SeedDB
{
    public static async void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<HetznerDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["User"];

        foreach (string role in roles)
        {
            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == role))
            {
                await roleStore.CreateAsync(new IdentityRole()
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
        }

        context.Database.EnsureCreated();
        if (!context.Users.Any())//Besser hier Accounts direkt prüfen
        {
            ApplicationUser userSimple = new ApplicationUser()
            {
                Email = "user@fhv.at",
                UserName = "user",
                SecurityStamp = Guid.NewGuid().ToString(),
                NormalizedUserName = "user".ToUpper()
            };
            await userManager.CreateAsync(userSimple, "1234567Aa!");
            await userManager.AddToRoleAsync(userSimple, "User");


            await context.SaveChangesAsync();

        }
    }


}
