using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Reseña
    {
        public int id { get; set; }
        public int idPerfilRemitente { get; set; }
        public string nombreRemitente { get; set; }
        public string apPaternoRemitente { get; set; }
        public string apMaternoRemitente { get; set; }
        public int idPerfilDestinatario { get; set; }
        public string contenido { get; set; }
        public DateTime fechaHora { get; set; }
        public int calificacion { get; set; }
    }
}