using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Anuncio
    {
        public int id { get; set; }
        public int codInstr { get; set; }
        public string nomInstr { get; set; }
        public string apPatInstr { get; set; }
        public string apMatInstr { get; set; }
        public string titulo { get; set; }
        public string contenido { get; set; }
        public DateTime fechaHora { get; set; }
    }
}