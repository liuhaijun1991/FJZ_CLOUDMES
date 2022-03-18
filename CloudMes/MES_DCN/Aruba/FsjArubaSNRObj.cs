using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using MESPubLab.Common;
using MESDataObject.Module.ARUBA;
using System.Globalization;
using System.Data;
using System.IO;

namespace MES_DCN.Aruba
{
    public class FsjArubaSNRObj
    {
        private string _mesdbstr, _apdbstr, _bustr, _filepath, _remotepath, _datatype, _keytype, _sourceType, _station;
        private SqlSugarClient sfcdb;
        private SqlSugarClient apdb;

        #region B2B sftp
        //TEData/PROD 正式環境
        //TEData/TEST 測試環境
        string CONST_SFTPHost = "10.132.48.74";
        string CONST_SFTPPort = "8022";
        string CONST_SFTPLogin = "hpe";
        string CONST_SFTPPassword = "0s02QtFZ";
        #endregion

        public FsjArubaSNRObj(string mesdbstr, string apdbstr, string bustr, string filepath, string remotepath, string datatype, string keytype)
        {
            _mesdbstr = mesdbstr;
            _apdbstr = apdbstr;
            _bustr = bustr;
            _filepath = filepath;
            _remotepath = remotepath;
            _datatype = datatype;
            _keytype = keytype;
            sfcdb = MESDBHelper.OleExec.GetSqlSugarClient(this._mesdbstr, false, SqlSugar.DbType.SqlServer);
            apdb = MESDBHelper.OleExec.GetSqlSugarClient(this._apdbstr, false);
            _sourceType = "ARUBA";
            _station = "SHIPOUT";
            if (_datatype == "SNR_BEFORE") _station = "CBS";
        }

        public void Build()
        {
            try
            {
                sfcdb.Ado.Open();
                apdb.Ado.Open();
                this.BuildHead();
                this.GetSNRData();
                this.ConvertData();
                this.SendSNRData();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                sfcdb.Close();
                apdb.Close();
            }
        }

        public void BuildHead()
        {
            try
            {
                var sysdate = sfcdb.GetDate(); //取數據庫時間好點
                var checkFrom = sysdate.AddDays(-30).Date;
                var checkTo = sysdate.Date;

                checkFrom = DateTime.Parse("2022/02/22");
                checkTo = DateTime.Parse("2022/02/23");
                string keyValue = string.Empty;
                DateTime startTime = DateTime.Parse("9999/12/31 23:59:59");
                DateTime endTime = DateTime.Parse("9999/12/31 23:59:59");
                while (checkFrom < checkTo)
                {
                    #region Get KeyType info
                    var StrcheckFrom = checkFrom.ToString("yyyyMMdd");
                    if (_keytype == "DAILY")
                    {
                        keyValue = StrcheckFrom.Substring(0, 8);
                        startTime = DateTime.Parse(keyValue.Substring(0, 4) + "/" + keyValue.Substring(4, 2) + "/" + keyValue.Substring(6, 2));
                        endTime = startTime.AddDays(1);
                    }
                    else if (_keytype == "MONTHLY")
                    {
                        keyValue = StrcheckFrom.Substring(0, 6);
                        startTime = DateTime.Parse(keyValue.Substring(0, 4) + "/" + keyValue.Substring(4, 2) + "/" + "01");
                        endTime = startTime.AddMonths(1);
                    }
                    else if (_keytype == "WEEKLY")
                    {
                        GregorianCalendar gregorianCalendar = new GregorianCalendar();
                        var ww = gregorianCalendar.GetWeekOfYear(checkFrom, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
                        DayOfWeek dayOfWeek = gregorianCalendar.GetDayOfWeek(checkFrom);
                        startTime = checkFrom.AddDays(-((int)dayOfWeek));
                        endTime = startTime.AddDays(7);
                        keyValue = startTime.ToString("yyyyMMdd").Substring(0, 4) + (ww.ToString().Length == 1 ? "0" + ww.ToString() : ww.ToString());
                    }
                    else
                    {
                        throw new Exception($@"BuildHead Err: keytype {_keytype} not exists(DAILY/WEEKLY/MONTHLY), Please check");
                    }
                    #endregion

                    if (endTime <= checkTo)
                    {
                        #region Insert R_SNR_HEAD
                        if (sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && t.Cancel == "N"
                         && ((t.Start_Time > startTime && t.Start_Time < endTime) || (t.End_Time > startTime && t.End_Time < endTime))).Any())
                        {
                            throw new Exception($@"BuildHead Err:{_datatype} and {_keytype} had same time startTime:{startTime.ToString("yyyyMMddHHmmss")} endTime:{endTime.ToString("yyyyMMddHHmmss")}");
                        }

                        if (!sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && t.Key_Value == keyValue && t.Cancel == "N").Any())
                        {
                            int seq = 1;
                            var headSeq = sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && SqlFunc.StartsWith(t.Key_Value, keyValue.Substring(0, 4))).OrderBy(t => t.Seq, OrderByType.Desc).ToList().FirstOrDefault();
                            if (headSeq != null) seq = int.Parse(headSeq.Seq.ToString()) + 1;
                            var RId = GetRId();
                            R_SNR_HEAD head = new R_SNR_HEAD
                            {
                                RId = RId,
                                Data_Type = _datatype,
                                Key_Type = _keytype,
                                Key_Value = keyValue,
                                Start_Time = startTime,
                                End_Time = endTime,
                                Seq = seq,
                                Get = "N",
                                Conver = "N",
                                Send = "N",
                                Cancel = "N"
                            };
                            sfcdb.Insertable<R_SNR_HEAD>(head).ExecuteCommand();
                        }
                        #endregion
                    }
                    checkFrom = checkFrom.AddDays(1);
                }
            }
            catch (Exception e)
            {
                MesLog.Info($@"BuildHead Err:{e.Message}");
                throw new Exception(e.Message);
            }
            finally
            {
                MesLog.Info("BuildHead End");
            }
        }

        public void GetSNRData()
        {
            try
            {
                var head = sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && t.Cancel == "N" && t.Get == "N").OrderBy(t => t.Key_Value, OrderByType.Asc).ToList();
                foreach (var h in head)
                {
                    List<R_SNR_PC> PCList = new List<R_SNR_PC>();
                    List<R_SNR_BP> BPList = new List<R_SNR_BP>();
                    List<R_SNR_BC> BCList = new List<R_SNR_BC>();
                    List<R_SNR_HC> HCList = new List<R_SNR_HC>();
                    List<R_SNR_VP> VPList = new List<R_SNR_VP>();
                    List<R_SNR_RT> RTList = new List<R_SNR_RT>();
                    List<R_SNR_SB> SBList = new List<R_SNR_SB>();

                    DeleteData(h.RId);

                    //Get PO,SHIP_TO_ADDRESS
                    var strSql = $@"select serial_number, po_no, street, city, country
                                      from (select a.sysserialno as serial_number,
                                                   f.po_no,
                                                   h.street,
                                                   h.city,
                                                   h.country,
                                                   row_number() over(partition by a.sysserialno order by a.scandatetime desc, e.lasteditdt desc, f.worktime desc) as rn
                                              from mfsysevent a(nolock)
                                             inner join mmprodmaster b(nolock)
                                                on b.partno = a.skuno
                                               and b.sourcetype = '{_sourceType}'
                                             inner join mfsysproduct c(nolock)
                                                on c.sysserialno = a.sysserialno
                                               and c.skuno = b.partno
                                             inner join mfworkstatus d(nolock)
                                                on d.sysserialno = a.sysserialno
                                              left join asshipped e(nolock)
                                                on e.topasssn = a.sysserialno
                                              left join r_dn_brocade f(nolock)
                                                on f.ship_order = e.shiporderno
                                               and f.order_no = e.sono
                                               and f.order_lineno = e.solineno
                                              left join r_to_detail g(nolock)
                                                on g.dn_no = f.dn_no
                                              left join r_report_list h(nolock)
                                                on h.to_no = g.to_no
                                             where a.eventname = '{_station}'
                                               and substring(a.sysserialno, 1, 1) not in ('#', '*', '~', '-')
                                               and a.scandatetime >= '{h.Start_Time}'
                                               and a.scandatetime < '{h.End_Time}') aa
                                     where aa.rn = 1";
                    var ShipToAddress = sfcdb.SqlQueryable<R_SNR_SHIP_TO_ADDRESS>(strSql).ToList();

                    // Parent SN
                    strSql = $@"select RId,
                                           Record_Type_Id,
                                           Serial_Number,
                                           Product_Number,
                                           Record_Origin_Id,
                                           Sub_FA_Origin_Id,
                                           Localization_Hardware_Option,
                                           Warranty_Option,
                                           Other_Options,
                                           Date,
                                           Asset_Tag,
                                           Future,
                                           Test_Result,
                                           Nb_Sub_Modules
                                      from (select '{h.RId}' as RId,
                                                   'PC' as Record_Type_Id,
                                                   a.sysserialno as Serial_Number,
                                                   a.Skuno as Product_Number,
                                                   2 as Record_Origin_Id,
                                                   'F1SJ01' as Sub_FA_Origin_Id,
                                                   '' as Localization_Hardware_Option,
                                                   '' as Warranty_Option,
                                                   '' as Other_Options,
                                                   replace(replace(replace(convert(varchar(100),
                                                                                   d.completedate,
                                                                                   120),
                                                                           '-',
                                                                           ''),
                                                                   ':',
                                                                   ''),
                                                           ' ',
                                                           '') + '/' + replace(replace(replace(convert(varchar(100),
                                                                                                       a.scandatetime,
                                                                                                       120),
                                                                                               '-',
                                                                                               ''),
                                                                                       ':',
                                                                                       ''),
                                                                               ' ',
                                                                               '') as Date,
                                                   '' as Asset_Tag,
                                                   '' as Future,
                                                   'Y' as Test_Result,
                                                   0 as Nb_Sub_Modules,
                                                   ROW_NUMBER() OVER(PARTITION BY a.sysserialno ORDER BY a.scandatetime, d.completedate desc) AS RN
                                              from mfsysevent a(nolock)
                                             inner join mmprodmaster b(nolock)
                                                on b.PartNo = a.Skuno
                                               and b.SourceType = '{_sourceType}'
                                             inner join mfsysproduct c(nolock)
                                                on c.sysserialno = a.sysserialno
                                               and c.skuno = b.PartNo
                                             inner join mfworkstatus d(nolock)
                                                on d.sysserialno = a.sysserialno
                                             where a.eventname = '{_station}'
                                               and substring(a.sysserialno, 1, 1) not in ('#', '*', '~', '-')
                                               and a.scandatetime >= '{h.Start_Time}'
                                               and a.scandatetime < '{h.End_Time}') aa
                                     where aa.rn = 1";
                    var parentPCList = sfcdb.SqlQueryable<R_SNR_PC>(strSql).ToList();
                    foreach (var parentPC in parentPCList)
                    {
                        var skunoTemp = parentPC.Product_Number;
                        var sfcCodeLikeExtend = GetSfcCodeLikeExtend(parentPC.Product_Number);
                        parentPC.Product_Number = sfcCodeLikeExtend.VALUE2;
                        var date = parentPC.Date.Split('/'); // completedate/shipdate(or scandatetime)
                        var STA = ShipToAddress.Find(t => t.Serial_Number == parentPC.Serial_Number);
                        string po = "";
                        string address = "";
                        if (STA != null)
                        {
                            po = STA.PO == null ? "" : STA.PO.Trim();
                            address = STA.Street == null ? "" : STA.Street.Trim() + "," + STA.City == null ? "" : STA.City.Trim() + "," + STA.Country == null ? "" : STA.Country.Trim();
                        }
                        List<R_SNR_BC> parentBCListTemp = new List<R_SNR_BC>();
                        #region 問題多多
                        //strSql = $@"WITH cao1 AS
                        //             (SELECT cao2.sysserialno, cao2.cserialno
                        //                FROM mfsyscserial cao2(NOLOCK)
                        //               WHERE sysserialno = '{parentPC.Serial_Number}'
                        //              UNION ALL
                        //              SELECT cao3.sysserialno, cao3.cserialno
                        //                FROM mfsyscserial cao3(NOLOCK)
                        //               INNER JOIN cao1 a3
                        //                  ON cao3.sysserialno = a3.cserialno)
                        //            SELECT DISTINCT cao1.cserialno as CSN, cao4.skuno
                        //              FROM cao1
                        //             INNER JOIN mfsysproduct cao4(NOLOCK)
                        //                ON cao4.sysserialno = cao1.cserialno
                        //             WHERE cao1.sysserialno <> cao1.cserialno
                        //               AND substring(cao1.cserialno, 1, 1) not in ('#', '*', '~', '-')
                        //             ORDER BY cao4.skuno, cao1.cserialno";
                        #endregion
                        #region Old logic
                        //strSql = $@"SELECT distinct a.cserialno as CSN, b.skuno
                        //              FROM mfsyscserial a(NOLOCK)
                        //             INNER join mfsysproduct b(NOLOCK)
                        //                ON b.sysserialno = a.cserialno
                        //             WHERE a.sysserialno <> a.cserialno
                        //               AND substring(a.cserialno, 1, 1) not in ('#', '*', '~', '-')
                        //               AND a.sysserialno = '{parentPC.Serial_Number}'";
                        #endregion
                        strSql = $@"SELECT distinct a.cserialno as CSN, b.skuno
                                      FROM mfsyscserial a(NOLOCK)
                                     INNER join mfsysproduct b(NOLOCK)
                                        ON b.sysserialno = a.cserialno
                                     WHERE a.sysserialno <> a.cserialno
                                       AND substring(a.cserialno, 1, 1) not in ('#', '*', '~', '-')
                                       AND a.sysserialno = '{parentPC.Serial_Number}'
                                       and not exists(select 1 from mfsyscserial c(NOLOCK) where c.sysserialno = a.cserialno and c.categoryname = 'MAC S/N' and a.categoryname = 'MB S/N')
									UNION 
									select distinct a.cserialno as CSN,a.partno as skuno from mfsyscserial a(NOLOCK) where 
                                       a.sysserialno = '{parentPC.Serial_Number}' and a.categoryname in ('FAN S/N','PSU S/N')
									   and substring(a.cserialno, 1, 1) not in ('#', '*', '~', '-')";
                        DataTable parentBC = sfcdb.Ado.GetDataTable(strSql);
                        if (parentBC.Rows.Count > 0)
                        {
                            R_SNR_BP BP = new R_SNR_BP
                            {
                                RId = h.RId,
                                Record_Type_Id = "BP",
                                Serial_Number = parentPC.Serial_Number,
                                Product_Number = parentPC.Product_Number,
                                Bundle_Description = sfcCodeLikeExtend.VALUE1, //"Bundle",
                                Record_Origin_Id = 1,
                                Sub_FA_Origin_Id = parentPC.Sub_FA_Origin_Id,
                                Localization_Hardware_Option = parentPC.Localization_Hardware_Option,
                                Warranty_Option = "",
                                Other_Options = "",
                                Ship_Date = date[1],
                                Asset_Tag = "",
                                Future = "",
                                Num_Products_In_Bundle = parentBC.Rows.Count
                            };
                            BPList.Add(BP);
                            foreach (DataRow r in parentBC.Rows)
                            {
                                sfcCodeLikeExtend = GetSfcCodeLikeExtend(r["SKUNO"].ToString());
                                R_SNR_BC BC = new R_SNR_BC
                                {
                                    RId = h.RId,
                                    Record_Type_Id = "BC",
                                    Serial_Number = r["CSN"].ToString(),
                                    Product_Number = sfcCodeLikeExtend.VALUE2,//r["SKUNO"].ToString(),
                                    Localization_Option = BP.Localization_Hardware_Option,
                                    Parent_Serial_Number = parentPC.Serial_Number
                                };
                                BCList.Add(BC);
                                parentBCListTemp.Add(BC);
                            }
                        }
                        parentPC.Date = date[0];
                        PCDetail pcDetail = GetPCDetail(parentBC.Rows.Count > 0 ? "BP" : "BC", h.RId, parentPC.Serial_Number, parentPC.Date, date[1], po, address, parentPC.Product_Number, parentPC.Serial_Number);
                        if (pcDetail.hcList.Count > 0) HCList.AddRange(pcDetail.hcList);
                        if (pcDetail.vpList.Count > 0) VPList.AddRange(pcDetail.vpList);
                        if (pcDetail.rtList.Count > 0) RTList.AddRange(pcDetail.rtList);
                        if (pcDetail.sbList.Count > 0) SBList.AddRange(pcDetail.sbList);
                        parentPC.Nb_Sub_Modules = pcDetail.hcList.Count + pcDetail.vpList.Count + pcDetail.rtList.Count + pcDetail.sbList.Count;
                        foreach (R_SNR_BC childBC in parentBCListTemp)
                        {
                            #region Sql
                            strSql = $@"select RId,
                                               Record_Type_Id,
                                               Serial_Number,
                                               Product_Number,
                                               Record_Origin_Id,
                                               Sub_FA_Origin_Id,
                                               Localization_Hardware_Option,
                                               Warranty_Option,
                                               Other_Options,
                                               Date,
                                               Asset_Tag,
                                               Future,
                                               Test_Result,
                                               Nb_Sub_Modules
                                          from (select '{h.RId}' as RId,
                                                       'PC' as Record_Type_Id,
                                                       a.sysserialno as Serial_Number,
                                                       a.Skuno as Product_Number,
                                                       2 as Record_Origin_Id,
                                                       'F1SJ01' as Sub_FA_Origin_Id,
                                                       '' as Localization_Hardware_Option,
                                                       '' as Warranty_Option,
                                                       '' as Other_Options,
                                                       replace(replace(replace(convert(varchar(100),
                                                                                       c.completedate,
                                                                                       120),
                                                                               '-',
                                                                               ''),
                                                                       ':',
                                                                       ''),
                                                               ' ',
                                                               '') as Date,
                                                       '' as Asset_Tag,
                                                       '' as Future,
                                                       'Y' as Test_Result,
                                                       0 as Nb_Sub_Modules,
                                                       ROW_NUMBER() OVER(PARTITION BY a.sysserialno ORDER BY c.completedate desc) AS RN
                                                  from mfsysproduct a(nolock)
                                                 inner join mfworkstatus c(nolock)
                                                    on c.sysserialno = a.sysserialno
                                                 where a.sysserialno = '{childBC.Serial_Number}') aa
                                         where aa.rn = 1";
                            #endregion
                            var childPC = sfcdb.SqlQueryable<R_SNR_PC>(strSql).ToList().FirstOrDefault();
                            if (childPC == null)
                            {
                                childPC = new R_SNR_PC
                                {
                                    RId = h.RId,
                                    Record_Type_Id = "PC",
                                    Serial_Number = childBC.Serial_Number,
                                    Product_Number = childBC.Product_Number,
                                    Record_Origin_Id = 2,
                                    Sub_FA_Origin_Id = "F1SJ01",
                                    Localization_Hardware_Option = "",
                                    Warranty_Option = "",
                                    Other_Options = "",
                                    Date = date[0],
                                    Asset_Tag = "",
                                    Future = "",
                                    Test_Result = "",
                                    Nb_Sub_Modules = 0
                                };
                            }
                            if (childPC != null)
                            {
                                skunoTemp = childPC.Product_Number;
                                sfcCodeLikeExtend = GetSfcCodeLikeExtend(childPC.Product_Number);
                                childPC.Product_Number = sfcCodeLikeExtend.VALUE2;
                                pcDetail = GetPCDetail("BC", h.RId, childPC.Serial_Number, childPC.Date, date[1], po, address, childPC.Product_Number, childBC.Parent_Serial_Number);
                                if (pcDetail.hcList.Count > 0) HCList.AddRange(pcDetail.hcList);
                                if (pcDetail.vpList.Count > 0) VPList.AddRange(pcDetail.vpList);
                                if (pcDetail.rtList.Count > 0) RTList.AddRange(pcDetail.rtList);
                                if (pcDetail.sbList.Count > 0) SBList.AddRange(pcDetail.sbList);
                                childPC.Nb_Sub_Modules = pcDetail.hcList.Count + pcDetail.vpList.Count + pcDetail.rtList.Count + pcDetail.sbList.Count;
                                PCList.Add(childPC);
                            }
                        }
                    }
                    if (parentPCList.Count > 0) PCList.AddRange(parentPCList);
                    if (BPList.Count > 0) sfcdb.Insertable(BPList).ExecuteCommand();
                    if (BCList.Count > 0) sfcdb.Insertable(BCList).ExecuteCommand();
                    if (PCList.Count > 0) sfcdb.Insertable(PCList).ExecuteCommand();
                    if (HCList.Count > 0) sfcdb.Insertable(HCList).ExecuteCommand();
                    if (VPList.Count > 0) sfcdb.Insertable(VPList).ExecuteCommand();
                    if (RTList.Count > 0) sfcdb.Insertable(RTList).ExecuteCommand();
                    if (SBList.Count > 0) sfcdb.Insertable(SBList).ExecuteCommand();

                    h.Get = "Y";
                    h.Get_Time = sfcdb.GetDate();
                    sfcdb.Updateable<R_SNR_HEAD>(h).Where(t => t.RId == h.RId).ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                MesLog.Info($@"GetSNRData Err:{e.Message}");
                throw new Exception(e.Message);
            }
            finally
            {
                MesLog.Info("GetSNRData End");
            }
        }

        public void ConvertData()
        {
            try
            {
                var head = sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && t.Cancel == "N" && t.Get == "Y" && t.Conver == "N").OrderBy(t => t.Key_Value, OrderByType.Asc).ToList();
                foreach (var h in head)
                {
                    List<string> line = new List<string>();
                    var sysdate = sfcdb.GetDate(); //取數據庫時間好點
                    h.File_Name = $@"HPE.WWSNR.FOXSANJOSE.US.F1SJ.15.{string.Format("{0:D6}", h.Seq)}_{sysdate.ToString("yyyyMMdd")}_PTR50.DAT";
                    var date = ((DateTime)h.End_Time).AddDays(-1).ToString("yyyyMMdd");
                    if (_keytype == "WEEKLY") date = ((DateTime)h.End_Time).AddDays(-2).ToString("yyyyMMdd");
                    h.Header_Record = $@"H-PTR|PLFA|F1SJ|{string.Format("{0:D6}", h.Seq)}|{sysdate.ToString("yyyyMMdd")}|{sysdate.ToString("HHmmss")}|{h.Key_Type.Substring(0, 1)}|{date}|PTR5.0|57";
                    var PBList = sfcdb.Queryable<R_SNR_BP>().Where(t => t.RId == h.RId).OrderBy(t => t.Serial_Number, OrderByType.Asc).ToList();
                    var BCList = sfcdb.Queryable<R_SNR_BC>().Where(t => t.RId == h.RId).ToList();
                    var HCList = sfcdb.Queryable<R_SNR_HC>().Where(t => t.RId == h.RId).ToList();
                    var VPList = sfcdb.Queryable<R_SNR_VP>().Where(t => t.RId == h.RId).ToList();
                    var RTList = sfcdb.Queryable<R_SNR_RT>().Where(t => t.RId == h.RId).ToList();
                    var SBList = sfcdb.Queryable<R_SNR_SB>().Where(t => t.RId == h.RId).ToList();
                    var PCList = sfcdb.Queryable<R_SNR_PC>().Where(t => t.RId == h.RId).OrderBy(t => t.Serial_Number, OrderByType.Asc).ToList();
                    foreach (var BP in PBList)
                    {
                        //BP
                        var BC = BCList.FindAll(t => t.Parent_Serial_Number == BP.Serial_Number).OrderBy(t => t.Serial_Number);
                        string sBP = $@"{BP.Record_Type_Id}|{BP.Serial_Number}|{BP.Product_Number}|{BP.Bundle_Description}|{BP.Record_Origin_Id}|" +
                                     $@"{BP.Sub_FA_Origin_Id}|{BP.Localization_Hardware_Option}|{BP.Warranty_Option}|{BP.Other_Options}|{BP.Ship_Date}|" +
                                     $@"{BP.Asset_Tag}|{BP.Future}|{BP.Num_Products_In_Bundle}|";
                        foreach (var bc in BC)
                        {
                            sBP = sBP + $@"{bc.Record_Type_Id}|{bc.Serial_Number}|{bc.Product_Number}|{bc.Localization_Option}|";
                        }
                        sBP = sBP + sBP.Length;
                        line.Add(sBP);

                        //PC by BP
                        var PCListByBP = PCList.FindAll(t => t.Serial_Number == BP.Serial_Number).OrderBy(t => t.Serial_Number);
                        foreach (var PC in PCListByBP)
                        {
                            string sPCByBP = $@"{PC.Record_Type_Id}|{PC.Serial_Number}|{PC.Product_Number}|{PC.Record_Origin_Id}|{PC.Sub_FA_Origin_Id}|" +
                                         $@"{PC.Localization_Hardware_Option}|{PC.Warranty_Option}|{PC.Other_Options}|{PC.Date}|{PC.Asset_Tag}|" +
                                         $@"{PC.Future}|{PC.Test_Result}|{PC.Nb_Sub_Modules}|";
                            var HCByBP = HCList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                            foreach (var hc in HCByBP)
                            {
                                sPCByBP = sPCByBP + $@"{hc.Module_Type_Id}|{hc.Generic_Category}|{hc.HP_Component_Part_Number}|{hc.Supplier_Part_Number}|{ hc.Serial_Number}|" +
                                            $@"{hc.CT_Serial_Number_Or_Date_Code}|{hc.Hardware_Revision}|{hc.Firmware_Revision}|{hc.Supplier_Name}|{hc.E_T_Status}|" +
                                            $@"{hc.Type_Of_Operation}|{hc.Quantity}|{hc.Parent_Product}|{hc.Family}|{ hc.Part_Category_Or_Commodity_Code}|" +
                                            $@"{hc.Description}|{hc.Eatra_Info}|";
                            }

                            var VPByBP = VPList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                            foreach (var vp in VPByBP)
                            {
                                sPCByBP = sPCByBP + $@"{vp.Module_Type_Id}|{vp.Generic_Category}|{vp.HP_Component_Part_Number}|{vp.Supplier_Part_Number}|{ vp.Serial_Number}|" +
                                            $@"{vp.Date_Code}|{vp.Hardware_Revision}|{vp.Firmware_Revision}|{vp.Supplier_Name}|{vp.E_T_Status}|" +
                                            $@"{vp.Type_Of_Operation}|{vp.Quantity}|{vp.Parent_Product}|{vp.Family}|{ vp.Part_Category_Or_Commodity_Code}|" +
                                            $@"{vp.Description}|{vp.Eatra_Info}|";
                            }

                            var RTByBP = RTList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                            foreach (var rt in RTByBP)
                            {
                                sPCByBP = sPCByBP + $@"{rt.Module_Type_Id}|{rt.Generic_Category}|{rt.IMG_Descriptor}||||||||{rt.Operation}|" +
                                            $@"{ rt.Quantity}||||{rt.Dependent_On_IMG_Descriptor}||";
                            }

                            var SBByBP = SBList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                            foreach (var sb in SBByBP)
                            {
                                sPCByBP = sPCByBP + $@"{sb.Module_Type_Id}|{sb.V_Product_Number}|{sb.Option_Localization}|{sb.Quantity}|{ sb.Description}|" +
                                            $@"{sb.Feature_Code}|{sb.Feature_Value}|{sb.Other_Options}|{sb.Type_Of_Operation}|{sb.Future}|";
                            }

                            sPCByBP = sPCByBP + sPCByBP.Length;
                            line.Add(sPCByBP);
                        }

                        //PC by BC
                        foreach (var bc in BC)
                        {
                            var PCListByBC = PCList.FindAll(t => t.Serial_Number == bc.Serial_Number).OrderBy(t => t.Serial_Number);
                            foreach (var PC in PCListByBC)
                            {
                                string sPCByBC = $@"{PC.Record_Type_Id}|{PC.Serial_Number}|{PC.Product_Number}|{PC.Record_Origin_Id}|{PC.Sub_FA_Origin_Id}|" +
                                             $@"{PC.Localization_Hardware_Option}|{PC.Warranty_Option}|{PC.Other_Options}|{PC.Date}|{PC.Asset_Tag}|" +
                                             $@"{PC.Future}|{PC.Test_Result}|{PC.Nb_Sub_Modules}|";
                                var HCByBC = HCList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                                foreach (var hc in HCByBC)
                                {
                                    sPCByBC = sPCByBC + $@"{hc.Module_Type_Id}|{hc.Generic_Category}|{hc.HP_Component_Part_Number}|{hc.Supplier_Part_Number}|{ hc.Serial_Number}|" +
                                                $@"{hc.CT_Serial_Number_Or_Date_Code}|{hc.Hardware_Revision}|{hc.Firmware_Revision}|{hc.Supplier_Name}|{hc.E_T_Status}|" +
                                                $@"{hc.Type_Of_Operation}|{hc.Quantity}|{hc.Parent_Product}|{hc.Family}|{ hc.Part_Category_Or_Commodity_Code}|" +
                                                $@"{hc.Description}|{hc.Eatra_Info}|";
                                }

                                var VPByBC = VPList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                                foreach (var vp in VPByBC)
                                {
                                    sPCByBC = sPCByBC + $@"{vp.Module_Type_Id}|{vp.Generic_Category}|{vp.HP_Component_Part_Number}|{vp.Supplier_Part_Number}|{ vp.Serial_Number}|" +
                                                $@"{vp.Date_Code}|{vp.Hardware_Revision}|{vp.Firmware_Revision}|{vp.Supplier_Name}|{vp.E_T_Status}|" +
                                                $@"{vp.Type_Of_Operation}|{vp.Quantity}|{vp.Parent_Product}|{vp.Family}|{ vp.Part_Category_Or_Commodity_Code}|" +
                                                $@"{vp.Description}|{vp.Eatra_Info}|";
                                }

                                var RTByBC = RTList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                                foreach (var rt in RTByBC)
                                {
                                    sPCByBC = sPCByBC + $@"{rt.Module_Type_Id}|{rt.Generic_Category}|{rt.IMG_Descriptor}||||||||{rt.Operation}|" +
                                                $@"{ rt.Quantity}||||{rt.Dependent_On_IMG_Descriptor}||";
                                }

                                var SBByBC = SBList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                                foreach (var sb in SBByBC)
                                {
                                    sPCByBC = sPCByBC + $@"{sb.Module_Type_Id}|{sb.V_Product_Number}|{sb.Option_Localization}|{sb.Quantity}|{ sb.Description}|" +
                                                $@"{sb.Feature_Code}|{sb.Feature_Value}|{sb.Other_Options}|{sb.Type_Of_Operation}|{sb.Future}|";
                                }

                                sPCByBC = sPCByBC + sPCByBC.Length;
                                line.Add(sPCByBC);
                            }
                        }
                    }
                    //Only PC without BP
                    var strSql = $@"select *
                                      from R_SNR_PC a(nolock)
                                     where a.RId = '{h.RId}'
                                       and not exists (select 1
                                              from R_SNR_BP b(nolock)
                                             where b.RId = a.RId
                                               and b.Serial_Number = a.Serial_Number)
                                       and not exists (select 1
                                              from R_SNR_BC c(nolock)
                                             where c.RId = a.RId
                                               and c.Serial_Number = a.Serial_Number)";
                    var OnlyPCList = sfcdb.SqlQueryable<R_SNR_PC>(strSql).OrderBy(t => t.Serial_Number, OrderByType.Asc).ToList();
                    foreach (var PC in OnlyPCList)
                    {
                        string sOnlyPC = $@"{PC.Record_Type_Id}|{PC.Serial_Number}|{PC.Product_Number}|{PC.Record_Origin_Id}|{PC.Sub_FA_Origin_Id}|" +
                                     $@"{PC.Localization_Hardware_Option}|{PC.Warranty_Option}|{PC.Other_Options}|{PC.Date}|{PC.Asset_Tag}|" +
                                     $@"{PC.Future}|{PC.Test_Result}|{PC.Nb_Sub_Modules}|";
                        var HCByOnlyPC = HCList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                        foreach (var hc in HCByOnlyPC)
                        {
                            sOnlyPC = sOnlyPC + $@"{hc.Module_Type_Id}|{hc.Generic_Category}|{hc.HP_Component_Part_Number}|{hc.Supplier_Part_Number}|{ hc.Serial_Number}|" +
                                        $@"{hc.CT_Serial_Number_Or_Date_Code}|{hc.Hardware_Revision}|{hc.Firmware_Revision}|{hc.Supplier_Name}|{hc.E_T_Status}|" +
                                        $@"{hc.Type_Of_Operation}|{hc.Quantity}|{hc.Parent_Product}|{hc.Family}|{ hc.Part_Category_Or_Commodity_Code}|" +
                                        $@"{hc.Description}|{hc.Eatra_Info}|";
                        }

                        var VPByOnlyPC = VPList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number).OrderBy(t => t.Part_Category_Or_Commodity_Code);
                        foreach (var vp in VPByOnlyPC)
                        {
                            sOnlyPC = sOnlyPC + $@"{vp.Module_Type_Id}|{vp.Generic_Category}|{vp.HP_Component_Part_Number}|{vp.Supplier_Part_Number}|{ vp.Serial_Number}|" +
                                        $@"{vp.Date_Code}|{vp.Hardware_Revision}|{vp.Firmware_Revision}|{vp.Supplier_Name}|{vp.E_T_Status}|" +
                                        $@"{vp.Type_Of_Operation}|{vp.Quantity}|{vp.Parent_Product}|{vp.Family}|{ vp.Part_Category_Or_Commodity_Code}|" +
                                        $@"{vp.Description}|{vp.Eatra_Info}|";
                        }

                        var RTByOnlyPC = RTList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                        foreach (var rt in RTByOnlyPC)
                        {
                            sOnlyPC = sOnlyPC + $@"{rt.Module_Type_Id}|{rt.Generic_Category}|{rt.IMG_Descriptor}||||||||{rt.Operation}|" +
                                        $@"{ rt.Quantity}||||{rt.Dependent_On_IMG_Descriptor}||";
                        }

                        var SBByOnlyPC = SBList.FindAll(t => t.PC_Serial_Number == PC.Serial_Number);
                        foreach (var sb in SBByOnlyPC)
                        {
                            sOnlyPC = sOnlyPC + $@"{sb.Module_Type_Id}|{sb.V_Product_Number}|{sb.Option_Localization}|{sb.Quantity}|{ sb.Description}|" +
                                        $@"{sb.Feature_Code}|{sb.Feature_Value}|{sb.Other_Options}|{sb.Type_Of_Operation}|{sb.Future}|";
                        }

                        sOnlyPC = sOnlyPC + sOnlyPC.Length;
                        line.Add(sOnlyPC);
                    }
                    h.Trailer_Record = $@"T-PTR|{string.Format("{0:D8}", line.Count + 2)}|||17";
                    h.File_Local_Path = _filepath + "\\" + h.File_Name;
                    if (!Directory.Exists(_filepath))
                    {
                        Directory.CreateDirectory(_filepath);
                    }
                    if (File.Exists(h.File_Local_Path))
                    {
                        File.Delete(h.File_Local_Path);
                    }
                    WriteFile(h.File_Local_Path, h.Header_Record);
                    foreach (string s in line)
                    {
                        WriteFile(h.File_Local_Path, s);
                    }
                    WriteFile(h.File_Local_Path, h.Trailer_Record);

                    h.Conver = "Y";
                    h.Conver_Time = sfcdb.GetDate();
                    sfcdb.Updateable<R_SNR_HEAD>(h).Where(t => t.RId == h.RId).ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                MesLog.Info($@"ConvertData Err:{e.Message}");
                throw new Exception(e.Message);
            }
            finally
            {
                MesLog.Info("ConvertData End");
            }
        }

        public void SendSNRData()
        {
            try
            {
                SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword);
                var head = sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.Data_Type == _datatype && t.Key_Type == _keytype && t.Cancel == "N" && t.Get == "Y" && t.Conver == "Y" && t.Send == "N").OrderBy(t => t.Key_Value, OrderByType.Asc).ToList();
                foreach (var h in head)
                {
                    h.File_Remote_Path = _remotepath + "/" + h.File_Name;
                    sftpHelp.Put(h.File_Local_Path, h.File_Remote_Path);
                    h.Send = "Y";
                    h.Send_Time = sfcdb.GetDate();
                    sfcdb.Updateable<R_SNR_HEAD>(h).Where(t => t.RId == h.RId).ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                MesLog.Info($@"SendSNRData Err:{e.Message}");
                throw new Exception(e.Message);
            }
            finally
            {
                MesLog.Info("SendSNRData End");
            }
        }

        public string GetRId()
        {
            var RIdsysdate = sfcdb.GetDate();
            var RId = RIdsysdate.ToString("yyyyMMddHHmmssfff");
            if (sfcdb.Queryable<R_SNR_HEAD>().Where(t => t.RId == RId).Any())
            {
                return GetRId();
            }
            else
            {
                return RId;
            }
        }

        public void DeleteData(string RId)
        {
            string strSql = $@"delete R_SNR_BP where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_BC where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_RP where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_PC where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_HC where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_VP where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_RT where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);

            strSql = $@"delete R_SNR_SB where RId = '{RId}'";
            sfcdb.Ado.ExecuteCommand(strSql);
        }

        public PCDetail GetPCDetail(string type, string RId, string sn, string completedate, string shipdate, string po, string address, string skunoTemp, string parentSN)
        {
            PCDetail pcDetail = new PCDetail()
            {
                sn = sn,
                hcList = new List<R_SNR_HC>(),
                vpList = new List<R_SNR_VP>(),
                rtList = new List<R_SNR_RT>(),
                sbList = new List<R_SNR_SB>()
            };

            R_SNR_HC HC = new R_SNR_HC();
            R_SNR_VP VP = new R_SNR_VP();
            R_SNR_RT RT = new R_SNR_RT();
            R_SNR_SB SB = new R_SNR_SB();

            int macCount = 1;

            if (string.IsNullOrEmpty(completedate) || completedate.StartsWith("1900")) completedate = shipdate;
            completedate = completedate.Length > 8 ? completedate.Substring(0, 8) : completedate;
            shipdate = shipdate.Length > 8 ? shipdate.Substring(0, 8) : shipdate;

            #region Add MANUF_DATE
            if (type == "BC")
            {
                //MANUF_DATE
                VP = new R_SNR_VP();
                VP.RId = RId;
                VP.PC_Serial_Number = sn;
                VP.Module_Type_Id = "CM";
                VP.Generic_Category = "HW";
                VP.HP_Component_Part_Number = "VIRTUAL_PART";
                VP.Supplier_Part_Number = "MANUF_DATE";
                VP.Serial_Number = completedate;
                VP.Date_Code = "";
                VP.Hardware_Revision = "";
                VP.Firmware_Revision = "";
                VP.Supplier_Name = "";
                VP.E_T_Status = "";
                VP.Type_Of_Operation = "I";
                VP.Quantity = 1;
                VP.Parent_Product = "";
                VP.Family = "";
                VP.Part_Category_Or_Commodity_Code = "Base Product";
                VP.Description = "";
                VP.Eatra_Info = "";
                pcDetail.vpList.Add(VP);

                //OLD_SN
                VP = new R_SNR_VP();
                VP.RId = RId;
                VP.PC_Serial_Number = sn;
                VP.Module_Type_Id = "CM";
                VP.Generic_Category = "HW";
                VP.HP_Component_Part_Number = "VIRTUAL_PART";
                VP.Supplier_Part_Number = "OLD_SN";
                VP.Serial_Number = sn;
                VP.Date_Code = "";
                VP.Hardware_Revision = "";
                VP.Firmware_Revision = "";
                VP.Supplier_Name = "";
                VP.E_T_Status = "";
                VP.Type_Of_Operation = "I";
                VP.Quantity = 1;
                VP.Parent_Product = "";
                VP.Family = "";
                VP.Part_Category_Or_Commodity_Code = "OLD_SN";
                VP.Description = "old s/n prior to transformation";
                VP.Eatra_Info = "";
                pcDetail.vpList.Add(VP);

                //OLD_MN
                VP = new R_SNR_VP();
                VP.RId = RId;
                VP.PC_Serial_Number = sn;
                VP.Module_Type_Id = "CM";
                VP.Generic_Category = "HW";
                VP.HP_Component_Part_Number = "VIRTUAL_PART";
                VP.Supplier_Part_Number = "OLD_MN";
                VP.Serial_Number = skunoTemp;
                VP.Date_Code = "";
                VP.Hardware_Revision = "";
                VP.Firmware_Revision = "";
                VP.Supplier_Name = "";
                VP.E_T_Status = "";
                VP.Type_Of_Operation = "I";
                VP.Quantity = 1;
                VP.Parent_Product = "";
                VP.Family = "";
                VP.Part_Category_Or_Commodity_Code = "OLD_MN";
                VP.Description = "old s/n prior to transformation";
                VP.Eatra_Info = "";
                pcDetail.vpList.Add(VP);
            }
            #endregion

            #region Add PCID
            //if (type == "BC" && sn.Length > 6)
            if (sn.Length > 6)
            {
                VP = new R_SNR_VP();
                VP.RId = RId;
                VP.PC_Serial_Number = sn;
                VP.Module_Type_Id = "CM";
                VP.Generic_Category = "HW";
                VP.HP_Component_Part_Number = "VIRTUAL_PART";
                VP.Supplier_Part_Number = "PCID";
                VP.Serial_Number = sn.Substring(4, 3);
                VP.Date_Code = "";
                VP.Hardware_Revision = "";
                VP.Firmware_Revision = "";
                VP.Supplier_Name = "";
                VP.E_T_Status = "";
                VP.Type_Of_Operation = "I";
                VP.Quantity = 1;
                VP.Parent_Product = "";
                VP.Family = "";
                VP.Part_Category_Or_Commodity_Code = "PCID";
                VP.Description = "Product Configuration ID";
                VP.Eatra_Info = "";
                pcDetail.vpList.Add(VP);
            }
            #endregion

            #region Add PO/SHIP_TO_ADDRESS
            if (_station != "SHIPOUT")
            {
                po = "Test Po";
                shipdate = "20220222";
                address = "HEWLETT PACKARD ENTERPRISE";
                //po = "";
                //shipdate = "";
                //address = "";
            }
            #region Add PO_NR
            VP = new R_SNR_VP();
            VP.RId = RId;
            VP.PC_Serial_Number = sn;
            VP.Module_Type_Id = "CM";
            VP.Generic_Category = "HW";
            VP.HP_Component_Part_Number = "VIRTUAL_PART";
            VP.Supplier_Part_Number = "PO_NR";
            VP.Serial_Number = po;
            VP.Date_Code = "";
            VP.Hardware_Revision = "";
            VP.Firmware_Revision = "";
            VP.Supplier_Name = "";
            VP.E_T_Status = "";
            VP.Type_Of_Operation = "I";
            VP.Quantity = 1;
            VP.Parent_Product = "";
            VP.Family = "";
            VP.Part_Category_Or_Commodity_Code = "Shipping Record";
            VP.Description = "";
            VP.Eatra_Info = "";
            pcDetail.vpList.Add(VP);
            #endregion

            #region Add SHIP_DATE
            VP = new R_SNR_VP();
            VP.RId = RId;
            VP.PC_Serial_Number = sn;
            VP.Module_Type_Id = "CM";
            VP.Generic_Category = "HW";
            VP.HP_Component_Part_Number = "VIRTUAL_PART";
            VP.Supplier_Part_Number = "SHIP_DATE";
            VP.Serial_Number = shipdate;
            VP.Date_Code = "";
            VP.Hardware_Revision = "";
            VP.Firmware_Revision = "";
            VP.Supplier_Name = "";
            VP.E_T_Status = "";
            VP.Type_Of_Operation = "I";
            VP.Quantity = 1;
            VP.Parent_Product = "";
            VP.Family = "";
            VP.Part_Category_Or_Commodity_Code = "Shipping Record";
            VP.Description = "";
            VP.Eatra_Info = "";
            pcDetail.vpList.Add(VP);
            #endregion

            #region Add SHIP_TO_ADDRESS
            VP = new R_SNR_VP();
            VP.RId = RId;
            VP.PC_Serial_Number = sn;
            VP.Module_Type_Id = "CM";
            VP.Generic_Category = "HW";
            VP.HP_Component_Part_Number = "VIRTUAL_PART";
            VP.Supplier_Part_Number = "SHIP_TO_ADDRESS";
            VP.Serial_Number = address;
            VP.Date_Code = "";
            VP.Hardware_Revision = "";
            VP.Firmware_Revision = "";
            VP.Supplier_Name = "";
            VP.E_T_Status = "";
            VP.Type_Of_Operation = "I";
            VP.Quantity = 1;
            VP.Parent_Product = "";
            VP.Family = "";
            VP.Part_Category_Or_Commodity_Code = "Shipping Record";
            VP.Description = "";
            VP.Eatra_Info = "";
            pcDetail.vpList.Add(VP);
            #endregion

            #endregion

            var strSql = $@"select *
                              from mfsyscserial a(nolock)
                             where a.sysserialno <> a.cserialno
                               and substring(a.cserialno, 1, 1) not in ('#', '*', '~', '-')
                               and a.sysserialno = '{sn}'";
            if (type == "BP") strSql = strSql + $@" and not exists (select 1 from mfsysproduct b(nolock) where b.sysserialno = a.cserialno) and a.categoryname not in ('FAN S/N','PSU S/N')";
            DataTable dt = sfcdb.Ado.GetDataTable(strSql);
            foreach (DataRow r in dt.Rows)
            {
                var sfcCodeLikeExtend = GetSfcCodeLikeExtend(r["custpartno"].ToString());
                if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("PCBA"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PCBA";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("MB S/N"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PCBA";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("CONSOLE S/N"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PCBA";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("PWR S/N"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PCBA";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("PSU S/N") && type != "BC")
                {
                    var sfcCodeLikeExtend_PSU = GetSfcCodeLikeExtend(sfcCodeLikeExtend.VALUE4);
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend_PSU.VALUE2;
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PSU";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("PSU S/N") && type == "BC")
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = "";
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PSU";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("FAN S/N") && type != "BC")
                {
                    var sfcCodeLikeExtend_FAN = GetSfcCodeLikeExtend(sfcCodeLikeExtend.VALUE4);
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend_FAN.VALUE2;
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"FAN";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("FAN S/N") && type == "BC")
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = "";
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["sysserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"FAN";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("SSD S/N"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"SSD";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("DIMM S/N"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"DIMM";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("RAM"))
                {
                    HC = new R_SNR_HC();
                    HC.RId = RId;
                    HC.PC_Serial_Number = sn;
                    HC.Module_Type_Id = "CM";
                    HC.Generic_Category = "HW";
                    HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Supplier_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                    HC.Serial_Number = r["cserialno"].ToString();
                    HC.CT_Serial_Number_Or_Date_Code = "";
                    HC.Hardware_Revision = "";
                    HC.Firmware_Revision = "";
                    HC.Supplier_Name = "";
                    HC.E_T_Status = "";
                    HC.Type_Of_Operation = "I";
                    HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                    HC.Parent_Product = "";
                    HC.Family = "";
                    HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"RAM";
                    HC.Description = sfcCodeLikeExtend.VALUE1;
                    HC.Eatra_Info = "";
                    pcDetail.hcList.Add(HC);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("MAC S/N"))
                {
                    VP = new R_SNR_VP();
                    VP.RId = RId;
                    VP.PC_Serial_Number = sn;
                    VP.Module_Type_Id = "CM";
                    VP.Generic_Category = "HW";
                    VP.HP_Component_Part_Number = "VIRTUAL_PART";
                    VP.Supplier_Part_Number = "MAC_ADDR" + macCount.ToString();
                    VP.Serial_Number = r["cserialno"].ToString();
                    VP.Date_Code = "";
                    VP.Hardware_Revision = "";
                    VP.Firmware_Revision = "";
                    VP.Supplier_Name = "";
                    VP.E_T_Status = "";
                    VP.Type_Of_Operation = "I";
                    VP.Quantity = 1;
                    VP.Parent_Product = "";
                    VP.Family = "";
                    VP.Part_Category_Or_Commodity_Code = "MAC";
                    VP.Description = "Starting address of MAC block";
                    VP.Eatra_Info = "";
                    pcDetail.vpList.Add(VP);

                    VP = new R_SNR_VP();
                    VP.RId = RId;
                    VP.PC_Serial_Number = sn;
                    VP.Module_Type_Id = "CM";
                    VP.Generic_Category = "HW";
                    VP.HP_Component_Part_Number = "VIRTUAL_PART";
                    VP.Supplier_Part_Number = "MAC_ADDR_COUNT" + macCount.ToString();
                    VP.Serial_Number = "128";
                    VP.Date_Code = "";
                    VP.Hardware_Revision = "";
                    VP.Firmware_Revision = "";
                    VP.Supplier_Name = "";
                    VP.E_T_Status = "";
                    VP.Type_Of_Operation = "I";
                    VP.Quantity = 1;
                    VP.Parent_Product = "";
                    VP.Family = "";
                    VP.Part_Category_Or_Commodity_Code = "MAC";
                    VP.Description = "Starting address of MAC block";
                    VP.Eatra_Info = "";
                    pcDetail.vpList.Add(VP);
                    macCount++;
                }
                else if (r["categoryname"].ToString().ToUpper().Trim() == "OS P/N")
                {
                    VP = new R_SNR_VP();
                    VP.RId = RId;
                    VP.PC_Serial_Number = sn;
                    VP.Module_Type_Id = "CM";
                    VP.Generic_Category = "HW";
                    VP.HP_Component_Part_Number = "VIRTUAL_PART";
                    VP.Supplier_Part_Number = "OS_SYSTEM";
                    VP.Serial_Number = r["cserialno"].ToString();
                    VP.Date_Code = "";
                    VP.Hardware_Revision = "";
                    VP.Firmware_Revision = "";
                    VP.Supplier_Name = "";
                    VP.E_T_Status = "";
                    VP.Type_Of_Operation = "I";
                    VP.Quantity = 1;
                    VP.Parent_Product = "";
                    VP.Family = "";
                    VP.Part_Category_Or_Commodity_Code = "OS";
                    VP.Description = "Version of OS code";
                    VP.Eatra_Info = "";
                    pcDetail.vpList.Add(VP);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim() == "PCID")
                {
                    VP = new R_SNR_VP();
                    VP.RId = RId;
                    VP.PC_Serial_Number = sn;
                    VP.Module_Type_Id = "CM";
                    VP.Generic_Category = "HW";
                    VP.HP_Component_Part_Number = "VIRTUAL_PART";
                    VP.Supplier_Part_Number = "PCID";
                    VP.Serial_Number = r["cserialno"].ToString();
                    VP.Date_Code = "";
                    VP.Hardware_Revision = "";
                    VP.Firmware_Revision = "";
                    VP.Supplier_Name = "";
                    VP.E_T_Status = "";
                    VP.Type_Of_Operation = "I";
                    VP.Quantity = 1;
                    VP.Parent_Product = "";
                    VP.Family = "";
                    VP.Part_Category_Or_Commodity_Code = "PCID";
                    VP.Description = "Product Configuration ID";
                    VP.Eatra_Info = "";
                    pcDetail.vpList.Add(VP);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim() == "IMG")
                {
                    RT = new R_SNR_RT();
                    RT.RId = RId;
                    RT.PC_Serial_Number = sn;
                    RT.Module_Type_Id = "CM";
                    RT.Generic_Category = "IMG";
                    RT.IMG_Descriptor = r["cserialno"].ToString();
                    RT.Operation = "I";
                    RT.Quantity = 1;
                    RT.Dependent_On_IMG_Descriptor = "";
                    pcDetail.rtList.Add(RT);
                }
                else if (r["categoryname"].ToString().ToUpper().Trim() == "SB")
                {
                    SB = new R_SNR_SB();
                    SB.RId = RId;
                    SB.PC_Serial_Number = sn;
                    SB.Module_Type_Id = "SB";
                    SB.V_Product_Number = r["cserialno"].ToString(); ;
                    SB.Option_Localization = "";
                    SB.Quantity = 1;
                    SB.Description = "";
                    SB.Feature_Code = "";
                    SB.Feature_Value = "";
                    SB.Other_Options = "";
                    SB.Type_Of_Operation = "I";
                    SB.Future = "";
                    pcDetail.sbList.Add(SB);
                }
            }
            if (dt.Rows.Count == 0 && type == "BC")
            {
                strSql = $@"select *
                              from mfsyscserial a(nolock)
                             where a.cserialno = '{sn}'
                               and a.sysserialno = '{parentSN}'
                               and a.categoryname in ('FAN S/N','PSU S/N')";
                dt = sfcdb.Ado.GetDataTable(strSql);
                foreach (DataRow r in dt.Rows)
                {
                    var sfcCodeLikeExtend = GetSfcCodeLikeExtend(r["custpartno"].ToString());
                    var sfcCodeLikeExtend_Extend = GetSfcCodeLikeExtend(sfcCodeLikeExtend.VALUE4);
                    if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("FAN S/N"))
                    {
                        HC = new R_SNR_HC();
                        HC.RId = RId;
                        HC.PC_Serial_Number = sn;
                        HC.Module_Type_Id = "CM";
                        HC.Generic_Category = "HW";
                        HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                        HC.Supplier_Part_Number = sfcCodeLikeExtend_Extend.VALUE2;
                        HC.Serial_Number = r["cserialno"].ToString();
                        HC.CT_Serial_Number_Or_Date_Code = "";
                        HC.Hardware_Revision = "";
                        HC.Firmware_Revision = "";
                        HC.Supplier_Name = "";
                        HC.E_T_Status = "";
                        HC.Type_Of_Operation = "I";
                        HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                        HC.Parent_Product = "";
                        HC.Family = "";
                        HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3;//"FAN";
                        HC.Description = sfcCodeLikeExtend.VALUE1;
                        HC.Eatra_Info = "";
                        pcDetail.hcList.Add(HC);
                    }
                    else if (r["categoryname"].ToString().ToUpper().Trim().StartsWith("PSU S/N"))
                    {
                        HC = new R_SNR_HC();
                        HC.RId = RId;
                        HC.PC_Serial_Number = sn;
                        HC.Module_Type_Id = "CM";
                        HC.Generic_Category = "HW";
                        HC.HP_Component_Part_Number = sfcCodeLikeExtend.VALUE2; //r["custpartno"].ToString();
                        HC.Supplier_Part_Number = sfcCodeLikeExtend_Extend.VALUE2;
                        HC.Serial_Number = r["cserialno"].ToString();
                        HC.CT_Serial_Number_Or_Date_Code = "";
                        HC.Hardware_Revision = "";
                        HC.Firmware_Revision = "";
                        HC.Supplier_Name = "";
                        HC.E_T_Status = "";
                        HC.Type_Of_Operation = "I";
                        HC.Quantity = dt.Select($@"custpartno = '{r["custpartno"].ToString().Trim()}'").Count();
                        HC.Parent_Product = "";
                        HC.Family = "";
                        HC.Part_Category_Or_Commodity_Code = sfcCodeLikeExtend.VALUE3; //"PSU";
                        HC.Description = sfcCodeLikeExtend.VALUE1;
                        HC.Eatra_Info = "";
                        pcDetail.hcList.Add(HC);
                    }
                }
            }
            return pcDetail;
        }

        public SFCCODELIKE_EXTEND GetSfcCodeLikeExtend(string skuno)
        {
            var strSql = $@"select SKUNO,CTYPE,VALUE1,VALUE2,VALUE3,VALUE4,VALUE5,INPUT_EMP,INPUT_DATE from SFCCODELIKE_EXTEND where CTYPE = 'ARUBA_SNR' and skuno = '{skuno}'";
            var sfcCodeLikeExtend = sfcdb.SqlQueryable<SFCCODELIKE_EXTEND>(strSql).OrderBy(t => t.INPUT_DATE, OrderByType.Desc).ToList().FirstOrDefault();
            if (sfcCodeLikeExtend == null)
            {
                return new SFCCODELIKE_EXTEND()
                {
                    SKUNO = "",
                    CTYPE = "",
                    VALUE1 = "",
                    VALUE2 = skuno,
                    VALUE3 = "",
                    VALUE4 = "",
                    VALUE5 = "",
                    INPUT_EMP = "",
                    INPUT_DATE = DateTime.Now
                };
            }
            else
            {
                if (sfcCodeLikeExtend.VALUE2 == "" || sfcCodeLikeExtend.VALUE2 == null) sfcCodeLikeExtend.VALUE2 = skuno;
                return sfcCodeLikeExtend;
            }
        }

        private void WriteFile(string fileFullName, string msg)
        {
            string Path = fileFullName;
            FileStream fs = new FileStream(Path, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
            //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Current);
            if (msg == "")
            {
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine(msg);
            }
            sw.Flush();
            sw.Close();
        }

        public class PCDetail
        {
            public string sn;
            public List<R_SNR_HC> hcList;
            public List<R_SNR_VP> vpList;
            public List<R_SNR_RT> rtList;
            public List<R_SNR_SB> sbList;
        }
    }
}
