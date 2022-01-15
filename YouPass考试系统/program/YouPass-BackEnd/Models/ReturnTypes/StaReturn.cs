using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouPass_BackEnd.Models
{
    public class InCorrect_Info //统计人数以及题目编号
    {
        public string question_id { get; init; }
        public int InCorrectNum { get; init; }
        public int type { get; init; }
        public string description { get; init; }

    }
    public class InCorrectReturn : Return
    {
        public List<InCorrect_Info> incorrect_list { get; init; }
    }

}
