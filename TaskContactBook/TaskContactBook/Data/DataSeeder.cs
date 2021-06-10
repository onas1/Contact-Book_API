using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskContactBook.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TaskContactBookDbContext context)
        {

            context.Database.EnsureCreated();
            await SeedRoleAsync(roleManager);
            SeedUser(userManager);
        }

        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            //check the role to make sure role is null before assigning it.
            var CheckRoleRegular = roleManager.RoleExistsAsync("Regular").Result;
            if ( CheckRoleRegular== false)
            {

                IdentityRole role = new IdentityRole
                {
                    Name = "Regular"
                };
                await roleManager.CreateAsync(role);
                    
            }

            //Create Admin Role...
            var CheckRoleAdmin = roleManager.RoleExistsAsync("Admin").Result;
            if (CheckRoleAdmin == false)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }
        
        }
        //code to seed Users and assign roles

        public static void SeedUser(UserManager<AppUser> usermanager)
        {
            //seed admin role
            var CheckUser =  usermanager.FindByEmailAsync("Iriajenfrancis@gmail.com");
            if (CheckUser.Result == null)
            {
                var user1 = new AppUser
                {
                    FirstName = "Onas",
                    LastName = "Iriajen",
                    Email = "Iriajenfrancis@gmail.com",
                    PhoneNumber = "08135017101",
                    Street = "7 Asanjon Way",
                    City = "Sangotedo",
                    Country = "Nigera",
                    UserName = "Iriajenfrancis@gmail.com"
                };
                IdentityResult result = usermanager.CreateAsync(user1, "Password@1").Result;
                if (result.Succeeded)
                {
                    usermanager.AddToRoleAsync(user1, "Admin").Wait(); 
                }
            }
            //seed regular role
            var CheckUser2 = usermanager.FindByEmailAsync("user2@localhost.com");
            if (CheckUser2.Result == null)
            {
                var user2 = new AppUser
                {
                    FirstName = "Aaron",
                    LastName = "Iriajen",
                    Email = "user2@localhost.com",
                    PhoneNumber = "08125017101",
                    Street = "9 Asanjon Way",
                    City = "Sangotedo",
                    Country = "Nigera",
                    UserName = "user2@localhost.com"

                };
                IdentityResult result = usermanager.CreateAsync(user2, "Password@2").Result;
                if (result.Succeeded)
                {
                    usermanager.AddToRoleAsync(user2, "Regular").Wait();
                }
            }



        }

    }
}
