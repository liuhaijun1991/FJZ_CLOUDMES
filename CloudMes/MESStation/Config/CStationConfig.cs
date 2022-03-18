using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class CStationConfig : MESPubLab.MESStation.MesAPIBase
    {
        public CStationConfig()
        {
            this.Apis.Add(FSelectByColumnName.FunctionName, FSelectByColumnName);
            this.Apis.Add(FStationInsert.FunctionName, FStationInsert);
            this.Apis.Add(FStationDelete.FunctionName, FStationDelete);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
            this.Apis.Add(FGetAllStation.FunctionName,FGetAllStation);
            this.Apis.Add(FUpdateStationByID.FunctionName, FUpdateStationByID); 
            this.Apis.Add(FGetStationSelectInputType.FunctionName, FGetStationSelectInputType);
            this.Apis.Add(FBPDGetFlagVlaue.FunctionName, FBPDGetFlagVlaue);
            //Mala: for L11 Select Cable Type 2/11/2020
            this.Apis.Add(FGetCableSelectInputType.FunctionName, FGetCableSelectInputType);
            this.Apis.Add(FGetStationNumber.FunctionName, FGetStationNumber);
            this.Apis.Add(FGetUpdateKPStationInputType.FunctionName, FGetUpdateKPStationInputType);
            this.Apis.Add(FGetXrayStatus.FunctionName, FGetXrayStatus);
            this.Apis.Add(FGetAllpartLinkQty.FunctionName, FGetAllpartLinkQty);
        }
        protected APIInfo FGetAllpartLinkQty = new APIInfo()
        {
            FunctionName = "GetAllpartLinkQty ",
            Description = "GetAllpartLinkQty ",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_VER", InputType = "string", DefaultValue = "" }                
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetUpdateKPStationInputType = new APIInfo()
        {
            FunctionName = "GetUpdateKPStationInputType",
            Description = "GetUpdateKPStationInputType",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetStationNumber = new APIInfo()
        {
            FunctionName = "GetStationNumber",
            Description = "GetStationNumber",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetXrayStatus = new APIInfo()
        {
            FunctionName = "GetXrayStatus",
            Description = "GetXrayStatus",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateStationByID = new APIInfo()
        {
            FunctionName = "UpdateStationByID",
            Description = "根据ID值更新STATION",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "StationName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllStation = new APIInfo()
        {
            FunctionName = "GetAllStation",
            Description = "返回所有站位字符串列表",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "查询STATION所有数据",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectByColumnName = new APIInfo()
        {
            FunctionName = "SelectByColumnName",
            Description = "根据传入的栏位及值进行查询操作",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ColumnName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ColumnValue", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationInsert = new APIInfo()
        {
            FunctionName = "StationInsert",
            Description = "执行插入操作",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "StationName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationDelete = new APIInfo()
        {
            FunctionName = "DeleteStationByID",
            Description = "根据ID删除信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetStationSelectInputType = new APIInfo()
        {
            FunctionName = "GetStationSelectInputType",
            Description = "獲取工站可以選擇的輸入類型",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        //START -Mala: for L11 Select Cable Type 02/11/2020
        protected APIInfo FGetCableSelectInputType = new APIInfo()
        {
            FunctionName = "GetCableSelectInputType",
            Description = "Get the input types power or rack",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FBPDGetFlagVlaue = new APIInfo()
        {
            FunctionName = "BPDGetFlagVlaue",
            Description = "獲取工站可以選擇的輸入類型",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        }; 
        public void UpdateStationByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cSection = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION r = (Row_C_STATION)cSection.NewRow();
                r = (Row_C_STATION)cSection.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                r.STATION_NAME = (Data["StationName"].ToString()).Trim();
                r.TYPE = (Data["TYPE"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet)>0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = ""; 
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.ShowAllData(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void GetAllStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> stationList = new List<string>();
                T_C_STATION cStation = new T_C_STATION(sfcdb,DB_TYPE_ENUM.Oracle);
                stationList = cStation.GetAllStation(sfcdb);
                if (stationList.Count>0)
                {
                    StationReturn.Data = stationList;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw;
            }
        }

        public void StationInsert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION rStation = (Row_C_STATION)cStation.NewRow();
                rStation.ID = cStation.GetNewID(BU, sfcdb);
                rStation.STATION_NAME = (Data["StationName"].ToString()).Trim();
                rStation.TYPE = (Data["TYPE"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(rStation.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void SelectByColumnName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string data = (Data["ColumnValue"].ToString()).Trim();
                string column = (Data["ColumnName"].ToString()).Trim();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.GetDataByColumn(column, data, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteStationByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION r = (Row_C_STATION)cStation.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "NotLatestData";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        ///獲取PTH StationNumber
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStationNumber(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //List<int> StationNum = new List<int>();
            //for (int i = 1; i < 18; i++)
            //{
            //    StationNum.Add(i);
            //}
            //StationNum.Sort();
            List<string> StationNum = new List<string>();
            StationNum.Add("");
            for (int i = 1; i < 18; i++)
            {
                StationNum.Add(i.ToString());
            }
            StationReturn.Data = StationNum;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetWashPcbReason(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> StationNum = new List<string>();
            StationNum.Add("");
            StationNum.Add("印刷少錫");
            StationNum.Add("印刷連錫");
            StationNum.Add("印刷拉尖");
            StationNum.Add("印刷堵孔");
            StationNum.Add("印刷偏位");
            StationNum.Add("錫膏偏厚");
            StationNum.Add("錫膏偏薄");
            StationNum.Add("貼裝少件");
            StationNum.Add("貼裝偏位");
            StationNum.Add("其他");
            StationReturn.Data = StationNum;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        /// <summary>
        /// 獲取工站可以選擇的輸入類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStationSelectInputType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {   
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("SN");
            listInputType.Add("PANEL");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        //Mala: for L11 Select Cable Type 02202020
        public void GetCableSelectInputType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("RACK");
            listInputType.Add("POWER");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        //Mala End: for L11 Select Cable Type

        //自定義select
        public void BPDGetFlagVlaue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("Y");
            listInputType.Add("N");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        public void GetPrintMode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> PrintMode = new List<string>();
            PrintMode.Add("Print");
            PrintMode.Add("Reprint");
            StationReturn.Data = PrintMode;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        public void GetStationTypeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> typeList = new List<string>();

            foreach (STATIONTYPE stationType in Enum.GetValues(typeof(STATIONTYPE)))
            {
                typeList.Add(stationType.ToString());
            }
            StationReturn.Data = typeList;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }


        /// <summary>
        /// 獲取工站可以選擇的輸入類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStationSelectReturnShipType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("SN");
            listInputType.Add("PALLET");
            listInputType.Add("WO");
            listInputType.Add("DN");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        /// <summary>
        /// 獲取工站可以選擇的輸入類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetUpdateKPStationInputType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("SN");
            listInputType.Add("PANEL");
            listInputType.Add("WO");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        /// <summary>
        /// 獲取 xray 結果輸入類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetXrayStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> xrayStatus = new List<string>();
            xrayStatus.Add("");
            xrayStatus.Add("PASS");
            xrayStatus.Add("FAIL");

            StationReturn.Data = xrayStatus;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        public void GetAllpartLinkQty(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec APDB = null;           
            try
            {
                APDB = DBPools["APDB"].Borrow();
                List<string> list = new List<string>() { ""};                
                string panel = Data["PANEL"] == null ? "" : (Data["PANEL"].ToString()).Trim();
                int linkQty = 0;
                bool bLink = false;
                string sql = $@"select distinct link_qty from mes1.C_PRODUCT_CONFIG a,mes4.r_sn_link b,mes4.r_wo_base c
                        where b.panel_no='{panel}' and b.wo=c.wo and c.p_no=a.p_no and c.p_version=a.p_version";
                DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    bLink = int.TryParse(dt.Rows[0][0].ToString(), out linkQty);
                    if (bLink)
                    {
                        for (int i = 0; i < linkQty; i++)
                        {
                            list.Add((i + 1).ToString());
                        }
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = null;
                StationReturn.Data = list;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
            finally
            {
                if (APDB != null)
                {
                    DBPools["SFCDB"].Return(APDB);
                }
            }
        }
    }
}

