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
                    Email = "petre@aol.com",
                    FirstName = "Petre",
                    LastName = "Popescu",
                    Password = "12345678",
                    Username = "ppetre",
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
                    Email = "petre@aol.com",
                    FirstName = "Petre",
                    LastName = "Popica",
                    Password = "12345678",
                    Username = "ppetre",
                };
                usersService.Register(added);
                var loggedIn = new examen_web_application.Viewmodels.LoginPostModel
                {
                    Username = "ppetre",
                    Password = "12345678"

                };
                var authoresult = usersService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(authoresult);
                Assert.AreEqual(1, authoresult.Id);
                Assert.AreEqual(loggedIn.Username, authoresult.Username);
                //Assert.AreEqual(loggedIn.Password, UsersService.);
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
                    Email = "petre@aol.com",
                    FirstName = "Petre",
                    LastName = "Popescu",
                    Password = "12345678",
                    Username = "ppetre",
                };
                usersService.Register(added);

                // Act
                var result = usersService.GetAll();

                // Assert
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
                //var regValidator = new RegisterValidator();
                //var crValidator = new CreateValidator();
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
                //var regValidator = new RegisterValidator();
                //var crValidator = new CreateValidator();
                var usersService = new UsersService(context, config);

                //UserRole addUserRoleRegular = new UserRole
                //{
                //    Name = "Regular",
                //    Description = "Created for test"
                //};
                //context.UserRoles.Add(addUserRoleRegular);
                //context.SaveChanges();

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
                //var validator = new RegisterValidator();
                //var crValidator = new CreateValidator();
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

                //Assert.AreEqual(0, usersService.GetAll().Count());

                var userDeleted = usersService.Delete(1);

                Assert.IsNotNull(userDeleted);
                Assert.AreEqual(0, usersService.GetAll().Count());

            }
        }
        //[Test]
        //public void ValidUpsertShouldModifyFieldsValues()
        //{
        //    var options = new DbContextOptionsBuilder<UsersDbContext>()
        //    .UseInMemoryDatabase(databaseName: nameof(ValidUpsertShouldModifyFieldsValues))
        //    .Options;

        //    using (var context = new UsersDbContext(options))
        //    {
        //        //var validator = new RegisterValidator();
        //        //var crValidator = new CreateValidator();
        //        var usersService = new UsersService(context,  config);
        //        var added = new examen_web_application.Viewmodels.UserPostModel
        //        {
        //            FirstName = "Nume",
        //            LastName = "Prenume",
        //            Username = "userName",
        //            Email = "user@yahoo.com",
        //            Password = "333333"
        //        };

        //        usersService.Create(added);

        //        var updated = new examen_web_application.Viewmodels.UserPostModel
        //        {
        //            FirstName = "Alin",
        //            LastName = "Popescu",
        //            Username = "popAlin",
        //            Email = "pop@yahoo.com",
        //            Password = "333333"
        //        };
        //        var addedBy = "Admin";
        //        var userUpdated = usersService.Upsert(1, updated, addedBy);

        //        Assert.NotNull(userUpdated);
        //        Assert.AreEqual("Alin", userUpdated.FirstName);
        //        Assert.AreEqual("Popescu", userUpdated.LastName);

        //    }
        //}
    }
}