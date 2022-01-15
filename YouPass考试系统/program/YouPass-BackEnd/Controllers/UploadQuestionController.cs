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
    public class UploadQuestionController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Post
         * 参数：userInfomation
         * 功能：老师上传题目到题库中
         * 返回值：成功返回TodolistReturn{"status":"Good"}，
         *        否则返回TodolistReturn{"status":"Bad"}。
         ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JArray userInfomation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string teacher_id = HttpContext.Session.GetString("id");
            var sql = "insert into QUESTION (Question_id,Teacher_id,description,Type,Standard_Answer,Subject,Create_time,isPrivate,Pool,course_id)" +
            "values(:question_id,:teacher_id,:description,:type,:standard_answer,:subject,:create_time,:isprivate,:pool,:course_id)";
            int res = 0;
            foreach (JObject item in userInfomation)
            {
                //错误处理
                if (item["Description"] == null ||
                    item["Type"] == null ||
                    item["Standard_Answer"] == null ||
                    item["Subject"] == null ||
                    item["Create_time"] == null ||
                    item["isPrivate"] == null ||
                    item["Pool"] == null ||
                    item["course_id"] == null)
                {
                    var r = new UploadQuestionReturn { status = "Bad", success_num = res };
                    return r;
                }
                //插入选择题但是没有选项，视为错误
                if (int.Parse(item["Type"].ToString()) == 0 && item["option"].Count() == 0)
                {
                    var r = new UploadQuestionReturn { status = "Bad", success_num = res };
                    return r;
                }

                DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var Create_time = converted.AddSeconds(double.Parse(item["Create_time"].ToString()));
                var description = item["Description"].ToString();
                var type = int.Parse(item["Type"].ToString());
                var standard_answer = item["Standard_Answer"].ToString();
                var subject = item["Subject"].ToString();
                //var create_time = Convert.ToDateTime(Convert.ToDateTime(Create_time.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                var isprivate = int.Parse(item["isPrivate"].ToString());
                var pool = int.Parse(item["Pool"].ToString());
                var course_id = item["course_id"].ToString();

                List<OracleParameter> parameters = new List<OracleParameter>();
                var Que_Id = Tools.GetMaxID("Question_id", "question").ToString();
                parameters.Add(new OracleParameter(":question_id", Que_Id));
                parameters.Add(new OracleParameter(":teacher_id", teacher_id));
                parameters.Add(new OracleParameter(":description", description));
                parameters.Add(new OracleParameter(":type", type));
                parameters.Add(new OracleParameter(":standard_answer", standard_answer));
                parameters.Add(new OracleParameter(":subject", subject));
                parameters.Add(new OracleParameter(":create_time", Create_time));
                parameters.Add(new OracleParameter(":isprivate", isprivate));
                parameters.Add(new OracleParameter(":pool", pool));
                parameters.Add(new OracleParameter(":course_id", course_id));
                var data = DbHelperOra.ExecuteSql(sql, parameters.ToArray());

                if (type == 0 || type == 1)
                {
                    int flag = 0;
                    int count = 0;
                    List<Option_info> options = new List<Option_info>();
                    foreach (var sub_item in item["option"])
                    {
                        options.Add(new Option_info { option_id = int.Parse((sub_item["Option_id"].ToString())), question_id = Que_Id, content = sub_item["content"].ToString() });
                    }
                    foreach (var sub_item in options)
                    {
                        var sql_option = "insert into Question_option (option_id,content,question_id) values (:option_id,:content,:question_id)";
                        List<OracleParameter> parameters_opt = new List<OracleParameter>();
                        parameters_opt.Add(new OracleParameter(":option_id", sub_item.option_id));
                        parameters_opt.Add(new OracleParameter(":content", sub_item.content));
                        parameters_opt.Add(new OracleParameter(":question_id", Que_Id));
                        var issuccess = DbHelperOra.ExecuteSql(sql_option, parameters_opt.ToArray());
                        if (issuccess > 0)
                        {
                            count++;
                        }
                    }
                    if ((count == options.Count()) && data > 0)
                    {
                        flag = 1;
                    }
                    res += flag;
                }
                else
                {
                    res += data;
                }
            }
            if (res == userInfomation.Count())
            {
                var ret = new UploadQuestionReturn { status = "Good", success_num = res };
                return ret;
            }
            else
            {
                var ret = new UploadQuestionReturn { status = "Good", success_num = res };
                return ret;
            }
        }

        /******************************************************************
         * 方式：Http Get
         * 参数：userInfomation
         * 功能：老师获取当前题库的所有信息
         * 返回值：成功返回TodolistReturn{"status":"Good"}，
         *        否则返回TodolistReturn{"status":"Bad"}。
         ******************************************************************/
        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}