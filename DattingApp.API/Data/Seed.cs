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
        public static void SeedUsers(UserManager<User> userManager) // (DataContext context)
        {
            if (!userManager.Users.Any())
            {
                var usrData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var usrs = JsonConvert.DeserializeObject<List<User>>(usrData);
                foreach(var u in usrs)
                {   
                    userManager.CreateAsync(u, "password").Wait();
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