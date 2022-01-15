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
    public class TodolistController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Get
         * 参数：id string
         * 功能：通过用户的id，获得这个用户的todolist
         * 返回值：成功返回TodolistReturn{"status":"Good",Todolist:["","",""......]}，
         *           否则返回TodolistReturn{"status":"Bad",Todolist:[]}。
         ******************************************************************/
        [HttpGet]
        public ActionResult<Return> Get()
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            var sql = "select id,description from todolist where person_id=:id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            var data = DbHelperOra.Query(sql, parameters.ToArray()).Tables[0];

            List<info_todo> todolist = new List<info_todo>();
            foreach (DataRow item in data.Rows)
            {
                todolist.Add(new info_todo { id = int.Parse(item["id"].ToString()), description = item["description"].ToString() });
            }
            var r = new TodolistGetReturn { status = "Good", todolist = todolist };
            return r;
        }
        /******************************************************************
        * 方式：Http Post
        * 参数：id string ;todo string
        *          id:      用来判断用户信息
        *          todo:    待添加
        * 功能：更改用户信息
        * 返回值：成功返回Return{"status":"Good","id":"todo的id"}，
        *           否则返回Return{"status":"Bad"，空"id"}。
        ******************************************************************/
        [HttpPut]
        public ActionResult<Return> Put([FromBody] JObject userInfomation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string person_id = HttpContext.Session.GetString("id");
            List<string> description = new List<string>();
            List<int> todo_id = new List<int>();
            bool flag = false;
            var sql_of_tododiagram = "select description,id from todolist where person_id=:person_id";
            List<OracleParameter> parameters_1 = new List<OracleParameter>();
            parameters_1.Add(new OracleParameter(":person_id", person_id));
            var data = DbHelperOra.Query(sql_of_tododiagram, parameters_1.ToArray()).Tables[0];
            foreach (DataRow item in data.Rows)
            {
                flag = true;
                description.Add(item["description"].ToString());
                todo_id.Add(int.Parse(item["id"].ToString()));
            }
            // var r2 = new Return { status = new_id.ToString() };
            // return r2;
            //表里已经有该人的信息
            if (flag)
            {
                //对id进行排序，然后取最大加一
                todo_id.Sort();
                var max_id = todo_id[todo_id.Count() - 1];
                int new_id = max_id + 1;
                var sql_insert = "insert into todolist (person_id,id,description) values(:person_id,:id,:description)";
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter(":person_id", person_id));
                parameters.Add(new OracleParameter(":id", new_id));
                parameters.Add(new OracleParameter(":description", userInfomation["description"].ToString()));
                var isok = DbHelperOra.ExecuteSql(sql_insert, parameters.ToArray());
                if (isok >= 0)
                {
                    var r = new TodolistPutReturn { status = "Good", new_id = new_id };
                    return r;
                }
                else
                {
                    var r = new Return { status = "Bad" };
                    return r;
                }
            }
            //表里没有该人的id
            else
            {
                var _new_id = 1;
                var sql_insert = "insert into todolist (person_id,id,description) values(:person_id,:id,:description)";
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter(":person_id", person_id));
                parameters.Add(new OracleParameter(":id", _new_id));
                parameters.Add(new OracleParameter(":description", userInfomation["description"].ToString()));
                var isok = DbHelperOra.ExecuteSql(sql_insert, parameters.ToArray());
                if (isok >= 0)
                {
                    var r = new TodolistPutReturn { status = "Good", new_id = _new_id };
                    return r;
                }
                else
                {
                    var r = new Return { status = "Bad" };
                    return r;
                }
            }
        }

        /******************************************************************
         * 方式：Http Delete
         * 参数：person_id string   id int
         * 功能：删除用户id为person_id，todo的id为id的description
         * 返回值：成功返回Return{"status":"Good"}，
         *           否则返回Return{"status":"Bad"}。
         ******************************************************************/
        [HttpDelete]
        public ActionResult<Return> Delete(int id)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string person_id = HttpContext.Session.GetString("id");
            var sql = "delete from todolist where person_id=:person_id and id=:id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":person_id", person_id));
            parameters.Add(new OracleParameter(":id", id));
            var isok = DbHelperOra.ExecuteSql(sql, parameters.ToArray());
            if (isok > 0)
            {
                var sql_commit = "commit";
                DbHelperOra.ExecuteSql(sql_commit);
                var r = new Return { status = "Good" };
                return r;
            }
            else
            {
                var r = new Return { status = "Bad" };
                return r;
            }
        }

        /*---------------------------------------------------------------
         * 工具函数
        ---------------------------------------------------------------*/
    }
}