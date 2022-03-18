using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MesBase;
using MESDataObject.Common;
using System.Data;
using MESPubLab.Common;

namespace MESInterface.JUNIPER
{
    public class JnpOrderDataTrans : testTesk
    {
        public string bu, dbstr,site;
        public bool IsRuning = false;
        public MESJuniper.SendData.SendCentralData CentralData;
        MESInterface.Common.FileHelp fileHelp = new MESInterface.Common.FileHelp();
        #region center ftp
        string CONST_SFTPHost = "ftp://10.134.177.75";
        string CONST_SFTPPort = "21";
        string CONST_SFTPLogin = "Mesftpuser";
        string CONST_SFTPPassword = "q3cp0wYz";
        string CONST_SFTPPath = "/MX_MES_FORMAL/";
        #endregion

        public override void init()
        {
            InitPara();
        }

        /// <summary>
        /// 加載配置參數
        /// </summary>
        void InitPara()
        {
            bu = ConfigGet("BU");
            dbstr = ConfigGet("MESDB");
            site = ConfigGet("SITE");
            CentralData = new MESJuniper.SendData.SendCentralData();
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("下載正在執行,請稍後再試");
            }
            IsRuning = true;
            try
            {
                OrderData();
            }
            catch (Exception e)
            {
                MesLog.Error(e.Message);
            }
            finally
            {
                IsRuning = false;
            }
        }

        void OrderData()
        {

            var filedate = new Func<DateTime>(()=> {
                var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                // 机器本地时间 -> 中国时间
                var chinaTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, chinaZone);
                return chinaTime;
            })();
            var fullfilename = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Jnp\\";
            var filenameMESOrder = $@"{site}_MES_ORDER_{filedate.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.txt";
            var filenameWoDetailReleased = $@"{site}_WO_DETAIL_RELEASED_{filedate.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.txt";
            var filenameWoDetailNoReleased = $@"{site}_WO_DETAIL_NOT_RELEASE_{filedate.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.txt";
            fileHelp.CreateFile(fullfilename, filenameMESOrder, GetData(CentralData.GetMESOrderData(dbstr), new string[] {"SOID"}));
            fileHelp.CreateFile(fullfilename, filenameWoDetailReleased, GetData(CentralData.GetWoDetailReleased(dbstr)));
            fileHelp.CreateFile(fullfilename, filenameWoDetailNoReleased, GetData(CentralData.GetWoDetailNoReleased(dbstr)));
            FTPHelp ftpHelp = new FTPHelp($@"{CONST_SFTPHost}{CONST_SFTPPath}", CONST_SFTPLogin, CONST_SFTPPassword);
            ftpHelp.Upload($@"{fullfilename}{filenameMESOrder}");
            ftpHelp.Upload($@"{fullfilename}{filenameWoDetailReleased}");
            ftpHelp.Upload($@"{fullfilename}{filenameWoDetailNoReleased}");
        }
                
        public string GetData<T>(List<T> list,string[] ExceptFile=null)
        {
            StringBuilder strFileData = new StringBuilder() ;
            if (list.Count == 0)
                return strFileData.ToString();
            Type t = list.First().GetType();
            var pro = t.GetProperties();
            for (int i = 0; i < pro.Count(); i++)
            {
                if (ExceptFile!=null&&ExceptFile.Contains(pro[i].Name))
                    continue;
                var asd = pro[i].GetCustomAttributes(false);
                foreach (var aitem in asd)
                {
                    if (aitem.GetType().Name.Equals("PropertiesDesc"))
                    {
                        var ai = aitem as PropertiesDesc;
                        if (i == pro.Count() - 1)
                            strFileData.Append(ai.desc+"\r\n");
                        else
                            strFileData.Append(ai.desc+"|");
                    }
                }
            }
            foreach (var item in list)
            {
                for (int i = 0; i < pro.Count(); i++)
                {
                    if (ExceptFile != null && ExceptFile.Contains(pro[i].Name))
                        continue;
                    if (i == pro.Count() - 1)
                        strFileData.Append( pro[i].GetValue(item) == null ? "" + "\r\n" : pro[i].GetValue(item).ToString().Trim() + "\r\n");
                    else
                        strFileData.Append( pro[i].GetValue(item)==null?"" + "|" : pro[i].GetValue(item).ToString().Trim() + "|");
                }
            }
            return strFileData.ToString();
        }
    }
}
