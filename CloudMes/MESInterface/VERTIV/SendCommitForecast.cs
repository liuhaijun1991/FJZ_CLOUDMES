using MESDBHelper;
using MESPubLab.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.Vertiv;
using System.IO;
using MESDataObject.Module;
using System.Threading;
using MESDataObject;
using MESStation.Interface.Vertiv;

namespace MESInterface.VERTIV
{
    public class SendCommitForecast : taskBase
    {
        public string localPath = "";
        public string remotePath = "";
        public string buStr = "";
        private string mesDBStr = "";
        SqlSugarClient SFCDB = null;
        public override void init()
        {
            mesDBStr = ConfigGet("DB");
            localPath = ConfigGet("LocalPath");
            remotePath = ConfigGet("RemotePath");
            buStr = ConfigGet("BU");
            SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
        }
        public override void Start()
        {
           
            SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);          
            List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
            string ip = temp[0].ToString();
            try
            {                
                CommitForecast commit = new CommitForecast(localPath, remotePath, buStr, SFCDB,ip);
                commit.Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
