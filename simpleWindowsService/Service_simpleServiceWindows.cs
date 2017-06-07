using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DailyCoding.EasyTimer;
using Metier;

namespace Service_serviceWindows
{
    

    public partial class Service_simpleServiceWindows : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Service_simpleServiceWindows));
        private SocketIOClient.Client socket;
        private String str_url_nodejs;
        
        private int  flagTimer; // permet de voir s'il y a quelque chose en traitement

        private System.Threading.TimerCallback timerDelegate;
        private System.Threading.Timer serviceTimer;
        private oMetier oMetier;
      

        // contructeur
        public Service_simpleServiceWindows()
        {
                        
        }

        public void GetApplicationParameters()
        {    
            str_url_nodejs = System.Configuration.ConfigurationManager.AppSettings["str_url_nodejs"].ToString();            
        }

        protected override void OnStart(string[] args)
        {
                                  
            GetApplicationParameters();
            oMetier = new oMetier(Log);
         
            timerDelegate = new System.Threading.TimerCallback(ConnectToNodeJsServer);
            serviceTimer = new System.Threading.Timer(timerDelegate, null, 1000, System.Threading.Timeout.Infinite);

        }

        protected override void OnStop()
        {
            socket.Close();
            Log.Info("service : STOP ");
        }


        /* function qui envoie le result du count vers nodejs*/
        public Boolean startSendToNodeJs()
        {            
            EasyTimer.SetInterval(() =>
            {

                try
                {

                    if (flagTimer == 0)
                    {
                        DataSet dataSet = new DataSet("dataSet");
                        dataSet.Namespace = "NetFrameWork";
                        DataTable table = new DataTable();

                        table.TableName = "MainNode";

                        DataColumn info1 = new DataColumn("info1");
                        DataColumn info2 = new DataColumn("info2");
                        DataColumn info3 = new DataColumn("info3");
                        DataRow newRow;

                        // table.Columns.Add(idColumn);
                        table.Columns.Add(info1);
                        table.Columns.Add(info2);
                        table.Columns.Add(info3);
                        dataSet.Tables.Add(table);

                        newRow = table.NewRow();
                        newRow["info1"] = "N1test1";
                        newRow["info2"] = "N1test2";
                        newRow["info3"] = "N1test3";
                        table.Rows.Add(newRow);

                        newRow = table.NewRow();
                        newRow["info1"] = "N2test1";
                        newRow["info2"] = "N2test2";
                        newRow["info3"] = "N2test3";
                        table.Rows.Add(newRow);                        

                        dataSet.AcceptChanges();
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataSet, Newtonsoft.Json.Formatting.Indented);
                       
                        socket.Emit("test", json, "/canal1");
                        flagTimer = 0;
                        
                    }
                    else
                    {
                        Log.Info("flagTimer = 0 => cant emit");
                        // on attend que la tache en cours soit termine avant d'en relancer une autre
                    }

                }
                catch (SystemException error)
                {
                    Log.Error(error.ToString());
                }     

            }, 5000);
            return true;
        }

        private void ConnectToNodeJsServer(object state)
        {            
            try
            {
                
                socket = new SocketIOClient.Client(str_url_nodejs); 
                socket.Opened += SocketOpened;
                socket.Connect();
                socket.Connect("/canal1");

                socket.Emit("login", "{\"name\":\"simpletest\",\"groupe\":\"simpletest\"}", "/canal1");
                socket.Emit("join", "simpletest", "/canal1");                                               
            }
            catch (SystemException error)
            {
                Log.Error(error.ToString());
            }            
        }

        // START + INITIALISATION
        private void SocketOpened(object sender, EventArgs e)
        {
            Log.Info("CONNECTION TO NodeJs");                                             
            startSendToNodeJs();            
        }
    }
}
