using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BL;


namespace StockVentas
{
    public partial class frmProgress : Form
    {
    //    private frmProgress instancia;
        public DataSet dt = null;
        public static DataSet dtEstatico = null;
        public static DataSet dsStockMov = null;
        public static DataSet dsArqueo = null;
        public static DataSet dsVentasPesosCons = null;
        public static DataTable tblEstatica = null;
        private DataRowView rowView = null;
        private string origen = null;
        private string accion = null;
        private string where = null;
        private int? codigoError = null;
        private int idLocal;
        private int idPc;
        private int PK;
        private string strFechaDesde;
        private string strFechaHasta;
        private string opcMov;
        private string tipo;
        private int forma;
        private string formaPago;
        private string locales;
        private int proveedor;
        private string articulo = null;
        private string descripcion = null;
        private bool actualizaDatos;


        public frmProgress()
        {
            InitializeComponent();
        }

        public frmProgress(string origen, string accion): this()
        {
        //    instancia = this;
            this.origen = origen;
            this.accion = accion;
        }

        public frmProgress(string origen, string accion, string where): this()
        {
       //     instancia = this;
            this.origen = origen;
            this.accion = accion;
            this.where = where;
        }

        public frmProgress(DataSet dt, string origen, string accion): this()
        {
        //    instancia = this;
             this.dt = dt;
             this.origen = origen;
             this.accion = accion;
        }

        public frmProgress(DataSet dt, string origen, string accion, string where): this()
        {
         //   instancia = this;
            this.dt = dt;
            this.origen = origen;
            this.accion = accion;
            this.where = where;
        }

        public frmProgress(DataSet dt, DataSet dtStock, string origen, string accion): this()
        {
     //       instancia = this;
            this.dt = dt;
      //      this.dtStock = dtStock;
            this.origen = origen;
            this.accion = accion;
        }

        //Constructor frmStockInter
        public frmProgress(string origen, string accion, string locales, int proveedor, string articulo, string descripcion)
            : this()
        {
            this.origen = origen;
            this.accion = accion;
            this.locales = locales;
            this.proveedor = proveedor;
            this.articulo = articulo;
            this.descripcion = descripcion;
        }

        // Constructor para frmVentasPesosCons
        public frmProgress(int forma, string strFechaDesde, string strFechaHasta, string locales, string origen, string accion)
            : this()
        {
            this.forma = forma;
            this.strFechaDesde = strFechaDesde;
            this.strFechaHasta = strFechaHasta;
            this.locales = locales;
            this.origen = origen;
            this.accion = accion;
        }

        // Constructor para frmVentasPesosDiarias
        public frmProgress(string strFechaDesde, string strFechaHasta, int local, string forma, string origen, string accion)
            : this()
        {
            this.strFechaDesde = strFechaDesde;
            this.strFechaHasta = strFechaHasta;
            this.idLocal = local;
            this.formaPago = forma;
            this.origen = origen;
            this.accion = accion;
        }

        private void frmProgress_Shown(object sender, EventArgs e)
        {
            if (accion == "cargar")
            {                 
                label1.Text = "Cargando datos...";
                label1.Left = 108;
            }
            if (actualizaDatos == true)
            {
              //  actualizarDatos();
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (accion == "cargar")
            {
                switch (origen)
                {
                    case "frmArticulos":
                        BL.ArticulosBLL.CrearDataset();
                        break;
                    case "frmStock":
                        dtEstatico = BL.StockBLL.CrearDataset(locales, proveedor, articulo, descripcion, ref codigoError);
                        break;
                    case "frmVentasPesosCons":
                        dsVentasPesosCons = BL.VentasBLL.CrearDatasetVentasPesos(forma, strFechaDesde, strFechaHasta, locales, ref codigoError);
                        break;
                    case "frmVentasPesosInter_diarias":
                        tblEstatica = BL.VentasBLL.GetVentasPesosDiarias(strFechaDesde, strFechaHasta, idLocal, formaPago);
                        break;
                }
            }
            else  //grabar en base de datos
            {                
                switch (origen)
                {
                    case "ActualizarDatos":
                        if (Utilitarios.HayInternet())
                        {
                            BL.DatosPosBLL.ActualizarBD("frmProgress");
                        }
                        else
                        {
                            this.Invoke((Action)delegate
                            {
                                MessageBox.Show("Verifique la conexión a internet. No se actualizaron datos.", "Trend", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                            });                            
                        }
                        break;
                 /*   case "frmArqueoCajaAdmin_borrarVenta":
                        BL.VentasBLL.BorrarByPK(PK, ref codigoError);
                        break;*/
                    case "frmClientes":
                      //  BL.ClientesBLL.GrabarDB(dt);
                        break;
                    case "frmVentas":
                  //      BL.TransaccionesBLL.GrabarVentas(dt, rowView, ref codigoError);
                        break;
                    case "frmTesoreriaMov":
                     //   BL.TesoreriaMovimientosBLL.GrabarDB(dt, ref codigoError);
                        break;
                    case "frmFondoCaja":
                   //     BL.FondoCajaBLL.GrabarDB(dt, ref codigoError);
                        break;
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (codigoError == null)
            {
                this.Close();
            }
            else if (codigoError == 1042)
            {
                this.Visible = false;
                if (accion == "grabar")
                {
                    MessageBox.Show("No se pudo establecer la conexión con el servidor (verifique la conexión a internet). No se guardaron los cambios.",
                        "Trend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("No se pudo establecer la conexión con el servidor (verifique la conexión a internet).",
                            "Trend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
            else
            {
                this.Visible = false;
                MessageBox.Show("No se guardaron los cambios. Se produjo un error inesperado. Consulte al administrador del sistema",
                "Trend", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

    }
}
