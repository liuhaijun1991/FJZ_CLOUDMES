using MESPubLab.MESStation;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MESReport.DCN
{
    /// <summary>
    /// 維修報表:Repair Report(FailDate) For DCN:DCN一大堆各種報表,照抄過來
    /// </summary>
    public class ShippingReport : ReportBase
    {
        ReportInput DN = new ReportInput() { Name = "DN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public ShippingReport()
        {
            Inputs.Add(DN);
            Inputs.Add(StartDate);
            Inputs.Add(EndDate);
        }

        public override void Init()
        {
            try
            {
                //OleExec SFCDB = DBPools["SFCDB"].Borrow();               
                StartDate.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                EndDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
                //DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Run()
        {
            //string DN = DN.Value.ToString();
            DateTime sTime = Convert.ToDateTime(StartDate.Value);
            DateTime eTime = Convert.ToDateTime(EndDate.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy/MM/dd");
            string eValue = eTime.ToString("yyyy/MM/dd");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string strsqlgetdn_list = $@"select distinct a.DN_NO
                                          from r_ship_detail a
                                         where length(DN_NO) = '10'  
                                           AND a.shipdate between to_date('{sValue}', 'yyyy-mm-dd') and to_date('{eValue}', 'yyyy-mm-dd') ";
                DataTable dn_list = SFCDB.ExecSelect(strsqlgetdn_list).Tables[0];

                for (int i = 0; i < dn_list.Rows.Count; i++)
                {
                    string DN_NO = "";
                    string strsku = "";
                    string strpo_no = "";
                    string strso_no = "";

                    DN_NO = dn_list.Rows[i][0].ToString();

                    var rsrp = SFCDB.ORM.Queryable<R_SHIPPING_REPORT>().Where(t => t.DN_NO == DN_NO).ToList();
                    if (rsrp.Count == 0)
                    //if (DN_NO == "3006001181")
                    {


                        var rds = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == DN_NO).First();
                        strpo_no = rds.PO_NO.ToString();
                        strso_no = rds.SO_NO.ToString();
                        strsku = rds.SKUNO.ToString();

                        var ship_qty = GetShip_QTY(SFCDB, DN_NO);
                        var PalletSum = GetPalletSum(SFCDB, DN_NO);
                        var pallet_qty = GetPallet_Qty_SUM(SFCDB, DN_NO, PalletSum);
                        var carton_qty = GetBox_qty_sum(SFCDB, DN_NO, PalletSum);

                        var total_gw = GetTotal_Gross_Weight_sum(SFCDB, DN_NO, strsku);
                        var total_nw = GetTotal_Net_Weight_sum(SFCDB, DN_NO, strsku);

                        string sql = $@"select to_char(shipdate, 'yyyy-mm-dd') shipdate
                                          from r_ship_detail
                                         where DN_NO = '{DN_NO}'
                                           and rownum = 1
                                         order by shipdate desc";

                        DataTable dtsd = SFCDB.ExecSelect(sql).Tables[0];
                        DateTime shipdate = Convert.ToDateTime(dtsd.Rows[0]["shipdate"].ToString());

                        T_R_SHIPPING_REPORT TRSR = new T_R_SHIPPING_REPORT(SFCDB, DB_TYPE_ENUM.Oracle);
                        Row_R_SHIPPING_REPORT RowRSR = (Row_R_SHIPPING_REPORT)TRSR.NewRow();
                        RowRSR.ID = (string)TRSR.GetNewID("VNDCN", SFCDB);
                        RowRSR.DN_NO = DN_NO;
                        RowRSR.PO_NO = strpo_no;
                        RowRSR.SO_NO = strso_no;
                        RowRSR.SKUNO = strsku;
                        RowRSR.SHIP_QTY = Convert.ToInt32(ship_qty);
                        RowRSR.PALLET_QTY = Convert.ToInt32(pallet_qty);
                        RowRSR.CARTON_QTY = Convert.ToInt32(carton_qty);
                        RowRSR.GROSS_WEIGHT = total_gw;
                        RowRSR.NET_WEIGHT = total_nw;
                        RowRSR.SHIP_DATE = shipdate;
                        RowRSR.DATA1 = "";
                        RowRSR.DATA2 = "";
                        RowRSR.DATA3 = "";
                        SFCDB.ExecSQL(RowRSR.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }

                string runSql = $@"select DN_NO,
                                   po_no,
                                   so_no,
                                   skuno,
                                   ship_qty,
                                   pallet_qty,
                                   carton_qty,
                                   gross_weight,
                                   net_weight,
                                   to_char(ship_date, 'yyyy-mm-dd') ship_date
                              from r_shipping_report
                             where ship_date between to_date('{sValue}', 'yyyy-mm-dd') and to_date('{eValue}', 'yyyy-mm-dd') 
                             TEMP_SKUSQL
                             ";

                if (!string.IsNullOrEmpty(DN.Value.ToString()))
                {
                    runSql = runSql.Replace("TEMP_SKUSQL", $@" and dn_no = '{DN.Value.ToString()}' order by ship_date ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_SKUSQL", $@" order by ship_date ");
                }

                DataTable resDT = SFCDB.RunSelect(runSql).Tables[0];
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, null);
                retTab.Tittle = "SHIPPING WEIGHT REPORT";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                //DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public string GetShip_QTY(OleExec SFCDB, string DN_NO)
        {
            var ship = SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(t => t.DN_NO == DN_NO).ToList();
            int c = 0;
            c = ship.Count;
            return c.ToString();
        }

        public string GetPL_SN_QTY(OleExec SFCDB, string PACKNO)
        {
            var packno = SFCDB.ORM.Queryable<R_PACKING>();

            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            var ctns = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == pack.ID).ToList();
            if (ctns.Count == 0)
            {
                return pack.QTY.ToString();
            }

            int c = 0;
            for (int i = 0; i < ctns.Count; i++)
            {
                c += (int)ctns[i].QTY;
            }

            return c.ToString();
        }

        public string GetPallet_Qty_SUM(OleExec SFCDB, string DN_NO, string PalletSum)
        {
            int PALLET_REMNANT = 0;
            var Pallet_Qty_sum = 0;
            string Strsql = $@"select distinct e.pack_no, e.skuno
                                  From r_ship_detail a, r_sn b, r_sn_packing c, R_PACKING d, R_PACKING e
                                 where a.dn_no = '{DN_NO}'
                                   and a.sn = b.sn
                                   and b.id = c.SN_ID
                                   and c.PACK_ID = d.id
                                   and d.PARENT_PACK_ID = e.id";
            var pack = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < pack.Rows.Count; i++)
            {
                var qty = GetPL_SN_QTY(SFCDB, pack.Rows[i][0].ToString());
                var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == pack.Rows[i][1].ToString()).First();
                if (cpw == null)
                {
                    throw new Exception("SkuNo:[" + pack.Rows[i][1].ToString() + "],Weight information is not Configured!");
                }
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (BOX_FULL_REMNAN >= 4)
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
                }
                Pallet_Qty_sum = PALLET_REMNANT * int.Parse(PalletSum);
            }
            return Pallet_Qty_sum.ToString();

        }
        public string GetBox_qty_sum(OleExec SFCDB, string DN_NO, string PalletSum)
        {
            int BOX_REMNANT = 0;
            int BOX_SINGLE = 0;
            var Box_qty_sum = 0;
            string Strsql = $@"select distinct e.pack_no, e.skuno
                                  From r_ship_detail a, r_sn b, r_sn_packing c, R_PACKING d, R_PACKING e
                                 where a.dn_no = '{DN_NO}'
                                   and a.sn = b.sn
                                   and b.id = c.SN_ID
                                   and c.PACK_ID = d.id
                                   and d.PARENT_PACK_ID = e.id";
            var pack = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < pack.Rows.Count; i++)
            {
                var qty = GetPL_SN_QTY(SFCDB, pack.Rows[i][0].ToString());
                var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == pack.Rows[i][1].ToString()).First();
                if (cpw == null)
                {
                    throw new Exception("SkuNo:[" + pack.Rows[i][1].ToString() + "],Weight information is not Configured!");
                }
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (PCS_B_REMNANT > 0)
                {
                    BOX_SINGLE = 1;
                }

                BOX_REMNANT = BOX_FULL_REMNAN + BOX_SINGLE;
                Box_qty_sum = BOX_REMNANT * int.Parse(PalletSum);
            }
            return Box_qty_sum.ToString();
        }
        public string GetPALLET_NET(OleExec SFCDB, string DN_NO, string SKUNO)
        {
            var qty = GetPL_SN_QTY(SFCDB, DN_NO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            if (cpw == null)
            {
                throw new Exception("SkuNo:[" + SKUNO + "],Weight information is not Configured!");
            }

            var PALLET_NET = Convert.ToDecimal(cpw.PCS_NT) * Convert.ToInt32(qty);

            return PALLET_NET.ToString();

        }
        public string GetPALLET_GW(OleExec SFCDB, string DN_NO, string SKUNO)
        {
            var PALLET_GW = "";
            var WEIGHT_REMNANT = "";
            int PALLET_REMNANT = 0;
            int BOX_REMNANT = 0;
            var qty = GetPL_SN_QTY(SFCDB, DN_NO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            if (cpw == null)
            {
                throw new Exception("SkuNo:[" + SKUNO + "],Weight information is not Configured!");
            }
            if (qty == cpw.PCS_P)
            {
                PALLET_GW = cpw.P_GW.ToString();
            }
            else
            {
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (PCS_B_REMNANT > 0)
                {
                    BOX_REMNANT = 1;
                }
                if (BOX_FULL_REMNAN >= 4)
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
                    WEIGHT_REMNANT = W_RT.ToString();

                }
                else
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    WEIGHT_REMNANT = W_RT.ToString();
                }
                var P_GW = Convert.ToDecimal(WEIGHT_REMNANT) + PALLET_REMNANT * Convert.ToDecimal(cpw.P_NULLWG) + BOX_REMNANT * (Convert.ToDecimal(cpw.BOX_GW) - Convert.ToDecimal(cpw.BOX_NT));

                //PALLET_GW = P_GW.ToString() + "kg";
                PALLET_GW = P_GW.ToString();
            }

            return PALLET_GW.ToString();

        }
        public string GetTotal_Gross_Weight_sum(OleExec SFCDB, string DN_NO, string SKUNO)
        {

            float Total_Gross_Weight_sum = 0;
            string Strsql = $@"select distinct e.pack_no, e.skuno
                                  From r_ship_detail a, r_sn b, r_sn_packing c, R_PACKING d, R_PACKING e
                                 where a.dn_no = '{DN_NO}'
                                   and a.sn = b.sn
                                   and b.id = c.SN_ID
                                   and c.PACK_ID = d.id
                                   and d.PARENT_PACK_ID = e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < Pallet.Rows.Count; i++)
            {
                var PALLET_GW = GetPALLET_GW(SFCDB, Pallet.Rows[i][0].ToString(), Pallet.Rows[i][1].ToString());
                PALLET_GW = PALLET_GW.Replace("kg", "").ToString();
                Total_Gross_Weight_sum += float.Parse(PALLET_GW);
            }
            string Total_Gross_Weight_Sum = Total_Gross_Weight_sum.ToString();
            if (!string.IsNullOrEmpty(Total_Gross_Weight_Sum) && (System.Text.RegularExpressions.Regex.IsMatch(Total_Gross_Weight_Sum, @"^[1-9]\d*|0$")
                 || System.Text.RegularExpressions.Regex.IsMatch(Total_Gross_Weight_Sum, @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
            {
                Total_Gross_Weight_Sum = (Convert.ToDecimal(Total_Gross_Weight_Sum).ToString("F2"));
            }

            return Total_Gross_Weight_Sum + "kg";
        }
        public string GetTotal_Net_Weight_sum(OleExec SFCDB, string DN_NO, string SKUNO)
        {

            float Total_Net_Weight_sum = 0;
            string Strsql = $@"  select distinct e.pack_no,e.skuno From r_ship_detail  a,r_sn b,r_sn_packing c,R_PACKING d,R_PACKING e 
                                                    where a.dn_no='{DN_NO}' and a.sn=b.sn and b.id=c.SN_ID and c.PACK_ID=d.id and d.PARENT_PACK_ID=e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < Pallet.Rows.Count; i++)
            {
                var PALLET_NET = GetPALLET_NET(SFCDB, Pallet.Rows[i][0].ToString(), Pallet.Rows[i][1].ToString());
                PALLET_NET = PALLET_NET.Replace("kg", "").ToString();
                Total_Net_Weight_sum += float.Parse(PALLET_NET);
            }

            return Total_Net_Weight_sum.ToString() + "kg";
        }
        public string GetPalletSum(OleExec SFCDB, string DN_NO)
        {
            int PalletSum = 0;
            string Strsql = $@"  select distinct e.pack_no,e.skuno From r_ship_detail  a,r_sn b,r_sn_packing c,R_PACKING d,R_PACKING e 
                                                    where a.dn_no='{DN_NO}' and a.sn=b.sn and b.id=c.SN_ID and c.PACK_ID=d.id and d.PARENT_PACK_ID=e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);
            PalletSum = Pallet.Rows.Count;
            return PalletSum.ToString();
        }
    }
}
