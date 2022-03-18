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
using System.IO;
using MESDataObject.Common;
using MESPubLab.MesBase;
using MES_DCN.Broadcom;
using CsvHelper;
using System.Globalization;
using DbType = SqlSugar.DbType;
using System.Collections;
using MESDataObject.Module.OM;
using MESDataObject.Module.Juniper;
using static MESJuniper.Base.SynAck;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Constants;
using MESJuniper.Base;
using static MESDataObject.Common.EnumExtensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using MESPubLab.MESInterface;
using MESPubLab.MESInterface.CRC;
//using log4net;

namespace MESInterface.UnitTest
{
    public class TestInterface : taskBase
    {
        public bool IsRuning = false;
        string dbstr, bustr, filepath, filebackpath;
        OleExec db = null;
        OleExec ldb = null;
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
                MesLog.Info("Start");
                testcrc();
                //t20220315();
                //t20220312();
                //t20220304();
                //t20220228();
                //test20220525();
                //test2022022490();
                //test20220126();
                //test20220115();
                //sys137bug();
                //testcsvnum();
                //testsqlsugarbug();
                //FsjErrorComponents();
                //bonpaile();
                //GetDcnAgingData();
                //daoshuju();
                //R20211224();
                //test211228();
                //R20211230();
                //BATCHTECO();
                //test140();
                //I2021();
                //resetwwn();
                //jnpbugprocess_fvn();
                //jnpbugprocess_fjz();
                //tecowo();
                //tecowo_fjz();
                //testrfc0593();
                //testdbupdateforceplant();
                //clearJnpRmq();
                //csvtest2();
                //fsj();
                //fsjtest();
                //test();
                //test1();
                //Test2();
                //Test3();
                //Test4();
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

        void testcrc()
        {
            //double A = 100.000;
            //var B = 0.0;
            //for (int i = 0; i < 60; i++)
            //{
            //    A = A * 0.95;
            //}


            var crc = new R_MES_CRC()
            {
                F_SITE = "FOL",
                F_BU = "NSD",
                F_FLOOR = "F11-2F",
                F_SUBJECT = "new exception occurred",
                F_PRIORITY = "P3",
                F_ERRORMESSAGE = "10.156.214.56 3306 port is down",
                F_DATASOURCE = "OM Issue",
                F_REPORTER = "G6001953",
                F_CONTACTNUMBER = "18378191995",
                F_EMAIL = "eden.q.wu@mail.foxconn.com",
                F_SENDER = "FOXCONN",
                F_OWNER = "吳強11",
                F_ESCALATION1 = "吳強11",
                F_ESCALATION2 = "吳強11",
                F_ESCALATION3 = "吳強11",
                F_CASEGROUP = "Group-C"
            };
            I_Crc crchelp = new FoxCrc();
            crchelp.Send(DataHelper.Mapper<CrcObj, R_MES_CRC>(crc));
        }


        string partnos = $@"750-136059|711-054743|711-054743,750-054758|711-054756|711-055267";

        class c20220115
        {
            public string PN { get; set; }
            public string SM750 { get; set; }
            public string W750 { get; set; }
            public string SW750 { get; set; }
            public string W711_1 { get; set; }
            public string W711_2 { get; set; }
            public string W711_3 { get; set; }
            public string W711_4 { get; set; }
            public string W711_5 { get; set; }

        }
        void t20220315()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var wolists = db.Queryable<R_WO_BASE,O_ORDER_MAIN,R_SN>((w,m,r)=>w.WORKORDERNO==m.PREWO && w.WORKORDERNO==r.WORKORDERNO)
                    .Where((w, m,r) => r.SHIPPED_FLAG == MesBool.No.ExtValue() ).Select((w, m,r) => w).Distinct().ToList();
                foreach (var item in wolists)
                {
                    var asbom = db.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == item.WORKORDERNO && t.PARENTPN == item.SKUNO).ToList().FirstOrDefault();
                    if (asbom != null && asbom.PARENTREV!= item.SKU_VER)
                    {
                        item.SKU_VER = asbom.PARENTREV;
                        db.Updateable(item).ExecuteCommand();
                    }
                }
            }
        }
        /// <summary>
        /// agiledata
        /// </summary>
        void t20220312()
        {
            var sourcefile = $@"C:\Users\G6001953.NN\source\repos\NN_CLOUDMES\CloudMes\MESInterface\bin\Debug\File\Jnp\cleifjz.xlsx";
            var dt = ExcelHelp.DBExcelToDataTableEpplus(sourcefile);
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var RES = db.Ado.UseTran(() =>
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            var citem = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item["ITEM_NUMBER"].ToString() && t.ACTIVED == MesBool.Yes.ExtValue()).Distinct().ToList().FirstOrDefault();
                            citem.ID = MesDbBase.GetNewID<O_AGILE_ATTR>(db, "FJZODB");
                            citem.REV = item["REV"].ToString();
                            citem.CLEI_CODE = item["CLEI"].ToString();
                            citem.ACTIVED = MesBool.No.ExtValue();
                            citem.ECI_BAR_CODE = item["ECI"].ToString();
                            citem.DATE_CREATED = Convert.ToDateTime(citem.DATE_CREATED).AddSeconds(-1);
                            db.Insertable(citem).ExecuteCommand();
                        }
                        catch (Exception E)
                        {
                           // throw E;
                        }
                    }
                });

            }
        }

        void t20220304()
        {
            var dnNo = "3006004192";
            var lineNo = "000010";
            var a = $@"{dnNo.Substring(2)}{(Convert.ToInt32(lineNo) / 10).ToString().PadLeft(2, '0')}";
        }

        void t20220228()
        {
            var sourcefile = $@"C:\Users\G6001953.NN\source\repos\NN_CLOUDMES\CloudMes\MESInterface\bin\Debug\File\soucefile\20220303.xlsx";
            var fullfilename = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\testfile\\";
            var filename = $@"Coo{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.csv";
            var dt = ExcelHelp.DBExcelToDataTableEpplus(sourcefile);
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                foreach (DataRow item in dt.Rows)
                {
                    var citem = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item["ITEM_NUMBER"].ToString() && t.ACTIVED==MesBool.Yes.ExtValue()).Distinct().ToList().FirstOrDefault() ;
                    citem.ID = MesDbBase.GetNewID<O_AGILE_ATTR>(db, "FJZJNP");
                    citem.REV = item["REV"].ToString();
                    citem.CLEI_CODE = item["CLEI"].ToString();
                    citem.ECI_BAR_CODE = item["ECI"].ToString();
                    db.Insertable(citem).ExecuteCommand();
                }
            }
            //ExcelHelp.ExportExcelToLoacl(dt,$@"{fullfilename}{filename}",true);
        }

        void test20220525()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var headdt = db.Ado.GetDataTable($@"select wo from r_pre_wo_head group by wo having count(1)>1");
                foreach (DataRow wodr in headdt.Rows)
                {
                    var wohead = db.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == wodr["wo"].ToString()).ToList();
                    if (wohead.Count > 1)
                    {
                        var delhead = wohead.FindAll(t => t.GROUPID == null).FirstOrDefault();
                        var detail = db.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == delhead.WO).ToList();
                        var pns = detail.Select(t => t.PARTNO).Distinct().ToList();
                        var delpns = new List<R_PRE_WO_DETAIL>();
                        foreach (var pn in pns)
                        {
                            var delpnduplicet = db.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == delhead.WO && t.PARTNO == pn).OrderBy(t => t.CREATETIME, OrderByType.Desc).ToList();
                            if(delpnduplicet.Count>1)
                                delpns.Add(delpnduplicet.FirstOrDefault());
                        }
                        var res = db.Ado.UseTran(()=> {
                            db.Deleteable(delhead).ExecuteCommand();
                            db.Deleteable(delpns).ExecuteCommand();
                        });
                    }
                }
            }
        }

        void test2022022490()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FVNJNPODB", false))
            {
                System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                var VALUE = gc.GetWeekOfYear(Convert.ToDateTime("2022-02-20"), System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
            }
        }

        void test20220126()
        {
            var file = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\I140.xlsx";
            //var file = "C:\\Users\\G6001953.NN\\Desktop\\jnp\\140\\I140.xlsx";
            var dt = ExcelHelp.DBExcelToDataTableEpplus(file);
            var fvn = dt.Select(" VENDOR_CODE = 'FXNVIETNAM'").ToList();
            var fjz = dt.Select(" VENDOR_CODE = 'FXNJUAREZ'").ToList();
            var fvni140 = new List<R_I140>();
            var fjzi140 = new List<R_I140>();
            var vntranid = "20220126105014430";
            var jztranid = "20000126105014455";
            var vnfilename = "AS2_FVN_I140_20210421_5bu5rg09f3p12jid0006k09i.xml";
            var jzfilename = "AS2_FJZ_I140_20210421_5bu5rg09f3p12jid0006k09i.xml";
            if (bustr == "FVN")
                using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FVNJNPODB", false))
                {
                    var res = db.Ado.UseTran(() =>
                    {
                        foreach (DataRow item in fvn)
                        {
                            fvni140.Add(new R_I140()
                            {
                                ID = MesDbBase.GetNewID<R_I140>(db, "JUNIPER"),
                                F_ID = item["F_ID"].ToString(),
                                TRANID = vntranid,
                                F_PLANT = "BTS",
                                FILENAME = vnfilename,
                                MESSAGEID = "EXECUTABLE",
                                CREATIONDATETIME = Convert.ToDateTime("2022-01-26"),
                                VENDORCODE = "0016000220",
                                PN = item["PRODUCT_CODE"].ToString(),
                                STARTDATETIME = Convert.ToDateTime(item["DEMAND_DATE"].ToString()),
                                ENDDATETIME = Convert.ToDateTime(item["DEMAND_DATE"].ToString()),
                                QUANTITY = item["DEMAND"].ToString(),
                                F_LASTEDITDT = DateTime.Now,
                                MFLAG = "N",
                                CREATETIME = DateTime.Now
                            });
                        }
                        db.Insertable(fvni140).ExecuteCommand();
                        if (!db.Queryable<R_I140_MAIN>().Where(t => t.TRANID == vntranid).Any())
                            db.Insertable(new R_I140_MAIN()
                            {
                                ID = MesDbBase.GetNewID<R_I140_MAIN>(db, Customer.JUNIPER.ExtValue()),
                                TRANID = vntranid,
                                WEEKNO = new GregorianCalendar().GetWeekOfYear(Convert.ToDateTime($@"{vntranid.Substring(0, 4)}-{vntranid.Substring(4, 2)}-{vntranid.Substring(6, 2)}"), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                                //YEARNO = DateTime.Now.Year.ToString(),
                                YEARNO = Convert.ToDateTime($@"{vntranid.Substring(0, 4)}-{vntranid.Substring(4, 2)}-{vntranid.Substring(6, 2)}").Year.ToString(),
                                COMPLETE = I_I140_MAIN_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
                                PLANT = JuniperB2BPlantCode.FVN.ToString(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now
                            }).ExecuteCommand();
                    });
                }
            if (bustr == "FJZ")
                using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
                {
                    var res = db.Ado.UseTran(() =>
                    {
                        foreach (DataRow item in fjz)
                        {
                            fjzi140.Add(new R_I140()
                            {
                                ID = MesDbBase.GetNewID<R_I140>(db, "JUNIPER"),
                                F_ID = item["F_ID"].ToString(),
                                TRANID = jztranid,
                                F_PLANT = "BTS",
                                FILENAME = jzfilename,
                                MESSAGEID = "EXECUTABLE",
                                CREATIONDATETIME = Convert.ToDateTime("2022-01-26"),
                                VENDORCODE = "0016000219",
                                PN = item["PRODUCT_CODE"].ToString(),
                                STARTDATETIME = Convert.ToDateTime(item["DEMAND_DATE"].ToString()),
                                ENDDATETIME = Convert.ToDateTime(item["DEMAND_DATE"].ToString()),
                                QUANTITY = item["DEMAND"].ToString(),
                                F_LASTEDITDT = DateTime.Now,
                                MFLAG = "N",
                                CREATETIME = DateTime.Now
                            });
                        }
                        db.Insertable(fjzi140).ExecuteCommand();
                        if (!db.Queryable<R_I140_MAIN>().Where(t => t.TRANID == vntranid).Any())
                            db.Insertable(new R_I140_MAIN()
                            {
                                ID = MesDbBase.GetNewID<R_I140_MAIN>(db, Customer.JUNIPER.ExtValue()),
                                TRANID = jztranid,
                                WEEKNO = new GregorianCalendar().GetWeekOfYear(Convert.ToDateTime($@"{jztranid.Substring(0, 4)}-{jztranid.Substring(4, 2)}-{jztranid.Substring(6, 2)}"), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                            //YEARNO = DateTime.Now.Year.ToString(),
                            YEARNO = Convert.ToDateTime($@"{jztranid.Substring(0, 4)}-{jztranid.Substring(4, 2)}-{jztranid.Substring(6, 2)}").Year.ToString(),
                                COMPLETE = I_I140_MAIN_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
                                PLANT = JuniperB2BPlantCode.FJZ.ToString(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now
                            }).ExecuteCommand();
                    });
                }
        }


        void test20220115()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false, DbType.SqlServer))
            {
                var pns = partnos.Split(',');
                var sws = db.Queryable<R_JUNIPER_SILVER_WIP>().ToList();
                var sws_wip = sws.FindAll(t => t.STATE_FLAG == "1").Select(t => t.SN).Distinct().ToList();
                var res = new List<c20220115>();
                foreach (var item in pns)
                {
                    var curenttres = new c20220115();
                    var pn750 = item.Split('|').FirstOrDefault();
                    var pn711s = item.Split('|').ToList().FindAll(t => t.StartsWith("711"));
                    //750 (Supermaket)
                    curenttres.SM750 = new Func<string>(()=> {
                        return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                                && !MESDBHelper.IMesDbEx.OracleContain(t.SN,sws_wip) && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.COMPLETED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                    })();
                    //750 (WIP)
                    curenttres.W750 = new Func<string>(() => {
                        return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                                && t.COMPLETED_FLAG == MesBool.No.ExtValue()).ToList().Count().ToString();
                    })();
                    //750 (Silver WIP)
                    curenttres.SW750 = new Func<string>(() => {
                        return db.Queryable<R_SN, R_JUNIPER_SILVER_WIP>((s,w)=>s.SN==w.SN).Where((s, w) => s.SKUNO == pn750 && s.VALID_FLAG == MesBool.Yes.ExtValue()
                                && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && w.STATE_FLAG=="1").ToList().Count().ToString();
                    })();

                    for (int i = 0; i < pn711s.Count(); i++)
                    {
                        var p711qty = db.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.SKUNO == pn711s[i]
                        && t.STARTED_FLAG==MesBool.Yes.ExtValue()).ToList().Count().ToString();
                        switch (i)
                        {
                            case 0: curenttres.W711_1 = p711qty; break;
                            case 1: curenttres.W711_2 = p711qty; break;
                            case 2: curenttres.W711_3 = p711qty; break;
                            case 3: curenttres.W711_4 = p711qty; break;
                            case 4: curenttres.W711_5 = p711qty; break;
                            default:
                                break;
                        }
                    }
                    res.Add(curenttres);
                }
                var key = DateTime.Now.ToString("yyyyMMddhhmmss");
                string filename = $@"C:\\Users\\G6001953.NN\\Desktop\\FJZ\\report\\{key}.csv";
                //scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
                ExcelHelp.ExportCsv(res, filename);
            }
        }

        string tecolist = $@"";

        void test20220114()
        {
            var a = new List<temp1111> {
                new temp1111(){ po="AA",version = "55"},
                new temp1111(){ po="BB",version = "44"},
                new temp1111(){ po="CC",version = "44"},
                new temp1111(){ po="DD",version = "33"},
                new temp1111(){ po="EE",version = "11"},
                new temp1111(){ po="FF",version = "22"} };

            var b = new List<temp1111> {
                new temp1111(){ version="55"},
                new temp1111(){ version="44"} };

            var aaa =(from t in a
             join e in b on t.version equals e.version
             select t).ToList();
        }

        void BATCHTECO()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FVNJNPODB", false))
            {
                var tartget = tecolist.Split(',');
                foreach (var item in tartget)
                {
                    try
                    {
                        var tecores = JuniperBase.TecoSapWo("VNJUNIPER", item);
                        if (tecores.issuccess)
                            MesLog.Info($@"wo:{item} res:{tecores.issuccess} msg:{tecores.msg}");
                    }
                    catch (Exception e)
                    {
                        MesLog.Info($@"wo:{item} res:fail msg:{e.Message}");
                    }
                }
            }
        }

        void daoshuju()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var res = db.Queryable<O_I137_HEAD>().Where(t=>t.VERSION=="0").Select(t=>new { t.PONUMBER ,t.CREATETIME,t.VERSION}).ToList();
                var orders = i211221.Split(',');
                var ress = new List<temp1111>();
                foreach (var item in orders)
                {
                    var o =res.FindAll(t => t.PONUMBER == item).ToList().FirstOrDefault();
                    var i = new temp1111() { po = item, version = o.VERSION,createtime = o.CREATETIME};
                    ress.Add(i);
                }
                ExcelHelp.ExportCsv(ress, "C:\\Users\\G6001953.NN\\Desktop\\VN\\data211221.csv");
            }
        }

        class temp1111
        {
            public string po { get; set; }
            public string version { get; set; }
            public DateTime? createtime { get; set; }
        }


        class R20211224detail_A
        {
            public string UPOID { get; set; }
            public string PID { get; set; }
            public string QTY { get; set; }
            public string PARENTPN { get; set; }
            public string PN { get; set; }
            public int STOCKQTY { get; set; }
            public DateTime? DELIVERY { get; set; }
            public List<R20211224detail_A> R711OBJ { get; set; }
        }

        class class20211230
        {
            public string sn { get; set; }
            public string station { get; set; }
        }

        void R20211230()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var nocheckstation = new List<string>() { "5DX","2DX"};
                var fiters = new List<string>() { "RETURN", "REWORK" };
                //var target = db.Queryable<R_SN_STATION_DETAIL>().Where(t => sstations.Contains(t.STATION_NAME) && t.VALID_FLAG==MesBool.Yes.ExtValue()).OrderBy(t => t.SN, OrderByType.Desc).OrderBy(t=>t.EDIT_TIME,OrderByType.Desc).ToList();
                var target = db.Queryable<R_SN_STATION_DETAIL>().Where(t => fiters.Contains(t.STATION_NAME) && t.VALID_FLAG == MesBool.Yes.ExtValue()).OrderBy(t => t.SN, OrderByType.Desc).OrderBy(t => t.EDIT_TIME, OrderByType.Desc)
                    .Select(t=>new { t.SN,t.STATION_NAME,t.EDIT_TIME}).ToList();
                var sns = target.Select(t => t.SN).Distinct();
                var res = new List<class20211230>();
                var tolnum = sns.Count();
                var processnum = 0;
                foreach (var sn in sns)
                {
                    processnum++;
                    var snitem = target.FindAll(t => t.SN == sn).OrderByDescending(t => t.EDIT_TIME).ToList().FirstOrDefault();
                    var ssnstation = db.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn  && t.EDIT_TIME > snitem.EDIT_TIME && t.VALID_FLAG == MesBool.Yes.ExtValue() && t.REPAIR_FAILED_FLAG == MesBool.No.ExtValue()) .OrderBy(t => t.EDIT_TIME, OrderByType.Asc).ToList();
                    if (ssnstation.Count==0)
                        continue;
                    var fsnevent = ssnstation.FirstOrDefault();
                    var testations = db.Queryable<R_SN, C_ROUTE_DETAIL, C_TEMES_STATION_MAPPING, C_ROUTE_DETAIL>((s, r, t, r2) => s.ROUTE_ID == r.ROUTE_ID && r.STATION_NAME == t.MES_STATION && s.ROUTE_ID == r2.ROUTE_ID)
                        .Where((s, r, t, r2) => s.SN == sn && s.VALID_FLAG == MesBool.Yes.ExtValue()  && r2.STATION_NAME == fsnevent.STATION_NAME && r.SEQ_NO >= r2.SEQ_NO)
                        .Select((s, r, t, r2) => new { s, r.STATION_NAME }).ToList();
                    if (testations.Count == 0)
                        continue;
                    var testrecords = db.Queryable<R_TEST_RECORD>().Where(t => t.SN == sn && t.STATE == "PASS").ToList();
                    var pass = db.Queryable<R_SN_PASS>().Where(t => t.SN == sn).ToList();
                    foreach (var item in testations)
                    {
                        if (testrecords.FindAll(t => t.MESSTATION == item.STATION_NAME).Count() == 0)
                            continue;
                        if (nocheckstation.Contains(item.STATION_NAME))
                            continue;
                        if (pass.FindAll(t=>t.PASS_STATION == item.STATION_NAME).Any())
                            continue;
                        if (ssnstation.FindAll(t => t.STATION_NAME == item.STATION_NAME).Any())
                        {
                            if (!testrecords.FindAll(t => t.EDIT_TIME > snitem.EDIT_TIME && t.MESSTATION == item.STATION_NAME && t.STATE== "PASS").Any())
                                res.Add(new class20211230() { 
                                sn = sn,station = item.STATION_NAME
                                });
                        }
                    }
                }
                try
                {
                    ExcelHelp.ExportCsv(res, $@"C:\\Users\\G6001953.NN\\Desktop\\VN\\VN{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.csv");
                }
                catch (Exception e)
                {

                }
            }
        }
        void R20211224()
        {
            using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var detial = mesdb.Queryable<O_ORDER_MAIN, R_SAP_AS_BOM, O_PO_STATUS>((m, w, s) => m.PREWO == w.WO && m.ID == s.POID).Where((m, w, s) => m.CANCEL == MesBool.No.ExtValue() && m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString()
                                      && new string[] { "32", "29", "28", "31", "13", "14" }.Contains(s.STATUSID) && w.PN.StartsWith("750-")).OrderBy((m, w, s) =>m.DELIVERY)
                                      .Select((m, w, s) => new R20211224detail(){ UPOID=m.UPOID, PID=m.PID, QTY=m.QTY, PARENTPN=w.PARENTPN, PN=w.PN, DELIVERY=m.DELIVERY }).ToList();

                mesdb.Queryable<O_ORDER_MAIN, R_SN, O_PO_STATUS>((m, w, s) => m.PREWO == w.WORKORDERNO && m.ID == s.POID).Where((m, w, s) => m.CANCEL == MesBool.No.ExtValue()
                                        && m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && w.VALID_FLAG == MesBool.Yes.ExtValue()
                                        && new string[] { "32", "29", "28", "31", "13", "14" }.Contains(s.STATUSID) && w.SHIPPED_FLAG == MesBool.No.ExtValue() && w.COMPLETED_FLAG == MesBool.Yes.ExtValue())
                                       .GroupBy((m, w, s) => m.UPOID).Select((m, w, s) => new { m.UPOID, FQTY = SqlFunc.AggregateCount(w.SN) }).ToList().ForEach(t=> {
                                           detial.FindAll(p => p.UPOID == t.UPOID).ForEach(c =>
                                             c.QTY = (double.Parse(c.QTY) - t.FQTY).ToString()
                                             );
                                       });             
                var r750711map = new List<R20211224detail>();
                    mesdb.SqlQueryable<R_WO_BASE>($@" select * from ( select a.*,ROW_NUMBER() OVER(PARTITION BY skuno ORDER BY download_date DESC) ROWNUMB from r_wo_base a ) where skuno like '750-%' and  ROWNUMB=1")
                    .ToList().ForEach(t=>{
                        mesdb.Queryable<R_WO_ITEM>().Where(s => s.AUFNR == t.WORKORDERNO && s.MATNR.StartsWith("711")).ToList().ForEach(p=> {
                            r750711map.Add(new R20211224detail()
                            {
                                PARENTPN = t.SKUNO,
                                PN = p.MATNR,
                                QTY = p.PARTS
                            });
                        });
                    });

                var pn750list = detial.Select(t => t.PN).Distinct().ToList();
                var pn711list = r750711map.FindAll(t => pn750list.Contains(t.PARENTPN)).Select(t => t.PN).Distinct().ToList();
                var s750711 = mesdb.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.COMPLETED_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue()
                && (pn750list.Contains(t.SKUNO) || pn711list.Contains(t.SKUNO))).GroupBy(t => t.SKUNO).Select(t => new R20211224detail (){ PN = t.SKUNO, STOCKQTY = SqlFunc.AggregateCount(t.SN) }).ToList();


                var gdetail_750 = (from t in detial
                               group t by new { t.PN, t.DELIVERY } into g
                               select new
                               {
                                   //PARENTPN = g.Key.PARENTPN,
                                   PN = g.Key.PN,
                                   DELIVERY = g.Key.DELIVERY,
                                   QTY = g.Sum(t => double.Parse(t.QTY)),
                               }).OrderBy(t=>t.DELIVERY).ToList();

                var res = new List<R20211224detail>();
                foreach (var item in gdetail_750)
                {
                    var itemobj750 = new R20211224detail();
                    var sqtyobj = s750711.FindAll(t => t.PN == item.PN).ToList().FirstOrDefault();
                    itemobj750.STOCKQTY = 0;
                    if (sqtyobj != null && sqtyobj.STOCKQTY > 0)
                    {
                        itemobj750.STOCKQTY = sqtyobj.STOCKQTY >= item.QTY ? int.Parse(item.QTY.ToString()) : sqtyobj.STOCKQTY;
                        sqtyobj.STOCKQTY = sqtyobj.STOCKQTY - int.Parse(item.QTY.ToString());
                    }
                    itemobj750.PN = item.PN;
                    itemobj750.DELIVERY = item.DELIVERY;
                    itemobj750.QTY = item.QTY.ToString();
                    if (itemobj750.STOCKQTY < int.Parse(itemobj750.QTY))
                    {
                        var r711pn = r750711map.FindAll(t => t.PARENTPN == item.PN).ToList();
                        var r711sub = new List<R20211224detail>();
                        foreach (var r711item in r711pn)
                        {
                            var itemobj711 = new R20211224detail();
                            itemobj711.PN = r711item.PN;
                            itemobj711.DELIVERY = item.DELIVERY;
                            itemobj711.QTY = (int.Parse(itemobj750.QTY) - itemobj750.STOCKQTY).ToString();
                            itemobj711.STOCKQTY = 0;
                            var r711ss = s750711.FindAll(t => t.PN == r711item.PN).ToList().FirstOrDefault();
                            if (r711ss != null && r711ss.STOCKQTY > 0)
                            {
                                itemobj711.STOCKQTY = r711ss.STOCKQTY >= item.QTY ? int.Parse(r711item.QTY.ToString()) : r711ss.STOCKQTY;
                                r711ss.STOCKQTY = r711ss.STOCKQTY - int.Parse(r711item.QTY.ToString());
                            }
                            r711sub.Add(itemobj711);
                        }
                        itemobj750.R711OBJ = r711sub;
                    }
                    res.Add(itemobj750);
                }

                ExportExcelToLoaclByTest(res,$@"C:\\Users\\G6001953.NN\\source\\repos\\NN_CLOUDMES\\CloudMes\\MESInterface\\bin\\Debug\\File\\{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}FJZJNP.xlsx", pn750list, r750711map);
            }
        }


        [Serializable]
        class R20211224detail
        {
            public string UPOID { get; set; }
            public string PID { get; set; }
            public string QTY { get; set; }
            public string PARENTPN { get; set; }
            public string PN { get; set; }
            public int STOCKQTY { get; set; }
            public DateTime? DELIVERY { get; set; }
            public List<R20211224detail> R711OBJ { get; set; }
        }

        void ExportExcelToLoaclByTest(List<R20211224detail> datas, string file, List<string> pn750list, List<R20211224detail> r750711map)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo = new FileInfo(file);
            }

            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage(fileInfo);
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}FJZJNP");
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1
            //if (withHeader)
            //{
            //    for (int i = 0; i < dt.Columns.Count; i++)
            //    {
            //        worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
            //        //自动调整列宽，也可以指定最小宽度和最大宽度
            //        worksheet.Column(colIndex + i).AutoFit();
            //    }
            //    rowIndex = 2;
            //}

            //// 跳过第一列列名
            //rowIndex++;
            //写入数据
            try
            {

                var deliverys = datas.Select(t => t.DELIVERY).Distinct().ToList();
                var colors = new List<Color>() { ColorTranslator.FromHtml("#7fcbfe"), ColorTranslator.FromHtml("#8fecee") ,ColorTranslator.FromHtml("#bf77ae") };                
                for (int i = 0; i < deliverys.Count(); i++)
                {
                    var lastrowIndex = 2;
                    worksheet.Cells[rowIndex, i * 6+1, rowIndex, (i+1) * 6].Merge = true;
                    worksheet.Cells[rowIndex, i * 6 + 1, rowIndex,  (i + 1) * 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowIndex, i * 6 + 1, rowIndex, (i + 1) * 6].Value = Convert.ToDateTime(deliverys[i]).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                    //worksheet.Column(colIndex - 1 + i * 5 + 1).AutoFit();

                    //worksheet.Cells[2, 1].Value = "750PN";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 1].Value = "750Pn";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 2].Value = "Request";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 3].Value = "Stock";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 4].Value = "711Pn";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 5].Value = "Request";
                    worksheet.Cells[lastrowIndex, colIndex - 1 + i * 6 + 6].Value = "Stock";
                    for (int w = 1; w <= 6; w++)
                    {
                        worksheet.Column(i * 6 + w).Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Column(i * 6 + w).Style.Fill.BackgroundColor.SetColor(colors[i % 3]);
                    }
                }
                rowIndex = 2;
                for (int i = 0; i < pn750list.Count(); i++)
                {
                    var map = r750711map.FindAll(t => t.PARENTPN == pn750list[i]).ToList();
                    var erow = rowIndex + (map == null || map.Count <= 1 ? 1 : map.Count());
                    ++rowIndex;
                    for (int k = 0; k < deliverys.Count(); k++)
                    {
                        worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 1, erow, colIndex - 1 + k * 6 + 1].Merge = true;
                        worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 1, erow, colIndex - 1 + k * 6 + 1].Value = pn750list[i];
                        var dpnobj = datas.FindAll(t => t.PN == pn750list[i] && t.DELIVERY == deliverys[k]).ToList().FirstOrDefault();
                        worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 2, erow, colIndex - 1 + k * 6 + 2].Value = dpnobj == null ? "0" : dpnobj.QTY;
                        worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 3, erow, colIndex - 1 + k * 6 + 3].Value = dpnobj == null ? "0" : dpnobj.STOCKQTY.ToString();
                        if (dpnobj == null)
                        {
                            worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 4, erow, colIndex - 1 + k * 6 + 4].Value = map == null || map.Count == 0 ? "" : map.FirstOrDefault().PN;
                            worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 5, erow, colIndex - 1 + k * 6 + 5].Value = "0";
                            worksheet.Cells[rowIndex, colIndex - 1 + k * 6 + 6, erow, colIndex - 1 + k * 6 + 6].Value = "0";
                            continue;
                        }
                        if (map == null || map.Count <=1)
                        {
                            worksheet.Cells[rowIndex, colIndex - 1 +k * 6 + 4, erow, colIndex - 1 + k * 6 + 4].Value = map == null || map.Count ==0 ? "" : map.FirstOrDefault().PN;
                            worksheet.Cells[rowIndex, colIndex - 1 + k * 6 +5, erow, colIndex - 1 + k * 6 + 5].Value = dpnobj.R711OBJ == null || dpnobj.R711OBJ.Count == 0 ? "0" : dpnobj.R711OBJ[0].QTY;
                            worksheet.Cells[rowIndex, colIndex - 1 +k * 6 + 6, erow, colIndex - 1 + k * 6 + 6].Value = dpnobj.R711OBJ == null || dpnobj.R711OBJ.Count == 0 ? "0" : dpnobj.R711OBJ[0].STOCKQTY.ToString();
                        }
                        else
                        {
                            for (int j = 0; j < map.Count; j++)
                            {
                                if (dpnobj.R711OBJ == null || dpnobj.R711OBJ.Count == 0)
                                {
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 4, rowIndex + j, colIndex - 1 + k * 6 + 4].Value = map == null ? "" : map[j].PN;
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 5, rowIndex + j, colIndex - 1 + k * 6 + 5].Value = "0";
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 6, rowIndex + j, colIndex - 1 + k * 6 + 6].Value = "0";
                                }
                                else
                                {
                                    var subobj = dpnobj.R711OBJ.FindAll(t => t.PN == map[j].PN).ToList().FirstOrDefault();
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 4, rowIndex + j, colIndex - 1 + k * 6 + 4].Value = subobj == null ? "" : subobj.PN;
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 5, rowIndex + j, colIndex - 1 + k * 6 + 5].Value = subobj == null ? "0" : subobj.QTY;
                                    worksheet.Cells[rowIndex + j, colIndex - 1 + k * 6 + 6, rowIndex + j, colIndex - 1 + k * 6 + 6].Value = subobj == null ? "0" : subobj.STOCKQTY.ToString();
                                }
                            }
                            rowIndex = erow;
                        }
                    }
                }
            }
            catch (Exception ee)
            {

            }


            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //    {
            //        worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
            //    }
            //    //自动调整行高
            //    worksheet.Row(rowIndex + i).CustomHeight = true;
            //}

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //单元格背景样式，要设置背景颜色必须先设置背景样式
            //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //单元格背景颜色
            //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
            //设置单元格所有边框样式和颜色
            //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
            //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
            //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            package.Save();
            worksheet.Dispose();
            package.Dispose();
        }

        void test211228()
        {
            var a = new List<R20211224detail>();
            a.Add(new R20211224detail() { UPOID ="111"});
            a.Add(new R20211224detail() { UPOID = "222" });
            a.Add(new R20211224detail() { UPOID = "333" });
            var b = Extensions.Clone(a);
            b[1].UPOID = "444";
        }

        void test140()
        {
            using (var b2bdb = MESDBHelper.OleExec.GetSqlSugarClient("LHB2BODB", false, DbType.SqlServer))
            {
                var vender = JuniperB2BPlantCode.FJZ.ExtValue();
                var waitsynlist = b2bdb.Queryable<B2B_R_I140>()
                    .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse((-3).ToString())) && t.VENDORCODE == vender).ToList();
                MesLog.Debug($@"test140 select B2B_R_I140: {DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue())}");
                if (waitsynlist.Count > 0)
                    using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
                    {
                        var tranids = waitsynlist.Select(t => t.TRANID).Distinct();
                        var mes140data = mesdb.Queryable<R_I140>().OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(100000).ToList();
                        MesLog.Debug($@"test140 mes140data cache: {DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue())}");
                        foreach (var ctranid in tranids)
                        {
                            var res = mesdb.Ado.UseTran(() =>
                            {
                                var traniddetail = waitsynlist.Where(t => t.TRANID == ctranid).ToList();
                                var targetdata = new List<R_I140>();
                                foreach (var item in traniddetail)
                                {
                                    //先检查缓存; 
                                    if (mes140data.Any(t => t.F_ID == item.F_ID))
                                        continue;
                                    if (mesdb.Queryable<R_I140>().Where(t => t.F_ID == item.F_ID).Any())
                                        continue;
                                    var aa = mesdb.Queryable<R_I140>().Where(t => t.F_ID == item.F_ID).ToList();
                                    var targetobj = ObjectDataHelper.Mapper<R_I140, B2B_R_I140>(item);
                                    targetobj.ID = MesDbBase.GetNewID<R_I140>(mesdb, Customer.JUNIPER.ExtValue());
                                    targetobj.CREATETIME = DateTime.Now;
                                    targetobj.MFLAG = "N";
                                    targetdata.Add(targetobj);
                                }
                                MesLog.Debug($@"test140 process cache: {DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue())}");
                                //mesdb.Insertable(targetdata).ExecuteCommand();
                                //if (!mesdb.Queryable<R_I140_MAIN>().Where(t => t.TRANID == ctranid).Any())
                                //    mesdb.Insertable(new R_I140_MAIN()
                                //    {
                                //        ID = MesDbBase.GetNewID<R_I140_MAIN>(mesdb, Customer.JUNIPER.ExtValue()),
                                //        TRANID = ctranid,
                                //        WEEKNO = new GregorianCalendar().GetWeekOfYear(Convert.ToDateTime($@"{ctranid.Substring(0, 4)}-{ctranid.Substring(4, 2)}-{ctranid.Substring(6, 2)}"), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                                //        //YEARNO = DateTime.Now.Year.ToString(),
                                //        YEARNO = Convert.ToDateTime($@"{ctranid.Substring(0, 4)}-{ctranid.Substring(4, 2)}-{ctranid.Substring(6, 2)}").Year.ToString(),
                                //        COMPLETE = I_I140_MAIN_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
                                //        PLANT = traniddetail[0].VENDORCODE == JuniperB2BPlantCode.FJZ.Ext<EnumValueAttribute>().Description ? JuniperB2BPlantCode.FJZ.ToString() : JuniperB2BPlantCode.FVN.ToString(),
                                //        CREATETIME = DateTime.Now,
                                //        EDITTIME = DateTime.Now
                                //    }).ExecuteCommand();
                            });
                        }
                    }
            }
        }

        void resetwwn()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var sns = czsns.Split(',');
                foreach (var item in sns)
                {
                    var sn = item.Replace("\r\n","");
                    //sn = "DUS4534S02C";
                    var wwn = db.Queryable<WWN_DATASHARING>().Where(t => t.WSN == sn).ToList().FirstOrDefault();
                    if (wwn == null)
                        continue;
                    var d84sn = db.Queryable<R_SN_KP, R_SN>((k, s) => k.SN == s.SN).Where((k, s) => k.VALUE == sn && k.VALID_FLAG.ToString()==MesBool.Yes.ExtValue()  && s.SKUNO.StartsWith("84-"))
                        .Select((k, s) => s).ToList();
                    var d84ssn = db.Queryable<R_SN_KP,R_SN_KP, R_SN>((k,k2, s) =>  k.SN == k2.VALUE && k2.SN==s.SN)
                        .Where((k, k2, s) => k.VALUE == sn && k.VALID_FLAG.ToString() == MesBool.Yes.ExtValue() && s.VALID_FLAG==MesBool.Yes.ExtValue() && k2.VALID_FLAG.ToString()==MesBool.Yes.ExtValue()
                        && s.SKUNO.StartsWith("84-"))
                        .Select((k, k2, s) =>new { k2 ,s}).ToList();
                    string d84snstr = "";
                    string d84sku = "";
                    string d60sn = "";
                    string d60sku = "";
                    if (d84sn.Count > 0)
                    {
                        d84snstr = d84sn.FirstOrDefault().SN;
                        d84sku = d84sn.FirstOrDefault().SKUNO;
                    }
                    else if (d84ssn.Count > 0)
                    { 
                        d84snstr = d84ssn.FirstOrDefault().s.SN;
                        d60sn = d84ssn.FirstOrDefault().k2.VALUE;
                        d60sku = d84ssn.FirstOrDefault().k2.PARTNO;
                        d84sku = d84ssn.FirstOrDefault().s.SKUNO;
                    }
                    if (wwn.CSSN != d84snstr && wwn.CSSN!="N/A")
                    {
                        db.Updateable<WWN_DATASHARING>().SetColumns(t => new WWN_DATASHARING() { CSSN = d84snstr, CSKU = d84sku }).Where(t => t.WSN == sn).ExecuteCommand();
                    }
                    if (d60sn=="")
                    {
                        db.Updateable<WWN_DATASHARING>().SetColumns(t => new WWN_DATASHARING() { VSSN = "N/A", VSKU = "N/A" }).Where(t => t.WSN == sn).ExecuteCommand();
                    }
                    else if (wwn.VSSN != d60sn)
                        db.Updateable<WWN_DATASHARING>().SetColumns(t => new WWN_DATASHARING() { VSSN = d60sn, VSKU = d60sku }).Where(t => t.WSN == sn).ExecuteCommand();
                }
            }

        }
        void testrfc0593()
        {
            var rfc0593 = new ZCPP_NSBG_0593("FJZTEST");
            rfc0593.SetValue("2021-11-15", "895", "VUEA");
            rfc0593.CallRFC();
            var dtres = rfc0593.GetTableValue("OUT_TAB");
            var res = rfc0593.GetValue("O_MESSAGE");
            if (res.Trim().Equals("FIND DATA SUCCESS!"))
            {
                ExcelHelp.ExportExcelToLoacl(dtres, "C:\\Users\\G6001953.NN\\Desktop\\temp\\outputfile\\0593.xlsx",true);
            }
        }

        void DCN8460KP()
        {
            var tartsns = $@"DYG4535S0AN,DUT4535S0AD,DUS4535S05R|
                            DYG4535S06S,DUT4535S06L,DUS4534S01V|
                            DYG4535S04F,DUT4535S04A,DUS4535S02D|
                            DYG4535S06R,DUT4535S06K,DUS4535S047|
                            DYG4535S086,DUT4535S082,DUS4535S00P|
                            DYG4535S0CS,DUT4535S00N,DUS4535S01K|
                            DYG4535S0AK,DUT4535S0AA,DUS4535S06F|
                            DYG4535S05T,DUT4535S05E,DUS4535S02W|
                            DYG4535S07F,DUT4535S07A,DUS4535S065|
                            DYG4535S0AF,DUT4535S0A6,DUS4535S09B|
                            ".Split('|');
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                foreach (var itemobj in tartsns)
                {
                    var SN84 = itemobj.Split(',')[0].Replace("\r\n", "").Trim().ToUpper();
                    var SN60 = itemobj.Split(',')[1].Replace("\r\n", "").Trim().ToUpper();
                    var SN40 = itemobj.Split(',')[2].Replace("\r\n", "").Trim().ToUpper();
                    var mes84 = db.Queryable<R_SN_KP>().Where(t => t.SN == SN84 && t.PARTNO.StartsWith("60-") && t.VALID_FLAG == Convert.ToDouble(MesBool.Yes.ExtValue())).ToList();
                    if (mes84.Count == 0)
                    {
                        MesLog.Info($@"DCN8460KP-SN:{SN84} err:84SN IS NOT EXISTS KP!");
                        continue;
                    }
                    var meswn84 = db.Queryable<WWN_DATASHARING>().Where(t => t.CSSN == SN84).ToList();
                    var mes60KP = mes84.FindAll(t => t.SCANTYPE.EndsWith("S/N")).ToList().FirstOrDefault();
                    if (mes60KP.VALUE.Trim().Equals(SN60))
                    {
                        MesLog.Info($@"DCN8460KP-SN:{SN84} err:84 SN MISS 60SN KP!");
                        continue;
                    }
                    var mes60 = db.Queryable<R_SN_KP>().Where(t => t.SN == mes60KP.VALUE.Trim() && t.PARTNO.StartsWith("60-") && t.VALID_FLAG == Convert.ToDouble(MesBool.Yes.ExtValue())).ToList();
                    if (mes60.Count == 0)
                    { }
                        var res = db.Ado.UseTran(() => {
                        var mes84_errsn = db.Queryable<R_SN_KP>().Where(t => t.VALUE == SN60 && t.VALID_FLAG == Convert.ToDouble(MesBool.Yes.ExtValue())).ToList().FirstOrDefault();
                        if (mes84_errsn == null)
                        {

                        }
                        else
                        {
                            var mes84_errkp = db.Queryable<R_SN_KP>().Where(t => t.VALUE == mes84_errsn.SN && t.VALID_FLAG == Convert.ToDouble(MesBool.Yes.ExtValue()) && t.PARTNO.StartsWith("60-")).ToList();
                           // var wn_errkp = db.Queryable<WWN_DATASHARING>().Where(t => t.VALUE == mes84_errsn.SN && t.VALID_FLAG == Convert.ToDouble(MesBool.Yes.ExtValue()) && t.PARTNO.StartsWith("60-")).ToList();
                            foreach (var item in mes84_errkp)
                            {

                                item.VALID_FLAG = Convert.ToDouble(MesBool.No.ExtValue());
                                db.Updateable(item).ExecuteCommand();
                                item.ID = MesDbBase.GetNewID<R_SN_KP>(db, "VNDCN");
                                item.VALID_FLAG = Convert.ToDouble(MesBool.Yes.ExtValue());
                                if (item.SCANTYPE.EndsWith("S/N"))
                                    item.VALUE = SN60;
                                item.EDIT_TIME = DateTime.Now;
                                db.Insertable(item).ExecuteCommand();


                            }
                        }
                    });
                }
            }
        }

        void tecowo()
        {
            var wolist = $@"007A00000084,
007A00000086,
007A00000087,
007A00000088,
007A00000089,
007A00000090,
007A00000092,
007A00000093,
007A00000094,
007A00000095,
007A00000097,
007A00000098,
007A00000099,
007A00000100,
007A00000101,
007A00000102,
007A00000103,
007A00000104,
007A00000105,
007A00000106,
007A00000107,
007A00000108,
007A00000181,
007A00000183,
007A00000188,
007A00000196,
007A00000200,
007A00000202,
007A00000211,
007A00000212,
007A00000213,
007A00000219,
007A00000222,
007A00000230,
007A00000265,
007A00000294,
007A00000331,
007A00000341,
007A00000344,
007A00000346,
007A00000347,
007A00000356,
007A00000360,
007A00000366,
007A00000367,
007A00000407,
007A00000722,
007A00000725,
007A00001078,
007A00001085,
007A00001955,
007A00001956,
007A00002021,
007A00002042,
007A00002043,
007A00002044,
007A00002046,
007A00002047,
007A00002111,
007B00000142,
007B00000159,
007B00000168,
007B00000265,
007B00000291,
007B00000293,
007B00000294,
007B00000295,
007B00000296,
007B00000300,
007B00000305,
007B00000306,
007B00000309,
007B00000314,
007B00000351,
007B00000362,
007B00000394,
007B00000491,
007B00000494,
007B00000512,
007B00000516,
007B00000518,
007B00000586,
007B00000602,
007B00000611,
007B00000612,
007B00000613,
007B00000617,
007B00000634,
007B00000672,
007B00000683,
007B00000700,
007B00000701,
007B00000755,
007B00000842,
007B00000896,
007B00000897,
007B00000904,
007B00000908,
007B00000910,
007B00000911,
007B00000913,
007B00000916,
007B00000919,
007B00000934,
007B00000937,
007B00000944,
007B00000951,
007B00000958,
007B00000962,
007B00000971,
007B00000998,
007B00001000,
007B00001008,
007B00001038,
007B00001063,
007B00001071,
007B00001072,
007B00001165,
007B00001175,
007B00001232,
007B00001325,
007B00001326,
007B00001329,
007B00001332,
007B00001431,
007B00001641,
007B00001644,
007B00001645,
007B00001656,
007B00001671,
007B00001678,
007B00001724,
007B00001768,
007B00002028,
007B00002157,
007B00002179,
007B00002183,
007B00002218,
007B00002393,
007B00002405,
007B00002472,
007B00002473,
007B00002478,
007B00002479,
007B00002542,
007B00002543,
007B00002544,
007B00002545,
007B00002546,
007B00002547,
007B00002550,
007B00002551,
007B00002568,
007B00002580,
007B00002609,
007B00002610,
007B00002611,
007B00002635,
007B00002636,
007B00002688,
007B00002694,
007B00002695,
007B00002701,
007B00002769,
007C00000169,
007C00000176,
007C00000177,
007C00000182,
007C00000196,
007C00000288,
007C00000306,
007C00000462,
007C00000538,
007C00000555,
007C00000634,
007C00000783,
007C00000799,
007C00000837,
007C00000838,
007C00000844,
007C00000852,
007C00000859,
007C00000861,
007C00000862,
007C00000865,
007C00000882,
007C00000883,
007C00000900,
007C00000910,
007C00000912,
007D00000106,
007D00000133,
007D00000135,
007D00000157,
007D00000158,
007D00000159,
007D00000165,
007D00000241,
007D00000293,
007D00000296,
007D00000297,
007D00000327,
007D00000328,
007D00000329,
007D00000330,
007D00000331,
007D00000332,
007D00000333,
007D00000334,
007D00000335,
007D00000336,
007D00000338,
007D00000410,
007D00000411,
007E00000037,
007G00004529,
007G00004700,
007G00006625,
007G00006626,
007G00006699,
007G00006702,
007G00007103,
007G00007104,
007G00007536,
007G00007537,
007G00007601,
007G00007607,
007G00007608,
007G00007609,
007G00007610,
007G00007613,
007G00007615,
007G00007616,
007G00007619,
007G00007804,
007G00007805,
007G00008212,
007G00008213,
007G00008537,
007G00008928,
007G00009487,
007G00009573,
007G00009611,
007G00009614,
007G00009773,
007G00009859,
007G00009860,
007G00009937,
007G00009941,
007G00010101,
007G00010168,
007G00010172,
007G00010188,
007G00010475,
007G00010476,
007G00010478,
007G00010479,
007G00013575,
007G00013808,
007G00013819,
007G00014912,
007G00015059,
007G00015060,
007L00000014".Replace("\r\n","").Split(',');
            foreach (var item in wolist)
            {
                //var tecores = JuniperBase.TecoSapWo("FJZJNP", item);
                var tecores = JuniperBase.TecoSapWo("VNJUNIPER", item);
                if (!tecores.issuccess)
                    MesLog.Info($@"teco wo fail :{item} {tecores.msg}");
                else
                    MesLog.Info($@"teco wo ok:{item} {tecores.msg}");
            }
        }

        void tecowo_fjz()
        {
            var wolist = $@"006A00009296,
006A00009298,
006A00020048,
006A00020790,
006A00021780,
006A00021978,
006A00022219,
006A00022220,
006A00022383,
006A00022655,
006A00022945,
006A00022958,
006A00023277,
006A00023361,
006A00023406,
006A00023424,
006A00023433,
006A00023464,
006A00023467,
006A00023496,
006A00023499,
006A00023501,
006A00023505,
006A00023517,
006A00023588,
006A00023632,
006A00023670,
006A00023671,
006A00023756,
006A00023759,
006A00023848,
006A00023849,
006A00023851,
006A00024134,
006A00024135,
006A00024244,
006A00024288,
006A00024290,
006A00024291,
006A00024298,
006A00024299,
006A00024300,
006A00024301,
006A00024302,
006A00024319,
006A00024320,
006A00024321,
006A00024322,
006A00024352,
006A00024353,
006A00024354,
006A00024355,
006A00024357,
006A00024358,
006A00024359,
006A00024361,
006A00024374,
006A00024375,
006A00024376,
006A00024377,
006A00024378,
006A00024379,
006A00024380,
006A00024382,
006A00024396,
006A00024397,
006A00024398,
006A00024581,
006A00024582,
006A00024586,
006A00024587,
006A00024588,
006A00024754,
006A00024762,
006A00024789,
006A00024790,
006A00024793,
006A00024913,
006A00024961,
006A00024970,
006A00024988,
006A00024990,
006A00025127,
006A00025166,
006A00025177,
006A00025183,
006A00025218,
006A00025219,
006A00025220,
006A00025247,
006A00025248,
006A00025253,
006A00025256,
006A00025262,
006A00025369,
006A00025911,
006A00025912,
006A00025966,
006A00025974,
006A00026061,
006A00026495,
006A00026555,
006A00026557,
006A00026561,
006A00026576,
006A00026666,
006A00026683,
006A00026690,
006A00026747,
006A00026774,
006A00026775,
006A00026776,
006A00026777,
006A00026778,
006A00026779,
006A00026780,
006A00026782,
006A00026783,
006A00026787,
006A00026788,
006A00026789,
006A00026797,
006A00026896,
006A00026897,
006A00026904,
006A00026936,
006A00026938,
006A00027270,
006A00027271,
006A00027274,
006A00027284,
006A00027285,
006A00027286,
006A00027291,
006A00027387,
006A00027413,
006A00027423,
006A00027448,
006A00027539,
006A00027681,
006A00027705,
006A00027708,
006A00027710,
006A00027711,
006A00027712,
006A00027714,
006A00027717,
006A00027734,
006A00027737,
006A00027747,
006A00027749,
006A00027751,
006A00027753,
006A00027754,
006A00027793,
006A00027794,
006A00027805,
006A00027806,
006A00027807,
006A00027809,
006A00027816,
006A00027825,
006A00027826,
006A00027837,
006A00027838,
006A00027840,
006A00027843,
006A00027847,
006A00027883,
006A00027884,
006A00027894,
006A00027897,
006A00027898,
006A00027901,
006A00027903,
006A00027904,
006A00027905,
006A00028233,
006A00028234,
006A00028235,
006A00028236,
006A00028237,
006A00028238,
006A00028239,
006A00028240,
006A00028241,
006A00028242,
006A00028243,
006A00028244,
006A00028245,
006A00028246,
006A00028247,
006A00028248,
006A00028249,
006A00028250,
006A00028251,
006A00028252,
006A00028253,
006A00028254,
006A00028255,
006A00028256,
006A00028257,
006A00028258,
006A00028259,
006A00028260,
006A00028261,
006A00028364,
006A00028401,
006A00028406,
006A00028409,
006A00028411,
006A00028413,
006A00028423,
006A00028424,
006A00028514,
006A00028516,
006A00028611,
006A00028612,
006A00028615,
006A00028619,
006A00028742,
006A00028828,
006A00028829,
006A00028858,
006A00028892,
006A00028989,
006A00028998,
006A00029092,
006A00029126,
006A00029135,
006A00029165,
006A00029247,
006A00029298,
006A00029310,
006A00029313,
006A00029316,
006A00029317,
006A00029318,
006A00029428,
006A00029431,
006A00029432,
006A00029433,
006A00029458,
006A00029459,
006A00029467,
006A00029507,
006A00029609,
006A00029874,
006A00030026,
006A00030027,
006A00030041,
006A00030048,
006A00030117,
006A00030121,
006A00030197,
006A00030284,
006A00030289,
006A00030548,
006A00030576,
006A00030577,
006A00030578,
006A00030584,
006A00030591,
006A00030718,
006A00030724,
006A00030774,
006A00030775,
006A00030776,
006A00030777,
006A00030783,
006A00030831,
006A00030832,
006A00030845,
006A00030854,
006A00030855,
006A00030907,
006A00030911,
006A00030912,
006A00030913,
006A00030914,
006A00030950,
006A00030951,
006A00030952,
006A00030953,
006A00030954,
006A00030955,
006A00030956,
006A00030962,
006A00030973,
006A00030974,
006A00031023,
006A00031082,
006A00031083,
006A00031084,
006A00031085,
006A00031086,
006A00031087,
006A00031107,
006A00031108,
006A00031146,
006A00031147,
006A00031153,
006A00031154,
006A00031155,
006A00031159,
006A00031176,
006A00031177,
006A00031178,
006A00031179,
006A00031180,
006A00031181,
006A00031182,
006A00031183,
006A00031184,
006A00031185,
006A00031186,
006A00031187,
006A00031188,
006A00031189,
006A00031190,
006A00031191,
006A00031192,
006A00031193,
006A00031196,
006A00031271,
006A00031341,
006A00031348,
006A00031350,
006A00031361,
006A00031362,
006A00031363,
006A00031371,
006A00031372,
006A00031373,
006A00031384,
006A00031386,
006A00031387,
006A00031388,
006A00031389,
006A00031390,
006A00031391,
006A00031392,
006A00031403,
006A00031404,
006A00031406,
006A00031407,
006A00031408,
006A00031409,
006A00031410,
006A00031411,
006A00031437,
006A00031439,
006A00031440,
006A00031441,
006A00031456,
006A00031457,
006A00031458,
006A00031459,
006A00031461,
006A00032181,
006A00032207,
006A00032311,
006A00032366,
006A00032392,
006A00032394,
006A00032490,
006A00032494,
006A00032495,
006A00032499,
006A00032500,
006A00032501,
006A00032502,
006A00032503,
006A00032504,
006A00032505,
006A00032506,
006A00032507,
006A00032508,
006A00032509,
006A00032510,
006A00032511,
006A00032512,
006A00032513,
006A00032515,
006A00032516,
006A00032602,
006A00032664,
006A00032757,
006A00032760,
006A00032888,
006A00032889,
006A00032890,
006A00032891,
006A00032892,
006A00032977,
006A00032978,
006A00033075,
006A00033079,
006A00033080,
006A00033097,
006A00033103,
006A00033520,
006A00033523,
006A00033525,
006A00033526,
006A00033528,
006A00033540,
006A00033543,
006A00033544,
006A00033545,
006A00033572,
006A00033573,
006A00033574,
006A00033575,
006A00033577,
006A00033578,
006A00033579,
006A00033580,
006A00033589,
006A00033592,
006A00033593,
006A00033594,
006A00033595,
006A00033596,
006A00033597,
006A00033598,
006A00033610,
006A00033611,
006A00033612,
006A00033620,
006A00033621,
006A00033622,
006A00033633,
006A00033637,
006A00033642,
006A00033768,
006A00033786,
006A00033789,
006A00033790,
006A00033793,
006A00033797,
006A00033799,
006A00033820,
006A00033821,
006A00033833,
006A00033834,
006A00033835,
006A00033836,
006A00033837,
006A00033838,
006A00033872,
006A00033896,
006A00033898,
006A00033899,
006A00033900,
006A00033902,
006A00033944,
006A00033969,
006A00034148,
006A00034149,
006A00034152,
006A00034153,
006A00034154,
006A00034155,
006A00034156,
006A00034157,
006A00034168,
006A00034169,
006A00034171,
006A00034178,
006A00034179,
006A00034181,
006A00034192,
006A00034197,
006A00034202,
006A00034281,
006A00034282,
006A00034283,
006A00034287,
006A00034291,
006A00034294,
006A00034305,
006A00034307,
006A00034308,
006A00034309,
006A00034310,
006A00034312,
006A00034313,
006A00034314,
006A00034341,
006A00034342,
006A00034343,
006A00034347,
006A00034358,
006A00034360,
006A00034362,
006A00034364,
006A00034365,
006A00034681,
006A00034739,
006A00034760,
006A00034861,
006A00034921,
006A00034967,
006A00035002,
006A00035003,
006A00035004,
006A00035272,
006A00035289,
006A00035439,
006A00035444,
006A00035460,
006A00035567,
006A00036198,
006A00036199,
006A00036200,
006A00036201,
006A00036202,
006A00036203,
006A00036264,
006A00036265,
006A00036352,
006A00036408,
006A00036412,
006A00036491,
006A00036498,
006A00036523,
006A00036952,
006A00036959,
006A00037100,
006A00037101,
006A00037204,
006A00037212,
006A00037214,
006A00037218,
006A00037429,
006A00037519,
006A00037552,
006A00037553,
006A00037578,
006A00037845,
006A00037919,
006A00037982,
006A00038038,
006A00038042,
006A00038052,
006A00038119,
006A00038177,
006A00038178,
006A00038196,
006A00038243,
006A00038533,
006A00038544,
006A00038607,
006A00038616,
006A00038782,
006A00038905,
006A00038906,
006A00038907,
006A00038908,
006A00038909,
006A00038910,
006A00038911,
006A00038912,
006A00038914,
006A00038915,
006A00038916,
006A00038917,
006A00038918,
006A00038919,
006A00038920,
006A00038921,
006A00038922,
006A00038923,
006A00038924,
006A00038925,
006A00038926,
006A00038927,
006A00038928,
006A00038929,
006A00038930,
006A00038931,
006A00038932,
006A00038933,
006A00038934,
006A00038935,
006A00038936,
006A00038937,
006A00038938,
006A00038939,
006A00038940,
006A00038941,
006A00038942,
006A00038943,
006A00038944,
006A00038945,
006A00038946,
006A00038947,
006A00038948,
006A00038949,
006A00038950,
006A00038951,
006A00038952,
006A00038953,
006A00038954,
006A00038955,
006A00038956,
006A00038957,
006A00038958,
006A00038959,
006A00038960,
006A00038961,
006A00038962,
006A00038963,
006A00038964,
006A00038965,
006A00038966,
006A00038967,
006A00038968,
006A00038969,
006A00038970,
006A00039170,
006A00039214,
006A00039238,
006A00039288,
006A00039295,
006A00039299,
006A00039303,
006A00039312,
006A00039313,
006A00039363,
006A00039364,
006A00039365,
006A00039367,
006A00039423,
006A00039449,
006A00039454,
006A00039455,
006A00039456,
006A00039457,
006A00039458,
006A00039459,
006A00039460,
006A00039461,
006A00039522,
006A00039523,
006A00039560,
006A00039561,
006A00039602,
006A00039728,
006A00039734,
006A00039798,
006A00039800,
006A00039806,
006A00039908,
006A00040014,
006A00040058,
006A00040064,
006A00040065,
006A00040108,
006A00040189,
006A00040198,
006A00040224,
006A00040278,
006A00040286,
006A00040296,
006A00040297,
006A00040370,
006A00040381,
006A00040383,
006A00040400,
006A00040401,
006A00040521,
006A00040549,
006A00040555,
006A00040572,
006A00040585,
006A00040605,
006A00040608,
006A00040609,
006A00040614,
006A00040621,
006A00040626,
006A00040700,
006A00040738,
006A00040743,
006A00040744,
006A00040745,
006A00040746,
006A00040752,
006A00040758,
006A00040764,
006A00040765,
006A00040766,
006A00040768,
006A00040770,
006A00040774,
006A00040798,
006A00040832,
006A00040833,
006A00040838,
006A00040843,
006A00040853,
006A00040854,
006A00040855,
006A00040857,
006A00040862,
006A00040870,
006A00040873,
006A00040876,
006A00040880,
006A00040881,
006A00040882,
006A00040906,
006A00040981,
006A00040982,
006A00041037,
006A00041051,
006A00041052,
006A00041053,
006A00041059,
006A00041064,
006A00041065,
006A00041066,
006A00041067,
006A00041068,
006A00041078,
006A00041079,
006A00041081,
006A00041082,
006A00041085,
006A00041087,
006A00041088,
006A00041089,
006A00041090,
006A00041091,
006A00041094,
006A00041097,
006A00041099,
006A00041100,
006A00041103,
006A00041104,
006A00041106,
006A00041113,
006A00041120,
006A00041123,
006A00041131,
006A00041135,
006A00041136,
006A00041200,
006A00041201,
006A00041205,
006A00041225,
006A00041229,
006A00041235,
006A00041237,
006A00041239,
006A00041240,
006A00041241,
006A00041243,
006A00041246,
006A00041247,
006A00041269,
006A00041270,
006A00041271,
006A00041272,
006A00041273,
006A00041274,
006A00041275,
006A00041276,
006A00041285,
006A00041286,
006A00041291,
006A00041292,
006A00041293,
006A00041294,
006A00041296,
006A00041298,
006A00041299,
006A00041300,
006A00041301,
006A00041302,
006A00041303,
006A00041304,
006A00041305,
006A00041306,
006A00041307,
006A00041308,
006A00041309,
006A00041311,
006A00041312,
006A00041314,
006A00041315,
006A00041321,
006A00041334,
006A00041341,
006A00041349,
006A00041352,
006A00041354,
006A00041355,
006A00041367,
006A00041372,
006A00041373,
006A00041374,
006A00041375,
006A00041376,
006A00041377,
006A00041378,
006A00041399,
006A00041400,
006A00041403,
006A00041406,
006A00041407,
006A00041408,
006A00041409,
006A00041415,
006A00041416,
006A00041417,
006A00041418,
006A00041419,
006A00041423,
006A00041424,
006A00041425,
006A00041426,
006A00041428,
006A00041429,
006A00041431,
006A00041439,
006A00041446,
006A00041459,
006A00041470,
006A00041474,
006A00041475,
006A00041482,
006A00041485,
006A00041493,
006A00041495,
006A00041502,
006A00041506,
006A00041507,
006A00041508,
006A00041509,
006A00041513,
006A00041514,
006A00041515,
006A00041520,
006A00041521,
006A00041522,
006A00041523,
006A00041524,
006A00041525,
006A00041527,
006A00041530,
006A00041536,
006A00041539,
006A00041540,
006A00041544,
006A00041546,
006A00041547,
006A00041549,
006A00041553,
006A00041555,
006A00041559,
006A00041561,
006A00041566,
006A00041571,
006A00041580,
006A00041581,
006A00041582,
006A00041583,
006A00041587,
006A00041588,
006A00041589,
006A00041594,
006A00041595,
006A00041598,
006A00041600,
006A00041601,
006A00041602,
006A00041603,
006A00041604,
006A00041605,
006A00041606,
006A00041607,
006A00041608,
006A00041612,
006A00041613,
006A00041614,
006A00041615,
006A00041616,
006A00041617,
006A00041620,
006A00041621,
006A00041622,
006A00041623,
006A00041624,
006A00041625,
006A00041626,
006A00041627,
006A00041628,
006A00041629,
006A00041630,
006A00041631,
006A00041632,
006A00041633,
006A00041634,
006A00041635,
006A00041636,
006A00041637,
006A00041638,
006A00041639,
006A00041640,
006A00041641,
006A00041642,
006A00041643,
006A00041644,
006A00041645,
006A00041646,
006A00041647,
006A00041648,
006A00041649,
006A00041650,
006A00041651,
006A00041652,
006A00041657,
006A00041658,
006A00041659,
006A00041660,
006A00041661,
006A00041662,
006A00041663,
006A00041664,
006A00041665,
006A00041666,
006A00041667,
006A00041668,
006A00041669,
006A00041673,
006A00041674,
006A00041676,
006A00041677,
006A00041678,
006A00041679,
006A00041681,
006A00041686,
006A00041693,
006A00041695,
006A00041696,
006A00041782,
006A00041810,
006A00041811,
006A00041813,
006A00041815,
006A00041816,
006A00041821,
006A00041836,
006A00041839,
006A00041840,
006A00041842,
006A00041844,
006A00041845,
006A00041847,
006A00041851,
006A00041854,
006A00041857,
006A00041861,
006A00041862,
006A00041863,
006A00041865,
006A00041870,
006A00041871,
006A00041881,
006A00041882,
006A00041883,
006A00041884,
006A00041885,
006A00041886,
006A00041887,
006A00041888,
006A00041889,
006A00041896,
006A00041899,
006A00041901,
006A00041902,
006A00041905,
006A00041906,
006A00041917,
006A00041932,
006A00041947,
006A00041988,
006A00041990,
006A00041991,
006A00041992,
006A00041993,
006A00041994,
006A00041996,
006A00042026,
006A00042036,
006A00042037,
006A00042044,
006A00042045,
006A00042048,
006A00042049,
006A00042059,
006A00042065,
006A00042066,
006A00042072,
006A00042076,
006A00042077,
006A00042078,
006A00042088,
006A00042098,
006A00042120,
006A00042334,
006A00042584,
006A00042616,
006B00000980,
006B00000984,
006B00000991,
006B00000995,
006B00000996,
006B00000997,
006B00000999,
006B00001004,
006B00001005,
006B00001010".Replace("\r\n", "").Split(',');
            foreach (var item in wolist)
            {
                var tecores = JuniperBase.TecoSapWo("FJZJNP", item);
                //var tecores = JuniperBase.TecoSapWo("VNJUNIPER", item);
                if (!tecores.issuccess)
                    MesLog.Info($@"teco wo fail :{item} {tecores.msg}");
                else
                    MesLog.Info($@"teco wo ok:{item} {tecores.msg}");
            }
        }

        void jnpbugprocess_fvn()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNJNPODB", false))
            {
                using (var b2bdb = MESDBHelper.OleExec.GetSqlSugarClient("LHB2BODB", false, DbType.SqlServer))
                {
                    var targetlist = db.Queryable<O_I137_ITEM>().Where(t => fvnlist.Contains(SqlSugar.SqlFunc.MergeString(t.TRANID,t.PONUMBER ,t.ITEM))).ToList();
                    foreach (var item in targetlist)
                    {
                        var res = db.Ado.UseTran(()=> {
                            var b2bdetails = b2bdb.Queryable<B2B_I137_D>().Where(t => t.TRANID == item.TRANID && t.ITEM == item.ITEM).ToList();
                            foreach (var detail in b2bdetails)
                            {
                                if (db.Queryable<O_I137_DETAIL>().Where(t => t.F_ID == detail.F_ID).Any())
                                    continue;
                                var detailmain = ObjectDataHelper.Mapper<I137_D, B2B_I137_D>(detail);
                                detailmain.ID = MesDbBase.GetNewID<O_I137_DETAIL>(db, Customer.JUNIPER.ExtValue());
                                detailmain.CREATETIME = DateTime.Now;
                                db.Insertable(detailmain).ExecuteCommand();
                            }

                            var itemlist = db.Queryable<O_I137_ITEM>().Where(t => SqlFunc.MergeString(t.PONUMBER, t.ITEM) == SqlFunc.MergeString(item.PONUMBER, item.ITEM))
                            .OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList();
                            var status = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID).Where((m, s) => m.UPOID == SqlFunc.MergeString(item.PONUMBER, item.ITEM) && s.VALIDFLAG==MesBool.Yes.ExtValue()).Select((m, s) => new { m, s }).ToList().FirstOrDefault();

                            if (itemlist.FirstOrDefault().ID == item.ID )
                            {
                                //var status = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID).Where((m, s) => m.UPOID == SqlFunc.MergeString(item.PONUMBER, item.ITEM)).Select((m, s) => new { m, s }).ToList().FirstOrDefault();
                                if ("8,9".Split(',').Contains(status.s.STATUSID))
                                {
                                    //teco wo
                                    var tecores = JuniperBase.TecoSapWo("VNJUNIPER", status.m.PREWO);
                                    if (!tecores.issuccess)
                                        throw new Exception(tecores.msg);
                                }
                                status.s.STATUSID = ENUM_O_PO_STATUS.ValidationI137.ExtValue();
                                db.Updateable(status.s).ExecuteCommand();                                
                                if (itemlist.Count > 1)
                                {
                                    itemlist.FirstOrDefault().MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                    db.Updateable(itemlist.FirstOrDefault()).ExecuteCommand();
                                }
                                else
                                {
                                    itemlist.FirstOrDefault().MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                    db.Updateable(itemlist.FirstOrDefault()).ExecuteCommand();
                                    MesLog.Info($@"first:{item.TRANID} {item.PONUMBER} {item.ITEM}");
                                }

                                MesLog.Info($@"process :{item.TRANID} {item.PONUMBER} {item.ITEM}");
                            }

                        });
                        if (!res.IsSuccess)
                            MesLog.Info($@"err:{item.TRANID} {item.PONUMBER} {item.ITEM} {res.ErrorMessage}");
                    }
                }
            }
        }

        void jnpbugprocess_fjz()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                using (var b2bdb = MESDBHelper.OleExec.GetSqlSugarClient("LHB2BODB", false, DbType.SqlServer))
                {
                    var targetlist = db.Queryable<O_I137_ITEM>().Where(t => fjzlist.Contains(SqlSugar.SqlFunc.MergeString(t.TRANID, t.PONUMBER, t.ITEM))).ToList();
                    foreach (var item in targetlist)
                    {
                        var res = db.Ado.UseTran(() => {
                            var b2bdetails = b2bdb.Queryable<B2B_I137_D>().Where(t => t.TRANID == item.TRANID && t.ITEM == item.ITEM).ToList();
                            foreach (var detail in b2bdetails)
                            {
                                if (db.Queryable<O_I137_DETAIL>().Where(t => t.F_ID == detail.F_ID).Any())
                                    continue;
                                var detailmain = ObjectDataHelper.Mapper<I137_D, B2B_I137_D>(detail);
                                detailmain.ID = MesDbBase.GetNewID<O_I137_DETAIL>(db, Customer.JUNIPER.ExtValue());
                                detailmain.CREATETIME = DateTime.Now;
                                db.Insertable(detailmain).ExecuteCommand();
                            }

                            var itemlist = db.Queryable<O_I137_ITEM>().Where(t => SqlFunc.MergeString(t.PONUMBER, t.ITEM) == SqlFunc.MergeString(item.PONUMBER, item.ITEM))
                            .OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList();
                            var status = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID).Where((m, s) => m.UPOID == SqlFunc.MergeString(item.PONUMBER, item.ITEM) && s.VALIDFLAG == MesBool.Yes.ExtValue()).Select((m, s) => new { m, s }).ToList().FirstOrDefault();

                            if (itemlist.FirstOrDefault().ID == item.ID)
                            {
                                //var status = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID).Where((m, s) => m.UPOID == SqlFunc.MergeString(item.PONUMBER, item.ITEM)).Select((m, s) => new { m, s }).ToList().FirstOrDefault();
                                if ("8,9,13".Split(',').Contains(status.s.STATUSID))
                                {
                                    //teco wo
                                    var tecores = JuniperBase.TecoSapWo("FJZJNP", status.m.PREWO);
                                    if (!tecores.issuccess)
                                        throw new Exception(tecores.msg);
                                    MesLog.Info($@"teco wo :{item.TRANID} {item.PONUMBER} {item.ITEM}");
                                }
                                status.s.STATUSID = ENUM_O_PO_STATUS.ValidationI137.ExtValue();
                                db.Updateable(status.s).ExecuteCommand();
                                if (itemlist.Count > 1)
                                {
                                    itemlist.FirstOrDefault().MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                    db.Updateable(itemlist.FirstOrDefault()).ExecuteCommand();
                                }
                                else
                                {
                                    itemlist.FirstOrDefault().MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                    db.Updateable(itemlist.FirstOrDefault()).ExecuteCommand();
                                    MesLog.Info($@"first:{item.TRANID} {item.PONUMBER} {item.ITEM}");
                                }

                                MesLog.Info($@"process :{item.TRANID} {item.PONUMBER} {item.ITEM}");
                            }

                        });
                        if (!res.IsSuccess)
                            MesLog.Info($@"err:{item.TRANID} {item.PONUMBER} {item.ITEM} {res.ErrorMessage}");
                    }
                }
            }
        }

        void bonpaile()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var res = db.Queryable<R_NORMAL_BONEPILE>().Where(t => t.CLOSED_FLAG == "0").ToList();
                foreach (var item in res)
                {
                    var snobj = db.Queryable<R_SN>().Where(t => t.SN == item.SN && t.VALID_FLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                    var sndetailstation = db.Queryable<R_SN_STATION_DETAIL>().Any(t => t.SN == item.SN && t.STATION_NAME == item.CURRENT_STATION && t.EDIT_TIME > item.FAIL_DATE);
                    if (snobj == null || snobj.COMPLETED_FLAG == MesBool.Yes.ExtValue() || snobj.SHIPPED_FLAG == MesBool.Yes.ExtValue() || sndetailstation)
                    {
                        item.CLOSED_FLAG = MesBool.Yes.ExtValue();
                        item.CLOSED_REASON = item.CURRENT_STATION;
                        item.CLOSED_BY = "S";
                        db.Updateable(item).ExecuteCommand();
                    }
                }
            }

        }

        void testsqlsugarbug()
        {
            using (var testdb = MESDBHelper.OleExec.GetSqlSugarClient("MESTESTDB", false))
            {
                using (var vnjnpdb = MESDBHelper.OleExec.GetSqlSugarClient("VNJNPODB", false))
                {

                }
            }
        }

        void fsjtest()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false, DbType.SqlServer))
            {
                var fileHelp = new FileHelp(filepath, null, new List<string> { ".csv" });
                var files = fileHelp.GetAllFiles();
                foreach (var item in files)
                {
                    try
                    {
                        if (item.IndexOf("FXSJ_P1rocess".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<process>(item);
                            var inlist = new List<nvd_pro>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_pro, process>(itema);
                                targetobj.FILENAME = item;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_pro>().Any(t => t.FILENAME == item))
                                db.Insertable(inlist).ExecuteCommand();

                        }
                        else if (item.IndexOf("FXSJ_M1aterial".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<Material>(item);
                            var inlist = new List<nvd_Material>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_Material, Material>(itema);
                                targetobj.FILENAME = item;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_Material>().Any(t => t.FILENAME == item))
                                db.Insertable(inlist).ExecuteCommand();
                        }
                        else if (item.IndexOf("FXSJ_Ship".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<ship>(item);
                            var inlist = new List<nvd_ship>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_ship, ship>(itema);
                                targetobj.FILENAME = item;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_ship>().Any(t => t.FILENAME == item))
                                db.Insertable(inlist).ExecuteCommand();
                        }
                    }
                    catch (Exception e)
                    {
                        //throw e;
                    }
                }
            }
        }

        void fsj()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false, DbType.SqlServer))
            {
                var fileHelp = new FileHelp(filepath, null, new List<string> { ".csv" });
                var files = fileHelp.GetAllFiles();
                foreach (var item in files)
                {
                    if (!db.Queryable<nvd_temp_job>().Any(t => t.FILENAME == item))
                        db.Insertable(new nvd_temp_job() { FILENAME = item, FLAG1 = "0", CREATETIME = DateTime.Now, TIME1 = DateTime.Now, TIME2 = DateTime.Now, TIME3 = DateTime.Now }).ExecuteCommand();
                }
                var waitlist = db.Queryable<nvd_temp_job>().Where(t => t.FLAG1 == "0").ToList();
                foreach (var item in waitlist)
                {
                    try
                    {
                        if (item.FILENAME.IndexOf("FXSJ_Process".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<process>(item.FILENAME);
                            var inlist = new List<nvd_pro>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_pro, process>(itema);
                                targetobj.FILENAME = item.FILENAME;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_pro>().Any(t => t.FILENAME == item.FILENAME))
                                db.Insertable(inlist).ExecuteCommand();
                            item.FLAG1 = "1";
                            item.TIME1 = DateTime.Now;
                            db.Updateable(item).ExecuteCommand();
                        }
                        else if (item.FILENAME.IndexOf("FXSJ_Material".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<Material>(item.FILENAME);
                            var inlist = new List<nvd_Material>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_Material, Material>(itema);
                                targetobj.FILENAME = item.FILENAME;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_Material>().Any(t => t.FILENAME == item.FILENAME))
                                db.Insertable(inlist).ExecuteCommand();
                            item.FLAG1 = "1";
                            item.TIME1 = DateTime.Now;
                            db.Updateable(item).ExecuteCommand();
                        }
                        else if (item.FILENAME.IndexOf("FXSJ_Ship".ToUpper()) > -1)
                        {
                            var reslist = GetCsv<ship>(item.FILENAME);
                            var inlist = new List<nvd_ship>();
                            foreach (var itema in reslist)
                            {
                                var targetobj = ObjectDataHelper.Mapper<nvd_ship, ship>(itema);
                                targetobj.FILENAME = item.FILENAME;
                                targetobj.CREATETIME = DateTime.Now;
                                inlist.Add(targetobj);
                            }
                            if (!db.Queryable<nvd_ship>().Any(t => t.FILENAME == item.FILENAME))
                                db.Insertable(inlist).ExecuteCommand();
                            item.FLAG1 = "1";
                            item.TIME1 = DateTime.Now;
                            db.Updateable(item).ExecuteCommand();
                        }
                    }
                    catch (Exception e)
                    {
                        //throw e;
                    }

                }


            };
        }

        void FsjErrorComponents()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FSJ_SUGAR_SQLSERVER", false, DbType.SqlServer))
            {
                var partlist = "7041529,8209685".Split(',');
                var target = db.Queryable<MFSYSCOMPONENT, MFWORKSTATUS>((c, m) => c.SYSSERIALNO == m.SYSSERIALNO)
                    .Where((c, m) => m.WORKORDERNO == "000021023280" && c.CATEGORYNAME.Contains(",") && partlist.Contains(c.CUSTPARTNO)).Select((c, m) => c).ToList();
                foreach (var item in target)
                {
                    var res = db.Ado.UseTran(() =>
                    {
                        var qty = Convert.ToInt32(item.QTY);
                        var locations = item.CATEGORYNAME.Split(',');

                        //db.Deleteable<MFSYSCOMPONENT>().Where(t => t.SYSSERIALNO == item.SYSSERIALNO && t.CUSTPARTNO == item.CUSTPARTNO).ExecuteCommand();
                        db.Ado.ExecuteCommand($@"delete from mfsyscomponent where sysserialno = '{item.SYSSERIALNO}' and custpartno = '{item.CUSTPARTNO}'");
                        for (int i = 0; i < qty; i++)
                        {
                            var newitem = item;
                            newitem.QTY = 1.00000;
                            newitem.CATEGORYNAME = locations[i];
                            newitem.PRODCATEGORYNAME = locations[i];
                            db.Insertable(newitem).ExecuteCommand();
                        }

                    });
                }
            }
        }
        
        void GetDcnAgingData()
        {
            var target = new List<DcnAgingData>();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_SUGAR_SQLSERVER", false, DbType.SqlServer))
            {
                //var res = db.Ado.GetDataTable($@"
                //                                select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                //                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                //                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                //                                from mfworkstatus a, mfworkorder b ,mfsysevent c 
                //                                where a.completed='0' 
                //                                and a.workorderno=b.workorderno 
                //                                and c.sysserialno=a.sysserialno 
                //                                AND b.closed=0
                //                                and c.eventname in ('SMTLOADING','SILOADING')
                //                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                //                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                //                                and a.factoryid in ('NBEA')
                //                                AND A.workorderno LIKE '002%'
                //                                AND A.sysserialno NOT LIKE '~%'
                //                                order by REPAIRHELD;
                //                                ");
                var res = db.Ado.GetDataTable($@"
                                                    select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,(select * from (
                                                select *,ROW_NUMBER() over(partition by sysserialno,eventname order by scandatetime desc) as sortnum from mfsysevent where eventname in ('SMTLOADING','SILOADING')   ) cc
                                                where sortnum=1) c 
                                                                                                where a.completed='0' 
                                                                                                and a.workorderno=b.workorderno 
                                                                                                and c.sysserialno=a.sysserialno 
                                                                                                AND b.closed=0
                                                                                               -- and c.eventname in ('SMTLOADING','SILOADING')
                                                                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                                                                and a.factoryid in ('NBEA')
                                                                                                AND A.workorderno LIKE '002%'
                                                                                                AND A.sysserialno NOT LIKE '~%'
                                                                                                order by REPAIRHELD;
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_ORACLE_SQLSERVER", false, DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                                               select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,(select * from (
                                                select *,ROW_NUMBER() over(partition by sysserialno,eventname order by scandatetime desc) as sortnum from mfsysevent where eventname in ('SMTLOADING','SILOADING')   ) cc
                                                where sortnum=1) c 
                                                                                                where a.completed='0' 
                                                                                                and a.workorderno=b.workorderno 
	                                                                                            AND b.closed=0
                                                                                                and c.sysserialno=a.sysserialno 
                                                                                               -- and c.eventname in ('SMTLOADING','SILOADING')
                                                                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                                                                and a.factoryid in ('NOEA')
                                                                                                AND b.workorderno NOT LIKE '%999%'
                                                                                                AND A.sysserialno NOT LIKE '~%'
                                                                                                order by AGING
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                              select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   c.plant,
                                                    to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               and c.closed_flag = '0'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VERTIVODB", false))
            {
                var res = db.Ado.GetDataTable($@"                                            
                                            select *
                                              from (select distinct decode(B.current_station,
                                                                           'SMTLOADING',
                                                                           'SMT-PCBA',
                                                                           'SILOADING',
                                                                           'SI-SYSTEM') AS WorkRouteType,
                                                                    c.plant,
                                                                    to_char(A.SN) as SN,
                                                                    A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                                    A.SKUNO,
                                                                    A.WORKORDERNO,
                                                                    A.CURRENT_STATION,
                                                                    A.NEXT_STATION,
                                                                    TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                                      from r_sn a, r_sn_station_detail b, r_wo_base c
                                                     where a.valid_flag = '1'
                                                       and a.completed_flag = '0'
                                                       and A.sn not like '*%'
                                                       and A.sn not like '#%'
                                                       and A.sn not like 'del%'
                                                       and c.closed_flag = '0'
                                                       and A.workorderno not like '%TEST%'
                                                       AND A.SN NOT LIKE '%TEST%'
                                                       AND A.workorderno LIKE '002%'
                                                       AND WO_TYPE = 'REGULAR'
                                                       and a.sn = b.sn
                                                       and b.current_station like '%LOADING'
                                                       and a.workorderno = c.workorderno
                                                       and a.current_station not in ('STOCKIN')
                                                       and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                                       and not exists (select 1
                                                              from r_sn_lock l
                                                             where l.lock_status = '1'
                                                               and l.sn = a.sn
                                                               and l.lock_reason like '%LLT%')
                                                    --order by a.next_station
                                                    union all
                                                    select distinct decode(B.current_station,
                                                                           'SMTLOADING',
                                                                           'SMT-PCBA',
                                                                           'SILOADING',
                                                                           'SI-SYSTEM') AS WorkRouteType,
                                                                    c.plant,
                                                                    to_char(A.SN) as SN,
                                                                    A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                                    A.SKUNO,
                                                                    A.WORKORDERNO,
                                                                    A.CURRENT_STATION,
                                                                    A.NEXT_STATION,
                                                                    TO_CHAR(ROUND(TO_NUMBER(sysdate - bc.edit_time))) as aging
                                                      from r_sn a,
                                                           (SELECT *
                                                              FROM (SELECT A.*,
                                                                           RANK() OVER(PARTITION BY SN ORDER BY EDIT_TIME DESC) NUMT
                                                                      FROM r_sn_station_detail A
                                                                     WHERE CURRENT_STATION = 'REWORK') BB
                                                             WHERE BB.NUMT = 1) bc,
                                                           r_sn_station_detail B,
                                                           r_wo_base c
                                                     where a.valid_flag = '1'
                                                       and a.completed_flag = '0'
                                                       and A.sn not like '*%'
                                                       and A.sn not like '#%'
                                                       and A.sn not like 'del%'
                                                       and c.closed_flag = '0'
                                                       and A.workorderno not like '%TEST%'
                                                       AND A.SN NOT LIKE '%TEST%'
                                                       AND A.workorderno LIKE '002%'
                                                       AND WO_TYPE = 'REWORK'
                                                       and a.sn = b.sn
                                                       and b.current_station like '%LOADING'
                                                       and a.workorderno = c.workorderno
                                                       and a.current_station not in ('STOCKIN')
                                                       and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                                       and not exists (select 1
                                                              from r_sn_lock l
                                                             where l.lock_status = '1'
                                                               and l.sn = a.sn
                                                               and l.lock_reason like '%LLT%')
                                                       and b.sn = bc.sn)
                                             order by next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FVNJNPODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                             select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   'VJGS' as plant,
                                                to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and c.closed_flag = '0'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            var key = DateTime.Now.ToString("yyyyMMddhhmmss");
            string filename = $@"C:\\Users\\G6001953.NN\\Desktop\\DCNAGING\\{key}.csv";
            //scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
            ExcelHelp.ExportCsv(target, filename);
        }

        void GetTempdata()
        {
            using (var aphisdb = MESDBHelper.OleExec.GetSqlSugarClient("DCNAPHIS", false))
            {
                using (var apdb = MESDBHelper.OleExec.GetSqlSugarClient("DCNAP", false))
                {
                    using (var sfcdb = MESDBHelper.OleExec.GetSqlSugarClient("DCN_SQLSERVER", false, DbType.SqlServer))
                    {
                        using (var sfchisdb = MESDBHelper.OleExec.GetSqlSugarClient("DCNHIS_SQLSERVER", false, DbType.SqlServer))
                        {
                            var pabasns = new List<aptemp00001>();
                            var pcbasnhis = aphisdb.Ado.GetDataTable($@"SELECT distinct a.p_sn,c.tr_sn,c.mfr_kp_no,c.date_code,c.lot_code
                            FROM mes4.r_tr_product_detail a, MES4.R_TR_CODE_DETAIL_2019 b,mes4.r_tr_sn c WHERE b.KP_NO ='B101.00326.005' and a.tr_code=b.tr_code  and b.tr_sn=c.tr_sn");
                            pabasns.AddRange(ObjectDataHelper.FromTable<aptemp00001>(pcbasnhis));
                            var pcbasnc = apdb.Ado.GetDataTable($@"SELECT distinct a.p_sn,c.tr_sn,c.mfr_kp_no,c.date_code,c.lot_code
                            FROM mes4.r_tr_product_detail a, MES4.R_TR_CODE_DETAIL b,mes4.r_tr_sn c WHERE b.KP_NO ='B101.00326.005' and a.tr_code=b.tr_code  and b.tr_sn=c.tr_sn");
                            pabasns.AddRange(ObjectDataHelper.FromTable<aptemp00001>(pcbasnc));
                            foreach (var pcbasn in pabasns)
                            {

                            }
                        }
                    }
                }
            }
        }

        void GetDcnWoAgingDataByBoss()
        {
            var target = new List<DcnAgingData>();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_SUGAR_SQLSERVER", false, DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                                                select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,mfsysevent c 
                                                where a.completed='0' 
                                                and a.workorderno=b.workorderno 
                                                and c.sysserialno=a.sysserialno 
                                                AND b.closed=0
                                                and c.eventname in ('SMTLOADING','SILOADING')
                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                and a.factoryid in ('NBEA')
                                                AND A.workorderno LIKE '002%'
                                                AND A.sysserialno NOT LIKE '~%'
                                                order by REPAIRHELD;
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_ORACLE_SQLSERVER", false, DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                                               select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,mfsysevent c 
                                                where a.completed='0' 
                                                and a.workorderno=b.workorderno 
	                                            AND b.closed=0
                                                and c.sysserialno=a.sysserialno 
                                                and c.eventname in ('SMTLOADING','SILOADING')
                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                and a.factoryid in ('NOEA')
                                                AND b.workorderno NOT LIKE '%999%'
                                                AND A.sysserialno NOT LIKE '~%'
                                                order by REPAIRHELD;
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                              select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   c.plant,
                                                    to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               and c.closed_flag = '0'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VERTIVODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                             select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   c.plant,
                                                to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and c.closed_flag = '0'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            var key = DateTime.Now.ToString("yyyyMMddhhmmss");
            string filename = $@"C:\\Users\\G6001953\\Desktop\\DCNAGING\\{key}.csv";
            //scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
            ExcelHelp.ExportCsv(target, filename);
        }

        void testcsvnum()
        {
            var target = new List<DcnAgingData>();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VERTIVODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                             select decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   c.plant,
                                                to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and c.closed_flag = '0'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            var key = DateTime.Now.ToString("yyyyMMddhhmmss");
            string filename = $@"C:\\Users\\G6001953\\Desktop\\DCNAGING\\{key}.csv";
            var a = target.FindAll(t => t.WORKORDERNO == "002329000733");
            scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, a);
            //ExcelHelp.ExportCsv(target, filename);
        }

        void scfile(string path,string filename,List<DcnAgingData> target)
        {
            //生成目录
            //创建文件夹
            if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(path);
            }

            // 判断文件是否存在，不存在则创建，否则读取值显示到txt文档
            if (!System.IO.File.Exists(path + "/" + filename + ".csv"))
            {
                FileStream fs1 = new FileStream(path + "/" + filename + ".csv", FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                foreach (var item in target)
                {
                    var log = $@"{item.WorkRouteType.Trim()},{item.PLANT.Trim()},{item.SN.Trim()},{item.REPAIRHELD.Trim()},{item.SKUNO.Trim()},{item.WORKORDERNO.Trim()},{item.CURRENT_STATION.Trim()},{item.NEXT_STATION.Trim()},{item.AGING.Trim()}";
                    sw.WriteLine(log);//开始写入值
                }                
                sw.Close();
                fs1.Close();
            }

        }

        void clearJnpRmq()
        {
            //var dbstr = "FJZODB";
            var dbstr = "VNJNPODB";
            using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(dbstr, false))
            {
                var wait = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.ORDERTYPE == null).ToList();
                var res = mesdb.Ado.UseTran(()=> {
                    foreach (var item in wait)
                    {
                        var head = mesdb.Queryable<O_ORDER_MAIN, I137_H, I137_I>((o, h, i) => o.ITEMID == i.ID && i.TRANID == h.TRANID).Where((o, h, i) => o.ID == item.ID).Select((o, h, i) => h).ToList().FirstOrDefault();
                        item.ORDERTYPE = head.PODOCTYPE;
                        item.RMQPONO = head.PODOCTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() ? "" : head.RMQPONUMBER;
                        if (head.PONUMBER != head.RMQPONUMBER && head.RMQPONUMBER != "NA" && head.PODOCTYPE != ENUM_I137_PoDocType.ZRMQ.ToString())
                        {
                            var rmqobj = mesdb.Queryable<O_I137_HEAD>().Where(t => t.PONUMBER == head.RMQPONUMBER && t.PODOCTYPE == ENUM_I137_PoDocType.ZRMQ.ToString()).ToList().FirstOrDefault();
                            if (rmqobj != null)
                                mesdb.Updateable<O_ORDER_MAIN>().SetColumns(t => new O_ORDER_MAIN() { RMQPONO = head.PONUMBER }).Where(t => t.PONO == rmqobj.PONUMBER).ExecuteCommand();
                        }
                        mesdb.Updateable(item).ExecuteCommand();
                    }
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
            }
        }

        void testdbupdateforceplant()
        {
            //var dbstr = "FJZODB";
            var dbstr = "TESTODB";
            var defaulplant = "VUEA";
            using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(dbstr, false))
            {
                var wait = mesdb.Queryable<R_PRE_WO_HEAD>().Where(t => t.PLANT == null).ToList();
                var plantskulist = mesdb.Queryable<R_SKU_PLANT>().ToList();
                var res = mesdb.Ado.UseTran(() => {
                    foreach (var item in wait)
                    {
                        var plantobj = plantskulist.Find(t => t.FOXCONN == item.PID);
                        var agile = mesdb.Queryable<O_ORDER_MAIN>().Where(t=>t.PONO == item.PONO && t.POLINE == item.POLINE).ToList().FirstOrDefault();
                        item.PLANT = plantobj == null ? defaulplant : plantobj.PLANTCODE;
                        item.CUSTPN = agile.CUSTPID;
                        mesdb.Updateable(item).ExecuteCommand();
                    }
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
            }
        }

        void sys137bug()
        {
            using (var b2bdb = MESDBHelper.OleExec.GetSqlSugarClient("LHB2BODB", false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_R_I137>()
                     .Where(t => t.SALESORDERREFERENCEID == JuniperB2BPlantCode.FJZ.ExtValue())
                     .OrderBy(t => t.REPLENISHMENTORDERID, OrderByType.Asc).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Asc).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<O_I137>().Where(t => MESDBHelper.IMesDbEx.OracleContain(t.F_ID.ToString(), filterIDs)).ToList();
                        foreach (var item in waitsynlist)
                        {
                            if (exsist.Any(t => t.F_ID.ToString() == item.F_ID))
                                continue;
                            try
                            {
                                var targetobj = ObjectDataHelper.Mapper<O_I137, B2B_R_I137>(item);
                                targetobj.ID = MesDbBase.GetNewID<O_I137>(mesdb, Customer.JUNIPER.ExtValue());
                                targetobj.CREATETIME = DateTime.Now;
                                targetobj.MFLAG = MesBool.No.ExtName();
                                mesdb.Insertable(targetobj).ExecuteCommand();
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }

            }
        }

        void sysfile()
        {
            var strConn = "";
            var sdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConn, //必填
                DbType = SqlSugar.DbType.Sqlite, //必填
                IsAutoCloseConnection = false, //默认false
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = false
            }); //默认SystemTable


            using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(dbstr, false))
            {

            }
        }

        void csvtest()
        {
            var entity = new { Name = "item", ID = 0, GuidType = Guid.Empty };
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("GuidType", typeof(Guid));
            for (int i = 0; i < 10; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["Name"] = "AA" + i;
                dr["ID"] = i;
                if (i % 2 == 0)
                    dr["GuidType"] = Guid.Empty;
                else
                {
                    dr["GuidType"] = DBNull.Value;
                }
                dataTable.Rows.Add(dr);
            }
            IList list = ObjectDataHelper.ConverDatatableToList(entity.GetType(), dataTable);
            var res = ExcelHelp.ExportCsvToBase64String(list);
        }

        void csvtest2()
        {
            List<csvobj> csvOutput = new List<csvobj>();
            csvobj rows = new csvobj();
            rows.ID = 123;
            rows.Name = "李四";
            csvOutput.Add(rows);

            string filename = "C:\\Users\\G6001953\\Desktop\\111\\test.csv";
            ExcelHelp.ExportCsv(csvOutput, filename);
        }        

        void fsj_sysevent()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false, DbType.SqlServer))
            {
                var objs = db.Queryable<nvd_pro>().ToList();
                foreach (var item in objs)
                {
                    // var itemevent = db.Queryable<mfsysevent>().Where(t=>t.sysserialno == item.NVSN && t.eventname == item. && t.scandatetime == item.)
                }
            }
        }

        void test()
        {
            var filename = $@"C:\Users\G6001953\Desktop\20201030 updat\XXAT_MDSSTR_B_FXVN_BD_20201005000000---update.dat";
            using (var fs1 = new StreamReader(filename))
            {
                MesLog.Debug($@"file:{filename} reading start:");
                var strrow = string.Empty; ;
                while ((strrow = fs1.ReadLine()) != null)
                {
                    if (strrow.Split('|').Length != 16)
                        MesLog.Debug(strrow);
                }
                MesLog.Debug($@"file:{filename} read end:");
            }
        }

        void test1()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                using (var apdb = MESDBHelper.OleExec.GetSqlSugarClient("VNAPODB", false))
                {
                    var waitdosnkp = db.Queryable<R_SN_KP>().Where(t => t.MPN.Contains(",") && t.SCANTYPE == "AUTOAP")
                        .ToList();
                    foreach (var item in waitdosnkp)
                    {
                        //var s = item.VALUE.Split(',');
                        //if (s.Length == 2 && s[0] == s[1])
                        //{
                        //    item.VALUE = s[0];
                        //}

                        //var m = item.MPN.Split(',');
                        //if (m.Length == 2 && m[0] == m[1])
                        //{
                        //    item.MPN = m[0];
                        //}
                        var strap = string.Empty;
                        var prostr = string.Empty;

                        if (item.STATION == "SMT1")
                            prostr = "B";
                        else if (item.STATION == "SMT2")
                            prostr = "T";
                        if (item.STATION != "SMTLOADING")
                            strap = $@" select c.* from mes4.r_tr_product_detail a,mes4.r_tr_code_detail b ,mes4.r_tr_sn c  where a.p_sn='{item.SN}' and a.tr_code =b.tr_code and b.kp_no='{item.PARTNO}' and b.tr_sn=c.tr_sn and a.process_flag='{prostr}' ";
                        else
                            strap = $@" select c.* from mes4.r_tr_product_detail a,mes4.r_tr_code_detail b ,mes4.r_tr_sn c  where a.p_sn='{item.SN}' and a.tr_code =b.tr_code and b.kp_no='{item.PARTNO}' and b.tr_sn=c.tr_sn  ";

                        var apmpn = apdb.Ado.GetDataSetAll(strap).Tables[0];
                        if (apmpn.Rows.Count != 0)
                        {
                            item.MPN = apmpn.Rows[0]["mfr_kp_no"].ToString();
                            item.VALUE =
                                $@"{apmpn.Rows[0]["date_code"].ToString()}/{apmpn.Rows[0]["lot_code"].ToString()}";

                            db.Updateable(item).ExecuteCommand();
                        }
                    }
                }
            }
        }

        public void Test2()
        {
            string[] list_type = new string[] { "APTRSN", "AUTOAP" };
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                using (var apdb = MESDBHelper.OleExec.GetSqlSugarClient("VNAPODB", false))
                {
                    var waitdosnkp = db.Queryable<R_SN_KP>().Where(t => SqlFunc.ContainsArray(list_type, t.SCANTYPE) && SqlFunc.StartsWith(t.VALUE, "T2"))
                        .ToList();
                    foreach (var item in waitdosnkp)
                    {
                        var strap = string.Empty;
                        var prostr = string.Empty;
                        string tr_sn = item.VALUE;
                        strap = $@"select * from mes4.r_tr_sn where tr_sn='{tr_sn}'";

                        var apmpn = apdb.Ado.GetDataSetAll(strap).Tables[0];
                        if (apmpn.Rows.Count != 0)
                        {
                            item.MPN = apmpn.Rows[0]["mfr_kp_no"].ToString();
                            item.EXKEY1 = "APSN";
                            item.EXVALUE1 = tr_sn;
                            item.VALUE = apmpn.Rows[0]["DATE_CODE"].ToString() + "/" + apmpn.Rows[0]["LOT_CODE"].ToString();
                            db.Updateable(item).ExecuteCommand();
                        }
                    }
                }
            }
        }

        public void Test3()
        {
            string[] list_type = new string[] { "APTRSN", "AUTOAP" };
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                using (var apdb = MESDBHelper.OleExec.GetSqlSugarClient("VNAPODB", false))
                {
                    //select count(1) from r_sn_kp where scantype = 'PN' and substr(value,1,2) = 'T2' and VALID_FLAG = '1'
                    var waitdosnkp = db.Queryable<R_SN_KP>().Where(t => t.SCANTYPE == "PN" && SqlFunc.StartsWith(t.VALUE, "T2") && t.VALID_FLAG == 1).ToList();
                    foreach (var item in waitdosnkp)
                    {
                        var strap = string.Empty;
                        var prostr = string.Empty;
                        string tr_sn = item.VALUE;
                        strap = $@"select * from mes4.r_tr_sn where tr_sn='{tr_sn}'";

                        var apmpn = apdb.Ado.GetDataSetAll(strap).Tables[0];
                        if (apmpn.Rows.Count != 0)
                        {
                            item.MPN = apmpn.Rows[0]["mfr_kp_no"].ToString();
                            item.SCANTYPE = "APTRSN";
                            item.EXKEY1 = "APSN";
                            item.EXVALUE1 = tr_sn;
                            item.VALUE = apmpn.Rows[0]["DATE_CODE"].ToString() + "/" + apmpn.Rows[0]["LOT_CODE"].ToString();
                            db.Updateable(item).ExecuteCommand();
                        }
                    }
                }
            }
        }

        public void Test4()
        {
            string[] list_type = new string[] { "APTRSN", "AUTOAP" };
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                using (var apdb = MESDBHelper.OleExec.GetSqlSugarClient("VNAPODB", false))
                {
                    //select count(1) from r_sn_kp where scantype = 'PN' and substr(value,1,2) = 'T2' and VALID_FLAG = '1'
                    var waitdosnkp = db.Queryable<R_SN_KP>().Where(t => t.SCANTYPE == "AUTOAP" && SqlFunc.StartsWith(t.VALUE, "T2") && t.VALID_FLAG == 1).ToList();
                    foreach (var item in waitdosnkp)
                    {
                        var strap = string.Empty;
                        var prostr = string.Empty;
                        string tr_sn = item.VALUE;
                        strap = $@"select * from mes4.r_tr_sn where tr_sn='{tr_sn}'";

                        var apmpn = apdb.Ado.GetDataSetAll(strap).Tables[0];
                        if (apmpn.Rows.Count != 0)
                        {
                            item.MPN = apmpn.Rows[0]["mfr_kp_no"].ToString();
                            item.SCANTYPE = "APTRSN";
                            item.EXKEY1 = "APSN";
                            item.EXVALUE1 = tr_sn;
                            item.VALUE = apmpn.Rows[0]["DATE_CODE"].ToString() + "/" + apmpn.Rows[0]["LOT_CODE"].ToString();
                            db.Updateable(item).ExecuteCommand();
                        }
                    }
                }
            }
        }



        void I2021()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FJZODB", false))
            {
                var wo = "009100001031";
                var targets = db.Queryable<R_SN_STATION_DETAIL, R_SN, R_SN>((rs, r1, r2) => rs.SN == r1.SN && r1.SN == r2.SN).Where((rs, r1, r2) => r1.WORKORDERNO == wo && r2.VALID_FLAG == "1")
                    .OrderBy((rs, r1, r2) => rs.SN, OrderByType.Asc).OrderBy((rs, r1, r2) => rs.EDIT_TIME, OrderByType.Asc)
                    .Select((rs, r1, r2) => new { rs.SN, r2.CURRENT_STATION, rs.SKUNO, rs.WORKORDERNO, rs.LINE, STATION = rs.CURRENT_STATION, rs.EDIT_TIME }).ToList();
                var sourc = new List<I1209>();
                var sns = targets.Select(t => t.SN).Distinct().ToList();
                foreach (var sn in sns)
                {
                    var snsour = targets.FindAll(t => t.SN == sn);
                    var siv = snsour.FindAll(t => t.SKUNO == "750-062572").ToList();
                    var smtv = snsour.FindAll(t => t.SKUNO == "711-062571").ToList();
                    var smt = new Func<string>(() => {
                        return new TimeSpan(Convert.ToDateTime(smtv.LastOrDefault().EDIT_TIME).Ticks - Convert.ToDateTime(smtv.FirstOrDefault().EDIT_TIME).Ticks).TotalHours.ToString();
                    })();
                    var si = new Func<string>(() => {
                        if (siv.Count > 0)
                            return new TimeSpan(Convert.ToDateTime(siv.LastOrDefault().EDIT_TIME).Ticks - Convert.ToDateTime(siv.FirstOrDefault().EDIT_TIME).Ticks).TotalHours.ToString();
                        else
                            return "";
                    })();
                    for (int i = 0; i < snsour.Count; i++)
                    {
                        var snob = new I1209()
                        {
                            SN = snsour[i].SN,
                            CURRENT_STATION = snsour[i].CURRENT_STATION,
                            SKUNO = snsour[i].SKUNO,
                            WORKORDERNO = snsour[i].WORKORDERNO,
                            LINE = snsour[i].LINE,
                            STATION = snsour[i].STATION,
                            EDIT_TIME = Convert.ToDateTime(snsour[i].EDIT_TIME),
                            SECTION = new Func<string>(() => {
                                if (snsour[i].SKUNO.StartsWith("711"))
                                    return "SMT";
                                else
                                    return "SI";
                            })(),
                            SMT = smt,
                            SI = si,
                            TIMESPAN = new Func<int>(() => {
                                if (snsour[i].STATION == "SMTLOADING" || snsour[i].STATION == "SILOADING")
                                    return 0;
                                else
                                    return Convert.ToInt32(new TimeSpan(Convert.ToDateTime(snsour[i].EDIT_TIME).Ticks - Convert.ToDateTime(snsour[i-1].EDIT_TIME).Ticks).TotalMinutes);
                            })()
                        };
                        sourc.Add(snob);
                    }
                }
                var timespans = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 24, 48, 72 };
                var smtobj = sourc.FindAll(t => t.SECTION == "SMT").ToList();
                var siobj = sourc.FindAll(t => t.SECTION == "SI").ToList();

                var toldt = new DataTable();
                var tstr = new List<string>();
                for (int i = 0; i < timespans.Length; i++)
                {
                    if(i==0)
                        tstr.Add("0-1(H)");
                    else 
                        tstr.Add($@"{timespans[i-1]}-{timespans[i]}(H)");
                }

                toldt.Columns.Add("SECTION");
                foreach (var item in tstr)
                {
                    toldt.Columns.Add(item);
                }
                toldt.Columns.Add(">72(H)");

                var smtdetail = toldt.Copy();
                var sidetail = toldt.Copy();
                //SMT
                var stldr = toldt.NewRow();
                stldr["SECTION"] = "SMT";
                for (int i = 0; i < timespans.Count(); i++)                 
                {
                    var col = tstr.FindAll(t => t.EndsWith($@"-{timespans[i]}(H)")).ToList().FirstOrDefault();
                    if (i > 0)
                        stldr[col] = smtobj.FindAll(t => t.TIMESPAN < timespans[i] * 60 && t.TIMESPAN > timespans[i - 1] * 60).ToList().Count;
                    else
                        stldr[col] = smtobj.FindAll(t => t.TIMESPAN < 60 && t.TIMESPAN != 0).ToList().Count();
                }
                stldr[">72(H)"] = smtobj.FindAll(t => t.TIMESPAN >72* 60).ToList().Count();

                toldt.Rows.Add(stldr);
                //SI
                var sItldr = toldt.NewRow();
                sItldr["SECTION"] = "SI";
                for (int i = 0; i < timespans.Count(); i++)
                {
                    var col = tstr.FindAll(t => t.EndsWith($@"-{timespans[i]}(H)")).ToList().FirstOrDefault();
                    if (i > 0)
                        sItldr[col] = siobj.FindAll(t => t.TIMESPAN < timespans[i] * 60 &&t.TIMESPAN > timespans[i - 1] * 60).ToList().Count;
                    else
                        sItldr[col] = siobj.FindAll(t =>t.TIMESPAN < 60).ToList().Count();
                }
                sItldr[">72(H)"] = siobj.FindAll(t => t.TIMESPAN > 72 * 60).ToList().Count();
                toldt.Rows.Add(sItldr);

                var smtstations = smtobj.Select(t => t.STATION).Distinct();
                foreach (var item in smtstations)
                {
                    var dr = smtdetail.NewRow();
                    dr["SECTION"] = item;
                    var currentstationde = smtobj.FindAll(t => t.STATION == item).ToList();
                    for (int i = 0; i < timespans.Count(); i++)
                    {
                        var col = tstr.FindAll(t => t.EndsWith($@"-{timespans[i]}(H)")).ToList().FirstOrDefault();
                        if (i > 0)
                            dr[col] = currentstationde.FindAll(t => t.TIMESPAN < timespans[i] * 60 &&t.TIMESPAN > timespans[i - 1] * 60).ToList().Count;
                        else
                            dr[col] = currentstationde.FindAll(t => t.TIMESPAN < 60).ToList().Count();
                    }
                    dr[">72(H)"] = currentstationde.FindAll(t => t.TIMESPAN > 72 * 60).ToList().Count();
                    smtdetail.Rows.Add(dr);
                }

                var sistations = siobj.Select(t => t.STATION).Distinct();
                foreach (var item in sistations)
                {
                    var dr = sidetail.NewRow();
                    dr["SECTION"] = item;
                    var currentstationde = siobj.FindAll(t => t.STATION == item).ToList();
                    for (int i = 0; i < timespans.Count(); i++)
                    {
                        var col = tstr.FindAll(t => t.EndsWith($@"-{timespans[i]}(H)")).ToList().FirstOrDefault();
                        if (i > 0)
                            dr[col] = currentstationde.FindAll(t => t.TIMESPAN < timespans[i] * 60 && t.TIMESPAN > timespans[i - 1] * 60).ToList().Count;
                        else
                            dr[col] = currentstationde.FindAll(t =>t.TIMESPAN < 60).ToList().Count();
                    }
                    dr[">72(H)"] = currentstationde.FindAll(t => t.TIMESPAN > 72 * 60).ToList().Count();
                    sidetail.Rows.Add(dr);
                }
                toldt.TableName = "toldt";
                smtdetail.TableName = "smttol";
                sidetail.TableName = "sitol";
                var ds = new DataSet();
                ds.Tables.Add(toldt);
                ds.Tables.Add(smtdetail);
                ds.Tables.Add(sidetail);
                ds.Tables.Add(DataHelper.ModelToTable(sourc, "Detail"));
                ds.Tables.Add(DataHelper.ModelToTable(smtobj, "SmtDetail"));
                ds.Tables.Add(DataHelper.ModelToTable(siobj, "SiDetail"));
                ExcelHelp.ExportExcelToLoacl(ds, $@"C:\\Users\\G6001953.NN\\Desktop\\VN\\{wo}_v3.csv", true);
                //ExcelHelp.ExportCsv(, "C:\\Users\\G6001953.NN\\Desktop\\VN\\Detail.csv");
                //ExcelHelp.ExportCsv(smtobj, "C:\\Users\\G6001953.NN\\Desktop\\VN\\SmtDetail.csv");
                //ExcelHelp.ExportCsv(siobj, "C:\\Users\\G6001953.NN\\Desktop\\VN\\SiDetail.csv");
                //ExcelHelp.ExportExcelToLoacl(toldt, "C:\\Users\\G6001953.NN\\Desktop\\VN\\tol.csv", true);
                //ExcelHelp.ExportExcelToLoacl(smtdetail, "C:\\Users\\G6001953.NN\\Desktop\\VN\\smttol.csv", true);
                //ExcelHelp.ExportExcelToLoacl(sidetail, "C:\\Users\\G6001953.NN\\Desktop\\VN\\sitol.csv", true);

            }
        }


        class DcnAgingData
        {
            public string WorkRouteType { get; set; }
            public string PLANT { get; set; }
            public string SN { get; set; }
            public string REPAIRHELD { get; set; }
            public string SKUNO { get; set; }
            public string WORKORDERNO { get; set; }
            public string CURRENT_STATION { get; set; }
            public string NEXT_STATION { get; set; }
            public string AGING { get; set; }
        }

        class csvobj
        {
            [CsvHelper.Configuration.Attributes.Name("A- /5.4g")]
            public int ID { get; set; }
            [CsvHelper.Configuration.Attributes.Name("A- /4.4g")]
            public string Name { get; set; }
        }

        List<T> GetCsv<T>(string path)
        {
            using (TextReader reader = File.OpenText(path))
            {
                reader.ReadLine();
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<T>().ToList();
                    return records;
                }
            }
        }

        public class Material
        {
            public string NVSN { get; set; }
            public string MACHINE { get; set; }
            public string START_TIME { get; set; }
            public string COMP_PN
            { get; set; }
            public string SCOMP_PN
            { get; set; }
            public string COMP_TYPE
            { get; set; }
            public string DATECODE
            { get; set; }
            public string LOT
            { get; set; }
            public string VENDOR

            { get; set; }
            public string LOCATION

            { get; set; }
            public string QTY

            { get; set; }
            public string NVPN

            { get; set; }
            public string COMP_SN

            { get; set; }
            public string MFG_PN

            { get; set; }
            public string MFG_SN

            { get; set; }
            public string EOL

            { get; set; }
        }

        public class nvd_Material
        {
            public string FILENAME { get; set; }
            public string NVSN { get; set; }
            public string MACHINE { get; set; }
            public string START_TIME { get; set; }
            public string COMP_PN { get; set; }
            public string SCOMP_PN { get; set; }
            public string COMP_TYPE { get; set; }
            public string DATECODE { get; set; }
            public string LOT { get; set; }
            public string VENDOR { get; set; }
            public string LOCATION { get; set; }
            public string QTY { get; set; }
            public string NVPN { get; set; }
            public string COMP_SN { get; set; }
            public string MFG_PN { get; set; }
            public string MFG_SN { get; set; }
            public string EOL { get; set; }
            public DateTime CREATETIME { get; set; }
        }

        class aptemp00001
        {
            public string p_sn { get; set; }
            public string tr_sn { get; set; }
            public string mfr_kp_no { get; set; }
            public string date_code { get; set; }
            public string lot_code { get; set; }
        }

        class Fs
        {
            public string NVSN { get; set; }
            public string PRODUCTION_FAIL_TIME { get; set; }
            public string STATION { get; set; }
            public string RSTATIONID { get; set; }
            public string ERRORCODE_PROD { get; set; }
            public string ERRORCODE_DESCRIPTION { get; set; }
            public string ERRORCODE_FA { get; set; }
            public string ERRORCODE_DESCRIPTION_FA { get; set; }
            public string REASONCODE { get; set; }
            public string REASON_DESCRIPTION { get; set; }
            public string DUTY { get; set; }
            public string FA_START_TIME { get; set; }
            public string FA_END_TIME { get; set; }
            public string FCOMPONENT { get; set; }
            public string SFCOMPONENT { get; set; }
            public string FLOCATION { get; set; }
            public string DATECODE { get; set; }
            public string LOT { get; set; }
            public string EMPID { get; set; }
            public string NVPN { get; set; }
            public string NVBUG { get; set; }
            public string STATUS { get; set; }
            public string EOL { get; set; }
        }

        class ship
        {
            public string DN { get; set; }
            public string SHIPNO { get; set; }
            public string NVPN { get; set; }
            public string NVPO { get; set; }
            public string NVSN { get; set; }
            public string CSN { get; set; }
            public string CARTON { get; set; }
            public string IN_TIME { get; set; }
            public string PALLET { get; set; }
            public string EMPID { get; set; }
            public string EOL { get; set; }
        }

        class nvd_ship
        {
            public string FILENAME { get; set; }
            public string DN { get; set; }
            public string SHIPNO { get; set; }
            public string NVPN { get; set; }
            public string NVPO { get; set; }
            public string NVSN { get; set; }
            public string CSN { get; set; }
            public string CARTON { get; set; }
            public string IN_TIME { get; set; }
            public string PALLET { get; set; }
            public string EMPID { get; set; }
            public string EOL { get; set; }
            public DateTime CREATETIME { get; set; }
        }

        class eco
        {
            public string NVSN { get; set; }
            public string DA { get; set; }
            public string ECO { get; set; }
            public string NVPN { get; set; }
        }

        public class process
        {
            public string NVSN { get; set; }
            public string NVPN { get; set; }
            public string LINE_NAME { get; set; }
            public string STATIONID { get; set; }
            public string STATION { get; set; }
            public string STATION_TYPE { get; set; }
            public string STATIONNAME { get; set; }
            public string START_TIME { get; set; }
            public string END_TIME { get; set; }
            public string RESULT { get; set; }
            public string EMPID { get; set; }
            public string ERRORCODE { get; set; }
            public string ERRORCODE_DESCRIPTION { get; set; }
            public string OTHER_SN { get; set; }
            public string ECID { get; set; }
            public string DIAG { get; set; }
            public string BIOS { get; set; }
            public string INTERPOSER { get; set; }
            public string BIN { get; set; }
            public string WO { get; set; }
            public string NVPBR { get; set; }
            public string TEST_LOG_NAME { get; set; }
            public string DA { get; set; }
            public string ECO { get; set; }
            public string DISPOSITIONCODE { get; set; }
            public string EOL { get; set; }
        }

        public class nvd_temp_job
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public string FILENAME { get; set; }
            public string FLAG1 { get; set; }
            public DateTime TIME1 { get; set; }
            public string FLAG2 { get; set; }
            public DateTime TIME2 { get; set; }
            public string FLAG3 { get; set; }
            public DateTime TIME3 { get; set; }
            public DateTime CREATETIME { get; set; }
        }

        public class mfsysevent_nvd
        {
            public string sysserialno { get; set; }
            public string eventname { get; set; }
            public string scandatetime { get; set; }
            public string factoryid { get; set; }
            public string productionline { get; set; }
            public string shift { get; set; }
            public string scanby { get; set; }
            public string eventpass { get; set; }
            public string eventfail { get; set; }
            public string productstatus { get; set; }
            public string MdsGet { get; set; }
        }

        public class mfsysevent
        {
            public string sysserialno { get; set; }
            public string eventname { get; set; }
            public string scandatetime { get; set; }
            public string factoryid { get; set; }
            public string productionline { get; set; }
            public string shift { get; set; }
            public string scanby { get; set; }
            public string eventpass { get; set; }
            public string eventfail { get; set; }
            public string productstatus { get; set; }
            public string MdsGet { get; set; }
        }

        public class nvd_pro
        {
            public string FILENAME { get; set; }
            public string NVSN { get; set; }
            public string NVPN { get; set; }
            public string LINE_NAME { get; set; }
            public string STATIONID { get; set; }
            public string STATION { get; set; }
            public string STATION_TYPE { get; set; }
            public string STATIONNAME { get; set; }
            public string START_TIME { get; set; }
            public string END_TIME { get; set; }
            public string RESULT { get; set; }
            public string EMPID { get; set; }
            public string ERRORCODE { get; set; }
            public string ERRORCODE_DESCRIPTION { get; set; }
            public string OTHER_SN { get; set; }
            public string ECID { get; set; }
            public string DIAG { get; set; }
            public string BIOS { get; set; }
            public string INTERPOSER { get; set; }
            public string BIN { get; set; }
            public string WO { get; set; }
            public string NVPBR { get; set; }
            public string TEST_LOG_NAME { get; set; }
            public string DA { get; set; }
            public string ECO { get; set; }
            public string DISPOSITIONCODE { get; set; }
            public string EOL { get; set; }
            public DateTime CREATETIME { get; set; }
        }

        public class I1209
        {
            public string SECTION { get; set; }
            public string SN { get; set; }
            public string CURRENT_STATION { get; set; }
            public string SKUNO { get; set; }
            public string WORKORDERNO { get; set; }
            public string LINE { get; set; }
            public string STATION { get; set; }
            public DateTime EDIT_TIME { get; set; }
            public int TIMESPAN { get; set; }
            public string SMT { get; set; }
            public string SI { get; set; }
        }


        string i211221 = $@"4700022571,
4500560306,
4500561033,
4500563566,
4500570345,
4500568361,
4500566803,
4500573715,
4500573997,
4500568064,
4500568070,
4500564390,
4500571189,
4500564560,
4500580107,
4500580510,
4500580830,
4500581848,
4500586457,
4700023165,
4500585065,
4500582649,
4500585365,
4500585412,
4500575637,
4700023445,
4500591534,
4700023794,
4500590899,
4500589507,
4500592077,
4500592094,
4500594096,
4700023762,
4500592511,
4500592597,
4500592502,
4500592517,
4500592607,
4500592719,
4500600009,
4500588451,
4700023728,
4500591108,
4500591652,
4500590589,
4500591681,
4500593712,
4500593713,
4500593717,
4500593721,
4500590897,
4500591470,
4500591479,
4500591484,
4500591523,
4500591533,
4500594721,
4500593186,
4500590528,
4500593194,
4500589023,
4500592495,
4500594697,
4500589804,
4500595786,
4500593942,
4500593945,
4500593999,
4500594005,
4500591117,
4500595855,
4700023933,
4500591089,
4500599826,
4500592783,
4500595529,
4300014563,
4500591336,
4500591338,
4500591945,
4300014599,
4500595721,
4500595720,
4500591356,
4500593202,
4500591610,
4500600134,
4500589455,
4500591009,
4500591011,
4500591014,
4500591013,
4500591017,
4500591022,
4500591349,
4500591378,
4500605073,
4500605480,
4500604085,
4500604555,
4500604558,
4500604559,
4500605793,
4500605798,
4500600251,
4500605285,
4500605572,
4500607814,
4700024243,
4500607104,
4500606996,
4500596540,
4500596545,
4500596554,
4500597052,
4500606995,
4500601829,
4700024251,
4500597020,
4500597019,
4500597024,
4500597455,
4500595784,
4500596223,
4500599727,
4500601633,
4500601630,
4500607524,
4500600234,
4500601642,
4500601741,
4500607113,
4500607242,
4500602135,
4500607125,
4500607628,
4500597957,
4500603122,
4500603129,
4500604486,
4500602791,
4500607706,
4500607709,
4500598526,
4500607807,
4700023938,
4500595949,
4500599676,
4500596393,
4500598634,
4500598644,
4500598645,
4500601681,
4500597833,
4500597832,
4500606284,
4500606351,
4700024239,
4500598242,
4500606354,
4500606356,
4500600390,
4500602704,
4500603455,
4500603409,
4500606400,
4700024096,
4500603821,
4500605353,
5100016718,
4500601089,
4500595988,
4700024188,
4500605516,
4500605513,
4500605525,
4500592244,
4500605520,
4500605562,
4500605608,
4300014382,
4500604607,
4500604608,
4300015313,
4300015310,
4500598734,
4500605434,
4500603165,
4500599087,
4500613352,
4700024474,
4700024475,
4500614411,
4500613943,
4500618449,
4500619076,
4500619073,
4500613011,
4500616497,
4500616923,
4500614412,
4500614010,
4500618647,
4500618648,
4500619118,
4500619122,
4300015169,
4500614521,
4500615469,
4500614965,
4500614524,
4500615029,
4500615625,
4500615652,
4500615653,
4500615656,
4500616332,
4500616567,
4500616571,
4500615494,
4500615809,
4500615722,
4500610380,
4500610883,
4500609286,
4500609288,
4500616364,
4500617273,
4500608538,
4500608540,
4500617415,
4500618477,
4500608952,
4500616820,
4500618476,
4500608960,
4500617001,
4700024755,
4500618629,
4500609306,
4500608115,
4500618798,
4500619013,
4500619125,
4500619164,
4500607633,
4500610499,
4500610497,
4500617674,
4500617861,
4500608699,
4500619174,
4500619180,
4500610698,
5100017020,
4500610426,
4500611834,
4500618592,
4500618618,
4500610456,
4500611690,
4500615250,
4500609072,
4500609081,
4500609083,
4500613121,
4500613147,
4500613330,
4500608370,
4500608373,
4500615259,
4500608378,
4500610185,
4500615587,
4500608799,
4500608800,
4500610197,
4500610203,
5100016992,
4500609529,
4500609896,
4500609897,
4500617528,
4500618162,
4500613383,
4500613386,
4500617985,
4500617242,
4500618021,
4500614931,
4500617676,
4500617679,
4500610299,
5100016912,
5100016914,
5100016915,
4500617618,
4500610786,
4500610789,
4500617776,
4500618210,
4500614243,
4500617053,
4500617376,
4500618546,
4500618548,
4500618770,
4500608812,
4500616795,
4500617154,
4500617159,
4500609400,
4500618891,
4500618887,
4500613646,
4500614448,
4500617816,
4500610617,
4700024535,
4500619165,
4500610844,
4500610852,
4500610848,
4500617904,
4500614834,
4500618188,
4500618937,
4700024856,
4500619247,
4500619717,
4700024892,
4500619289,
4500619413,
4500619705,
4500595082,
4500601637,
4500619631,
4500619633,
4500619634,
4500619521,
4500560404,
4500561624,
4700022754,
4500567350,
4500570141,
4500570140,
4500569422,
4500570137,
4500566644,
4500570380,
4500568726,
4700023598,
4500586437,
4700023601,
4500577845,
4500581550,
4500583772,
4500583778,
4500584938,
4500576916,
4500577611,
4700023124,
4500581517,
4500584629,
4500585007,
4500577662,
4500587012,
4500587013,
4500585291,
4700023392,
4500583049,
4500584760,
4500579206,
4500591711,
4500591714,
4500591715,
4500591536,
4500591527,
4500595075,
4300014516,
4300014519,
4700023734,
4500592514,
4500592509,
4500595127,
4500589505,
4500589506,
4500592076,
4500593479,
4500591636,
4500592503,
4500589717,
4500591507,
4500591664,
4500591658,
4700023776,
4500591796,
4500592080,
4500596538,
4500593537,
4500591662,
4500591686,
4500593715,
4500593719,
4500591643,
4500591668,
4500593726,
4500593771,
4500593773,
4500597718,
4500593991,
4500590900,
4500591477,
4500593992,
4500593998,
4500591639,
4500590922,
4500591508,
4500590927,
4500590925,
4500590924,
4500591521,
4500591531,
4500591379,
4500591388,
4500593188,
4500591389,
4500594760,
4500593891,
4500592377,
4500589638,
4500591007,
4500590928,
4500592416,
4700023676,
4500593937,
4500593939,
4500593953,
4500590839,
4500594012,
4500591120,
4500595853,
4500591332,
4500591351,
4300014952,
4300014562,
4700023751,
4500591944,
4700023811,
4500591343,
4700023756,
4500592246,
4500591617,
4300014858,
4500589454,
4500593794,
4500594480,
4500594478,
4500595385,
4500591025,
4500594569,
4500594759,
4500591373,
4500591386,
4500596505,
4500604557,
4500604561,
4500604563,
4500605870,
4500605565,
4500605886,
4500607805,
4500607872,
4500607879,
4500600403,
4500607148,
4500596547,
4500597406,
4500601013,
4500607103,
4500597955,
4500596299,
4500597013,
4500597015,
4500596232,
4500607341,
4500596229,
4500601685,
4500597169,
4500600746,
4500602492,
4500601634,
4500606909,
4500602981,
4500601641,
4500607646,
4500596351,
4500596353,
4500602530,
4500602786,
4500606821,
4500607791,
4300015437,
4500604252,
4500604483,
4500604481,
4500602956,
4500607792,
4500607802,
4500604476,
4500595953,
4500596177,
4500597487,
4500599677,
4500594486,
4500601336,
4500597680,
4500602068,
4500600867,
4500600868,
4500599032,
4500606286,
4500606285,
4500598886,
4500606425,
4500602209,
4500602208,
4500600052,
4500606403,
4500603797,
4500600434,
4500605354,
4500605362,
4500604600,
4500604597,
4500600795,
4500605442,
4500605444,
4500604605,
4500596457,
4500602673,
4300015186,
4500597293,
4500603130,
4500591111,
4300015315,
4500604610,
4500599005,
4300015432,
4500599081,
4500613354,
4500619069,
4500619227,
4500614415,
4300015168,
4500613683,
4500613685,
4500615472,
4700024665,
4500614964,
4500614970,
5100016947,
5100016949,
4500611773,
4500615662,
4500611948,
4500616018,
4500610374,
4300015679,
4500610384,
4500609284,
4500616365,
4500609293,
4500609299,
4500616602,
4500616605,
4500616612,
4500616611,
4500617423,
4500608117,
4500608946,
4500608947,
4500616603,
4500616775,
4500616818,
4500608552,
4500618510,
4700024819,
4500608954,
4500617209,
4500617401,
4500618631,
4500609304,
4500609311,
4500609312,
4500618796,
4500618795,
4500619126,
4500619127,
4500619162,
4500610496,
4500608701,
4500619173,
4500619186,
4500619200,
4500610691,
4500610700,
4500610703,
4500610709,
5100017017,
4500619226,
4500610708,
4500611333,
4500618479,
4500618482,
4500618578,
4500618581,
4500618585,
4500618591,
4500612470,
4500611331,
4500613242,
4500613296,
4500618617,
4500618912,
4500618918,
4700024386,
4500610372,
4500607870,
4500612675,
4500615242,
4500609077,
4500613180,
4500615513,
4500615524,
4700024673,
4500610192,
4500616050,
4500608798,
4500608796,
4300015796,
4500617527,
4500617074,
4500618208,
4500611647,
4500611641,
4500611645,
4500614929,
4700024790,
4500610284,
4700024288,
4500610291,
4500618063,
4500615173,
4500615174,
4500617678,
4500610409,
4500616544,
4500617626,
4500610568,
4500610788,
4700024810,
4500618417,
4500618415,
4500614277,
4500614485,
4500614484,
4500616731,
4500617052,
4500617375,
4500610792,
4500610799,
4700024641,
4500618616,
4500608817,
4500616965,
4500617160,
4500617161,
4500617163,
4500618904,
5100016919,
4500610613,
4300015753,
4500619083,
4500610963,
4500617912,
4500617915,
4500618085,
4500618133,
4500614838,
5100016693,
4500611084,
4500610114,
4500617326,
4500619310,
4500619291,
4500591558,
4500619605,
4500619609,
4500619610,
4500619527,
4500619533,
4500619235,
5100015217,
4500557125,
4700022554,
4500556520,
4500564756,
4500571054,
4500567482,
4500568523,
4500566642,
4500568807,
4500565435,
4700023600,
4500581393,
4500583765,
4500583785,
4500583781,
4500583771,
4500583779,
4500582502,
4500577345,
4500577773,
4500587355,
4500574502,
4700023086,
4500580826,
4500581398,
4500582582,
4500582584,
4500576992,
4500583050,
4500579237,
4500585679,
4500578782,
4500591713,
4500591764,
4500591529,
4300014517,
4500591080,
4500591710,
4500592500,
4500595128,
4500588158,
4500592074,
4500592095,
4500593482,
4500594097,
4500589961,
4500592512,
4500593229,
4500588366,
4500588365,
4500588363,
4500588359,
4500600008,
4500594530,
4500594323,
4500591082,
4700023774,
4500591369,
4500591665,
4500591655,
4500591157,
4500596535,
4500590841,
4500591687,
4500593716,
4500590894,
4500593727,
4500593766,
4500590896,
4500591469,
4500591518,
4500591525,
4500591535,
4500588113,
4500593184,
4500593185,
4500587759,
4500589026,
4500592449,
4500592837,
4500589800,
4500589398,
4500595778,
4500589812,
4500589798,
4500593943,
4500593952,
4500590840,
4500590847,
4500591114,
4500595443,
4300014956,
4700023753,
4500595722,
4500591365,
4500594424,
4500594537,
4500595517,
4500595519,
4700023862,
4700023863,
4500593181,
4500595716,
4500595724,
4500594473,
4500597126,
4500592662,
4500591006,
4500591012,
4500588210,
4500595187,
4500604073,
4500598538,
4500598539,
4500605485,
4500605488,
4500604551,
4500599211,
4500605563,
4500599763,
4500607123,
4500607122,
4500607120,
4500607112,
4500607109,
4500607803,
4500607813,
4500600459,
4500605973,
4500606999,
4500596539,
4500596546,
4500596548,
4500606906,
4500607100,
4500607114,
4500597014,
4500607289,
4500607433,
4500597456,
4500601680,
4500599855,
4500600735,
4500590926,
4500601640,
4500607238,
4500607239,
4500606823,
4500607124,
4500597965,
4500598430,
4500607121,
4500607106,
4500598432,
4500604474,
4500604487,
4500604485,
4500607797,
4500598532,
4700023939,
4500600970,
4500600972,
4500597454,
5100016695,
4500597676,
4500601108,
4500602185,
4500606355,
4500596061,
4500596298,
4500603084,
4500600402,
4500600431,
4500600294,
4500604183,
4500603817,
4500603818,
4500605355,
4500605348,
4500604371,
4500604603,
4700023976,
4500605524,
4500599829,
4500603736,
4500591539,
4500605527,
4500598726,
4500598728,
4500603131,
4500602451,
4500602450,
4700024061,
4300015312,
4500598736,
4500604631,
4500599001,
4500599082,
4500613009,
4500613092,
4500613093,
4700024546,
4500618650,
4500619120,
4500619119,
4500619123,
4700024615,
4700024614,
4500615092,
4500615470,
5100016946,
4700024587,
4500611117,
4500616569,
4500611795,
4500615776,
4700024362,
4300015682,
4500616017,
4500616039,
4500610885,
4500610887,
4500616691,
4500608124,
4700024743,
4500617183,
4500616601,
4500616607,
4500608542,
4500618232,
4500608950,
4500616604,
4500608961,
4500608944,
5100016976,
4500617005,
4500609314,
4500608698,
4500618831,
4500607642,
4500609993,
4500610495,
4500619170,
4500619184,
4500619188,
4500619199,
4500610701,
5100017013,
4500610434,
4500610433,
4500611718,
4500611325,
4500611329,
4500612725,
4500611909,
4500610928,
4500611212,
4500614798,
4500612678,
4700024383,
4500609075,
4500609079,
4500613132,
4500608365,
4500608376,
4500609073,
4500609096,
4500609097,
4500609105,
4500609107,
4500608794,
4500610195,
4500610193,
4500608809,
4500617073,
4500617556,
4500609988,
4500618160,
4500617983,
4700024804,
4500610283,
4500610286,
4500610287,
4500613060,
4500613736,
4500613740,
4700024590,
4500617680,
4500616535,
4500616543,
4700024717,
4700024718,
4500617037,
4500610795,
4700024780,
4500618514,
4500618516,
4500614241,
4500614245,
4500614482,
4500610791,
4500610797,
4500618515,
4500618549,
4500618775,
4500618771,
4500614166,
4500609394,
4500618895,
4500618900,
5100016927,
4500610618,
4500619015,
4500619016,
4500619196,
4700024806,
4500617323,
4700024344,
4500619408,
4500619604,
4500619612,
4500608687,
4700024867,
4500619234,
4500619547,
4500619520,
5100015613,
5100015607,
4500556056,
4500571631,
4500570350,
4500574295,
4500573029,
4500568366,
4500570138,
4500571198,
4500564392,
4500587695,
4700023599,
4500583782,
4500583769,
4500583773,
4500583775,
4500583787,
4700023555,
4500587455,
4500587564,
4500582583,
4500575358,
4500582641,
4500582650,
4500587014,
4500583051,
4700023418,
4700023419,
4500583501,
4700023408,
4500585680,
4500577986,
4500591712,
4700023914,
4500591008,
4500591023,
4500590917,
4500591088,
4700023769,
4500594789,
4300014570,
4500593230,
4500588364,
4500588361,
4500590991,
4500600011,
4500591003,
4500594322,
4500591002,
4500588481,
4700023772,
4500600207,
4500599847,
4500591344,
4500591657,
4500593995,
4500596536,
4500590033,
4500593539,
4500591653,
4500591667,
4500591689,
4500591683,
4500591674,
4500591692,
4500593762,
4700023765,
4500591486,
4500593758,
4500593776,
4500593774,
4700023885,
4500591499,
4500591504,
4500594052,
4500590921,
4500594377,
4500591513,
4500591522,
4500594669,
4500591383,
4500591394,
4500593187,
4500592129,
4500593894,
4500589354,
4500590838,
4500594695,
4500589805,
4500593077,
4500589807,
4500589799,
4500593946,
4500593949,
4500595436,
4500594000,
4500594460,
4500591112,
4500591116,
4500591118,
4500591119,
4500591121,
4500591004,
4500591158,
4300014811,
4500595439,
4300014948,
4700023754,
4700023784,
4500590996,
4500591342,
4700023759,
4500593211,
4500601815,
4300014949,
4300014960,
4500595944,
4500588710,
4500588714,
4500591607,
4500591611,
4500595485,
4500595516,
4700023922,
4500595513,
4500593643,
4500593795,
4500595330,
4500594483,
4500595389,
4500591018,
4500588805,
4700023646,
4500591348,
4500604078,
4500604075,
4500598832,
4500599208,
4500604556,
4500604550,
4500605797,
4500605838,
4500599531,
4500599532,
4500599870,
4500600386,
4500600400,
4700023963,
4500600604,
4500606905,
4500606989,
4500601016,
4500597021,
4500607288,
4500598238,
4500598239,
4500607520,
4500601686,
4700024064,
4500601311,
4500601631,
4500606993,
4500607527,
4500607107,
4500596348,
4500596358,
4500603047,
4500602360,
4500602531,
4500602785,
4500607101,
4500597958,
4500606745,
4500607105,
4500598444,
4500604489,
4500604488,
4500603423,
4500607129,
4500607801,
4500604477,
4500596390,
4500594485,
4500601589,
4500602070,
4500597835,
4500597721,
4500606240,
4500600384,
4500602707,
4500599749,
4500600054,
4500603414,
4500603410,
4500606401,
4500603352,
4500603383,
4500604152,
4500604593,
4500604598,
4500604604,
4500605440,
4500605523,
4500604595,
4500603731,
4500605519,
4500603135,
4500603201,
4500605438,
4500602542,
4500602543,
4500603294,
4500599080,
4500613357,
4500618450,
4500619075,
4500614097,
4500615471,
5100016952,
5100016950,
4500614525,
4300015747,
4700024325,
4500611796,
4500615654,
4500616362,
4500616568,
4500611793,
4500611946,
4500615778,
4500615936,
4500610382,
4700024702,
4700024707,
4500616167,
4300015681,
5100016885,
4500609277,
4500609281,
4500609290,
4500609292,
4500616178,
4500616279,
4500617274,
4500608546,
4500608541,
4500618002,
4500618231,
5100017040,
4500608955,
4500608956,
4500616819,
4500618509,
4500608962,
4500617004,
4500618627,
4500617405,
4500608554,
4500608697,
4500618989,
4500619163,
4500617864,
4500608693,
4500610886,
4500619176,
4500619187,
4500619183,
4500619182,
4500619194,
4500619223,
4500610697,
4500610704,
4500618575,
4600008324,
4500611990,
4700024831,
4500618976,
4500610371,
4500610914,
4500612680,
4500609074,
4500613148,
4500613196,
4500608372,
4500608380,
4500615525,
4500609092,
4500609100,
4500609106,
4500614190,
4500616104,
4300015782,
4500610190,
4500609900,
4500609987,
4500610202,
5100016997,
4700024848,
4500619231,
4500611250,
4500613384,
4500617987,
4500610279,
4500617241,
4500610297,
4500610298,
4500617042,
4500617039,
4500618370,
4500610801,
4500610798,
4500617623,
4500609110,
4500618772,
4500608024,
4500608816,
4700024758,
4500609402,
4500618889,
4500618894,
4500618964,
4300015844,
4500609401,
4300015751,
4700024858,
4500619201,
4500619209,
4500610847,
4500618190,
4500610979,
4500611083,
4500611085,
4500608219,
4500617333,
4500617328,
4500619309,
4500619616,
4300015854,
4500619668,
4500619465,
4500619466,
5100017050,
4500619471,
5100015221,
4500570650,
4500573520,
4500571088,
4500573572,
4500573599,
4500573999,
4500567543,
4500567545,
4500567765,
4500576103,
4500583766,
4500583780,
4500583777,
4500584900,
4500585788,
4500583385,
4500586586,
4500587457,
5100016337,
4500577104,
4500582728,
4500582647,
4500585862,
4300014199,
4500574798,
4500578934,
4500584582,
4500579209,
4500591532,
4500594735,
4700023735,
4500591073,
4500591709,
4500592497,
4500592082,
4500593478,
4500589958,
4500589959,
4500589960,
4700023916,
4500591649,
4500592598,
4500592595,
4500588455,
4500591503,
4500591501,
4500591646,
4500592130,
4500590842,
4500590898,
4500590892,
4500591670,
4500591447,
4500593724,
4500593760,
4500593759,
4700023882,
4500593990,
4500591498,
4500593996,
4500594053,
4500591524,
4500594722,
4500591381,
4500591399,
4500591393,
4500591390,
4500593190,
4500591826,
4500599800,
4500592417,
4700023902,
4500589796,
4500589801,
4500589806,
4500589810,
4500589815,
4500593944,
4500589820,
4500589822,
4500593947,
4500594003,
4500591109,
4500595002,
4500592786,
4500595445,
4300014950,
4300014560,
4700023755,
4500591333,
4500592088,
4500595715,
4500591346,
4500593198,
4500592782,
4300014953,
4500594269,
4700023892,
4500591608,
4500591604,
4500595514,
4700023864,
4500595718,
4700023896,
4500591005,
4500591015,
4500594470,
4500592020,
4500592030,
4500592023,
4700023787,
4300014958,
4500591020,
4500591033,
4500594587,
4500604084,
4500607808,
4500598833,
4500598834,
4500604562,
4500605836,
4500599286,
4700023967,
4500605290,
4500605698,
4500607882,
4700024024,
4500600458,
4500602880,
4500596541,
4500596550,
4500596549,
4700024247,
4500601014,
4500597023,
4500597017,
4500607342,
4500597218,
4500601522,
4500601525,
4500598243,
4500598933,
4500607517,
4500601684,
4500600747,
4500598018,
4500602511,
4500602532,
4500598899,
4500607635,
4700024257,
4500596346,
4500596349,
4500596356,
4500603124,
4500607116,
4500607613,
4500597960,
4500596484,
4500596485,
4500606743,
4500607118,
4500598439,
4500598447,
4500607708,
4500607711,
4500598528,
4500604490,
4500595948,
4500595950,
4500595952,
4500600969,
4500598895,
4500599678,
4500596306,
4500598635,
4500601587,
4500597834,
4500600393,
4500603339,
4500603799,
4500600432,
4500604151,
4500603820,
4500605351,
4500605364,
4500604370,
4500604599,
4500596964,
4500596969,
4500597289,
4500605518,
4500605517,
4700024010,
4500596459,
4300015185,
4500603734,
4500605521,
4500603123,
4500603127,
4500603134,
4500598738,
4500596220,
4300015439,
4500599079,
4500599089,
4500599091,
4700024476,
4500619071,
4300015131,
4500614758,
5100016948,
4500611792,
4500615626,
4700024677,
4500615657,
4700024676,
5100016966,
4500610381,
4500616166,
4500612170,
4500613103,
4500609280,
4500616179,
4500608120,
4500609297,
4500617416,
4500618001,
4500618003,
4500618513,
4500617002,
4500591677,
4500618628,
4700024825,
4500617665,
4500608558,
4500617669,
4500618832,
4500619017,
4500619168,
4500619171,
4500619193,
4500619197,
4500610702,
4500610707,
4500610432,
4500611326,
4500618260,
4500618481,
4500618480,
4500618583,
4500611991,
4500610484,
4700024549,
4500612690,
4500612958,
4500615204,
4500615208,
4500615210,
4500615211,
4500615246,
4500609091,
4500613217,
4500608361,
4500609101,
4500616496,
4300015618,
4500610189,
4500610191,
4500615804,
4500616051,
4500608801,
4500616585,
4500617662,
4500610187,
4500610198,
4500617072,
4500617094,
4500617620,
4500610952,
4500617554,
4500618907,
4500617663,
4500618168,
4700024791,
4500610280,
4500610285,
4500618704,
4500616534,
4500617902,
5100016913,
4500617040,
4500610794,
4500610793,
4500616733,
4500617625,
4500617627,
4500618774,
4500618897,
4500608022,
4500608813,
4500608815,
4500617153,
4500617157,
4500617165,
4500617156,
4500619230,
4500609396,
4500618893,
4300015843,
4500614445,
4500617903,
4500619172,
4500619178,
4500619185,
4500619192,
4500619198,
4500608543,
4500617914,
4500618131,
4500618189,
4500611077,
4500611079,
4500618938,
4500608208,
4500608213,
4500617334,
4500617325,
4500617324,
4500619246,
4500619682,
4500598800,
4500619288,
4500619670,
4500619467,
4500619632,
4500619636,
4500611078,
5100015223,
5100015707,
4500570139,
4500564755,
4500571678,
4500574215,
4500568945,
4500566211,
4500587730,
4500579842,
4500580104,
4500576870,
4700023219,
4500581216,
4500583783,
4500583767,
4500581899,
4500582929,
4500586663,
4700023343,
4500577282,
4500574494,
4500574509,
4500586638,
4500583052,
4500581585,
4500585579,
4700023390,
4500591526,
4500591076,
4500592508,
4500591090,
4500592073,
4500592081,
4700023763,
4500590651,
4500594094,
4700023773,
4500592593,
4500592608,
4500593227,
4500592605,
4500592602,
4500600014,
4500600206,
4500591659,
4500591656,
4500593277,
4500593431,
4500590891,
4500593720,
4500591672,
4500591694,
4500593722,
4500593767,
4500590895,
4700023764,
4500591495,
4500593997,
4500591509,
4500594729,
4500591384,
4500591382,
4500591396,
4500591392,
4500593189,
4500593192,
4500584444,
4500588028,
4500592301,
4500587764,
4500587761,
4500587766,
4500592450,
4700023719,
4500589795,
4500592833,
4500595849,
4500589809,
4500589813,
4500589797,
4700023884,
4500593950,
4500595004,
4500597900,
4500593956,
4500594006,
4500591115,
4500591122,
4500591092,
4500591986,
4300014951,
4500591335,
4700023757,
4500591367,
4500591363,
4500593204,
4500593199,
4500593196,
4700023813,
4500595912,
4500592663,
4500591010,
4500591016,
4500592022,
4500592021,
4300014959,
4500595544,
4500591019,
4500591024,
4500591375,
4500604071,
4500598533,
4500604552,
4500604554,
5100016805,
4500606497,
4500607119,
4500607110,
4500607867,
4500600405,
4500602743,
4500602749,
4500605895,
4500607099,
4500607046,
4500596544,
4500600479,
4500607001,
4500607047,
4500597407,
4500607192,
4700023977,
4500597481,
4500597772,
4500598240,
4500596221,
4500602134,
4500600745,
4500597773,
4700023997,
4500596787,
4500596788,
4500602533,
4500602534,
4500591637,
4500601632,
4500601635,
4500603045,
4500603046,
4500602361,
4500602790,
4500598435,
4500603133,
4300015436,
4500606742,
4500598431,
4500604482,
4500602860,
4500598449,
4500598527,
4500598535,
4500598525,
4500595947,
4500600567,
4500597451,
4500597830,
4500600430,
4500599679,
4500598638,
4500598643,
4500597598,
4500596910,
4500597838,
4500597720,
4500597898,
4500602186,
4500598778,
4500601856,
4500602425,
4500597070,
4500599799,
4500602705,
4300015317,
4500597359,
4500606402,
4500603343,
4500599721,
4500604185,
4500602563,
4500603816,
4500603819,
4500605352,
4500605359,
4500605344,
4500604320,
4500605443,
4500599168,
4500604106,
4500592075,
4500604609,
4300015308,
4500599128,
4500599092,
4500617947,
4500619074,
4500616382,
4500618651,
4500618652,
4500619121,
4500619117,
4300014561,
4500615093,
4500614967,
4500614966,
5100016951,
4500614523,
4500614527,
4500611121,
4500611772,
4500615495,
4500608539,
4500615810,
4500610375,
4500616168,
4500612996,
4500610383,
4500616689,
4500616704,
4500616240,
4500617056,
4500609294,
4500616609,
4500616610,
4500608537,
4700024274,
4500618233,
4500608957,
4500608948,
4500608951,
4500608963,
4500608549,
4500609302,
4500609303,
4500617210,
4500618626,
4500618630,
4500618671,
4500609309,
4500609313,
4500617403,
4500608560,
4500608556,
4500618738,
4500618801,
4500618829,
4500610498,
4500619167,
4500619191,
4500619190,
4500610427,
4500610431,
4500610430,
4500611327,
5100017015,
4500618672,
4500619067,
4500609393,
4700024384,
4500612684,
4500612682,
4500615239,
4500609078,
4500609082,
4500609088,
4500613195,
4500608363,
4500615261,
4500609093,
4500610194,
4500610200,
5100016990,
5100016991,
4500617526,
4500609109,
4500609539,
4500610201,
4500610956,
4500611255,
4500617555,
4500610275,
4500610276,
4700024803,
5100017027,
4500618167,
4500614930,
4500617437,
4500617981,
4500610282,
4500610300,
4500610296,
4500610289,
4500615138,
4500618773,
4500616843,
4500617158,
4500617164,
4500617155,
4500619228,
4500619229,
4500609398,
4500618890,
4500618892,
4500618955,
4500617817,
4500610619,
4500610614,
4500610615,
4500610616,
4500614455,
4300015752,
4500619175,
4500619179,
4500619181,
4500610843,
4500617910,
4500618130,
4500618132,
4500608214,
4500610113,
4500617322,
4500619243,
4500619244,
4500619245,
4500591446,
4500619290,
4500619411,
4500619412,
4500619607,
4500619614,
4500619617,
4500619669,
4500619409,
4500619469,
4500619470,
4500619514,
4500619523,
4500619535,
5100015610,
5100015884,
4500574397,
4500571776,
4500489581,
4500567934,
4500566977,
4500567766,
4500570385,
4500566033,
4500587693,
4500583784,
4500581341,
4500583774,
4500584937,
4500582969,
4500575764,
4500584347,
4500587421,
4500579526,
4500587565,
4500576044,
5100016339,
4500582587,
4500585011,
4500584522,
4500577661,
4500582643,
4500581136,
4500585640,
4500580213,
4500591493,
4500591483,
4500591528,
4500590903,
4500594956,
4500591026,
4500591070,
4500588453,
4500592346,
4500592505,
4500592507,
4500595121,
4500595126,
4500591087,
4500592071,
4500592079,
4700023768,
4500592513,
4500594787,
4500595003,
4500591644,
4500588362,
4500592604,
4500587930,
4500591645,
4500597991,
4500591797,
4500591666,
4500592078,
4500596537,
4500590496,
4500590740,
4500591685,
4500593714,
4500593723,
4500593763,
4500593761,
4500591482,
4700023881,
4500593778,
4500593994,
4500591512,
4500591516,
4700023903,
4500591391,
4500588757,
4500593191,
4500591829,
4500589794,
4500589803,
4500595781,
4700023934,
4500589808,
4500589814,
4500593938,
4700023883,
4500593941,
4500595437,
4500589064,
4500591941,
4500595854,
4500591083,
4500595185,
4700023752,
4500595680,
4500591341,
4500593209,
4500593205,
4300014947,
4500591354,
4500594270,
4300014857,
4500593180,
4500595725,
4500591029,
4500595331,
4500594497,
4500591021,
4500595779,
4500605481,
4500604014,
4500602072,
4500603962,
4500598530,
4700024167,
4500599212,
4500596719,
4500605566,
4500605570,
4500599615,
4500599616,
4500602616,
4500607806,
4500607102,
4500596542,
4500596551,
4500597410,
4700023970,
4500607117,
4500597452,
4500601193,
4500599853,
4500591395,
4500597992,
4500602510,
4500595481,
4500602907,
4500601636,
4500607640,
4500607639,
4500603132,
4300015433,
4300015438,
4500607098,
4500607707,
4500604009,
4500604022,
4500604018,
4500595945,
4500595951,
4500596300,
4500599134,
4500597831,
4500594471,
4500597596,
4500597672,
4500597841,
4500597839,
4500598647,
4500597846,
4500601715,
4500598775,
4500596115,
4500602397,
4500599794,
4700024020,
4500602708,
4300015316,
4500603347,
4500599725,
4500604019,
4700024126,
4500600440,
4500605350,
4500605361,
5100016745,
4500604321,
4500604369,
4500604592,
4500604601,
4500595989,
4500596965,
4500605439,
4500601065,
4700024119,
4500604596,
4500603126,
4500591660,
4300015314,
4500605918,
4500605929,
4500604630,
4500599078,
4300015431,
4500603766,
4500613355,
4500612726,
4500614549,
4500618649,
4500613681,
4700024586,
4500611118,
4500611593,
4500615655,
4500615719,
4500611794,
4700024696,
4500615938,
4500610373,
4500615723,
4500615823,
4500616149,
4500616301,
4500611957,
4500613102,
4500616059,
4500616060,
4500610378,
4700024710,
4500609278,
4500609287,
4500616404,
5100016979,
4500617272,
4500616606,
4500616608,
4500608536,
4500608545,
4500617418,
4500608949,
4500618512,
4500617000,
4500618625,
4500608559,
4500618740,
5100017043,
4500618797,
4500618800,
4500609315,
4500609317,
4500617672,
4500617673,
4500608696,
4500618833,
4500618910,
4500619124,
4500607710,
4500608700,
4500608692,
4500619166,
4500610695,
4500610706,
4500611717,
4500618478,
4500611385,
4500613240,
4500619068,
4700024366,
4500612358,
4500612674,
4500612677,
4500610457,
4500610485,
4500607871,
4500612679,
4700024385,
4500612681,
4500612683,
4500611221,
4500610293,
4500615207,
4500615240,
4500615248,
4500609089,
4500613027,
4500615512,
4500609098,
4500609104,
4300015616,
4300015617,
4500615992,
4500607930,
4500609526,
4700024749,
4500610274,
4500618165,
4500611638,
4500617986,
4500610278,
4500610281,
4500617243,
4500615175,
4500610295,
4500610290,
4500610408,
4500610412,
4500616537,
4500616536,
4700024720,
4500617803,
4500617802,
4500614244,
4500614724,
4500610790,
4500610800,
4500614725,
4700024639,
4500618545,
4500608818,
4500616844,
4700024725,
4500618903,
4500618947,
4500619014,
4500614446,
4500617377,
4500617908,
4500617907,
4500617906,
4500610612,
4500614452,
4500614797,
4500614802,
4500619169,
4700024299,
4500610851,
4500610109,
5100016988,
4500619414,
4500619410,
4500619407,
4500619667,
4500619464,
4500619519,
4500619537,
4700022526,
4500560302,
4500560303,
5100015906,
4500560412,
4500563643,
4500572115,
4500572044,
4500569424,
4500566095,
4500573571,
4500565869,
4500567233,
4500573337,
4500573871,
4500586069,
4700023176,
4500579239,
4500587104,
4500580010,
4500586252,
4500583250,
4500583768,
4500583770,
4500583776,
4500582501,
4500584897,
4500585498,
4500586313,
4500578627,
4500575494,
5100016338,
4500574496,
4500578418,
4700023164,
4500586747,
4500577317,
4500585013,
4500585064,
4500585197,
4500580512,
4500582648,
4500584761,
4500591530,
4500595119,
4500592206,
4500592499,
4500591147,
4500592089,
4700023770,
4500589957,
5100016564,
4500594785,
4500595073,
4700023771,
4700023775,
4500591650,
4500592501,
4500592603,
4500588457,
4500591654,
4500591113,
4500591156,
4500591663,
4500596532,
4500592093,
4500590843,
4500593010,
4500590893,
4500591676,
4500591673,
4500591671,
4500591691,
4700023761,
4500591767,
4500592884,
4500593728,
4500593757,
4500591478,
4500590909,
4500591506,
4500591505,
4500591517,
4500590923,
4500591511,
4500594725,
4500591387,
4500593183,
4500599825,
4500589802,
4500593082,
4500593081,
4500589394,
4500589395,
4500589811,
4500593940,
4500593948,
4500590787,
4300014808,
4500594001,
4700023928,
4500591347,
4500595438,
4300014957,
4500591337,
4500591339,
4500595682,
4500591340,
4700023758,
4500593197,
4500592245,
4500591669,
4500588715,
4500594425,
4500591603,
4500595483,
4500595486,
4500593182,
4500593385,
4500593642,
4500595719,
4500594472,
4500592248,
4500592661,
4500594474,
4500592024,
4300014955,
4500591032,
4700023642,
4500595203,
4500591385,
4500604494,
4500605483,
4500604082,
4500602073,
4500603957,
4500598529,
4500605281,
4500605794,
4500605291,
4500605872,
4500607115,
4500607111,
4500607108,
5100016771,
4500602751,
4500606997,
4500607128,
4500596543,
4700023964,
4500596553,
4500595130,
4500600460,
4500600602,
4500600641,
4500596909,
4500607130,
4500597022,
4500597109,
4500598801,
4500607286,
4500607431,
4500597453,
4500597775,
4500597027,
4500601682,
4500598235,
4500598245,
4500598241,
4500607523,
4500607521,
4500607529,
4500596227,
4500601688,
4500602490,
4500601638,
4500603043,
4500603125,
4500602136,
4500597963,
4500603690,
4500607126,
4500604484,
4500602792,
4500607705,
4500607799,
4500607794,
4500598524,
4500604007,
4500595943,
4500595946,
4500596174,
4500601282,
4500598637,
4500598633,
4500598640,
4500598641,
4500597678,
4500599475,
4500601451,
4500601454,
4500600439,
4500597717,
4500606242,
4700023996,
4500598777,
4500598780,
4500600383,
4500599750,
4500599330,
4500603351,
4500603085,
4500599726,
4500600391,
4500600303,
4500604182,
4500605360,
4500605347,
4500605363,
4500604594,
4500604602,
4500605441,
4500605514,
4500599171,
4700024121,
4500604606,
4500603732,
4500603735,
4500603887,
4500605522,
4500605609,
4500603202,
4500592096,
4300015309,
4500605159,
4500603297,
4500599129,
4500613578,
4500613665,
4500619072,
4500616493,
4500613968,
4500614413,
4500618653,
4500596516,
4500613686,
4500614518,
4500616333,
4700024541,
4300015746,
4500615659,
4500615720,
4700024687,
4500610376,
4500616148,
4700024708,
4500611958,
4300015680,
4500616058,
4500610377,
4500610379,
4500609285,
4700024273,
4500608547,
4500617417,
4700024814,
4500618475,
4500608945,
4500616776,
4500618511,
4500618632,
4700024828,
4700024829,
4700024830,
4500609307,
4500609310,
4500617647,
4500617664,
4500608689,
4500618830,
4600008329,
4500610500,
4500617860,
4500619189,
4500610693,
4500610705,
5100017016,
5100017019,
4500610429,
4500610428,
4500618483,
4500618593,
4700024401,
4500619070,
4500612676,
4500607881,
4500611248,
4500607869,
4500609084,
4500609086,
4500614189,
4500608358,
4500615267,
4700024674,
4500609099,
4500609102,
4500614193,
4500614191,
4500615606,
4500616037,
4500608797,
4500608805,
4500616584,
4500616586,
4300015797,
4500610196,
4500608808,
4500608811,
4500609108,
4500609898,
4500617071,
4500617619,
4500617551,
4500619232,
4500610272,
4500617529,
4500618209,
4500611635,
4500613924,
4500617982,
4500610277,
4500613739,
4500610288,
4500610294,
4500610471,
4500616539,
4500610796,
4500610787,
4500618368,
4500618369,
4500616732,
4500617425,
4500615202,
4500618739,
4500618776,
4500618888,
4500616842,
4500614734,
4500609391,
4500618898,
5100017049,
4500614447,
4500617378,
4500617380,
4500610840,
4500619177,
4500619195,
4500610850,
4500617911,
4500618083,
4500610110,
5100016989,
4500619233,
4500619468,
4500619637,
4500619635,
4300014518,
4500619517,
4500619525,
4500619530,
4500604253,
4500618886";

        string dcnwoaging_oracle = $@"   
SELECT *
                      FROM (SELECT TTTT.*,
                                   ROW_NUMBER() OVER(PARTITION BY sysserialno ORDER BY LoadingTime desc) numbs
                              FROM(
SELECT 'SMT' PRODTYPE,S.sysserialno,S.workorderno,E.SKUNO,
case when S.currentevent in('CBS','STOCKIN') then 'FG' else 'PROD' end ProdStatus,S.currentevent,S.nextevent,E.scandatetime LoadingTime,
                               CASE
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 30 then
                                  '<30'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 60 then
                                  '30<day<=60'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 90 then
                                  '30<day<=90'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 120 then
                                  '60<day<=120'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 180 then
                                  '120<day<=180'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 270 then
                                  '180<day<=270'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 360 then
                                  '270<day<=360'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) > 360 then
                                  '>360'
                               END Aging,
							   DATEDIFF(day, E.scandatetime, getdate()) AgingDays
                          FROM mfworkstatus S, mfsysevent E ,dbo.mfworkorder D
                         WHERE S.sysserialno = E.sysserialno
						 AND S.workorderno= D.workorderno
						   and S.workorderno  =E.Workorderno
                           AND S.shipped = 0
						   AND S.Quited=0
                           AND S.currentevent <> 'MRB'
                           AND E.eventname ='SMTLOADING'
						                  and d.factoryid in ('NOEA')
                                                AND d.workorderno NOT LIKE '%999%'
                                                AND s.sysserialno NOT LIKE '~%'
                           AND E.eventpass = 1
                           AND S.repairheld = 0
                           AND exists (select top 1 1
                                  from mfworkorder
                                 where workorderno = S.WORKORDERNO
                                   AND CLOSED = 0
								   )
)TTTT
)nu WHERE numbs = 1
order by agingdays";

        string dcnwoaging_dcn = $@"   
SELECT *
                      FROM (SELECT TTTT.*,
                                   ROW_NUMBER() OVER(PARTITION BY sysserialno ORDER BY LoadingTime desc) numbs
                              FROM(
SELECT 'SMT' PRODTYPE,S.sysserialno,S.workorderno,E.SKUNO,
case when S.currentevent in('CBS','STOCKIN') then 'FG' else 'PROD' end ProdStatus,S.currentevent,S.nextevent,E.scandatetime LoadingTime,
                               CASE
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 30 then
                                  '<30'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 60 then
                                  '30<day<=60'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 90 then
                                  '30<day<=90'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 120 then
                                  '60<day<=120'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 180 then
                                  '120<day<=180'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 270 then
                                  '180<day<=270'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) <= 360 then
                                  '270<day<=360'
                                 WHEN DATEDIFF(day, E.scandatetime, getdate()) > 360 then
                                  '>360'
                               END Aging,
							   DATEDIFF(day, E.scandatetime, getdate()) AgingDays
                          FROM mfworkstatus S, mfsysevent E ,dbo.mfworkorder D
                         WHERE S.sysserialno = E.sysserialno
						 AND S.workorderno= D.workorderno
						   and S.workorderno  =E.Workorderno
                           AND S.shipped = 0
						   AND S.Quited=0
                           AND S.currentevent <> 'MRB'
                           AND E.eventname ='SMTLOADING'
						                  and d.factoryid in ('NBEA')
                             AND d.workorderno LIKE '002%'
                                                AND s.sysserialno NOT LIKE '~%'
                           AND E.eventpass = 1
                           AND S.repairheld = 0
                           AND exists (select top 1 1
                                  from mfworkorder
                                 where workorderno = S.WORKORDERNO
                                   AND CLOSED = 0
								   )
)TTTT
)nu WHERE numbs = 1
order by agingdays";

        string meswoaging = $@"select distinct decode(B.current_station,
                       'SMTLOADING',
                       'SMT-PCBA',
                       'SILOADING',
                       'SI-SYSTEM') AS PRODTYPE,
                c.plant,
                to_char(A.SN) as sysserialno,
                A.WORKORDERNO,
                A.SKUNO,
                decode(A.CURRENT_STATION,
                       'CBS',
                       'FG',
                       'STOCKIN',
                       'FG',
                       'PROD') AS ProdStatus,
                A.REPAIR_FAILED_FLAG REPAIRHELD,
                A.CURRENT_STATION,
                A.NEXT_STATION,
                a.start_time LoadingTime,
                case
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 30 then
                   '<30'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 60 then
                   '30<day<=60'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 90 then
                   '30<day<=90'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 120 then
                   '60<day<=120'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 180 then
                   '120<day<=180'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 270 then
                   '180<day<=270'
                  when ROUND(TO_NUMBER(sysdate - A.START_TIME)) <= 360 then
                   '270<day<=360'
                  else
                   '>360'
                end AS aging,
                TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as agingdays
  from r_sn a, r_sn_station_detail b, r_wo_base c
 where a.valid_flag = '1'
   and a.completed_flag = '0'
   and A.sn not like '*%'
   and A.sn not like '#%'
   and A.sn not like 'del%'
   and c.closed_flag = '0'
   and A.workorderno not like '%TEST%'
   AND A.SN NOT LIKE '%TEST%'
   AND A.workorderno LIKE '002%'
   and a.sn = b.sn
   and b.current_station like '%LOADING'
   and a.workorderno = c.workorderno
   and a.current_station not in ('STOCKIN')
   and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
 order by a.next_station";



        string[] fvnlist = $@"20210827032551318450058842500120,
20210827173613564450058842500080,
20210827173613564450058842500220,
20210827173613564450058842500230,
20210827173624939450058842500130,
20210827174037397450058842500010,
20210907165613517450059018100010,
20210909162235369450058972100010,
20210909162905972450058972100010,
20210910085910999450059018100010,
20210910090800408450059018100010,
2021091417154866450059018100010,
20210915155937615450059018100010,
20210917235314485450059427600010,
20210918165024672450059427600010,
20210920174158867450059427700010,
20210920175125455450059427600010,
20210920175400614450059427700010,
20210921150513332450059427700010,
20210922173459965450059427800020,
20210922193330728450059203100010,
20210927193518342450059203100020,
20210927232901764450059679000070,
20210927233049296450059680500020,
20210927235315287450059681700010,
20210928012743418450059691300040,
20210928012743418450059691300420,
20210928173900631450059680500020,
20210929231433259450059018100010,
20210929231553453450059018100010,
20210930110902445450059684000010,
20210930110926454450059684000010,
20210930123259763450059685500030,
20210930123259763450059685500040,
20210930162520475450059682700010,
20210930162538396450059682700030,
20210930174257653450059684000010,
20211001113750118450059878200040,
20211001170354437450059679100030,
20211002100816681450059683100030,
20211002100920853450059683100030,
20211002180230778450059878200020,
20211002180230778450059878200030,
20211002180230778450059878200080,
20211002180249422450059878200060,
20211004162721589450059682700020,
20211004163039837450059682700030,
2021100416330997450059681100010,
20211004163335319450059683800040,
20211004163335771450059683200020,
20211004163348458450059683100080,
20211004163451958450059683000100,
2021100416352190450059684100010,
20211004163920483450059878200060,
20211004163920483450059878200070,
20211004164053817450059717400040,
20211004164134217450059717400040,
20211004164243428450059878200010,
20211004164243428450059878200020,
20211004164243428450059878200040,
20211004164243428450059878200070,
20211004164400979450059018100010,
20211004164439565450059691300070,
20211004164439565450059691300300,
20211004164450881450059691300050,
20211004164450881450059691300200,
20211004172203483450058842500140,
20211004172203483450058842500180,
20211004172203483450058842500210,
20211004172310817450058842500040,
20211004172310817450058842500050,
20211004172310817450058842500140,
20211004172310817450058842500170,
20211004172310817450058842500200,
20211004172310817450058842500220,
20211004174411368450059878200080,
20211004174411368450059878200090,
2021100513532662450059878200010,
2021100513532662450059878200020,
2021100513532662450059878200030,
2021100513532662450059878200060,
2021100513532662450059878200070,
20211005144846782450059684100010,
20211006044037567450059683000040,
2021100609013527450059427700010,
20211006112923796450059878200090,
20211006134333433450059954600020,
20211006152747629450059680500020,
20211006160600407450059680500020,
2021100622400771450059683000080,
20211007101458574450059683000050,
20211007103136224450059680500020".Replace("\r\n", "").Split(',');

        string[] fjzlist = $@"20210907100738487450058995700010,
20210907232449208450059083800050,
20210907232500593450059083900010,
20210907234057524450059084100050,
20210908013221667450059089800040,
20210908013221667450059089800080,
2021090801325150450059091700040,
2021090801325150450059091700050,
20210908013305575450059089900030,
20210908013313362450059089400100,
20210908013320462450059092300030,
20210908013347143450059089200030,
20210908013355116450059092100050,
20210908013355116450059092100070,
20210908013419412450059089500030,
20210908013423403450059089600010,
20210908013429808450059089700040,
20210908013435819450059090300030,
20210908013435819450059090300050,
20210908013442579450059090000070,
20210908013452497450059092500020,
20210908033036439450059100200050,
20210908033045528450059100300030,
20210908033045528450059100300060,
20210908035042246450059100400190,
2021090807335240450059111300030,
20210908093735613450059100400110,
20210908093735613450059100400140,
20210908093735613450059100400150,
20210908093735613450059100400160,
20210908093735613450059100400170,
20210908103303655470002372800030,
20210908113526912450059115800010,
20210908214612587450059099600020,
20210909014005349450059136900040,
20210909014010245470002375100010,
20210909053943850450059151100030,
20210909054136888450059150700030,
20210909054224363450059150800030,
2021090909502313450059133200010,
20210909113423939450059089500030,
20210909113423939450059089500040,
20210909113441979450059089500030,
20210909113455738450059091700010,
20210909113455738450059091700080,
20210909113455738450059091700100,
20210909113457433450059089700030,
20210909113458267450059092100020,
20210909113458267450059092100040,
20210909113458267450059092100060,
20210909113458267450059092100070,
20210909113459937450059091700080,
20210909113500711450059089700030,
20210909113505284450059089200050,
20210909113505284450059089200060,
20210909113505284450059089200100,
20210909113505284450059089200120,
20210909113505284450059089200150,
20210909113506133450059092400070,
20210909113506886450059090000020,
20210909113506886450059090000040,
20210909113512912450059089300020,
20210909113518159450059084100030,
20210909113525243450059084000010,
20210909113525243450059084000050,
20210909113533604450059092300020,
20210909113533604450059092300030,
20210909113537472450059089400040,
20210909113537472450059089400110,
20210909113558512450059092100040,
2021090911355889450059089200120,
20210909113606172450059092100010,
20210909113606172450059092100030,
20210909113606172450059092100070,
20210909113606172450059092100090,
20210909113616127450059089500040,
20210909113616127450059089500050,
20210909113623980450059089900070,
20210909113623980450059089900090,
20210909113638616450059090300010,
20210909113638616450059090300040,
20210909113641187450059089200080,
20210909113654821450059090300100,
20210909113658148450059090300100,
20210909113658842450059089600020,
20210909113658842450059089600030,
20210909113700575450059089300050,
20210909113702514450059089600030,
20210909113702514450059089600040,
20210909113709918450059084100030,
20210909113709918450059084100050,
2021090911371278450059089700020,
20210909113722828450059089800080,
202109091137246450059089800100,
20210909113725827450059090000100,
20210909113728973450059084100050,
20210909113734871450059084000020,
20210909113736953450059083900020,
20210909113740765450059091700010,
20210909113740765450059091700030,
20210909113740765450059091700080,
20210909113753244450059084000010,
20210909113753244450059084000030,
20210909113753244450059084000040,
20210909113809297450059083800020,
20210909113809297450059083800050,
20210909113809501450059163600010,
20210909113817871450059165000150,
20210909113817871450059165000210,
20210909113817871450059165000260,
20210909113817871450059165000390,
20210909113817871450059165000430,
20210909113817871450059165000440,
20210909113817871450059165000450,
20210909113823713450059092200020,
20210909113826908450059092200030,
20210909113826908450059092200090,
20210909113828727450059089100010,
20210909113839121450059083800010,
20210909113854685450059089100040,
20210909113905404450059092300060,
20210909113905404450059092300070,
2021090911390987450059164700020,
20210909113911381450059092400080,
20210909113921772450059089900030,
20210909113929458450059089800090,
20210909113939957450059089400120,
20210909113943158450059089100050,
202109091139438450059092200010,
202109091139438450059092200040,
2021090911395147450059090900040,
2021090911395147450059090900050,
2021090911395147450059090900100,
20210909114003289450059090900020,
20210909114003289450059090900050,
20210909114003289450059090900060,
20210909114003289450059090900100,
20210909114003301450059089400020,
20210909114003301450059089400040,
20210909114003301450059089400070,
20210909114003301450059089400110,
20210909114009841450059090900030,
20210909114009841450059090900060,
20210909114009841450059090900100,
20210909115015906450059166000010,
20210909115057429450059166700010,
20210909115109951450059165400050,
20210909115109951450059165400070,
20210909115109951450059165400170,
2021090911520433450059165200260,
2021090911520433450059165200450,
20210909134522720450059171000110,
20210909134536224450059171300020,
20210909134536224450059171300030,
20210909134546684450059171100260,
20210909134546684450059171100400,
20210909134549551450059170900120,
20210909145148886450059100700010,
2021090916282453450059135100030,
20210909172833217450059135100020,
20210909172833217450059135100030,
20210909173235908450059133200010,
20210909210532272470002372800010,
20210909210532272470002372800020,
20210909211605932450059099600010,
20210909211605932450059099600020,
20210910005622175450059166000010,
20210910012509758510001655000010,
2021091002060013450059134500010,
20210910022157964450059092600010,
20210910022227515450059153900010,
20210910022252881450059138100010,
20210910022426175450059134400020,
20210910032646916450059198600050,
20210910062354518450059166700010,
2021091008225322450059151300020,
2021091008225322450059151300060,
2021091008231516450059150500010,
20210910082453458450059134600010,
20210910082557506470002375800010,
20210910082734734450059133200020,
20210910082802176450059150700030,
20210910082806952450059134600020,
20210910082811197470002375600010,
20210910083012984450059151100050,
20210910083031822450059133200010,
20210910083257384450059150500010,
20210910084505369450059166200010,
20210910084538741450059166000010,
20210910085514211450059160900010,
20210910085643782450059171100060,
20210910085643782450059171100240,
20210910085643782450059171100260,
20210910085643782450059171100310,
20210910085721217450059100700020,
20210910092352426450059163700020,
20210910123225281450059166000010,
20210910183117384450059092700020,
2021091018313259450059092800010,
20210910233754940450059092500020,
202109102343035450059092800010,
20210911004944295450059133200010,
20210911014725739450059208800010,
2021091104384066510001655700040,
20210911064722113510001655700040,
20210911065126957450059208800010,
20210911074547752450059171100290,
20210911074547752450059171100350,
20210911074547752450059171100390,
20210911074547752450059171100450,
20210911083148373450059207300010,
2021091109480343450059208200030,
20210911105434527450059208200020,
20210911132644153450059163600010,
20210913212700558450059224600040,
2021091321381289510001655000010,
20210913213842911450059092600010,
20210913213852224450059224600030,
20210913214142794470002372800020,
20210914000959992470002372800030,
2021091409444872450059115800010,
20210914094635477450059092600010,
20210914094641442450059110900010,
20210914094649163450059092600010,
20210914095122406450059115700010,
20210914095248736450059100300050,
2021091409543564450059198600040,
2021091409543564450059198600050,
20210914095757213450059084300020,
20210914095953736450059115600010,
20210914100020925450059110900010,
20210914100029773450059224400010,
20210914100653997450059100400050,
20210914100653997450059100400090,
20210914100653997450059100400200,
20210914100653997450059100400220,
20210914100829496450059102600020,
20210914220716231450059224600010,
20210915011609445450059198600030,
20210915011631157450059198600010,
20210915052409242450059318000010,
20210915093348848510001655600010,
20210915235511943450059134700010,
20210916034107191450059207800020,
20210916034107191450059207800030,
20210916034132886450059166400010,
202109160342311450059134400030,
202109160342311450059134400060,
20210916034349911450059208000030,
20210916034447444450059136900030,
20210916034532840450059134400050,
20210916050501103450059165700010,
20210916074855290450058995700010,
20210916093054960450059372700040,
20210916111520384450059100700010,
20210916230833914450059224600020,
20210917013453112450059393900010,
20210917025721834450059208800010,
20210917032931495450059399500080,
2021091703404449450059399900020,
20210917071833654450059377000010,
20210917074154649450059138100020,
20210917074328748510001660800010,
20210917131615738450059166900010,
20210917182151938450059092800010,
20210917210025142450059318000010,
20210917210632746450059108000020,
20210918054240671450059442500010,
20210918092531427450059447100010,
20210918101606718450059224600020,
20210918101606718450059224600030,
20210918101606718450059224600040,
20210918103044750450059099600010,
20210918103049761450059448600010,
20210918103323436450059099100020,
20210918103532108450059100200070,
20210918103934312450059110800010,
20210918110117185470002388500040,
2021091907540582450059446000020,
20210919082335418450059393900010,
20210919083005339450059318000010,
20210919083239799450059161100010,
20210919083316524450059160900010,
20210919083409857450059208800010,
20210919224937352510001660800010,
20210920011043326450059092600010,
20210920013808558450059212900050,
2021092002023266450059084300010,
20210921071224674450059134700010,
20210921205810756450059447300010,
20210922003050339450059472900010,
20210922041516807470002372800010,
20210922041538501450059100400040,
20210922041913372450059249500010,
20210922041922323450059108000010,
20210922041949875450059100400030,
20210922042041984450059249500020,
20210922071437259450059135100020,
20210922080059271450059212900020,
20210922080059271450059212900040,
20210922093257348450059110900010,
20210922093822446450059472900010,
20210922094030280450059400000010,
20210922094106958450059447000020,
20210922094256951450059399900010,
20210922094455511450059393900010,
20210922094641711450059399500070,
20210922094653939450059447300010,
20210922142053446450059209600010,
20210922142053446450059209600020,
20210922142053446450059209600030,
20210922211042480450059194100020,
20210922211042480450059194100030,
20210922225645963450059099600010,
20210923000307781450059212900040,
20210923000702428450059133100010,
20210923005904822450059133100010,
20210923073125908450059399100010,
20210923074034238450059111100010,
2021092307415913450059400000010,
20210923074248682450059400300010,
20210923121816752450059212900040,
20210923153415240450059543600010,
20210924045444425450059151200050,
2021092413220841450059166900020,
20210924141526428450059446000010,
20210924154508464450059139500010,
20210924154508464450059139500030,
20210924154519440450059139500010,
20210924220150949450059100500010,
20210924220851672450059103300020,
20210925014313655450059622000010,
20210925032611313450059629900010,
20210925094838619450059629900120,
20210925095950741450059447100010,
20210925210738542450059578400010,
20210925211359324450059578400010,
20210925234231387450059399900010,
20210925234231387450059399900020,
20210926083609105450059447300010,
202109260856286450059212900060,
20210926095007527450059212900010,
20210926095007527450059212900060,
20210927144510298450059442500020,
20210928084806348450059163600010,
2021092809081170450059548100010,
202109280930059450059318000010,
20210928093944369450059448600010,
20210928095210136450059208200030,
20210928095223898450059448500010,
20210928095706498450059634800010,
20210928100154437450059629700010,
20210928102031112450059447100010,
20210928102249300450059629800010,
20210928102339826450059629900130,
20210928102339826450059629900200,
20210928105241951450059629800010,
20210928111828601450059224800010,
20210928125537528450059150400040,
20210928131737496450059150400010,
20210928131737496450059150400030,
20210928131737496450059150400040,
20210928133051907450059150600010,
20210928160947478450059163700010,
20210928160947478450059163700020,
20210929001855600470002396700040,
2021092901244119470002396700050,
202109290724284450059399100010,
20210929074424752450059548100010,
20210929092638911450059629900190,
20210929093911112450059702300010,
20210929104557871450059166000010,
20210929153019565450059716900010,
20210929220935174450059740900020,
2021092923143758450058995700010,
20210929231513697450059716900010,
20210929231632249470002375800010,
20210929231654366450058995700010,
20210929232657356450059212900030,
20210929232657356450059212900060,
20210930013850794450059771800040,
20210930013850794450059771800080,
20210930021501177450059194100030,
20210930062318524450059110900010,
20210930062446560450059629700010,
20210930082618350450059629900050,
20210930082618350450059629900070,
20210930082618350450059629900090,
20210930083111587450059153900010,
2021093008333856450059629900090,
20210930084610399450059548100010,
20210930084610399450059548100020,
20210930090714608450059139500010,
20210930160736171450059777300010,
20210930161407542450059138100010,
20210930161407542450059138100020,
20210930162124671450059707000010,
20210930162124671450059707000020,
2021093016230810450059135100020,
20210930183339813450059138100020,
20210930215439615450059799100020,
20210930215439615450059799100040,
20210930224521219450059164800010,
20210930232815626510001655600040,
2021093023370681450059212900040,
20211001035420189450059843000010,
20211001051422811450059777300010,
20211001052644159450059768000010,
20211001054815870450059853300010,
20211001054841446450059645800050,
20211001061645370450059134600020,
20211001061645370450059134600030,
20211001075554225450059799100040,
20211001085700347450059164700010,
20211001090603150450059645800020,
2021100113330649450059880600010,
20211001212006677450059799100010,
20211002070613655450059134700020,
20211002070831688450059207500010,
20211002093522162450059771800010,
20211002093522162450059771800040,
20211002093723100450059702700010,
20211002093857234450059728900010,
20211002094023444450059777300010,
20211002111646696450059212900010,
20211002140721388450059372700030,
2021100214111938450059690600010,
20211002141319737450059702400010,
20211002141349619450059707000020,
20211004172239775450059853300010,
20211004172253842450059853300010,
20211004172259546450059151100040,
20211004172259546450059151100080,
20211004174702113450058995700010,
20211004222659176450059100700020,
20211004230333455450059843100010,
20211005053323558450059921200010,
20211005085431664450059889500010,
20211005102329196450059161200010,
20211005143536506450059099600020,
20211005155541629450059099600020,
2021100522031315450059629700010,
20211005234914927510001655700020,
20211005234914927510001655700030,
20211006011857727450059163700020,
20211006060757762450059151100070,
20211006083541525450059880600010,
20211006084317207450059843000020,
20211006085459311450059799100020,
20211006224914835450059198600010,
20211006225213282450059224400010,
20211007052319638450059980000020,
20211007072717306450059982500080,
20211007084345591470002375600010,
20211007092955180450059984700010,
20211007102353320450059843100010,
T20210619001009T45999990200010,
T20210620001009T45999990100010,
T20210621001009T45999990300010".Replace("\r\n", "").Split(',');


        string czsns = $@"DUS4535S060,
FMJ4534S0EL,
FMJ4534S015,
FMJ4534S1FE,
DUS4534S04W,
FMJ4534S15A,
FMJ4534S1HH,
FMJ4534S1P4,
FMJ4534S0FT,
FMJ4534S0L9,
DUS4534S0DA,
DUS4534S009,
DUS4535S01D,
FMJ4534S0TM,
FMJ4534S1HE,
FMJ4534S1ER,
DUS4534S0BL,
FMJ4534S173,
DUS4535S01M,
FMJ4534S0SE,
FMJ4534S1L8,
DUS4534S03X,
FMJ4534S10L,
FMJ4534S0WN,
FMJ4534S09F,
DUS4534S0A9,
FMJ4534S0NT,
DUS4535S058,
FMJ4534S1SJ,
FMJ4534S1JL,
DUS4534S04F,
FMJ4534S113,
FMJ4534S08W,
DUS4535S06G,
FMJ4534S18A,
FMJ4534S0NA,
DUS4534S0FE,
FMJ4534S0FB,
FMJ4534S15L,
FMJ4534S1RR,
DUS4534S005,
FMJ4534S1SG,
FMJ4534S158,
FMJ4534S1KX,
DUS4534S0G2,
DUS4535S02E,
FMJ4534S1N2,
FMJ4534S1DY,
DUS4534S057,
FMJ4534S0CZ,
FMJ4534S1SA,
DUS4535S02B,
FMJ4534S1P1,
FMJ4534S187,
FMJ4534S0S1,
DUS4534S06M,
FMJ4534S07Z,
DUS4535S007,
FMJ4534S1K9,
FMJ4534S1GH,
DUS4535S06R,
FMJ4534S07J,
DUS4534S0HA,
FMJ4534S1D9,
FMJ4534S1KE,
FMJ4534S1H9,
DUS4535S05F,
FMJ4534S0Z9,
FMJ4534S106,
FMJ4534S05J,
FMJ4534S02R,
DUS4534S0G6,
FMJ4534S0VK,
FMJ4534S0MW,
DUS4534S0D4,
DUS4535S085,
FMJ4534S12E,
FMJ4534S0ZD,
FMJ4534S1B9,
FMJ4534S095,
DUS4534S0HR,
FMJ4534S1KZ,
FMJ4534S11C,
DUS4534S00V,
FMJ4534S0LG,
FMJ4534S0CG,
DUS4534S0HZ,
DUS4535S04X,
FMJ4534S1V8,
FMJ4534S1E3,
DUS4535S02T,
FMJ4534S1PF,
FMJ4534S1N7,
DUS4534S0C5,
FMJ4534S00H,
FMJ4534S0T2,
DUS4535S08W,
FMJ4534S0KL,
FMJ4534S06H,
DUS4534S05E,
FMJ4534S029,
FMJ4534S0VM,
DUS4535S03D,
FMJ4534S038,
FMJ4534S0JB,
FMJ4534S08P,
FMJ4534S1AD,
DUS4534S04L,
FMJ4534S1CG,
DUS4534S07N,
FMJ4534S0FE,
DUS4535S03G,
FMJ4534S152,
FMJ4534S1EK,
FMJ4534S1DM,
DUS4534S02B,
FMJ4534S0W4,
DUS4535S02C,
FMJ4534S1M6,
FMJ4534S103,
DUS4534S0JM,
FMJ4534S0FW,
FMJ4534S0F9,
FMJ4534S1D0,
DUS4534S081,
FMJ4534S117,
DUS4534S0CS,
FMJ4534S1D1,
FMJ4534S167,
DUS4535S067,
FMJ4534S035,
FMJ4534S111,
DUS4535S02W,
FMJ4534S0LT,
FMJ4534S1V1,
DUS4534S0FD,
FMJ4534S16J,
FMJ4534S191,
DUS4534S0JP,
FMJ4534S0TP,
FMJ4534S089,
DUS4535S04G,
FMJ4534S0PT,
FMJ4534S1H8,
DUS4534S0EF,
FMJ4534S0CF,
FMJ4534S06P,
FMJ4534S19X,
FMJ4534S0BS,
DUS4534S0HW,
FMJ4534S07H,
DUS4534S0CM,
FMJ4534S0K1,
FMJ4534S0SZ,
FMJ4534S0H0,
DUS4520S064,
DUS4535S02D,
FMJ4534S1EG,
FMJ4534S1EW,
FMJ4534S1MT,
FMJ4534S0SG,
DUS4534S0AV,
DUS4535S09A,
FMJ4534S18K,
FMJ4534S0KE,
DUS4534S0HG,
FMJ4534S08N,
FMJ4534S0GB,
FMJ4534S0NN,
FMJ4534S0GV,
DUS4534S08W,
DUS4535S06N,
FMJ4534S0TB,
FMJ4534S0X8,
FMJ4534S00Y,
DUS4534S02E,
FMJ4534S0PN,
DUS4535S00N,
FMJ4534S11K,
FMJ4534S15H,
DUS4535S043,
FMJ4534S04J,
FMJ4534S0NS,
DUS4534S0CA,
FMJ4534S1HZ,
FMJ4534S1B0,
DUS4535S06F,
FMJ4534S1TF,
DUS4534S0DX,
FMJ4534S1DK,
FMJ4534S1VM,
FMJ4534S0ZA,
DUS4535S08A,
FMJ4534S1SN,
FMJ4534S0C7,
DUS4534S07K,
FMJ4534S010,
FMJ4534S0DG,
DUS4534S061,
FMJ4534S18R,
FMJ4534S1CA,
DUS4535S08H,
FMJ4534S1FM,
FMJ4534S1P2,
DUS4535S00S,
FMJ4534S0WZ,
FMJ4534S0VX,
FMJ4534S1HG,
FMJ4534S0MC,
DUS4534S0G0,
DUS4535S03P,
FMJ4534S1PL,
FMJ4534S1PA,
FMJ4534S1C0,
FMJ4534S11Y,
DUS4534S0GR,
FMJ4534S0N0,
FMJ4534S0H8,
DUS4534S011,
DUS4535S0AZ,
FMJ4534S13D,
FMJ4534S1JE,
DUS4535S030,
DUS4534S06D,
FMJ4534S057,
FMJ4534S0PY,
FMJ4534S0JG,
FMJ4534S0T1,
DUS4535S07D,
FMJ4534S1N6,
FMJ4534S18P,
FMJ4534S1CC,
DUS4534S00E,
FMJ4534S14Z,
DUS4535S086,
FMJ4534S034,
FMJ4534S0X0,
FMJ4534S0CW,
FMJ4534S16F,
DUS4534S04Y,
DUS4535S003,
FMJ4534S0AK,
FMJ4534S02P,
FMJ4534S0SM,
DUS4534S067,
FMJ4534S1MK,
DUS4535S063,
FMJ4534S1VE,
FMJ4534S0NM,
FMJ4534S17M,
FMJ4534S0GH,
DUS4534S03W,
DUS4535S05K,
FMJ4534S1PP,
FMJ4534S1J5,
FMJ4534S03C,
FMJ4534S0FR,
DUS4534S04S,
DUS4534S0CP,
FMJ4534S13E,
FMJ4534S0HS,
DUS4535S02M,
FMJ4534S18S,
FMJ4534S1K0,
DUS4535S07K,
FMJ4534S0NJ,
FMJ4534S0BY,
DUS4534S0HE,
FMJ4534S1F1,
FMJ4534S1VP,
DUS4535S008,
FMJ4534S0S5,
FMJ4534S1VV,
FMJ4534S1BN,
FMJ4534S1DH,
DUS4534S052,
DUS4535S07R,
FMJ4534S1PY,
FMJ4534S014,
FMJ4534S1BM,
DUS4534S0C1,
FMJ4534S1MM,
DUS4535S015,
FMJ4534S03R,
FMJ4534S0KF,
FMJ4534S179,
DUS4534S087,
FMJ4534S104,
DUS4535S01X,
FMJ4534S0WG,
FMJ4534S136,
FMJ4534S00W,
DUS4534S007,
FMJ4534S1TS,
DUS4535S0A8,
FMJ4534S1EF,
DUS4534S0DJ,
FMJ4534S06N,
FMJ4534S16P,
FMJ4534S0HG,
FMJ4534S092,
DUS4534S09A,
FMJ4534S0F1,
DUS4535S03A,
FMJ4534S1S3,
FMJ4534S1PB,
DUS4535S03K,
FMJ4534S0ME,
FMJ4534S115,
FMJ4534S0SY,
DUS4534S014,
FMJ4534S09R,
DUS4535S01V,
FMJ4534S1RF,
FMJ4534S0NK,
FMJ4534S1V5,
DUS4534S01N,
FMJ4534S1LE,
DUS4534S0E8,
FMJ4534S0JH,
FMJ4534S0HZ,
FMJ4534S0ZB,
DUS4534S00B,
FMJ4534S1MW,
FMJ4534S1NH,
FMJ4534S1C8,
DUS4534S0F2,
DUS4535S05M,
FMJ4534S1G2,
FMJ4534S1GD,
DUS4534S0F3,
FMJ4534S18T,
FMJ4534S14N,
DUS4535S05S,
FMJ4534S0YL,
FMJ4534S0YZ,
DUS4534S028,
FMJ4534S13T,
FMJ4534S0NH,
DUS4535S0A3,
FMJ4534S19B,
FMJ4534S1H4,
DUS4535S026,
FMJ4534S0EM,
FMJ4534S122,
DUS4534S09W,
FMJ4534S15R,
FMJ4534S03G,
FMJ4534S0ZW,
DUS4534S08H,
FMJ4534S0L1,
DUS4535S047,
FMJ4534S0Z7,
FMJ4534S164,
DUS4535S0A0,
FMJ4534S0RW,
FMJ4534S0JK,
DUS4534S0BJ,
FMJ4534S0X3,
FMJ4534S1PC,
FMJ4534S12P,
FMJ4534S0XB,
FMJ4534S01F,
FMJ4534S17P,
DUS4534S03B,
DUS4534S02H,
FMJ4534S1SD,
FMJ4534S0N3,
DUS4534S0CL,
DUS4535S052,
FMJ4534S1TD,
FMJ4534S1TX,
DUS4535S00R,
FMJ4534S0RV,
FMJ4534S1DD,
FMJ4534S065,
DUS4534S04A,
FMJ4534S0JW,
DUS4535S00A,
FMJ4534S1CE,
FMJ4534S1NA,
FMJ4534S06A,
FMJ4534S0B8,
DUS4534S00C,
DUS4534S0J4,
FMJ4534S10M,
FMJ4534S051,
DUS4535S00J,
FMJ4534S1DA,
FMJ4534S1ST,
DUS4535S096,
DUS4534S0HT,
FMJ4534S1CF,
FMJ4534S14M,
FMJ4534S0TG,
FMJ4534S12X,
DUS4535S09N,
FMJ4534S1RH,
FMJ4534S186,
FMJ4534S0WK,
DUS4534S0A0,
FMJ4534S1AM,
DUS4535S03V,
FMJ4534S1VG,
FMJ4534S0VJ,
FMJ4534S0KZ,
FMJ4534S1LC,
DUS4534S0ED,
DUS4535S06Y,
FMJ4534S01T,
FMJ4534S1E6,
FMJ4534S1LL,
FMJ4534S00N,
DUS4534S064,
DUS4535S09T,
FMJ4534S1G3,
FMJ4534S1G4,
DUS4534S07J,
FMJ4534S03B,
FMJ4534S1RP,
DUS4535S00H,
FMJ4534S1G1,
FMJ4534S1E5,
DUS4534S0D7,
FMJ4534S07S,
FMJ4534S11G,
FMJ4534S0WW,
FMJ4534S0HV,
DUS4534S0FK,
DUS4535S0AN,
FMJ4534S0LN,
FMJ4534S10P,
DUS4535S01C,
FMJ4534S1LN,
FMJ4534S19F,
FMJ4534S0AC,
DUS4534S05Y,
FMJ4534S0ZR,
DUS4535S02N,
FMJ4534S0VH,
FMJ4534S0VW,
FMJ4534S15N,
FMJ4534S00X,
DUS4534S0EC,
DUS4534S05P,
FMJ4534S1BX,
FMJ4534S07C,
DUS4535S044,
FMJ4534S1MV,
FMJ4534S1JG,
DUS4535S0B1,
FMJ4534S13K,
FMJ4534S0R3,
FMJ4534S0ES,
FMJ4534S1N1,
DUS4534S09X,
DUS4535S02V,
FMJ4534S0XA,
FMJ4534S1AW,
FMJ4534S0AR,
FMJ4534S0GN,
DUS4534S03V,
DUS4535S01J,
FMJ4534S0BM,
FMJ4534S0VN,
FMJ4534S1MS,
DUS4534S05D,
FMJ4534S0Y9,
DUS4534S0HL,
FMJ4534S0X7,
FMJ4534S1JV,
DUS4535S033,
FMJ4534S1K2,
FMJ4534S0M6,
DUS4535S03Y,
FMJ4534S0LH,
FMJ4534S1L4,
FMJ4534S0M2,
FMJ4534S00S,
DUS4534S0DS,
DUS4535S039,
FMJ4534S0N6,
FMJ4534S0FG,
DUS4534S0AK,
FMJ4534S1AT,
FMJ4534S1RT,
DUS4535S02H,
FMJ4534S0ZY,
FMJ4534S1N4,
DUS4534S0DH,
FMJ4534S0LJ,
FMJ4534S0T8,
DUS4535S02A,
FMJ4534S1V2,
FMJ4534S163,
FMJ4534S03J,
FMJ4534S00V,
DUS4534S0DL,
DUS4535S01T,
FMJ4534S0HL,
FMJ4534S1AJ,
FMJ4534S1MH,
FMJ4534S1ME,
DUS4534S08M,
FMJ4534S1TN,
DUS4534S0F1,
FMJ4534S09E,
DUS4535S00P,
FMJ4534S0L5,
FMJ4534S1CD,
DUS4535S05Z,
FMJ4534S04V,
FMJ4534S0N8,
DUS4534S07H,
FMJ4534S1MX,
FMJ4534S0V0,
DUS4534S0GF,
FMJ4534S144,
FMJ4534S1JJ,
DUS4535S029,
FMJ4534S1S9,
FMJ4534S1RW,
FMJ4534S0NC,
FMJ4534S1DF,
DUS4534S0FB,
DUS4535S0A1,
FMJ4534S0YP,
FMJ4534S0ZH,
DUS4535S06B,
FMJ4534S1VC,
FMJ4534S10D,
FMJ4534S0JA,
DUS4534S0DR,
FMJ4534S0VL,
DUS4534S0J5,
FMJ4534S0SH,
FMJ4534S09N,
FMJ4534S1L3,
DUS4534S01V,
FMJ4534S1L6,
DUS4534S05G,
FMJ4534S0CP,
FMJ4534S0DB,
DUS4535S09B,
FMJ4534S11T,
FMJ4534S1MA,
FMJ4534S14B,
DUS4534S0E7,
FMJ4534S0BK,
DUS4535S09K,
FMJ4534S119,
FMJ4534S1P7,
DUS4535S01F,
FMJ4534S16D,
FMJ4534S0X1,
FMJ4534S1GV,
DUS4534S04T,
FMJ4534S1GX,
DUS4535S00V,
FMJ4534S1V6,
FMJ4534S0MS,
FMJ4534S05A,
DUS4534S04M,
FMJ4534S1C5,
DUS4535S01G,
FMJ4534S0EC,
FMJ4534S0XW,
DUS4534S08Z,
FMJ4534S1G8,
FMJ4534S1G9,
DUS4535S07P,
FMJ4534S1J8,
FMJ4534S1NC,
DUS4534S08K,
FMJ4534S00F,
FMJ4534S0FN,
FMJ4534S0VE,
DUS4534S0A5,
FMJ4534S0YN,
DUS4535S065,
FMJ4534S120,
FMJ4534S1KG,
FMJ4534S0X2,
DUS4534S0EN,
FMJ4534S0WH,
DUS4535S05J,
FMJ4534S1RV,
FMJ4534S1T9,
DUS4535S04W,
FMJ4534S10F,
FMJ4534S0JM,
FMJ4534S1B3,
FMJ4534S1F2,
DUS4534S030,
DUS4515S00T,
DUS4534S06S,
FMJ4534S09G,
FMJ4534S0P1,
FMJ4534S102,
FMJ4534S1AV,
DUS4534S06X,
FMJ4534S0B1,
FMJ4534S0GC,
FMJ4534S0SX,
FMJ4534S0HR,
DUS4520S00E,
DUS4535S06Z,
FMJ4534S1F7,
FMJ4534S188,
FMJ4534S05Y,
FMJ4534S1TV,
DUS4534S017,
DUS4534S0GL,
FMJ4534S0KR,
FMJ4534S17J,
DUS4534S0A6,
FMJ4534S0FK,
FMJ4534S0KM,
FMJ4534S088,
FMJ4534S0YH,
DUS4534S012,
DUS4535S0AD,
FMJ4534S1W0,
FMJ4534S1EN,
DUS4535S036,
FMJ4534S129,
FMJ4534S149,
FMJ4534S05L,
FMJ4534S0V2,
DUS4534S077,
DUS4535S04S,
FMJ4534S09P,
DUS4534S0HX,
FMJ4534S04B,
FMJ4534S04R,
FMJ4534S0L4,
DUS4535S04Y,
DUS4534S0E0,
FMJ4534S0AY,
FMJ4534S04K,
FMJ4534S1K3,
FMJ4534S1R4,
DUS4534S062,
FMJ4534S04D,
FMJ4534S0JV,
FMJ4534S0KJ,
FMJ4534S0NL,
DUS4534S032,
DUS4535S02K,
FMJ4534S07A,
FMJ4534S0ZN,
FMJ4534S10V,
FMJ4534S0MA,
DUS4534S098,
DUS4535S083,
FMJ4534S0ND,
FMJ4534S1JB,
DUS4534S01J,
FMJ4534S08S,
FMJ4534S1B8,
FMJ4534S153,
FMJ4534S0NV,
DUS4534S0EV,
DUS4535S05N,
FMJ4534S1T6,
FMJ4534S1ED,
DUS4535S04Z,
FMJ4534S0TW,
FMJ4534S0P8,
DUS4534S08P,
FMJ4534S0YR,
FMJ4534S1H0,
FMJ4534S1G0,
DUS4534S0AL,
FMJ4534S0TA,
DUS4534S08L,
FMJ4534S1HB,
FMJ4534S1LW,
DUS4535S019,
FMJ4534S0HK,
FMJ4534S0HM,
DUS4534S07L,
FMJ4534S1GA,
FMJ4534S0D6,
FMJ4534S1BY,
DUS4534S0FJ,
FMJ4534S0DK,
DUS4535S005,
FMJ4534S130,
FMJ4534S1C1,
DUS4535S098,
FMJ4534S166,
FMJ4534S1MF,
FMJ4534S123,
FMJ4534S1TT,
DUS4534S0BT,
FMJ4534S0S0,
DUS4534S0CR,
FMJ4534S0AX,
DUS4535S068,
FMJ4534S13V,
FMJ4534S1S4,
DUS4535S03B,
FMJ4534S1T1,
FMJ4534S1M1,
FMJ4534S147,
DUS4534S0D6,
FMJ4534S01R,
FMJ4534S05W,
FMJ4534S1JT,
DUS4534S0HH,
DUS4535S06C,
FMJ4534S1TR,
FMJ4534S1V9,
DUS4535S00M,
FMJ4534S18Z,
FMJ4534S0WP,
DUS4534S0C6,
FMJ4534S0X5,
FMJ4534S004,
DUS4535S057,
FMJ4534S0YY,
FMJ4534S0SS,
FMJ4534S19S,
DUS4534S021,
FMJ4534S0RK,
DUS4535S01S,
FMJ4534S0J8,
FMJ4534S0TK,
FMJ4534S1LT,
DUS4534S0AY,
FMJ4534S1PX,
DUS4535S0A7,
FMJ4534S0NE,
FMJ4534S0TF,
FMJ4534S1PG,
FMJ4534S1M8,
DUS4534S09V,
FMJ4534S1TA,
FMJ4534S18M,
DUS4534S0A7,
DUS4535S04T,
FMJ4534S0LM,
FMJ4534S1RZ,
FMJ4533R009,
DUS4532R00S,
FMJ4534S08J,
DUS4534S01X,
FMJ4534S05S,
FMJ4534S04Y,
FMJ4534S0CN,
DUS4534S0BC,
FMJ4534S14L,
FMJ4534S1DT,
DUS4534S0FF,
FMJ4534S0VF,
FMJ4534S1AZ,
DUS4534S09L,
FMJ4534S19T,
DUS4534S0FP,
FMJ4534S09H,
FMJ4534S078,
FMJ4534S0NR,
DUS4534S0GT,
FMJ4534S017,
FMJ4534S07F,
DUS4534S0JJ,
FMJ4534S02E,
FMJ4534S02F,
DUS4534S038,
FMJ4534S02G,
FMJ4534S0C2,
DUS4534S0G8,
DUS4534S0G9,
FMJ4534S0AH,
FMJ4534S079,
DUS4534S0GV,
FMJ4534S0DL,
FMJ4534S09M,
FMJ4534S1FC,
FMJ4534S1GT,
DUS4534S0FX,
DUS4534S0H6,
FMJ4534S0N5,
FMJ4534S0YF,
FMJ4534S0DJ,
FMJ4534S0CS,
DUS4534S0JB,
FMJ4534S1RX,
DUS4534S0DN,
FMJ4534S1CP,
FMJ4534S17H,
FMJ4534S0HD,
DUS4534S0H3,
DUS4535S0B9,
FMJ4534S1JA,
FMJ4534S0P4,
FMJ4534S1KL,
DUS4534S0HC,
FMJ4534S050,
FMJ4534S128,
DUS4534S0A3,
FMJ4534S0YX,
DUS4534S031,
FMJ4534S10S,
FMJ4534S0VG,
FMJ4534S12V,
FMJ4534S13F,
DUS4534S02Z,
DUS4534S0BG,
FMJ4534S0L0,
FMJ4534S0K2,
FMJ4534S0MP,
DUS4534S02N,
FMJ4534S0KG,
DUS4534S0CH,
FMJ4534S0VB,
FMJ4534S1B5,
FMJ4534S0A0,
DUS4534S03S,
FMJ4534S0F2,
DUS4534S09K,
FMJ4534S01J,
FMJ4534S060,
FMJ4534S002,
DUS4534S0C7,
FMJ4534S0WD,
FMJ4534S1AR,
DUS4534S0JD,
FMJ4534S1BL,
FMJ4534S1FN,
DUS4534S07M,
FMJ4534S056,
FMJ4534S0T4,
DUS4534S08S,
FMJ4534S05P,
FMJ4534S1HF,
FMJ4534S1R7,
DUS4534S0B7,
FMJ4534S1PH,
FMJ4534S0L8,
DUS4534S0GH,
DUS4535S0B3,
FMJ4534S11Z,
FMJ4534S1TZ,
FMJ4534S1H5,
FMJ4534S0M4,
DUS4534S0JH,
DUS4535S07N,
FMJ4534S1JX,
FMJ4534S0MM,
FMJ4534S10E,
FMJ4534S0L7,
DUS4534S0G7,
DUS4534S07P,
FMJ4534S0DA,
FMJ4534S14D,
DUS4534S0BP,
FMJ4534S0BT,
FMJ4534S0Y4,
FMJ4534S0Y8,
FMJ4534S0NP,
DUS4534S0BN,
DUS4534S09J,
FMJ4534S0AS,
FMJ4534S0D2,
FMJ4534S08D,
FMJ4534S03Y,
DUS4534S0CY,
FMJ4534S0LL,
FMJ4534S16G,
DUS4534S0E1,
FMJ4534S171,
DUS4534S026,
FMJ4534S02X,
DUS4534S0C8,
FMJ4534S13Z,
FMJ4534S155,
FMJ4534S0G2,
FMJ4534S0RG,
DUS4534S0HJ,
DUS4534S0GW,
FMJ4534S182,
FMJ4534S0A8,
FMJ4534S0AW,
DUS4534S0F7,
FMJ4534S0BZ,
FMJ4534S03Z,
FMJ4534S03H,
DUS4534S0HN,
FMJ4534S17G,
DUS4534S03N,
FMJ4534S0AN,
FMJ4534S0F8,
FMJ4534S054,
DUS4534S00S,
FMJ4534S08M,
DUS4534S07Z,
FMJ4534S043,
DUS4534S07S,
FMJ4534S0EG,
FMJ4534S06K,
FMJ4534S0YA,
DUS4534S07X,
FMJ4534S06T,
DUS4535S043,
FMJ4534S04J,
FMJ4534S0NS,
DUS4534S0CA,
FMJ4534S1HZ,
FMJ4534S1B0,
FMJ4534S14B,
DUS4534S0E7,
FMJ4534S0BK,
DUS4535S09K,
FMJ4534S119,
FMJ4534S1P7";
    }


}