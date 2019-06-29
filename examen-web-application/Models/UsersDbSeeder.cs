using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace examen_web_application.Models
{
    public class UsersDbSeeder
    {
        public static void Initialize(UsersDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            context.Users.AddRange(
                new User
                {
                    FirstName = "ovi1",
                    LastName = "ovi1",
                    Username = "ovi1",
                    Email = "ovi1@yahoo.com",
                    Password = ComputeSha256Hash("123456789"),
                    UserRole = UserRole.Admin,
                },
               new User
               {
                   FirstName = "ovi12",
                   LastName = "ovi12",
                   Username = "ovi12",
                   Email = "ovi12@yahoo.com",
                   Password = ComputeSha256Hash("123456789"),
                   UserRole = UserRole.Regular,
               },
               new User
               {
                   FirstName = "ovi123",
                   LastName = "ovi123",
                   Username = "ovi123",
                   Email = "ovi12@yahoo.com",
                   Password = ComputeSha256Hash("123456789"),
                   UserRole = UserRole.Moderator,
               }
            );
            context.SaveChanges();
        }   
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            // TODO: also use salt
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

}

