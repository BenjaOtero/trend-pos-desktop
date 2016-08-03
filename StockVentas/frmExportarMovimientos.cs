using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BL;

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
            string strFecha = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            Cursor.Current = Cursors.WaitCursor;
            string pc = BL.PcsBLL.GetId().ToString();
            string idRazonSocial = BL.RazonSocialBLL.GetId().ToString() + "_pc" + pc + "_" + strFecha + ".sql.gz";
            BL.DatosPosBLL.ExportAll(strFecha);
            Utilitarios.ExportarDatos(idRazonSocial);
            
        }
    }
}
