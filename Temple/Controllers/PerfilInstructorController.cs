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
    public class PerfilInstructorController : Controller
    {
		SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
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

		private List<Subcategoria> Subcategorias()
		{
			List<Subcategoria> lista = new List<Subcategoria>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_Curso", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Subcategoria sca = new Subcategoria();
				sca.id = reader.GetInt32(0);
				sca.descripcion = reader.GetString(1);
				
				lista.Add(sca);
			}

			con.Close();
			reader.Close();


			return lista;
		}


		private List<Usuario> InformeInstructor()
		{
			List<Usuario> lista = new List<Usuario>();
			con.Open();
			SqlCommand cmd = new SqlCommand("select", con);
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
		// GET: PerfilInstructor
		public ActionResult Index()
        {
			
			return View();
        }

		public ActionResult NuevoInstructor()
		{
			ViewBag.categorias = new SelectList(categorias(), "id", "descripcion");
			ViewBag.Subcategorias = new SelectList(Subcategorias(),"id", "descripcion"); 
			return View(new Usuario());
		}

		[HttpPost]
		public ActionResult NuevoInstructor(Usuario usu)
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
				ViewBag.Subcategorias = new SelectList(Subcategorias(), "id", "descripcion");
				con.Close();
			}
			return View(usu);
		}
	}
}