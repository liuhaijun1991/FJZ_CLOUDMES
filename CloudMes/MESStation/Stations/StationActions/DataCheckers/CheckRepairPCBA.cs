using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using SqlSugar;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckRepairPCBA 
    {
        public static void RepairPCBAChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            List<string> woList = new List<string>();
            List<string> SNList = new List<string>();
            List<string> notValidSN = new List<string>();

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string parentSN = sessionSN.Value.ToString();
            var objParentSN = DB.ORM.Queryable<R_SN>()
                        .Where(r => r.SN == parentSN && r.VALID_FLAG == "1").First();
            

            if (objParentSN.SKUNO.Contains("711-"))
            {
                return;
            }
            else
            {
                if (CheckPCBASNRepair(parentSN, DB))
                {
                    var repairedSNsList = DB.ORM.Queryable<R_REPAIR_PCBA_RELATIONSHIP>()
                    .Where(r => r.PARENT_SN == parentSN && r.WO_FLAG == "0").Select(r => r.REPAIRED_SN).ToList();

                    foreach (string verifySN in repairedSNsList)
                    {
                        bool snExists = DB.ORM.Queryable<R_SN>().Where(r => r.SN == verifySN && r.SKUNO.Contains("711-")).OrderBy(r => r.START_TIME, OrderByType.Desc).Any();
                        if (!snExists)
                        {
                            notValidSN.Add(verifySN);
                        }
                    }
                    foreach (string invlaidSN in notValidSN) repairedSNsList.Remove(invlaidSN);


                    UIInputData I = new UIInputData()
                    {
                        MustConfirm = false,
                        Timeout = 30000000,
                        IconType = IconType.Message,
                        UIArea = new string[] { "60%", "70%" },
                        Tittle = "<span style=\"font-size:25px;\">This unit will Rollback!</span> ",
                        Type = UIInputType.Confirm,
                        ErrMessage = "No input"
                    };

                    I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "Parent SN", Value = parentSN });
                    if (repairedSNsList.Count > 0)
                    {
                        string repairedSNsString = string.Join(", ", repairedSNsList);
                        I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "Repaired SN(s)", Value = repairedSNsString });
                        I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "Important Information", Value = "Due to Major Repair, Parent SN " + parentSN + " will be reversed and SN(s) " + repairedSNsString + " will be assigned to new WO to send it back to ICT" });
                    }
                    else
                    {
                        I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "Important Information", Value = "Due to Major Repair, Parent SN " + parentSN + " will be reversed and repaired PCBA needs to send to ICT." });
                    }

                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();

                    if (ret == "NO")
                    {
                        return;
                    }
                    else
                    {
      
                        var objRepairedKPSN = DB.ORM.Queryable<R_SN_KP>()
                            .Where(r => r.SN == parentSN && r.MPN.Contains("711-") && r.SCANTYPE == "KEEP_SN").First();
                        string parentSNID = objParentSN.ID.ToString();

                        ReverseParentSN(objParentSN, Station.BU.ToString(), DB.ORM, Station.LoginUser.EMP_NO);

                        if (repairedSNsList.Count > 0)
                        {
                            foreach (string repairedSN in repairedSNsList)
                            {
                                if (objParentSN.SN != repairedSN)
                                {
                                    var objDaughter = DB.ORM.Queryable<R_SN>().Where(r => r.SN == repairedSN && r.VALID_FLAG == "1").First();
                                    if (objDaughter.SKUNO.Contains("711"))
                                    {
                                        //Do Nothing
                                    }
                                    else
                                    {
                                        ReverseParentSN(objDaughter, Station.BU.ToString(), DB.ORM, Station.LoginUser.EMP_NO);
                                    }
                                }

                                var objRepairedSN = DB.ORM.Queryable<R_SN>().Where(r => r.SN == repairedSN && r.SKUNO.Contains("711-")).OrderBy(r => r.START_TIME, OrderByType.Desc).First();
                                var objOldRepairedSNWO = DB.ORM.Queryable<R_WO_BASE>().Where(w => w.WORKORDERNO == objRepairedSN.WORKORDERNO).First();
                                string WO = AssignWorkorder(objParentSN, objRepairedSN, Station.BU.ToString(), objOldRepairedSNWO, Station.LoginUser.EMP_NO, DB.ORM);
                                SNList.Add(repairedSN);
                                woList.Add(WO);
                            }
                        }

                    }
                    UIInputData E = new UIInputData()
                    {
                        MustConfirm = false,
                        Timeout = 30000000,
                        IconType = IconType.Message,
                        UIArea = new string[] { "65%", "70%" },
                        //Message = "Do you do the JIA Check ?",
                        Tittle = "<span style=\"font-size:25px;\">Please Save this Information</span> ",
                        Type = UIInputType.YesNo,
                        //Name = "test jupe",
                        ErrMessage = "No input"
                        //CBMessage = "OKOK"
                    };

                    if (SNList.Count > 0)
                    {
                        for (int i = 0; i < SNList.Count; i++)
                        {
                            E.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "IMPORTANT", Value="Parent SN " + parentSN + " has been rolled Back. Please Inform Production" });
                            E.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "SN", Value = SNList[i] });
                            E.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "New WO", Value = woList[i] });
                            //I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "Important Information", Value = "Due to Major Repair, Parent SN " + parentSN + " will be reversed and SN(s) " + repairedSNsString + " will be assigned to new WO to send it back to ICT" });
                        }

                        
                        //if (retW == "NO")
                        //{
                        //    return;
                        //    DB.ORM.Ado.RollbackTran(); //ELIMINAR Y AGREGAR EL COMMIT

                        //}
                        //else
                        //{
                        //    // DB.ORM.Ado.CommitTran(); //ELIMINAR Y AGREGAR EL COMMIT
                        //}
                    }
                    else
                    {
                        E.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "Important", Value="Parent SN " + parentSN + " has been rolled Back. Please Inform Production" });

                    }
                    var retW = E.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();

                    Station.AddMessage("FJZMJR000000001", new string[] { parentSN }, StationMessageState.Pass);

                    DB.ORM.Ado.CommitTran(); //ELIMINAR Y AGREGAR EL COMMIT
                }
                else
                {
                    Station.AddMessage("FJZMJR000000002", new string[] { parentSN }, StationMessageState.Pass);
                }
                //string sn2 = sessionSN.Value.ToString()
            }
        }

        public static bool CheckPCBASNRepair(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_PCBA_RELATIONSHIP>()
                .Where(r => r.PARENT_SN == SN && r.WO_FLAG == "0").Any();
        }

        [Obsolete]
        public static void ReverseParentSN(R_SN objParentSN, string BU,  SqlSugarClient DB, string user)
        {
            var state = DB.Ado.Connection.State;
            if(state.ToString() != "Open")
            {
                DB.Ado.BeginTran();
            }

            string parentSNID = objParentSN.ID.ToString();

            try
            {
                #region Reverse Station Detail
                var detail = DB.Queryable<R_SN_STATION_DETAIL>().Where(r => r.R_SN_ID == parentSNID).ToList();
                for (int i = 0; i < detail.Count; i++)
                {
                    detail[i].SN = "REVERSE_" + detail[i].SN;
                    detail[i].WORKORDERNO = "REVERSE_" + detail[i].WORKORDERNO; //Reverse WO Final pa
                    detail[i].VALID_FLAG = "0";
                }
                if (detail.Count > 0)
                {
                    DB.Updateable(detail).ExecuteCommand();
                }
                #endregion

                #region Reverse R_SN
                var rSN = DB.Queryable<R_SN>().Where(r => r.ID == parentSNID).ToList();
                for (int i = 0; i < rSN.Count; i++)
                {
                    rSN[i].SN = "REVERSE_" + rSN[i].SN;
                    rSN[i].WORKORDERNO = "REVERSE_" + rSN[i].WORKORDERNO;
                    rSN[i].VALID_FLAG = "0";
                }
                if (rSN.Count > 0)
                {
                    DB.Updateable(rSN).ExecuteCommand();
                }
                #endregion

                #region Reverse Key Part & Last Level SN

                var kpUpdate = DB.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == parentSNID).ToList();
                for (int i = 0; i < kpUpdate.Count; i++)
                {
                    kpUpdate[i].SN = "REVERSE_" + kpUpdate[i].SN;
                    kpUpdate[i].VALUE = "REVERSE_" + kpUpdate[i].VALUE;
                    kpUpdate[i].VALID_FLAG = 0;
                }
                if (kpUpdate.Count > 0)
                {
                    DB.Updateable(kpUpdate).ExecuteCommand();
                }
                #endregion

                #region Update WO Input Qty
                var updateWO = DB.Queryable<R_WO_BASE>().Where(a => a.WORKORDERNO == objParentSN.WORKORDERNO).First();
                    updateWO.WORKORDER_QTY = updateWO.WORKORDER_QTY - 1;
                    updateWO.INPUT_QTY = updateWO.INPUT_QTY - 1;
                DB.Updateable(updateWO).ExecuteCommand();
                #endregion

                #region Update 711 Valid SN
                var updateSN = DB.Queryable<R_SN>().Where(t => t.SN == objParentSN.SN && t.SKUNO.Contains("711-")).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).First();
                    updateSN.SHIPPED_FLAG = "0";
                    updateSN.VALID_FLAG = "1";
                DB.Updateable(updateSN).ExecuteCommand();
                #endregion

                DB.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                DB.Ado.RollbackTran();
                throw ex;
            }
        }

        public static bool CheckVirtualWOExists(string SKUNO, SqlSugarClient DB)
        {
            return DB.Queryable<R_WO_BASE>()
                .Where(r => r.SKUNO == SKUNO && r.WORKORDERNO.Contains("0024")).Any();
        }

        [Obsolete]
        public static string AssignWorkorder(R_SN objParentSN, R_SN objRepairedSN, string BU, R_WO_BASE objOldRepairedSNWO, string user, SqlSugarClient DB)
        {
            string assignedWO = "";
            string assignedWOID = "";
            try
            {
                var state = DB.Ado.Connection.State;
                if (state.ToString() != "Open")
                {
                    DB.Ado.BeginTran();
                };
                

                #region Assign WO for Repaired SN
                string repairedSNSKU = objRepairedSN.SKUNO.ToString();
                DateTime date = DB.GetDate();
                var objRouteInfo = DB.Queryable<C_ROUTE>().Where(c => c.ROUTE_NAME == "Virtual-WO-Routing-ICT").First();
                

                if (CheckVirtualWOExists(repairedSNSKU, DB))
                {
                    var objWOInfo = DB.Queryable<R_WO_BASE>()
                    .Where(r => r.SKUNO == repairedSNSKU && r.WORKORDERNO.Contains("0024")).First();

                    var updateWO = DB.Queryable<R_WO_BASE>().Where(a => a.ID == objWOInfo.ID).First();
                        updateWO.WORKORDER_QTY = updateWO.WORKORDER_QTY + 1;
                    DB.Updateable(updateWO).ExecuteCommand();

                    string r_snID = MesDbBase.GetNewID(DB, BU, "R_SN");

                    R_SN RSN = new R_SN()
                    {
                        ID = r_snID,
                        SN = objRepairedSN.SN.ToString(),
                        SKUNO = repairedSNSKU,
                        WORKORDERNO = objWOInfo.WORKORDERNO.ToString(),
                        PLANT = "MBGA",
                        ROUTE_ID = objRouteInfo.ID.ToString(),
                        STARTED_FLAG = "1",
                        START_TIME = date,
                        PACKED_FLAG = "0",
                        COMPLETED_FLAG = "0",
                        SHIPPED_FLAG = "0",
                        REPAIR_FAILED_FLAG = "0",
                        CURRENT_STATION = "REWORK",
                        NEXT_STATION = "ICT",
                        CUST_PN = repairedSNSKU,
                        SCRAPED_FLAG = "0",
                        PRODUCT_STATUS = "FRESH",
                        REWORK_COUNT = objRepairedSN.REWORK_COUNT,
                        VALID_FLAG = "1",
                        EDIT_EMP = user,
                        EDIT_TIME = date
                    };
                    DB.Insertable(RSN).ExecuteCommand();

                    string detail_snID = MesDbBase.GetNewID(DB, BU, "R_SN_STATION_DETAIL");

                    R_SN_STATION_DETAIL RSSD = new R_SN_STATION_DETAIL()
                    {
                        ID = detail_snID,
                        R_SN_ID = r_snID,
                        SN = objRepairedSN.SN.ToString(),
                        SKUNO = repairedSNSKU,
                        WORKORDERNO = objWOInfo.WORKORDERNO.ToString(),
                        PLANT = "FJZ",
                        ROUTE_ID = objRouteInfo.ID.ToString(),
                        LINE = "Line1",
                        STARTED_FLAG = "1",
                        START_TIME = date,
                        PACKED_FLAG = "0",
                        COMPLETED_FLAG = "0",
                        SHIPPED_FLAG = "0",
                        REPAIR_FAILED_FLAG = "0",
                        CURRENT_STATION = "REWORK",
                        NEXT_STATION = "ICT",
                        CUST_PN = repairedSNSKU,
                        DEVICE_NAME = "ICT",
                        STATION_NAME = "ICT",
                        PRODUCT_STATUS = "FRESH",
                        REWORK_COUNT = objRepairedSN.REWORK_COUNT,
                        VALID_FLAG = "1",
                        EDIT_EMP = user,
                        EDIT_TIME = date
                    };
                    DB.Insertable(RSSD).ExecuteCommand();

                    assignedWO = objWOInfo.WORKORDERNO.ToString();
                    assignedWOID = objWOInfo.ID;
                }
                else
                {
                    string woID = MesDbBase.GetNewID(DB, BU, "R_WO_BASE");
                    string newWO = MesDbBase.GetNewWorkorder(DB, "JNP711VWO");

                    R_WO_BASE RWB = new R_WO_BASE()
                    {
                        ID = woID,
                        WORKORDERNO = newWO,
                        PLANT = objOldRepairedSNWO.PLANT,
                        RELEASE_DATE = date,
                        DOWNLOAD_DATE = date,
                        PRODUCTION_TYPE = objOldRepairedSNWO.PRODUCTION_TYPE,
                        WO_TYPE = "VIRTUAL",
                        SKUNO = objOldRepairedSNWO.SKUNO,
                        SKU_VER = objOldRepairedSNWO.SKU_VER,
                        SKU_SERIES = objOldRepairedSNWO.SKU_SERIES,
                        SKU_NAME = objOldRepairedSNWO.SKU_NAME,
                        SKU_DESC = objOldRepairedSNWO.SKU_DESC,
                        ROUTE_ID = objRouteInfo.ID,
                        START_STATION = "ICT",
                        CLOSED_FLAG = "0",
                        WORKORDER_QTY = 1,
                        INPUT_QTY = 0,
                        FINISHED_QTY = 0,
                        SCRAPED_QTY = 0,
                        EDIT_EMP = user,
                        EDIT_TIME = date
                    };
                    DB.Insertable(RWB).ExecuteCommand();

                    var objNewWO = DB.Queryable<R_WO_BASE>().Where(w => w.WORKORDERNO == newWO).First();
                    string r_snID = MesDbBase.GetNewID(DB, BU, "R_SN"); //

                    R_SN RSN = new R_SN()
                    {
                        ID = r_snID,
                        SN = objRepairedSN.SN.ToString(),
                        SKUNO = repairedSNSKU,
                        WORKORDERNO = objNewWO.WORKORDERNO.ToString(),
                        PLANT = "MBGA",
                        ROUTE_ID = objRouteInfo.ID.ToString(),
                        STARTED_FLAG = "1",
                        START_TIME = date,
                        PACKED_FLAG = "0",
                        COMPLETED_FLAG = "0",
                        SHIPPED_FLAG = "0",
                        REPAIR_FAILED_FLAG = "0",
                        CURRENT_STATION = "REWORK",
                        NEXT_STATION = "ICT",
                        CUST_PN = repairedSNSKU,
                        SCRAPED_FLAG = "0",
                        PRODUCT_STATUS = "FRESH",
                        REWORK_COUNT = objRepairedSN.REWORK_COUNT,
                        VALID_FLAG = "1",
                        EDIT_EMP = user,
                        EDIT_TIME = date
                    };
                    DB.Insertable(RSN).ExecuteCommand();

                    string detail_snID = MesDbBase.GetNewID(DB, BU, "R_SN_STATION_DETAIL");

                    R_SN_STATION_DETAIL RSSD = new R_SN_STATION_DETAIL()
                    {
                        ID = detail_snID,
                        R_SN_ID = r_snID,
                        SN = objRepairedSN.SN.ToString(),
                        SKUNO = repairedSNSKU,
                        WORKORDERNO = objNewWO.WORKORDERNO.ToString(),
                        PLANT = "MBGA",
                        ROUTE_ID = objRouteInfo.ID.ToString(),
                        LINE = "Line1",
                        STARTED_FLAG = "1",
                        START_TIME = date,
                        PACKED_FLAG = "0",
                        COMPLETED_FLAG = "0",
                        SHIPPED_FLAG = "0",
                        REPAIR_FAILED_FLAG = "0",
                        CURRENT_STATION = "REWORK",
                        NEXT_STATION = "ICT",
                        CUST_PN = repairedSNSKU,
                        DEVICE_NAME = "ICT",
                        STATION_NAME = "ICT",
                        PRODUCT_STATUS = "FRESH",
                        REWORK_COUNT = objRepairedSN.REWORK_COUNT,
                        VALID_FLAG = "1",
                        EDIT_EMP = user,
                        EDIT_TIME = date
                    };
                    DB.Insertable(RSSD).ExecuteCommand();
                    #endregion
                    assignedWO = newWO;
                    assignedWOID = woID;

                }
                #region Set valid flag to reapaired SN in R_REPAIR_PCBA_RELATIONSHIP

                DB.Updateable<R_REPAIR_PCBA_RELATIONSHIP>()
                .UpdateColumns(a => new R_REPAIR_PCBA_RELATIONSHIP()
                {
                    WO_FLAG = "1",
                    WORKORDERNO_ID = assignedWOID
                }).Where(a => a.PARENT_SN == objParentSN.SN.ToString() && a.REPAIRED_SN == objRepairedSN.SN.ToString() && a.WO_FLAG == "0").ExecuteCommand();

                //Update LAST SN VALID FLAG
                #endregion

                #region Set valid flag to 0 to last 711 SN
                var updateLastLvlSN = DB.Queryable<R_SN>().Where(r => r.ID == objRepairedSN.ID).First();
                updateLastLvlSN.VALID_FLAG = "0";
                DB.Updateable(updateLastLvlSN).ExecuteCommand();
                #endregion

                #region Update flag when Parent SN and SN repaired are the same
                if (objParentSN.SN == objRepairedSN.SN)
                {
                    var updateParentSN = DB.Queryable<R_SN>().Where(t => t.SN == objParentSN.SN && t.SKUNO.Contains("711-") && t.WORKORDERNO != assignedWO).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).First();
                    updateParentSN.SHIPPED_FLAG = "0";
                    updateParentSN.VALID_FLAG = "0";
                    DB.Updateable(updateParentSN).ExecuteCommand();
                }
                #endregion
                
                DB.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                DB.Ado.RollbackTran();
                throw ex;
            }
            return assignedWO;
        }

        public static string generateNewWO(SqlSugarClient DB)
        {
            try
            {
                DB.Ado.BeginTran();
            }
            catch (Exception ex)
            {
                DB.Ado.RollbackTran();
                throw ex;
            }
            return "002400000000";
        }
    }
}
