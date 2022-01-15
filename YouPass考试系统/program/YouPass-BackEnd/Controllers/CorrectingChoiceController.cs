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
using Oracle.ManagedDataAccess.Client;
using YouPass_BackEnd.Models.ReturnTypes;
using Microsoft.AspNetCore.Http;
namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorrectingChoiceController : ControllerBase
    {
        /******************************************************************
        * 方式：Http Post
        * 参数：questioninfo
        *          course_id      课程编号
        *          exam_id        考试编号
        *          question_id    问题编号
        * 功能：   自动批改
        * 返回值：成功返回Return{"status":"Good"}，
        *        否则返回Return{"status":"Bad"}。
        ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject questioninfo)
        {
            string courseid = questioninfo["course_id"].ToString();
            int examid = int.Parse(questioninfo["exam_id"].ToString());
            string questionid = questioninfo["question_id"].ToString();
            OracleParameter findanswer = new OracleParameter(":questionid", questionid);
            var sign = DbHelperOra.Exists("select count(*) from question where question_id=:questionid", findanswer);
            if (!sign)
            {
                Return r = new Return { status = "Bad" };
                return r;
            }
            else
            {
                string standardanswer = DbHelperOra.GetSingle("select standard_answer from question where question_id=:questionid", findanswer).ToString();
                List<OracleParameter> paper = new List<OracleParameter>();
                paper.Add(new OracleParameter(":courseid", courseid));
                paper.Add(new OracleParameter(":examid", examid));
                paper.Add(new OracleParameter(":questionid", questionid));
                if (DbHelperOra.Exists("select count(*) from student_exam_question where course_id=:courseid and exam_id=:examid and question_id=:questionid", paper.ToArray()))
                {
                    var data = DbHelperOra.Query("select * from student_exam_question where course_id=:courseid and exam_id=:examid and question_id=:questionid", paper.ToArray()).Tables[0];
                    foreach (DataRow item in data.Rows)
                    {
                        string stunum = item["student_id"].ToString();
                        string stuanswer = item["student_answer"].ToString();
                        int score = new int();
                        if (stuanswer == standardanswer) { score = int.Parse(item["value"].ToString()); }
                        else score = 0;
                        List<OracleParameter> update = new List<OracleParameter>();
                        update.Add(new OracleParameter(":score", score));
                        update.Add(new OracleParameter(":student_id", stunum));
                        update.Add(new OracleParameter(":courseid", courseid));
                        update.Add(new OracleParameter(":examid", examid));
                        update.Add(new OracleParameter(":questionid", questionid));
                        var change = DbHelperOra.ExecuteSql("update student_exam_question set student_point=:score where student_id=:student_id and course_id=:courseid and exam_id=:examid and question_id=:questionid", update.ToArray());

                    }
                    Return r = new Return { status = "Good" };
                    return r;
                }
                else
                {
                    Return r = new Return { status = "Bad" };
                    return r;
                }
            }
        }
        /******************************************************************
        * 方式：Http Post
        * 参数：questioninfo
        *          course_id      课程编号
        *          exam_id        考试编号
        *          question_id    问题编号
        *          student_score{ID,score}每个学生的成绩
        * 功能：   手动批改
        * 返回值：成功返回Return{"status":"Good"}，
        *        否则返回Return{"status":"Bad"}。
        ******************************************************************/
        [HttpPost("ManualCorrectingPost")]
        public ActionResult<Return> ManualCorrectingPost([FromBody] JObject questioninfo)
        {
            //questionid examid courseid studentscore{score id}
            string courseid = questioninfo["course_id"].ToString();
            int examid = int.Parse(questioninfo["exam_id"].ToString());
            string questionid = questioninfo["question_id"].ToString();
            List<StudentScore> SS = new List<StudentScore>();
            foreach (var sub_item in questioninfo["student_score"])
            {
                SS.Add(new StudentScore { ID = sub_item["ID"].ToString(), Score = int.Parse(sub_item["score"].ToString()) });
            }
            foreach (var temp in SS)
            {
                List<OracleParameter> find = new List<OracleParameter>();
                find.Add(new OracleParameter(":ID", temp.ID));
                find.Add(new OracleParameter(":courseid", courseid));
                find.Add(new OracleParameter(":examid", examid));
                find.Add(new OracleParameter(":questionid", questionid));
                var ex = DbHelperOra.Exists("select count(*) from student_exam_question where student_id=:ID and course_id=:courseid and exam_id=:examid and question_id=:questionid", find.ToArray());
                if (!ex)
                {
                    Return r = new Return { status = "Bad" };
                    return r;
                }
            }
            foreach (var temp in SS)
            {
                List<OracleParameter> change = new List<OracleParameter>();
                change.Add(new OracleParameter(":studentpoint", temp.Score));
                change.Add(new OracleParameter(":ID", temp.ID));
                change.Add(new OracleParameter(":courseid", courseid));
                change.Add(new OracleParameter(":examid", examid));
                change.Add(new OracleParameter(":questionid", questionid));
                var ex = DbHelperOra.ExecuteSql("update student_exam_question set student_point=:studentpoint where student_id=:ID and course_id=:courseid and exam_id=:examid and question_id=:questionid", change.ToArray());
            }
            Return re = new Return { status = "Good" };
            return re;
        }

    }
}
