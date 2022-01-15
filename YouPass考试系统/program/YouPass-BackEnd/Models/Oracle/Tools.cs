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
namespace YouPass_BackEnd.Models
{
    /// <summary>
    /// 一些公共函数
    /// </summary>
    public abstract class Tools
    {
        //生成随机数
        public static int creatrand(int x, int y)
        {
            Random ran = new Random();
            int RandKey = ran.Next(x, y + 1);
            return RandKey;
        }
        /*查看该人是否是老师*/
        public static bool IsExistInTeacher(string id)
        {
            var sql = "select count (id) from teacher where id=:id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            return DbHelperOra.Exists(sql, parameters.ToArray());
        }
        /*查看该人是否是学生*/
        public static bool IsExistInStudent(string id)
        {
            var sql = "select count (id) from student where id=:id";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter(":id", id));
            return DbHelperOra.Exists(sql, parameters.ToArray());
        }
        /*查看该人是否是老师或学生*/
        public static bool IsExist(string id)
        {

            var sql_1 = "select count (id) from student where id=:id";
            List<OracleParameter> parameters_1 = new List<OracleParameter>();
            parameters_1.Add(new OracleParameter(":id", id));
            bool res1 = DbHelperOra.Exists(sql_1, parameters_1.ToArray());

            var sql_2 = "select count (id) from teacher where id=:id";
            List<OracleParameter> parameters_2 = new List<OracleParameter>();
            parameters_2.Add(new OracleParameter(":id", id));
            bool res2 = DbHelperOra.Exists(sql_2, parameters_2.ToArray());
            return res1 || res2;
        }

        public static int GetMaxID(string FieldName, string TableName)
        {

            string strsql = "select max(TO_NUMBER(" + TableName + "." + FieldName + ")) from " + TableName;
            object obj = DbHelperOra.GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString()) + 1;
            }
        }
        public static int GetMaxID_Course(string FieldName, string TableName)
        {

            string strsql = "select max(TO_NUMBER(" + TableName + "." + FieldName + ")) from " + TableName;
            object obj = DbHelperOra.GetSingle(strsql);
            if (obj == null)
            {
                return 420240;
            }
            else
            {
                return int.Parse(obj.ToString()) + 1;
            }
        }
        public static int GetMaxID_Person(string FieldName, string TableName, int type)
        {

            string strsql = "select max(TO_NUMBER(" + TableName + "." + FieldName + ")) from " + TableName;
            object obj = DbHelperOra.GetSingle(strsql);
            if (obj == null)
            {
                //0是老师  1 是学生
                if (type == 0)
                    return 10500;
                else
                    return 1950000;
            }
            else
            {
                return int.Parse(obj.ToString()) + 1;
            }
        }

        //test code
        /*static int Main(string[] args)
        {
            string[] arr = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
            int[] arr2 = new int[] { 7, 3 };
            List<string> ans_list = new List<string>(arr);
            List<int> idx_list = new List<int>(arr2);
            int a = 8;
            var b = get_answer.calindex(a, ans_list, rand_answer.rand_order_index(a, ans_list, idx_list).ans_index);
            return 1;
        }*/
        //统计student_exam_question表格里面的分数更新到student_exam
        public static bool Student_Exam_Score_sum(string student_id, string course_id, int exam_id)
        {
            var sql = "select sum(student_point) t from student_exam_question where student_id =:s_id and course_id =:c_id and exam_id =:e_id ";
            /*
            select sum(student_point)
            from student_exam_question
            where student_id=:s_id and course_id=:c_id and exam_id=:e_id
             */
            List<OracleParameter> p = new List<OracleParameter>();
            p.Add(new OracleParameter(":s_id", student_id));
            p.Add(new OracleParameter(":c_id", course_id));
            p.Add(new OracleParameter(":e_id", exam_id));
            var data = DbHelperOra.Query(sql, p.ToArray()).Tables[0];
            foreach (DataRow item in data.Rows)
            {
                var update_sql = "update student_exam set score =:score where student_id=:s_id and course_id=:c_id and exam_id=:e_id ";
                /*
                UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                 */
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter(":score", int.Parse(item["t"].ToString())));
                parameters.Add(new OracleParameter(":s_id", student_id));
                parameters.Add(new OracleParameter(":c_id", course_id));
                parameters.Add(new OracleParameter(":e_id", exam_id));
                var isok = DbHelperOra.ExecuteSql(update_sql, parameters.ToArray());
                if (isok > 0)
                    return true;
                else
                    return false;
            }
            return true;
        }
    }
    //class rand_answer  编码 对题目的选项进行打乱，并返回打乱后的标准答案与选项list
    public abstract class rand_answer
    {
        //题目信息，可根据前端需求增添，目前为id与题干，注意随之更改前构造函数
        //选项答案编码函数 传入选项list和编码方法 返回经编码过后的选项答案list(index)
        public static UpdateReturn rand_order_index(int order, string stu_answer)
        {
            int lengthFirst = stu_answer.Length;
            if (stu_answer is null || lengthFirst == 0)
            {
                Console.WriteLine("Error");
                var ErrorReturn = new UpdateReturn { status = "Good", rand_index = null };
                return ErrorReturn;
            }

            List<int> standard_index = new List<int>();
            for (int i = 0; i < lengthFirst; i++)
            {
                if (stu_answer[i] != '0')
                {
                    standard_index.Add(i + 1);
                }
            }

            int lengthIndex = standard_index.Count;
            foreach (int index in standard_index)
            {
                if (lengthFirst < index)
                {
                    Console.WriteLine("Error");
                    var ErrorReturn = new UpdateReturn { status = "Good", rand_index = standard_index };
                    return ErrorReturn;
                }
            }
            for (int i = 0; i < lengthIndex; i++)
            {
                switch (order)
                {
                    case 1:
                        //前移
                        standard_index[i] = standard_index[i] == 1 ? lengthFirst : standard_index[i] - 1;
                        break;
                    case 2:
                        //后移
                        standard_index[i] = standard_index[i] == lengthFirst ? 1 : standard_index[i] + 1;
                        break;
                    case 3:
                        //奇偶调换
                        if (standard_index[i] != lengthFirst || standard_index[i] % 2 == 0)
                            standard_index[i] = standard_index[i] % 2 == 0 ? standard_index[i] - 1 : standard_index[i] + 1;
                        break;
                    case 4:
                        //逆序
                        standard_index[i] = 1 + lengthFirst - standard_index[i];
                        break;
                    case 5:
                        //奇逆序
                        if (standard_index[i] % 2 != 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst : lengthFirst + 1;
                            standard_index[i] = s_size - standard_index[i];
                        }
                        break;
                    case 6:
                        //偶逆序
                        if (standard_index[i] % 2 == 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst + 2 : lengthFirst + 1;
                            standard_index[i] = s_size - standard_index[i];
                        }
                        break;
                    case 7:
                        //奇逆序 奇偶换
                        if (standard_index[i] % 2 != 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst : lengthFirst + 1;
                            standard_index[i] = s_size - standard_index[i];
                            if (standard_index[i] == lengthFirst)
                                break;
                            else
                                standard_index[i] = standard_index[i] + 1;
                        }
                        else
                            standard_index[i] = standard_index[i] - 1;
                        break;
                    case 8:
                        //偶逆序 奇偶换
                        if (standard_index[i] % 2 == 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst + 2 : lengthFirst + 1;
                            standard_index[i] = s_size - standard_index[i];
                            standard_index[i] = standard_index[i] - 1;
                        }
                        else
                        {
                            if (standard_index[i] == lengthFirst)
                                break;
                            else
                                standard_index[i] = standard_index[i] + 1;
                        }
                        break;
                }
            }
            for (int i = 0; i < lengthIndex; i++)
            {
                standard_index[i] -= 1;
            }
            foreach (int i in standard_index)
                Console.WriteLine(i);
            var person_authority = new UpdateReturn { status = "Good", rand_index = standard_index };
            return person_authority;
        }
        //选项结果编码函数 传入选项list和编码方法 返回编码过后的所有选项list
        public static List<Option_info> rand_order(int order, List<Option_info> ans_list)
        {
            int lengthFirst = ans_list.Count;
            switch (order)
            {
                case 1:
                    //前移
                    var front = ans_list[0];
                    ans_list.Insert(lengthFirst, front);
                    ans_list.RemoveAt(0);
                    break;
                case 2:
                    //后移
                    front = ans_list[lengthFirst - 1];
                    ans_list.Insert(0, front);
                    ans_list.RemoveAt(lengthFirst);
                    break;
                case 3:
                    //奇偶调换
                    for (int i = 0; i < lengthFirst - 1; i += 2)
                    {
                        var temp = ans_list[i];
                        ans_list[i] = ans_list[i + 1];
                        ans_list[i + 1] = temp;
                    }
                    break;
                case 4:
                    //逆序
                    ans_list.Reverse();
                    break;
                case 5:
                    //奇逆序
                    List<Option_info> singlelist = new List<Option_info>();
                    for (int i = 0; i < lengthFirst; i += 2)
                        singlelist.Add(ans_list[i]);
                    singlelist.Reverse();
                    for (int i = 0; i < lengthFirst; i += 2)
                    {
                        ans_list[i] = singlelist[0];
                        singlelist.RemoveAt(0);
                    }
                    break;
                case 6:
                    //偶逆序
                    List<Option_info> doublelist = new List<Option_info>();
                    for (int i = 1; i < lengthFirst; i += 2)
                        doublelist.Add(ans_list[i]);
                    doublelist.Reverse();
                    for (int i = 1; i < lengthFirst; i += 2)
                    {
                        ans_list[i] = doublelist[0];
                        doublelist.RemoveAt(0);
                    }
                    break;
                case 7:
                    //奇逆序 奇偶换
                    List<Option_info> stranlist = new List<Option_info>();
                    for (int i = 0; i < lengthFirst; i += 2)
                        stranlist.Add(ans_list[i]);
                    stranlist.Reverse();
                    for (int i = 0; i < lengthFirst; i += 2)
                    {
                        ans_list[i] = stranlist[0];
                        stranlist.RemoveAt(0);
                    }
                    for (int i = 0; i < lengthFirst - 1; i += 2)
                    {
                        var temp = ans_list[i];
                        ans_list[i] = ans_list[i + 1];
                        ans_list[i + 1] = temp;
                    }
                    break;
                case 8:
                    //偶逆序 奇偶换
                    List<Option_info> dtranlist = new List<Option_info>();
                    for (int i = 1; i < lengthFirst; i += 2)
                        dtranlist.Add(ans_list[i]);
                    dtranlist.Reverse();
                    for (int i = 1; i < lengthFirst; i += 2)
                    {
                        ans_list[i] = dtranlist[0];
                        dtranlist.RemoveAt(0);
                    }
                    for (int i = 0; i < lengthFirst - 1; i += 2)
                    {
                        var temp = ans_list[i];
                        ans_list[i] = ans_list[i + 1];
                        ans_list[i + 1] = temp;
                    }
                    break;
            }

            for (int j = 0; j < lengthFirst; j++)
                Console.WriteLine(ans_list[j]);

            return ans_list;
        }
    }

    //class get_answer 解码 传入学生的答案 得到对于打乱选项前题目的学生选项(only index)
    public abstract class get_answer
    {
        // 传入option_id 加1处理
        //题目信息，可根据前端需求增添，目前为id与题干，注意随之更改前构造函数
        //选项答案编码函数 传入选项list和编码方法 返回经编码过后的选项答案list(index)

        public static RandOrderIndexReturn calindex(int order, List<Option_info> ans_list, List<int> stu_ans_idx)
        {
            int lengthFirst = ans_list.Count;
            int lengthIndex = stu_ans_idx.Count;

            //stu_ans_index错误处理
            for (int i = 0; i < lengthIndex; i++)
            {
                stu_ans_idx[i] += 1;
            }
            foreach (int index in stu_ans_idx)
            {
                if (lengthFirst < index)
                {
                    Console.WriteLine("Error");
                    var ErrorReturn = new RandOrderIndexReturn { status = "Good", ans_index = stu_ans_idx };
                    return ErrorReturn;
                }
            }
            for (int i = 0; i < lengthIndex; i++)
            {
                switch (order)
                {
                    case 1:
                        //前移
                        stu_ans_idx[i] = stu_ans_idx[i] == lengthFirst ? 1 : stu_ans_idx[i] + 1;
                        break;
                    case 2:
                        //后移
                        stu_ans_idx[i] = stu_ans_idx[i] == 1 ? lengthFirst : stu_ans_idx[i] - 1;
                        break;
                    case 3:
                        //奇偶调换
                        if (stu_ans_idx[i] != lengthFirst || stu_ans_idx[i] % 2 == 0)
                            stu_ans_idx[i] = stu_ans_idx[i] % 2 == 0 ? stu_ans_idx[i] - 1 : stu_ans_idx[i] + 1;
                        break;
                    case 4:
                        //逆序
                        stu_ans_idx[i] = 1 + lengthFirst - stu_ans_idx[i];
                        break;
                    case 5:
                        //奇逆序
                        if (stu_ans_idx[i] % 2 != 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst : lengthFirst + 1;
                            stu_ans_idx[i] = s_size - stu_ans_idx[i];
                        }
                        break;
                    case 6:
                        //偶逆序
                        if (stu_ans_idx[i] % 2 == 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst + 2 : lengthFirst + 1;
                            stu_ans_idx[i] = s_size - stu_ans_idx[i];
                        }
                        break;
                    case 7:
                        //奇逆序 奇偶换
                        if (stu_ans_idx[i] != lengthFirst || stu_ans_idx[i] % 2 == 0)
                            stu_ans_idx[i] = stu_ans_idx[i] % 2 == 0 ? stu_ans_idx[i] - 1 : stu_ans_idx[i] + 1;
                        if (stu_ans_idx[i] % 2 != 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst : lengthFirst + 1;
                            stu_ans_idx[i] = s_size - stu_ans_idx[i];
                        }
                        break;
                    case 8:
                        //偶逆序 奇偶换
                        if (stu_ans_idx[i] != lengthFirst || stu_ans_idx[i] % 2 == 0)
                            stu_ans_idx[i] = stu_ans_idx[i] % 2 == 0 ? stu_ans_idx[i] - 1 : stu_ans_idx[i] + 1;
                        if (stu_ans_idx[i] % 2 == 0)
                        {
                            int s_size = lengthFirst % 2 == 0 ? lengthFirst + 2 : lengthFirst + 1;
                            stu_ans_idx[i] = s_size - stu_ans_idx[i];
                        }
                        break;
                }
            }

            foreach (int i in stu_ans_idx)
                Console.WriteLine(i);
            string ans = "";
            for (int i = 1; i < lengthFirst + 1; i++)
            {
                if (stu_ans_idx.Contains(i))
                {
                    ans += "1";
                }
                else
                {
                    ans += "0";
                }
            }

            var cal_index_list = new RandOrderIndexReturn { status = "Good", ans_index = stu_ans_idx, answer_code = ans };
            return cal_index_list;
        }

    }
}

