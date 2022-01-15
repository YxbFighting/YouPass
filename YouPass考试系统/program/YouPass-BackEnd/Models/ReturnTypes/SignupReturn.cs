using System;

namespace YouPass_BackEnd.Models
{
    public class SignupReturn:Return
    {
        public String generatedId { get; init; }
        public String tag { get; init; }
    }
}