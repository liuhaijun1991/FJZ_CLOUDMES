using MESDataObject;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using System.Text.RegularExpressions;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System.Data;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckAging
    {
        /// <summary>
        /// 新建老化任務,輸入老化櫃,檢查該老化櫃是否還有未提交的資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CabinetInputCheckAction(MESStationBase Station,MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            if (sessionCabinet.Value.ToString().Contains(","))
            {
                //throw new Exception("老化櫃序號不能有逗號！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814093648"));
            }
            if (sessionCabinet.Value.ToString() == "")
            {
                //throw new Exception("請輸入老化櫃！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814094104"));
            }
            OleExec sfcdb = Station.SFCDB;
            string cabinetNo = sessionCabinet.Value.ToString();
            string temp = "";
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);

            if (!TCA.CabinetIsExist(sfcdb, cabinetNo))
            {
                //throw new Exception("老化櫃序號不存在！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814094234"));
            }
            List<R_SN_AGING_INFO> listWaitStart = TRS.GetSNAgingList(sfcdb, "", cabinetNo, "", "", "", "", "", "3", "");
            if (listWaitStart.Count > 0)
            {
                //throw new Exception("該老化櫃正處在待開始老化狀態！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814094436"));
            }
            List<R_SN_AGING_INFO> listAging = TRS.GetSNAgingList(sfcdb, "", cabinetNo, "", "", "", "", "", "4", "");
            if (listAging.Count > 0)
            {
                //throw new Exception("該老化櫃正處在老化中狀態！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095051"));
            }
            List<R_SN_AGING_INFO> listSumbit = TRS.GetNoSubmitSNList(sfcdb,"",cabinetNo,"","","","");
            if (listSumbit.Count > 0)
            {
                List<string> listIP = listSumbit.Select(i => i.IPADDRESS).Distinct().ToList();
                if (listIP.Count > 1)
                {
                    temp = "";
                    foreach (string i in listIP)
                    {
                        temp = temp + "," + i;
                    }
                    //throw new Exception(cabinetNo + " 在" + listIP.Count + "個IP上使用！"+ temp.Substring(1));
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095213", new string[] { cabinetNo, listIP.Count.ToString(), temp.Substring(1) }));
                }
                if (!listIP.FirstOrDefault().Equals(Station.IP))
                {
                    //throw new Exception(cabinetNo + " 在IP " + listIP.FirstOrDefault() + "上使用，請到該IP使用！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814095642", new string[] { cabinetNo, listIP.FirstOrDefault() }));
                }
            }

            List<R_SN_AGING_INFO> list = TRS.GetNoSubmitSNList(sfcdb, Station.IP);
            if (list.Count > 0)
            {
                List<string> listCabinet = list.Select(i => i.CABINETNO).Distinct().ToList();
                if (listCabinet.Count > 1)
                {
                    //throw new Exception("當前電腦" + Station.IP + " 有" + listCabinet.Count + "個老化柜的資料沒有提交");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100202", new string[] { Station.IP, listCabinet.Count.ToString() }));
                }
                if (!cabinetNo.Equals(listCabinet.FirstOrDefault()))
                {
                    StationMessage sm = new StationMessage();
                    //sm.Message = "當前電腦" + Station.IP + "還有未提交的資料老化櫃" + listCabinet.FirstOrDefault() + "，是否要更改老化櫃，并清除當前電腦收集的老化櫃資料？";
                    sm.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100415", new string[] { Station.IP, listCabinet.FirstOrDefault() });
                    sm.State = StationMessageState.Alert;
                    Station.StationMessages.Add(sm);
                    //throw new Exception("當前電腦"+ Station.IP + "還有未提交的資料老化櫃"+ listCabinet.FirstOrDefault() + "，如果要更改老化櫃并清除當前老化櫃" + listCabinet.FirstOrDefault() + "收集的資料，請按ChangeCabinet按鈕");
                }
            }            

        }
     
        /// <summary>
        /// 新建老化任務,輸入老化框,檢查該老化框信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ShelfInputCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
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
            MESStationSession sessionCabinetInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionCabinetInputQty == null || sessionCabinetInputQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            string shelf = sessionShelf.Value.ToString();            
            string cabinet = sessionCabinet.Value.ToString();
            string itemCode = sessionItemCode.Value.ToString();
            if (shelf.Contains(","))
            {
                //throw new Exception("老化框序號不能有逗號！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814093648"));
            }
            if (!shelf.StartsWith("A"))
            {
                //throw new Exception("老化框首字母必須為A!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100821"));
            }
            if (shelf.Substring(0, shelf.Length - 2) != ("A" + cabinet))
            {
                //throw new Exception("老化框與老化櫃不匹配！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101037"));
            }
            OleExec sfcdb = Station.SFCDB;
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_AGING_INFO> listCabinet = TRS.GetNoSubmitSNList(sfcdb, Station.IP, cabinet, "", "", "", "");
            if (listCabinet.Count >= Convert.ToInt32(sessionCabinetInputQty.Value))
            {
                //throw new Exception("老化櫃已放滿，請提交該老化柜到待開始老化狀態！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101245"));
            }
            if (!TCA.ShelfIsConsistent(sfcdb, cabinet, itemCode, shelf.Substring(shelf.Length-2,2)))
            {
                //throw new Exception("老化框與老化櫃不匹配，請確認！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101037"));
            }
            if (TRS.ShelfIsFull(sfcdb, shelf, cabinet))
            {
                //throw new Exception("該老化框已經放滿！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101341"));
            }
            if (TRS.ShelfIsWaitForStartAging(sfcdb, shelf, cabinet))
            {
                //throw new Exception("老化框在等待開始老化狀態！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101610"));
            }
            if (TRS.ShelfIsAginging(sfcdb, shelf, cabinet))
            {
                //throw new Exception("老化框正在老化中！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101739"));
            }
            List<R_SN_AGING_INFO> list = TRS.GetSNAgingList(sfcdb, Station.IP, cabinet, "", "", "", "", "", "1", "");
            if (list.Count > 0)
            {
                List<string> listShelf = list.OrderByDescending(i => i.EDIT_TIME).Select(i => i.SHELFNO).Distinct().ToList();
                if (!shelf.Equals(listShelf.FirstOrDefault()))
                {
                    //sessionShelf.Value = listShelf.FirstOrDefault();
                    StationMessage sm = new StationMessage();
                    //sm.Message = "當前老化櫃還沒有放滿的老化框" + listCabinet.FirstOrDefault() + "，是否要更改老化框，并清除當前電腦未收集的老化框資料？";
                    sm.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101841", new string[] { listCabinet.FirstOrDefault().ToString() });
                    sm.State = StationMessageState.Alert;
                    Station.StationMessages.Add(sm);
                }
            }           
        }

        /// <summary>
        /// 暫時無用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ShelfInputQtyCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //if (Paras.Count != 4)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            //}
            MESStationSession sessionCabinet = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCabinet == null || sessionCabinet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionShelf = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionShelf == null || sessionShelf.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionShelfInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionShelfInputQty == null || sessionShelfInputQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionToolType == null || sessionToolType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            //MESStationSession sessionCabinetInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            //if (sessionCabinetInputQty == null || sessionCabinetInputQty.Value == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE }));
            //}
            //MESStationSession sessionToolInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            //if (sessionToolInputQty == null || sessionToolInputQty.Value == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE }));
            //}

            string regNum = @"^\+?[1-9][0-9]*$";
            string inputQty = sessionShelfInputQty.Value.ToString();
            string shelf = sessionShelf.Value.ToString();
            string cabinet = sessionCabinet.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            string ip = Station.IP;
            string toolType = sessionToolType.Value.ToString().ToUpper();
            //int toolInput = Convert.ToInt32(sessionToolInputQty.Value.ToString());
            //int cabinetInputQty = Convert.ToInt32(sessionCabinetInputQty.Value.ToString());
            if (!Regex.IsMatch(inputQty, regNum))
            {
                //throw new Exception("請輸入非零整數");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102025"));

            }
            string tempShelf = shelf.Substring(shelf.Length - 2, 2);
            T_R_SN_AGING_INFO TRS = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_C_AGING_CONFIG_DETAIL TCA = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            C_AGING_CONFIG_DETAIL CAC = null;
            CAC = TCA.GetConfigByCabinetAndShelf(sfcdb, cabinet, tempShelf);
            if (CAC != null)
            {
                if (Convert.ToDouble(inputQty) > CAC.ONESHELFQTY)
                {
                    //throw new Exception("輸入的數量大於配置的數量");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102135"));
                }
            }

            List<R_SN_AGING_INFO> list = TRS.GetNoSubmitSNList(sfcdb,ip);
            if (list.Count > 0)
            {
                List<R_SN_AGING_INFO> listInfo = TRS.GetSNAgingList(sfcdb, ip, "", shelf, "", "", "", "", "1", "");
                if (listInfo.Count > 0)
                {

                    if (toolType == "Y")
                    {
                        Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Tool");
                    }
                    else
                    {
                        if (inputQty == "1")
                        {
                            Station.Inputs.Find(i => i.DisplayName == "Location").Enable = false;
                            Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "SN");
                        }
                        else
                        {
                            Station.Inputs.Find(i => i.DisplayName == "Location").Enable = true;
                            Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Location");
                        }
                    }

                }
                else
                {
                    if (toolType == "Y")
                    {
                        Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Tool");
                    }
                }
            }
            else
            {                
                if (inputQty == "1")
                {
                    Station.Inputs.Find(i => i.DisplayName == "Location").Enable = false;
                    Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "SN");
                }
                else
                {
                    Station.Inputs.Find(i => i.DisplayName == "Location").Enable = true;
                    Station.NextInput = Station.Inputs.Find(i => i.DisplayName == "Location");
                }
            }

            List<R_SN_AGING_INFO> listShelfInput = TRS.GetSNAgingList(sfcdb, ip, cabinet, shelf, "", "", "", "", "1", "");
            if (listShelfInput.Count > 0)
            {
                if (listShelfInput.Count > Convert.ToUInt32(inputQty))
                {
                    //throw new Exception("輸入的數量小於掃描的數量");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102225"));

                }
                if ((listShelfInput.Count == Convert.ToUInt32(inputQty) || Convert.ToUInt32(inputQty) == 1))
                {
                    //調用SP處理數據邏輯
                    //Obj.Execute_Get_message(DatabaseSet, "CMC_AGING_SP", "", txtfloor.Text, txtcabinetno.Text, txtskuno.Text, txtitemname.Text, txtshelfno.Text, txttoolsno.Text, strUser, "USEQTY", strtoolstype, ip_address, txtAgingQty, txtuseqty.Text, txttoolsqty.Text, txtslotno.Text, txtremark.Text)
                    List<R_SN_AGING_INFO> listIPInput = TRS.GetNoSubmitSNList(sfcdb, ip);
                    //if(cabinetInputQty > listIPInput.Count)
                    //{
                    //    if (toolType == "Y") //使用工具板
                    //    {
                    //        //List<R_SN_AGING_INFO> listToolInput = TRS.GetToolInputList(sfcdb, ip,to);
                    //        // if (toolInput>) //為完,待續
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception("'該老化櫃" + cabinet + "已經數量已經放滿,請更換老化櫃'");
                    //}
                }
            }
        }

        /// <summary>
        /// 新建老化任務輸入工具板序號進行CHECK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ToolInputCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
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
            if (sessionTool == null || sessionTool.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            string cabinet = sessionCabinet.Value.ToString();
            string shelf = sessionShelf.Value.ToString();
            string itemCode = sessionItemCode.Value.ToString();
            string tool = sessionTool.Value.ToString().Substring(0, 6);
            OleExec sfcdb = Station.SFCDB;
            T_C_AGING_CONFIG_DETAIL TCACD = new T_C_AGING_CONFIG_DETAIL(sfcdb,DB_TYPE_ENUM.Oracle);
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            List<C_AGING_CONFIG_DETAIL> listToolConfig = TCACD.GetConfigByItemAndTool(sfcdb, itemCode, tool);
            if (listToolConfig.Count == 0)
            {
                //throw new Exception("工具板與編碼規則不匹配！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102325"));
            }
            if (TRSA.ToolIsFull(sfcdb,tool))
            {
                //throw new Exception("工具板已放滿！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102510"));
            }

            if (TRSA.ToolIsWaitForStartAging(sfcdb, tool))
            {
                //throw new Exception("工具板正在等待開始老化狀態！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102615"));
            }

            if (TRSA.ToolIsAginging(sfcdb, tool))
            {
                //throw new Exception("工具板正在老化中！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102719"));
            }

            List<R_SN_AGING_INFO> listIPAgingTool = TRSA.GetSNAgingList(sfcdb, Station.IP, cabinet, shelf, "", "", "", "", "1", "Y");
            if (listIPAgingTool.Count > 0)
            {
                List<string> listTool = listIPAgingTool.OrderByDescending(i => i.EDIT_TIME).Select(i => i.TOOLSNO).ToList();
                if (!tool.Equals(listTool.FirstOrDefault()))
                {                    
                    StationMessage sm = new StationMessage();
                    //sm.Message = "當前電腦還沒有放滿的工具板" + listTool.FirstOrDefault() + "，是否要更改工具板，并清除當前電腦收集的工具板資料？";
                    sm.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102810", new string[] { Paras[0].SESSION_TYPE });
                    sm.State = StationMessageState.Alert;
                    Station.StationMessages.Add(sm);
                }
            }
        }
        /// <summary>
        /// 新建老化任務輸入位置進行CHECK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LocationInputCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 6)
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

            MESStationSession sessionToolType = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (sessionToolType == null || sessionToolType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE }));
            }

            MESStationSession sessionLocation = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            if (sessionLocation == null || sessionLocation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE }));
            }

            string location = sessionLocation.Value.ToString().ToUpper();
            string cabinet = sessionCabinet.Value.ToString();
            string itemCode = sessionItemCode.Value.ToString();
            string shelf = sessionShelf.Value.ToString();
            string tool = "";
            OleExec sfcdb = Station.SFCDB;
            T_C_AGING_CONFIG_DETAIL TCAC = new T_C_AGING_CONFIG_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            if (location.Length != 3)
            {
                //throw new Exception("輸入的工具板的欄位序號長度錯誤!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102919"));

            }
            C_AGING_CONFIG_DETAIL CAC = TCAC.GetConfigObject(sfcdb, cabinet, itemCode, "", "");
            if (CAC == null)
            {
                //throw new Exception("老化柜與料號不匹配！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103006"));
            }
            if (!CAC.TOOLS_SLOT.Contains(location))
            {
                //throw new Exception("該老化框沒有這個欄位!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103149"));
            }
            if (sessionToolType.Value.ToString().ToUpper() == "Y")
            {
                if (sessionTool == null || sessionTool.Value == null)
                {
                    //throw new Exception ("請輸入工具板！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103234"));
                }
                tool = sessionTool.Value.ToString();
                if (!location.StartsWith("G"))
                {
                    //throw new Exception("輸入的不是工具板欄位序號!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103301"));
                }
                if (TRSA.SoltIsUseed(sfcdb, Station.IP, cabinet, shelf, tool, location))
                {
                    //throw new Exception("該工具板的這個欄位已經有序列號存在!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103331"));
                }
            }
            else
            {
                if (!location.StartsWith("C"))
                {
                    //throw new Exception("輸入的不是工具板欄位序號!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103301"));
                }
                if (TRSA.SoltIsUseed(sfcdb, Station.IP, cabinet, shelf, "", location))
                {
                    //throw new Exception("該老化框的這個欄位已經有序列號存在!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103436"));
                }
            }
        }
       
        /// <summary>
        /// 新建老化任務輸入SN進行CHECK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInputCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }           

            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionItemCode = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionItemCode == null || sessionItemCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            
            string ip = Station.IP;
            string itemCode = sessionItemCode.Value.ToString();
            SN snObj = (SN)sessionSN.Value;
            if(snObj.SerialNo== "EMPTY" || snObj.SerialNo== "ERROR")
            {
                return;
            }
            OleExec sfcdb = Station.SFCDB;
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);
            T_C_ITEMCODE_MAPPING_EMS TCIM = new T_C_ITEMCODE_MAPPING_EMS(sfcdb, DB_TYPE_ENUM.Oracle);
            R_SN_AGING_INFO objSNAging = TRSA.GetSNAgingObj(sfcdb, snObj.SerialNo);
            T_R_SN TRSN = new T_R_SN(sfcdb, DB_TYPE_ENUM.Oracle);
            string HWItem = TCIM.Get_Customer_Partno("KP_NO", snObj.SkuNo, sfcdb);
            if (objSNAging != null)
            {
                if (objSNAging.IPADDRESS == ip)
                {
                    //throw new Exception("此SN已經在本電腦掃描過");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103505"));

                }
                else
                {
                    //throw new Exception("此SN已經在" + ip + "掃描過");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103815", new string[] { ip }));
                }
            }
            if (!itemCode.Equals(HWItem) && itemCode.IndexOf(HWItem) <= 0)
            {
                //throw new Exception("輸入的序號的機種與輸入的ITEMCODE不匹配!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103859"));
            }

        }
       
        /// <summary>
        /// 新建老化任務檢查SN 315膠固化時間
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingGlueCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            SN snObject = (SN)sessionSN.Value;
            string sql =$@"SELECT COUNT(*) FROM mes1.c_station_kp WHERE kp_no IN('WN90020221', '0405002-0190102') AND p_no = '{snObject.SkuNo}'";
            DataTable table = Station.APDB.ExecSelect(sql).Tables[0];
            if (Convert.ToInt32(table.Rows[0][0].ToString()) > 0)
            {
                sql = $@" SELECT COUNT(*) FROM mfsysevent WHERE sysserialno='{snObject.SerialNo}' AND eventname = 'ASSY' AND (SYSDATE - scandatetime) * 24 >= 1";
                table = Station.APDB.ExecSelect(sql).Tables[0];
                if (Convert.ToInt32(table.Rows[0][0].ToString()) == 0)
                {
                    //throw new Exception("315膠固化時間不夠不能上老化！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103943"));
                }
            }
        }
      
        /// <summary>
        ///  新建老化柜任務,提交前進行檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NewAgingSubmitCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            
            string cabinet = sessionCabinet.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DB_TYPE_ENUM.Oracle);

            List<R_SN_AGING_INFO> list = TRSA.GetSNAgingList(sfcdb,Station.IP,cabinet,"","","","","","1","");
            if (list.Count > 0)
            {
                //throw new Exception("此電腦還有老化框或者工具板未收集滿資料！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104030"));
            }
        }
    }
}
