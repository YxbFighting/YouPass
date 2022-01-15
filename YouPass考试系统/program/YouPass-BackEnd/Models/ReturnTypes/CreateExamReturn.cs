using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouPass_BackEnd.Models.ReturnTypes
{
    public class CreateExamReturn:Return
    {
        public int exam_id { get; init; }
        public string errorinfo { get; init; }
    }
}
