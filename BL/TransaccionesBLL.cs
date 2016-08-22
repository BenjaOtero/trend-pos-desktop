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
using System.Diagnostics;


namespace BL
{
    public class TransaccionesBLL
    {
        private static object _sync = new object();

        public TransaccionesBLL()
        {
        }

        // constructor para grabar inserciones de registros
        public static void GrabarVentas(DataSet dtVentas, ref int? codigoError, bool grabarFallidas)
        {
            MySqlTransaction tr = null;
            try
            {
                MySqlConnection SqlConnection1 = DALBase.GetConnection();
                tr = SqlConnection1.BeginTransaction();
                DAL.VentasDAL.GrabarDB(dtVentas, SqlConnection1, tr);
                DAL.VentasDetalleDAL.GrabarDB(dtVentas, SqlConnection1, tr);
                tr.Commit();
                SqlConnection1.Close();

            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    dtVentas.RejectChanges();
                    codigoError = 1042;
                }
                else
                {
                    dtVentas.RejectChanges();
                    if (tr != null)
                    {
                        tr.Rollback();
                    }
                    codigoError = ex.Number;
                }
            }
            catch (TimeoutException)
            {
                dtVentas.RejectChanges();
            }
        }

        // constructor para grabar ediciones de registros
        public static void GrabarVentas(DataSet dtVentas, ref int? codigoError, 
            DataView viewDetalleOriginal, DataTable tblActual, bool grabarFallidas)
        {
            MySqlTransaction tr = null;
            try
            {
                MySqlConnection SqlConnection1 = DALBase.GetConnection();
                tr = SqlConnection1.BeginTransaction();
                DAL.VentasDAL.GrabarDB(dtVentas, SqlConnection1, tr);
                DAL.VentasDetalleDAL.GrabarDB(dtVentas, SqlConnection1, tr);
                tr.Commit();
                SqlConnection1.Close();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042) //no se pudo abrir la conexion por falta de internet
                {
                    dtVentas.RejectChanges();
                    codigoError = 1042;
                }
                else
                {
                    dtVentas.RejectChanges();
                    if (tr != null)
                    {
                        tr.Rollback();
                    }
                    codigoError = ex.Number;
                }
            }
        }

        // borrar ventas desde el arqueo de caja
        public static void BorrarVentasByPK(int PK, ref int? codigoError, bool borrarRemotas)
        {
            try
            {
                DAL.VentasDAL.BorrarByPK(PK, borrarRemotas);
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
