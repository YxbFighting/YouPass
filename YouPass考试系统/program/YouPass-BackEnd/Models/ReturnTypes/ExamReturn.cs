using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{

    public class info_exam
    {
        public String course_id { get; init; }
        public String exam_id { get; init; }
        public String title { get; init; }
        public String start_time { get; init; }
        public String end_time { get; init; }
        public String exam_type { get; init; }

    }

    public class Score_list
    {
        public string course_id { get; init; }
        public int exam_id { get; init; }
        public string title { get; init; }
        public int score { get; init; }
    }
    public class ExaminfoReturn : Return
    {
        public List<info_exam> info_Exam { get; init; }
        //else
    }


    public class ScoreReturn : Return
    {
        public List<Score_list> exam_list { get; init; }
    }
}