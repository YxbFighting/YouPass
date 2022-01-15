using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Models
{
    public class StudentScore
    {
        public string ID { get; init; }
        public int Score { get; init; }
    }
}
