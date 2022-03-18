using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.VERTIV
{
    public class SendShipment : taskBase
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
                MESStation.Interface.Vertiv.SendShipment shipment = new MESStation.Interface.Vertiv.SendShipment(localPath, remotePath, buStr, SFCDB, ip);
                shipment.Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
