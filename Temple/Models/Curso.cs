using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Curso
    {
        public Curso() {

            modalidades = new List<Modalidad>();

        }
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string silabo { get; set; }
        public List<Modalidad> modalidades {get;set;}
	}
}