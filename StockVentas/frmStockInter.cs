using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BL;

namespace StockVentas
{
    public partial class frmStockInter : Form
    {
        DataTable tblStock;
        DataTable tblLocales;
        DataTable dtCruzada;        
        int intLocal;
        string parametro;

        public frmStockInter()
        {
            InitializeComponent();
        }

        private void frmStockMovInter_Load(object sender, EventArgs e)
        {
            this.Location = new Point(50, 50);
            tblLocales = BL.LocalesBLL.CrearDataset();
            intLocal = (int)tblLocales.Rows[0]["IdLocalLOC"];
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DataTable tblStock;
            try
            {
                parametro = txtParametros.Text;
                tblStock = BL.StockBLL.GetStock(intLocal, parametro);
            }
            catch
            {
                MessageBox.Show("No se encontraron artículos coincidentes", "Trend", MessageBoxButtons.OK, MessageBoxIcon.Information);                 
                return;
            }
            frmStockInforme stockInforme = new frmStockInforme(tblStock);
            stockInforme.ShowDialog();
            Cursor.Current = Cursors.Arrow;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
