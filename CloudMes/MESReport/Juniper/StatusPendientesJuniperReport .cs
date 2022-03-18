using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;

namespace MESReport.Juniper
{
    public class StatusPendientesJuniperReport : ReportBase
    {

        List<string> tittles = new List<string>()
        {"711's", "Part No(750's)", "WO",  "SKU",  "MRR",  "MRRDate", "QTY", "Router",   
            "PTH",  "Pressfit", "5DX",  "ICT",  "ICT Fails",    "PVA",  "Heatsink press", "Heatsink Assy.",   
            "SI LOADING",   "Mech Assy",  "FCT TEST", "RIT TEST", "FST SAPARE",   
            "DVT TEST", "SUPER MARKET", "Pending"};

        List<string> FillZeroGroup = new List<string> { "PTH", "Pressfit", "5DX", "ICT", "ICT Fails", "PVA", "SI LOADING",   "Mech Assy",  "FCT TEST", "RIT TEST", "FST SAPARE",
            "DVT TEST", "SUPER MARKET", "Pending"};

        List<R_WO_BASE> GetWOs(OleExec sfcdb, string skuno)
        {
            var wos = sfcdb.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == skuno).ToList();
            return wos;
        }

        DataTable GetReportTable()
        {
            DataTable ret = new DataTable();
            for (int i = 0; i < tittles.Count; i++)
            {
                ret.Columns.Add(tittles[i]);
            }
            return ret;
        }

        List<string> Get750SKU(OleExec sfcdb)
        {
            var r = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "StatusPendientesJuniperReport").Select(t => t.SKUNO).Distinct().ToList();
            var ret = r.FindAll(t => !t.StartsWith("CONFIG"));
            return ret;
        }
        List<string>Get711SKU(OleExec sfcdb,string _750SKU)
        {
            var r = sfcdb.ORM.Queryable<C_SKU_DETAIL>()
                .Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "StatusPendientesJuniperReport"
                && t.SKUNO==_750SKU).Select(t => t.VALUE).Distinct().ToList();
            return r;
        }

        void FillData(string col, string data, List<DataRow> rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                try
                {
                    rows[i][col] = data;
                }
                catch
                { }
            }
        }


        public override void Run()
        {
            var rep = GetReportTable();
            var link = GetReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            List<string> _750Skus;
            Dictionary<string, List<string>> LinkMap = new Dictionary<string, List<string>>();
            try
            {
                _750Skus = Get750SKU(SFCDB);
                for (int i = 0; i < _750Skus.Count; i++)
                {
                    List<DataRow> data = new List<DataRow>();//; = rep.NewRow();
                    List<DataRow> linkdata = new List<DataRow>();// = link.NewRow();
                    var _711s = Get711SKU(SFCDB, _750Skus[i]);
                    LinkMap.Add(_750Skus[i], _711s);
                    var _750sku = _750Skus[i];
                    for (int j = 0; j < _711s.Count; j++)
                    {
                        var d = rep.NewRow();
                        d["711's"] = _711s[j];
                        d["Part No(750's)"] = _750sku;

                        var l = link.NewRow();
                        rep.Rows.Add(d);
                        link.Rows.Add(l);
                        data.Add(d);
                        var link750 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SkuWoReportNew&RunFlag=1&SKUNO=" + _750sku + "&CloseFlag=ALL&GROUP_TYPE=SKU"; 
                        l["Part No(750's)"] = link750;
                        linkdata.Add(l);
                    }
                    //750BaseData
                    var _750b = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == _750sku).First();
                    FillData("SKU", _750b.CUST_PARTNO, data);
                    var _750W0s = GetWOs(SFCDB, _750sku);
                    double? _750QTY = 0;
                    string str_750Wo = "";
                    double? _750Siloading = 0;
                    for (int j = 0; j < _750W0s.Count; j++)
                    {
                        _750QTY += _750W0s[j].WORKORDER_QTY;
                        str_750Wo += _750W0s[j].WORKORDERNO + " ";
                        _750Siloading += _750W0s[j].WORKORDER_QTY - _750W0s[j].INPUT_QTY;
                    }
                    FillData("WO", str_750Wo, data);
                    FillData("QTY", _750QTY.ToString(), data);
                    FillData("SI LOADING", _750Siloading.ToString(), data);

                    //711 station
                    for (int j = 0; j < data.Count; j++)
                    {
                       
                        var _711sku = data[j]["711's"].ToString();
                        var link711 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SkuWoReportNew&RunFlag=1&SKUNO=" + _711sku + "&CloseFlag=ALL&GROUP_TYPE=SKU";
                        linkdata[j]["711's"] = link711;
                        int wipqty = 0;
                        int failqty = 0;
                        #region ICT
                        var sns = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_711sku}' and s.next_station like 'ICT%'
                                  and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();

                        
                        for (int k = 0; k < sns.Count; k++)
                        {
                            if (sns[k].REPAIR_FAILED_FLAG == "1")
                            {
                                failqty++;
                                continue;
                            }
                            else
                            {
                                var testrecort = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.R_SN_ID == sns[k].ID && t.MESSTATION == "ICT")
                                    .OrderBy(t => t.ENDTIME, SqlSugar.OrderByType.Desc).ToList();
                                if (testrecort.Count == 0)
                                {
                                    wipqty++;
                                    continue;
                                }
                                if (testrecort[0].STATE.ToUpper() == "PASS")
                                {
                                    wipqty++;
                                    continue;
                                }
                                else
                                {
                                    failqty++;
                                    continue;
                                }

                            }


                        }
                        data[j]["ICT"] = wipqty;
                        data[j]["ICT Fails"] = failqty;
                        #endregion

                        #region 5DX
                        sns = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_711sku}' and s.next_station like '5DX%'
                              and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                       
                        if (sns.Count > 0)
                        {
                            data[j]["ICT Fails"] = sns.Count;
                        }

                        #endregion

                        #region Pressfit
                        sns = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_711sku}' and s.next_station like 'PRESS-FIT'
                              and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                        if (sns.Count > 0)
                        {
                            data[j]["Pressfit"] = sns.Count;
                        }
                        #endregion

                        #region PTH
                        sns = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_711sku}' and s.next_station like 'PTH'
                              and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                        if (sns.Count > 0)
                        {
                            data[j]["PTH"] = sns.Count;
                        }
                        #endregion

                    }

                    //750 station
                    #region MA
                    var SNs = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_750sku}' and s.next_station like 'MA%'
                              and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                    if (SNs.Count > 0)
                    {
                        FillData("Mech Assy", SNs.Count.ToString(), data);
                    }
                    #endregion

                    #region FCT TEST
                     SNs = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_750sku}' and s.next_station in ('FPCTEST','PICTEST')
                           and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                    if (SNs.Count > 0)
                    {
                        FillData("FCT TEST", SNs.Count.ToString(), data);
                    }
                    #endregion

                    #region RIT TEST
                    SNs = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_750sku}' and s.next_station in ('RIT')
                          and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                    if (SNs.Count > 0)
                    {
                        FillData("RIT TEST", SNs.Count.ToString(), data);
                    }
                    #endregion
                    #region FST_SPARES
                    SNs = SFCDB.ORM.SqlQueryable<R_SN>($@"select * from r_sn s where s.skuno ='{_750sku}' and s.next_station in ('FST_SPARES')
                          and exists(select * from r_wo_base b where b.workorderno = s.workorderno ) and valid_flag = 1").ToList();
                    if (SNs.Count > 0)
                    {
                        FillData("FST SAPARE", SNs.Count.ToString(), data);
                    }
                    #endregion



                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(rep, link);
                retTab.FixedHeader = true;
                retTab.Tittle = $@"Status Pendientes Juniper Integracion";
                for (int i = 0; i < _750Skus.Count; i++)
                {
                    var rs = retTab.Rows.FindAll(t => t.RowData["Part No(750's)"].Value == _750Skus[i]);
                    if (rs.Count > 1)
                    {
                        rs[0]["Part No(750's)"].RowSpan = rs.Count;
                        rs[0]["SI LOADING"].RowSpan = rs.Count;
                        rs[0]["Mech Assy"].RowSpan = rs.Count;
                        rs[0]["FCT TEST"].RowSpan = rs.Count;
                        rs[0]["RIT TEST"].RowSpan = rs.Count;

                        rs[0]["FST SAPARE"].RowSpan = rs.Count;
                        rs[0]["DVT TEST"].RowSpan = rs.Count;
                        rs[0]["SUPER MARKET"].RowSpan = rs.Count;
                        rs[0]["Pending"].RowSpan = rs.Count;
                        rs[0]["WO"].RowSpan = rs.Count;
                        rs[0]["SKU"].RowSpan = rs.Count;
                        

                        //rs[0].RowStyle

                        for (int j = 1; j < rs.Count; j++)
                        {
                            //rs[j].Remove("Part No(750's)");
                        }
                    }

                    
                    //rs[0].RowStyle = "background-color: rgba(0,0,0,0.05)";
                    for (int j = 0; j < rs.Count; j++)
                    {
                        var keys = rs[j].Keys;
                        foreach (string k in keys)
                        {
                            if (i % 2 == 1)
                            {
                                rs[j][k].CellStyle.Add("background-color", "rgba(0,0,0,0.05)");
                            }
                            
                            if(FillZeroGroup.Contains(k) && (rs[j][k].Value == null || rs[j][k].Value == ""))
                            {
                                rs[j][k].Value = "0";
                            }
                        }

                    }
                    

                }
                retTab.FixedHeader = true;
                retTab.FixedCol = 4;
                retTab.MergeCells = true;
                this.Outputs.Add(retTab);
            }
            catch (Exception ee)
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
