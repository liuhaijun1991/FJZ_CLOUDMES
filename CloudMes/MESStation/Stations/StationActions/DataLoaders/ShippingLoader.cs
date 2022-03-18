using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using SqlSugar;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class ShippingLoader
    {
        /// <summary>
        /// HWT Shipping Station TO Data Loader
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShippingStationTODataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionTONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionTONO == null)
            {
                sessionTONO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = "", SessionKey = Paras[0].SESSION_KEY, ResetInput = null, Value = "" };
                Station.StationSession.Add(sessionTONO);
            }
            MESStationSession sessionTOList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTOList == null)
            {
                sessionTOList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionTOList);
            }
            MESDataObject.Module.HWT.T_R_TO_HEAD_HWT TRTH = new MESDataObject.Module.HWT.T_R_TO_HEAD_HWT(Station.SFCDB, Station.DBType);
            sessionTOList.Value = TRTH.GetWaitShippingToData(Station.SFCDB, "");
        }
        /// <summary>
        /// HWT Shipping Station DN Data Loader
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShippingStationDNDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 9)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionDNList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDNList == null || sessionDNList.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTONO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTONO == null)
            {
                sessionTONO = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionTONO);
            }              

            MESStationSession sessionDNItem = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionDNItem == null)
            {
                sessionDNItem = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionDNItem);
            }

            MESStationSession sessionSKUNO = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSKUNO == null)
            {
                sessionSKUNO = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = "", SessionKey = Paras[3].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionSKUNO);
            }

            MESStationSession sessionGTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionGTQTY == null)
            {
                sessionGTQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = "", SessionKey = Paras[4].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionGTQTY);
            }

            MESStationSession sessionREALQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionREALQTY == null)
            {
                sessionREALQTY = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = "", SessionKey = Paras[5].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionREALQTY);
            }

            MESStationSession sessionDNNO = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionDNNO == null || sessionDNNO.Value == null)
            {
                sessionDNNO = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = "", SessionKey = Paras[6].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionDNNO);
            }

            MESStationSession sessionTOHeadObject = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (sessionTOHeadObject == null)
            {
                sessionTOHeadObject = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = "", SessionKey = Paras[7].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionTOHeadObject);
            }

            MESStationSession sessionDNItemObject = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (sessionDNItemObject == null)
            {
                sessionDNItemObject = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = "", SessionKey = Paras[8].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionDNItemObject);
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            Newtonsoft.Json.Linq.JObject inputObj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(sessionDNList.Value.ToString());
            if (inputObj["DN_NO"] == null)
            {
                throw new MESReturnMessage(" Please select the DN!");
            }
            if (inputObj["DN_ITEM"] == null)
            {
                throw new MESReturnMessage(" Please select the DN Item!");
            }
            string dn_no = inputObj["DN_NO"].ToString();
            string dn_item = inputObj["DN_ITEM"].ToString();

            MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT TRTD = new MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_DN_DETAIL TRDD = new MESDataObject.Module.HWT.T_R_DN_DETAIL(sfcdb, dbtype);
            MESDataObject.Module.HWT.R_TO_DETAIL_HWT TODetail = TRTD.GetDetailByDNNO(sfcdb, dn_no);
            MESDataObject.Module.HWT.R_DN_DETAIL DNDetail = TRDD.GetDetailByDNNO(sfcdb, dn_no, dn_item);
            sessionTOHeadObject.Value = TODetail;
            sessionDNItemObject.Value = DNDetail;

            if (TODetail == null)
            {
                //throw new MESReturnMessage( dn_no + " 對應的TO不存在！");

                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145459", new string[] { "DN:" + dn_no });
            }
            if (DNDetail == null)
            {
                // throw new MESReturnMessage(dn_no + " 不存在！");

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160555", new string[] { "DN:" + dn_no }));
            }           
           
            sessionTONO.Value = TODetail.TO_NO;
            sessionDNNO.Value = DNDetail.DN_NO;
            sessionDNItem.Value = DNDetail.DN_ITEM_NO;
            sessionSKUNO.Value = DNDetail.P_NO;
            sessionGTQTY.Value = DNDetail.P_NO_QTY;
            sessionREALQTY.Value = DNDetail.REAL_QTY;
        }

        //add by Winster FJZ JUNIPER TruckLoad Open Loading
        public static void JNPOpenLoading(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 6)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                TONO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Paras[0].VALUE };
                Station.StationSession.Add(TONO);
            }
            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOQTY == null)
            {
                TOQTY = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(TOQTY);
            }
            else
            {
                TOQTY.Value = Paras[1].VALUE;
            }
            MESStationSession OPENFLAG = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (OPENFLAG == null || OPENFLAG.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession TrailerNumber = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TrailerNumber == null)
            {
                TrailerNumber = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = Paras[3].VALUE };
                Station.StationSession.Add(TrailerNumber);
            }
            else
            {
                TrailerNumber.Value = Paras[3].VALUE;
            }
            MESStationSession Newpallet = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (Newpallet == null)
            {
                Newpallet = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = Paras[4].VALUE };
                Station.StationSession.Add(Newpallet);
            }
            else
            {
                Newpallet.Value = Paras[4].VALUE;
            }
            MESStationSession NewpalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (NewpalletQty == null)
            {
                NewpalletQty = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY, Value = Paras[5].VALUE };
                Station.StationSession.Add(NewpalletQty);
            }
            else
            {
                NewpalletQty.Value = Paras[5].VALUE;
            }

            OleExec sfcdb = Station.SFCDB;
            string StrTONO = "";
            int Stoqty = 0, NewplQty = 0;
            DataTable dtDetail = new DataTable();
            R_JUNIPER_TRUCKLOAD_DETAIL TODetail = null;
            T_R_JUNIPER_TRUCKLOAD_TO JNPOpenTONO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_JUNIPER_TRUCKLOAD_DETAIL JNPOpenTODetail = new T_R_JUNIPER_TRUCKLOAD_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            if (OPENFLAG.Value.ToString() == "S")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210426180535"));
            }
            else if (OPENFLAG.Value.ToString() == "N")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210426180535"));
            }
            else
            {
                if (TONO.Value == null)
                {
                    TOQTY.Value = "0";
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210426180931"));
                }
                else
                {
                    StrTONO = TONO.Value.ToString();
                    Stoqty = JNPOpenTODetail.CheckQTY(StrTONO, Station.BU, Station.SFCDB);
                    TOQTY.Value = Convert.ToString(Stoqty);

                    TODetail = JNPOpenTODetail.CheckTONOData(StrTONO, Station.BU, Station.SFCDB);
                    if (TODetail != null)
                    {
                        dtDetail = JNPOpenTODetail.GetOpenTOdata(StrTONO, Station.BU, Station.SFCDB);
                        if (dtDetail != null)
                        {
                            TrailerNumber.Value = dtDetail.Rows[0]["TRAILER_NUM"].ToString();
                            Newpallet.Value = dtDetail.Rows[0]["NEW_PACK_NO"].ToString();
                            NewplQty = JNPOpenTODetail.GetNewPLQty(StrTONO, Newpallet.Value.ToString(), Station.BU, Station.SFCDB);
                            NewpalletQty.Value = NewplQty.ToString();
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210426180931"));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210426180931"));
                    }

                }

                //TrailerNumber.Value = "CH123";
                //Newpallet.Value = "PLJ2021400149";
                //NewpalletQty.Value = "5";
            }

        }

        public static void JNPPalletListLoading(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                TONO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Paras[0].VALUE };
                Station.StationSession.Add(TONO);
            }
            MESStationSession Newpallet = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Newpallet == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession PalletList = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PalletList == null)
            {
                PalletList = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(PalletList);
            }
            MESStationSession newPalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (newPalletQty == null)
            {
                newPalletQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = Paras[3].VALUE };
                Station.StationSession.Add(newPalletQty);
            }

            OleExec _DB = Station.SFCDB;
            T_R_JUNIPER_TRUCKLOAD_DETAIL JNPOpenTODetail = new T_R_JUNIPER_TRUCKLOAD_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var palletlist = JNPOpenTODetail.GetOpenTOPalletListOfNewPallet(TONO.Value.ToString(), Newpallet.Value.ToString(), Station.BU, _DB);
            PalletList.Value = palletlist;
            newPalletQty.Value = palletlist.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetWaitToShipDataDistinct(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                var res = Station.SFCDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, OrderByType.Desc)
                    .GroupBy((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME,rds.DN_NO,rds.SKUNO })
                    .Select((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME, rds.DN_NO,rds.SKUNO })
                    .Distinct().ToList();                

                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
                s.Value = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Combine dn to ship out loading ship data to output table
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CombineShipoutLoadingShipData(MESPubLab.MESStation.MESStationBase Station,
           MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region CheckDn Is Shiped
            Newtonsoft.Json.Linq.JObject inputObj =
                (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(Input.Value.ToString());
            var dnNo = inputObj["DnNo"].ToString();
            var dnLine = inputObj["DnLine"].ToString();
            var rDnData = Station.SFCDB.ORM
                .Queryable<R_DN_STATUS, R_TO_DETAIL>((rds, rtd) => rds.DN_NO == rtd.DN_NO && rds.DN_NO == dnNo && rds.DN_LINE == dnLine)
                .Select((rds, rtd) => new { rds, rtd.TO_NO }).ToList();
            if (rDnData.Count != 1)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dnNo, dnLine }));
            var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList();
            #endregion
           
            #region make table
            MESStationSession tableDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (tableDataSession == null)
            {
                tableDataSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(tableDataSession);
            }
            var outputTable = Station.StationOutputs.Find(t => t.NAME == Paras[0].SESSION_TYPE);
            if (outputTable == null)
            {
                throw new Exception($@"No output [{Paras[0].SESSION_TYPE}]");
            }
            DataTable table;
            if (tableDataSession.Value != null)
            {
                table = (DataTable)tableDataSession.Value;
            }
            else
            {
                table = new DataTable();
                table.Columns.Add("TO_NO");
                table.Columns.Add("DN_NO");
                table.Columns.Add("DN_ITEM");
                table.Columns.Add("SKU_NO");
                table.Columns.Add("GT_QTY");
                table.Columns.Add("REAL_QTY");
            }

            //多DN合并掃描出貨
            if (table.Rows.Count > 0)
            {
                T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                bool bCombinedSkuno = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", rDnData.FirstOrDefault().rds.SKUNO, Station.SFCDB);
                bool bCombinedAll = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", "ALL", Station.SFCDB);
                if (bCombinedAll || bCombinedSkuno)
                {

                    //料號要一樣
                    var skuData = table.AsEnumerable();
                    IEnumerable<string> skuRow = skuData.Select(s => s.Field<string>("SKU_NO")).Distinct().ToList();
                    if (skuRow.Count() > 1)
                    {
                        throw new Exception($@"more skuno in the waiting list.");
                    }
                    if (skuRow.Count() == 1 && !rDnData.FirstOrDefault().rds.SKUNO.Equals(skuRow.First()))
                    {
                        throw new Exception($@"[{rDnData.FirstOrDefault().rds.SKUNO}] skuno not same as {skuRow.First()}");
                    }
                    //TO要一樣
                    var toData = table.AsEnumerable();
                    IEnumerable<string> toRow = toData.Select(s => s.Field<string>("TO_NO")).Distinct().ToList();
                    if (toRow.Count() > 1)
                    {
                        throw new Exception($@"more skuno in the waiting list.");
                    }
                    if (toRow.Count() == 1 && !rDnData.FirstOrDefault().TO_NO.Equals(toRow.First()))
                    {
                        throw new Exception($@"[{rDnData.FirstOrDefault().TO_NO}] skuno not same as {toRow.First()}");
                    }

                    var dnRow = table.Select($@" DN_NO='{rDnData.FirstOrDefault().rds.DN_NO}' and DN_ITEM='{rDnData.FirstOrDefault().rds.DN_LINE}'");
                    if (dnRow.Length == 0)
                    {
                        DataRow row = table.NewRow();
                        row["TO_NO"] = rDnData.FirstOrDefault().TO_NO;
                        row["DN_NO"] = rDnData.FirstOrDefault().rds.DN_NO;
                        row["DN_ITEM"] = rDnData.FirstOrDefault().rds.DN_LINE;
                        row["SKU_NO"] = rDnData.FirstOrDefault().rds.SKUNO;
                        row["GT_QTY"] = rDnData.FirstOrDefault().rds.QTY;
                        row["REAL_QTY"] = rShipDetail.Count;
                        table.Rows.Add(row);
                    }
                    else
                    {
                        throw new Exception($@"{rDnData.FirstOrDefault().rds.DN_NO} already exist.");
                    }
                }
                else
                {
                    table.Rows.Clear();
                    DataRow row = table.NewRow();
                    row["TO_NO"] = rDnData.FirstOrDefault().TO_NO;
                    row["DN_NO"] = rDnData.FirstOrDefault().rds.DN_NO;
                    row["DN_ITEM"] = rDnData.FirstOrDefault().rds.DN_LINE;
                    row["SKU_NO"] = rDnData.FirstOrDefault().rds.SKUNO;
                    row["GT_QTY"] = rDnData.FirstOrDefault().rds.QTY;
                    row["REAL_QTY"] = rShipDetail.Count;
                    table.Rows.Add(row);
                }
            }
            else
            {
                DataRow row = table.NewRow();
                row["TO_NO"] = rDnData.FirstOrDefault().TO_NO;
                row["DN_NO"] = rDnData.FirstOrDefault().rds.DN_NO;
                row["DN_ITEM"] = rDnData.FirstOrDefault().rds.DN_LINE;
                row["SKU_NO"] = rDnData.FirstOrDefault().rds.SKUNO;
                row["GT_QTY"] = rDnData.FirstOrDefault().rds.QTY;
                row["REAL_QTY"] = rShipDetail.Count;
                table.Rows.Add(row);
            }

            tableDataSession.Value = table;
            Station.NextInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");

            MESStationInput input = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
            input.Value = Station.SFCDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, OrderByType.Desc)
                    .GroupBy((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME, rds.DN_NO, rds.SKUNO })
                    .Select((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME, rds.DN_NO, rds.SKUNO })
                    .Distinct().ToList();
            #endregion
        }

    }
}
