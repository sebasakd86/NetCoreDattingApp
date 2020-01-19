using System;
using System.Collections.Generic;
using System.Linq;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DattingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(
            UserManager<User> userManager,
            RoleManager<Role> roleManager
        ) // (DataContext context)
        {
            if (!userManager.Users.Any())
            {
                var usrData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var usrs = JsonConvert.DeserializeObject<List<User>>(usrData);

                var roles = new List<Role>
                {
                    new Role{Name = "Member"},
                    new Role{Name = "Admin"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "VIP"}
                };
                foreach(var r in roles)
                {
                    roleManager.CreateAsync(r).Wait();
                }
                foreach(var u in usrs)
                {   
                    userManager.CreateAsync(u, "password").Wait();
                    userManager.AddToRoleAsync(u, "Member");
                }

                var admUser = new User
                {
                    UserName = "Admin"
                };
                
                var result = userManager.CreateAsync(admUser, "password").Result;;
                if(result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
                }
            }
        }
       private static void CreatePassWordHash(string password, out byte[] pHash, out byte[] pSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pSalt = hmac.Key;
                pHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}