using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BL;
using System.Net;

namespace StockVentas
{
    public partial class frmExportarMovimientos : Form
    {
        public frmExportarMovimientos()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (BL.Utilitarios.ValidarServicioMysql())
            {
                try
                {
                    string strFecha = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    Cursor.Current = Cursors.WaitCursor;
                    string pc = BL.PcsBLL.GetId().ToString();
                    string idRazonSocial = BL.RazonSocialBLL.GetId().ToString() + "_pc" + pc + "_" + strFecha + ".sql.gz";
                    DatosPosBLL.ExportAll(strFecha);
                    DatosPosBLL.ExportarDatos(idRazonSocial);
                }
                catch (WebException)
                {
                    MessageBox.Show("No se pudo conectar con el servidor."
                    + '\r' + "No se exportaron los datos.", "Trend Sistemas",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else 
            {
                MessageBox.Show("No se pudo conectar con el servidor de base de datos."
                                + '\r' + "Consulte al administrador del sistema.", "Trend Sistemas", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
    }
}
