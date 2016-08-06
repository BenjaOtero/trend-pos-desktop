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
            }
            catch (PingException)
            {
                conexion = false;
            }
            return conexion;
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
            ftpServerIP = "127.0.0.1:22/datos";
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

        public static bool DownloadFileFTP(string hilo, string coco)
        {
            bool descargado = false;
            try
            {
                string inputfilepath = @"C:\Windows\Temp\datos.sql.xz";
                string ftphost = "127.0.0.1:22/datos";
                string ftpfilepath = @"/" + BL.RazonSocialBLL.GetId().ToString() + "_datos.sql.xz";
                string ftpPassword = "8953#AFjn";
                string ftpUserID = "Benja";
                string ftpfullpath = "ftp://" + ftphost + ftpfilepath;
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    request.DownloadFile(ftpfullpath, inputfilepath);
                    descargado = true;
                }
            }
            catch (WebException)
            {
                if(hilo != "frmInicio")
                {
                    MessageBox.Show("No se pudo conectar con el servidor FTP. No se actualizaron datos.", "Trend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
            return descargado;
        }

        public static bool DownloadFileFTP(string hilo)
        {
            bool descargado = false;

            string connectionString = ConfigurationManager.ConnectionStrings["FtpLocal"].ConnectionString;
            //string connectionString = ConfigurationManager.ConnectionStrings["Ftp"].ConnectionString;
            Char delimiter = ';';
            String[] substrings = connectionString.Split(delimiter);
            string ftpServerIP = substrings[0];
            string ftpUserID = substrings[1];
            string ftpPassword = substrings[2];
            string inputfilepath = @"C:\Windows\Temp\datos.sql.xz";

                string ftphost = ftpServerIP + "/datos";
                string ftpfilepath = @"/" + BL.RazonSocialBLL.GetId().ToString() + "_datos.sql.xz";
                string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            FtpWebRequest objRequest = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
            NetworkCredential objCredential = new NetworkCredential(ftpUserID, ftpPassword);
            objRequest.Credentials = objCredential;
            objRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse objResponse = (FtpWebResponse)objRequest.GetResponse();
            byte[] buffer = new byte[32768];
            using (Stream input = objResponse.GetResponseStream())
            {
                if (File.Exists(inputfilepath)) File.Delete(inputfilepath);
                using (FileStream output = new FileStream(inputfilepath, FileMode.CreateNew))
                {
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
                descargado = true;
            }
            return descargado;
        }
    }

    }


