﻿using MESDataObject;
using MESDataObject.Module;
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
    class SendCommitOrderFile : taskBase
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
                CommitOrder commit = new CommitOrder(localPath, remotePath, buStr, SFCDB, ip);
                commit.Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
    }
}
