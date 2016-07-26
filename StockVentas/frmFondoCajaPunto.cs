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
    public partial class frmFondoCajaPunto : Form
    {
        DataSet dsFondoCaja;
        DataTable tblFondoCaja;
        DataTable tblLocales;
        DataView viewFondoCaja;
        DataRowView rowView;
        public string PK = string.Empty;
        string fecha;
        private int? codigoError = null;

        public frmFondoCajaPunto()
        {
            InitializeComponent();
        }

        private void frmFondoCaja_Load(object sender, EventArgs e)
        {
            this.Location = new Point(50, 50);
            System.Drawing.Icon ico = Properties.Resources.icono_app;
            this.Icon = ico;
            lblClave.ForeColor = System.Drawing.Color.DarkRed;
            lblFecha.Text = DateTime.Today.ToLongDateString();
            fecha = DateTime.Today.ToString("yyyy-MM-dd");
            dsFondoCaja = BL.FondoCajaBLL.CrearDataset(fecha, 1);
            tblFondoCaja = dsFondoCaja.Tables[1];
            viewFondoCaja = new DataView(tblFondoCaja);         
            if (viewFondoCaja.Count == 0)
            {
                viewFondoCaja.RowStateFilter = DataViewRowState.Added;
                Random rand = new Random();
                int clave = rand.Next(1, 2000000000);
                lblClave.Text = clave.ToString();
                lblClave.ForeColor = System.Drawing.Color.DarkRed;
                DataTable tblPcs = BL.PcsBLL.CrearDataset();
                rowView = viewFondoCaja.AddNew();
                rowView["IdFondoFONP"] = clave;
                rowView["FechaFONP"] = DateTime.Today;
                rowView["IdPcFONP"] = tblPcs.Rows[0]["IdPC"];
                rowView["ImporteFONP"] = 0;
                rowView.EndEdit();
            }
            else
            {
                rowView = viewFondoCaja[0];
            }
            lblClave.DataBindings.Add("Text", rowView, "IdFondoFONP", false, DataSourceUpdateMode.OnPropertyChanged);
            txtImporte.DataBindings.Add("Text", rowView, "ImporteFONP", true, DataSourceUpdateMode.OnPropertyChanged);
            txtImporte.Focus();
            txtImporte.KeyPress += new KeyPressEventHandler(BL.Utilitarios.SoloNumerosConComa);
            txtImporte.KeyDown += new System.Windows.Forms.KeyEventHandler(BL.Utilitarios.EnterTab);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtImporte.Text == "") return;
            rowView.EndEdit();
            Cursor.Current = Cursors.WaitCursor;
            BL.FondoCajaBLL.GrabarDB(dsFondoCaja, ref codigoError, false);
            Cursor.Current = Cursors.Arrow;
            Close();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmFondoCajaPunto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lblClave_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtImporte_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

    }
}
 