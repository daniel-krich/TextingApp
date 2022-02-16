using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.Enums;

namespace TextAppData.ResponseModels
{
    public class ChatResponseModel
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public IList<UserResponseModel> Participants { get; set; }
        public MessageResponseModel LastMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
