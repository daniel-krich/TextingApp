using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.ResponseModels
{
    public class ChatResponseModel
    {
        public IList<UserResponseModel> Participants { get; set; }
        public MessageResponseModel LastMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
