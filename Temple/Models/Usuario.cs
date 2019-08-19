using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Usuario
    {
		public Usuario()
		{
			categoria = new Categoria();
			subcategoria = new Subcategoria();
		}


        public int codigo { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
		public string correo { get; set; }
		public string telefono { get; set; }
        public int edad { get; set; }
        public int idGen { get; set; }
        public string desGen { get; set; }
		public string login { get; set; }
        public string clave { get; set; }
        public int idRol { get; set; }
        public string desRol { get; set; }
        public string sobreMi { get; set; }
        public string buscando { get; set; }
        public string especialidad { get; set; }
		public Categoria categoria { get; set; }
		public Subcategoria subcategoria { get; set; }
        
	}
}