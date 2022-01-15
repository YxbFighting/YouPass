using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{
    public class AuthorityReturn : Return
    {
        public List<String> authorities { get; init; }
    }
}