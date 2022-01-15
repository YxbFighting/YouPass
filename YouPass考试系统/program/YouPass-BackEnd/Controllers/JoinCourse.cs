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
    public class JoinCourseController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：title string
         * 功能：通过课程名称，获得这个名称的所有课程
         * 返回值：成功返回CourseGetReturn{"status":"Good",courselist:["","",""......]}，
         ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject userInfomation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string student_id = HttpContext.Session.GetString("id");

            var course_id = userInfomation["course_id"].ToString();

            //查询该id是否已经在该课程中
            var sql_isExist="select id from student_course where course_id=:course_id and id=:student_id";
            List<OracleParameter> parameter_check = new List<OracleParameter>();
            parameter_check.Add(new OracleParameter(":course_id", course_id));
            parameter_check.Add(new OracleParameter(":student_id", student_id));

            var isExist=DbHelperOra.Exists(sql_isExist,parameter_check.ToArray());
            if(isExist){
                return new JoinCourseReturn { message = "您已经加入课程！", status = "Bad" };
            }
            //查询该课程的密码
            var sql_password = "select password from course where course_id=:course_id";
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter(":course_id", course_id));
            var password = "";
            var data = DbHelperOra.Query(sql_password, parameter.ToArray()).Tables[0];
            foreach (DataRow item in data.Rows)
            {
                password = item["password"].ToString();
            }
            if (password == userInfomation["password"].ToString())
            {
                var sql = "insert into student_course(id,course_id) values(:id,:course_id)";
                List<OracleParameter> parameter2 = new List<OracleParameter>();
                parameter2.Add(new OracleParameter(":id", student_id));
                parameter2.Add(new OracleParameter(":course_id", course_id));
                int is_success = DbHelperOra.ExecuteSql(sql, parameter2.ToArray());
                if (is_success > 0)
                {
                    List<string> examid = new List<string>();
                    OracleParameter findexam = new OracleParameter(":course_id", course_id);
                    var datadata = DbHelperOra.Query("select distinct exam_id from student_exam where course_id=:course_id", findexam).Tables[0];
                    foreach (DataRow yy in datadata.Rows) examid.Add(yy["exam_id"].ToString());
                    foreach(string examidddd in examid)
                    {
                        List<OracleParameter> studentexam = new List<OracleParameter>();
                        studentexam.Add(new OracleParameter(":student_id", student_id));
                        studentexam.Add(new OracleParameter(":course_id", course_id));
                        studentexam.Add(new OracleParameter(":exam_id", int.Parse(examidddd)));
                        var nnnnnnn = DbHelperOra.ExecuteSql("insert into student_exam values(:student_id,:course_id,:exam_id,null,null)", studentexam.ToArray());
                    }
                    return new JoinCourseReturn { message = "成功加入课程", status = "Good" };
                }
                else
                {
                    return new JoinCourseReturn { message = "系统故障，加入课程失败", status = "Bad" };
                }
            }
            else
            {
                return new JoinCourseReturn { message = "密码错误", status = "Bad" };
            }
        }


        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}