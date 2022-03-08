using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.DTOs.Response
{
    public class ResponseDto
    {
        public int? ErrorId { get; set; }
        public string Header { get; set; }
        public string Comment { get; set; }

        public ResponseDto(int errorId, string error_header, string comment)
        {
            ErrorId = errorId;
            Header = error_header;
            Comment = comment;
        }


        public ResponseDto(string success_header, string comment)
        {
            Header = success_header;
            Comment = comment;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
