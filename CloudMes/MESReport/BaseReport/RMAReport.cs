using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using MESDBHelper;


namespace MESReport.BaseReport
{
    /// <summary>
    /// 注意！！！！！！！！！！
    /// 這個類還沒實現具體功能，只是簡單的查詢SN
    /// </summary>
   public class RMAReport : ReportBase
    {
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public RMAReport()
        {
            Inputs.Add(SN);

        }
        public override void Init()
        {
            //待定          
        }
        public override void Run()
        {
            if (SN.Value == null || SN.Value.ToString().Trim().Length <= 0)
            {
                throw new Exception("SN Can not be null");
            }
            // string runSql = string.Format(Sqls["strGetWoSN"], WO.Value.ToString());
            string runSql = "select * from r_sn where sn=:snno ";
            string strSn = SN.Value.ToString().Trim();
            RunSqls.Add(runSql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                OleDbParameter[] paramet = new OleDbParameter[]{
                    new OleDbParameter(":snno",strSn)
                };
                DataTable res = SFCDB.ExecuteDataTable(runSql, CommandType.Text, paramet);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "SN RMA Message";
                retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }
    }
}
