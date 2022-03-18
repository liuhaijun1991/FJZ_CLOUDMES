using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using MES_DCN.Aruba;

namespace MESInterface.DCN
{
    public class ArubaSendSNRB2BSyn : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, apdbstr, bustr, seriesname, Series, filepath, filebackpath, remotepath, keytype;

        public override void init()
        {
            //try
            //{
            mesdbstr = ConfigGet("MESDB");
            apdbstr = ConfigGet("APDB");
            bustr = ConfigGet("BU");
            seriesname = ConfigGet("SERIESNAME");
            filepath = ConfigGet("FILEPATH");
            filebackpath = ConfigGet("FILEPATHBACKPATH");
            remotepath = ConfigGet("REMOTEPATH");
            keytype = ConfigGet("KEY_TYPE");
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
            if (keytype != "DAILY" && keytype != "WEEKLY" && keytype != "MONTHLY")
            {
                throw new Exception("Please setup KEY_TYPE is (DAILY/WEEKLY/MONTHLY) in config.ini");
            }
            //}
            //catch (Exception)
            //{
            //}
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
                ArubaSNRObj arubaSNR = new ArubaSNRObj(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, seriesname, keytype);
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
