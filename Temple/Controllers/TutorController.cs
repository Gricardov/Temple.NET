using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using Temple.Models;



namespace Temple.Controllers
{
    public class TutorController : Controller
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

		// GET: Tutor
		public ActionResult Index()
        {
            return View();
        }
		public ActionResult NuevoTutor()
		{
			ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion");

			return View( new Tutor());
		}

		[HttpPost]public ActionResult NuevoTutor(Tutor reg)
		{
			ViewBag.mensaje = "";
			con.Open();
			try
			{
				SqlCommand cmd = new SqlCommand("insert into tb_tutor values (@id,@nom,@app,@apm,@co,@fono,@di,@idca,@idsub,@log,@cla,@rol,@drol)", con);
				cmd.Parameters.AddWithValue("@id", reg.codigo);
				cmd.Parameters.AddWithValue("@nom", reg.nombres);
				cmd.Parameters.AddWithValue("@app", reg.apPaterno);
				cmd.Parameters.AddWithValue("@apm", reg.apMaterno);
				cmd.Parameters.AddWithValue("@co", reg.correo);
				cmd.Parameters.AddWithValue("@fono", reg.telefonos);
				cmd.Parameters.AddWithValue("@di", reg.direccion);
				cmd.Parameters.AddWithValue("@idca", reg.idcate);
				cmd.Parameters.AddWithValue("@idsub", reg.idsubcate);
				cmd.Parameters.AddWithValue("@log", reg.login);
				cmd.Parameters.AddWithValue("@cla", reg.clave);
				cmd.Parameters.AddWithValue("@rol", reg.idRol);
				cmd.Parameters.AddWithValue("@drol", reg.desRol);

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
				con.Close();
			}
			return View(reg);
		
		}


	}
}