using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockVentas
{
    public partial class frmStockInforme : Form
    {
        public frmStockInforme(DataTable datos)
        {
            InitializeComponent();
            bindingSource1.DataSource = datos;
            bindingNavigator1.BindingSource = bindingSource1;
            dgvDatos.DataSource = bindingSource1;
            dgvDatos.Columns["IdArticuloSTK"].HeaderText = "Artículo";
            dgvDatos.Columns["DescripcionART"].HeaderText = "Descripción";
            dgvDatos.Columns["CantidadSTK"].HeaderText = "Cantidad";
        }

        private void frmStockInforme_Load(object sender, EventArgs e)
        {            
            this.CenterToScreen();
        }

    }
}
