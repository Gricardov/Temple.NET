using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Modalidad
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal precioHora { get; set; }
    }
}
/*MODALIDADES?
    DEPENDIENDO DE LA MODALIDAD, VARÍA EL PRECIO POR HORA

    PERSONALIZADO, GRUPAL, FULL EJERCICIOS, FULL TEORÍA, PREPARACIÓN EXAMEN     

 */