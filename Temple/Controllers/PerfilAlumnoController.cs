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
        /*SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
		private List<Alumno> InformeAlumno()
		{
			List<Alumno> lista = new List<Alumno>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_ListaAlumno", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Alumno alu = new Alumno();
				alu.foto = reader.GetString(0);
				alu.codigo = reader.GetInt32(1);
				alu.nombres = reader.GetString(2);
				alu.apellidos = reader.GetString(3);
				alu.grado = reader.GetString(4);
				alu.correo = reader.GetString(5);
				alu.telefono = reader.GetString(6);

				lista.Add(alu);


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
    }*/
    }
}