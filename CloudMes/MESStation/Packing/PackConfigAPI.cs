using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.Packing
{
    public class PackConfigAPI : MesAPIBase
    {
        private APIInfo _GetPackConfigBySKUNO = new APIInfo()
        {
            FunctionName = "GetPackConfigBySKUNO",
            Description = "獲取料號的包裝配置信息",
            Parameters = new List<APIInputInfo>()
            { new APIInputInfo() { InputName="SkuNo", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _AlertPackConfig = new APIInfo()
        {
            FunctionName = "AlertPackConfig",
            Description = "修改料號的包裝配置信息",
            Parameters = new List<APIInputInfo>()
            { new APIInputInfo() { InputName="PackObj", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetPackType = new APIInfo()
        {
            FunctionName = "GetPackType",
            Description = "獲取可用的PackingType",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetTransportType = new APIInfo()
        {
            FunctionName = "GetTransportType",
            Description = "獲取可用的PackingType",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _RemovePackConfig = new APIInfo()
        {
            FunctionName = "RemovePackConfig",
            Description = " 刪除SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID_LIST", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        private APIInfo FGetNewPackNOBySkuno = new APIInfo()
        {
            FunctionName = "GetNewPackNoBySkuno",
            Description = "Get New Pack NO By Skuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Type", InputType= "string" , DefaultValue=""  },
                new APIInputInfo() { InputName="Skuno", InputType= "string" , DefaultValue=""  },
                new APIInputInfo() { InputName="Line", InputType= "string" , DefaultValue=""  },
                new APIInputInfo() { InputName="Station", InputType= "string" , DefaultValue=""  }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        public PackConfigAPI()
        {
            Apis.Add(_GetPackConfigBySKUNO.FunctionName, _GetPackConfigBySKUNO);
            Apis.Add(_AlertPackConfig.FunctionName, _AlertPackConfig);
            Apis.Add(_GetPackType.FunctionName, _GetPackType);
            Apis.Add(_GetTransportType.FunctionName, _GetTransportType);
            Apis.Add(_RemovePackConfig.FunctionName, _RemovePackConfig);
            Apis.Add(FGetNewPackNOBySkuno.FunctionName, FGetNewPackNOBySkuno);
        }

        public void RemovePackConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["ID_LIST"];
                //T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);

                for (int i = 0; i < OBJ.Count(); i++)
                {
                    Row_C_PACKING RCP = (Row_C_PACKING)TCP.GetObjByID(OBJ[i].ToString(), db);
                    db.ExecSQL(RCP.GetDeleteString(DB_TYPE_ENUM.Oracle));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void AlertPackConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["PackObj"];
                T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                //判斷ID如果為空則插入,如果不為空則更新
                if (OBJ["ID"].ToString() == "")
                {
                    Row_C_PACKING RCP = (Row_C_PACKING)TCP.NewRow();
                    RCP.SKUNO = OBJ["SKUNO"].ToString();
                    RCP.PACK_TYPE = OBJ["PACK_TYPE"].ToString();
                    RCP.TRANSPORT_TYPE = OBJ["TRANSPORT_TYPE"].ToString();
                    RCP.INSIDE_PACK_TYPE = OBJ["INSIDE_PACK_TYPE"].ToString();
                    RCP.MAX_QTY = double.Parse(OBJ["MAX_QTY"].ToString());
                    RCP.DESCRIPTION = OBJ["DESCRIPTION"].ToString();
                    RCP.SN_RULE = OBJ["SN_RULE"].ToString();
                    RCP.EDIT_EMP = LoginUser.EMP_NO;
                    RCP.EDIT_TIME = DateTime.Now;

                    RCP.ID = TCP.GetNewID(BU, db);

                    db.ExecSQL(RCP.GetInsertString(DB_TYPE_ENUM.Oracle));
                    StationReturn.Status = StationReturnStatusValue.Pass;

                }
                else
                {
                    Row_C_PACKING RCP = (Row_C_PACKING)TCP.GetObjByID(OBJ["ID"].ToString(), db);
                    RCP.SKUNO = OBJ["SKUNO"].ToString();
                    RCP.PACK_TYPE = OBJ["PACK_TYPE"].ToString();
                    RCP.TRANSPORT_TYPE = OBJ["TRANSPORT_TYPE"].ToString();
                    RCP.INSIDE_PACK_TYPE = OBJ["INSIDE_PACK_TYPE"].ToString();
                    RCP.MAX_QTY = double.Parse(OBJ["MAX_QTY"].ToString());
                    RCP.DESCRIPTION = OBJ["DESCRIPTION"].ToString();
                    RCP.SN_RULE = OBJ["SN_RULE"].ToString();
                    RCP.EDIT_EMP = LoginUser.EMP_NO;
                    RCP.EDIT_TIME = DateTime.Now;

                    db.ExecSQL(RCP.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetPackConfigBySKUNO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                string strSku = Data["SkuNo"].ToString();
                StationReturn.Data = PackingBase.GetPackingConfigBySKU(strSku, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetPackType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_PACKING_TYPE TCPT = new T_C_PACKING_TYPE(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCPT.GetAllList(db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void GetTransportType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_TRANSPORT_TYPE TCTT = new T_C_TRANSPORT_TYPE(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCTT.GetAllList(db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void GetNewPackNoBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["Type"] == null || Data["Type"].ToString() == "")
                {
                    throw new Exception("Please input type");
                }
                if (Data["Skuno"] == null || Data["Skuno"].ToString() == "")
                {
                    throw new Exception("Please input Skuno");
                }
                if (Data["Line"] == null || Data["Line"].ToString() == "")
                {
                    throw new Exception("Please input Line");
                }
                if (Data["Station"] == null || Data["Station"].ToString() == "")
                {
                    throw new Exception("Please input Station");
                }
                string type = Data["Type"].ToString().Trim();
                string Skuno = Data["Skuno"].ToString().Trim().ToUpper();
                string line = Data["Line"].ToString().Trim();
                string station = Data["Station"].ToString().Trim();

                T_C_PACKING TCP = new T_C_PACKING(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_PACKING TRP = new T_R_PACKING(SFCDB, DB_TYPE_ENUM.Oracle);

                List<C_PACKING> PackConfigs = TCP.GetPackingBySku(Skuno, SFCDB);
                C_PACKING CartonConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON");
                C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET");
                if (CartonConfig == null)
                {
                    throw new Exception("Can't find CartionConfig");
                }
                if (PalletConfig == null)
                {
                    throw new Exception("Can't find PalletConfig");
                }
                Row_R_PACKING RowPallet = null;
                try
                {
                    RowPallet = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{Skuno}' and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
                                                and LINE='{line}' and STATION='{station}' and IP='{this.IP}' and CLOSED_FLAG='0'", SFCDB, DB_TYPE_ENUM.Oracle);
                }
                catch
                {
                    RowPallet = Packing.PackingBase.GetNewPacking(PalletConfig, line, station, this.IP, this.BU, this.LoginUser.EMP_NO, SFCDB);
                }
                Row_R_PACKING RowCartion = null;
                if (type.ToUpper() == "CARTON")
                {
                    try
                    {
                        RowCartion = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{Skuno}' and PACK_TYPE='{CartonConfig.PACK_TYPE}' 
                                                and LINE='{line}' and STATION='{station}' and IP='{this.IP}' and CLOSED_FLAG='0'", SFCDB, DB_TYPE_ENUM.Oracle);
                    }
                    catch
                    {
                        RowCartion = Packing.PackingBase.GetNewPacking(CartonConfig, line, station, this.IP, this.BU, this.LoginUser.EMP_NO, SFCDB);
                    }
                    Packing.CartionBase Cartion = new Packing.CartionBase(RowCartion);
                    Packing.PalletBase Pallet = new Packing.PalletBase(RowPallet);
                    if (Cartion.DATA.PARENT_PACK_ID == null || Cartion.DATA.PARENT_PACK_ID == "")
                    {
                        Pallet.Add(Cartion, this.BU, this.LoginUser.EMP_NO, SFCDB);
                    }                   
                    StationReturn.Data = new { Pallet = RowPallet.PACK_NO, Carton = RowCartion.PACK_NO };
                }
                else if (type.ToUpper() == "PALLET")
                {
                    StationReturn.Data = new { Pallet = RowPallet.PACK_NO, Carton = "" };
                }
                TRP.ClosePack(RowPallet.PACK_NO, this.LoginUser.EMP_NO, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
    }
}
