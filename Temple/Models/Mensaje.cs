using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Mensaje
    {
        public int idMensaje { get; set; }
        public int codRemit { get; set; }
        public string nombreRemit { get; set; }
        public string apPatRemit { get; set; }
        public string apMatRemit { get; set; }

        public int codDestin { get; set; }
        public string nombreDestin { get; set; }
        public string apPatDestin { get; set; }
        public string apMatDestin { get; set; }

        public string desRolRemit { get; set; }
        public string desRolDestin { get; set; }

        public DateTime fechaHora { get; set; }
        public string contenido { get; set; }
        public int idChat { get; set; }
    }
}