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
using System.IO;

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

        public Usuario ActualizarAlumno(Usuario u)
        {
            Usuario res = new Usuario();
            con.Open();

            SqlCommand cmd = new SqlCommand("USP_ACTUALIZAR_USUARIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", u.codigo);
            Debug.WriteLine(u.codigo);
            cmd.Parameters.AddWithValue("@NOMUSU", u.nombres);
            Debug.WriteLine(u.nombres);
            cmd.Parameters.AddWithValue("@APAUSU", u.apPaterno);
            Debug.WriteLine(u.apPaterno);
            cmd.Parameters.AddWithValue("@AMAUSU", u.apMaterno);
            Debug.WriteLine(u.apMaterno);
            cmd.Parameters.AddWithValue("@CORREO", u.correo);
            Debug.WriteLine(u.correo);
            cmd.Parameters.AddWithValue("@EDAD", u.edad);
            Debug.WriteLine(u.edad);
            cmd.Parameters.AddWithValue("@TEL", u.telefono);
            Debug.WriteLine(u.telefono);
            cmd.Parameters.AddWithValue("@CLAUSU", u.clave);
            Debug.WriteLine(u.clave);
            cmd.Parameters.AddWithValue("@SOBREMI", u.sobreMi);
            Debug.WriteLine(u.sobreMi);
            int ok = cmd.ExecuteNonQuery();
            Debug.WriteLine(ok + "");
            if (ok > 0)
            {
                Debug.WriteLine("si");
                res = u;
            }
            else
            {
                Debug.WriteLine("no");
            }
            con.Close();
            return res;

        }


        [HttpPost]
        public ActionResult Configuracion(string nombres, string apPat, string apMat, int edad,
           string correo, string telefono, string clave, string sobreMi,
           HttpPostedFileBase imagen)
        {
            Debug.WriteLine(nombres);
            Debug.WriteLine(apPat);
            Debug.WriteLine(apMat);
            Debug.WriteLine(edad);
            Debug.WriteLine(correo);
            Debug.WriteLine(telefono);
            Debug.WriteLine(clave);
            Debug.WriteLine(sobreMi);

            ViewBag.usuario = Session["usuario"];

            try
            {
                Usuario u = new Usuario();
                u.codigo = ((Usuario)Session["usuario"]).codigo;
                u.nombres = nombres;
                u.apPaterno = apPat;
                u.apMaterno = apMat;
                u.edad = edad;
                u.correo = correo;
                u.telefono = telefono; ;
                u.clave = clave;
                u.sobreMi = sobreMi;

                Usuario res = ActualizarAlumno(u);
                Debug.WriteLine(res.codigo);
                Debug.WriteLine(res.nombres);
                if (res != null)
                {
                    if (imagen != null)
                    {
                        var originalFilename = Path.GetFileName(imagen.FileName);
                        string fileId = Guid.NewGuid().ToString().Replace("-", "");
                        string userId = res.codigo.ToString(); // Function to get user id based on your schema

                        var path = Path.Combine(Server.MapPath("~/imagenes/perfiles/"), userId/*, fileId*/);
                        imagen.SaveAs(path + Path.GetExtension(imagen.FileName));
                    }


                    ViewBag.mensaje = "¡Actualización exitosa!";
                    Session["usuario"] = null;
                    return RedirectToAction("Bienvenida", "Bienvenida");
                }
                else
                {

                    ViewBag.mensaje = "Error al actualizar en la base de datos";
                    return View();
                }
            }
            catch (Exception e)
            {

                ViewBag.mensaje = "Error al actualizar. Verifica que has llenado todos los datos correctamente, incluida la foto. Además, verifica que hayas especificado cursos diferentes";
                return View();

            }

        }

        public ActionResult Configuracion()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PERFIL_ALUMNO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", (((Usuario)Session["usuario"]).codigo));
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ((Usuario)Session["usuario"]).sobreMi = reader.GetString(4);
            }

            reader.Close();
            con.Close();

            ViewBag.usuario = Session["usuario"];
            return View();
        }

        public ActionResult AcercaDe()
        {
            ViewBag.usuario = Session["usuario"];
            return View();
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
                    i.ubicacion = ObtenerUbicacion(con, i.codigo);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    int idUsuario = ((Usuario) Session["usuario"]).codigo;

                    i.distancia=obtenerDistanciaString(ObtenerUbicacion(con, idUsuario),i.ubicacion);
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
                    i.ubicacion = ObtenerUbicacion(con,i.codigo);
                    i.numResenas = reader.GetInt32(6);
                    i.calificacion = reader.GetInt32(7);
                    i.verificado = reader.GetBoolean(8);

                    int idUsuario = ((Usuario)Session["usuario"]).codigo;

                i.distancia = obtenerDistanciaString(ObtenerUbicacion(con,idUsuario), i.ubicacion);

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


        private Perfil ObtenerPerfilInstructor(int codUsu)
        {

            Perfil p = new Perfil();
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
                p.horarios = ListadoHorariosInstructor(con, p.codigo);

                int idUsuario = ((Usuario)Session["usuario"]).codigo;

                p.distancia = obtenerDistanciaString(ObtenerUbicacion(con,idUsuario), u);

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
                r.codUsuRemitente = reader.GetInt32(1);
                r.idPerfilRemitente = reader.GetInt32(2);
                r.nombreRemitente = reader.GetString(3);
                r.apPaternoRemitente = reader.GetString(4);
                r.apMaternoRemitente = reader.GetString(5);
                r.idPerfilDestinatario = reader.GetInt32(6);
                r.contenido = reader.GetString(7);
                r.fechaHora = reader.GetDateTime(8);
                r.calificacion = reader.GetInt32(9);
                r.editado = reader.GetBoolean(10);
                lista.Add(r);

            }
            reader.Close();

            return lista;

        }

        public List<Evento> ListadoHorariosInstructor(SqlConnection con, int codUsu)
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

        public List<Evento> ListadoMisCitas(SqlConnection con, int codUsu)
        {
            List<Evento> lista = new List<Evento>();
            SqlCommand cmd = new SqlCommand("SELECT INICIO, FIN FROM TB_TRANSACCIONES WHERE CODALUMNO=@CODUSU", con);
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Evento e = new Evento();
                e.inicio = reader.GetDateTime(0);
                e.fin = reader.GetDateTime(1);
                lista.Add(e);
            }
            reader.Close();

            return lista;

        }

        public List<Transaccion> ListadoTransacciones(SqlConnection con, int codUsu)
        {
            List<Transaccion> lista = new List<Transaccion>();
            SqlCommand cmd = new SqlCommand("USP_TRANSACCIONES_ALUMNO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Transaccion t = new Transaccion();
                t.codTran = reader.GetInt32(0);
                t.codInstr = reader.GetInt32(1);
                t.nombresInstr = reader.GetString(2);
                t.apPatInstr = reader.GetString(3);
                t.apMatInstr = reader.GetString(4);
                t.desCat = reader.GetString(5);
                t.desSub = reader.GetString(6);
                t.desMod = reader.GetString(7);
                t.precioHora = reader.GetDecimal(8);
                t.inicio = reader.GetDateTime(9);
                t.fin = reader.GetDateTime(10);
                t.total = reader.GetDecimal(11);
                t.fechaHora = reader.GetDateTime(12);
                lista.Add(t);
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
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODALU", ((Usuario)Session["usuario"]).codigo);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Anuncio a = new Anuncio();
                a.id = reader.GetInt32(0);
                a.codInstr = reader.GetInt32(1);
                a.nomInstr = reader.GetString(2);
                a.apPatInstr = reader.GetString(3);
                a.apMatInstr = reader.GetString(4);
                a.titulo = reader.GetString(5);
                a.contenido = reader.GetString(6);
                a.fechaHora = reader.GetDateTime(7);
                lista.Add(a);
            }

            con.Close();
            reader.Close();


            return lista;
        }

        private Perfil ObtenerMiPerfil()
        {

            Perfil p = new Perfil();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PERFIL_ALUMNO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", (((Usuario)Session["usuario"]).codigo));
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                p.codigo = reader.GetInt32(0);
                p.nombres = reader.GetString(1);
                p.apPaterno = reader.GetString(2);
                p.apMaterno = reader.GetString(3);
                p.sobreMi = reader.GetString(4);
                p.buscando = reader.GetString(5);
                p.verificado = reader.GetBoolean(6);
                p.conectado = reader.GetBoolean(7);

                Ubicacion u = new Ubicacion();
                u.latitud = reader.GetDecimal(8);
                u.longitud = reader.GetDecimal(9);
                p.idPerfil = reader.GetInt32(10);
                p.ubicacion = u;
                p.transacciones = ListadoTransacciones(con, p.codigo);
                p.citas = ListadoMisCitas(con, p.codigo);

                int idUsuario = ((Usuario)Session["usuario"]).codigo;

            }

            con.Close();
            reader.Close();

            return p;
        }

        public Boolean puedeResenar(int codDestin) {
            Boolean puede = false;
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT DBO.UFN_PUEDE_RESENAR (@CODREMIT, @CODDESTIN)", con);

            int codUsuario = ((Usuario)Session["usuario"]).codigo;


            cmd.Parameters.AddWithValue("@CODREMIT", codUsuario);
            cmd.Parameters.AddWithValue("@CODDESTIN", codDestin);
            puede = (Boolean)cmd.ExecuteScalar();
            con.Close();
            return puede;

        }

        public string InsertarResena(int codDestin, string contenido, int calificacion) {
            string respuesta = "";
            con.Open();
            int codUsuario = ((Usuario)Session["usuario"]).codigo;
            SqlCommand cmd = new SqlCommand("USP_INSERTAR_RESENA", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODREMIT", codUsuario);
            cmd.Parameters.AddWithValue("@CODDESTIN", codDestin);
            cmd.Parameters.AddWithValue("@CONTENIDO", contenido);
            cmd.Parameters.AddWithValue("@CALIFICACION", calificacion);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {

                respuesta = reader.GetString(0);
            }
            reader.Close();
            con.Close();
            return respuesta;

        }

        public string ActualizarResena(int idRes, string resena, int calificacion)
        {
            string respuesta = "";
            con.Open();
            int codUsuario = ((Usuario)Session["usuario"]).codigo;
            SqlCommand cmd = new SqlCommand("USP_ACTUALIZAR_RESENA", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODREMIT", codUsuario);
            cmd.Parameters.AddWithValue("@IDRES", idRes);
            cmd.Parameters.AddWithValue("@CONTENIDO", resena);
            cmd.Parameters.AddWithValue("@CALIFICACION", calificacion);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                respuesta = reader.GetString(0);
            }
            reader.Close();
            con.Close();
            return respuesta;

        }

        // Action Results

        //vista: Cuadrícula=0, Mapa=1
        public ActionResult Inicio(int cboCategoria = -1, int cboSubcategoria = -1, string vista = "0")
        {
            // Después de realizar la transacción
            if (Session["mensaje"] != null) {

                ViewBag.mensaje = Session["mensaje"].ToString();
                Session["mensaje"] = null;

            }
            
            if (cboCategoria == -1) {

                cboCategoria = (ListadoCategorias().Count()>0?ListadoCategorias().ElementAt(0).id:0);

            }

            if (cboSubcategoria == -1) {

                cboSubcategoria = (ListadoSubcategorias(cboCategoria).Count()>0?ListadoSubcategorias(cboCategoria).ElementAt(0).id:0);
            }
            Usuario u = (Usuario) Session["usuario"];

            ViewBag.usuario = u;
            ViewBag.recomendados = ListadoInstructoresRecomendados();
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", cboCategoria);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(cboCategoria)), "id", "descripcion", cboSubcategoria);
            ViewBag.vista = vista;
            ViewBag.ubicacionGuardada = ObtenerUbicacion(null, u.codigo);
            ViewBag.busqueda = ListadoInstructoresBusqueda(cboCategoria, cboSubcategoria);
            
            return View();

        }


        public ActionResult PerfilInstructor(int codUsu)
        {
            Perfil perfil = ObtenerPerfilInstructor(codUsu);
            ViewBag.usuario = Session["usuario"];
            ViewBag.titulo = perfil.nombres +" "+ perfil.apPaterno +" "+perfil.apMaterno;
            ViewBag.resenas = perfil.reseñas;
            ViewBag.cursos = perfil.cursos;
            ViewBag.horarios = perfil.horarios;
            ViewBag.puedeResenar=puedeResenar(codUsu);

            
            return View(perfil);

        }

        public ActionResult RegistroResena(string resena, int calificacion, int codInstr) {

            if (resena != "" && calificacion >= 1 && calificacion <= 5)
            {
                Session["mensaje"] = InsertarResena(codInstr, resena, calificacion);

            }
            else {

                Session["mensaje"] = "Datos no válidos has insertado";
            }
            return RedirectToAction("Inicio");


        }

        public ActionResult ActualizacionResena(int idRes, string resena, int calificacion) {
            Debug.WriteLine(idRes + " " + resena + " " + calificacion);
            if (resena != "" && calificacion >= 1 && calificacion <= 5 && idRes!=-1)
            {
                Session["mensaje"] = ActualizarResena(idRes, resena, calificacion);

            }
            else
            {

                Session["mensaje"] = "Datos no válidos has insertado";
            }
            return RedirectToAction("Inicio");

        }

        public ActionResult AnunciosInstructores() {
            ViewBag.usuario = Session["usuario"];
            return View();

        }

        public ActionResult MiPerfil()
        {
            Perfil perfil =ObtenerMiPerfil();
            ViewBag.usuario = Session["usuario"];
            ViewBag.titulo = perfil.nombres + " " + perfil.apPaterno + " " + perfil.apMaterno;
            //ViewBag.citas = perfil.citas;
            ViewBag.transacciones = perfil.transacciones;
            return View(perfil);

        }

        public ActionResult MisCitas() {
            ViewBag.usuario = Session["usuario"];
            ViewBag.citas = ObtenerMiPerfil().citas;
            return View();

        }

        public ActionResult MiUbicacion() {
            Usuario u = (Usuario) Session["usuario"];
            ViewBag.usuario = u;
            ViewBag.ubicacion = ObtenerUbicacion(null, u.codigo);
            return View();

        }
        [HttpPost]
        public ActionResult MiUbicacion(decimal latitud, decimal longitud)
        {
            try
            {
                // Actualiza la ubicación
                int rs = ActualizarUbicacion(latitud, longitud);

                if (rs != -1)
                {

                    ViewBag.mensaje = "Actualizado correctamente";

                }
                else


                    ViewBag.mensaje = "Error al actualizar";


                

            }
            catch (Exception e) {

                ViewBag.mensaje = "Coordenadas inválidas";
                this.con.Close();

            }
            // Actualiza la ubicación
            Usuario u = (Usuario)Session["usuario"];
            ViewBag.usuario = u;
            ViewBag.ubicacion = ObtenerUbicacion(null, u.codigo);
            return View();

        }

        public ActionResult UbicacionInstructor(int codInstr) {
            Usuario u = (Usuario)Session["usuario"];
            ViewBag.usuario = u;
            ViewBag.ubicacion = ObtenerUbicacion(null, u.codigo);
            ViewBag.ubicacionInstructor = ObtenerUbicacion(null, codInstr);
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

        [HttpPost]
        public JsonResult obtenerAnuncios()
        {
            string json = new JavaScriptSerializer().Serialize((ListadoAnuncios()));
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

        private int ActualizarUbicacion(decimal latitud, decimal longitud)
        {
            
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_ACTUALIZAR_UBICACION_USUARIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            int idUsuario = ((Usuario)Session["usuario"]).codigo;
            cmd.Parameters.AddWithValue("@CODUSU", idUsuario);
            cmd.Parameters.AddWithValue("@LATITUD", latitud);
            cmd.Parameters.AddWithValue("@LONGITUD", longitud);
            int rs = cmd.ExecuteNonQuery();
            con.Close();            
            return rs;

        }

        private Ubicacion ObtenerUbicacion(SqlConnection con,int idUsuarioObjetivo)
        {

            if (con == null) {

                this.con.Open();

            }

            Ubicacion uObjetivo = new Ubicacion();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_UBICACION_USUARIO", this.con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", idUsuarioObjetivo);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                uObjetivo.latitud = reader.GetDecimal(0);
                uObjetivo.longitud = reader.GetDecimal(1);

            }
            if (con == null)
            {
                this.con.Close();

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