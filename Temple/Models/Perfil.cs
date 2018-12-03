using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class Perfil
    {
        public Perfil()
        {

            reseñas = new List<Reseña>();
            cursos = new List<PreferenciaEnsenanza>();
            horarios = new List<Evento>();
            transacciones = new List<Transaccion>();

        }
        public int codigo { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
        public string especialidad { get; set; }
        public string sobreMi { get; set; }
        public string buscando { get; set; }
        public string cv { get; set; }
        public string distancia { get; set; }
        public int calificacion { get; set; }
        public bool verificado { get; set; }
        public bool conectado { get; set; }
        public int idPerfil { get; set; }
        public Ubicacion ubicacion { get; set; }
        public List<Reseña> reseñas { get; set; }
        public List<PreferenciaEnsenanza> cursos { get; set; }
        public List<Evento> horarios { get; set; }
        public List<Evento> citas { get; set; }
        public List<Transaccion> transacciones { get; set; }
        
        
    }
}