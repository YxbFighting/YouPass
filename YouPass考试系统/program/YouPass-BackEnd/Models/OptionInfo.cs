using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Models
{
    public class Option_info
    {
        public int option_id { get; set; }
        public String question_id { get; set; }
        public String content { get; set; }
    }
    public class Option_info_specifial
    {
        public int id { get; set; }
        public String question_id { get; set; }
        public String content { get; set; }
    }
}