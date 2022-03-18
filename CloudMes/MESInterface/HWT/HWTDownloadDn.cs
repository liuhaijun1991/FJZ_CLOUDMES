using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESPubLab;

namespace MESInterface.HWT
{
    public class HWTDownloadDn:taskBase
    {
        public string bu, plant, dbstr, cust;
        public bool IsRuning = false;
        public MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0003E ZRFC_SFC_NSG_0003E;
        public DnBase_HWT DnBase_HWT;
        public string IsGGCS;
        public string _to;
        public OleExec SFCDB = null;

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
            DnBase_HWT = new DnBase_HWT(bu, plant, dbstr, cust);
            SFCDB = new OleExec(dbstr, false);
        }

        /// <summary>
        /// 初始化顯示界面
        /// </summary>
        void InitView()
        {
            Output.UI = new HWTDownloadDn_UI(this);
            ZRFC_SFC_NSG_0003E = DnBase_HWT.ZRFC_SFC_NSG_0003E;
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_PO"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_PO_ITEM"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_SO"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_DN_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_REPORT_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_TO_DETAIL"));
            Output.Tables.Add(ZRFC_SFC_NSG_0003E.GetTableValue("SD_TO_HEAD"));
        }

        /// <summary>
        /// 設置界面顯示數據源
        /// </summary>
        void SetViewDataSource()
        {
            ZRFC_SFC_NSG_0003E = DnBase_HWT.ZRFC_SFC_NSG_0003E;
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("下載正在執行,請稍後再試");
            }
            IsRuning = true;
            DownDnDataToMes();
            ConverDataToHWT();
            IsRuning = false;
        }
        
 

        /// <summary>
        /// SAP 原始數據寫入MES
        /// </summary>
        void DownDnDataToMes()
        {
            DnBase_HWT.GetDnData();
            SetViewDataSource();
            #region By dn寫入DB
            foreach (var sdToDetail in DnBase_HWT.sdToDetailList)
            {
                if (DnBase_HWT.isShipOut(sdToDetail.VBELN))
                    continue;
                DnBase_HWT.DownDnDataToMes(sdToDetail.VBELN, sdToDetail.TKNUM);
            }
            #endregion
        }

        /// <summary>
        /// 從原始數據提取Vertiv需要的信息
        /// </summary>
        void ConverDataToHWT()
        {
            foreach (var dn in DnBase_HWT.currentDnList)
            {
                SaveTo(dn);
                SaveDn(dn);
            }
        }

        void SaveTo(string dn)
        {
            int counts;           
            var sdToDetail = DnBase_HWT.sdToDetailList.Where(x => x.VBELN == dn).Distinct().ToList();
            var sdToHead = DnBase_HWT.sdToHeadList.Where(x => x.TKNUM == sdToDetail.FirstOrDefault().TKNUM).Distinct().ToList();
            using (var db = OleExec.GetSqlSugarClient(dbstr, false))
            {
                var result = db.Ado.UseTran(() =>
                {
                    //先判斷TO是否開始掃描
                    counts = db.Queryable<R_TO_HEAD_HWT>().Where(t => t.TO_NO == sdToDetail[0].TKNUM && t.REAL_STARTTIME != null).ToList().Count;
                    if (counts == 0)
                    {
                        if (sdToHead.FirstOrDefault().EXTI1 == "GG0001" || sdToHead.FirstOrDefault().EXTI1 == "CS0001")
                        {
                            IsGGCS = "GGCS";//標記TO是GG0001或CS0001的，後面再拿出來執行
                        }
                        else
                        {
                            IsGGCS = "NOGGCS";//不是是GG0001或CS0001的
                        }
                        //賦值一個TO後面處理用到
                        _to = sdToHead.FirstOrDefault().TKNUM;
                        #region delete old data
                        db.Deleteable<R_TO_HEAD_HWT>().Where(x => x.TO_NO == sdToDetail[0].TKNUM).ExecuteCommand();
                        db.Deleteable<R_TO_DETAIL_HWT>().Where(x => x.DN_NO == dn).ExecuteCommand();
                        #endregion
                        #region new data
                        db.Insertable<R_TO_HEAD_HWT>(
                            new R_TO_HEAD_HWT()
                            {
                                ID = MesDbBase.GetNewID(db, bu, "R_TO_HEAD_HWT"),
                                TO_NO = sdToHead.FirstOrDefault().TKNUM,
                                //PLAN_STARTIME = DateTime.Parse($@"{RFCDate(sdToHead.FirstOrDefault().DPREG)} {sdToHead.FirstOrDefault().UPREG}"),
                                //PLAN_ENDTIME = DateTime.Parse($@"{RFCDate(sdToHead.FirstOrDefault().DPABF)} {sdToHead.FirstOrDefault().UPABF}"),
                                //TO_CREATETIME = $@"{RFCDate(sdToHead.FirstOrDefault().ERDAT)} {sdToHead.FirstOrDefault().ERZET}",
                                TO_CREATETIME =SqlSugar.SqlFunc.ToDate($@"{RFCDate(sdToHead.FirstOrDefault().ERDAT)} {sdToHead.FirstOrDefault().ERZET}"),
                                CONTAINER_NO = sdToHead.FirstOrDefault().SIGNI,
                                VEHICLE_NO = sdToHead.FirstOrDefault().EXTI2,
                                EXTERNAL_NO = sdToHead.FirstOrDefault().EXTI1,
                                DROP_FLAG = "0",
                                TO_FLAG="0",
                                ABNORMITY_FLAG="0",
                                SECOND_FLAG="0",
                                SEND_FLAG="0",
                                ASN_FLAG="N",
                                EDIT_EMP="SYSTEM",
                                EDIT_TIME=DateTime.Now
                            }
                            ).ExecuteCommand();

                        db.Insertable<R_TO_DETAIL_HWT>(
                            new R_TO_DETAIL_HWT()
                            {
                                ID = MesDbBase.GetNewID(db, bu, "R_TO_DETAIL_HWT"),
                                TO_NO = sdToDetail.FirstOrDefault().TKNUM,
                                TO_ITEM_NO = sdToDetail.FirstOrDefault().TPNUM,
                                DN_NO = dn,
                                DN_CUSTOMER = sdToDetail.FirstOrDefault().KUNNR,
                                DN_FLAG="0",
                                SECOND_FLAG="0",
                                SEND_FLAG="0",
                                EDIT_EMP="SYSTEM",
                                EDIT_TIME = System.DateTime.Now
                            }).ExecuteCommand();
                        #endregion
                    }

                });
                if (!result.IsSuccess)
                    WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.HWT.HWTDownloadDn", "HWTDownloadDn", $@"Dn:{dn};Err:{result.ErrorMessage}", "", "SaveTo", dn, "", "system","N");
            }
        }

        void SaveDn(string dn)
        {
            int counts;
            
            try
            {
                var sdDnDetail = DnBase_HWT.sdDnDetailList.Where(x => x.VBELN == dn).Distinct().ToList();
                var sdToDetail = DnBase_HWT.sdToDetailList.Where(x => x.VBELN == dn).Distinct().LastOrDefault();
                var sdToHead = DnBase_HWT.sdToHeadList.Where(x => x.TKNUM == sdToDetail.TKNUM).LastOrDefault();
                //var GtRoute = DnBase_HWT.GetGtRoute(sdToDetail.KUNNR.Trim());
                //#region CustmerCode未配置GtRoute的Dn忽略
                //if (GtRoute.Count == 0)
                //{
                //    WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.HWT.HWTDownloadDn", "HWTDownloadDn", $@"Dn:{dn};To:{sdToDetail.TKNUM};CustmerCode:{sdToDetail.KUNNR}未配置GtRoute", "", sdToDetail.TKNUM, dn, sdToDetail.KUNNR, "system","N");
                //    return;
                //}
                //#endregion
                using (var db = OleExec.GetSqlSugarClient(dbstr, false))
                {
                    var result = db.Ado.UseTran(() =>
                    {
                        //先判斷DN是否開始掃描
                        counts = db.Queryable<R_DN_DETAIL>().Where(t => t.DN_NO == dn && t.REAL_QTY != 0).ToList().Count;
                        if (counts == 0)
                        {
                            db.Deleteable<R_DN_DETAIL>().Where(x => x.DN_NO == dn).ExecuteCommand();
                            foreach (var item in sdDnDetail)
                            {
                                db.Insertable<R_DN_DETAIL>(new R_DN_DETAIL()
                                {
                                    ID = MesDbBase.GetNewID(db, bu, "R_DN_DETAIL"),
                                    DN_NO = dn,
                                    DN_ITEM_NO = item.POSNR,
                                    P_NO = DnBase_HWT.SkunoNameHandle(item.MATNR),
                                    P_NO_DESC = item.ARKTX,
                                    NET_WEIGHT = item.NTGEW,
                                    GROSS_WEIGHT = item.BRGEW,
                                    VOLUME = item.VOLUM,
                                    PRICE = item.KBETR,
                                    P_NO_QTY = double.Parse(item.LFIMG),
                                    SO_NO = item.VGBEL,
                                    SO_ITEM_NO = item.POSNR,
                                    PO_NO = item.MATNR,//DnBase_HWT.sdCustmerSoList.Where(x => x.VBELN == dn && x.POSNR == item.POSNR).Single().BSTKD,
                                                       //PO_LINE = DnBase_HWT.sdCustmerSoList.Where(x => x.VBELN == dn && x.POSNR == item.POSNR).Single().POSEX,
                                    UNIT = item.GEWEI,
                                    //VERSION="",
                                    WAREHOUSE = item.LGORT,
                                    REAL_QTY = 0,
                                    DN_ITEM_FLAG = "0",
                                    SECOND_FLAG = "0",
                                    SEND_FLAG = "0",
                                    NN_GT = "0",
                                    CREATE_TIME = DateTime.Now
                                    //GTTYPE = GtRoute.LastOrDefault().ROUTENAME,
                                    //GT_FLAG = "0",
                                    //DN_FLAG = "0",
                                    //DN_PLANT = sdToHead.WERKS,
                                    //GTEVENT = GtRoute.FirstOrDefault().SEQ,
                                    //CREATETIME = System.DateTime.Now,
                                    //EDITTIME = System.DateTime.Now
                                }).ExecuteCommand();
                            }

                            if (IsGGCS == "GGCS")//Ship to Code 為GG0001或CS0001的插入r_shipping_notice表
                            {                                
                                string strSql = string.Format(@"select*from r_shipping_notice where to_no='{0}'", _to);
                                counts = db.Queryable<R_SHIPPING_NOTICE>().Where(t => t.TO_NO == _to).ToList().Count;
                                if (counts == 0)
                                {
                                    strSql = $@"INSERT INTO r_shipping_notice
                                                  (ship_no,
                                                   to_no,
                                                   ship_date,
                                                   ship_time,
                                                   send_car_no,
                                                   car_number,
                                                   ship_address,
                                                   ship_flag,
                                                   total_net_weight,
                                                   total_gross_weight,
                                                   total_pallet_num,
                                                   total_carton_num,
                                                   data1,
                                                   data2,
                                                   data3,
                                                   edit_by,
                                                   edit_time)
                                                  SELECT to_no,
                                                         to_no,
                                                         to_char(to_createtime, 'yyyy/mm/dd') to_createtime,
                                                         to_char(SYSDATE, 'hh24') || ':00',
                                                         'NA',
                                                         'NA',
                                                         deliver_address,
                                                         '0',
                                                         to_char(SUM(to_number(net_weight)), 'fm9990.0999') total_net_weight,
                                                         to_char(SUM(to_number(gross_weight)), 'fm9990.0999') tatol_gross_weight,
                                                         SUM(palletqty) total_pallet_num,
                                                         SUM(cartonqty) total_carton_num,
                                                         '',
                                                         '',
                                                         '',
                                                         'SYSTEM',
                                                         SYSDATE
                                                    FROM (SELECT a.to_no,
                                                                 b.dn_no,
                                                                 b.p_no,
                                                                 b.net_weight,
                                                                 b.gross_weight,
                                                                 b.p_no_qty,
                                                                 c.carton_qty,
                                                                 c.pallet_qty,
                                                                 d.deliver_address,
                                                                 e.to_createtime,
                                                                 ceil(b.p_no_qty / c.carton_qty) cartonqty,
                                                                 ceil(b.p_no_qty / (c.carton_qty * c.pallet_qty)) palletqty
                                                            FROM r_to_detail_hwt a,
                                                                 r_dn_detail b,
                                                                 (SELECT a.skuno,
                                                                         a.pack_type,
                                                                         a.max_qty   pallet_qty,
                                                                         b.pack_type,
                                                                         b.max_qty   carton_qty
                                                                    FROM (SELECT * FROM c_packing WHERE pack_type = 'PALLET') a,
                                                                         (SELECT * FROM c_packing WHERE pack_type = 'CARTON') b
                                                                   WHERE 1 = 1
                                                                     AND a.skuno = b.skuno) c,
                                                                 r_skuno_address d,
                                                                 r_to_head_hwt e
                                                           WHERE a.dn_no = b.dn_no
                                                             AND a.to_no = e.to_no
                                                             AND a.to_no = '{_to}'
                                                             AND b.p_no = c.skuno
                                                             AND c.skuno = d.fskuno
                                                             AND d.data2 = e.external_no)
                                                   GROUP BY to_no, deliver_address, to_createtime";
                                    SFCDB.ExecSQL(strSql);
                                }
                                //DataTable dt2 = MyDB.DoSelectDT(strSql);                                
                            }
                        }                            
                    });
                    if (!result.IsSuccess)
                        WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.HWT.HWTDownloadDn", "HWTDownloadDn", $@"Dn:{dn};Err:{result.ErrorMessage}", "", "SaveDn", dn, "", "system","N");
                }
            }
            catch (Exception e)
            {
                WriteLog.WriteIntoMESLog(dbstr, bu, "Interface", "MESInterface.HWT.HWTDownloadDn", "HWTDownloadDn", $@"Dn:{dn};Err:{e.Message}", "", "SaveDn", dn, "", "system","N");
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
