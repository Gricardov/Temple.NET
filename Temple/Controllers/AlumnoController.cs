using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Temple.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace Temple.Controllers
{
    public class AlumnoController : Controller
    {
        // GET: Alumno
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        private List<PreferenciaAprendizaje> ListadoPreferenciaAprendizaje() {

            List<PreferenciaAprendizaje> lista = new List<PreferenciaAprendizaje>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PREFERENCIAS_APRENDIZAJE", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            int codUsu= ((Usuario) Session["usuario"]).codigo;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                PreferenciaAprendizaje p = new PreferenciaAprendizaje();
                p.idCat=reader.GetInt32(0);
                p.desCat = reader.GetString(1);
                p.idSub = reader.GetInt32(2);
                p.desSub = reader.GetString(3);
                lista.Add(p);

            }

            con.Close();
            reader.Close();

            return lista;
        }

        private List<InstructoresRecomendados> ListadoInstructoresRecomendados() {

            // Primero obtengo las preferencias de aprendizaje del alumno
            List<PreferenciaAprendizaje> preferencias = ListadoPreferenciaAprendizaje();

            // Declaro mi listado para las tarjetas de preferencias de instructores
            List<InstructoresRecomendados> listado = new List<InstructoresRecomendados>();

            con.Open();

            foreach (PreferenciaAprendizaje p in preferencias) {

                SqlCommand cmd = new SqlCommand("USP_OBTENER_INSTRUCTORES_RECOMENDADOS_PREFERENCIA", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                int idSub = p.idSub;
                cmd.Parameters.AddWithValue("@IDSUB", idSub);
                SqlDataReader reader = cmd.ExecuteReader();

                InstructoresRecomendados g= new InstructoresRecomendados();
                g.prefApr = p;
                g.instructores = new List<TarjetaInstructor>();

                while (reader.Read()) {
                                       
                    TarjetaInstructor i = new TarjetaInstructor();
                    i.codigo = reader.GetInt32(0);
                    i.nombres = reader.GetString(1);
                    i.apPaterno = reader.GetString(2);
                    i.apMaterno = reader.GetString(3);
                    i.especialidad = reader.GetString(4);
                    i.distancia = reader.GetString(5);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    g.instructores.Add(i);
                    
                }
                listado.Add(g);
                reader.Close();

            }
            con.Close();

            return listado;

        }

        private List<TarjetaInstructor> ListadoInstructoresBusqueda(int idCat = 0, int idSub=0)
        {
                List<TarjetaInstructor> lista = new List<TarjetaInstructor>();
                con.Open();
                
                SqlCommand cmd = new SqlCommand("USP_OBTENER_INSTRUCTORES_BUSQUEDA", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@IDCAT", idCat);
                cmd.Parameters.AddWithValue("@IDSUB", idSub);
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {

                    TarjetaInstructor i = new TarjetaInstructor();
                    i.codigo = reader.GetInt32(0);
                    i.nombres = reader.GetString(1);
                    i.apPaterno = reader.GetString(2);
                    i.apMaterno = reader.GetString(3);
                    i.especialidad = reader.GetString(4);
                    i.distancia = reader.GetString(5);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    lista.Add(i);

                }
                reader.Close();

            
            con.Close();

            return lista;

        }

        public List<Categoria> ListadoCategorias() {

            List<Categoria> lista = new List<Categoria>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_CATEGORIAS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {

                Categoria c = new Categoria();
                c.id = reader.GetInt32(0);
                c.descripcion = reader.GetString(1);

                lista.Add(c);

            }
            reader.Close();
            con.Close();

            return lista;

        }

        public List<Subcategoria> ListadoSubcategorias(int idCat) {

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

        /*[HttpPost]public ActionResult Inicio(int cboCategoria=0, int cboSubcategoria=0) {

            return View();

        }
        */

        public ActionResult Inicio(int cboCategoria = -1, int cboSubcategoria = -1)
        {

            if (cboCategoria == -1) {

                cboCategoria = ListadoCategorias().ElementAt(0).id;

            }

            if (cboSubcategoria == -1) {

                cboSubcategoria = ListadoSubcategorias(cboCategoria).ElementAt(0).id;
            }

            ViewBag.usuario = Session["usuario"];
            ViewBag.recomendados = ListadoInstructoresRecomendados();
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", cboCategoria);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(cboCategoria)), "id", "descripcion", cboSubcategoria);
            return View();

        }
    }
}