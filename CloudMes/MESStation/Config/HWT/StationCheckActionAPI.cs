using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.HWT
{
    public class StationCheckActionAPI : MesAPIBase
    {
        private APIInfo FHWTCBSPalletDoubleCheckAction = new APIInfo()
        {
            FunctionName = "HWT CBS Station Pallet Double Check Action",
            Description = " HWT CBS Station Pallet Double Check Action",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PalletNo", InputType= "string" , DefaultValue="" },
                 new APIInputInfo() { InputName="CheckType", InputType= "string" , DefaultValue="" },
                new APIInputInfo() { InputName="CheckValue", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };
        private APIInfo FHWTCBSPalletDoubleCheckLoading = new APIInfo()
        {
            FunctionName = "HWT CBS Pallet Double Check Loading",
            Description = "HWT CBS Pallet Double Check Loading",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PalletNo", InputType= "string" , DefaultValue="" },
                new APIInputInfo() { InputName="CheckType", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        public StationCheckActionAPI()
        {
            Apis.Add(FHWTCBSPalletDoubleCheckLoading.FunctionName, FHWTCBSPalletDoubleCheckLoading);
            Apis.Add(FHWTCBSPalletDoubleCheckAction.FunctionName, FHWTCBSPalletDoubleCheckAction);
        }

        public void HWTCBSPalletDoubleCheckLoading(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["PalletNo"] == null || Data["PalletNo"].ToString() == "")
                {
                    throw new Exception("Please input PalletNo");
                }
                if (Data["CheckType"] == null || Data["CheckType"].ToString() == "")
                {
                    throw new Exception("Please input CheckType");
                }                
                string palletNo = Data["PalletNo"].ToString().Trim();
                string checkType = Data["CheckType"].ToString().Trim();
                T_R_PALLET_DOUBLE_CHECK TRPD = new T_R_PALLET_DOUBLE_CHECK(SFCDB, DBTYPE);
                T_R_PACKING TRP = new T_R_PACKING(SFCDB, DBTYPE);
                R_PACKING packObject = TRP.GetBYPACKNO(palletNo, SFCDB);
                int totalQty = 0;
                if (packObject == null)
                {
                    throw new Exception("");
                }
                if (checkType.ToUpper() == "CARTON")
                {
                    totalQty = TRP.GetListPackByParentPackId(packObject.ID, SFCDB).Count;
                }
                else if (checkType.ToUpper() == "SN")
                {
                    totalQty = TRP.GetPakcingSNList(packObject.ID, packObject.PACK_TYPE, SFCDB).Count;
                }
                List<R_PALLET_DOUBLE_CHECK> list = TRPD.GetCheckList(SFCDB, palletNo, checkType);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = new { TotalQty = totalQty, CheckQty = list.Count, CheckList = list };
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(totalQty);
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
        public void HWTCBSPalletDoubleCheckAction(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["PalletNo"] == null || Data["PalletNo"].ToString() == "")
                {
                    throw new Exception("Please input PalletNo");
                }
                if (Data["CheckType"] == null || Data["CheckType"].ToString() == "")
                {
                    throw new Exception("Please input CheckType");
                }
                if (Data["CheckValue"] == null || Data["CheckValue"].ToString() == "")
                {
                    throw new Exception("Please input type");
                }
                string palletNo = Data["PalletNo"].ToString().Trim();
                string checkType = Data["CheckType"].ToString().Trim();
                string checkValue = Data["CheckValue"].ToString().Trim().ToUpper();
                T_R_PALLET_DOUBLE_CHECK TRPD = new T_R_PALLET_DOUBLE_CHECK(SFCDB, DBTYPE);
                if (TRPD.IsExist(SFCDB, palletNo, checkType, checkValue))
                {
                    //throw new Exception(checkValue + " 該序列號已經掃描，請不要重復掃描！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144405", new string[] { checkValue }));
                }

                R_PALLET_DOUBLE_CHECK objCheck = new R_PALLET_DOUBLE_CHECK();
                objCheck.ID = TRPD.GetNewID(this.BU, SFCDB);
                objCheck.PALLET_NO = palletNo;
                objCheck.CHECK_TYPE = checkType;
                objCheck.CHECK_VALUE = checkValue;
                objCheck.STATUS = 1;
                objCheck.EDIT_EMP = this.LoginUser.EMP_NO;
                objCheck.EDIT_TIME = this.GetDBDateTime();
                int result = TRPD.Save(SFCDB, objCheck);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PALLET_DOUBLE_CHECK:" + palletNo, "INSERT" }));
                }

                List<R_PALLET_DOUBLE_CHECK> list = TRPD.GetCheckList(SFCDB, palletNo, checkType);  
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(list.Count);
                StationReturn.Data = new { CheckQty = list.Count, CheckList = list };
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
    }
}
