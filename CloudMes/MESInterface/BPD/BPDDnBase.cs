using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using MESDataObject.Module;

namespace MESInterface.BPD
{
    public class BPDDnBase
    {
        public string Bu, Plant, DbStr, Cust, DnCreatDateTime;
        public MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0003E ZRFC_SFC_NSG_0003E;
        public List<SD_CUSTMER_PO> sdCustmerPoList = new List<SD_CUSTMER_PO>();
        public List<SD_CUSTOMER_PO_ITEM> sdCustmerPoItemList = new List<SD_CUSTOMER_PO_ITEM>();
        public List<SD_CUSTOMER_SO> sdCustmerSoList = new List<SD_CUSTOMER_SO>();
        public List<SD_DN_DETAIL> sdDnDetailList = new List<SD_DN_DETAIL>();
        public List<SD_REPORT_DETAIL> sdReportDetailList = new List<SD_REPORT_DETAIL>();
        public List<SD_TO_DETAIL> sdToDetailList = new List<SD_TO_DETAIL>();
        public List<SD_TO_HEAD> sdToHeadList = new List<SD_TO_HEAD>();
        public List<string> currentDnList;


        public BPDDnBase(string bu, string plant, string dbStr, string cust)
        {
            Bu = bu; Plant = plant; DbStr = dbStr; Cust = cust;
            try
            {
                ZRFC_SFC_NSG_0003E = new MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0003E(Bu);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public void CallRfc()
        {
            if (DnCreatDateTime != "" && !string.IsNullOrEmpty(DnCreatDateTime))
            {
                ZRFC_SFC_NSG_0003E.SetValue(Bu, DnCreatDateTime, this.Cust, this.Plant, "");
            }
            else
            {
                ZRFC_SFC_NSG_0003E.SetValue(Bu, DateTime.Now.ToString("yyyy-MM-dd"), this.Cust, this.Plant, "");
            }
            ZRFC_SFC_NSG_0003E.CallRFC();
        }

        void RfcDataToModelList()
        {
            currentDnList = new List<string>();
            if (ZRFC_SFC_NSG_0003E.GetTableValue("SD_TO_HEAD").Rows.Count == 0)
                return;
            if (ZRFC_SFC_NSG_0003E.GetTableValue("SD_DN_DETAIL").Rows.Count == 0)
                return;
            sdCustmerPoList = MESDataObject.DataObjectTable.ConvertToEx<SD_CUSTMER_PO>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_PO"))?.Distinct().ToList();
            sdCustmerPoItemList = MESDataObject.DataObjectTable.ConvertToEx<SD_CUSTOMER_PO_ITEM>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_PO_ITEM"))?.Distinct().ToList();
            sdCustmerSoList = MESDataObject.DataObjectTable.ConvertToEx<SD_CUSTOMER_SO>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_CUSTOMER_SO"))?.Distinct().ToList();
            sdDnDetailList = MESDataObject.DataObjectTable.ConvertToEx<SD_DN_DETAIL>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_DN_DETAIL"))?.Distinct().ToList();
            sdReportDetailList = MESDataObject.DataObjectTable.ConvertToEx<SD_REPORT_DETAIL>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_REPORT_DETAIL"))?.Distinct().ToList();
            sdToDetailList = MESDataObject.DataObjectTable.ConvertToEx<SD_TO_DETAIL>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_TO_DETAIL"))?.Distinct().ToList();
            sdToHeadList = MESDataObject.DataObjectTable.ConvertToEx<SD_TO_HEAD>(ZRFC_SFC_NSG_0003E.GetTableValue("SD_TO_HEAD"))?.Distinct().ToList();
        }

        /// <summary>
        /// GetSapData到緩存
        /// </summary>
        public void GetDnData()
        {
            CallRfc();
            RfcDataToModelList();
        }

        public bool DownDnDataToMes(string dn, string to)
        {
            List<string> soList = new List<string>();
            foreach (DataRow dataRow in ZRFC_SFC_NSG_0003E.GetTableValue("SD_DN_DETAIL").Select($@"VBELN={dn}"))
                soList.Add(dataRow["VGBEL"].ToString());
            using (var db = OleExec.GetSqlSugarClient(this.DbStr, false))
            {
                var result = db.Ado.UseTran(() =>
                {
                    #region delete old data
                    db.Deleteable<MESDataObject.Module.SD_CUSTMER_PO>().Where(x => soList.Contains(x.VBELN)).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_CUSTOMER_PO_ITEM>().Where(x => soList.Contains(x.VBELN)).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_CUSTOMER_SO>().Where(x => x.VBELN == dn).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_DN_DETAIL>().Where(x => x.VBELN == dn).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_REPORT_DETAIL>().Where(x => x.VBELN == dn).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_TO_DETAIL>().Where(x => x.VBELN == dn).ExecuteCommand();
                    db.Deleteable<MESDataObject.Module.SD_TO_HEAD>().Where(x => x.TKNUM == to).ExecuteCommand();
                    #endregion
                    #region writer new data
                    db.Insertable<SD_CUSTMER_PO>(sdCustmerPoList.Where(x => soList.Contains(x.VBELN)).ToArray()).ExecuteCommand();
                    db.Insertable<SD_CUSTOMER_PO_ITEM>(sdCustmerPoItemList.Where(x => soList.Contains(x.VBELN)).ToArray()).ExecuteCommand();
                    db.Insertable<SD_CUSTOMER_SO>(sdCustmerSoList.Where(x => x.VBELN == dn).ToArray()).ExecuteCommand();
                    db.Insertable<SD_DN_DETAIL>(sdDnDetailList.Where(x => x.VBELN == dn).ToArray()).ExecuteCommand();
                    db.Insertable<SD_REPORT_DETAIL>(sdReportDetailList.Where(x => x.VBELN == dn).ToArray()).ExecuteCommand();
                    db.Insertable<SD_TO_DETAIL>(sdToDetailList.Where(x => x.VBELN == dn).ToArray()).ExecuteCommand();
                    db.Insertable<SD_TO_HEAD>(sdToHeadList.Where(x => x.TKNUM == to).ToArray()).ExecuteCommand();
                    #endregion
                    currentDnList.Add(dn);
                });
                if (result.IsSuccess)
                    return true;
                else
                    return false;
            }
        }



        /// <summary>
        /// 已經出貨的數據不再更新
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public bool isShipOut(string dn)
        {
            using (var db = OleExec.GetSqlSugarClient(this.DbStr, false))
            {
                var entity = db.Queryable<MESDataObject.Module.R_DN_STATUS>().With(SqlSugar.SqlWith.NoLock).Any(x => x.DN_FLAG == "1" && x.DN_NO == dn);
                return entity;
            }
        }

        /// <summary>
        /// 1.处理机种名前有多个0
        /// </summary>
        /// <param name="SkuName"></param>
        /// <returns></returns>
        public static string SkunoNameHandle(string SkuName)
        {
            if (SkuName.IndexOf("0000") == 0)
                for (int i = 0; i < SkuName.Length; i++)
                {
                    if (SkuName[i].ToString() != "0")
                    {
                        SkuName = SkuName.Substring(i);
                        break;
                    }
                }
            return SkuName;
        }

        /// <summary>
        /// By customerCode get GtRoute => OrderBySEQ.Asc
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns>List C_SHIPPING_ROUTE_DETAIL</returns>
        public List<C_SHIPPING_ROUTE_DETAIL> GetGtRoute(string customerCode)
        {
            List<C_SHIPPING_ROUTE_DETAIL> cShipRouteDetailList = new List<C_SHIPPING_ROUTE_DETAIL>();
            using (var db = OleExec.GetSqlSugarClient(DbStr, false))
            {
                cShipRouteDetailList = db.Queryable<C_SHIPPING_ROUTE_DETAIL, C_SHIP_CUSTOMER>((csrd, cs) =>
                cs.ROUTENAME == csrd.ROUTENAME && cs.CUSTOMERNAME == customerCode).OrderBy((csrd) => csrd.SEQ, SqlSugar.OrderByType.Asc).Select((csrd) => csrd).ToList();
            }
            return cShipRouteDetailList;
        }

        public bool CheckSkunoExists(string Skuno)
        {
            using (var db = OleExec.GetSqlSugarClient(this.DbStr, false))
            {
                return db.Queryable<C_SKU>().Any(t => t.SKUNO == Skuno);
            }
        }
    }
}
