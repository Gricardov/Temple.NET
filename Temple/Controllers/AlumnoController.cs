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

        private List<InstructoresRecomendados> ListadoInstructoresRecomendados() {

            // Primero obtengo las preferencias de aprendizaje del alumno
            List<PreferenciaAprendizaje> preferencias = ListadoPreferenciaAprendizaje();

            // Declaro mi listado para las tarjetas de preferencias de instructores
            List<InstructoresRecomendados> recomendados = new List<InstructoresRecomendados>();

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
                    i.ubicacion = obtenerUbicacion(con, i.codigo);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    int idUsuario = ((Usuario) Session["usuario"]).codigo;

                    i.distancia=obtenerDistanciaString(obtenerUbicacion(con, idUsuario),i.ubicacion);
                    g.instructores.Add(i);
                    
                }
                recomendados.Add(g);
                reader.Close();

            }
            con.Close();

            return recomendados;

        }

        private List<TarjetaInstructor> ListadoInstructoresBusqueda(int idCat = 0, int idSub=0)
        {
                List<TarjetaInstructor> lista = new List<TarjetaInstructor>();
                con.Open();
                
                SqlCommand cmd = new SqlCommand("USP_OBTENER_INSTRUCTORES_BUSQUEDA", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                Debug.WriteLine("idCat " + idCat + " idSub " + idSub);
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
                    i.ubicacion = obtenerUbicacion(con,i.codigo);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    int idUsuario = ((Usuario)Session["usuario"]).codigo;

                i.distancia = obtenerDistanciaString(obtenerUbicacion(con,idUsuario), i.ubicacion);

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


        private PerfilInstructor ObtenerPerfilInstructor(int codUsu)
        {

            PerfilInstructor p = new PerfilInstructor();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PERFIL_INSTRUCTOR", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                p.codigo = reader.GetInt32(0);
                p.nombres = reader.GetString(1);
                p.apPaterno = reader.GetString(2);
                p.apMaterno = reader.GetString(3);
                p.especialidad = reader.GetString(4);
                p.sobreMi = reader.GetString(5);
                p.cv = reader.GetString(6);
                p.calificacion = reader.GetInt32(7);
                p.verificado = reader.GetBoolean(8);
                p.conectado = reader.GetBoolean(9);

                Ubicacion u = new Ubicacion();
                u.latitud = reader.GetDecimal(10);
                u.longitud = reader.GetDecimal(11);
                p.idPerfil = reader.GetInt32(12);
                p.ubicacion = u;

                p.reseñas = ListadoResenas(con,p.idPerfil);
                p.cursos = ListadoPreferenciaEnsenanza(con, p.codigo);
                p.horarios = ListadoHorarios(con, p.codigo);

                int idUsuario = ((Usuario)Session["usuario"]).codigo;

                p.distancia = obtenerDistanciaString(obtenerUbicacion(con,idUsuario), u);

            }

            con.Close();
            reader.Close();

            return p;
        }

        public List<Reseña> ListadoResenas(SqlConnection con,int idPer) {
            List<Reseña> lista = new List<Reseña>();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_RESENAS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IDPERFIL", idPer);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                Reseña r = new Reseña();
                r.id = reader.GetInt32(0);
                r.idPerfilRemitente = reader.GetInt32(1);
                r.nombreRemitente = reader.GetString(2);
                r.apPaternoRemitente = reader.GetString(3);
                r.apMaternoRemitente = reader.GetString(4);
                r.idPerfilDestinatario = reader.GetInt32(5);
                r.contenido = reader.GetString(6);
                r.fechaHora = reader.GetDateTime(7);
                r.calificacion = reader.GetInt32(8);
                lista.Add(r);

            }
            reader.Close();

            return lista;

        }

        public List<Evento> ListadoHorarios(SqlConnection con, int codUsu)
        {
            List<Evento> lista = new List<Evento>();
            SqlCommand cmd = new SqlCommand("SELECT*FROM TB_HORARIO_INSTRUCTOR WHERE COD_USU=@CODUSU", con);
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Evento e = new Evento();
                e.id = reader.GetInt32(0);
                e.inicio = reader.GetDateTime(2);
                e.fin = reader.GetDateTime(3);
                lista.Add(e);
            }
            reader.Close();

            return lista;

        }

        public List<Modalidad> ListadoModalidadesEnsenanza(SqlConnection con, int idCat, int idSub, int codUsu) {
            List<Modalidad> lista = new List<Modalidad>();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_MODALIDADES_ENSENANZA", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            cmd.Parameters.AddWithValue("@IDCAT", idCat);
            cmd.Parameters.AddWithValue("@IDSUB", idSub);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {

                Modalidad m = new Modalidad();
                m.id = reader.GetInt32(0);
                m.descripcion = reader.GetString(1);
                m.precioHora = reader.GetDecimal(2);

                lista.Add(m);

            }

            reader.Close();

            return lista;

        }

        public List<PreferenciaEnsenanza> ListadoPreferenciaEnsenanza(SqlConnection con, int codUsu)
        {            
            List<PreferenciaEnsenanza> lista = new List<PreferenciaEnsenanza>();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PREFERENCIAS_ENSENANZA", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                PreferenciaEnsenanza p = new PreferenciaEnsenanza();
                p.idCat = reader.GetInt32(0);
                p.desCat = reader.GetString(1);
                p.idSub = reader.GetInt32(2);
                p.desSub = reader.GetString(3);
                p.descripcion = reader.GetString(4);
                p.silabo = reader.GetString(5);
                p.modalidades = ListadoModalidadesEnsenanza(con, p.idCat, p.idSub, codUsu);
                lista.Add(p);

            }
            reader.Close();

            return lista;

        }

        private List<Anuncio> ListadoAnuncios()
        {
            List<Anuncio> lista = new List<Anuncio>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_ANUNCIOS_POR_ALUMNO", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Anuncio a = new Anuncio();
                a.id = reader.GetInt32(0);
                a.codInstr = reader.GetInt32(1);
                a.titulo = reader.GetString(2);
                a.contenido = reader.GetString(3);
                a.fechaHora = reader.GetDateTime(4);
                lista.Add(a);
            }

            con.Close();
            reader.Close();


            return lista;
        }

        // Action Results

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


        public ActionResult PerfilInstructor(int codUsu)
        {
           
            PerfilInstructor perfil = ObtenerPerfilInstructor(codUsu);
            ViewBag.usuario = Session["usuario"];
            ViewBag.titulo = perfil.nombres +" "+ perfil.apPaterno +" "+perfil.apMaterno;
            ViewBag.resenas = perfil.reseñas;
            ViewBag.cursos = perfil.cursos;
            ViewBag.horarios = perfil.horarios;
            return View(perfil);

        }

        public ActionResult AnunciosInstructores() {
            ViewBag.usuario = Session["usuario"];
            return View();

        }

        public ActionResult CerrarSesion() {
            Session["usuario"] = null;
            return RedirectToAction("Bienvenida","Bienvenida");

        }

        // JSON Result y funciones

        [HttpPost]
        public JsonResult obtenerSubcategorias(int categoria)
        {
            string json = new JavaScriptSerializer().Serialize((ListadoSubcategorias(categoria)));
            return Json(json);
        }

        private string obtenerDistanciaString(Ubicacion origen, Ubicacion obj) {
            
            double distanciaKM = obtenerDistanciaKM(origen, obj);

            // Para saber en qué formato lo voy a devolver, en kilómetros, metros o millas
            string respuesta = "";
            double convertido = 0;
            if (distanciaKM >= 1.60934)
            {
                convertido = Math.Round(distanciaKM * 1.60934);
                respuesta = convertido + " millas";
            }
            else if (distanciaKM >= 1)
            {
                respuesta = Math.Round(distanciaKM) + " kilómetros";

            }
            else
            {

                convertido = Math.Round(distanciaKM * 1000);
                respuesta = convertido + " metros";

            }

            return respuesta;

        }

        private Ubicacion obtenerUbicacion(SqlConnection con,int idUsuarioObjetivo)
        {
            Ubicacion uObjetivo = new Ubicacion();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_UBICACION_USUARIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", idUsuarioObjetivo);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                uObjetivo.latitud = reader.GetDecimal(0);
                uObjetivo.longitud = reader.GetDecimal(1);

            }

            return uObjetivo;

        }

        private double obtenerDistanciaKM (Ubicacion origen, Ubicacion obj)
        {

        double lat1 =(double)origen.latitud;
        double long1 =(double)origen.longitud;

        double lat2 =(double)obj.latitud;
        double long2 = (double)obj.longitud;

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