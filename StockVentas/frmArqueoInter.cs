﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BL;

namespace StockVentas
{
    public partial class frmArqueoInter : Form
    {

        public frmArqueoInter()
        {
            InitializeComponent();            
            this.MaximizeBox = false;
            dateTimeFecha.Value = DateTime.Today;
        }

        private void frmArqueoInter_Load(object sender, EventArgs e)
        {
            this.Location = new Point(50, 50);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!BL.Utilitarios.ValidarServicioMysql())
            {
                string mensaje = "No se pudo establecer la conexión con el servidor de base de datos.";
                MessageBox.Show(this, mensaje, "Trend Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            frmArqueoCaja arqueo = new frmArqueoCaja(dateTimeFecha);
            arqueo.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
