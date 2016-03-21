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

namespace StockVentas
{
    public partial class frmPrincipal : Form
    {
        frmInicio instanciaInicio;
        public frmProgress progreso;
        string idRazonSocial;

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
            Fallidas fllds = new Fallidas();
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

        private void stockDeArtículosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStockInter newMDIChild = new frmStockInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }       

        private void arqueoDeCajaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmArqueoInter newMDIChild = new frmArqueoInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void enPesosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVentasPesosInter newMDIChild = new frmVentasPesosInter();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void ventasEnPesos_Click(object sender, EventArgs e)
        {
            frmVentasPesosInter newMDIChild = new frmVentasPesosInter();
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

        private void lotesTarjetasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVentasLotesTarjetas newMDIChild = new frmVentasLotesTarjetas();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void clientesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmClientes newMDIChild = new frmClientes();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }

        private void actualizarDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            DownloadFileFTP();
            RestaurarDatos();
            Cursor.Current = Cursors.Arrow;
        }

        private void DownloadFileFTP()
        {
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
                byte[] fileData = request.DownloadData(ftpfullpath);

                using (FileStream file = File.Create(inputfilepath))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
                MessageBox.Show("Download Complete");
            }
        }

        private void RestaurarDatos()
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
            sb.AppendLine(@"gzip -d C:\Windows\Temp\datos.sql.gz");
            sb.AppendLine(@"mysql -u ncsoftwa_re -p8953#AFjn pos_desktop < C:\Windows\Temp\datos.sql");
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

        private void RestaurarDatos_Exited(object sender, System.EventArgs e)
        {
            if (File.Exists("c:\\Windows\\Temp\\restore.bat")) File.Delete("c:\\Windows\\Temp\\restore.bat");
            if (File.Exists("c:\\Windows\\Temp\\datos.sql")) File.Delete("c:\\Windows\\Temp\\datos.sql");
            if (File.Exists("c:\\Windows\\Temp\\datos.sql.gz")) File.Delete("c:\\Windows\\Temp\\datos.sql.gz");
        }

    }
}
