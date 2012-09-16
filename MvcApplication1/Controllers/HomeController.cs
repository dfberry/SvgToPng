using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using SvgToPngLibrary;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }

        public JsonResult PieChart()
        {

            List<Series> list = new List<Series>();

            list.Add( new Series { Name ="Alpha", Value = 4});
            list.Add( new Series { Name ="Beta", Value = 15});
            list.Add(new Series { Name = "Gamma", Value = 7 });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SvgToPng(string svgpie)
        {
            SvgToPng svgToPng = new SvgToPng(null);

            // convert svg data to png file
            string gridPngFileLocation = svgToPng.Save(HttpUtility.UrlDecode(svgpie), "sample", "pie", Server.MapPath("~/Content/Images"));

            // convert file path back to unc
            string uncPath = gridPngFileLocation.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], "/").Replace(@"\", "/");

            return Json(uncPath);
        }
    }
}
