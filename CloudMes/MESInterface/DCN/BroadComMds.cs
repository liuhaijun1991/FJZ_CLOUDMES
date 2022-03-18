using HWDNNSFCBase;
using MESInterface.Common;
using MESPubLab.SAP_RFC;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using MESStation.LogicObject;
using SqlSugar;
using System.Collections.Generic;
using MESDataObject.Common;
using MESPubLab.MesBase;
using MES_DCN.Broadcom;
using System.Data.OleDb;
using System.IO;
using MESDBHelper;

namespace MESInterface.DCN
{
    public class BroadComMds : taskBase
    {
        public bool IsRuning = false;
        string dbstr, bustr ,filepath,filebackpath;
        HWDNNSFCBase.OleExec db = null;
        HWDNNSFCBase.OleExec ldb = null;
        public override void init()
        {
            try
            {
                dbstr = ConfigGet("DB");
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

                MESDBHelper.OleExec MESDB =new MESDBHelper.OleExec(dbstr, false);
                string str = DateTime.Now.ToString("yyyyMMddHHmmss");
                this.WriteLog("START", "OK", str, MESDB);
                MesLog.Info("Start");
                //test();
                BroadComMdsObj broadComMds = new BroadComMdsObj(dbstr, bustr, filepath, filebackpath);
                broadComMds.Build();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MESDBHelper.OleExec MESDB = new MESDBHelper.OleExec(dbstr, false);
                string str = DateTime.Now.ToString("yyyyMMddHHmmss");
                this.WriteLog("END", "OK", str, MESDB);
                MesLog.Info("End");
                IsRuning = false;
            }
        }
        public void WriteLog(string EVENT_TYPE, string MESSAGE, string RUNTIME_ID, MESDBHelper.OleExec oleExec)
        {
            string str = this.GetNewID("VNDCN", oleExec, "R_PROCCESS_EVENT");
            string[] textArray1 = new string[] { "insert into R_PROCCESS_EVENT (ID,PROCCESS_NAME,EVENT_TYPE,MESSAGE,EVENT_LV,RUNTIME_ID,IP,STATE,EDIT_EMP,EDIT_DATE)\r\n   VALUES ('", str, "','BROADCOM_MDS','", EVENT_TYPE, "','", MESSAGE, "','','", RUNTIME_ID, "','','','SYSTEM',SYSDATE) " };
            string str2 = string.Concat(textArray1);
            oleExec.ExecSQL(str2);
        }
        public string GetNewID(string BU, MESDBHelper.OleExec DB, string TableName)
        {
            System.Data.OleDb.OleDbParameter[] parameterArray = new OleDbParameter[] { new OleDbParameter(":IN_BU", OleDbType.VarChar, 300), new OleDbParameter(":IN_TYPE", OleDbType.VarChar, 300), new OleDbParameter(":OUT_RES", OleDbType.VarChar, 500) };
            parameterArray[0].Value = BU;
            parameterArray[1].Value = TableName;
            parameterArray[2].Direction = ParameterDirection.Output;
            DB.ExecProcedureNoReturn("SFC.GET_ID", parameterArray);
            string str = parameterArray[2].Value.ToString();
            if (str.StartsWith("ERR"))
            {
                throw new Exception("獲取表'" + TableName + "' ID 異常! " + str);
            }
            return str;
        }
        private string _DBName1;


        void test()
        {
 


            //using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
            //{
            //    var waitdosnkp = db.Queryable<R_SN_KP>().Where(t => t.VALUE.Contains(",") && t.SCANTYPE == "AUTOAP")
            //        .ToList();
            //    foreach (var item in waitdosnkp)
            //    {
            //        var s = item.VALUE.Split(',');
            //        if (s.Length == 2 && s[0] == s[1])
            //        {
            //            item.VALUE = s[0];
            //        }

            //        var m = item.MPN.Split(',');
            //        if (m.Length == 2 && m[0] == m[1])
            //        {
            //            item.MPN = m[0];
            //        }

            //        db.Updateable(item).ExecuteCommand();
            //    }
            //}

            //using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
            //{
            //    string CONST_FILENAME_PRE = "XXAT";
            //    string CONST_FILENAME_PRE_CM = "B_FXVN_BD";
            //    BroadComMdsObj broadComMds = new BroadComMdsObj(dbstr, bustr, filepath, filebackpath);
            //    var itemfilename_MDSYLD =
            //        $@"{CONST_FILENAME_PRE}_MDSYLD_{CONST_FILENAME_PRE_CM}_{Convert.ToDateTime("2020-10-05").ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumExtensions.EnumValueAttribute>().Description)}.txt";
            //    var yieldobjlist = db.Queryable<R_MDS_YIELD>().Where(t => Convert.ToInt32(t.YIELD_DATE) < Convert.ToInt32("20201006000000")).ToList();
            //    var resMDSYLD = broadComMds.GanarationFile(yieldobjlist, filepath, itemfilename_MDSYLD);

            //    var itemfilename_MDSSTR =
            //        $@"{CONST_FILENAME_PRE}_MDSSTR_{CONST_FILENAME_PRE_CM}_{Convert.ToDateTime("2020-10-05").ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumExtensions.EnumValueAttribute>().Description)}.txt";
            //    var strobjlist = db.Queryable<R_MDS_STR>().Where(t => Convert.ToInt32(t.STRUC_DATE) < Convert.ToInt32("20201006000000")).ToList();
            //    var resMDSSTR = broadComMds.GanarationFile(strobjlist, filepath, itemfilename_MDSSTR);

            //    var itemfilename_MDSIQC =
            //        $@"{CONST_FILENAME_PRE}_MDSIQC_{CONST_FILENAME_PRE_CM}_{Convert.ToDateTime("2020-10-05").ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumExtensions.EnumValueAttribute>().Description)}.txt";
            //    var iqcobjlist = db.Queryable<R_MDS_IQC>().Where(t => Convert.ToInt32(t.RECORD_CREATION_DATE) < Convert.ToInt32("20201006000000")).ToList();
            //    var resMDSIQC = broadComMds.GanarationFile(iqcobjlist, filepath, itemfilename_MDSIQC);
            //}

            //string CONST_SFTPHost = "10.120.176.114";
            //string CONST_SFTPPort = "21";
            //string CONST_SFTPLogin = "sfc01";
            //string CONST_SFTPPassword = "1123";
            //string CONST_SFTPPath = "/ITTEMP";
            //SFTPHelper sftpHelp =
            //    new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword);
            //sftpHelp.Put($@"C:\Users\G6001953\Desktop\temp\testfile.trn", $@"{CONST_SFTPPath}/testfile.trn");

            //using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
            //{
            //    //var sql = $@" select sn,location,count(1) as nums from r_sn_kp where location like kp_name||'-%' and sn not like '#%' and valid_flag='1' group by sn,location having count(1)>1 ";
            //    //var waitlist = db.Ado.GetDataTable(sql);
            //    //foreach (DataRow item in waitlist.Rows)
            //    //{
            //    //    var targetlist = db.Queryable<R_SN_KP>().Where(t =>
            //    //        t.SN == item["sn"].ToString() && t.LOCATION == item["location"].ToString() && t.VALID_FLAG ==
            //    //        Convert.ToDouble(ENUM_R_SN_KP.VALID_FLAG_TRUE.Ext<EnumExtensions.EnumValueAttribute>()
            //    //            .Description)).ToList();
            //    //    var res = db.Ado.UseTran(() =>
            //    //    {
            //    //        for (int i = 1; i <= targetlist.Count; i++)
            //    //        {
            //    //            targetlist[i - 1].LOCATION = $@"{targetlist[i - 1].KP_NAME}-{i}";
            //    //            db.Updateable(targetlist[i - 1]).ExecuteCommand();
            //    //        }
            //    //    });
            //    //}

            //}
        }

    }
}
