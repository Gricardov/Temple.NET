using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Reseña
    {
        public int id { get; set; }
        public int idRemitente { get; set; }
        public int idDestinatario { get; set; }
        public string contenido { get; set; }
        public int calificacion { get; set; }
        public DateTime fechaHora { get; set; }
    }
}