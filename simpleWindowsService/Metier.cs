using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace Metier
{
    public class oMetier
    {
        private string MysqlHost;
        private string MysqlUser;
        private string MysqlPass;
        private string MysqlPort;
        private string MysqlBase;
        private static readonly ILog Log = LogManager.GetLogger(typeof(oMetier));
        private MySqlConnection connection;

        public oMetier(ILog LogParam)
        {
            GetApplicationParameters();            
        }

        public void GetApplicationParameters()
        {
            MysqlHost = System.Configuration.ConfigurationManager.AppSettings["MysqlHost"].ToString();
            MysqlUser = System.Configuration.ConfigurationManager.AppSettings["MysqlUser"].ToString();
            MysqlPass = System.Configuration.ConfigurationManager.AppSettings["MysqlPass"].ToString();
            MysqlPort = System.Configuration.ConfigurationManager.AppSettings["MysqlPort"].ToString();
            MysqlBase = System.Configuration.ConfigurationManager.AppSettings["MysqlBase"].ToString();
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                string connectionString;
                connectionString = "SERVER=" + MysqlHost + ";" + "DATABASE=" + MysqlBase + ";" + "UID=" + MysqlUser + ";" + "PASSWORD=" + MysqlPass + ";";
                // Log.Info(connectionString);
                connection = new MySqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Log.Info("OpenConnection FAILED :" + ex.Message);                               
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Log.Info("CloseConnection FAILED :" + ex.Message);
                return false;
            }
        }

        public Boolean test()
        {
            return true;
        }      
    }    
}
