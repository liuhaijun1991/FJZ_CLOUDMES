using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class WareHouseConfig : MesAPIBase
    {
        private APIInfo getWarehouseConfig = new APIInfo()
        {
            FunctionName = "GetWarehouseConfig",
            Description = "GetWarehouseConfig",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };
        public WareHouseConfig()
        {
            this.Apis.Add(getWarehouseConfig.FunctionName, getWarehouseConfig);
        }
        public void GetWarehouseConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            DataTable dt;
            T_C_WAREHOUSE_CONFIG_T CwhConfig = new T_C_WAREHOUSE_CONFIG_T(SFCDB,DBTYPE);
            try
            {
                dt = CwhConfig.GetAllWarehouseConfig(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = dt;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception exception)
            {

                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAllPositionByWHID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<Dictionary<int, Dictionary<string, int>>> data_mapping = new List<Dictionary<int, Dictionary<string, int>>>();
            T_C_WAREHOUSE_PALLET_POSITION_T CwhConfig = null;
            string WH_ID = string.Empty;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CwhConfig = new T_C_WAREHOUSE_PALLET_POSITION_T(sfcdb, DBTYPE);
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                data_mapping = CwhConfig.GetAllPositionByWHID(WH_ID, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = data_mapping;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void GetInfoPosition(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_WAREHOUSE_PALLET_POSITION_T> PositionList = new List<C_WAREHOUSE_PALLET_POSITION_T>();
            T_C_WAREHOUSE_PALLET_POSITION_T CwhConfig = null;
            string WH_ID = string.Empty;
            int result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CwhConfig = new T_C_WAREHOUSE_PALLET_POSITION_T(sfcdb, DBTYPE);
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                int ROW_POSITION = int.Parse(Data["ROW_POSITION"].ToString().Trim());
                int COL_POSITION = int.Parse(Data["COL_POSITION"].ToString().Trim());
                result = CwhConfig.CheckInfoPosition(WH_ID, ROW_POSITION, COL_POSITION, sfcdb);
                if (result <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Invalid Position!";
                    StationReturn.Data = null;
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                PositionList = CwhConfig.GetInfoPosition(WH_ID, ROW_POSITION, COL_POSITION, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = PositionList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void CheckPallet(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = _DBPools["SFCDB"].Borrow();
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string pallet = string.Empty;
            string resul;
            oleDB.BeginTrain();
            try
            {
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                pallet = Data["pallet"].ToString().Trim().ToUpper();
                T_R_PACKING rpk = new T_R_PACKING(oleDB, DBTYPE);
                resul = rpk.CheckStatusSNinPallet(pallet, oleDB);
                if (!rpk.PackNoIsExist(pallet, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else if (resul !="OK")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "PALLET already SHIPOUT";
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                //else if (Table.checkplexit(pallet,oleDB))
                //{
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.MessageCode = "PALLET already at WHS";
                //    this.DBPools["SFCDB"].Return(oleDB);
                //    return;
                //}
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void CheckinPLLocation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = _DBPools["SFCDB"].Borrow();
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string listpl = string.Empty;
            string WH_ID = string.Empty;
            int resul;
            oleDB.BeginTrain();
            try
            {
                string EMP = this.LoginUser.EMP_NO;
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                listpl = Data["listpl"].ToString().Trim().ToUpper();
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                string list = "'" + listpl.Trim().Replace("\n", "','") + "'";
                T_R_PACKING rpk = new T_R_PACKING(oleDB, DBTYPE);
                resul = Table.InsertPLtoLocation(WH_ID,listpl, EMP, oleDB);
                
                if (resul != 1)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "FAIL";
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }

        public void CheckPalletEXIT(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = _DBPools["SFCDB"].Borrow();
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string pallet = string.Empty;
            string WH_ID = string.Empty;
            string resul;
            oleDB.BeginTrain();
            try
            {
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                pallet = Data["pallet"].ToString().Trim().ToUpper();
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                T_R_PACKING rpk = new T_R_PACKING(oleDB, DBTYPE);
                //resul = rpk.CheckStatusSNinPallet(pallet, oleDB);
              
                if (!Table.checkplexitlocation(pallet, WH_ID,oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "PALLET not in WHS";
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }

        public void CheckoutPLLocation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = _DBPools["SFCDB"].Borrow();
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string listpl = string.Empty;
            string WH_ID = string.Empty;
            int resul;
            oleDB.BeginTrain();
            try
            {
                string EMP = this.LoginUser.EMP_NO;
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                listpl = Data["listpl"].ToString().Trim().ToUpper();
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                string list = "'" + listpl.Trim().Replace("\n", "','") + "'";
                T_R_PACKING rpk = new T_R_PACKING(oleDB, DBTYPE);
                resul = Table.UpdatePLLocation(WH_ID, listpl, EMP, oleDB);

                if (resul != 1)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "FAIL";
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void InsertPalletToPosition(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = _DBPools["SFCDB"].Borrow();
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string WH_ID = string.Empty;
            string PALLET_NO = string.Empty;
            int ROW_POSITION = 0;
            int COL_POSITION = 0;
            int SORT_POSITION = 0;
            string EMP = string.Empty;
            int result;
            oleDB.BeginTrain();
            try
            {
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                PALLET_NO = Data["PALLET_NO"].ToString().Trim().ToUpper();
                ROW_POSITION = int.Parse(Data["ROW_POSITION"].ToString().Trim());
                COL_POSITION = int.Parse(Data["COL_POSITION"].ToString().Trim());
                SORT_POSITION = int.Parse(Data["SORT_POSITION"].ToString().Trim());
                EMP = this.LoginUser.EMP_NO;
                result = Table.InsertPalletToPosition(WH_ID, PALLET_NO, ROW_POSITION, COL_POSITION, SORT_POSITION, EMP, BU, DBTYPE, oleDB);
                if (result != 1)
                {
                    if (result == -1)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "PALLET_NO not exist or have been shipped";
                        StationReturn.Data = new object();
                    }
                    if (result == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "PALLET_NO existed";
                        StationReturn.Data = new object();
                    }
                    else if (result == 2)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "Insert fail";
                        StationReturn.Data = new object();
                    }
                    else if (result == 3)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                        StationReturn.Data = new object();
                    }
                    else if (result == 4)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "Position Full";
                        StationReturn.Data = new object();
                    }
                    else if (result == 5)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "Sort of position have pallet";
                        StationReturn.Data = new object();
                    }
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Insert success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void GetPalletInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_WAREHOUSE_PALLET_POSITION_T CWHPP = null;
            List<C_WAREHOUSE_PALLET_POSITION_T> ret = new List<C_WAREHOUSE_PALLET_POSITION_T>();
            string DATA_SEARCH = string.Empty;
            string WH_ID = string.Empty;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CWHPP = new T_C_WAREHOUSE_PALLET_POSITION_T(sfcdb, DBTYPE);
                DATA_SEARCH = Data["DATA_SEARCH"].ToString().Trim().ToUpper();
                WH_ID = Data["WH_ID"].ToString().Trim().ToUpper();
                ret = CWHPP.GetPalletInfo(WH_ID, DATA_SEARCH, sfcdb);
                if (ret == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Not found data!";
                    StationReturn.Data = ret;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000016";
                    StationReturn.Data = ret;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void DeletePalletOutPosition(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_WAREHOUSE_PALLET_POSITION_T Table = null;
            string WH_ID = string.Empty;
            int ROW_POSITION = 0;
            int COL_POSITION =0;
            Newtonsoft.Json.Linq.JArray PALLET_LIST = null;
            int result;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_WAREHOUSE_PALLET_POSITION_T(oleDB, DBTYPE);
                WH_ID = Data["WH_ID"].ToString().Trim();
                ROW_POSITION = int.Parse(Data["ROW_POSITION"].ToString().Trim());
                COL_POSITION = int.Parse(Data["COL_POSITION"].ToString().Trim());
                PALLET_LIST = (Newtonsoft.Json.Linq.JArray)Data["PALLET_LIST"];
                result = Table.DeletePalletOutPosition(WH_ID, PALLET_LIST.ToObject<List<string>>(),ROW_POSITION,COL_POSITION ,oleDB);
                if (result != 1)
                {
                    if (result == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "Delete fail";
                        StationReturn.Data = new object();
                    }
                    else if (result == 2)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "Delete fail ID is empty";
                        StationReturn.Data = new object();
                    }
                    else if (result == 3)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                        StationReturn.Data = new object();
                    }
                    else if (result == 4)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = $"This is pallet not in location {ROW_POSITION+"x"+COL_POSITION}";
                        StationReturn.Data = new object();
                    }
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                    return;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Delete success";
                    StationReturn.Data = new object();
                    oleDB.CommitTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (oleDB != null)
                {
                    oleDB.RollbackTrain();
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }

        }
        public void InsertData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
           string status = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_WAREHOUSE_CONFIG_T T_C_WAREHOUSE_CONFIG_T = new T_C_WAREHOUSE_CONFIG_T(sfcdb, DBTYPE);
                string WH_NAME = Data["WH_NAME"].ToString();
                //int ROW_SIZE = Int32.Parse(Data["ROW_SIZE"].ToString());
                //int COL_SIZE = Int32.Parse(Data["COL_SIZE"].ToString());
                string ID_FACT = Data["ID_FACT"].ToString();
                string EMP_NO = LoginUser.EMP_NO;
                C_WAREHOUSE_CONFIG_T newline = new C_WAREHOUSE_CONFIG_T();
                T_C_WAREHOUSE_CONFIG_T line = new T_C_WAREHOUSE_CONFIG_T(sfcdb, DBTYPE);
                newline.WH_ID = line.GetNewID(BU, sfcdb);
                string id = newline.WH_ID.ToString();
                status = T_C_WAREHOUSE_CONFIG_T.Insert(WH_NAME, EMP_NO, ID_FACT ,id, sfcdb);
                if (status == "FAIL")
                {
                    StationReturn.Message = "DUPLICATE TABLE NAME";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Data = status;
                    StationReturn.Message = "Success!";
                    StationReturn.Status = StationReturnStatusValue.Pass;

                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void ModifyConfigWh(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string status = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_WAREHOUSE_CONFIG_T T_C_WAREHOUSE_CONFIG_T = new T_C_WAREHOUSE_CONFIG_T(sfcdb, DBTYPE);
                string WH_NAME = Data["WH_NAME"].ToString();
                //int ROW_SIZE = Int32.Parse(Data["ROW_SIZE"].ToString());
                //int COL_SIZE = Int32.Parse(Data["COL_SIZE"].ToString());
                string EMP_NO = LoginUser.EMP_NO;
                //C_WAREHOUSE_CONFIG_T newline = new C_WAREHOUSE_CONFIG_T();
                //T_C_WAREHOUSE_CONFIG_T line = new T_C_WAREHOUSE_CONFIG_T(sfcdb, DBTYPE);
                //newline.WH_ID = line.GetNewID(BU, sfcdb);
                //string id = newline.WH_ID.ToString();
                status = T_C_WAREHOUSE_CONFIG_T.Edit(WH_NAME,  EMP_NO, sfcdb);
                if (status == "FAIL")
                {
                    StationReturn.Message = "deo dk";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Data = status;
                    StationReturn.Message = "Success!";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void DeleteConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string status = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_WAREHOUSE_CONFIG_T T_C_WAREHOUSE_CONFIG_T = new T_C_WAREHOUSE_CONFIG_T(sfcdb, DBTYPE);
                string WH_NAME = Data["WH_NAME"].ToString();
                //int ROW_SIZE = Int32.Parse(Data["ROW_SIZE"].ToString());
                //int COL_SIZE = Int32.Parse(Data["COL_SIZE"].ToString());
                string EMP_NO = LoginUser.EMP_NO;
                status = T_C_WAREHOUSE_CONFIG_T.EditConfigWh(WH_NAME, EMP_NO, sfcdb);
                if (status == "FAIL")
                {
                    StationReturn.Message = "Vị trí này vẫn còn pallet chưa xuất hàng !!";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Data = status;
                    StationReturn.Message = "Success!";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void GetAllWarehouse(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            sfcdb = this.DBPools["SFCDB"].Borrow();
            List<C_BU_LOCATION> whlist = new List<C_BU_LOCATION>();
            T_C_BU_LOCATION WH = new T_C_BU_LOCATION(sfcdb,DBTYPE);
            try
            {
               
                whlist = WH.GetAllWarehouse(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = whlist;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}
