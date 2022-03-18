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

namespace MESStation.WOManager
{
    class WOKPCompare : MesAPIBase
    {


        protected APIInfo FGetMenuList = new APIInfo()
        {
            FunctionName = "GetMenuList",
            Description = "获取主页面需要确认的工单号",
            Parameters = new List<APIInputInfo>()
            {
                
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSapKpBom = new APIInfo()
        {
            FunctionName = "GetSapKpBom",
            Description = "配置的KP和SAP里Bom一样的料号列表",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetKpSetBom = new APIInfo()
        {
            FunctionName = "GetKpSetBom",
            Description = "配置的KP",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetLastSnKp = new APIInfo()
        {
            FunctionName = "GetLastSnKp",
            Description = "该机种最后一个LOADING SN的KP",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FConfirmKp = new APIInfo()
        {
            FunctionName = "ConfirmKp",
            Description = "KP确认动作",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetKpQeSetAndSapBom = new APIInfo()
        {
            FunctionName = "GetKpQeSetAndSapBom",
            Description = "合併查詢QESET與SAPBOM",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo{ InputName="WORKORDERNO", InputType="String" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public WOKPCompare()
        {
            this.Apis.Add(FGetMenuList.FunctionName, FGetMenuList);
            this.Apis.Add(FGetSapKpBom.FunctionName, FGetSapKpBom);
            this.Apis.Add(FGetKpSetBom.FunctionName, FGetKpSetBom);
            this.Apis.Add(FGetLastSnKp.FunctionName, FGetLastSnKp);
            this.Apis.Add(FConfirmKp.FunctionName, FConfirmKp);
            this.Apis.Add(FGetKpQeSetAndSapBom.FunctionName, FGetKpQeSetAndSapBom);
        }

        public void GetMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_HEADER TRWOHEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
                List<WOKPCompareMenuList> MenuList = new List<WOKPCompareMenuList>();
                DataTable dt = TRWOHEADER.GetWoSpecialVar(SFCDB, new string[0],this.BU);

                foreach (DataRow item in dt.Rows)
                {
                bool CheckKp = TRWOLOG.CheckKpSetCount(item["WORKORDERNO"].ToString(), SFCDB);
                if (!CheckKp)
                    {
                        MenuList.Add(new WOKPCompareMenuList
                        {
                            WORKORDERNO= item["WORKORDERNO"].ToString(),
                            SKUNO= item["SKUNO"].ToString(),
                            VERSION=item["VERSION"].ToString(),
                            WORKORDERQTY=item["QTY"].ToString(),
                            STARTDATE = item["STARTDATE"].ToString(),
                        });
                    }
                }
                StationReturn.Data = MenuList;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetSapKpBom(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string workorderno = Data["workorderno"].ToString();
            try
            {
                T_R_WO_ITEM TRWOITEM = new T_R_WO_ITEM(SFCDB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TRWOITEM.GetKpSetInSapList(workorderno, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetKpSetBom(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string workorderno = Data["workorderno"].ToString();
            try
            {
                T_C_KP_LIST TCKPL = new T_C_KP_LIST(SFCDB, DB_TYPE_ENUM.Oracle);
                T_C_KP_List_Item TCKPLI = new T_C_KP_List_Item(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_WO_HEADER TRWOHEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                string skuno = TRWOHEADER.GetDetailByWo(SFCDB, workorderno).MATNR;
                string listID = TCKPL.GetListIDbySkuno(skuno,SFCDB);
                StationReturn.Data = TCKPLI.GetItemObjectByListId(listID,SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetLastSnKp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string workorderno = Data["workorderno"].ToString();
            try
            {
                T_R_SN_KP TRSNKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_WO_HEADER TRWOHEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                string skuno = TRWOHEADER.GetDetailByWo(SFCDB, workorderno).MATNR;
                StationReturn.Data = TRSNKP.GetLastLoadingRegularSnKp(skuno, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void ConfirmKp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string WORKORDERNO = Data["WORKORDERNO"].ToString();
            string SKUNO = Data["SKUNO"].ToString();
            string VERSION = Data["VERSION"].ToString();
            string REASON = Data["REASON"].ToString();
            try
            {   T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_LOG r = (Row_R_WO_LOG)TRWOLOG.NewRow();

                r.ID=TRWOLOG.GetNewID(this.BU, SFCDB);
                r.FUNCTIONNAME = "MESStation.WOManager.WOKPCompare.ConfirmKp";
                r.SKUNO = SKUNO;
                r.VERSION = VERSION;
                r.WORKORDERNO = WORKORDERNO;
                r.REASON = REASON;
                r.FLAG = "1";
                r.EDITTIME = GetDBDateTime();
                r.EDITBY= this.LoginUser.EMP_NO;

                SFCDB.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));


                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// QE要求Check方式與NN一致，因此增加此方法合併查詢QESET與SAPBOM
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetKpQeSetAndSapBom(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string workorderno = Data["workorderno"].ToString();
            try
            {
                string sql = $@"
                SELECT * FROM (
                SELECT 'QESETKP' AS QESETKP,A.MATNR AS SKUNO,A.REVLV AS VERSION,C.KP_PARTNO AS QE_KPPN,C.QTY,C.STATION 
                FROM R_WO_HEADER A, C_KP_LIST B, C_KP_LIST_ITEM C WHERE A.MATNR=B.SKUNO AND B.ID=C.LIST_ID AND B.FLAG='1' 
                AND A.AUFNR='{workorderno}' ORDER BY C.SEQ) A FULL OUTER JOIN (
                SELECT 'SAPBOM' AS SAPBOM,A.AUFNR AS WORKORDERNO,A.BAUGR AS BOMSKU,A.MATNR AS SAP_KPPN
                FROM R_WO_ITEM A, R_WO_HEADER B, C_KP_LIST C, C_KP_LIST_ITEM D
                WHERE A.AUFNR=B.AUFNR AND B.MATNR=C.SKUNO AND C.ID=D.LIST_ID AND A.MATNR=D.KP_PARTNO
                AND C.FLAG='1' AND A.AUFNR='{workorderno}') B ON A.SKUNO=B.BOMSKU AND A.QE_KPPN=B.SAP_KPPN";

                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

    }

    class WOKPCompareMenuList
    {
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string WORKORDERQTY { get; set; }
        public string STARTDATE { get; set; }

    }
}
