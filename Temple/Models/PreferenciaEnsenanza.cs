using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class PreferenciaEnsenanza
    {
        public int idCat { get; set; }
        public string desCat { get; set; }
        public int idSub { get; set; }
        public string desSub { get; set; }
        public string descripcion { get; set; }
        public string silabo { get; set; }
        public List<Modalidad> modalidades { get; set; }
    }
}