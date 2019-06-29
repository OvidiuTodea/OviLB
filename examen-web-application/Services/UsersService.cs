using examen_web_application.Models;
using examen_web_application.Viewmodels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace examen_web_application.Services
{
    public interface IUsersService
    {
        UserGetModel Authenticate(string username, string password);
        UserGetModel Register(RegisterPostModel registerInfo);
        User GetCurrentUser(HttpContext httpContext);
        IEnumerable<UserGetModel> GetAll();

        User GetById(int id);
        User Create(UserPostModel user);
        User Upsert(int id, UserPostModel userPostModel, User addeBy);
        User Delete(int id);

    }

    public class UsersService : IUsersService
    {
        private UsersDbContext context;
        private readonly AppSettings appSettings;

        public UsersService(UsersDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }
        public UserGetModel Authenticate(string username, string password)
        {
            var user = context.Users
                .SingleOrDefault(x => x.Username == username &&
                                 x.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role,user.UserRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token)
            };

            return result;
        }

        private string ComputeSha256Hash(string rawData)
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

        public UserGetModel Register(RegisterPostModel registerInfo)
        {
            User existing = context.Users.FirstOrDefault(u => u.Username == registerInfo.Username);
            if (existing != null)
            {
                return null;
            }

            context.Users.Add(new User
            {
                Email = registerInfo.Email,
                LastName = registerInfo.LastName,
                FirstName = registerInfo.FirstName,
                Password = ComputeSha256Hash(registerInfo.Password),
                Username = registerInfo.Username,
                UserRole = UserRole.Regular,

            });
            context.SaveChanges();
            return Authenticate(registerInfo.Username, registerInfo.Password);
        }

        public User GetCurrentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            //string accountType = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
            //return context.Users.FirstOrDefault(u => u.Username == username && u.AccountType.ToString() == accountType);
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public IEnumerable<UserGetModel> GetAll()
        {
            // return users without passwords
            return context.Users.Select(user => new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = null
            });
        }

        public User GetById(int id)
        {
            return context.Users
                .FirstOrDefault(u => u.Id == id);
        }

        public User Create(UserPostModel user)
        {
            User toAdd = UserPostModel.ToUser(user);

            context.Users.Add(toAdd);
            context.SaveChanges();
            return toAdd;

        }

        public User Upsert (int id, UserPostModel user, User addedBy)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                User toAdd = UserPostModel.ToUser(user);
                user.Password = ComputeSha256Hash(user.Password);
                context.Users.Add(toAdd);
                context.SaveChanges();
                return toAdd;
            }

            User toUpdate = UserPostModel.ToUser(user);
            toUpdate.Password = existing.Password;
            toUpdate.CreatedAt = existing.CreatedAt;
            toUpdate.Id = id;

            if (user.UserRole.Equals("Admin") && !addedBy.UserRole.Equals(UserRole.Admin))
            {
                return null;
            }
            else if ((existing.UserRole.Equals(UserRole.Regular) && addedBy.UserRole.Equals(UserRole.Moderator)) ||
                (existing.UserRole.Equals(UserRole.Moderator) && addedBy.UserRole.Equals(UserRole.Moderator) && addedBy.CreatedAt.AddMonths(6) <= DateTime.Now))
            {
                context.Users.Update(toUpdate);
                context.SaveChanges();
                return toUpdate;
            }
            else if (addedBy.UserRole.Equals(UserRole.Admin))
            {
                context.Users.Update(toUpdate);
                context.SaveChanges();
                return toUpdate;
            }
            return null;
        }

        public User Delete(int id)
        {
            var existing = context.Users.FirstOrDefault(user => user.Id == id);
            if (existing == null)
            {
                return null;
            }
            context.Users
           .Remove(existing);
            context.SaveChanges();
            return existing;
        }
    }
} 

