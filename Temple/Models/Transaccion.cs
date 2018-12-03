using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Transaccion
    {
        public int codTran { get; set; }
        public int codInstr { get; set; }
        public string nombresInstr { get; set; }
        public string apPatInstr { get; set; }
        public string apMatInstr { get; set; }
        public int codAlu { get; set; }
        public int idCat { get; set; }
        public int idSub { get; set; }
        public string desCat { get; set; }
        public string desSub { get; set; }
        public int idMod { get; set; }
        public string desMod { get; set; }
        public decimal precioHora { get; set; }
        public decimal total { get; set; }
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
        public DateTime fechaHora { get; set; }
    }
}