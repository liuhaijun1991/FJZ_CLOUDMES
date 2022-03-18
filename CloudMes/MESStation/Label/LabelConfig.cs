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

namespace MESStation.Label
{
    public class LabelConfig : MesAPIBase
    {
        private APIInfo _GetLabelConfigBySkuno = new APIInfo()
        {
            FunctionName = "GetLabelConfigBySkuno",
            Description = "獲取SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Skuno", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _AlertLabelConfig = new APIInfo()
        {
            FunctionName = "AlertLabelConfig",
            Description = "增加或SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="LabelObject", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };
        private APIInfo _RemoveLabelConfig = new APIInfo()
        {
            FunctionName = "RemoveLabelConfig",
            Description = " 刪除SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID_LIST", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };
        private APIInfo _GetLabelTypes = new APIInfo()
        {
            FunctionName = "GetLabelTypes",
            Description = "獲取LabelTypes配置",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()
            { }
        };

        public LabelConfig()
        {
            Apis.Add(_GetLabelConfigBySkuno.FunctionName, _GetLabelConfigBySkuno);
            Apis.Add(_AlertLabelConfig.FunctionName, _AlertLabelConfig);
            Apis.Add(_RemoveLabelConfig.FunctionName, _RemoveLabelConfig);
            Apis.Add(_GetLabelTypes.FunctionName, _GetLabelTypes);
        }
        public void GetLabelTypes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_Label_Type TCLT = new T_C_Label_Type(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCLT.GetLabelTypes(db);
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
        public void RemoveLabelConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["ID_LIST"];
                //T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);

                for (int i = 0; i < OBJ.Count(); i++)
                {
                    Row_C_SKU_Label RCKL = (Row_C_SKU_Label)TCSL.GetObjByID(OBJ[i].ToString(), db);
                    db.ExecSQL(RCKL.GetDeleteString(DB_TYPE_ENUM.Oracle));
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
        public void GetLabelConfigBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                List<C_SKU_Label> ret = new List<C_SKU_Label>();
                string strSkuno = Data["Skuno"].ToString();
                T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);
                List<C_SKU_Label> RCSLS = TCSL.GetLabelConfigBySku(strSkuno, db);
                for (int i = 0; i < RCSLS.Count; i++)
                {
                    ret.Add(RCSLS[i]);
                }
                StationReturn.Data = ret;
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

        public void AlertLabelConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["LabelObject"];
                //T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);

                //HWD GeneralSN Label添加卡關
                if (BU == "HWD" && OBJ["LABELTYPE"].ToString() == "GeneralSN")
                {
                    T_C_SKU t_c_sku = new T_C_SKU(db, DBTYPE);
                    if (!t_c_sku.HWDGeneralSNLabelCheck(OBJ["SKUNO"].ToString(), db))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037"; 
                        //StationReturn.MessagePara.Add(OBJ["SKUNO"].ToString()+$@" 不能打印 GeneralSN，請找PE確認！");
                        StationReturn.MessagePara.Add(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143245", new string[] { OBJ["SKUNO"].ToString() }));
                        return;
                    }
                }

                //判斷ID如果為空則插入,如果不為空則更新
                if (OBJ["ID"].ToString() == "")
                {
                    Row_C_SKU_Label RCKL = (Row_C_SKU_Label)TCSL.NewRow();
                    RCKL.SKUNO = OBJ["SKUNO"].ToString();
                    RCKL.STATION = OBJ["STATION"].ToString();
                    RCKL.SEQ = double.Parse(OBJ["SEQ"].ToString());
                    RCKL.QTY = double.Parse(OBJ["QTY"].ToString());
                    RCKL.LABELNAME = OBJ["LABELNAME"].ToString();
                    RCKL.LABELTYPE = OBJ["LABELTYPE"].ToString();

                    RCKL.EDIT_EMP = LoginUser.EMP_NO;
                    RCKL.EDIT_TIME = DateTime.Now;

                    RCKL.ID = TCSL.GetNewID(BU, db);

                    db.ExecSQL(RCKL.GetInsertString(DB_TYPE_ENUM.Oracle));
                    StationReturn.Status = StationReturnStatusValue.Pass;

                }
                else
                {

                    Row_C_SKU_Label RCKL = (Row_C_SKU_Label)TCSL.GetObjByID(OBJ["ID"].ToString(), db);
                    RCKL.SKUNO = OBJ["SKUNO"].ToString();
                    RCKL.STATION = OBJ["STATION"].ToString();
                    RCKL.SEQ = double.Parse(OBJ["SEQ"].ToString());
                    RCKL.QTY = double.Parse(OBJ["QTY"].ToString());
                    RCKL.LABELNAME = OBJ["LABELNAME"].ToString();
                    RCKL.LABELTYPE = OBJ["LABELTYPE"].ToString();

                    RCKL.EDIT_EMP = LoginUser.EMP_NO;
                    RCKL.EDIT_TIME = DateTime.Now;

                    db.ExecSQL(RCKL.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
            }
            catch
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
    }
}
