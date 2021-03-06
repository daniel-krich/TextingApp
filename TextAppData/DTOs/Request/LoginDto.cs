using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.DTOs.Request
{
    public class LoginDto
    {
        [Required, MinLength(4), MaxLength(24)]
        public string Username { get; set; }

        [Required, MinLength(6), MaxLength(24)]
        public string Password { get; set; }
    }
}
