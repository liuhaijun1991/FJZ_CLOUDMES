using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using MESPubLab.MESStation.SNMaker;
using System.Threading;

namespace MESStation.Test
{
    public class APITest : MESPubLab.MESStation.MesAPIBase
    {
        protected APIInfo FSNTEST = new APIInfo()
        {
            FunctionName = "SNTEST",
            Description = "SN TEST",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN_RULE", InputType = "string", DefaultValue = "" },
                
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDBTEST = new APIInfo()
        {
            FunctionName = "DBTEST",
            Description = "數據庫測試",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "data1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "data2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "data3", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSNRULETEST = new APIInfo()
        {
            FunctionName = "SNRULETEST",
            Description = "SNRULETEST",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "RULE", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCallTest  = new APIInfo()
        {
            FunctionName = "CallTest",
            Description = "CallTest",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "RULE", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        public APITest()
        {
            this.Apis.Add(FDBTEST.FunctionName, FDBTEST);
            this.Apis.Add(FSNRULETEST.FunctionName, FSNRULETEST);
            this.Apis.Add(FSNTEST.FunctionName, FSNTEST);
            this.Apis.Add(FCallTest.FunctionName, FCallTest);
        }
        public void CallTest(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
           
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var a = sfcdb.ExecSelect("select * from dual");
                StationReturn.Data = a;
                Thread.Sleep(200);
            }
            catch (Exception ee)
            {
                StationReturn.Data = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void SNRULETEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string SN = Data["SN"].ToString();
            string RULE = Data["RULE"].ToString();
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                SNmaker.CkeckSNRule(SN, RULE, sfcdb);
                StationReturn.Data = "OK";
            }
            catch (Exception ee)
            {
                StationReturn.Data = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DBTEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            Language = "ENGLISH";
            MESReturnMessage.Language = Language;
            MESReturnMessage.GetMESReturnMessage("MES00000002");
            //throw new Exception("sdsdsdsdsedsdsds");
            string data1 =  Data["data1"].ToString();
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            string strSql = "select 1 from dual";
            
            System.Data.DataSet res = sfcdb.ExecSelect(strSql);
            StationReturn.Data = "OK";

            MESDataObject.Module.T_C_ROUTE T = new MESDataObject.Module.T_C_ROUTE(sfcdb, DB_TYPE_ENUM.Oracle);

            //string ID = T.GetNewID(BU, sfcdb);

            //StationReturn.Data = ID;

            MESDataObject.Module.T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new MESDataObject.Module.T_C_SAP_STATION_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
            string sap_station_code = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySku("A03024XNG-A",sfcdb);

            this.DBPools["SFCDB"].Return(sfcdb);
            //this.DBPools.Clear();
        }
        public void SNTEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var rule = Data["SN_RULE"].ToString();
                var sn = SNmaker.GetNextSN(rule, sfcdb.ORM);
                StationReturn.Data = sn;
            }
            catch(Exception ee)
            {
                StationReturn.Data = ee;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }



        }
    }
}
