using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESInterface.Common;
using MESPubLab.Common;
using MESStation.Interface.Vertiv;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MESInterface.VERTIV
{
    public class AnalyseForecast : taskBase
    {
        public string localPath = "";
        public string remotePath = "";
        public string buStr = "";
        private string mesDBStr = "";
        SqlSugarClient SFCDB = null;
        public override void init()
        {           
            mesDBStr = ConfigGet("DB");
            buStr = ConfigGet("BU");
            localPath = ConfigGet("LocalPath");
            remotePath = ConfigGet("RemotePath");
            SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);            
        }

        public override void Start()
        {            
            SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
            List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
            string ip = temp[0].ToString();
            try
            {                
                AnalyesForecast forecast = new AnalyesForecast(localPath, remotePath, buStr, SFCDB,ip);
                forecast.Run();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SFCDB.Deleteable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals("AnalyseForecast") && t.LOCK_IP.Equals(ip)).ExecuteCommand();
            }
        }
    }
}
