using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
	public class Alumno
	{
		public int codigo { get; set; }
		public string nombres { get; set; }
		public string apellidos { get; set; }
		public string grado { get; set; }
		public int idCurso { get; set; }
		public string desCurso { get; set; }
		public string foto { get; set; }
		public DateTime fecha { get; set; }
		public string correo { get; set; }
		public string desfecha { get; set; }
		
		public string telefono { get; set; }
	}
}