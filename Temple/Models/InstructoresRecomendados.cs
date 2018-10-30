using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class InstructoresRecomendados
    {
        public PreferenciaAprendizaje prefApr { get; set; }

        public List<TarjetaInstructor> instructores { get; set; }
    }
}