using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MESDataObject;
using MESDataObject.Module;
using MESStation;
using MESPubLab.MESStation;
using MESPubLab.MESInterface;
using MESDBHelper;
using System.Data;
using System.Timers;
using MESStation.Interface;
using MESStation.Interface.SAPRFC;
using MESPubLab.SAP_RFC;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Common;
using MESDataObject.Constants;

namespace MESStation.Interface
{
    public class DownLoad_WO: MesAPIBase
    {
        T_C_INTERFACE C_Interface;
        //Row_C_INTERFACE Row_C_Interface;
        //OleExec Sfcdb;
        //string IP = "";

        protected APIInfo FDownload = new APIInfo()
        {
            FunctionName = "GetInterfaceStatus",
            Description = "Get Interface Status",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PROGRAM", InputType = "STRING", DefaultValue = "INTERFACE"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo FGetOneWOFromSAP = new APIInfo()
        {
            FunctionName = "GetOneWOFromSAP",
            Description = "Get One WO From SAP",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo FDownloadOneWOFromSAP = new APIInfo()
        {
            FunctionName = "DownloadOneWOFromSAP",
            Description = "Download One WO From SAP",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public DownLoad_WO()
        {
            this.Apis.Add(FDownload.FunctionName, FDownload);
        }

        public Dictionary<string, string> GetInterfacePara()
        {
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            Dic_Interface.Add("Start", "0");
            Dic_Interface.Add("CurrentDate", "2017-12-21");
            Dic_Interface.Add("NextDate", "2017-12-22");
            Dic_Interface.Add("ConsoleIP", "127.0.0.1");

            return Dic_Interface;
        }

        public void GetInterfaceStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            try
            {
                string Program_Name = Data["PROGRAM"].ToString();
                List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_Interface = new T_C_INTERFACE(Sfcdb, DB_TYPE_ENUM.Oracle);
                ListInterface = C_Interface.GetInterfaceStatus(BU, IP, Program_Name, "ALL", LoginUser.EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);

                StationReturn.Data = ListInterface;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                }
            }
        }
        /// <summary>
        /// DonwLoad WO From SAP
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Download_WO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //Interface Interface=new Interface();
           
            bool BoolStart = InterfacePublicValues.TimerStarted;
            InterfacePublicValues.InterfaceTimerStart("Interface");

            OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string status = Data["STATUS"].ToString();
                string ProgramName = Data["PROGRAM_NAME"].ToString();
                string ItemName = Data["ITEM_NAME"].ToString();
                string StrDate = Data["DATE_TIME"].ToString();
                string IP = Data["IP"].ToString();
                Interface InterFace = new Interface();
                string Local_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
                if (IP != Local_IP)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000103");

                }
                else if (InterFace.CheckInterfaceRun(ProgramName, ItemName, Sfcdb))// Ensure only one interface console program run;
                {
                    try
                    {
                        InterFace.LockItem(ProgramName, ItemName, Sfcdb);
                        Download(ItemName, StrDate);
                        AutoConvert();//Regular WO AutoConvert
                        InterFace.UpdateNextRunTime(ProgramName, ItemName, Sfcdb);//Update Next RunTime;
                        InterFace.UnLockItem(ProgramName, ItemName, Sfcdb);
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000102");
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                        InterFace.UnLockItem(ProgramName, ItemName, Sfcdb);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = ex.Message.ToString();
                        throw (ex);
                    }
                }
            }catch(Exception exp)
            {
                throw exp;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
        }

        public void Download(string ItemName, string Date)
        {
            string StrSql = "";
            OleExec sfcdb;
            DataTable RFC_Table = new DataTable();
            T_C_TAB_COLUMN_MAP C_TAB_COLUMN_MAP;
            DataObjectBase Row;
            T_R_WO_HEADER R_WO_HEADER;
            T_R_WO_ITEM R_WO_ITEM;
            T_R_WO_TEXT R_WO_TEXT;
            T_R_WO_BASE R_WO_BASE;
            T_C_SKU C_SKU;
            T_C_PARAMETER C_PARAMETER;
            //Row_C_PARAMETER Row_PARAMETER;
            Dictionary<string, string> DicPara = new Dictionary<string, string>();
            string StrColumn = "";
            string StrValue = "";
            string[] StrColumn_Name;
            string[] StrColumn_Value;
            string StrWo = "";
            bool Exist_WO_Flag = false;
            bool Exist_WO_Base_Flag = false;
            bool Exist_SKU_Flag = false;
            bool DownLoad_Auto = false;

            if (string.IsNullOrEmpty(StrWo))
            {
                DownLoad_Auto = true;
            }

            sfcdb = this.DBPools["SFCDB"].Borrow();
            C_PARAMETER = new T_C_PARAMETER(sfcdb, DB_TYPE_ENUM.Oracle);
            DicPara = C_PARAMETER.Get_Interface_Parameter_2(ItemName, sfcdb, DB_TYPE_ENUM.Oracle);
            this.DBPools["SFCDB"].Return(sfcdb);

            SAPRFC.ZRFC_SFC_NSG_0001B Zrfc_SFC_NSG_001B = new SAPRFC.ZRFC_SFC_NSG_0001B(StrWo);
            Zrfc_SFC_NSG_001B.SetValue("PLANT", DicPara["PLANT"]);//NHGZ,WDN1//WDN1,WSL3
            Zrfc_SFC_NSG_001B.SetValue("SCHEDULED_DATE", Date);
            Zrfc_SFC_NSG_001B.SetValue("RLDATE", Date);
            Zrfc_SFC_NSG_001B.SetValue("COUNT", DicPara["COUNT"]);
            Zrfc_SFC_NSG_001B.SetValue("CUST", DicPara["CUST"]);
            Zrfc_SFC_NSG_001B.SetValue("IN_CNF", DicPara["IN_CNF"]);  //IN_CNF=0,Download WO not Confirmed
            Zrfc_SFC_NSG_001B.CallRFC();

            for (int i = 0; i < Zrfc_SFC_NSG_001B.ReturnDatatableByIndex.Count; i++)
            {
                string ErrorMessage = "";
                switch (Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i].TableName.ToString().ToUpper())
                {
                    case "ITAB":
                        if (Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[0].Rows.Count > 0)
                        {
                            ErrorMessage = Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[0].Rows[0][1].ToString();
                            throw new Exception(ErrorMessage);
                        }
                        break;
                    case "WO_HEADER":
                        sfcdb = this.DBPools["SFCDB"].Borrow();

                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_HEADER", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_HEADER.CheckWoHeadByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            C_SKU = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = C_SKU.CheckSku(RFC_Table.Rows[m][3].ToString(), sfcdb);

                            if (!Exist_WO_Flag && Exist_SKU_Flag && !Exist_WO_Base_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    //StrColumn_Value[j] = ReplaceSpecialChar(RFC_Table.Rows[m][StrColumn_Name[j]].ToString());
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }
                                StrSql = " insert into R_WO_HEADER（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");";

                                R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_HEADER.EditWoHead(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);

                        break;
                    case "WO_ITEM":
                        StrSql = "";
                        sfcdb = this.DBPools["SFCDB"].Borrow();
                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_ITEM", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_ITEM = new T_R_WO_ITEM(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_ITEM.CheckWoItemByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), RFC_Table.Rows[m]["MATNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            C_SKU = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = C_SKU.CheckSku(RFC_Table.Rows[m][8].ToString(), sfcdb);

                            if (!Exist_WO_Flag && Exist_SKU_Flag && !Exist_WO_Base_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    //StrColumn_Value[j] = ReplaceSpecialChar(RFC_Table.Rows[m][StrColumn_Name[j]].ToString());
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }

                                StrSql = "insert into R_WO_ITEM（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");\n";

                                R_WO_ITEM = new T_R_WO_ITEM(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_ITEM.EditWoItem(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);
                        break;
                    case "WO_TEXT":
                        StrSql = "";
                        sfcdb = this.DBPools["SFCDB"].Borrow();
                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_TEXT", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_TEXT = new T_R_WO_TEXT(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_TEXT.CheckWoTextByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = R_WO_HEADER.CheckWoHeadByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), true, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            if (!Exist_WO_Flag && !Exist_WO_Base_Flag && Exist_SKU_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }
                                StrSql = "insert into R_WO_TEXT（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");";

                                R_WO_TEXT = new T_R_WO_TEXT(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_TEXT.EditWoText(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);
                        break;
                }
            }
        }

        public void AutoConvert()
        {
            T_R_WO_HEADER R_WO_HEADER;
            T_R_WO_BASE R_WO_BASE;
            T_C_SKU C_SKU;
            T_C_ROUTE C_ROUTE;
            OleExec Sfcdb = null;
            string Rows_ID = "";
            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                R_WO_HEADER = new T_R_WO_HEADER(Sfcdb, DB_TYPE_ENUM.Oracle);
                DataTable dt = R_WO_HEADER.GetConvertWoList(Sfcdb, DB_TYPE_ENUM.Oracle);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        R_WO_BASE = new T_R_WO_BASE(Sfcdb, DB_TYPE_ENUM.Oracle);
                        Rows_ID = R_WO_BASE.GetNewID(BU, Sfcdb);
                        Row_R_WO_BASE Rows = (Row_R_WO_BASE)R_WO_BASE.NewRow();

                        C_SKU = new T_C_SKU(Sfcdb, DB_TYPE_ENUM.Oracle);
                        C_SKU Sku = C_SKU.GetSku(dr["MATNR"].ToString(), Sfcdb);

                        C_ROUTE = new T_C_ROUTE(Sfcdb, DB_TYPE_ENUM.Oracle);
                        Row_C_ROUTE Rows_Route = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(dr["MATNR"].ToString(), Sfcdb, DB_TYPE_ENUM.Oracle);

                        if (Rows != null && Sku != null && Rows_Route != null)
                        {
                            Rows.ID = Rows_ID;
                            Rows.WORKORDERNO = dr["AUFNR"].ToString();
                            Rows.SKUNO = dr["MATNR"].ToString();
                            Rows.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                            Rows.CUST_PN = Sku.CUST_PARTNO;
                            Rows.WORKORDER_QTY = Convert.ToDouble(dr["GAMNG"]);
                            Rows.SKU_VER = dr["REVLV"].ToString();
                            Rows.SKU_NAME = Sku.SKU_NAME;
                            Rows.SKU_DESC = Sku.DESCRIPTION;
                            Rows.ROHS = dr["ROHS_VALUE"].ToString();
                            Rows.ROUTE_ID = Rows_Route.ID; //路由應該加版本//Rows.KP_LIST_ID
                            Rows.CLOSED_FLAG = "0";
                            Rows.EDIT_EMP = "LLF";
                            string str = Sfcdb.ExecSQL(Rows.GetInsertString(DB_TYPE_ENUM.Oracle));
                        }
                    }
                }
            }catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(Sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                }
            }
        }

        public void GetOneWOFromSAP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //OleExec SFCDB = null;
            try
            {
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString().Trim();
                string plant = System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Plant"];
                ZRFC_SFC_NSG_0001HW zrfc_sfc_nsg_0001hw = new ZRFC_SFC_NSG_0001HW(BU);
                zrfc_sfc_nsg_0001hw.SetValue(wo,wo, "", "ALL", "", plant, "", "");
                zrfc_sfc_nsg_0001hw.CallRFC();
                DataTable tableWOHeader = zrfc_sfc_nsg_0001hw.GetTableValue("WO_HEADER");
                DataTable tableWOItem = zrfc_sfc_nsg_0001hw.GetTableValue("WO_ITEM");
                DataTable tableWOText = zrfc_sfc_nsg_0001hw.GetTableValue("WO_TEXT");
                Dictionary<string, object> woInfo = new Dictionary<string, object>();
                woInfo.Add("WO_HEADER", tableWOHeader);
                woInfo.Add("WO_ITEM", tableWOItem);
                woInfo.Add("WO_TEXT", tableWOText);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";                
                StationReturn.Data = woInfo;
                //SFCDB = this.DBPools["SFCDB"].Borrow();
            }
            catch (Exception e)
            {
                //if (DBPools["SFCDB"] != null)
                //{
                //    this.DBPools["SFCDB"].Return(SFCDB);
                //}
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }           
        }

        public void DownloadOneWOFromSAP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString().Trim();
                string plant = System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Plant"];
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(SFCDB, DBTYPE);
                string msg = "";
                T_R_SN t_r_sn = new T_R_SN(SFCDB, DBTYPE);
                DataTable tableLoading = t_r_sn.GetSNByWo(wo, SFCDB);
                if (tableLoading.Rows.Count > 0)
                {
                    throw new Exception($@"{wo} Have Been Loading!");
                }
                ZRFC_SFC_NSG_0001HW zrfc_sfc_nsg_0001hw = new ZRFC_SFC_NSG_0001HW(BU);
                zrfc_sfc_nsg_0001hw.SetValue(wo, wo, "", "ALL", "", plant, "", "");
                zrfc_sfc_nsg_0001hw.CallRFC();
                DataTable tableWOHeader = zrfc_sfc_nsg_0001hw.GetTableValue("WO_HEADER");
                DataTable tableWOItem = zrfc_sfc_nsg_0001hw.GetTableValue("WO_ITEM");
                DataTable tableWOText = zrfc_sfc_nsg_0001hw.GetTableValue("WO_TEXT");

                if (tableWOHeader.Rows.Count > 0 && BU != "HWD")
                {
                    if (tableWOHeader.Rows[0]["FTRMI"].ToString().Trim().StartsWith("0000"))
                    {
                        throw new Exception($@"{wo} Not Release!");
                    }
                }
                if (tableWOHeader.Rows[0]["MATNR"].ToString().Trim() == "")
                {
                    throw new Exception($@"Get Skuno[MATNR] From SAP Error!");
                }
                //SaveWOHeader
                string saveMsg = "";
                SaveWOHeader(tableWOHeader, tableWOItem, SFCDB, DBTYPE,this.BU, wo, IP, LoginUser.EMP_NO, false, ref saveMsg);
                if (t_r_wo_base.IsExist(wo, SFCDB))
                {
                    //如果工單已經轉則先刪掉再重轉
                    SFCDB.ORM.Deleteable<R_WO_BASE>().Where(r => r.WORKORDERNO == wo).ExecuteCommand();
                }
                if (BU == "HWD")
                {
                    //HWD 轉工單                    
                    HWDDOConvert(BU,wo, null, LoginUser.EMP_NO, IP, SFCDB, DBTYPE, false, ref msg);
                }
                else if (BU == "VERTIV")
                {
                    //VERTIV 轉工單
                    T_R_WO_TYPE t_r_wo_type = new T_R_WO_TYPE(SFCDB, DBTYPE);
                    bool isRework = t_r_wo_type.GetWOTypeByPREFIX("REWORK", wo.Substring(0,6),SFCDB);
                    if (!isRework)
                    { 
                        VERTIVConvertWO(BU,wo, null, LoginUser.EMP_NO, IP, SFCDB, DBTYPE, true, ref msg);
                    }
                }
                if (saveMsg != "")
                {
                    throw new Exception(saveMsg);
                }
                if (msg != "")
                {
                    throw new Exception(msg);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.MessagePara.Add("Download WO");
                StationReturn.Message = "Download WO";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// HWD 轉工單方法如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="_wo">按工單轉</param>
        /// <param name="_woType">按工單類型轉</param>
        /// <param name="emp_no">工號</param>
        /// <param name="SFCDB">數據庫連接</param>
        public void HWDDOConvert(string bu,string _wo, string[] _woType,string emp_no,string ip, OleExec SFCDB, DB_TYPE_ENUM dbtype,bool IsRelease, ref string msg)
        {           
            T_R_WO_HEADER TRWH = new T_R_WO_HEADER(SFCDB, dbtype);
            T_C_SKU TCS = new T_C_SKU(SFCDB, dbtype);
            T_C_SERIES T_Series = new T_C_SERIES(SFCDB, dbtype);
            T_R_WO_TYPE TRWT = new T_R_WO_TYPE(SFCDB, dbtype);
            T_C_ROUTE TCR = new T_C_ROUTE(SFCDB, dbtype);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(SFCDB, dbtype);
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(SFCDB, dbtype);
            T_C_KEYPART Keypart = new T_C_KEYPART(SFCDB, dbtype);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, dbtype);
            T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, dbtype);
            T_C_CONTROL TCC = new T_C_CONTROL(SFCDB, dbtype);
            T_R_FAI TRF = new T_R_FAI(SFCDB, dbtype);

            C_SKU skuObj = null;
            Row_C_ROUTE rowRoute = null;
            Row_R_WO_BASE rowWOBase = null;
            R_WO_TYPE rowWOType = null;
            C_SERIES C_Series = null;
            R_MES_LOG converLog = null;
            C_CONTROL control =new C_CONTROL();
            C_ROUTE Croute = new C_ROUTE();
            R_FAI rfai = new R_FAI();

            this.BU = bu;
            List<C_ROUTE_DETAIL> routeDetailList = new List<C_ROUTE_DETAIL>();
            List<C_KEYPART> keypartList = new List<C_KEYPART>();
            List<string> keypartIDList = new List<string>();
            List<C_ROUTE_DETAIL> linkStationList = new List<C_ROUTE_DETAIL>();

            DataTable dtConvertWO = new DataTable();
            string wo = "";
            string sql = "", series = "";bool WoIsExistInFai;
            string result = string.Empty;
            string result1 = string.Empty;
            string result2 = string.Empty;
            if (_wo != "")
            {
                if (IsRelease)
                {
                    dtConvertWO = TRWH.GetConvertWoTableByWO(SFCDB, DB_TYPE_ENUM.Oracle, _wo);
                }
                {
                    dtConvertWO = TRWH.GetConvertWoTableByWONoRelease(SFCDB, DB_TYPE_ENUM.Oracle, _wo);
                }
            }
            else
            {
                dtConvertWO = TRWH.GetConvertWoTable(SFCDB, DB_TYPE_ENUM.Oracle, _woType);
            }           
            if (dtConvertWO.Rows.Count > 0)
            {
                foreach (DataRow row in dtConvertWO.Rows)
                {
                    //rowSku = null;
                    rowRoute = null;
                    rowWOBase = null;
                    rowWOType = null;
                    routeDetailList = null;
                    keypartList = null;
                    keypartIDList = null;
                    linkStationList = null;
                    C_Series = null;
                    converLog = null;
                    sql = "";
                    series = "";
                    try
                    {
                        wo = row["AUFNR"].ToString(); 
                        sql = $@"select * from r_mes_log where DATA3='DELETE' and PROGRAM_NAME ='WEB' and DATA1='{wo}' and FUNCTION_NAME='CutWO' ";
                        DataTable de = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        //已經被刪除過的工單不再次轉
                        if (de.Rows.Count > 0)
                        {
                            continue;
                        }

                        skuObj = TCS.GetSku(row["MATNR"].ToString(), SFCDB);
                        if (skuObj == null)
                        {
                            throw new Exception(" sku " + row["MATNR"].ToString() + " not exist");
                        }
                        //2018.10.22 PE杜軍要求不卡工單版本與機種版本是否一致
                        //if (!string.Equals(Sku.VERSION.ToString(), row["REVLV"].ToString()))
                        //{
                        //    throw new Exception(" The sku version is not the same," + Sku.VERSION.ToString() + "," + row["REVLV"].ToString());
                        //}
                        if (skuObj.C_SERIES_ID != null && skuObj.C_SERIES_ID.ToString() != "")
                        {
                            C_Series = T_Series.GetDetailById(SFCDB, skuObj.C_SERIES_ID);
                            if (C_Series == null)
                            {
                                throw new Exception(" the series of " + row["MATNR"].ToString() + " not exist");
                            }
                            series = C_Series.SERIES_NAME;
                        }
                        else
                        {
                            series = "HWD_DEFAULT";
                        }

                        if (BU == "HWT")
                        {
                            //HWT的工單類型通過工單前綴來判斷
                            rowWOType = TRWT.GetWOTypeByWO_HWT(SFCDB, row["AUFNR"].ToString());
                        }
                        else
                        {
                            rowWOType = TRWT.GetWOTypeByWO(SFCDB, row["AUART"].ToString());
                        }

                        if (rowWOType == null)
                        {
                            throw new Exception("get wo type fail");
                        }
                        rowRoute = (Row_C_ROUTE)TCR.GetRouteBySkuno(skuObj.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                        if (rowRoute == null)
                        {
                            throw new Exception(" the route of " + row["MATNR"].ToString() + " not exist");
                        }
                        routeDetailList = TCRD.GetByRouteIdOrderBySEQASC(rowRoute.ID, SFCDB);
                        if (routeDetailList == null || routeDetailList.Count == 0)
                        {
                            throw new Exception("get route detail fail by " + rowRoute.ID);
                        }

                        keypartIDList = t_c_kp_list.GetListIDBySkuno(skuObj.SKUNO, SFCDB);
                        if (keypartIDList.Count == 0)
                        {
                            linkStationList = routeDetailList.Where(link => link.STATION_NAME == "LINK").ToList();
                            if (linkStationList != null && linkStationList.Count > 0)
                            {
                                keypartList = Keypart.GetKeypartListBySkuVersion(SFCDB, row["MATNR"].ToString(), row["REVLV"].ToString());
                                keypartIDList = keypartList.Select(id => id.KEYPART_ID).Distinct().ToList();
                                if (keypartIDList == null || keypartIDList.Count != 1)
                                {
                                    throw new Exception("get keypart id fail by skuno:" + row["MATNR"].ToString() + " and version:" + row["REVLV"].ToString());
                                }
                            }
                        }
                        else if (keypartIDList.Count != 1)
                        {
                            throw new Exception("skuno:" + row["MATNR"].ToString() + " have more keypart id");
                        }
                        control = TCC.GetRouteInControlbyWO(rowRoute.ID, SFCDB);

                        if (control != null)
                        {
                            WoIsExistInFai = TRF.CheckWoHaveDoneFai(wo, control.CONTROL_LEVEL, SFCDB);
                            if (WoIsExistInFai)
                            {
                                rfai.ID = TRF.GetNewID(BU, SFCDB);
                                rfai.FAITYPE = "WORKORDER";
                                rfai.STATUS = "0";
                                rfai.WORKORDERNO = wo;
                                rfai.CREATEBY = emp_no;
                                rfai.CREATETIME = TRWB.GetDBDateTime(SFCDB);
                                result = SFCDB.ORM.Insertable<R_FAI>(rfai).ExecuteCommand().ToString();

                                T_R_FAI_STATION RFAIS = new T_R_FAI_STATION(SFCDB, dbtype);
                                R_FAI_STATION rfais = new R_FAI_STATION();
                                rfais.ID = RFAIS.GetNewID(BU, SFCDB);
                                rfais.FAIID = rfai.ID;
                                rfais.STARTSTATION = "BIP";
                                rfais.FAISTATION = control.CONTROL_LEVEL;
                                rfais.CREATEBY = emp_no;
                                rfais.CREATETIME = TRWB.GetDBDateTime(SFCDB);
                                result1 = SFCDB.ORM.Insertable<R_FAI_STATION>(rfais).ExecuteCommand().ToString();

                                T_R_FAI_STATION RFAISD = new T_R_FAI_STATION(SFCDB, dbtype);
                                R_FAI_DETAIL rfaisd = new R_FAI_DETAIL();
                                rfaisd.ID = RFAISD.GetNewID(BU, SFCDB);
                                rfaisd.FAISTATIONID = rfais.ID;
                                rfaisd.CREATEBY = emp_no;
                                rfaisd.CREATETIME = TRWB.GetDBDateTime(SFCDB);
                                result2 = SFCDB.ORM.Insertable<R_FAI_DETAIL>(rfaisd).ExecuteCommand().ToString();
                                

                            }
                        }

                        rowWOBase = (Row_R_WO_BASE)TRWB.NewRow();
                        rowWOBase.ID = TRWB.GetNewID(BU, SFCDB);
                        rowWOBase.WORKORDERNO = row["AUFNR"].ToString();
                        rowWOBase.PLANT = row["WERKS"].ToString();
                        rowWOBase.RELEASE_DATE = TRWB.GetDBDateTime(SFCDB);
                        rowWOBase.DOWNLOAD_DATE = TRWB.GetDBDateTime(SFCDB);
                        rowWOBase.PRODUCTION_TYPE = "BTO";//HWD原有邏輯寫死                       
                        rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                        rowWOBase.SKUNO = row["MATNR"].ToString();
                        rowWOBase.SKU_VER = row["REVLV"].ToString();
                        rowWOBase.SKU_SERIES = series;
                        rowWOBase.SKU_NAME = skuObj.SKU_NAME;
                        rowWOBase.SKU_DESC = skuObj.DESCRIPTION;
                        rowWOBase.CUST_PN = skuObj.CUST_PARTNO;
                        rowWOBase.CUST_PN_VER = row["CHARG"].ToString();//HWT需要這個欄位，原來為空
                        rowWOBase.CUSTOMER_NAME = skuObj.CUST_SKU_CODE;
                        rowWOBase.ROUTE_ID = rowRoute.ID;
                        rowWOBase.START_STATION = routeDetailList[0].STATION_NAME;
                        rowWOBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                        rowWOBase.CLOSED_FLAG = "0";
                        rowWOBase.WORKORDER_QTY = Convert.ToDouble(row["GAMNG"]);
                        rowWOBase.INPUT_QTY = 0;
                        rowWOBase.FINISHED_QTY = 0;
                        rowWOBase.SCRAPED_QTY = 0;
                        rowWOBase.STOCK_LOCATION = row["LGORT"].ToString();
                        rowWOBase.PO_NO = "";
                        rowWOBase.CUST_ORDER_NO = row["ABLAD"].ToString();
                        rowWOBase.ROHS = row["ROHS_VALUE"].ToString();
                        rowWOBase.EDIT_EMP = emp_no;
                        rowWOBase.EDIT_TIME = TRWB.GetDBDateTime(SFCDB);
                        SFCDB.ThrowSqlExeception = true;
                        sql = rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle));
                        SFCDB.CommitTrain();
 
                    }
                    catch (Exception ex)
                    {
                        converLog = new R_MES_LOG();
                        converLog.ID = TRML.GetNewID(BU, SFCDB);
                        converLog.PROGRAM_NAME = "MESStation.Interface";
                        converLog.CLASS_NAME = "DownLoad_WO";
                        converLog.FUNCTION_NAME = "HWDDOConvert";
                        converLog.LOG_MESSAGE = row["AUFNR"].ToString() + ";ConvertWO fail," + ex.Message.ToString();
                        converLog.LOG_SQL = sql;
                        converLog.DATA1 = row["AUFNR"].ToString();
                        converLog.DATA2 = ip;
                        converLog.DATA3 = "CONVERT";
                        converLog.EDIT_EMP = emp_no;
                        converLog.EDIT_TIME = TRML.GetDBDateTime(SFCDB);
                        TRML.InsertMESLogOld(converLog, SFCDB);
                        msg = converLog.LOG_MESSAGE;
                    }
                }
            }
        }
        /// <summary>
        /// VERTIV 轉工單方法如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="_wo"></param>
        /// <param name="_woType"></param>
        /// <param name="emp_no"></param>
        /// <param name="ip"></param>
        /// <param name="SFCDB"></param>
        /// <param name="dbtype"></param>
        public void VERTIVConvertWO(string bu,string _wo, string[] _woType, string emp_no, string ip, OleExec SFCDB, DB_TYPE_ENUM dbtype, bool IsRelease, ref string msg)
        {
            T_R_WO_HEADER t_r_wo_header = new T_R_WO_HEADER(SFCDB, dbtype);
            T_C_SKU t_c_sku = new T_C_SKU(SFCDB, dbtype);
            T_C_SERIES t_c_series = new T_C_SERIES(SFCDB, dbtype);
            T_R_WO_TYPE t_r_wo_type = new T_R_WO_TYPE(SFCDB, dbtype);
            T_R_WO_BASE t_wo_base = new T_R_WO_BASE(SFCDB, dbtype);
            T_C_ROUTE t_c_route = new T_C_ROUTE(SFCDB, dbtype);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(SFCDB, dbtype);
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(SFCDB, dbtype);
            T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, dbtype);
            T_R_FAI TRF = new T_R_FAI(SFCDB, dbtype);
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, dbtype);

            //Row_C_ROUTE rowRoute;
            R_WO_TYPE rowWOType;
            Row_R_WO_BASE rowWOBase;
            C_SERIES C_Series;
            C_SKU Sku = null;
            R_MES_LOG converLog = null;
            C_ROUTE Croute = new C_ROUTE();
            R_FAI rfai = new R_FAI();
            R_SN_LOCK LOCK = null;
            List<R_F_CONTROL> RFC;

            DataTable dtConvertWO = new DataTable();
            List<C_ROUTE_DETAIL> routeDetailList;          
            List<string> keypartIDList;

            this.BU = bu;
            string sql = "", series = ""; ; bool WoIsExistInFai;
            string result = string.Empty;
            string result1 = string.Empty;
            string result2 = string.Empty;
            string route_name = string.Empty;
            if (_wo != "")
            {
                if (IsRelease)
                {
                    dtConvertWO = t_r_wo_header.GetConvertWoTableByWO(SFCDB, DB_TYPE_ENUM.Oracle, _wo);
                }
                else
                {
                    dtConvertWO = t_r_wo_header.GetConvertWoTableByWONoRelease(SFCDB, DB_TYPE_ENUM.Oracle, _wo);
                }
            }
            else
            {
                dtConvertWO = t_r_wo_header.GetConvertWoTable(SFCDB, DB_TYPE_ENUM.Oracle, _woType);
            }
            if (dtConvertWO.Rows.Count > 0)
            {
                foreach (DataRow row in dtConvertWO.Rows)
                {                   
                    //rowRoute = null;
                    rowWOBase = null;
                    rowWOType = null;
                    routeDetailList = null;                   
                    keypartIDList = null;
                    C_Series = null;
                    sql = "";
                    series = "";
                    try
                    {
                        Sku = t_c_sku.GetSku(row["MATNR"].ToString().Trim(), row["REVLV"].ToString().Trim(), SFCDB);
                        if (Sku == null)
                        {
                            throw new Exception(" sku " + row["MATNR"].ToString().Trim() + ",version " + row["REVLV"].ToString().Trim() + " not exist");
                        }
                        if (Sku.C_SERIES_ID != null && Sku.C_SERIES_ID.ToString() != "")
                        {
                            C_Series = t_c_series.GetDetailById(SFCDB, Sku.C_SERIES_ID);
                            if (C_Series == null)
                            {
                                throw new Exception(" the series of " + row["MATNR"].ToString() + " not exist");
                            }
                            series = C_Series.SERIES_NAME;
                        }
                        else
                        {
                            series = "VERTIV_DEFAULT";
                        }
                        rowWOType = t_r_wo_type.GetWOTypeByWO(SFCDB, row["AUART"].ToString());
                        if (rowWOType == null)
                        {
                            throw new Exception("get wo type fail");
                        }
                        //檢查機種是否設置拋帳點 Add By ZHB 2020年8月26日08:26:04
                        T_C_SAP_STATION_MAP t_cssm = new T_C_SAP_STATION_MAP(SFCDB, dbtype);
                        List<C_SAP_STATION_MAP> cssmList = t_cssm.GetSAPStationMapBySkuOrderBySAPCodeASC(row["MATNR"].ToString().Trim(), SFCDB);
                        if (cssmList == null || cssmList.Count == 0)
                        {
                            throw new Exception($@" sku:{row["MATNR"].ToString().Trim()} have no setup sap station mapping");
                        }

                        //BPD 根據工單類型查找路由
                        if (BU.Equals("BPD") && rowWOType.WORKORDER_TYPE.Equals("RMA"))
                        {
                            Croute = t_c_route.GetRouteBySkuIdAndWoType(Sku.ID, SFCDB, rowWOType.WORKORDER_TYPE);
                        }
                        else
                        {
                            Croute = t_c_route.GetRouteBySkuIdAndWoType(Sku.ID, SFCDB);
                        }
                        if (Croute == null)
                        {
                            throw new Exception(" the route of " + row["MATNR"].ToString() + " not exist");
                        }
                        route_name = Croute.ROUTE_NAME;
                        if (route_name.Contains("CTO"))
                        {
                            bool ss = SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((csk, css, ccu) => csk.ID == css.ID && css.CUSTOMER_ID == ccu.ID)
                                      .Where((csk, css, ccu) => csk.SKUNO == row["MATNR"].ToString().Trim() && ccu.DESCRIPTION == "NETGEAR ODM").Select((csk, css, ccu) => csk).Any();
                            if (ss)
                            {
                                string strSql = $@"select * from c_sku_label where skuno = '{row["MATNR"].ToString().Trim()}' AND STATION = 'P-BOX' AND EDIT_TIME < sysdate -3";
                                DataTable dtt = SFCDB.RunSelect(strSql).Tables[0];
                                if (dtt.Rows.Count > 0)
                                {
                                    //throw new Exception("skuno:" + row["MATNR"].ToString() + "P-BOX LABEL更新日期超過三天,請聯系PE/QE確認處理 ");
                                    throw new Exception("skuno:" + row["MATNR"].ToString() + "P-BOX LABEL update date is more than three days, please contact PE/QE for confirmation ");
                                }
                            }
                        }
                        //rowRoute = (Row_C_ROUTE)t_c_route.GetRouteBySkuno(Sku.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                        //if (rowRoute == null)
                        //{
                        //    throw new Exception(" the route of " + row["MATNR"].ToString() + " not exist");
                        //}
                        routeDetailList = t_c_route_detail.GetByRouteIdOrderBySEQASC(Croute.ID, SFCDB);
                        if (routeDetailList == null || routeDetailList.Count == 0)
                        {
                            throw new Exception("get route detail fail by " + Croute.ID);
                        }
                        keypartIDList = t_c_kp_list.GetListIDBySkuno(Sku.SKUNO, SFCDB);
                        if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                        {
                            throw new Exception("skuno:" + row["MATNR"].ToString() + " have more keypart id");
                        }

                        T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
                        bool CheckKp = TRWOLOG.CheckKpSetCount(row["AUFNR"].ToString(), SFCDB);
                        bool checkBU = TRFC.CheckUserFunctionExist("WOKPComfirm", "WOKPComfirm", this.BU, SFCDB);
                        if (!CheckKp && checkBU)
                        {
                            throw new Exception("The quantity of KP in this workorderno is inconsistent with that of SAP BOM or that of Sn Kp in recent production.Please contact QE for confirmation!");
                        }

                        bool  Csd = TRFC.CheckUserFunctionExist("FAI_CONFIG", "FAI_CONFIG", row["MATNR"].ToString(), SFCDB);
                        
                        if (!Csd)
                        {
                            RFC = TRFC.GetListByFcv("FAI_CONFIG", "FAI_CONFIG", row["MATNR"].ToString(), SFCDB);
                            if (RFC.Count>0) {
                                WoIsExistInFai = TRF.CheckWoHaveDoneFai(row["AUFNR"].ToString(), RFC[0].EXTVAL, SFCDB);
                                if (WoIsExistInFai)
                                {
                                    rfai.ID = TRF.GetNewID(BU, SFCDB);
                                    rfai.FAITYPE = "WORKORDER";
                                    rfai.STATUS = "0";
                                    rfai.WORKORDERNO = row["AUFNR"].ToString();
                                    rfai.CREATEBY = emp_no;
                                    rfai.CREATETIME = t_wo_base.GetDBDateTime(SFCDB);
                                    result = SFCDB.ORM.Insertable<R_FAI>(rfai).ExecuteCommand().ToString();

                                    T_R_FAI_STATION RFAIS = new T_R_FAI_STATION(SFCDB, dbtype);
                                    R_FAI_STATION rfais = new R_FAI_STATION();
                                    rfais.ID = RFAIS.GetNewID(BU, SFCDB);
                                    rfais.FAIID = rfai.ID;
                                    rfais.STARTSTATION = "";
                                    rfais.FAISTATION = RFC[0].EXTVAL;
                                    rfais.CREATEBY = emp_no;
                                    rfais.CREATETIME = t_wo_base.GetDBDateTime(SFCDB);
                                    result1 = SFCDB.ORM.Insertable<R_FAI_STATION>(rfais).ExecuteCommand().ToString();

                                    T_R_FAI_DETAIL RFAISD = new T_R_FAI_DETAIL(SFCDB, dbtype);
                                    R_FAI_DETAIL rfaisd = new R_FAI_DETAIL();
                                    rfaisd.ID = RFAISD.GetNewID(BU, SFCDB);
                                    rfaisd.FAISTATIONID = rfais.ID;
                                    rfaisd.STATUS = "0";
                                    rfaisd.CREATEBY = emp_no;
                                    rfaisd.CREATETIME = t_wo_base.GetDBDateTime(SFCDB);
                                    result2 = SFCDB.ORM.Insertable<R_FAI_DETAIL>(rfaisd).ExecuteCommand().ToString();

                                }
                            }                        
                        }

                        RFC = TRFC.GetListByFcv("RMAPREFIX_CHECK", "WO", row["MATNR"].ToString().Substring(0, 6), SFCDB);
                        if (RFC.Count > 0)
                        {
                            foreach (R_F_CONTROL rev in RFC)
                            {
                                SFCDB.ORM.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK"),
                                    WORKORDERNO = row["AUFNR"].ToString(),
                                    LOCK_EMP = "SYSTEM",
                                    LOCK_REASON = "RMA產品WO未验证自动锁定",
                                    LOCK_STATUS = "1",
                                    LOCK_TIME = DateTime.Now,
                                    TYPE = "WO",
                                    LOCK_STATION = rev.EXTVAL
                                }).ExecuteCommand();
                            }
                        }
                        rowWOBase = (Row_R_WO_BASE)t_wo_base.NewRow();
                        rowWOBase.ID = t_wo_base.GetNewID(BU, SFCDB);
                        rowWOBase.WORKORDERNO = row["AUFNR"].ToString();
                        rowWOBase.PLANT = row["WERKS"].ToString();
                        rowWOBase.RELEASE_DATE = t_wo_base.GetDBDateTime(SFCDB);
                        rowWOBase.DOWNLOAD_DATE = t_wo_base.GetDBDateTime(SFCDB);
                        rowWOBase.PRODUCTION_TYPE = "BTO";//沒有確定先寫死                       
                        rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                        rowWOBase.SKUNO = row["MATNR"].ToString();
                        rowWOBase.SKU_VER = row["REVLV"].ToString();
                        rowWOBase.SKU_SERIES = series;
                        rowWOBase.SKU_NAME = Sku.SKU_NAME;
                        rowWOBase.SKU_DESC = Sku.DESCRIPTION;
                        rowWOBase.CUST_PN = Sku.CUST_PARTNO;
                        rowWOBase.CUST_PN_VER = "";
                        rowWOBase.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                        rowWOBase.ROUTE_ID = Croute.ID;
                        rowWOBase.START_STATION = routeDetailList[0].STATION_NAME;
                        rowWOBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                        rowWOBase.CLOSED_FLAG = "0";
                        rowWOBase.WORKORDER_QTY = Convert.ToDouble(row["GAMNG"]);
                        rowWOBase.INPUT_QTY = 0;
                        rowWOBase.FINISHED_QTY = 0;
                        rowWOBase.SCRAPED_QTY = 0;
                        rowWOBase.STOCK_LOCATION = row["LGORT"].ToString();
                        rowWOBase.PO_NO = "";
                        rowWOBase.CUST_ORDER_NO = row["ABLAD"].ToString();
                        rowWOBase.ROHS = row["ROHS_VALUE"].ToString();
                        rowWOBase.EDIT_EMP = emp_no;
                        rowWOBase.EDIT_TIME = t_wo_base.GetDBDateTime(SFCDB);
                        SFCDB.ThrowSqlExeception = true;
                        sql = rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle));
                        SFCDB.CommitTrain();

                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "ConvertWO", ip + ";" + row["AUFNR"].ToString() + ";ConvertWO fail," + ex.Message.ToString(), sql, "interface");
                        converLog = new R_MES_LOG();
                        converLog.ID = TRML.GetNewID(BU, SFCDB);
                        converLog.PROGRAM_NAME = "MESStation.Interface";
                        converLog.CLASS_NAME = "DownLoad_WO";
                        converLog.FUNCTION_NAME = "VERTIVConvertWO";
                        converLog.LOG_MESSAGE = row["AUFNR"].ToString() + ";ConvertWO fail," + ex.Message.ToString();
                        converLog.LOG_SQL = sql;
                        converLog.DATA1 = row["AUFNR"].ToString();
                        converLog.DATA2 = ip;
                        converLog.DATA3 = "CONVERT";
                        converLog.EDIT_EMP = emp_no;
                        converLog.EDIT_TIME = TRML.GetDBDateTime(SFCDB);
                        TRML.InsertMESLogOld(converLog, SFCDB);
                        msg = converLog.LOG_MESSAGE;
                    }
                }
            }
        }

        /// <summary>
        /// 保存從SAP上下載下來的WO_HERDER,如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="dtWOHeader"></param>
        /// <param name="dtWOItem"></param>
        /// <param name="SFCDB"></param>
        /// <param name="dbtype"></param>
        /// <param name="wo"></param>
        /// <param name="ip"></param>
        /// <param name="emp_no"></param>
        /// <param name="autoDownLoad"></param>
        public void SaveWOHeaderJnp(DataTable dtWOHeader, DataTable dtWOItem, OleExec SFCDB, DB_TYPE_ENUM dbtype, string bu, string wo, string ip, string emp_no, bool autoDownLoad, ref string out_msg)
        {
            string sql = "";
            bool woIsExist = false;
            DataRow[] rowWOHeader = null;
            DataRow[] rowWOItem = null;
            bool skuIsExist = false;
            T_C_SKU t_c_sku = new T_C_SKU(SFCDB, dbtype);
            T_R_WO_HEADER t_r_wo_header = new T_R_WO_HEADER(SFCDB, dbtype);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, dbtype);

            R_MES_LOG saveHeaderLog = null;

            if (wo != "")
            {
                try
                {
                    rowWOHeader = dtWOHeader.Select("AUFNR='" + wo + "'");
                    if (rowWOHeader.Length == 0)
                    {
                        throw new Exception("wo " + wo + " dose not exist on sap");
                    }
                    skuIsExist = t_c_sku.CheckSku(rowWOHeader[0]["MATNR"].ToString(), SFCDB);
                    if (!skuIsExist)
                    {
                        DataTable dt = SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");
                        if (bu.Equals("VNJUNIPER") || bu.Equals("FJZJNP") || bu.Equals("FJZTESTJNP")|| bu.Equals("FJZ"))
                        {
                            var skuObj = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, C_SKU>
                                ((w, s) => w.PID == s.SKUNO)
                                .Where((w, s) => w.WO == wo && w.GROUPID == rowWOHeader[0]["MATNR"].ToString())
                                .Select((w, s) => s).ToList().FirstOrDefault();

                            if (skuObj == null)
                            {
                                throw new Exception(" the sku of gourp ID " + rowWOHeader[0]["MATNR"].ToString() + " dose not exist");
                            }
                        }
                        else
                        {
                            throw new Exception(" sku " + rowWOHeader[0]["MATNR"].ToString() + " dose not exist");
                        }
                    }
                    woIsExist = t_r_wo_header.CheckWoHeadByWo(rowWOHeader[0]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                    if (woIsExist)
                    {
                        return;
                    }
                    Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)t_r_wo_header.NewRow();
                    rowRWOHeader.ID = t_r_wo_header.GetNewID(bu, SFCDB);
                    rowRWOHeader.AUFNR = rowWOHeader[0]["AUFNR"].ToString();
                    rowRWOHeader.WERKS = rowWOHeader[0]["WERKS"].ToString();
                    rowRWOHeader.AUART = rowWOHeader[0]["AUART"].ToString();
                    rowRWOHeader.MATNR = rowWOHeader[0]["MATNR"].ToString();
                    rowRWOHeader.REVLV = rowWOHeader[0]["REVLV"].ToString();
                    rowRWOHeader.KDAUF = rowWOHeader[0]["KDAUF"].ToString();
                    rowRWOHeader.GSTRS = rowWOHeader[0]["GSTRS"].ToString();
                    rowRWOHeader.GAMNG = rowWOHeader[0]["GAMNG"].ToString();
                    rowRWOHeader.KDMAT = rowWOHeader[0]["KDMAT"].ToString();
                    rowRWOHeader.AEDAT = rowWOHeader[0]["AEDAT"].ToString();
                    rowRWOHeader.AENAM = rowWOHeader[0]["AENAM"].ToString();
                    rowRWOHeader.MATKL = rowWOHeader[0]["MATKL"].ToString();
                    rowRWOHeader.MAKTX = rowWOHeader[0]["MAKTX"].ToString();
                    rowRWOHeader.ERDAT = rowWOHeader[0]["ERDAT"].ToString();
                    rowRWOHeader.GSUPS = rowWOHeader[0]["GSUPS"].ToString();
                    rowRWOHeader.ERFZEIT = rowWOHeader[0]["ERFZEIT"].ToString();
                    rowRWOHeader.GLTRS = rowWOHeader[0]["GLTRS"].ToString();
                    rowRWOHeader.GLUPS = rowWOHeader[0]["GLUPS"].ToString();
                    rowRWOHeader.LGORT = rowWOHeader[0]["LGORT"].ToString();
                    rowRWOHeader.ABLAD = rowWOHeader[0]["ABLAD"].ToString();
                    rowRWOHeader.ROHS_VALUE = rowWOHeader[0]["ROHS_VALUE"].ToString();
                    rowRWOHeader.FTRMI = rowWOHeader[0]["FTRMI"].ToString();
                    rowRWOHeader.MVGR3 = rowWOHeader[0]["MVGR3"].ToString();
                    rowRWOHeader.WEMNG = rowWOHeader[0]["WEMNG"].ToString();
                    rowRWOHeader.BISMT = rowWOHeader[0]["BISMT"].ToString();
                    rowRWOHeader.CHARG = rowWOHeader[0]["CHARG"].ToString();
                    rowRWOHeader.SAENR = rowWOHeader[0]["SAENR"].ToString();
                    rowRWOHeader.AETXT = rowWOHeader[0]["AETXT"].ToString();
                    try
                    {
                        rowRWOHeader.GLTRP = rowWOHeader[0]["GLTRP"].ToString();
                    }
                    catch
                    {
                        rowRWOHeader.GLTRP = "";
                    }
                    sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);
                    rowWOItem = null;
                    rowWOItem = dtWOItem.Select("AUFNR='" + rowWOHeader[0]["AUFNR"].ToString() + "'");
                    SaveWOItem(rowWOItem, SFCDB, dbtype, bu, ip, emp_no, autoDownLoad);


                    //SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + _downloadWO + "; Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                    saveHeaderLog = new R_MES_LOG();
                    saveHeaderLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                    saveHeaderLog.PROGRAM_NAME = "MESStation.Interface";
                    saveHeaderLog.CLASS_NAME = "DownLoad_WO";
                    saveHeaderLog.FUNCTION_NAME = "SaveWOHeader";
                    saveHeaderLog.LOG_MESSAGE = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                    saveHeaderLog.LOG_SQL = sql;
                    saveHeaderLog.DATA1 = wo;
                    saveHeaderLog.DATA2 = ip;
                    saveHeaderLog.DATA3 = "Download r_wo_header";
                    saveHeaderLog.EDIT_EMP = emp_no;
                    saveHeaderLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                    t_r_mes_log.InsertMESLogOld(saveHeaderLog, SFCDB);
                    out_msg = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                }
            }
            else
            {
                for (int i = 0; i < dtWOHeader.Rows.Count; i++)
                {
                    sql = "";
                    woIsExist = false;
                    try
                    {
                        wo = dtWOHeader.Rows[i]["AUFNR"].ToString();
                        skuIsExist = t_c_sku.CheckSku(dtWOHeader.Rows[i]["MATNR"].ToString(), SFCDB);
                        if (!skuIsExist)
                        {
                            DataTable dt = SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");
                            if (bu.Equals("VNJUNIPER") || bu.Equals("FJZJNP") || bu.Equals("FJZTESTJNP") || bu.Equals("FJZ"))
                            {
                                var skuObj = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, C_SKU>
                                    ((w, s) => w.PID == s.SKUNO)
                                    .Where((w, s) => w.WO == wo && w.GROUPID == dtWOHeader.Rows[i]["MATNR"].ToString())
                                    .Select((w, s) => s).ToList().FirstOrDefault();

                                if (skuObj == null)
                                {
                                    throw new Exception("the sku of gourp ID " + dtWOHeader.Rows[i]["MATNR"].ToString() + " dose not exist");
                                }
                            }
                            else
                            {
                                throw new Exception(" sku " + dtWOHeader.Rows[i]["MATNR"].ToString() + " dose not exist");
                            }
                        }
                        woIsExist = t_r_wo_header.CheckWoHeadByWo(dtWOHeader.Rows[i]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                        if (woIsExist)
                        {
                            continue;
                        }
                        Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)t_r_wo_header.NewRow();
                        rowRWOHeader.ID = t_r_wo_header.GetNewID(bu, SFCDB);
                        rowRWOHeader.AUFNR = dtWOHeader.Rows[i]["AUFNR"].ToString();
                        rowRWOHeader.WERKS = dtWOHeader.Rows[i]["WERKS"].ToString();
                        rowRWOHeader.AUART = dtWOHeader.Rows[i]["AUART"].ToString();
                        rowRWOHeader.MATNR = dtWOHeader.Rows[i]["MATNR"].ToString();
                        rowRWOHeader.REVLV = dtWOHeader.Rows[i]["REVLV"].ToString();
                        rowRWOHeader.KDAUF = dtWOHeader.Rows[i]["KDAUF"].ToString();
                        rowRWOHeader.GSTRS = dtWOHeader.Rows[i]["GSTRS"].ToString();
                        rowRWOHeader.GAMNG = dtWOHeader.Rows[i]["GAMNG"].ToString();
                        rowRWOHeader.KDMAT = dtWOHeader.Rows[i]["KDMAT"].ToString();
                        rowRWOHeader.AEDAT = dtWOHeader.Rows[i]["AEDAT"].ToString();
                        rowRWOHeader.AENAM = dtWOHeader.Rows[i]["AENAM"].ToString();
                        rowRWOHeader.MATKL = dtWOHeader.Rows[i]["MATKL"].ToString();
                        rowRWOHeader.MAKTX = dtWOHeader.Rows[i]["MAKTX"].ToString();
                        rowRWOHeader.ERDAT = dtWOHeader.Rows[i]["ERDAT"].ToString();
                        rowRWOHeader.GSUPS = dtWOHeader.Rows[i]["GSUPS"].ToString();
                        rowRWOHeader.ERFZEIT = dtWOHeader.Rows[i]["ERFZEIT"].ToString();
                        rowRWOHeader.GLTRS = dtWOHeader.Rows[i]["GLTRS"].ToString();
                        rowRWOHeader.GLUPS = dtWOHeader.Rows[i]["GLUPS"].ToString();
                        rowRWOHeader.LGORT = dtWOHeader.Rows[i]["LGORT"].ToString();
                        rowRWOHeader.ABLAD = dtWOHeader.Rows[i]["ABLAD"].ToString();
                        rowRWOHeader.ROHS_VALUE = dtWOHeader.Rows[i]["ROHS_VALUE"].ToString();
                        rowRWOHeader.FTRMI = dtWOHeader.Rows[i]["FTRMI"].ToString();
                        rowRWOHeader.MVGR3 = dtWOHeader.Rows[i]["MVGR3"].ToString();
                        rowRWOHeader.WEMNG = dtWOHeader.Rows[i]["WEMNG"].ToString();
                        rowRWOHeader.BISMT = dtWOHeader.Rows[i]["BISMT"].ToString();
                        rowRWOHeader.CHARG = dtWOHeader.Rows[i]["CHARG"].ToString();
                        rowRWOHeader.SAENR = dtWOHeader.Rows[i]["SAENR"].ToString();
                        rowRWOHeader.AETXT = dtWOHeader.Rows[i]["AETXT"].ToString();
                        try
                        {
                            rowRWOHeader.GLTRP = dtWOHeader.Rows[i]["GLTRP"].ToString();
                        }
                        catch
                        { }

                        sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(sql);
                        rowWOItem = null;
                        rowWOItem = dtWOItem.Select("AUFNR='" + dtWOHeader.Rows[i]["AUFNR"].ToString() + "'");
                        SaveWOItem(rowWOItem, SFCDB, dbtype, bu, ip, emp_no, autoDownLoad);


                        //SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + dtWOHeader.Rows[i]["AUFNR"].ToString() + ";Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                        saveHeaderLog = new R_MES_LOG();
                        saveHeaderLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                        saveHeaderLog.PROGRAM_NAME = "MESStation.Interface";
                        saveHeaderLog.CLASS_NAME = "DownLoad_WO";
                        saveHeaderLog.FUNCTION_NAME = "SaveWOHeader";
                        saveHeaderLog.LOG_MESSAGE = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                        saveHeaderLog.LOG_SQL = sql;
                        saveHeaderLog.DATA1 = wo;
                        saveHeaderLog.DATA2 = ip;
                        saveHeaderLog.DATA3 = "Download r_wo_header";
                        saveHeaderLog.EDIT_EMP = emp_no;
                        saveHeaderLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                        t_r_mes_log.InsertMESLogOld(saveHeaderLog, SFCDB);
                        out_msg = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 保存從SAP上下載下來的WO_HERDER,如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="dtWOHeader"></param>
        /// <param name="dtWOItem"></param>
        /// <param name="SFCDB"></param>
        /// <param name="dbtype"></param>
        /// <param name="wo"></param>
        /// <param name="ip"></param>
        /// <param name="emp_no"></param>
        /// <param name="autoDownLoad"></param>
        public void SaveWOHeader(DataTable dtWOHeader, DataTable dtWOItem, OleExec SFCDB, DB_TYPE_ENUM dbtype,string bu, string wo, string ip,string emp_no,bool autoDownLoad,ref string out_msg)
        {
            string sql = "";
            bool woIsExist = false;
            DataRow[] rowWOHeader = null;
            DataRow[] rowWOItem = null;           
            bool skuIsExist = false;
            T_C_SKU t_c_sku = new T_C_SKU(SFCDB, dbtype);
            T_R_WO_HEADER t_r_wo_header = new T_R_WO_HEADER(SFCDB, dbtype);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, dbtype);

            R_MES_LOG saveHeaderLog = null;

            if (wo != "")
            {                
                try
                {
                    rowWOHeader = dtWOHeader.Select("AUFNR='" + wo + "'");
                    if (rowWOHeader.Length == 0)
                    {
                        throw new Exception("wo " + wo + " dose not exist on sap");
                    }
                    skuIsExist = t_c_sku.CheckSku(rowWOHeader[0]["MATNR"].ToString(), SFCDB);
                    if (!skuIsExist)
                    {
                        DataTable dt = SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");                       
                        if (bu.Equals("VNJUNIPER") || bu.Equals("FJZJNP") || bu.Equals("FJZ") || bu.Equals("FJZTESTJNP"))
                        {
                            var skuObj = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, C_SKU>
                                ((w, s) => w.PID == s.SKUNO)
                                .Where((w, s) => w.WO == wo && w.GROUPID == rowWOHeader[0]["MATNR"].ToString())
                                .Select((w, s) => s).ToList().FirstOrDefault();

                            if (skuObj == null)
                            {
                                throw new Exception(" the sku of gourp ID " + rowWOHeader[0]["MATNR"].ToString() + " dose not exist");
                            }
                        }
                        else
                        {
                            throw new Exception(" sku " + rowWOHeader[0]["MATNR"].ToString() + " dose not exist");
                        }                        
                    }
                    woIsExist = t_r_wo_header.CheckWoHeadByWo(rowWOHeader[0]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                    if (woIsExist)
                    {
                        return;
                    }
                    Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)t_r_wo_header.NewRow();
                    rowRWOHeader.ID = t_r_wo_header.GetNewID(bu, SFCDB);
                    rowRWOHeader.AUFNR = rowWOHeader[0]["AUFNR"].ToString();
                    rowRWOHeader.WERKS = rowWOHeader[0]["WERKS"].ToString();
                    rowRWOHeader.AUART = rowWOHeader[0]["AUART"].ToString();
                    rowRWOHeader.MATNR = rowWOHeader[0]["MATNR"].ToString();
                    rowRWOHeader.REVLV = rowWOHeader[0]["REVLV"].ToString();
                    rowRWOHeader.KDAUF = rowWOHeader[0]["KDAUF"].ToString();
                    rowRWOHeader.GSTRS = rowWOHeader[0]["GSTRS"].ToString();
                    rowRWOHeader.GAMNG = rowWOHeader[0]["GAMNG"].ToString();
                    rowRWOHeader.KDMAT = rowWOHeader[0]["KDMAT"].ToString();
                    rowRWOHeader.AEDAT = rowWOHeader[0]["AEDAT"].ToString();
                    rowRWOHeader.AENAM = rowWOHeader[0]["AENAM"].ToString();
                    rowRWOHeader.MATKL = rowWOHeader[0]["MATKL"].ToString();
                    rowRWOHeader.MAKTX = rowWOHeader[0]["MAKTX"].ToString();
                    rowRWOHeader.ERDAT = rowWOHeader[0]["ERDAT"].ToString();
                    rowRWOHeader.GSUPS = rowWOHeader[0]["GSUPS"].ToString();
                    rowRWOHeader.ERFZEIT = rowWOHeader[0]["ERFZEIT"].ToString();
                    rowRWOHeader.GLTRS = rowWOHeader[0]["GLTRS"].ToString();
                    rowRWOHeader.GLUPS = rowWOHeader[0]["GLUPS"].ToString();
                    rowRWOHeader.LGORT = rowWOHeader[0]["LGORT"].ToString();
                    rowRWOHeader.ABLAD = rowWOHeader[0]["ABLAD"].ToString();
                    rowRWOHeader.ROHS_VALUE = rowWOHeader[0]["ROHS_VALUE"].ToString();
                    rowRWOHeader.FTRMI = rowWOHeader[0]["FTRMI"].ToString();
                    rowRWOHeader.MVGR3 = rowWOHeader[0]["MVGR3"].ToString();
                    rowRWOHeader.WEMNG = rowWOHeader[0]["WEMNG"].ToString();
                    rowRWOHeader.BISMT = rowWOHeader[0]["BISMT"].ToString();
                    rowRWOHeader.CHARG = rowWOHeader[0]["CHARG"].ToString();
                    rowRWOHeader.SAENR = rowWOHeader[0]["SAENR"].ToString();
                    rowRWOHeader.AETXT = rowWOHeader[0]["AETXT"].ToString();
                    try
                    {
                        rowRWOHeader.GLTRP = rowWOHeader[0]["GLTRP"].ToString();
                    }
                    catch
                    {
                        rowRWOHeader.GLTRP = "";
                    }
                    sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);
                    rowWOItem = null;
                    rowWOItem = dtWOItem.Select("AUFNR='" + rowWOHeader[0]["AUFNR"].ToString() + "'");
                    SaveWOItem(rowWOItem, SFCDB, dbtype,bu,ip,emp_no, autoDownLoad);


                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + _downloadWO + "; Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                    saveHeaderLog = new R_MES_LOG();
                    saveHeaderLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                    saveHeaderLog.PROGRAM_NAME = "MESStation.Interface";
                    saveHeaderLog.CLASS_NAME = "DownLoad_WO";
                    saveHeaderLog.FUNCTION_NAME = "SaveWOHeader";
                    saveHeaderLog.LOG_MESSAGE = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                    saveHeaderLog.LOG_SQL = sql;
                    saveHeaderLog.DATA1 = wo;
                    saveHeaderLog.DATA2 = ip;
                    saveHeaderLog.DATA3 = "Download r_wo_header";
                    saveHeaderLog.EDIT_EMP = emp_no;
                    saveHeaderLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                    t_r_mes_log.InsertMESLogOld(saveHeaderLog, SFCDB);
                    out_msg = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                }
            }
            else
            {
                for (int i = 0; i < dtWOHeader.Rows.Count; i++)
                {
                    sql = "";
                    woIsExist = false;
                    try
                    {
                        wo= dtWOHeader.Rows[i]["AUFNR"].ToString();
                        skuIsExist = t_c_sku.CheckSku(dtWOHeader.Rows[i]["MATNR"].ToString(), SFCDB);
                        if (!skuIsExist)
                        {
                            DataTable dt = SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");
                            if (bu.Equals("VNJUNIPER") || bu.Equals("FJZJNP") || bu.Equals("FJZ") || bu.Equals("FJZTESTJNP"))
                            {
                                var skuObj = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, C_SKU>
                                    ((w, s) => w.PID == s.SKUNO)
                                    .Where((w, s) => w.WO == wo && w.GROUPID == dtWOHeader.Rows[i]["MATNR"].ToString())
                                    .Select((w, s) => s).ToList().FirstOrDefault();

                                if (skuObj == null)
                                {
                                    throw new Exception("the sku of gourp ID " + dtWOHeader.Rows[i]["MATNR"].ToString() + " dose not exist");
                                }
                            }
                            else
                            {
                                throw new Exception(" sku " + dtWOHeader.Rows[i]["MATNR"].ToString() + " dose not exist");
                            }                            
                        }
                        woIsExist = t_r_wo_header.CheckWoHeadByWo(dtWOHeader.Rows[i]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                        if (woIsExist)
                        {
                            continue;
                        }
                        Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)t_r_wo_header.NewRow();
                        rowRWOHeader.ID = t_r_wo_header.GetNewID(bu, SFCDB);
                        rowRWOHeader.AUFNR = dtWOHeader.Rows[i]["AUFNR"].ToString();
                        rowRWOHeader.WERKS = dtWOHeader.Rows[i]["WERKS"].ToString();
                        rowRWOHeader.AUART = dtWOHeader.Rows[i]["AUART"].ToString();
                        rowRWOHeader.MATNR = dtWOHeader.Rows[i]["MATNR"].ToString();
                        rowRWOHeader.REVLV = dtWOHeader.Rows[i]["REVLV"].ToString();
                        rowRWOHeader.KDAUF = dtWOHeader.Rows[i]["KDAUF"].ToString();
                        rowRWOHeader.GSTRS = dtWOHeader.Rows[i]["GSTRS"].ToString();
                        rowRWOHeader.GAMNG = dtWOHeader.Rows[i]["GAMNG"].ToString();
                        rowRWOHeader.KDMAT = dtWOHeader.Rows[i]["KDMAT"].ToString();
                        rowRWOHeader.AEDAT = dtWOHeader.Rows[i]["AEDAT"].ToString();
                        rowRWOHeader.AENAM = dtWOHeader.Rows[i]["AENAM"].ToString();
                        rowRWOHeader.MATKL = dtWOHeader.Rows[i]["MATKL"].ToString();
                        rowRWOHeader.MAKTX = dtWOHeader.Rows[i]["MAKTX"].ToString();
                        rowRWOHeader.ERDAT = dtWOHeader.Rows[i]["ERDAT"].ToString();
                        rowRWOHeader.GSUPS = dtWOHeader.Rows[i]["GSUPS"].ToString();
                        rowRWOHeader.ERFZEIT = dtWOHeader.Rows[i]["ERFZEIT"].ToString();
                        rowRWOHeader.GLTRS = dtWOHeader.Rows[i]["GLTRS"].ToString();
                        rowRWOHeader.GLUPS = dtWOHeader.Rows[i]["GLUPS"].ToString();
                        rowRWOHeader.LGORT = dtWOHeader.Rows[i]["LGORT"].ToString();
                        rowRWOHeader.ABLAD = dtWOHeader.Rows[i]["ABLAD"].ToString();
                        rowRWOHeader.ROHS_VALUE = dtWOHeader.Rows[i]["ROHS_VALUE"].ToString();
                        rowRWOHeader.FTRMI = dtWOHeader.Rows[i]["FTRMI"].ToString();
                        rowRWOHeader.MVGR3 = dtWOHeader.Rows[i]["MVGR3"].ToString();
                        rowRWOHeader.WEMNG = dtWOHeader.Rows[i]["WEMNG"].ToString();
                        rowRWOHeader.BISMT = dtWOHeader.Rows[i]["BISMT"].ToString();
                        rowRWOHeader.CHARG = dtWOHeader.Rows[i]["CHARG"].ToString();
                        rowRWOHeader.SAENR = dtWOHeader.Rows[i]["SAENR"].ToString();
                        rowRWOHeader.AETXT = dtWOHeader.Rows[i]["AETXT"].ToString();
                        try
                        {
                            rowRWOHeader.GLTRP = dtWOHeader.Rows[i]["GLTRP"].ToString();
                        }
                        catch
                        { }
                        
                        sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(sql);
                        rowWOItem = null;
                        rowWOItem = dtWOItem.Select("AUFNR='" + dtWOHeader.Rows[i]["AUFNR"].ToString() + "'");
                        SaveWOItem(rowWOItem, SFCDB,dbtype, bu, ip, emp_no, autoDownLoad);


                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + dtWOHeader.Rows[i]["AUFNR"].ToString() + ";Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                        saveHeaderLog = new R_MES_LOG();
                        saveHeaderLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                        saveHeaderLog.PROGRAM_NAME = "MESStation.Interface";
                        saveHeaderLog.CLASS_NAME = "DownLoad_WO";
                        saveHeaderLog.FUNCTION_NAME = "SaveWOHeader";
                        saveHeaderLog.LOG_MESSAGE = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                        saveHeaderLog.LOG_SQL = sql;
                        saveHeaderLog.DATA1 = wo;
                        saveHeaderLog.DATA2 = ip;
                        saveHeaderLog.DATA3 = "Download r_wo_header";
                        saveHeaderLog.EDIT_EMP = emp_no;
                        saveHeaderLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                        t_r_mes_log.InsertMESLogOld(saveHeaderLog, SFCDB);
                        out_msg = wo + ";Download r_wo_header fail," + ex.Message.ToString();
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// 保存從SAP上下載下來的WO_ITEM,如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="rowDownloadItem"></param>
        /// <param name="SFCDB"></param>
        /// <param name="dbtype"></param>
        /// <param name="ip"></param>
        /// <param name="emp_no"></param>
        /// <param name="autoDownLoad"></param>
        private void SaveWOItem(DataRow [] rowDownloadItem, OleExec SFCDB, DB_TYPE_ENUM dbtype,string bu,string ip,string emp_no, bool autoDownLoad)
        {
            string sql = "";
            bool woIsExist = false;
            T_R_WO_ITEM t_r_wo_item = new T_R_WO_ITEM(SFCDB, dbtype);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, dbtype);

            R_MES_LOG saveItemLog = null;
            for (int m = 0; m < rowDownloadItem.Length; m++)
            {
               
                try
                {
                    woIsExist = t_r_wo_item.CheckWoItemByWo(rowDownloadItem[m]["AUFNR"].ToString(), rowDownloadItem[m]["MATNR"].ToString(), autoDownLoad, SFCDB);
                    if (woIsExist)
                    {
                        continue;
                    }
                    Row_R_WO_ITEM rowRWOItem = (Row_R_WO_ITEM)t_r_wo_item.NewRow();
                    rowRWOItem.ID = t_r_wo_item.GetNewID(bu, SFCDB);
                    rowRWOItem.AUFNR = rowDownloadItem[m]["AUFNR"].ToString();
                    rowRWOItem.POSNR = rowDownloadItem[m]["POSNR"].ToString();
                    rowRWOItem.MATNR = rowDownloadItem[m]["MATNR"].ToString();
                    rowRWOItem.PARTS = rowDownloadItem[m]["PARTS"].ToString();
                    rowRWOItem.KDMAT = rowDownloadItem[m]["KDMAT"].ToString();
                    rowRWOItem.BDMNG = rowDownloadItem[m]["BDMNG"].ToString();
                    rowRWOItem.MEINS = rowDownloadItem[m]["MEINS"].ToString();
                    rowRWOItem.REVLV = rowDownloadItem[m]["REVLV"].ToString();
                    rowRWOItem.BAUGR = rowDownloadItem[m]["BAUGR"].ToString();
                    rowRWOItem.REPNO = rowDownloadItem[m]["REPNO"].ToString();
                    rowRWOItem.REPPARTNO = rowDownloadItem[m]["REPPARTNO"].ToString();
                    rowRWOItem.AUART = rowDownloadItem[m]["AUART"].ToString();
                    rowRWOItem.AENAM = rowDownloadItem[m]["AENAM"].ToString();
                    rowRWOItem.AEDAT = rowDownloadItem[m]["AEDAT"].ToString();
                    rowRWOItem.MAKTX = rowDownloadItem[m]["MAKTX"].ToString();
                    rowRWOItem.MATKL = rowDownloadItem[m]["MATKL"].ToString();
                    rowRWOItem.WGBEZ = rowDownloadItem[m]["WGBEZ"].ToString();
                    rowRWOItem.ALPOS = rowDownloadItem[m]["ALPOS"].ToString();
                    rowRWOItem.ABLAD = rowDownloadItem[m]["ABLAD"].ToString();
                    rowRWOItem.MVGR3 = rowDownloadItem[m]["MVGR3"].ToString();
                    rowRWOItem.RGEKZ = rowDownloadItem[m]["RGEKZ"].ToString();
                    rowRWOItem.LGORT = rowDownloadItem[m]["LGORT"].ToString();
                    rowRWOItem.ENMNG = rowDownloadItem[m]["ENMNG"].ToString();
                    rowRWOItem.DUMPS = rowDownloadItem[m]["DUMPS"].ToString();
                    rowRWOItem.BISMT = rowDownloadItem[m]["BISMT"].ToString();
                    rowRWOItem.XLOEK = rowDownloadItem[m]["XLOEK"].ToString();
                    rowRWOItem.SHKZG = rowDownloadItem[m]["SHKZG"].ToString();
                    rowRWOItem.CHARG = rowDownloadItem[m]["CHARG"].ToString();
                    try
                    {
                        rowRWOItem.RSPOS = rowDownloadItem[m]["RSPOS"].ToString();
                    }
                    catch
                    { }
                    try
                    {
                        rowRWOItem.VORNR = rowDownloadItem[m]["VORNR"].ToString();
                    }
                    catch
                    { }
                    sql = rowRWOItem.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);                   
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOItem", ip + ";" + rowDownloadItem[m]["AUFNR"].ToString() + "; Download r_wo_item  fail," + ex.Message.ToString(), sql, "interface");

                    saveItemLog = new R_MES_LOG();
                    saveItemLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                    saveItemLog.PROGRAM_NAME = "MESStation.Interface";
                    saveItemLog.CLASS_NAME = "DownLoad_WO";
                    saveItemLog.FUNCTION_NAME = "SaveWOHeader";
                    saveItemLog.LOG_MESSAGE = rowDownloadItem[m]["AUFNR"].ToString() + ";Download r_wo_item fail," + ex.Message.ToString();
                    saveItemLog.LOG_SQL = sql;
                    saveItemLog.DATA1 = rowDownloadItem[m]["AUFNR"].ToString();
                    saveItemLog.DATA2 = ip;
                    saveItemLog.DATA3 = "Download r_wo_item";
                    saveItemLog.EDIT_EMP = emp_no;
                    saveItemLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                    t_r_mes_log.InsertMESLogOld(saveItemLog, SFCDB);
                    continue;
                }
            }
        }
        /// <summary>
        /// 保存從SAP上下載下來的WO_TEXT,如果有更新請同步生成新的INTERFACE，并放到服務器上
        /// </summary>
        /// <param name="dtWOText"></param>
        /// <param name="SFCDB"></param>
        /// <param name="dbtype"></param>
        /// <param name="ip"></param>
        /// <param name="emp_no"></param>
        public void SaveWOText(DataTable dtWOText, OleExec SFCDB, DB_TYPE_ENUM dbtype,string bu, string ip, string emp_no)
        {
            string sql = "";
            bool woIsExist = false;
            T_R_WO_TEXT t_r_wo_text = new T_R_WO_TEXT(SFCDB, dbtype);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, dbtype);
            R_MES_LOG saveTextLog = null;
            for (int m = 0; m < dtWOText.Rows.Count; m++)
            {
               
                try
                {
                    //新增一個長文條件，HWT這邊會放多個任務令（換行會產生多筆） BY ZXY 20190725                    
                    woIsExist = t_r_wo_text.CheckWoTextByWo(dtWOText.Rows[m]["AUFNR"].ToString(), dtWOText.Rows[m]["LTXA1"].ToString(), true, SFCDB);
                    if (woIsExist)
                    {
                        continue;
                    }
                    Row_R_WO_TEXT rowRWOText = (Row_R_WO_TEXT)t_r_wo_text.NewRow();
                    rowRWOText.ID = t_r_wo_text.GetNewID(bu, SFCDB);
                    rowRWOText.AUFNR = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.MATNR = dtWOText.Rows[m]["MATNR"].ToString();
                    rowRWOText.ARBPL = dtWOText.Rows[m]["ARBPL"].ToString();
                    rowRWOText.LTXA1 = dtWOText.Rows[m]["LTXA1"].ToString();
                    rowRWOText.ISAVD = dtWOText.Rows[m]["ISAVD"].ToString();
                    rowRWOText.VORNR = dtWOText.Rows[m]["VORNR"].ToString();
                    rowRWOText.MGVRG = dtWOText.Rows[m]["MGVRG"].ToString();
                    rowRWOText.LMNGA = dtWOText.Rows[m]["LMNGA"].ToString();
                    sql = rowRWOText.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //write log 
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOText", ip + ";" + dtWOText.Rows[m]["AUFNR"].ToString() + ";Down load r_wo_text fail," + ex.Message.ToString(), "", "interface");

                    saveTextLog = new R_MES_LOG();
                    saveTextLog.ID = t_r_mes_log.GetNewID(bu, SFCDB);
                    saveTextLog.PROGRAM_NAME = "MESStation.Interface";
                    saveTextLog.CLASS_NAME = "DownLoad_WO";
                    saveTextLog.FUNCTION_NAME = "SaveWOHeader";
                    saveTextLog.LOG_MESSAGE = dtWOText.Rows[m]["AUFNR"].ToString() + ";Download r_wo_text fail," + ex.Message.ToString();
                    saveTextLog.LOG_SQL = sql;
                    saveTextLog.DATA1 = dtWOText.Rows[m]["AUFNR"].ToString();
                    saveTextLog.DATA2 = ip;
                    saveTextLog.DATA3 = "Download r_wo_text";
                    saveTextLog.EDIT_EMP = emp_no;
                    saveTextLog.EDIT_TIME = t_r_mes_log.GetDBDateTime(SFCDB);
                    t_r_mes_log.InsertMESLogOld(saveTextLog, SFCDB);
                    continue;
                }
            }
        }
    }
}
