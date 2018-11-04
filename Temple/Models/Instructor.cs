using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
	public class Instructor
	{
		public int codigo { get; set; }
		public string nombres { get; set; }
		public string apellidos{ get; set; }
		public string especialidad { get; set; }
		public int idCurso { get; set; }
		public string desCurso { get; set; }
		public string foto { get; set; }
		public int codigoHorario { get; set; }
		public string descriHorarios { get; set; }
		public string correo { get; set; }
		public string experiencia { get; set; }
		public string habilidades { get; set; }
		public string telefono { get; set; }


	}
}