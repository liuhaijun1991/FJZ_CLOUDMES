using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using System.Data;
using SqlSugar;
using MESDataObject.Module;
using MESDBHelper;

namespace MES_DCN.Schneider
{
    public class SchneiderAction
    {
        private string _customerdbstr = "SCHNEIDERVNLOCALDB";
        private string _seriesname = "Schneider";

        public SchneiderAction()
        {
        }

        public void SendPCBASNTOSchneiderDB(PCBA_Master sn)
        {
            string sql = "";
            //bool send = false;
            try
            {
                using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                {
                    sql = $@"select * from [dbo].[PCBA_Master] where PCBA_SerialNumber = '{sn.Sn}'";
                    if (customerdb.Ado.GetDataTable(sql).Rows.Count == 0)
                    {
                        sql = $@"insert into [dbo].[PCBA_Master](PCBA_SerialNumber,PCBA_PartNo,PCBA_Part_Description) values ('{sn.Sn}','{sn.Skuno}','{sn.SkunoDescription}')";
                        customerdb.Ado.ExecuteCommand(sql);
                    }
                }
                //send = true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendPCBASNTOSchneiderDB Error: " + ex.Message);
            }
            //return send;
        }

        public void SendModelWoToSchneiderDB(Model_WO wo)
        {
            string sql = "";
            //bool send = false;
            try
            {
                using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                {
                    sql = $@"select * from [dbo].[Plan] where workorderno = '{wo.Wo}'";
                    if (customerdb.Ado.GetDataTable(sql).Rows.Count == 0)
                    {
                        sql = $@"insert into [dbo].[Plan](skuno,skunodescription,workorderno,quantity) values ('{wo.Skuno}','{wo.SkunoDescription}','{wo.Wo}','{wo.Quantity}')";
                        customerdb.Ado.ExecuteCommand(sql);
                    }
                    else
                    {
                        sql = $@"select * from [dbo].[Plan] where workorderno = '{wo.Wo}' and quantity <> '{wo.Quantity}'";
                        if (customerdb.Ado.GetDataTable(sql).Rows.Count > 0)
                        {
                            sql = $@"update [dbo].[Plan] set quantity = '{wo.Quantity}' where workorderno = '{wo.Wo}'";
                            customerdb.Ado.ExecuteCommand(sql);
                        }
                    }
                }
                //send = true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendModelWoToSchneiderDB Error: " + ex.Message);
            }
            //return send;
        }

        public SI_SN GetModelSNFromSchneiderDB(string sn)
        {
            string sql = "";
            SI_SN ModelSN = new SI_SN()
            {
                result = false,
            };

            try
            {
                using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                {
                    sql = $@"select * from [dbo].[PCBA_Trace] where SerialNumber = '{sn}' order by id desc";
                    var dt = customerdb.Ado.GetDataTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        sql = $@"select * from [dbo].[PCBA_Trace] where PCBA = '{dt.Rows[0]["PCBA"].ToString().ToUpper().Trim()}' order by id desc";
                        if (customerdb.Ado.GetDataTable(sql).Rows.Count > 1)
                        {
                            throw new Exception($@"PCBA {dt.Rows[0]["PCBA"].ToString().ToUpper().Trim()} has multiple data in Schneider DB");
                        }
                        ModelSN.result = true;
                        ModelSN.Sn = dt.Rows[0]["SerialNumber"].ToString().ToUpper().Trim();
                        ModelSN.Workorderno = dt.Rows[0]["OrderNo"].ToString().ToUpper().Trim();
                        ModelSN.Status = "JOBFINISH";
                        ModelSN.Csn = dt.Rows[0]["PCBA"].ToString().ToUpper().Trim();
                    }
                    else if (dt.Rows.Count > 1)
                    {
                        throw new Exception($@"SN {sn} has multiple data in Schneider DB");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetModelSNFromSchneiderDB Error: " + ex.Message);
            }
            return ModelSN;
        }

        public void SendPCBASNTOSE(OleExec db, string sn)
        {
            var snList = db.ORM.Queryable<R_SN, R_WO_BASE, C_SKU, C_SERIES>((a, b, c, d) => a.WORKORDERNO == b.WORKORDERNO && b.SKUNO == c.SKUNO && b.SKU_VER == c.VERSION && c.C_SERIES_ID == d.ID)
                .Where((a, b, c, d) => a.NEXT_STATION == "JOBFINISH" && a.SN == sn && d.DESCRIPTION == "Schneider" && c.SKU_TYPE == "PCBA" && a.VALID_FLAG == "1")
                .Select((a, b, c, d) => new { a.SN, a.SKUNO, c.DESCRIPTION }).ToList().FirstOrDefault();
            if (snList != null)
            {
                SchneiderAction.PCBA_Master pcba_sn = new SchneiderAction.PCBA_Master()
                {
                    Sn = snList.SN,
                    Skuno = snList.SKUNO,
                    SkunoDescription = snList.DESCRIPTION
                };
                SchneiderAction Schneider = new SchneiderAction();
                try
                {
                    Schneider.SendPCBASNTOSchneiderDB(pcba_sn);
                }
                catch (Exception ex)
                {
                    throw new Exception("VNDCNPCBADataSendToSchneiderDB Error: " + ex.Message);
                }
            }
        }
        public DataTable GetPCBATrace(string sn, string pcba, string wo)
        {
            DataTable dt = new DataTable();
            using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
            {
                string sql = $@"select * from [dbo].[PCBA_Trace] where 1=1";
                if (!string.IsNullOrEmpty(sn))
                {
                    sql = sql + $@" and SerialNumber='{sn}'";
                }
                if (!string.IsNullOrEmpty(pcba))
                {
                    sql = sql + $@" and PCBA='{pcba}'";
                }
                if (!string.IsNullOrEmpty(wo))
                {
                    sql = sql + $@" and OrderNo='{wo}'";
                }
                dt = customerdb.Ado.GetDataTable(sql);
            }
            return dt;
        }

        public DataTable GetPCBAMaster(string sn, string partno)
        {
            DataTable dt = new DataTable();
            using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
            {
                string sql = $@"select * from [dbo].[PCBA_Master] where 1=1 ";
                if (!string.IsNullOrEmpty(sn))
                {
                    sql = sql + $@" and PCBA_SerialNumber='{sn}'";
                }
                if (!string.IsNullOrEmpty(partno))
                {
                    sql = sql + $@" and PCBA_PartNo='{partno}'";
                }
                dt = customerdb.Ado.GetDataTable(sql);
            }
            return dt;
        }
        public DataTable GetPlanTable(string skuno, string wo)
        {
            DataTable dt = new DataTable();
            using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
            {
                string sql = $@"select * from [dbo].[Plan] where 1=1 ";
                if (!string.IsNullOrEmpty(skuno))
                {
                    sql = sql + $@" and SKUNO='{skuno}'";
                }
                if (!string.IsNullOrEmpty(wo))
                {
                    sql = sql + $@" and WORKORDERNO='{wo}'";
                }
                dt = customerdb.Ado.GetDataTable(sql);
            }
            return dt;
        }
        public class PCBA_Master
        {
            public string Sn;
            public string Skuno;
            public string SkunoDescription;
        }

        public class Model_WO
        {
            public string Wo;
            public string Skuno;
            public string SkunoDescription;
            public string Quantity;
        }
        
        public class SI_SN
        {
            public bool result;
            public string Sn;
            public string Workorderno;
            public string Status;
            public string Csn;
        }

    }
}
