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
            int pc = BL.PcsBLL.GetId();
            fecha = DateTime.Today.ToString("yyyy-MM-dd");
            dsFondoCaja = BL.FondoCajaBLL.CrearDataset(fecha, pc);  // poner el nro de pc desde tabla pcs
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
            if (!BL.Utilitarios.ValidarServicioMysql())
            {
                MessageBox.Show("No se pudo conectar con el servidor de base de datos."
                + '\r' + "No se grabaron los datos.", "Trend Sistemas",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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

    }
}
 