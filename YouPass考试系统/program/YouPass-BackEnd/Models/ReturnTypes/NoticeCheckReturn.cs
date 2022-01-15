using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouPass_BackEnd.Models
{
    public class NoticeCheckReturn : Return
    {
        public List<Notice> Noticelist { get; init; }
    }
    public class Notice
    {
        public int notice_id { get; init; }
        public string course_id { get; init; }
        public string content { get; init; }
        public string date { get; init; }
        public string title { get; set; }

    }
}
