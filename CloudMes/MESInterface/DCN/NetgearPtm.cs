using HWDNNSFCBase;
using MESInterface.Common;
using MESPubLab.SAP_RFC;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using MESStation.LogicObject;
using MES_DCN.Broadcom;
using SqlSugar;

namespace MESInterface.DCN
{
    public class NetgearPtm : taskBase
    {
        public bool IsRuning = false;
        string dbstr, bustr, filepath, filebackpath;
        OleExec db = null;
        OleExec ldb = null;
        public override void init()
        {
            try
            {
                dbstr =ConfigGet("DB");
                bustr = ConfigGet("BU");
                filepath = ConfigGet("FILEPATH");
                filebackpath = ConfigGet("FILEBACKPATH");
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
                //test();
                NetgearPtmObj broadComMds = new NetgearPtmObj(bustr,null, filepath, dbstr);
                //broadComMds.Build();
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

        void test()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                try
                {
                    var ctlobj = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t => t.ID == "VNDCN0000000004Y").ToList()
                        .FirstOrDefault();
                    NetgearPtmObj broadComMds = new NetgearPtmObj(this.bustr);
                    //var funcres = broadComMds.GanarationFileByCtl(ctlobj, "C:\\temp\\PTM", db);
                    var s = broadComMds.SendPtmData(ctlobj, "C:\\temp\\PTM",db);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

    }
}
