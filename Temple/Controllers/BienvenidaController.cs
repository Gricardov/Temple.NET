using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Temple.Controllers
{
    public class BienvenidaController : Controller
    {
        // GET: Login
        public ActionResult Bienvenida() {
            return View();
        }

        public ActionResult AcercaDe()
        {
            return View();
        }
                
        public ActionResult Descargar()
        {
            return View();
        }
}}