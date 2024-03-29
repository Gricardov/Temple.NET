﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using Temple.Models;
using System.Diagnostics;

namespace Temple.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        private Usuario Login(string usuario, string contrasena) {
            Usuario u = null;
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_LOGIN", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@USUARIO", usuario);
            cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                u = new Usuario();
                u.codigo = reader.GetInt32(0);
                u.nombres = reader.GetString(1);
                u.apPaterno = reader.GetString(2);
                u.apMaterno = reader.GetString(3);
                u.correo = reader.GetString(4);
                u.edad = reader.GetInt32(5);
                u.idGen = reader.GetInt32(6);
                u.telefono = reader.GetString(7);
                u.login = reader.GetString(8);
                // u.clave
                u.idRol = reader.GetInt32(9);
                u.desRol = reader.GetString(10);

            }

            reader.Close();
            con.Close();

            return u;

        }

        // GET: Login
        public ActionResult IniciarSesion()
        {

            if (Session["usuario"] != null)
            {
                return RedirectToAction("Inicio");

            }
            else
            {

                return View();
            }
        }

        [HttpPost] public ActionResult IniciarSesion(string usuario, string contrasena)
        {


            Usuario u = Login(usuario, contrasena);
            if (u != null)
            {
                Session["usuario"] = u;

                if (u.idRol == 2)
                {

                    return RedirectToAction("Inicio", "Alumno");

                }
                else if (u.idRol == 1)
                {

                    return RedirectToAction("Inicio", "Instructor");
                }
                else
                {
                    ViewBag.mensaje = "Ha ocurrido un error";
                    return View();

                }

            }
            else
            {
                ViewBag.mensaje = "Usuario o contraseña incorrectos";
                return View();

            }




        }


    }
}