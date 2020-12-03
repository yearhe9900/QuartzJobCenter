using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzJobCenter.Web.Controllers
{
    public class GrpcTaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogView()
        {
            return View();
        }
    }
}
