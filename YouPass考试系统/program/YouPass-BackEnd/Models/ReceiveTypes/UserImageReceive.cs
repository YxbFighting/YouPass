using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Models
{
    public class UserImageReceive
    {
        public IFormFile files { get; set; }
    }
}