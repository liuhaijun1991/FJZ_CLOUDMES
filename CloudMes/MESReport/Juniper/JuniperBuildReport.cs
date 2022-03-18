using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperBuildReport : ReportBase
    {
        ReportInput PONO = new ReportInput { Name = "PO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SO = new ReportInput { Name = "SO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CUSTPID = new ReportInput { Name = "CUSTPID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput GROUPID = new ReportInput { Name = "GROUPID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CUSTDN = new ReportInput { Name = "CUSTDN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            Value = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput toDate = new ReportInput()
        {
            Name = "To",
            InputType = "DateTime",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        public JuniperBuildReport()
        {
            Inputs.Add(PONO);
            Inputs.Add(SO);
            Inputs.Add(WO);
            Inputs.Add(SKUNO);
            Inputs.Add(CUSTPID);
            Inputs.Add(GROUPID);
            Inputs.Add(CUSTDN);
            this.Inputs.Add(fromDate);
            this.Inputs.Add(toDate);
        }

        public override void Run()
        {
            DataTable dt = new DataTable();
            string runSql = "";
            string po = PONO.Value.ToString().Trim();
            string so = SO.Value.ToString().Trim();
            string wo = WO.Value.ToString().Trim();
            string skuno = SKUNO.Value.ToString().Trim();
            string custpid = CUSTPID.Value.ToString().Trim();
            string groupid = GROUPID.Value.ToString().Trim();
            string custdn = CUSTDN.Value.ToString().Trim();
            string start = null;
            string end = null;
            if (fromDate.Value != null && fromDate.Value.ToString() != "")
            {
                try
                {
                    start = Convert.ToDateTime(fromDate.Value.ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch (Exception)
                {
                    throw new Exception("Incorrect date format！");
                }

            }
            if (toDate.Value != null && toDate.Value.ToString() != "")
            {
                try
                {
                    end = Convert.ToDateTime(toDate.Value.ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                }
                catch (Exception)
                {
                    throw new Exception("Incorrect date format！");
                }

            }

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlWO = "",sqlSO="",sqlPO="",sqlSku="", sqltime="", sqlcustpid = "", sqlgroupid = "", sqlcustdn = "";
            string workorderno = "";
            try
            {
                //if (string.IsNullOrWhiteSpace(po) && string.IsNullOrWhiteSpace(so) && string.IsNullOrWhiteSpace(wo) && string.IsNullOrWhiteSpace(skuno))
                //    throw new Exception("Please input PO or SO or WO or SKUNO");
                if (!string.IsNullOrWhiteSpace(po))
                {
                    sqlPO = $" and C.pono='{po}'";
                }
                if (!string.IsNullOrWhiteSpace(so))
                {
                    sqlSO = $" and F.salesordernumber='{so}'";
                }
                if (!string.IsNullOrWhiteSpace(wo))
                {
                    sqlWO = $" and C.prewo='{wo}'";
                }
                if (!string.IsNullOrWhiteSpace(custpid))
                {
                    sqlcustpid = $" and C.CUSTPID='{custpid}'";
                }
                if (!string.IsNullOrWhiteSpace(groupid))
                {
                    sqlgroupid = $" and d.GROUPID='{groupid}'";
                }
                if (!string.IsNullOrWhiteSpace(custdn))
                {
                    sqlcustdn = $" and R.DELIVERYNUMBER='{custdn}'";
                }
                if (!string.IsNullOrWhiteSpace(skuno))
                {
                    var skunolist = "''";
                    foreach (var item in skuno.Split(','))
                        skunolist += $@",'{item}'";
                    sqlSku = $" and d.PID in ({skunolist}) or d.GROUPID in  ({skunolist})";
                }
                if (!string.IsNullOrWhiteSpace(start) && !string.IsNullOrWhiteSpace(end))
                {
                    sqltime = $" and c.delivery between to_date('{start}','YYYY-MM-DD HH24:MI:SS') AND to_date('{end}','YYYY-MM-DD HH24:MI:SS')  ";
                }
                string sqlcustomer = $@"select*From c_customer";
                DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
                if (dcc.Rows[0]["BU"].ToString() == "VNJUNIPER")
                {
                    runSql = $@"SELECT DISTINCT aa.WORKORDERNO,aa.SKUNO,CUSTPID,GROUPID,DESCRIPTION,QUANTITY,CARTONNO,GROSSWEIGHT,rk.PARTNO,rk.LOCATION,cust_po_number,cust_SO,cust_DN,FXVNDN,SHIPSTATUS
                              FROM(SELECT C.PREWO as WORKORDERNO,
                                           C.pid as SKUNO,
                                           C.CUSTPID,
                                           d.GROUPID,
                                           TRIM(A.DESCRIPTION) AS DESCRIPTION,
                                           '0' as QUANTITY, 
                                           '0' as CARTONNO,
                                           '0' as GROSSWEIGHT,
                                           '' as COO,
                                           c.pono as cust_po_number,
                                           f.salesordernumber as cust_SO,'' as cust_DN,
                                           case when exists(select*From r_ship_detail aa,r_sn bb where aa.sn=bb.sn and bb.workorderno=c.prewo and bb.skuno=c.pid and bb.valid_flag=1) then
                                           (select distinct aa.dn_no From r_ship_detail aa,r_sn bb where aa.sn=bb.sn 
                                           and bb.workorderno=c.prewo and bb.valid_flag=1 and bb.skuno=c.pid) else '' end FXVNDN,
                                           decode (c.finalasn,'0','WaitShip','Shipped') as SHIPSTATUS
                                      FROM(select *
                                              from o_agile_attr
                                             where rowid in (select max(rowid)
                                                               from o_agile_attr
                                                              where DESCRIPTION is not null
                                                               group by item_number)) A,
                                           O_ORDER_MAIN C,
                                           r_pre_wo_head d,
                                           O_I137_ITEM e,
                                           O_I137_HEAD f,
                                           R_I282 R
                                     WHERE A.item_number = c.pid
                                       and d.wo = c.prewo
                                       and c.itemid = e.id
                                       and e.tranid = f.tranid
                                       and e.actioncode <> '02'
                                       AND C.PREASN=R.ASNNUMBER
                                       AND A.DESCRIPTION is not null {sqlPO} {sqlWO} {sqlSO} {sqlSku} {sqltime} {sqlcustpid} {sqlgroupid} {sqlcustdn}
                                   ) aa,R_SN_KP rk, R_SN rs where rs.ID=rk.R_SN_ID 
                                   and aa.WORKORDERNO=rs.WORKORDERNO and  rs.VALID_FLAG='1'
                                   and rk.KP_NAME like '%AutoKP%'";

                }
                else
                {
                    runSql = $@"SELECT DISTINCT aa.WORKORDERNO,aa.SKUNO,CUSTPID,GROUPID,DESCRIPTION,QUANTITY,CARTONNO,GROSSWEIGHT,rk.PARTNO,rk.LOCATION,cust_po_number,cust_SO,cust_DN,SHIPSTATUS
                              FROM(SELECT C.PREWO as WORKORDERNO,
                                           C.pid as SKUNO,
                                           C.CUSTPID,
                                           d.GROUPID,
                                           TRIM(A.DESCRIPTION) AS DESCRIPTION,
                                           '0' as QUANTITY, 
                                           '0' as CARTONNO,
                                           '0' as GROSSWEIGHT,
                                           '' as COO,
                                           c.pono as cust_po_number,
                                           f.salesordernumber as cust_SO,'' as cust_DN,
                                           decode (c.finalasn,'0','WaitShip','Shipped') as SHIPSTATUS
                                      FROM(select *
                                              from o_agile_attr
                                             where rowid in (select max(rowid)
                                                               from o_agile_attr
                                                              where DESCRIPTION is not null
                                                               group by item_number)) A,
                                           O_ORDER_MAIN C,
                                           r_pre_wo_head d,
                                           O_I137_ITEM e,
                                           O_I137_HEAD f,
                                           R_I282 R
                                     WHERE A.item_number = c.pid
                                       and d.wo = c.prewo
                                       and c.itemid = e.id
                                       and e.tranid = f.tranid
                                       and e.actioncode <> '02'
                                       AND C.PREASN=R.ASNNUMBER
                                       AND A.DESCRIPTION is not null {sqlPO} {sqlWO} {sqlSO} {sqlSku} {sqltime} {sqlcustpid} {sqlgroupid} {sqlcustdn}
                                   ) aa,R_SN_KP rk, R_SN rs where rs.ID=rk.R_SN_ID 
                                   and aa.WORKORDERNO=rs.WORKORDERNO and  rs.VALID_FLAG='1'
                                   and rk.KP_NAME like '%AutoKP%'";
                }


                    
                
                dt = SFCDB.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                T_R_WEIGHT t_r_weight = new T_R_WEIGHT(SFCDB, DB_TYPE_ENUM.Oracle);
                foreach (DataRow row in dt.Rows)
                {
                    workorderno = row["WORKORDERNO"].ToString();
                    var snPackingList = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((sn, rsp) => sn.ID == rsp.SN_ID).Where((sn, rsp) => sn.WORKORDERNO == workorderno && sn.VALID_FLAG == "1")
                        .Select((sn, rsp) => rsp).ToList();                   

                    var cartonid = snPackingList.Select(r => r.PACK_ID).Distinct().ToList();
                    var snid = snPackingList.Select(r => r.SN_ID).Distinct().ToList();
                    row["QUANTITY"] = snid.Count();

                    row["CARTONNO"] = cartonid.Count();

                    row["GROSSWEIGHT"] = new Func<string>(() =>
                    {
                        if (cartonid.Count() == 0)
                            return "0";
                        double grossweight = 0;                        
                        var palletList = SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((carton, pallet) => carton.PARENT_PACK_ID == pallet.ID)
                        //.Where((carton, pallet) => SqlSugar.SqlFunc.ContainsArray(cartonid, carton.ID))
                        .Where((carton, pallet) => IMesDbEx.OracleContain(carton.ID, cartonid))
                        .Select((carton, pallet) => pallet.PACK_NO).ToList();
                        foreach (var item in palletList)
                        {
                            string gw = t_r_weight.GetallCTNweight(item, "VNJUNIPER", SFCDB);
                            grossweight += Convert.ToDouble(gw==""?"0":gw);
                        }                       
                        return grossweight.ToString();
                    })();

                    //row["COO"] = new Func<string>(() =>
                    //{
                    //    //var cooList = SFCDB.ORM.Queryable<R_SN_KP, MESDataObject.Module.OM.O_ORDER_MAIN>((r, o) => r.PARTNO == o.PID)
                    //    ////.Where((r, o) => o.PREWO == workorderno && SqlSugar.SqlFunc.ContainsArray(snid, r.R_SN_ID))
                    //    //.Where((r, o) => o.PREWO == workorderno && IMesDbEx.OracleContain(r.R_SN_ID,snid))
                    //    //.Select((r, o) => r.LOCATION).Distinct().ToList();
                    //    var cooList = SFCDB.ORM.Queryable<R_SN_KP, MESDataObject.Module.OM.O_ORDER_MAIN,R_SN>((rk, o,rs) => rs.ID==rk.R_SN_ID&&o.PREWO==rs.WORKORDERNO)
                    // .Where((rk, o, rs) => o.PREWO == workorderno && rs.VALID_FLAG=="1"&&rk.KP_NAME.Contains("AutoKP"))
                    // .Select((rk, o, rs) => rk.LOCATION).Distinct().ToList();
                    //    var Detail = "";
                    //    if (cooList.Count > 1)
                    //    {
                    //        string[] coo = new string[cooList.Count];
                    //        for (int i = 0; i < cooList.Count; i++)
                    //        {
                    //            coo[i] = cooList[i].ToString();
                    //        }

                    //        Detail = string.Join(",", coo);
                    //    }
                    //    return Detail.ToString();
                    //})();

                    row["CUST_DN"] = new Func<string>(() =>
                    {
                        return SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282, MESDataObject.Module.OM.O_ORDER_MAIN>((r, o) => r.ASNNUMBER == o.PREASN)
                        .Where((r, o) => o.PREWO == workorderno && SqlSugar.SqlFunc.IsNullOrEmpty(r.ERRORCODE) == true)
                        .OrderBy((r, o) => r.CREATETIME, SqlSugar.OrderByType.Desc).Select((r, o) => r.DELIVERYNUMBER)
                        .ToList().FirstOrDefault();
                    })();
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "POSOReport";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
