using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlTableHelper;

namespace AspNetMvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var datas = new[] { new { Name = "ITWeiHan", Age = "25",Country = "Taiwan" } };
            ViewBag.Table = datas.ToHtmlTable();
            return View();
        }
    }
}