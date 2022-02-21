using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.Models
{
    public class SearchUsersModel : TokenLoginModel
    {
        [Required]
        public string Query { get; set; }
    }
}
