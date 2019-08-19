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
    public class InstructorController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        public ActionResult Inicio()
        {
            ViewBag.usuario = Session["usuario"];

            return View();


        }

        public ActionResult MiPerfil()
        {
            Perfil perfil = ObtenerMiPerfil();
            ViewBag.usuario = Session["usuario"];
            ViewBag.titulo = perfil.nombres + " " + perfil.apPaterno + " " + perfil.apMaterno;
            ViewBag.resenas = perfil.reseñas;
            ViewBag.cursos = perfil.cursos;
            ViewBag.horarios = perfil.horarios;
            ViewBag.citas = perfil.citas;
            ViewBag.transacciones = perfil.transacciones;
            return View(perfil);


        }

        public ActionResult MisHorarios()
        {
            ViewBag.usuario = Session["usuario"];
            Perfil perfil = ObtenerMiPerfil();

            ViewBag.horarios = perfil.horarios;
            ViewBag.citas = perfil.citas;
            return View();

        }

        public ActionResult MiUbicacion()
        {
            Usuario u = (Usuario)Session["usuario"];
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
            catch (Exception e)
            {

                ViewBag.mensaje = "Coordenadas inválidas";
                this.con.Close();

            }
            // Actualiza la ubicación
            Usuario u = (Usuario)Session["usuario"];
            ViewBag.usuario = u;
            ViewBag.ubicacion = ObtenerUbicacion(null, u.codigo);
            return View();

        }

        public ActionResult Configuracion()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PERFIL_INSTRUCTOR", con);
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

                Usuario res = ActualizarInstructor(u);
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

        public Usuario ActualizarInstructor(Usuario u)
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

        public ActionResult AcercaDe()
        {
            ViewBag.usuario = Session["usuario"];
            return View();


        }
                
        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Bienvenida", "Bienvenida");

        }

        private int ActualizarUbicacion(decimal latitud, decimal longitud) {

            con.Open();
            SqlCommand cmd = new SqlCommand("USP_ACTUALIZAR_UBICACION_USUARIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", ((Usuario)Session["usuario"]).codigo);
            cmd.Parameters.AddWithValue("@LATITUD", latitud);
            cmd.Parameters.AddWithValue("@LONGITUD", longitud);
            int rs = cmd.ExecuteNonQuery();
            con.Close();
            return rs;

        }

        private Ubicacion ObtenerUbicacion(SqlConnection con, int idUsuarioObjetivo)
        {

            if (con == null)
            {

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

        private Perfil ObtenerMiPerfil()
        {

            Perfil p = new Perfil();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PERFIL_INSTRUCTOR", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", (((Usuario)Session["usuario"]).codigo));
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
                p.transacciones = ListadoTransacciones(con, p.codigo);
                p.reseñas = ListadoResenas(con, p.idPerfil);
                p.cursos = ListadoPreferenciaEnsenanza(con, p.codigo);
                p.horarios = ListadoHorarios(con, p.codigo);
                p.citas = ListadoMisCitas(con, ((Usuario)Session["usuario"]).codigo);
                int idUsuario = ((Usuario)Session["usuario"]).codigo;

            }

            con.Close();
            reader.Close();

            return p;
        }

        public List<Transaccion> ListadoTransacciones(SqlConnection con, int codUsu)
        {
            List<Transaccion> lista = new List<Transaccion>();
            SqlCommand cmd = new SqlCommand("USP_TRANSACCIONES_INSTRUCTOR", con);
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

        public List<Evento> ListadoMisCitas(SqlConnection con, int codUsu)
        {
            if (con == null)
            {

                this.con.Open();
                //SqlConnection conAux= new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
            }
            List<Evento> lista = new List<Evento>();
            SqlCommand cmd = new SqlCommand("SELECT INICIO, FIN FROM TB_TRANSACCIONES WHERE CODINSTR=@CODUSU", this.con);
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
            if (con == null)
            {

                this.con.Close();

            }
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

        public List<Reseña> ListadoResenas(SqlConnection con, int idPer)
        {
            List<Reseña> lista = new List<Reseña>();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_RESENAS", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IDPERFIL", idPer);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
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

        public List<Evento> ListadoHorarios(SqlConnection con, int codUsu)
        {

            if (con == null)
            {

                this.con.Open();

            }

            List<Evento> lista = new List<Evento>();
            SqlCommand cmd = new SqlCommand("SELECT*FROM TB_HORARIO_INSTRUCTOR WHERE COD_USU=@CODUSU", this.con);
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

            if (con == null)
            {

                this.con.Close();

            }

            return lista;

        }

        public List<Modalidad> ListadoModalidadesEnsenanza(SqlConnection con, int idCat, int idSub, int codUsu)
        {
            List<Modalidad> lista = new List<Modalidad>();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_MODALIDADES_ENSENANZA", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            cmd.Parameters.AddWithValue("@IDCAT", idCat);
            cmd.Parameters.AddWithValue("@IDSUB", idSub);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                Modalidad m = new Modalidad();
                m.id = reader.GetInt32(0);
                m.descripcion = reader.GetString(1);
                m.precioHora = reader.GetDecimal(2);

                lista.Add(m);

            }

            reader.Close();

            return lista;

        }

        private List<Anuncio> ListadoAnuncios()
        {
            List<Anuncio> lista = new List<Anuncio>();
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT*FROM TB_ANUNCIO_INSTRUCTOR", con);
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

        private int InsertarAnuncio(string titulo, string contenido)
        {
            Usuario u = (Usuario)Session["usuario"];
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_INSERTAR_ANUNCIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODINSTR", u.codigo);
            cmd.Parameters.AddWithValue("@CONTENIDO", contenido);
            cmd.Parameters.AddWithValue("@TITULO", titulo);

            int rs = cmd.ExecuteNonQuery();


            con.Close();

            return rs;
        }

        private string InsertarEvento(Evento e)
        {
            string mensaje = "oh";
            Usuario u = (Usuario)Session["usuario"];
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_INSERTAR_EVENTO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODINSTR", u.codigo);
            cmd.Parameters.AddWithValue("@INICIO", e.inicio);
            cmd.Parameters.AddWithValue("@FIN", e.fin);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                mensaje = reader.GetString(0);

            }

            con.Close();

            return mensaje;
        }

        private string ModificarEvento(Evento e)
        {
            string mensaje = "oh";
            Usuario u = (Usuario)Session["usuario"];
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_MODIFICAR_EVENTO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODINSTR", u.codigo);
            cmd.Parameters.AddWithValue("@INICIO", e.inicio);
            cmd.Parameters.AddWithValue("@FIN", e.fin);
            cmd.Parameters.AddWithValue("@IDEVEN", e.id);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                mensaje = reader.GetString(0);

            }

            con.Close();

            return mensaje;
        }

        private string EliminarEvento(int idEvento)
        {
            string mensaje = "";
            Usuario u = (Usuario)Session["usuario"];
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM TB_HORARIO_INSTRUCTOR WHERE COD_USU=@CODUSU AND ID_HOR=@IDEVEN", con);
            cmd.Parameters.AddWithValue("@CODUSU", u.codigo);
            cmd.Parameters.AddWithValue("@IDEVEN", idEvento);

            int rs = cmd.ExecuteNonQuery();

            if (rs > 0)
            {

                mensaje = "Se eliminó correctamente";
            }
            else
            {
                mensaje = "No se pudo eliminar";

            }

            con.Close();

            return mensaje;
        }

        // ?

        private List<Usuario> InformeInstructor()
        {
            List<Usuario> lista = new List<Usuario>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_ListaAlumno", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                Usuario us = new Usuario();
                us.codigo = reader.GetInt32(0);
                us.nombres = reader.GetString(1);
                us.apPaterno = reader.GetString(2);
                us.apMaterno = reader.GetString(3);
                us.correo = reader.GetString(4);
                us.telefono = reader.GetString(5);
                us.login = reader.GetString(6);
                us.clave = reader.GetString(7);
                us.idRol = reader.GetInt32(8);
                us.desRol = reader.GetString(9);


                lista.Add(us);


            }

            con.Close();
            reader.Close();

            return lista;
        }

        private List<Categoria> categorias()
        {
            List<Categoria> lista = new List<Categoria>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_Curso", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Categoria cu = new Categoria();
                cu.id = reader.GetInt32(0);
                cu.descripcion = reader.GetString(1);

                lista.Add(cu);
            }

            con.Close();
            reader.Close();


            return lista;
        }

        // GET: Instructor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NuevoInstructor()
        {
            ViewBag.categorias = new SelectList(categorias(), "id", "descripcion");
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult NuevoAlumno(Usuario usu)
        {
            ViewBag.mensaje = "";
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("insert into tb_usuarios values (@id,@nom,@app,@apm,@co,@u,@cla,@rol)", con);
                cmd.Parameters.AddWithValue("@id", usu.codigo);
                cmd.Parameters.AddWithValue("@nom", usu.nombres);
                cmd.Parameters.AddWithValue("@app", usu.apPaterno);
                cmd.Parameters.AddWithValue("@apm", usu.apMaterno);
                cmd.Parameters.AddWithValue("@co", usu.correo);
                cmd.Parameters.AddWithValue("@u", usu.login);
                cmd.Parameters.AddWithValue("@cla", usu.clave);
                cmd.Parameters.AddWithValue("@rol", usu.idRol);
                cmd.ExecuteNonQuery();
                ViewBag.mensaje = "se registra";
            }
            catch (SqlException ex)
            {
                ViewBag.mensaje = ex.Message;
            }
            finally
            {
                ViewBag.categorias = new SelectList(categorias(), "id", "descripcion");
                con.Close();
            }
            return View(usu);
        }
        // ¿


        [HttpPost]
        public JsonResult insertarAnuncio(string titulo, string contenido)
        {
            int rs = InsertarAnuncio(titulo, contenido);
            string json = new JavaScriptSerializer().Serialize((ListadoAnuncios()));
            return Json(json);
        }

        [HttpPost]
        public JsonResult obtenerAnuncios()
        {
            string json = new JavaScriptSerializer().Serialize((ListadoAnuncios()));
            return Json(json);
        }
        [HttpPost]
        public JsonResult insertarEvento(Int64 inicio, Int64 fin)
        {
            //Debug.WriteLine("inicio " + inicio + " fin " + fin);
            Evento e = new Evento();
            e.inicio = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(inicio);
            e.fin = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(fin);

            string respuesta = InsertarEvento(e);

            Debug.WriteLine("inicio recibido " + e.inicio.ToString() + " fin recibido " + e.fin.ToString());
            List<Evento> lista = ListadoHorarios(null, ((Usuario)Session["usuario"]).codigo);
            List<Evento> citas = ListadoMisCitas(null, ((Usuario)Session["usuario"]).codigo);

            var resultado = new { eventos = lista, citas = citas, mensaje = respuesta };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult modificarEvento(int id, Int64 inicio, Int64 fin)
        {
            //Debug.WriteLine("id recibido " + id + " inicio recibido " + inicio.ToString() + " fin recibido " + fin.ToString());

            Evento e = new Evento();
            e.id = id;
            e.inicio = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(inicio);
            e.fin = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(fin);

            string respuesta = ModificarEvento(e);

            List<Evento> lista = ListadoHorarios(null, ((Usuario)Session["usuario"]).codigo);
            List<Evento> citas = ListadoMisCitas(null, ((Usuario)Session["usuario"]).codigo);

            var resultado = new { eventos = lista, citas = citas, mensaje = respuesta };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult eliminarEvento(int id)
        {
            string respuesta = EliminarEvento(id);
            List<Evento> lista = ListadoHorarios(null, ((Usuario)Session["usuario"]).codigo);
            List<Evento> citas = ListadoMisCitas(null, ((Usuario)Session["usuario"]).codigo);
            var resultado = new { eventos = lista, citas = citas, mensaje = respuesta };
            return Json(resultado, JsonRequestBehavior.AllowGet);

        }
    }
}