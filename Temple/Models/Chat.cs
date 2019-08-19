using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Chat
    {
        public int idChat { get; set; }

        // Aquí se guarda el último mensaje
        public Mensaje ultimoMensaje { get; set; }

        public int codDestin { get; set; }
        public string nombreDestin { get; set; }
        public string apPatDestin { get; set; }
        public string apMatDestin { get; set; }
        public string desRolDestin { get; set; }

        public List<Mensaje> mensajes { get; set; }
    }
}