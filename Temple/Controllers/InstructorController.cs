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
    public class InstructorController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);
		private List<Instructor> InformeInstructor()
		{
			List<Instructor> lista = new List<Instructor>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_ListaInstructor", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Instructor instru = new Instructor();
				instru.foto = reader.GetString(0);
				instru.codigo = reader.GetInt32(1);
				instru.nombres = reader.GetString(2);
				instru.apellidos = reader.GetString(3);
				instru.especialidad= reader.GetString(4);
				instru.correo= reader.GetString(5);
				instru.experiencia = reader.GetString(6);
				instru.habilidades = reader.GetString(7);
				instru.telefono = reader.GetString(8);

				lista.Add(instru);


			}

			con.Close();
			reader.Close();

			return lista;
		}

		private List<Curso> ListaCurso()
		{
			List<Curso> lista = new List<Curso>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_ListaCurso", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			String nombreCu = ((Instructor)Session["nombre"]).desCurso;
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

		private List<Horario> ListaHorario()
		{
			List<Horario> lista = new List<Horario>();
			con.Open();
			SqlCommand cmd = new SqlCommand("sp_Horario", con);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				Horario h = new Horario();
				h.codigoHorario = reader.GetInt32(0);
				h.codigoDia = reader.GetInt32(0);
				h.codigoHora = reader.GetInt32(0);
				h.descriHorarios = reader.GetString(3);
				lista.Add(h);
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
    }
}