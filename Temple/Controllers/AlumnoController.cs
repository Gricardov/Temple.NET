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
using System.Web.Script.Serialization;

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

        private string obtenerDistanciaString(TarjetaInstructor ti) {

            int codUsuarioObjetivo = ti.codigo;

            //con.Open();
            Ubicacion uOrigen = new Ubicacion();
            Ubicacion uObjetivo = new Ubicacion();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_UBICACIONES_USUARIOS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            int codUsu = ((Usuario)Session["usuario"]).codigo;
            cmd.Parameters.AddWithValue("@CODUSUORIG", codUsu);
            cmd.Parameters.AddWithValue("@CODUSUOBJ", codUsuarioObjetivo);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                uOrigen.latitud = reader.GetDecimal(0);
                uOrigen.longitud = reader.GetDecimal(1);
                uObjetivo.latitud = reader.GetDecimal(2);
                uObjetivo.longitud = reader.GetDecimal(3);

            }

            ti.ubicacion = uObjetivo;

            //reader.Close();
            //con.Close();
            double distanciaKM = obtenerDistanciaKM((double)uOrigen.latitud, (double)uOrigen.longitud, (double)uObjetivo.latitud, (double)uObjetivo.longitud);

            // Para saber en qué formato lo voy a devolver, en kilómetros, metros o millas
            string respuesta = "";
            double convertido = 0;
            if (distanciaKM >= 1.60934) {
                convertido = Math.Round(distanciaKM * 1.60934);
                respuesta = convertido + " millas";
            }
            else if (distanciaKM >= 1) {
                respuesta = Math.Round(distanciaKM) + " kilómetros";

            }
            else {

                convertido = Math.Round(distanciaKM * 1000);
                respuesta = convertido + " metros";

            }

            return respuesta;

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

                while (reader.Read()) {
                                       
                    TarjetaInstructor i = new TarjetaInstructor();
                    i.codigo = reader.GetInt32(0);
                    i.nombres = reader.GetString(1);
                    i.apPaterno = reader.GetString(2);
                    i.apMaterno = reader.GetString(3);
                    i.especialidad = reader.GetString(4);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);
                    
                    i.distancia="A "+obtenerDistanciaString(i)+" de distancia";
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
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);
                    i.distancia = "A " + obtenerDistanciaString(i) + " de distancia";

                lista.Add(i);

                }
                reader.Close();

            
            con.Close();

            return lista;

        }

        //
        private List<TarjetaInstructor> ListadoInstructores()
        {
            List<TarjetaInstructor> lista = new List<TarjetaInstructor>();
            con.Open();

            SqlCommand cmd = new SqlCommand("USP_OBTENER_INSTRUCTORES", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

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
        //

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
        //vista: Cuadrícula=0, Mapa=1
        public ActionResult Inicio(int cboCategoria = -1, int cboSubcategoria = -1, string vista="0")
        {

            if (cboCategoria == -1) {

                cboCategoria = (ListadoCategorias().Count()>0?ListadoCategorias().ElementAt(0).id:0);

            }

            if (cboSubcategoria == -1) {

                cboSubcategoria = (ListadoSubcategorias(cboCategoria).Count()>0?ListadoSubcategorias(cboCategoria).ElementAt(0).id:0);
            }
    
            Debug.WriteLine(cboCategoria+" "+cboSubcategoria);

            ViewBag.usuario = Session["usuario"];
            ViewBag.recomendados = ListadoInstructoresRecomendados();
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", cboCategoria);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(cboCategoria)), "id", "descripcion", cboSubcategoria);
            ViewBag.vista = vista;
            //ViewBag.busqueda = ListadoInstructores();
            ViewBag.busqueda = ListadoInstructoresBusqueda(cboCategoria, cboSubcategoria);
            
            return View();

        }

        [HttpPost]
        public JsonResult ObtenerSubcategorias(int categoria)
        {
            string json = new JavaScriptSerializer().Serialize((ListadoSubcategorias(categoria)));
            return Json(json);
        }

        private double obtenerDistanciaKM(double lat1, double long1, double lat2, double long2)
        {
            int tierra = 6371;

            //Point 1 cords
            double lat1Rad = (Math.PI / 180) * lat1;
            double long1Rad = (Math.PI / 180) * long1;

            double lat2Rad = (Math.PI / 180) * lat2;
            double long2Rad = (Math.PI / 180) * long2;

            double dlong = long2Rad - long1Rad;
            double dlat = lat2Rad - lat1Rad;

            double senlat = Math.Sin(dlat / 2);
            double senlong = Math.Sin(dlong / 2);

            double a = (senlat *senlat)+Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * (senlong*senlong);
                       
            double c= 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = Math.Round(tierra *c);
            return d;
        }

    }
}