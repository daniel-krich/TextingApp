using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.DTOs.Response
{
    public class ChatMessagesResponseDto
    {
        public string ChatId { get; set; }
        public ChatType Type { get; set; }
        public IList<MessageResponseDto> Messages { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
