using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{

    public class info_Course
    {
        public String teacher_id { get; init; }
        public String course_id { get; init; }
        public String title { get; init; }
        public String password { get; init; }
    }
    public class stu_infoo_inc
    {
        public String stu_id { get; init; }
        public String stu_name { get; init; }
    }
    public class exam_stu_inc
    {
        public String stu_id { get; init; }
        public String stu_name { get; init; }
        public int stu_score { get; init; }
    }
    public class CourseGetReturn : Return
    {
        public List<info_Course> info_course { get; init; }
        //else
    }
    public class CoursePostReturn : Return
    {
        public string generated_Course_Id { get; init; }
        //else
    }
    public class StuListReturn : Return
    {
        public List<stu_infoo_inc> student_list { get; init; }
        //else
    }
    public class StuExamReturn : Return
    {
        public List<exam_stu_inc> student_list { get; init; }
        //else
    }
    
}