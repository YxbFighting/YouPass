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
    public class GetCourseExam : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：course_id string
         * 功能：通过课程编号，获得这个课程的所有考试
         * 返回值：成功返回CourseGetReturn{"status":"Good",courselist:["","",""......]}，
         ******************************************************************/
        [HttpGet("{course_id}")]
        public ActionResult<Return> Get(string course_id)
        {
            if (course_id == "" || course_id == null)
            {
                return new Return { status = "Bad" };
            }
            var sql = "select course_id,exam_id,title,start_time,end_time,nature from exam where course_id=:course_id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":course_id", course_id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            List<info_exam> exam_list = new List<info_exam>();
            foreach (DataRow item in data.Rows)
            {
                int type = int.Parse(item["nature"].ToString());
                var Exam_type = "";
                if (type == 1)
                    Exam_type = "正式考试";
                else
                {
                    Exam_type = "模拟考试";
                }
                exam_list.Add(new info_exam
                {
                    course_id = item["course_id"].ToString(),
                    exam_id = item["exam_id"].ToString(),
                    title = item["title"].ToString(),
                    start_time = Convert.ToDateTime(item["start_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    end_time = Convert.ToDateTime(item["end_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    exam_type = Exam_type
                });
            }
            var r = new ExaminfoReturn { status = "Good", info_Exam = exam_list };
            return r;
        }


        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}