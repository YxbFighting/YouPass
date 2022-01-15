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
    public class UserInfomationController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：id string
         * 功能：通过用户的id，获得这个用户的信息
         * 返回值：成功返回UserInfomationReturn{"status":"Good",name:"",id:"",email:""}，
         *           否则返回UserInfomationReturn{"status":"Bad"}。
         ******************************************************************/
        [HttpGet("CheckState")]
        // public ActionResult<Return> Get(String id)
        public ActionResult<Return> CheckState()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            else
            {
                return new Return { status = "Good" };
            }
        }

        [HttpGet("GetIdentity")]
        // public ActionResult<Return> Get(String id)
        public ActionResult<Return> GetIdentity()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            if (Tools.IsExistInTeacher(id))
            {
                return new IdentityReturn { status = "Good", identity = 0 };
            }
            else
            {
                return new IdentityReturn { status = "Good", identity = 1 };
            }
        }

        /******************************************************************
         * 方式：Http Get
         * 参数：id string
         * 功能：通过用户的id，获得这个用户的信息
         * 返回值：成功返回UserInfomationReturn{"status":"Good",name:"",id:"",email:""}，
         *           否则返回UserInfomationReturn{"status":"Bad"}。
         ******************************************************************/
        [HttpGet]
        // public ActionResult<Return> Get(String id)
        public ActionResult<Return> Get()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            if (!Tools.IsExist(id))
            {
                return new Return { status = "Bad" };
            }
            var sql = "";
            if (Tools.IsExistInStudent(id))
            {
                sql = "select name,email,location from student where student.id=:id";
            }
            else
            {
                sql = "select name,email,location from teacher where teacher.id=:id";
            }
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];
            string _name = "";
            string _eamil = "";
            string _location = "";
            foreach (DataRow item in data.Rows)
            {
                _name = item["name"].ToString();
                _eamil = item["email"].ToString();
                _location = item["location"].ToString();
            }
            var r = new UserInfomationReturn { status = "Good", name = _name, id = id, email = _eamil, location = _location };
            return r;
        }
        /******************************************************************
        * 方式：Http Put
        * 参数：userInfomation json
        *          id:    session获得                                       用来判断用户信息
        *          name:  userInfomation["name"].ToString()                 待修改
        *          email: userInfomation["email"].ToString()                待修改
        *          password: userInfomation["password"].ToString()          待修改
        *          location: userInfomation["location"].ToString()          待修改
        
        * 功能：更改用户信息
        * 返回值：成功返回Return{"status":"Good"}，
        *           否则返回Return{"status":"Bad"}。
        ******************************************************************/
        [HttpPut]
        public ActionResult<Return> Put([FromBody] JObject userInfomation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            if (userInfomation["name"] == null
            || userInfomation["email"] == null
            || userInfomation["location"] == null)
            {
                Return sr = new Return { status = "Bad" };
                return sr;
            }
            var name = userInfomation["name"].ToString();
            var email = userInfomation["email"].ToString();
            var location = userInfomation["location"].ToString();

            List<OracleParameter> parameters1 = new List<OracleParameter>();
            //List<OracleParameter> parameters2 = new List<OracleParameter>();
            List<OracleParameter> parameters3 = new List<OracleParameter>();
            List<OracleParameter> parameters4 = new List<OracleParameter>();

            parameters1.Add(new OracleParameter(":name", name));
            parameters1.Add(new OracleParameter(":id", id));
            //parameters2.Add(new OracleParameter(":password", password));
            //parameters2.Add(new OracleParameter(":id", id));
            parameters3.Add(new OracleParameter(":email", email));
            parameters3.Add(new OracleParameter(":id", id));
            parameters4.Add(new OracleParameter(":location", location));
            parameters4.Add(new OracleParameter(":id", id));
            //该ID不存在
            if (!Tools.IsExist(id))
            {
                return new Return { status = "Bad" };
            }
            //是学生
            else if (Tools.IsExistInStudent(id))
            {
                var sql_update_name = @"update student set name = :name where id=:id";
                var sql_update_email = @"update student set email = :email where id=:id";
                var sql_update_location = @"update student set location = :location where id=:id";
                DbHelperOra.ExecuteSql(sql_update_name, parameters1.ToArray());
                DbHelperOra.ExecuteSql(sql_update_email, parameters3.ToArray());
                DbHelperOra.ExecuteSql(sql_update_location, parameters4.ToArray());
            }
            //是老师
            else
            {
                var sql_update_name = @"update teacher set name = :name where id=:id";
                var sql_update_email = @"update teacher set email = :email where id=:id";
                var sql_update_location = @"update teacher set location = :location where id=:id";
                DbHelperOra.ExecuteSql(sql_update_name, parameters1.ToArray());
                DbHelperOra.ExecuteSql(sql_update_email, parameters3.ToArray());
                DbHelperOra.ExecuteSql(sql_update_location, parameters4.ToArray());
            }
            var r = new Return { status = "Good" };
            return r;
        }

        /******************************************************************
        * 方式：Http Delete
        * 参数：无
        *          id: 从session中获得
        * 功能：通过用户的id，获得这个用户的权力（指导航栏里的内容，如
        *          “获得考试成绩”）
        * 返回值：成功返回AuthorityReturn{"status":"Good",authorities:[]}，
        *           否则返回AuthorityReturn{"status":"Bad"}。
        ******************************************************************/
        [HttpDelete]
        public ActionResult<Return> Delete()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            else
            {
                HttpContext.Session.Remove("id");
                return new Return { status = "Good" };
            }
        }

        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}