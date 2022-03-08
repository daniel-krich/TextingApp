using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.DTOs.Request
{
    public class CreateUserDto
    {
        [Required, MinLength(4), MaxLength(24)]
        public string Username { get; set; }

        [Required, MinLength(6), MaxLength(24)]
        public string Password { get; set; }

        [Required, MinLength(10), MaxLength(60)]
        public string Email { get; set; }

        [Required, MinLength(1), MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MinLength(1), MaxLength(30)]
        public string LastName { get; set; }
    }
}
