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
                    DatosPosBLL.ExportAll(strFecha);
                    DatosPosBLL.ExportarDatos(strFecha);
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
