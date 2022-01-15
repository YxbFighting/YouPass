using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{

    public class stud_info
    {
        //题目信息，可根据前端需求增添，目前为id与题干，注意随之更改前构造函数
        public string stud_id { get; init; }
        public int numinpaper { get; init; }
        public string student_answer { get; init; }

    }
    public class ques_info
    {
        //题目信息，可根据前端需求增添，目前为id与题干，注意随之更改前构造函数


        public string ques_id { get; init; }
        public string description { get; init; }
        public string subject { get; init; }
        public string standard_answer { get; init; }
        public int ques_value { get; init; }
        public int type { get; init; }
        public List<Option_info> options { get; init; }

    }
    public class GetStudofQuesReturn : Return
    {
        public List<stud_info> student_list { get; init; }
        public ques_info question_info { get; init; }



        //else
    }
}