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
using System.ServiceProcess;
using System.Diagnostics;


namespace BL
{
    public class DatosPosBLL
    {

        public static DataSet GetAll()
        {
            DataSet ds = DAL.DatosPosDAL.GetAll();
            return ds;
        }

        public static void ExportAll(string fecha)
        {
            DAL.DatosPosDAL.ExportAll(fecha);
        }

        // ACTUALIAR DATOS

        public static void DeleteAll()
        {
            DAL.DatosPosDAL.DeleteAll();
        }

        public static void ActualizarBD(string hilo)
        {
            try
            {
                if (Utilitarios.HayInternet())
                {
                    if (Backup())
                    {
                        if (Utilitarios.DownloadFileFTP(hilo))  //tratar errores en DownloadFileFTP()
                        {
                            DeleteAll();
                            RestaurarDatos();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Verifique la conexión a internet. No se actualizaron datos.", "Trend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (!SeActualizaronDatos())
                {
                    RestaurarBD();
                    ActualizarBD(hilo);
                }
            }
        }

        private static bool Backup()
        {
            bool bckp = false;
            try
            {
                string archivo = @"C:\Windows\Temp\backup.sql";
                System.IO.StreamWriter sw = System.IO.File.CreateText("c:\\Windows\\Temp\\backup.bat"); // creo el archivo .bat
                sw.Close();
                StringBuilder sb = new StringBuilder();
                string path = Application.StartupPath;
                string unidad = path.Substring(0, 2);
                sb.AppendLine(unidad);
                sb.AppendLine(@"cd " + path + @"\Mysql");
                //  sb.AppendLine(@"mysqldump --skip-comments -u ncsoftwa_re -p8953#AFjn -h ns21a.cyberneticos.com --opt ncsoftwa_re > " + fichero.FileName);
                sb.AppendLine(@"mysqldump --skip-comments -u ncsoftwa_re -p8953#AFjn -h localhost --routines --opt pos_desktop > " + archivo);
                using (StreamWriter outfile = new StreamWriter("c:\\Windows\\Temp\\backup.bat", true)) // escribo el archivo .bat
                {
                    outfile.Write(sb.ToString());
                }
                Process process = new Process();
                process.StartInfo.FileName = "c:\\Windows\\Temp\\backup.bat";
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
                process.Exited += new EventHandler(Backup_Exited);
                process.Start();
                process.WaitForExit();
                FileInfo f = new FileInfo(archivo);
                if (f.Length > 0) bckp = true;
            }
            catch (Exception)
            {
                // IOException (backup.bat está siendo utilizado por otro proceso
            }
            return bckp;
        }

        private static void Backup_Exited(object sender, System.EventArgs e)
        {
            if (File.Exists("c:\\Windows\\Temp\\backup.bat")) File.Delete("c:\\Windows\\Temp\\backup.bat");
        }

        private static void RestaurarDatos()
        {
            // gzip -d n:\2147483647_datos.sql.gz
            // mysql -u ncsoftwa_re -p8953#AFjn pos < n:\2147483647_datos.sql  
            /*
             * crear archivo bat
             * escribir archivo bat con sentencias
             * ejecutar bat
             * borrar bat
             */
            if (File.Exists(@"C:\Windows\Temp\datos.sql")) File.Delete(@"C:\Windows\Temp\datos.sql");
            System.IO.StreamWriter sw = System.IO.File.CreateText("c:\\Windows\\Temp\\restore.bat"); // creo el archivo .bat
            sw.Close();
            StringBuilder sb = new StringBuilder();
            string path = Application.StartupPath;
            string unidad = path.Substring(0, 2);
            sb.AppendLine(unidad);
            sb.AppendLine(@"cd " + path + @"\Mysql");
            sb.AppendLine("gzip -d \"C:\\Windows\\Temp\\datos.sql.gz\"");
            sb.AppendLine("mysql -u ncsoftwa_re -p8953#AFjn pos_desktop < \"C:\\Windows\\Temp\\datos.sql\"");
            using (StreamWriter outfile = new StreamWriter("c:\\Windows\\Temp\\restore.bat", true)) // escribo el archivo .bat
            {
                outfile.Write(sb.ToString());
            }
            Process process = new Process();
            process.StartInfo.FileName = "c:\\Windows\\Temp\\restore.bat";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
            process.Exited += new EventHandler(RestaurarDatos_Exited);
            process.Start();
            process.WaitForExit();
        }

        private static void RestaurarDatos_Exited(object sender, System.EventArgs e)
        {
            if (File.Exists("c:\\Windows\\Temp\\restore.bat")) File.Delete("c:\\Windows\\Temp\\restore.bat");
            //  if (File.Exists("c:\\Windows\\Temp\\datos.sql")) File.Delete("c:\\Windows\\Temp\\datos.sql");
            //  if (File.Exists("c:\\Windows\\Temp\\datos.sql.gz")) File.Delete("c:\\Windows\\Temp\\datos.sql.gz");
        }

        private static void RestaurarBD()
        {
            DAL.DatosPosDAL.BorrarCrearBD();
            RestaurarDatosBD();
        }

        private static void RestaurarDatosBD()
        {
            // gzip -d n:\2147483647_datos.sql.gz
            // mysql -u ncsoftwa_re -p8953#AFjn pos < n:\2147483647_datos.sql  
            /*
             * crear archivo bat
             * escribir archivo bat con sentencias
             * ejecutar bat
             * borrar bat
             */
            if (File.Exists("c:\\Windows\\Temp\\restore.bat")) File.Delete("c:\\Windows\\Temp\\restore.bat");
            System.IO.StreamWriter sw = System.IO.File.CreateText("c:\\Windows\\Temp\\restore.bat"); // creo el archivo .bat
            sw.Close();
            StringBuilder sb = new StringBuilder();
            string path = Application.StartupPath;
            string unidad = path.Substring(0, 2);
            sb.AppendLine(unidad);
            sb.AppendLine(@"cd " + path + @"\Mysql");
            sb.AppendLine("mysql -u ncsoftwa_re -p8953#AFjn pos_desktop < \"C:\\Windows\\Temp\\backup.sql\"");
            using (StreamWriter outfile = new StreamWriter("c:\\Windows\\Temp\\restore.bat", true)) // escribo el archivo .bat
            {
                outfile.Write(sb.ToString());
            }
            Process process = new Process();
            process.StartInfo.FileName = "c:\\Windows\\Temp\\restore.bat";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
            process.Exited += new EventHandler(RestaurarDatosBD_Exited);
            process.Start();
            process.WaitForExit();
        }

        private static void RestaurarDatosBD_Exited(object sender, System.EventArgs e)
        {
            //    if (File.Exists("c:\\Windows\\Temp\\backup.bat")) File.Delete("c:\\Windows\\Temp\\backup.bat");
        }

        private static bool SeActualizaronDatos()
        {
            bool seActualizaron = true;
            DataSet ds = DAL.DatosPosDAL.ControlarUpdate();
            int records;
            foreach (DataTable tbl in ds.Tables)
            {
                records = Convert.ToInt16(tbl.Rows[0][0].ToString());
                if (records == 0)
                {
                    seActualizaron = false;
                    break;
                }
            }
            return seActualizaron;
        }

        // END ACTUALIAR DATOS
    }
}
