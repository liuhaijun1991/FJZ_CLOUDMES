using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using MES_DCN.Aruba;

namespace MESInterface.FSJ
{
    public class FsjArubaSendSNRB2BSyn : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, apdbstr, bustr, filepath, remotepath, datatype, keytype;

        public override void init()
        {
            mesdbstr = ConfigGet("MESDB");
            apdbstr = ConfigGet("APDB");
            bustr = ConfigGet("BU");
            datatype = ConfigGet("DATA_TYPE");
            keytype = ConfigGet("KEY_TYPE");
            filepath = ConfigGet("FILE_PATH");
            remotepath = ConfigGet("REMOTE_PATH");

            if (string.IsNullOrEmpty(datatype) || string.IsNullOrEmpty(keytype))
            {
                throw new Exception("Please setup DATA_TYPE and KEY_TYPE in config.ini");
            }

            if (keytype != "DAILY" && keytype != "WEEKLY" && keytype != "MONTHLY")
            {
                throw new Exception("Please setup KEY_TYPE is (DAILY/WEEKLY/MONTHLY) in config.ini");
            }

            if (datatype != "SNR_BEFORE" && datatype != "SNR_AFTER")
            {
                throw new Exception("Please setup DATA_TYPE is (SNR_BEFORE/SNR_AFTER) in config.ini");
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                FsjArubaSNRObj arubaSNR = new FsjArubaSNRObj(mesdbstr, apdbstr, bustr, filepath, remotepath, datatype, keytype);
                arubaSNR.Build();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }

    }
}
