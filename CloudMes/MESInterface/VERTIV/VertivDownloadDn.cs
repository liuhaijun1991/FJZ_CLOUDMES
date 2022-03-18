using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDBHelper;
using MESDataObject.Module;
using MESPubLab;

namespace MESInterface.VERTIV
{
    public class VertivDownloadDn:taskBase
    {
        public string bu, plant, dbstr, cust;
        public bool IsRuning = false;
        public MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0003 ZRFC_SFC_NSG_0003;
        public DnBase dnBase;

        public override void init()
        {
            InitPara();
            InitView();
        }

        /// <summary>
        /// 加載配置參數
        /// </summary>
        void InitPara()
        {
            bu = ConfigGet("BU");
            plant = ConfigGet("PLANT");
            dbstr = ConfigGet("DB");
            cust = ConfigGet("CUST");
            dnBase = new DnBase(bu, plant, dbstr, cust);
        }

        /// <summary>
        /// 初始化顯示界面
        /// </summary>
        void InitView()
        {
            Output.UI = new VertivDownloadDn_UI(this);
            ZRFC_SFC_NSG_0003 = dnBase.ZRFC_SFC_NSG_0003;
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_CUSTOMER_PO"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_CUSTOMER_PO_ITEM"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_CUSTOMER_SO"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_DN_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_REPORT_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_TO_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003.GetTableValue("SD_TO_HEAD"));
        }

        /// <summary>
        /// 設置界面顯示數據源
        /// </summary>
        void SetViewDataSource()
        {
            ZRFC_SFC_NSG_0003 = dnBase.ZRFC_SFC_NSG_0003;
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("下載正在執行,請稍後再試");
            }
            IsRuning = true;
            try
            {
                if (dnBase.DnCreatDateTime != "" && !string.IsNullOrEmpty(dnBase.DnCreatDateTime))
                {
                    DownDnDataToMes();
                    ConverDataToVertiv();
                }
                else
                {
                    //add donwload lately 7 days
                    var rundate = 7;
                    while (rundate >= 0)
                    {
                        dnBase.DnCreatDateTime = DateTime.Now.AddDays(rundate - 7).ToString("yyyy-MM-dd");
                        DownDnDataToMes();
                        ConverDataToVertiv();
                        rundate--;
                    }
                    dnBase.DnCreatDateTime = "";
                }
                
            }
            catch { }
            finally
            {
                IsRuning = false;
            }
        }
        
 

        /// <summary>
        /// SAP 原始數據寫入MES
        /// </summary>
        void DownDnDataToMes()
        {
            dnBase.GetDnData();
            SetViewDataSource();
            #region By dn寫入DB
            foreach (var sdToDetail in dnBase.sdToDetailList)
            {
                if (dnBase.isShipOut(sdToDetail.VBELN))
                    continue;
                dnBase.DownDnDataToMes(sdToDetail.VBELN, sdToDetail.TKNUM);
            }
            #endregion
        }

        /// <summary>
        /// 從原始數據提取Vertiv需要的信息
        /// </summary>
        void ConverDataToVertiv()
        {
            foreach (var dn in dnBase.currentDnList)
            {
             
                SaveTo(dn);
                SaveDn(dn);

            }
        }

        void SaveTo(string dn)
        {
            var sdToDetail = dnBase.sdToDetailList.Where(x => x.VBELN == dn).Distinct().ToList();
            var sdToHead = dnBase.sdToHeadList.Where(x => x.TKNUM == sdToDetail.FirstOrDefault().TKNUM).Distinct().ToList();
            using (var db = OleExec.GetSqlSugarClient(dbstr, false))
            {
                var result = db.Ado.UseTran(() =>
                {
                    #region delete old data
                    db.Deleteable<R_TO_HEAD>().Where(x => x.TO_NO == sdToDetail[0].TKNUM).ExecuteCommand();
                    db.Deleteable<R_TO_DETAIL>().Where(x => x.DN_NO == dn).ExecuteCommand();
                    #endregion
                    #region new data
                    db.Insertable<R_TO_HEAD>(
                        new R_TO_HEAD()
                        {
                            ID = MesDbBase.GetNewID(db,bu, "R_TO_HEAD"),
                            TO_NO = sdToHead.FirstOrDefault().TKNUM,
                            PLAN_STARTIME = DateTime.Parse($@"{RFCDate(sdToHead.FirstOrDefault().DPREG)} {sdToHead.FirstOrDefault().UPREG}"),
                            PLAN_ENDTIME = DateTime.Parse($@"{RFCDate(sdToHead.FirstOrDefault().DPABF)} {sdToHead.FirstOrDefault().UPABF}"),
                            TO_CREATETIME = $@"{RFCDate(sdToHead.FirstOrDefault().ERDAT)} {sdToHead.FirstOrDefault().ERZET}",
                            CONTAINER_NO = sdToHead.FirstOrDefault().SIGNI,
                            VEHICLE_NO = sdToHead.FirstOrDefault().EXTI2,
                            EXTERNAL_NO = sdToHead.FirstOrDefault().EXTI1,
                            DROP_FLAG = "0"
                        }
                        ).ExecuteCommand();

                    db.Insertable<R_TO_DETAIL>(
                        new R_TO_DETAIL()
                        {
                            ID = MesDbBase.GetNewID(db, bu, "R_TO_DETAIL"),
                            TO_NO = sdToDetail.FirstOrDefault().TKNUM,
                            TO_ITEM_NO = sdToDetail.FirstOrDefault().TPNUM,
                            DN_NO = dn,
                            DN_CUSTOMER = sdToDetail.FirstOrDefault().KUNNR,
                            CREATETIME = System.DateTime.Now
                        }).ExecuteCommand();
                    #endregion
                });
                if (!result.IsSuccess)
                    WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.VERTIV.VertivDownloadDn", "VertivDownloadDn", $@"Dn:{dn};Err:{result.ErrorMessage}", "", "SaveTo", dn, "", "system","N");
            }
        }

        void SaveDn(string dn)
        {
            try
            {
                var sdDnDetail = dnBase.sdDnDetailList.Where(x => x.VBELN == dn).Distinct().ToList();
                var sdToDetail = dnBase.sdToDetailList.Where(x => x.VBELN == dn).Distinct().LastOrDefault();
                var sdToHead = dnBase.sdToHeadList.Where(x => x.TKNUM == sdToDetail.TKNUM).LastOrDefault();
                var GtRoute = dnBase.GetGtRoute(sdToDetail.KUNNR.Trim());
                var sfcodb = OleExec.GetSqlSugarClient(dbstr, false);

                # region FVN juniper 一個客戶代碼 兩種扣帳流程 C_SHIP_CUSTOMER  BILLTOCODE 必須配置廠別
                var y =sfcodb.Queryable<C_SHIP_CUSTOMER>().Where(x => x.CUSTOMERNAME == sdToDetail.KUNNR.Trim()).ToList();
                if (y.Count > 1) GtRoute = dnBase.GetGtRouteByPlant(sdToDetail.KUNNR.Trim(), sdToHead.WERKS);
                #endregion

                #region CustmerCode未配置GtRoute的Dn忽略
                if (GtRoute.Count == 0 && bu != "HWD")//HWD增加DownLoadDN做TGMES出貨但不拋帳
                {
                    WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.VERTIV.VertivDownloadDn", "VertivDownloadDn", $@"Dn:{dn};To:{sdToDetail.TKNUM};CustmerCode:{sdToDetail.KUNNR}未配置GtRoute", "", sdToDetail.TKNUM, dn, sdToDetail.KUNNR, "system","N");
                    return;
                }
                #endregion
                using (var db = OleExec.GetSqlSugarClient(dbstr, false))
                {
                    var result = db.Ado.UseTran(() =>
                    {
                        db.Deleteable<R_DN_STATUS>().Where(x => x.DN_NO == dn).ExecuteCommand();
                        foreach (var item in sdDnDetail)
                        {
                            db.Insertable<R_DN_STATUS>(new R_DN_STATUS()
                            {
                                ID = MesDbBase.GetNewID(db, bu, "R_DN_STATUS"),
                                DN_NO = dn,
                                DN_LINE = item.POSNR,
                                PO_NO = dnBase.sdCustmerSoList.Where(x => x.VBELN == dn && x.POSNR == item.POSNR).Single().BSTKD,
                                PO_LINE = dnBase.sdCustmerSoList.Where(x => x.VBELN == dn && x.POSNR == item.POSNR).Single().POSEX,
                                SO_NO = item.VGBEL,
                                SKUNO = DnBase.SkunoNameHandle(item.MATNR),
                                QTY = double.Parse(item.LFIMG),
                                //GTTYPE = GtRoute.LastOrDefault().ROUTENAME,
                                GTTYPE = bu != "HWD" ? GtRoute.LastOrDefault().ROUTENAME : "TGMES",//HWD增加DownLoadDN做TGMES出貨但不拋帳
                                GT_FLAG = "0",
                                DN_FLAG = "0",
                                DN_PLANT = sdToHead.WERKS,
                                //GTEVENT = GtRoute.FirstOrDefault().SEQ,
                                GTEVENT = bu != "HWD" ? GtRoute.FirstOrDefault().SEQ : "10",//HWD增加DownLoadDN做TGMES出貨但不拋帳
                                CREATETIME = System.DateTime.Now,
                                EDITTIME = System.DateTime.Now
                            }).ExecuteCommand();
                        }
                    });
                    if (!result.IsSuccess)
                        WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.VERTIV.VertivDownloadDn", "VertivDownloadDn", $@"Dn:{dn};Err:{result.ErrorMessage}", "", "SaveDn", dn, "", "system","N");
                }
            }
            catch (Exception e)
            {
                WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.VERTIV.VertivDownloadDn", "VertivDownloadDn", $@"Dn:{dn};Err:{e.Message}", "", "SaveDn", dn, "", "system","N");
            }
        }



        string RFCDate(object RFCDATE)
        {
            if (RFCDATE.ToString() == "0000-00-00")
            {
                return "1970-01-01";
            }
            else
            {
                return RFCDATE.ToString();
            }
        }
    }
}
