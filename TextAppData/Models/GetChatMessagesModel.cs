using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.Models
{
    public class GetChatMessagesModel : TokenLoginModel
    {
        [Required]
        public string ChatId { get; set; }
        [Required]
        public ChatType TypeChat { get; set; }
        [Required]
        public int MessageOffset { get; set; }
    }
}
