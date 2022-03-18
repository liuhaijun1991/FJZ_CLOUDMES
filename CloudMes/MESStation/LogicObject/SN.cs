using DcnSfcModel;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESStation.LogicObject
{
    public class SN : I_LabelValue
    {
        public string ID { get { return baseSN.ID; } }    // ID號 
        public string SerialNo { get { return baseSN.SN; } }    // 產品SN 
        public string SkuNo { get { return baseSN.SKUNO; } }    // 機種料號 
        public string WorkorderNo { get { return baseSN.WORKORDERNO; } }    // 工單號 
        public string Plant { get { return baseSN.PLANT; } }    // 廠別 
        public string RouteID { get { return baseSN.ROUTE_ID; } }    // 工單路由 
        public string StartedFlag { get { return baseSN.STARTED_FLAG; } }    // 產品是否已loading進工單標位 
        public DateTime? StartTime { get { return baseSN.START_TIME; } }    // 產品Loading時間 
        public string PackedFlag { get { return baseSN.PACKED_FLAG; } }    // 包裝標志位 
        public DateTime? PackDate { get { return baseSN.PACKDATE; } }    // 包裝時間 
        public string CompletedFlag { get { return baseSN.COMPLETED_FLAG; } }    // 產品完工狀態標誌位 
        public DateTime? CompletedTime { get { return baseSN.COMPLETED_TIME; } }    // 產品完工時間 
        public string ShippedFlag { get { return baseSN.SHIPPED_FLAG; } }    // 出貨標誌位 
        public DateTime? ShipDate { get { return baseSN.SHIPDATE; } }    // 出貨時間 
        public string RepairFailedFlag { get { return baseSN.REPAIR_FAILED_FLAG; } }    // Fail標誌位 
        public string CurrentStation { get { return baseSN.CURRENT_STATION; } }    // 當前站 
        public string NextStation { get { return baseSN.NEXT_STATION; } }    // 下一站 
        public string KP_LIST_ID { get { return baseSN.KP_LIST_ID; } }      // PE配置的keypart信息 
        public string PONO { get { return baseSN.PO_NO; } }    // 產品的PO 
        public string CustomerOrderNo { get { return baseSN.CUST_ORDER_NO; } }    // 產品的任務令 
        public string CustomerPartNo { get { return baseSN.CUST_PN; } }    // 客戶料號 
        public string BoxSN { get { return baseSN.BOXSN; } }    // Box條碼 
        public string ScrapedFlag { get { return baseSN.SCRAPED_FLAG; } }    // 是否報廢 
        public DateTime? ScrapedTime { get { return baseSN.SCRAPED_TIME; } }    // 報廢日期 
        public string ProductStatus { get { return baseSN.PRODUCT_STATUS; } }    // 產品狀態 
        public double? ReworkCount { get { return baseSN.REWORK_COUNT; } }    // 重工次數 
        public string ValidFlag { get { return baseSN.VALID_FLAG; } }    // 是否有效 
        public string StockStatus { get { return baseSN.STOCK_STATUS; } }    //是否入庫  
        public DateTime? StockTime { get { return baseSN.STOCK_IN_TIME; } }  //入庫時間
        //public string EDIT_EMP;    // 最後編輯人 
        //public DateTime EDIT_TIME;    // 最後編輯時間 

        public List<C_KEYPART> KeyPartList { get { return _keyPartList; } }

        public bool isPacked(OleExec DB)
        {
            string SNID = this.baseSN.ID;

            string strSql = $@"SELECT COUNT(1) FROM R_SN_PACKING WHERE SN_ID ='{SNID}'";
            int count;
            if (!Int32.TryParse(DB.ExecSelectOneValue(strSql).ToString(), out count))
            {
                throw new Exception("Err Select !");
            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public Route Route
        {
            get
            {
                if (_route == null)
                {
                    //_route = new Route(this.baseSN.ROUTE_ID,GetRouteType.ROUTEID, sfcdb,);
                    //_route.Init()
                    //(this.RouteID, sfcdb, DBType);
                    //_route = new Route();
                }
                return _route;
            }
        }

        //Row_R_SN rBaseSN;
        public R_SN baseSN;
        OleExec sfcdb;
        MESDataObject.DB_TYPE_ENUM DBType;
        List<C_KEYPART> _keyPartList;
        Route _route;

        public SN() { }

        public void Unbond(OleExec db, string emp_no, string bu, DB_TYPE_ENUM dbType)
        {
            var KPS = db.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == baseSN.ID).ToList();
            string wo = baseSN.WORKORDERNO;

            //報廢的SN需要增加完工數量
            //edit by ZHB 20210607 增加完工數量前做判斷
            if (baseSN.COMPLETED_FLAG != "1")
            {
                db.ORM.Updateable<R_WO_BASE>().SetColumns(r => r.FINISHED_QTY == r.FINISHED_QTY + 1).Where(t => t.WORKORDERNO == wo).ExecuteCommand();
            }

            for (int i = 0; i < KPS.Count; i++)
            {
                KPS[i].VALID_FLAG = 0;
                if (KPS[i].SCANTYPE == "SystemSN" || KPS[i].SCANTYPE == "PCBA S/N" || KPS[i].SCANTYPE == "PPM S/N") //DCN主要用的後面兩種類型
                {
                    SN kpsn = new SN(KPS[i].VALUE, db, dbType);
                    if (kpsn.baseSN != null)
                    {
                        kpsn.baseSN.SHIPPED_FLAG = "0";
                        kpsn.baseSN.SHIPDATE = null;
                        kpsn.baseSN.EDIT_EMP = emp_no;
                        kpsn.baseSN.EDIT_TIME = DateTime.Now;
                        db.ORM.Updateable<R_SN>(kpsn.baseSN).Where(t => t.ID == kpsn.baseSN.ID).ExecuteCommand();

                        //DCN Need to update more 2 tables
                        if (bu.ToUpper().Equals("VNDCN"))
                        {
                            //update r_sn_link
                            db.ORM.Updateable<R_SN_LINK>().SetColumns(t => new R_SN_LINK { VALIDFLAG = "0", EDITBY = emp_no, EDITTIME = DateTime.Now })
                                .Where(t => t.SN == baseSN.SN && t.CSN == kpsn.baseSN.SN).ExecuteCommand();
                            //upadte wwn_datasharing 3階
                            db.ORM.Updateable<WWN_Datasharing>().SetColumns(t => new WWN_Datasharing { CSKU = "N/A", CSSN = "N/A", lasteditby = emp_no, lasteditdt = DateTime.Now })
                                .Where(t => t.CSSN == baseSN.SN && t.VSSN == kpsn.baseSN.SN).ExecuteCommand();
                            //upadte wwn_datasharing 2階
                            db.ORM.Updateable<WWN_Datasharing>().SetColumns(t => new WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A", lasteditby = emp_no, lasteditdt = DateTime.Now })
                                .Where(t => t.VSSN == baseSN.SN && t.WSN == kpsn.baseSN.SN).ExecuteCommand();
                        }
                    }
                }
                KPS[i].EDIT_EMP = emp_no;
                KPS[i].EDIT_TIME = DateTime.Now;
                db.ORM.Updateable<R_SN_KP>(KPS[i]).Where(t => t.ID == KPS[i].ID).ExecuteCommand();
            }
            baseSN.SCRAPED_FLAG = "1";
            baseSN.SCRAPED_TIME = DateTime.Now;
            baseSN.CURRENT_STATION = "Unbond";
            baseSN.COMPLETED_FLAG = "1";
            baseSN.NEXT_STATION = "";
            baseSN.EDIT_EMP = emp_no;
            baseSN.EDIT_TIME = DateTime.Now;
            db.ORM.Updateable<R_SN>(baseSN).Where(t => t.ID == baseSN.ID).ExecuteCommand();
            //報廢的SN需要增加完工數量
            //db.ORM.Updateable<R_WO_BASE>().UpdateColumns(r => r.FINISHED_QTY == r.FINISHED_QTY + 1).Where(t => t.WORKORDERNO == wo).ExecuteCommand();
        }

        //add by LLF 2018-02-22 begin
        public void PanelAndSN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType)
        {
            PanelAndSNLoad(Sn, sfcdb, dbType);
        }
        //add by LLF 2018-02-22 end
        public SN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType, bool loadbyId = false)
        {
            if (loadbyId)
            {
                this.sfcdb = sfcdb;
                this.DBType = dbType;
                T_R_SN trsn = new T_R_SN(sfcdb, DBType);
                if (!string.IsNullOrEmpty(Sn))
                {
                    baseSN = sfcdb.ORM.Queryable<R_SN>().Where(t=>t.ID == Sn).First();
                }
                T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
                if (baseSN != null)
                {
                    Sn = baseSN.SN;
                    if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
                    {
                        _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
                    }
                    try
                    {
                        _route = new Route(baseSN.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    }
                    catch
                    {
                        var routes = sfcdb.ORM.Queryable<R_SKU_ROUTE, C_SKU>((r, c) => r.SKU_ID == c.ID).Where((r, c) => c.SKUNO == baseSN.SKUNO).Select((r, c) => r).ToList();
                        var f = routes.Find(t => t.DEFAULT_FLAG == "Y");
                        if (f != null)
                        {
                            _route = new Route(f.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                        }
                        else
                        {
                            if (routes.Count > 0)
                            {
                                _route = new Route(routes[0].ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                            }
                            else
                            {
                                _route = null;
                            }
                        }
                    }
                }
            }
            else
            {
                Load(Sn, sfcdb, dbType);
            }

        }

        public void PanelSN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType)
        {
            PanelLoad(Sn, sfcdb, dbType);
        }

        public void PanelLoad(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailByPanelSN(Sn, sfcdb);
            }
            T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
            if (baseSN != null)
            {
                if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
                {
                    _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
                }
            }

        }

        //Add by LLF 2018-02-22 begin
        public void PanelAndSNLoad(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailByPanelAndSN(Sn, sfcdb);
            }
        }
        //Add by LLF 2018-02-22 end

        public void Load(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailBySN(Sn, sfcdb);
            }
            T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
            if (baseSN != null)
            {
                if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
                {
                    _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
                }
                try
                {
                    _route = new Route(baseSN.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                }
                catch
                {
                    var routes = sfcdb.ORM.Queryable<R_SKU_ROUTE, C_SKU>((r, c) => r.SKU_ID == c.ID).Where((r, c) => c.SKUNO == baseSN.SKUNO).Select((r, c) => r).ToList();
                    var f = routes.Find(t => t.DEFAULT_FLAG == "Y");
                    if (f != null)
                    {
                        _route = new Route(f.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    }
                    else
                    {
                        if (routes.Count > 0)
                        {
                            _route = new Route(routes[0].ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                        }
                        else
                        {
                            _route = null;
                        }
                    }
                }
            }

        }

        public void LoadById(string ID, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(ID))
            {
                baseSN = trsn.GetDetailByID(ID, sfcdb);
            }
            T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
            if (baseSN != null)
            {
                if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
                {
                    _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
                }
                try
                {
                    _route = new Route(baseSN.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                }
                catch
                {
                    var routes = sfcdb.ORM.Queryable<R_SKU_ROUTE, C_SKU>((r, c) => r.SKU_ID == c.ID).Where((r, c) => c.SKUNO == baseSN.SKUNO).Select((r, c) => r).ToList();
                    var f = routes.Find(t => t.DEFAULT_FLAG == "Y");
                    if (f != null)
                    {
                        _route = new Route(f.ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    }
                    else
                    {
                        if (routes.Count > 0)
                        {
                            _route = new Route(routes[0].ROUTE_ID, GetRouteType.ROUTEID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                        }
                        else
                        {
                            _route = null;
                        }
                    }
                }
            }

        }

        public void Reload(string Sn, OleExec sfcdb)
        {
            Load(Sn, sfcdb, DBType);
        }
        /// <summary>
        /// 獲取實際連板數量
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="PanelFlag"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        //Add by LLF 2018-01-27 Begin
        public int GetLinkQty(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            int LinkQty = 1;
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            List<R_SN> ListSN = new List<R_SN>();

            T_R_SN R_SN = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                ListSN = R_SN.GetRSNbySN(Sn, sfcdb);
                if (ListSN == null)
                {
                    ListSN = R_SN.GetRSNbyPsn(Sn, sfcdb);
                    LinkQty = ListSN.Count;
                }
            }
            return LinkQty;
        }
        /// <summary>
        /// 檢查序列號規則
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="RuleName"></param>
        /// <param name="sfcdb"></param>
        /// <param name="_DBType"></param>
        /// <returns></returns>
        //Add by LLF 2018-02-01 begin
        public bool CheckSNRule(string StrSN, string RuleName, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            bool CheckFlag = false;

            var useRegex = RuleName.ToUpper().StartsWith("REG://");
            //如果 RuleName 是 C_SN_RULE 里配置的规则,就按照该规则检查.Tiny 2020-02-18
            //否则使用简易规则
            var Rule = sfcdb.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == RuleName).First();
            if (useRegex || Rule != null)
            {
                return MESPubLab.MESStation.SNMaker.SNmaker.CkeckSNRule(StrSN, RuleName, sfcdb);
            }


            T_C_SN_RULE C_SN_RULE = new T_C_SN_RULE(sfcdb, DBType);
            CheckFlag = C_SN_RULE.CheckSNRule(StrSN, RuleName, sfcdb);
            return CheckFlag;
        }
        //Add by LLF 2018-02-01 end
        //public bool CheckSNExist(string StrSN,OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        //{
        //    bool CheckFlag = false;
        //    T_R_SN R_SN = new T_R_SN(sfcdb, DBType);
        //    CheckFlag = R_SN.CheckSNExists(StrSN,sfcdb);
        //    return CheckFlag;
        //}

        public R_SN LoadSN(string SerialNo, OleExec DB)
        {
            R_SN RSN = null;
            T_R_SN R_Sn = new T_R_SN(DB, DBType);
            RSN = R_Sn.LoadSN(SerialNo, DB);
            return RSN;
        }
        public R_SN CheckSn(string SerialNo, OleExec DB)
        {
            R_SN RSN = null;
            T_R_SN R_Sn = new T_R_SN(DB, DBType);
            RSN = R_Sn.CheckSn(SerialNo, DB);
            return RSN;
        }
        public R_SN CheckSnStatus(string SerialNo, OleExec DB)
        {
            R_SN serialno = null;
            T_R_SN _R_SN = new T_R_SN(DB, DBType);
            serialno = _R_SN.CheckSnStatus(SerialNo, DB);
            return serialno;
        }



        public R_PANEL_SN LoadPanelBySN(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            R_PANEL_SN R_Panel_SN = null;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            R_Panel_SN = R_PANEL_SN.GetPanelBySn(StrSN, sfcdb);
            return R_Panel_SN;
        }

        public bool CheckPanelVirtualSNExist(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            bool CheckFlag = false;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            CheckFlag = R_PANEL_SN.CheckPanelVirtualSN(StrSN, sfcdb);
            return CheckFlag;
        }
        /// <summary>
        /// 獲取虛擬SN add by LLF 2018-02-05
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="sfcdb"></param>
        /// <param name="_DBType"></param>
        /// <returns></returns>
        public R_PANEL_SN GetPanelVirtualSN(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            R_PANEL_SN Row = null;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            Row = R_PANEL_SN.GetPanelVirtualSN(StrSN, sfcdb);
            return Row;
        }

        public string StringListToString(List<string> Obj)
        {
            string Str = "";
            if (Obj.Count > 0)
            {
                for (int i = 0; i < Obj.Count; i++)
                {
                    if (i == Obj.Count - 1)
                    {
                        Str += Obj[i].ToString();
                    }
                    else
                    {
                        Str += Obj[i].ToString() + ",";
                    }
                }
            }
            return Str;
        }


        public override string ToString()
        {
            //return SerialNo;
            return this.baseSN == null ? null : SerialNo;
        }

        //void GetKeyPart(string PN, string seq)
        //{

        //}


        /// <summary>
        ///  刪除 r_sn_kp 
        ///  add by hgb 2019.07.28
        /// </summary>
        /// <param name="rowWo"></param>
        /// <param name="r_sn"></param>
        /// <param name="sfcdb"></param>
        /// <param name="Station"></param>
        /// <param name="sfcdbType"></param>
        /// <param name=""></param>
        public bool DeleteR_SN_KP(WorkOrder woObject, R_SN r_sn, OleExec sfcdb, MESPubLab.MESStation.MESStationBase Station, MESDataObject.DB_TYPE_ENUM sfcdbType)
        {
            string SNID = this.baseSN.ID;
            try
            {
                string strSql = $@"select * from  R_SN_KP WHERE R_SN_ID ='{SNID}' and station ='{Station.StationName}' ";
                DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];

                strSql = $@"DELETE R_SN_KP WHERE R_SN_ID ='{SNID}' and station ='{Station.StationName}' ";
                Station.SFCDB.ExecSelectOneValue(strSql);

                strSql = $@"select * from  R_SN_KP WHERE R_SN_ID ='{SNID}' and station ='{Station.StationName}' ";
                dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            }
            catch
            {
                return false;
            }
            return true;

        }

        /// <summary>
        ///  寫入 r_sn_kp BY STATION
        ///  ADD BY HGB 2019.07.29 HWT FI工站更新KEYPART用
        /// </summary>
        /// <param name="rowWo"></param>
        /// <param name="r_sn"></param>
        /// <param name="sfcdb"></param>
        /// <param name="Station"></param>
        /// <param name="sfcdbType"></param>
        /// <param name=""></param>
        public void InsertR_SN_KP_bySTATION(WorkOrder woObject, R_SN r_sn, OleExec sfcdb, MESPubLab.MESStation.MESStationBase Station, MESDataObject.DB_TYPE_ENUM sfcdbType)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(sfcdb, sfcdbType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(sfcdb, sfcdbType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, sfcdbType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(sfcdb, sfcdbType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(sfcdb, sfcdbType);
            T_C_ORACLE_MFASSEMBLYDATA t_c_oracle_mfassemblydata = new T_C_ORACLE_MFASSEMBLYDATA(sfcdb, sfcdbType);
            //T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, sfcdbType);
            T_C_PARTNO_EXCEPTION t_c_partno_exception = new T_C_PARTNO_EXCEPTION(sfcdb, sfcdbType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            int scanseq = 0;
            int result;
            string skuno = woObject.SkuNO;
            string skuMpn = "";
            try
            {

                // kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, sfcdb);
                kpItemList = t_c_kp_list_item.GetListItemByListIdStation(woObject.KP_LIST_ID, Station.StationName, sfcdb);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }

                foreach (C_KP_List_Item kpItem in kpItemList)
                {
                    //For Oracle ATO KP --start
                    bool isOraKP = false;
                    string[] OraLocation = null;
                    List<C_ORACLE_MFASSEMBLYDATA> orakp = new List<C_ORACLE_MFASSEMBLYDATA>();
                    string kpPno = kpItem.KP_PARTNO;
                    int countLoc = 0;
                    if (r_sn.PLANT == "TOGA")
                    {
                        if (t_c_partno_exception.ValueIsExist(kpItem.KP_PARTNO, sfcdb))
                        {
                            isOraKP = false;
                        }
                        else
                        {
                            orakp = t_c_oracle_mfassemblydata.GetLocations(skuno, kpPno, sfcdb);
                            if (orakp.Count > 0)
                            {
                                isOraKP = true;
                                if (orakp[0].LOCATION == null || orakp[0].LOCATION.ToString().Trim() == "")
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000273", new string[] { kpPno }));
                                }

                                OraLocation = orakp[0].LOCATION.Split(new char[] { ',' });
                                //if (OraLocation.Length != kpItem.QTY) //QTY between assembly mapping and KP QTY needs to be exactly matched.
                                //{
                                //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000274", new string[] { skuno, kpPno }));
                                //}
                                //make sure if scantype SN has been generated, if yes, count + 1 to find next location
                                countLoc = t_r_sn_kp.CountGeneratedKPPN(kpItem.KP_PARTNO, r_sn.SN, sfcdb);
                            }
                            else
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000264", new string[] { skuno, kpPno }));
                            }
                        }
                    }

                    //For Oracle ATO KP --end
                    itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, sfcdb);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                    }

                    skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(sfcdb, woObject.SkuNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count != 0)
                    {
                        skuMpn = skuMpnList[0].MPN;
                    }

                    for (int i = 0; i < kpItem.QTY; i++)
                    {
                        if (Station.BU != "VERTIV" && Station.BU != "ORACLE")//don't understand why only check VERTIV? Anyway added Oracle...
                        {
                            scanseq = scanseq + 1;
                        }

                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            if (Station.BU == "VERTIV" || Station.BU == "ORACLE")
                            {
                                scanseq = scanseq + 1;
                            }
                            kpRule = c_kp_rule.GetKPRule(sfcdb, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                            if (kpRule == null)
                            {
                                //throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                throw new Exception($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Missing kpRule");
                            }
                            if (kpRule.REGEX == "")
                            {
                                ////throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                throw new Exception($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Rule in null");
                            }
                            rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                            rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, sfcdb);
                            rowSNKP.R_SN_ID = r_sn.ID;
                            rowSNKP.SN = r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kpItem.KP_PARTNO;
                            rowSNKP.KP_NAME = kpItem.KP_NAME;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kpItem.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kpItem.STATION;
                            rowSNKP.REGEX = kpRule.REGEX;
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            if (isOraKP)
                            {
                                rowSNKP.EXKEY1 = "LOCATION";
                                if (countLoc > OraLocation.Count() - 1)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000273", new string[] { kpItem.KP_PARTNO }));
                                }
                                // rowSNKP.EXVALUE1 = OraLocation[i];
                                rowSNKP.EXVALUE1 = OraLocation[countLoc].ToString().Trim().ToUpper();
                                //20190109 FTX patty added new logic for Oracle ---start
                                //if multiple MPN existed then do not wirte MPN and REGEX into R_SN_KP 
                                if (skuMpnList.Count > 1)
                                {
                                    rowSNKP.MPN = "";
                                    if (rowSNKP.SCANTYPE == "MPN")
                                    {
                                        rowSNKP.REGEX = "";
                                    }
                                }
                                //20190109 FTX patty added new logic for Oracle ---end
                            }

                            result = Convert.ToInt32(sfcdb.ExecSQL(rowSNKP.GetInsertString(sfcdbType)));
                            if (result <= 0)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }
                        } // end foreach for itemDetail

                        //When finish itemDetail (all scantype), count next location/
                        countLoc++;
                    }
                }
                #region add BEGIN by hgb 2019.08.09,HWTFI檢查二階MAC TC0005需要插入一筆MACSON，以便檢查下階MAC
                //3.綁定MAC檢查(1階，2階，3階)//目前只有到2階，以後如果有三階，在加個管控類型        
                T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (t_c_control.ValueIsExist("TC0005", r_sn.SKUNO, sfcdb) && Station.StationName == "FI")
                {
                    string kp_name = t_c_control.GetControlList("TC0005", r_sn.SKUNO, sfcdb)[0].CONTROL_LEVEL;
                    rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                    rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, sfcdb);
                    rowSNKP.R_SN_ID = r_sn.ID;
                    rowSNKP.SN = r_sn.SN;
                    rowSNKP.VALUE = "";
                    rowSNKP.PARTNO = "MACSON";
                    rowSNKP.KP_NAME = kp_name + "的MAC";
                    rowSNKP.MPN = "";
                    rowSNKP.SCANTYPE = "SN";
                    rowSNKP.ITEMSEQ = 100;
                    rowSNKP.SCANSEQ = 100; ;
                    rowSNKP.DETAILSEQ = 100;
                    rowSNKP.STATION = "FI";
                    rowSNKP.REGEX = "^[0-9,A-Z]{12}$";//kpRule.REGEX;
                    rowSNKP.VALID_FLAG = 1;
                    rowSNKP.EXKEY1 = "";
                    rowSNKP.EXVALUE1 = "";
                    rowSNKP.EXKEY2 = "";
                    rowSNKP.EXVALUE2 = "";
                    rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                    result = Convert.ToInt32(sfcdb.ExecSQL(rowSNKP.GetInsertString(sfcdbType)));
                    if (result <= 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                    }
                }

                #endregion add BEGIN by hgb 2019.08.09,FI檢查二階MAC TC0005需要插入一筆MACSON，以便檢查下階MAC



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        ///  寫入 r_sn_kp 
        /// </summary>
        /// <param name="rowWo"></param>
        /// <param name="r_sn"></param>
        /// <param name="sfcdb"></param>
        /// <param name="Station"></param>
        /// <param name="sfcdbType"></param>
        /// <param name=""></param>
        public void InsertR_SN_KP(WorkOrder woObject, R_SN r_sn, OleExec sfcdb, MESPubLab.MESStation.MESStationBase Station, MESDataObject.DB_TYPE_ENUM sfcdbType)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(sfcdb, sfcdbType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(sfcdb, sfcdbType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, sfcdbType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(sfcdb, sfcdbType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(sfcdb, sfcdbType);
            T_C_ORACLE_MFASSEMBLYDATA t_c_oracle_mfassemblydata = new T_C_ORACLE_MFASSEMBLYDATA(sfcdb, sfcdbType);
            //T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, sfcdbType);
            T_C_PARTNO_EXCEPTION t_c_partno_exception = new T_C_PARTNO_EXCEPTION(sfcdb, sfcdbType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            int scanseq = 0;
            int result;
            string skuno = woObject.SkuNO;
            string skuMpn = "";
            try
            {

                kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, sfcdb);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }
                var kpcatch = new List<Row_R_SN_KP>();
                foreach (C_KP_List_Item kpItem in kpItemList)
                {
                    //For Oracle ATO KP --start
                    bool isOraKP = false;
                    string[] OraLocation = null;
                    List<C_ORACLE_MFASSEMBLYDATA> orakp = new List<C_ORACLE_MFASSEMBLYDATA>();
                    string kpPno = kpItem.KP_PARTNO;
                    int countLoc = 0;
                    if (r_sn.PLANT == "TOGA")
                    {
                        if (t_c_partno_exception.ValueIsExist(kpItem.KP_PARTNO, sfcdb))
                        {
                            isOraKP = false;
                        }
                        else
                        {
                            orakp = t_c_oracle_mfassemblydata.GetLocations(skuno, kpPno, sfcdb);
                            if (orakp.Count > 0)
                            {
                                isOraKP = true;
                                if (orakp[0].LOCATION == null || orakp[0].LOCATION.ToString().Trim() == "")
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000273", new string[] { kpPno }));
                                }

                                OraLocation = orakp[0].LOCATION.Split(new char[] { ',' });
                                //if (OraLocation.Length != kpItem.QTY) //QTY between assembly mapping and KP QTY needs to be exactly matched.
                                //{
                                //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000274", new string[] { skuno, kpPno }));
                                //}
                                //make sure if scantype SN has been generated, if yes, count + 1 to find next location
                                countLoc = t_r_sn_kp.CountGeneratedKPPN(kpItem.KP_PARTNO, r_sn.SN, sfcdb);
                            }
                            else
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000264", new string[] { skuno, kpPno }));
                            }
                        }
                    }

                    //For Oracle ATO KP --end
                    itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, sfcdb);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                    }

                    skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(sfcdb, woObject.SkuNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count != 0)
                    {
                        skuMpn = skuMpnList[0].MPN;
                    }
                    else
                    {
                        skuMpn = "";
                    }

                    for (int i = 0; i < kpItem.QTY; i++)
                    {
                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            scanseq = scanseq + 1;
                            kpRule = c_kp_rule.GetKPRule(sfcdb, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);

                            //#region define new function automatically get values to populate variables of REGEX.
                            //// [PN] partno from keypart setting.
                            //// [REV] revsion of partno from table R_WO_ITEM.REVLV.
                            //if (kpRule.REGEX.StartsWith("{{") && kpRule.REGEX.EndsWith("}}"))
                            //{
                            //    kpRule.REGEX = kpRule.REGEX.Replace("{{", "").Replace("}}", "");
                            //    if (kpRule.REGEX.Contains("[PN]"))
                            //    {
                            //        kpRule.REGEX = kpRule.REGEX.Replace("[PN]", kpRule.PARTNO);
                            //    }
                            //    if (kpRule.REGEX.Contains("[REV]"))
                            //    {
                            //        var pnrev = sfcdb.ORM.Queryable<R_WO_ITEM>()
                            //             .Where(t => t.AUFNR == woObject.WorkorderNo && t.MATNR == kpRule.PARTNO)
                            //             .Select(t => t.REVLV)
                            //             .First();
                            //        if (pnrev != null)
                            //        {
                            //            kpRule.REGEX = kpRule.REGEX.Replace("[REV]", pnrev);
                            //        }
                            //    }
                            //}
                            //#endregion

                            if (Station.BU == "HWD" && kpRule == null)
                            {
                                //对于SYSTEMSN 不需要配置规制
                                if (itemDetail.SCANTYPE.ToUpper() == "SYSTEMSN")
                                {
                                    kpRule = new C_KP_Rule() { MPN = skuMpn, PARTNO = kpItem.KP_PARTNO, SCANTYPE = itemDetail.SCANTYPE, REGEX = $@"[\w\W]*" };
                                }
                            }

                            ///ALLPART带料无需设置规则
                            if (("APTRSN,AUTOAP").IndexOf(itemDetail.SCANTYPE) == -1)
                            {
                                if (kpRule == null)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110603", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                    //throw new Exception($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Missing kpRule");
                                }

                                if (kpRule.REGEX == "")
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110857", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                    //throw new Exception($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Rule in null");
                                }
                            }

                            rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                            rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, sfcdb);
                            rowSNKP.R_SN_ID = r_sn.ID;
                            rowSNKP.SN = r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kpItem.KP_PARTNO;
                            rowSNKP.KP_NAME = kpItem.KP_NAME;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kpItem.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kpItem.STATION;
                            rowSNKP.REGEX = kpRule != null ? kpRule.REGEX : "";
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            //rowSNKP.EXKEY1 = itemDetail.LOCATION.Length > 0 ? "LOCATION" : "";
                            //增加LOCATION add by Eden
                            rowSNKP.LOCATION = new Func<string>(() =>
                            {
                                if (itemDetail.LOCATION != null && itemDetail.LOCATION.Length > 0)
                                {
                                    var locations = itemDetail.LOCATION.Split('|');
                                    if (i < locations.Length)
                                        return $@"{locations[i]}-1";
                                    else
                                        return $@"{locations.LastOrDefault()}-{i - locations.Length + 2}";
                                    //return locations.Length <= i ? locations.LastOrDefault() : locations[i];
                                }
                                else
                                {
                                    ///上傳KPNAME禁止以-符號結尾
                                    var locationSeq = kpcatch.Count > 0 ? kpcatch.FindAll(t => t.LOCATION.StartsWith($@"{kpItem.KP_NAME}-") && t.LOCATION.Length < (kpItem.KP_NAME.Length + 4)).Count + 1 : 1;
                                    return $@"{kpItem.KP_NAME}-{locationSeq}";
                                }
                            })();
                            result = Convert.ToInt32(sfcdb.ExecSQL(rowSNKP.GetInsertString(sfcdbType)));
                            if (result <= 0)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }
                            kpcatch.Add(rowSNKP);
                        } // end foreach for itemDetail

                        //When finish itemDetail (all scantype), count next location/
                        countLoc++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  20190305 Patty added: FTX Oracle ATO for r_sn_kp 
        /// </summary>
        /// <param name="rowWo"></param>
        /// <param name="r_sn"></param>
        /// <param name="sfcdb"></param>
        /// <param name="Station"></param>
        /// <param name="sfcdbType"></param>
        /// <param name=""></param>
        public void ORAInsertR_SN_KP(WorkOrder woObject, R_SN r_sn, OleExec sfcdb, MESPubLab.MESStation.MESStationBase Station, MESDataObject.DB_TYPE_ENUM sfcdbType)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(sfcdb, sfcdbType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(sfcdb, sfcdbType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, sfcdbType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(sfcdb, sfcdbType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(sfcdb, sfcdbType);
            T_C_ORACLE_MFASSEMBLYDATA t_c_oracle_mfassemblydata = new T_C_ORACLE_MFASSEMBLYDATA(sfcdb, sfcdbType);
            //T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, sfcdbType);
            T_C_PARTNO_EXCEPTION t_c_partno_exception = new T_C_PARTNO_EXCEPTION(sfcdb, sfcdbType);
            Row_R_SN_KP rowSNKP;
            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();

            //20190312 patty added for ATO
            List<C_ORACLE_MFASSEMBLYDATA> orakp = new List<C_ORACLE_MFASSEMBLYDATA>();
            //T_R_WO_ITEM t_r_wo_item = new T_R_WO_ITEM(sfcdb, sfcdbType);
            T_C_KP_GROUP t_c_kp_group = new T_C_KP_GROUP(sfcdb, sfcdbType);
            C_KP_GROUP kpGroupObj = new C_KP_GROUP();
            T_C_KP_GROUP_PARTNO t_c_kp_group_partno = new T_C_KP_GROUP_PARTNO(sfcdb, sfcdbType);
            List<C_KP_GROUP_PARTNO> kpGroupPNList = new List<C_KP_GROUP_PARTNO>();
            string PF = woObject.SKU_NAME;
            bool isOraKP = false;
            int scanseq = 0;
            int result;
            string skuno = woObject.SkuNO;
            string skuMpn = "";

            double WOQTY = Convert.ToDouble(woObject.WORKORDER_QTY);
            try
            {

                kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, sfcdb);
                kpItemList = sfcdb.ORM.Queryable<C_KP_List_Item>().Where((e) => e.LIST_ID == woObject.KP_LIST_ID).ToList();

                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }

                //Get component list which not from WO but need to be within KP list. 
                //if flag = 1, all PN under the KP group need to be added to KP generation for the units
                var FTXKPLIST = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_GROUP, C_KP_GROUP_PARTNO>((I, G, GP) => new object[]{
                    SqlSugar.JoinType.Left,I.KP_PARTNO==G.GROUPNAME,
                    SqlSugar.JoinType.Left,G.ID==GP.KP_GROUP_ID
                })
                .Where((I, G, GP) => I.LIST_ID == woObject.KP_LIST_ID && G.REQUIRED_FLAG == "1" && GP.PARTNO != null)
                .Select((I, G, GP) => new { KPITEMID = I.ID, I.KP_PARTNO, I.KP_NAME, PN = GP.PARTNO, I.STATION, I.SEQ, KPQTY = SqlSugar.SqlFunc.ToDouble(I.QTY), WOPNQTY = SqlSugar.SqlFunc.ToDouble(I.QTY) }).ToList();


                //component from table C_PARTNO_MAPPING , TYPE = KP_LIST, for BOM shows PartA but cloudMES need to scan PartB, etc., it's on PF level
                var KPFROMMAPPING = sfcdb.ORM.Queryable<C_KP_LIST, C_KP_List_Item, C_KP_GROUP, C_KP_GROUP_PARTNO, R_WO_BOM, C_PARTNO_MAPPING>((L, I, G, GP, WB, PM) => new object[]{
                    SqlSugar.JoinType.Left,L.ID==I.LIST_ID,
                    SqlSugar.JoinType.Left,I.KP_PARTNO==G.GROUPNAME,
                    SqlSugar.JoinType.Left,G.ID==GP.KP_GROUP_ID,
                    SqlSugar.JoinType.Inner,GP.PARTNO==WB.MATNR,
                    SqlSugar.JoinType.Left,PM.VALUE1==WB.MATNR && L.SKUNO == PM.SKUNO
                })
                .Where((L, I, G, GP, WB, PM) => L.FLAG == "1" && I.LIST_ID == woObject.KP_LIST_ID && G.REQUIRED_FLAG == "0" && WB.AUFNR == woObject.WorkorderNo && PM.CTYPE == "KP_LIST")
                .OrderBy((L, I, G, GP, WB, PM) => I.SEQ)
                .Select((L, I, G, GP, WB, PM) => new { KPITEMID = I.ID, I.KP_PARTNO, I.KP_NAME, PN = PM.VALUE2, STATION = PM.VALUE4, I.SEQ, KPQTY = SqlSugar.SqlFunc.ToDouble(PM.VALUE3) * (SqlSugar.SqlFunc.ToDouble(WB.MENGE) / WOQTY), WOPNQTY = SqlSugar.SqlFunc.ToDouble(WB.MENGE) / WOQTY }).ToList();

                //component from table C_PARTNO_MAPPING , TYPE = KP_FIX_QTY, for BOM shows multiple PartA with differnet QTY but cloudMES need to scan only particular QYU, it's on SKU level
                var KPFROMMAPPING2 = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_GROUP, C_KP_GROUP_PARTNO, R_WO_BOM, R_MFPRESETWOHEAD, C_PARTNO_MAPPING>((I, G, GP, WB, MF, PM) => new object[]{
                    SqlSugar.JoinType.Left,I.KP_PARTNO==G.GROUPNAME,
                    SqlSugar.JoinType.Left,G.ID==GP.KP_GROUP_ID,
                    SqlSugar.JoinType.Inner,GP.PARTNO==WB.MATNR,
                    SqlSugar.JoinType.Inner,MF.WO==WB.AUFNR,
                    SqlSugar.JoinType.Left,PM.VALUE1==WB.MATNR && (MF.SKUNO == PM.SKUNO || MF.GROUPID == PM.SKUNO)
                })
                .Where((I, G, GP, WB, MF, PM) => I.LIST_ID == woObject.KP_LIST_ID && G.REQUIRED_FLAG == "0" && WB.AUFNR == woObject.WorkorderNo && PM.CTYPE == "KP_FIX_QTY" && SqlSugar.SqlFunc.ToDouble(PM.VALUE3) == (SqlSugar.SqlFunc.ToDouble(WB.MENGE) / SqlSugar.SqlFunc.ToDouble(MF.WOQTY)))
                .OrderBy((I, G, GP, WB, MF, PM) => I.SEQ)
                .Select((I, G, GP, WB, MF, PM) => new { KPITEMID = I.ID, I.KP_PARTNO, I.KP_NAME, PN = PM.VALUE2, STATION = PM.VALUE4, I.SEQ, KPQTY = SqlSugar.SqlFunc.ToDouble(PM.VALUE3), WOPNQTY = SqlSugar.SqlFunc.ToDouble(WB.MENGE) / WOQTY }).ToList();

                var PNmapping = sfcdb.ORM.Queryable<C_PARTNO_MAPPING>().Where((t) => t.SKUNO == PF || t.SKUNO == skuno).Select((t) => t.VALUE1).ToList();

                //Get component list based on WO. 
                var KPFORWOITEM = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_GROUP, C_KP_GROUP_PARTNO, R_WO_BOM>((I, G, GP, WB) => new object[]{
                    SqlSugar.JoinType.Left,I.KP_PARTNO==G.GROUPNAME,
                    SqlSugar.JoinType.Left,G.ID==GP.KP_GROUP_ID,
                    SqlSugar.JoinType.Inner,GP.PARTNO==WB.MATNR
                })
                .Where((I, G, GP, WB) => I.LIST_ID == woObject.KP_LIST_ID && G.REQUIRED_FLAG == "0" && WB.AUFNR == woObject.WorkorderNo && !PNmapping.Contains(GP.PARTNO) && I.KP_NAME == null && GP.PARTNO != null)
                .OrderBy((I, G, GP, WB) => I.SEQ)
                .Select((I, G, GP, WB) => new { KPITEMID = I.ID, I.KP_PARTNO, I.KP_NAME, PN = GP.PARTNO, I.STATION, I.SEQ, KPQTY = SqlSugar.SqlFunc.ToDouble(WB.MENGE) / WOQTY, WOPNQTY = SqlSugar.SqlFunc.ToDouble(WB.MENGE) / WOQTY }).ToList();

                if (KPFORWOITEM.Count == 0)
                {
                    throw new Exception("KP LIST for this WO is worng, please contact IT to check!");
                }

                KPFORWOITEM.AddRange(FTXKPLIST);
                KPFORWOITEM.AddRange(KPFROMMAPPING.Distinct());
                KPFORWOITEM.AddRange(KPFROMMAPPING2.Distinct());
                //--------------------------20190405 Patty modified code ---start
                var KPFinalList = FTXKPLIST.ToList();
                KPFinalList.Clear();
                //get control value for KP_NAME
                var ControlKPName = sfcdb.ORM.Queryable<C_CONTROL>().Where((t) => t.CONTROL_NAME == "KP_NAME").Select((t) => new { t.CONTROL_TYPE, t.CONTROL_VALUE }).ToList();

                string strKPRepeatLoop = "";
                DataTable dtKPRepeatLoop = null;
                var KPSpecial = KPFORWOITEM.ToList();
                KPSpecial.Clear();
                double? SEQtemp = 0.0;

                //loop for ABAB component sequences display
                foreach (var controlKP in ControlKPName)
                {
                    strKPRepeatLoop = "select REGEXP_COUNT(location,'" + controlKP.CONTROL_VALUE + "') keycount," +
                        "I.ID,I.KP_PARTNO,I.KP_NAME, GP.PARTNO,I.STATION, I.SEQ, AD.LOCATION " +
                        "from C_KP_LIST_ITEM I left join C_KP_GROUP G on I.KP_PARTNO = G.GROUPNAME " +
                        "left join C_KP_GROUP_PARTNO GP on GP.KP_GROUP_ID = G.ID " +
                        "inner join R_WO_BOM WB on WB.MATNR = GP.PARTNO " +
                        "inner join C_ORACLE_MFASSEMBLYDATA AD on AD.CUSTPARTNO = GP.PARTNO and configheaderid = '" + skuno + "' " +
                        "where I.LIST_ID = '" + woObject.KP_LIST_ID + "' and G.REQUIRED_FLAG = '0' " +
                        "and WB.AUFNR = '" + woObject.WorkorderNo + "'and GP.PARTNO not in (SELECT VALUE1 FROM C_PARTNO_MAPPING where skuno in(select skuno from r_wo_base where workorderno=WB.AUFNR)) " +
                        "and KP_NAME = '" + controlKP.CONTROL_TYPE + "' " +
                        "and REGEXP_COUNT(location,'" + controlKP.CONTROL_VALUE + "') <> 0 order by I.SEQ";

                    dtKPRepeatLoop = sfcdb.ExecuteDataSet(strKPRepeatLoop, CommandType.Text, null).Tables[0];

                    for (int i = 0; i < dtKPRepeatLoop.Rows.Count; i++)
                    {
                        if (i == 0 && KPSpecial.Count == 0)
                        {
                            SEQtemp = Convert.ToDouble(dtKPRepeatLoop.Rows[i]["SEQ"].ToString());
                        }
                        else
                        {
                            SEQtemp = Math.Round(Convert.ToDouble(KPSpecial.Max(t => t.SEQ)) + 0.01, 2);
                        }

                        KPSpecial.Add(new { KPITEMID = dtKPRepeatLoop.Rows[i]["ID"].ToString(), KP_PARTNO = dtKPRepeatLoop.Rows[i]["KP_PARTNO"].ToString(), KP_NAME = dtKPRepeatLoop.Rows[i]["KP_NAME"].ToString(), PN = dtKPRepeatLoop.Rows[i]["PARTNO"].ToString(), STATION = dtKPRepeatLoop.Rows[i]["STATION"].ToString(), SEQ = SEQtemp, KPQTY = Convert.ToDouble(dtKPRepeatLoop.Rows[i]["keycount"].ToString()), WOPNQTY = 0.00 });
                    }

                }
                KPFORWOITEM.AddRange(KPSpecial);
                KPFinalList = KPFORWOITEM.OrderBy(o => o.SEQ).ToList();


                //20190604 Patty added: check all component in R_WO_BOM
                var PNNoScan = sfcdb.ORM.Queryable<C_PARTNO_EXCEPTION>().Where((t) => t.EXCEPTIONTYPE == "NoNeedScan").Select((t) => t.PARTNO).ToList();
                var WOBOM = sfcdb.ORM.Queryable<R_WO_BOM>().Where((t) => t.AUFNR == woObject.WorkorderNo && !PNNoScan.Contains(t.MATNR) && !PNmapping.Contains(t.MATNR) && !SqlSugar.SqlFunc.Contains(t.MATNR, "-A")).Select((t) => new { t.MATNR }).ToList();
                foreach (var bomcheck in WOBOM)
                {
                    if (KPFinalList.Find(x => x.PN == bomcheck.MATNR) == null)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000275", new string[] { bomcheck.MATNR }));
                    }
                }
                //--------------------------20190405 Patty modified code ---end

                foreach (var kp in KPFinalList)
                {
                    //--check and get locaitons --------start
                    string[] OraLocation = null;
                    int countLoc = 0;
                    //20190604 Patty added: if PN in not in SkipLocation list, then get locaiton
                    if (t_c_partno_exception.ValueIsExist(kp.PN, sfcdb))
                    {
                        isOraKP = false;
                    }
                    else
                    {
                        orakp = t_c_oracle_mfassemblydata.GetLocations(skuno, kp.PN, sfcdb);
                        if (orakp.Count > 0)
                        {
                            isOraKP = true;
                            if (orakp[0].LOCATION == null || orakp[0].LOCATION.ToString().Trim() == "")
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000273", new string[] { kp.PN }));
                            }

                            OraLocation = orakp[0].LOCATION.Split(new char[] { ',' });
                            //if (OraLocation.Length != kp.KPQTY) //QTY between assembly mapping and KP QTY needs to be exactly matched.
                            //{
                            //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000274", new string[] { skuno, kp.PN }));
                            //}
                            //make sure if scantype SN has been generated, if yes, count + 1 to find next location
                            countLoc = t_r_sn_kp.CountGeneratedKPPN(kp.PN, r_sn.SN, sfcdb);
                        }
                        else
                        {
                            //isOraKP = false;
                            ////if the pn is not getting location and not in C_control, then throw error
                            ////20190513 Patty moved PN from c_control to c_partno_exception
                            //if (!t_c_partno_exception.ValueIsExist(kp.PN, sfcdb))
                            //{
                            //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000264", new string[] { skuno, kp.PN }));
                            //}
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000264", new string[] { skuno, kp.PN }));
                        }
                    }




                    //--check and get locaitons --------end
                    //--check item list
                    itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kp.KPITEMID, sfcdb);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                    }

                    //--check SKU MPN
                    skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(sfcdb, PF, kp.PN);
                    if (skuMpnList.Count != 0)
                    {
                        skuMpn = skuMpnList[0].MPN;
                    }


                    //start adding KP into R_SN_KP!!
                    for (int i = 0; i < kp.KPQTY; i++)
                    {
                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            scanseq = scanseq + 1;
                            kpRule = c_kp_rule.GetKPRule(sfcdb, kp.PN, skuMpn, itemDetail.SCANTYPE);
                            if (kpRule == null)
                            {
                                throw new Exception($@"PNO:{kp.PN},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Missing kpRule");
                            }
                            if (kpRule.REGEX == "")
                            {
                                throw new Exception($@"PNO:{kp.PN},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Rule is null");
                            }
                            rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                            rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, sfcdb);
                            rowSNKP.R_SN_ID = r_sn.ID;
                            rowSNKP.SN = r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kp.PN;
                            rowSNKP.KP_NAME = kp.KP_PARTNO;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kp.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kp.STATION;
                            rowSNKP.REGEX = kpRule.REGEX;
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            if (isOraKP)
                            {
                                rowSNKP.EXKEY1 = "LOCATION";
                                if (countLoc > OraLocation.Count() - 1)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000273", new string[] { kp.PN }));
                                }
                                rowSNKP.EXVALUE1 = OraLocation[countLoc].ToString().Trim().ToUpper();
                                //if multiple MPN existed then do not wirte MPN and REGEX into R_SN_KP 
                                if (skuMpnList.Count > 1)
                                {
                                    rowSNKP.MPN = "";
                                    if (rowSNKP.SCANTYPE == "MPN")
                                    {
                                        rowSNKP.REGEX = "";
                                    }
                                }
                            }

                            result = Convert.ToInt32(sfcdb.ExecSQL(rowSNKP.GetInsertString(sfcdbType)));
                            if (result <= 0)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }

                        }
                        countLoc++;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateSNKP(WorkOrder woObject, List<R_SN> snList, MESPubLab.MESStation.MESStationBase Station)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(Station.SFCDB, Station.DBType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            int scanseq = 0;
            int result;
            string skuMpn = "";
            try
            {

                kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, Station.SFCDB);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }
                foreach (R_SN r_sn in snList)
                {
                    //更新SN的KP=》刪除未綁定的所有KP項目，新增未過站之後的工站最新KP配置
                    Station.SFCDB.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == r_sn.ID && t.VALUE == null)
                        .ExecuteCommand();
                    //先刪除再添加
                    //if (!t_r_sn_kp.CheckLinkBySNID(r_sn.ID, Station.SFCDB))
                    //{
                    //    Station.SFCDB.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == r_sn.ID && t.VALUE == null)
                    //        .ExecuteCommand();
                    //    //result = t_r_sn_kp.DeleteBySNID(r_sn.ID, Station.SFCDB);
                    //}
                    var waitDoStationList = Station.SFCDB.ORM.Queryable<R_SN, C_ROUTE_DETAIL, C_ROUTE_DETAIL>(
                            (rs, cr1, cr2) => rs.ROUTE_ID == cr1.ROUTE_ID
                                              && rs.NEXT_STATION == cr1.STATION_NAME && cr1.ROUTE_ID == cr2.ROUTE_ID &&
                                              cr2.SEQ_NO >= cr1.SEQ_NO).Where((rs, cr1, cr2) => rs.ID == r_sn.ID).OrderBy((rs, cr1, cr2) => cr2.SEQ_NO, OrderByType.Asc).Select((rs, cr1, cr2) => cr2.STATION_NAME).ToList();
                    if (waitDoStationList.Count == 0)
                        throw new MESDataObject.MESReturnMessage($@"SN:{r_sn.SN} NextStation is not in Route,pls check! ");

                    var sncurentkpitemlist = kpItemList.FindAll(t => waitDoStationList.Contains(t.STATION));
                    foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
                    {
                        itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                        if (itemDetailList == null || itemDetailList.Count == 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                        }

                        skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(Station.SFCDB, woObject.SkuNO, kpItem.KP_PARTNO);
                        if (skuMpnList.Count == 0)
                        {
                            //throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110201", new string[] { kpItem.KP_PARTNO }));
                        }
                        skuMpn = skuMpnList[0].MPN;

                        var kpcatch = new List<Row_R_SN_KP>();
                        for (int j = 0; j < kpItem.QTY; j++)
                        {
                            foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                            {
                                scanseq = scanseq + 1;
                                kpRule = c_kp_rule.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                                if (kpRule == null)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                }
                                if (kpRule.REGEX == "")
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                }

                                rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                                rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                                rowSNKP.R_SN_ID = r_sn.ID;
                                rowSNKP.SN = r_sn.SN;
                                rowSNKP.VALUE = "";
                                rowSNKP.PARTNO = kpItem.KP_PARTNO;
                                rowSNKP.KP_NAME = kpItem.KP_NAME;
                                rowSNKP.MPN = skuMpn;
                                rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                                rowSNKP.ITEMSEQ = kpItem.SEQ;
                                rowSNKP.SCANSEQ = scanseq;
                                rowSNKP.DETAILSEQ = itemDetail.SEQ;
                                rowSNKP.STATION = kpItem.STATION;
                                rowSNKP.REGEX = kpRule.REGEX;
                                rowSNKP.VALID_FLAG = 1;
                                rowSNKP.EXKEY1 = "";
                                rowSNKP.EXVALUE1 = "";
                                rowSNKP.EXKEY2 = "";
                                rowSNKP.EXVALUE2 = "";
                                rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                                rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                                //增加LOCATION add by Eden  Copy By ZHB 20200711
                                rowSNKP.LOCATION = new Func<string>(() =>
                                {
                                    if (itemDetail.LOCATION != null && itemDetail.LOCATION.Length > 0)
                                    {
                                        var locations = itemDetail.LOCATION.Split('|');
                                        if (j < locations.Length)
                                            return $@"{locations[j]}-1";
                                        else
                                            return $@"{locations.LastOrDefault()}-{j - locations.Length + 2}";
                                        //return locations.Length <= i ? locations.LastOrDefault() : locations[i];
                                    }
                                    else
                                    {
                                        ///上傳KPNAME禁止以-符號結尾
                                        var locationSeq = kpcatch.Count > 0 ? kpcatch.FindAll(t => t.LOCATION.StartsWith($@"{kpItem.KP_NAME}-") && t.LOCATION.Length < (kpItem.KP_NAME.Length + 4)).Count + 1 : 1;
                                        return $@"{kpItem.KP_NAME}-{locationSeq}";
                                    }
                                })();
                                result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                                if (result <= 0)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                                }
                                kpcatch.Add(rowSNKP);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSNKP(string wo, List<R_SN> snList, MESPubLab.MESStation.MESStationBase Station)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(Station.SFCDB, Station.DBType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            R_WO_BASE woObj = null;
            int scanseq = 0;
            int result;
            string skuMpn = "";
            string skuno = "";
            string noNeedChangeKP = "";
            try
            {
                if (!string.IsNullOrEmpty(wo))
                {
                    woObj = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == wo).ToList().FirstOrDefault();
                    if (woObj == null)
                    {
                        throw new Exception($@"{wo} Not Exists!");
                    }
                    skuno = woObj.SKUNO;
                }
                else if (snList.Count > 0)
                {
                    skuno = snList[0].SKUNO;
                    wo = snList[0].WORKORDERNO;
                }
                if (string.IsNullOrEmpty(skuno) || string.IsNullOrEmpty(wo))
                {
                    throw new Exception("SKUNO And WO Error!");
                }
                string kpid = Station.SFCDB.ORM.Queryable<C_KP_LIST>().Where(r => r.SKUNO == skuno && r.FLAG == "1").ToList().FirstOrDefault().ID;
                kpItemList = t_c_kp_list_item.GetItemObjectByListId(kpid, Station.SFCDB);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { skuno }));
                }

                Station.SFCDB.ORM.Updateable<R_WO_BASE>().SetColumns(r => new R_WO_BASE { KP_LIST_ID = kpid }).Where(r => r.WORKORDERNO == wo).ExecuteCommand();

                foreach (R_SN r_sn in snList)
                {
                    noNeedChangeKP = ",";//待過工站的KP是APTRSN的已有綁過的不再重綁2021-04-23,按南寧邏輯，拳頭沒人家大
                    var noNeedChangeKPList = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.R_SN_ID == r_sn.ID && r.STATION == r_sn.NEXT_STATION && r.VALID_FLAG == 1
                         && r.SCANTYPE == "APTRSN" && r.VALUE != null).Select(r => r.PARTNO).Distinct().ToList();
                    foreach (string kp in noNeedChangeKPList)
                    {
                        noNeedChangeKP = noNeedChangeKP + "," + kp;
                    }

                    //更新SN的KP=》刪除未綁定的未過工站所有KP項目，新增未過站之後的工站最新KP配置
                    Station.SFCDB.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == r_sn.ID && t.VALUE == null && t.STATION != r_sn.NEXT_STATION).ExecuteCommand();

                    //更新SN的KP=》刪除未綁定的待過工站未綁料號所有KP項目，新增待過工站未綁料號最新KP配置
                    Station.SFCDB.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == r_sn.ID && t.VALUE == null && t.STATION == r_sn.NEXT_STATION && !SqlFunc.Contains(noNeedChangeKP, t.PARTNO)).ExecuteCommand();

                    //把已綁定的Keypart的shipped_flag該為0,以便重綁
                    List<string> listValue = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(k => k.R_SN_ID == r_sn.ID && k.STATION == r_sn.NEXT_STATION && k.VALID_FLAG == 1
                    && SqlFunc.Subqueryable<R_SN>().Where(r => k.VALUE == r.SN && r.VALID_FLAG == "1").Any())
                        .Select(k => k.VALUE).ToList();

                    Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(r => new R_SN { SHIPPED_FLAG = "0" })
                        .Where(r => r.VALID_FLAG == "1" && listValue.Contains(r.SN)).ExecuteCommand();

                    //當前待過工站已綁定的KP Update 為0                   
                    Station.SFCDB.ORM.Updateable<R_SN_KP>().UpdateColumns(r => new R_SN_KP { VALID_FLAG = 0 })
                        .Where(r => r.R_SN_ID == r_sn.ID && r.STATION == r_sn.NEXT_STATION && r.VALID_FLAG == 1 && !SqlFunc.Contains(noNeedChangeKP, r.PARTNO)).ExecuteCommand();

                    if (Station.BU.Equals("VNDCN"))
                    {
                        //update wwn_datasharing
                        //upadte 3階/出貨階
                        Station.SFCDB.ORM.Updateable<WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.CSSN == r_sn.SN && listValue.Contains(w.VSSN)).ExecuteCommand();
                        //upadte 2階
                        Station.SFCDB.ORM.Updateable<WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.VSSN == r_sn.SN && listValue.Contains(w.WSN)).ExecuteCommand();
                    }

                    var waitDoStationList = Station.SFCDB.ORM.Queryable<R_SN, C_ROUTE_DETAIL, C_ROUTE_DETAIL>(
                            (rs, cr1, cr2) => rs.ROUTE_ID == cr1.ROUTE_ID
                                              && rs.NEXT_STATION == cr1.STATION_NAME && cr1.ROUTE_ID == cr2.ROUTE_ID &&
                                              cr2.SEQ_NO >= cr1.SEQ_NO).Where((rs, cr1, cr2) => rs.ID == r_sn.ID).OrderBy((rs, cr1, cr2) => cr2.SEQ_NO, OrderByType.Asc).Select((rs, cr1, cr2) => cr2.STATION_NAME).ToList();
                    if (waitDoStationList.Count == 0)
                        continue;

                    var sncurentkpitemlist = kpItemList.FindAll(t => waitDoStationList.Contains(t.STATION));
                    foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
                    {
                        itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                        if (itemDetailList == null || itemDetailList.Count == 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { skuno }));
                        }

                        //ALLPART带料无需设置规则 Copy From Eden By ZHB 20200711
                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(Station.SFCDB, skuno, kpItem.KP_PARTNO);
                            if (("APTRSN,AUTOAP").IndexOf(itemDetail.SCANTYPE) == -1)
                            {
                                //skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(Station.SFCDB, skuno, kpItem.KP_PARTNO);
                                if (skuMpnList.Count == 0)
                                {
                                    //throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110201", new string[] { kpItem.KP_PARTNO }));
                                }
                                skuMpn = skuMpnList[0].MPN;
                            }
                            else
                            {
                                skuMpn = "";
                                if (skuMpnList.Count > 0)
                                {
                                    skuMpn = skuMpnList[0].MPN;
                                }
                            }
                        }

                        var kpcatch = new List<Row_R_SN_KP>();
                        for (int j = 0; j < kpItem.QTY; j++)
                        {
                            foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                            {
                                scanseq = scanseq + 1;
                                kpRule = c_kp_rule.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                                //ALLPART带料无需设置规则 Copy From Eden By ZHB 20200711
                                if (("APTRSN,AUTOAP").IndexOf(itemDetail.SCANTYPE) == -1)
                                {
                                    if (kpRule == null)
                                    {
                                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                    }
                                    if (kpRule.REGEX == "")
                                    {
                                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                    }
                                }

                                if (noNeedChangeKP.Contains(kpItem.KP_PARTNO) && kpItem.STATION == r_sn.NEXT_STATION) continue;//待過工站的KP是APTRSN的已有綁過的不再重綁

                                rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                                rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                                rowSNKP.R_SN_ID = r_sn.ID;
                                rowSNKP.SN = r_sn.SN;
                                rowSNKP.VALUE = "";
                                rowSNKP.PARTNO = kpItem.KP_PARTNO;
                                rowSNKP.KP_NAME = kpItem.KP_NAME;
                                rowSNKP.MPN = skuMpn;
                                rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                                rowSNKP.ITEMSEQ = kpItem.SEQ;
                                rowSNKP.SCANSEQ = scanseq;
                                rowSNKP.DETAILSEQ = itemDetail.SEQ;
                                rowSNKP.STATION = kpItem.STATION;
                                rowSNKP.REGEX = kpRule == null ? "" : kpRule.REGEX;//ALLPART带料无需设置规则 Copy From Eden By ZHB 20200711
                                rowSNKP.VALID_FLAG = 1;
                                rowSNKP.EXKEY1 = "";
                                rowSNKP.EXVALUE1 = "";
                                rowSNKP.EXKEY2 = "";
                                rowSNKP.EXVALUE2 = "";
                                rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                                rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                                //增加LOCATION add by Eden  Copy By ZHB 20200711
                                rowSNKP.LOCATION = new Func<string>(() =>
                                {
                                    if (itemDetail.LOCATION != null && itemDetail.LOCATION.Length > 0)
                                    {
                                        var locations = itemDetail.LOCATION.Split('|');
                                        if (j < locations.Length)
                                            return $@"{locations[j]}-1";
                                        else
                                            return $@"{locations.LastOrDefault()}-{j - locations.Length + 2}";
                                        //return locations.Length <= i ? locations.LastOrDefault() : locations[i];
                                    }
                                    else
                                    {
                                        ///上傳KPNAME禁止以-符號結尾
                                        var locationSeq = kpcatch.Count > 0 ? kpcatch.FindAll(t => t.LOCATION.StartsWith($@"{kpItem.KP_NAME}-") && t.LOCATION.Length < (kpItem.KP_NAME.Length + 4)).Count + 1 : 1;
                                        return $@"{kpItem.KP_NAME}-{locationSeq}";
                                    }
                                })();
                                result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                                if (result <= 0)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                                }
                                kpcatch.Add(rowSNKP);
                            }
                        }
                    }

                    Station.SFCDB.ORM.Updateable<R_SN>().SetColumns(r => new R_SN { KP_LIST_ID = kpid }).Where(r => r.ID == r_sn.ID).ExecuteCommand();

                    T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                    R_SN_LOG check_log = null;
                    check_log = new R_SN_LOG();
                    check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                    check_log.SNID = r_sn.ID;
                    check_log.SN = r_sn.SN;
                    check_log.LOGTYPE = "UpdateKP";
                    check_log.DATA1 = r_sn.ID;
                    check_log.DATA2 = kpid;
                    check_log.DATA3 = "";
                    check_log.DATA4 = "";
                    check_log.FLAG = "1";
                    check_log.CREATETIME = Station.SFCDB.ORM.GetDate();
                    check_log.CREATEBY = Station.LoginUser.EMP_NO;
                    Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LockCheck(OleExec db)
        {
            LockCheck(this.baseSN.SN, db);
            return;
            #region 将检查改为函数处理
            //2019/04/22 patty added: t.LOCK_STATION == "ALL" so WO or SN can be locked for ALL stations.
            //检查SN直接锁定的
            var nextRouteItem = Route.DETAIL.Find(t => t.STATION_NAME == this.NextStation);
            List<RouteDetail> BefRouteItems = null;
            if (nextRouteItem != null)
            {
                BefRouteItems = Route.DETAIL.FindAll(t => t.SEQ_NO <= nextRouteItem.SEQ_NO);
            }
            else
            {
                BefRouteItems = Route.DETAIL.FindAll(t => 1 == 1);
            }

            string[] stations = new string[BefRouteItems.Count];
            for (int i = 0; i < stations.Length; i++)
            {
                stations[i] = BefRouteItems[i].STATION_NAME;
            }

            var SnLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == this.baseSN.SN
             //&& stations.Contains(t.LOCK_STATION) 
             && (t.LOCK_STATION == this.NextStation || t.LOCK_STATION == "ALL")
             && t.LOCK_STATUS == "1"
             && t.TYPE == "SN").ToList();
            string ErrMsg = "";
            if (SnLock.Count > 0)
            {

                try
                {
                    ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { this.baseSN.SN, SnLock[0].LOCK_EMP, SnLock[0].LOCK_REASON });
                }
                catch
                {
                    throw new Exception($@"SN:'{this.baseSN.SN}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                }
                throw new Exception(ErrMsg);
            }

            //檢查機種被鎖
            var skuLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == this.baseSN.SKUNO
             && (t.LOCK_STATION == this.NextStation || t.LOCK_STATION == "ALL")
             && t.LOCK_STATUS == "1"
             && t.TYPE == "SKU").ToList();
            if (skuLock.Count > 0)
            {
                try
                {
                    ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { this.baseSN.WORKORDERNO, skuLock[0].LOCK_EMP, skuLock[0].LOCK_REASON });
                }
                catch
                {
                    throw new Exception($@"SKU:'{this.baseSN.WORKORDERNO}' Locked By:'{skuLock[0].LOCK_EMP}' Reason:'{skuLock[0].LOCK_REASON}' Station:'{skuLock[0].LOCK_STATION}'");
                }
                throw new Exception(ErrMsg);
            }
            //检查工单被锁的
            var WoLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == this.baseSN.WORKORDERNO
            //&& stations.Contains(t.LOCK_STATION)
            && (t.LOCK_STATION == this.NextStation || t.LOCK_STATION == "ALL")
            && t.LOCK_STATUS == "1"
            && t.TYPE == "WO").ToList();
            if (WoLock.Count > 0)
            {
                try
                {
                    ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { this.baseSN.WORKORDERNO, WoLock[0].LOCK_EMP, WoLock[0].LOCK_REASON });
                }
                catch
                {
                    throw new Exception($@"SN:'{this.baseSN.WORKORDERNO}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                }
                throw new Exception(ErrMsg);
            }

            WoLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == this.baseSN.WORKORDERNO
            && t.LOCK_STATUS == "1"
            && t.TYPE == "WO").ToList();
            if (WoLock.Count == 0)
            {
                return;
            }
            string[] WoStation = new string[WoLock.Count];
            for (int i = 0; i < WoStation.Length; i++)
            {
                WoStation[i] = WoLock[i].LOCK_STATION;
            }
            var jobStart = Route.DETAIL.FindAll(t => t.STATION_TYPE == "JOBSTART" && WoStation.Contains(t.STATION_NAME));
            if (jobStart.Count > 0)
            {
                throw new Exception($@"WO:'{this.baseSN.WORKORDERNO}' Locked 'JOBSTART' Call PQE");
            }
            #endregion 

        }

        public static void LockCheck(string sn, OleExec db, List<string> CheckedSN = null, string trace = "", bool useID = false)
        {
            var tc = sn;
            if (trace == "")
            {
                trace = sn;
            }
            else
            {
                trace += (">>" + sn);
            }

            if (CheckedSN == null)
            {
                CheckedSN = new List<string>();
            }

            if (CheckedSN.Contains(sn))
            {
                return;
            }

            SN checksn = null;// new SN(sn, db, DB_TYPE_ENUM.Oracle);

            if (useID == false)
            {
                try
                {
                    checksn = new SN(sn, db, DB_TYPE_ENUM.Oracle);
                }
                catch
                { }
            }
            else
            {
                try
                {
                    checksn = new SN(sn, db, DB_TYPE_ENUM.Oracle, true);
                }
                catch
                { }
            }


            if (checksn == null || checksn.baseSN == null)
            {
                var snBypass1 = db.ORM.Queryable<R_LOCK_BYPASS>().Where(t => t.TYPE == "SN" && t.VALUE1 == sn && t.BYPASS_STATUS == 1).First();
                if (snBypass1 != null)
                {
                    CheckedSN.Add(sn);
                    return;
                }
                var locks = db.ORM.Queryable<R_SN_LOCK>()
                            .Where((r) =>
                            r.SN == sn
                            && r.TYPE == "SN"
                            && r.LOCK_STATUS == "1"
                            )
                        .Distinct()
                        .ToList();
                if (locks.Count > 0)
                {
                    throw new Exception(trace + ":" + $@"SN:'{sn}' Locked By:'{locks[0].LOCK_EMP}' Reason:'{locks[0].LOCK_REASON}'");
                }
                CheckedSN.Add(sn);
                return;
            }
            sn = checksn.baseSN.SN;
            //
            var snBypass = db.ORM.Queryable<R_LOCK_BYPASS>().Where(t => t.TYPE == "SN" && t.VALUE1 == sn && t.BYPASS_STATUS == 1).First();
            if (snBypass != null)
            {
                CheckedSN.Add(sn);
                return;
            }
            var WoBypass = db.ORM.Queryable<R_LOCK_BYPASS>().Where(t => t.TYPE == "WO" && t.VALUE1 == checksn.WorkorderNo && t.BYPASS_STATUS == 1).First();
            if (WoBypass != null)
            {
                CheckedSN.Add(sn);
                return;
            }


            R_WO_BASE wo = db.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == checksn.baseSN.WORKORDERNO).First();

            try
            {
                //2019/04/22 patty added: t.LOCK_STATION == "ALL" so WO or SN can be locked for ALL stations.
                //检查SN直接锁定的
                var nextRouteItem = checksn.Route.DETAIL.Find(t => t.STATION_NAME == checksn.NextStation);
                List<RouteDetail> BefRouteItems = null;
                if (nextRouteItem != null)
                {
                    BefRouteItems = checksn.Route.DETAIL.FindAll(t => t.SEQ_NO <= nextRouteItem.SEQ_NO);
                }
                else
                {
                    BefRouteItems = checksn.Route.DETAIL.FindAll(t => 1 == 1);
                }

                string[] stations = new string[BefRouteItems.Count];
                for (int i = 0; i < stations.Length; i++)
                {
                    stations[i] = BefRouteItems[i].STATION_NAME;
                }

                var SnLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == checksn.baseSN.SN
                 //&& stations.Contains(t.LOCK_STATION) 
                 && (t.LOCK_STATION == checksn.NextStation || t.LOCK_STATION == "ALL")
                 && t.LOCK_STATUS == "1"
                 && t.TYPE == "SN").ToList();
                string ErrMsg = "";
                if (SnLock.Count > 0)
                {

                    try
                    {
                        ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { checksn.baseSN.SN, SnLock[0].LOCK_EMP, SnLock[0].LOCK_REASON });
                    }
                    catch
                    {
                        throw new Exception($@"SN:'{checksn.baseSN.SN}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                    }
                    throw new Exception(ErrMsg);
                }

                //檢查機種被鎖
                var skuLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == checksn.baseSN.SKUNO
                 && (t.LOCK_STATION == checksn.NextStation || t.LOCK_STATION == "ALL")
                 && t.LOCK_STATUS == "1"
                 && t.TYPE == "SKU" && (t.SN == null || t.SN == "" || t.SN == "ALL")).ToList();
                if (skuLock.Count > 0)
                {
                    try
                    {
                        ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { checksn.baseSN.WORKORDERNO, skuLock[0].LOCK_EMP, skuLock[0].LOCK_REASON });
                    }
                    catch
                    {
                        throw new Exception($@"SKU:'{checksn.baseSN.WORKORDERNO}' Locked By:'{skuLock[0].LOCK_EMP}' Reason:'{skuLock[0].LOCK_REASON}' Station:'{skuLock[0].LOCK_STATION}'");
                    }
                    throw new Exception(ErrMsg);
                }
                //鎖版本
                var VerLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == checksn.baseSN.SKUNO
                 && t.LOCK_STATUS == "1"
                 && t.TYPE == "SKU" && t.SN == wo.SKU_VER && t.LOCK_TIME < DateTime.Now).ToList();
                if (VerLock.Count > 0)
                {
                    throw new Exception($@"SKU:'{wo.SKUNO} Rev:{wo.SKU_VER}' Locked By:'{VerLock[0].LOCK_EMP}' Reason:'{VerLock[0].LOCK_REASON}' Station:'{VerLock[0].LOCK_STATION}'");
                }


                //检查工单被锁的
                var WoLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == checksn.baseSN.WORKORDERNO
                //&& stations.Contains(t.LOCK_STATION)
                && (t.LOCK_STATION == checksn.NextStation || t.LOCK_STATION == "ALL")
                && t.LOCK_STATUS == "1"
                && t.TYPE == "WO").ToList();
                if (WoLock.Count > 0)
                {
                    try
                    {
                        ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { checksn.baseSN.WORKORDERNO, WoLock[0].LOCK_EMP, WoLock[0].LOCK_REASON });
                    }
                    catch
                    {
                        throw new Exception($@"SN:'{checksn.baseSN.WORKORDERNO}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                    }
                    throw new Exception(ErrMsg);
                }

                WoLock = db.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == checksn.baseSN.WORKORDERNO
                && t.LOCK_STATUS == "1"
                && t.TYPE == "WO").ToList();
                if (WoLock.Count != 0)
                {


                    string[] WoStation = new string[WoLock.Count];
                    for (int i = 0; i < WoStation.Length; i++)
                    {
                        WoStation[i] = WoLock[i].LOCK_STATION;
                    }
                    var jobStart = checksn.Route.DETAIL.FindAll(t => t.STATION_TYPE == "JOBSTART" && WoStation.Contains(t.STATION_NAME));
                    if (jobStart.Count > 0)
                    {
                        throw new Exception($@"WO:'{checksn.baseSN.WORKORDERNO}' Locked 'JOBSTART' Call PQE");
                    }
                }

                #region juniper PO/SO lock
                O_ORDER_MAIN poinfo = null;
                try
                {
                    poinfo = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == checksn.WorkorderNo).First();
                }
                catch (Exception)
                {
                }
                List<R_SN_LOCK> locks = null;
                if (poinfo != null)
                {
                    var so = db.ORM.Queryable<O_I137_HEAD, O_I137_ITEM>((H, I) => new object[] { JoinType.Left, H.TRANID == I.TRANID }).Where((H, I) => I.ID == poinfo.ITEMID).Select((H, I) => H.SALESORDERNUMBER).First();
                    var pn = db.ORM.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == poinfo.ID).Select(t => t.FOXPN).Distinct().ToList();
                    locks = db.ORM.Queryable<R_SN_LOCK>()
                        .Where((r) =>
                        r.LOCK_STATUS == "1"
                        && (r.LOCK_STATION == checksn.NextStation || r.LOCK_STATION == "ALL")
                        && ((r.WORKORDERNO == poinfo.UPOID && r.TYPE == "POLine")
                            || (r.WORKORDERNO == so && r.TYPE == "SO")
                            || (pn.Contains(r.WORKORDERNO) && r.TYPE == "PN"))//增加检查PN锁
                        )
                      .Distinct()
                      .ToList();
                    if (locks.Count > 0)
                    {
                        throw new Exception($@"{locks[0].TYPE}:'{locks[0].WORKORDERNO}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                    }
                }
                #endregion

                //检查所有KP是否被锁定
                var kps = checksn.getSNKps(db);
                CheckedSN.Add(tc);
                int ii = 0;
                try
                {
                    for (ii = 0; ii < kps.Count; ii++)
                    {
                        if (kps[ii].KP_NAME == "KEEP_SN")
                        {
                            LockCheck(kps[ii].EXVALUE1, db, CheckedSN, trace, true);
                        }
                        else
                        {
                            LockCheck(kps[ii].VALUE, db, CheckedSN, trace);
                        }

                    }
                }
                catch (Exception eee)
                {
                    throw eee;
                }
            }
            catch (Exception ee)
            {
                throw new Exception(trace + ":" + ee.Message);

            }


        }

        //public List<R_SN_KP> getKps(OleExec db)
        //{
        //    return db.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == this.baseSN.ID && t.VALID_FLAG == 1).ToList();
        //}
        /// <summary>
        /// 查询同一个SN下各个阶段绑定的SystemSN KP
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<R_SN_KP> getSNKps(OleExec db)
        {
            //查询同一个SN下各个阶段绑定的SystemSN KP；
            return db.ORM.Queryable<R_SN_KP, R_SN>((kp, sn) => kp.R_SN_ID == sn.ID)
                .Where
                ((kp, sn) => kp.R_SN_ID == this.baseSN.ID && kp.VALID_FLAG == 1 && (sn.VALID_FLAG == "1" || sn.VALID_FLAG == "2") ).Select((kp, sn) => kp).ToList();
        }




        /// <summary>
        /// JOBSTOCK STATION PASS ACTION
        /// </summary>
        /// <param name="objWorkorder">wo obj</param>
        /// <param name="objSN">sn obj</param>
        /// <param name="Station">Station</param>
        /// <param name="confirmed_flag">confirmed_flag</param>
        public void JobStockPass(WorkOrder objWorkorder, SN objSN, MESPubLab.MESStation.MESStationBase Station, string confirmed_flag)
        {
            T_R_STOCK t_r_stock = new T_R_STOCK(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_R_STOCK_GT t_r_stock_gt = new T_R_STOCK_GT(Station.SFCDB, Station.DBType);
            T_C_SAP_STATION_MAP t_c_sap_station_map = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            string gt_id = "";

            #region  write r_stock,r_stock_gt           

            List<C_SAP_STATION_MAP> sapCodeList = t_c_sap_station_map.GetSAPStationMapBySkuOrderBySAPCodeASC(objWorkorder.SkuNO, Station.SFCDB);
            if (sapCodeList.Count == 0)
            {
                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { objWorkorder.SkuNO }));
            }
            Row_R_STOCK_GT rowStockGT;
            R_STOCK_GT objGT = t_r_stock_gt.GetNotGTbjByWO(objWorkorder.WorkorderNo, confirmed_flag, Station.SFCDB);
            if (objGT == null)
            {
                gt_id = t_r_stock_gt.GetNewID(Station.BU, Station.SFCDB);
                rowStockGT = (Row_R_STOCK_GT)t_r_stock_gt.NewRow();
                rowStockGT.ID = gt_id;
                rowStockGT.WORKORDERNO = objWorkorder.WorkorderNo;
                rowStockGT.SKUNO = objWorkorder.SkuNO;
                rowStockGT.TOTAL_QTY = 1;
                rowStockGT.FROM_STORAGE = objWorkorder.WorkorderNo;
                rowStockGT.TO_STORAGE = objWorkorder.STOCK_LOCATION;
                rowStockGT.SAP_FLAG = "0";
                rowStockGT.CONFIRMED_FLAG = confirmed_flag;
                rowStockGT.SAP_STATION_CODE = sapCodeList.Last().SAP_STATION_CODE;
                rowStockGT.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowStockGT.EDIT_TIME = Station.GetDBDateTime();
                Station.SFCDB.ExecSQL(rowStockGT.GetInsertString(Station.DBType));
            }
            else
            {
                rowStockGT = (Row_R_STOCK_GT)t_r_stock_gt.GetObjByID(objGT.ID, Station.SFCDB);
                gt_id = rowStockGT.ID;
                rowStockGT.TOTAL_QTY = rowStockGT.TOTAL_QTY + 1;
                rowStockGT.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowStockGT.EDIT_TIME = Station.GetDBDateTime();
                Station.SFCDB.ExecSQL(rowStockGT.GetUpdateString(Station.DBType));
            }

            Row_R_STOCK rowStock = (Row_R_STOCK)t_r_stock.NewRow();
            rowStock.ID = t_r_stock.GetNewID(Station.BU, Station.SFCDB);
            rowStock.SN = objSN.SerialNo;
            rowStock.WORKORDERNO = objWorkorder.WorkorderNo;
            rowStock.SKUNO = objWorkorder.SkuNO;
            rowStock.NEXT_STATION = objSN.NextStation;
            rowStock.FROM_STORAGE = objWorkorder.WorkorderNo;
            rowStock.TO_STORAGE = objWorkorder.STOCK_LOCATION;
            rowStock.CONFIRMED_FLAG = confirmed_flag;
            rowStock.SAP_FLAG = "0";
            rowStock.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowStock.EDIT_TIME = Station.GetDBDateTime();
            rowStock.GT_ID = gt_id;
            Station.SFCDB.ExecSQL(rowStock.GetInsertString(Station.DBType));
            #endregion

            #region update status
            Row_R_SN rowSN = (Row_R_SN)t_r_sn.GetObjByID(objSN.ID, Station.SFCDB);
            rowSN.NEXT_STATION = "JOBFINISH";
            rowSN.STOCK_STATUS = "1";
            rowSN.COMPLETED_FLAG = "1";
            rowSN.COMPLETED_TIME = Station.GetDBDateTime();
            rowSN.STOCK_IN_TIME = Station.GetDBDateTime();
            rowSN.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowSN.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(rowSN.GetUpdateString(Station.DBType));

            t_r_sn.RecordPassStationDetail(rowSN.GetDataObject(), Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            t_r_wo_base.UpdateFinishQty(objWorkorder.WorkorderNo, 1, Station.SFCDB);
            #endregion
        }

        public int UpdateKPSNID(OleExec db)
        {
            return db.ORM.Updateable<R_SN_KP>().UpdateColumns(r => new R_SN_KP { R_SN_ID = this.ID }).Where(r => r.SN == this.SerialNo).ExecuteCommand();
        }

        public object GetLabelValue()
        {
            return this.baseSN.SN;
        }

        public void ReworkUpateSNKP(R_SN newSnObj, MESPubLab.MESStation.MESStationBase Station, string reworkStation)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail c_kp_list_item_detail = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule t_c_kp_rule = new T_C_KP_Rule(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            DateTime sysDt = Station.GetDBDateTime();
            List<R_SN_KP> OldKPList = t_r_sn_kp.GetKPRecordBySnID(this.ID, sfcdb);

            List<C_ROUTE_DETAIL> listLastRoute = t_c_route_detail.GetAllNextStationsByCurrentStation(newSnObj.ROUTE_ID, reworkStation, sfcdb);
            string[] arryNextStation = listLastRoute.Select(r => r.STATION_NAME).ToArray();

            //update flag=0
            R_SN r_sn = null;
            List<R_SN> listKPSN = new List<R_SN>();

            foreach (var o in OldKPList)
            {
                r_sn = t_r_sn.GetSN(o.VALUE, sfcdb);
                if (r_sn != null && arryNextStation.Contains(o.STATION))
                {
                    //只修改重工工站及往後工站所綁定的系統SN的ShippingFlag
                    listKPSN.Add(r_sn);
                    if (Station.BU.Equals("VNDCN"))
                    {
                        //update wwn_datasharing
                        //upadte 3階/出貨階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.CSSN == newSnObj.SN && w.VSSN == o.VALUE).ExecuteCommand();
                        //upadte 2階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.VSSN == newSnObj.SN && w.WSN == o.VALUE).ExecuteCommand();
                    }
                }
                t_r_sn_kp.ReworkUpdateKP(o.ID, Station.LoginUser.EMP_NO, sfcdb);
            }
            t_r_sn.UpdateShippingFlag(listKPSN, "0", Station.LoginUser.EMP_NO, sfcdb);


            //重工工站前的KP保留,只更改SN ID
            List<R_SN_KP> listBeforeCurrentStationKP = OldKPList.Where(r => !arryNextStation.Contains(r.STATION)).ToList();
            foreach (var c in listBeforeCurrentStationKP)
            {
                c.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                c.R_SN_ID = newSnObj.ID;
                //c.EDIT_EMP = Station.LoginUser.EMP_NO;
                //c.EDIT_TIME = sysDt;
                t_r_sn_kp.Save(Station.SFCDB, c);
            }

            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            C_KP_Rule kpRule = new C_KP_Rule();
            Row_R_SN_KP rowSNKP;
            int result;
            string skuMpn = "";
            double? scanseq = 0;
            double? kpMaxSeq = listBeforeCurrentStationKP.Max(r => r.SCANSEQ);
            scanseq = kpMaxSeq == null ? 0 : kpMaxSeq;
            R_WO_BASE objWO = t_r_wo_base.GetWoByWoNo(newSnObj.WORKORDERNO, Station.SFCDB);
            if (string.IsNullOrEmpty(objWO.KP_LIST_ID))
            {
                return;
            }
            List<C_KP_List_Item> kpItemList = t_c_kp_list_item.GetItemObjectByListId(objWO.KP_LIST_ID, Station.SFCDB);

            //重工工站及之後的keypart            
            var sncurentkpitemlist = kpItemList.FindAll(t => arryNextStation.Contains(t.STATION));
            foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
            {
                itemDetailList = c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                if (itemDetailList == null || itemDetailList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWO.SKUNO }));
                }

                skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(Station.SFCDB, objWO.SKUNO, kpItem.KP_PARTNO);
                if (skuMpnList.Count == 0)
                {
                    //throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");//gan 
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110201", new string[] { kpItem.KP_PARTNO }));
                }
                skuMpn = skuMpnList[0].MPN;
                //添加新KEY
                var kpcatch = new List<Row_R_SN_KP>();
                for (int i = 0; i < kpItem.QTY; i++)
                {
                    foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                    {
                        scanseq = scanseq + 1;
                        kpRule = t_c_kp_rule.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                        if (kpRule == null)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        if (kpRule.REGEX == "")
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }

                        rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                        rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                        rowSNKP.R_SN_ID = newSnObj.ID;
                        rowSNKP.SN = newSnObj.SN;
                        rowSNKP.VALUE = "";
                        rowSNKP.PARTNO = kpItem.KP_PARTNO;
                        rowSNKP.KP_NAME = kpItem.KP_NAME;
                        rowSNKP.MPN = skuMpn;
                        rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                        rowSNKP.ITEMSEQ = kpItem.SEQ;
                        rowSNKP.SCANSEQ = scanseq;
                        rowSNKP.DETAILSEQ = itemDetail.SEQ;
                        rowSNKP.STATION = kpItem.STATION;
                        rowSNKP.REGEX = kpRule.REGEX;
                        rowSNKP.VALID_FLAG = 1;
                        rowSNKP.EXKEY1 = "";
                        rowSNKP.EXVALUE1 = "";
                        rowSNKP.EXKEY2 = "";
                        rowSNKP.EXVALUE2 = "";
                        rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rowSNKP.EDIT_TIME = sysDt;
                        rowSNKP.LOCATION = new Func<string>(() =>
                        {
                            if (itemDetail.LOCATION != null && itemDetail.LOCATION.Length > 0)
                            {
                                var locations = itemDetail.LOCATION.Split('|');
                                if (i < locations.Length)
                                    return $@"{locations[i]}-1";
                                else
                                    return $@"{locations.LastOrDefault()}-{i - locations.Length + 2}";
                                //return locations.Length <= i ? locations.LastOrDefault() : locations[i];
                            }
                            else
                            {
                                ///上傳KPNAME禁止以-符號結尾
                                var locationSeq = kpcatch.Count > 0 ? kpcatch.FindAll(t => t.LOCATION.StartsWith($@"{kpItem.KP_NAME}-") && t.LOCATION.Length < (kpItem.KP_NAME.Length + 4)).Count + 1 : 1;
                                return $@"{kpItem.KP_NAME}-{locationSeq}";
                            }
                        })();
                        result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                        if (result <= 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + newSnObj.SN, "ADD" }));
                        }
                        kpcatch.Add(rowSNKP);
                    }
                }
            }
        }

        public void ChangeSN(string sn)
        {
            this.baseSN.SN = sn;
        }

        /// <summary>
        /// 自動生成PPID條碼
        /// </summary>
        /// <returns></returns>
        public string AutoPPIDSNMaker(MESPubLab.MESStation.MESStationBase Station, SN _SN, R_SN_KP _PPIDKP)
        {
            string result = "";
            T_C_SKU_DETAIL _SKU_DETAIL = new T_C_SKU_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            //檢查機種是否設置MC Label規則：只有配置了CUSTPN_MAPPING的機種才會生成PPID條碼
            var skuDetail = _SKU_DETAIL.LoadData(_SN.SkuNo, "CUSTPN_MAPPING", "CUSTPN_SHARE", Station.SFCDB);
            if (skuDetail != null)
            {
                if (string.IsNullOrEmpty(skuDetail.VALUE))
                {
                    //result = $@"ERROR,機種:{_SN.SkuNo}未設置客戶料號(Cust PN),請聯繫PE![C_SKU_DETAIL.VALUE]";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143629", new string[] { _SN.SkuNo });
                    return result;
                }

                //檢查是否配置此編碼規則
                var shareFlag = false;
                T_C_SN_RULE _SN_RULE = null;
                T_C_SN_RULE_DETAIL _SN_RULE_DETAIL = null;
                C_SN_RULE rule = null;
                List<C_SN_RULE_DETAIL> ruleDetailList = null;
                if (!string.IsNullOrEmpty(skuDetail.EXTEND))
                {
                    shareFlag = true;
                    _SN_RULE = new T_C_SN_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    rule = _SN_RULE.GetDataByName(skuDetail.EXTEND, Station.SFCDB);
                    if (rule == null)
                    {
                        //result = $@"ERROR,機種:{_SN.SkuNo}設置的共用名稱:{skuDetail.EXTEND}無對應數據,請聯繫PE找IT新增![C_SN_RULE]";
                        result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144041", new string[] { _SN.SkuNo, skuDetail.EXTEND });
                        return result;
                    }
                    _SN_RULE_DETAIL = new T_C_SN_RULE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    ruleDetailList = _SN_RULE_DETAIL.GetDataByRuleID(skuDetail.EXTEND, Station.SFCDB);
                    if (ruleDetailList == null || ruleDetailList.Count == 0)
                    {
                        //result = $@"ERROR,機種:{_SN.SkuNo}設置的共用名稱:{skuDetail.EXTEND}無對應數據,請聯繫PE找IT新增![C_SN_RULE_DETAIL]";
                        result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144201", new string[] { _SN.SkuNo, skuDetail.EXTEND });
                        return result;
                    }
                }

                if (!shareFlag)
                {
                    //result = $@"ERROR,機種:{_SN.SkuNo}未設置MC Label規則,請聯繫PE![C_SKU_DETAIL.VALUE]";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144233", new string[] { _SN.SkuNo });
                    return result;
                }
                #region
                //檢查機種是否設置ServiceNo：只有配置了MCLABEL_SERVICE才會查詢操作R_BRCD_EXSN表
                //R_F_CONTROL linkService = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "MCLABEL_SERVICE" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == _SN.SkuNo && t.EXTVAL == Station.StationName).First();
                //if (linkService != null)
                //{
                //    //檢查SN是否已獲取ServiceNo
                //    R_BRCD_EXSN brcdExsn = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.SN == _SN.SerialNo).First();
                //    if (brcdExsn == null)
                //    {
                //        //按順序取得未綁定SN的可用的ServiceNo
                //        //brcdExsn = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.USE_FLAG == "0" && (t.SN == "" || t.SN == null))
                //        //    .OrderBy(t => t.SERVICE_NO.Substring(3, 1)).OrderBy(t => t.SERVICE_NO.Substring(2, 1))
                //        //    .OrderBy(t => t.SERVICE_NO.Substring(1, 1)).OrderBy(t => t.SERVICE_NO.Substring(0, 1)).First();

                //        brcdExsn = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.USE_FLAG == "0" && (t.SN == "" || t.SN == null))
                //            .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 3, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 2, 1))
                //            .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 1, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 0, 1)).First();

                //        if (brcdExsn == null)
                //        {
                //            result = $@"ERROR,系統中配置的SERVICE_NO已經用完,請聯繫PE![R_BRCD_EXSN]";
                //            return result;
                //        }
                //        brcdExsn.SN = _SN.SerialNo;
                //        brcdExsn.USE_FLAG = "1";
                //        brcdExsn.EDIT_EMP = "SYSTEM";
                //        brcdExsn.EDIT_TIME = Station.GetDBDateTime();
                //        Station.SFCDB.ORM.Updateable(brcdExsn).Where(t => t.SERVICE_NO == brcdExsn.SERVICE_NO && t.USE_FLAG == "0").ExecuteCommand();

                //        T_R_SN_KP SN_KP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //        Row_R_SN_KP row_KP = (Row_R_SN_KP)SN_KP.NewRow();
                //        row_KP.ID = SN_KP.GetNewID(Station.BU, Station.SFCDB);
                //        row_KP.R_SN_ID = _SN.ID;
                //        row_KP.SN = _SN.SerialNo;
                //        row_KP.VALUE = brcdExsn.SERVICE_NO;
                //        row_KP.PARTNO = "SER TAG";
                //        row_KP.KP_NAME = "SERVICE TAG";
                //        row_KP.MPN = "SER TAG";
                //        row_KP.SCANTYPE = "SERVICE TAG D/C";
                //        row_KP.STATION = Station.StationName;
                //        row_KP.VALID_FLAG = 1;
                //        row_KP.EDIT_TIME = Station.GetDBDateTime();
                //        row_KP.EDIT_EMP = "SYSTEM";
                //        row_KP.LOCATION = "SERVICE_NO";
                //        Station.SFCDB.ExecSQL(row_KP.GetInsertString(DB_TYPE_ENUM.Oracle));

                //        //DCN SFC系統還會更新一個表：s_loading_qty_alert，更新累加使用數量，沒發現有什麼用，MES暫時不添加
                //    }
                //    else
                //    {

                //        var SERVICE = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(T => T.SN == _SN.SerialNo&&T.VALUE== brcdExsn.SERVICE_NO&&T.KP_NAME== "SERVICE TAG").First();
                //        if (SERVICE == null)
                //        {
                //            T_R_SN_KP SN_KP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //            Row_R_SN_KP row_KP = (Row_R_SN_KP)SN_KP.NewRow();
                //            row_KP.ID = SN_KP.GetNewID(Station.BU, Station.SFCDB);
                //            row_KP.R_SN_ID = _SN.ID;
                //            row_KP.SN = _SN.SerialNo;
                //            row_KP.VALUE = brcdExsn.SERVICE_NO;
                //            row_KP.PARTNO = "SER TAG";
                //            row_KP.KP_NAME = "SERVICE TAG";
                //            row_KP.MPN = "SER TAG";
                //            row_KP.SCANTYPE = "SERVICE TAG D/C";
                //            row_KP.STATION = Station.StationName;
                //            row_KP.VALID_FLAG = 1;
                //            row_KP.EDIT_TIME = Station.GetDBDateTime();
                //            row_KP.EDIT_EMP = "SYSTEM";
                //            row_KP.LOCATION = "SERVICE_NO";
                //            Station.SFCDB.ExecSQL(row_KP.GetInsertString(DB_TYPE_ENUM.Oracle));
                //        }


                //    }

                //    //檢查SN是否有ST S/N類型的KP且未有此類型KP的綁定關係
                //    R_SN_KP _STKP = _SNKPList.Find(t => t.SCANTYPE == "ST S/N");
                //    if (_STKP != null && string.IsNullOrEmpty(_STKP.VALUE))
                //    {
                //        //ST S/N類型KP：將ServiceNo與SN綁定
                //        _STKP.VALUE = brcdExsn.SERVICE_NO;
                //        _STKP.EDIT_TIME = Station.GetDBDateTime();
                //        _STKP.EDIT_EMP = "SYSTEM";
                //        Station.SFCDB.ORM.Updateable(_STKP).Where(t => t.ID == _STKP.ID).ExecuteCommand();
                //    }
                //}
                #endregion
                //檢查SN是否已綁定TEMP S/N1或PPID S/N類型KP
                var hasLink = false;
                var prefix = ruleDetailList.Find(r => r.SEQ == 1).CURVALUE;
                hasLink = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALID_FLAG == 1 && t.SN == _SN.SerialNo && (t.SCANTYPE == "TEMP S/N1" || t.SCANTYPE == "PPID S/N") && SqlFunc.Contains(t.VALUE, prefix)).Any();
                if (hasLink)
                {
                    //result = $@"ERROR,機種:{_SN.SkuNo}設置的共用名稱:{skuDetail.EXTEND} 規則不匹配！ ";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144302", new string[] { _SN.SkuNo, skuDetail.EXTEND });
                    return result;
                }

                var ppidSN = "";
                //var hasOld = false;                
                T_R_SN_KP _SN_KP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //如果是重工工站則不生成新PPIDSN而是直接取得上次生成的PPIDSN
                //if (Station.StationName.ToUpper() == "REWORK" || CREATETYPE== "UpdateSNKP")
                //{
                //    var oldPPIDSn = Station.SFCDB.ORM.Queryable<R_SN_KP>()
                //        .Where(t => t.VALID_FLAG == 0 && t.SN == _SN.SerialNo && t.SCANTYPE == "TEMP S/N1")
                //        .OrderBy(t => t.EDIT_TIME, OrderByType.Desc).Select(t => t.VALUE).First();
                //    if (!string.IsNullOrEmpty(oldPPIDSn))
                //    {
                //        ppidSN = oldPPIDSn;
                //        hasOld = true;
                //    }
                //}

                ////如果不是重工工站或是重工工站但歷史綁定信息無ppidSN則生成新流水碼
                //if (!hasOld)
                //{
                //根據規則生成新流水碼：PPIDSN
                ppidSN = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(skuDetail.EXTEND, Station.SFCDB);

                if (!ppidSN.Contains(skuDetail.VALUE))
                {
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144345", new string[] { ppidSN, skuDetail.VALUE });
                    //result = $@"ERROR,PPID編碼規則錯誤，生成的新號碼:{ppidSN}與客戶料號:{skuDetail.VALUE}不匹配，請聯繫PE確認";
                    return result;

                }
                //while循環檢查流水碼是否已生成過直到生成新的流水碼
                while (Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.LOGTYPE == "AUTOPPIDSNMAKER" && t.DATA1 == ppidSN).Any())
                {
                    ppidSN = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(skuDetail.EXTEND, Station.SFCDB);
                }
                //檢查是否已在系統中綁定生成的流水碼                    
                hasLink = _SN_KP.CheckLinkByValue(ppidSN, Station.SFCDB);
                if (hasLink)
                {
                    //result = $@"ERROR,生成的新號碼:{ppidSN}已經存在系統中,請聯繫IT![R_SN_KP]";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144431", new string[] { ppidSN });
                    return result;
                }

                //寫入R_SN_LOG表
                T_R_SN_LOG _SN_LOG = new T_R_SN_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_SN_LOG row_rsLOG = (Row_R_SN_LOG)_SN_LOG.NewRow();
                row_rsLOG.ID = _SN_LOG.GetNewID(Station.BU, Station.SFCDB);
                row_rsLOG.SNID = _SN.ID;
                row_rsLOG.SN = _SN.SerialNo;
                row_rsLOG.LOGTYPE = "AUTOPPIDSNMAKER";
                row_rsLOG.DATA1 = ppidSN;
                row_rsLOG.DATA2 = _SN.SkuNo;
                row_rsLOG.DATA3 = Station.StationName;
                row_rsLOG.DATA4 = "2D";
                row_rsLOG.DATA5 = skuDetail.VALUE;
                row_rsLOG.DATA6 = "HP SN";
                row_rsLOG.DATA7 = "TEMP S/N1";
                row_rsLOG.CREATETIME = Station.GetDBDateTime();
                row_rsLOG.CREATEBY = "SYSTEM";
                Station.SFCDB.ExecSQL(row_rsLOG.GetInsertString(DB_TYPE_ENUM.Oracle));
                //}

                //寫入R_SN_KP表 - TEMP S/N1
                Row_R_SN_KP row_rsKP = (Row_R_SN_KP)_SN_KP.NewRow();
                row_rsKP.ID = _SN_KP.GetNewID(Station.BU, Station.SFCDB);
                row_rsKP.R_SN_ID = _SN.ID;
                row_rsKP.SN = _SN.SerialNo;
                row_rsKP.VALUE = ppidSN;
                row_rsKP.PARTNO = skuDetail.VALUE;
                row_rsKP.KP_NAME = "HP SN";
                row_rsKP.MPN = "";
                row_rsKP.SCANTYPE = "TEMP S/N1";
                row_rsKP.ITEMSEQ = _PPIDKP.ITEMSEQ;
                row_rsKP.SCANSEQ = _PPIDKP.SCANSEQ;
                row_rsKP.DETAILSEQ = _PPIDKP.DETAILSEQ;
                row_rsKP.STATION = _PPIDKP.STATION;
                row_rsKP.REGEX = _PPIDKP.REGEX;
                row_rsKP.VALID_FLAG = 1;
                row_rsKP.EDIT_TIME = Station.GetDBDateTime();
                row_rsKP.EDIT_EMP = "SYSTEM";
                row_rsKP.LOCATION = "HP SN";
                Station.SFCDB.ExecSQL(row_rsKP.GetInsertString(DB_TYPE_ENUM.Oracle));

                //個別機種寫入PPID S/N類型KP時還會加上擴展表配置的版本(非機種版本)
                string version = string.Empty;
                if (ppidSN.Contains("-"))
                {
                    version = Station.SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == _SN.SkuNo && t.CATEGORY_NAME == "REV").Select(t => t.VALUE).First();
                }
                result = ppidSN.Replace("-", "") + (string.IsNullOrEmpty(version) ? "" : version);
            }
            else
            {
                //PPID S/N類型的KP需要根據機種配置的MCLabel規則生成流水碼與SN綁定，所以機種未配置則不給繼續Loading
                //result = $@"ERROR,該機種存在PPID類型KP但未設置MC Label規則,請聯繫PE![C_SKU_DETAIL.CATEGORY=CUSTPN_MAPPING && C_SKU_DETAIL.CATEGORY_NAME=CUSTPN_SHARE]";
                result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144500");
                return result;
            }

            return result;
        }

        public string AutoSTSNLink(MESPubLab.MESStation.MESStationBase Station, SN _SN, string CREATETYPE)
        {
            string result = "";
            //檢查機種是否設置ServiceNo：只有配置了MCLABEL_SERVICE才會查詢操作R_BRCD_EXSN表
            //R_F_CONTROL linkService = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "MCLABEL_SERVICE" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == _SN.SkuNo && t.EXTVAL == Station.StationName).First();
            //if (linkService != null)
            //{
            //檢查SN是否已獲取ServiceNoMCLABEL_SERVICE
            R_BRCD_EXSN brcdsn = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.SN == _SN.SerialNo && t.USE_FLAG == "1").First();
            if (brcdsn == null)
            {
                //按順序取得未綁定SN的可用的ServiceNo
                R_BRCD_EXSN rbe = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.USE_FLAG == "0" && t.SN == null)
                    .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 3, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 2, 1))
                    .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 1, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 0, 1)).First();

                if (rbe == null)
                {
                    //result = "ERROR,系統中配置的SERVICE_NO已經用完,請聯繫PE![R_BRCD_EXSN]";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144544");
                    // return result;
                }
                else
                {
                    rbe.SN = _SN.SerialNo;
                    rbe.USE_FLAG = "1";
                    rbe.EDIT_EMP = "SYSTEM";
                    rbe.EDIT_TIME = Station.GetDBDateTime();
                    Station.SFCDB.ORM.Updateable(rbe).Where(t => t.SERVICE_NO == rbe.SERVICE_NO && t.USE_FLAG == "0").ExecuteCommand();
                    result = rbe.SERVICE_NO;
                }
            }

            if (Station.StationName.ToUpper() == "REWORK" || CREATETYPE == "UpdateSNKP")
            {
                var oldstsn = Station.SFCDB.ORM.Queryable<R_SN_KP>()
                    .Where(t => t.VALID_FLAG == 0 && t.SN == _SN.SerialNo && t.SCANTYPE == "ST S/N")
                    .OrderBy(t => t.EDIT_TIME, OrderByType.Desc).Select(t => t.VALUE).First();
                if (!string.IsNullOrEmpty(oldstsn))
                {
                    result = oldstsn;
                }
            }
            //}
            return result;
        }
        public string UpdateReturnSTKP(MESPubLab.MESStation.MESStationBase Station, SN _SN)
        {
            //string result = "ERROR,系統不存在SERVICE_NO，請聯繫PE！再進行退站";
            string result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144614");
            R_BRCD_EXSN brcdsn = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.SN == _SN.SerialNo && t.USE_FLAG == "1").First();
            if (brcdsn != null)
            {
                result = brcdsn.SERVICE_NO;
                return result;
            }
            else
            {
                R_BRCD_EXSN rbe = Station.SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.USE_FLAG == "0" && t.SN == null)
                       .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 3, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 2, 1))
                       .OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 1, 1)).OrderBy(t => SqlFunc.Substring(t.SERVICE_NO, 0, 1)).First();

                if (rbe == null)
                {
                    //result = "ERROR,系統中配置的SERVICE_NO已經用完,請聯繫PE![R_BRCD_EXSN]，再進行退站";
                    result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144646");
                    return result;
                }

                rbe.SN = _SN.SerialNo;
                rbe.USE_FLAG = "1";
                rbe.EDIT_EMP = "SYSTEM";
                rbe.EDIT_TIME = Station.GetDBDateTime();
                Station.SFCDB.ORM.Updateable(rbe).Where(t => t.SERVICE_NO == rbe.SERVICE_NO && t.USE_FLAG == "0").ExecuteCommand();
                result = rbe.SERVICE_NO;

            }
            return result;
        }

        public void PassStationTimeControl(SqlSugarClient DB, string stationName)
        {
            var control = DB.Queryable<R_F_CONTROL, R_F_CONTROL_EX>((r, e) => r.ID == e.DETAIL_ID)
                    .Where((r, e) => r.FUNCTIONNAME == "PassStationTimeControl" && r.CATEGORY == "SKUNO" && r.VALUE == SkuNo && r.CONTROLFLAG == "Y" && e.NAME == "NextStation" && e.VALUE == stationName)
                    .Select((r, e) => r).ToList().FirstOrDefault();
            if (control != null)
            {
                bool noControlWo = DB.Queryable<R_SN_PASS>().Any(r => r.TYPE == "WO" && r.WORKORDERNO == WorkorderNo && r.STATUS == "1");
                bool noControlSN = DB.Queryable<R_SN_PASS>().Any(r => r.TYPE == "SN" && r.SN == SerialNo && r.STATUS == "1");
                if (noControlWo || noControlSN)
                {
                    return;
                }
                var controlStation = DB.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == control.ID).ToList();
                if (controlStation.Count < 2)
                {
                    throw new MESReturnMessage($@"{SkuNo},[PassStationTimeControl] setting error.");
                }
                if (string.IsNullOrWhiteSpace(control.EXTVAL))
                {
                    throw new MESReturnMessage($@"{SkuNo},[PassStationTimeControl] is no set time.");
                }
                var firstStation = controlStation.Find(r => r.NAME == "FirstStation").VALUE;
                //var nextStation = controlStation.Find(r => r.NAME == "NextStation").VALUE;
                if (string.IsNullOrWhiteSpace(firstStation))
                {
                    throw new MESReturnMessage($@"{SkuNo},[PassStationTimeControl] is no set FirstStation.");
                }
                //if (string.IsNullOrWhiteSpace(nextStation))
                //{
                //    throw new MESReturnMessage($@"{SkuNo},[PassStationTimeControl] is no set NextStation.");
                //}
                //if (!nextStation.Equals(stationName))
                //{
                //    return;
                //}
                double controlTime = 0;
                if (!double.TryParse(control.EXTVAL, out controlTime))
                {
                    throw new MESReturnMessage($@"{SkuNo},[PassStationTimeControl] control time set error.");
                }
                if (controlTime > 0)
                {
                    DateTime? firstTime = DB.Queryable<R_SN_STATION_DETAIL>()
                        .Where(r => r.SN == SerialNo && r.VALID_FLAG == "1" && r.STATION_NAME == firstStation)
                        .OrderBy(r => r.EDIT_TIME).ToList().FirstOrDefault()?.EDIT_TIME;
                    if (firstTime == null)
                    {
                        return;
                    }
                    var tempTime = DB.GetDate().Subtract(Convert.ToDateTime(firstTime));
                    if (controlTime > tempTime.TotalMinutes)
                    {
                        throw new MESReturnMessage($@"sn:{SerialNo},control minutes:{controlTime},staying minutes:{tempTime.TotalMinutes};[PassStationTimeControl] not staying long enough.");
                    }
                }
            }
        }
    }
}