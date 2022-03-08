using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.DTOs.Response
{
    public class MessageResponseDto
    {
        public UserResponseDto Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
