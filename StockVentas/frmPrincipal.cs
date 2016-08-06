using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BL;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System.Timers;
using System.Configuration;
using System.IO.Compression;


namespace StockVentas
{
    public partial class frmPrincipal : Form
    {
        frmInicio instanciaInicio;
        public frmProgress progreso;
        string idRazonSocial;
        int n = 0;
        int fallidos = 0;
        System.Timers.Timer tmrUpload = new System.Timers.Timer();
        System.Timers.Timer tmrDownload = new System.Timers.Timer();

        public frmPrincipal(frmInicio instanciaInicio)
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
            {
                MdiClient client = control as MdiClient;
                if (!(client == null))
                {
                    client.BackColor = this.BackColor;
                    break;
                }
            }
            this.instanciaInicio = instanciaInicio;
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            System.Drawing.Icon ico = Properties.Resources.icono_app;
            this.Icon = ico;
        }

        private void ventasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmVentas newMDIChild = new frmVentas();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
            Cursor.Current = Cursors.Arrow;
        }

        private void movimientosDeTesoreríaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTesoreriaMov newMDIChild = new frmTesoreriaMov();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void fondoDeCajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFondoCajaPunto newMDIChild = new frmFondoCajaPunto();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void clientesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmClientes newMDIChild = new frmClientes();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }        

        private void arqueoDeCajaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmArqueoInter newMDIChild = new frmArqueoInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void btnArticulos_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmArticulos articulos = new frmArticulos();
            articulos.Show();
            Cursor.Current = Cursors.Arrow;
        }

        private void lotesTarjetasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVentasLotesTarjetas newMDIChild = new frmVentasLotesTarjetas();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void stockDeArtículosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStockInter newMDIChild = new frmStockInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void enPesosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVentasPesosInter newMDIChild = new frmVentasPesosInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void ventasToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmVentasPesosInter newMDIChild = new frmVentasPesosInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void actualizarDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProgress frm = new frmProgress("ActualizarDatos", "grabar");
            frm.ShowDialog();

          /*  tmrUpload = new System.Timers.Timer(1000);
            tmrUpload.Elapsed += new ElapsedEventHandler(ProbarUpload);
            tmrUpload.Enabled = true;*/

        }

        private void salirToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            instanciaInicio.cerrando = true;
            instanciaInicio.Visible = true;
        }

        private void ProbarUpload(object source, ElapsedEventArgs e)
        {
            tmrUpload.Enabled = false;

            if (File.Exists("c:\\Windows\\Temp\\1663670023_datos.sql.xz")) File.Delete("c:\\Windows\\Temp\\1663670023_datos.sql.xz");
            if (File.Exists("c:\\Windows\\Temp\\1663670023_datos.sql")) File.Delete("c:\\Windows\\Temp\\1663670023_datos.sql");

                 // COMPRIMO ARCHIVO
            Process processComprimir = new Process();
            processComprimir.StartInfo.FileName = "c:\\Windows\\Temp\\comprimir.bat";
            processComprimir.StartInfo.CreateNoWindow = false;
            processComprimir.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processComprimir.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
            processComprimir.Start();
            processComprimir.WaitForExit();            

                 // SUBO ARCHIVO
            string connectionString = ConfigurationManager.ConnectionStrings["FtpLocal"].ConnectionString;
            //string connectionString = ConfigurationManager.ConnectionStrings["Ftp"].ConnectionString;
            Char delimiter = ';';
            String[] substrings = connectionString.Split(delimiter);
            string ftpServerIP = substrings[0];
            string ftpUserID = substrings[1];
            string ftpPassword = substrings[2];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1:22/datos/1663670023_datos.sql.xz");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            byte[] fileContents = File.ReadAllBytes(@"n:\1663670023_datos.sql.xz");
            request.ContentLength = fileContents.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();

            System.Threading.Thread.Sleep(1500);

            // DESCARGO ARCHIVO
            FtpWebRequest objRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://127.0.0.1:22/datos/1663670023_datos.sql.xz");
            NetworkCredential objCredential = new NetworkCredential(ftpUserID, ftpPassword);
            objRequest.Credentials = objCredential;
            objRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse objResponse = (FtpWebResponse)objRequest.GetResponse();
            byte[] buffer = new byte[32768];
            using (Stream input = objResponse.GetResponseStream())
            {
                using (FileStream output = new FileStream(@"C:\Windows\Temp\1663670023_datos.sql.xz", FileMode.CreateNew))
                {
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }

            if (FileCompare(@"n:\1663670023_datos.sql.xz", @"C:\Windows\Temp\1663670023_datos.sql.xz"))
            {
                // BORRO DATOS
            //    BL.DatosPosBLL.DeleteAll();

                //RESTAURO DATOS
                Process processRestaurar = new Process();
                processRestaurar.StartInfo.FileName = "c:\\Windows\\Temp\\restore.bat";
                processRestaurar.StartInfo.CreateNoWindow = false;
                processRestaurar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processRestaurar.EnableRaisingEvents = true;  // permite disparar el evento process_Exited
                processRestaurar.Start();
                processRestaurar.WaitForExit();

                // CONTROLO ACTUALIZACION
                DataSet ds = DAL.DatosPosDAL.ControlarUpdate();
                int records;
                foreach (DataTable tbl in ds.Tables)
                {
                    records = Convert.ToInt16(tbl.Rows[0][0].ToString());
                    if (records == 0)
                    {
                        MessageBox.Show("Que cagada ! ! !");
                        tmrUpload.Enabled = false;
                        tmrUpload.Interval = 0;
                    }
                }

                n++;
                tmrUpload.Enabled = true;
                tmrUpload.Interval = 1000;
            }
            else
            {
                n++;
                fallidos++;
                tmrUpload.Enabled = true;
                tmrUpload.Interval = 1000;
                return;
            }



        }

        private void exportarMovimientosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmExportarMovimientos newMDIChild = new frmExportarMovimientos();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }






    }
}
