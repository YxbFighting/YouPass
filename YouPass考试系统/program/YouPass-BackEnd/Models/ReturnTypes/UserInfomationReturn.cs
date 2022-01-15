using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{
    public class UserInfomationReturn : Return
    {
        public String name { get; init; }
        public String id { get; init; }
        public String email { get; init; }
        public String location { get; init; }
        //else
    }
    public class IdentityReturn : Return
    {
        public int identity { get; init; }
    }
}