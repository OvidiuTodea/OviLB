using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace examen_web_application.Viewmodels
{
    public class RegisterPostModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(55, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
