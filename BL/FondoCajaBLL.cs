using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using DAL;
using System.Threading;

namespace BL
{
    public class FondoCajaBLL
    {

        public static DataSet CrearDataset()
        {
            DataSet dt = DAL.FondoCajaDAL.CrearDataset();
            return dt;
        }

        public static DataSet CrearDataset(string fecha, int idPc)
        {
            DataSet dt = DAL.FondoCajaDAL.CrearDataset(fecha, idPc);
            dt.Tables[1].TableName = "FondoCaja";
            dt.Tables[2].TableName = "TesoreriaMovimientos";
            return dt;
        }

        public static void GrabarDB(DataSet dt, ref int? codigoError, bool grabarFallidas)
        {
            MySqlTransaction tr = null;
            try
            {
                MySqlConnection SqlConnection1 = DALBase.GetConnection();
                SqlConnection1.Open();
                DAL.FondoCajaDAL.GrabarDB(dt, SqlConnection1);
                SqlConnection1.Close();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    dt.RejectChanges();
                    codigoError = 1042;
                }
                else
                {
                    dt.RejectChanges();
                    if (tr != null)
                    {
                        tr.Rollback();
                    }
                    codigoError = ex.Number;
                }
            }
        }

    }
}
