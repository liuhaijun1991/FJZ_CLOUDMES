using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MesBase;
using MESPubLab.Common;
using MESStation.Config;
using MESDBHelper;

namespace MESInterface.DCN
{
    class UpdateBroadcomPO:taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr;
        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
            }
            catch (Exception)
            {
            }
        }
        public override void Start()
        {
            OleExec MESDB = new OleExec(mesdbstr, false);

            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                ShippingScheduleConfig.UPDATE_Broadcom_PO(MESDB, "VNDCN", "SYSTEM");
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
