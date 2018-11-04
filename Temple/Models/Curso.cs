using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
	public class Curso
	{
		public int idCurso { get; set; }
		public string nombreCurso { get; set; }
		public string silabo { get; set; }
		public string ejercicios { get; set; }
	}
}