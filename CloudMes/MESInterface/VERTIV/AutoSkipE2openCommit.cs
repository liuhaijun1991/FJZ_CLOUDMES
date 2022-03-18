using MESDBHelper;
using MESStation.Interface.Vertiv;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.VERTIV
{
    public class AutoSkipE2openCommit : taskBase
    {
        private string mesDBStr = "";
        private int waitHour = 24;
        SqlSugarClient SFCDB = null;
        public override void init()
        {
            mesDBStr = ConfigGet("DB");
            try
            {
                waitHour = Convert.ToInt32(ConfigGet("WaitHour"));
            }
            catch (Exception ex)
            {
                waitHour = 24;
            }            
            SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
        }
        public override void Start()
        {
            SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
            List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
            string ip = temp[0].ToString();
            try
            {
                VertivB2B b2b = new VertivB2B();
                b2b.AutoSkipE2openCommit(SFCDB, waitHour, ip);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
