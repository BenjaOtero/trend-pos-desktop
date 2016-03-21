using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;
using DAL;

namespace BL
{
    public class RazonSocialBLL
    {

        public static int GetId()
        {
            int idRazon = DAL.RazonSocialDAL.GetId();
            return idRazon;        
        }

    }
}
