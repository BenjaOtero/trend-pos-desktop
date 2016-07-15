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
    public class TesoreriaMovimientosBLL
    {

        public static DataSet CrearDataset()
        {
            DataSet dt = DAL.TesoreriaMovimientosDAL.CrearDataset();
            return dt;
        }

        public static DataSet CrearDatasetMovimiento(int idMov)
        {
            DataSet ds = new DataSet();
            ds = DAL.TesoreriaMovimientosDAL.CrearDatasetMovimiento(idMov);
            return ds;
        }

        public static void GrabarDB(DataSet dt, ref int? codigoError, bool grabarFallidas)
        {
            try
            {
                DataSet dsRemoto;
                dsRemoto = dt.GetChanges();
                DAL.TesoreriaMovimientosDAL.GrabarDB(dt, grabarFallidas);
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

        public static void BorrarByPK(int PK, ref int? codigoError, bool borrarRemotas)
        {
            try
            {
                DAL.TesoreriaMovimientosDAL.BorrarByPK(PK, borrarRemotas);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    codigoError = 1042;
                }
                else
                {
                    codigoError = ex.Number;
                }
            }
        }

    }
}
