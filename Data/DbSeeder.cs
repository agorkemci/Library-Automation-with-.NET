using Library_Automation.Models;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_Automation.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            // Context nesnesi: C# kodunuz ile fiziksel veritabanı arasındaki köprüyü, iletişimi yöneten nesnedir.
            var context =services.GetRequiredService<ApplicationDbContext> ();
            if(!await context.Categories.AnyAsync())//tabloda en az bir kayıt var mı? yoksa...
            {//birden fazla kategori eklemek için AddRange kullanıyoruz
                context.Categories.AddRange(
                    new Category { Name = "Science Fiction" },
                    new Category { Name = "Fantasy" },
                    new Category { Name = "Mystery" },
                    new Category { Name = "Romance" });
     
            }
            await context.SaveChangesAsync();

            string[] roles = new string[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            const string adminEmail = "admin@library.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Admin User"
                };
                var result = await userManager.CreateAsync(admin, "Admin123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

            }
        }
    }
}
