using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{
    public class Test
    {
        public int score_sum { get; init; }
    }
    public class TestReturn : Return
    {
        // public IFormFile files { get; set; }
        public List<Test> T { get; init; }
    }

   
}