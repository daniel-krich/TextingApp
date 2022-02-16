using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.Models
{
    public class PushMessageModel : TokenLoginModel
    {
        public string ChatId { get; set; }
        public ChatType TypeChat { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
