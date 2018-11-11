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
		private List<Usuario> InformeAlumno()
		{
			List<Alumno> lista = new List<Alumno>();
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
				us.telefonos = reader.GetString(5);
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

		private List<Curso> ListaCurso()
		{
			List<Curso> lista = new List<Curso>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_ListaCursoxAlu", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			String nombreCu = ((Alumno)Session["nombre"]).desCurso;
			cmd.Parameters.AddWithValue("@nomCu", nombreCu);
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Curso cu = new Curso();
				cu.idCurso = reader.GetInt32(0);
				cu.nombreCurso = reader.GetString(1);
				cu.silabo = reader.GetString(2);
				cu.ejercicios = reader.GetString(3);
				lista.Add(cu);
			}

			con.Close();
			reader.Close();


			return lista;
		}

		private List<Alumno> InformeActividades()
		{
			List<Alumno> lista = new List<Alumno>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_ListaAlumno", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Alumno alu = new Alumno();
				alu.fecha = reader.GetDateTime(0);
				alu.desfecha = reader.GetString(1);
				

				lista.Add(alu);


			}

			con.Close();
			reader.Close();

			return lista;
		}
		// GET: PerfilAlumno
		public ActionResult Index()
        {
            return View();
        }
		public ActionResult NuevoAlumno()
		{
			return View(new Usuario());
		}


	}
}