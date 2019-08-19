using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Temple.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Temple.Controllers
{
    public class RegistroController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        // GET: Registro
        public ActionResult RegistroAlumno()
        {
            //ViewBag.usuario = Session["usuario"];
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", 0);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(ListadoCategorias().ElementAt(0).id)), "id", "descripcion", 0);

            return View();
        }

        [HttpPost]
        public ActionResult RegistroAlumno(string nombres, string apPat, string apMat, int edad, int genero,
            string correo, string telefono, string preferencias, string usuario, string clave, string sobreMi, string buscando,
            HttpPostedFileBase imagen, decimal latitud, decimal longitud) {
            try
            {
                //Debug.WriteLine("nombre recibido " + nombres +" "+apPat+" "+apMat+" "+edad+" genero: "+genero+" "+correo+" "+telefono+" preferencias: "+preferencias+" usuario: "+usuario+" clave: "+clave+" "+sobreMi+" "+buscando);
                ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", 0);
                ViewBag.subcategorias = new SelectList((ListadoSubcategorias(ListadoCategorias().ElementAt(0).id)), "id", "descripcion", 0);

                try
                {
                    // Deserializamos el json
                    List<PreferenciaAprendizaje> lista = JArray.Parse(preferencias).ToObject<List<PreferenciaAprendizaje>>();

                    // Construimos la ubicación
                    Ubicacion ub = new Ubicacion();
                    ub.latitud = latitud;
                    ub.longitud = longitud;

                    Usuario u = new Usuario();
                    u.nombres = nombres;
                    u.apPaterno = apPat;
                    u.apMaterno = apMat;
                    u.edad = edad;
                    u.idGen = genero;
                    u.correo = correo;
                    u.telefono = telefono;
                    u.login = usuario;
                    u.clave = clave;
                    u.idRol = 2;
                    u.sobreMi = sobreMi;
                    u.buscando = buscando;

                    Usuario res = RegistrarAlumno(u, lista, ub);


                    if (res != null)
                    {
                        var originalFilename = Path.GetFileName(imagen.FileName);
                        string fileId = Guid.NewGuid().ToString().Replace("-", "");
                        string userId = res.codigo.ToString(); // Function to get user id based on your schema

                        var path = Path.Combine(Server.MapPath("~/imagenes/perfiles/"), userId/*, fileId*/);
                        imagen.SaveAs(path + Path.GetExtension(imagen.FileName));
                        ViewBag.mensaje = "Registro exitoso";
                    }
                    else
                    {

                        ViewBag.mensaje = "Error al registrar en la base de datos";


                    }
                }
                catch (Exception e)
                {

                    ViewBag.mensaje = "Error al registrar. Verifica que has llenado todos los datos correctamente, incluida la foto. Además, verifica que hayas especificado cursos diferentes";
                    return View();

                }
            }
            catch (Exception e) {

                ViewBag.mensaje = "Error al registrar. Verifica que has llenado todos los datos correctamente, incluida la foto. Además, verifica que hayas especificado cursos diferentes";


            }

            return View();

        }

        public ActionResult RegistroInstructor() {

            //ViewBag.usuario = Session["usuario"];
            ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", 0);
            ViewBag.subcategorias = new SelectList((ListadoSubcategorias(ListadoCategorias().ElementAt(0).id)), "id", "descripcion", 0);
            ViewBag.modalidades = ListadoModalidades();
            return View();

        }

        [HttpPost]
        public ActionResult RegistroInstructor(string nombres, string apPat, string apMat, int edad, int genero,
            string correo, string telefono, string preferencias, string usuario, string clave, string sobreMi, string especialidad,
            HttpPostedFileBase imagen, decimal latitud, decimal longitud, HttpPostedFileBase cv)
        {
            try
            {
                ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion", 0);
                ViewBag.subcategorias = new SelectList((ListadoSubcategorias(ListadoCategorias().ElementAt(0).id)), "id", "descripcion", 0);
                ViewBag.modalidades = ListadoModalidades();
                Debug.WriteLine(preferencias);
                // Deserializamos el json
                List<PreferenciaEnsenanza> lista = JArray.Parse(preferencias).ToObject<List<PreferenciaEnsenanza>>();

                // Construimos la ubicación
                Ubicacion ub = new Ubicacion();
                ub.latitud = latitud;
                ub.longitud = longitud;

                Usuario u = new Usuario();
                u.nombres = nombres;
                u.apPaterno = apPat;
                u.apMaterno = apMat;
                u.edad = edad;
                u.idGen = genero;
                u.correo = correo;
                u.telefono = telefono;
                u.login = usuario;
                u.clave = clave;
                u.idRol = 1;
                u.sobreMi = sobreMi;
                u.especialidad = especialidad;

                Usuario res = RegistrarInstructor(u, lista, ub);


                if (res != null)
                {
                    var originalFilename = Path.GetFileName(imagen.FileName);
                    string fileId = Guid.NewGuid().ToString().Replace("-", "");
                    string userId = res.codigo.ToString(); // Function to get user id based on your schema

                    var path = Path.Combine(Server.MapPath("~/imagenes/perfiles/"), userId/*, fileId*/);
                    imagen.SaveAs(path + Path.GetExtension(imagen.FileName));

                    if (cv != null)
                    {

                        var cvFileName = Path.GetFileName(cv.FileName);
                        string cvId = Guid.NewGuid().ToString().Replace("-", "");

                        var pathCv = Path.Combine(Server.MapPath("~/cvs/"), userId/*, fileId*/);
                        imagen.SaveAs(path + Path.GetExtension(imagen.FileName));

                    }


                    ViewBag.mensaje = "Registro exitoso";
                }
                else
                {

                    ViewBag.mensaje = "Error al registrar en la base de datos";


                }

            }
            catch (Exception e) {

                ViewBag.mensaje = "Error al registrar en la base de datos";

            }


            return View();
            
       }


        //

        public Usuario RegistrarAlumno(Usuario u, List<PreferenciaAprendizaje> preferencias, Ubicacion ub)
        {
            Usuario res = new Usuario();
            con.Open();
            SqlTransaction t1 = con.BeginTransaction();
            
                SqlCommand cmd = new SqlCommand("USP_REGISTRAR_USUARIO", con, t1);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMUSU", u.nombres);
                cmd.Parameters.AddWithValue("@APAUSU", u.apPaterno);
                cmd.Parameters.AddWithValue("@AMAUSU", u.apMaterno);
                cmd.Parameters.AddWithValue("@CORREO", u.correo);
                cmd.Parameters.AddWithValue("@EDAD", u.edad);
                cmd.Parameters.AddWithValue("@IDGEN", u.idGen);
                cmd.Parameters.AddWithValue("@TEL", u.telefono);
                cmd.Parameters.AddWithValue("@LOGUSU", u.login);
                cmd.Parameters.AddWithValue("@CLAUSU", u.clave);
                cmd.Parameters.AddWithValue("@IDROL", u.idRol);
                cmd.Parameters.AddWithValue("@SOBREMI", u.sobreMi);
                cmd.Parameters.AddWithValue("@BUSCANDO", u.buscando);
                cmd.Parameters.AddWithValue("@ESPECIALIDAD", ' ');
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    res.codigo = reader.GetInt32(0);

                    reader.Close();

                    int rs = -1;
                    // Ahora, itero la lista
                    for (int i = 0; i < preferencias.Count(); i++)
                    {
                        Debug.WriteLine("pref "+i);
                        cmd = new SqlCommand("USP_REGISTRAR_PREFERENCIA_APRENDIZAJE", con, t1);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CODUSU", res.codigo);
                        cmd.Parameters.AddWithValue("@IDCAT", preferencias.ElementAt(i).idCat);
                        cmd.Parameters.AddWithValue("@IDSUB", preferencias.ElementAt(i).idSub);
                        rs = cmd.ExecuteNonQuery();

                    }

                    // Finalmente, ingreso la ubicación
                    cmd = new SqlCommand("INSERT INTO TB_UBICACION_USUARIO VALUES (@CODUSU,@LAT,@LON)", con, t1);
                    cmd.Parameters.AddWithValue("@CODUSU", res.codigo);
                    cmd.Parameters.AddWithValue("@LAT", ub.latitud);
                    cmd.Parameters.AddWithValue("@LON", ub.longitud);
                    rs = cmd.ExecuteNonQuery();

                    if (rs != -1)
                    {
                        t1.Commit();
                    }
                    else
                    {
                        t1.Rollback();
                        res = null;

                    }
                }
                else
                {

                    t1.Rollback();
                    res = null;
                }
            

            return res;

        }

        public Usuario RegistrarInstructor(Usuario u, List<PreferenciaEnsenanza> preferencias, Ubicacion ub)
        {
            Usuario res = new Usuario();
            con.Open();
            SqlTransaction t1 = con.BeginTransaction();
            
                SqlCommand cmd = new SqlCommand("USP_REGISTRAR_USUARIO", con, t1);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMUSU", u.nombres);
                cmd.Parameters.AddWithValue("@APAUSU", u.apPaterno);
                cmd.Parameters.AddWithValue("@AMAUSU", u.apMaterno);
                cmd.Parameters.AddWithValue("@CORREO", u.correo);
                cmd.Parameters.AddWithValue("@EDAD", u.edad);
                cmd.Parameters.AddWithValue("@IDGEN", u.idGen);
                cmd.Parameters.AddWithValue("@TEL", u.telefono);
                cmd.Parameters.AddWithValue("@LOGUSU", u.login);
                cmd.Parameters.AddWithValue("@CLAUSU", u.clave);
                cmd.Parameters.AddWithValue("@IDROL", u.idRol);
                cmd.Parameters.AddWithValue("@SOBREMI", u.sobreMi);
                cmd.Parameters.AddWithValue("@BUSCANDO", ' ');
                cmd.Parameters.AddWithValue("@ESPECIALIDAD", u.especialidad);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    res.codigo = reader.GetInt32(0);

                    reader.Close();

                    int rs = -1;

                    // Ahora, itero la lista
                    for (int i = 0; i < preferencias.Count(); i++)
                    {               
                            cmd = new SqlCommand("USP_REGISTRAR_PREFERENCIA_ENSENANZA", con, t1);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CODUSU", res.codigo);
                            cmd.Parameters.AddWithValue("@IDCAT", preferencias.ElementAt(i).idCat);
                            cmd.Parameters.AddWithValue("@IDSUB", preferencias.ElementAt(i).idSub);
                            cmd.Parameters.AddWithValue("@DESPREF", preferencias.ElementAt(i).descripcion);
                            cmd.Parameters.AddWithValue("@SILPREF", preferencias.ElementAt(i).silabo);
                            SqlDataReader readerIdPref = cmd.ExecuteReader();

                        int idPref = -1;
                        // Obtengo las modalidades
                        List<Modalidad> modAux = preferencias.ElementAt(i).modalidades;

                        if (readerIdPref.Read())
                        {
                            idPref = readerIdPref.GetInt32(0);
                            
                            // Cerramos el lector
                            readerIdPref.Close();

                        for (int j = 0; j < modAux.Count(); j++)
                            {

                                cmd = new SqlCommand("USP_REGISTRAR_PREFERENCIA_ENSENANZA_MODALIDAD", con, t1);
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@IDPREF", idPref);
                                cmd.Parameters.AddWithValue("@IDMOD", modAux.ElementAt(j).id);
                                cmd.Parameters.AddWithValue("@PRECIOHORA", modAux.ElementAt(j).precioHora);
                                int rsPem=cmd.ExecuteNonQuery();

                                if (rsPem == -1) {
                                    
                                    t1.Rollback();
                                    res = null;
                                    
                                }

                            }
                        }

                        else
                        {

                            t1.Rollback();
                            res = null;

                        }
                                     
                    }

                    // Finalmente, ingreso la ubicación
                    cmd = new SqlCommand("INSERT INTO TB_UBICACION_USUARIO VALUES (@CODUSU,@LAT,@LON)", con, t1);
                    cmd.Parameters.AddWithValue("@CODUSU", res.codigo);
                    cmd.Parameters.AddWithValue("@LAT", ub.latitud);
                    cmd.Parameters.AddWithValue("@LON", ub.longitud);
                    rs = cmd.ExecuteNonQuery();

                    if (rs != -1)
                    {
                        t1.Commit();
                    }
                    else
                    {
                        t1.Rollback();
                        res = null;

                    }
                }
                else
                {

                    t1.Rollback();
                    res = null;
                }
            
            return res;

        }

        public List<Modalidad> ListadoModalidades()
        {
            con.Open();
            List<Modalidad> lista = new List<Modalidad>();
            SqlCommand cmd = new SqlCommand("SELECT*FROM TB_MODALIDAD_ENSENANZA", con);
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {

                Modalidad m = new Modalidad();
                m.id = reader.GetInt32(0);
                m.descripcion = reader.GetString(1);
                lista.Add(m);

            }

            reader.Close();
            con.Close();
            return lista;

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