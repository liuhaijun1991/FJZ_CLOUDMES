using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class AgingDataloader
    {
        /// <summary>
        /// 老化工站加載時獲取登錄賬號的樓層信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AgingStationLoadingDataLoador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionFloor = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionFloor == null)
            {
                sessionFloor = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(sessionFloor);
            }
           
            OleExec sfcdb = Station.SFCDB;
            T_c_user tc = new T_c_user(sfcdb, DB_TYPE_ENUM.Oracle);
            Row_c_user u = tc.getC_Userbyempno(Station.LoginUser.EMP_NO, sfcdb, DB_TYPE_ENUM.Oracle);

          
            sessionFloor.Value = u.POSITION_NAME;
           
        }

        /// <summary>
        /// 老化工站加載SN Table,Shelf Table
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingLoadingTableDataloador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionShelfTable = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionShelfTable == null)
            {
                sessionShelfTable = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(sessionShelfTable);
            }
            MESStationSession sessionSNTable = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSNTable == null)
            {
                sessionSNTable = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionSNTable);
            }
            OleExec sfcdb = Station.SFCDB;
            T_R_AGING_SHELF_INFO tra = new T_R_AGING_SHELF_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            DataTable shelfTable = tra.GetScanList(sfcdb, Station.IP);

            T_R_SN_AGING_INFO trs = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            DataTable snTable = trs.GetScanList(sfcdb, Station.IP);
            sessionShelfTable.Value = ConvertToJson.DataTableToJson(shelfTable);
            sessionSNTable.Value = ConvertToJson.DataTableToJson(snTable);

        }

        /// <summary>
        /// 老化工站輸入skuno,加載skuno的老化配置信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AgingStationInputSkuDataLoador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); 
            }
            MESStationSession sessionItem = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionItem == null || sessionItem.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE })); 
            }
            #region output 
            MESStationSession sessionItemName = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionItemName == null)
            {
                sessionItemName = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionItemName);
            }
            MESStationSession sessionAgingTime = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionAgingTime == null)
            {
                sessionAgingTime = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(sessionAgingTime);
            }
            MESStationSession sessionCabinetDes = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (sessionCabinetDes == null)
            {
                sessionCabinetDes = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(sessionCabinetDes);
            }
            MESStationSession sessionPCBADes = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            if (sessionPCBADes == null)
            {
                sessionPCBADes = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                Station.StationSession.Add(sessionPCBADes);
            }
            MESStationSession sessionCabinetInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[6].SESSION_TYPE && s.SessionKey == Paras[6].SESSION_KEY);
            if (sessionCabinetInputQty == null)
            {
                sessionCabinetInputQty = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY };
                Station.StationSession.Add(sessionCabinetInputQty);
            }           
            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[7].SESSION_TYPE && s.SessionKey == Paras[7].SESSION_KEY);
            if (sessionToolType == null)
            {
                sessionToolType = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, SessionKey = Paras[7].SESSION_KEY };
                Station.StationSession.Add(sessionToolType);
            }
            #endregion 
            string inputItem = sessionItem.Value.ToString();
            string cabinet = sessionCabinet.Value.ToString();
            string skuno = "";
            OleExec sfcdb = Station.SFCDB;
            T_R_SN trsn = new T_R_SN(sfcdb, DB_TYPE_ENUM.Oracle);
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            C_AGING_CONFIG_DETAIL ca = null;
            if (inputItem.Length > 10)
            {
                R_SN sn = trsn.GetSNByBoxSN(inputItem, sfcdb);
                if (sn == null)
                {
                    // throw new Exception("輸入SN錯誤！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095017"));

                }
                skuno = sn.SKUNO.Substring(sn.SKUNO.Length - 8, 8);
            }
            else if (inputItem.Length == 6)
            {
                skuno = "03" + inputItem;
            }
            else
            {
                skuno = inputItem;
            }
            if (string.IsNullOrEmpty(skuno))
            {
                //throw new Exception("請輸入機種或序列號！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100448"));
            }
            ca = TCA.GetConfigDetail(sfcdb, cabinet, skuno);
            if (ca == null)
            {
                // throw new Exception("機種料號與老化櫃不匹配，請確認！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100936"));
            }
            Input.Value = skuno;
            //Input.Enable = false;

            sessionItem.Value = ca.ITEM_CODE;
            sessionItemName.Value = ca.ITEM_NAME;
            sessionAgingTime.Value = ca.AGINGTIME;
            sessionCabinetDes.Value = ca.DESCRIPTION1;
            sessionPCBADes.Value = ca.DESCRIPTION2;
            sessionToolType.Value = ca.TOOLS_FLAG.ToUpper();
            sessionCabinetInputQty.Value = ca.SHELF_QTY * ca.ONESHELFQTY;
            if (ca.TOOLS_FLAG.ToUpper() == "Y" && ca.ONESHELFQTY == 1)
            {
                sessionCabinetInputQty.Value = ca.SHELF_QTY;
            }            
        }

        /// <summary>
        /// 老化工站輸入老化框,加載老化框配置信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AgingStationInputShelfDataLoador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
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

            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionToolType == null || sessionToolType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            MESStationSession sessionShelfInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (sessionShelfInputQty == null)
            {
                sessionShelfInputQty = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(sessionShelfInputQty);
            }

            string shelf = sessionShelf.Value.ToString();
            string cabinet = sessionCabinet.Value.ToString();
            string itemCode= sessionItemCode.Value.ToString();
            Station.Inputs.Find(s => s.DisplayName == "Tool").Enable = true;
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_AGING_CONFIG_DETAIL cac = TCA.GetConfigByCabinetAndShelf(Station.SFCDB, cabinet, itemCode, shelf.Substring(shelf.Length - 2, 2));
            if (cac == null)
            {
                // throw new Exception("老化框與老化櫃不匹配，請確認！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101545"));
            }
            if (sessionToolType.Value.ToString().ToUpper() == "Y")
            {
                sessionShelfInputQty.Value = 1;              
                Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "Tool");
            }            
            else
            {
                Station.Inputs.Find(s => s.DisplayName == "Tool").Enable = false;
                sessionShelfInputQty.Value = cac.ONESHELFQTY;
                if (cac.ONESHELFQTY == 1)
                {
                    Station.Inputs.Find(s => s.DisplayName == "Location").Value = "C01";
                    Station.Inputs.Find(s => s.DisplayName == "Location").Enable = false;
                    Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "SN");
                }
                else
                {
                    Station.Inputs.Find(s => s.DisplayName == "Location").Value = "";
                    Station.Inputs.Find(s => s.DisplayName == "Location").Enable = true;
                    Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "Location");
                }
            }
        }

        /// <summary>
        /// 老化工站輸入工具板,加載工具板配置信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AgingStationInputToolDataLoador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionItemCode = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionItemCode == null || sessionItemCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTool = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTool == null || sessionTool.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }           

            MESStationSession sessionToolInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionToolInputQty == null )
            {
                sessionToolInputQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionToolInputQty);
            }
           
            string itemCode = sessionItemCode.Value.ToString();
            string tool = sessionTool.Value.ToString();
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_AGING_CONFIG_DETAIL cac = TCA.GetConfigObject(Station.SFCDB, "", itemCode, "", tool);
            if (cac == null)
            {
                // throw new Exception("輸入的工具板與編碼不匹配，請確認！");

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102526"));
            }
            sessionToolInputQty.Value = cac.ONESHELFQTY;
            if (cac.ONESHELFQTY == 1)
            {
                Station.Inputs.Find(s => s.DisplayName == "Location").Value = "G01";
                Station.Inputs.Find(s => s.DisplayName == "Location").Enable = false;
                Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "SN");
            }
            else
            {
                Station.Inputs.Find(s => s.DisplayName == "Location").Value = "";
                Station.Inputs.Find(s => s.DisplayName == "Location").Enable = true;
                Station.NextInput = Station.Inputs.Find(s => s.DisplayName == "Location");
            }
        }

        /// <summary>
        /// 老化工站輸入位置,加載位置配置信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AgingStationInputLocationDataLoador(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
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

            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionToolType == null || sessionToolType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            MESStationSession sessionLocation = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (sessionLocation == null || sessionLocation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE }));
            }
            string location = sessionLocation.Value.ToString().ToUpper();
            string cabinet = sessionCabinet.Value.ToString();
            string itemCode = sessionItemCode.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            T_C_AGING_CONFIG_DETAIL TCAC = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            if (location.Length != 3)
            {
                // throw new Exception("輸入的工具板的欄位序號長度錯誤!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103210"));
            }
            C_AGING_CONFIG_DETAIL CAC = TCAC.GetConfigObject(sfcdb, cabinet, itemCode, "", "");
            if (CAC == null)
            {
                // throw new Exception("老化柜與料號不匹配！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103925"));
            }
            if (!CAC.SOLT_NO.Contains(location))
            {
                //throw new Exception("該老化框沒有這個欄位!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104349"));
            }
            if (sessionToolType.Value.ToString().ToUpper() == "Y")
            {
                if (!location.StartsWith("G"))
                {
                    //throw new Exception("輸入的不是工具板欄位序號!");

                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105220"));
                }
            }
            else
            {
                if (!location.StartsWith("C"))
                {
                    //throw new Exception("輸入的不是工具板欄位序號!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105220"));
                }
            }
        }
              
    }
}
