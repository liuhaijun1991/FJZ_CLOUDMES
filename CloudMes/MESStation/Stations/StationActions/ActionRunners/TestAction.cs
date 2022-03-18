using System;
using System.Collections.Generic;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class TestAction
    {
        public static void UIINPUTTEST(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            UIInputData IW = new UIInputData() { MustConfirm = true, Timeout = 3000000, IconType = IconType.None, UIArea = new string[] { "30%", "35%" }, Message = "請稱重!", Tittle = "請稱重", Type = UIInputType.Weight, Name = "WEIGHT", ErrMessage = "請稱重!" };
            //IW.OutInputs.Add(new DisplayOutPut() { Name = "WEIGHT", DisplayType = UIOutputType.TextArea.ToString(), Value = "請稱重!" });
           // var weightdata = IW.GetUiInput(Station.API, UIInput.Normal, Station);

            UIInputData I = new UIInputData() { MustConfirm = false,Timeout = 3000000,IconType = IconType.None, UIArea = new string[]{"30%","25%"}, Message = "請輸入IMEI!", Tittle = "請輸入IMEI", Type = UIInputType.Alart, Name = "IMEI", ErrMessage ="未輸入IMEI" };
            I.OutInputs.Add(new DisplayOutPut() { Name = "PCBA S/N", DisplayType = UIOutputType.TextArea.ToString(), Value = "465136N+2009RR01F50000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000aaaaaa" });
            var ret = I.GetUiInput(Station.API, UIInput.Normal);

            UIInputData O = new UIInputData() { Timeout =50000,IconType = IconType.None, Message = "請輸入IMEI", Tittle = "8989898989", Type = UIInputType.Password, Name = "IMEI", ErrMessage = "未輸入IMEI" };
            O.OutInputs.Add(new DisplayOutPut() { Name = "PCBA S/N", DisplayType = UIOutputType.TextArea.ToString(), Value = "465136N+" });
            O.OutInputs.Add(new DisplayOutPut() { Name = "MAC S/N", DisplayType = UIOutputType.Text.ToString(), Value = "01:B0:L4:58:12" });
            var testval = "";
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
            {
                if (res != "1")
                {
                    testval = res;
                    return true;
                }
                else
                    O.CBMessage = $@"驗證失敗=>{res},請重試!";
                    return false;
            });

            var ret2 = O.GetUiInput(Station.API, UIInput.Normal, Station);
        }
        public static void TEST1(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string SN = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN("TEST",Station.DBS["SFCDB"]);

            Station.StationMessages.Add(new StationMessage() { Message = SN, State = StationMessageState.Message });
        }

        public static void TestUpdateSkuVersion(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904013101"));
            }          

            C_SKU sku = (C_SKU)skuSession.Value;

            //T_C_SKU t_c_sku = new T_C_SKU(Station.SFCDB, Station.DBType);
            //Row_C_SKU rowSku = (Row_C_SKU)t_c_sku.GetObjByID(sku.ID,Station.SFCDB);
            //rowSku.VERSION = "test";
            //rowSku.EDIT_EMP = Station.LoginUser.EMP_NO;
            //rowSku.EDIT_TIME = Station.GetDBDateTime();
            //var i = Station.SFCDB.ExecSQL(rowSku.GetUpdateString(Station.DBType));

            //DateTime dt = Station.GetDBDateTime();
            //var n = Station.SFCDB.ORM.Updateable<C_SKU>().UpdateColumns(c => new C_SKU { VERSION = "test222", EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = dt })
            //     .Where(c => c.ID == sku.ID).ExecuteCommand();
        }

        public static void ChangeStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //Station.ParentTransferStation = new TransferStation(Station)
            //{
            //    ParentStationName = Station.DisplayName,
            //    TransferStationName = "EDENTEST_B",
            //    transferStationInputs = new List<TransferStationInput>() {
            //        new TransferStationInput()
            //        {
            //            inputName = "SN",inputValue = Input.Value
            //        }
            //    }
            //};
            Station.CurrActionRrturn = StationActionReturn.PassStopRunNext;
        }

        public static void UIInputTest(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
          
            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.None,
                UIArea = new string[] { "90%", "90%" },
                Message = "SN",
                Tittle = "AutoKP",
                Type = UIInputType.String,
                Name = "XXX",
                ErrMessage = "No input",
                CBMessage = ""
            };

            I.OutInputs.Add(new DisplayOutPut()
            {
                Name = "SKUNO",
                DisplayType = UIOutputType.TextArea.ToString(),
                Value = "XXX"
            });

            var strSN = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
            Station.CurrActionRrturn = StationActionReturn.PassStopRunNext;
        }

    }
}
