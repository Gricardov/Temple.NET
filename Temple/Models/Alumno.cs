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
		public string apPaterno { get; set; }
		public string apMaterno { get; set; }
		public string correo { get; set; }
		public string telefonos { get; set; }
		public string direccion { get; set; }
		public int idcate { get; set; }
		public string nombrescate { get; set; }
		public int idsubcate { get; set; }
		public string nombresubcate { get; set; }
		public string login { get; set; }
		public string clave { get; set; }
		public int idRol { get; set; }
		public string desRol { get; set; }
	}
}