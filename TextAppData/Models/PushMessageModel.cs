using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.Models
{
    public class PushMessageModel : TokenLoginModel
    {
        public string ChatId { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
