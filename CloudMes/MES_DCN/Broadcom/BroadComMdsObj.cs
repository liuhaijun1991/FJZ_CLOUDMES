using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MES_DCN.Broadcom
{
    /// <summary>
    /// BroadCom Mds數據傳送業務對象
    /// </summary>
    public  class BroadComMdsObj
    {
        #region const
        /// <summary>
        /// BROADCOM_MDS
        /// </summary>
        private string CONST_PROGRAM_NAME = "BROADCOM_MDS";
        /// <summary>
        /// "STATION"
        /// </summary>
        private string CONST_STATION = "STATION";
        /// <summary>
        /// SCANTYPE
        /// </summary>
        private string CONST_SCANTYPE = "SCANTYPE";
        /// <summary>
        /// PARTNO
        /// </summary>
        private string CONST_PARTNO = "PARTNO";
        /// <summary>
        /// NOCHECK_MDS
        /// </summary>
        private string CONST_NOCHECK_MDS = "NOCHECK_MDS";
        /// <summary>
        /// "SKUNO"
        /// </summary>
        private string CONST_SKUNO = "SKUNO";
        /// <summary>
        /// "WO"
        /// </summary>
        private string CONST_WO = "WO";
        /// <summary>
        /// "SKU AND STATION"
        /// </summary>
        private string CONST_STATIONANDSKU = "SKU AND STATION";
        /// <summary>
        /// "ROUTENAME"
        /// </summary>
        private string CONST_ROUTENAME = "ROUTENAME";
        /// <summary>
        ///"B_FXVN_US_BD"
        /// </summary>
        private string CONST_CMCODE = "B_FXVN_US_BD";
        /// <summary>
        /// "A"
        /// </summary>
        private string CONST_ROUTEREV = "A";

        /// <summary>
        /// "KATARA,-IO"
        /// </summary>
        string[] CONST_FILTERSPEROUTE = new string[]{"KATARA","-IO"};
        /// <summary>
        /// ""
        /// </summary>
        string CONST_NULLVALUE = "";
        /// <summary>
        /// MDSYLD
        /// </summary>
        string CONST_MDSYLD = "MDSYLD";
        /// <summary>
        /// MDSSTR
        /// </summary>
        string CONST_MDSSTR = "MDSSTR";
        /// <summary>
        /// MDSIQC
        /// </summary>
        string CONST_MDSIQC = "MDSIQC";
        /// <summary>
        /// XXAT
        /// </summary>
        string CONST_FILENAME_PRE = "XXAT";
        /// <summary>
        /// B_FXVN_BD
        /// </summary>
        string CONST_FILENAME_PRE_CM = "B_FXVN_BD";
        /// <summary>
        /// MDSFILE_EXTENSION:.dat
        /// </summary>
        string CONST_MDSFILE_EXTENSION = ".dat";
        /// <summary>
        /// "/"
        /// </summary>
        string CONST_DCLC_SPLIT_CHAR = "/";
        /// <summary>
        /// ID,CREATETIME,HEADID
        /// </summary>
        string CONST_NOT_FILE_COL = "ID,CREATETIME,HEADID";
        /// <summary>
        /// "N/A"
        /// </summary>
        string CONST_NA = "N/A";

        /// <summary>
        /// BROADCOM
        /// </summary>
        private string CONST_CUSTOMER = Customer.BROADCOM.Ext<EnumValueAttribute>().Description;
        /// <summary>
        /// "SN","S/N"
        /// </summary>
        string[] CONST_SN_SCANTYPE = new string[]{"SN","S/N"};

        /// <summary>
        /// CONST_CORRENTTYPE
        /// </summary>
        enum CONST_CORRENTTYPE
        {
            /// <summary>
            /// "A"
            /// </summary>
            [EnumValue("A")]
            A,
            /// <summary>
            /// "D"
            /// </summary>
            [EnumValue("D")]
            D,
            /// <summary>
            /// X
            /// </summary>
            [EnumValue("X")]
            X
        }

        /// <summary>
        /// MDS RECORD STATUS
        /// </summary>
        enum CONST_MDSRECORDSTATUS
        {
            /// <summary>
            /// PASS
            /// </summary>
            [EnumValue("PASS")]
            Pass,
            /// <summary>
            /// FAIL
            /// </summary>
            [EnumValue("FAIL")]
            Fail,
            /// <summary>
            /// value=SYM
            /// name=SYMPTOM
            /// </summary>
            [EnumValue("SYM")]
            [EnumName("SYMPTOM")]
            Sym,
            /// <summary>
            /// value=DEF
            /// name=DEFECT
            /// </summary>
            [EnumValue("DEF")]
            [EnumName("DEFECT")]
            Def,
            /// <summary>
            /// value=SYM
            /// name=SYMPTOM
            /// </summary>
            [EnumValue("REP")]
            [EnumName("REP")]
            Rep
        }
        /// <summary>
        /// MDS WO TYPE
        /// </summary>
        enum CONST_MDSWOTYPE 
        {
            /// <summary>
            /// RMA
            /// </summary>
            [EnumValue("RMA")]
            RMA,
            /// <summary>
            /// REG
            /// </summary>
            [EnumValue("REG")]
            REG,
            /// <summary>
            /// REC
            /// </summary>
            [EnumValue("REC")]
            REC
        }
        /// <summary>
        /// MDS DATAPOINT
        /// </summary>
        enum CONST_MDSDATAPOINT
        {
            /// <summary>
            /// 210
            /// </summary>
            [EnumValue("210")]
            Yeild_RmaDataPoint,
            /// <summary>
            /// MDSYLD
            /// </summary>
            [EnumValue("MDSYLD")]
            Yeild_NotRmaDataPoint,
            /// <summary>
            /// 211
            /// </summary>
            [EnumValue("211")]
            Str_RmaDataPoint,
            /// <summary>
            /// MDSSTR
            /// </summary>
            [EnumValue("MDSSTR")]
            Str_NotRmaDataPoint,
            /// <summary>
            /// MDSIQC
            /// </summary>
            [EnumValue("MDSIQC")]
            Iqc_DataPoint,
        }
        /// <summary>
        /// MDS數據生成狀態
        /// </summary>
        enum CONST_MDS_CONVER_STATUS
        {
            /// <summary>
            /// 未生成數據
            /// 0
            /// </summary>
            [EnumValue("0")]
            NO,
            /// <summary>
            /// 已生成數據
            /// 1
            /// </summary>
            [EnumValue("1")]
            YES
        }
        enum CONST_MDS_SEND_STATUS
        {
            /// <summary>
            /// 未生成數據
            /// 0
            /// </summary>
            [EnumValue("0")]
            NO,
            /// <summary>
            /// 已生成數據
            /// 1
            /// </summary>
            [EnumValue("1")]
            YES
        }
        /// <summary>
        /// user-defined exception
        /// </summary>
        enum CONST_MDS_EXCEPTION
        {
            /// <summary>
            /// RouteName is no exist!
            /// </summary>
            [EnumValue("RouteName is no exist! ")]
            MDS100EXCEPTION
        }

        enum CONST_LOG_TYPE
        {
            /// <summary>
            /// BuildHead
            /// </summary>
            [EnumValue("BuildHead")]
            BuildHead,
            /// <summary>
            /// BuildMdsData
            /// </summary>
            [EnumValue("BuildMdsData")]
            BuildMdsData,
            /// <summary>
            /// SendMdsData
            /// </summary>
            [EnumValue("SendMdsData")]
            SendMdsData,
            [EnumValue("GanarationFile")]
            GanarationFile
        }

        #region b2b sftp
        string CONST_SFTPHost = "10.132.48.74";
        string CONST_SFTPPort = "21";
        string CONST_SFTPLogin = "Broadcom_SFC";
        string CONST_SFTPPassword = "Broadcom!";
        string CONST_SFTPPath = "/VN/Inventory";
        #endregion
        #region b2b ftp
        string CONST_FTPHost = "ftp://10.132.48.74";
        string CONST_FTPPort = "21";
        string CONST_FTPLogin = "Broadcom_SFC";
        string CONST_FTPPassword = "Broadcom!";
        string CONST_FTPPath = "/VN/Inventory/";
        #endregion

        #region vn local ftp,send file by manual with pe 
        string CONST_LOCAL_SFTPHost = "ftp://10.221.86.121";
        string CONST_LOCAL_SFTPPort = "21";
        string CONST_LOCAL_SFTPLogin = "MDS_ADMIN";
        string CONST_LOCAL_SFTPPassword = "MDS_PWD";
        string CONST_LOCAL_SFTPPath = "/";
        #endregion

        #endregion end const
        #region var
        private string _dbstr,_bustr,_filepath,_filebackpath;
        private List<string> nocheckRouteList = new List<string>();
        private List<string> nocheckStationList = new List<string>();
        private List<string> nocheckSkunoList = new List<string>();
        private List<R_F_CONTROL> nocheckskuandstationlist = new List<R_F_CONTROL>();
        private List<string> nocheckWoList = new List<string>();
        private List<string> nocheckKeyPart = new List<string>();
        private List<string> nocheckScantype = new List<string>();
        private List<C_ROUTE> routeList = new List<C_ROUTE>();
        #endregion
        public BroadComMdsObj(string dbstr,string bustr,string filepath,string filebackpath)
        {
            _dbstr = dbstr;
            _bustr = bustr;
            _filepath = filepath;
            _filebackpath = filebackpath;
        }
        public void Build()
        {
            this.BuildHead();
            this.BuildMdsData();
            this.GanarationFile();
            //this.SendMdsDataToLocalFtp();
            //this.SendMdsData();
            this.SendMdsDataToB2bFtp();
        }
        /// <summary>
        /// 創建需要傳送的數據目標
        /// </summary>
        void BuildHead()
        {
            try
            {
                using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
                {
                    //var dokey = DateTime.Now.AddDays(-60);
                    var sysdate = db.GetDate(); //取數據庫時間好點
                    var dokey = DateTime.Parse(sysdate.AddDays(-60).ToString("yyyy/MM/dd 00:00:00"));
                    var dokeystr = dokey.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.Ext<EnumValueAttribute>().Description);
                    var existsdo = db.Queryable<R_MDS_HEAD>().Where(t =>
                            SqlFunc.ToInt32(t.MDSKEY.Replace("-", "")) >= SqlFunc.ToInt32(dokeystr.Replace("-", "")))
                        .ToList();
                    //while (dokey.DayOfYear < DateTime.Now.DayOfYear)
                    while (dokey < DateTime.Parse(sysdate.ToString("yyyy/MM/dd 00:00:00")))
                    {
                        var currentkey =
                            dokey.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.Ext<EnumValueAttribute>().Description);
                        var fullfix = existsdo.FindAll(t => t.MDSKEY == currentkey).Count;
                        if (fullfix != 3)
                        {
                            var reshead = db.Ado.UseTran(() =>
                            {
                                db.Deleteable<R_MDS_HEAD>().Where(t => t.MDSKEY == currentkey).ExecuteCommand();
                                var targetobjlist = new List<R_MDS_HEAD>()
                                {
                                    new R_MDS_HEAD()
                                    {
                                        ID = MESDataObject.MesDbBase.GetNewID<R_MDS_HEAD>(db, _bustr),
                                        MDSKEY = currentkey,
                                        MDSTYPE = CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>()
                                            .Description
                                    },
                                    new R_MDS_HEAD()
                                    {
                                        ID = MESDataObject.MesDbBase.GetNewID<R_MDS_HEAD>(db, _bustr),
                                        MDSKEY = currentkey,
                                        MDSTYPE = CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>()
                                            .Description
                                    },
                                    new R_MDS_HEAD()
                                    {
                                        ID = MESDataObject.MesDbBase.GetNewID<R_MDS_HEAD>(db, _bustr),
                                        MDSKEY = currentkey,
                                        MDSTYPE = CONST_MDSDATAPOINT.Iqc_DataPoint.Ext<EnumValueAttribute>().Description
                                    }
                                };
                                db.Insertable(targetobjlist).ExecuteCommand();
                            });
                            if (!reshead.IsSuccess) throw new Exception(reshead.ErrorMessage);
                        }

                        dokey = dokey.AddDays(1);
                    }
                }
            }
            catch (Exception e)
            {
                MdsLog(CONST_LOG_TYPE.BuildHead, e.Message);
                MesLog.Info($@"BuildHead Err:{e.Message}");
            }
            finally
            {
                MesLog.Info("BuildHead End");
            }
        }
        /// <summary>
        /// 在DB中創建Mds數據
        /// </summary>
        void BuildMdsData()
        {
            InitBuildSection();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                #region write data to db
                var waitBuild = db.Queryable<R_MDS_HEAD>().Where(t => t.CONVERTED ==CONST_MDS_CONVER_STATUS.NO.Ext<EnumValueAttribute>().Description).ToList();                                
                foreach (var item in waitBuild)
                {
                    try
                    {
                        #region build data to cache
                        var yieldobjlist = new List<R_MDS_YIELD>();
                        var strobjlist = new List<R_MDS_STR>();
                        var iqcobjlist = new List<R_MDS_IQC>();
                        #endregion
                        switch (item.MDSTYPE)
                        {
                            case "MDSYLD":
                                yieldobjlist = BuildYield(item);
                                break;
                            case "MDSSTR":
                                strobjlist = BuildStr(item);
                                break;
                            case "MDSIQC":
                                iqcobjlist = BuildIqc(item);
                                break;
                            default: break;
                        }
                        var resbuild = db.Ado.UseTran(() =>
                        {
                            db.Insertable(strobjlist).ExecuteCommand();
                            db.Insertable(iqcobjlist).ExecuteCommand();
                            db.Insertable(yieldobjlist).ExecuteCommand();
                            item.CONVERTED = CONST_MDS_CONVER_STATUS.YES.Ext<EnumValueAttribute>().Description;
                            item.EDITTIME = DateTime.Now;
                            db.Updateable(item).ExecuteCommand();
                        });
                        if (!resbuild.IsSuccess)
                            throw new Exception(resbuild.ErrorMessage);
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.BuildMdsData,e.Message, item);
                        MesLog.Info($@"BuildMdsData Err:{e.Message};");
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 初始化過濾條件
        /// </summary>
        void InitBuildSection()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                try
                {

                    var nocheckObjList = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == CONST_NOCHECK_MDS)
                        .ToList();
                    this.nocheckStationList = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_STATION && t.CONTROLFLAG=="Y")
                        .Select(t => t.VALUE).ToList();
                    this.nocheckSkunoList = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_SKUNO)
                        .Select(t => t.VALUE).ToList();
                    this.nocheckWoList = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_WO).Select(t => t.VALUE)
                        .ToList();
                    this.nocheckskuandstationlist =
                        nocheckObjList.FindAll(t => t.CREATEBY == this.CONST_STATIONANDSKU).ToList();
                    var routenameobj = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_ROUTENAME)
                        .Select(t => t.VALUE).ToList();
                    this.routeList = db.Queryable<C_ROUTE>().ToList();
                    this.nocheckRouteList = routeList.FindAll(t => routenameobj.Contains(t.ROUTE_NAME))
                        .Select(t => t.ID).ToList();
                    this.nocheckKeyPart = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_PARTNO)
                        .Select(t => t.VALUE).ToList();
                    this.nocheckScantype = nocheckObjList.FindAll(t => t.CATEGORY == this.CONST_SCANTYPE)
                        .Select(t => t.VALUE).ToList();
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData,e.Message);
                    throw;
                }
            }
        }
        /// <summary>
        /// 過濾不傳送的數據
        /// </summary>
        /// <param name="station"></param>
        /// <param name="routeid"></param>
        /// <param name="skuno"></param>
        /// <param name="wo"></param>
        /// <param name="partno"></param>
        /// <param name="scantype"></param>
        /// <returns></returns>
        bool FilterMdsYield(string station, string routeid, string skuno, string wo,string partno = default(string), string scantype = default(string))
        {
            if (nocheckStationList.Any(t => t.ToUpper().Trim().Equals(station.ToUpper().Trim()))) return true;
            if (nocheckRouteList.Any(t => t.ToUpper().Trim().Equals(routeid.ToUpper().Trim()))) return true;
            if (nocheckSkunoList.Any(t => t.ToUpper().Trim().Equals(skuno.ToUpper().Trim()))) return true;
            if (nocheckWoList.Any(t => t.ToUpper().Trim().Equals(wo.ToUpper().Trim()))) return true;
            if (nocheckskuandstationlist.Any(t =>t.VALUE.ToUpper().Trim() == skuno.ToUpper().Trim() &&t.EXTVAL.ToUpper().Trim() == station.ToUpper().Trim())) return true;
            foreach (var ispeRouteitem in CONST_FILTERSPEROUTE)
                if (routeList.Any(t => t.ID == routeid && t.ROUTE_NAME.Contains(ispeRouteitem))) return true;
            if(nocheckKeyPart.Any(t => partno!=null && t.ToUpper().Trim().Equals(partno.ToUpper().Trim()))) return true;
            if (nocheckScantype.Any(t => scantype != null && t.ToUpper().Trim().Equals(scantype.ToUpper().Trim()))) return true;
            return false;
        }
        /// <summary>
        /// 過濾不傳送的數據
        /// </summary>
        /// <param name="station"></param>
        /// <param name="routeid"></param>
        /// <param name="skuno"></param>
        /// <param name="wo"></param>
        /// <param name="partno"></param>
        /// <param name="scantype"></param>
        /// <returns></returns>
        bool FilterMdsStr(string station, string routeid, string skuno, string wo, string partno = default(string), string scantype = default(string))
        {
            if (nocheckRouteList.Any(t => t.ToUpper().Trim().Equals(routeid.ToUpper().Trim()))) return true;
            if (nocheckSkunoList.Any(t => t.ToUpper().Trim().Equals(skuno.ToUpper().Trim()))) return true;
            if (nocheckWoList.Any(t => t.ToUpper().Trim().Equals(wo.ToUpper().Trim()))) return true;
            if (nocheckskuandstationlist.Any(t => t.VALUE.ToUpper().Trim() == skuno.ToUpper().Trim() && t.EXTVAL.ToUpper().Trim() == station.ToUpper().Trim())) return true;
            foreach (var ispeRouteitem in CONST_FILTERSPEROUTE)
                if (routeList.Any(t => t.ID == routeid && t.ROUTE_NAME.Contains(ispeRouteitem))) return true;
            if (nocheckKeyPart.Any(t => partno != null && t.ToUpper().Trim().Equals(partno.ToUpper().Trim()))) return true;
            if (nocheckScantype.Any(t => scantype != null && t.ToUpper().Trim().Equals(scantype.ToUpper().Trim()))) return true;
            return false;
        }
        /// <summary>
        /// 傳送數據給B2B
        /// </summary>
        void SendMdsData()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {

                #region send to custermor
                var waitsendobj = db.Queryable<R_MDS_HEAD>().Where(t =>
                    t.SEND == ENUM_R_MDS_HEAD.SEND_FALSE.Ext<EnumValueAttribute>().Description &&
                    t.CONVERTED == ENUM_R_MDS_HEAD.CONVERTED_TRUE.Ext<EnumValueAttribute>().Description && t.MDSFILE != null).ToList();
                var waitsendkey = waitsendobj.Select(t => t.MDSKEY).Distinct(StringComparer.InvariantCultureIgnoreCase);
                SFTPHelper sftpHelp =
                    new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword);
                foreach (var item in waitsendkey)
                {
                    try
                    {
                        var itemsend = waitsendobj.Where(t => t.MDSKEY == item);
                        if (itemsend != null && itemsend.Count() == 3)
                        {
                            foreach (var sendobj in itemsend)
                               if(!File.Exists($@"{_filepath}/{sendobj.MDSFILE}")) 
                                   throw  new Exception($@"{_filepath}/{sendobj.MDSFILE} is not exists!");
                            foreach (var sendobj in itemsend)
                            {
                                sftpHelp.Put($@"{_filepath}/{sendobj.MDSFILE}", $@"{CONST_SFTPPath}/{sendobj.MDSFILE}");
                                sendobj.SEND = ENUM_R_MDS_HEAD.SEND_TRUE.Ext<EnumValueAttribute>().Description;
                                sendobj.EDITTIME = DateTime.Now;
                                db.Updateable(sendobj).ExecuteCommand();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.SendMdsData, e.Message,null, item);
                        //throw;
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  vn local ftp,send file by manual with pe 
        /// </summary>
        void SendMdsDataToLocalFtp()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {

                #region send to custermor
                var waitsendobj = db.Queryable<R_MDS_HEAD>().Where(t =>
                    t.SEND == ENUM_R_MDS_HEAD.SEND_FALSE.Ext<EnumValueAttribute>().Description &&
                    t.CONVERTED == ENUM_R_MDS_HEAD.CONVERTED_TRUE.Ext<EnumValueAttribute>().Description && t.MDSFILE != null).ToList();
                var waitsendkey = waitsendobj.Select(t => t.MDSKEY).Distinct(StringComparer.InvariantCultureIgnoreCase);
                FTPHelp ftpHelp =
                    new FTPHelp($@"{CONST_LOCAL_SFTPHost}{CONST_LOCAL_SFTPPath}", CONST_LOCAL_SFTPLogin, CONST_LOCAL_SFTPPassword);
                foreach (var item in waitsendkey)
                {
                    try
                    {
                        var itemsend = waitsendobj.Where(t => t.MDSKEY == item);
                        if (itemsend != null && itemsend.Count() == 3)
                        {
                            foreach (var sendobj in itemsend)
                                if (!File.Exists($@"{_filepath}/{sendobj.MDSFILE}"))
                                    throw new Exception($@"{_filepath}/{sendobj.MDSFILE} is not exists!");
                            foreach (var sendobj in itemsend)
                            {
                                ftpHelp.Upload($@"{_filepath}\{sendobj.MDSFILE}");
                                sendobj.SEND = ENUM_R_MDS_HEAD.SEND_TRUE.Ext<EnumValueAttribute>().Description;
                                db.Updateable(sendobj).ExecuteCommand();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.SendMdsData, e.Message, null, item);
                        //throw;
                    }
                }
                #endregion
            }
        }
        void SendMdsDataToB2bFtp()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {

                #region send to custermor
                var waitsendobj = db.Queryable<R_MDS_HEAD>().Where(t =>
                    t.SEND == ENUM_R_MDS_HEAD.SEND_FALSE.Ext<EnumValueAttribute>().Description &&
                    t.CONVERTED == ENUM_R_MDS_HEAD.CONVERTED_TRUE.Ext<EnumValueAttribute>().Description && t.MDSFILE != null).ToList();
                var waitsendkey = waitsendobj.Select(t => t.MDSKEY).Distinct(StringComparer.InvariantCultureIgnoreCase);
                FTPHelp ftpHelp =
                    new FTPHelp($@"{CONST_FTPHost}{CONST_FTPPath}", CONST_FTPLogin, CONST_FTPPassword);
                foreach (var item in waitsendkey)
                {
                    try
                    {
                        var itemsend = waitsendobj.Where(t => t.MDSKEY == item);
                        if (itemsend != null && itemsend.Count() == 3)
                        {
                            foreach (var sendobj in itemsend)
                                if (!File.Exists($@"{_filepath}/{sendobj.MDSFILE}"))
                                    throw new Exception($@"{_filepath}/{sendobj.MDSFILE} is not exists!");
                            foreach (var sendobj in itemsend)
                            {
                                ftpHelp.Upload($@"{_filepath}\{sendobj.MDSFILE}");
                                sendobj.SEND = ENUM_R_MDS_HEAD.SEND_TRUE.Ext<EnumValueAttribute>().Description;
                                db.Updateable(sendobj).ExecuteCommand();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.SendMdsData, e.Message, null, item);
                        //throw;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 創建Yield數據到緩存
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield(R_MDS_HEAD head)
        {
            var resYield = new List<R_MDS_YIELD>();
            resYield.AddRange(BuildYield_Pass(head));
            resYield.AddRange(BuildYield_Symptom(head));
            resYield.AddRange(BuildYield_Defect(head)); 
            resYield.AddRange(BuildYield_DefectRepair(head));
            resYield.AddRange(BuildYield_Ort(head));
            resYield.AddRange(BuildYield_OrtFail(head));
            return resYield;
        }
        /// <summary>
        /// 創建Str數據到緩存
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_STR> BuildStr(R_MDS_HEAD head)
        {
            var resStr = new List<R_MDS_STR>();
            resStr.AddRange(BuildStr_Normal(head));
            resStr.AddRange(BuildStr_SwKitPn(head));
            resStr.AddRange(BuildStr_Rework(head));
            resStr.AddRange(BuildStr_RepairReplaceKp(head));
            return resStr;
        }
        /// <summary>
        /// 創建Iqc數據到緩存
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_IQC> BuildIqc(R_MDS_HEAD head)
        {
            var resIqc = new List<R_MDS_IQC>();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var iqcdata = db.Queryable<R_MDS_IQC_AP>().Where(t => SqlFunc.Between(t.MDSDATE,
                    SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}").AddDays(-2),
                    SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}"))).ToList();
                foreach (var iqcitem in iqcdata)
                {
                    try
                    {
                        resIqc.Add(new R_MDS_IQC()
                        {
                            HEADID = head.ID,
                            ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                            DATAPOINT = CONST_MDSDATAPOINT.Iqc_DataPoint.Ext<EnumValueAttribute>().Description,
                            RECORD_CREATION_DATE = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                            CM_CODE = CONST_CMCODE,
                            INSPECTION_DATE = new Func<string>(() =>
                            {
                                var scantime = iqcitem.MDSDATE != null
                                    ? Convert.ToDateTime(iqcitem.MDSDATE.ToString())
                                    : DateTime.Now;
                                return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                            })(),
                            REJECTCODE = iqcitem.REJECTCODE,
                            PARTNO = iqcitem.PARTNO,
                            MPN = iqcitem.MPN,
                            MANUFACTURER = iqcitem.MANUFACTURER,
                            LOTNO = iqcitem.LOTNO,
                            DATECODE = iqcitem.DATECODE,
                            LOTCODE = iqcitem.LOTCODE,
                            RECEIVEQTY = iqcitem.RECEIVEQTY.ToString(),
                            SAMPLESIZE = iqcitem.SAMPLESIZE.ToString(),
                            ACCEPTQTY = iqcitem.ACCEPTQTY.ToString(),
                            REJECTQTY = iqcitem.REJECTQTY.ToString(),
                            AVLSTATUS = iqcitem.AVLSTATUS,
                            FIRSTINCOMING = iqcitem.FIRSTINCOMING,
                            INSPECTOR = iqcitem.INSPECTOR,
                            RMANO = iqcitem.RMANO,
                            ACTIO = iqcitem.RACTION,
                            CREATETIME = DateTime.Now
                        });
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                        MesLog.Error(e.Message);
                        throw e;
                    }
                }
            }
            return resIqc;
        }
        /// <summary>
        /// Repair Symptom Yield記錄
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield_Symptom(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region  Repair Symptom Yield記錄統計
                try
                {
                    var waitobjlist = db
                        .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, R_REPAIR_MAIN, R_REPAIR_FAILCODE,
                            C_ERROR_CODE>(
                            (rs, cs, css, cc, rwb, rrm, rrf, cec) => rs.SKUNO == cs.SKUNO &&
                                                                     cs.C_SERIES_ID == css.ID &&
                                                                     css.CUSTOMER_ID == cc.ID &&
                                                                     rs.WORKORDERNO == rwb.WORKORDERNO &&
                                                                     rs.SN == rrm.SN && rrm.ID == rrf.MAIN_ID &&
                                                                     rrf.FAIL_CODE == cec.ERROR_CODE)
                        .Where((rs, cs, css, cc, rwb, rrm, rrf, cec) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                            //rrf.F_CATEGORY == CONST_MDSRECORDSTATUS.Sym.Ext<EnumNameAttribute>().Description &&
                            rrf.REPAIR_FLAG == "0" &&   //scan fail but not repair
                            SqlFunc.Between(rrm.CREATE_TIME,
                                                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rs, cs, css, cc, rwb, rrm, rrf, cec) => new {rs, cs, css, cc, rwb, rrm, rrf, cec})
                        .ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsYield(item.rrm.FAIL_STATION, item.rs.ROUTE_ID, item.rs.SKUNO,
                                item.rrm.WORKORDERNO))
                                continue;
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rs.SN,
                                EVENTPOINT = item.rrm.FAIL_STATION,
                                YIELD_DATE =
                                    item.rrm.CREATE_TIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.rrm.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Sym.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                SCANBY = item.rrm.EDIT_EMP,
                                FAILCODE = item.rrf.FAIL_CODE,
                                COMMENTS = item.cec.ENGLISH_DESC,//fail_code description
                                FAILLOCATION = item.rrf.F_LOCATION,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rs.ROUTE_ID)
                                        .Select(t => t.ROUTE_NAME)
                                        .FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception(
                                            $@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rs.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resYield;
            }
        }
        /// <summary>
        /// 正常Pass Yield記錄
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield_Pass(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region 正常過站記錄統計

                try
                {
                    var waitobjlist = db.Queryable<R_SN_S_D, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE>(
                            (rssd, cs, css, cc, rwb) => rssd.SKUNO == cs.SKUNO &&
                                                        cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID &&
                                                        rssd.WORKORDERNO == rwb.WORKORDERNO)
                        .Where((rssd, cs, css, cc, rwb) =>
                            rssd.VALID_FLAG == R_SN_STATION_DETAIL_ENUM.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER && rssd.REPAIR_FAILED_FLAG ==
                            R_SN_STATION_DETAIL_ENUM.REPAIR_FAILED_FLAG_PASS.Ext<EnumValueAttribute>().Description &&
                            SqlFunc.Between(rssd.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rssd, cs, css, cc, rwb) => new {rssd, cs, css, cc, rwb}).ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsYield(item.rssd.STATION_NAME, item.rssd.ROUTE_ID, item.rssd.SKUNO,
                                item.rssd.WORKORDERNO))
                                continue;
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rssd.SKUNO,
                                SYSSERIALNO = item.rssd.SN,
                                EVENTPOINT = item.rssd.STATION_NAME,
                                YIELD_DATE =
                                    item.rssd.EDIT_TIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.rssd.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Pass.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                SCANBY = item.rssd.EDIT_EMP,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rssd.ROUTE_ID)
                                        .Select(t => t.ROUTE_NAME)
                                        .FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception(
                                            $@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rssd.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }
                #endregion
                return resYield;
            }
        }
        /// <summary>
        ///  Repair Defect Fail Yield記錄 
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield_Defect(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region  Repair Symptom Yield記錄統計

                try
                {
                    #region fixxxxx
                    //var waitobjlist = db
                    //    .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, R_REPAIR_MAIN, R_REPAIR_FAILCODE,
                    //        C_ERROR_CODE, R_REPAIR_ACTION>(
                    //        (rs, cs, css, cc, rwb, rrm, rrf, cec, rra) => new object[]
                    //        {
                    //            JoinType.Inner, rs.SKUNO == cs.SKUNO,
                    //            JoinType.Inner, cs.C_SERIES_ID == css.ID,
                    //            JoinType.Inner, css.CUSTOMER_ID == cc.ID,
                    //            JoinType.Inner, rs.WORKORDERNO == rwb.WORKORDERNO,
                    //            JoinType.Inner, rs.SN == rrm.SN,
                    //            JoinType.Inner, rrm.ID == rrf.MAIN_ID,
                    //            JoinType.Inner, rrf.FAIL_CODE == cec.ERROR_CODE,
                    //            JoinType.Left, rrf.ID == rra.R_FAILCODE_ID
                    //        })
                    //    .Where((rs, cs, css, cc, rwb, rrm, rrf, cec, rra) =>
                    //        rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                    //        cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                    //        rrf.F_CATEGORY == CONST_MDSRECORDSTATUS.Def.Ext<EnumNameAttribute>().Description &&
                    //        rrm.CLOSED_FLAG == Convert.ToInt32(true).ToString() &&
                    //        SqlFunc.Between(rrm.EDIT_TIME,
                    //            SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                    //            SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                    //    .Select((rs, cs, css, cc, rwb, rrm, rrf, cec, rra) =>
                    //        new {rs, cs, css, cc, rwb, rrm, rrf, cec, rra}).ToList();
                    #endregion

                    var waitobjlist = db
                        .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, C_ERROR_CODE>(
                            (rs, cs, css, cc, rwb, rrm, rrf, rra, cec) => new object[]
                            {
                                JoinType.Inner, rs.SKUNO == cs.SKUNO,
                                JoinType.Inner, cs.C_SERIES_ID == css.ID,
                                JoinType.Inner, css.CUSTOMER_ID == cc.ID,
                                JoinType.Inner, rs.WORKORDERNO == rwb.WORKORDERNO,
                                JoinType.Inner, rs.SN == rrm.SN,
                                JoinType.Inner, rrm.ID == rrf.MAIN_ID,
                                JoinType.Inner, rrf.ID == rra.R_FAILCODE_ID,
                                JoinType.Inner, rra.REASON_CODE == cec.ERROR_CODE
                            })
                        .Where((rs, cs, css, cc, rwb, rrm, rrf, rra, cec) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                            rrm.CLOSED_FLAG == Convert.ToInt32(true).ToString() &&
                            SqlFunc.Between(rrm.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rs, cs, css, cc, rwb, rrm, rrf, rra, cec) => new { rs, cs, css, cc, rwb, rrm, rrf, rra, cec }).ToList();
                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsYield(item.rrm.FAIL_STATION, item.rs.ROUTE_ID, item.rs.SKUNO, item.rrm.WORKORDERNO))
                                continue;
                            var keypartitemobj = db.Queryable<R_SN_KP>().Where(t => t.VALUE == item.rra.KEYPART_SN).ToList().FirstOrDefault();
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rs.SN,
                                EVENTPOINT = item.rrm.FAIL_STATION,
                                YIELD_DATE = item.rrm.CREATE_TIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.rrm.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Def.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                SCANBY = item.rrm.EDIT_EMP,
                                FAILCODE = item.rrf.FAIL_CODE,
                                COMMENTS = item.cec.ENGLISH_DESC,   //reason_code description
                                FAILLOCATION = item.rrf.F_LOCATION,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rs.ROUTE_ID).Select(t => t.ROUTE_NAME).FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception($@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rs.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                FAILPARTNO = item.rra.KP_NO,
                                FAILSERIALNO = item.rra.KEYPART_SN,
                                REPLACSERIALNO = item.rra.NEW_KEYPART_SN,
                                ATTRIBUTE1 = new Func<string>(() =>
                                {
                                    if (item.rra.KEYPART_SN==null || item.rra.KEYPART_SN.Trim() == "") return "";
                                    if (keypartitemobj == null) return "";
                                    return keypartitemobj.MPN;
                                })(),
                                ATTRIBUTE2 = item.rra.DATE_CODE,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resYield;
            }
        }
        /// <summary>
        ///  Repair Ort Yield記錄 
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield_Ort(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region  ORT Yield記錄統計

                try
                {
                    var waitobjlist = db
                        .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, R_ORT>(
                            (rs, cs, css, cc, rwb, ro) => new object[]
                            {
                                JoinType.Inner, rs.SKUNO == cs.SKUNO,
                                JoinType.Inner, cs.C_SERIES_ID == css.ID,
                                JoinType.Inner, css.CUSTOMER_ID == cc.ID,
                                JoinType.Inner, rs.WORKORDERNO == rwb.WORKORDERNO,
                                JoinType.Inner, rs.SN == ro.SN 
                            })
                        .Where((rs, cs, css, cc, rwb, ro) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description && (ro.ORTEVENT != null ||ro.ORTEVENT !="") &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&SqlFunc.Between(ro.WORKTIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rs, cs, css, cc, rwb, ro) => new { rs, cs, css, cc, rwb, ro }).ToList();
                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsYield(item.ro.ORTEVENT, item.rs.ROUTE_ID, item.rs.SKUNO, item.ro.WORKORDERNO))
                                continue;
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rs.SN,
                                EVENTPOINT = item.ro.ORTEVENT,
                                YIELD_DATE = item.ro.WORKTIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.ro.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Def.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                SCANBY = item.rs.EDIT_EMP,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rs.ROUTE_ID).Select(t => t.ROUTE_NAME).FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception($@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rs.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resYield;
            }
        }

        List<R_MDS_YIELD> BuildYield_OrtFail(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region  ORT Yield記錄統計

                try
                {
                    var waitobjlist = db
                        .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE,R_TEST_BRCD, R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, C_ERROR_CODE>(
                            (rs, cs, css, cc, rwb, rt, rrm, rrf, rra, cec) => new object[]
                            {
                                JoinType.Inner, rs.SKUNO == cs.SKUNO,
                                JoinType.Inner, cs.C_SERIES_ID == css.ID,
                                JoinType.Inner, css.CUSTOMER_ID == cc.ID,
                                JoinType.Inner, rs.WORKORDERNO == rwb.WORKORDERNO,
                                JoinType.Inner, rs.SN == rt.SYSSERIALNO,
                                JoinType.Inner, rs.SN == rrm.SN,
                                JoinType.Inner, rrm.ID == rrf.MAIN_ID,
                                JoinType.Inner, rrf.ID == rra.R_FAILCODE_ID,
                                JoinType.Inner, rra.REASON_CODE == cec.ERROR_CODE
                            })
                        .Where((rs, cs, css, cc, rwb, rt, rrm, rrf, rra, cec) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description && rt.EVENTNAME.Contains("ORT")&& rt.STATUS!="PASS" &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                             SqlFunc.Between(rrm.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}"))
                            && SqlFunc.Between(rt.TESTDATE,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}"))
                                )
                        .Select((rs, cs, css, cc, rwb, rt, rrm, rrf, rra, cec) => new { rs, cs, css, cc, rwb, rt, rrm, rrf, rra, cec }).ToList();
                    foreach (var item in waitobjlist)
                    {
                         try
                        {
                            if (FilterMdsYield(item.rt.EVENTNAME, item.rs.ROUTE_ID, item.rs.SKUNO, item.rs.WORKORDERNO))
                                continue;
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rs.SN,
                                EVENTPOINT = item.rt.EVENTNAME,
                                YIELD_DATE = item.rt.TATIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.rs.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Def.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),

                                SCANBY = item.rrm.EDIT_EMP,
                                FAILCODE = item.rrf.FAIL_CODE,
                                COMMENTS = item.cec.ENGLISH_DESC,   //reason_code description
                                FAILLOCATION = item.rrf.F_LOCATION,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rs.ROUTE_ID).Select(t => t.ROUTE_NAME).FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception($@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rs.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resYield;
            }
        }
        /// <summary>
        ///  Repair Defect Fail action Yield記錄 
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_YIELD> BuildYield_DefectRepair(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resYield = new List<R_MDS_YIELD>();
                #region  Repair Symptom Yield記錄統計

                try
                {
                    var waitobjlist = db
                        .Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, C_ACTION_CODE>(
                            (rs, cs, css, cc, rwb, rrm, rrf, rra, cac) => new object[]
                            {
                                JoinType.Inner, rs.SKUNO == cs.SKUNO,
                                JoinType.Inner, cs.C_SERIES_ID == css.ID,
                                JoinType.Inner, css.CUSTOMER_ID == cc.ID,
                                JoinType.Inner, rs.WORKORDERNO == rwb.WORKORDERNO,
                                JoinType.Inner, rs.SN == rrm.SN,
                                JoinType.Inner, rrm.ID == rrf.MAIN_ID,
                                //JoinType.Left, rrf.ID == rra.R_FAILCODE_ID,
                                JoinType.Inner, rrf.ID == rra.R_FAILCODE_ID,
                                JoinType.Inner, rra.ACTION_CODE == cac.ACTION_CODE,
                            })
                        .Where((rs, cs, css, cc, rwb, rrm, rrf, rra, cac) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                            //rrf.F_CATEGORY == CONST_MDSRECORDSTATUS.Def.Ext<EnumNameAttribute>().Description &&
                            rrm.CLOSED_FLAG == Convert.ToInt32(true).ToString() &&
                            SqlFunc.Between(rrm.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rs, cs, css, cc, rwb, rrm, rrf, rra, cac) => new { rs, cs, css, cc, rwb, rrm, rrf, rra, cac }).ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsYield(item.rrm.FAIL_STATION, item.rs.ROUTE_ID, item.rs.SKUNO, item.rrm.WORKORDERNO))
                                continue;
                            var keypartitemobj = db.Queryable<R_SN_KP>().Where(t => t.VALUE == item.rra.KEYPART_SN).ToList().FirstOrDefault();
                            resYield.Add(new R_MDS_YIELD()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_YIELD>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Yeild_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Yeild_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rs.SN,
                                EVENTPOINT = item.rrm.FAIL_STATION,
                                YIELD_DATE = item.rrm.CREATE_TIME?.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                WORKORDERNO = item.rrm.WORKORDERNO,
                                RECORDTYPE = CONST_MDSRECORDSTATUS.Rep.Ext<EnumValueAttribute>().Description,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                SCANBY = item.rrm.EDIT_EMP,
                                FAILCODE = item.rra.ACTION_CODE,
                                COMMENTS = item.cac.ENGLISH_DESC,   //action_code description
                                FAILLOCATION = item.rrf.F_LOCATION,
                                CORRENTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                ROUTEID = new Func<string>(() =>
                                {
                                    var routeobj = this.routeList.FindAll(t => t.ID == item.rs.ROUTE_ID).Select(t => t.ROUTE_NAME).FirstOrDefault();
                                    if (routeobj == null)
                                        throw new Exception($@"{CONST_MDS_EXCEPTION.MDS100EXCEPTION}{item.rs.ROUTE_ID}");
                                    return routeobj;
                                })(),
                                ROUTEREV = CONST_ROUTEREV,
                                FAILPARTNO = item.rra.KP_NO,
                                FAILSERIALNO = item.rra.KEYPART_SN,
                                REPLACSERIALNO = item.rra.NEW_KEYPART_SN,
                                ATTRIBUTE1 = new Func<string>(() =>
                                {
                                    if (item.rra.KEYPART_SN==null||item.rra.KEYPART_SN.Trim() == "") return "";
                                    if (keypartitemobj == null) return "";
                                    return keypartitemobj.MPN;
                                })(),
                                ATTRIBUTE2 = item.rra.DATE_CODE,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resYield;
            }}
        /// <summary>
        /// BuildStr_Normal記錄
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_STR> BuildStr_Normal(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resStr = new List<R_MDS_STR>();
                #region 正常過站記錄統計

                try
                {
                    var waitobjlist = db.Queryable<R_SN_KP, R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE>(
                            (rsk, rs, cs, css, cc, rwb) => rsk.R_SN_ID == rs.ID && rs.SKUNO == cs.SKUNO &&
                                                           cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID &&
                                                           rs.WORKORDERNO == rwb.WORKORDERNO)
                        .Where((rsk, rs, cs, css, cc, rwb) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description && rsk.VALUE!=null &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                            SqlFunc.Between(rsk.EDIT_TIME,
                                     SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                     SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rsk, rs, cs, css, cc, rwb) => new {rsk, rs, cs, css, cc, rwb}).ToList();
                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsStr(item.rsk.STATION, item.rs.ROUTE_ID, item.rs.SKUNO,
                            item.rs.WORKORDERNO, CONST_NULLVALUE, item.rsk.SCANTYPE))
                                continue;
                            resStr.Add(new R_MDS_STR()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Str_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rsk.SN,
                                PARTNO = item.rsk.PARTNO.Substring(0,
                                    item.rsk.PARTNO.Length > 13 ? 13 : item.rsk.PARTNO.Length),
                                CSERIALNO = new Func<string>(() =>
                                {
                                    foreach (var scanitem in CONST_SN_SCANTYPE)
                                        if (item.rsk.SCANTYPE.IndexOf(scanitem) > 0)
                                            return item.rsk.VALUE;
                                    return CONST_NULLVALUE;
                                })(),
                                VENDORID = new Func<string>(() =>
                                {
                                    if (item.rsk.SCANTYPE.IndexOf("S/N") > 0 || item.rsk.SCANTYPE.IndexOf("P/N") > 0 || item.rsk.VALUE.IndexOf("N/A") > -1)
                                        return CONST_NULLVALUE;
                                    return item.rsk.VALUE;
                                    //if (item.rsk.VALUE.IndexOf(CONST_DCLC_SPLIT_CHAR) > 0)
                                    //{
                                    //    var venditems = item.rsk.VALUE.Replace(CONST_NA, CONST_NULLVALUE).Split('/');
                                    //    if (venditems.Length > 1)
                                    //        return venditems[1];
                                    //}

                                    //return CONST_NULLVALUE;
                                })(),
                                VENDORNAME = new Func<string>(() =>
                                {
                                    if (item.rsk.SCANTYPE.IndexOf("S/N") > 0)
                                        return CONST_NULLVALUE;
                                    return item.rsk.VALUE.Replace("/N/A","");
                                    //if (item.rsk.VALUE.IndexOf(CONST_DCLC_SPLIT_CHAR) > 0)
                                    //{
                                    //    var venditems = item.rsk.VALUE.Replace(CONST_NA, CONST_NULLVALUE).Split('/');
                                    //    if (venditems.Length > 0)
                                    //        return venditems[0];
                                    //}

                                    //return CONST_NULLVALUE;
                                })(),
                                STRUC_DATE = new Func<string>(() =>
                                {
                                    var scantime = item.rsk.EDIT_TIME != null
                                        ? Convert.ToDateTime(item.rsk.EDIT_TIME.ToString())
                                        : DateTime.Now;
                                    return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                                })(),
                                CORRECTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                WORKORDERNO = item.rs.WORKORDERNO,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                ATTRIBUTE1 = item.rsk.MPN == CONST_NULLVALUE ? item.rsk.PARTNO : item.rsk.MPN,
                                ATTRIBUTE3 = item.rsk.LOCATION,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resStr;
            }
        }
        /// <summary>
        /// BuildStr_SwKitPn記錄
        /// SW_KIT_PN 類型的KP的KP
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_STR> BuildStr_SwKitPn(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resStr = new List<R_MDS_STR>();
                #region SwKitPn記錄統計

                try
                {
                    var waitobjlist = db
                        .Queryable<R_SN_KP, R_SN, C_SKU, C_SERIES, C_CUSTOMER, R_WO_BASE, C_KP_LIST, C_KP_List_Item,
                            C_KP_LIST_I_D>(
                            (rsk, rs, cs, css, cc, rwb, ckl, ckli, cklid) =>
                                rsk.R_SN_ID == rs.ID && rs.SKUNO == cs.SKUNO &&
                                cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID &&
                                rs.WORKORDERNO == rwb.WORKORDERNO && rsk.PARTNO == ckl.SKUNO &&
                                ckl.ID == ckli.LIST_ID && ckli.ID == cklid.ITEM_ID)
                        .Where((rsk, rs, cs, css, cc, rwb, ckl, ckli, cklid) =>
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            cc.CUSTOMER_NAME == CONST_CUSTOMER &&
                            SqlFunc.Between(rsk.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")) &&
                            ckl.FLAG == ENUM_C_KP_LIST.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsk.SCANTYPE == DcnKeyPartScantype.SW_KIT_PN.ExtName())
                        .Select((rsk, rs, cs, css, cc, rwb, ckl, ckli, cklid) =>
                            new {rsk, rs, cs, css, cc, rwb, ckl, ckli, cklid}).ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsStr(item.rsk.STATION, item.rs.ROUTE_ID, item.rs.SKUNO,
                                item.rs.WORKORDERNO, item.rsk.PARTNO))
                                continue;
                            resStr.Add(new R_MDS_STR()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Str_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rs.SKUNO,
                                SYSSERIALNO = item.rsk.SN,
                                PARTNO = item.ckli.KP_PARTNO,
                                VENDORNAME = new Func<string>(() =>
                                {
                                    if (item.cklid.SCANTYPE.Equals(DcnKeyPartScantype.FIRMWARE_PN.ExtName()))
                                        return item.ckl.CUSTVERSION;
                                    return CONST_NULLVALUE;
                                })(),
                                STRUC_DATE = new Func<string>(() =>
                                {
                                    var scantime = item.rsk.EDIT_TIME != null
                                        ? Convert.ToDateTime(item.rsk.EDIT_TIME.ToString())
                                        : DateTime.Now;
                                    return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                                })(),
                                CORRECTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                WORKORDERNO = item.rs.WORKORDERNO,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                ATTRIBUTE1 = item.ckli.KP_PARTNO,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resStr;
            }
        }
        /// <summary>
        /// BuildStr_RepairOldKp記錄
        /// 維修換料舊KP和新KP數據
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_STR> BuildStr_RepairReplaceKp(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resStr = new List<R_MDS_STR>();
                #region RepairReplaceKp記錄統計

                try
                {
                    var waitobjlist = db
                        .Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, R_WO_BASE, C_SKU, C_SERIES,
                            C_CUSTOMER>(
                            (rrm, rrf, rra, rwb, cs, css, cc) =>
                                rrm.ID == rrf.MAIN_ID && rrf.ID == rra.R_FAILCODE_ID &&
                                rrm.WORKORDERNO == rwb.WORKORDERNO && cs.SKUNO == rwb.SKUNO &&
                                cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID)
                        .Where((rrm, rrf, rra, rwb, cs, css, cc) =>
                            cc.CUSTOMER_NAME == CONST_CUSTOMER && rra.KEYPART_SN!=null && rra.KP_NO!=null &&
                            SqlFunc.Between(rra.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rrm, rrf, rra, rwb, cs, css, cc) => new {rrm, rrf, rra, rwb, cs, css, cc}).ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsStr(item.rrm.FAIL_STATION, item.rwb.ROUTE_ID, item.rwb.SKUNO,
                                item.rwb.WORKORDERNO, item.rra.KP_NO))
                                continue;

                            #region olb kp

                            resStr.Add(new R_MDS_STR()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Str_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rwb.SKUNO,
                                SYSSERIALNO = item.rrm.SN,
                                PARTNO = item.rra.KP_NO.Substring(0,
                                    item.rra.KP_NO.Length > 13 ? 13 : item.rra.KP_NO.Length),
                                CSERIALNO = new Func<string>(() =>
                                {
                                    if (item.rra.KEYPART_SN == null) return CONST_NULLVALUE;
                                    var thiskps = db.Queryable<R_SN_KP>().Where(t => 
                                        item.rra.KEYPART_SN != CONST_NULLVALUE && t.VALUE == item.rra.KEYPART_SN &&
                                                                                   ENUM_R_SN_KP.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description ==
                                                                               t.VALID_FLAG.ToString()).ToList();
                                    foreach (var itemKp in CONST_SN_SCANTYPE)
                                        if (thiskps.FindAll(t => t.SCANTYPE.Contains(itemKp)).Count > 0)
                                            return item.rra.KEYPART_SN;
                                    return CONST_NULLVALUE;
                                })(),
                                VENDORID = item.rra.LOT_CODE,
                                VENDORNAME = item.rra.DATE_CODE,
                                STRUC_DATE = new Func<string>(() =>
                                {
                                    var scantime = item.rra.EDIT_TIME != null
                                        ? Convert.ToDateTime(item.rra.EDIT_TIME.ToString())
                                        : DateTime.Now;
                                    return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                                })(),
                                CORRECTTYPE = CONST_CORRENTTYPE.D.Ext<EnumValueAttribute>().Description,
                                WORKORDERNO = item.rwb.WORKORDERNO,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                ATTRIBUTE1 = item.rra.MPN,
                                ATTRIBUTE3 = item.rra.FAIL_LOCATION,
                                CREATETIME = DateTime.Now
                            });

                            #endregion

                            #region new kp

                            resStr.Add(new R_MDS_STR()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Str_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rwb.SKUNO,
                                SYSSERIALNO = item.rrm.SN,
                                PARTNO = item.rra.KP_NO.Substring(0,
                                    item.rra.KP_NO.Length > 13 ? 13 : item.rra.KP_NO.Length),
                                CSERIALNO = new Func<string>(() =>
                                {
                                    if (item.rra.KEYPART_SN == null) return CONST_NULLVALUE;
                                    var thiskps = db.Queryable<R_SN_KP>().Where(t =>
                                        item.rra.KEYPART_SN != CONST_NULLVALUE && t.VALUE == item.rra.KEYPART_SN &&
                                                                                   ENUM_R_SN_KP.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description ==
                                                                               t.VALID_FLAG.ToString()).ToList();
                                    foreach (var itemKp in CONST_SN_SCANTYPE)
                                        if (thiskps.FindAll(t => t.SCANTYPE.Contains(itemKp)).Count > 0)
                                            return item.rra.NEW_KEYPART_SN;
                                    return CONST_NULLVALUE;
                                })(),
                                VENDORID = item.rra.NEW_LOT_CODE,
                                VENDORNAME = item.rra.NEW_DATE_CODE,
                                STRUC_DATE = new Func<string>(() =>
                                {
                                    var scantime = item.rra.EDIT_TIME != null
                                        ? Convert.ToDateTime(item.rra.EDIT_TIME.ToString())
                                        : DateTime.Now;
                                    return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                                })(),
                                CORRECTTYPE = CONST_CORRENTTYPE.A.Ext<EnumValueAttribute>().Description,
                                WORKORDERNO = item.rwb.WORKORDERNO,
                                WORKORDERTYPE = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSWOTYPE.RMA.Ext<EnumValueAttribute>().Description;
                                    else
                                        return CONST_MDSWOTYPE.REG.Ext<EnumValueAttribute>().Description;
                                })(),
                                ATTRIBUTE1 = item.rra.NEW_MPN,
                                ATTRIBUTE3 = item.rra.FAIL_LOCATION,
                                CREATETIME = DateTime.Now
                            });

                            #endregion
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resStr;
            }
        }
        /// <summary>
        /// BuildStr_Rework記錄
        /// 重工數據
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        List<R_MDS_STR> BuildStr_Rework(R_MDS_HEAD head)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var resStr = new List<R_MDS_STR>();
                #region SwKitPn記錄統計

                try
                {
                    var waitobjlist = db.Queryable<R_MRB, R_WO_BASE, R_SN, C_SKU, C_SERIES, C_CUSTOMER>(
                            (rm, rwb, rs, cs, css, cc) =>
                                rm.SN == rs.SN && rm.WORKORDERNO == rwb.WORKORDERNO && cs.SKUNO == rwb.SKUNO &&
                                cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID)
                        .Where((rm, rwb, rs, cs, css, cc) =>
                            cc.CUSTOMER_NAME == CONST_CUSTOMER && rm.REWORK_WO != null &&
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            SqlFunc.Between(rm.EDIT_TIME,
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_BEGIN.Ext<EnumValueAttribute>().Description}"),
                                SqlFunc.ToDate($@"{head.MDSKEY} {MES_CONST_DAY_RANGE.CONST_DAY_END.Ext<EnumValueAttribute>().Description}")))
                        .Select((rm, rwb, rs, cs, css, cc) => new {rm, rwb, rs, cs, css, cc}).ToList();

                    foreach (var item in waitobjlist)
                    {
                        try
                        {
                            if (FilterMdsStr(CONST_NULLVALUE, item.rwb.ROUTE_ID, item.rwb.SKUNO,
                                item.rwb.WORKORDERNO))
                                continue;
                            resStr.Add(new R_MDS_STR()
                            {
                                HEADID = head.ID,
                                ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                                DATAPOINT = new Func<string>(() =>
                                {
                                    if (item.rwb.WO_TYPE.Equals(WorkType.RMA.Ext<EnumValueAttribute>().Description))
                                        return CONST_MDSDATAPOINT.Str_RmaDataPoint.Ext<EnumValueAttribute>().Description;
                                    return CONST_MDSDATAPOINT.Str_NotRmaDataPoint.Ext<EnumValueAttribute>().Description;
                                })(),
                                RECORD_CREATION_DATE =
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description),
                                CM_CODE = CONST_CMCODE,
                                SKUNO = item.rwb.SKUNO,
                                SYSSERIALNO = item.rm.SN,
                                STRUC_DATE = new Func<string>(() =>
                                {
                                    var scantime = item.rm.EDIT_TIME != null
                                        ? Convert.ToDateTime(item.rm.EDIT_TIME.ToString())
                                        : DateTime.Now;
                                    return scantime.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description);
                                })(),
                                CORRECTTYPE = CONST_CORRENTTYPE.X.Ext<EnumValueAttribute>().Description,
                                WORKORDERNO = item.rwb.WORKORDERNO,
                                WORKORDERTYPE = CONST_MDSWOTYPE.REC.Ext<EnumValueAttribute>().Description,
                                CREATETIME = DateTime.Now
                            });
                        }
                        catch (Exception e)
                        {
                            MesLog.Error(e.Message);
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    MdsLog(CONST_LOG_TYPE.BuildMdsData, e.Message, head);
                    throw e;
                }

                #endregion
                return resStr;
            }
        }
        /// <summary>
        /// genaration file
        /// </summary>
        void GanarationFile()
        {
            #region genaration file
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var waitGanarationList = db.Queryable<R_MDS_HEAD>().Where(t =>
                    t.CONVERTED == CONST_MDS_CONVER_STATUS.YES.Ext<EnumValueAttribute>().Description &&
                    t.SEND == CONST_MDS_SEND_STATUS.NO.Ext<EnumValueAttribute>().Description && t.MDSFILE == null).ToList();
                foreach (var item in waitGanarationList)
                {
                    try
                    {
                        var itemfilename =
                            $@"{CONST_FILENAME_PRE}_{item.MDSTYPE}_{CONST_FILENAME_PRE_CM}_{Convert.ToDateTime(item.MDSKEY).ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description)}{CONST_MDSFILE_EXTENSION}";
                        switch (item.MDSTYPE)
                        {
                            case "MDSYLD":
                                var yieldobjlist = db.Queryable<R_MDS_YIELD>().Where(t => t.HEADID == item.ID).ToList();
                                GanarationFile(yieldobjlist, _filepath, itemfilename);
                                break;
                            case "MDSSTR":
                                var strobjlist = db.Queryable<R_MDS_STR>().Where(t => t.HEADID == item.ID).ToList();
                                GanarationFile(strobjlist, _filepath, itemfilename);
                                break;
                            case "MDSIQC":
                                var iqcobjlist = db.Queryable<R_MDS_IQC>().Where(t => t.HEADID == item.ID).ToList();
                                GanarationFile(iqcobjlist, _filepath, itemfilename);
                                break;
                            default: break;
                        }

                        item.MDSFILE = itemfilename;
                        item.EDITTIME = DateTime.Now;
                        db.Updateable(item).ExecuteCommand();
                    }
                    catch (Exception e)
                    {
                        MdsLog(CONST_LOG_TYPE.GanarationFile, e.Message, item);
                        MesLog.Error(e.Message);
                        throw e;
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// genaration file
        /// </summary>
        bool GanarationFile<T>(List<T> objList, string path, string filename)
        {
            try
            {


                var fullfilename = $@"{path}/{filename}";
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                if (System.IO.File.Exists(fullfilename))
                {
                    FileInfo fi = new FileInfo(fullfilename);
                    fi.MoveTo(
                        $@"{path}/{filename}_{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description)}");
                }

                using (var fs1 = new FileStream(fullfilename, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs1);
                    foreach (var objitem in objList)
                    {
                        var newType = objitem.GetType();
                        var linestr = "";
                        foreach (var item in newType.GetRuntimeProperties())
                            if (CONST_NOT_FILE_COL.IndexOf(item.Name) == -1)
                                linestr += item.GetValue(objitem)==null? $@"{item.GetValue(objitem)}|": $@"{item.GetValue(objitem).ToString().Replace(" ", "").Replace("|", "")}|";
                                //linestr += $@"{item.GetValue(objitem).ToString().Replace(" ","").Replace("|","")}|";
                        linestr = linestr.Substring(0, linestr.LastIndexOf("|"));
                        sw.WriteLine(linestr);
                    }
                    sw.WriteLine($@"CTL|{objList.Count}");
                    sw.Flush();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        /// <summary>
        /// log to db
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="head"></param>
        void MdsLog(CONST_LOG_TYPE type,string message, R_MDS_HEAD head=null,string mdskey=null)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                db.Insertable(new R_MES_LOG()
                {
                    ID = MESDataObject.MesDbBase.GetNewID<R_MDS_STR>(db, _bustr),
                    PROGRAM_NAME = this.CONST_PROGRAM_NAME,
                    CLASS_NAME = WriteLog.GetCurrentMethodFullName(),
                    FUNCTION_NAME = type.Ext<EnumValueAttribute>().Description,
                    LOG_MESSAGE = message,
                    EDIT_TIME = DateTime.Now,
                    DATA1 = head!=null? head.MDSKEY : mdskey,
                    DATA2 = head != null ? head.MDSTYPE : mdskey,
                    MAILFLAG = ENUM_R_MES_LOG.MAILFLAG_TRUE.Ext<EnumValueAttribute>().Description
                }).ExecuteCommand();
            }
        }
        void test()
        {
            DateTime? Foremdstime=null;

            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(Foremdstime).Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            string hours = ts.Hours.ToString(), minutes = ts.Minutes.ToString(), seconds = ts.Seconds.ToString();

            new TimeSpan(DateTime.Now.Ticks).Subtract(new TimeSpan(Convert.ToDateTime(Foremdstime).Ticks)).Duration();




            var aa = MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description;
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                //db.BeginTran();
                db.Ado.BeginTran();
                var tes = db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == "002625000000").ToList()
                    .FirstOrDefault();
                tes.PO_NO = "1";
                db.Updateable(tes).ExecuteCommand();
                using (var db2 = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
                {
                    var tes2 = db2.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == "002625000000").ToList()
                        .FirstOrDefault();
                    tes2.PO_NO = "2";
                    db2.Updateable(tes2).ExecuteCommand();
                    //db2.RollbackTran();
                    //db.CommitTran();
                }
                //db.RollbackTran();
                //db.CommitTran();
                //var yieldobjlist = db.Queryable<R_MDS_YIELD>().Where(t => t.HEADID == "VNDCN0000000000V").ToList();
                //var itemfilename =
                //      $@"{CONST_FILENAME_PRE}_MDSYLD_{CONST_FILENAME_PRE_CM}_{Convert.ToDateTime("2020-08-01").ToString(GetEnumValue(MES_CONST_DATETIME_FORMAT.DEFAULT))}.txt";
                //GanarationFile(yieldobjlist,_filepath, itemfilename);
            }
        }
    }
}
