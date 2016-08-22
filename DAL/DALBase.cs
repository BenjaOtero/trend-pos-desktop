using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.ServiceProcess;

namespace DAL
{
    public class DALBase
    {
        static int intentos = 0;
        static ServiceController sc;

        public DALBase()
        {
        }

        public static MySqlConnection GetConnection999()
        {
            string connectionString;
            MySqlConnection objCon;
            connectionString = ConfigurationManager.ConnectionStrings["DBMainLocal"].ConnectionString;
            //  connectionString = ConfigurationManager.ConnectionStrings["DBMainCaro"].ConnectionString;
            objCon = new MySqlConnection(connectionString);
            return objCon;
        }

        public static MySqlConnection GetConnection()
        {
            string connectionString;
            MySqlConnection objCon;
            connectionString = ConfigurationManager.ConnectionStrings["DBMainLocal"].ConnectionString;
            //  connectionString = ConfigurationManager.ConnectionStrings["DBMainCaro"].ConnectionString;
            objCon = new MySqlConnection(connectionString);
        abrirConexion:
            try
            {
                objCon.Open();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042)
                {
                    if (intentos == 0)
                    {
                        if (ExisteServicioMySQL())
                        {
                            IniciarServicioMysql();
                            intentos++;
                            goto abrirConexion;
                        }
                    }
                    else
                    {
                        intentos = 0;
                        throw new ServidorMysqlInaccesibleException("No se pudo conectar con el servidor de base de datos."
                            + '\r' + "Consulte al administrador del sistema.", ex);
                    }
                }
            }
            return objCon;
        }

        public static MySqlConnection GetDumpPosConnection()
        {
            string connectionString;
            MySqlConnection objCon;
            connectionString = ConfigurationManager.ConnectionStrings["DBDumpPos"].ConnectionString;
            objCon = new MySqlConnection(connectionString);
        abrirConexion:
            try
            {
                objCon.Open();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042)
                {
                    if (intentos == 0)
                    {
                        if (ExisteServicioMySQL())
                        {
                            IniciarServicioMysql();
                            intentos++;
                            goto abrirConexion;
                        }
                    }
                    else
                    {
                        intentos = 0;
                        throw new ServidorMysqlInaccesibleException("No se pudo conectar con el servidor de base de datos.", ex);
                    }
                }
            }
            return objCon;
        }

        public static MySqlConnection GetRemoteConnection()
        {
            string connectionString;
            MySqlConnection objCon;
            //connectionString = ConfigurationManager.ConnectionStrings["DBMain"].ConnectionString;
            //connectionString = ConfigurationManager.ConnectionStrings["DBPruebas"].ConnectionString;
            connectionString = ConfigurationManager.ConnectionStrings["NcSoftwa_local"].ConnectionString;
            objCon = new MySqlConnection(connectionString);
            return objCon;
        }

        public static OleDbConnection GetConnectionAccess()
        {
            string connectionString;
            OleDbConnection objCon;
            connectionString = ConfigurationManager.ConnectionStrings["DBAccess"].ConnectionString;
            objCon = new OleDbConnection(connectionString);
            return objCon;
        }

        private static void IniciarServicioMysql()
        {
            ServiceController sc = new ServiceController("MySQL");
            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                sc.Start();
            }
        }

        private static bool ExisteServicioMySQL()
        {
            bool existeServicio = false;
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName == "MySQL")
                {
                    existeServicio = true;
                    continue;
                }
            }
            return existeServicio;
        }

        public static bool ValidarServicioMysql()
        {
            bool funcionando = true;
            sc = new ServiceController("MySQL");
            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                sc.Start();
                sc = new ServiceController("MySQL");
                if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    funcionando = false;
                }
            }
            return funcionando;
        }
    }
}
