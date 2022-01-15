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
using YouPass_BackEnd.Models.ReturnTypes;

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：id string
         * 功能：通过用户的id，获得这个用户的考试信息
         * 返回值：成功返回ExamReturn{"status":"Good",course_id:"",exam_id:"",start_time:"",end_time:"",title""}，
         *         否则返回ExamReturn{"status":"Bad"}。
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            if (!Tools.IsExist(id))
                return new Return { status = "Bad" };

            var sql = "select exam.course_id,exam.exam_id,title,start_time,end_time from STUDENT_EXAM,EXAM where student_id=:id and student_exam.course_id = exam.course_id and student_exam.exam_id = exam.exam_id order by start_time";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<info_exam> infoExam = new List<info_exam>();
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            long timeStamp_end = 0;
            long timeStamp_now = 0;
            bool isFinish = true;



            foreach (DataRow item in data.Rows)
            {
                DateTime startTime = Convert.ToDateTime(item["end_time"].ToString());
                timeStamp_end = Convert.ToInt64((startTime - dateStart).TotalSeconds);

                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0);
                timeStamp_now = int.Parse(Convert.ToInt64(ts.TotalSeconds).ToString());
                if (timeStamp_now < timeStamp_end)
                {
                    isFinish = false;
                }


                if (!isFinish)
                {
                    infoExam.Add(new info_exam
                    {

                        course_id = item["course_id"].ToString(),
                        exam_id = item["exam_id"].ToString(),
                        title = item["title"].ToString(),
                        start_time = Convert.ToDateTime(item["start_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        end_time = Convert.ToDateTime(item["end_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }


            }
            var r = new ExaminfoReturn { status = "Good", info_Exam = infoExam };
            return r;
        }
        /******************************************************************
        * 方式：Http Get
        * 参数：id string
        * 功能：通过course_id获得所有考试
        ******************************************************************/
        [HttpGet("GetbyCourseId")]
        public ActionResult<Return> GetbyCourseId(string course_id)
        {
            var sql = "select exam.exam_id,title,start_time,end_time from EXAM where course_id = :course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<info_exam> infoExam = new List<info_exam>();

            foreach (DataRow item in data.Rows)
            {
                infoExam.Add(new info_exam
                {

                    course_id = course_id,
                    exam_id = item["exam_id"].ToString(),
                    title = item["title"].ToString(),
                    start_time = Convert.ToDateTime(item["start_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    end_time = Convert.ToDateTime(item["end_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                });

            }
            var r = new ExaminfoReturn { status = "Good", info_Exam = infoExam };
            return r;
        }
        /******************************************************************
        * 方式：Http Get
        * 参数：id string
        * 功能：通过course id和exam id获得所有考试题目
        ******************************************************************/
        [HttpGet("GetQuestions")]
        public ActionResult<Return> GetQuestions(string course_id, string exam_id)
        {
            var sql = "select question.question_id,description,type,Count (distinct student_id) InCorrectNum from student_exam_question,question where student_exam_question.question_id=question.question_id and student_point is null and student_exam_question.course_id=:course_id and exam_id=:exam_id group by question.question_id,description,type";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            parameters.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<InCorrect_Info> incorrect_list = new List<InCorrect_Info>();
            foreach (DataRow item in data.Rows)
            {
                incorrect_list.Add(new InCorrect_Info
                {
                    question_id = item["question_id"].ToString(),
                    description = item["description"].ToString(),
                    type = int.Parse(item["type"].ToString()),
                    InCorrectNum = int.Parse(item["InCorrectNum"].ToString())
                });
            }
            var r = new InCorrectReturn { status = "Good", incorrect_list = incorrect_list };
            return r;

        }
        [HttpGet("GetStuScore")]

        public ActionResult<Return> GetStuScore(string course_id)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string student_id = HttpContext.Session.GetString("id");

            var SQL = "SELECT COUNT(course_id) FROM exam WHERE course_id =:course_id";
            List<OracleParameter> parameters1 = new List<OracleParameter>();
            parameters1.Add(new OracleParameter(":course_id", course_id));
            var check = DbHelperOra.ExecuteSql(SQL, parameters1.ToArray());
            if (check == 0)
            {
                var re = new Return { status = "Bad" };
                return re;
            }
            // var sql = "select exam_id from student_exam";
            var sql = "select exam.exam_id,score,title from student_exam,exam where student_exam.course_id=exam.course_id and student_exam.exam_id=exam.exam_id and student_id=:student_id and exam.course_id=:course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":student_id", student_id));
            parameters.Add(new OracleParameter(":course_id", course_id));

            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<Score_list> ret_list = new List<Score_list>();
            foreach (DataRow item in data.Rows)
            {
                var temp = -1;
                if (item["score"].ToString() != "")
                    temp = int.Parse(item["score"].ToString());
                ret_list.Add(new Score_list
                {
                    course_id = course_id,
                    exam_id = int.Parse(item["exam_id"].ToString()),
                    title = item["title"].ToString(),
                    score = temp
                });
            }
            var r = new ScoreReturn { status = "Good", exam_list = ret_list };
            return r;
        }

        [HttpPost("CalStuScore")]
        public ActionResult<Return> CalStuScore([FromBody] JObject stu_info)
        {
            var course_id = stu_info["course_id"].ToString();
            var exam_id = int.Parse(stu_info["exam_id"].ToString());
            var sql = "select student_id from student_exam where course_id =:course_id and exam_id =:exam_id";
            List<OracleParameter> p1 = new List<OracleParameter>();
            p1.Add(new OracleParameter(":course_id", course_id));
            p1.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql, p1.ToArray()).Tables[0];
            var re = true;
            foreach (DataRow item in data.Rows)
            {
                re = Tools.Student_Exam_Score_sum(item["student_id"].ToString(), course_id, exam_id);
            }
            if (re == true)
            {
                var r = new Return { status = "Good" };
                return r;
            }
            else
            {
                var r = new Return { status = "Bad" };
                return r;
            }
        }
    }
}