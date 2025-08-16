using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Fintrak Credit360 API";

            return View();
        }
    }
}
