using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class TarjetaInstructor
    {
        public TarjetaInstructor() {
            ubicacion = new Ubicacion();

        }

        

        public int codigo { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
        public string especialidad { get; set; }
        public string distancia { get; set; }
        public int numResenas { get; set; }
        public int calificacion { get; set; }
        public bool verificado { get; set; }
        public Ubicacion ubicacion { get; set; }

    }
}