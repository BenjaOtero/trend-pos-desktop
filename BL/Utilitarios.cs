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

        public static void UploadFromFile(string nombreLocal, string nombreServidor)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["FtpLocal"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["Ftp"].ConnectionString;
            Char delimiter = ';';
            String[] substrings = connectionString.Split(delimiter);
            string ftpServerIP = substrings[0];
            string ftpUserID = substrings[1];
            string ftpPassword = substrings[2];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpServerIP + nombreServidor);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            byte[] fileContents = File.ReadAllBytes(nombreLocal);
            request.ContentLength = fileContents.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        public static void DownloadFile(string nombreLocal, string nombreServidor)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["FtpLocal"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["Ftp"].ConnectionString;
            Char delimiter = ';';
            String[] substrings = connectionString.Split(delimiter);
            string ftpServerIP = substrings[0];
            string ftpUserID = substrings[1];
            string ftpPassword = substrings[2];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpServerIP + nombreServidor);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            FtpWebResponse objResponse = (FtpWebResponse)request.GetResponse();
            byte[] buffer = new byte[32768];
            using (Stream input = objResponse.GetResponseStream())
            {
                if (File.Exists(nombreLocal)) File.Delete(nombreLocal);
                using (FileStream output = new FileStream(nombreLocal, FileMode.CreateNew))
                {
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }
            objResponse.Close();
        }

        public static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;
            if (file1 == file2)
            {
                return true;
            }
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);
            if (fs1.Length != fs2.Length)
            {
                fs1.Close();
                fs2.Close();
                return false;
            }
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));
            fs1.Close();
            fs2.Close();
            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static bool DownloadFileFTP(string hilo)
        {
            bool descargado = false;

            //string connectionString = ConfigurationManager.ConnectionStrings["FtpLocal"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["Ftp"].ConnectionString;
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

        public static bool ValidarServicioMysql()
        {
            bool funcionando = DAL.DALBase.ValidarServicioMysql();
            return funcionando;
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

    }

}


