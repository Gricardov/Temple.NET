using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Temple.Models;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Temple.Controllers
{
    public class RegistroController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        // GET: Registro
        public ActionResult RegistroAlumno()
        {
            ViewBag.usuario = Session["usuario"];
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", 0);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(ListadoCategorias().ElementAt(0).id)), "id", "descripcion", 0);

            return View();
        }

        [HttpPost]
        public ActionResult RegistroAlumno(int idPref) {

            return View();

        }

        public List<Categoria> ListadoCategorias()
        {

            List<Categoria> lista = new List<Categoria>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_CATEGORIAS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                Categoria c = new Categoria();
                c.id = reader.GetInt32(0);
                c.descripcion = reader.GetString(1);

                lista.Add(c);

            }
            reader.Close();
            con.Close();

            return lista;

        }

        public List<Subcategoria> ListadoSubcategorias(int idCat)
        {

            List<Subcategoria> lista = new List<Subcategoria>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_SUBCATEGORIAS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IDCAT", idCat);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                Subcategoria c = new Subcategoria();
                c.id = reader.GetInt32(0);
                c.descripcion = reader.GetString(1);

                lista.Add(c);

            }
            reader.Close();
            con.Close();

            return lista;

        }

    }
}