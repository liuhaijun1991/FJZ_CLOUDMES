using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using MESPubLab;
using SqlSugar;
using System.Data;
using System.Reflection;
using MESDataObject.Constants;
using static MESDataObject.Common.EnumExtensions;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Module.OM;
using MESPubLab.MesBase;
using MESDataObject.Module.Juniper;
using MESPubLab.SAP_RFC;
using MESJuniper.OrderManagement;

namespace MESJuniper.Base
{
    public class JuniperBase
    {
        public RecordServiceLog recordServiceLog;

        private static ZCPP_NSBG_0140 rfc140 = null;
        private static ZCPP_NSBG_0141 rfc141 = null;
        private static ZCPP_NSBG_0009 rfc009 = null;
        private static ZCPP_NSBG_0045 rfc045 = null;



        public static DataTable FormatTableSingleRowViewData<T>(T model, int colnum)
        {
            DataTable dt = new DataTable();
            for (int i = 1; i <= colnum; i++)
            {
                dt.Columns.Add("Data" + i);
            }
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            int rownum = (int)Math.Ceiling((double)PropertyList.Count() / (double)dt.Columns.Count);
            for (int i = 0; i < rownum; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns.Count * i + j <= PropertyList.Count() - 1)
                        dr[dt.Columns[j]] = PropertyList[dt.Columns.Count * i + j].Name;
                }
                dt.Rows.Add(dr);

                DataRow drv = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns.Count * i + j <= PropertyList.Count() - 1)
                        drv[dt.Columns[j]] = PropertyList[dt.Columns.Count * i + j].GetValue(model, null)?.ToString();
                }
                dt.Rows.Add(drv);
            }
            return dt;
        }

        public static void CloseAllExcption(SqlSugarClient mesdb, string upoid)
        {
            using (var _db = OleExec.GetSqlSugarClient(mesdb.CurrentConnectionConfig.ConnectionString))
            {
                _db.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue(), EDITTIME = DateTime.Now })
                      .Where(t => t.UPOID == upoid && t.STATUS != JuniperErrStatus.Close.ExtValue()).ExecuteCommand();
            }
        }

        public static string GetCurrentSite(SqlSugarClient db)
        {
            var defaultplantcodeobj = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "DefaulSite" && t.CATEGORY == "SiteName" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList().FirstOrDefault();
            if (defaultplantcodeobj == null)
                throw new Exception("pls config default plantcode!");
            var plantCode = defaultplantcodeobj.VALUE.ToString();
            return plantCode;
        }

        /// <summary>
        /// 記錄異常
        /// </summary>
        /// <param name="mesdb"></param>
        /// <param name="upoid"></param>
        /// <param name="ordermainId"></param>
        /// <param name="juniperErrType"></param>
        /// <param name="exception"></param>
        public static void RecordJuniperExcption(SqlSugarClient mesdb, string exception, string upoid, string ordermainId, JuniperErrType juniperErrType, JuniperSubType JuniperSubType, bool closeflag)
        {
            using (var _db = OleExec.GetSqlSugarClient(mesdb.CurrentConnectionConfig.ConnectionString))
            {
                try
                {
                    if (!closeflag)
                    {
                        exception = exception.Length > 1999 ? exception.Substring(0, 1999) : exception;
                        var exceptobj = _db.Queryable<O_EXCEPTION_DATA>().Where(t => t.UPOID == upoid && t.ORIGINALID == ordermainId && t.EXCEPTIONINFO == exception //&& t.STATUS == JuniperErrStatus.Open.ExtValue()
                                         && t.EXCEPCODE == JuniperSubType.Ext<EnumValueAttribute>().Description
                                         && t.EXCTYPE == juniperErrType.Ext<EnumValueAttribute>().Description).ToList().FirstOrDefault();
                        if (exceptobj != null)
                        {
                            exceptobj.EDITTIME = DateTime.Now;
                            exceptobj.STATUS = JuniperErrStatus.Open.ExtValue();
                            _db.Updateable(exceptobj).ExecuteCommand();
                        }
                        else _db.Insertable(new O_EXCEPTION_DATA()
                        {
                            ID = MesDbBase.GetNewID<O_EXCEPTION_DATA>(_db, Customer.JUNIPER.ExtValue()),
                            UPOID = upoid,
                            ORIGINALID = ordermainId,
                            EXCTYPE = juniperErrType.Ext<EnumValueAttribute>().Description,
                            EXCEPTIONINFO = exception,
                            EXCEPCODE = JuniperSubType.Ext<EnumValueAttribute>().Description,
                            STATUS = JuniperErrStatus.Open.Ext<EnumValueAttribute>().Description,
                            MAILFLAG = MesBool.No.Ext<EnumValueAttribute>().Description,
                            CREATETIME = DateTime.Now,
                            EDITTIME = DateTime.Now,
                            CREATEBY = MesSysUser.Sys.Ext<EnumValueAttribute>().Description
                        }).ExecuteCommand();
                    }
                    else if (!string.IsNullOrEmpty(exception))
                        _db.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue(), EDITTIME = DateTime.Now })
                        .Where(t => t.UPOID == upoid && t.ORIGINALID == ordermainId && t.EXCEPTIONINFO == exception && t.STATUS != JuniperErrStatus.Close.ExtValue() && t.EXCEPCODE == JuniperSubType.Ext<EnumValueAttribute>().Description
                        && t.EXCTYPE == juniperErrType.Ext<EnumValueAttribute>().Description).ExecuteCommand();                    
                    else if (upoid== ordermainId)
                        _db.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue(), EDITTIME = DateTime.Now })
                        .Where(t => t.UPOID == upoid  && t.STATUS != JuniperErrStatus.Close.ExtValue() && t.EXCEPCODE == JuniperSubType.Ext<EnumValueAttribute>().Description
                        && t.EXCTYPE == juniperErrType.Ext<EnumValueAttribute>().Description).ExecuteCommand();
                    else
                        _db.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue(), EDITTIME = DateTime.Now })
                        .Where(t => t.UPOID == upoid && t.ORIGINALID == ordermainId && t.STATUS != JuniperErrStatus.Close.ExtValue() && t.EXCEPCODE == JuniperSubType.Ext<EnumValueAttribute>().Description
                        && t.EXCTYPE == juniperErrType.Ext<EnumValueAttribute>().Description).ExecuteCommand();
                }
                catch (Exception e)
                {
                }
            }
        }

        /// <summary>
        /// 記錄異常
        /// </summary>
        /// <param name="mesdb"></param>
        /// <param name="upoid"></param>
        /// <param name="ordermainId"></param>
        /// <param name="juniperErrType"></param>
        /// <param name="exception"></param>
        public static void ReleaseJuniperExcption(SqlSugarClient mesdb, string upoid, string ordermainId)
        {
            using (var _db = OleExec.GetSqlSugarClient(mesdb.CurrentConnectionConfig.ConnectionString))
            {
                try
                {
                    _db.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue(), EDITTIME = DateTime.Now })
                    .Where(t => t.UPOID == upoid && t.STATUS != JuniperErrStatus.Close.ExtValue() && t.ORIGINALID != ordermainId).ExecuteCommand();
                }
                catch (Exception e)
                {
                }
            }
        }

        public static ENUM_O_PO_STATUS GetPoNextStatus(ENUM_O_PO_STATUS currentStatus)
        {
            ENUM_O_PO_STATUS res = ENUM_O_PO_STATUS.ValidationI137;
            switch (currentStatus)
            {
                case ENUM_O_PO_STATUS.ValidationI137:
                    res = ENUM_O_PO_STATUS.WaitCreatePreWo;
                    break;
                case ENUM_O_PO_STATUS.WaitCreatePreWo:
                    res = ENUM_O_PO_STATUS.OnePreUploadBom;
                    break;
                case ENUM_O_PO_STATUS.OnePreUploadBom:
                    res = ENUM_O_PO_STATUS.AddNonBom;
                    break;
                case ENUM_O_PO_STATUS.AddNonBom:
                    res = ENUM_O_PO_STATUS.ReceiveGroupId;
                    break;
                case ENUM_O_PO_STATUS.ReceiveGroupId:
                    res = ENUM_O_PO_STATUS.SecPreUploadBom;
                    break;
                case ENUM_O_PO_STATUS.CreateWo:
                    res = ENUM_O_PO_STATUS.DownloadWo;
                    break;
                case ENUM_O_PO_STATUS.SecPreUploadBom:
                    res = ENUM_O_PO_STATUS.CreateWo;
                    break;
                case ENUM_O_PO_STATUS.DownloadWo:
                    res = ENUM_O_PO_STATUS.Production;
                    break;
                case ENUM_O_PO_STATUS.Production:
                    res = ENUM_O_PO_STATUS.CBS;
                    break;
                case ENUM_O_PO_STATUS.CBS:
                    res = ENUM_O_PO_STATUS.PreAsn;
                    break;
                case ENUM_O_PO_STATUS.PreAsn:
                    res = ENUM_O_PO_STATUS.PrintLabelAndList;
                    break;
                case ENUM_O_PO_STATUS.DOAShipment:
                    res = ENUM_O_PO_STATUS.PrintLabelAndList;
                    break;
                case ENUM_O_PO_STATUS.PrintLabelAndList:
                    res = ENUM_O_PO_STATUS.ShipOut;
                    break;
                case ENUM_O_PO_STATUS.ShipOut:
                    res = ENUM_O_PO_STATUS.Finish;
                    break;
                case ENUM_O_PO_STATUS.WaitDismantle:
                    break;
                case ENUM_O_PO_STATUS.RmqEnd:
                    break;
                default:
                    break;
            }

            return res;
        }

        /// <summary>
        /// if bom change then order status= WaitCreatePreWo else return true
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bu"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static MesFunctionRes JuniperBomExplosionForConverWo(O_ORDER_MAIN item, string bu, SqlSugarClient db)
        {
            var res = new MesFunctionRes();
            try
            {
                var rfc = new ZCPP_NSBG_0302(bu);
                var wotypelist = db.Queryable<O_J_WOTYPE>().ToList();
                var skuconfiglist = db.Queryable<O_SKU_CONFIG>().ToList();
                var i137item = db.Queryable<O_I137_ITEM>().Where(t => t.ID == item.ITEMID).ToList().FirstOrDefault();
                #region bomextend flag
                //var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                #endregion

                var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                var defaultplantcodeobj = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "DefaultPlantCode" && t.CATEGORY == "PlantCode" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList().FirstOrDefault();
                if (defaultplantcodeobj == null)
                    throw new Exception("pls config default plantcode!");
                var plantCode = skunewplant != null ? skunewplant.PLANTCODE : defaultplantcodeobj.VALUE.ToString();
                var wotype = wotypelist.FindAll(t => t.WOPRE == item.PREWO.Substring(0, 4)).ToList().FirstOrDefault();
                if (wotype == null)
                    throw new Exception($@"{item.PREWO } miss wo type!");
                var hbmap = db.Queryable<R_PN_HB_MAP>().Where(t => t.CUSTPN == item.CUSTPID).ToList().FirstOrDefault();
                var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID && t.ACTIVED == MesBool.Yes.ExtValue()).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault();

                #region bomextend flag
                var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == agiledata.USER_ITEM_TYPE && t.OFFERINGTYPE == agiledata.OFFERING_TYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                #endregion

                DataTable ZWO_HEADER = rfc.GET_NEW_ZWO_HEADER();
                DataTable ZWO_ITEM = rfc.GET_NEW_ZWO_ITEM();
                DataTable ZWO_HIDBOM = rfc.GET_NEW_ZWO_HIDBOM();
                DataRow hdr = ZWO_HEADER.NewRow();
                var samplewo = $@"{item.PREWO.Substring(0, 4)}Z{item.PREWO.Substring(5)}";
                hdr["WO"] = samplewo;
                hdr["PID"] = agiledata.CUSTPARTNO;
                hdr["WOTYPE"] = wotype.WOTYPE;
                hdr["PLANT"] = plantCode;
                //hdr["QTY"] = item.QTY;
                hdr["QTY"] = "1";
                hdr["PO"] = item.PONO;
                hdr["EXBOM"] = bomexflag;
                ZWO_HEADER.Rows.Add(hdr);
                //OPTION
                var itemoption = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.CTO.ExtValue()).ToList();
                var igonepn = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JNPPN_IGNORE" && t.FUNCTIONTYPE == "NOSYSTEM").ToList();
                foreach (var option in itemoption)
                {
                    if (igonepn.Any(t => t.VALUE == option.PARTNO))
                        continue;
                    var dr = ZWO_ITEM.NewRow();
                    dr["WO"] = samplewo;
                    dr["PN"] = option.PARTNO.ToUpper();
                    dr["QTY"] = option.QTY;
                    ZWO_ITEM.Rows.Add(dr);
                }
                //HB
                if (hbmap != null)
                {
                    DataRow hddr = ZWO_HIDBOM.NewRow();
                    hddr["WO"] = samplewo;
                    hddr["HB"] = hbmap.HBPN.ToUpper();
                    hddr["QTY"] = "";
                    ZWO_HIDBOM.Rows.Add(hddr);
                }

                rfc.SetValue(plantCode, "", "", ZWO_HEADER, ZWO_ITEM, ZWO_HIDBOM);
                rfc.CallRFC(() => { MesSapHelp.SapLog(item.ID, rfc.getSapParameobj(), db); });

                DataTable PODETAIL = rfc.GetTableValue("PODETAIL");
                DataTable BOM_LIST = rfc.GetTableValue("BOM_LIST");
                DataTable MINI_LIST = rfc.GetTableValue("MINI_LIST");

                var om_podetail = db.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == item.PREWO).ToList();
                var om_bomlist = db.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == item.PREWO).ToList();
                var om_minilist = db.Queryable<R_SAP_HB>().Where(t => t.WO == item.PREWO).ToList();
                try
                {
                    foreach (DataRow dr in PODETAIL.Rows)
                        if (!om_podetail.FindAll(t =>
                         //t.WO == dr["AUFNR"].ToString() && 
                         t.PLANT == dr["WERKS"].ToString() && t.PN.ToUpper() == dr["MATNR"].ToString().ToUpper()
                         && (t.PNREV == dr["REVLV"].ToString() || (t.PNREV == null && dr["REVLV"].ToString() == ""))
                         && t.ORDERQTY == dr["BDMNG"].ToString()
                         && (t.PIDREV == dr["FREVLV"].ToString() || (t.PIDREV == null && dr["FREVLV"].ToString() == ""))
                         && t.SPARTDESC.ToUpper() == dr["MAKTX"].ToString().ToUpper()
                         && t.PPARTDESC.ToUpper() == dr["FMAKTX"].ToString().ToUpper()).Any())
                            //throw new Exception($@"BOMCHANGE:The BOM has been changed(PODETAIL), pn: { dr["MATNR"].ToString() } ,rev: { dr["FREVLV"].ToString()}, Orderqty: { dr["BDMNG"].ToString()},pls check!");
                            throw new Exception($@"BOMCHANGE:The BoM has been changed (PODETAIL).  Pls regenerate the WO.");

                    foreach (DataRow dr in BOM_LIST.Rows)
                    {
                        if (!om_bomlist.FindAll(t =>
                           //t.WO == dr["AUFNR"].ToString() &&
                           t.PN.ToUpper() == dr["IDNRK"].ToString().ToUpper() && t.USAGE == dr["MENGE"].ToString()
                           && t.PARENTPN.ToUpper() == dr["MATNR"].ToString().ToUpper() && t.CUSTPN.ToUpper() == dr["PIDNRK"].ToString().ToUpper()
                           && t.CUSTPARENTPN.ToUpper() == dr["PMATNR"].ToString().ToUpper()
                           //&& ((t.CLEI1 == null && dr["POTX1"].ToString() == "") || t.CLEI1 == dr["POTX1"].ToString())
                           //&& ((t.CLEI2 == null && dr["POTX2"].ToString() == "") || t.CLEI2 == dr["POTX2"].ToString())
                           && t.SPARTDESC.ToUpper() == dr["MAKTX"].ToString().ToUpper() && t.PPARTDESC.ToUpper() == dr["FMAKTX"].ToString().ToUpper()).Any())
                            throw new Exception($@"BOMCHANGE:The BOM has been changed(BOM_LIST),pn: {dr["IDNRK"].ToString()},pls check!");
                    }
                    foreach (DataRow dr in MINI_LIST.Rows)
                        if (!om_minilist.FindAll(t =>
                              //t.WO == dr["AUFNR"].ToString() && 
                              t.PN.ToUpper() == dr["IDNRK"].ToString().ToUpper() && t.USAGE == dr["MENGE"].ToString()
                             && t.PARENTPN.ToUpper() == dr["MATNR"].ToString().ToUpper() && t.CUSTPN.ToUpper() == dr["PIDNRK"].ToString().ToUpper()
                             && t.CUSTPARENTPN.ToUpper() == dr["PMATNR"].ToString().ToUpper()
                             //&& ((t.CLEI1 == null && dr["POTX1"].ToString() == "") || t.CLEI1 == dr["POTX1"].ToString())
                             //&& ((t.CLEI2 == null && dr["POTX2"].ToString() == "") || t.CLEI2 == dr["POTX2"].ToString())
                             && t.SPARTDESC.ToUpper() == dr["MAKTX"].ToString().ToUpper() && t.PPARTDESC.ToUpper() == dr["FMAKTX"].ToString().ToUpper()).Any())
                            throw new Exception($@"BOMCHANGE:The BOM has been changed(Hb),pn: {dr["IDNRK"].ToString()},pls check!");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return new MesFunctionRes() { issuccess = true };
        }
        /// <summary>
        /// if bom change then order status= WaitCreatePreWo else return true
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bu"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static MesFunctionRes JuniperBomExplosion(O_ORDER_MAIN item, string bu, SqlSugarClient db)
        {
            var res = new MesFunctionRes();
            try
            {
                var rfc = new ZCPP_NSBG_0302(bu);
                var wotypelist = db.Queryable<O_J_WOTYPE>().ToList();
                var skuconfiglist = db.Queryable<O_SKU_CONFIG>().ToList();
                var i137item = db.Queryable<O_I137_ITEM>().Where(t => t.ID == item.ITEMID).ToList().FirstOrDefault();
                #region bomextend flag
                //var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                #endregion

                var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                var defaultplantcodeobj = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "DefaultPlantCode" && t.CATEGORY == "PlantCode" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList().FirstOrDefault();
                if (defaultplantcodeobj == null)
                    throw new Exception("pls config default plantcode!");
                var plantCode = skunewplant != null ? skunewplant.PLANTCODE : defaultplantcodeobj.VALUE.ToString();
                var wotype = wotypelist.FindAll(t => t.WOPRE == item.PREWO.Substring(0, 4)).ToList().FirstOrDefault();
                if (wotype == null)
                    throw new Exception($@"{item.PREWO } miss wo type!");
                var hbmap = db.Queryable<R_PN_HB_MAP>().Where(t => t.CUSTPN == item.CUSTPID).ToList().FirstOrDefault();
                var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault();

                #region bomextend flag
                var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == agiledata.USER_ITEM_TYPE && t.OFFERINGTYPE == agiledata.OFFERING_TYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                #endregion

                DataTable ZWO_HEADER = rfc.GET_NEW_ZWO_HEADER();
                DataTable ZWO_ITEM = rfc.GET_NEW_ZWO_ITEM();
                DataTable ZWO_HIDBOM = rfc.GET_NEW_ZWO_HIDBOM();
                DataRow hdr = ZWO_HEADER.NewRow();
                var samplewo = $@"{item.PREWO.Substring(0, 4)}Z{item.PREWO.Substring(5)}";
                hdr["WO"] = samplewo;
                hdr["PID"] = agiledata.CUSTPARTNO;
                hdr["WOTYPE"] = wotype.WOTYPE;
                hdr["PLANT"] = plantCode;
                //hdr["QTY"] = item.QTY;
                hdr["QTY"] = "1";
                hdr["PO"] = item.PONO;
                hdr["EXBOM"] = bomexflag;
                ZWO_HEADER.Rows.Add(hdr);
                //OPTION
                var itemoption = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.CTO.ExtValue()).ToList();
                foreach (var option in itemoption)
                {
                    var dr = ZWO_ITEM.NewRow();
                    dr["WO"] = samplewo;
                    dr["PN"] = option.PARTNO;
                    dr["QTY"] = option.QTY;
                    ZWO_ITEM.Rows.Add(dr);
                }
                //HB
                if (hbmap != null)
                {
                    DataRow hddr = ZWO_HIDBOM.NewRow();
                    hddr["WO"] = samplewo;
                    hddr["HB"] = hbmap.HBPN;
                    hddr["QTY"] = "";
                    ZWO_HIDBOM.Rows.Add(hddr);
                }

                rfc.SetValue(plantCode, "", "", ZWO_HEADER, ZWO_ITEM, ZWO_HIDBOM);
                rfc.CallRFC(() => { MesSapHelp.SapLog(item.ID, rfc.getSapParameobj(), db); });

                DataTable PODETAIL = rfc.GetTableValue("PODETAIL");
                DataTable BOM_LIST = rfc.GetTableValue("BOM_LIST");
                DataTable MINI_LIST = rfc.GetTableValue("MINI_LIST");

                var om_podetail = db.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == item.PREWO).ToList();
                var om_bomlist = db.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == item.PREWO).ToList();
                var om_minilist = db.Queryable<R_SAP_HB>().Where(t => t.WO == item.PREWO).ToList();
                try
                {
                    foreach (DataRow dr in PODETAIL.Rows)
                        if (!om_podetail.FindAll(t =>
                         //t.WO == dr["AUFNR"].ToString() && 
                         t.PLANT == dr["WERKS"].ToString() && t.PN == dr["MATNR"].ToString()
                         && (t.PNREV == dr["REVLV"].ToString() || (t.PNREV == null && dr["REVLV"].ToString() == ""))
                         && t.ORDERQTY == dr["BDMNG"].ToString()
                         && (t.PIDREV == dr["FREVLV"].ToString() || (t.PIDREV == null && dr["FREVLV"].ToString() == ""))
                         && t.SPARTDESC == dr["MAKTX"].ToString()
                         && t.PPARTDESC == dr["FMAKTX"].ToString()).Any())
                            throw new Exception($@"The BOM has been changed(PODETAIL), pn: { dr["MATNR"].ToString() } ,rev: { dr["FREVLV"].ToString()}, Orderqty: { dr["BDMNG"].ToString()},pls check!");

                    foreach (DataRow dr in BOM_LIST.Rows)
                    {
                        if (!om_bomlist.FindAll(t =>
                           //t.WO == dr["AUFNR"].ToString() &&
                           t.PN == dr["IDNRK"].ToString() && t.USAGE == dr["MENGE"].ToString()
                           && t.PARENTPN == dr["MATNR"].ToString() && t.CUSTPN == dr["PIDNRK"].ToString()
                           && t.CUSTPARENTPN == dr["PMATNR"].ToString()
                           && ((t.CLEI1 == null && dr["POTX1"].ToString() == "") || t.CLEI1 == dr["POTX1"].ToString())
                           && ((t.CLEI2 == null && dr["POTX2"].ToString() == "") || t.CLEI2 == dr["POTX2"].ToString())
                           && t.SPARTDESC == dr["MAKTX"].ToString() && t.PPARTDESC == dr["FMAKTX"].ToString()).Any())
                            throw new Exception($@"The BOM has been changed(BOM_LIST),pn: {dr["IDNRK"].ToString()},pls check!");
                    }
                    foreach (DataRow dr in MINI_LIST.Rows)
                        if (!om_minilist.FindAll(t =>
                              //t.WO == dr["AUFNR"].ToString() && 
                              t.PN == dr["IDNRK"].ToString() && t.USAGE == dr["MENGE"].ToString()
                             && t.PARENTPN == dr["MATNR"].ToString() && t.CUSTPN == dr["PIDNRK"].ToString()
                             && t.CUSTPARENTPN == dr["PMATNR"].ToString()
                             && ((t.CLEI1 == null && dr["POTX1"].ToString() == "") || t.CLEI1 == dr["POTX1"].ToString())
                             && ((t.CLEI2 == null && dr["POTX2"].ToString() == "") || t.CLEI2 == dr["POTX2"].ToString())
                             && t.SPARTDESC == dr["MAKTX"].ToString() && t.PPARTDESC == dr["FMAKTX"].ToString()).Any())
                            throw new Exception($@"The BOM has been changed(Hb),pn: {dr["IDNRK"].ToString()},pls check!");
                }
                catch (Exception e)
                {
                    using (var cdb = OleExec.GetSqlSugarClient(db.CurrentConnectionConfig.ConnectionString))
                    {
                        var updateorderres = cdb.Ado.UseTran(() =>
                        {
                            var tecores = TecoSapWo(bu, item.PREWO);
                            if (!tecores.issuccess)
                                throw new Exception(tecores.msg);
                            cdb.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == item.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ExecuteCommand();
                            cdb.Insertable(new O_PO_STATUS()
                            {
                                ID = MesDbBase.GetNewID<O_PO_STATUS>(cdb, Customer.JUNIPER.ExtValue()),
                                STATUSID = ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),
                                VALIDFLAG = MesBool.Yes.ExtValue(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                POID = item.ID
                            }).ExecuteCommand();
                        });
                        if (updateorderres.IsSuccess)
                            return new MesFunctionRes()
                            {
                                issuccess = false,
                                msg = e.Message
                            };
                        else
                            throw updateorderres.ErrorException;
                    }     
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return new MesFunctionRes() { issuccess = true };
        }
        public static MesFunctionRes RegeneratedWo(string bu, O_ORDER_MAIN item, SqlSugarClient cdb)
        {
            var updateorderres = cdb.Ado.UseTran(() =>
            {
                var tecores = TecoSapWo(bu, item.PREWO);
                if (!tecores.issuccess)
                    throw new Exception(tecores.msg);
                cdb.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == item.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ExecuteCommand();
                cdb.Insertable(new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(cdb, Customer.JUNIPER.ExtValue()),
                    STATUSID = ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    POID = item.ID
                }).ExecuteCommand();
            });
            if (updateorderres.IsSuccess)
                return new MesFunctionRes()
                {
                    issuccess = true
                };
            else
                throw updateorderres.ErrorException;
        }
        public static MesFunctionRes RegeneratedWoByOne(string bu, O_ORDER_MAIN item, SqlSugarClient cdb)
        {
            var juniperPreWoGanerate = new JuniperPreWoGanerate(string.Empty,bu);
            var waitHandleOrder = cdb.Queryable<O_ORDER_MAIN>().Where(m => m.UPOID == item.UPOID).ToList();
            var skuconfiglist = cdb.Queryable<O_SKU_CONFIG>().ToList();
            var tecores = TecoSapWo(bu, item.PREWO);
            if (!tecores.issuccess)
                throw new Exception(tecores.msg);
            var res =juniperPreWoGanerate.WoGanerate(cdb, waitHandleOrder, skuconfiglist);
            if (res.Any(t=>t.issucess==true))
                return new MesFunctionRes()
                {
                    issuccess = true,
                    msg = res.FirstOrDefault().wo
                };
            else
                throw new Exception(res.Count>0?res.FirstOrDefault().msg: "miss Agile Data!");
        }
        public static MesFunctionRes RelSapWo(string bu, string wo)
        {
            //SAP CALL RFC ERROR:max no of 200 conversations exceeded,Change rfc to Static
            if (rfc140 == null)
            {
                rfc140 = new ZCPP_NSBG_0140(bu);
            }
            lock (rfc140)
            {
                rfc140.ClearValues();
                rfc140.SetValues(wo);
                rfc140.CallRFC();
                return new MesFunctionRes()
                {
                    issuccess = rfc140.GetValue("OUT_IND").ToString() == "0" || rfc140.GetValue("OUT_MSG").ToString().IndexOf("Status is not CRTD") > -1 ? true : false,
                    msg = rfc140.GetValue("OUT_MSG").ToString()
                };
            }
        }

        public static MesFunctionRes ChangeCrsdWithSap(string bu, string wo, string crsd)
        {
            //SAP CALL RFC ERROR:max no of 200 conversations exceeded, Change rfc to Static
            if (rfc141 == null)
            {
                rfc141 = new ZCPP_NSBG_0141(bu);
            }
            lock (rfc141)
            {
                rfc141.ClearValues();
                rfc141.SetValues(wo, crsd);
                rfc141.CallRFC();
                return new MesFunctionRes()
                {
                    //issuccess = rfc141.GetValue("OUT_IND").ToString() == "0" ? true : false,
                    issuccess = new Func<bool>(() =>
                    {
                        if (rfc141.GetValue("OUT_IND").ToString() == "0")
                            return true;
                        if (rfc141.GetValue("OUT_IND").ToString() != "0" && !(rfc141.GetValue("OUT_MSG").ToString().IndexOf("Runtime error DYNPRO_SEND_IN_BACKGROUND has occurr") > -1))
                            return true;
                        return false;
                    })(),
                    msg = rfc141.GetValue("OUT_MSG").ToString()
                };
            }
        }
        /// <summary>
        /// Check Wo create in sap && wo havn't product =>return true
        /// </summary>
        /// <param name="mesdb"></param>
        /// <param name="ordermaincurrent"></param>
        /// <returns></returns>
        public static bool CheckCrtdStatusWithOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent, O_PO_STATUS postatus)
        {
            return (postatus.STATUSID == ENUM_O_PO_STATUS.RmqEnd.ExtValue() && ordermaincurrent.CANCEL==MesBool.No.ExtValue()) || postatus.STATUSID == ENUM_O_PO_STATUS.DownloadWo.ExtValue()
                || (postatus.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue() && !mesdb.Queryable<R_SN>().Any(t => t.WORKORDERNO == ordermaincurrent.PREWO && t.VALID_FLAG == MesBool.Yes.ExtValue()));
        }

        /// <summary>
        /// Check wo havn't product =>return true
        /// </summary>
        /// <param name="mesdb"></param>
        /// <param name="ordermaincurrent"></param>
        /// <returns></returns>
        public static bool CheckNotProductStatusWithOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent)
        {
            return !mesdb.Queryable<R_SN>().Any(t => t.WORKORDERNO == ordermaincurrent.PREWO && t.VALID_FLAG == MesBool.Yes.ExtValue());
        }

        public static MesFunctionRes TecoSapWo(string bu, string wo)
        {
            try
            {
                if (bu.StartsWith(ENUM_JNP_SITE.FJZ.ExtValue()))
                {
                    //FJZ
                    if (rfc009 == null)
                    {
                        rfc009 = new ZCPP_NSBG_0009(bu);
                    }
                    lock (rfc009)
                    {
                        rfc009.ClearValues();
                        rfc009.SetValues(wo, true);
                        rfc009.CallRFC();
                        var dtres = rfc009.GetTableValue("WOACT");
                        return new MesFunctionRes()
                        {
                            issuccess = new Func<bool>(() =>
                            {
                                if (dtres.Rows.Count == 0)
                                    return false;
                                if (dtres.Rows[0]["MSGTY"].ToString().Equals("E") && !(dtres.Rows[0]["MESSAGE"].ToString().StartsWith("Modify WO failed") || dtres.Rows[0]["MESSAGE"].ToString().IndexOf("not found (check entry)") > -1 || dtres.Rows[0]["MESSAGE"].ToString().IndexOf("only WO scheudling type is 3 that startdate can in")>-1))
                                    return false;
                                return true;
                            })(),
                            msg = dtres.Rows.Count == 0 ? "" : dtres.Rows[0]["MESSAGE"].ToString()
                        };
                    }
                }
                else
                {
                    //FVN
                    if (rfc045 == null)
                    {
                        rfc045 = new ZCPP_NSBG_0045(bu);
                    }
                    lock (rfc045)
                    {
                        rfc045.ClearValues();
                        rfc045.SetValues(wo, true);
                        rfc045.CallRFC();
                        var dtres = rfc045.GetTableValue("WOACT");
                        return new MesFunctionRes()
                        {
                            issuccess = new Func<bool>(() =>
                            {
                                if (dtres.Rows.Count == 0)
                                    return false;
                                if (dtres.Rows[0]["MSGTY"].ToString().Equals("E") && !(dtres.Rows[0]["MESSAGE"].ToString().StartsWith("Modify WO failed") || dtres.Rows[0]["MESSAGE"].ToString().IndexOf("not found (check entry)")>-1 || dtres.Rows[0]["MESSAGE"].ToString().IndexOf("only WO scheudling type is 3 that startdate can in") > -1))
                                    return false;
                                return true;
                            })(),
                            msg = dtres.Rows.Count == 0 ? "" : dtres.Rows[0]["MESSAGE"].ToString()
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return new MesFunctionRes()
                {
                    issuccess = false,
                    msg = e.Message
                };
            }

        }
    }
}
