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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /******************************************************************
        * 测试用
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            Return sr = new Return { status = "Good" };
            return sr;
        }

        // [HttpGet("get")]
        // public ActionResult<Return> Get()
        // {
        //     var sessionData = HttpContext.Session.GetString("id");
        //     Return sr = new Return { status = sessionData };
        //     return sr;
        // }

        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject userInfomation)
        {
            // using (FileStream fileStream = System.IO.File.Create("D:\\MyProject\\file\\" + objectFile.files.FileName))
            // {
            //     objectFile.files.CopyTo(fileStream);
            //     fileStream.Flush();
            // }
            var sessionData = HttpContext.Session.GetString("keyname");
            Return sr = new Return { status = userInfomation.ToString() };
            return sr;
        }

        [HttpPut]
        public ActionResult<Return> Put([FromBody] JObject userInfomation)
        {
            Return sr = new Return { status = "Put" };
            return sr;
        }

        [HttpDelete]
        public ActionResult<Return> Delete([FromBody] JObject userInfomation)
        {
            Return sr = new Return { status = "Delete" };
            return sr;
        }


        /*---------------------------------------------------------------
         * 工具函数
         ---------------------------------------------------------------*/
    }
}