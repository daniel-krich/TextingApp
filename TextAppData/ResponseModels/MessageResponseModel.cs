using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.ResponseModels
{
    public class MessageResponseModel
    {
        public UserResponseModel Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
