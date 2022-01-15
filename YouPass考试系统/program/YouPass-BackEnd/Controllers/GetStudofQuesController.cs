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
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetStudofQuesController : ControllerBase
    {
        /***********************************************************************
         * 参数：ques_info (见takeexamreturn)
         * 功能：根据考生信息(考生id，考试id，课程id)获取本门考试题目信息
         * 要传入的信息：考生id，考试id，课程id
         * 返回值：成功返回TakeexamGetReturn{"status":"Good",question_list:[]}，
         *           否则返回TakeexamGetReturn{"status":"Bad"}。    
         ***********************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject ques_info)
        {
            #region tell
            string course_id = ques_info["course_id"].ToString();
            string exam_id = ques_info["exam_id"].ToString();
            string question_id = ques_info["question_id"].ToString();
            //获取做这道题,并且这道题没有被批改的人
            List<stud_info> stud_list = new List<stud_info>();
            int ques_value = 0;
            var sql_stud = "select student_id,student_answer,numinpaper,value from student_exam_question where question_id=:question_id and course_id=:course_id and exam_id=:exam_id and student_point is null";
            List<OracleParameter> parameters1 = new List<OracleParameter>();
            parameters1.Add(new OracleParameter(":question_id", question_id));
            parameters1.Add(new OracleParameter(":course_id", course_id));
            parameters1.Add(new OracleParameter(":exam_id", exam_id));
            var data_exam = DbHelperOra.Query(sql_stud, parameters1.ToArray()).Tables[0];
            foreach (DataRow item in data_exam.Rows)
            {
                ques_value = int.Parse(item["value"].ToString());
                stud_list.Add(new stud_info
                {
                    stud_id = item["student_id"].ToString(),
                    numinpaper = int.Parse(item["numinpaper"].ToString()),
                    student_answer = item["student_answer"].ToString()
                });
            }
            //获取这道题目的相关信息
            var sql = "select description,type,standard_answer,subject from question where question_id=:question_id ";

            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":question_id", question_id));
            ques_info questions = new ques_info();
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];

            foreach (DataRow item in data.Rows)
            {
                List<Option_info> opt_list = new List<Option_info>();
                string description = item["description"].ToString();
                int type = int.Parse(item["type"].ToString());
                string subject = item["subject"].ToString();
                string standard_answer = item["standard_answer"].ToString();

                var sql_opt = "select option_id,content from question_option where question_id=:question_id order by option_id";
                List<OracleParameter> parameters_opt = new List<OracleParameter>();
                parameters_opt.Add(new OracleParameter("question_id", question_id));
                var options = DbHelperOra.Query(sql_opt, parameters_opt.ToArray()).Tables[0];
                foreach (DataRow opt in options.Rows)
                {
                    opt_list.Add(new Option_info
                    {
                        question_id = question_id,
                        option_id = int.Parse(opt["option_id"].ToString()),
                        content = opt["content"].ToString(),
                    });
                }

                questions = (new ques_info
                {
                    ques_id = question_id,
                    description = description,
                    type = type,
                    standard_answer = standard_answer,
                    subject = subject,
                    options = opt_list,
                    ques_value = ques_value
                });

            }
            var person_authority = new GetStudofQuesReturn { status = "Good", student_list = stud_list, question_info = questions };
            return person_authority;
            #endregion
        }


    }
}