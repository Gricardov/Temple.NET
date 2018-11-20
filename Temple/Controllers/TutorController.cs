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

		public List<Subcategoria> ListadoSubcategorias()
		{

			List<Subcategoria> lista = new List<Subcategoria>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select ID_SUB,DES_SUB from SUBCATEGORIAS", con);
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

		public List<Dia> ListadoDias()
		{

			List<Dia> lista = new List<Dia>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select ID_DIA,DES_DIA from TB_DIA", con);
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{

				Dia d = new Dia();
				d.idDia = reader.GetInt32(0);
				d.desDia = reader.GetString(1);

				lista.Add(d);

			}
			reader.Close();
			con.Close();

			return lista;

		}

		public List<Hora> ListadoHoras()
		{

			List<Hora> lista = new List<Hora>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select ID_HO,DES_HO from TB_HORARIO", con);
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{

				Hora h = new Hora();
				h.idH = reader.GetInt32(0);
				h.desH = reader.GetString(1);

				lista.Add(h);

			}
			reader.Close();
			con.Close();

			return lista;

		}

		public List<Rol> ListadoRol()
		{
			List<Rol> lista = new List<Rol>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select ID_ROL,DES_ROL from TB_ROL", con);
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{

				Rol rol = new Rol();
				rol.idR = reader.GetInt32(0);
				rol.desR = reader.GetString(1);

				lista.Add(rol);

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
		public ActionResult NuevoInstructor03()
		{
			ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion");
			ViewBag.subcategorias = new SelectList(ListadoSubcategorias(), "id", "descripcion");
			ViewBag.dias = new SelectList(ListadoDias(), "idDia", "desDia_");
			ViewBag.horas= new SelectList(ListadoHoras(), "idH", "desH_");
			ViewBag.rol= new SelectList(ListadoHoras(), "idR", "desR_");



			return View( new Tutor());
		}

		[HttpPost]public ActionResult NuevoTutor03(Tutor reg)
		{
			ViewBag.mensaje = "";
			con.Open();
			try
			{
				SqlCommand cmd = new SqlCommand("USP_REGISTRAR_TUTOR (@ID,@NOM,@APP,@APM,@CO,@FONO,@DIRE,@IDCA,@IDSUB,@LOG,@CLA,@ROL,@HOR,@DIA)", con);
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
				cmd.Parameters.AddWithValue("@HOR", reg.idHora);
				cmd.Parameters.AddWithValue("@DIA", reg.idDia);


				cmd.ExecuteNonQuery();
				ViewBag.mensaje = "se registra";
			}
			catch (SqlException ex)
			{
				ViewBag.mensaje = ex.Message;
			}
			finally
			{
				ViewBag.categorias = new SelectList(ListadoCategorias(), "id", "descripcion",reg.idcate);

				ViewBag.subcategorias = new SelectList(ListadoSubcategorias(), "id", "descripcion",reg.idsubcate);
				ViewBag.dias = new SelectList(ListadoDias(), "idDia", "desDia",reg.idDia);
				ViewBag.horas = new SelectList(ListadoHoras(), "idH", "desH",reg.idHora);
				ViewBag.rol = new SelectList(ListadoHoras(), "idR", "desR",reg.idRol);
				con.Close();
			}
			return View(reg);
		
		}


	}
}