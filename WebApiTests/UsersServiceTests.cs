using examen_web_application.Models;
using examen_web_application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING"
            });
        }

        [Test]
        public void ValidRegisterShouldCreateANewUser()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(ValidRegisterShouldCreateANewUser))// "ValidRegisterShouldCreateANewUser")
              .Options;

            using (var context = new UsersDbContext(options))
            {
                UsersService usersService = new UsersService(context, config);
                var added = new examen_web_application.Viewmodels.RegisterPostModel

                {
                    Email = "ovi@yahoo.com",
                    FirstName = "ovi",
                    LastName = "ovi",
                    Password = "123456789",
                    Username = "ovi",
                };
                var result = usersService.Register(added);

                Assert.IsNotNull(result);
                Assert.AreEqual(added.Username, result.Username);

            }
        }

        [Test]
        public void AuthenticateShouldLoginSuccessfullyTheUser()
        {

            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(AuthenticateShouldLoginSuccessfullyTheUser))// "ValidUsernameAndPasswordShouldLoginSuccessfully")
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UsersService(context, config);

                var added = new examen_web_application.Viewmodels.RegisterPostModel

                {
                    Email = "ovi1@yahoo.com",
                    FirstName = "ovi1",
                    LastName = "ovi1",
                    Password = "123456789",
                    Username = "ovi1",
                };
                usersService.Register(added);
                var loggedIn = new examen_web_application.Viewmodels.LoginPostModel
                {
                    Username = "ovi1",
                    Password = "123456789"

                };
                var authoresult = usersService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(authoresult);
                Assert.AreEqual(1, authoresult.Id);
                Assert.AreEqual(loggedIn.Username, authoresult.Username);
                
            }


        }



        [Test]
        public void ValidGetAllShouldDisplayAllUsers()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(AuthenticateShouldLoginSuccessfullyTheUser))// "ValidGetAllShouldDisplayAllUsers")
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UsersService(context, config);

                var added = new examen_web_application.Viewmodels.RegisterPostModel

                {
                    Email = "ovi1@aol.com",
                    FirstName = "ovi1",
                    LastName = "ovi1",
                    Password = "123456789",
                    Username = "ovi1",
                };
                usersService.Register(added);

                
                var result = usersService.GetAll();

                
                Assert.IsNotEmpty(result);
                Assert.AreEqual(1, result.Count());

            }
        }

        [Test]
        public void GetByIdShouldReturnAnValidUser()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
         .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnAnValidUser))
         .Options;

            using (var context = new UsersDbContext(options))
            {
                
                var usersService = new UsersService(context, config);
                var added1 = new examen_web_application.Viewmodels.RegisterPostModel
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Username = "test_user1",
                    Email = "test1@yahoo.com",
                    Password = "111111111"
                };

                usersService.Register(added1);
                var userById = usersService.GetById(1);

                Assert.NotNull(userById);
                Assert.AreEqual("firstName", userById.FirstName);

            }
        }

        [Test]
        public void CreateShouldReturnNotNullIfValidUserGetModel()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(CreateShouldReturnNotNullIfValidUserGetModel))
            .Options;

            using (var context = new UsersDbContext(options))
            {
               
                var usersService = new UsersService(context, config);

               

                var added1 = new examen_web_application.Viewmodels.UserPostModel
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Username = "test_user",
                    Email = "test@yahoo.com",
                    Password = "11111111",
                    UserRole = "Regular",
                };

                var userCreated = usersService.Create(added1);

                Assert.IsNotNull(userCreated);
            }
        }
        [Test]
        public void ValidDeleteShouldRemoveTheUser()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(ValidDeleteShouldRemoveTheUser))
            .Options;

            using (var context = new UsersDbContext(options))
            {
                
                var usersService = new UsersService(context, config);
                var added = new examen_web_application.Viewmodels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "firstName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111111111111"
                };

                var userCreated = usersService.Register(added);

                Assert.NotNull(userCreated);

               

                var userDeleted = usersService.Delete(1);

                Assert.IsNotNull(userDeleted);
                Assert.AreEqual(0, usersService.GetAll().Count());

            }
        }
        
    }
}