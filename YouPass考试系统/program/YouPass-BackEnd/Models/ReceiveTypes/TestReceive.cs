using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Models
{
    public class TestReceive
    {
        public IFormFile files { get; set; }
        public String id { get; set; }
    }
}