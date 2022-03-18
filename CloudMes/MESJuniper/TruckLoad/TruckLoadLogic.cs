using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.SNMaker;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.TruckLoad
{
    public class TruckLoadLogic
    {
        SqlSugarClient _db = null;
        public TruckLoadLogic(SqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// Get Open TO List
        /// </summary>
        /// <param name="Trailer"></param>
        /// <returns></returns>
        public List<R_JUNIPER_TRUCKLOAD_TO> GetOpenTOList(string Trailer)
        {
            var TOList = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>()
                .Where(t => t.TRAILER_NUM == Trailer && t.CLOSED == "0")
                .ToList();
            return TOList;
        }

        /// <summary>
        /// Get a list of the trailer match and the TO pending Truck-Load GT 
        /// </summary>
        /// <param name="Trailer"></param>
        /// <returns></returns>
        public List<R_JUNIPER_TRUCKLOAD_TO> GetPendingTruckLoadGTTOList(string Trailer)
        {
            var TOList = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>()
                .Where(t => t.TRAILER_NUM == Trailer && t.CLOSED == "1")
                .ToList();
            return TOList;
        }

        /// <summary>
        ///  Get a list of all TO where the TO pending Truck-Load GT 
        /// </summary>
        /// <returns></returns>
        public List<R_JUNIPER_TRUCKLOAD_TO> GetPendingTruckLoadGTTOList()
        {
            var TOList = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>()
                .Where(t => t.CLOSED == "1")
                .ToList();
            return TOList;
        }

        /// <summary>
        /// Get TO Detail Data 
        /// </summary>
        /// <param name="TO_NO"></param>
        /// <returns></returns>
        public List<R_JUNIPER_TRUCKLOAD_DETAIL> GetTruckLoadDetail(string TO_NO)
        {
            var detailList = _db.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>()
                .Where(t => t.TO_NO == TO_NO)
                .ToList();
            return detailList;
        }

        public List<string> GetPhysicPalletList(string TO_NO)
        {
            var detailList = _db.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>()
                .Where(t => t.TO_NO == TO_NO)
                .Select(t => t.NEW_PACK_NO)
                .ToList();
            return detailList;
        }

        public List<object> GetPackList(string newPalletID)
        {
            string Strsql1 = "";

            Strsql1 = $@"SELECT DISTINCT M.SALESORDERNUMBER SO#,
                            L.SALESORDERLINEITEM SOLINE,
                            K.PONO              PO#,
                            K.POLINE            POLINE,
                            A.SALESORDER        DN#,
                            A.LINEID            DNLINE,
                            A.SKUNO,
                            B.PACK_NO,
                            M.SHIPTOCOUNTRYCODE SHIPTO
                       FROM R_JUNIPER_MFPACKINGLIST A,
                            R_JUNIPER_TRUCKLOAD_DETAIL B,
                            (SELECT F.WORKORDERNO, I.PACK_NO, COUNT(F.ID) QTY
                               FROM R_SN F, R_SN_PACKING G, R_PACKING H, R_PACKING I
                              WHERE F.ID = G.SN_ID
                                AND G.PACK_ID = H.ID
                                AND H.PARENT_PACK_ID = I.ID
                                AND I.PACK_NO IN
                                    (SELECT PACK_NO
                                       FROM R_JUNIPER_TRUCKLOAD_DETAIL
                                      WHERE NEW_PACK_NO = '{newPalletID}')
                              GROUP BY F.WORKORDERNO, I.PACK_NO) J,
                            O_ORDER_MAIN K,
                            O_I137_ITEM L,
                            O_I137_HEAD M
                      WHERE A.INVOICENO = B.TO_NO
                        AND A.PALLETID = B.NEW_PACK_NO
                        AND A.SKUNO = B.SKUNO
                        AND A.SALESORDER = B.DELIVERYNUMBER
                        AND A.WORKORDERNO=J.WORKORDERNO
                        AND A.QUANTITY=J.QTY
                        AND B.PACK_NO=J.PACK_NO
                        AND A.WORKORDERNO = K.PREWO
                        AND K.ITEMID = L.ID
                        AND L.TRANID = M.TRANID
                        AND A.PALLETID = '{newPalletID}'
                        ORDER BY SO#,SOLINE";

            return _db.SqlQueryable<object>(Strsql1).ToList();
        }

        /// <summary>
        /// Generate New TO Number
        /// </summary>
        /// <param name="Trailer"></param>
        /// <param name="EMP_NO"></param>
        /// <param name="BU"></param>
        /// <returns></returns>
        public string GenerateTONumber(string Trailer, string EMP_NO, string BU)
        {
            var emptyTO = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TRAILER_NUM == Trailer && t.QTY == 0 && t.CLOSED == "0").ToList().FirstOrDefault();
            if (emptyTO != null)
            {
                return emptyTO.TO_NO;
            }
            var plant = _db.Queryable<C_CONTROL>()
                .Where(t => t.CONTROL_NAME == "TRUCKLOAD_PLANT")
                .Select(t => t.CONTROL_VALUE)
                .ToList()
                .FirstOrDefault();
            if (plant == null || plant == "")
            {
                plant = "MBGA";
            }
            var tono = T_R_JUNIPER_TRUCKLOAD_TO.GetNextTO("TRUCK_LOADTO_TONO", _db);
            var NewTo = new R_JUNIPER_TRUCKLOAD_TO()
            {
                ID = MESDataObject.MesDbBase.GetNewID<R_JUNIPER_TRUCKLOAD_TO>(_db, BU),
                TO_NO = tono,
                TRAILER_NUM = Trailer,
                PLANT = plant,
                QTY = 0,
                BU = BU,
                CLOSED = "0",
                EDIT_EMP = EMP_NO,
                EDIT_TIME = DateTime.Now
            };
            _db.Insertable(NewTo).ExecuteCommand();
            return tono;
        }

        public void CloseToNumber(string Trailer, string EMP_NO)
        {
            var openToList = GetOpenTOList(Trailer);
            for (int i = 0; i < openToList.Count; i++)
            {
                openToList[i].CLOSED = "1";
                openToList[i].EDIT_TIME = DateTime.Now;
                openToList[i].EDIT_EMP = EMP_NO;
                var res_to = _db.Updateable<R_JUNIPER_TRUCKLOAD_TO>(openToList[i]).ExecuteCommand();
                var res_b2b = _db.Updateable<R_JUNIPER_MFPACKINGLIST>()
                    .SetColumns(t => new R_JUNIPER_MFPACKINGLIST() { STATUS = "N" })
                    .Where(t => t.INVOICENO == openToList[i].TO_NO && t.STATUS == "S")
                    .ExecuteCommand();

                if (res_to == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154548"));
                }
                else if (res_b2b == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154548"));
                }
            }
        }
        public void RemoveToNumber(string TONO)
        {
            var TOObj = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TO_NO == TONO && t.CLOSED == "0").ToList().FirstOrDefault();
            if (TOObj == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { " TO# where TO# is ", TONO }));
            }
            if (TOObj.QTY > 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { "TO# " + TONO + " " }));
            }
            _db.Deleteable(TOObj).ExecuteCommand();
        }

        public string GeneratePhysicPallet(string ShippingType)
        {
            if (ShippingType.ToUpper() == "YES")
            {
                return SNmaker.GetNextSN("PhysicCartonForOutbound", _db);
            }
            else
            {
                return SNmaker.GetNextSN("PhysicPalletForOutbound", _db);
            }
        }
    }
}
