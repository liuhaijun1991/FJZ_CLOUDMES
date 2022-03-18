using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using MESDataObject.Common;
using System.Data;
using MESDBHelper;
using MESStation.LogicObject;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class AgingAction
    {
        /// <summary>
        /// 新建老化任務刪除當前IP未提交的掃描記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>  
        /// <param name="Paras"></param>
        public static void DeleteWaitSubmitCabinetAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {          
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_AGING_SHELF_INFO TRA = new T_R_AGING_SHELF_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            TRS.DeleteWaitSubmitListByIP(Station.SFCDB, Station.IP);
            TRA.DeleteWaitSubmitListByIP(Station.SFCDB, Station.IP);         

        }
        /// <summary>
        /// 新建老化任務刪除當前老化柜未放滿的老化框
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DeleteNoFullShelfAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            TRS.DeleteNoFullShelf(Station.SFCDB, Station.IP, sessionCabinet.Value.ToString());
        }

        /// <summary>
        /// 新建老化任務刪除當前老化框未放滿的工具板
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DeleteNoFullToolAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionShelf = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShelf == null || sessionShelf.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            TRS.DeleteNoFullTool(Station.SFCDB, Station.IP, sessionCabinet.Value.ToString(),sessionShelf.Value.ToString());
        }

        /// <summary>
        /// 新建老化任務輸入SN動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingSNInputAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 14)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionShelf = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShelf == null || sessionShelf.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionItemCode = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionItemCode == null || sessionItemCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionTool = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            MESStationSession sessionLocation = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);

            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE }));
            }

            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[6].SESSION_TYPE && s.SessionKey == Paras[6].SESSION_KEY);
            if (sessionToolType == null || sessionToolType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE }));
            }

            MESStationSession sessionCabinetInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[7].SESSION_TYPE && s.SessionKey == Paras[7].SESSION_KEY);
            if (sessionCabinetInputQty == null || sessionCabinetInputQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE }));
            }

            MESStationSession sessionToolInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[8].SESSION_TYPE && s.SessionKey == Paras[8].SESSION_KEY);           

            MESStationSession sessionItemName = Station.StationSession.Find(s => s.MESDataType == Paras[9].SESSION_TYPE && s.SessionKey == Paras[9].SESSION_KEY);
            if (sessionItemName == null || sessionItemName.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE }));
            }

            MESStationSession sessionAgingTime = Station.StationSession.Find(s => s.MESDataType == Paras[10].SESSION_TYPE && s.SessionKey == Paras[10].SESSION_KEY);
            if (sessionAgingTime == null || sessionAgingTime.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE }));
            }

            MESStationSession sessionFloor = Station.StationSession.Find(s => s.MESDataType == Paras[11].SESSION_TYPE && s.SessionKey == Paras[11].SESSION_KEY);
            if (sessionFloor == null || sessionFloor.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE }));
            }

            MESStationSession sessionShelfInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[12].SESSION_TYPE && s.SessionKey == Paras[12].SESSION_KEY);  
            MESStationSession sessionRemark = Station.StationSession.Find(s => s.MESDataType == Paras[13].SESSION_TYPE && s.SessionKey == Paras[13].SESSION_KEY);
            string remark = "";
            if (sessionRemark != null && sessionRemark.Value != null)
            {
                remark = sessionRemark.Value.ToString();
            }

            string cabinet = sessionCabinet.Value.ToString();
            string shelf = sessionShelf.Value.ToString();
            string itemCode = sessionItemCode.Value.ToString();
            string ip = Station.IP;
            string itemName = sessionItemName.Value.ToString();
            string agingTime = sessionAgingTime.Value.ToString();
            string floor = sessionFloor.Value.ToString();
            string toolFlag = sessionToolType.Value.ToString().ToUpper();

            string tool = "";
            string location = "";

            int cabinetInputQty = Convert.ToInt32(sessionCabinetInputQty.Value.ToString());
            int shelfInputQty = 0;
            int toolInputQty = 0;

            
            if (sessionShelfInputQty != null || sessionShelfInputQty.Value != null)
            {
                shelfInputQty = Convert.ToInt32(sessionShelfInputQty.Value.ToString());
            }

            SN snObj = (SN)sessionSN.Value;
            OleExec sfcdb = Station.SFCDB;

            string lot = string.Empty;
            bool IsFull = false;
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_AGING_SHELF_INFO TRAS = new T_R_AGING_SHELF_INFO(sfcdb, DB_TYPE_ENUM.Oracle);

            R_SN_AGING_INFO snAgingInfo = null;
            R_AGING_SHELF_INFO agingShelfInfo = null;


            MESStationInput inputLocation = Station.Inputs.Find(s => s.DisplayName == "Location");
            if (inputLocation.Enable && (sessionLocation == null || sessionLocation.Value == null))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814092726"));
            }

            location = sessionLocation.Value.ToString();
            #region 生成老化批次
            List<R_SN_AGING_INFO> snList = TRSA.GetNoSubmitSNList(sfcdb, Station.IP, cabinet, "", "", "", "");
            if (snList.Count > 0)
            {
                List<string> snLot = snList.Select(r => r.LOT_NO).Distinct().ToList();
                if (snLot.Count != 1)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814092754"));
                }
                lot = snLot.FirstOrDefault();
            }
            else
            {
                List<R_AGING_SHELF_INFO> shelfList = TRAS.GetShelfWaitSubmitList(sfcdb, cabinet, ip);
                if (shelfList.Count > 0)
                {
                    List<string> shelfLot = snList.Select(r => r.LOT_NO).Distinct().ToList();
                    if (shelfLot.Count != 1)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814092754"));
                    }
                    lot = shelfLot.FirstOrDefault();
                }
                else
                {
                    lot = TRSA.GetAgingLot(sfcdb);
                }
            }
            #endregion
            if (cabinetInputQty <= snList.Count)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814093518", new string[] { cabinet }));
            }
            if (toolFlag == "Y")
            {
                if (sessionTool == null || sessionTool.Value == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814094631"));
                }
                if (sessionToolInputQty == null || sessionToolInputQty.Value == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095325"));
                }

                tool = sessionTool.Value.ToString();
                toolInputQty = Convert.ToInt32(sessionToolInputQty.Value.ToString());
                List<R_SN_AGING_INFO> listSNTool = TRSA.GetSNAgingList(sfcdb, ip, cabinet, shelf, tool, "", "", "", "1", "");
                if (toolInputQty <= listSNTool.Count)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095443", new string[] { tool }));
                }
                if (toolInputQty == 1)
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    //snAgingInfo.STARTTIME = ;
                    //snAgingInfo.ENDTIME = "";
                    //snAgingInfo.STARTEMPNO = "";
                    //snAgingInfo.REALFINISHTIME = "";
                    //snAgingInfo.ENDEMPNO = "";
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "2";
                    //snAgingInfo.EVENTPASS = "";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);

                    agingShelfInfo = new R_AGING_SHELF_INFO();
                    agingShelfInfo.ID = TRAS.GetNewID(Station.BU, sfcdb);
                    agingShelfInfo.CABINETNO = cabinet;
                    agingShelfInfo.SHELFNO = shelf;
                    agingShelfInfo.USEQTY = shelfInputQty.ToString();
                    agingShelfInfo.UNUSEQTY = "0";
                    agingShelfInfo.ITEMCODE = itemCode;
                    agingShelfInfo.ITEMNAME = itemName;
                    agingShelfInfo.LOT_NO = lot;
                    agingShelfInfo.IPADDRESS = ip;
                    agingShelfInfo.WORK_FLAG = "2";
                    agingShelfInfo.TOOLSNO = tool;
                    agingShelfInfo.TOOLS_FLAG = toolFlag;
                    agingShelfInfo.REMARK = remark;
                    agingShelfInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    agingShelfInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRAS.Save(sfcdb, agingShelfInfo);

                    IsFull = true;
                    
                }
                else if (toolInputQty == listSNTool.Count + 1)
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "1";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);

                    List<R_SN_AGING_INFO> listNoUse = TRSA.GetNoUseList(sfcdb, ip, cabinet, shelf, tool, lot);
                    agingShelfInfo = new R_AGING_SHELF_INFO();
                    agingShelfInfo.ID = TRAS.GetNewID(Station.BU, sfcdb);
                    agingShelfInfo.CABINETNO = cabinet;
                    agingShelfInfo.SHELFNO = shelf;
                    agingShelfInfo.USEQTY = (shelfInputQty - listNoUse.Count()).ToString();
                    agingShelfInfo.UNUSEQTY = listNoUse.Count().ToString();
                    agingShelfInfo.ITEMCODE = itemCode;
                    agingShelfInfo.ITEMNAME = itemName;
                    agingShelfInfo.LOT_NO = lot;
                    agingShelfInfo.IPADDRESS = ip;
                    agingShelfInfo.WORK_FLAG = "2";
                    agingShelfInfo.TOOLSNO = tool;
                    agingShelfInfo.TOOLS_FLAG = toolFlag;
                    agingShelfInfo.REMARK = remark;
                    agingShelfInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    agingShelfInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRAS.Save(sfcdb, agingShelfInfo);

                    TRSA.UpdateWorkFlag(sfcdb, ip, cabinet, shelf, tool, lot, "1", "2");
                    IsFull = true;                   
                }
                else
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "1";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);
                    IsFull = false;
                }
            }
            else
            {
                List<R_SN_AGING_INFO> listSNShelf = TRSA.GetNoSubmitSNList(sfcdb, ip, cabinet, shelf, "", "", "");
                if (shelfInputQty <= listSNShelf.Count)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095641", new string[] { shelf }));
                }
                if (shelfInputQty == 1)
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "2";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);

                    agingShelfInfo = new R_AGING_SHELF_INFO();
                    agingShelfInfo.ID = TRAS.GetNewID(Station.BU, sfcdb);
                    agingShelfInfo.CABINETNO = cabinet;
                    agingShelfInfo.SHELFNO = shelf;
                    agingShelfInfo.USEQTY = shelfInputQty.ToString();
                    agingShelfInfo.UNUSEQTY = "0";
                    agingShelfInfo.ITEMCODE = itemCode;
                    agingShelfInfo.ITEMNAME = itemName;
                    agingShelfInfo.LOT_NO = lot;
                    agingShelfInfo.IPADDRESS = ip;
                    agingShelfInfo.WORK_FLAG = "2";
                    agingShelfInfo.TOOLSNO = tool;
                    agingShelfInfo.TOOLS_FLAG = toolFlag;
                    agingShelfInfo.REMARK = remark;
                    agingShelfInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    agingShelfInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRAS.Save(sfcdb, agingShelfInfo);

                    IsFull = true;
                }
                else if (shelfInputQty == listSNShelf.Count + 1)
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "1";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);

                    List<R_SN_AGING_INFO> listNoUse = TRSA.GetNoUseList(sfcdb, ip, cabinet, shelf, tool, lot);
                    agingShelfInfo = new R_AGING_SHELF_INFO();
                    agingShelfInfo.ID = TRAS.GetNewID(Station.BU, sfcdb);
                    agingShelfInfo.CABINETNO = cabinet;
                    agingShelfInfo.SHELFNO = shelf;
                    agingShelfInfo.USEQTY = (shelfInputQty - listNoUse.Count()).ToString();
                    agingShelfInfo.UNUSEQTY = listNoUse.Count().ToString();
                    agingShelfInfo.ITEMCODE = itemCode;
                    agingShelfInfo.ITEMNAME = itemName;
                    agingShelfInfo.LOT_NO = lot;
                    agingShelfInfo.IPADDRESS = ip;
                    agingShelfInfo.WORK_FLAG = "2";
                    agingShelfInfo.TOOLSNO = tool;
                    agingShelfInfo.TOOLS_FLAG = toolFlag;
                    agingShelfInfo.REMARK = remark;
                    agingShelfInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    agingShelfInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRAS.Save(sfcdb, agingShelfInfo);

                    TRSA.UpdateWorkFlagByLot(sfcdb, ip, cabinet, shelf, lot, "1", "2");

                    IsFull = true;
                }
                else
                {
                    snAgingInfo = new R_SN_AGING_INFO();
                    snAgingInfo.ID = TRSA.GetNewID(Station.BU, sfcdb);
                    snAgingInfo.SN = snObj.SerialNo;
                    snAgingInfo.WORKORDERNO = snObj.WorkorderNo;
                    snAgingInfo.ITEMCODE = itemCode;
                    snAgingInfo.ITEMNAME = itemName;
                    snAgingInfo.AGINGTIME = agingTime;
                    snAgingInfo.FLOOR = floor;
                    snAgingInfo.CABINETNO = cabinet;
                    snAgingInfo.SHELFNO = shelf;
                    snAgingInfo.TOOLS_FLAG = toolFlag;
                    snAgingInfo.TOOLSNO = tool;
                    snAgingInfo.SLOTNO = location;
                    snAgingInfo.IPADDRESS = ip;
                    snAgingInfo.LOT_NO = lot;
                    snAgingInfo.WORK_FLAG = "1";
                    snAgingInfo.REMARK = remark;
                    snAgingInfo.EDIT_EMP = Station.LoginUser.EMP_NO;
                    snAgingInfo.EDIT_TIME = Station.GetDBDateTime();
                    TRSA.Save(sfcdb, snAgingInfo);
                    IsFull = false;
                }
            }
            sessionLocation.Value = "";
            Station.Inputs.Find(i => i.DisplayName == "Location").Value = "";
            sessionSN.Value = "";
            Station.Inputs.Find(i => i.DisplayName == "SN").Value = "";
            if (sessionRemark != null && sessionRemark.Value != null)
            {
                sessionRemark.Value = "";
            }
            

            if (shelfInputQty == 1)
            {
                if (toolFlag == "Y" && toolInputQty == 1)
                {
                    Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "ShelfNO");
                }
                else if (toolFlag == "Y" && toolInputQty != 1)
                {
                    Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Location");
                }
                else
                {
                    Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "ShelfNO");
                }
            }
            else
            {
                Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Location");
            }

            if (IsFull)
            {
                if (toolFlag == "Y")
                {                    
                    sessionTool.Value = "";                    
                }           
                sessionShelf.Value = "";
                Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "ShelfNO");
            }            
        }
        /// <summary>
        /// 新建老化任務提交動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingSubmitAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionRemark = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            
            MESStationSession sessionFloor = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFloor == null || sessionFloor.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            string cabinet = sessionCabinet.Value.ToString();        
            OleExec sfcdb = Station.SFCDB;
            sfcdb.ThrowSqlExeception = true;
            string ip = Station.IP;
            string remark = "";
            if (sessionRemark != null && sessionRemark.Value != null)
            {
                remark = sessionRemark.Value.ToString();
            }

            T_R_AGING_CABINET_INFO TRAC = new T_R_AGING_CABINET_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_AGING_SHELF_INFO TRAS = new T_R_AGING_SHELF_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);

            //獲取老化時間最長的機種資訊
            string sql = $@"SELECT agingtime, item_code, item_name, lot_no FROM(SELECT a.floor,a.cabinetno,b.item_code,b.item_name, a.lot_no,a.starttime,a.endtime,
                           a.realfinishtime,b.agingtime,a.ipaddress FROM r_sn_aging_info a, c_aging_config_detail b WHERE b.item_code = a.itemcode
                           and a.ipaddress='{ip}' AND a.cabinetno = '{cabinet}' AND a.work_flag = '2' ORDER BY b.agingtime DESC) WHERE rownum< 2";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];

            List<R_SN_AGING_INFO> list = TRSA.GetUseList(sfcdb, ip, cabinet, "", "", "");
            R_AGING_CABINET_INFO info = new R_AGING_CABINET_INFO();
            info.ID = TRAC.GetNewID(Station.BU, sfcdb);
            info.FLOOR = sessionFloor.Value.ToString();
            info.CABINETNO = cabinet;
            info.ITEMCODE = dt.Rows[0]["item_code"].ToString();
            info.ITEMNAME = dt.Rows[0]["item_name"].ToString();
            info.AGINGTIME = dt.Rows[0]["agingtime"].ToString();
            info.QTY = list.Count.ToString();
            //info.STARTTIME = "";
            //info.ENDTIME = "";
            //info.STARTEMPNO = "";
            // info.REALFINISHTIME = "";
            //info.ENDEMPNO = "";
            info.IPADDRESS = ip;
            info.LOT_NO = dt.Rows[0]["lot_no"].ToString();
            info.SUBMITTIME = Station.GetDBDateTime();
            info.SUBMITEMPNO = Station.LoginUser.EMP_NO;
            info.WORK_FLAG = "3";
            info.REMARK = remark;
            info.LASTEDITDT = Station.GetDBDateTime();
            TRAC.Save(sfcdb, info);

            TRSA.UpdateWorkFlag(sfcdb, ip, cabinet, "2", "3");
            TRAS.UpdateWorkFlag(sfcdb, ip, cabinet, "2", "3");

        }
        /// <summary>
        /// 新建老化任務刪除SN動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingDeleteSNAction(MESStationBase Station,MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string sn = sessionSN.Value.ToString();
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            TRSA.DeleteSn(Station.SFCDB, Station.IP, sn);
        }
        /// <summary>
        /// 新建老化任務刪除老化框動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingDeleteShelfAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionShelf = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionShelf == null || sessionShelf.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string shelf = sessionShelf.Value.ToString();
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_AGING_SHELF_INFO TRAS = new T_R_AGING_SHELF_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            TRSA.DeleteShelf(Station.SFCDB, Station.IP, shelf, "2");
            TRSA.DeleteShelf(Station.SFCDB, Station.IP, shelf, "2");
        }
        /// <summary>
        /// 新建老化任務獲取IP最後的老化柜
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingGetLastCabinetAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }            
            string inputName = Paras[0].SESSION_TYPE.ToString();
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_AGING_INFO> list = TRSA.GetNoSubmitSNList(Station.SFCDB, Station.IP);
            if (list.Count > 0)
            {
                List<string> listCabinet = list.OrderByDescending(i => i.ENDTIME).Select(i => i.CABINETNO).Distinct().ToList();
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = listCabinet.FirstOrDefault();
            }
            else
            {
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = "";
            }
        }
        /// <summary>
        /// 新建老化任務獲取未放滿的老化框
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingGetLastShelfAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }            
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string inputName = Paras[0].SESSION_TYPE.ToString();
            string cabinet = sessionCabinet.Value.ToString();
        
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);            
            List<R_SN_AGING_INFO> list = TRSA.GetSNAgingList(Station.SFCDB, Station.IP, cabinet, "", "", "", "", "", "1", "");
            if (list.Count > 0)
            {
                List<string> listShelf = list.OrderByDescending(i => i.EDIT_TIME).Select(i => i.SHELFNO).Distinct().ToList();
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = listShelf.FirstOrDefault();
            }
            else
            {
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = "";
            }
        }
        /// <summary>
        /// 新建老化任務獲取未放滿的工具板
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingGetLastToolAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionShelf = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionShelf == null || sessionShelf.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            string inputName = Paras[0].SESSION_TYPE.ToString();
            string cabinet = sessionCabinet.Value.ToString();
            string shelf = sessionShelf.Value.ToString();
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_AGING_INFO> list = TRSA.GetSNAgingList(Station.SFCDB, Station.IP, cabinet, shelf, "", "", "", "", "1", "Y");
            if (list.Count > 0)
            {
                List<string> listTool = list.OrderByDescending(i => i.EDIT_TIME).Select(i => i.TOOLSNO).ToList();
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = listTool.FirstOrDefault();
            }
            else
            {
                Station.Inputs.Find(r => r.DisplayName == inputName).Value = "";
            }
        }
               
    }
}
