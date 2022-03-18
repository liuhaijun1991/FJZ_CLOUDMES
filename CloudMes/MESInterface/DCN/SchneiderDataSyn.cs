using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using MES_DCN.Schneider;

namespace MESInterface.DCN
{
    public class SchneiderDataSyn: taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, customerdbstr, bustr, seriesname, Series, days, getSISN;

        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                customerdbstr = ConfigGet("CUSTOMERDB");
                bustr = ConfigGet("BU");
                seriesname = ConfigGet("SERIESNAME");
                days = ConfigGet("DAYS");
                getSISN = ConfigGet("GETSISN");
                if (seriesname == "")
                {
                    throw new Exception("請設置要傳送的系列！");
                }
                string[] list = seriesname.Split(',');
                for (var i = 0; i < list.Length; i++)
                {
                    Series += $@",'{list[i]}'";
                }
                Series = Series.Substring(1);
            }
            catch (Exception)
            {
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
                SchneiderDataObj schneiderData = new SchneiderDataObj(mesdbstr, customerdbstr, bustr, Series, days, getSISN);
                schneiderData.Build();
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
