using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Temple.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Temple.Controllers
{
    public class ChatController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        // GET: Chat
        public ActionResult Chat()
        {
            ViewBag.usuario = Session["usuario"];
            Debug.WriteLine("chats " + ListadoChats().Count());
            List<Chat> chats = ListadoChats();
            chats.Reverse();
            ViewBag.chats = chats;
            return View();
        }

        private List<Int32> ListadoIdsChatsUsuario() {

            List<Int32> ids = new List<Int32>();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_IDS_CHATS_USUARIO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            int codUsu = ((Usuario)Session["usuario"]).codigo;
            cmd.Parameters.AddWithValue("@CODUSU", codUsu);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }

            con.Close();
            reader.Close();

            return ids;

        }


        private List<Chat> ListadoChats() {

            // Primero obtengo los ids de chats para este usuario
            List<Int32> ids = ListadoIdsChatsUsuario();
            // Declaro mi listado de chats
            List<Chat> chats = new List<Chat>();

            con.Open();

            foreach (Int32 id in ids)
            {
                SqlCommand cmd = new SqlCommand("USP_OBTENER_MENSAJES_POR_CHAT", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDCHAT", id);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Mensaje> ms = new List<Mensaje>();

                while (reader.Read())
                {

                    Mensaje m = new Mensaje();
                    m.idChat = reader.GetInt32(0);
                    m.idMensaje = reader.GetInt32(1);
                    m.codRemit = reader.GetInt32(2);
                    m.nombreRemit = reader.GetString(3);
                    m.apPatRemit = reader.GetString(4);
                    m.apMatRemit = reader.GetString(5);
                    m.desRolRemit = reader.GetString(6);
                    m.codDestin = reader.GetInt32(7);
                    m.nombreDestin = reader.GetString(8);
                    m.apPatDestin = reader.GetString(9);
                    m.apMatDestin = reader.GetString(10);
                    m.desRolDestin = reader.GetString(11);
                    m.fechaHora = reader.GetDateTime(12);
                    m.contenido = reader.GetString(13);

                    ms.Add(m);

                }

                // Obtengo el usuario para saber quien es el destinatario
                Usuario u = (Usuario)Session["usuario"];
                
                // Finalmente, armo mi chat
                Chat c = new Chat();
                c.idChat = id;
                c.mensajes = ms;

                // Evalúo para obtener mi destinatario. Sí o sí, para el chat tiene que ser mi interlocutor de chat
                Mensaje ultimo = c.mensajes.Last();
                c.codDestin = (ultimo.codDestin != u.codigo ? ultimo.codDestin : (ultimo.codRemit != u.codigo?ultimo.codRemit:u.codigo));
                c.nombreDestin = (ultimo.codDestin != u.codigo ? ultimo.nombreDestin : (ultimo.codRemit != u.codigo ? ultimo.nombreRemit : u.nombres));
                c.apPatDestin = (ultimo.codDestin != u.codigo ? ultimo.apPatDestin : (ultimo.codRemit != u.codigo ? ultimo.apPatRemit : u.apPaterno));
                c.apMatDestin = (ultimo.codDestin != u.codigo ? ultimo.apMatDestin : (ultimo.codRemit != u.codigo ? ultimo.apMatRemit : u.apMaterno));
                c.desRolDestin = (ultimo.codDestin != u.codigo ? ultimo.desRolDestin : (ultimo.codRemit != u.codigo ? ultimo.desRolRemit : u.desRol));
                                             
                c.ultimoMensaje = ultimo;
                
                chats.Add(c);

                reader.Close();

            }
            con.Close();

            return chats;

        }

        private int EnviarMensaje(int codRemit ,int codDestin, string contenido)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_ENVIAR_MENSAJE", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODREMIT", codRemit);
            cmd.Parameters.AddWithValue("@CODDESTIN", codDestin);
            cmd.Parameters.AddWithValue("@CONTENIDO", contenido);

            int rs = cmd.ExecuteNonQuery();

            con.Close();

            return rs;
        }

        [HttpPost]
        public JsonResult enviarConsulta(string contenido, int codDestin)
        {   Debug.WriteLine(contenido+" "+codDestin);
            Usuario remit = (Usuario)Session["usuario"];
            int rs = EnviarMensaje(remit.codigo, codDestin, contenido);

            string respuesta = rs > 0 ? "Se envió la consulta" : "Error al enviar la consulta";

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult obtenerChats() {

            string json = new JavaScriptSerializer().Serialize((ListadoChats()));
            return Json(json);

        }

        [HttpPost]
        public JsonResult enviarMensaje(string contenido, int codChat, int codDestin)
        {
            Usuario remit = (Usuario)Session["usuario"];

            // Validación de seguridad
            if (remit.codigo != codDestin) {

                int rs = EnviarMensaje(remit.codigo, codDestin, contenido);
                return Json(rs, JsonRequestBehavior.AllowGet);
            }

            return Json(-1, JsonRequestBehavior.AllowGet);
        }


    }
}