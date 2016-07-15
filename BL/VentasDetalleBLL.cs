using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;
using DAL;


namespace BL
{
    public class VentasDetalleBLL
    {
        public static DataTable GetTabla()
        {
            DataTable tbl = DAL.VentasDetalleDAL.GetTabla();
            return tbl;
        }

        public static void GrabarDB(DataSet dt, ref int? codigoError, bool grabarFallidas)
        {
            MySqlTransaction tr = null;
            try
            {
                MySqlConnection SqlConnection1 = DALBase.GetConnection();
                SqlConnection1.Open();
                tr = SqlConnection1.BeginTransaction();
                DAL.VentasDetalleDAL.GrabarDB(dt, SqlConnection1, tr);
                tr.Commit();
                SqlConnection1.Close();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    codigoError = 1042;
                }
                else
                {
                    if (tr != null)
                    {
                        tr.Rollback();
                    }
                    codigoError = ex.Number;
                }
            }
        }

        public static DataSet GetSchema(int idVenta)
        {
            DataSet dt = DAL.VentasDetalleDAL.GetSchema(idVenta);
            return dt;
        }

    }
}
