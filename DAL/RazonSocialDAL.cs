using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class RazonSocialDAL
    {
        public static int GetId()
        {
            DataTable tblRazon = new DataTable();
            int idRazon = 0;
            MySqlConnection SqlConnection1 = DALBase.GetConnection();
            MySqlDataAdapter SqlDataAdapter1 = new MySqlDataAdapter();
            MySqlCommand SqlSelectCommand1 = new MySqlCommand("RazonSocial_GetId", SqlConnection1);
            SqlDataAdapter1.SelectCommand = SqlSelectCommand1;
            SqlSelectCommand1.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter1.Fill(tblRazon);
            idRazon = Convert.ToInt32(tblRazon.Rows[0][0].ToString());
            return idRazon;        
        }

    }
}