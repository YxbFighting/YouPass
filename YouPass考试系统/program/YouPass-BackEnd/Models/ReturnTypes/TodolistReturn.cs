using System;
using System.Collections.Generic;

namespace YouPass_BackEnd.Models
{

    public class info_todo
    {

        public int id { get; init; }
        public String description { get; init; }
    }



    public class TodolistGetReturn : Return
    {
        public List<info_todo> todolist { get; init; }
    }
    public class TodolistPutReturn : Return
    {
        public int new_id { get; init; }
    }
}