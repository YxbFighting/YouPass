using System;

namespace YouPass_BackEnd.Models
{
    /******************************************************************
    * 所有return类都必须是Return类的子类，即必须继承Return
    ******************************************************************/
    public class UploadQuestionReturn : Return
    {
        public int success_num { get; init; }
    }
}