using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Api
{
    public class TruckLoadAPI : MesAPIBase
    {
        protected APIInfo FGetTOPhysicalPalletListLoading = new APIInfo()
        {
            FunctionName = "GetTOPhysicalPalletListLoading",
            Description = "",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FPrintJuniperMasterPalletLabel = new APIInfo()
        {
            FunctionName = "PrintJuniperMasterPalletLabel",
            Description = "",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        public TruckLoadAPI()
        {
            this.Apis.Add(FGetTOPhysicalPalletListLoading.FunctionName, FGetTOPhysicalPalletListLoading);
            this.Apis.Add(FPrintJuniperMasterPalletLabel.FunctionName, FPrintJuniperMasterPalletLabel);
        }

        public void GetTOPhysicalPalletListLoading(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            var TONO = Data["TONO"].ToString();
            OleExec _DB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var toObj = _DB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TO_NO == TONO).First();
                if (toObj == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143020", new string[] { "TONO:" + TONO }));
                }
                var sql = $@"SELECT wm_concat(POLINES) POLINE,
                                   SALESORDER,
                                   wm_concat(SALESORDERLINEITEM) SOLINE,
                                   wm_concat(SKUNO) SKUNO,
                                   wm_concat(GROUPID) GROUPID,
                                   PALLETID
                              FROM (SELECT DISTINCT O.PONO||'.'||O.POLINE POLINES,
                                                    H.SALESORDERNUMBER SALESORDER,
                                                    I.SALESORDERLINEITEM,
                                                    A.SKUNO,
                                                    A.GROUPID,
                                                    A.PALLETID
                                      FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I,O_I137_HEAD H
                                     WHERE A.WORKORDERNO = O.PREWO
                                       AND O.ITEMID = I.ID
                                       AND I.TRANID = H.TRANID
                                       AND INVOICENO = '{TONO}'
                                       AND PALLETID IN (SELECT PALLETID
                                                          FROM (SELECT DISTINCT PALLETID, SALESORDER
                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                 WHERE INVOICENO = '{TONO}')
                                                         GROUP BY PALLETID
                                                        HAVING COUNT(1) = 1)) T
                             GROUP BY SALESORDER, PALLETID
                             ORDER BY SALESORDER, PALLETID";
                var pallets = _DB.ORM.Ado.SqlQuery<MasterPallet>(sql);

                StationReturn.Data = pallets;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(_DB);
            }
        }

        public void PrintJuniperMasterPalletLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string newpalletno = Data["NEWPALLETNO"] == null ? "" : Data["NEWPALLETNO"].ToString().Trim();
                if (string.IsNullOrEmpty(newpalletno))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "PO And PO Item" }));
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var palletlist = SFCDB.ORM.Queryable<R_JUNIPER_MFPACKINGLIST>().Where(t => t.PALLETID == newpalletno).ToList();
                if (palletlist.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151913", new string[] { newpalletno }));
                }

                bool bPrinted = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(r => r.PROGRAM_NAME == "Master Pallet Double Check" && r.FUNCTION_NAME == "MasterPalletDoubleCheck" && r.DATA1 == newpalletno).Any();

                bool bCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDoubleCheck" && r.CATEGORY == "MasterPalletLabelCheck"
                                && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && SqlFunc.ToUpper(r.VALUE) == "YES").Any();
                if (!bPrinted && bCheck)
                {
                    //double check pallte
                    MasterPalletDoubleCheck(SFCDB, newpalletno, $@"Physical Pallet:{newpalletno},MasterPalletDoubleCheck");
                }

                #region 獲取模板
                R_F_CONTROL template_file = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl"
                  && r.CATEGORY == "PrintMasterPalletLabelTemplate" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                    .OrderBy(r => r.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (template_file == null || string.IsNullOrEmpty(template_file.VALUE))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_MARST_LABEL"));
                }
                if (string.IsNullOrEmpty(template_file.EXTVAL))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "PrintMasterPalletLabelTemplate's LabelType" }));
                }
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == template_file.VALUE).ToList().FirstOrDefault();
                if (tempLabel == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "Master Pallet Label: " + template_file.VALUE }));

                }
                T_R_Label TRL = new T_R_Label(SFCDB, DBTYPE);
                T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DBTYPE);

                Row_R_Label RL = TRL.GetLabelConfigByLabelName(tempLabel.LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(template_file.EXTVAL, SFCDB);
                if (RL == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153446", new string[] { tempLabel.LABELNAME }));
                }
                if (RC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "LabelType: " + template_file.EXTVAL }));
                }
                LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = MESPubLab.Json.JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, SFCDB);
                    Lab = API_CLASS;
                }
                var label_input_palletno = Lab.Inputs.Find(l => l.StationSessionType == "PALLETNO" && l.StationSessionKey == "1");
                if (label_input_palletno == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting [SessionType=ASN,SessionKey=1] Input!");
                }
                label_input_palletno.Value = newpalletno;

                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = 1;
                Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                Lab.MakeLabel(SFCDB);
                var noprint = Lab.Outputs.Find(t => t.Name == "NotPrint" && t.Value.ToString() == "TRUE");
                if (noprint != null)
                {
                    return;
                }
                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);
                
                Dictionary<string, List<LabelBase>> LabelPrints = new Dictionary<string, List<LabelBase>>();
                LabelPrints.Add(RL.R_FILE_NAME, pages);

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, this.DBTYPE);
                R_MES_LOG log_object = new R_MES_LOG();
                log_object.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                log_object.PROGRAM_NAME = "Print Master Pallet Label";
                log_object.CLASS_NAME = "MESJuniper.Api.TruckLoadAPI";
                log_object.FUNCTION_NAME = "JuniperPrintMasterPalletLabel";
                log_object.DATA1 = newpalletno;
                log_object.EDIT_EMP = LoginUser.EMP_NO;
                log_object.EDIT_TIME = SFCDB.ORM.GetDate();

                SFCDB.ORM.Insertable<R_MES_LOG>(log_object).ExecuteCommand();

                StationReturn.Data = new { LabelPrints = LabelPrints };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                #endregion

            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                if (SFCDB != null)
                    this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void MasterPalletDoubleCheck(OleExec SFCDB, string PhysicalPallet, string title)
        {
            T_R_PACKING t_r_packing = new T_R_PACKING(SFCDB, DBTYPE);
            string[] pallet_arry = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>().Where(o => o.NEW_PACK_NO == PhysicalPallet).Select(o => o.PACK_NO).ToArray();

            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, DBTYPE);
            R_MES_LOG check_log = null;

            UIInputData O = new UIInputData() { };
            O.Timeout = 3000000;
            O.IconType = IconType.Warning;
            O.Type = UIInputType.String;
            O.Tittle = title;
            O.ErrMessage = "No input";
            O.UIArea = new string[] { "40%", "80%" };
            O.OutInputs.Clear();
            O.Message = "PALLET NO";
            O.Name = "PALLET_NO";
            O.CBMessage = "";

            StringBuilder s = new StringBuilder();
            var isLoop = true;
            while (isLoop)
            {
                var input_sn = O.GetUiInput(this, UIInput.Normal);
                if (input_sn == null)
                {
                    O.CBMessage = $@"Please Scan One Pallet Number On Physical Pallet.";
                }
                else
                {
                    string check_value = input_sn.ToString().Trim();
                    if (string.IsNullOrEmpty(check_value))
                    {
                        O.CBMessage = $@"Please Scan One Pallet Number On Physical Pallet.";
                    }
                    else if (check_value.Equals("No input"))
                    {
                        throw new Exception("User Cancel");
                    }
                    else if (pallet_arry.Contains(check_value))
                    {
                        isLoop = false;
                        check_log = new R_MES_LOG();
                        check_log.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                        check_log.PROGRAM_NAME = "Master Pallet Double Check";
                        check_log.CLASS_NAME = "MESJuniper.Api.TruckLoadAPI";
                        check_log.FUNCTION_NAME = "MasterPalletDoubleCheck";
                        check_log.LOG_MESSAGE = title;
                        check_log.DATA1 = PhysicalPallet;
                        check_log.DATA2 = check_value;
                        check_log.DATA3 = PhysicalPallet;
                        check_log.EDIT_TIME = SFCDB.ORM.GetDate();
                        check_log.EDIT_EMP = LoginUser.EMP_NO;
                        SFCDB.ORM.Insertable<R_MES_LOG>(check_log).ExecuteCommand();
                    }
                    else
                    {
                        O.CBMessage = $@"The pallet number entered is not on the physical pallet.";
                    }
                }
            }

        }

        class MasterPallet 
        {
            public string PONO { get; set; }
            public string POLINE { get; set; }
            public string SALESORDER { get; set; }
            public string SOLINE { get; set; }
            public string SKUNO { get; set; }
            public string GROUPID { get; set; }
            public string PALLETID { get; set; }
        }
    }
    
}
