using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Pedido
    {
        public int codInstr { get; set; }
        public string nombresInstructor { get; set; }
        public int idCat { get; set; }
        public int idSub { get; set; }
        public string nombreCurso { get; set; }
        public int idMod { get; set; }
        public string nombreModalidad { get; set; }

        public double precioHora { get; set; }

        public double cantHoras { get; set; }

        public Evento horario { get; set; }
        public double precioTotal { get; set; }
    }
}