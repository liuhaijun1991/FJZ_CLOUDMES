using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESStation.Management
{
    public class SQLSearch : MesAPIBase
    {
        protected APIInfo FRunSQL = new APIInfo
        {
            FunctionName = "RunSQL",
            Description = "Run SQL",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SQL" } },
            Permissions = new List<MESPermission>()
        };
        public SQLSearch()
        {
            this.Apis.Add(FRunSQL.FunctionName, FRunSQL);
        }
        public void RunSQL(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.ThrowSqlExeception = true;
                string sql = Data["SQL"].ToString();
                if (sql.ToUpper().Contains("UPDATE") || sql.ToUpper().Contains("DELETE") || sql.ToUpper().Contains("INSERT"))
                {
                    throw new Exception("Can Not Input Update/Delete/Insert Sql !");
                }
                DataTable resDt = SFCDB.RunSelect(sql).Tables[0];
                if (resDt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                for (int i = 0; i < resDt.Columns.Count; i++)
                {
                    if (resDt.Columns[i].ToString().Equals("EMP_PASSWORD"))
                    {
                        for (int j = 0; j < resDt.Rows.Count; j++)
                        {
                            resDt.Rows[j][i] = "******";
                        }
                    }
                }
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Data = resDt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
