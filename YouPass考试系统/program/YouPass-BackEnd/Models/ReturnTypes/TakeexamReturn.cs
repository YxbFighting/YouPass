using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouPass_BackEnd.Models;
namespace YouPass_BackEnd.Models
{
    public class q_info
    {
        //题目信息，可根据前端需求增添，目前为id与题干，注意随之更改前构造函数
        public string id { get; init; }
        public string description { get; init; }
        public string standard_answer { get; init; }
        public int order { get; init; }
        public int type { get; init; }
        public int numinpaper { get; init; }
        public List<Option_info> options { get; init; }
        public string fill_content { get; init; }
        public List<int> s_picked { get; init; }
        public List<int> m_picked { get; init; }
        public bool done { get; init; }

    }
    public class code_info
    {
        public string id { get; init; }
        public int order { get; init; }
        public int type { get; init; }
        public List<Option_info> options { get; init; }
    }

    public class input_info
    {
        public string course_id { get; init; }
        public string exam_id { get; init; }
    }


    public class TakeexamGetReturn : Return
    {
        public List<q_info> question_list { get; init; }
        public String title { get; init; }
        public String start_time { get; init; }
        public String end_time { get; init; }
    }
    public class TakeexamPostReturn : Return
    {
        public string result { get; init; }
    }
    public class codeReturn : Return
    {
        public string result { get; init; }
        public string code { get; init; }
    }


    public class RandOrderReturn : Return
    {
        public List<Option_info> choice_list { get; init; }
    }
    public class RandOrderIndexReturn : Return
    {
        public List<int> ans_index { get; init; }
        public string answer_code { get; init; }
    }
    public class UpdateReturn : Return
    {
        public List<int> rand_index { get; init; }
    }
}
