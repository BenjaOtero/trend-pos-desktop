using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace DAL
{
    public class StockDAL
    {

        public static DataTable GetStock(int intLocal, string parametro)
        {
            MySqlConnection SqlConnection1 = DALBase.GetConnection();
            MySqlDataAdapter SqlDataAdapter1 = new MySqlDataAdapter();
            MySqlCommand SqlSelectCommand1 = new MySqlCommand("Stock_Cons", SqlConnection1);
            SqlDataAdapter1.SelectCommand = SqlSelectCommand1;
            SqlSelectCommand1.Parameters.AddWithValue("p_local", intLocal);
            SqlSelectCommand1.Parameters.AddWithValue("p_parametro", parametro);
            SqlSelectCommand1.CommandType = CommandType.StoredProcedure;
            DataTable tbl = new DataTable();
            SqlDataAdapter1.Fill(tbl);
            SqlConnection1.Close();
            return tbl;
        }

        public static DataSet CrearDataset(string whereLocales, int proveedor, string articulo, string descripcion)
        {
            string genero = string.Empty;
            MySqlConnection SqlConnection1 = DALBase.GetConnection();
            MySqlDataAdapter SqlDataAdapter1 = new MySqlDataAdapter();
            MySqlCommand SqlSelectCommand1 = new MySqlCommand("Stock_Cons", SqlConnection1);
            SqlDataAdapter1.SelectCommand = SqlSelectCommand1;
            SqlSelectCommand1.Parameters.AddWithValue("p_locales", whereLocales);
            SqlSelectCommand1.Parameters.AddWithValue("p_genero", genero);
            SqlSelectCommand1.Parameters.AddWithValue("p_proveedor", proveedor);
            SqlSelectCommand1.Parameters.AddWithValue("p_articulo", articulo);
            SqlSelectCommand1.Parameters.AddWithValue("p_descripcion", descripcion);
            SqlSelectCommand1.Parameters.AddWithValue("p_activoWeb", 0);
            SqlSelectCommand1.CommandType = CommandType.StoredProcedure;
            DataSet dt = new DataSet();
            SqlDataAdapter1.Fill(dt, "Stock");
            SqlConnection1.Close();
            return dt;
        }

    }

}
