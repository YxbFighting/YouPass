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
    public class LoginController : ControllerBase
    {
        /******************************************************************
         * 参数：userInfomation Json类型的参数
         *          调用方法: 账号：userInfomation["id"].ToString()
         *                    密码：userInfomation["keyWord"].ToString()
         * 功能：首先查找它的id，如果在表内则进入下一步，否则返回Return{"status":"Bad"}。
         *       查找id对应的password并与keyword比对，若相同返回Return{"status":"Good"}，否则返回Return{"status":"Bad"}
         * 返回值：如上
         * 
         ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject userInfomation)
        {
            if (userInfomation["id"] == null || userInfomation["keyWord"] == null)
            {
                var ret = new Return { status = "Bad" };
                return ret;
            }
            var id = userInfomation["id"].ToString();
            List<OracleParameter> SearchItem = new List<OracleParameter>();
            SearchItem.Add(new OracleParameter(":id", id));
            string sql = "";
            string sql0 = "";
            if (Tools.IsExistInStudent(id)) { sql = "select password from student where id=:id"; sql0 = "select count(password) from student where id=:id"; }
            else if (Tools.IsExistInTeacher(id)) { sql = "select password from teacher where id=:id"; sql0 = "select count(password) from teacher where id=:id"; }
            //var ifsigned = DbHelperOra.Exists(sql0, SearchItem);
            var data = DbHelperOra.Query(sql, SearchItem.ToArray()).Tables[0];
            string password = "";
            foreach (DataRow item in data.Rows)
            {
                password = item["password"].ToString();
            }
            if (password != "")
            {

                if (password == userInfomation["keyWord"].ToString())
                {
                    // create session
                    // TODO:
                    HttpContext.Session.SetString("id", id);
                    var ret = new Return { status = "Good" };
                    return ret;
                }
                else
                {
                    var ret = new Return { status = "Bad" };
                    return ret;
                }
            }
            else
            {
                var ret = new Return { status = "Bad" };
                return ret;
            }

        }

        /*---------------------------------------------------------------
         * 工具函数
         ---------------------------------------------------------------*/
    }
}