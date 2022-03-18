using System;
using MESDataObject.Module;
using MESDBHelper;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace MESReport.BaseReport
{
    public class Jnp750and711Report : ReportBase
    {
        public Jnp750and711Report()
        {
        }



        string partnos = $@"750-062572|711-062571||||/
750-136059|711-098840|711-054743|||/
750-054758|711-054756|711-055267|||/
750-070866|711-070865||||/
750-063181|711-063179|711-045719|||/
750-017782-01|||||/
750-072925|711-072924|711-055267|||/
750-045715|711-045716|711-059960|711-045719||/
750-059919|711-058968|711-047068|||/
750-063180|711-063179|711-045719|||/
750-070395|711-070394|711-070910|||/
750-049846|711-046904|711-046907|||/
750-028387|711-028402||||/
750-136058|711-098703|711-054743|711-055619||/
750-077330|711-068803|711-077329|||/
750-060845|711-059218||||/
750-028380|711-028399||||/
750-021093-01|||||/
750-069467|711-069466||||/
750-077332|711-077331|711-078421|||/
750-038768|711-051592||||/
750-055992|711-057161|711-057163|||/
750-043596|711-032384||||/
750-061489|711-061488||||/
750-063183|711-063179|711-045719|||/
750-054563|711-045716|711-059960|711-045719||/
750-033307|711-033306||||/
750-063184|711-063179|711-045719|||/
750-054564|711-046013|711-045719|||/
750-065925|711-053322|711-055619|711-054743||/
750-063718|711-059218||||/
750-047849-01|||||/
750-078633|711-078632|711-070910|||/
750-063414|711-063413|711-054743|||/
750-086583|711-102627|711-086431|711-087866|711-070910|/
750-061622|711-061621||||/
750-073435|711-073308|711-077143|||/
750-033199|711-033198||||/
750-046905|711-046904|711-046907|||/
750-064422|711-087558|711-059363|711-059218||/
750-061262|711-050134|711-061263|||/
750-022854-01|||||/
750-060847|711-059218||||/
750-065634|||||/
750-028392|711-028407|711-028406|||/
750-060844|711-058968||||/
750-049488|711-033306||||/
750-064569|711-062864|711-062860|711-062866||/
750-065926|711-056518|711-054743|||/
750-061286|711-062576||||/
750-067371|711-067370||||/
750-062787|711-062576|711-058968|||/
750-073160|711-055085|711-041855|711-072924|711-055267|/
750-054678|711-045716|711-059960|711-045719||/
750-046005|711-046013|711-045719|||/
750-044130|711-044129|711-046638|711-045719||/
750-054576|711-058851|711-054743|||/
750-064572|711-063748|711-063749|711-046904|711-046907|/
750-060846|711-059218||||/
750-095572|711-070865||||/
750-066337|711-062571||||/
750-055469|||||/
750-036996|711-037114|711-036995|||/
750-095568|711-072924|711-055267|||/
750-060718|711-060514||||/
750-049607|711-032432||||/
750-055087|711-054756|711-055085|711-041855|711-055267|/
750-099871|711-098840|711-100225|||/
750-055732|711-028407|711-028406|||/
750-062243|711-045716|711-062244|711-059960||/
750-032479|711-033364|711-032478|||/
750-062242|711-046013|711-062244|||/
750-062852|711-062571||||/
750-067373|711-067372|711-054388|||/
750-067664|711-063179|711-045719|||/
750-075212|711-054756|711-087558|711-058968|711-055267|/
750-075203|711-087558|711-054756|711-059363|711-059218|711-055267/
750-023491-01|||||/
750-063459|711-033306||||/
750-064570|711-063748|711-063749|711-028407|711-028406|/
750-099870|711-098703|711-055619|711-100225||/
750-067705|711-041371|711-049310|||/
750-028390|711-032432||||/
750-023519-01|||||/
750-060558|711-060512||||/
750-059751|711-059750||||/
750-029016|||||/
750-036233|711-036232||||/
750-063461|711-046904|711-046907|||/
750-052893|711-052892||||/
750-113847|711-070865|711-054756|711-059363|711-059218|711-055267/
750-049487|711-036232||||/
750-106329|711-070394|711-070910|||/
750-064568|711-062862|711-062860|||/
";

        string partnos_new = $@"750-062572,
750-136059,
750-054758,
750-070866,
750-063181,
750-017782-01,
750-072925,
750-045715,
750-059919,
750-063180,
750-070395,
750-049846,
750-028387,
750-136058,
750-077330,
750-060845,
750-028380,
750-021093-01,
750-069467,
750-077332,
750-038768,
750-055992,
750-043596,
750-061489,
750-063183,
750-054563,
750-033307,
750-063184,
750-054564,
750-065925,
750-063718,
750-047849-01,
750-078633,
750-063414,
750-086583,
750-061622,
750-073435,
750-033199,
750-046905,
750-064422,
750-061262,
750-022854-01,
750-060847,
750-065634,
750-028392,
750-060844,
750-049488,
750-065926,
750-064569,
750-061286,
750-067371,
750-062787,
750-073160,
750-054678,
750-046005,
750-044130,
750-054576,
750-064572,
750-060846,
750-095572,
750-066337,
750-055469,
750-036996,
750-029016,
750-095568,
750-060718,
750-055087,
750-049607,
750-099871,
750-055732,
750-062243,
750-032479,
750-062242,
750-062852,
750-067373,
750-067664,
750-075212,
750-075203,
750-023491-01,
750-064570,
750-063459,
750-028390,
750-099870,
750-067705,
750-023519-01,
750-059751,
750-060558,
750-063461,
750-036233,
750-052893,
750-113847,
750-106329,
750-049487,
750-064568";


        [Serializable]
        class c20220115
        {
            public string PN { get; set; }
            public string SM750 { get; set; }
            public string W750 { get; set; }
            public string SW750 { get; set; }
            public string W711_1PN { get; set; }
            public string W711_1sm { get; set; }
            public string W711_1 { get; set; }
            public string W711_2PN { get; set; }
            public string W711_2sm { get; set; }
            public string W711_2 { get; set; }

            public string W711_3PN { get; set; }
            public string W711_3sm { get; set; }
            public string W711_3 { get; set; }

            public string W711_4PN { get; set; }
            public string W711_4sm { get; set; }
            public string W711_4 { get; set; }

            public string W711_5PN { get; set; }
            public string W711_5sm { get; set; }
            public string W711_5 { get; set; }
            public string tol { get; set; }

        }

        class s750711
        {
            public string P750 { get; set; }
            public List<string> P711s { get; set; }
        }

        public override void Run()
        {
            DataRow linkDataRow = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            var db = SFCDB.ORM;
            try
            {
                var pns = partnos.Replace("\r\n", "").Split('/');
                var pnslist = new List<s750711>();
                foreach (var item in pns)
                {
                    var pn750 = item.Split('|').FirstOrDefault();
                    var pn711s = item.Split('|').ToList().FindAll(t => t.StartsWith("711"));
                    var pnsobj = new s750711();
                    pnsobj.P750 = pn750;
                    var p711s = new List<string>();
                    foreach (var s711 in pn711s)
                        p711s.Add(s711);
                    pnsobj.P711s = p711s;
                    pnslist.Add(pnsobj);
                }

                var pnnew = partnos_new.Replace("\r\n", "").Split(',');
                //var sws = db.Queryable<R_JUNIPER_SILVER_WIP>().ToList();
                //var sws_wip = sws.FindAll(t => t.STATE_FLAG == "1").Select(t => t.SN).Distinct().ToList();
                var res = new List<c20220115>();
                foreach (var pnn in pnnew)
                {
                    var item = pnslist.FindAll(t => t.P750 == pnn).ToList().FirstOrDefault();
                    if (item == null)
                        throw new Exception("Err!miss 750-711 mapping!");
                    if (item.P750.Trim() == "")
                        continue;
                    //item = item.Trim();
                    var curenttres = new c20220115();
                    //var pn750 = item.Split('|').FirstOrDefault();
                    //var pn711s = item.Split('|').ToList().FindAll(t => t.StartsWith("711"));
                    var pn750 = item.P750.Trim();
                    var pn711s = item.P711s;
                    curenttres.PN = pn750;
                    var tol = 0;
                    //if (pn750 == "750-075203")
                    //    pn750 = "750-075203";
                    //750 (Supermaket)
                    curenttres.SM750 = new Func<string>(() =>
                    {
                        return db.Ado.GetDataTable($@" 
                                            SELECT distinct sn
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                and skuno='{pn750}')
                                           AND S.COMPLETED_FLAG = 1
                                            and SUBSTR(s.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') 
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG=1
                                           AND S.NEXT_STATION <> 'REWORK'
                                            and s.sn not in (select sn from r_juniper_silver_wip where state_flag='1')
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno ) ").Rows.Count.ToString();
                        //return db.Ado.GetDataTable($@"select*from r_sn a where skuno='{pn750}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                        //                        and a.sn not like '*%'
                        //                            and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                        //and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')   ").Rows.Count.ToString();
                        //return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                        //        && !MESDBHelper.IMesDbEx.OracleContain(t.SN, sws_wip) && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.COMPLETED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                    })();
                    //750 (WIP)
                    var wopre =new string[]{ "006A", "0091", "0092", "0093", "0094" };
                    curenttres.W750 = new Func<string>(() =>
                    {
                        var wos = db.Queryable<R_WO_BASE>().Where(t => t.SKUNO == pn750 && t.CLOSED_FLAG == "0" && wopre.Contains(SqlSugar.SqlFunc.Substring(t.WORKORDERNO,0,4))).Select(t => t.WORKORDERNO).ToList();
                        var noloading = 0;
                        foreach (var wo in wos)
                        {
                            var wonoloading = db.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where   SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND A.workorderno='{wo}' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' ) 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}' ");
                            if (wonoloading.Rows.Count > 0)
                                noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                        }
                        return (db.Ado.GetDataTable($@"select*from r_sn a where SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND skuno='{pn750}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                                    FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )  ").Rows.Count + noloading).ToString();
                        //return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                        //        && t.COMPLETED_FLAG == MesBool.No.ExtValue()).ToList().Count().ToString();
                    })();
                    //750 (Silver WIP)
                    curenttres.SW750 = new Func<string>(() =>
                    {
                        return db.Ado.GetDataTable($@"SELECT distinct SN
                                          FROM R_SN S
                                         WHERE EXISTS
                                         (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO
                                                    and ww.skuno='{pn750}'
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO))
                                         AND EXISTS (select * from r_juniper_silver_wip sw where sw.sn=s.sn and sw.skuno=s.skuno and state_flag=1)
                                         AND S.VALID_FLAG<>0
                                        AND SUBSTR(s.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') 
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG=1
                                           AND S.NEXT_STATION <> 'REWORK'
                                            and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                         ").Rows.Count.ToString();
                        //return db.Ado.GetDataTable($@"select* from r_sn a,r_juniper_silver_wip b where a.sn=b.sn and a.valid_flag='1' and a.skuno='{pn750}' and a.completed_flag='1' and b.STATE_FLAG='1' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                        //                        and a.sn not like '*%'").Rows.Count.ToString();
                    })();
                    //var mainpn = db.Queryable<R_LINK_CONTROL>().Where(t => t.MAIN_ITEM == pn750 && t.SUB_ITEM.StartsWith("711") && pn711s.Contains(t.SUB_ITEM)).ToList().FirstOrDefault();

                    var res711s = new List<c20220115>();
                    for (int i = 0; i < pn711s.Count(); i++)
                    {
                        //var p711qty = db.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.SKUNO == pn711s[i]
                        //&& t.STARTED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                        var p711qty = new Func<string>(() =>
                        {
                            var wos = db.Queryable<R_WO_BASE>().Where(t => t.SKUNO == pn711s[i] && t.CLOSED_FLAG == "0" && wopre.Contains(SqlSugar.SqlFunc.Substring(t.WORKORDERNO, 0, 4))).Select(t => t.WORKORDERNO).ToList();
                            var noloading = 0;
                            foreach (var wo in wos)
                            {
                                var wonoloading = db.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where   SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND A.workorderno='{wo}' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' ) 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}' ");
                                if (wonoloading.Rows.Count > 0)
                                    noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                            }
                            return (db.Ado.GetDataTable($@"select*from r_sn a where SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND skuno='{pn711s[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                                    FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  ) ").Rows.Count + noloading).ToString();
                            //return (db.Ado.GetDataTable($@"select*from r_sn a where  SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') and skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                            //                    and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                            //FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )  ").Rows.Count + noloading).ToString();
                        })();
                        //db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' ").Rows.Count.ToString();
                        var p711smqty = db.Ado.GetDataTable($@"SELECT distinct sn
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                and skuno='{pn711s[i]}')
                                           AND S.COMPLETED_FLAG = 1
                                           AND SUBSTR(s.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') 
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG=1
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno ) ").Rows.Count.ToString();
                        //var p711smqty = db.Ado.GetDataTable($@"select*from r_sn a where  SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') and skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                        //                        and a.sn not like '*%'
                        //                            and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                        //                        and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')    ").Rows.Count.ToString(); 
                        res711s.Add(new c20220115() { W711_1 = p711qty, W711_1sm = p711smqty, W711_1PN = pn711s[i] });
                    }
                    for (int i = 0; i < res711s.Count(); i++)
                    {
                        switch (i)
                        {
                            case 0: curenttres.W711_1 = res711s[i].W711_1; curenttres.W711_1sm = res711s[i].W711_1sm; curenttres.W711_1PN = res711s[i].W711_1PN; break;
                            case 1: curenttres.W711_2 = res711s[i].W711_1; curenttres.W711_2sm = res711s[i].W711_1sm; curenttres.W711_2PN = res711s[i].W711_1PN; break;
                            case 2: curenttres.W711_3 = res711s[i].W711_1; curenttres.W711_3sm = res711s[i].W711_1sm; curenttres.W711_3PN = res711s[i].W711_1PN; break;
                            case 3: curenttres.W711_4 = res711s[i].W711_1; curenttres.W711_4sm = res711s[i].W711_1sm; curenttres.W711_4PN = res711s[i].W711_1PN; break;
                            case 4: curenttres.W711_5 = res711s[i].W711_1; curenttres.W711_5sm = res711s[i].W711_1sm; curenttres.W711_5PN = res711s[i].W711_1PN; break;
                            default:
                                break;
                        }
                    }
                    if (res711s.Count > 0)
                    {
                        res711s = res711s.OrderBy(t => (int.Parse(t.W711_1sm) + int.Parse(t.W711_1))).ToList();
                        tol = int.Parse(curenttres.SM750) + int.Parse(curenttres.W750) + int.Parse(curenttres.SW750) + int.Parse(res711s.FirstOrDefault().W711_1) + int.Parse(res711s.FirstOrDefault().W711_1sm);
                    }
                    else
                        tol = int.Parse(curenttres.SM750) + int.Parse(curenttres.W750) + int.Parse(curenttres.SW750);
                    curenttres.tol = tol.ToString();
                    //if (pn750 == "750-059919")
                    //    pn750 = "750-059919";

                    res.Add(curenttres);
                }
                var key = DateTime.Now.ToString("yyyyMMddhhmmss");
                //string filename = $@"C:\\Users\\G6001953.NN\\Desktop\\FJZ\\report\\{key}.csv";
                var path = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Jnp\\report\\";
                string filename = $@"{path}{key}.csv";
                //scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
                //ExcelHelp.ExportCsv(res, filename);


                DataTable dt = new DataTable();
                dt.Columns.Add("PN");
                dt.Columns.Add("SM750");
                dt.Columns.Add("W750");
                dt.Columns.Add("SW750");
                dt.Columns.Add("W711_1PN");
                dt.Columns.Add("W711_1sm");
                dt.Columns.Add("W711_1");
                dt.Columns.Add("W711_2PN");
                dt.Columns.Add("W711_2sm");
                dt.Columns.Add("W711_2");
                dt.Columns.Add("W711_3PN");
                dt.Columns.Add("W711_3sm");
                dt.Columns.Add("W711_3");
                dt.Columns.Add("W711_4PN");
                dt.Columns.Add("W711_4sm");
                dt.Columns.Add("W711_4");
                dt.Columns.Add("W711_5PN");
                dt.Columns.Add("W711_5sm");
                dt.Columns.Add("W711_5");
                dt.Columns.Add("tol");

                foreach (var item in res)
                {
                    var dr = dt.NewRow();
                    dr["PN"] = item.PN;
                    dr["SM750"] = item.SM750;
                    dr["W750"] = item.W750;
                    dr["SW750"] = item.SW750;
                    dr["W711_1PN"] = item.W711_1PN;
                    dr["W711_1sm"] = item.W711_1sm;
                    dr["W711_1"] = item.W711_1;
                    dr["W711_2PN"] = item.W711_2PN;
                    dr["W711_2sm"] = item.W711_2sm;
                    dr["W711_2"] = item.W711_2;
                    dr["W711_3PN"] = item.W711_3PN;
                    dr["W711_3sm"] = item.W711_3sm;
                    dr["W711_3"] = item.W711_3;
                    dr["W711_4PN"] = item.W711_4PN;
                    dr["W711_4sm"] = item.W711_4sm;
                    dr["W711_4"] = item.W711_4;
                    dr["W711_5PN"] = item.W711_5PN;
                    dr["W711_5sm"] = item.W711_5sm;
                    dr["W711_5"] = item.W711_5;
                    dr["tol"] = item.tol;
                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt);
                retTab.Tittle = "StationScanLog Report";
                Outputs.Add(retTab);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void Run_bak()
        {
            DataRow linkDataRow = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            var db = SFCDB.ORM;
            try
            {
                    var pns = partnos.Replace("\r\n", "").Split('/');
                    //var sws = db.Queryable<R_JUNIPER_SILVER_WIP>().ToList();
                    //var sws_wip = sws.FindAll(t => t.STATE_FLAG == "1").Select(t => t.SN).Distinct().ToList();
                    var res = new List<c20220115>();
                    foreach (var item in pns)
                    {
                        if (item.Trim() == "")
                            continue;
                        //item = item.Trim();
                        var curenttres = new c20220115();
                        var pn750 = item.Split('|').FirstOrDefault();
                        var pn711s = item.Split('|').ToList().FindAll(t => t.StartsWith("711"));
                        curenttres.PN = pn750;
                        var tol = 0;
                        //if (pn750 == "750-075203")
                        //    pn750 = "750-075203";
                        //750 (Supermaket)
                        curenttres.SM750 = new Func<string>(() => {
                            return db.Ado.GetDataTable($@"select*from r_sn a where skuno='{pn750}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%'
                                                    and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                        and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')   ").Rows.Count.ToString();
                            //return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                            //        && !MESDBHelper.IMesDbEx.OracleContain(t.SN, sws_wip) && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.COMPLETED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                        })();
                        //750 (WIP)
                        curenttres.W750 = new Func<string>(() => {
                            var wos = db.Queryable<R_WO_BASE>().Where(t => t.SKUNO == pn750 && t.CLOSED_FLAG == "0").Select(t => t.WORKORDERNO).ToList();
                            var noloading = 0;
                            foreach (var wo in wos)
                            {
                                var wonoloading = db.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where A.workorderno='{wo}' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' ) 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}' ");
                                if (wonoloading.Rows.Count > 0)
                                    noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                            }
                            return (db.Ado.GetDataTable($@"select*from r_sn a where skuno='{pn750}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                                    FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )  ").Rows.Count + noloading).ToString();
                                                                //return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                                                                //        && t.COMPLETED_FLAG == MesBool.No.ExtValue()).ToList().Count().ToString();
                        })();
                        //750 (Silver WIP)
                        curenttres.SW750 = new Func<string>(() => {
                            return db.Ado.GetDataTable($@"select* from r_sn a,r_juniper_silver_wip b where a.sn=b.sn and a.valid_flag='1' and a.skuno='{pn750}' and a.completed_flag='1' and b.STATE_FLAG='1' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%'").Rows.Count.ToString();
                        })();
                        var mainpn = db.Queryable<R_LINK_CONTROL>().Where(t => t.MAIN_ITEM == pn750 && t.SUB_ITEM.StartsWith("711") && pn711s.Contains(t.SUB_ITEM)).ToList().FirstOrDefault();

                        var res711s = new List<c20220115>();
                        for (int i = 0; i < pn711s.Count(); i++)
                        {
                            //var p711qty = db.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.SKUNO == pn711s[i]
                            //&& t.STARTED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                            var p711qty = new Func<string>(() => {
                                var wos = db.Queryable<R_WO_BASE>().Where(t => t.SKUNO == pn711s[i] && t.CLOSED_FLAG == "0").Select(t => t.WORKORDERNO).ToList();
                                var noloading = 0;
                                foreach (var wo in wos)
                                {
                                    var wonoloading = db.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where A.workorderno='{wo}' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%') 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}' ");
                                    if (wonoloading.Rows.Count > 0)
                                        noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                                }
                                return (db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                            FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )  ").Rows.Count + noloading).ToString();
                                                        })();
                            //db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' ").Rows.Count.ToString();
                            var p711smqty = db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711s[i]}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%'
                                                    and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                                                and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')    ").Rows.Count.ToString(); ;
                            res711s.Add(new c20220115() { W711_1 = p711qty, W711_1sm = p711smqty, W711_1PN = pn711s[i] });
                        }
                        if (res711s.Count > 0)
                        {
                            res711s = res711s.OrderBy(t => (int.Parse(t.W711_1sm) + int.Parse(t.W711_1))).ToList();
                            tol = int.Parse(curenttres.SM750) + int.Parse(curenttres.W750) + int.Parse(curenttres.SW750) + int.Parse(res711s.FirstOrDefault().W711_1) + int.Parse(res711s.FirstOrDefault().W711_1sm);
                        }
                        else
                            tol = int.Parse(curenttres.SM750) + int.Parse(curenttres.W750) + int.Parse(curenttres.SW750);
                        curenttres.tol = tol.ToString();
                        //if (pn750 == "750-059919")
                        //    pn750 = "750-059919";
                        var pn711sco = new List<string>();
                        if (mainpn != null)
                        {
                            pn711sco.Add(mainpn.SUB_ITEM);
                            var pnc = pn711s.FindAll(t => t != mainpn.SUB_ITEM).ToList();
                            foreach (var pncv in pnc)
                                pn711sco.Add(pncv);

                            for (int i = 0; i < pn711sco.Count(); i++)
                            {
                                //var p711qty = db.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.SKUNO == pn711s[i]
                                //&& t.STARTED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                                //var p711qty = db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711sco[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' ").Rows.Count.ToString();
                                var p711qty = new Func<string>(() => {
                                    var wos = db.Queryable<R_WO_BASE>().Where(t => t.SKUNO == pn711sco[i] && t.CLOSED_FLAG == "0").Select(t => t.WORKORDERNO).ToList();
                                    var noloading = 0;
                                    foreach (var wo in wos)
                                    {
                                        var wonoloading = db.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where A.workorderno='{wo}' and sn not like 'REV%' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%') 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}' ");
                                        if (wonoloading.Rows.Count > 0)
                                            noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                                    }
                                    return (db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711sco[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0'  and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                                    FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )  ").Rows.Count + noloading).ToString();
                                                                    })();

                                var p711smqty = db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711sco[i]}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%'
                                                    and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                                                and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')    ").Rows.Count.ToString(); ;
                                switch (i)
                                {
                                    case 0: curenttres.W711_1 = p711qty; curenttres.W711_1sm = p711smqty; curenttres.W711_1PN = pn711sco[i]; break;
                                    case 1: curenttres.W711_2 = p711qty; curenttres.W711_2sm = p711smqty; curenttres.W711_2PN = pn711sco[i]; break;
                                    case 2: curenttres.W711_3 = p711qty; curenttres.W711_3sm = p711smqty; curenttres.W711_3PN = pn711sco[i]; break;
                                    case 3: curenttres.W711_4 = p711qty; curenttres.W711_4sm = p711smqty; curenttres.W711_4PN = pn711sco[i]; break;
                                    case 4: curenttres.W711_5 = p711qty; curenttres.W711_5sm = p711smqty; curenttres.W711_5PN = pn711sco[i]; break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            pn711sco = pn711s;
                            //var res711s = new List<c20220115>();
                            //for (int i = 0; i < pn711sco.Count(); i++)
                            //{
                            //    //var p711qty = db.Queryable<R_SN>().Where(t => t.VALID_FLAG == MesBool.Yes.ExtValue() && t.SHIPPED_FLAG == MesBool.No.ExtValue() && t.SKUNO == pn711s[i]
                            //    //&& t.STARTED_FLAG == MesBool.Yes.ExtValue()).ToList().Count().ToString();
                            //    var p711qty = db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711sco[i]}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' ").Rows.Count.ToString();
                            //    var p711smqty = db.Ado.GetDataTable($@"select*from r_sn a where skuno='{ pn711sco[i]}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0'
                            //                            and sn not in (select sn from r_juniper_silver_wip where shipped_flag='0')
                            //                        and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')    ").Rows.Count.ToString(); ;
                            //    res711s.Add(new c20220115() { W711_1 = p711qty, W711_1sm = p711smqty, W711_1PN = pn711sco[i]  });
                            //}
                            for (int i = 0; i < res711s.Count(); i++)
                            {
                                switch (i)
                                {
                                    case 0: curenttres.W711_1 = res711s[i].W711_1; curenttres.W711_1sm = res711s[i].W711_1sm; curenttres.W711_1PN = res711s[i].W711_1PN; break;
                                    case 1: curenttres.W711_2 = res711s[i].W711_1; curenttres.W711_2sm = res711s[i].W711_1sm; curenttres.W711_2PN = res711s[i].W711_1PN; break;
                                    case 2: curenttres.W711_3 = res711s[i].W711_1; curenttres.W711_3sm = res711s[i].W711_1sm; curenttres.W711_3PN = res711s[i].W711_1PN; break;
                                    case 3: curenttres.W711_4 = res711s[i].W711_1; curenttres.W711_4sm = res711s[i].W711_1sm; curenttres.W711_4PN = res711s[i].W711_1PN; break;
                                    case 4: curenttres.W711_5 = res711s[i].W711_1; curenttres.W711_5sm = res711s[i].W711_1sm; curenttres.W711_5PN = res711s[i].W711_1PN; break;
                                    default:
                                        break;
                                }

                            }
                        }
                        res.Add(curenttres);
                    }
                    var key = DateTime.Now.ToString("yyyyMMddhhmmss");
                    //string filename = $@"C:\\Users\\G6001953.NN\\Desktop\\FJZ\\report\\{key}.csv";
                    var path = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Jnp\\report\\";
                    string filename = $@"{path}{key}.csv";
                //scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
                //ExcelHelp.ExportCsv(res, filename);


                DataTable dt = new DataTable();
                dt.Columns.Add("PN");
                dt.Columns.Add("SM750");
                dt.Columns.Add("W750");
                dt.Columns.Add("SW750");
                dt.Columns.Add("W711_1PN");
                dt.Columns.Add("W711_1sm");
                dt.Columns.Add("W711_1");
                dt.Columns.Add("W711_2PN");
                dt.Columns.Add("W711_2sm");
                dt.Columns.Add("W711_2");
                dt.Columns.Add("W711_3PN");
                dt.Columns.Add("W711_3sm");
                dt.Columns.Add("W711_3");
                dt.Columns.Add("W711_4PN");
                dt.Columns.Add("W711_4sm");
                dt.Columns.Add("W711_4");
                dt.Columns.Add("W711_5PN");
                dt.Columns.Add("W711_5sm");
                dt.Columns.Add("W711_5");
                dt.Columns.Add("tol");

                foreach (var item in res)
                {
                    var dr = dt.NewRow();
                    dr["PN"] = item.PN;
                    dr["SM750"] = item.SM750;
                    dr["W750"] = item.W750;
                    dr["SW750"] = item.SW750;
                    dr["W711_1PN"] = item.W711_1PN;
                    dr["W711_1sm"] = item.W711_1sm;
                    dr["W711_1"] = item.W711_1;
                    dr["W711_2PN"] = item.W711_2PN;
                    dr["W711_2sm"] = item.W711_2sm;
                    dr["W711_2"] = item.W711_2;
                    dr["W711_3PN"] = item.W711_3PN;
                    dr["W711_3sm"] = item.W711_3sm;
                    dr["W711_3"] = item.W711_3;
                    dr["W711_4PN"] = item.W711_4PN;
                    dr["W711_4sm"] = item.W711_4sm;
                    dr["W711_4"] = item.W711_4;
                    dr["W711_5PN"] = item.W711_5PN;
                    dr["W711_5sm"] = item.W711_5sm;
                    dr["W711_5"] = item.W711_5;
                    dr["tol"] = item.tol;
                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt);
                retTab.Tittle = "StationScanLog Report";
                Outputs.Add(retTab);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
