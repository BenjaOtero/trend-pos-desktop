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
            dateTimePicker1.TabStop = false;
            cmbLocal.TabStop = false;
            #region comboLocal

            tblLocales = new DataTable();
            DataColumn IdPc = new DataColumn();
            IdPc.ColumnName = "IdPc";
            tblLocales.Columns.Add(IdPc);
            DataColumn NombreLOC = new DataColumn();
            NombreLOC.ColumnName = "NombreLOC";
            tblLocales.Columns.Add(NombreLOC);
            DataRow row = tblLocales.NewRow();
            row["IdPc"] = 1;
            row["NombreLOC"] = "JESUS MARIA";
            tblLocales.Rows.Add(row);
            cmbLocal.ValueMember = "IdPC";
            cmbLocal.DisplayMember = "NombreLOC";
            cmbLocal.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocal.DataSource = tblLocales;
            cmbLocal.SelectedValue = 1;

            #endregion

            fecha = dateTimePicker1.Value.ToString("yyyy-MM-dd");
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
                rowView = viewFondoCaja.AddNew();
                rowView["IdFondoFONP"] = clave;
                rowView["FechaFONP"] = DateTime.Today;
                rowView["IdPcFONP"] = 1; // Jesus Maria
                rowView["ImporteFONP"] = 0;
                rowView.EndEdit();
            }
            else
            {
                rowView = viewFondoCaja[0];
            }
            lblClave.DataBindings.Add("Text", rowView, "IdFondoFONP", false, DataSourceUpdateMode.OnPropertyChanged);
            dateTimePicker1.DataBindings.Add("Text", rowView, "FechaFONP", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbLocal.DataBindings.Add("SelectedValue", rowView, "IdPcFONP", false, DataSourceUpdateMode.OnPropertyChanged);
            txtImporte.DataBindings.Add("Text", rowView, "ImporteFONP", false, DataSourceUpdateMode.OnPropertyChanged);
            txtImporte.Focus();
            txtImporte.KeyPress += new KeyPressEventHandler(BL.Utilitarios.SoloNumerosConComa);
            txtImporte.KeyDown += new System.Windows.Forms.KeyEventHandler(BL.Utilitarios.EnterTab);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtImporte.Text == "") return;
            rowView.EndEdit();
            if (tblFondoCaja.GetChanges() != null)
            {
                Cursor.Current = Cursors.WaitCursor;
                BL.FondoCajaBLL.GrabarDB(dsFondoCaja, ref codigoError, false);
                Cursor.Current = Cursors.Arrow;
            }
            else
            {
                dsFondoCaja.RejectChanges();
            }
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
 