using MES_DCN.Juniper;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MESInterface.JUNIPER
{
    public class AnalyesSapXml : taskBase
    {
        private string mesDBStr, buStr, keyPath, fileType, filePath, backupPath, errorPath, remotePath, sendFlag, sendFilePath, sendRemotePath, testKeyPath, remotePath2;

        public override void init()
        {
            try
            {
                mesDBStr = ConfigGet("MESDB");
                buStr = ConfigGet("BU");
                filePath = ConfigGet("FILE_PATH");
                backupPath = ConfigGet("BACKUP_PATH");
                errorPath = ConfigGet("ERROR_PATH");
                keyPath = ConfigGet("KEY_PATH");
                remotePath = ConfigGet("REMOTE_PATH");
                fileType = ConfigGet("FILE_TYPE");
                sendFlag = ConfigGet("SEND_FLAG");
                sendFilePath = ConfigGet("SEND_FILE_PATH");
                sendRemotePath = ConfigGet("SEND_REMOTE_PATH");
                testKeyPath = ConfigGet("TEST_KEY_PATH");
                remotePath2 = ConfigGet("REMOTE_PATH2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Start()
        {
            SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
            List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
            string ip = temp[0].ToString();
            string lockName = $"AnalyesSapXml_{fileType}";

            try
            {
                R_SYNC_LOCK runingObj = SFCDB.Queryable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME == lockName).ToList().FirstOrDefault();
                if (runingObj != null)
                {
                    throw new Exception($@"{lockName} interface is running on {runingObj.LOCK_IP},Please try again later");
                }
                runingObj = new R_SYNC_LOCK();
                runingObj.ID = MesDbBase.GetNewID<R_SYNC_LOCK>(SFCDB, buStr);
                runingObj.LOCK_NAME = lockName;
                runingObj.LOCK_KEY = "1";
                runingObj.LOCK_TIME_LONG = 5;
                runingObj.EDIT_EMP = "SYSTEM";
                runingObj.LOCK_TIME = SFCDB.GetDate();
                runingObj.LOCK_IP = ip;
                SFCDB.Insertable<R_SYNC_LOCK>(runingObj).ExecuteCommand();

                AnalyseSAPFile analyse = new AnalyseSAPFile(mesDBStr, buStr, filePath, backupPath, errorPath, remotePath, keyPath, fileType, sendFlag, sendFilePath, sendRemotePath, testKeyPath, remotePath2);
                analyse.Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SFCDB.Deleteable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME == lockName && t.LOCK_IP.Equals(ip)).ExecuteCommand();
            }
        }
    }
}
