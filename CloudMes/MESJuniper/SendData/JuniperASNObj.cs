using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.ModuleHelp;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using MESDBHelper;
using MESPubLab.MesException;
using static MESDataObject.Common.EnumExtensions;
using System.Data;
using MESPubLab.MESStation.SNMaker;
using MESPubLab.MESStation;
using MESJuniper.OrderManagement;

namespace MESJuniper.SendData
{
    public class JuniperASNObj
    {
        #region Const
        //private OleExecPool _dbExecPool = null;
        //private OleExec SFCDB = null;
        private SqlSugarClient _db = null;
        private string strSql;

        public enum AsnRule
        {
            [EnumName("JUNIPER-PRESHIP-ASN")]
            [EnumValue("JUNIPER-PRESHIP-ASN")]
            PreShip,
            [EnumName("JUNIPER-ACTUAL-ASN")]
            [EnumValue("JUNIPER-ACTUAL-ASN")]
            Actual,
            [EnumName("JUNIPER-PRESHIP-ASN-FJZ")]
            [EnumValue("JUNIPER-PRESHIP-ASN-FJZ")]
            PreShipFJZ
        }

        public class SendPoList
        {
            public string Po { get; set; }
            public string PoLine { get; set; }
        }
        #endregion
        public JuniperASNObj(SqlSugarClient db)
        {
            _db = db;
        }


        public bool CancelPreAsn(string po, string item, string bu, string empno)
        {
            var plant = "";
            int soQty = 0;



            //var nowTime = _db.GetDate();//取數據庫的時間保險點
            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);

            var oOrderMain = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == po && t.POLINE == item).ToList().FirstOrDefault();
            if (oOrderMain == null)
            {
                throw new Exception($@"PO-LINE {po}-{item} not exists(O_ORDER_MAIN)");
            }
            if (oOrderMain.FINALASN != null && oOrderMain.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
            {
                throw new Exception($@"PO-LINE {po}-{item} has send FinalAsn(O_ORDER_MAIN)");
            }
            if (oOrderMain.PREASN == null || oOrderMain.PREASN == ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
            {
                throw new Exception($@"PO-LINE {po}-{item} has not send PreAsn(O_ORDER_MAIN)");
            }

            var preAsn = oOrderMain.PREASN;
            #region 新增preasn沒有ACK回來前不能CANCEL
            if (!_db.Queryable<O_B2B_ACK>().Any(t => t.F_DOC_NO == preAsn))
                throw new Exception($@"PreAsn#:{preAsn} has not receive ACk,Cancellation is not allowed!");
            #endregion


            if (preAsn.Length != 16)//为什麼是16 ？
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn.Length !=16 PreAsn# {preAsn}(O_ORDER_MAIN)");
            }
            if (!preAsn.StartsWith("PRESHIP_"))
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn# {preAsn} not StartsWith 'PRESHIP_' (O_ORDER_MAIN)");
            }
            if (_db.Queryable<R_I139>().Where(t => t.ASNNUMBER == preAsn && t.DELIVERYCODE == "03").Count() > 0)
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn# {preAsn} has been cancel (R_I139)");
            }
            if (_db.Queryable<R_I139>().Where(t => t.ASNNUMBER == preAsn && t.DELIVERYCODE == "01").Count() == 0)
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn# {preAsn} not exists (R_I139)");
            }

            bool firstPlant = true;
            var oOrderMainList = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREASN == preAsn).ToList();
            foreach (var v in oOrderMainList)
            {
                if (v.FINALASN != null && v.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send FinalAsn(O_ORDER_MAIN)");
                }
                if (v.PREASN == null || v.PREASN == ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has not send PreAsn(O_ORDER_MAIN)");
                }
                if (_db.Queryable<O_PO_STATUS>().Where(t => t.POID == v.ID && (t.STATUSID == "12" || t.STATUSID == "28" || t.STATUSID == "29") && t.VALIDFLAG == "1").Count() == 0)
                {
                    throw new Exception($@"PO:{v.PONO} status error(12/28/29), please check!");
                }

                if (firstPlant == true)
                {
                    plant = v.PLANT;
                    firstPlant = false;
                }
                else
                {
                    if (plant != v.PLANT) throw new Exception("Multiple plant in SO, Please check!");
                }
                soQty = soQty + int.Parse(Double.Parse(v.QTY).ToString());
            }

            if (plant != "FVN" && plant != "FJZ")
            {
                throw new Exception($@"Plant error in PO(" + po + "),Plant:{plant}!");
            }

            var tranID = plant + nowTime.ToString("yyyyMMddHHmmssfff");
            if (_db.Queryable<R_I139>().Where(t => t.TRANID == tranID).Count() > 0)
            {
                throw new Exception($@"Tranid {tranID} has exists(R_I139)");
            }

            List<R_I139> i139List = new List<R_I139>();
            var actualsn = _db.Queryable<R_I139>().Where(t => t.ASNNUMBER == preAsn && t.DELIVERYCODE == "01").ToList();
            foreach (var r in actualsn)
            {
                var i139actual = new R_I139();
                i139actual = r;
                i139actual.DELIVERYCODE = "03";
                i139actual.TRANID = tranID; //nowTime.ToString("yyyyMMddHHmmssfff");
                i139actual.CREATETIME = nowTime;
                i139List.Add(i139actual);
            }

            //if ((i139List.Count - i139List.FindAll(t => t.PN.StartsWith("7") == true).Count) != soQty || i139List.Count == 0)
            //{
            //    throw new Exception($@"PO {po} is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
            //}

            var sql = "";
            try
            {
                _db.Ado.BeginTran();

                foreach (var r in i139List)
                {
                    r.ID = MesDbBase.GetNewID<R_I139>(_db, bu);
                    _db.Insertable<R_I139>(r).ExecuteCommand();
                }

                foreach (var v in oOrderMainList)
                {
                    sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{v.ID}' and VALIDFLAG = '1'";
                    _db.Ado.ExecuteCommand(sql);

                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                        POID = v.ID,
                        STATUSID = "30",
                        VALIDFLAG = "1",
                        CREATETIME = nowTime,
                        CREATEBY = empno,
                        EDITTIME = nowTime,
                        EDITBY = empno
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();

                }
                sql = $@"update o_order_main set PREASN = '0', PREASNTIME = sysdate where PREASN = '{preAsn}'";
                _db.Ado.ExecuteCommand(sql);

                _db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                throw new Exception(ex.Message);
            }

            return true;
        }

        public bool BuildPreAsnByCombina(string[] polist, string bu, string weight, object mesapi)
        {
            var plant = "";
            var sql = "";
            DataTable dt = null;
            var upoid = polist[0].ToString();
            var firstorder = _db.Queryable<O_ORDER_MAIN, I137_I, I137_H>((m, ii, ih) => m.ITEMID == ii.ID && ii.TRANID == ih.TRANID).Where((m, ii, ih) => m.UPOID == upoid).Select((m, ii, ih) => new { m.PONO, m.POLINE, ih.COMPLETEDELIVERY }).ToList().FirstOrDefault();
            var oOrderMainList = new List<O_ORDER_MAIN>();
            if (firstorder.COMPLETEDELIVERY == "X")
                oOrderMainList = JuniperASNObj.GetPoList(firstorder.PONO, firstorder.POLINE, _db);
            else
                oOrderMainList = JuniperASNObj.GetPoList(polist, _db);
            if (oOrderMainList.Count <= 0)
            {
                throw new Exception("Get PO error(O_ORDER_MAIN)");
            }

            //PO Plant Check
            int soQty = 0;
            bool firstPlant = true;
            foreach (var r in oOrderMainList)
            {
                if (firstPlant == true)
                {
                    plant = r.PLANT;
                    firstPlant = false;
                }
                else
                {
                    if (plant != r.PLANT) throw new Exception("Multiple plant in SO, Please check!");
                }
                soQty = soQty + int.Parse(Double.Parse(r.QTY).ToString());
            }
            if (plant != "FVN" && plant != "FJZ")
            {
                throw new Exception($@"Plant error ,Plant:{plant}!");
            }
            if (soQty == 0)
            {
                throw new Exception("Workorder Qty error, please check!");
            }

            //var nowTime = _db.GetDate();//取數據庫的時間保險點
            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var tranID = plant + nowTime.ToString("yyyyMMddHHmmssfff");
            if (_db.Queryable<R_I139>().Where(t => t.TRANID == tranID).Count() > 0)
            {
                throw new Exception($@"Tranid {tranID} has exists(R_I139)");
            }
            var usTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(nowTime, TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));//轉為US PST時間或有什麼辦法可以取？


            //取PO已出貨數
            int poShippedQty = 0; //應該都是0，出貨了只能重開PO做
            //poShippedQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SHIP_DETAIL>((a, b, c, d, e)
            //      => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO && c.WORKORDERNO == d.WORKORDERNO && d.SN == e.SN)
            //    .Where((a, b, c, d, e) => b.PONO == po && a.VALID == "1" && d.VALID_FLAG == "1").Select((a, b, c, d, e) => e.SN).Distinct().Count();

            var deliveryCode = "01";//默認為第一次傳ASN “01”
            double poWeight = 0.0; //取重量
            double poNetWeight = 0.0; //取重量NetWeight
            foreach (var v in oOrderMainList)
            {
                double poLineWeight = 0.0;
                double poLineNetWeight = 0.0;
                sql = $@"SELECT a.sn, b.pack_id, c.snid, c.weight
                            FROM r_sn a
                                LEFT JOIN r_sn_packing   b ON a.id = b.sn_id
                                LEFT JOIN r_weight       c ON b.pack_id = c.snid
                            WHERE a.valid_flag = '1' and a.workorderno = '{v.PREWO}'
                                AND c.snid IS NULL";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} SN {dt.Rows[0]["SN"].ToString()} is missing the Weight or/and Weight UOM");
                }
                var tt = _db.Queryable<R_SN, R_SN_PACKING, R_WEIGHT>((d, e, f) => d.ID == e.SN_ID && e.PACK_ID == f.SNID)
                    .Where((d, e, f) => d.VALID_FLAG == "1" && d.WORKORDERNO == v.PREWO)
                    .Select((d, e, f) => new { f.SNID, f.WEIGHT }).Distinct().ToList();
                if (tt.Count > 0)
                {
                    poLineWeight = Math.Round(tt.Sum(t => SqlFunc.ToDouble(t.WEIGHT)), 2);//取2位小數就好 2021-12-22
                }
                if (poLineWeight <= 0)
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the Weight or/and Weight UOM");
                }
                poWeight = poWeight + poLineWeight;

                //墨西哥要求如果是路由為Juniper-OPTIC-PACK的重量NetWeight取配置表裡的-保康、達豪; 越南也要和墨西哥一樣
                //if (plant == "FJZ")
                //{
                //保康 要求所有的NetWeight重量均由C_SKU_DETAIL获取 2021年10月11日
                //if (_db.Queryable<R_WO_BASE, C_ROUTE>((a, b) => a.ROUTE_ID == b.ID).Where((a, b) => b.ROUTE_NAME == "Juniper-OPTIC-PACK" && a.WORKORDERNO == v.PREWO).Select((a, b) => a).Any())
                //{
                //保康要求CTO機種需要配置NET_WEIGHT_CTO 取重量2021年10月28日
                var cSkuType = _db.Queryable<C_SKU, C_SERIES>((CSK, CSE) => CSK.C_SERIES_ID == CSE.ID)
                    .Where((CSK, CSE) => CSK.SKUNO == v.PID && CSK.SKU_TYPE == "CTO" && CSE.SERIES_NAME == "Juniper-Configurable System")
                    .Select((CSK, CSE) => CSK)
                    .ToList().FirstOrDefault();
                if (cSkuType == null)
                {
                    //var cSkuDetail = _db.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT" && t.SKUNO == v.PID).ToList().FirstOrDefault();
                    var cSkuDetail = _db.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && (t.CATEGORY_NAME == "NET_WEIGHT" || t.CATEGORY_NAME == "NET_WEIGHT_CTO") && t.SKUNO == v.PID).ToList().FirstOrDefault();
                    if (cSkuDetail == null)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} , C_SKU_DETAIL not setting NetWeight");
                    }
                    else
                    {
                        poLineNetWeight = Double.Parse(cSkuDetail.VALUE) * Double.Parse(v.QTY);
                    }
                    if (poLineNetWeight <= 0)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the NetWeight");
                    }
                    if (poLineNetWeight > poLineWeight)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} net weight > gross weight");
                    }
                    poNetWeight = poNetWeight + poLineNetWeight;
                }
                else
                {
                    var cSkuDetail = _db.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT_CTO" && t.SKUNO == v.PID).ToList().FirstOrDefault();
                    if (cSkuDetail == null)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} SKU IS CTO, C_SKU_DETAIL NOT SETTING NET_WEIGHT_CTO");
                    }
                    else
                    {
                        poLineNetWeight = poLineWeight - Double.Parse(cSkuDetail.VALUE) * Double.Parse(v.QTY);
                    }
                    if (poLineNetWeight <= 0)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the NetWeight");
                    }
                    if (poLineNetWeight > poLineWeight)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} net weight > gross weight");
                    }
                    poNetWeight = poNetWeight + poLineNetWeight;
                }

                //}
                //else
                //{
                //    poNetWeight = poWeight;
                //}
                //}
                //else
                //{
                //    poNetWeight = poWeight;
                //}
            }

            poWeight = poWeight + Double.Parse(weight);//重量需加上傳進來的棧板重量
            if (poWeight <= 0)
            {
                throw new Exception($@"missing the Weight or/and Weight UOM");
            }

            if (poNetWeight <= 0)
            {
                throw new Exception($@"missing the NetWeight or/and NetWeight UOM");
            }

            int i = 1;
            List<R_I139> i139List = new List<R_I139>();
            foreach (var r in oOrderMainList)
            {
                var IsUseSN = true;
                var strSql = $@"select * from o_agile_attr a where  a.item_number='{r.PID}' and a.actived=1";
                var agailattr = _db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == r.PID && t.ACTIVED == "1").First();
                if (agailattr != null)
                {
                    if (agailattr.SERIALIZATION.ToUpper() == "NO")
                    {
                        IsUseSN = false;
                    }
                }
                var snList = (List<R_I139>)GetSnListData(r.PONO, r.POLINE);

                foreach (var v in snList)
                {
                    var poLineQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE>((a, b, c) => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO)
                .Where((a, b, c) => b.PONO == r.PONO && b.POLINE == r.POLINE && a.WO == b.PREWO).Sum((a, b, c) => SqlFunc.ToInt32(c.WORKORDER_QTY));

                    var poLineShippedQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SHIP_DETAIL>((a, b, c, d, e)
                  => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO && c.WORKORDERNO == d.WORKORDERNO && d.SN == e.SN)
                .Where((a, b, c, d, e) => b.PONO == r.PONO && b.POLINE == r.POLINE && a.VALID == "1" && d.VALID_FLAG == "1").Select((a, b, c, d, e) => e.SN).Distinct().Count();

                    if (v.PN.ToString() == "")
                    {
                        throw new Exception("R_CUSTPN_MAP not exists, please check!");
                    }
                    #region 弃用
                    //var cseries = _db.Queryable<R_SN, C_SKU, C_SERIES>((n, c, s) => n.SKUNO == c.SKUNO && c.C_SERIES_ID == s.ID)
                    //        .Where((n, c, s) => n.SN == v.SERIALID && n.VALID_FLAG == "1" && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((n, c, s) => s)
                    //        .ToList().FirstOrDefault();
                    //if (cseries != null)
                    //{
                    //    if (_db.Queryable<R_I054>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //    {
                    //        throw new Exception($@"Positive i054 ACK of SN {v.SERIALID} is not yet received");
                    //    }
                    //}
                    //else
                    //{
                    //    if (v.F_PLANT == "BTS")
                    //    {
                    //        if (_db.Queryable<R_I054>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //        {
                    //            throw new Exception($@"Positive i054 ACK of SN {v.SERIALID} is not yet received");
                    //        }
                    //    }
                    //    if (v.F_PLANT == "CTO")
                    //    {
                    //        if (_db.Queryable<R_I244>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //        {
                    //            throw new Exception($@"i244 of parent SN {v.SERIALID} is not yet received");
                    //            //if (series.SERIES_NAME != "JNP-ODM-SYS")
                    //            //{
                    //            //    throw new Exception($@"i244 of parent SN {v.SERIALID} is not yet received");
                    //            //}
                    //        }
                    //    }
                    //}
                    #endregion

                    if (IsUseSN)
                    {
                        //非SN管控不传I054；
                        //Check I054 Ack
                        MESJuniper.OrderManagement.JuniperOmBase.JuniperI054AckCheck(v.SERIALID, _db);
                    }

                    MESJuniper.OrderManagement.JuniperOmBase.JuniperI244Check(v.SERIALID, _db);

                    sql = $@"select * from r_i139 a
                                 where a.SERIALID = '{v.SERIALID}'
                                   and a.DELIVERYCODE = '01'
                                   and substr(a.ASNNUMBER, 1, 8) = 'PRESHIP_'
                                   and not exists (select 1
                                          from r_i139 b
                                         where b.SERIALID = a.SERIALID
                                           and b.asnnumber = a.asnnumber
                                           and b.DELIVERYCODE = '03')";
                    dt = _db.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception($@"parent SN {v.SERIALID} has send PreAsn, Please cancel PreAsn");
                    }

                    var i139 = new R_I139();
                    i139 = v;
                    i139.DELIVERYCODE = deliveryCode; //01 或 03
                    i139.ASNCREATIONTIME = nowTime;
                    i139.GROSSWEIGHT = poWeight.ToString(); //重量
                    i139.GROSSCODE = "KG"; //定值
                    i139.NETWEIGHT = poNetWeight.ToString(); //重量
                    i139.NETCODE = "KG"; //定值
                    i139.VOLUMEWEIGHT = "12"; //定值
                    i139.VOLUMECODE = "MTQ"; //定值
                    i139.ARRIVALDATE = usTime.AddHours(24); //PRESHIP =+ 24H
                    i139.ISSUEDATE = usTime.AddHours(24); //PRESHIP =+ 24H
                    i139.FREIGHTINVOICEID = "PRESHIP"; //PRESHIP=“” ACTUAL= i282 DELIVERYNUMBER
                    i139.LINE = string.Format("{0:D6}", i);
                    i139.SHIPPEDQUANTITY = poLineShippedQty.ToString(); //poShippedQty.ToString(); //PO已出貨數
                    i139.QUANTITY = poLineQty.ToString();//poQty.ToString(); //PO出貨數
                    i139.TRANID = tranID; // nowTime.ToString("yyyyMMddHHmmssfff");
                    i139.CREATETIME = nowTime;
                    i139List.Add(i139);
                }
                i++;
            }

            if (i139List.Count != soQty || i139List.Count == 0)
            {
                //throw new Exception($@"PO is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                if (i139List.Count == 1 && int.Parse(i139List[0].QUANTITY) != soQty)
                {
                    throw new Exception($@"PO is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                }
            }

            //Check One SN of each Pallet/Carton 
            CheckPreAsnSn(bu, oOrderMainList, mesapi);

            foreach (var r in oOrderMainList)
            {
                //這段是送沒開工單7開頭的料號也送ASN
                //var oI137Item = _db.Queryable<I137_I>().Where(t => t.PONUMBER == r.PONO && t.MATERIALID == r.PID && SqlFunc.Substring(t.PN, 0, 1) == "7" && t.ACTIONCODE != "02").OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList().FirstOrDefault();
                //var oI137Item = _db.Queryable<I137_I>().Where(t => t.ID == r.ITEMID).ToList().FirstOrDefault();
                //               select d.*
                // from o_order_option a, o_order_main b, o_i137_item c, o_i137_item d
                //where a.mainid = b.id
                //  and b.itemid = c.id
                //  and c.tranid = d.tranid
                //  and d.salesorderlineitem = a.citemid
                //  and b.pono = '4500489318'
                //  and b.poline = '00010'
                var oI137Item = _db.Queryable<O_ORDER_OPTION, O_ORDER_MAIN, O_I137_ITEM, O_I137_ITEM>((a, b, c, d) => a.MAINID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID
                && d.SALESORDERLINEITEM == a.CITEMID).Where((a, b, c, d) => b.PONO == r.PONO && b.POLINE == r.POLINE).Select((a, b, c, d) => d).ToList().FirstOrDefault();
                if (oI137Item != null)
                {
                    //if (oI137Item.LINESCHEDULINGSTATUS != "SC")
                    //{
                    //    throw new Exception($@"PO-Line {oI137Item.PONUMBER}-{oI137Item.ITEM} scheduling or rescheduling is NOT completed");
                    //}
                    //if (oI137Item.SALESORDERHOLD != "NA,NA,NA,NA,NA,NA")
                    //{
                    //    throw new Exception($@"PO-Line {oI137Item.PONUMBER}-{oI137Item.ITEM} is on hold");
                    //}

                    var i139Mc = i139List.Find(t => t.PONUMBER == r.PONO && t.ITEM == r.POLINE);
                    R_I139 i139M = new R_I139();
                    i139M.F_PLANT = oI137Item.F_PLANT;
                    i139M.RECIPIENTID = i139Mc.RECIPIENTID;
                    i139M.DELIVERYCODE = i139Mc.DELIVERYCODE;
                    i139M.ASNNUMBER = i139Mc.ASNNUMBER;
                    i139M.ASNCREATIONTIME = i139Mc.ASNCREATIONTIME;
                    i139M.VENDORID = i139Mc.VENDORID;
                    i139M.SHIPTOID = i139Mc.SHIPTOID;
                    i139M.GROSSWEIGHT = i139Mc.GROSSWEIGHT;
                    i139M.GROSSCODE = i139Mc.GROSSCODE;
                    i139M.NETWEIGHT = i139Mc.NETWEIGHT;
                    i139M.NETCODE = i139Mc.NETCODE;
                    i139M.VOLUMEWEIGHT = i139Mc.VOLUMEWEIGHT;
                    i139M.VOLUMECODE = i139Mc.VOLUMECODE;
                    i139M.ARRIVALDATE = i139Mc.ARRIVALDATE;
                    i139M.ISSUEDATE = i139Mc.ISSUEDATE;
                    i139M.TRACKINGID = i139Mc.TRACKINGID;
                    i139M.WAYBILLID = i139Mc.WAYBILLID;
                    i139M.FREIGHTINVOICEID = i139Mc.FREIGHTINVOICEID;
                    i139M.CLASSIFICATIONCODE = i139Mc.CLASSIFICATIONCODE;
                    i139M.TRANSFERLOCATIONNAME = i139Mc.TRANSFERLOCATIONNAME;
                    i139M.LINE = string.Format("{0:D6}", i);
                    i139M.PONUMBER = oI137Item.PONUMBER;
                    i139M.ITEM = oI137Item.ITEM;
                    i139M.PN = oI137Item.PN;
                    i139M.SPECIALREQUEST = "";
                    i139M.COO = "";
                    i139M.SERIALID = "";
                    i139M.SHIPPEDQUANTITY = "0";
                    i139M.QUANTITY = int.Parse(double.Parse(oI137Item.BASEQUANTITY).ToString()).ToString();
                    i139M.TRANID = i139Mc.TRANID;
                    i139M.CREATETIME = i139Mc.CREATETIME;
                    i139List.Add(i139M);
                    i++;
                }
            }

            var asnNumber = SNmaker.GetNextSN(plant == "FVN" ? AsnRule.PreShip.ExtName() : AsnRule.PreShipFJZ.ExtName(), _db);
            var asnNumberList = _db.Queryable<R_I139>().Where(t => t.ASNNUMBER == asnNumber).ToList();
            if (asnNumberList.Count > 0)
            {
                throw new Exception($@"Pre-Ship ASN # {asnNumber} is duplicated");
            }

            try
            {
                _db.Ado.BeginTran();

                //增加对主料号不管控SN的处理
                foreach (var r in oOrderMainList)
                {
                    //从agile获取有关配置
                    var IsUseSN = true;
                    var strSql = $@"select * from o_agile_attr a where  a.item_number='{r.PID}' and a.actived=1";
                    var agailattr = _db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == r.PID && t.ACTIVED == "1").First();
                    if (agailattr != null)
                    {
                        if (agailattr.SERIALIZATION.ToUpper() == "NO")
                        {
                            IsUseSN = false;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        continue;
                    }
                    //取出该PO的全部数据
                    var l = i139List.FindAll(t => t.PONUMBER == r.PONO && t.ITEM == r.POLINE);
                    //将除第一行外全部移除
                    for (int ii = 1; ii < l.Count; ii++)
                    {
                        i139List.Remove(l[ii]);
                    }
                    //将第一行数据的序列号清空
                    l[0].SERIALID = null;


                }

                foreach (var r in i139List)
                {
                    r.ID = MesDbBase.GetNewID<R_I139>(_db, bu);
                    r.ASNNUMBER = asnNumber;
                    _db.Insertable<R_I139>(r).ExecuteCommand();
                    //_db.Insertable<R_I139_TEMP>(r).ExecuteCommand();
                }

                sql = $@"select asnnumber, serialid, count(1) from r_i139 where asnnumber = '{asnNumber}' and deliverycode = '01' and serialid is not null group by asnnumber,serialid having count(1) > 1";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"asnnumber {asnNumber} has multiple SN");
                }

                foreach (var v in oOrderMainList)
                {
                    sql = $@"update o_po_status set validflag = '0',edittime = sysdate where poid = '{v.ID}' and validflag = '1'";
                    _db.Ado.ExecuteCommand(sql);

                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                        POID = v.ID,
                        STATUSID = "12",
                        VALIDFLAG = "1",
                        CREATETIME = nowTime,
                        EDITTIME = nowTime
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();

                    sql = $@"update o_order_main set PREASN = '{asnNumber}', PREASNTIME = sysdate where id = '{v.ID}'";
                    _db.Ado.ExecuteCommand(sql);
                }
                var strpolist = "";
                polist.ToList().ForEach(t => { strpolist += t.ToString(); });

                R_SN_LOG rsnlog = new R_SN_LOG()
                {
                    ID = MesDbBase.GetNewID<R_SN_LOG>(_db, bu),
                    DATA7 = strpolist,
                    DATA2 = weight,
                    LOGTYPE = "PreAsnWeightLog",
                    FLAG = "1",
                    CREATETIME = nowTime,
                    CREATEBY = ((MesAPIBase)mesapi).LoginUser.EMP_NO
                };
                _db.Insertable(rsnlog).ExecuteCommand();

                _db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                throw new Exception(ex.Message);
            }

            return true;
        }



        public bool BuildPreAsn(string po, string item, string bu, string weight, object mesapi)
        {
            var plant = "";
            var sql = "";
            DataTable dt = null;

            var oOrderMainList = JuniperASNObj.GetPoList(po, item, _db);

            var oOrderMain = oOrderMainList.Find(t => t.POLINE == item);
            //var sku = _db.Queryable<C_SKU>().Where(t => t.SKUNO == oOrderMain.PID).First();
            //var series = _db.Queryable<C_SERIES>().Where(t => t.ID == sku.C_SERIES_ID).First();


            #region 什么時候又要改按SO ？？
            //var poList = _db.Queryable<O_ORDER_MAIN>().Where(a => 1 == 2).ToList();
            //r_i137  SODelCompl=Y表示整PO送ASN,否則按poline送ASN
            //檢查PO是按ITEM出貨還是整PO出貨，整PO出貨整個PO需完工
            //什么時候又要改按SO ？？  被以下卡住的找PE和客人處理  或 CANCEL
            //var deliveryTypeList = _db.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((a, b, c) => a.ITEMID == b.ID && b.TRANID == c.TRANID)
            //    .Where((a, b, c) => a.PONO == po).Select((a, b, c) => c.COMPLETEDELIVERY).Distinct().ToList();
            //if (deliveryTypeList.Count != 1)
            //{
            //    throw new Exception("It has different O_I137_HEAD.completedelivery status in PO(" + po + "), please check!");
            //}
            //var deliveryType = deliveryTypeList.FirstOrDefault().ToString();
            //deliveryType = "X"; //X=YES,NA=NO
            //if (deliveryType == "X") //整PO出貨
            //{
            //    //取PO所有LINE
            //    poList = _db.Queryable<O_ORDER_MAIN>().Where(a => a.PONO == po).ToList();
            //}
            //else if (deliveryType == "NA") //單條PO LINE出貨
            //{
            //    //取PO所以LINE
            //    poList = _db.Queryable<O_ORDER_MAIN>().Where(a => a.PONO == po && a.POLINE == item).ToList();
            //}
            //else
            //{
            //    throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY = {deliveryType},Should be 'X' Or 'NA'");
            //}
            #endregion

            if (oOrderMainList.Count <= 0)
            {
                throw new Exception("Get PO error(O_ORDER_MAIN)");
            }

            //PO Plant Check
            int soQty = 0;
            bool firstPlant = true;
            foreach (var r in oOrderMainList)
            {
                if (firstPlant == true)
                {
                    plant = r.PLANT;
                    firstPlant = false;
                }
                else
                {
                    if (plant != r.PLANT) throw new Exception("Multiple plant in SO, Please check!");
                }
                soQty = soQty + int.Parse(Double.Parse(r.QTY).ToString());
            }
            if (plant != "FVN" && plant != "FJZ")
            {
                throw new Exception($@"Plant error in PO(" + po + "),Plant:{plant}!");
            }
            if (soQty == 0)
            {
                throw new Exception("Workorder Qty error, please check!");
            }

            //var nowTime = _db.GetDate();//取數據庫的時間保險點
            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var tranID = plant + nowTime.ToString("yyyyMMddHHmmssfff");
            if (_db.Queryable<R_I139>().Where(t => t.TRANID == tranID).Count() > 0)
            {
                throw new Exception($@"Tranid {tranID} has exists(R_I139)");
            }
            var usTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(nowTime, TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));//轉為US PST時間或有什麼辦法可以取？


            //取PO已出貨數
            int poShippedQty = 0; //應該都是0，出貨了只能重開PO做
                                  //poShippedQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SHIP_DETAIL>((a, b, c, d, e)
                                  //      => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO && c.WORKORDERNO == d.WORKORDERNO && d.SN == e.SN)
                                  //    .Where((a, b, c, d, e) => b.PONO == po && a.VALID == "1" && d.VALID_FLAG == "1").Select((a, b, c, d, e) => e.SN).Distinct().Count();

            var deliveryCode = "01";//默認為第一次傳ASN “01”
            double poWeight = 0.0; //取重量
            double poNetWeight = 0.0; //取重量NetWeight
            foreach (var v in oOrderMainList)
            {
                double poLineWeight = 0.0;
                double poLineNetWeight = 0.0;
                sql = $@"SELECT a.sn, b.pack_id, c.snid, c.weight
                            FROM r_sn a
                                LEFT JOIN r_sn_packing   b ON a.id = b.sn_id
                                LEFT JOIN r_weight       c ON b.pack_id = c.snid
                            WHERE a.valid_flag = '1' and a.workorderno = '{v.PREWO}'
                                AND c.snid IS NULL";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} SN {dt.Rows[0]["SN"].ToString()} is missing the Weight or/and Weight UOM");
                }
                var tt = _db.Queryable<R_SN, R_SN_PACKING, R_WEIGHT>((d, e, f) => d.ID == e.SN_ID && e.PACK_ID == f.SNID)
                    .Where((d, e, f) => d.VALID_FLAG == "1" && d.WORKORDERNO == v.PREWO)
                    .Select((d, e, f) => new { f.SNID, f.WEIGHT }).Distinct().ToList();
                if (tt.Count > 0)
                {
                    poLineWeight = tt.Sum(t => SqlFunc.ToDouble(t.WEIGHT));
                }
                if (poLineWeight <= 0)
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the Weight or/and Weight UOM");
                }
                poWeight = poWeight + poLineWeight;

                //墨西哥要求如果是路由為Juniper-OPTIC-PACK的重量NetWeight取配置表裡的-保康、達豪; 越南也要和墨西哥一樣
                //if (plant == "FJZ")
                //{
                if (_db.Queryable<R_WO_BASE, C_ROUTE>((a, b) => a.ROUTE_ID == b.ID).Where((a, b) => b.ROUTE_NAME == "Juniper-OPTIC-PACK" && a.WORKORDERNO == v.PREWO).Select((a, b) => a).Any())
                {
                    var cSkuDetail = _db.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT" && t.SKUNO == v.PID).ToList().FirstOrDefault();
                    if (cSkuDetail == null)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} Route is Juniper-OPTIC-PACK, C_SKU_DETAIL not setting NetWeight");
                    }
                    else
                    {
                        poLineNetWeight = Double.Parse(cSkuDetail.VALUE) * Double.Parse(v.QTY);
                    }
                    if (poLineNetWeight <= 0)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the NetWeight");
                    }
                    if (poLineNetWeight > poLineWeight)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} net weight > gross weight");
                    }
                    poNetWeight = poNetWeight + poLineNetWeight;
                }
                else
                {
                    poNetWeight = poWeight;
                }
                //}
                //else
                //{
                //    poNetWeight = poWeight;
                //}
            }

            poWeight = poWeight + Double.Parse(weight);//重量需加上傳進來的棧板重量
            if (poWeight <= 0)
            {
                throw new Exception($@"PO-Line {po}-{item} is missing the Weight or/and Weight UOM");
            }
            if (poNetWeight <= 0)
            {
                throw new Exception($@"missing the NetWeight or/and NetWeight UOM");
            }

            int i = 1;
            List<R_I139> i139List = new List<R_I139>();
            foreach (var r in oOrderMainList)
            {
                var IsUseSN = true;
                var strSql = $@"select * from o_agile_attr a where  a.item_number='{r.PID}' and a.actived=1 AND a.PLANT== {plant}";
                var agailattr = _db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == r.PID && t.ACTIVED == "1" && r.PLANT == plant).First();
                if (agailattr != null)
                {
                    if (agailattr.SERIALIZATION.ToUpper() == "NO")
                    {
                        IsUseSN = false;
                    }
                }
                var snList = (List<R_I139>)GetSnListData(r.PONO, r.POLINE);

                foreach (var v in snList)
                {
                    var poLineQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE>((a, b, c) => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO)
                .Where((a, b, c) => b.PONO == r.PONO && b.POLINE == r.POLINE && a.WO == b.PREWO).Sum((a, b, c) => SqlFunc.ToInt32(c.WORKORDER_QTY));

                    var poLineShippedQty = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SHIP_DETAIL>((a, b, c, d, e)
                  => a.ORIGINALID == b.ID && a.WO == c.WORKORDERNO && c.WORKORDERNO == d.WORKORDERNO && d.SN == e.SN)
                .Where((a, b, c, d, e) => b.PONO == r.PONO && b.POLINE == r.POLINE && a.VALID == "1" && d.VALID_FLAG == "1").Select((a, b, c, d, e) => e.SN).Distinct().Count();

                    if (v.PN.ToString() == "")
                    {
                        throw new Exception("R_CUSTPN_MAP not exists, please check!");
                    }

                    //var cseries = _db.Queryable<R_SN, C_SKU, C_SERIES>((n, c, s) => n.SKUNO == c.SKUNO && c.C_SERIES_ID == s.ID)
                    //        .Where((n, c, s) => n.SN == v.SERIALID && n.VALID_FLAG == "1" && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((n, c, s) => s)
                    //        .ToList().FirstOrDefault();
                    //if (cseries != null)
                    //{
                    //    if (_db.Queryable<R_I054>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //    {
                    //        throw new Exception($@"Positive i054 ACK of SN {v.SERIALID} is not yet received");
                    //    }
                    //}
                    //else
                    //{
                    //    if (v.F_PLANT == "BTS")
                    //    {
                    //        if (_db.Queryable<R_I054>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //        {
                    //            throw new Exception($@"Positive i054 ACK of SN {v.SERIALID} is not yet received");
                    //        }
                    //    }
                    //    if (v.F_PLANT == "CTO")
                    //    {
                    //        if (_db.Queryable<R_I244>().Where(t => t.PARENTSN == v.SERIALID).Count() == 0)
                    //        {
                    //            throw new Exception($@"i244 of parent SN {v.SERIALID} is not yet received");
                    //            //if (series.SERIES_NAME != "JNP-ODM-SYS")
                    //            //{
                    //            //    throw new Exception($@"i244 of parent SN {v.SERIALID} is not yet received");
                    //            //}
                    //        }
                    //    }
                    //}

                    if (IsUseSN)
                    {
                        //非SN管控不传I054；
                        //Check I054 Ack
                        MESJuniper.OrderManagement.JuniperOmBase.JuniperI054AckCheck(v.SERIALID, _db);
                    }
                    JuniperOmBase.JuniperI244Check(v.SERIALID, _db);

                    sql = $@"select * from r_i139 a
                                 where a.SERIALID = '{v.SERIALID}'
                                   and a.DELIVERYCODE = '01'
                                   and substr(a.ASNNUMBER, 1, 8) = 'PRESHIP_'
                                   and not exists (select 1
                                          from r_i139 b
                                         where b.SERIALID = a.SERIALID
                                           and b.asnnumber = a.asnnumber
                                           and b.DELIVERYCODE = '03')";
                    dt = _db.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception($@"parent SN {v.SERIALID} has send PreAsn, Please cancel PreAsn");
                    }

                    var i139 = new R_I139();
                    i139 = v;
                    i139.DELIVERYCODE = deliveryCode; //01 或 03
                    i139.ASNCREATIONTIME = nowTime;
                    i139.GROSSWEIGHT = poWeight.ToString(); //重量
                    i139.GROSSCODE = "KG"; //定值
                    i139.NETWEIGHT = poNetWeight.ToString(); //重量
                    i139.NETCODE = "KG"; //定值
                    i139.VOLUMEWEIGHT = "12"; //定值
                    i139.VOLUMECODE = "MTQ"; //定值
                    i139.ARRIVALDATE = usTime.AddHours(24); //PRESHIP =+ 24H
                    i139.ISSUEDATE = usTime.AddHours(24); //PRESHIP =+ 24H
                    i139.FREIGHTINVOICEID = "PRESHIP"; //PRESHIP=“” ACTUAL= i282 DELIVERYNUMBER
                    i139.LINE = string.Format("{0:D6}", i);
                    i139.SHIPPEDQUANTITY = poLineShippedQty.ToString(); //poShippedQty.ToString(); //PO已出貨數
                    i139.QUANTITY = poLineQty.ToString();//poQty.ToString(); //PO出貨數
                    i139.TRANID = tranID; // nowTime.ToString("yyyyMMddHHmmssfff");
                    i139.CREATETIME = nowTime;
                    i139List.Add(i139);
                }
                i++;

            }


            if (i139List.Count != soQty || i139List.Count == 0)
            {
                //throw new Exception($@"PO {po} is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                if (i139List.Count == 1 && int.Parse(i139List[0].QUANTITY) != soQty)
                {
                    throw new Exception($@"PO {po} is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                }
            }

            //Check One SN of each Pallet/Carton 
            CheckPreAsnSn(bu, oOrderMainList, mesapi);

            foreach (var r in oOrderMainList)
            {
                //這段是送沒開工單7開頭的料號也送ASN
                //var oI137Item = _db.Queryable<I137_I>().Where(t => t.PONUMBER == r.PONO && t.MATERIALID == r.PID && SqlFunc.Substring(t.PN, 0, 1) == "7" && t.ACTIONCODE != "02").OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList().FirstOrDefault();
                //var oI137Item = _db.Queryable<I137_I>().Where(t => t.ID == r.ITEMID).ToList().FirstOrDefault();
                //               select d.*
                // from o_order_option a, o_order_main b, o_i137_item c, o_i137_item d
                //where a.mainid = b.id
                //  and b.itemid = c.id
                //  and c.tranid = d.tranid
                //  and d.salesorderlineitem = a.citemid
                //  and b.pono = '4500489318'
                //  and b.poline = '00010'
                var oI137Item = _db.Queryable<O_ORDER_OPTION, O_ORDER_MAIN, O_I137_ITEM, O_I137_ITEM>((a, b, c, d) => a.MAINID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID
                && d.SALESORDERLINEITEM == a.CITEMID).Where((a, b, c, d) => b.PONO == r.PONO && b.POLINE == r.POLINE).Select((a, b, c, d) => d).ToList().FirstOrDefault();
                if (oI137Item != null)
                {
                    //if (oI137Item.LINESCHEDULINGSTATUS != "SC")
                    //{
                    //    throw new Exception($@"PO-Line {oI137Item.PONUMBER}-{oI137Item.ITEM} scheduling or rescheduling is NOT completed");
                    //}
                    //if (oI137Item.SALESORDERHOLD != "NA,NA,NA,NA,NA,NA")
                    //{
                    //    throw new Exception($@"PO-Line {oI137Item.PONUMBER}-{oI137Item.ITEM} is on hold");
                    //}

                    var i139Mc = i139List.Find(t => t.PONUMBER == r.PONO && t.ITEM == r.POLINE);
                    R_I139 i139M = new R_I139();
                    i139M.F_PLANT = oI137Item.F_PLANT;
                    i139M.RECIPIENTID = i139Mc.RECIPIENTID;
                    i139M.DELIVERYCODE = i139Mc.DELIVERYCODE;
                    i139M.ASNNUMBER = i139Mc.ASNNUMBER;
                    i139M.ASNCREATIONTIME = i139Mc.ASNCREATIONTIME;
                    i139M.VENDORID = i139Mc.VENDORID;
                    i139M.SHIPTOID = i139Mc.SHIPTOID;
                    i139M.GROSSWEIGHT = i139Mc.GROSSWEIGHT;
                    i139M.GROSSCODE = i139Mc.GROSSCODE;
                    i139M.NETWEIGHT = i139Mc.NETWEIGHT;
                    i139M.NETCODE = i139Mc.NETCODE;
                    i139M.VOLUMEWEIGHT = i139Mc.VOLUMEWEIGHT;
                    i139M.VOLUMECODE = i139Mc.VOLUMECODE;
                    i139M.ARRIVALDATE = i139Mc.ARRIVALDATE;
                    i139M.ISSUEDATE = i139Mc.ISSUEDATE;
                    i139M.TRACKINGID = i139Mc.TRACKINGID;
                    i139M.WAYBILLID = i139Mc.WAYBILLID;
                    i139M.FREIGHTINVOICEID = i139Mc.FREIGHTINVOICEID;
                    i139M.CLASSIFICATIONCODE = i139Mc.CLASSIFICATIONCODE;
                    i139M.TRANSFERLOCATIONNAME = i139Mc.TRANSFERLOCATIONNAME;
                    i139M.LINE = string.Format("{0:D6}", i);
                    i139M.PONUMBER = oI137Item.PONUMBER;
                    i139M.ITEM = oI137Item.ITEM;
                    i139M.PN = oI137Item.PN;
                    i139M.SPECIALREQUEST = "";
                    i139M.COO = "";
                    i139M.SERIALID = "";
                    i139M.SHIPPEDQUANTITY = "0";
                    i139M.QUANTITY = int.Parse(double.Parse(oI137Item.BASEQUANTITY).ToString()).ToString();
                    i139M.TRANID = i139Mc.TRANID;
                    i139M.CREATETIME = i139Mc.CREATETIME;
                    i139List.Add(i139M);
                    i++;
                }
            }

            var asnNumber = SNmaker.GetNextSN(plant == "FVN" ? AsnRule.PreShip.ExtName() : AsnRule.PreShipFJZ.ExtName(), _db);
            var asnNumberList = _db.Queryable<R_I139>().Where(t => t.ASNNUMBER == asnNumber).ToList();
            if (asnNumberList.Count > 0)
            {
                throw new Exception($@"Pre-Ship ASN # {asnNumber} is duplicated");
            }

            try
            {
                _db.Ado.BeginTran();

                //增加对主料号不管控SN的处理
                foreach (var r in oOrderMainList)
                {
                    //从agile获取有关配置
                    var IsUseSN = true;
                    var strSql = $@"select * from o_agile_attr a where  a.item_number='{r.PID}' and a.actived=1 and a.PLANT== {plant}";
                    var agailattr = _db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == r.PID && t.ACTIVED == "1" && t.PLANT == r.PLANT).First();
                    if (agailattr != null)
                    {
                        if (agailattr.SERIALIZATION.ToUpper() == "NO")
                        {
                            IsUseSN = false;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        continue;
                    }
                    //取出该PO的全部数据
                    var l = i139List.FindAll(t => t.PONUMBER == r.PONO && t.ITEM == r.POLINE);
                    //将除第一行外全部移除
                    for (int ii = 1; ii < l.Count; ii++)
                    {
                        i139List.Remove(l[ii]);
                    }
                    //将第一行数据的序列号清空
                    l[0].SERIALID = null;


                }

                foreach (var r in i139List)
                {
                    r.ID = MesDbBase.GetNewID<R_I139>(_db, bu);
                    r.ASNNUMBER = asnNumber;
                    _db.Insertable<R_I139>(r).ExecuteCommand();
                    //_db.Insertable<R_I139_TEMP>(r).ExecuteCommand();
                }

                sql = $@"select asnnumber, serialid, count(1) from r_i139 where asnnumber = '{asnNumber}' and deliverycode = '01' and serialid is not null group by asnnumber,serialid having count(1) > 1";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"asnnumber {asnNumber} has multiple SN");
                }

                foreach (var v in oOrderMainList)
                {
                    sql = $@"update o_po_status set validflag = '0',edittime = sysdate where poid = '{v.ID}' and validflag = '1'";
                    _db.Ado.ExecuteCommand(sql);

                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                        POID = v.ID,
                        STATUSID = "12",
                        VALIDFLAG = "1",
                        CREATETIME = nowTime,
                        EDITTIME = nowTime
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();

                    sql = $@"update o_order_main set PREASN = '{asnNumber}', PREASNTIME = sysdate where id = '{v.ID}'";
                    _db.Ado.ExecuteCommand(sql);
                }

                R_SN_LOG rsnlog = new R_SN_LOG()
                {
                    ID = MesDbBase.GetNewID<R_SN_LOG>(_db, bu),
                    SN = po,
                    DATA1 = item,
                    DATA2 = weight,
                    LOGTYPE = "PreAsnWeightLog",
                    FLAG = "1",
                    CREATETIME = nowTime,
                    CREATEBY = ((MesAPIBase)mesapi).LoginUser.EMP_NO
                };
                _db.Insertable<R_SN_LOG>(rsnlog).ExecuteCommand();

                _db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                throw new Exception(ex.Message);
            }

            return true;
        }

        public bool BuildFinalAsn(string po, string item, string bu)
        {
            var plant = "";
            int soQty = 0;

            //var nowTime = _db.GetDate();//取數據庫的時間保險點
            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var usTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(nowTime, TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));//轉為US PST時間或有什麼辦法可以取？

            var oOrderMain = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == po && t.POLINE == item).ToList().FirstOrDefault();
            if (oOrderMain == null)
            {
                throw new Exception($@"PO-LINE {po}-{item} not exists(O_ORDER_MAIN)");
            }
            if (oOrderMain.FINALASN != null && oOrderMain.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
            {
                throw new Exception($@"PO-LINE {po}-{item} has send FinalAsn(O_ORDER_MAIN)");
            }
            if (oOrderMain.PREASN != null && oOrderMain.PREASN == ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
            {
                throw new Exception($@"PO-LINE {po}-{item} not send PreAsn(O_ORDER_MAIN)");
            }
            if (oOrderMain.PREASN == null)
            {
                throw new Exception($@"PO-LINE {po}-{item} not send PreAsn(O_ORDER_MAIN)");
            }

            var preAsn = oOrderMain.PREASN;
            var finalAsn = preAsn.Replace("PRESHIP_", "ACTUAL_");
            if (preAsn.Length != 16)//为什麼是16 ？
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn.Length !=16 PreAsn# {preAsn}(O_ORDER_MAIN)");
            }
            if (!preAsn.StartsWith("PRESHIP_"))
            {
                throw new Exception($@"PO-LINE {po}-{item} PreAsn# {preAsn} not StartsWith 'PRESHIP_' (O_ORDER_MAIN)");
            }
            if (_db.Queryable<R_I139>().Where(t => t.ASNNUMBER == finalAsn).Count() > 0)
            {
                throw new Exception($@"PO-LINE {po}-{item} FinalAsn# {finalAsn} has exists(R_I139)");
            }
            if (_db.Queryable<O_ORDER_MAIN>().Where(t => t.FINALASN == finalAsn).Count() > 0)
            {
                throw new Exception($@"PO-LINE {po}-{item} FinalAsn# {finalAsn} has exists(O_ORDER_MAIN)");
            }

            var deliveryTypeList = _db.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((a, b, c) => a.ITEMID == b.ID && b.TRANID == c.TRANID)
                .Where((a, b, c) => a.PREASN == preAsn).Select((a, b, c) => c.COMPLETEDELIVERY).Distinct().ToList();
            if (deliveryTypeList.Count != 1)
            {
                throw new Exception("It has different O_I137_HEAD.completedelivery status in PreAsn(" + preAsn + "), please check!");
            }
            var deliveryType = deliveryTypeList.FirstOrDefault().ToString();

            bool firstPlant = true;
            var oOrderMainList = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREASN == preAsn).ToList();
            foreach (var v in oOrderMainList)
            {
                if (v.FINALASN != null && v.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send FinalAsn(O_ORDER_MAIN)");
                }
                if (v.PREASN != null && v.PREASN == ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} not send PreAsn(O_ORDER_MAIN)");
                }
                if (v.PREASN == null)
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} not send PreAsn(O_ORDER_MAIN)");
                }
                var holdobj = MESJuniper.OrderManagement.JuniperOmBase.JuniperHoldCheck(v.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.FINALASN, _db);
                if (holdobj.HoldFlag)
                {
                    throw new Exception($@"PO {v.PONO} " + holdobj.HoldReason);
                }

                if (_db.Queryable<O_PO_STATUS>().Where(t => t.POID == v.ID && t.STATUSID == "29" && t.VALIDFLAG == "1").Count() == 0)
                {
                    throw new Exception($@"PO:{v.PONO} status error(29), please check!");
                }


                //PO Status Check
                var poStatusCheck = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((a, b, c, d) => a.ORIGINALID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID)
                    .Where((a, b, c, d) => a.VALID == "1" && b.PONO == v.PONO && b.POLINE == v.POLINE).Select((a, b, c, d) => new { c.ACTIONCODE, d.HEADERSCHEDULINGSTATUS, c.LINESCHEDULINGSTATUS, c.SALESORDERHOLD, c.PODELIVERYDATE }).Distinct().ToList();
                if (poStatusCheck.Count() == 0)
                {
                    throw new Exception("Get PO error, please check!");
                }
                if (poStatusCheck.Find(t => t.ACTIONCODE == "02") != null)
                {
                    throw new Exception($@"PO# {v.PONO} is cancelled already (ACTIONCODE=02)");
                }
                if (deliveryType == "X" && (poStatusCheck.Find(t => t.HEADERSCHEDULINGSTATUS != "C" && t.HEADERSCHEDULINGSTATUS != "CSC") != null))
                {
                    throw new Exception($@"PO# {v.PONO} scheduling or rescheduling is NOT completed (HEADERSCHEDULINGSTATUS not in ('C','CSC'))");
                }
                if (deliveryType == "NA" && (poStatusCheck.Find(t => t.LINESCHEDULINGSTATUS != "SC") != null))
                {
                    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} scheduling or rescheduling is NOT completed (LINESCHEDULINGSTATUS not in ('SC'))");
                }
                //if (poStatusCheck.Find(t => t.SALESORDERHOLD != "NA,NA,NA,NA,NA,NA") != null)
                //{
                //    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} is on hold");
                //}
                //if (poStatusCheck.Find(t => t.PODELIVERYDATE > usTime.AddDays(5)) != null)
                //{
                //    throw new Exception("Ship Date shall not be 5 days before PODeliveryDate, please check! (O_I137_ITEM.PODELIVERYDATE)");
                //}

                if (firstPlant == true)
                {
                    plant = v.PLANT;
                    firstPlant = false;
                }
                else
                {
                    if (plant != v.PLANT) throw new Exception("Multiple plant in SO, Please check!");
                }

                soQty = soQty + int.Parse(Double.Parse(v.QTY).ToString());
            }

            if (plant != "FVN" && plant != "FJZ")
            {
                throw new Exception($@"Plant error in PO(" + po + "),Plant:{plant}!");
            }
            var tranID = plant + nowTime.ToString("yyyyMMddHHmmssfff");
            if (_db.Queryable<R_I139>().Where(t => t.TRANID == tranID).Count() > 0)
            {
                throw new Exception($@"Tranid {tranID} has exists(R_I139)");
            }

            var freightInvoiceId = "";
            var strSql = $@"select * from R_I282 where asnnumber = '{preAsn}' and ERRORCODE is null order by f_lasteditdt desc";
            DataTable dt = _db.Ado.GetDataTable(strSql);
            if (dt.Rows.Count == 0)
            {
                strSql = $@"select * from R_I282 where asnnumber = '{preAsn}' order by f_lasteditdt desc";
                dt = _db.Ado.GetDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    var erroecode = "I282 ERROR CODE:" + dt.Rows[0]["ERRORCODE"].ToString() + ";ERRORDESCRIPTION:" + dt.Rows[0]["ERRORDESCRIPTION"].ToString();
                    throw new Exception(erroecode);
                }
                else
                {
                    throw new Exception("Waitting I282 data!");
                }
            }
            else
            {
                freightInvoiceId = dt.Rows[0]["DELIVERYNUMBER"].ToString();
            }

            if (freightInvoiceId.Length == 0)
            {
                throw new Exception("Waitting I282 data");
            }

            var sql = "";

            var tolist = _db.Queryable<R_I139, R_SHIP_DETAIL, SD_TO_DETAIL, SD_TO_HEAD, O_ORDER_MAIN, I137_I, I137_H>((r, s, d, h, m, ii, ih) => r.SERIALID == s.SN && s.DN_NO == d.VBELN && d.TKNUM == h.TKNUM && r.PONUMBER == m.PONO
               && r.ITEM == m.POLINE && m.ITEMID == ii.ID && ii.TRANID == ih.TRANID).Where((r, s, d, h, m, ii, ih) => r.ASNNUMBER == preAsn && r.DELIVERYCODE == "01")
                .Select((r, s, d, h, m, ii, ih) => new { r.SERIALID, h.TPBEZ, h.EXTI2, ih.INCO1, ih.INCO2 }).ToList();
            List<R_I139> i139List = new List<R_I139>();
            var actualsn = _db.Queryable<R_I139>().Where(t => t.ASNNUMBER == preAsn && t.DELIVERYCODE == "01").ToList();
            foreach (var r in actualsn)
            {
                var i139actual = new R_I139();
                i139actual = r;
                i139actual.ASNNUMBER = finalAsn;
                i139actual.ASNCREATIONTIME = nowTime;
                i139actual.ARRIVALDATE = usTime; //PRESHIP =+ 24H
                i139actual.ISSUEDATE = usTime; //PRESHIP =+ 24H
                i139actual.FREIGHTINVOICEID = freightInvoiceId; //PRESHIP=“” ACTUAL= i282 DELIVERYNUMBER
                i139actual.TRANID = tranID; // nowTime.ToString("yyyyMMddHHmmssfff");
                i139actual.CREATETIME = nowTime;
                var currentto = tolist.FindAll(t => t.SERIALID == r.SERIALID && t.INCO1 == "FCA" && t.INCO2 == "ORIGIN").FirstOrDefault();
                if (currentto != null)
                {
                    i139actual.CARRIERNAME = currentto.EXTI2;
                    i139actual.TRACKINGID = currentto.TPBEZ;

                }
                i139List.Add(i139actual);
            }


            if (i139List.Count == 0)
            {
                throw new Exception($@"PO {po} not send PreAsn");
            }

            if ((i139List.Count - i139List.FindAll(t => t.PN.StartsWith("7") == true).Count) != soQty || i139List.Count == 0)
            {
                //throw new Exception($@"PO {po} is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                if (i139List.Count == 1 && int.Parse(i139List[0].QUANTITY) != soQty)
                {
                    throw new Exception($@"PO {po} is NOT completed for delivery (PO-Lines are NOT consolidated or/and PO-Line is NOT in full quantity)");
                }
            }

            try
            {
                //這個原來是界面點的，現在改為可讓shipout調用，哪個地方調用這裡要記得在外層加事務
                //_db.Ado.BeginTran();

                foreach (var r in i139List)
                {
                    r.ID = MesDbBase.GetNewID<R_I139>(_db, bu);
                    _db.Insertable<R_I139>(r).ExecuteCommand();
                }

                sql = $@"select asnnumber, serialid, count(1) from r_i139 where asnnumber = '{finalAsn}' and deliverycode = '01' and serialid is not null group by asnnumber,serialid having count(1) > 1";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"asnnumber {preAsn} has multiple SN");
                }

                foreach (var v in oOrderMainList)
                {
                    sql = $@"update o_po_status set validflag = '0',edittime = sysdate where poid = '{v.ID}' and validflag = '1'";
                    _db.Ado.ExecuteCommand(sql);
                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                        POID = v.ID,
                        STATUSID = "31",
                        VALIDFLAG = "1",
                        CREATETIME = nowTime,
                        EDITTIME = nowTime
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();

                    sql = $@"update o_order_main set FINALASN = '{finalAsn}', FINALASNTIME = sysdate where id = '{v.ID}'";
                    _db.Ado.ExecuteCommand(sql);
                }
                //_db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                //_db.Ado.RollbackTran();
                throw new Exception(ex.Message);
            }

            return true;
        }

        public object GetSnListData(string po, string item)
        {
            //這裡只管取數據，能進這裡說明前面卡關生效（數量，狀態，等等等等...）



            //取PO、ITEM對應工單的SN
            //var snList = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN>((a, b, c, d) => a.ORIGINALID == b.ID && c.WORKORDERNO == a.WO && d.WORKORDERNO == c.WORKORDERNO)
            //    .Where((a, b, c, d) => a.VALID == "0" && b.PONO == po && b.POLINE == item)
            //    .Select((a, b, c, d) => new { b.POTYPE, b.PONO, b.POLINE, d.SKUNO, d.SN }).Distinct().ToList();

            //var snList = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SN_KP, R_CUSTPN_MAP>((a, b, c, d, e, f) => new JoinQueryInfos(JoinType.Inner, a.ORIGINALID == b.ID,
            //      JoinType.Inner, a.WO == c.WORKORDERNO, JoinType.Inner, c.WORKORDERNO == d.WORKORDERNO, JoinType.Left, d.ID == e.R_SN_ID && d.SN == e.SN && d.SN == e.VALUE && e.VALID_FLAG == 1,
            //      JoinType.Left, e.PARTNO == f.PARTNO))
            //    .Where((a, b) => a.VALID == "1" && b.PONO == po && b.POLINE == item)
            //    .Select((a, b, c, d, e, f) => new { b.POTYPE, b.PONO, b.POLINE, SKUNO = f.CUSTPN, d.SN, COO = e.LOCATION }).Distinct().ToList();

            var custPnList = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE>((a, b, c) => new JoinQueryInfos(JoinType.Inner, a.ORIGINALID == b.ID, JoinType.Inner, a.WO == c.WORKORDERNO))
    .Where((a, b) => a.VALID == "1" && b.PONO == po && b.POLINE == item)
    .Select((a, b, c) => new { c.SKUNO, b.PONO, b.PLANT, b.PREWO }).Distinct().ToList();
            var custPn = "";
            var wo = "";
            if (custPnList.Count() > 0)
            {
                var fplant = custPnList.Find(t => t.PONO == po).PLANT;
                var fpn = custPnList.Find(t => t.PONO == po).SKUNO;
                wo = custPnList.Find(t => t.PONO == po).PREWO;
                var strSql = $@"select * from o_agile_attr where ITEM_NUMBER = '{fpn}' and CUSTOMER_PART_NUMBER is not null and plant = '{fplant}' order by release_date desc";
                var oagile = _db.Ado.GetDataTable(strSql);
                if (oagile.Rows.Count == 0)
                {
                    throw new Exception("Get FNN PN error, please check!");
                }
                custPn = oagile.Rows[0]["CUSTOMER_PART_NUMBER"].ToString();
            }
            else
            {
                throw new Exception("Get FNN PN error, please check!");
            }

            var snList = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, R_WO_BASE, R_SN, R_SN_KP>((a, b, c, d, e) => new JoinQueryInfos(JoinType.Inner, a.ORIGINALID == b.ID,
      JoinType.Inner, a.WO == c.WORKORDERNO, JoinType.Inner, c.WORKORDERNO == d.WORKORDERNO, JoinType.Left, d.ID == e.R_SN_ID && d.SN == e.SN && d.SN == e.VALUE && e.SCANTYPE == "SN" && e.KP_NAME == "AutoKP" && e.VALID_FLAG == 1))
    .Where((a, b, c, d, e) => a.VALID == "1" && b.PONO == po && b.POLINE == item && d.VALID_FLAG == "1")
    .Select((a, b, c, d, e) => new { b.POTYPE, b.PONO, b.POLINE, d.SKUNO, d.SN, COO = e.LOCATION, d.WORKORDERNO }).Distinct().ToList();

            //取PO、ITEM對應的JNP_PLANT,SELLERPARTYID,INCO1,INCO2,SPECIALREQUEST
            var i137List = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD, R_WO_BASE>((a, b, c, d, e)
                => a.ORIGINALID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID && a.WO == e.WORKORDERNO)
                .Where((a, b, c, d, e) => a.VALID == "1" && b.PONO == po && b.POLINE == item)
                .Select((a, b, c, d, e) => new { b.PONO, b.POLINE, c.JNP_PLANT, d.SELLERPARTYID, d.INCO1, d.INCO2, PID = b.CUSTPID, d.ECO_FCO, c.SWPARTNUMBER, c.SWVERSION, c.SWTYPE }).Distinct().ToList();
            //RemoveNullValue(i137List);

            var rSapPoDetail = _db.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == wo).ToList();
            var pid = "";
            if (rSapPoDetail.Count > 0)
            {
                pid = rSapPoDetail.Find(t => t.WO == wo).PIDREV;
            }

            List<R_I139> i139List = new List<R_I139>();
            foreach (var r in snList)
            {
                var JNP_PLANT = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).JNP_PLANT;
                if (JNP_PLANT == null) JNP_PLANT = "";
                var SELLERPARTYID = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).SELLERPARTYID;
                if (SELLERPARTYID == null) SELLERPARTYID = "";
                //var INCO1 = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).INCO1;
                //if (INCO1 == null) INCO1 = "";
                //var INCO2 = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).INCO2;
                //if (INCO2 == null) INCO2 = "";
                //var PID = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).PID;
                //if (PID == null) PID = "";
                var ECO_FCO = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).ECO_FCO;
                if (ECO_FCO == null) ECO_FCO = "";
                var SWPARTNUMBER = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).SWPARTNUMBER;
                if (SWPARTNUMBER == null) SWPARTNUMBER = "";
                var SWVERSION = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).SWVERSION;
                if (SWVERSION == null) SWVERSION = "";
                var SWTYPE = i137List.Find(t => t.PONO == r.PONO && t.POLINE == r.POLINE).SWTYPE;
                if (SWTYPE == null) SWTYPE = "";


                if ((ECO_FCO == "Z09" || ECO_FCO == "Z10") && pid == "")
                {
                    throw new Exception($@"ECO/FCO order - specialrequest field in pre-ASN of PO-Line {r.PONO}-{r.POLINE} cannot be blank");
                }

                if ((SWPARTNUMBER.Length > 0 || SWVERSION.Length > 0 || SWTYPE.Length > 0) && (SWPARTNUMBER == "" || SWVERSION == "" || SWTYPE == ""))
                {
                    throw new Exception($@"SW Customization order - specialrequest field in pre-ASN of PO-Line {r.PONO}-{r.POLINE} cannot be blank");
                }

                var i139 = new R_I139()
                {
                    F_PLANT = r.POTYPE, //POTYPE
                    RECIPIENTID = "PLANT" + JNP_PLANT, //字串“PLANT”+ 137.Jnp_Plant'例：PLANT1160
                    VENDORID = SELLERPARTYID, //表I137欄位'SellerPartyID‘
                    SHIPTOID = JNP_PLANT, //表I137欄位'Jnp_Plant'
                                          //CLASSIFICATIONCODE = INCO1, //表I137欄位'INCO1'
                                          //TRANSFERLOCATIONNAME = INCO2, //表I137欄位'INCO2'
                    PONUMBER = r.PONO,
                    ITEM = r.POLINE,
                    PN = custPn,
                    //PN = r.SKUNO,//r.SKUNO.EndsWith("-RB") == true ? r.SKUNO.Substring(0, r.SKUNO.IndexOf("-RB")) : r.SKUNO,//r.SKUNO,
                    SPECIALREQUEST = (ECO_FCO == "Z09" || ECO_FCO == "Z10") ? pid
              : (SWPARTNUMBER == "" && SWVERSION == "" && SWTYPE == "") ? "" : "CSW", //Special Requirements in i137 PO
                    COO = "", //r.COO,
                    SERIALID = r.SN
                };
                i139List.Add(i139);
            }
            return i139List;
        }

        public void RemoveNullValue(object value)
        {
            var t = value.GetType();
            var ps = t.GetProperties();
            for (int i = 0; i < ps.Count(); i++)
            {
                if (ps[i].GetType() == typeof(string))
                {
                    var v = ps[i].GetValue(value);
                    if (v == null)
                    {
                        ps[i].SetValue(value, "");
                    }
                }
                if (ps[i].GetType() == typeof(int))
                {
                    var v = ps[i].GetValue(value);
                    if (v == null)
                    {
                        ps[i].SetValue(value, 0);
                    }
                }
            }
        }


        public static List<O_ORDER_MAIN> GetPoList(string[] polist, SqlSugarClient _db)
        {
            List<O_ORDER_MAIN> oOrderMain = _db.Queryable<O_ORDER_MAIN>().Where(t => polist.Contains(t.UPOID)).ToList();

            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var usTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(nowTime, TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));//轉為US PST時間或有什麼辦法可以取？

            if (oOrderMain.Count == 0)
            {
                throw new Exception($@"Get PO Error!");
            }

            //PO StatusCheck
            foreach (var v in oOrderMain)
            {
                if (v.PREWO.ToString() == "")
                {
                    throw new Exception($@"PO no completed O_Order_Main.Prewo = Null, please check!");
                }

                //fuck cao gan!
                //if (_db.Queryable<O_ORDER_HOLD>().Where(t => t.ITEMID == v.ITEMID && t.HOLDFLAG == "1").Count() > 0)
                //{
                //    throw new Exception($@"PO has been hold O_ORDER_HOLD.HOLDFLAG = '1', please check!");
                //}
                var holdobj = JuniperOmBase.JuniperHoldCheck(v.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PREASN, _db);
                if (holdobj.HoldFlag)
                {
                    throw new Exception($@"PO:{v.UPOID} has been hold ,holdreason:{holdobj.HoldReason}, please check!");
                }

                if (_db.Queryable<R_ORDER_WO>().Where(t => t.WO == v.PREWO && t.VALID == "1").Count() == 0)
                {
                    throw new Exception($@"WO:{v.PREWO} invalid(R_ORDER_WO), please check!");
                }

                if (_db.Queryable<O_PO_STATUS>().Where(t => t.POID == v.ID && (t.STATUSID == "11" || t.STATUSID == "30") && t.VALIDFLAG == "1").Count() == 0)
                {
                    throw new Exception($@"PO:{v.PONO} status error(11/30), please check!");
                }

                //var oOrderMain = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == v.PONO && t.POLINE == v.POLINE).ToList().FirstOrDefault();
                //if (oOrderMain == null)
                //{
                //    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} not exists(O_ORDER_MAIN)");
                //}

                if (v.FINALASN != null && v.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send FinalAsn(O_ORDER_MAIN)");
                }
                if (v.PREASN != null && v.PREASN != ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send PreAsn(O_ORDER_MAIN)");
                    //throw new Exception($@"Previous Pre-Ship ASN # {v.PREASN} PO# {v.PONO} POLINE {v.POLINE} is not cancelled yet");
                }

                var poStatusCheck = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((a, b, c, d) => a.ORIGINALID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID)
                    .Where((a, b, c, d) => a.VALID == "1" && b.PONO == v.PONO && b.POLINE == v.POLINE).Select((a, b, c, d) => new { c.ACTIONCODE, d.HEADERSCHEDULINGSTATUS, d.COMPLETEDELIVERY, c.LINESCHEDULINGSTATUS, c.SALESORDERHOLD, c.PODELIVERYDATE }).Distinct().ToList();
                if (poStatusCheck.Count() == 0)
                {
                    throw new Exception("Get PO error, please check!");
                }

                var deliveryType = poStatusCheck.FirstOrDefault().COMPLETEDELIVERY;
                if (poStatusCheck.Find(t => t.ACTIONCODE == "02") != null)
                {
                    throw new Exception($@"PO# {v.PONO} (O_I137_ITEM.ACTIONCODE= '02') is cancelled already");
                }
                if (deliveryType == "X" && (poStatusCheck.Find(t => t.HEADERSCHEDULINGSTATUS != "C" && t.HEADERSCHEDULINGSTATUS != "CSC") != null))
                {
                    throw new Exception($@"PO# {v.PONO} scheduling or rescheduling is NOT completed (X O_I137_HEAD.HEADERSCHEDULINGSTATUS not in ('C','CSC'))");
                }
                if (deliveryType == "NA" && (poStatusCheck.Find(t => t.LINESCHEDULINGSTATUS != "SC") != null))
                {
                    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} scheduling or rescheduling is NOT completed (NA O_I137_ITEM.LINESCHEDULINGSTATUS not in ('SC'))");
                }
                //if (poStatusCheck.Find(t => t.SALESORDERHOLD != "NA,NA,NA,NA,NA,NA") != null)
                //{
                //    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} is on hold (O_I137_ITEM.SALESORDERHOLD)");
                //}
                //if (poStatusCheck.Find(t => t.PODELIVERYDATE > usTime.AddDays(5)) != null)
                //{
                //    throw new Exception("Ship Date shall not be 5 days before PODeliveryDate, please check! (O_I137_ITEM.PODELIVERYDATE)");
                //}

                //同一SO下的PONO POLINE 有送PREASN 但未送FINALASN提示要先送完FINALASN才能再送PREASN
                string sql = $@"select distinct c.salesordernumber
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and a.PONO = '{v.PONO}'
                           and a.poline = '{v.POLINE}'";
                DataTable dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count != 1)
                {
                    throw new Exception($@"PO# {v.PONO} may be has cancelled Or multiple salesordernumber");
                }
                var salesOrderNumber = dt.Rows[0]["salesordernumber"].ToString();
                if (salesOrderNumber.Length == 0)
                {
                    throw new Exception($@"SO# {salesOrderNumber} null,Please check");
                }
                //越南的暫時不管控
                var functioncontrol = _db.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "DNBYPASS" && r.CATEGORY == "DNBYPASS" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == "N").First();
                if (functioncontrol == null)
                {
                    sql = $@"select a.*
                      from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                     where a.ITEMID = b.ID
                       and b.TRANID = c.TRANID
                       and b.actioncode <> '02'
                       and c.salesordernumber = '{salesOrderNumber}'
                       and a.preasn <> '0'
                       and a.finalasn = '0' AND a.plant<>'FVN' ";
                    dt = _db.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception($@"PO {dt.Rows[0]["PONO"].ToString()} POLINE {dt.Rows[0]["POLINE"].ToString()} has send PreAsn but not send FinalAsn");
                    }
                }

            }

            return oOrderMain.OrderBy(t => t.PONO).OrderBy(t => t.POLINE).ToList();
        }

        public static List<O_ORDER_MAIN> GetPoList(string po, string item, SqlSugarClient _db)
        {
            List<SendPoList> buildPoList = new List<SendPoList>();
            var deliveryType = "";
            var sql = "";
            DataTable dt = null;

            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var usTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(nowTime, TimeZoneInfo.Local), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));//轉為US PST時間或有什麼辦法可以取？

            //r_i137  SODelCompl=Y表示整PO送ASN,否則按poline送ASN
            //檢查PO是按ITEM出貨還是整PO出貨，整PO出貨整個PO需完工
            //什么時候又要改按SO ？？  被以下卡住的找PE和客人處理  或 CANCEL
            sql = $@"select distinct c.salesordernumber
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and a.PONO = '{po}'
                           and a.poline = '{item}'";
            dt = _db.Ado.GetDataTable(sql);
            if (dt.Rows.Count != 1)
            {
                throw new Exception($@"PO# {po} may be has cancelled Or multiple salesordernumber");
            }
            var salesOrderNumber = dt.Rows[0]["salesordernumber"].ToString();
            if (salesOrderNumber.Length == 0)
            {
                throw new Exception($@"SO# {salesOrderNumber} null,Please check");
            }

            //PO SO 是否匹配，找PE和客人處理
            sql = $@"select distinct a.pono
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and c.salesordernumber = '{salesOrderNumber}'";
            dt = _db.Ado.GetDataTable(sql);
            foreach (DataRow r in dt.Rows)
            {
                sql = $@"select distinct c.salesordernumber
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and a.pono = '{r["pono"].ToString()}'";
                DataTable dt1 = _db.Ado.GetDataTable(sql);
                if (dt1.Rows.Count != 1)
                {
                    throw new Exception($@"PO# {r["pono"].ToString()} has multiple SO Or SO Null,Please check");
                }
                if (salesOrderNumber != dt1.Rows[0]["salesordernumber"].ToString())
                {
                    throw new Exception($@"PO# {r["pono"].ToString()} has multiple SO {dt1.Rows[0]["salesordernumber"].ToString()},Please check");
                }
            }

            //先按SO查
            sql = $@"select distinct c.completedelivery
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and c.salesordernumber = '{salesOrderNumber}'";
            dt = _db.Ado.GetDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                throw new Exception($@"PO# {po} may be has cancelled");
            }
            else if (dt.Rows.Count == 1)
            {
                deliveryType = dt.Rows[0]["completedelivery"].ToString();
                if (deliveryType == "X") //整SO出貨
                {
                    //取SO所有LINE
                    sql = $@"select distinct a.pono,a.poline
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and c.salesordernumber = '{salesOrderNumber}'
                     
                           and not exists (select 1
                                  from o_po_status d
                                 where d.poid = a.id
                                   and d.validflag = '1'
                                   and d.statusid = '14')";
                    dt = _db.Ado.GetDataTable(sql);
                    foreach (DataRow r in dt.Rows)
                    {
                        SendPoList sendPoList = new SendPoList()
                        {
                            Po = r["pono"].ToString(),
                            PoLine = r["poline"].ToString()
                        };
                        buildPoList.Add(sendPoList);
                    }
                }
                else if (deliveryType == "NA") //單條PO LINE出貨
                {
                    SendPoList sendPoList = new SendPoList()
                    {
                        Po = po,
                        PoLine = item
                    };
                    buildPoList.Add(sendPoList);
                }
                else
                {
                    throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY = {deliveryType},Should be 'X' Or 'NA'");
                }
            }
            else
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r["completedelivery"].ToString() != "X" && r["completedelivery"].ToString() != "NA")
                    {
                        throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY = {deliveryType},Should be 'X' Or 'NA'");
                    }
                }

                //同一PO應該只有一種，找PE和客人處理  或 CANCEL
                sql = $@"select pono, count( distinct pono)
                            from (select distinct a.pono, c.completedelivery
                                    from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                                    where a.ITEMID = b.ID
                                    and b.TRANID = c.TRANID
                                    and b.actioncode <> '02'
                                    and c.salesordernumber = '{salesOrderNumber}') aa
                            group by pono
                        having count(distinct pono) > 1";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY has multiple status in po({dt.Rows[0]["pono"].ToString()})");
                }

                sql = $@"select distinct c.completedelivery
                            from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                            where a.ITEMID = b.ID
                            and b.TRANID = c.TRANID
                            and b.actioncode <> '02'
                            and a.pono = '{po}'
                            and a.poline = '{item}'";
                dt = _db.Ado.GetDataTable(sql);

                deliveryType = dt.Rows[0]["completedelivery"].ToString();
                if (deliveryType == "X") //整SO出貨
                {
                    //取SO所有LINE
                    sql = $@"select distinct a.pono,a.poline
                          from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                         where a.ITEMID = b.ID
                           and b.TRANID = c.TRANID
                           and b.actioncode <> '02'
                           and c.completedelivery = 'X'
                           and c.salesordernumber = '{salesOrderNumber}'
                           and not exists (select 1
                                  from o_po_status d
                                 where d.poid = a.id
                                   and d.validflag = '1'
                                   and d.statusid = '14')";
                    dt = _db.Ado.GetDataTable(sql);
                    foreach (DataRow r in dt.Rows)
                    {
                        SendPoList sendPoList = new SendPoList()
                        {
                            Po = r["pono"].ToString(),
                            PoLine = r["poline"].ToString()
                        };
                        buildPoList.Add(sendPoList);
                    }
                }
                else if (deliveryType == "NA") //單條PO LINE出貨
                {
                    SendPoList sendPoList = new SendPoList()
                    {
                        Po = po,
                        PoLine = item
                    };
                    buildPoList.Add(sendPoList);
                }
                else
                {
                    throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY = {deliveryType},Should be 'X' Or 'NA'");
                }
            }

            if (deliveryType != "X" && deliveryType != "NA")
            {
                throw new Exception($@"O_I137_HEAD.COMPLETEDELIVERY = {deliveryType},Should be 'X' Or 'NA'");
            }
            //越南的暫時不管控
            var functioncontrol = _db.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "DNBYPASS" && r.CATEGORY == "DNBYPASS" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == "N").First();
            if (functioncontrol == null)
            {
                sql = $@"select a.*
                      from O_ORDER_MAIN a, O_I137_ITEM b, O_I137_HEAD c
                     where a.ITEMID = b.ID
                       and b.TRANID = c.TRANID
                       and b.actioncode <> '02'
                       and c.salesordernumber = '{salesOrderNumber}'
                       and a.preasn <> '0'
                       and a.finalasn = '0' AND a.plant<>'FVN'";
                dt = _db.Ado.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    throw new Exception($@"PO {dt.Rows[0]["PONO"].ToString()} POLINE {dt.Rows[0]["POLINE"].ToString()} has send PreAsn but not send FinalAsn");
                }
            }

            if (buildPoList.Count == 0)
            {
                throw new Exception($@"Get PO {po} Error");
            }
            List<O_ORDER_MAIN> oOrderMain = new List<O_ORDER_MAIN>();
            foreach (var r in buildPoList)
            {
                oOrderMain.AddRange(_db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == r.Po && t.POLINE == r.PoLine).ToList());
            }

            if (oOrderMain.Count == 0)
            {
                throw new Exception($@"Get PO {po} Error");
            }

            //PO StatusCheck
            foreach (var v in oOrderMain)
            {

                if (v.PREWO.ToString() == "")
                {
                    throw new Exception($@"PO no completed O_Order_Main.Prewo = Null, please check!");
                }

                //fuck cao gan!
                //if (_db.Queryable<O_ORDER_HOLD>().Where(t => t.ITEMID == v.ITEMID && t.HOLDFLAG == "1").Count() > 0)
                //{
                //    throw new Exception($@"PO has been hold O_ORDER_HOLD.HOLDFLAG = '1', please check!");
                //}

                if (JuniperOmBase.JuniperHoldCheck(v.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PREASN, _db).HoldFlag)
                {
                    throw new Exception($@"PO has been hold O_ORDER_HOLD.HOLDFLAG = '1', please check!");
                }

                if (_db.Queryable<R_ORDER_WO>().Where(t => t.WO == v.PREWO && t.VALID == "1").Count() == 0)
                {
                    throw new Exception($@"WO:{v.PREWO} invalid(R_ORDER_WO), please check!");
                }

                if (_db.Queryable<O_PO_STATUS>().Where(t => t.POID == v.ID && (t.STATUSID == "11" || t.STATUSID == "30") && t.VALIDFLAG == "1").Count() == 0)
                {
                    throw new Exception($@"PO:{v.PONO} status error(11/30), please check!");
                }

                //var oOrderMain = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == v.PONO && t.POLINE == v.POLINE).ToList().FirstOrDefault();
                //if (oOrderMain == null)
                //{
                //    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} not exists(O_ORDER_MAIN)");
                //}

                if (v.FINALASN != null && v.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send FinalAsn(O_ORDER_MAIN)");
                }
                if (v.PREASN != null && v.PREASN != ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
                {
                    throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} has send PreAsn(O_ORDER_MAIN)");
                    //throw new Exception($@"Previous Pre-Ship ASN # {v.PREASN} PO# {v.PONO} POLINE {v.POLINE} is not cancelled yet");
                }

                var holdobj = MESJuniper.OrderManagement.JuniperOmBase.JuniperHoldCheck(v.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PREASN, _db);
                if (holdobj.HoldFlag)
                {
                    throw new Exception($@"PO {v.PONO} " + holdobj.HoldReason);
                }

                var poStatusCheck = _db.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((a, b, c, d) => a.ORIGINALID == b.ID && b.ITEMID == c.ID && c.TRANID == d.TRANID)
                    .Where((a, b, c, d) => a.VALID == "1" && b.PONO == v.PONO && b.POLINE == v.POLINE).Select((a, b, c, d) => new { c.ACTIONCODE, d.HEADERSCHEDULINGSTATUS, c.LINESCHEDULINGSTATUS, c.SALESORDERHOLD, c.PODELIVERYDATE }).Distinct().ToList();
                if (poStatusCheck.Count() == 0)
                {
                    throw new Exception("Get PO error, please check!");
                }
                if (poStatusCheck.Find(t => t.ACTIONCODE == "02") != null)
                {
                    throw new Exception($@"PO# {v.PONO} (O_I137_ITEM.ACTIONCODE= '02') is cancelled already");
                }
                if (deliveryType == "X" && (poStatusCheck.Find(t => t.HEADERSCHEDULINGSTATUS != "C" && t.HEADERSCHEDULINGSTATUS != "CSC") != null))
                {
                    throw new Exception($@"PO# {v.PONO} scheduling or rescheduling is NOT completed (X O_I137_HEAD.HEADERSCHEDULINGSTATUS not in ('C','CSC'))");
                }
                if (deliveryType == "NA" && (poStatusCheck.Find(t => t.LINESCHEDULINGSTATUS != "SC") != null))
                {
                    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} scheduling or rescheduling is NOT completed (NA O_I137_ITEM.LINESCHEDULINGSTATUS not in ('SC'))");
                }
                //if (poStatusCheck.Find(t => t.SALESORDERHOLD != "NA,NA,NA,NA,NA,NA") != null)
                //{
                //    throw new Exception($@"PO-Line {v.PONO}-{v.POLINE} is on hold (O_I137_ITEM.SALESORDERHOLD)");
                //}
                //Donald cancel  by 2021-12-20
                //if (poStatusCheck.Find(t => t.PODELIVERYDATE > usTime.AddDays(5)) != null)
                //{
                //    throw new Exception("Ship Date shall not be 5 days before PODeliveryDate, please check! (O_I137_ITEM.PODELIVERYDATE)");
                //}

            }

            return oOrderMain.OrderBy(t => t.PONO).OrderBy(t => t.POLINE).ToList();
        }

        public void CheckPreAsnSn(string bu, List<O_ORDER_MAIN> oOrderMainList, object mesapi)
        {
            R_SN_LOG check_log = null;

            var packlist = _db.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((a, b, c, d) => a.ID == b.SN_ID && b.PACK_ID == c.ID && c.PARENT_PACK_ID == d.ID)
                .Where((a, b, c, d) => a.VALID_FLAG == "1" && c.PACK_TYPE == "CARTON" && d.PACK_TYPE == "PALLET" && a.WORKORDERNO == "")
                .Select((a, b, c, d) => new { PALLET = d.PACK_NO, CARTON = c.PACK_NO, a.SN }).ToList();
            foreach (var r in oOrderMainList)
            {
                var rpacklist = _db.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((a, b, c, d) => a.ID == b.SN_ID && b.PACK_ID == c.ID && c.PARENT_PACK_ID == d.ID)
                .Where((a, b, c, d) => a.VALID_FLAG == "1" && c.PACK_TYPE == "CARTON" && d.PACK_TYPE == "PALLET" && a.WORKORDERNO == r.PREWO)
                .Select((a, b, c, d) => new { PALLET = d.PACK_NO, CARTON = c.PACK_NO, a.SN }).ToList();
                packlist.AddRange(rpacklist);
            }

            if (packlist.Count == 0)
            {
                throw new Exception($@"Can't find Pallet/Carton");
            }
            var API = (MesAPIBase)mesapi;

            //packlist.Select(t => t.PALLET).Distinct();
            //int check_time = packlist.Select(t => t.PALLET).Distinct().Count();            
            //for (var i = 0; i < check_time; i++)
            var carton_list = "";
            var total = packlist.Select(t => t.CARTON).Distinct().Count();
            var current = 0;
            while (packlist.Count > 0)
            {
                UIInputData O = new UIInputData() { };
                O.Timeout = 3000000;
                O.IconType = IconType.Warning;
                O.Type = UIInputType.String;
                O.Tittle = "CheckPreAsnSn";
                O.ErrMessage = "No input";
                O.UIArea = new string[] { "90%", "90%" };
                O.OutInputs.Clear();
                O.Message = "SN";
                O.Name = "SN";
                O.CBMessage = "";
                O.OutInputs.Add(new DisplayOutPut
                {
                    DisplayType = "Text",
                    Name = "Text",
                    Value = $@"{current.ToString()}/{total.ToString()}"
                });
                O.OutInputs.Add(new DisplayOutPut
                {
                    DisplayType = "TextArea",
                    Name = "Already Scan CARTON",
                    Value = carton_list
                });
                while (true)
                {
                    var input_sn = O.GetUiInput((MesAPIBase)mesapi, UIInput.Normal);
                    if (input_sn == null)
                    {
                        O.CBMessage = $@"Please Scan SN";
                    }
                    else
                    {
                        string check_sn = input_sn.ToString().ToUpper().Trim();
                        if (string.IsNullOrEmpty(check_sn))
                        {
                            O.CBMessage = $@"Please Scan SN";
                        }
                        else if (check_sn.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                        }
                        else
                        {
                            //T_C_SKU_DETAIL cSkuDetail = new T_C_SKU_DETAIL(null, DB_TYPE_ENUM.Oracle);
                            //check_sn = cSkuDetail.SNPreprocessor(null, null, null);
                            //只傳SN 整個SO、PO有多條LINE就會存在多個SKUNO，這裡目前只有Juniper用，先這樣吧，以後有多再循環去判定
                            var tryCheckSN = packlist.Find(r => r.SN == check_sn);
                            if (tryCheckSN == null)
                            {
                                if (check_sn.StartsWith("S"))
                                {
                                    check_sn = check_sn.Substring(1, check_sn.Length > 1 ? check_sn.Length - 1 : 0);
                                }
                            }
                            var k = packlist.Find(r => r.SN == check_sn);
                            if (k == null)
                            {
                                O.CBMessage = $@"{check_sn} not exists in CARTON/PALLET";
                            }
                            else
                            {
                                //var palletNo = packlist.FindAll(t => t.CARTON == k.CARTON);
                                packlist.RemoveAll(t => t.CARTON == k.CARTON);
                                carton_list += k.CARTON + ",";
                                current++;
                                check_log = new R_SN_LOG()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOG>(_db, bu),
                                    SN = check_sn,
                                    LOGTYPE = "CheckPreAsnSn",
                                    DATA1 = k.SN,
                                    DATA2 = k.CARTON,
                                    DATA3 = k.PALLET,
                                    FLAG = "1",
                                    CREATETIME = _db.GetDate(),
                                    CREATEBY = API.LoginUser.EMP_NO
                                };
                                _db.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void ShipOutChangePOStatus(string palletNo, string bu)
        {
            var woList = _db.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == palletNo).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();
            foreach (var v in woList)
            {
                var rWoBase = _db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == v).ToList();
                if (woList.Count != 1)
                {
                    throw new Exception($@"R_WO_BASE {v} has multiple data!");
                }
                int woqty = int.Parse(rWoBase[0].WORKORDER_QTY.ToString());
                var rSNList = _db.Queryable<R_SN>().Where(t => t.WORKORDERNO == v && t.VALID_FLAG == "1" && t.CURRENT_STATION == "SHIPOUT").ToList();
                if (woqty == rSNList.Count && woqty > 0)
                {
                    var oOrderMainList = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == v).ToList();
                    if (oOrderMainList.Count != 1)
                    {
                        throw new Exception($@"O_ORDER_MAIN {v} has multiple data!");
                    }
                    var mainID = oOrderMainList[0].ID;

                    //VN ODM 出貨不需要做PreAsn和FianlAsn Asked By PE&PM 2021-11-27
                    var seriesName = _db.Queryable<R_PACKING, C_SKU, C_SERIES>((p, c, s) => p.SKUNO == c.SKUNO && c.C_SERIES_ID == s.ID).Where((p, c, s) => p.PACK_NO == palletNo).Select((p, c, s) => s.SERIES_NAME).ToList().FirstOrDefault();
                    if (!_db.Queryable<O_PO_STATUS>().Where(t => t.POID == mainID && t.VALIDFLAG == "1" && t.STATUSID == "28").Any())
                    {
                        if (bu.Equals("VNJUNIPER"))
                        {
                            //ODM的工單不做PreAsn直接可以掃出貨，因此這裡報錯前排除掉ODM;
                            //add contition:ORDERTYPE!="IDOA" 2022-01-05
                            if (!seriesName.ToUpper().StartsWith("JNP-ODM") && !oOrderMainList[0].ORDERTYPE.Equals("IDOA"))
                            {
                                throw new Exception($@"O_PO_STATUS.STATUSID(!=28) can't scan shipout!");
                            }
                        }
                        else
                        {
                            throw new Exception($@"O_PO_STATUS.STATUSID(!=28) can't scan shipout!");
                        }
                    }
                    string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
                    _db.Ado.ExecuteCommand(sql);

                    //待FinalAsn
                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                        POID = mainID,
                        //STATUSID = "29",
                        STATUSID = (seriesName.ToUpper().StartsWith("JNP-ODM") && bu.Equals("VNJUNIPER")) || (oOrderMainList[0].ORDERTYPE.Equals("IDOA") && bu.Equals("VNJUNIPER")) ? "31" : "29", //ODM的工單不做FinalAsn，出貨完成後狀態直接Finish; //add if ORDERTYPE=IDOA, change po status to finish
                        VALIDFLAG = "1",
                        CREATETIME = _db.GetDate(),
                        EDITTIME = _db.GetDate()
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
                }
            }
        }


        /// <summary>
        /// 調此方法需加事務，務必要加事務
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="bu"></param>
        public void ShipOutSendFinalAsn(string palletNo, string bu)
        {
            var woList = _db.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == palletNo).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();
            foreach (var v in woList)
            {
                var oOrderMain = _db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((a, b) => a.ID == b.POID).Where((a, b) => b.VALIDFLAG == "1" && b.STATUSID == "29" && a.PREWO == v).Select((a, b) => a).ToList();
                if (oOrderMain.Count > 0)
                {
                    try
                    {
                        MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(_db);
                        var res = juniperAsn.BuildFinalAsn(oOrderMain[0].PONO, oOrderMain[0].POLINE, bu);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public void ShipOutChangePOStatusByWo(string wo, string bu)
        {
            var rWoBase = _db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo).ToList();
            int woqty = int.Parse(rWoBase[0].WORKORDER_QTY.ToString());
            var rSNList = _db.Queryable<R_SN>().Where(t => t.WORKORDERNO == wo && t.VALID_FLAG == "1" && t.CURRENT_STATION == "SHIPOUT").ToList();
            if (woqty == rSNList.Count && woqty > 0)
            {
                var oOrderMainList = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == wo).ToList();
                if (oOrderMainList.Count != 1)
                {
                    throw new Exception($@"O_ORDER_MAIN {wo} has multiple data!");
                }
                var mainID = oOrderMainList[0].ID;
                if (!_db.Queryable<O_PO_STATUS>().Where(t => t.POID == mainID && t.VALIDFLAG == "1" && t.STATUSID == "28").Any())
                {
                    throw new Exception($@"O_PO_STATUS.STATUSID(!=28) can't scan shipout!");
                }
                string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
                _db.Ado.ExecuteCommand(sql);

                //待FinalAsn
                O_PO_STATUS oPoStatus = new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, bu),
                    POID = mainID,
                    STATUSID = "29",
                    VALIDFLAG = "1",
                    CREATETIME = _db.GetDate(),
                    EDITTIME = _db.GetDate()
                };
                _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
            }
            else
            {
                throw new Exception($@"WO {wo} not all unit complated!");
            }
        }

        /// <summary>
        /// 調此方法需加事務，務必要加事務
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="bu"></param>
        public void ShipOutSendFinalAsnByWo(string wo, string bu)
        {
            var oOrderMain = _db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((a, b) => a.ID == b.POID)
                .Where((a, b) => b.VALIDFLAG == "1" && b.STATUSID == "29" && a.PREWO == wo)
                .Select((a, b) => a).ToList();
            if (oOrderMain.Count > 0)
            {
                if (oOrderMain[0].ORDERTYPE == "IDOA")
                {
                    return;
                }
                try
                {
                    MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(_db);
                    var res = juniperAsn.BuildFinalAsn(oOrderMain[0].PONO, oOrderMain[0].POLINE, bu);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
