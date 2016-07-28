using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Management;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;


namespace BL
{
    public class Utilitarios
    {
        static Button grabar;
        static DataTable tblTabla;
        static string idRazonSocial;

        public static void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.')
            {
                e.Handled = false;
            }
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }

        public static void SoloNumerosConComa(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ',')
            {
                e.Handled = false;
            }
            if (e.KeyChar == '-')
            {
                e.Handled = false;
            }
            if (e.KeyChar == '.')
            {
                // si se pulsa en el punto se convertirá en coma
                e.Handled = true; //anula la tecla "." pulsada
                SendKeys.Send(",");
            }
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }

        public static DataTable Pivot(DataTable dt, DataColumn pivotColumn, DataColumn pivotValue)
        {
            // find primary key columns 
            //(i.e. everything but pivot column and pivot value)
            DataTable temp = dt.Copy();
            temp.Columns.Remove(pivotColumn.ColumnName);
            temp.Columns.Remove(pivotValue.ColumnName);
            string[] pkColumnNames = temp.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToArray();

            // prep results table
            DataTable result = temp.DefaultView.ToTable(true, pkColumnNames).Copy();
            result.PrimaryKey = result.Columns.Cast<DataColumn>().ToArray();

            dt.AsEnumerable()
                .Select(r => r[pivotColumn.ColumnName].ToString())
                .Distinct().ToList()
                .ForEach(c => result.Columns.Add(c, pivotColumn.DataType));

            // load it
            foreach (DataRow row in dt.Rows)
            {
                // find row to update
                DataRow aggRow = result.Rows.Find(
                    pkColumnNames
                        .Select(c => row[c])
                        .ToArray());
                // the aggregate used here is LATEST 
                // adjust the next line if you want (SUM, MAX, etc...)
                aggRow[row[pivotColumn.ColumnName].ToString()] = row[pivotValue.ColumnName];
            }

            return result;
        }

        public static bool HayInternet()
        {
            bool conexion = false;
            Ping Pings = new Ping();
            int timeout = 3000;
            try
            {
                if (Pings.Send("ns21a.cyberneticos.com", timeout).Status == IPStatus.Success)
                {
                    conexion = true;
                }
                conexion = true;
            }
            catch (PingException)
            {
                conexion = true;
            }
            return conexion;
        }

        public static void SelTextoTextBox(object sender, EventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)sender;
                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }
            catch (InvalidCastException)
            {
                MaskedTextBox tb = (MaskedTextBox)sender;
                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }
        }

        public static void EnterTab(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) SendKeys.Send("{TAB}");
        }

        public static void AddEventosABM(Control grpCampos, ref Button btnGrabar, ref DataTable tbl)
        {
            tblTabla = tbl;
         //   tblTabla.ColumnChanged += new DataColumnChangeEventHandler(HabilitarGrabar);
            grabar = btnGrabar;
            foreach (Control ctl in grpCampos.Controls)
            {
                if (ctl is TextBox || ctl is MaskedTextBox)
                {
                    ctl.Enter += new System.EventHandler(SelTextoTextBox);
                    ctl.KeyDown += new System.Windows.Forms.KeyEventHandler(EnterTab);
                //    ctl.TextChanged += new System.EventHandler(HabilitarGrabar);
                }
            }
        }

        public static void HabilitarGrabar(object sender, EventArgs e)
        {
            if (grabar.Enabled == false)
            {
                grabar.Enabled = true;
            }
        }

        public static void DataBindingsAdd(BindingSource bnd, GroupBox grp)
        {
            foreach (Control ctl in grp.Controls)
            {
                if (ctl is TextBox)
                {
                    string campo = ctl.Name.Substring(3, ctl.Name.Length - 3);
                    ctl.DataBindings.Add("Text", bnd, campo, false, DataSourceUpdateMode.OnPropertyChanged);
                }
                else if (ctl is CheckBox)
                {
                    string campo = ctl.Name.Substring(3, ctl.Name.Length - 3);
                    //   ctl.DataBindings.Add("YesNo", bnd, campo, false, DataSourceUpdateMode.OnPropertyChanged);                
                }
            }
        }

        public static void ValidarComboBox(object sender, CancelEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (!string.IsNullOrEmpty(cmb.Text))
            {
                if (cmb.SelectedValue == null)
                {
                    e.Cancel = true;
                }
            }
        }

        public static void ExportarDatos(string idRazonSocial)
        {
            Utilitarios.idRazonSocial = idRazonSocial;
            System.IO.StreamWriter sw = System.IO.File.CreateText("c:\\Windows\\Temp\\export.bat"); // creo el archivo .bat
            sw.Close();
            StringBuilder sb = new StringBuilder();
            string path = Application.StartupPath;
            string unidad = path.Substring(0, 2);
            sb.AppendLine(unidad);
            sb.AppendLine(@"cd " + path + @"\Mysql");
            sb.AppendLine(@"mysqldump --skip-comments -u ncsoftwa_re -p8953#AFjn -h localhost --opt pos_desktop exportar_fondo_caja exportar_tesoreria_movimientos exportar_ventas exportar_ventas_detalle | gzip > c:\windows\temp\" + idRazonSocial);
            using (StreamWriter outfile = new StreamWriter("c:\\Windows\\Temp\\export.bat", true)) // escribo el archivo .bat
            {
                outfile.Write(sb.ToString());
            }
            Process process = new Process();
            process.StartInfo.FileName = "c:\\Windows\\Temp\\export.bat";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
            process.Exited += new EventHandler(ExportarDatos_Exited);
            process.Start();
            process.WaitForExit();
        }

        private static void ExportarDatos_Exited(object sender, System.EventArgs e)
        {
            UploadDatosPos(@"c:\windows\temp\" + idRazonSocial, idRazonSocial);
            if (File.Exists("c:\\Windows\\Temp\\export.bat")) File.Delete("c:\\Windows\\Temp\\export.bat");
            if (File.Exists(@"c:\windows\temp\" + idRazonSocial)) File.Delete(@"c:\windows\temp\" + idRazonSocial);
        }

        public static void UploadDatosPos(string nombreLocal, string nombreServidor)
        {
            string ftpServerIP;
            string ftpUserID;
            string ftpPassword;

            /*  ftpServerIP = "trendsistemas.com/datos";
              ftpUserID = "benja@trendsistemas.com";
              ftpPassword = "8953#AFjn";*/

            // FTP local
            ftpServerIP = "127.0.0.1:22";
            ftpUserID = "Benja";
            ftpPassword = "8953#AFjn";

            FileInfo fileInf = new FileInfo(nombreLocal);
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + nombreServidor));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload Error");
            }
        }

        public static void ActualizarBD()
        {
            try
            {
                Backup();
                if (DownloadFileFTP())
                {
                    BL.DatosPosBLL.DeleteAll();
                    RestaurarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (!SeActualizaronDatos())
                {
                    RestaurarBD(); 
                    ActualizarBD();
                }                
            }
        }

        private static void RestaurarBD()
        {
            DAL.DatosPosDAL.BorrarBD();
        }

        private static bool DownloadFileFTP()
        {
            bool descargado = false;
            /* 
             string inputfilepath = @"C:\Windows\Temp\datos.sql.gz";
             string ftphost = "trendsistemas.com";
             string ftpfilepath = @"/" + BL.RazonSocialBLL.GetId().ToString() + "_datos.sql.gz";
             string ftpPassword = "8953#AFjn";
             string ftpUserID = "benja@trendsistemas.com";
             */

            string inputfilepath = @"C:\Windows\Temp\datos.sql.gz";
            string ftphost = "127.0.0.1:22";
            string ftpfilepath = @"/" + BL.RazonSocialBLL.GetId().ToString() + "_datos.sql.gz";
            string ftpPassword = "8953#AFjn";
            string ftpUserID = "Benja";
            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                request.DownloadFile(ftpfullpath, inputfilepath);
                descargado = true;
            }
            return descargado;
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

        private static void Backup()
        {
            string archivo = @"C:\Windows\Temp\backup.sql";
            System.IO.StreamWriter sw = System.IO.File.CreateText("c:\\Windows\\Temp\\backup.bat"); // creo el archivo .bat
            sw.Close();
            StringBuilder sb = new StringBuilder();
            string path = Application.StartupPath;
            string unidad = path.Substring(0, 2);
            sb.AppendLine(unidad);
            sb.AppendLine(@"cd " + path + @"\Backup");
            //  sb.AppendLine(@"mysqldump --skip-comments -u ncsoftwa_re -p8953#AFjn -h ns21a.cyberneticos.com --opt ncsoftwa_re > " + fichero.FileName);
            sb.AppendLine(@"mysqldump --skip-comments -u ncsoftwa_re -p8953#AFjn -h localhost --routines --opt pos_desktop > " + archivo);
            //mysqldump -u... -p... mydb t1 t2 t3 > mydb_tables.sql
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
        }

        private static void Backup_Exited(object sender, System.EventArgs e)
        {
            if (File.Exists("c:\\Windows\\Temp\\backup.bat")) File.Delete("c:\\Windows\\Temp\\backup.bat");
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
    }

}
