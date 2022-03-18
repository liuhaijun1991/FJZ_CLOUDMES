using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    //維修報表

    public class RepairOffReport : ReportBase
    {

        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018/02/12 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput WO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        

        public RepairOffReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(SN);
            Inputs.Add(SkuNo);
            Inputs.Add(WO);
            
        }

        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                SFCDB = DBPools["SFCDB"].Borrow();
                InitSkuno(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string runSql = $@" SELECT a.sn, b.SKUNO ,b.WORKORDERNO,a.LOGTYPE, a.DATA5 FaliCode ,b.CURRENT_STATION,b.NEXT_STATION,a.CREATETIME,a.CREATEBY FROM SFCRUNTIME.R_SN_LOG a, r_sn b WHERE a.LOGTYPE='Repair_offline'AND  a.SNID =b.id 
                    AND b.VALID_FLAG=1
                   and a.CREATETIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                   AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ";
                if (SN.Value.ToString() != "ALL" && SN.Value.ToString() != string.Empty)
                {
                    runSql += $@" and b.SN = '{ SN.Value.ToString()}'";
                }
                if (SkuNo.Value.ToString() != "ALL" && SkuNo.Value.ToString() != string.Empty)
                {
                    runSql += $@" and b.skuno = '{SkuNo.Value.ToString()}'";
                }
                
                if (WO.Value.ToString() != "ALL" && WO.Value.ToString() != string.Empty)
                {
                    runSql += $@" and  b.workorderno = '{ WO.Value.ToString()}'";
                }
               
                RunSqls.Add(runSql);
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Repair OffLine Report";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }

        }

        public void InitSkuno(OleExec db)
        {
            List<string> skuno = new List<string>();
            //DataTable dt = new DataTable();
            //T_C_SKU sku = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            //dt = sku.GetALLSkuno(db);

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string strSql = $@"SELECT SKUNO FROM SFCBASE.C_SKU WHERE SKU_NAME IN (
                                  SELECT SKUNO FROM SFCBASE.C_SERIES WHERE CUSTOMER_ID IN (
                                   SELECT ID FROM SFCBASE.C_CUSTOMER WHERE CUSTOMER_NAME='NETGEAR')) order by skuno";
            try
            {

                DataTable dt = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                skuno.Add("ALL");
                foreach (DataRow dr in dt.Rows)
                {
                    skuno.Add(dr["SKUNO"].ToString());

                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                SkuNo.ValueForUse = skuno;
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
    }
}
