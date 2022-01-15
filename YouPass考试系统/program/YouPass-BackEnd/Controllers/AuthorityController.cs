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

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        /******************************************************************
         * 参数：id int
         * 功能：通过用户的id，获得这个用户的权力（指导航栏里的内容，如
         *          “获得考试成绩”）
         * 返回值：成功返回AuthorityReturn{"status":"Good",authorities:[]}，
         *           否则返回AuthorityReturn{"status":"Bad"}。
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get(string id)
        {
            var r = new AuthorityReturn { status = "Good", authorities = new List<string>() { "a", "b" } };
            return r;
        }

        /*---------------------------------------------------------------
         * 工具函数
         ---------------------------------------------------------------*/
    }
}