using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Test
{
    public class SkuTest: MESPubLab.MESStation.MesAPIBase
    {
        //private static DB_TYPE_ENUM DB_TYPE = DB_TYPE_ENUM.Oracle;
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        //private APIInfo AllSKU = new APIInfo()
        //{
        //    FunctionName="GetAllSku",
        //    Description="獲取所有機種",
        //    Parameters=new List<APIInputInfo>()
        //    { },
        //    Permissions=new List<MESPermission>()
        //    { }
        //};

        //private APIInfo SkuByName = new APIInfo()
        //{
        //    FunctionName = "GetSkuByName",
        //    Description = "根據機種名獲取機種",
        //    Parameters = new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() { InputName="Sku_Name",InputType="string",DefaultValue=""}
        //    },
        //    Permissions = new List<MESPermission>()
        //    { }
            
        //};

        //private APIInfo _UpdateSku = new APIInfo()
        //{
        //    FunctionName="UpdateSku",
        //    Description="修改機種信息",
        //    Parameters=new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
        //    },
        //    Permissions=new List<MESPermission>()
        //    { }
        //};

        //private APIInfo _AddSku = new APIInfo()
        //{
        //    FunctionName = "AddSku",
        //    Description = "添加機種信息",
        //    Parameters = new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
        //    },
        //    Permissions = new List<MESPermission>()
        //    { }
        //};

        //private APIInfo _DeleteSku = new APIInfo()
        //{
        //    FunctionName = "DeleteSku",
        //    Description = "刪除機種信息",
        //    Parameters = new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
        //    },
        //    Permissions = new List<MESPermission>()
        //    { }
        //};

        //private APIInfo _DeleteSkuById = new APIInfo()
        //{
        //    FunctionName = "DeleteSkuById",
        //    Description = "刪除機種信息",
        //    Parameters = new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() { InputName="SkuId",InputType="string",DefaultValue=""}
        //    },
        //    Permissions = new List<MESPermission>()
        //    { }
        //};



        //public SkuTest()
        //{
        //    this.Apis.Add(AllSKU.FunctionName, AllSKU);
        //    this.Apis.Add(SkuByName.FunctionName, SkuByName);
        //    //this.Apis.Add(ChangeSku.FunctionName, ChangeSku);
        //    this.Apis.Add(_AddSku.FunctionName, _AddSku);
        //    this.Apis.Add(_DeleteSku.FunctionName, _DeleteSku);
        //    this.Apis.Add(_DeleteSkuById.FunctionName, _DeleteSkuById);
        //    this.Apis.Add(_UpdateSku.FunctionName, _UpdateSku);
        //}

        //public void GetAllSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    List<C_SKU> SkuList = new List<C_SKU>();
        //    T_C_SKU table = null;

        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        //session = new SessionManager<C_SKU>(sfcdb);
        //        //SkuList = session.Select(null, null);
        //        //table = new T_C_SKU(sfcdb, DB_TYPE);
        //        //SkuList = table.GetAllSku(sfcdb);

        //        ConstructReturns(ref StationReturn, "Success", "獲取成功", SkuList);
        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        //public void GetSkuByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    List<C_SKU> SkuList = new List<C_SKU>();
        //    T_C_SKU table = null;
        //    string SkuName = string.Empty;

        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        session = new SessionManager<C_SKU>(sfcdb);
        //        Dictionary<string, object> conditions = new Dictionary<string, object>();
                

        //        //table = new T_C_SKU(sfcdb, DB_TYPE);
        //        SkuName = Data["Sku_Name"].ToString();
        //        //SkuList = table.GetSkuByName(SkuName, sfcdb);
        //        conditions.Add("SKUNO", SkuName);
        //        SkuList = session.Select(conditions, null);
        //        ConstructReturns(ref StationReturn, "Success", "獲取成功", SkuList);
                
        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }

        //}

        //public void AddSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    string SkuStr = string.Empty;
        //    string jsonTest = @"
        //    {
        //    'BU': 'CPE',
        //    'SKUNO': 'U81B049.00222',
        //    'VERSION': '20',
        //    'CODE_NAME': 'U81B049.00',
        //    'C_SERIES_ID': '11',
        //    'CUST_PARTNO': 'NOKIA',
        //    'CUST_SKU_CODE': 'U81B049.00',
        //    'SN_RULE': 'EB****6****',
        //    'LAST_EDIT_USER': 'A0225204',
        //    'LAST_EDIT_TIME': '2017/12/01 09:08:00'
        //    }";

        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        session = new SessionManager<C_SKU>(sfcdb);
        //        //SkuStr = Data["SkuObject"].ToString(); //實際 json 串
        //        SkuStr = jsonTest; //臨時測試
        //        C_SKU c_sku = (C_SKU)JsonConvert.Deserialize(SkuStr, typeof(C_SKU));
        //        string result = session.Save(c_sku);

        //        if (result.Contains("SQL"))
        //        {
        //            ConstructReturns(ref StationReturn, "Fail", result, result);
        //        }
        //        else
        //        {
        //            ConstructReturns(
        //                ref StationReturn,
        //                "Success",
        //                string.Format("成功更新 {0} 行數據！", result),
        //                string.Format("成功更新 {0} 行數據！", result)
        //                );
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        //public void DeleteSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    string SkuStr = string.Empty;
        //    string jsonTest = @"
        //    {
        //    'ID':'C_SKU_000000000000000000000000000006',
        //    'BU': 'CPE',
        //    'SKUNO': 'U81B049.001111',
        //    'VERSION': '25',
        //    'CODE_NAME': 'U81B049.00111',
        //    'C_SERIES_ID': '11',
        //    'CUST_PARTNO': 'NOKIA',
        //    'CUST_SKU_CODE': 'U81B049.00',
        //    'SN_RULE': 'EB****6****',
        //    'LAST_EDIT_USER': 'A0225204',
        //    'LAST_EDIT_TIME': '2017/11/27 15:49:00'
        //    }";

        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        session = new SessionManager<C_SKU>(sfcdb);
        //        //SkuStr = Data["SkuObject"].ToString(); //實際 json 串
        //        SkuStr = jsonTest; //臨時測試
        //        C_SKU c_sku = (C_SKU)JsonConvert.Deserialize(SkuStr, typeof(C_SKU));
        //        string result = session.Remove(c_sku);

        //        if (result.Contains("SQL"))
        //        {
        //            ConstructReturns(ref StationReturn, "Fail", result, result);
        //        }
        //        else
        //        {
        //            ConstructReturns(
        //                ref StationReturn,
        //                "Success",
        //                string.Format("成功更新 {0} 行數據！", result),
        //                string.Format("成功更新 {0} 行數據！", result)
        //                );
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        //public void DeleteSkuById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    string SkuId = string.Empty;
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        session = new SessionManager<C_SKU>(sfcdb);
        //        SkuId = Data["SkuId"].ToString();
        //        string result = session.Remove(SkuId);

        //        if (result.Contains("SQL"))
        //        {
        //            ConstructReturns(ref StationReturn, "Fail", result, result);
        //        }
        //        else
        //        {
        //            ConstructReturns(
        //                ref StationReturn,
        //                "Success",
        //                string.Format("成功更新 {0} 行數據！", result),
        //                string.Format("成功更新 {0} 行數據！", result)
        //                );
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        //public void UpdateSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    SessionManager<C_SKU> session = null;
        //    OleExec sfcdb = null;
        //    string SkuStr = string.Empty;
        //    string jsonTest = @"
        //    {
        //    'ID': 'C_SKU_000000000000000000000000000008',
        //    'BU': 'CPE',
        //    'SKUNO': 'U81B049.00222',
        //    'VERSION': '25',
        //    'CODE_NAME': 'U81B049.00',
        //    'C_SERIES_ID': '11',
        //    'CUST_PARTNO': 'NOKIA',
        //    'CUST_SKU_CODE': 'U81B049.00',
        //    'SN_RULE': 'EB****6****',
        //    'LAST_EDIT_USER': 'A0225204',
        //    'LAST_EDIT_TIME': '2017/12/01 09:17:00'
        //    }";

        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        session = new SessionManager<C_SKU>(sfcdb);
        //        //SkuStr = Data["SkuObject"].ToString(); //實際 json 串
        //        SkuStr = jsonTest; //臨時測試
        //        C_SKU c_sku = (C_SKU)JsonConvert.Deserialize(SkuStr, typeof(C_SKU));
        //        //table = new T_C_SKU(sfcdb, DB_TYPE);
        //        //Row_C_SKU row = (Row_C_SKU)table.NewRow();
        //        //c_sku.ApplyData(ref row);
        //        //string result = table.UpdateSKU(row, operation, sfcdb);
        //        string result = session.Update(c_sku);

        //        if (result.Contains("SQL"))
        //        {
        //            ConstructReturns(ref StationReturn, "Fail", result, result);
        //        }
        //        else
        //        {
        //            ConstructReturns(
        //                ref StationReturn, 
        //                "Success", 
        //                string.Format("成功更新 {0} 行數據！", result),
        //                string.Format("成功更新 {0} 行數據！", result)
        //                );
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        ///// <summary>
        ///// 構建返回到前端的結果對象
        ///// </summary>
        ///// <param name="StationReturn">最終返回到前端的結果對象</param>
        ///// <param name="Status">狀態</param>
        ///// <param name="Message">信息</param>
        ///// <param name="Data">數據</param>
        //public void ConstructReturns(ref MESStationReturn StationReturn, string Status, string Message, object Data)
        //{
        //    StationReturn.Status = Status;
        //    StationReturn.Message = Message;
        //    StationReturn.Data = Data;
        //}

    }

}
