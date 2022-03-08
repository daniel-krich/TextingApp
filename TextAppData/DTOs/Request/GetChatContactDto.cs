using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.DTOs.Request
{
    public class GetChatContactDto
    {
        [Required]
        public string ChatId { get; set; }
    }
}
