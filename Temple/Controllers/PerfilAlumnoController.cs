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
    public class PerfilAlumnoController : Controller
    {
		SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
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

		public List<Subcategoria> ListadoSubcategorias()
		{

			List<Subcategoria> lista = new List<Subcategoria>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select id,descripcion from SUBCATEGORIAS", con);
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


		// GET: PerfilAlumno
		public ActionResult Index()
        {
            return View();
        }
		public ActionResult NuevoEstudiante()
		{
			ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion");
			ViewBag.subcategorias = new SelectList(ListadoSubcategorias(), "id", "descripcion");


			return View(new Alumno());
		}

		[HttpPost]
		public ActionResult NuevoEstudiante(Alumno reg)
		{
			ViewBag.mensaje = "";
			con.Open();
			try
			{
				SqlCommand cmd = new SqlCommand(" USP_REGISTRAR_ESTUDIANTE (@ID,@NOM,@APP,@APM,@CO,@FONO,@DIRE,@IDCA,@IDSUB,@LOG,@CLA,@ROL)", con);
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ID", reg.codigo);
				cmd.Parameters.AddWithValue("@NOM", reg.nombres);
				cmd.Parameters.AddWithValue("@APP", reg.apPaterno);
				cmd.Parameters.AddWithValue("@APM", reg.apMaterno);
				cmd.Parameters.AddWithValue("@CO", reg.correo);
				cmd.Parameters.AddWithValue("@FONO", reg.telefonos);
				cmd.Parameters.AddWithValue("@DIRE", reg.direccion);
				cmd.Parameters.AddWithValue("@IDCA", reg.idcate);
				cmd.Parameters.AddWithValue("@IDSUB", reg.idsubcate);
				cmd.Parameters.AddWithValue("@LOG", reg.login);
				cmd.Parameters.AddWithValue("@CLA", reg.clave);
				cmd.Parameters.AddWithValue("@ROL", reg.idRol);

				cmd.ExecuteNonQuery();
				ViewBag.mensaje = "se registra";
			}
			catch (SqlException ex)
			{
				ViewBag.mensaje = ex.Message;
			}
			finally
			{
				ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion");

				ViewBag.subcategorias = new SelectList(ListadoSubcategorias(), "id", "descripcion");
				con.Close();
			}
			return View(reg);

		}




	}
}