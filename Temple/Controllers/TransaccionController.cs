
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
        public ActionResult Resumen(int idSub, int idCat, int idMod, Int64 inicioHorario, Int64 finHorario, int codInstr)
        {

            Usuario u = (Usuario)Session["usuario"];
            ViewBag.usuario = u;

            Evento e = new Evento();
            e.inicio = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(inicioHorario);
            e.fin = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(finHorario);

            Pedido p = ObtenerPedido(codInstr, idCat, idSub, idMod);
            p.horario = e;
            p.cantHoras = calcularCantidadHoras(inicioHorario, finHorario);
            p.precioTotal = calcularPrecioTotal(p.cantHoras, p.precioHora);

            Session["pedido"] = p;

            return View(p);
        }

        public ActionResult RegistrarTransaccion() {
            ViewBag.usuario = (Usuario)Session["usuario"];

            Pedido p = ((Pedido)Session["pedido"]);
            if (p != null)
            {

                int rs = RegistrarPedido(p);
                ViewBag.confirmacion = true;
                if (rs != -1)
                {

                    ViewBag.mensaje = "Pedido registrado";

                }
                else
                {

                    ViewBag.mensaje = "Error al registrar";
                }
                return View();
            }
            else {

                return RedirectToAction("Inicio", "Alumno");
            }



        }

        public int RegistrarPedido(Pedido p) {
            int rs = -1;
            con.Open();
            SqlTransaction transaccion = con.BeginTransaction();

            SqlCommand cmd = new SqlCommand("USP_REGISTRAR_TRANSACCION", con,transaccion);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CODINSTR", p.codInstr);
            cmd.Parameters.AddWithValue("@CODALUMNO", ((Usuario) Session["usuario"]).codigo);
            cmd.Parameters.AddWithValue("@IDCAT", p.idCat);
            cmd.Parameters.AddWithValue("@IDSUB", p.idSub);
            cmd.Parameters.AddWithValue("@IDMOD", p.idMod);
            cmd.Parameters.AddWithValue("@PRECIOHORA", p.precioHora);
            cmd.Parameters.AddWithValue("@INICIO", p.horario.inicio);
            cmd.Parameters.AddWithValue("@FIN", p.horario.fin);
            cmd.Parameters.AddWithValue("@TOTAL", p.precioTotal);

            rs = cmd.ExecuteNonQuery();

            transaccion.Commit();

            con.Close();

            return rs;

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
                p.codInstr = codInstr;
                p.idCat = idCat;
                p.idSub = idSub;
                p.idMod = idMod;
                p.nombresInstructor = reader.GetString(0);
                p.nombreCurso = reader.GetString(1);
                p.nombreModalidad = reader.GetString(2);
                p.precioHora = Convert.ToDouble(reader.GetDecimal(3));

            }

            con.Close();
            reader.Close();

            return p;

        }

        private double calcularCantidadHoras(Int64 inicioHorario, Int64 finHorario) {
            Int64 segInicio = inicioHorario / 1000;
            Int64 segFin = finHorario / 1000;
            
            double cantHor = (segFin - segInicio)/3600.0;
            Debug.WriteLine("cantHor "+cantHor);

            return Math.Round(Convert.ToDouble(cantHor),1);

        }

        private double calcularPrecioTotal(double cantHoras, double precioHora) {

            return cantHoras * precioHora;
        }
    }
}