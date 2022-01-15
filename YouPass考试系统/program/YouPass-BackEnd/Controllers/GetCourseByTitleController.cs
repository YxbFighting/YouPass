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

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCourseByTitleController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：title string
         * 功能：通过课程名称，获得这个名称的所有课程
         * 返回值：成功返回CourseGetReturn{"status":"Good",courselist:["","",""......]}，
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get(String title)
        {
            if (title == "" || title == null)
            {
                return new Return { status = "Bad" };
            }
            var sql = "select teacher_id,course_id,password from course where title=:title";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":title", title));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<info_Course> courselist = new List<info_Course>();
            foreach (DataRow item in data.Rows)
            {
                courselist.Add(new info_Course { teacher_id = item["teacher_id"].ToString(), course_id = item["course_id"].ToString(), title = title, password = item["password"].ToString() });
            }
            var r = new CourseGetReturn { status = "Good", info_course = courselist };
            return r;
        }


        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}