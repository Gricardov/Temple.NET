using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Temple.Controllers
{
    public class RegistroController : Controller
    {
        // GET: Registro
        public ActionResult RegistroAlumno()
        {
            ViewBag.usuario = Session["usuario"];

            return View();
        }

        [HttpPost]
        public ActionResult RegistroAlumno(int idPref) {

            return View();

        }

    }
}