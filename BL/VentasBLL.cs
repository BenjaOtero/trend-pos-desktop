﻿using System;
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


namespace BL
{
    public class VentasBLL
    {
        public static DataTable GetTabla()
        {
            DataTable tbl = new DataTable();
            tbl.TableName = "Ventas";
            tbl.Columns.Add("IdVentaVEN", typeof(int));
            tbl.Columns.Add("IdPCVEN", typeof(int));
            tbl.Columns.Add("FechaVEN", typeof(DateTime));
            tbl.Columns.Add("IdClienteVEN", typeof(int));
            tbl.PrimaryKey = new DataColumn[] { tbl.Columns["IdVentaVEN"] };
            return tbl;
        }

        public static DataSet CrearDatasetForaneos()
        {
            DataSet dt = DAL.VentasDAL.CrearDatasetForaneos();
            return dt;
        }

        public static DataSet CrearDatasetArqueo(string fechaDesde, string fechaHasta, int pc, ref int? codigoError)
        {
            DataSet dt = null;
            try
            {
                dt = DAL.VentasDAL.CrearDatasetArqueo(fechaDesde, fechaHasta, pc);
                dt.Tables[0].TableName = "Ventas";
                dt.Tables[1].TableName = "VentasDetalle";
                dt.Tables[3].TableName = "TesoreriaMovimientos";
                return dt;
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
            return dt;
        }

        public static DataSet CrearDatasetVentasPesos(int forma, string desde, string hasta, string locales, ref int? codigoError)
        {
            try
            {
                DataSet dt = DAL.VentasDAL.CrearDatasetVentasPesos(forma, desde, hasta, locales);
                return dt;
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
                DataSet dt = new DataSet();
                dt = null;
                return dt;
            }            
        }

        public static DataTable GetVentasPesosDiarias(string desde, string hasta, int local, string forma)
        {
            DataTable tbl = DAL.VentasDAL.GetVentasPesosDiarias(desde, hasta, local, forma);
            return tbl;
        }

        public static DataSet CrearDatasetVentas(int idVenta)
        {
            DataSet dt = DAL.VentasDAL.CrearDatasetVentas(idVenta);
            return dt;
        }

        public static DataSet GetLotesTarjetas()
        {
            DataSet dt = DAL.VentasDAL.GetLotesTarjetas();
            return dt;
        }
    }
}

