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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace YouPass_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserImageController : ControllerBase
    {
        /******************************************************************
        * 测试用
         ******************************************************************/
        [HttpGet]
        // public IActionResult Get(string id)
        public IActionResult Get()
        {
            string id;
            if (HttpContext.Session.GetString("id") == null)
            {
                id = "default";
            }
            else
            {
                id = HttpContext.Session.GetString("id");
            }
            Byte[] b;
            string workingDirectory = Environment.CurrentDirectory;
            string imagePath = workingDirectory + @"/Img/UserImg/" + id + ".jpg";
            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = workingDirectory + @"/Img/UserImg/default.jpg";
            }
            b = System.IO.File.ReadAllBytes(imagePath);
            return File(b, "image/jpeg");
        }

        [HttpPost]
        public ActionResult<Return> Post([FromForm] UserImageReceive objectFile)
        {
            if (HttpContext.Session.GetString("id") == null)
            {
                return new Return { status = "Bad" };
            }
            string id = HttpContext.Session.GetString("id");
            if (objectFile.files == null)
            {
                Return r = new Return { status = "Bad" };
                return r;
            }
            string workingDirectory = Environment.CurrentDirectory;
            string imagePath = workingDirectory + @"/Img/UserImg/" + id + ".jpg";
            using (FileStream fileStream = System.IO.File.Create(imagePath))
            {
                objectFile.files.CopyTo(fileStream);
                fileStream.Flush();
            }
            Return sr = new Return { status = "Good" };
            return sr;
        }

        /*---------------------------------------------------------------
         * 工具函数
         ---------------------------------------------------------------*/
    }
}