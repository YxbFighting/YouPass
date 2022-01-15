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
    public class TakeexamController : ControllerBase
    {

        [HttpPost]
        public ActionResult<Return> SetSession([FromBody] JObject stu_info)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            var student_id = HttpContext.Session.GetString("id");
            //检测当前时间与考试开始时间   未开始的考试不允许进入
            var course_id = stu_info["course_id"].ToString();
            var exam_id = stu_info["exam_id"].ToString();
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            long timeStamp_start = 0;
            long timeStamp_now = 0;
            bool isStart = false;
            var sql = "select start_time from exam where course_id=:course_id and exam_id=:exam_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            parameters.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            foreach (DataRow item in data.Rows)
            {
                DateTime startTime = Convert.ToDateTime(item["start_time"].ToString());
                timeStamp_start = Convert.ToInt64((startTime - dateStart).TotalSeconds);

                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0);
                timeStamp_now = int.Parse(Convert.ToInt64(ts.TotalSeconds).ToString());
                if (timeStamp_now > timeStamp_start)
                {
                    isStart = true;
                }
            }
            if (isStart)
            {
                var sql1 = "select state from student_exam where student_id=:student_id and course_id=:course_id and exam_id=:exam_id";
                List<OracleParameter> parameters2 = new List<OracleParameter>();
                parameters2.Add(new OracleParameter(":student_id", student_id));
                parameters2.Add(new OracleParameter(":course_id", course_id));
                parameters2.Add(new OracleParameter(":exam_id", exam_id));
                var ret = DbHelperOra.Query(sql1, parameters2.ToArray()).Tables[0];
                var temp_state = 0;
                foreach (DataRow item in ret.Rows)
                {
                    temp_state = int.Parse(item["state"].ToString());
                }
                if (temp_state == 0 || temp_state == 1)
                {
                    List<OracleParameter> updatestate = new List<OracleParameter>();
                    updatestate.Add(new OracleParameter(":student_id", student_id));
                    updatestate.Add(new OracleParameter(":course_id", course_id));
                    updatestate.Add(new OracleParameter(":exam_id", exam_id));
                    var temp = DbHelperOra.ExecuteSql("update student_exam set state=1 where student_id=:student_id and course_id=:course_id and exam_id=:exam_id", updatestate.ToArray());
                    HttpContext.Session.SetString("course_id", course_id);
                    HttpContext.Session.SetString("exam_id", exam_id);
                    return new Return { status = "Good" };
                }
                else
                {
                    return new Return { status = "Bad" };
                }
            }
            else
            {
                return new Return { status = "Bad" };
            }

        }
        /***********************************************************************
         * 参数：stu_info (见takeexamreturn)
         * 功能：根据考生信息(考生id，考试id，课程id)获取本门考试题目信息
         * 要传入的信息：考生id，考试id，课程id
         * 返回值：成功返回TakeexamGetReturn{"status":"Good",question_list:[]}，
         *           否则返回TakeexamGetReturn{"status":"Bad"}。    
         ***********************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            #region tell
            if (HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("course_id") == null || HttpContext.Session.GetString("exam_id") == null)
            {
                return new Return { status = "Bad" };
            }
            string student_id = HttpContext.Session.GetString("id");
            string course_id = HttpContext.Session.GetString("course_id");
            int exam_id = int.Parse(HttpContext.Session.GetString("exam_id"));
            var sql_exam = "select title,start_time,end_time from exam where exam_id=:exam_id";
            List<OracleParameter> parameters1 = new List<OracleParameter>();
            parameters1.Add(new OracleParameter(":exam_id", exam_id));
            string title = "";
            string start_time = "";
            string end_time = "";
            int numinpaper = 0;
            var data_exam = DbHelperOra.Query(sql_exam, parameters1.ToArray()).Tables[0];
            foreach (DataRow item in data_exam.Rows)
            {
                title = item["title"].ToString();
                start_time = Convert.ToDateTime(item["start_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                end_time = Convert.ToDateTime(item["end_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }
            parameters1.Add(new OracleParameter(":exam_id", exam_id));
            var sql = "select student_answer,self_order,numinpaper,question.question_id,description,type from student_exam_question,question where student_exam_question.question_id=question.question_id and student_id=:student_id and student_exam_question.course_id=:course_id and exam_id=:exam_id order by numinpaper";

            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":student_id", student_id));
            parameters.Add(new OracleParameter(":course_id", course_id));
            parameters.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];

            List<q_info> question_list = new List<q_info>();

            foreach (DataRow item in data.Rows)
            {
                List<Option_info> opt_list = new List<Option_info>();
                string q_id = item["question_id"].ToString();
                string desc = item["description"].ToString();
                int s_order = int.Parse(item["self_order"].ToString());
                int type = int.Parse(item["type"].ToString());
                numinpaper = int.Parse(item["numinpaper"].ToString());
                var sql_opt = "select option_id,content from question_option where question_id=:question_id order by option_id";
                List<OracleParameter> parameters_opt = new List<OracleParameter>();
                parameters_opt.Add(new OracleParameter("question_id", q_id));
                var options = DbHelperOra.Query(sql_opt, parameters_opt.ToArray()).Tables[0];
                foreach (DataRow opt in options.Rows)
                {
                    opt_list.Add(new Option_info
                    {
                        question_id = q_id,
                        option_id = int.Parse(opt["option_id"].ToString()),
                        content = opt["content"].ToString()
                    });
                }
                if (s_order > 0 && (type == 0 || type == 1))
                {
                    opt_list = rand_answer.rand_order(s_order, opt_list);
                }

                // 解码
                bool done = false;
                string fill_content = null;
                List<int> s_picked = null;
                List<int> m_picked = null;
                if (item["student_answer"].ToString() != null && item["student_answer"].ToString() != "")
                {
                    done = true;
                    if (type == 2 || type == 3)
                    {
                        fill_content = item["student_answer"].ToString();
                    }
                    else if (type == 0)
                    {
                        s_picked = rand_answer.rand_order_index(s_order, item["student_answer"].ToString()).rand_index;
                    }
                    else if (type == 1)
                    {
                        m_picked = rand_answer.rand_order_index(s_order, item["student_answer"].ToString()).rand_index;
                    }
                    else
                    {
                        var error_return = new TakeexamGetReturn { status = "Bad" };
                        return error_return;
                    }
                }

                question_list.Add(new q_info
                {
                    id = q_id,
                    description = desc,
                    type = type,
                    order = s_order,
                    options = opt_list,
                    numinpaper = numinpaper,
                    fill_content = fill_content,
                    s_picked = s_picked,
                    m_picked = m_picked,
                    done = done
                });
            }
            var person_authority = new TakeexamGetReturn { status = "Good", question_list = question_list, title = title, start_time = start_time, end_time = end_time };
            return person_authority;
            #endregion
        }

        /**********************************************************************************
         * 参数：stu_id course_id exam_id question_id 
         * 功能：上传考生题目答案
         * 要传入的信息：考生id，考试id，课程id，题目id
         * 返回值：成功返回TakeexamPostReturn{"status":"Good",result:"Update Successful"}，
         *           否则返回TakeexamPostReturn{"status":"Bad",result:"Update Successful"}。
         *           // 选择题传入：  List<int> choice_answer
         *           //填空大题传入： string fillin_answer
         ***********************************************************************************/

        [HttpPost("PostAnswer")]
        public ActionResult<Return> PostAnswer([FromBody] JObject stu_info)
        {
            if (HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("course_id") == null || HttpContext.Session.GetString("exam_id") == null)
            {
                return new Return { status = "Bad" };
            }
            string student_id = HttpContext.Session.GetString("id");
            string course_id = HttpContext.Session.GetString("course_id");
            int exam_id = int.Parse(HttpContext.Session.GetString("exam_id"));

            var sql_code = "select self_order from student_exam_question where question_id=:question_id and student_id=:student_id and course_id=:course_id and exam_id=:exam_id";
            List<OracleParameter> parameters_code = new List<OracleParameter>();
            parameters_code.Add(new OracleParameter(":question_id", stu_info["question_id"].ToString()));
            parameters_code.Add(new OracleParameter(":student_id", student_id));
            parameters_code.Add(new OracleParameter(":course_id", course_id));
            parameters_code.Add(new OracleParameter(":exam_id", exam_id));
            var data = DbHelperOra.Query(sql_code, parameters_code.ToArray()).Tables[0];

            var sql_opt = "select option_id,content from question_option where question_id=:question_id order by option_id";
            List<OracleParameter> parameters_opt = new List<OracleParameter>();
            parameters_opt.Add(new OracleParameter("question_id", stu_info["question_id"].ToString()));
            var options = DbHelperOra.Query(sql_opt, parameters_opt.ToArray()).Tables[0];
            List<Option_info> opt_list = new List<Option_info>();
            foreach (DataRow opt in options.Rows)
            {
                opt_list.Add(new Option_info
                {
                    question_id = "",
                    option_id = int.Parse(opt["option_id"].ToString()),
                    content = opt["content"].ToString()
                });
            }

            string fillin_answer = stu_info["fillin_answer"].ToString();
            List<int> choice_answer = new List<int>();
            foreach (var item in stu_info["choice_answer"])
            {
                choice_answer.Add(int.Parse(item.ToString()));
            }

            string Ans = "";
            foreach (DataRow code_data in data.Rows)
            {
                if (int.Parse(code_data["self_order"].ToString()) > 0)
                    Ans = get_answer.calindex(int.Parse(code_data["self_order"].ToString()), opt_list, choice_answer).answer_code;
                else
                    Ans = fillin_answer;
            }

            var sql = "update student_exam_question set student_answer=:Ans where question_id=:Q_Id and student_id =:Stu_Id and course_id=:Cou_Id and exam_id=:Exam_Id";
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter(":Ans", Ans));
            parameters.Add(new OracleParameter(":Q_Id", stu_info["question_id"].ToString()));
            parameters.Add(new OracleParameter(":Stu_Id", student_id));
            parameters.Add(new OracleParameter(":Cou_Id", course_id));
            parameters.Add(new OracleParameter(":Exam_Id", exam_id));

            var isok = DbHelperOra.ExecuteSql(sql, parameters.ToArray());
            Console.WriteLine(isok);
            string result = isok >= 0 ? "Update Successful" : "Update Unsucessful";
            var person_authority = new codeReturn { status = "Good", result = result, code = Ans };
            return person_authority;
        }


        [HttpDelete]
        public ActionResult<Return> Delete()
        {
            if (HttpContext.Session.GetString("id") == null || HttpContext.Session.GetString("course_id") == null || HttpContext.Session.GetString("exam_id") == null)
            {
                return new Return { status = "Bad" };
            }
            else
            {
                var student_id = HttpContext.Session.GetString("id");
                var course_id = HttpContext.Session.GetString("course_id");
                var exam_id = HttpContext.Session.GetString("exam_id");
                List<OracleParameter> updatestate = new List<OracleParameter>();
                updatestate.Add(new OracleParameter(":student_id", student_id));
                updatestate.Add(new OracleParameter(":course_id", course_id));
                updatestate.Add(new OracleParameter(":exam_id", exam_id));
                var temp = DbHelperOra.ExecuteSql("update student_exam set state=2 where student_id=:student_id and course_id=:course_id and exam_id=:exam_id", updatestate.ToArray());
                HttpContext.Session.Remove("course_id");
                HttpContext.Session.Remove("exam_id");
                return new Return { status = "Good" };
            }
        }
    }
}