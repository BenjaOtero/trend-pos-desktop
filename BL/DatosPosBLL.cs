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
    public class DatosPosBLL
    {
        public DataSet dt;
        public DataTable tblClientes;

        public static DataSet GetAll()
        {
            DataSet ds = DAL.DatosPosDAL.GetAll();
            return ds;
        }

        public static void ExportAll()
        {
            DAL.DatosPosDAL.ExportAll();
        }

        public static string GetFechaExport()
        {
            string fecha = DAL.DatosPosDAL.GetFechaExport();
            return fecha;
        }

        public static DataTable GetFechaSubidos(string fechaSubida)
        {
            DataTable tblFecha = DAL.DatosPosDAL.GetFechaSubidos(fechaSubida);
            return tblFecha;
        }

        public static void InsertFechaSubidos(string fechaSubida, DataTable tblFecha)
        {
            DAL.DatosPosDAL.InsertFechaSubidos(fechaSubida, tblFecha);
        }

        public static void DeleteAll(Int16 existenClientesFallidas)
        {
            DAL.DatosPosDAL.DeleteAll(existenClientesFallidas);
        }

    }
}
