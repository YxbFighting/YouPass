using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;
using System.Net;
using System.Data;
using System.Text.Json;
using YouPass_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：id string
         * 功能：通过用户的id，获得这个人（老师或学生）的所有课程
         * 返回值：成功返回CourseGetReturn{"status":"Good",courselist:["","",""......]}，
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            string sql;
            if (Tools.IsExistInTeacher(id)) sql = "select course_id,title,password from course where teacher_id=:id";
            else sql = "select course.course_id,title,password from student_course, course where id=:id and student_course.course_id=course.course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];

            List<info_Course> courselist = new List<info_Course>();
            foreach (DataRow item in data.Rows)
            {
                courselist.Add(new info_Course { teacher_id = id, course_id = item["course_id"].ToString(), title = item["title"].ToString(), password = item["password"].ToString() });
            }
            var r = new CourseGetReturn { status = "Good", info_course = courselist };
            return r;
        }

        /******************************************************************
        * 方式：Http Post
        * 参数：userinfomation
        *          title:      课程名字
        *          password:   密码
        * 功能：   发布一门课程
        * 返回值：成功返回Return{"status":"Good","id":"课程的id"}，
        *        否则返回Return{"status":"Bad"}。
        ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject userInfomation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string teacher_id = HttpContext.Session.GetString("id");

            if (userInfomation["title"] == null || userInfomation["password"] == null)
            {
                var r = new Return { status = "Bad" };
                return r;
            }
            var title = userInfomation["title"].ToString();
            var password = userInfomation["password"].ToString();
            var generate_id = Tools.GetMaxID_Course("course_id", "course").ToString();
            var sql = "insert into course (teacher_id,course_id,title,password) values (:teacher_id,:course_id,:title,:password)";
            List<OracleParameter> parameters1 = new List<OracleParameter>();
            parameters1.Add(new OracleParameter(":teacher_id", teacher_id));
            parameters1.Add(new OracleParameter(":course_id", generate_id));
            parameters1.Add(new OracleParameter(":title", title));
            parameters1.Add(new OracleParameter(":password", password));
            var issuccess = DbHelperOra.ExecuteSql(sql, parameters1.ToArray());
            if (issuccess > 0)
            {
                int notice_id = Tools.GetMaxID("notice_id", "notice");
                DateTime current = DateTime.Now;
                List<OracleParameter> noticeadd = new List<OracleParameter>();
                noticeadd.Add(new OracleParameter(":notice_id", notice_id));
                noticeadd.Add(new OracleParameter(":course_id", generate_id));
                noticeadd.Add(new OracleParameter(":title", title + "开课啦"));
                noticeadd.Add(new OracleParameter(":time", current));
                int temp = DbHelperOra.ExecuteSql("insert into notice values(:notice_id,:course_id,:title,:time)", noticeadd.ToArray());
                var r = new CoursePostReturn { status = "Good", generated_Course_Id = generate_id };
                return r;
            }
            else
            {
                var r = new Return { status = "Bad" };
                return r;
            }
        }
        /******************************************************************
        * 方式：Http Get
        * 参数：course_id
        * 功能：  查找某门课程的全部学生信息
        * 返回值：成功返回Return{"status":"Good","stu_id","stu_name"}，
        *        否则返回Return{"status":"Bad"}。
        ******************************************************************/
        [HttpGet("StuGet")]
        public ActionResult<Return> StuGet(string course_id)
        {
            var sql = "select student.id,student.name from student,student_course where student.id=student_course.id and course_id = :course_id ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<stu_infoo_inc> Stu_list = new List<stu_infoo_inc>();
            foreach (DataRow item in data.Rows)
            {
                Stu_list.Add(new stu_infoo_inc
                {
                    stu_id = item["id"].ToString(),
                    stu_name = item["name"].ToString()
                });
            }
            var stu_r = new StuListReturn { status = "Good", student_list = Stu_list };
            return stu_r;
        }

        [HttpGet("GradeGet")]
        public ActionResult<Return> GradeGet(string course_id, int exam_id)
        {
            var sql = "select student_id,student.name,score from student_exam,student where id=student_id and course_id=:course_id and exam_id=:exam_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            parameters.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<exam_stu_inc> Stu_list = new List<exam_stu_inc>();
            foreach (DataRow item in data.Rows)
            {
                int temp = 0;
                if (item["score"].ToString() != "")
                {
                    temp = int.Parse(item["score"].ToString());
                }

                Stu_list.Add(new exam_stu_inc
                {
                    stu_id = item["student_id"].ToString(),
                    stu_name = item["name"].ToString(),
                    stu_score = temp
                });
            }
            var stu_r = new StuExamReturn { status = "Good", student_list = Stu_list };
            return stu_r;
        }

        /******************************************************************
         * 方式：Http Delete
         * 参数：course_id string   teacher_id string
         * 功能：删除用户id为teacher_id，course_id的id为id的课程
         * 返回值：成功返回Return{"status":"Good"}，
         *           否则返回Return{"status":"Bad"}。
         ******************************************************************/
        [HttpDelete]
        public ActionResult<Return> Delete(string course_id)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string teacher_id = HttpContext.Session.GetString("id");
            if (!Tools.IsExistInTeacher(teacher_id))
            {
                return new Return { status = "Bad" };
            }
            OracleParameter delete = new OracleParameter(":course_id", course_id);
            if (!DbHelperOra.Exists("select * from course where course_id=:course_id", delete))
            {
                return new Return { status = "Bad" };
            }
            int temp = 0;
            temp = DbHelperOra.ExecuteSql("delete from student_exam_question where course_id=:course_id", delete);
            temp = DbHelperOra.ExecuteSql("delete from student_exam where course_id=:course_id", delete);
            temp = DbHelperOra.ExecuteSql("delete from exam where course_id=:course_id", delete);
            temp = DbHelperOra.ExecuteSql("delete from notice where course_id=:course_id", delete);
            temp = DbHelperOra.ExecuteSql("delete from student_course where course_id=:course_id", delete);
            var sql = "delete from course where teacher_id=:teacher_id and course_id=:course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":teacher_id", teacher_id));
            parameters.Add(new OracleParameter(":course_id", course_id));
            var isok = DbHelperOra.ExecuteSql(sql, parameters.ToArray());
            if (isok > 0)
            {
                var sql_commit = "commit";
                DbHelperOra.ExecuteSql(sql_commit);
                var r = new Return { status = "Good" };
                return r;
            }
            else
            {
                var r = new Return { status = "Bad" };
                return r;
            }
        }

        /******************************************************************
         * 方式：Http Delete
         * 参数：course_id string   teacher_id string
         * 功能：删除用户id为teacher_id，course_id的id为id的课程
         * 返回值：成功返回Return{"status":"Good"}，
         *           否则返回Return{"status":"Bad"}。
         ******************************************************************/
        [HttpDelete("DeleteStu")]
        public ActionResult<Return> DeleteStu(string student_id, string course_id)
        {
            // if (HttpContext.Session.GetString("id") == null)
            // {
            //     return new Return { status = "Bad" };
            // }
            // string teacher_id = HttpContext.Session.GetString("id");
            var sql = "delete from student_course where id=:student_id and course_id=:course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":student_id", student_id));
            parameters.Add(new OracleParameter(":course_id", course_id));
            if (!DbHelperOra.Exists("select * from student_course where id=:student_id and course_id=:course_id"))
            {
                var r = new Return { status = "Bad" };
                return r;
            }
            int temp = 0;
            temp = DbHelperOra.ExecuteSql("delete from student_exam_question where student_id=:student_id and course_id=:course_id", parameters.ToArray());
            temp = DbHelperOra.ExecuteSql("delete from student_exam where student_id=:student_id and course_id=:course_id", parameters.ToArray());
            var isok = DbHelperOra.ExecuteSql(sql, parameters.ToArray());
            if (isok > 0)
            {
                var sql_commit = "commit";
                DbHelperOra.ExecuteSql(sql_commit);
                var r = new Return { status = "Good" };
                return r;
            }
            else
            {
                var r = new Return { status = "Bad" };
                return r;
            }
        }
        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}