using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class MoreThanOneWoReport: ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TextArea", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public MoreThanOneWoReport() {
            Inputs.Add(WO);
            Inputs.Add(CloseFlag);
        }
        public override void Run()
        {
            if (WO.Value == null|| WO.Value.ToString() =="")
            {
                throw new Exception("WO Can not be null");
            }
            string wo = WO.Value.ToString();
            string closeflag = CloseFlag.Value.ToString().Trim();
            string[] arrWO = wo.Split('\n');

            string inWO = "''";
            foreach (var item in arrWO) {
                inWO = inWO + ",'" + item+"'";
            }
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                
                string Sqlwo;
                Sqlwo = $@"SELECT RWB.WORKORDERNO,CASE WHEN RWB.CLOSED_FLAG='1' THEN 'Y' ELSE 'N' END AS CLOSEFLAG,RSN.SN ,RSN.CURRENT_STATION ,RSN.NEXT_STATION, 
                               RWB.SKUNO,RPH.GROUPID,RWB.WORKORDER_QTY ,RWB.SKU_VER 
                                FROM R_WO_BASE RWB LEFT JOIN R_PRE_WO_HEAD  RPH ON RWB.WORKORDERNO=RPH.WO,R_SN RSN WHERE RSN.WORKORDERNO = RWB.WORKORDERNO 
                                 AND RWB.WORKORDERNO IN({inWO}) AND RSN.VALID_FLAG = 1  ";
                                 

                if (closeflag == "Y")
                {
                    Sqlwo = Sqlwo + " AND CLOSED_FLAG = 1 ";
                }
                else if (closeflag == "N")
                {
                    Sqlwo = Sqlwo + " AND CLOSED_FLAG = 0 ";
                }
                Sqlwo = Sqlwo + "ORDER BY RSN.WORKORDERNO ,SN";
                DataTable dts = SFCDB.RunSelect(Sqlwo).Tables[0];
                if (dts.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable resdt = new DataTable();
                DataRow linkDataRow = null;
                resdt.Columns.Add("WORKORDERNO");
                resdt.Columns.Add("CLOSEFLAG");
                resdt.Columns.Add("SN");
                resdt.Columns.Add("CURRENT_STATION");
                resdt.Columns.Add("NEXT_STATION");
                resdt.Columns.Add("SKUNO");
                resdt.Columns.Add("GROUPID");
                resdt.Columns.Add("WORKORDER_QTY");
                resdt.Columns.Add("SKU_VER");
                for (int i = 0;i<dts.Rows.Count;i++)
                {
                    linkDataRow = resdt.NewRow();
                    linkDataRow["workorderno"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + dts.Rows[i]["workorderno"].ToString() + "&EventName=";
                    linkDataRow["CLOSEFLAG"] = "";
                    linkDataRow["SN"] = "";
                    linkDataRow["CURRENT_STATION"] = "";
                    linkDataRow["NEXT_STATION"] = "";
                    linkDataRow["SKUNO"] = "";
                    linkDataRow["GROUPID"] = "";
                    linkDataRow["WORKORDER_QTY"] = "";
                    linkDataRow["SKU_VER"] = "";
                    resdt.Rows.Add(linkDataRow);

                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dts, resdt);
                retTab.Tittle = "MoreThanOne Wo Report";
                Outputs.Add(retTab);

            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                   WO.Value = "";
                   DBPools["SFCDB"].Return(SFCDB);
            }
        }
        }
}
