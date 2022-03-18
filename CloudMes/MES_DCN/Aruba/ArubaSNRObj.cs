using MESDataObject;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.ModuleHelp;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESPubLab.MesException;
using static MESDataObject.Common.EnumExtensions;
using Renci.SshNet;
using System.Data;
using System.Globalization;

namespace MES_DCN.Aruba
{
    public class ArubaSNRObj
    {
        private string _mesdbstr, _apdbstr, _bustr, _filepath, _filebackpath, _remotepath, _seriesname;
        private SqlSugarClient sfcdb;
        private SqlSugarClient apdb;
        private string _datetype;

        //static string Aruba SNR SFTP;
        #region B2B sftp
        //IP: 10.132.48.74:8022
        //ID: hpe
        //PWD: 0s02QtFZ
        //文件存放路徑：
        ///TEData/PROD 正式環境
        ///TEData/TEST 測試環境
        string CONST_SFTPHost = "10.132.48.74";
        string CONST_SFTPPort = "8022";
        string CONST_SFTPLogin = "hpe";
        string CONST_SFTPPassword = "0s02QtFZ";
        string CONST_SFTPPath = "TEData\\PROD";

        #endregion

        public ArubaSNRObj(string mesdbstr, string apdbstr, string bustr, string filepath, string filebackpath, string remotepath, string seriesname, string keytype)
        {
            _mesdbstr = mesdbstr;
            _apdbstr = apdbstr;
            _bustr = bustr;
            _filepath = filepath;
            _filebackpath = filebackpath;
            _remotepath = remotepath;
            _seriesname = seriesname;
            _datetype = keytype;
            sfcdb = MESDBHelper.OleExec.GetSqlSugarClient(this._mesdbstr, false);
            apdb = MESDBHelper.OleExec.GetSqlSugarClient(this._apdbstr, false);
        }

        public void Build()
        {
            try
            {
                sfcdb.Ado.Open();
                apdb.Ado.Open();
                this.BuildHead();
                this.GetSNRData();
                this.SendSNRData();
            }
            catch (Exception e)
            {
                MesLog.Info($@"Build Err:{e.Message}");
            }
            finally
            {
                sfcdb.Close();
                apdb.Close();
            }
        }

        void BuildHead()
        {
            try
            {
                var sysdate = sfcdb.GetDate().AddDays(-1); //取數據庫時間好點
                sysdate = sysdate.AddDays(-(int)sysdate.DayOfWeek);

                var dokey = DateTime.Parse(sysdate.AddDays(-30).ToString("yyyy/MM/dd 00:00:00"));

                var _seq = sfcdb.Ado.SqlQuery<string>(@"SELECT max(seq) FROM SFCRUNTIME.R_ARUBADATA_HEAD WHERE DATETYPE = 'WEEKLY' ").Single();
                int seq = int.Parse(_seq) + 1;
                #region by weekly
                //_datetype = "WEEKLY";
                while (dokey < sysdate.AddDays(-1))
                {
                    var existsDoKey = sfcdb.Ado.SqlQuery<R_ARUBADATA_HEAD>(@"SELECT *
                                                                  FROM R_ARUBADATA_HEAD
                                                                 WHERE DATATYPE = 'SNR'
                                                                   AND DATETYPE = 'WEEKLY'
                                                                   AND @dokey BETWEEN STARTTIME AND ENDTIME", new { dokey }).ToList();
                    if (existsDoKey.Count == 0)
                    {
                        var currentkey = dokey.ToString("yyyyMMdd");
                        R_ARUBADATA_HEAD rah = new R_ARUBADATA_HEAD()
                        {
                            ID = MesDbBase.GetNewID<R_ARUBADATA_HEAD>(sfcdb, _bustr),
                            DATEKEY = currentkey,
                            DATETYPE = _datetype,//lastHead.DATETYPE,
                            STARTTIME = DateTime.Parse(dokey.AddDays(-(int)dokey.DayOfWeek + 1).ToString("yyyy/MM/dd 00:00:00")),
                            ENDTIME = DateTime.Parse(dokey.AddDays(-(int)dokey.DayOfWeek + 7).ToString("yyyy/MM/dd 23:59:59")),
                            SEQ = string.Format("{0:D6}", seq),// string.Format("{0:D6}", dokey.DayOfYear + 1),
                            DATATYPE = "SNR",
                            GET = "0",
                            CONVERT = "0",
                            SEND = "0",
                            CREATETIME = sfcdb.GetDate()
                    };
                        sfcdb.Insertable<R_ARUBADATA_HEAD>(rah).ExecuteCommand();
                        seq += 1;
                    }
                    dokey = dokey.AddDays(1);
                }
                #endregion

                #region by DAILY
                ////_datetype = "DAILY";
                //while (dokey < DateTime.Parse(sysdate.ToString("yyyy/MM/dd 00:00:00")))
                //{
                //    var currentkey = dokey.ToString("yyyyMMdd");
                //    var existsdo = sfcdb.Queryable<R_ARUBADATA_HEAD>().Where(t => t.DATATYPE == "SNR" && t.DATETYPE == _datetype && t.DATEKEY == currentkey).ToList();
                //    if (existsdo.Count == 0)
                //    {
                //        R_ARUBADATA_HEAD rah = new R_ARUBADATA_HEAD()
                //        {
                //            ID = MesDbBase.GetNewID<R_ARUBADATA_HEAD>(sfcdb, _bustr),
                //            DATEKEY = currentkey,
                //            DATETYPE = "DAILY",
                //            STARTTIME = dokey,
                //            ENDTIME = DateTime.Parse(dokey.ToString("yyyy/MM/dd 23:59:59")),
                //            SEQ = string.Format("{0:D6}", seq),// string.Format("{0:D6}", dokey.DayOfYear + 1),
                //            DATATYPE = "SNR",
                //            GET = "0",
                //            CONVERT = "0",
                //            SEND = "0",
                //            CREATETIME = sysdate
                //        };
                //        sfcdb.Insertable<R_ARUBADATA_HEAD>(rah).ExecuteCommand();
                //        seq += 1;
                //    }
                //    dokey = dokey.AddDays(1);
                //}
                #endregion
            }
            catch (Exception e)
            {
                MesLog.Info($@"BuildHead Err:{e.Message}");
            }
            finally
            {
                MesLog.Info("BuildHead End");
            }
        }

        public void GetSNRData()
        {
            var sysdate = sfcdb.GetDate();
            var headlist = sfcdb.Queryable<R_ARUBADATA_HEAD>().Where(t => t.DATATYPE == "SNR" && t.DATETYPE == _datetype && t.GET == "0").OrderBy(t => t.DATEKEY, OrderByType.Asc).ToList();
            foreach (var r in headlist)
            {
                //var bodystr = "";
                R_ARUBADATA_HEAD rShipped = new R_ARUBADATA_HEAD();

                r.FILENAME = "HPE.WWSNR.FOXVIETNAM.VN.FVN1.I5." + r.SEQ + "_" + r.DATEKEY + "_PTR50.dat";
                if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME))
                {
                    File.Delete(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME);
                }
                r.HEADERRECORD = "H-PTR|PLI5|FVN1|" + r.SEQ + "|" + sysdate.ToString("yyyyMMdd") + "|" + sysdate.ToString("HHmmss") + "|" + _datetype.Substring(0, 1) + "|" + r.DATEKEY + "|PTR5.0|57";
                WriteFile(r.FILENAME, r.HEADERRECORD);

                rShipped.FILENAME = "HPE.WWSNR.FOXVIETNAM.VN.FVN1.I5." + (int.Parse(r.SEQ) + 1).ToString("000000") + "_" + r.DATEKEY + "_PTR50.dat";
                if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + rShipped.FILENAME))
                {
                    File.Delete(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + rShipped.FILENAME);
                }
                rShipped.HEADERRECORD = "H-PTR|PLI5|FVN1|" + (int.Parse(r.SEQ) + 1).ToString("000000") + "|" + sysdate.ToString("yyyyMMdd") + "|" + sysdate.ToString("HHmmss") + "|" + _datetype.Substring(0, 1) + "|" + r.DATEKEY + "|PTR5.0|57";
                WriteFile(rShipped.FILENAME, rShipped.HEADERRECORD);

                var rAsnList = new List<SNR>();
                var rAsn = new List<R_ARUBADATA_SN>();
                var rAsnSub = new List<R_ARUBADATA_SN_SUB>();
                rAsnList = GetRASN(r.DATEKEY);
                foreach (var l in rAsnList)
                {
                    if(string.IsNullOrEmpty(l.parentSN))
                    {
                        l.rAsn.NBSUBMODULES = l.rAsnSubList.Count;
                        var bodys = Bodys(l);
                        l.rAsn.TOTALLENGTH = bodys.Length + 1;
                        rAsn.Add(l.rAsn);
                        rAsnSub.AddRange(l.rAsnSubList);
                        WriteFile(r.FILENAME, bodys + "|" + (bodys.Length + 1));
                    }
                    #region When Ship
                    if (!string.IsNullOrEmpty(l.parentSN))
                    {
                        l.rAsn.NBSUBMODULES = l.rAsnSubList.Count;
                        var bodys = Bodys(l);
                        l.rAsn.TOTALLENGTH = bodys.Length + 1;
                        rAsn.Add(l.rAsn);
                        rAsnSub.AddRange(l.rAsnSubList);
                        WriteFile(rShipped.FILENAME, bodys + "|" + (bodys.Length + 1));
                    }
                    #endregion
                }
                r.TRAILERRECORD = "T-PTR|" + string.Format("{0:D8}", rAsnList.Where(x => string.IsNullOrEmpty(x.parentSN)).Count() + 2) + "|||17";
                WriteFile(r.FILENAME, r.TRAILERRECORD);

                #region When Ship
                rShipped.TRAILERRECORD = "T-PTR|" + string.Format("{0:D8}", rAsnList.Where(x => !string.IsNullOrEmpty(x.parentSN)).Count() + 2) + "|||17";
                WriteFile(rShipped.FILENAME, rShipped.TRAILERRECORD);

                #endregion

                #region delete file when not content
                if (rAsnList != null && rAsnList.Count <= 0)
                {
                    if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME))
                    {
                        File.Delete(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME);
                    }
                    if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + rShipped.FILENAME))
                    {
                        File.Delete(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + rShipped.FILENAME);
                    }
                }
                #endregion

                var resbuild = sfcdb.Ado.UseTran(() =>
                {
                    sfcdb.Insertable(rAsn).ExecuteCommand();
                    sfcdb.Insertable(rAsnSub).ExecuteCommand();
                    r.GET = "1";
                    r.CONVERT = "1";
                    r.EDITTIME = sfcdb.GetDate();
                    sfcdb.Updateable(r).ExecuteCommand();
                });
                if (!resbuild.IsSuccess) throw new Exception(resbuild.ErrorMessage);
            }
        }

        public string Bodys(SNR l)
        {
            var bodys = "";
            bodys = l.rAsn.RECORDTYPE + "|" + l.rAsn.SN + "|" + l.rAsn.PN + "|" + l.rAsn.RECORDORIGIN + "|" + l.rAsn.SUBFAORIGIN + "|" + l.rAsn.LOCALIZATION + "|" +
                l.rAsn.WARRANTY + "|" + l.rAsn.OTHER + "|" + l.rAsn.SHIPDATE?.ToString("yyyyMMddHHmmss") + "|" + l.rAsn.ASSETTAG + "|" + l.rAsn.FUTURE + "|" +
                l.rAsn.TESTRESULT + "|" + l.rAsn.NBSUBMODULES;
            foreach (var r in l.rAsnSubList)
            {
                bodys = bodys + "|" + r.MODULETYPE + "|" + r.GENERICCATEGORY + "|" + r.CPN + "|" + r.SPN + "|" + r.SN + "|" + r.CT_DC + "|" + r.HREVISION + "|" + r.FREVISION + "|" +
                    r.SUPPLIERNAME + "|" + r.ETSTATUS + "|" + r.OPERATION + "|" + r.QUANTITY + "|" + r.PARENTPRODUCT + "|" + r.FAMILY + "|" + r.PARTCATEGORY + "|" + r.DESCRIPTION.Replace("|", "") + "|" +
                    r.EATRAINFO;
            }
            return bodys;
        }

        public List<SNR> GetRASN(string dokey)
        {
            var snR = new List<SNR>();
            var headlist = sfcdb.Queryable<R_ARUBADATA_HEAD>().Where(t => t.DATATYPE == "SNR" && t.DATETYPE == _datetype && t.GET == "0" && t.DATEKEY == dokey).ToList();
            foreach (var r in headlist)
            {
                var sql = $@"select sn, vssn, vsku, wsn, skuno, to_char(edit_time,'yyyy/mm/dd hh24:mi:ss') as edit_time
                                from (select a.sn, d.vssn, d.vsku, d.WSN, a.SKUNO,
                                            a.edit_time,
                                            row_number() over(partition by a.sn order by a.edit_time desc) r
                                        from r_sn_station_detail a
                                        LEFT JOIN r_sn b
                                        on a.r_sn_id = b.id and a.sn = b.sn
                                        LEFT JOIN SFCRUNTIME.WWN_DATASHARING d
                                        ON (a.sn = d.CSSN OR a.sn = d.VSSN) AND d.MAC IS NOT NULL
                                        where exists
                                        (select 1
                                                from c_sku_detail c
                                                where c.skuno = b.skuno
                                                and c.category = 'ARUBA_SNR')
                                        and b.valid_flag = '1' AND a.sn = d.VSSN
                                        and a.station_name = 'SHIPOUT'
                                        and a.edit_time between to_date('{r.STARTTIME?.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')
                                        and to_date('{r.ENDTIME?.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) aa
                                where aa.r = 1";
                DataTable snlist = sfcdb.Ado.GetDataTable(sql);
                foreach (DataRow l in snlist.Rows)
                {
                    var tempSn = l["sn"].ToString();
                    var vsku = l["vsku"].ToString();
                    var wsn = l["wsn"].ToString();
                    var skuno = l["skuno"].ToString();
                    var editTime = l["edit_time"].ToString();
                    var sns = tempSn.Split(' ');
                    SNR s = new SNR();
                    s.sn = l["sn"].ToString();
                    s.rAsn = new R_ARUBADATA_SN()
                    {
                        ID = MesDbBase.GetNewID<R_ARUBADATA_SN>(sfcdb, _bustr),
                        HEADID = r.ID,
                        RECORDTYPE = "PC",
                        SN = sns[0],
                        //PN = sns.Length > 1 ? sns[sns.Length - 1] : "",
                        PN = skuno,
                        RECORDORIGIN = "2",
                        SUBFAORIGIN = "FVN101",
                        SHIPDATE = DateTime.Parse(editTime),
                        TESTRESULT = "Y",
                        SNPN = tempSn,
                        CREATETIME = sfcdb.GetDate()
                    };
                    s.rAsnSubList = GetRASNSub(tempSn, vsku, wsn, s.rAsn.ID, DateTime.Parse(editTime));

                    //When Shipped
                    SNR _snShipped = new SNR();
                    _snShipped.sn = l["sn"].ToString();
                    _snShipped.parentSN = s.rAsn.ID;
                    _snShipped.rAsn = new R_ARUBADATA_SN()
                    {
                        ID = MesDbBase.GetNewID<R_ARUBADATA_SN>(sfcdb, _bustr),
                        HEADID = r.ID,
                        RECORDTYPE = "PC",
                        SN = sns[0],
                        //PN = sns.Length > 1 ? sns[sns.Length - 1] : "",
                        PN = skuno,
                        RECORDORIGIN = "2",
                        SUBFAORIGIN = "FVN101",
                        SHIPDATE = DateTime.Parse(editTime),
                        TESTRESULT = "Y",
                        SNPN = tempSn,
                        CREATETIME = sfcdb.GetDate(),
                        PARENTID = s.rAsn.ID,
                        PARENTSN = s.rAsn.SN
                    };
                    _snShipped.rAsnSubList = GetRASNShippedSub(tempSn, _snShipped.rAsn.ID, DateTime.Parse(editTime));

                    //if config in C_SKU_DETAIL and R_WHEN_SHIPPED then create and send file
                    if (s.rAsnSubList != null && _snShipped.rAsnSubList != null)
                    {
                        snR.Add(s);
                        snR.Add(_snShipped);
                    }
                }
            }
            return snR;
        }

        public List<R_ARUBADATA_SN_SUB> GetRASNSub(string sn, string vsku, string wsn, string snid, DateTime dtime)
        {
            string sql = "";
            int i = 1;
            var rASnSub = new List<R_ARUBADATA_SN_SUB>();
            var cSkuDetail = sfcdb.Queryable<R_SN, C_SKU_DETAIL>((a, b) => a.SKUNO == b.SKUNO).Where((a, b) => a.VALID_FLAG == "1" && b.CATEGORY == "ARUBA_SNR" && a.SN == sn)
                .OrderBy((a, b) => b.STATION_NAME, OrderByType.Asc).Select((a, b) => b).ToList();

            string cpnTemp, spnTemp, serialNumber, partCategory, description, supplierName, cust_kp_no;

            if(cSkuDetail.Count <= 0)
            {
                return null;
            }
            
            foreach (var r in cSkuDetail)
            {
                cpnTemp = spnTemp = serialNumber = partCategory = description = supplierName = cust_kp_no = "";
                if (r.EXTEND == "RAM" || r.EXTEND == "NAND")
                {
                    //cpnTemp = spnTemp = r.VALUE.Replace(".", "");
                    #region
                    //sql = $@"select mfr_name
                    //      from mes4.r_tr_product_detail a
                    //     INNER join mes4.r_tr_code_detail b ON a.tr_code = b.tr_code AND a.SMT_CODE = b.SMT_CODE
                    //     INNER JOIN mes4.R_KITTING_SCAN_DETAIL d ON b.TR_SN = d.TR_SN AND b.tr_sn = d.TR_SN
                    //     INNER join mes1.c_mfr_config c ON b.mfr_code = c.mfr_code
                    //     where
                    //     a.p_sn = '{wsn}' 
                    //    AND (d.CUST_KP_NO = '{r.VALUE}'
                    //     OR  d.CUST_KP_NO in(SELECT memo FROM mes1.C_ICBODY_CONFIG WHERE CUST_KP_NO = '{r.VALUE}') )
                    //     order by b.work_time DESC";
                    #endregion
                    sql = $@"SELECT b.tr_sn, b.cust_kp_no, e.mfr_code, b.qty, b.to_location, e.MFR_NAME
                                FROM
	                                mes4.r_kitting_scan_detail b,
	                                mes4.r_tr_code_detail c,
	                                mes4.r_tr_product_detail d,
	                                mes1.c_mfr_config e
                                WHERE b.TR_SN = c.TR_SN
	                                AND d.tr_code = c.tr_code AND d.SMT_CODE = c.SMT_CODE
	                                AND c.mfr_code = e.mfr_code
	                                --AND b.to_location IN (SELECT t_wo FROM mes4.r_v_wo WHERE v_wo IN ( SELECT v_wo FROM mes4.r_v_wo WHERE  T_WO = d.WO) )
	                                AND B.CUST_KP_NO in('{r.VALUE.Replace(",", "','")}')
	                                AND d.P_SN IN('{wsn}')";
                    var allpartDesc = apdb.Ado.GetDataTable(sql);
                    if (allpartDesc.Rows.Count > 0)
                    {
                        //run auto scan
                        supplierName = allpartDesc.Rows[0]["mfr_name"].ToString();
                        cust_kp_no = allpartDesc.Rows[0]["cust_kp_no"].ToString();
                        cpnTemp = spnTemp = cust_kp_no.Replace(".", "");
                    }
                    //else
                    //{
                    //    //not run auto scan
                    //    sql = $@"SELECT mfr_name FROM mes4.R_KP_LIST a, mes1.C_MFR_CONFIG b 
                    //            WHERE a.MFR_CODE=b.MFR_NO and wo IN (SELECT wo FROM mes4.r_tr_product_detail WHERE p_sn like '{wsn}') AND KP_NO  IN ('{r.VALUE}') ";
                    //    allpartDesc = apdb.Ado.GetDataTable(sql);
                    //    if (allpartDesc.Rows.Count > 0)
                    //    {
                    //        supplierName = allpartDesc.Rows[0]["mfr_name"].ToString();
                    //    }
                    //}
                    if (string.IsNullOrEmpty(supplierName)) continue;

                    partCategory = "RAM";
                    sql = $@"select * from mes1.c_cust_kp_config where cust_kp_no = '{cust_kp_no}' order by edit_time desc";
                    allpartDesc = apdb.Ado.GetDataTable(sql);
                    if (allpartDesc.Rows.Count > 0)
                    {
                        description = allpartDesc.Rows[0]["CUST_KP_DESC"].ToString();
                    }
                }
                else if (r.EXTEND == "FCT_TST")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var fctTime = sfcdb.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == sn && t.EVENTNAME == "FCA" && t.TESTDATE < dtime).OrderBy(t => t.TESTDATE, OrderByType.Desc).ToList().FirstOrDefault();
                    if (fctTime != null)
                    {
                        serialNumber = fctTime.TESTDATE?.ToString("yyyyMMddHHmmss");
                    }
                    partCategory = "TST";
                    description = r.BASETEMPLATE != null? r.BASETEMPLATE : "FCT Test finish time and date";
                }
                else if (r.EXTEND == "OP_SYSTEM")
                {
                    var rfData = sfcdb.Queryable<R_F_CONTROL>().Where(a => a.VALUE == vsku && a.FUNCTIONNAME == "TEST_CHECK_FW" && a.CATEGORY == "CHECK_FW" && a.CONTROLFLAG == "Y").OrderBy(a => a.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (rfData != null)
                    {
                        serialNumber = rfData.EXTVAL; // from TE Version of OS code
                    }

                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    partCategory = "OS";
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Version of OS code";
                }
                else if (r.EXTEND == "CPLD_SYSTEM")
                {
                    var rfData = sfcdb.Queryable<R_F_CONTROL>().Where(a => a.VALUE == r.SKUNO && a.FUNCTIONNAME == "TEST_CHECK_CPLD" && a.CATEGORY == "CHECK_CPLD" && a.CONTROLFLAG == "Y").OrderBy(a => a.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (rfData != null)
                    {
                        //serialNumber = "0x22"; // from TE Version of CPLD Code
                        serialNumber = rfData.EXTVAL;
                    }

                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    partCategory = "CPLD";
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Version of CPLD Code";
                }
                else if (r.EXTEND == "MAC_ADDR_COUNT1")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var skuno = sfcdb.Queryable<C_SKU>().Where(t => t.SKUNO == r.SKUNO).ToList().FirstOrDefault();
                    if (skuno.SKU_NAME.Trim().ToUpper() == "SCOOTER" || skuno.SKU_NAME.Trim().ToUpper() == "CHAMP")
                        serialNumber = "09"; // from TE Number of MAC addresses in block
                    else if (skuno.SKU_NAME.Trim().ToUpper() == "BUDDY" || skuno.SKU_NAME.Trim().ToUpper() == "DUKE")
                        serialNumber = "27"; // from TE Number of MAC addresses in block
                    else if (skuno.SKU_NAME.Trim().ToUpper() == "ROCKY" || skuno.SKU_NAME.Trim().ToUpper() == "BUSTER")
                        serialNumber = "53"; // from TE Number of MAC addresses in block
                    else
                        serialNumber = "00";
                    partCategory = "MAC";
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Number of MAC addresses in block";
                }
                else if (r.EXTEND == "MBD")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.VALUE;
                    //var rSnKp = sfcdb.Queryable<R_SN_KP>().Where(t => t.VALID_FLAG == 1 && t.SN == sn && t.PARTNO == r.VALUE).OrderBy(t => t.EDIT_TIME, OrderByType.Asc).ToList().FirstOrDefault();
                    //if (rSnKp != null)
                    //{
                    //    serialNumber = rSnKp.SN.Substring(4, 3);//rSnKp.VALUE;
                    //    var cSkuno = sfcdb.Queryable<C_SKU>().Where(t => t.SKUNO == r.VALUE).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
                    //    if (cSkuno != null)
                    //    {
                    //        description = cSkuno.DESCRIPTION;
                    //    }
                    //}
                    serialNumber = "";// sn.Substring(4, 3);
                    sql = $@"SELECT * FROM MES1.C_CUST_KP_CONFIG WHERE CUST_KP_NO = '{r.VALUE}' AND CUST_CODE = 'HPE ARUBA'";
                    var allpartDesc = apdb.Ado.GetDataTable(sql);
                    if (allpartDesc.Rows.Count > 0)
                    {
                        description = allpartDesc.Rows[0]["cust_kp_desc"].ToString();
                    }
                    partCategory = "MBD";
                }
                else if (r.EXTEND == "MANUF_DATE")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var rSnDetail = sfcdb.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.STATION_NAME == "CBS" && t.EDIT_TIME < dtime).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (rSnDetail != null)
                    {
                        serialNumber = rSnDetail.EDIT_TIME?.ToString("yyyyMMdd");
                    }
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Date of manufacture";
                }
                else if (r.EXTEND == "MAC_ADDR1")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var wwnData = sfcdb.Queryable<WWN_DATASHARING>().Where(t => t.CSSN == sn && t.MAC != null).OrderBy(t => t.LASTEDITDT, OrderByType.Desc).ToList().FirstOrDefault();
                    if (wwnData != null)
                    {
                        serialNumber = wwnData.MAC;
                    }
                    partCategory = "MAC";
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Starting address of wired MAC block";
                }
                else if(r.EXTEND == "SHIP_DATE" || r.EXTEND == "PO_NO" || r.EXTEND == "SHIP_TO_ADDRESS")
                {
                    continue;
                    //cpnTemp = r.VALUE;
                    //spnTemp = r.EXTEND;
                }

                rASnSub.Add(new R_ARUBADATA_SN_SUB()
                {
                    ID = MesDbBase.GetNewID<R_ARUBADATA_SN_SUB>(sfcdb, _bustr),
                    SNID = snid,
                    SEQ = i,
                    MODULETYPE = "CM",
                    GENERICCATEGORY = "HW",
                    CPN = cpnTemp,
                    SPN = spnTemp,
                    SN = serialNumber,
                    OPERATION = "I",
                    QUANTITY = 1,
                    PARTCATEGORY = partCategory,
                    DESCRIPTION = description,
                    SUPPLIERNAME = supplierName,
                    CREATETIME = sfcdb.GetDate()
                });
                i = i + 1;
            }
            return rASnSub;
        }


        private List<R_ARUBADATA_SN_SUB> GetRASNShippedSub(string sn, string snid, DateTime dtime)
        {
            var sqlcheck = $@"SELECT * FROM SFCRUNTIME.R_WHEN_SHIPPED WHERE DN_NO IN (select DN_NO from SFCRUNTIME.R_SHIP_DETAIL where sn ='{sn}') and valid_flag = 1 ";
            DataTable dt = sfcdb.Ado.GetDataTable(sqlcheck);
            if (dt.Rows.Count <= 0)
            {
                return null;
            }

            int i = 1;
            var rASnSub = new List<R_ARUBADATA_SN_SUB>();
            var cSkuDetail = sfcdb.Queryable<R_SN, C_SKU_DETAIL>((a, b) => a.SKUNO == b.SKUNO).Where((a, b) => a.VALID_FLAG == "1" && b.CATEGORY == "ARUBA_SNR" && a.SN == sn)
                .OrderBy((a, b) => b.STATION_NAME, OrderByType.Asc).Select((a, b) => b).ToList();

            foreach(var r in cSkuDetail)
            {
                string cpnTemp = "";
                string spnTemp = "";
                string serialNumber = "";
                string partCategory = "";
                string description = "";
                string supplierName = "";

                if (r.EXTEND == "MANUF_DATE")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var rSnDetail = sfcdb.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.STATION_NAME == "CBS" && t.EDIT_TIME < dtime).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (rSnDetail != null)
                    {
                        serialNumber = rSnDetail.EDIT_TIME?.ToString("yyyyMMdd");
                    }
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Date of manufacture";
                }
                else if(r.EXTEND == "SHIP_DATE")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    var listsn = sfcdb.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && t.SHIPPED_FLAG == "1").ToList();

                    if (listsn.Count != 0)
                    {
                        DateTime ship_date = listsn[0].SHIPDATE.ObjToDate();
                        serialNumber = ship_date.ToString("yyyyMMdd");
                    }
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Shipping Record";
                }
                else if (r.EXTEND == "PO_NO")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    serialNumber = dt.Rows[0]["PO_NO"].ToString();
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Shipping Record";
                }
                else if (r.EXTEND == "SHIP_TO_ADDRESS")
                {
                    cpnTemp = r.VALUE;
                    spnTemp = r.EXTEND;
                    serialNumber = dt.Rows[0]["SHIP_TO_ADDRESS"].ToString();
                    description = r.BASETEMPLATE != null ? r.BASETEMPLATE : "Shipping Record";
                }

                if (string.IsNullOrEmpty(cpnTemp) && string.IsNullOrEmpty(spnTemp)) continue;

                rASnSub.Add(new R_ARUBADATA_SN_SUB()
                {
                    ID = MesDbBase.GetNewID<R_ARUBADATA_SN_SUB>(sfcdb, _bustr),
                    SNID = snid,
                    SEQ = i,
                    MODULETYPE = "CM",
                    GENERICCATEGORY = "HW",
                    CPN = cpnTemp,
                    SPN = spnTemp,
                    SN = serialNumber,
                    OPERATION = "I",
                    QUANTITY = 1,
                    PARTCATEGORY = partCategory,
                    DESCRIPTION = description,
                    SUPPLIERNAME = supplierName,
                    CREATETIME = sfcdb.GetDate()
                });
                i = i + 1;
            }
            return rASnSub;
        }

        public FuncExecRes SendSNRData()
        {
            var funcExecres = new FuncExecRes();

            #region send to custermor      
            SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword);
            try
            {
                var rAsnList = sfcdb.Queryable<R_ARUBADATA_HEAD>().Where(t => t.DATATYPE == "SNR" && t.DATETYPE == _datetype && t.GET == "1" && t.CONVERT == "1" && t.SEND == "0").OrderBy(t => t.DATEKEY, OrderByType.Asc).ToList();
                foreach (var r in rAsnList)
                {
                    if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME))
                    {
                        sftpHelp.Put(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + r.FILENAME, $@"{CONST_SFTPPath}\\{r.FILENAME}");
                    }

                    string fileShipped = r.FILENAME.Replace(r.SEQ, (int.Parse(r.SEQ) + 1).ToString("000000"));
                    if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + fileShipped))
                    {
                        sftpHelp.Put(System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + fileShipped, $@"{CONST_SFTPPath}\\{fileShipped}");
                    }

                    r.SEND = "1";
                    sfcdb.Updateable(r).ExecuteCommand();
                }

                funcExecres.IsSuccess = true;
            }
            catch (Exception e)
            {
                funcExecres.ErrorException = e;
                funcExecres.ErrorMessage = e.Message;
                funcExecres.IsSuccess = false;
            }
            return funcExecres;
            #endregion
        }


        private void WriteFile(string fileName, string msg)
        {
            string logPath = System.IO.Directory.GetCurrentDirectory() + "\\SNR\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string logFile = System.IO.Directory.GetCurrentDirectory() + "\\SNR\\" + fileName;
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
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

        public class SNR
        {
            public string sn;
            public string parentSN;
            public R_ARUBADATA_SN rAsn;
            public List<R_ARUBADATA_SN_SUB> rAsnSubList;
        }
    }
}
