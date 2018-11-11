using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Usuario
    {
        public int codigo { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
		public string correo { get; set; }
		public string telefonos { get; set; }
		public string login { get; set; }
        public string clave { get; set; }
        public int idRol { get; set; }
        public string desRol { get; set; }
    }
}