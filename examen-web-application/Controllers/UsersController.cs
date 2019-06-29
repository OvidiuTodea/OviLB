using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using examen_web_application.Models;
using examen_web_application.Services;
using examen_web_application.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace examen_web_application.Controllers
{
   // https://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
        //[Route("api/[controller]/[action]")]
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private IUsersService _userService;

            public UsersController(IUsersService userService)
            {
                _userService = userService;
            }

            /// <summary>
            /// Login for user 
            /// </summary>
            /// <remarks>
            ///            {
            ///            "username":"ilie91",
            ///            "password":"123456"
            ///            }
            /// 
            /// 
            /// </remarks>
            /// <param name="login">Enter username and password</param>
            /// <returns>Return username , email and token</returns>
            [AllowAnonymous]
            [HttpPost("authenticate")]
            public IActionResult Authenticate([FromBody]LoginPostModel login)
            {
                var user = _userService.Authenticate(login.Username, login.Password);

                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(user);
            }

            /// <summary>
            /// Register a user in the database
            /// </summary>
            /// <remarks>
            ///     {
            ///         "firstName":"Carmen",
            ///         "lastName":"Barbu",
            ///         "username":"bcarmen",
            ///         "email":"cb@yahoo.com",
            ///         "password":"123456789"
            ///        }
            /// </remarks>
            /// <param name="register">Introduce firstname, lastname,username,email and password</param>
            /// <returns>Inserted user in database</returns>
            [AllowAnonymous]
            [HttpPost("register")]
            //[HttpPost]
            public IActionResult Register([FromBody]RegisterPostModel register)
            {
                var user = _userService.Register(register);
                if (user == null)
                {
                    return BadRequest(new { ErrorMessage = "Username already exists." });
                }
                return Ok(user);
            }

            [AllowAnonymous]
            //[Authorize(Roles = "Moderator, Admin")]
            [HttpGet]
            public IActionResult GetAll()
            {
                var users = _userService.GetAll();
                return Ok(users);
            }

            /// <summary>
            /// Find an user by the given id.
            /// </summary>
            /// <remarks>
            /// Sample response:
            ///
            ///     Get /users
            ///     {  
            ///        id: 3,
            ///        firstName = "Pop",
            ///        lastName = "Andrei",
            ///        userName = "user123",
            ///        email = "Us1@yahoo.com",
            ///        userRole = "Regular"
            ///     }
            /// </remarks>
            /// <param name="id">The id given as parameter</param>
            /// <returns>The user with the given id</returns>
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            // GET: api/Users/5
            //[Authorize(Roles = "Admin, Moderator, Regular")]
            [HttpGet("{id}", Name = "GetUser")]
            public IActionResult Get(int id)
            {
                var found = _userService.GetById(id);
                if (found == null)
                {
                    return NotFound();
                }
                return Ok(found);
            }


            /// <summary>
            /// Add an new User
            /// </summary>
            ///   /// <remarks>
            /// Sample response:
            ///
            ///     Post /users
            ///     {
            ///        firstName = "Pop",
            ///        lastName = "Andrei",
            ///        userName = "user123",
            ///        email = "Us1@yahoo.com",
            ///        password = "123456",
            ///        userRole = "regular"
            ///     }
            /// </remarks>
            /// <param name="userPostModel">The input user to be added</param>
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            //[Authorize(Roles = "Admin, Moderator")]
            [HttpPost]
            public void Post([FromBody] UserPostModel userPostModel)
            {
                _userService.Create(userPostModel);
            }



            /// <summary>
            /// Modify an user if exists in dbSet , or add if not exist
            /// </summary>
            /// <param name="id">id-ul user to update</param>
            /// <param name="userPostModel">obiect userPostModel to update</param>
            /// Sample request:
            ///     <remarks>
            ///     Put /users/id
            ///     {
            ///        firstName = "Pop",
            ///        lastName = "Andrei",
            ///        userName = "user123",
            ///        email = "Us1@yahoo.com",
            ///        userRole = "regular"
            ///     }
            /// </remarks>
            /// <returns>Status 200 daca a fost modificat</returns>
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            //[Authorize(Roles = "Admin,Moderator")]
            [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserPostModel userPostModel)
        {
            User addedBy = _userService.GetCurrentUser(HttpContext);
            var result = _userService.Upsert(id, userPostModel, addedBy);
            if (result == null)
            {
                return Forbid("You don't have rigts to perform this action!");
            }
            return Ok(result);
        }



        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User id to delete</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            //[Authorize(Roles = "Admin,Moderator")]
            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                //User addedBy = _userService.GetCurrentUser(HttpContext);
                //var result = _userService.Delete(id, addedBy);
                //if (result == null)
                //{
                //    return Forbid("You don't have rigts to perform this action!");
                //}
                //return Ok(result);

                User currentLogedUser = _userService.GetCurrentUser(HttpContext);
                var regDate = currentLogedUser.CreatedAt;
                var currentDate = DateTime.Now;
                var minDate = currentDate.Subtract(regDate).Days / (365 / 12);

                if (currentLogedUser.UserRole == UserRole.Moderator)
                {
                    User getUser = _userService.GetById(id);
                    if (getUser.UserRole == UserRole.Admin)
                    {
                        return Forbid();
                    }

                }

                if (currentLogedUser.UserRole == UserRole.Moderator)
                {
                    User getUser = _userService.GetById(id);
                    if (getUser.UserRole == UserRole.Moderator && minDate <= 6)

                        return Forbid();
                }
                if (currentLogedUser.UserRole == UserRole.Moderator)
                {
                    User getUser = _userService.GetById(id);
                    if (getUser.UserRole == UserRole.Moderator && minDate >= 6)
                    {
                        var result1 = _userService.Delete(id);
                        return Ok(result1);
                    }


                }

                var result = _userService.Delete(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }

        }
    }
