﻿using System;
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
            dgvDatos.DataSource = datos;
        }

        private void frmStockInforme_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}