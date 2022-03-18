using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using System.Data;
using SAP.Middleware.Connector;
using MESPubLab.SAP_RFC;
using MESDataObject;

namespace MESStation.Config.HWD
{
    public class C7B5API : MesAPIBase
    {
        protected APIInfo FGet7B5LinkList = new APIInfo()
        {
            FunctionName = "Get7B5LinkList",
            Description = "Get Skuno Link 7B5 List",
            Parameters = new List<APIInputInfo>() {     
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDelete7B5Link = new APIInfo()
        {
            FunctionName = "Delete7B5Link",
            Description = "Delete Skuno 7B5 Link By ID",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "ArrayID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditSkunoLink7B5 = new APIInfo()
        {
            FunctionName = "EditSkunoLink7B5",
            Description = "Add Or Modify A Recording",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "ObjectModel", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5ECList = new APIInfo
        {
            FunctionName = "Get7B5ECList",
            Description = "Get 7B5 EC List",
            Parameters = new List<APIInputInfo>(){ },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5ECStatusList = new APIInfo()
        {
            FunctionName = "Get7B5ECStatusList",
            Description = "獲取R_7B5_EC界面中下拉框的值",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDelete7B5EC = new APIInfo()
        {
            FunctionName = "Delete7B5EC",
            Description = "Delete7B5EC",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEdit7B5EC = new APIInfo()
        {
            FunctionName = "Edit7B5EC",
            Description = "Edit7B5EC",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ObjectModel", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5PlanUploadDate = new APIInfo()
        {
            FunctionName = "Get7B5PlanUploadDate",
            Description = "上傳R 7B5 PLAN頁面獲取上傳的系統時間及修改上傳時間權限",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpload7B5PlanExcel = new APIInfo()
        {
            FunctionName = "Upload7B5PlanExcel",
            Description = "通過Excel上傳R 7B5 PLAN",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5PlanList = new APIInfo()
        {
            FunctionName = "Get7B5PlanList",
            Description = "獲取7B5 Plan 記錄",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5ShipModel = new APIInfo()
        {
            FunctionName = "Get7B5ShipModel",
            Description = "獲取SHIP_PLAN頁面的model及系統時間",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5ShipList = new APIInfo()
        {
            FunctionName = "Get7B5ShipList",
            Description = "SHIP_PLAN頁面獲取數據",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "Data", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyShipPlan = new APIInfo()
        {
            FunctionName = "ModifyShipPlan",
            Description = "SHIP PLAN頁面修改數據數據",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "Data", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5ShipDataList = new APIInfo()
        {
            FunctionName = "Get7B5ShipDataList",
            Description = "R_7B5_SHIP_DATA(SHIP_PLAN_DATA)頁面獲取數據",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "Data", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteShipPlanData = new APIInfo()
        {
            FunctionName = "DeleteShipPlanData",
            Description = "R_7B5_SHIP_DATA(SHIP_PLAN_DATA)頁面刪除數據",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "LOTNO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5WOTIME = new APIInfo()
        {
            FunctionName = "Get7B5WOTIME",
            Description = "R_7B5_WO頁面加載時執行動作",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5WOList = new APIInfo()
        {
            FunctionName = "Get7B5WOList",
            Description = " R_7B5_WO頁面獲取數據",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteR7B5WO = new APIInfo()
        {
            FunctionName = "DeleteR7B5WO",
            Description = "R_7B5_WO頁面刪除數據",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "SAP_WO", InputType = "string", DefaultValue = "" }              
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetWOTempSeleteValue = new APIInfo()
        {
            FunctionName = "GetWOTempSeleteValue",
            Description = "R_7B5_WO_TEMP頁面加載獲取信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5WOTempList = new APIInfo()
        {
            FunctionName = "Get7B5WOTempList",
            Description = "R_7B5_WO_TEMP頁面搜索事件",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() {InputName = "Data", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FR7B5WOTEMPageLoadAction = new APIInfo()
        {
            FunctionName = "R7B5WOTEMPageLoadAction",
            Description = "R_7B5_WO_TEMP頁面頁面加載執行事件",
            Parameters = new List<APIInputInfo>() {},
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FR7B5WOTempDelete = new APIInfo()
        {
            FunctionName = "R7B5WOTempDelete",
            Description = "R7B5WOTempDelete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "HH_ITEM", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };        
        protected APIInfo FR7B5WOTempEdit = new APIInfo()
        {
            FunctionName = "R7B5WOTempEdit",
            Description = "R7B5WOTempEdit",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RECEIVE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_NO_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_NO_USE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SAP_FACTORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "MODEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "V_TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "HH_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "HW_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "START_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "COMPLETE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PLAN_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PO_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "MAIN_WO_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "ZNP195_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "WO_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "CREAT_WO_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SUGGEST_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "CANCEL_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_NO_LEVEL", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetR7B5XMLTSeleteValue = new APIInfo()
        {
            FunctionName = "GetR7B5XMLTSeleteValue",
            Description = " GetR7B5XMLTSeleteValue",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5XMLTList = new APIInfo()
        {
            FunctionName = "Get7B5XMLTList",
            Description = "Get7B5XMLTList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "HW_PN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "MODEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PLAN_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "CANCEL_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PO_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DATE_FROM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DATE_TO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FR7B5XMLTSplitQty = new APIInfo()
        {
            FunctionName = "R7B5XMLTSplitQty",
            Description = "R7B5XMLTSplitQty",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SPLIT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FR7B5XMLTUpdate = new APIInfo()
        {
            FunctionName = "R7B5XMLTUpdate",
            Description = "R7B5XMLTUpdate",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PLAN_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PO_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "CANCEL_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "REMARK", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_CHANGE_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_CHANGE_CONFIRM", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpload7B5XMLTExcel = new APIInfo()
        {
            FunctionName = "Upload7B5XMLTExcel",
            Description = "Upload7B5XMLTExcel",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FRTaskOverdueLoadingAction = new APIInfo()
        {
            FunctionName = "RTaskOverdueLoadingAction",
            Description = "RTaskOverdueLoadingAction",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>() { }
        };   
        protected APIInfo FRTaskOverdueGetDate = new APIInfo()
        {
            FunctionName = "RTaskOverdueGetDate",
            Description = "RTaskOverdueGetDate",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FRTaskOverdueSearchAction = new APIInfo()
        {
            FunctionName = "RTaskOverdueSearchAction",
            Description = "RTaskOverdueSearchAction",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "TASK_NO_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DATE_FROM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DATE_TO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSKU7B5ConfigType = new APIInfo()
        {
            FunctionName = "GetSKU7B5ConfigType",
            Description = "GetSKU7B5ConfigType",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSKU7B5ConfigList = new APIInfo()
        {
            FunctionName = "GetSKU7B5ConfigList",
            Description = "GetSKU7B5ConfigList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSKU7B5ConfigAdd = new APIInfo()
        {
            FunctionName = "SKU7B5ConfigAdd",
            Description = "SKU7B5ConfigAdd",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "UPD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "VIR_FLAG", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSKU7B5ConfigEdit = new APIInfo()
        {
            FunctionName = "SKU7B5ConfigEdit",
            Description = "SKU7B5ConfigEdit",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "OLD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "NEW", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSKU7B5ConfigDelete = new APIInfo()
        {
            FunctionName = "SKU7B5ConfigDelete",
            Description = "SKU7B5ConfigDelete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "UPD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "VIR_FLAG", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet7B5POList = new APIInfo()
        {
            FunctionName = "Get7B5POList",
            Description = "Get7B5POList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TASK_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpload7B5POExcel = new APIInfo()
        {
            FunctionName = "Upload7B5POExcel",
            Description = "Upload7B5POExcel",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetCutWoList = new APIInfo()
        {
            FunctionName = "GetCutWoList",
            Description = " HWD Cut Workorder頁面加載要Cut 的工單",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetWODataInfo = new APIInfo()
        {
            FunctionName = "GetWODataInfo",
            Description = "HWD Cut Workorder頁面選擇工單獲取工單信息",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCutWoCheckWoData = new APIInfo()
        {
            FunctionName = "CutWoCheckWoData",
            Description = "HWD Cut Workorder頁面GheckData按鈕事件",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCutWo = new APIInfo()
        {
            FunctionName = "CutWo",
            Description = "  HWD Cut Workorder頁面Submit按鈕事件",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "Version", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "StartStation", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyShipExtQty = new APIInfo()
        {
            FunctionName = "ModifyShipExtQty",
            Description = "ModifyShipExtQty",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "TASK_NO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "HH_ITEM", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "HW_ITEM", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "EXT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        public C7B5API()
        {       
            this.Apis.Add(FGet7B5LinkList.FunctionName, FGet7B5LinkList);
            this.Apis.Add(FDelete7B5Link.FunctionName, FDelete7B5Link);
            this.Apis.Add(FEditSkunoLink7B5.FunctionName, FEditSkunoLink7B5);
            this.Apis.Add(FGet7B5ECList.FunctionName, FGet7B5ECList);
            this.Apis.Add(FGet7B5ECStatusList.FunctionName, FGet7B5ECStatusList);            
            this.Apis.Add(FDelete7B5EC.FunctionName, FDelete7B5EC);
            this.Apis.Add(FEdit7B5EC.FunctionName, FEdit7B5EC);
            this.Apis.Add(FGet7B5PlanUploadDate.FunctionName, FGet7B5PlanUploadDate);
            this.Apis.Add(FUpload7B5PlanExcel.FunctionName, FUpload7B5PlanExcel);
            this.Apis.Add(FGet7B5PlanList.FunctionName, FGet7B5PlanList);
            this.Apis.Add(FGet7B5ShipModel.FunctionName, FGet7B5ShipModel);
            this.Apis.Add(FGet7B5ShipList.FunctionName, FGet7B5ShipList);
            this.Apis.Add(FModifyShipPlan.FunctionName, FModifyShipPlan);
            this.Apis.Add(FGet7B5ShipDataList.FunctionName, FGet7B5ShipDataList);
            this.Apis.Add(FDeleteShipPlanData.FunctionName, FDeleteShipPlanData);
            this.Apis.Add(FGet7B5WOTIME.FunctionName, FGet7B5WOTIME);
            this.Apis.Add(FGet7B5WOList.FunctionName, FGet7B5WOList);
            this.Apis.Add(FDeleteR7B5WO.FunctionName, FDeleteR7B5WO);
            this.Apis.Add(FGetWOTempSeleteValue.FunctionName, FGetWOTempSeleteValue);
            this.Apis.Add(FGet7B5WOTempList.FunctionName, FGet7B5WOTempList);
            this.Apis.Add(FR7B5WOTEMPageLoadAction.FunctionName, FR7B5WOTEMPageLoadAction);
            this.Apis.Add(FR7B5WOTempDelete.FunctionName, FR7B5WOTempDelete);
            this.Apis.Add(FR7B5WOTempEdit.FunctionName, FR7B5WOTempEdit);
            this.Apis.Add(FGetR7B5XMLTSeleteValue.FunctionName, FGetR7B5XMLTSeleteValue);
            this.Apis.Add(FGet7B5XMLTList.FunctionName, FGet7B5XMLTList);
            this.Apis.Add(FR7B5XMLTSplitQty.FunctionName, FR7B5XMLTSplitQty);
            this.Apis.Add(FR7B5XMLTUpdate.FunctionName, FR7B5XMLTUpdate);            
            this.Apis.Add(FUpload7B5XMLTExcel.FunctionName, FUpload7B5XMLTExcel);
            this.Apis.Add(FRTaskOverdueLoadingAction.FunctionName, FRTaskOverdueLoadingAction);
            this.Apis.Add(FRTaskOverdueGetDate.FunctionName, FRTaskOverdueGetDate);
            this.Apis.Add(FRTaskOverdueSearchAction.FunctionName, FRTaskOverdueSearchAction);
            this.Apis.Add(FGetSKU7B5ConfigType.FunctionName, FGetSKU7B5ConfigType);
            this.Apis.Add(FGetSKU7B5ConfigList.FunctionName, FGetSKU7B5ConfigList);
            this.Apis.Add(FSKU7B5ConfigAdd.FunctionName, FSKU7B5ConfigAdd);
            this.Apis.Add(FSKU7B5ConfigEdit.FunctionName, FSKU7B5ConfigEdit);
            this.Apis.Add(FSKU7B5ConfigDelete.FunctionName, FSKU7B5ConfigDelete);
            this.Apis.Add(FGet7B5POList.FunctionName, FGet7B5POList);
            this.Apis.Add(FUpload7B5POExcel.FunctionName, FUpload7B5POExcel);
            this.Apis.Add(FGetCutWoList.FunctionName, FGetCutWoList);
            this.Apis.Add(FGetWODataInfo.FunctionName, FGetWODataInfo);
            this.Apis.Add(FCutWoCheckWoData.FunctionName, FCutWoCheckWoData);
            this.Apis.Add(FCutWo.FunctionName, FCutWo);
            this.Apis.Add(FModifyShipExtQty.FunctionName, FModifyShipExtQty);
        }
       
        /// <summary>
        /// 獲取C_SKU_LINK_7B5里的所有記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5LinkList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {                
                List<C_SKU_LINK_7B5> list = new List<C_SKU_LINK_7B5>();
                T_C_SKU_LINK_7B5 TCSL = new T_C_SKU_LINK_7B5(SFCDB, DBTYPE);
                list = TCSL.GetLinkList(SFCDB, "");
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// By ID 刪除C_SKU_LINK_7B5里的記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Delete7B5Link(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            Newtonsoft.Json.Linq.JArray idArray = (Newtonsoft.Json.Linq.JArray)Data["ArrayID"];           
            if (idArray.Count == 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("Array ID");
                StationReturn.Data = "";
                return;
            }
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string id = "";
                for (int i = 0; i < idArray.Count; i++)
                {
                    id = idArray[i].ToString();
                    SFCDB.ORM.Deleteable<C_SKU_LINK_7B5>().Where(r => r.ID == id).ExecuteCommand();
                }                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Data = "";         
            }
            catch (Exception exception)
            {                 
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);               
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        /// <summary>
        /// 添加或修改C_SKU_LINK_7B5記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void EditSkunoLink7B5(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
            string objectData = Data["ObjectModel"].ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (string.IsNullOrEmpty(objectData))
                {
                    throw new Exception("Please input Object Model!");
                }
                T_C_SKU_LINK_7B5 TCSL = new T_C_SKU_LINK_7B5(SFCDB, DBTYPE);
                T_C_SKU TCS = new T_C_SKU(SFCDB, DBTYPE);
                C_SKU_LINK_7B5 objSkuLink = (C_SKU_LINK_7B5)JsonConvert.Deserialize(objectData, typeof(C_SKU_LINK_7B5));
                if (string.IsNullOrEmpty(objSkuLink.SKUNO))
                {
                    throw new Exception("Please Input Skuno!");
                }
                if (string.IsNullOrEmpty(objSkuLink.SUBSKUNO))
                {
                    throw new Exception("Please Input Sub Skuno!");
                }
                if (!TCS.IsExists(objSkuLink.SKUNO, SFCDB))
                {
                    throw new Exception(objSkuLink.SKUNO+ " Not Exist!");
                }
                if (!TCS.IsExists(objSkuLink.SUBSKUNO, SFCDB))
                {
                    throw new Exception(objSkuLink.SUBSKUNO + " Not Exist!");
                }                
                int result = 0;
                bool bAdd = true;
                if (string.IsNullOrEmpty(objSkuLink.ID))
                {                   
                    C_SKU_LINK_7B5 objExist = TCSL.GetLinkObject(SFCDB, objSkuLink.SKUNO, objSkuLink.SUBSKUNO);
                    if (objExist != null)
                    {
                        throw new Exception("The Link Of " + objSkuLink.SKUNO + " And" + objSkuLink.SUBSKUNO + "Already Exist!");
                    }

                    objSkuLink.ID = TCSL.GetNewID(this.BU, SFCDB);
                    objSkuLink.LASTEDIT = TCSL.GetDBDateTime(SFCDB);
                    objSkuLink.LASTEDITBY = LoginUser.EMP_NO;
                    result = TCSL.SaveObject(SFCDB, objSkuLink);
                }
                else
                {
                    bAdd = false;
                    objSkuLink.LASTEDIT = TCSL.GetDBDateTime(SFCDB);
                    objSkuLink.LASTEDITBY = LoginUser.EMP_NO;
                    result = TCSL.UpdateObject(SFCDB, objSkuLink);
                }
                if (result > 0)
                {
                    if (bAdd)
                    {
                        StationReturn.MessageCode = "MES00000002";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000003";
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;                   
                    StationReturn.Data = "";
                }
                else
                {
                    if (bAdd)
                    {
                        StationReturn.MessageCode = "MES00000021";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000025";
                    }
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }           

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        /// <summary>
        /// 獲取R_7B5_EC裡面的記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5ECList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_7B5_EC TREC = new T_R_7B5_EC(SFCDB, DBTYPE);
                //List<R_7B5_EC> list = new List<R_7B5_EC>();
                //list = TREC.GetECList(SFCDB);
                DataTable dt = TREC.GetECTable(SFCDB);
                if (dt.Rows.Count > 0)
                {                    
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 獲取R_7B5_EC界面中下拉框的值
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5ECStatusList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {                
                T_R_7B5_EC TREC = new T_R_7B5_EC(SFCDB, DBTYPE);
                List<R_7B5_EC> list = new List<R_7B5_EC>();       
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();                
                List<string> listItemType = new List<string>();
                list = TREC.GetListByCustomerFileName(SFCDB, "#ITEM_TYPE");
                dictionaryReturn.Add("ITEM_TYPE", list.Select(r => r.ITEM_TYPE).Distinct().ToList());

                list = TREC.GetListByCustomerFileName(SFCDB, "#STOCK_DEAL_REMARK");
                dictionaryReturn.Add("STOCK_DEAL_REMARK", list.Select(r => r.STOCK_DEAL_REMARK).Distinct().ToList());

                list = TREC.GetListByCustomerFileName(SFCDB, "#EC_TYPE");
                dictionaryReturn.Add("EC_TYPE", list.Select(r => r.EC_TYPE).Distinct().ToList());

                list = TREC.GetListByCustomerFileName(SFCDB, "#STATUS");
                dictionaryReturn.Add("STATUS", list.Select(r => r.STATUS).Distinct().ToList());

                string dtSys = TREC.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("SYSDATE", dtSys);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// BY ID 刪除7B5 EC記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Delete7B5EC(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {          
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (Data["ID"] == null || Data["ID"].ToString() == "")
                {
                    throw new Exception("Please input ID");
                }
                T_R_7B5_EC TREC = new T_R_7B5_EC(SFCDB, DBTYPE);
                int result = TREC.DeleteRecordingByID(SFCDB, Data["ID"].ToString());
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "MES00000023";
                    StationReturn.MessagePara.Add(Data["ID"].ToString());
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "MES00000004";
                    StationReturn.Data = "";
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        /// <summary>
        /// 添加或修改R_7B5_EC記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Edit7B5EC(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
            string objectData = Data["ObjectModel"].ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (string.IsNullOrEmpty(objectData))
                {
                    throw new Exception("Please input Object Model!");
                }
                R_7B5_EC objInputEC = (R_7B5_EC)JsonConvert.Deserialize(objectData, typeof(R_7B5_EC));
                T_R_7B5_EC TREC = new T_R_7B5_EC(SFCDB, DBTYPE);
                System.Reflection.PropertyInfo[] arrayProperty = objInputEC.GetType().GetProperties();
                foreach (var item in arrayProperty)
                {
                    if (item.Name == "ID" || item.Name == "EDIT_DT" || item.Name == "EDIT_EMP")
                    {
                        continue;
                    }
                    if (item.GetValue(objInputEC, null).ToString() == "" || item.GetValue(objInputEC, null) == null)
                    {
                        //throw new Exception(item.ToString() + "欄位必填,為空可填NA!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110659", new string[] { item.ToString() }));
                    }
                }
                DateTime online = (DateTime)objInputEC.ONLINE_DT;
                if ((online.ToString("yyyy-MM-dd") == "1900-01-01 " || objInputEC.WO == "待訂單" || objInputEC.TASK_NO == "待訂單") && objInputEC.STATUS == "CLOSED")
                {
                    //throw new Exception("上線時間、工單、任務令， 内容全都不是待订单才可設為CLOSED");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110826"));
                }
                TimeSpan ts = (TimeSpan)(objInputEC.FACTORY_EFFECT_DT - objInputEC.RECEIVE_DT);
                if (ts.TotalDays < 60 && objInputEC.STATUS == "CLOSED")
                {
                    //throw new Exception("當前時間大於生效時間60天才可設為CLOSED");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111518"));
                }
               
                int result = 0;
                if (string.IsNullOrEmpty(objInputEC.ID))
                {
                    objInputEC.ID = TREC.GetNewID(this.BU, SFCDB);
                    objInputEC.EDIT_DT = TREC.GetDBDateTime(SFCDB);
                    objInputEC.EDIT_EMP = LoginUser.EMP_NO;
                    result = TREC.SaveECObject(SFCDB, objInputEC);
                }
                else
                {
                    objInputEC.EDIT_DT = TREC.GetDBDateTime(SFCDB);
                    objInputEC.EDIT_EMP = LoginUser.EMP_NO;
                    result = TREC.UpdateECObject(SFCDB, objInputEC);
                }
                if (result > 0)
                {
                    if (string.IsNullOrEmpty(objInputEC.ID))
                    {
                        StationReturn.MessageCode = "MES00000002";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000003";
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    if (string.IsNullOrEmpty(objInputEC.ID))
                    {
                        StationReturn.MessageCode = "MES00000021";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000025";
                    }
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        /// <summary>
        /// 上傳R 7B5 PLAN頁面獲取上傳的系統時間及修改上傳時間權限
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5PlanUploadDate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);               
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();
                bool bChangeUploadDate = TCUP.CheckpPivilegeByName(SFCDB, "CHANGE_PLAN_UPLOAD_DATE", this.LoginUser.EMP_NO);                
                dictionaryReturn.Add("CHANGE_UPLOAD_DATE", bChangeUploadDate);

                string dtSys = TCUP.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("SYSDATE", dtSys);
                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 上傳7B5排配Excel
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Upload7B5PlanExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        { 
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();           
            SFCDB.ThrowSqlExeception = true;           
            try
            {
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
                }
                if (Data["UploadDate"] == null)
                {
                    throw new Exception("Please Input Upload Date");
                }
                if (Data["UploadDate"].ToString() == "")
                {
                    throw new Exception("Please Input Upload Date");
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);
                string uploadDate = Data["UploadDate"].ToString();

                string filePath = System.IO.Directory.GetCurrentDirectory() + $@"\UploadFile\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    //throw new Exception($@"上傳的Excel中沒有內容!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                }
                if (dt.Rows[0][22].ToString().ToUpper().Trim() != "BUCKET TYPE")
                {
                    //throw new Exception($@"上傳的文件內容有誤,W 列必須為: Bucket type !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111929"));
                }
                string sql = "", msg = "上傳成功！";
                string item = "", pcba = "", data1 = "", data2 = "", data3 = "", data4 = "", data5 = "", data6 = "", data7 = "", data8 = "",
                    data9 = "", data10 = "", data11 = "", data12 = "", data13 = "", data14 = "", model = "";
                string result = "", upload_msg = "";
                sql = " delete from  R_7B5_PLAN_UPLOAD ";
                SFCDB.ExecSQL(sql);
                for (int i = 2; i < dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i][22].ToString().ToUpper().Trim() == "EMS SCHEDULE")
                    {
                        model = dt.Rows[i][8].ToString().ToUpper().Trim() == "" ? "" : dt.Rows[i][8].ToString().ToUpper().Trim();
                        item = dt.Rows[i][10].ToString().ToUpper().Trim() == "" ? " " : dt.Rows[i][10].ToString().ToUpper().Trim();
                        pcba = dt.Rows[i][12].ToString().ToUpper().Trim() == "" ? " " : dt.Rows[i][12].ToString().ToUpper().Trim();
                        data1 = dt.Rows[i][24].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][24].ToString().ToUpper().Trim().Replace(",", "");
                        data2 = dt.Rows[i][25].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][25].ToString().ToUpper().Trim().Replace(",", "");
                        data3 = dt.Rows[i][26].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][26].ToString().ToUpper().Trim().Replace(",", "");
                        data4 = dt.Rows[i][27].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][27].ToString().ToUpper().Trim().Replace(",", "");
                        data5 = dt.Rows[i][28].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][28].ToString().ToUpper().Trim().Replace(",", "");
                        data6 = dt.Rows[i][29].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][29].ToString().ToUpper().Trim().Replace(",", "");
                        data7 = dt.Rows[i][30].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][30].ToString().ToUpper().Trim().Replace(",", "");
                        data8 = dt.Rows[i][31].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][31].ToString().ToUpper().Trim().Replace(",", "");
                        data9 = dt.Rows[i][32].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][32].ToString().ToUpper().Trim().Replace(",", "");
                        data10 = dt.Rows[i][33].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][33].ToString().ToUpper().Trim().Replace(",", "");
                        data11 = dt.Rows[i][34].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][34].ToString().ToUpper().Trim().Replace(",", "");
                        data12 = dt.Rows[i][35].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][35].ToString().ToUpper().Trim().Replace(",", "");
                        data13 = dt.Rows[i][36].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][36].ToString().ToUpper().Trim().Replace(",", "");
                        data14 = dt.Rows[i][37].ToString().ToUpper().Trim() == "" ? "0" : dt.Rows[i][37].ToString().ToUpper().Trim().Replace(",", "");

                        sql = $@"insert into  R_7B5_PLAN_UPLOAD (ITEM, PCBA, DAY1, DAY2, DAY3, DAY4, DAY5, DAY6, DAY7, DAY8, DAY9, DAY10, DAY11, DAY12, DAY13, DAY14, UPLOADDT, UPLOADBY,MODEL) 
                                Values
                                ('{item}','{pcba}','{data1}','{data2}','{data3}','{data4}','{data5}','{data6}','{data7}','{data8}','{data9}','{data10}','{data11}','{data12}','{data13}','{data14}',SYSDATE,'{LoginUser.EMP_NO}','{model}')";
                        result = SFCDB.ExecSQL(sql);
                        if (result != "1")
                        {                            
                            //throw new Exception("上傳的文件內容有誤,請檢查數字是否為文本格式 !");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112132"));
                        }
                    }
                }

                try
                {
                    C7B5Function.UploadPlan(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle, LoginUser.EMP_NO, uploadDate,out upload_msg);
                }
                catch (Exception ex)
                {
                    upload_msg = ex.Message;
                }
                if (upload_msg != "")
                {
                    msg = "鎖排分配異常!" + upload_msg;
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara.Add(msg);
                }
                else
                {
                    StationReturn.MessageCode = "MES00000001";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Message = msg;
                //StationReturn.MessageCode = "MES00000002";
                //StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {               
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 獲取7B5 Plan 記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5PlanList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                //string sql = "select distinct a.item,c.sku_name MODEL,case when a.day1<0 then  a.day1 + b.day1 else a.day1 end day1, case when a.day2<0 then  a.day2 + b.day2 else a.day2 end day2, ";
                //sql = sql + "case when a.day3<0 then  a.day3 + b.day3 else a.day3 end day3,case when a.day4<0 then  a.day4 + b.day4 else a.day4 end day4, ";
                //sql = sql + "case when a.day5<0 then  a.day5 + b.day5 else a.day5 end day5,case when a.day6<0 then  a.day6 + b.day6 else a.day6 end day6, ";
                //sql = sql + "case when a.day7<0 then  a.day7 + b.day7 else a.day7 end day7,case when a.day8<0 then  a.day8 + b.day8 else a.day8 end day8, ";
                //sql = sql + "case when a.day9<0 then  a.day9 + b.day9 else a.day9 end day9,case when a.day10<0 then  a.day10 + b.day10 else a.day10 end day10, ";
                //sql = sql + "case when a.day11<0 then  a.day11 + b.day11 else a.day11 end day11,case when a.day12<0 then  a.day12 + b.day12 else a.day12 end day12, ";
                //sql = sql + "case when a.day13<0 then  a.day13 + b.day13 else a.day13 end day13,case when a.day14<0 then  a.day14 + b.day14 else a.day14 end day14,A.LASTEDITDT,A.LASTEDITBY  ";
                //sql = sql + "from r_7b5_plan a,  r_7b5_plan_temp b, c_sku c where a.item=b.item and a.item=c.cust_partno(+) order by a.item";
                string sql = "select distinct a.item,case when a.MODEL is null then (select distinct c.sku_name from  c_sku c where a.item=c.cust_partno(+) )  else a.MODEL end MODEL,case when a.day1<0 then  a.day1 + b.day1 else a.day1 end day1, case when a.day2<0 then  a.day2 + b.day2 else a.day2 end day2, ";
                sql = sql + "case when a.day3<0 then  a.day3 + b.day3 else a.day3 end day3,case when a.day4<0 then  a.day4 + b.day4 else a.day4 end day4, ";
                sql = sql + "case when a.day5<0 then  a.day5 + b.day5 else a.day5 end day5,case when a.day6<0 then  a.day6 + b.day6 else a.day6 end day6, ";
                sql = sql + "case when a.day7<0 then  a.day7 + b.day7 else a.day7 end day7,case when a.day8<0 then  a.day8 + b.day8 else a.day8 end day8, ";
                sql = sql + "case when a.day9<0 then  a.day9 + b.day9 else a.day9 end day9,case when a.day10<0 then  a.day10 + b.day10 else a.day10 end day10, ";
                sql = sql + "case when a.day11<0 then  a.day11 + b.day11 else a.day11 end day11,case when a.day12<0 then  a.day12 + b.day12 else a.day12 end day12, ";
                sql = sql + "case when a.day13<0 then  a.day13 + b.day13 else a.day13 end day13,case when a.day14<0 then  a.day14 + b.day14 else a.day14 end day14,A.LASTEDITDT,A.LASTEDITBY  ";
                sql = sql + "from r_7b5_plan a,  r_7b5_plan_temp b where a.item=b.item order by a.item";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 獲取SHIP_PLAN和SHIP_PLAN_DATA頁面的model及系統時間
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5ShipModel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_7B5_SHIP TRS = new T_R_7B5_SHIP(SFCDB, DBTYPE);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();
                if (Data["Data"].ToString() == "R_7B5_SHIP")
                {
                    var modelShip = SFCDB.ORM.Queryable<R_7B5_SHIP>().Select(r => new { name = r.MODEL, value = r.MODEL }).ToList().Distinct().ToList().OrderBy(r => r.name);
                    dictionaryReturn.Add("Model", modelShip);
                }
                else if (Data["Data"].ToString() == "R_7B5_SHIP_DATA")
                {
                    var modelShipData = SFCDB.ORM.Queryable<R_7B5_SHIP, R_7B5_SHIP_DATA>((rs, rsd) => rs.TASK_NO == rsd.TASK_NO)
                        .Select((rs, rsd) => new { name = rs.MODEL, value = rs.MODEL }).ToList().Distinct().ToList().OrderBy(r => r.name);
                    dictionaryReturn.Add("Model", modelShipData);
                }
                string dtSys = TRS.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("DATE_FROM", dtSys + " 00:00:00");
                dictionaryReturn.Add("DATE_TO", dtSys + " 23:59:59");

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_7B5_SHIP(SHIP_PLAN)頁面獲取數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5ShipList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {           
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = $@"select A.MODEL, A.TASK_NO, A.HH_ITEM,to_char(A.TASK_QTY) TASK_QTY, B.PO_NO, B.PRICE,to_char(A.TASK_QTY-A.BUFFER_QTY-A.TOTAL_PLAN_QTY)  EXT_QTY, 
                            to_char(A.BUFFER_QTY) BUFFER_QTY,BUFFER_REMARK,to_char(A.SHIPPED_QTY) SHIPPED_QTY,to_char(A.TOTAL_PLAN_QTY-A.SHIPPED_QTY) PLAN_NOT_PGI,
                            A.HW_ITEM,A.LASTEDITBY,A.RECEIVE_DATE  from R_7B5_SHIP a, r_7b5_po b,r_7b5_xml_t c where c.TASK_NO=a.task_no and c.CANCEL_FLAG='N'
                            and a.task_no=b.task_no(+)  ";
                string sub_sql = " union select A.MODEL, A.TASK_NO, A.HH_ITEM,to_char(A.TASK_QTY) TASK_QTY, B.PO_NO, B.PRICE,to_char(A.TASK_QTY-A.BUFFER_QTY-A.TOTAL_PLAN_QTY)  EXT_QTY,"
                            + " to_char(A.BUFFER_QTY) BUFFER_QTY,A.BUFFER_REMARK,to_char(A.SHIPPED_QTY) SHIPPED_QTY,to_char(A.TOTAL_PLAN_QTY-A.SHIPPED_QTY) PLAN_NOT_PGI,"
                            + " A.HW_ITEM, A.LASTEDITBY, A.RECEIVE_DATE  from R_7B5_SHIP a, r_7b5_po b,c_sku_link_7b5 c, r_7b5_ship d, r_7b5_xml_t e"
                            + " where e.task_no = d.task_no AND e.cancel_flag = 'N' and d.task_qty>d.TOTAL_PLAN_QTY and c.skuno=d.hh_item"
                            + " and c.subskuno=a.hh_item and c.seqno=8 and a.task_no=b.task_no(+)  ";        
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Data["Data"].ToString());
                
                string task_no = obj["TASK_NO"].ToString();
                string hw_pn = obj["HW_PN"].ToString();
                string hh_pn = obj["HH_PN"].ToString();
                string model = obj["MODEL"].ToString();
                string date_from = obj["DATE_FROM"].ToString();
                string date_to = obj["DATE_TO"].ToString();
                string show_sub = obj["SHOW_SUB"].ToString();
                string have_buffer = obj["HAVE_BUFFER"].ToString();
                string open_task_only = obj["OPEN_TASK_ONLY"].ToString();
                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND a.TASK_NO = '{task_no}' ";
                    sub_sql = sub_sql + $@" AND d.TASK_NO = '{task_no}' ";
                }
                if (!string.IsNullOrEmpty(hw_pn))
                {
                    sql = sql + $@" AND a.HW_ITEM = '{hw_pn}' ";
                    sub_sql = sub_sql + $@" AND d.HW_ITEM = '{hw_pn}' ";
                }
                if (!string.IsNullOrEmpty(hh_pn))
                {
                    sql = sql + $@" AND a.HH_ITEM = '{hh_pn}' ";
                    sub_sql = sub_sql + $@" AND d.HH_ITEM = '{hh_pn}' ";
                }
                if (!string.IsNullOrEmpty(model))
                {
                    sql = sql + $@" AND a.model = '{model}' ";
                    sub_sql = sub_sql + $@" AND d.model = '{model}' ";
                }
                if (have_buffer.ToLower() == "true")
                {
                    sql = sql + " AND a.BUFFER_QTY > 0 ";
                    sub_sql = sub_sql + " AND d.BUFFER_QTY > 0 ";
                }
                if (open_task_only.ToLower() == "true")
                {
                    sql = sql + " AND a.task_qty>a.TOTAL_PLAN_QTY ";
                    sub_sql = sub_sql + " AND a.task_qty>a.TOTAL_PLAN_QTY ";
                }
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    sql = sql + $@" AND   A.RECEIVE_DATE  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{date_to}','YYYY/MM/DD HH24:MI:SS') ";
                    sub_sql = sub_sql + $@" AND   d.RECEIVE_DATE  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{date_to}','YYYY/MM/DD HH24:MI:SS') ";
                }
                if (show_sub.ToLower() == "true")
                {
                    sql = sql + sub_sql;                   
                }               
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// SHIP PLAN頁面修改數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void ModifyShipPlan(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Data["Data"].ToString());
                int result = 0;
                string task_no, hh_item, hw_item, remark;
                string user = LoginUser.EMP_NO;
                Decimal ext_qty;
                if (obj["TYPE"] == null || obj["TYPE"].ToString() == "")
                {
                    throw new Exception("Please Input Type!");
                }
                else if (obj["TYPE"].ToString() == "ModifyBuffer")
                {
                    ext_qty = Convert.ToDecimal(obj["EXT_QTY"].ToString());
                    Decimal old_buffer_qty = Convert.ToDecimal(obj["OLD_BUFFER_QTY"].ToString());
                    Decimal new_buffer_qty = Convert.ToDecimal(obj["BUFFER_QTY"].ToString());
                    remark = obj["BUFFER_REMARK"].ToString();
                    task_no = obj["TASK_NO"].ToString();
                    hh_item = obj["HH_ITEM"].ToString();
                    if (new_buffer_qty < 0)
                    {
                        //throw new Exception("輸入的BUFFER QTY無效!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112332"));
                    }
                    if (ext_qty + old_buffer_qty < new_buffer_qty)
                    {
                        //throw new Exception("輸入的BUFFER QTY大於未出貨數量!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112503"));
                    }
                    double buffer_qty = Convert.ToDouble(new_buffer_qty);
                    T_R_7B5_SHIP TRS = new T_R_7B5_SHIP(SFCDB, DBTYPE);
                    DateTime dateTime = TRS.GetDBDateTime(SFCDB);
                    result = SFCDB.ORM.Updateable<R_7B5_SHIP>().UpdateColumns(
                        r => new R_7B5_SHIP
                        {
                            BUFFER_QTY = buffer_qty,
                            BUFFER_REMARK = remark,
                            LASTEDITBY = user,
                            LASTEDITDT = dateTime
                        })
                        .Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ExecuteCommand();
                    if (result <= 0)
                    {
                        //throw new Exception("修改失敗!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112950"));
                    }
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.MessagePara = null;
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    SFCDB.CommitTrain();
                }
                else if(obj["TYPE"].ToString() == "ModifyPlan")
                {
                    task_no = obj["TASK_NO"].ToString();
                    hh_item = obj["HH_ITEM"].ToString();
                    hw_item = obj["HW_ITEM"].ToString();
                    ext_qty = Convert.ToDecimal(obj["EXT_QTY"].ToString());
                    string plan_qty = obj["PLAN_QTY"].ToString();
                    remark = obj["REMARK"].ToString();
                    string with_sub_flag = obj["WITH_SUB"].ToString().ToUpper() == "TRUE" ? "1" : "0";
                    if (plan_qty == "")
                    {
                        //throw new Exception("PLAN QTY不能為空!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "PLAN_QTY" }));
                    }
                    if (Convert.ToDecimal(plan_qty) < 0 || Convert.ToDecimal(plan_qty) > Convert.ToDecimal(ext_qty))
                    {
                        //throw new Exception("輸入的PLAN QTY能小於0或大於EXT_QTY數量!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113514"));
                    }
                    //新BS業務模式,BOM比對結果含NG的不能發貨(楊兵) BY ZXY 20211202
                    string sql = $@"SELECT * FROM r_task_sappop_compare WHERE 1 = 1 AND task_no = '{task_no}' AND RESULT IN ('NG', 'DEL', 'UNLOCK')";
                    DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dt.Rows.Count > 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211202094353"));
                    }

                    C7B5Function.ShipPlan(SFCDB, DBTYPE, task_no, hh_item, Convert.ToDouble(plan_qty), with_sub_flag, user, remark);
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.MessagePara = null;
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    SFCDB.CommitTrain();
                }
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        /// <summary>
        /// R_7B5_SHIP_DATA(SHIP_PLAN_DATA)頁面獲取數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5ShipDataList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "";
                sql = "select a.LOTNO,c.po_no,a.TASK_NO,a.HH_ITEM,b.model,'PC' PRICE_UNIT,TO_CHAR(a.QTY) QTY,c.PRICE/1000 PRICE,a.REMARK,a.LASTEDITBY,a.LASTEDITDT,a.DELETE_FLAG,"
                    + "a.SAP_FLAG from r_7b5_ship_data a, r_7b5_ship b, r_7b5_po c where a.task_no=b.task_no and a.task_no=c.task_no and a.HH_ITEM=b.HH_ITEM  ";
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Data["Data"].ToString());
                string lot_no = obj["LOT_NO"].ToString();
                string task_no = obj["TASK_NO"].ToString();
                string hw_item = obj["HW_ITEM"].ToString();
                string hh_item = obj["HH_ITEM"].ToString();
                string model = obj["MODEL"].ToString();
                string date_from = obj["DATE_FROM"].ToString();
                string date_to = obj["DATE_TO"].ToString();
                string sap_flag = obj["SAP_FLAG"].ToString();
                if (!string.IsNullOrEmpty(lot_no))
                {
                    sql = sql + $@" AND a.LotNo = '{lot_no}' ";
                }
                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND a.TASK_NO = '{task_no}' ";                    
                }
                if (!string.IsNullOrEmpty(hw_item))
                {
                    sql = sql + $@" AND a.HW_ITEM = '{hw_item}' ";                    
                }
                if (!string.IsNullOrEmpty(hh_item))
                {
                    sql = sql + $@" AND a.HH_ITEM = '{hh_item}' ";
                }
                if (!string.IsNullOrEmpty(model))
                {
                    sql = sql + $@" AND b.model = '{model}' ";                 
                }
                if (!string.IsNullOrEmpty(sap_flag))
                {
                    sql = sql + $@" AND a.SAP_FLAG ='{sap_flag}' ";                  
                }               
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    sql = sql + $@" AND   A.LASTEDITDT  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{date_to}','YYYY/MM/DD HH24:MI:SS') ";
                    
                }
                sql = sql + " order by a.LASTEDITDT ";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);                
            }
        }
        /// <summary>
        /// R_7B5_SHIP_DATA(SHIP_PLAN_DATA)頁面刪除數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteShipPlanData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string task_no = Data["TASK_NO"].ToString();
                string lot_no = Data["LOTNO"].ToString();
                string qty = Data["QTY"].ToString();
                T_R_7B5_SHIP_DATA TRSD = new T_R_7B5_SHIP_DATA(SFCDB, DBTYPE);
                T_R_7B5_PGI_DATA TRPD = new T_R_7B5_PGI_DATA(SFCDB, DBTYPE);
                List<R_7B5_SHIP_DATA> listShip = TRSD.GetListByLotNo(SFCDB, lot_no);
                List<R_7B5_SHIP_DATA> listShipDelete = listShip.Where(r => r.SAP_FLAG == "Y" || r.DELETE_FLAG == "Y").ToList();
                if (listShipDelete.Count > 0)
                {
                    //throw new Exception(lot_no + ",此出貨預排已經刪除!!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113641", new string[] { lot_no }));
                }
                List<R_7B5_PGI_DATA> listPGI = TRPD.GetListByTaskAndLot(SFCDB, task_no, lot_no);
                if (listPGI.Count > 0)
                {
                    //throw new Exception(lot_no + "," + task_no + ",已做PGI的出貨預排不能刪除!!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113927", new string[] { lot_no, task_no }));
                }
                DateTime dateTime = TRSD.GetDBDateTime(SFCDB);
                foreach (var s in listShip)
                {
                    SFCDB.ORM.Updateable<R_7B5_SHIP>().UpdateColumns(
                        r => new R_7B5_SHIP
                        {
                            TOTAL_PLAN_QTY = r.TOTAL_PLAN_QTY - s.QTY,
                            LASTEDITDT = dateTime,
                            LASTEDITBY = LoginUser.EMP_NO
                        })
                        .Where(r => r.TASK_NO == s.TASK_NO && r.HH_ITEM == s.HH_ITEM).ExecuteCommand();
                }
                SFCDB.ORM.Updateable<R_7B5_SHIP_DATA>().UpdateColumns(
                    r => new R_7B5_SHIP_DATA
                    {
                        DELETE_FLAG = "Y",
                        LASTEDITDT = dateTime,
                        LASTEDITBY = LoginUser.EMP_NO
                    }).Where(r => r.LOTNO == lot_no).ExecuteCommand();
                SFCDB.CommitTrain();
                StationReturn.MessageCode = "MES00000002";
                StationReturn.MessagePara = null;
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
                SFCDB.RollbackTrain();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);                
            }
        }
        /// <summary>
        /// R_7B5_WO頁面加載時執行動作
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5WOTIME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                T_R_7B5_WO TRW = new T_R_7B5_WO(SFCDB, DBTYPE);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();               
                string dtSys = TRW.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("DATE_FROM", dtSys + " 00:00:00");
                dictionaryReturn.Add("DATE_TO", dtSys + " 23:59:59");

                string sql = $@"update r_7b5_wo a set a.WO_STATUS='release' where a.sap_wo  is not null and exists (select * from r_wo_base b where a.sap_wo=b.WORKORDERNO ) 
                                   and a.create_time>sysdate-30 and a.wo_status = 'release'";
                SFCDB.ExecSQL(sql);

                sql = $@"update r_7b5_wo a set load_qty=(select count(*) from r_sn b where a.sap_wo=b.workorderno) where a.sap_wo is not null 
                            and load_qty<>(select count(*) from r_sn c where a.sap_wo=c.workorderno) and a.create_time>sysdate-30 ";
                SFCDB.ExecSQL(sql);

                sql = $@"update r_7b5_wo a set a.wo_qty=(select b.WORKORDER_QTY from r_wo_base b where a.SAP_WO=b.workorderno) where sap_wo is not null and DELETE_FLAG='N' AND WO_STATUS='release' 
                           and  a.wo_qty<>(select c.WORKORDER_QTY from r_wo_base c where a.SAP_WO=c.workorderno) and a.create_time>sysdate-30 ";
                SFCDB.ExecSQL(sql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_7B5_WO頁面獲取數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5WOList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "select b.SAP_FACTORY,a.task_no,a.v_task_no,to_char(a.CREATE_TIME,'yyyy/mm/dd') CREATE_TIME,a.TASK_qty,a.HW_ITEM,a.HH_ITEM,a.SAP_WO,"
                        + "a.WO_QTY,a.LOAD_QTY,a.DELETE_FLAG,a.WO_STATUS from r_7b5_wo a, r_7b5_wo_temp b  where a.SAP_WO IS NOT NULL and a.TASK_NO=b.task_no "
                        + "and a.V_TASK_NO=b.V_TASK_NO and a.hh_item=b.hh_item ";
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Data["Data"].ToString());

                string task_no = obj["TASK_NO"].ToString();
                string hw_item = obj["HW_ITEM"].ToString();
                string date_from = obj["DATE_FROM"].ToString();
                string date_to = obj["DATE_TO"].ToString();

                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND a.TASK_NO = '{task_no}' ";
                }
                if (!string.IsNullOrEmpty(hw_item))
                {
                    sql = sql + $@" AND a.HW_ITEM = '{hw_item}' ";
                }
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    sql = sql + $@" AND A.CREATE_TIME  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{date_to}','YYYY/MM/DD HH24:MI:SS') ";

                }
                sql = sql + " order by a.CREATE_TIME ";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_WO頁面刪除數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteR7B5WO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();           
            try
            {
                string task_no = Data["TASK_NO"].ToString();
                string sap_wo = Data["SAP_WO"].ToString();
               
                T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);
                List<R_SN> list = TRS.GetRSNbyWo(sap_wo, SFCDB);
                if (list.Count > 0)
                {
                    throw new Exception(sap_wo + ",此工單已投產,不允許刪除!!");
                }
                int resutl = SFCDB.ORM.Updateable<R_7B5_WO>().UpdateColumns(
                                r => new R_7B5_WO
                                {
                                    DELETE_FLAG = "Y",
                                    WO_STATUS = "delete"
                                }).Where(r => r.TASK_NO == task_no && r.SAP_WO == sap_wo).ExecuteCommand();
                if (resutl == 0)
                {
                    //throw new Exception("刪除失敗！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057"));
                }
                StationReturn.Message = "工單刪除OK,請及時到SAP刪除相應工單!";
                StationReturn.MessageCode = "";
                StationReturn.MessagePara = null;
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);              
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// R_7B5_WO_TEMP頁面加載獲取信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetWOTempSeleteValue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                T_R_7B5_XML_T TRXT = new T_R_7B5_XML_T(SFCDB, DBTYPE);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();
                string dtSys = TRXT.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("RECEIVE_DATE_FROM", dtSys + " 00:00:00");
                dictionaryReturn.Add("RECEIVE_DATE_TO", dtSys + " 23:59:59");
                dictionaryReturn.Add("PLAN_DATE_FROM", dtSys);
                dictionaryReturn.Add("PLAN_DATE_TO", dtSys);
                var model = SFCDB.ORM.Queryable<R_7B5_XML_T>().Select(r => new { name = r.MODEL, value = r.MODEL }).ToList().Distinct().ToList().OrderBy(r=>r.name);
                dictionaryReturn.Add("MODEL", model);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_7B5_WO_TEMP頁面加載執行的動作
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void R7B5WOTEMPageLoadAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string countMsg = "", saveMsg = "";
                C7B5Function.SaveWOQty(SFCDB, DBTYPE, this.IP, ref saveMsg);
                C7B5Function.CountQty(SFCDB, DBTYPE,ref countMsg);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = null;
                StationReturn.MessagePara = null;
                StationReturn.Data = "";
                StationReturn.Message = "Save:" + saveMsg + ";Count:" + countMsg;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_7B5_WO_TEMP頁面搜索事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5WOTempList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                //string sql = "select to_char(a.RECEIVE_DATE,'YYYY/MM/DD') RECEIVE_DATE, a.TASK_NO_TYPE, a.TASK_NO_USE, a.SAP_FACTORY,a.MODEL, a.PRODUCT_LINE, a.TASK_NO, a.V_TASK_NO,"
                //        + " a.HH_ITEM, a.HW_ITEM,to_char(a.QTY) QTY, a.START_DATE,A.COMPLETE_DATE,a.MAIN_WO_FLAG,a.WO_REMARK, a.CREATE_TIME, a.ZNP195_FLAG,a.WO_TYPE, a.ZNP195_MESSAGE,"
                //        + "to_char(a.CREAT_WO_QTY) CREAT_WO_QTY, to_char(a.SUGGEST_QTY) SUGGEST_QTY,b.plan_flag,b.po_flag,b.cancel_flag,a.TASK_NO_LEVEL from r_7b5_wo_temp a,r_7b5_xml_t b"
                //        + " where a.task_no=b.task_no  "; 

                string sql = $@"select to_char(a.RECEIVE_DATE,'YYYY/MM/DD') RECEIVE_DATE,a.MODEL,a.TASK_NO,a.V_TASK_NO,a.HH_ITEM, a.HW_ITEM,b.plan_flag,b.po_flag, to_char(a.QTY) QTY,
                                to_char(a.CREAT_WO_QTY) CREAT_WO_QTY,to_char(a.SUGGEST_QTY) SUGGEST_QTY,a.ZNP195_FLAG,a.START_DATE,A.COMPLETE_DATE,a.MAIN_WO_FLAG,a.WO_REMARK,a.CREATE_TIME,
                                a.WO_TYPE, a.ZNP195_MESSAGE,a.TASK_NO_TYPE, a.SAP_FACTORY, a.TASK_NO_USE, a.PRODUCT_LINE,b.cancel_flag, a.TASK_NO_LEVEL from r_7b5_wo_temp a,r_7b5_xml_t b
                           where a.task_no=b.task_no  ";

                //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Data["Data"].ToString());

                string task_no = Data["TASK_NO"] == null ? "" : Data["TASK_NO"].ToString();
                string hw_item = Data["HW_PN"] == null ? "" : Data["HW_PN"].ToString();
                string model = Data["MODEL"] == null ? "" : Data["MODEL"].ToString();
                string task_no_type = Data["TASK_NO_TYPE"] == null ? "" : Data["TASK_NO_TYPE"].ToString();
                string zpn195_flag = Data["ZPN195_FLAG"] == null ? "" : Data["ZPN195_FLAG"].ToString();
                string po_flag = Data["PO_FLAG"] == null ? "" : Data["PO_FLAG"].ToString();
                string receive_date_from = Data["RECEIVE_DATE_FROM"] == null ? "" : Data["RECEIVE_DATE_FROM"].ToString();
                string receive_date_to = Data["RECEIVE_DATE_TO"] == null ? "" : Data["RECEIVE_DATE_TO"].ToString();
                string plan_date_from = Data["PLAN_DATE_FROM"] == null ? "" : Data["PLAN_DATE_FROM"].ToString();
                string plan_date_to = Data["PLAN_DATE_TO"] == null ? "" : Data["PLAN_DATE_TO"].ToString();
                string open_task_only = Data["OPEN_TASK_ONLY"] == null ? "" : Data["OPEN_TASK_ONLY"].ToString();
                string abnormal_task = Data["ABNORMAL_TASK"] == null ? "" : Data["ABNORMAL_TASK"].ToString();

                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND a.TASK_NO = '{task_no}' ";
                }
                if (!string.IsNullOrEmpty(hw_item))
                {
                    sql = sql + $@" AND a.HW_ITEM = '{hw_item}' ";
                }
                if (!string.IsNullOrEmpty(plan_date_from) && !string.IsNullOrEmpty(plan_date_to))
                {
                    sql = sql + $@" AND exists (select * from r_7b5_plan_task c where a.task_no=c.task_no and c.cancel_flag='N'
                                    and c.plan_dt BETWEEN to_date('{plan_date_from}','YYYY/MM/DD') and to_date('{plan_date_to}','YYYY/MM/DD'))  ";

                }
                if (!string.IsNullOrEmpty(receive_date_from) && !string.IsNullOrEmpty(receive_date_to))
                {
                    sql = sql + $@" AND  a.RECEIVE_DATE  BETWEEN TO_DATE('{receive_date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{receive_date_to}','YYYY/MM/DD HH24:MI:SS') ";
                }
                if (!string.IsNullOrEmpty(model))
                {
                    sql = sql + $@" AND A.model = '{model}' ";
                }
                if (!string.IsNullOrEmpty(zpn195_flag))
                {
                    sql = sql + $@" AND A.ZNP195_FLAG = '{zpn195_flag}' ";
                }
                if (!string.IsNullOrEmpty(po_flag))
                {
                    sql = sql + $@"  AND b.po_flag = '{po_flag}' ";
                }
                if (!string.IsNullOrEmpty(task_no_type))
                {
                    sql = sql + $@" AND A.TASK_NO_TYPE = '{task_no_type}' ";
                }                
                if (open_task_only.ToUpper() == "TRUE")
                {
                    sql = sql + $@" AND a.CREAT_WO_QTY < a.qty and b.CANCEL_FLAG='N' ";
                }
                if (abnormal_task.ToUpper() == "TRUE")
                {
                    sql = sql + $@" AND LEFT (a.task_no, 2) <> 'BS' AND (SUBSTR (a.task_no, 3, 1) <> 'Z'  OR SUBSTR (a.task_no, 2, 1) = 'R' OR SUBSTR (a.task_no, 2, 1) = 'S' OR SUBSTR (a.task_no, 4, 1) = 'S')";
                }
                sql = sql + "   order by a.CREATE_TIME desc";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_WO_TEMP頁面刪除事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void R7B5WOTempDelete(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            SFCDB.BeginTrain();
            try
            {
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);
                T_R_7B5_WO TRW = new T_R_7B5_WO(SFCDB, DBTYPE);
                bool bDeletePivilege = TCUP.CheckpPivilegeByName(SFCDB, "R_7B5_WO_TEMP_DELETE", this.LoginUser.EMP_NO);
                if (!bDeletePivilege)
                {
                    //throw new Exception("此賬號無權做DELETE動作!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115744"));
                }
                string task_no = Data["TASK_NO"].ToString();
                string hh_item = Data["HH_ITEM"].ToString();
                //sql = "select * from r_7b5_wo where task_no='" + str_task_no + "' and delete_flag='N' and sap_wo is not null"
                List<R_7B5_WO> listWO = SFCDB.ORM.Queryable<R_7B5_WO>().Where(r => r.TASK_NO == task_no && r.DELETE_FLAG == "N" && !SqlSugar.SqlFunc.IsNullOrEmpty(r.SAP_WO)).ToList();
                if (listWO.Count > 0)
                {
                    //throw new Exception("此任務令存在已經開立的工單,請將已開立的工單刪除再做delete動作!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115837"));
                }
                //sql = "delete r_7b5_wo_temp where task_no='" + str_task_no + "'"
                SFCDB.ORM.Deleteable<R_7B5_WO_TEMP>().Where(r => r.TASK_NO == task_no).ExecuteCommand();
                //sql = "update r_7b5_xml_t set TRANSFER_FLAG='N' where task_no='" + str_task_no + "'"
                SFCDB.ORM.Updateable<R_7B5_XML_T>().UpdateColumns(r => new R_7B5_XML_T { TRANSFER_FLAG = "N" }).Where(r => r.TASK_NO == task_no).ExecuteCommand();
                if (task_no.Substring(3, 1) == "S")
                {
                    //sql = "delete r_7b5_ship where task_no='" + Left(str_task_no, 3) + "6" + Right(str_task_no, str_task_no.Length - 4) + "' AND HH_ITEM='" + str_hh_item + "'"    
                    string task_no_temp = task_no.Substring(0, 3) + "6" + task_no.Substring(4, task_no.Length - 4);
                    SFCDB.ORM.Deleteable<R_7B5_SHIP>().Where(r => r.TASK_NO == task_no_temp && r.HH_ITEM == hh_item).ExecuteCommand();
                }
                else
                {
                    //sql = "delete r_7b5_ship where task_no='" + str_task_no + "' AND HH_ITEM='" + str_hh_item + "'"
                    SFCDB.ORM.Deleteable<R_7B5_SHIP>().Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ExecuteCommand();
                }
                T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB,DBTYPE);
                R_MES_LOG log = new R_MES_LOG();
                log.ID = TRML.GetNewID(BU, SFCDB);
                log.PROGRAM_NAME = "WEB";
                log.CLASS_NAME = "MESStation.Config.HWD.C7B5API";
                log.FUNCTION_NAME = "DELETE_WO_TEMP";
                log.DATA1 = task_no;
                log.DATA2 = hh_item;
                log.EDIT_EMP = LoginUser.EMP_NO;
                log.EDIT_TIME = TRML.GetDBDateTime(SFCDB);
                TRML.InsertMESLogOld(log, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MSGCODE20210814115159";
                StationReturn.Message = "任務令匹配的HH料號資料已全部刪除成功,請到SAP中刪除ZPN195信息!";
                StationReturn.MessagePara = null;
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {               
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                SFCDB.RollbackTrain();
            }
            finally {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// R_7B5_WO_TEMP頁面編輯事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void R7B5WOTempEdit(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            SFCDB.BeginTrain();
            try
            {
                string receive_date = Data["RECEIVE_DATE"].ToString();
                string task_no_type = Data["TASK_NO_TYPE"].ToString();
                string task_no_use = Data["TASK_NO_USE"].ToString();               
                string sap_factory = Data["SAP_FACTORY"].ToString();
                string model = Data["MODEL"].ToString();
                string task_no = Data["TASK_NO"].ToString();
                string v_task_no = Data["V_TASK_NO"].ToString();
                string hh_item = Data["HH_ITEM"].ToString();
                string hw_item = Data["HW_ITEM"].ToString();
                string qty = Data["QTY"].ToString();
                string start_date = Data["START_DATE"].ToString();
                string complete_date = Data["COMPLETE_DATE"].ToString();
                string plan_flag = Data["PLAN_FLAG"].ToString();
                string po_flag = Data["PO_FLAG"].ToString();
                string main_wo_flag = Data["MAIN_WO_FLAG"].ToString();
                string znp_195_flag = Data["ZNP195_FLAG"].ToString();
                string wo_type = Data["WO_TYPE"].ToString();
                string creat_wo_qty = Data["CREAT_WO_QTY"].ToString();
                string suggest_qty = Data["SUGGEST_QTY"].ToString();
                string cancel_flag = Data["CANCEL_FLAG"].ToString();
                string task_no_level = Data["TASK_NO_LEVEL"].ToString();

                start_date = System.DateTime.Now.ToString("yyyyMMdd");
                complete_date = System.DateTime.Now.AddDays(2).ToString("yyyyMMdd");                
                string result = "";
                string message = "";
                string returnMessage = "";
                #region CALL SAP ZCPP_NSBG_0121
                if (znp_195_flag == "N")
                {                    
                    ZCPP_NSBG_0121 zcpp_nsbg_0121 = new ZCPP_NSBG_0121(BU);
                    DataTable in_table = new DataTable();
                    DataTable out_table = new DataTable();
                    in_table.Columns.Add("POSDT");
                    in_table.Columns.Add("TSORT");
                    in_table.Columns.Add("TSORU");
                    in_table.Columns.Add("TSORL");
                    in_table.Columns.Add("WERKS");
                    in_table.Columns.Add("EXTWG");
                    in_table.Columns.Add("TSORNUM");
                    in_table.Columns.Add("MATNR");
                    in_table.Columns.Add("HWDMAT");
                    in_table.Columns.Add("MAKTX");
                    in_table.Columns.Add("REQTY");
                    in_table.Columns.Add("REQDT");
                    in_table.Columns.Add("MEINS");
                    in_table.Columns.Add("HWPONU");
                    in_table.Columns.Add("FOXDES");
                    in_table.Columns.Add("HWDDES");
                    in_table.Columns.Add("REMARK");

                    in_table.Clear();
                    DataRow dr = in_table.NewRow();
                    dr["POSDT"] = DateTime.Parse(receive_date).ToString("yyyyMMdd");
                    dr["TSORT"] = task_no_type;
                    dr["TSORU"] = task_no_use;
                    dr["TSORL"] = task_no_level;
                    dr["WERKS"] = sap_factory;
                    dr["EXTWG"] = model;
                    dr["TSORNUM"] = v_task_no;
                    dr["MATNR"] = hh_item;
                    dr["HWDMAT"] = hw_item;
                    dr["MAKTX"] = "";
                    dr["REQTY"] = qty;
                    dr["REQDT"] = complete_date; 
                    dr["MEINS"] = "";
                    dr["HWPONU"] = "";
                    dr["TSORNUM"] = v_task_no;
                    if (!task_no.Equals(v_task_no))
                    {
                        dr["FOXDES"] = task_no;
                    }
                    else if (task_no.Substring(3, 1) == "S")
                    {
                        dr["FOXDES"] = task_no.Substring(0, 3) + "6" + task_no.Substring(4, 7);
                    }
                    else
                    {
                        dr["FOXDES"] = "SFCUPLOAD";
                    }
                    dr["HWDDES"] = "";
                    if (main_wo_flag == "Y")
                    {
                        dr["REMARK"] = "1";
                    }
                    else
                    {
                        dr["REMARK"] = "";
                    }
                    in_table.Rows.Add(dr);

                    zcpp_nsbg_0121.SetValue(in_table);
                    zcpp_nsbg_0121.CallRFC();
                    out_table = zcpp_nsbg_0121.GetTableValue("OUT_TAB");
                    result = out_table.Rows[0]["FLAG"].ToString();
                    message = out_table.Rows[0]["MESS"].ToString();
                    if (result == "S")
                    {
                        SFCDB.ORM.Updateable<R_7B5_WO_TEMP>().UpdateColumns(r => new R_7B5_WO_TEMP { ZNP195_MESSAGE = message, ZNP195_FLAG = "Y" })
                            .Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ExecuteCommand();
                        returnMessage = $@"上傳 ZNP195 成功 ! {message}";
                    }
                    else
                    {
                        SFCDB.ORM.Updateable<R_7B5_WO_TEMP>().UpdateColumns(r => new R_7B5_WO_TEMP { ZNP195_MESSAGE = message })
                           .Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ExecuteCommand();
                        returnMessage = $@"上傳 ZNP195 失敗 ! {message}";                        
                    }
                }
                #endregion              
                
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);            
                bool bCreatWOPivilege = TCUP.CheckpPivilegeByName(SFCDB, "CREAT_WO", this.LoginUser.EMP_NO);
                string sql = "";

                var t = DateTime.Parse(receive_date).ToString("MM/dd/yyyy hh:mm:ss");

                DataTable dataTable = new DataTable();
                if (!bCreatWOPivilege)
                {
                    //throw new Exception(returnMessage + "此帳號沒有開工單權限!");
                    returnMessage = returnMessage + "此帳號沒有開工單權限!";
                }
                else
                {
                    if (string.IsNullOrEmpty(suggest_qty) || suggest_qty == "0")
                    {
                        throw new Exception(returnMessage + ",SUGGEST QTY 不能為0 !");
                    }
                    if (znp_195_flag == "N" || plan_flag == "N" || po_flag == "N" || cancel_flag == "Y")
                    {
                        throw new Exception(returnMessage + "此任務令狀態不允許開工單!");
                    }
                    if (Convert.ToInt32(suggest_qty) + Convert.ToInt32(creat_wo_qty) > Convert.ToInt32(qty))
                    {
                        throw new Exception(returnMessage + "新開工單數量+已開工單數量>任務令數量!");
                    }

                    sql = $@"select * from r_7b5_xml_t where LASTEDITDT<TO_DATE('2015/08/08 00:00:00','YYYY/MM/DD HH24:MI:SS') AND TASK_NO='{task_no}'";
                    dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dataTable.Rows.Count > 0)
                    {
                        throw new Exception(returnMessage + "8/8號之前接受的任務令不允許開工單!!");
                    }
                    sql = $@"select * from r_7b5_xml_t where TASK_NO='{task_no}' and TASK_CHANGE_NO is not null and (TASK_CHANGE_CONFIRM IS NULL OR TASK_CHANGE_CONFIRM<>'Y')";
                    dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dataTable.Rows.Count > 0)
                    {
                        throw new Exception(returnMessage + "該任務令有技改未確認,不能開工單!!");
                    }
                    int i = hh_item.IndexOf("-") + 1;
                    i = i == -1 ? 0 :i;
                    string current = hh_item.Substring(i, 1);
                    //string next = C7B5Function.Todec36(Convert.ToInt32(current));
                    string next = C7B5Function.Todes36(C7B5Function.Todec36(current) + 1);
                    string hh_item_next = hh_item.Replace("-" + hh_item.Substring(i, 1), "-" + next.ToString());
                    sql = $@"select * from c_sku where skuno='{hh_item_next}'";
                    dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dataTable.Rows.Count > 0)
                    {
                        throw new Exception(returnMessage + "該HH料號有升級版本, 不允許開工單!");
                    }
                    sql = $@"SELECT * FROM r_7b5_wo a WHERE v_task_no = '{v_task_no}' and HH_ITEM='{hh_item}' and sap_wo is not null and DELETE_FLAG<>'Y'  AND NOT EXISTS (SELECT * FROM r_wo_base b WHERE a.sap_wo = b.workorderno)";
                    dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dataTable.Rows.Count > 0)
                    {
                        throw new Exception(returnMessage + "SAP存在還沒有DOWN到SFC的工單,不允許再開新工單!");
                    }
                    //sql = $@"SELECT * FROM c_sku_vir  WHERE skuno = '{hh_item}' ";
                    sql = $@"SELECT * FROM c_sku_7b5_config  WHERE skuno = '{hh_item}' and type='VIR'";
                    dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dataTable.Rows.Count > 0)
                    {
                        throw new Exception(returnMessage + "虛擬料號不允許再開新工單!");
                    }

                    #region Call SAP 開工單
                    ZCPP_NSBG_0123 zcpp_nsbg_0123 = new ZCPP_NSBG_0123(BU);
                    DataTable in_table_wo = new DataTable();
                    DataTable out_table_wo = new DataTable();
                    in_table_wo.Columns.Add("WERKS");
                    in_table_wo.Columns.Add("AUART");
                    in_table_wo.Columns.Add("MATNR");
                    in_table_wo.Columns.Add("GAMNG");
                    in_table_wo.Columns.Add("GSTRP");
                    in_table_wo.Columns.Add("GLTRP");
                    in_table_wo.Columns.Add("ABLAD");
                    in_table_wo.Clear();
                    DataRow dr_wo = in_table_wo.NewRow();
                    dr_wo["WERKS"] = sap_factory;
                    dr_wo["AUART"] = wo_type;
                    dr_wo["MATNR"] = hh_item;
                    dr_wo["GAMNG"] = suggest_qty;
                    dr_wo["GSTRP"] = start_date; //日期格式yyyyMMdd
                    dr_wo["GLTRP"] = complete_date; //日期格式yyyyMMdd
                    dr_wo["ABLAD"] = v_task_no;
                    in_table_wo.Rows.Add(dr_wo);

                    zcpp_nsbg_0123.SetValue(in_table_wo);
                    zcpp_nsbg_0123.CallRFC();
                    out_table_wo = zcpp_nsbg_0123.GetTableValue("OUT_TAB");
                    result = out_table_wo.Rows[0]["FLAG"].ToString();
                    message = out_table_wo.Rows[0]["MESSAGE"].ToString();
                    string wo = out_table_wo.Rows[0]["AUFNR"].ToString();
                    sql = $@"insert into r_7b5_wo (TASK_NO,V_TASK_NO,HH_ITEM,HW_ITEM,TASK_QTY,WO_QTY,SAP_WO,SAP_MESSAGE,LOAD_QTY,DELETE_FLAG,WO_STATUS,CREATE_TIME,CREATE_BY,FINISHQTY,RELEASEDDATE)
                     values ('{task_no}','{v_task_no}','{hh_item}','{hw_item}','{qty}','{suggest_qty}','{wo}','{message}',0,'N','Create',SYSDATE,'{LoginUser.EMP_NO}',0,TO_DATE('1900/01/01','YYYY/MM/DD') )";
                    SFCDB.ExecSQL(sql);
                    sql = $@" UPDATE r_7b5_wo_temp SET WO_REMARK='{message}' WHERE TASK_NO='{task_no}' AND HH_ITEM='{hh_item}' ";
                    SFCDB.ExecSQL(sql);
                    if (result == "S")
                    {
                        returnMessage = returnMessage + $@"{task_no},開立工單成功 ! " + wo;
                    }
                    else
                    {
                        returnMessage = returnMessage + $@"{task_no},開立工單失敗 ! " + message;
                    }
                    #endregion
                }
                string countMsg = "";
                C7B5Function.CountQtyByVTask(SFCDB, DBTYPE, v_task_no,ref countMsg);
                StationReturn.Message = "CountMsg:" + countMsg + ";" + returnMessage;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessagePara = null;
                StationReturn.MessageCode = null;
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                SFCDB.RollbackTrain();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// R_7B5_XML_T頁面加載獲取MODEL和查詢的起止日期
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetR7B5XMLTSeleteValue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                T_R_7B5_XML_T TRXT = new T_R_7B5_XML_T(SFCDB, DBTYPE);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();
                string dtSys = TRXT.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                dictionaryReturn.Add("DATE_FROM", dtSys + " 00:00:00");
                dictionaryReturn.Add("DATE_TO", dtSys + " 23:59:59");            
                var model = SFCDB.ORM.Queryable<R_7B5_XML_T>().Select(r => new { name = r.MODEL, value = r.MODEL }).ToList().Distinct().ToList();
                dictionaryReturn.Add("MODEL", model);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        ///  R_7B5_XML_T頁面搜索事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5XMLTList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = " select EMS_CODE, TASK_NO, MODEL,ITEM, ROHS, to_char(QTY) QTY,PLAN_FLAG, PLAN_QTY,PO_FLAG, CANCEL_FLAG, START_DATE, COMPLETE_DATE,"
                        + " TASK_NOREMARK, COMPONENT, COMPONENT_VERSION, COMPONENT_QTY, PUBLISH_TIME, TRANSFER_FLAG, LASTEDITDT, LASTEDITBY, CREATE_WO_FLAG,SUPPLY_TYPE,"
                        + " ITEM_VERSION, DESCRIPTION, PRODUCT_LINE, CATEGORY,'' REMARK,TASK_CHANGE_NO,TASK_CHANGE_CONFIRM from r_7b5_xml_t where 1 = 1 ";

                string task_no = Data["TASK_NO"].ToString();
                string hw_item = Data["HW_PN"].ToString();
                string model = Data["MODEL"] == null ? "" : Data["MODEL"].ToString();
                string plan_flag = Data["PLAN_FLAG"] == null ? "" : Data["PLAN_FLAG"].ToString();
                string cancel_flag = Data["CANCEL_FLAG"] == null ? "" : Data["CANCEL_FLAG"].ToString();
                string po_flag = Data["PO_FLAG"] == null ? "" : Data["PO_FLAG"].ToString();
                string date_from = Data["DATE_FROM"].ToString();
                string date_to = Data["DATE_TO"].ToString();              

                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND TASK_NO = '{task_no}' ";
                }
                if (!string.IsNullOrEmpty(hw_item))
                {
                    sql = sql + $@" AND ITEM = '{hw_item}' ";
                }
               
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    sql = sql + $@" AND  LASTEDITDT  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{date_to}','YYYY/MM/DD HH24:MI:SS') ";
                }
                if (!string.IsNullOrEmpty(model))
                {
                    sql = sql + $@" AND model = '{model}' ";
                }
                if (!string.IsNullOrEmpty(plan_flag))
                {
                    sql = sql + $@" AND PLAN_FLAG = '{plan_flag}' ";
                }
                if (!string.IsNullOrEmpty(po_flag))
                {
                    sql = sql + $@"  AND po_flag = '{po_flag}' ";
                }
                if (!string.IsNullOrEmpty(cancel_flag))
                {
                    sql = sql + $@" AND CANCEL_FLAG = '{cancel_flag}' ";
                }
              
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_XML_T頁面SplitQty事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void R7B5XMLTSplitQty(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            SFCDB.BeginTrain();
            try
            {
                string sql = "";

                string task_no = Data["TASK_NO"] == null ? "" : Data["TASK_NO"].ToString();
                string task_qty = Data["QTY"] == null ? "" : Data["QTY"].ToString();
                string split_qty = Data["SPLIT_QTY"] == null ? "" : Data["SPLIT_QTY"].ToString();
                if (string.IsNullOrEmpty(split_qty))
                {
                    throw new Exception("SPLIT QTY不能為空!");
                }
                if (Convert.ToDecimal(split_qty) < 0 || Convert.ToDecimal(split_qty) > Convert.ToDecimal(task_qty))
                {
                    throw new Exception("輸入的SPLIT QTY無效!");
                }

                sql = $@"select * from r_7b5_wo_temp where task_no='{task_no}' and QTY-CREAT_WO_QTY<{split_qty}";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("拆分數量+已開工單數量>任務令數量!");
                }

                string new_task_no = task_no.Substring(0, 3) + "S" + task_no.Substring(4, 7);
                int new_qty = Convert.ToInt32(task_qty) - Convert.ToInt32(split_qty);
                sql = $@"select * from r_7b5_xml_t where task_no='{new_task_no}'";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("此任務令已拆分過,不允許再次拆分!");
                }

                string item = SFCDB.ExecuteDataTable($@"select * from r_7b5_xml_t where task_no='{task_no}'", CommandType.Text, null).Rows[0]["ITEM"].ToString();
                sql = $@"select * from c_sku where skuno like '%{item}%' and rownum=1";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows[0]["SKU_TYPE"].ToString().Trim() == "PCBA")
                {
                    throw new Exception("PCBA的任務令不允許拆分!");
                }

                sql = $@"select a.task_no,a.ITEM,a.PLAN_QTY,to_char(PLAN_DT,'YYYY/MM/DD') from r_7b5_plan_task a, R_7B5_PLAN b where a.task_no='{task_no}'and a.cancel_flag='N' AND a.PLAN_DT>=b.LASTEDITDT and a.item=b.item";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql = " UPDATE r_7b5_plan SET day1 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt THEN day1 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day1 END,";
                    sql = sql + "day2 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+1 THEN day2 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day2 END,";
                    sql = sql + "day3 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+2 THEN day3 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day3 END,";
                    sql = sql + "day4 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+3 THEN day4 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day4 END,";
                    sql = sql + "day5 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+4 THEN day5 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day5 END,";
                    sql = sql + "day6 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+5 THEN day6 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day6 END,";
                    sql = sql + "day7 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+6 THEN day7 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day7 END,";
                    sql = sql + "day8 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+7 THEN day8 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day8 END,";
                    sql = sql + "day9 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+8 THEN day9 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day9 END,";
                    sql = sql + "day10 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+9 THEN day10 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day10 END,";
                    sql = sql + "day11 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+10 THEN day11 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day11 END,";
                    sql = sql + "day12 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+11 THEN day12 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day12 END,";
                    sql = sql + "day13 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+12 THEN day13 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day13 END,";
                    sql = sql + "day14 = CASE WHEN TO_DATE ('" + dt.Rows[0][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+13 THEN day14 + " + dt.Rows[0][2].ToString().Trim() + "  ELSE day14 END WHERE item = '" + dt.Rows[0][1].ToString() + "' ";
                    SFCDB.ExecSQL(sql);
                }

                sql = $@"update r_7b5_xml_t set qty='{new_qty}',COMPONENT_QTY='{new_qty}', plan_qty='0',plan_flag='N' where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                sql = $@"update r_7b5_plan_task set cancel_flag='Y',LASTEDITBY='HAND2' where task_no='{task_no}' AND cancel_flag='N'";
                SFCDB.ExecSQL(sql);

                sql = $@"insert into r_7b5_xml_t select EMS_CODE,'{new_task_no}',ITEM, SUPPLY_TYPE,PO_SUPPLY_INFORMATION,CHANGE_INFORMATION, ITEM_VERSION, ROHS, {split_qty},
                        RELEASE_DATE, START_DATE, COMPLETE_DATE, TASK_NOREMARK, COMPONENT, COMPONENT_VERSION,{split_qty},COMPONENT_REMARK, PUBLISH_TIME, 'N',
                        LASTEDITDT, LASTEDITBY, CREATE_WO_FLAG,LOT_NO, DESCRIPTION, PRODUCT_LINE, MODEL, CATEGORY, PLAN_FLAG, PLAN_QTY, PO_FLAG, CANCEL_FLAG,TASK_CHANGE_NO,''
                        from r_7b5_xml_t where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                sql = $@"update r_7b5_wo_temp set QTY='{new_qty}', ZNP195_FLAG='N' where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                sql = $@"update R_7B5_SHIP set TASK_QTY='{new_qty}' where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                //sql = $@"update r_7b5_wo set QTY='{new_qty}' where task_no='{task_no}'";
                //SFCDB.ExecSQL(sql);

                sql = $@"insert into R_7B5_UPDATE_LIST(TYPE,TASK_NO,QTY,LASTEDITBY) values('SPLIT_QTY','{task_no}',{split_qty},'{LoginUser.EMP_NO}')";
                SFCDB.ExecSQL(sql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Message = "拆分任務令OK!";
                StationReturn.MessagePara = null;
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_XML_T頁面Update事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void R7B5XMLTUpdate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            SFCDB.BeginTrain();
            try
            {
                string sql = "";
                string task_no = Data["TASK_NO"] == null ? "" : Data["TASK_NO"].ToString();
                string qty = Data["QTY"] == null ? "" : Data["QTY"].ToString();
                string plan_flag = Data["PLAN_FLAG"] == null ? "" : Data["PLAN_FLAG"].ToString();
                string po_flag = Data["PO_FLAG"] == null ? "" : Data["PO_FLAG"].ToString();
                string cancel_flag = Data["CANCEL_FLAG"] == null ? "" : Data["CANCEL_FLAG"].ToString();
                string remark = Data["REMARK"] == null ? "" : Data["REMARK"].ToString();
                string task_change_no = Data["TASK_CHANGE_NO"] == null ? "" : Data["TASK_CHANGE_NO"].ToString();
                string task_change_confrim = Data["TASK_CHANGE_CONFIRM"] == null ? "" : Data["TASK_CHANGE_CONFIRM"].ToString();

                sql = $@"select qty,Plan_Flag,Po_Flag,Cancel_Flag from r_7b5_xml_t where task_no='{task_no}'";

                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                string old_qty = dt.Rows[0][0].ToString().Trim();
                string old_plan_flag = dt.Rows[0][1].ToString().Trim();
                string old_po_flag = dt.Rows[0][2].ToString().Trim();
                string old_cancel_flag = dt.Rows[0][3].ToString().Trim();

                if (cancel_flag != "Y" && cancel_flag != "N" )
                {
                    throw new Exception("CANCEL_FLAG只能修改為Y或者N!");
                }


                if (po_flag != "Y1" && po_flag != "N" && po_flag != old_po_flag)
                {
                    throw new Exception("PO_FLAG只能修改為Y1或者N!");
                }

                if (string.IsNullOrEmpty(remark))
                {
                    throw new Exception("修改數據必須填寫REMARK!");
                }
          
                if (plan_flag != "Y1" && plan_flag != "N" && plan_flag != old_plan_flag)
                {
                    throw new Exception("Plan_Flag只能修改為Y1或者N !");
                }
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);
                //bool bUpdatePoFlagPivilege = TCUP.CheckpPivilegeByName(SFCDB, "UPDATE_PO_FLAG", this.LoginUser.EMP_NO);
                //if (!bUpdatePoFlagPivilege && po_flag != old_po_flag)
                //{
                //    throw new Exception("此帳號沒有修改PO_FLAG的權限!");
                //}
                //bool bUpdatePlanFlagPivilege = TCUP.CheckpPivilegeByName(SFCDB, "UPDATE_PLAN_FLAG", this.LoginUser.EMP_NO);
                //if (!bUpdatePlanFlagPivilege && plan_flag != old_plan_flag)
                //{
                //    throw new Exception("此帳號沒有修改PLAN_FLAG的權限!");
                //}
                //bool bUpdateCancelFlagPivilege = TCUP.CheckpPivilegeByName(SFCDB, "UPDATE_CANCEL_FLAG", this.LoginUser.EMP_NO);
                //if (!bUpdateCancelFlagPivilege && cancel_flag != old_cancel_flag)
                //{
                //    throw new Exception("此帳號沒有修改CANCEL_FLAG的權限!");
                //}
                //bool bUpdateQtyPivilege = TCUP.CheckpPivilegeByName(SFCDB, "UPDATE_QTY", this.LoginUser.EMP_NO);
                //if (!bUpdateCancelFlagPivilege && qty != old_qty)
                //{
                //    throw new Exception("此帳號沒有修改QTY的權限!!");
                //}

                //只判斷是否有修改權限就行
                bool bUpdatePivilege = TCUP.CheckpPivilegeByName(SFCDB, "R_7B5_XML_T_UPDATE", this.LoginUser.EMP_NO);
                if (!bUpdatePivilege)
                {
                    throw new Exception("此帳號沒有修改權限!!");
                }


                sql = $@"select * from r_7b5_wo_temp where task_no='{task_no}' and CREAT_WO_QTY>{qty}";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0 )
                {
                    throw new Exception("新的任務令數量不允許小於任務令已開工單數量!");
                }

                sql = $@"select * from r_7b5_wo where task_no='{task_no}' and delete_flag='N' and sap_wo is not null";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0 && cancel_flag == "Y" && old_cancel_flag == "N")
                {
                    throw new Exception("此任務令存在已經開立的工單,請將已開立的工單刪除再做CANCEL動作!");
                }               
                if (dt.Rows.Count > 0 && task_change_no != "")
                {
                    throw new Exception("此任務令存在已經開立的工單,不允許備註技改!");
                }
                if ((plan_flag == "N" && (old_plan_flag == "Y" || old_plan_flag == "Y1")) || (cancel_flag == "Y" && old_cancel_flag == "N"))
                {
                    sql = $@"select a.task_no,a.ITEM,a.PLAN_QTY,to_char(PLAN_DT,'YYYY/MM/DD') from r_7b5_plan_task a, R_7B5_PLAN b 
                                where a.task_no='{task_no}'and a.cancel_flag='N' AND a.PLAN_DT>=b.LASTEDITDT and a.item=b.item";
                    dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sql = "UPDATE r_7b5_plan SET day1 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt THEN day1 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day1 END,";
                        sql = sql + "day2 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+1 THEN day2 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day2 END,";
                        sql = sql + "day3 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+2 THEN day3 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day3 END,";
                        sql = sql + "day4 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+3 THEN day4 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day4 END,";
                        sql = sql + "day5 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+4 THEN day5 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day5 END,";
                        sql = sql + "day6 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+5 THEN day6 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day6 END,";
                        sql = sql + "day7 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+6 THEN day7 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day7 END,";
                        sql = sql + "day8 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+7 THEN day8 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day8 END,";
                        sql = sql + "day9 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+8 THEN day9 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day9 END,";
                        sql = sql + "day10 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+9 THEN day10 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day10 END,";
                        sql = sql + "day11 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+10 THEN day11 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day11 END,";
                        sql = sql + "day12 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+11 THEN day12 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day12 END,";
                        sql = sql + "day13 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+12 THEN day13 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day13 END,";
                        sql = sql + "day14 = CASE WHEN TO_DATE ('" + dt.Rows[i][3].ToString().Trim() + "', 'YYYY/MM/DD') = lasteditdt+13 THEN day14 + " + dt.Rows[i][2].ToString().Trim() + "  ELSE day14 END WHERE item = '" + dt.Rows[i][1].ToString().Trim() + "' ";

                        SFCDB.ExecSQL(sql);
                    }
                    sql = $@"update r_7b5_xml_t set plan_qty='0',plan_flag='N' where task_no='{task_no}'";
                    SFCDB.ExecSQL(sql);

                    sql = $@"update r_7b5_plan_task set cancel_flag = 'Y', LASTEDITBY = 'HAND1' where task_no = '{task_no}' AND cancel_flag = 'N'";
                    SFCDB.ExecSQL(sql);

                    if (cancel_flag == "Y" && old_cancel_flag == "N")
                    {
                        sql = $@"delete r_7b5_ship where task_no='{task_no}'";
                        SFCDB.ExecSQL(sql);

                        sql = $@"delete r_7b5_wo_temp where task_no='{task_no}'";
                        SFCDB.ExecSQL(sql);

                        sql = $@"update r_7b5_xml_t set TRANSFER_FLAG = 'N' where task_no = '{task_no}'";
                        SFCDB.ExecSQL(sql);
                    }
                }
              
                sql = $@"insert into R_7B5_UPDATE_LIST(TYPE,TASK_NO,PLAN_FLAG,QTY,PO_FLAG,CANCEL_FLAG,REMARK,DATA1,DATA2,DATA3,LASTEDITBY,LASTEDITDT)
                        SELECT 'R7B5XMLT_UPDATE' as TYPE,TASK_NO,PLAN_FLAG,QTY,PO_FLAG,CANCEL_FLAG,'{remark}' as REMARK,
                        '{qty}' as DATA1,TASK_CHANGE_CONFIRM as DATA2,'{task_change_confrim}' as data3,'{LoginUser.EMP_NO}' as LASTEDITBY,sysdate as LASTEDITDT from r_7b5_xml_t where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                sql = $@"update r_7b5_xml_t set qty='{qty}',plan_flag='{plan_flag}',po_flag='{po_flag}',cancel_flag='{cancel_flag}',TASK_CHANGE_NO='{task_change_no}',TASK_CHANGE_CONFIRM='{task_change_confrim}' where task_no='{task_no}'";
                SFCDB.ExecSQL(sql);

                if (qty != old_qty)
                {
                    sql = $@"update r_7b5_wo_temp set ZNP195_FLAG='N',QTY='{qty}' where task_no='{task_no}'";
                    SFCDB.ExecSQL(sql);

                    sql = $@"update R_7B5_SHIP set TASK_QTY='{qty}' where task_no='{task_no}'";
                    SFCDB.ExecSQL(sql);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Message = "數據修改OK!";
                StationReturn.MessagePara = null;
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_XML_T頁面Upload事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Upload7B5XMLTExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "TASK_NO", "MODEL", "ITEM", "QTY", "START_DATE", "COMPLETE_DATE", "PUBLISH_TIME", "DESCRIPTION" };
                string errTitle = "";
                string task_no = "", model = "", item = "", qty = "", start_date = "", complete_date = "", publish_time = "", description = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    //throw new Exception($@"上傳的文件內容為空!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111744"));
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception($@"上傳的文件內容有誤,必須包含{errTitle}列!");
                }
                string sql = "";
                string result = "";
                string message = "";
                string has_upload = "";
                string upload_fail = "";
                DataTable dt = new DataTable();                
                #region 写入数据库
                SFCDB.BeginTrain();
                for (int i = 0; i < array.Count; i++)
                {
                    task_no = array[i]["TASK_NO"].ToString().ToUpper().Trim();
                    model = array[i]["MODEL"].ToString().ToUpper().Trim();
                    item = array[i]["ITEM"].ToString().ToUpper().Trim();
                    qty = array[i]["QTY"].ToString().ToUpper().Trim();
                    start_date = array[i]["START_DATE"].ToString().ToUpper().Trim();
                    complete_date = array[i]["COMPLETE_DATE"].ToString().ToUpper().Trim();
                    publish_time = array[i]["PUBLISH_TIME"].ToString().ToUpper().Trim();
                    description = array[i]["DESCRIPTION"].ToString().ToUpper().Trim();
                    if (!string.IsNullOrEmpty(task_no))
                    {
                        sql = $@"select * from  R_7B5_XML_T WHERE TASK_NO='{task_no}' ";
                        dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        if (dt.Rows.Count>0)
                        {
                            has_upload = has_upload + task_no + ",";
                            continue;
                        }
                        sql = $@"Insert into R_7B5_XML_T (EMS_CODE, TASK_NO, ITEM, SUPPLY_TYPE, ITEM_VERSION, ROHS, QTY, START_DATE, COMPLETE_DATE, TASK_NOREMARK,
                                    COMPONENT, COMPONENT_VERSION, COMPONENT_QTY, PUBLISH_TIME, LASTEDITDT, LASTEDITBY, DESCRIPTION, PRODUCT_LINE, MODEL, CATEGORY,CANCEL_FLAG)
                                Values ('Z000J2', '{task_no}', '{item}', 'AI', '0', 'Y', {qty}, TO_TIMESTAMP ('{start_date}', 'YYYY/MM/DD HH24:MI:SS '), TO_TIMESTAMP ('{complete_date}', 'YYYY/MM/DD HH24:MI:SS '), 'ROHS',
                                    '{item}', '0', {qty},  TO_TIMESTAMP ('{publish_time}', 'YYYY/MM/DD HH24:MI:SS '), sysdate, '{LoginUser.EMP_NO}', '{description}', '移動寬帶與家庭終端','{model}', 'AI','N')";
                        result = SFCDB.ExecSQL(sql);
                        if (result != "1")
                        {
                            upload_fail = upload_fail + task_no + ",";
                        }
                    }
                }
                SFCDB.CommitTrain();

                if (has_upload != "")
                {                    
                    message = message + $@"以下任務令: {has_upload} 已上傳過,請勿重複上傳!";
                }
                if (upload_fail != "")
                {
                    message = message + $@"以下任務令: {upload_fail} 上傳失敗!";
                }
                if (message == "")
                {
                    message = "任務令全部上傳成功!";
                }
                #endregion                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = message;
                StationReturn.MessageCode = null;
                StationReturn.Data = null;               
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_TASK_OVERDUE頁面加載事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void RTaskOverdueLoadingAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string sql = "select * from r_7b5_wo where sap_wo is not null and DELETE_FLAG='N' and FINISHQTY<WO_QTY and create_time>sysdate-30";
                DataTable dataTable = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dataTable.Rows.Count > 0)
                {                    
                    string wo = "";
                    string sap_qty = "";                    
                    ZCPP_NSBG_0027 zcpp_nsbg_0027 = new ZCPP_NSBG_0027(BU);
                    DataTable out_table = new DataTable();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        wo = dataTable.Rows[i]["SAP_WO"].ToString().ToUpper().Trim();
                        zcpp_nsbg_0027.SetValue("P_FLAG", "");
                        zcpp_nsbg_0027.SetValue("P_MESSAGE", "");
                        zcpp_nsbg_0027.SetValue("AUFNR", wo);
                        zcpp_nsbg_0027.CallRFC();
                        out_table= zcpp_nsbg_0027.GetTableValue("ZSFC23C");
                        sap_qty = out_table.Rows[0]["LMNGA"].ToString();                 
                        sql = "UPDATE r_7b5_wo SET  FINISHQTY=" + sap_qty + " WHERE sap_wo='" + wo + "'";
                        SFCDB.ExecSQL(sql);
                    }
                }

                sql = $@"update r_7b5_wo a set a.wo_qty=( select b.WORKORDER_QTY from r_wo_base b where a.SAP_WO=b.workorderno) where a.sap_wo is not null and a.DELETE_FLAG='N' AND a.WO_STATUS='release' 
                            and a.wo_qty<>(select c.WORKORDER_QTY from r_wo_base c where a.SAP_WO=c.workorderno) and a.create_time>sysdate-30 ";
                SFCDB.ExecSQL(sql);

                sql = $@"UPDATE r_7b5_wo A SET RELEASEDDATE = (SELECT TO_DATE(TO_CHAR(RELEASEDDATE, 'yyyy/mm/dd'), 'yyyy/mm/dd') FROM r_wo_base B WHERE A.SAP_WO = B.WORKORDERNO ) 
                            WHERE A.SAP_WO IS NOT NULL AND EXISTS(SELECT* FROM r_wo_base C WHERE A.SAP_WO= C.WORKORDERNO)
                            and a.RELEASEDDATE <> (SELECT TO_DATE(TO_CHAR(RELEASEDDATE, 'yyyy/mm/dd'), 'yyyy/mm/dd') FROM r_wo_base c WHERE A.SAP_WO = c.WORKORDERNO ) ";
                SFCDB.ExecSQL(sql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = "";
                StationReturn.MessagePara = null;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_TASK_OVERDUE頁面獲取日期
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void RTaskOverdueGetDate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "select to_char(sysdate-20,'YYYY-MM-DD') as DATE_FROM,to_char(sysdate,'YYYY-MM-DD') as DATE_TO from dual";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();               
                dictionaryReturn.Add("DATE_FROM", dt.Rows[0]["DATE_FROM"].ToString());
                dictionaryReturn.Add("DATE_TO", dt.Rows[0]["DATE_TO"].ToString());
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// R_TASK_OVERDUE頁面查詢事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void RTaskOverdueSearchAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = $@"select * from (SELECT to_char(m.receive_date,'yyyy/mm/dd') receive_date,m.TASK_NO_TYPE, m.model, m.v_task_no,m.hh_item, m.hw_item, m.task_qty, m.task_qty-m.task_creat_qty task_creat_dif , m.task_stockin_qty, m.task_stockin_qty - m.task_qty AS task_difqty, to_char(m.plan_dt,'yyyy/mm/dd') plan_dt,to_char(m.start_date,'yyyy/mm/dd') start_date ,plan_dt - m.start_date dif_start_dt, m.sap_wo, m.wo_qty, m.finishqty, m.finishqty - m.wo_qty wo_dif_qty, to_char(m.releaseddate,'yyyy/mm/dd') releaseddate , m.dif_rel_dt 
                            FROM (SELECT b.receive_date,b.TASK_NO_TYPE, b.model, a.v_task_no,a.hh_item, a.hw_item, a.task_qty, (SELECT SUM (finishqty) FROM r_7b5_wo e WHERE e.v_task_no = a.v_task_no AND e.hh_item = a.hh_item) task_stockin_qty,(SELECT SUM (WO_QTY) FROM r_7b5_wo e WHERE e.v_task_no = a.v_task_no AND e.hh_item = a.hh_item and sap_wo is not null and delete_flag='N') task_creat_qty,(select max(plan_dt) FROM r_7b5_plan_task d WHERE d.task_no = a.task_no )  plan_dt, f.start_date, a.sap_wo, a.wo_qty, a.finishqty, a.releaseddate, CASE a.releaseddate WHEN TO_DATE ('1900/01/01', 'YYYY/MM/DD') THEN 0 ELSE   TO_DATE (TO_CHAR ((SYSDATE), 'yyyy/mm/dd'),'yyyy/mm/dd' ) - a.releaseddate END AS dif_rel_dt 
                            FROM r_7b5_wo a, r_7b5_wo_temp b, r_wo_base c,r_7b5_xml_t f WHERE a.sap_wo = c.workorderno(+) and f.task_no=a.task_no AND F.CANCEL_FLAG='N' AND a.task_no = b.task_no AND a.hh_item = b.hh_item AND a.sap_wo IS NOT NULL AND a.delete_flag = 'N') m WHERE m.task_stockin_qty < m.task_qty union 
                            select to_char(m.receive_date,'yyyy/mm/dd') receive_date,m.TASK_NO_TYPE, m.model, m.v_task_no,m.hh_item, m.hw_item, m.task_qty, m.task_qty-m.task_creat_qty task_creat_dif, m.task_stockin_qty, m.task_stockin_qty - m.task_qty AS difqty,to_char(m.plan_dt,'yyyy/mm/dd') plan_dt, to_char(m.start_date,'yyyy/mm/dd') start_date,plan_dt - m.start_date difdt,null,null,null,null,null,null 
                            from (select a.RECEIVE_DATE,a.TASK_NO_TYPE,a.MODEL,a.V_TASK_NO,a.hh_item,a.hw_ITEM,a.QTY task_qty,0 task_creat_qty,0 task_stockin_qty,(select max(plan_dt) FROM r_7b5_plan_task d WHERE d.task_no = a.task_no )  plan_dt,b.start_date 
                            from R_7B5_WO_TEMP a,r_7b5_xml_t b  where a.task_no=b.task_no AND B.CANCEL_FLAG='N' and not exists(select * from R_7B5_WO b where a.task_no=b.task_no and a.HH_ITEM=b.HH_ITEM and sap_wo is not null )) m ) x where 1=1 ";

                string task_no = Data["TASK_NO"] == null ? "" : Data["TASK_NO"].ToString();
                string hw_item = Data["TIME"] == null ? "" : Data["TIME"].ToString();
                string task_no_type = Data["TASK_NO_TYPE"] == null ? "" : Data["TASK_NO_TYPE"].ToString();       
                string date_from = Data["DATE_FROM"] == null ? "" : Data["DATE_FROM"].ToString();
                string date_to = Data["DATE_TO"] == null ? "" : Data["DATE_TO"].ToString();

                if (!string.IsNullOrEmpty(task_no))
                {
                    sql = sql + $@" AND  x.V_TASK_NO = '{task_no}' ";
                }
                if (!string.IsNullOrEmpty(hw_item))
                {
                    sql = sql + $@" AND  x.hw_item = '{hw_item}' ";
                }
                if (!string.IsNullOrEmpty(task_no_type))
                {
                    sql = sql + $@" AND  x.task_no_type = '{task_no_type}' ";
                }
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    sql = sql + $@" AND to_date(x.receive_date,'YYYY/MM/DD')  BETWEEN TO_DATE('{date_from}','YYYY/MM/DD') AND TO_DATE('{date_to}','YYYY/MM/DD')  ";
                }
                sql = sql + " order by RECEIVE_DATE,V_TASK_NO ";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// SKU_7B5_CONFIG頁面獲取TYPE
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetSKU7B5ConfigType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                T_C_SKU_7B5_CONFIG TCSC = new T_C_SKU_7B5_CONFIG(SFCDB, DBTYPE);
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();
                var model = TCSC.GetListByType(SFCDB, "CONFIG_TYPE").Select(r => new { name = r.DATA1, value = r.DATA1 }).ToList().Distinct().ToList();
                dictionaryReturn.Add("TYPE", model);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// SKU_7B5_CONFIG頁面獲取數據LIST
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetSKU7B5ConfigList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (Data["TYPE"] == null)
                {
                    throw new Exception("Please Input Type!");
                }
                string type = Data["TYPE"].ToString().Trim();
                T_C_SKU_7B5_CONFIG TCSC = new T_C_SKU_7B5_CONFIG(SFCDB, DBTYPE);
                List<C_SKU_7B5_CONFIG> list = TCSC.GetListByType(SFCDB, type);

                if (type.Equals("UPD"))
                {
                    StationReturn.Data = list.Select(r => new { r.TYPE,r.SKUNO, r.UPD, r.LASTEDITBY, r.LASTEDITDT }).ToList().Distinct().ToList();
                }
                else if (type.Equals("VIR"))
                {
                    StationReturn.Data = list.Select(r => new { r.TYPE,r.SKUNO,r.VIR_FLAG, r.LASTEDITBY, r.LASTEDITDT }).ToList().Distinct().ToList();
                }
                else
                {
                    StationReturn.Data = list;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";                
                StationReturn.MessagePara.Add("null");
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// SKU_7B5_CONFIG頁面添加
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void SKU7B5ConfigAdd(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;            
            try
            {
                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString();
                string skuno = Data["SKUNO"] == null ? "" : Data["SKUNO"].ToString().ToUpper().Trim();
                string upd = Data["UPD"] == null ? "" : Data["UPD"].ToString();
                string vir_flag = Data["VIR_FLAG"] == null ? "" : Data["VIR_FLAG"].ToString();
                T_C_SKU_7B5_CONFIG TCSC = new T_C_SKU_7B5_CONFIG(SFCDB, DBTYPE);
                if (string.IsNullOrEmpty(type))
                {
                    throw new Exception("Please Input Type!");
                }
                if (string.IsNullOrEmpty(skuno))
                {
                    throw new Exception("Please Input Skuno!");
                }
                T_C_SKU TCS = new T_C_SKU(SFCDB, DBTYPE);
                if (!TCS.CheckSku(skuno, SFCDB))
                {
                    throw new Exception(skuno + ",該料號在SFC中不存在!");
                }
                List<C_SKU_7B5_CONFIG> list = TCSC.GetListByType(SFCDB, type);
                C_SKU_7B5_CONFIG configModel = new C_SKU_7B5_CONFIG();
                configModel.TYPE = type;
                configModel.SKUNO = skuno;
                bool bExist = false;
                if (type.Equals("UPD"))
                {
                    double dUPD;
                    if (string.IsNullOrEmpty(upd))
                    {
                        throw new Exception("Please Input UPD!");
                    }
                    if (!double.TryParse(upd, out dUPD))
                    {
                        throw new Exception("UPD為整數數字!");
                    }
                    bExist = list.Any(r => r.SKUNO == skuno && r.UPD != null);
                    if (bExist)
                    {
                        throw new Exception(skuno + ",該UPD LINK關係已存在!");
                    }
                    configModel.UPD = dUPD;
                }
                if (type.Equals("VIR"))
                {
                    if (string.IsNullOrEmpty(vir_flag))
                    {
                        throw new Exception("Please Input VIR_FLAG!");
                    }
                    bExist = list.Any(r => r.SKUNO == skuno && r.VIR_FLAG != "");
                    if (bExist)
                    {
                        throw new Exception(skuno + ",該VIR_FLAG LINK關係已存在!");
                    }
                    configModel.VIR_FLAG = vir_flag;
                }
                configModel.LASTEDITBY = LoginUser.EMP_NO;
                configModel.LASTEDITDT = TCSC.GetDBDateTime(SFCDB);
                int result = TCSC.SaveNewModel(SFCDB, configModel);
                if (result == 0)
                {
                    throw new Exception("添加失敗！");
                }
                StationReturn.Message = "添加成功！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessagePara = null;               
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);               
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// SKU_7B5_CONFIG頁面修改功能
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void SKU7B5ConfigEdit(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;            
            try
            {
                string old_type = Data["OLD"]["TYPE"] == null ? "" : Data["OLD"]["TYPE"].ToString();
                string old_skuno = Data["OLD"]["SKUNO"] == null ? "" : Data["OLD"]["SKUNO"].ToString();
                string old_upd = Data["OLD"]["UPD"] == null ? "" : Data["OLD"]["UPD"].ToString();
                string old_vir_flag = Data["OLD"]["VIR_FLAG"] == null ? "" : Data["OLD"]["VIR_FLAG"].ToString();
                string new_type = Data["NEW"]["TYPE"] == null ? "" : Data["NEW"]["TYPE"].ToString();
                string new_skuno = Data["NEW"]["SKUNO"] == null ? "" : Data["NEW"]["SKUNO"].ToString();
                string new_upd = Data["NEW"]["UPD"] == null ? "" : Data["NEW"]["UPD"].ToString();
                string new_vir_flag = Data["NEW"]["VIR_FLAG"] == null ? "" : Data["NEW"]["VIR_FLAG"].ToString();
                if (string.IsNullOrEmpty(new_type))
                {
                    throw new Exception("Please Input Type!");
                }
                if (string.IsNullOrEmpty(new_skuno))
                {
                    throw new Exception("Please Input Skuno!");
                }
                
                int result = 0;
                string sql = "", re = "";
                if (new_type.Equals("UPD"))
                {
                    if (string.IsNullOrEmpty(new_upd))
                    {
                        throw new Exception("Please Input UPD!");
                    }
                    double d_new_upd;
                    double d_old_upd;
                    if (!double.TryParse(new_upd, out d_new_upd))
                    {
                        throw new Exception("UPD為整數數字!");
                    }
                    if (!double.TryParse(old_upd, out d_old_upd))
                    {
                        throw new Exception("UPD為整數數字!");
                    }

                    sql = $@" update C_SKU_7B5_CONFIG set upd='{d_new_upd}',LASTEDITBY='{LoginUser.EMP_NO}',LASTEDITDT=sysdate where type='{old_type}' and skuno='{old_skuno}' and upd='{d_old_upd}'";
                    re = SFCDB.ExecSQL(sql);
                    result = Convert.ToInt32(re);
                    
                }
                else if (new_type.Equals("VIR") )
                {
                    if (string.IsNullOrEmpty(new_vir_flag))
                    {
                        throw new Exception("Please Input VIR_FLAG!");
                    }

                    sql = $@" update C_SKU_7B5_CONFIG set VIR_FLAG='{new_vir_flag}',LASTEDITBY='{LoginUser.EMP_NO}',LASTEDITDT=sysdate  where type='{old_type}' and skuno='{old_skuno}' and VIR_FLAG='{old_vir_flag}'";
                    re = SFCDB.ExecSQL(sql);
                    result = Convert.ToInt32(re);
                    //result = SFCDB.ORM.Updateable<C_SKU_7B5_CONFIG>().UpdateColumns(r => new C_SKU_7B5_CONFIG { VIR_FLAG = new_vir_flag })
                    //    .Where(r => r.TYPE == old_type && r.SKUNO == old_skuno && r.VIR_FLAG == old_vir_flag).ExecuteCommand();
                }
                if (result == 0)
                {
                    throw new Exception("修改失敗！");
                }
                StationReturn.Message = "修改成功！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessagePara = null;               
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);                
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// SKU_7B5_CONFIG頁面刪除功能
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void SKU7B5ConfigDelete(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;            
            try
            {
                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString();
                string skuno = Data["SKUNO"] == null ? "" : Data["SKUNO"].ToString();
                string upd = Data["UPD"] == null ? "" : Data["UPD"].ToString();
                string vir_flag = Data["VIR_FLAG"] == null ? "" : Data["VIR_FLAG"].ToString();
                int result = 0;
                if (type.Equals("UPD"))
                {
                    double d_upd;
                    if (!double.TryParse(upd, out d_upd))
                    {
                        throw new Exception("UPD為整數數字!");
                    }
                    result = SFCDB.ORM.Deleteable<C_SKU_7B5_CONFIG>().Where(r => r.TYPE == type && r.SKUNO == skuno && r.UPD == d_upd).ExecuteCommand();
                }
                else if (type.Equals("VIR"))
                {
                    result = SFCDB.ORM.Deleteable<C_SKU_7B5_CONFIG>().Where(r => r.TYPE == type && r.SKUNO == skuno && r.VIR_FLAG == vir_flag).ExecuteCommand();
                }
                if (result == 0)
                {
                    //throw new Exception("刪除失敗！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057"));
                }
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessagePara = null;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);               
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// R_7B5_PO頁面加載數據和查詢API
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Get7B5POList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "";
                string task_no = Data["TASK_NO"] == null ? "" : Data["TASK_NO"].ToString();

                if (string.IsNullOrEmpty(task_no))
                {
                    sql = "select * from r_7b5_po order by UPLOADDT DESC";
                }
                else
                {
                    sql = $@"select * from r_7b5_po where TASK_NO='{task_no}' OR ITEM='{task_no}'";
                }
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// R_7B5_PO頁面上傳Excel調用API
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Upload7B5POExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();            
            SFCDB.ThrowSqlExeception = true;
            try
            {
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "TASK_NO", "PO_NO", "ItemNum", "Qty", "SalesUnit", "PRICE", "Currency", "BaseQty", "PriceUnit" };
                string errTitle = "";
                string task_no = "", po_no = "", item_num = "", qty = "", sales_unit = "", price = "", currency = "", base_qty = "", price_unit = "";
                string data = Data["ExcelData"].ToString(); 
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    //throw new Exception($@"上傳的文件內容為空!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111744"));
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle,out errTitle);
                if (!hasErr)
                {                    
                    throw new Exception($@"上傳的文件內容有誤,必須包含{errTitle}列!");
                }
                string v_task_no = "";
                string sql = "";
                string result = "";
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                #region 写入数据库
                SFCDB.BeginTrain();
                for (int i = 0; i < array.Count; i++)
                {
                    task_no = array[i]["TASK_NO"].ToString().ToUpper().Trim();
                    po_no = array[i]["PO_NO"].ToString().ToUpper().Trim();
                    item_num = array[i]["ItemNum"].ToString().ToUpper().Trim();
                    qty = array[i]["Qty"].ToString().ToUpper().Trim();
                    sales_unit = array[i]["SalesUnit"].ToString().ToUpper().Trim();
                    price = array[i]["PRICE"].ToString().ToUpper().Trim();
                    currency = array[i]["Currency"].ToString().ToUpper().Trim();
                    base_qty = array[i]["BaseQty"].ToString().ToUpper().Trim();
                    price_unit = array[i]["PriceUnit"].ToString().ToUpper().Trim();
                    if (!string.IsNullOrEmpty(task_no))
                    {
                        v_task_no = task_no.Substring(0, 3) + "S" + task_no.Substring(4, 7);
                        if (task_no.Substring(0, 1) == "D" && po_no.Substring(0, 1) != "9" && po_no.Substring(0, 1) != "L")
                        {
                            throw new Exception("D開頭的任務令只能上傳9或L開頭的PO !");
                        }
                        if (task_no.Substring(0, 3) == "BSD" && po_no.Substring(0, 1) != "D" && po_no.Substring(0, 1) != "9")
                        {
                            throw new Exception("BSD開頭的任務令只能上傳D or 9 開頭的PO ! ");
                        }
                        if(task_no.Substring(0, 1) == "T" && po_no.Substring(0, 1) != "8")
                        {
                            throw new Exception("T開頭的任務令只能上傳8開頭的PO !");
                        }
                        if (task_no.Substring(0, 3) == "BST" && po_no.Substring(0, 1) != "M" && po_no.Substring(0, 1) != "8")
                        {
                            throw new Exception("BST開頭的任務令只能上傳M or 8開頭的PO ! ");
                        }
                        sql = $@"select sum(qty) qty  from  r_7b5_xml_t where task_no='{task_no}' or task_no='{v_task_no}'";
                        dt1 = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);

                        sql = $@"select *  from  r_7b5_xml_t where task_no='{task_no}' and item='{item_num}'";
                        dt2 = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        if (dt2.Rows.Count > 0 && qty.Replace(",", "") == dt1.Rows[0][0].ToString().Trim())
                        {
                            sql = $@"select *  from  r_7b5_po where task_no='{task_no}'";
                            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                            if (dt.Rows.Count > 0)
                            {
                                sql = $@"update r_7b5_po set po_no='{po_no}',QTY='{qty.Replace(",", "")}',SALES_UNIT='{sales_unit.Replace(",", "") }',
                                            PRICE='{price.Replace(",", "")}',CURRENCY='{currency.Replace(",", "")}',BASE_QTY='{base_qty.Replace(",", "") }',
                                            PRICE_UNIT='{price_unit}' , SAP_FLAG='N', UPLOADDT=sysdate,UPLOADBY='{LoginUser.EMP_NO}' 
                                        where task_no='{task_no}'";
                                SFCDB.ExecSQL(sql);

                                sql = $@"UPDATE r_7b5_xml_t  SET PO_FLAG='N' WHERE TASK_NO='{task_no}' ";
                                SFCDB.ExecSQL(sql);
                            }
                            else
                            {
                                sql = $@" insert into  r_7b5_po (TASK_NO,PO_NO,ITEM,QTY,SALES_UNIT,PRICE,CURRENCY,BASE_QTY,PRICE_UNIT,UPLOADDT,UPLOADBY,SAP_FLAG)
                                    Values ('{task_no}','{po_no}','{item_num}','{qty.Replace(",", "") }','{sales_unit.Replace(",", "")}','{price.Replace(",", "")}',
                                            '{currency.Replace(",", "")}','{base_qty.Replace(",", "") }','{price_unit}',SYSDATE,'{LoginUser.EMP_NO}','N')";
                                SFCDB.ExecSQL(sql);
                            }
                        }
                        else
                        {
                            result = result + task_no + ",";
                        }
                    }
                }
                SFCDB.CommitTrain();

                if (result == "")
                {
                    result = "PO全部上傳OK ! ";
                }
                else
                {
                    result = "以下PO上傳失敗:" + result;
                }
                #endregion

                #region 調用SAP
                       
                string message = "";
                string ZPSD24Message = "";
                string ESS = "";
                string O_FLAG = "";

                ZCPP_NSBG_0122 zcpp_nsbg_0122 = new ZCPP_NSBG_0122(BU);
                DataTable in_table = new DataTable();
                DataTable out_table = new DataTable();
                in_table.Columns.Add("MANDT");
                in_table.Columns.Add("BATNO");
                in_table.Columns.Add("BSTKD");
                in_table.Columns.Add("MABNR");
                in_table.Columns.Add("KWMENG");
                in_table.Columns.Add("VRKME");
                in_table.Columns.Add("KBETR");
                in_table.Columns.Add("KONWA");
                in_table.Columns.Add("KPEIN");
                in_table.Columns.Add("KMEIN");
                in_table.Columns.Add("ERFDT");

                DataRow dr = null;
                sql = "select * from r_7b5_po where SAP_FLAG='N'";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    in_table.Clear();
                    dr = in_table.NewRow();
                    dr["MANDT"] = "";
                    dr["BATNO"] = dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim();
                    dr["BSTKD"] = dt.Rows[i]["PO_NO"].ToString().ToUpper().Trim();
                    dr["MABNR"] = dt.Rows[i]["ITEM"].ToString().ToUpper().Trim();
                    dr["KWMENG"] = dt.Rows[i]["QTY"].ToString().ToUpper().Trim();
                    dr["VRKME"] = dt.Rows[i]["SALES_UNIT"].ToString().ToUpper().Trim();
                    dr["KBETR"] = dt.Rows[i]["PRICE"].ToString().ToUpper().Trim();
                    dr["KONWA"] = dt.Rows[i]["CURRENCY"].ToString().ToUpper().Trim();
                    dr["KPEIN"] = dt.Rows[i]["BASE_QTY"].ToString().ToUpper().Trim();
                    dr["KMEIN"] = dt.Rows[i]["PRICE_UNIT"].ToString().ToUpper().Trim();
                    dr["ERFDT"] = DateTime.Parse(dt.Rows[i]["UPLOADDT"].ToString().ToUpper().Trim()).ToString("yyyyMMdd");
                    in_table.Rows.Add(dr);
                    zcpp_nsbg_0122.SetValue(in_table);
                    zcpp_nsbg_0122.CallRFC();
                    out_table = zcpp_nsbg_0122.GetTableValue("OUT_TAB");
                    O_FLAG = zcpp_nsbg_0122.GetValue("O_FLAG");
                    message = zcpp_nsbg_0122.GetValue("O_MESSAGE");
                    if (O_FLAG == "0")
                    {
                        sql = $@" UPDATE r_7b5_po SET SAP_MESSAGE='{message}',SAP_FLAG='Y' WHERE TASK_NO='{dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim()}' AND PO_NO='{dt.Rows[i]["PO_NO"].ToString().ToUpper().Trim()}' ";
                        SFCDB.ExecSQL(sql);

                        sql = $@"UPDATE r_7b5_xml_t  SET PO_FLAG='Y' WHERE TASK_NO='{dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim()}' ";
                        SFCDB.ExecSQL(sql);
                    }
                    else
                    {
                        if (out_table.Rows.Count > 0)
                        {
                            ESS = out_table.Rows[0]["ESS"].ToString();
                            if (ESS.Trim() == "Batch Order is duplicate!")
                            {
                                //如果是有重複的則默認SAP上傳成功
                                sql = $@"UPDATE r_7b5_po SET SAP_MESSAGE='{ESS}',SAP_FLAG='Y' WHERE TASK_NO='{dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim()}' AND PO_NO='{dt.Rows[i]["PO_NO"].ToString().ToUpper().Trim()}' ";
                            }
                            else
                            {
                                sql = $@"UPDATE r_7b5_po SET SAP_MESSAGE='{ESS}' WHERE TASK_NO='{dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim()}' AND PO_NO='{dt.Rows[i]["PO_NO"].ToString().ToUpper().Trim()}' ";
                            }
                            SFCDB.ExecSQL(sql);
                            ZPSD24Message = ZPSD24Message + dt.Rows[i]["TASK_NO"].ToString().ToUpper().Trim() + ",";
                        }
                    }
                }
                if (ZPSD24Message == "")
                {
                    result = result + "ZPSD24全部上傳OK";
                }
                else
                {
                    result = result + "以下任務令ZPSD24上傳失敗:" + ZPSD24Message;
                }
                #endregion

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result;
                //StationReturn.MessageCode = "MES00000002";
                //StationReturn.Data = "";               
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// HWD Cut Workorder頁面加載要Cut 的工單
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetCutWoList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "select distinct WORKORDERNO from r_wo_base where closed_flag=0 and workorderno not like '*%' and DOWNLOAD_DATE >sysdate-100  order by WORKORDERNO asc";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// HWD Cut Workorder頁面選擇工單獲取工單信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetWODataInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString();              
                string sql = $@"select WORKORDER_QTY,SKU_VER,START_STATION from r_wo_base where workorderno='{wo}'";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// HWD Cut Workorder頁面GheckData按鈕事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CutWoCheckWoData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString();
                string startStation = Data["StartStation"] == null ? "" : Data["StartStation"].ToString(); 
                string sql = $@"select count(sn) as qty from r_sn where workorderno='{wo}'and NEXT_STATION='{startStation}' AND REPAIR_FAILED_FLAG=0 AND COMPLETED_flag=0";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt.Rows[0][0].ToString();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        /// <summary>
        /// HWD Cut Workorder頁面Submit按鈕事件
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CutWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString().Trim();
                string version = Data["Version"] == null ? "" : Data["Version"].ToString().Trim();
                string startStation = Data["StartStation"] == null ? "" : Data["StartStation"].ToString().Trim();
                int qty ;
                if (!int.TryParse(Data["QTY"].ToString().Trim(), out qty))
                {
                    throw new Exception("Input QTY Error!");
                }
                
                if (string.IsNullOrEmpty(wo))
                {
                    throw new Exception("You must input the WO!");
                }               
                string message = "";
                int result,loadingQty;
                bool updateWo = false;

                T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBTYPE);
                T_C_SKU TCS = new T_C_SKU(SFCDB,DBTYPE);
                T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, DBTYPE);
                T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);

                C_SKU skuObject = null;
                R_MES_LOG logObject = null;
                C_ROUTE_DETAIL routeDetail = null;

                List<R_SN> snList = new List<R_SN>();
                DateTime systemDt = TRWB.GetDBDateTime(SFCDB);
                Row_R_WO_BASE rowBase = TRWB.LoadWorkorder(wo, SFCDB);
                R_WO_BASE woObject = rowBase == null ? null : rowBase.GetDataObject();
                if (woObject == null)
                {
                    throw new Exception($@"{wo} Not Exist!");
                }
                snList = SFCDB.ORM.Queryable<R_SN>().Where(r => r.WORKORDERNO == wo).ToList();
                loadingQty = snList.Count();

                #region 變更版本﹕
                if (version != woObject.SKU_VER && version.ToUpper() != "DEL")
                {
                    // 判斷c_sku中的版本與變更后的版本是否一致﹐不一致則提示"不允許變更版本"
                    skuObject = SFCDB.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == woObject.SKUNO && r.VERSION == version).ToList().FirstOrDefault();
                    if (skuObject == null)
                    {
                        throw new Exception("SKUVERSION IS ERROR,PL INPUT AGAIN");
                    }
                    woObject.SKU_VER = version;
                    result = TRWB.Update(woObject, SFCDB);
                    if (result > 0)
                    {
                        message = "OK,UPDATE SKUVERSION SUCCESS!";
                    }
                    else
                    {
                        throw new Exception("UPDATE SKUVERSION FAIL!");
                    }
                }
                #endregion

                //依據變更前后的對比﹐判斷用戶需要變更的類型﹕
                #region (1).僅砍工單數量(TC010)﹕起始站位不變,但數量不同時﹐只有未投的數量才能砍
                if (qty != woObject.WORKORDER_QTY && woObject.START_STATION == startStation)
                {  
                    if (qty < 0)
                    {
                        throw new Exception("INPUT WO_QTY IS ERROR,PL INPUT AGAIN");
                    }
                    if (woObject.WORKORDER_QTY < qty)
                    {
                        throw new Exception("INPUT WO_QTY IS ERROR,PL INPUT AGAIN");
                    }

                    //如果數量=0﹐則直接關閉工單  數量為0，且沒有loading過才可以直接關閉
                    if (qty == 0 && qty == loadingQty)
                    {
                        woObject.WORKORDER_QTY = 0;
                        woObject.INPUT_QTY = 0;
                        woObject.FINISHED_QTY = 0;
                        woObject.CLOSED_FLAG = "1";
                        woObject.CLOSE_DATE = systemDt;
                        updateWo = true;
                    }
                    //進行砍工單處理
                    if (woObject.WORKORDER_QTY > qty)
                    {
                        //如果修改后的數量剛好等于當前投入數量﹐則直接置工單數量為修改后的數量
                        //如果當前投入數量小于調整后的數量﹐則更新工單的數量為調整后的工單數量                        
                        if (qty >= loadingQty)
                        {
                            woObject.WORKORDER_QTY = qty;

                        }
                        //如果當前投入數量大于調整后的數量﹐則提示不允許這樣砍工單數量 
                        else if (loadingQty > qty)
                        {
                            throw new Exception($@"{wo} ONLINE_QTY:{loadingQty}>{qty} ,CANT NOT BE UPDATED!'");
                        }
                        //取得已經入庫的工單數量﹐如果數量等于調整后的數量﹐則直接關閉工單
                        int completedQty = snList.Where(r => r.COMPLETED_FLAG == "1").ToList().Count();
                        if (completedQty == qty)
                        {
                            woObject.CLOSED_FLAG = "1";
                            woObject.CLOSE_DATE = systemDt;
                        }
                        updateWo = true;
                    }
                    if (updateWo)
                    {
                        woObject.EDIT_EMP = LoginUser.EMP_NO;
                        woObject.EDIT_TIME = systemDt;
                        result = TRWB.Update(woObject, SFCDB);
                        if (result > 0)
                        {
                            message = "OK,CUT WO SUCCESS!";
                        }
                        else
                        {
                            throw new Exception("UPDATE MFWORKORDER ERROR!");
                        }
                        logObject = new R_MES_LOG();
                        logObject.ID = TRML.GetNewID(BU, SFCDB);
                        logObject.PROGRAM_NAME = "WEB";
                        logObject.CLASS_NAME = "MESStation.Config.HWD.C7B5API";
                        logObject.FUNCTION_NAME = "CutWO";
                        logObject.LOG_MESSAGE = "Cut " + wo;
                        logObject.DATA1 = wo;
                        logObject.DATA2 = qty.ToString() + ";" + startStation + ";" + version;
                        logObject.DATA3 = "UPDATE";
                        logObject.EDIT_EMP = LoginUser.EMP_NO;
                        logObject.EDIT_TIME = systemDt;
                        TRML.InsertMESLogOld(logObject, SFCDB);
                    }
                }
                #endregion;

                #region (2).刪除整個工單(TC020)﹕ 輸入界面中的版本欄位輸入"DEL"時﹐只有未投入的工單才能刪除
                if (version.ToUpper() == "DEL")
                {                
                    //工單已經投入不能刪除整個工單 
                    if (loadingQty > 0)
                    {
                        throw new Exception($@"{wo} ALREADY ONLINE,CANT NOT BE CANCELED!");
                    }
                    result = SFCDB.ORM.Deleteable<R_WO_BASE>().Where(r => r.ID == woObject.ID).ExecuteCommand();
                    if (result > 0)
                    {
                        message = "OK,DELETE WO SUCCESS!";
                    }
                    else
                    {
                        throw new Exception("DELETE WO FAIL!");
                    }
                    logObject = new R_MES_LOG();
                    logObject.ID = TRML.GetNewID(BU, SFCDB);
                    logObject.PROGRAM_NAME = "WEB";
                    logObject.CLASS_NAME = "MESStation.Config.HWD.C7B5API";
                    logObject.FUNCTION_NAME = "CutWO";
                    logObject.LOG_MESSAGE = "Delete "+wo ;
                    logObject.DATA1 = wo;
                    logObject.DATA3 = "DELETE";
                    logObject.EDIT_EMP = LoginUser.EMP_NO;
                    logObject.EDIT_TIME = systemDt;
                    TRML.InsertMESLogOld(logObject, SFCDB);
                }
                #endregion;

                #region (3). 依站位砍工單數量(TC030)﹕起始站位及數量均不同﹐只有37和外包工單才能砍 
                if (qty != woObject.WORKORDER_QTY && woObject.START_STATION != startStation)
                {
                    //判斷工站是否正確﹐不能為虛擬工站﹐不能是JobFinish
                    routeDetail = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == woObject.ROUTE_ID && r.STATION_NAME == startStation).ToList().FirstOrDefault();
                    if (routeDetail != null)
                    {
                        throw new Exception("STATION IS ERROR,PL INPUT AGAIN");
                    }
                    if (routeDetail.STATION_TYPE == "JOBFINISH")
                    {
                        throw new Exception("INPUT STATION IS JOBFINISH,PL INPUT AGAIN");
                    }

                    //判斷輸入的數量是否等于當前工站的數量﹐除去維修品數量
                    List<R_SN> listDelete = snList.Where(r => r.REPAIR_FAILED_FLAG == "0" && r.NEXT_STATION == startStation && r.COMPLETED_FLAG == "0").ToList();
                    int deleteQty = listDelete.Count();
                    if (qty != deleteQty)
                    {
                        throw new Exception(" SOME PRODUCTS AT REPAIR OR HAD BEEN COMPLETED,CAN NOT CUT!");
                    }
                    //1.刪除主表
                    string tempSN = "", tempWO = "", tempSku = "";
                    foreach (R_SN sn in listDelete)
                    {
                        tempSN = "DEL_" + sn.SN;
                        tempWO = "DEL_" + sn.WORKORDERNO;
                        tempSku = "DEL_" + tempSku;
                        sn.BOXSN = sn.BOXSN == "" ? "" : "DEL_" + sn.BOXSN;
                        result = TRS.Update(sn, SFCDB);
                        if (result == 0)
                        {
                            throw new Exception("UPDATE SN FAIL!");
                        }
                    }
                    //2.更新工單表
                    woObject.WORKORDER_QTY = woObject.WORKORDER_QTY - deleteQty;
                    woObject.INPUT_QTY = woObject.INPUT_QTY - deleteQty;
                    if (woObject.WORKORDER_QTY == woObject.FINISHED_QTY)
                    {
                        woObject.CLOSED_FLAG = "1";
                        woObject.CLOSE_DATE = systemDt;
                    }
                    woObject.EDIT_EMP = LoginUser.EMP_NO;
                    woObject.EDIT_TIME = systemDt;
                    result = TRWB.Update(woObject, SFCDB);
                    if (result > 0)
                    {
                        message = "OK,CUT WO SUCCESS!";
                    }
                    else
                    {
                        throw new Exception("UPDATE MFWORKORDER ERROR!");
                    }
                    logObject = new R_MES_LOG();
                    logObject.ID = TRML.GetNewID(BU, SFCDB);
                    logObject.PROGRAM_NAME = "WEB";
                    logObject.CLASS_NAME = "MESStation.Config.HWD.C7B5API";
                    logObject.FUNCTION_NAME = "CutWO";
                    logObject.LOG_MESSAGE = "Cut " + wo;
                    logObject.DATA1 = wo;
                    logObject.DATA2 = qty.ToString() + ";" + startStation + ";" + version;
                    logObject.DATA3 = "UPDATE";
                    logObject.EDIT_EMP = LoginUser.EMP_NO;
                    logObject.EDIT_TIME = systemDt;
                    TRML.InsertMESLogOld(logObject, SFCDB);

                }
                #endregion;

                #region (4).僅變更起始站位﹕起始站位不同﹐但數量相同
                if (woObject.START_STATION != startStation && qty == woObject.WORKORDER_QTY && version.ToUpper() != "DEL")
                {
                    //判斷輸入的起始站位是否在工單的流程內﹐且不為虛擬工站﹐如不存在則報錯:
                    routeDetail = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == woObject.ROUTE_ID && r.STATION_NAME == startStation).ToList().FirstOrDefault();
                    if (routeDetail != null)
                    {
                        throw new Exception("STATION IS ERROR,PL INPUT AGAIN");
                    }
                    if (routeDetail.STATION_TYPE == "JOBFINISH")
                    {
                        throw new Exception("INPUT STATION IS JOBFINISH,PL INPUT AGAIN");
                    }
                    woObject.START_STATION = startStation;
                    woObject.EDIT_EMP = LoginUser.EMP_NO;
                    woObject.EDIT_TIME = systemDt;
                    result = TRWB.Update(woObject, SFCDB);
                    if (result > 0)
                    {
                        message = "OK,UPDATE START_STATION SUCCESS!";
                    }
                    else
                    {
                        throw new Exception("UPDATE MFWORKORDER ERROR!");
                    }
                    logObject = new R_MES_LOG();
                    logObject.ID = TRML.GetNewID(BU, SFCDB);
                    logObject.PROGRAM_NAME = "WEB";
                    logObject.CLASS_NAME = "MESStation.Config.HWD.C7B5API";
                    logObject.FUNCTION_NAME = "CutWO";
                    logObject.LOG_MESSAGE = "UPDATE START_STATION " + wo;
                    logObject.DATA1 = wo;
                    logObject.DATA2 = qty.ToString() + ";" + startStation + ";" + version;
                    logObject.DATA3 = "UPDATE";
                    logObject.EDIT_EMP = LoginUser.EMP_NO;
                    logObject.EDIT_TIME = systemDt;
                    TRML.InsertMESLogOld(logObject, SFCDB);
                }
                #endregion;


                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.MessagePara.Add(message); 
                StationReturn.Message = message;                      
                this.DBPools["SFCDB"].Return(SFCDB);                
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

        public void ModifyShipExtQty(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {              
                int result = 0;
                string task_no, hh_item, hw_item;
                string user = LoginUser.EMP_NO;
                double? ext_qty;
                ext_qty = Convert.ToDouble(Data["EXT_QTY"].ToString());
                task_no = Data["TASK_NO"].ToString();
                hh_item = Data["HH_ITEM"].ToString();
                hw_item = Data["HW_ITEM"].ToString();
                T_R_7B5_SHIP TRS = new T_R_7B5_SHIP(SFCDB, DBTYPE);
                DateTime dateTime = TRS.GetDBDateTime(SFCDB);
                result = SFCDB.ORM.Updateable<R_7B5_SHIP>().UpdateColumns(
                    r => new R_7B5_SHIP
                    {
                        TOTAL_PLAN_QTY = r.TASK_QTY-r.BUFFER_QTY-ext_qty,                       
                        LASTEDITBY = user,
                        LASTEDITDT = dateTime
                    })
                    .Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item&& r.HW_ITEM == hw_item).ExecuteCommand();
                if (result <= 0)
                {
                    throw new Exception("修改失敗!");
                }
                StationReturn.MessageCode = "MES00000002";
                StationReturn.MessagePara = null;
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                SFCDB.CommitTrain();

            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        /// <summary>
        /// 檢查上傳的Excle是否包含模板中的列
        /// </summary>
        /// <param name="inputExcelColumn"></param>
        /// <param name="listTitle"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool CheckInputExcelTitle(Newtonsoft.Json.Linq.JObject inputExcelColumn, List<string> listTitle,out string title)
        {
            bool bColumnExists = true;
            string out_title = "";
            foreach (string t in listTitle)
            {
                bColumnExists = inputExcelColumn.Properties().Any(p => p.Name == t);                
                if (!bColumnExists)
                {
                    out_title = t;
                    break;
                }
            }
            title = out_title;
            return bColumnExists;
        }
    }   
}
