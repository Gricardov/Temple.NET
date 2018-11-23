
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Temple.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace Temple.Controllers
{
    public class TransaccionController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString);

        // GET: Transaccion
        public ActionResult Resumen(int idSub,int idCat, int idMod, Int64 inicioHorario, Int64 finHorario, int codInstr)
        {
            Usuario u =(Usuario) Session["usuario"];
            ViewBag.usuario = u;

            Evento e = new Evento();
            e.inicio = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(inicioHorario);
            e.fin = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(finHorario);

            Pedido p = ObtenerPedido(codInstr, idCat, idSub, idMod);
            p.horario = e;

            return View(p);
        }

        public Pedido ObtenerPedido(int codInstr, int idCat, int idSub, int idMod) {

            Pedido p = new Pedido();
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_OBTENER_PEDIDO", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODINSTR", codInstr);
            cmd.Parameters.AddWithValue("@IDCAT", idCat);
            cmd.Parameters.AddWithValue("@IDSUB", idSub);
            cmd.Parameters.AddWithValue("@IDMOD", idMod);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Debug.WriteLine(codInstr + " " + idCat + " " + idSub + " " + idMod);
                p.codInstr = codInstr;
                p.idCat = idCat;
                p.idSub = idSub;
                p.idMod = idMod;
                p.nombresInstructor = reader.GetString(0);
                p.nombreCurso = reader.GetString(1);
                p.precioHora = reader.GetDecimal(2);

            }

            con.Close();
            reader.Close();

            return p;

        }
    }
}