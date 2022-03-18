using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESPubLab;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject;

namespace MESJuniper.Api
{
    public class JnpReportApi : MesAPIBase
    {
        protected APIInfo FGet750and711WipData = new APIInfo
        {
            FunctionName = "Get750and711WipData",
            Description = "Get750and711WipData",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="PlantCode", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };


        public JnpReportApi()
        {
            this.Apis.Add(FGet750and711WipData.FunctionName, FGet750and711WipData);
        }

        class data750and711Wip
        {
            public string pn { get; set; }
            public string plantcode { get; set; }
            public string smqty { get; set; }
            public string wipqty { get; set; }
            public string swqty { get; set; }
        }

        public void Get750and711WipData(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var wopre = new string[] { "006A", "0091", "0092", "0093", "0094" };
                var plantcode = Data["PlantCode"].ToString().Trim();
                var skus = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO.StartsWith("750") || t.SKUNO.StartsWith("711")).OrderBy(t => t.SKUNO).Select(t => t.SKUNO).Distinct().ToList();
                var res = new List<data750and711Wip>();
                foreach (var item in skus)
                {
                    var ires = new data750and711Wip();
                    ires.pn = item;
                    ires.plantcode = plantcode;

                    ires.wipqty = new Func<string>(() =>
                    {
                        var wos = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == item && t.CLOSED_FLAG == "0" && wopre.Contains(SqlSugar.SqlFunc.Substring(t.WORKORDERNO, 0, 4))).Select(t => t.WORKORDERNO).ToList();
                        var noloading = 0;
                        foreach (var wo in wos)
                        {
                            var wonoloading = SFCDB.ORM.Ado.GetDataTable($@"SELECT WORKORDER_QTY-(select COUNT(distinct(sn)) as noloading from r_sn A where   SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND A.workorderno='{wo}' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' ) 
                                                FROM R_WO_BASE  WHERE workorderno='{wo}'");
                            if (wonoloading.Rows.Count > 0)
                                noloading += int.Parse(wonoloading.Rows[0][0].ToString());
                        }
                        return (SFCDB.ORM.Ado.GetDataTable($@"select*from r_sn a where SUBSTR(a.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') AND skuno='{item}' and valid_flag='1' and completed_flag='0' and  repair_failed_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                                                and a.sn not like '*%' AND NOT EXISTS ( SELECT * FROM  (SELECT C.*,ROW_NUMBER() OVER(PARTITION BY C.SN,C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS 
                                    FROM R_TEST_RECORD C WHERE A.SN=C.SN AND A.NEXT_STATION=C.MESSTATION   ) WHERE NUMS='1' AND  STATE='FAIL'  )   ").Rows.Count + noloading).ToString();
                        //return db.Queryable<R_SN>().Where(t => t.SKUNO == pn750 && t.VALID_FLAG == MesBool.Yes.ExtValue()
                        //        && t.COMPLETED_FLAG == MesBool.No.ExtValue()).ToList().Count().ToString();
                    })();
                    var tolsmqty = new Func<string>(() =>
                    {
                        return SFCDB.ORM.Ado.GetDataTable($@" SELECT distinct sn
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                and skuno='{item}')
                                           AND S.COMPLETED_FLAG = 1
                                            and SUBSTR(s.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') 
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG in ('1','2')
                                           AND S.NEXT_STATION <> 'REWORK'
                                            and s.sn not in (select sn from r_juniper_silver_wip where state_flag='1')
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )   ").Rows.Count.ToString();
                        //return SFCDB.ORM.Ado.GetDataTable($@"select*from r_sn a where skuno='{item}' and valid_flag='1' and completed_flag='1' AND A.CURRENT_station<>'MRB' and shipped_flag='0' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                        //                        and a.sn not like '*%'
                        //                            and sn not in (select sn from r_juniper_silver_wip where state_flag='0')
                        //and not exists (select 1 from r_sn_kp b where a.sn=b.value  and valid_flag='1' and scantype<>'KEEP_SN')   ").Rows.Count.ToString();
                    })();
                    if (item.StartsWith("750"))
                    {
                        ires.smqty = new Func<string>(() =>
                        {
                            return SFCDB.ORM.Ado.GetDataTable($@"  SELECT 1 FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO                                                   
                                                    and skuno='{item}')
                                           and EXISTS (select * from r_supermarket mm where mm.r_sn_id = s.id and mm.status = 1 )
                                            and SUBSTR(s.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') 
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )   ").Rows.Count.ToString();
                        })();
                        ires.wipqty = (Convert.ToInt64(ires.wipqty) + Convert.ToInt64(tolsmqty) - Convert.ToInt64(ires.smqty)).ToString();
                    }
                    else
                        ires.smqty = tolsmqty;
                    ires.swqty = new Func<string>(() =>
                    {
                        return SFCDB.ORM.Ado.GetDataTable($@"SELECT distinct SN
                                          FROM R_SN S
                                         WHERE EXISTS
                                         (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO
                                                    and ww.skuno='{item}'
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
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )").Rows.Count.ToString();
                        //return SFCDB.ORM.Ado.GetDataTable($@"select* from r_sn a,r_juniper_silver_wip b where a.sn=b.sn and a.valid_flag='1' and a.skuno='{item}' and a.completed_flag='1' and b.STATE_FLAG='1' and a.sn not like 'REV%' and a.sn not like '~%' and a.sn not like '#%' 
                        //                        and a.sn not like '*%'").Rows.Count.ToString();
                    })();
                    res.Add(ires);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
