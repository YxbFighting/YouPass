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
using YouPass_BackEnd.Models.ReturnTypes;
using Microsoft.AspNetCore.Http;
namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateExamController : ControllerBase
    {
        /******************************************************************
         * 方式：Http Post
         * 参数：examInfomation Json类型的参数
         * 功能：传入考试的信息，在数据库中添加考试并与题目联系起来，从而生成考试试卷
         * 返回值：成功返回Return{"status":"Good"}，
         *           否则返回Return{"status":"Bad"}。
         ******************************************************************/
        [HttpPost]
        public ActionResult<Return> Post([FromBody] JObject examinformation)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string teacher_id = HttpContext.Session.GetString("id");
            string course_Id = examinformation["course_Id"].ToString();
            string exam_title = examinformation["exam_title"].ToString();
            DateTime start_time = new DateTime();
            start_time = Convert.ToDateTime(examinformation["exam_starttime"].ToString());
            DateTime end_time = new DateTime();
            end_time = Convert.ToDateTime(examinformation["exam_endtime"].ToString());
            int[] questiontype = new int[4];
            questiontype[0] = int.Parse(examinformation["choice"].ToString());
            questiontype[1] = int.Parse(examinformation["multiplechoice"].ToString());
            questiontype[2] = int.Parse(examinformation["completion"].ToString());
            questiontype[3] = int.Parse(examinformation["written"].ToString());
            int[] questionvalue = new int[4];
            questionvalue[0]= int.Parse(examinformation["choicevalue"].ToString());
            questionvalue[1] = int.Parse(examinformation["multiplechoicevalue"].ToString());
            questionvalue[2] = int.Parse(examinformation["completionvalue"].ToString());
            questionvalue[3] = int.Parse(examinformation["writtenvalue"].ToString());
            int self_order = new int();
            int numinpaper = new int();
            //查看老师是否教授该课程
            //
            //
            List<OracleParameter> checkteachercourse = new List<OracleParameter>();
            checkteachercourse.Add(new OracleParameter(":course_id", course_Id));
            checkteachercourse.Add(new OracleParameter(":teacher_id", teacher_id));
            string checktc = "select * from course where course_id=:course_id and teacher_id=:teacher_id";
            if (!DbHelperOra.Exists(checktc, checkteachercourse.ToArray()))
            {
                CreateExamReturn re = new CreateExamReturn { status = "Bad", exam_id = -1, errorinfo = "该老师不教授该课程" };
                return re;
            }
            //
            //
            //查看该课程是否有学生
            //
            //
            OracleParameter checkcoursestudent = new OracleParameter(":course_id", course_Id);
            string checkcs = "select count(*) from student_course where course_id=:course_id";
            if (!DbHelperOra.Exists(checkcs, checkcoursestudent))
            {
                CreateExamReturn re = new CreateExamReturn { status = "Bad", exam_id = -1, errorinfo = "该课程没有学生" };
                return re;
            }

            OracleParameter test1 = new OracleParameter(":course_id", course_Id);
            List<OracleParameter> test2 = new List<OracleParameter>();
            test2.Add(new OracleParameter(":course_id", course_Id));
            test2.Add(new OracleParameter(":title", exam_title));
            List<OracleParameter> ci = new List<OracleParameter>();
            ci.Add(new OracleParameter(":course_id", course_Id));
            object temp = DbHelperOra.GetSingle("select max(exam_id)+1 from (select exam_id from exam where course_id=:course_id)", test1);
            int examid;
            if (temp == null) examid = 1;
            else examid = int.Parse(temp.ToString());
            ci.Add(new OracleParameter(":exam_id", examid));
            ci.Add(new OracleParameter(":start_time", start_time));
            ci.Add(new OracleParameter(":end_time", end_time));
            ci.Add(new OracleParameter(":title", exam_title));
            var if1 = DbHelperOra.Exists("select count(*) from course where course_id=:course_id", test1);
            var if2 = DbHelperOra.Exists("select count(*) from exam where course_id=:course_id and title=:title", test2.ToArray());
            if (!if1)
            {
                CreateExamReturn re = new CreateExamReturn { status = "Bad", exam_id = -1, errorinfo = "该课程不存在" };
                return re;
            }
            else if (if2)
            {
                CreateExamReturn re = new CreateExamReturn { status = "Bad", exam_id = -1, errorinfo = "重复的考试名" };
                return re;
            }
            else
            {
                var ifinsert = DbHelperOra.ExecuteSql("insert into exam values(:course_id,:exam_id,:start_time,:end_time,0,:title)", ci.ToArray());
                if (ifinsert <= 0) { CreateExamReturn re = new CreateExamReturn { status = "Bad", exam_id = -1, errorinfo = "数据库操作失败，请联系系统管理员" }; return re; }
                else
                {
                    List<string> studentlist = new List<string>();
                    var data = DbHelperOra.Query("select ID from student_course where course_id=:course_id", test1).Tables[0];
                    foreach (DataRow item in data.Rows)
                    {
                        studentlist.Add(item["ID"].ToString());
                    }
                    foreach (string stu in studentlist)
                    {
                        numinpaper = 1;
                        List<OracleParameter> studentexam = new List<OracleParameter>();
                        studentexam.Add(new OracleParameter(":student_id", stu));
                        studentexam.Add(new OracleParameter(":course_id", course_Id));
                        studentexam.Add(new OracleParameter(":exam_id", examid));
                        var kkk = DbHelperOra.ExecuteSql("insert into student_exam values(:student_id,:course_id,:exam_id,null,0)",studentexam.ToArray());
                        for (int i = 0; i < 4; i++)
                        {
                            List<OracleParameter> selectinfo = new List<OracleParameter>();
                            selectinfo.Add(new OracleParameter(":questiontype", i));
                            selectinfo.Add(new OracleParameter(":course_id", course_Id));
                            selectinfo.Add(new OracleParameter(":questionnum", questiontype[i]));
                            var datatemp = DbHelperOra.Query("select * from (select * from (select question_id from question where type=:questiontype and course_id=:course_id) order by dbms_random.value) where rownum<=:questionnum", selectinfo.ToArray()).Tables[0];
                            foreach (DataRow itemtemp in datatemp.Rows)
                            {
                                List<OracleParameter> cmd = new List<OracleParameter>();
                                cmd.Add(new OracleParameter(":studentid", stu));
                                cmd.Add(new OracleParameter(":courseid", course_Id));
                                cmd.Add(new OracleParameter(":examid", examid));
                                cmd.Add(new OracleParameter(":questionid", itemtemp["question_id"].ToString()));
                                if (i == 0 || i == 1) self_order = Tools.creatrand(1, 8);
                                else self_order = -1;
                                cmd.Add(new OracleParameter(":self_order", self_order));
                                cmd.Add(new OracleParameter(":value", questionvalue[i]));
                                cmd.Add(new OracleParameter(":numinpaper", numinpaper++));
                                var ttt = DbHelperOra.ExecuteSql("insert into student_exam_question values(:studentid,:courseid,:examid,:questionid,:self_order,'',:value,null,:numinpaper)", cmd.ToArray());
                            }
                        }
                    }
                    int notice_id = DbHelperOra.GetMaxID("notice_id", "notice");
                    DateTime current = DateTime.Now;
                    List<OracleParameter> noticeadd = new List<OracleParameter>();
                    noticeadd.Add(new OracleParameter(":notice_id", notice_id));
                    noticeadd.Add(new OracleParameter(":course_id", course_Id));
                    noticeadd.Add(new OracleParameter(":title", exam_title + "已经发布,开始时间为"+start_time.ToString("yyyy-MM-dd HH:mm:ss")+",结束时间为"+end_time.ToString("yyyy-MM-dd HH:mm:ss")));
                    noticeadd.Add(new OracleParameter(":time", current));
                    int temp000 = DbHelperOra.ExecuteSql("insert into notice values(:notice_id,:course_id,:title,:time)", noticeadd.ToArray());
                    CreateExamReturn re = new CreateExamReturn { status = "Good", exam_id = examid, errorinfo = "" };
                    return re;
                }
            }
        }
    }
}
