using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class PerfilInstructor
    {
        public PerfilInstructor()
        {

            reseñas = new List<Reseña>();
            cursos = new List<Curso>();
            horarios = new List<DateTime>();

        }
        public int codigo { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
        public string especialidad { get; set; }
        public string sobreMi { get; set; }
        public string cv { get; set; }
        public string distancia { get; set; }
        public int calificacion { get; set; }
        public bool verificado { get; set; }
        public bool conectado { get; set; }
        public Ubicacion ubicacion { get; set; }
        public List<Reseña> reseñas { get; set; }
        public List<Curso> cursos { get; set; }
        public List<DateTime> horarios { get; set; }


    }
}