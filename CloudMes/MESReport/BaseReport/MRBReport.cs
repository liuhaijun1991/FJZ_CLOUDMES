using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using MESDBHelper;
using MESDataObject;





namespace MESReport.BaseReport
{
   public class MRBReport : ReportBase
    {
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput OldWO = new ReportInput() { Name = "OldWO", InputType = "TXT", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse =null};
        ReportInput NewWO = new ReportInput() { Name = "NewWO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuInput = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput FromStorage = new ReportInput() { Name = "FromStorage", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "", "B23", "B24", "B25", "B26" } };
        ReportInput ToStorage = new ReportInput() { Name = "ToStorage", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "", "B27", "B28", "B29", "B30" } };
        ReportInput isReworked=new ReportInput { Name = "Reworked", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "YES", "NO"} };
        public MRBReport()
        {
            Inputs.Add(SN);
            Inputs.Add(OldWO);
            Inputs.Add(NewWO);           
            Inputs.Add(FromStorage);
            Inputs.Add(ToStorage);
            Inputs.Add(skuInput);
            Inputs.Add(isReworked);
            //string strGetWoSN = @"select * from r_sn where workorderno = '{0}' and rownum < 30 ";
            //Sqls.Add("strGetWoSN", strGetWoSN);
        }
        public override void Init()
        {
            string strSql = "select distinct(storage_code) from c_storage_code ";
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable table = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                string[] StorageAssy;
                if (table.Rows.Count > 0)
                {
                    StorageAssy = new string[table.Rows.Count + 1];
                    StorageAssy[0] = "";
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        StorageAssy[i + 1] = (table.Rows[i][0] == null) ? "" : table.Rows[i][0].ToString();
                    }
                }
                else
                {
                    //throw new Exception("獲取倉碼失敗或者沒有數據!");

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816145954"));
                }
                FromStorage.ValueForUse = StorageAssy;
                ToStorage.ValueForUse = StorageAssy;
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                Outputs.Add(new ReportAlart(ex.Message));                
                //throw ex;
            }
        }
        public override void Run()
        {           
            string runSql = "select * from r_mrb ";
            string strSn = (SN.Value==null)?"": SN.Value.ToString().Trim();
            string strOldWO= (OldWO.Value == null) ? "" : OldWO.Value.ToString().Trim();
            string strNewWO = (NewWO.Value == null) ? "" : NewWO.Value.ToString().Trim();
            string strFromStorage = (FromStorage.Value == null) ? "" : FromStorage.Value.ToString().Trim();
            string strToStorage = (ToStorage.Value == null) ? "" : ToStorage.Value.ToString().Trim();
            string strSkuno = (skuInput.Value == null) ? "" : skuInput.Value.ToString().Trim();
            string strIsRework = (isReworked.Value == null) ? "" : isReworked.Value.ToString().Trim();
            bool isContainWhere = false;
            OleDbParameter[] paramet = null;
            List<OleDbParameter> parametList = new List<OleDbParameter>();
            if (strSn.Length > 0)
            {
                runSql = runSql + $@" where sn=:sn ";
                OleDbParameter SNParamet = new OleDbParameter(":sn", strSn);
                parametList.Add(SNParamet);
                isContainWhere = true;
            }
            if (strOldWO.Length > 0)
            {
                if (!isContainWhere)
                {
                    runSql = runSql + $@" where workorderno=:wono ";
                    isContainWhere = true;
                }
                else
                {
                    runSql = runSql + $@" and workorderno=:wono ";
                }
                OleDbParameter WOParamet = new OleDbParameter(":wono", strOldWO);
                parametList.Add(WOParamet);
            }
            if (strNewWO.Length > 0)
            {
                if (!isContainWhere)
                {
                    runSql = runSql + $@" where rework_wo=:rewono ";
                    isContainWhere = true;
                }
                else
                {
                    runSql = runSql + $@" and rework_wo=:rewono ";
                }
                OleDbParameter REWOParamet = new OleDbParameter(":rewono", strNewWO);
                parametList.Add(REWOParamet);
            }
            if (strFromStorage.Length > 0)
            {
                if (!isContainWhere)
                {
                    runSql = runSql + $@" where from_storage=:FromStorage ";
                    isContainWhere = true;
                }
                else
                {
                    runSql = runSql + $@" and from_storage=:FromStorage ";
                }
                OleDbParameter FromStorageParamet = new OleDbParameter(":FromStorage", strFromStorage);
                parametList.Add(FromStorageParamet);
            }
            if (strToStorage.Length > 0)
            {
                if (!isContainWhere)
                {
                    runSql = runSql + $@" where to_storage=:ToStorage ";
                    isContainWhere = true;
                }
                else
                {
                    runSql = runSql + $@" and to_storage=:ToStorage ";
                }
                OleDbParameter ToStorageParamet = new OleDbParameter(":ToStorage", strToStorage);
                parametList.Add(ToStorageParamet);
            }
            if (strSkuno.Length > 0)
            {
                if (strSkuno.Trim().ToUpper() != "ALL")
                {
                    if (!isContainWhere)
                    {
                        runSql = runSql + $@" where skuno=:skuno ";
                        isContainWhere = true;
                    }
                    else
                    {
                        runSql = runSql + $@" and skuno=:skuno ";
                    }
                }
                OleDbParameter skunoParamet = new OleDbParameter(":skuno", strSkuno);
                parametList.Add(skunoParamet);
            }
            if (strIsRework.Length > 0)
            {
                string sqlIsRework = "";
                switch (strIsRework.ToUpper())
                {
                    case "YES":
                        sqlIsRework = $@" rework_wo is not null ";
                        break;
                    case "NO":
                        sqlIsRework = $@" rework_wo is null ";
                        break;
                    case "ALL":
                    default:
                        sqlIsRework = "";
                        break;
                }
                if (sqlIsRework.Length > 0)
                {
                    if (!isContainWhere)
                    {
                        runSql = runSql + $@" where {sqlIsRework} ";
                        isContainWhere = true;
                    }
                    else
                    {
                        runSql = runSql + $@" and {sqlIsRework} ";
                    }
                }
            }
            if (parametList.Count<=0)
            {
                //ReportAlart alart = new ReportAlart("請輸入查詢條件!");

                ReportAlart alart = new ReportAlart("Please enter query criteria!");
                Outputs.Add(alart);
                return;
            }
            else
            {
                paramet = new OleDbParameter[parametList.Count];
                for (int i = 0; i < parametList.Count; i++)
                {
                    paramet[i] = parametList[i];
                }
            }
            RunSqls.Add(runSql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable res = SFCDB.ExecuteDataTable(runSql,CommandType.Text, paramet);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "MRB List";
                retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
                return;
            }
        }
    }
}
