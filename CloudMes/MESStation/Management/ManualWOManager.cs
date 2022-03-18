using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESDataObject.Module.ORACLE;

namespace MESStation.Management
{
    public class ManualWOManager : MesAPIBase
    {
        private APIInfo AllCWO = new APIInfo()
        {
            FunctionName = "GetAllCWO",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo AllWO = new APIInfo()
        {
            FunctionName = "GetAllWO",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        protected APIInfo FQueryWO = new APIInfo()
        {
            FunctionName = "QueryWO",
            Description = "Query WO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateWO = new APIInfo()
        {
            FunctionName = "UpdateWO",
            Description = "Update WO",
            Parameters = new List<APIInputInfo>()
            {

                new APIInputInfo() {InputName = "WOIDS", InputType = "string", DefaultValue = "" },


            },
            Permissions = new List<MESPermission>() { }
        };
        public ManualWOManager()
        {
            this.Apis.Add(AllCWO.FunctionName, AllCWO);
            this.Apis.Add(AllWO.FunctionName, AllWO);
            this.Apis.Add(FUpdateWO.FunctionName, FUpdateWO);
            this.Apis.Add(FQueryWO.FunctionName, FQueryWO);
        }
        public void QueryWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Wo = Data["WO"].ToString().Trim();
            OleExec oleDB = null;
            T_R_MFPRESETWOHEAD cWO = null;
            List<R_MFPRESETWOHEAD> cWOList = new List<R_MFPRESETWOHEAD>();
        
            try
            {
                
                    oleDB = this.DBPools["SFCDB"].Borrow();
                    cWO = new T_R_MFPRESETWOHEAD(oleDB, DBTYPE);
                    cWOList = cWO.GetWO(oleDB, Wo);

                    if (cWOList.Count > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(cWOList.Count);
                        StationReturn.Data = cWOList;
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = "";
                    }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
      
        public void UpdateWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string WOIDS = Data["WOIDS"].ToString().Trim();
            ArrayList RelIds = new ArrayList(WOIDS.Split(','));
            OleExec oleDB = null;
            T_R_MFPRESETWOHEAD cWOflag = null;

            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cWOflag = new T_R_MFPRESETWOHEAD(oleDB, DBTYPE);

                foreach (var Wo in (string[])RelIds.ToArray(typeof(string)))
                {
                    if (Wo != "")
                    {

                        string strSql = $@"update R_MFPRESETWOHEAD set SAPFLAG='4',EDIT_TIME=sysdate,EDIT_EMP='{LoginUser.EMP_NO}' where WO='{Wo}'";
                        oleDB.ExecSQL(strSql);

                    }

                }


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
            }
            catch (Exception e)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000025";
                StationReturn.Data = "";
                throw e;
            }
            finally
            {

                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetAllCWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {


            OleExec sfcdb = null;
            List<R_MFPRESETWOHEAD> WOList = new List<R_MFPRESETWOHEAD>();
            //List<R_MFPRESETWOHEAD> WOCList = new List<R_MFPRESETWOHEAD>();
            T_R_MFPRESETWOHEAD Table = null;
           

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_R_MFPRESETWOHEAD(sfcdb, DBTYPE);


                WOList = Table.GetAllCWOList(sfcdb);
                if (WOList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(WOList.Count().ToString());
                    StationReturn.Data = WOList;
                }


            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }


            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

    }
}



