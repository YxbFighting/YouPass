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
    public class SignupController : ControllerBase
    {
        /******************************************************************
         * 参数：userInfomation Json类型的参数
         *          调用方法: 名字：userInfomation["name"].ToString()
         *                    密码：userInfomation["keyWord"].ToString()
         *                    类型：int.Parse(userInfomation["type"].ToString()) 
         *                          标识是学生，还是老师，还是管理员。分别为0 1 2
         *                    邮箱：userInfomation["email"].ToString()
         *                    ....(以后再说)
         * 功能：首先生成它的id，生成方法是从数据库中找出最小的id，然后+1。
         *       然后将内容插入。要区分老师、学生、管理员。
         * 返回值：成功返回SignupReturn({status:"Good",generatedId:生成的账号})，
         *         否则返回SignupReturn({status:"Bad"})
         * {"name":"2313","keyWord":"dsadas","type":"1"}
         ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject userInfomation)
        {
            var name = userInfomation["name"].ToString();
            var password = userInfomation["keyWord"].ToString();
            var email = userInfomation["email"].ToString();
            var leixing = int.Parse(userInfomation["type"].ToString());
            if (leixing == 0)//老师
            {
                var _new_id = Tools.GetMaxID_Person("ID", "teacher", leixing).ToString();
                var sql_insert = "insert into teacher(id,name,password,email) values(:id,:name,:password,:email)";
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter(":id", _new_id));
                parameters.Add(new OracleParameter(":name", name));
                parameters.Add(new OracleParameter(":password", password));
                parameters.Add(new OracleParameter(":email", email));

                List<OracleParameter> parameters1 = new List<OracleParameter>();
                parameters1.Add(new OracleParameter(":e_mail", email));
                var check = DbHelperOra.Exists("SELECT COUNT(email) FROM teacher WHERE email =:e_mail", parameters1.ToArray());



                if (check)
                {
                    SignupReturn r = new SignupReturn { status = "Bad", tag = "0" };
                    return r;
                }
                var isok = DbHelperOra.ExecuteSql(sql_insert, parameters.ToArray());
                if (isok > 0)
                {
                    SignupReturn r = new SignupReturn { status = "Good", generatedId = _new_id };
                    return r;
                }
                else
                {
                    SignupReturn r = new SignupReturn { status = "Bad" };
                    return r;
                }
            }
            else if (leixing == 1)//学生
            {
                var _new_id = Tools.GetMaxID_Person("ID", "student", leixing).ToString();
                var sql_insert = "insert into student(id,name,password,email) values(:id,:name,:password,:email)";
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter(":id", _new_id));
                parameters.Add(new OracleParameter(":name", name));
                parameters.Add(new OracleParameter(":password", password));
                parameters.Add(new OracleParameter(":email", email));

                List<OracleParameter> parameters2 = new List<OracleParameter>();
                parameters2.Add(new OracleParameter(":e_mail", email));
                var check = DbHelperOra.Exists("SELECT COUNT(email) FROM student WHERE email =:e_mail", parameters2.ToArray());

                if (check)
                {
                    SignupReturn r = new SignupReturn { status = "Bad", tag = "0" };
                    return r;
                }
                var isok = DbHelperOra.ExecuteSql(sql_insert, parameters.ToArray());
                if (isok > 0)
                {
                    SignupReturn r = new SignupReturn { status = "Good", generatedId = _new_id };
                    return r;
                }
                else
                {
                    SignupReturn r = new SignupReturn { status = "Bad" };
                    return r;
                }

            }
            var sr = new Return { status = "Bad" };
            return sr;
        }


        /*---------------------------------------------------------------
         * 工具函数
         ---------------------------------------------------------------*/

        //SignupReturn sr;
        //var name = userInfomation["name"].ToString();
        //var password = userInfomation["keyWord"].ToString();
        //var email = userInfomation["email"].ToString();
        //var leixing = int.Parse(userInfomation["type"].ToString());
        //var _new_id = DbHelperOra.GetMaxID("ID", "teacher");
        //var sql_insert = "insert into teacher(id,name,password,email) values(:_new_id,:name,:password,:email)";
        //List<OracleParameter> parameters = new List<OracleParameter>();
        //parameters.Add(new OracleParameter(":_new_id", _new_id));
        //parameters.Add(new OracleParameter(":name", name));
        //parameters.Add(new OracleParameter(":password", password));
        //parameters.Add(new OracleParameter(":email", email));

    }

}