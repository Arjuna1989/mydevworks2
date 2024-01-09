using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUser(DataContext context)
        {

            if (!await context.users.AnyAsync()) return;

            var UserData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<User>>(UserData, options);

            foreach (var user in users)
            {
                using var hamc = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hamc.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hamc.Key;

                context.users.Add(user);

            }
            await context.SaveChangesAsync();


        }
    }
}