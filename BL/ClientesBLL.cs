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
    public class ClientesBLL 
    {
        private static object _sync = new object();

        public static DataSet GetClientes(sbyte frm)
        {
            DataSet dt = DAL.ClientesDAL.GetClientes(frm);            
            return dt;
        }

        public static void GrabarDB(DataSet dt, DataTable tblFallidas ,ref int? codigoError, bool grabarFallidas)
        {
            try
            {
                DAL.ClientesDAL.GrabarDB(dt, grabarFallidas);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    dt.RejectChanges(); ;
                    codigoError = 1042;
                }
                else
                {
                    dt.RejectChanges();
                    codigoError = ex.Number;
                }
            }
        }

        // Inserta los datos remotos obtenidos del servidor al iniciar la aplicación (Tablas articulos, clientes, FormasPago)
        public static void InsertRemotos(DataSet dt)
        {
            MySqlTransaction tr = null;
            try
            {
                MySqlConnection SqlConnection1 = DALBase.GetConnection();
                tr = SqlConnection1.BeginTransaction();
                DAL.ClientesDAL.InsertRemotos(dt, SqlConnection1, tr);
                tr.Commit();
                SqlConnection1.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Trend", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dt.RejectChanges();
                tr.Rollback();
            }
        }


    }
}
