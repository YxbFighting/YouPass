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
    public class NoticeCheckController : ControllerBase
    {
        /******************************************************************
         * Http Get
         * 参数：id string
         * 功能：通过学生id返回其选的课程的所有信息
         * 返回值：成功返回MessageCheckReturn{"status":"Good",Noticelist:["","",""......]}，
         *           否则返回MessageCheckReturn{"status":"Bad",Noticelist:[]}。
         * 
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            
            var sql = "SELECT NOTICE_ID,NOTICE.COURSE_ID,CONTENT,TIME,COURSE.TITLE FROM NOTICE, STUDENT_COURSE,COURSE WHERE STUDENT_COURSE.ID=:id AND STUDENT_COURSE.COURSE_ID=NOTICE.COURSE_ID AND STUDENT_COURSE.COURSE_ID=COURSE.COURSE_ID";
            List<OracleParameter> stuid = new List<OracleParameter>();
            stuid.Add(new OracleParameter(":id", id));
            var data = DbHelperOra.Query(sql, stuid.ToArray()).Tables[0];
            List<Notice> noticelist = new List<Notice>();
            foreach (DataRow item in data.Rows)
            {
                noticelist.Add(new Notice
                {
                    notice_id = int.Parse(item["NOTICE_ID"].ToString()),
                    course_id = item["COURSE_ID"].ToString(),
                    content = item["CONTENT"].ToString(),
                    date = Convert.ToDateTime(item["TIME"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    title = item["TITLE"].ToString()
                });
            }
            var r = new NoticeCheckReturn { status = "Good", Noticelist = noticelist };
            return r;
        }
    }
}
