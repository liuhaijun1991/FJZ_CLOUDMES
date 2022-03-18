using MES_DCN.Broadcom;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDBHelper;
using MESStation.Label.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.DCN
{
    public class PackingListLabel
    {

        public BroadcomPackingList GetPackingList(OleExec SFCDB, string DN_NO)
        {
            BroadcomPackingList obj = new BroadcomPackingList();
            DCNBroadcomInvoiceValueGroup dvg = new DCNBroadcomInvoiceValueGroup();
            var JSON_PO = dvg.GetJSON_CUST_PO_DATA(SFCDB, DN_NO);
            obj.DN_NO = DN_NO;
            obj.SKUNO = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == DN_NO).Select(t => t.SKUNO).First();
            obj.SALES_ORDER_NUMBER = dvg.Get_Orderno(SFCDB, JSON_PO);
            obj.Solineno=dvg.Get_Orderno_line(SFCDB, JSON_PO);
            obj.SHIP_DATE = dvg.Get_SHIP_TIME(SFCDB, obj.SALES_ORDER_NUMBER, obj.Solineno);
            obj.CUSTOMER_PO_NUMBER=dvg.CUSTOMER_PO_NUMBER(SFCDB, JSON_PO);
            obj.BILL_TO_COMPANY_NAME=dvg.BILL_TO_COMPANY_NAME(SFCDB, JSON_PO);
            obj.BILL_TO_ADDRESS1=dvg.BILL_TO_ADDRESS1(SFCDB, JSON_PO);
            obj.BILL_TO_ADDRESS2=dvg.BILL_TO_ADDRESS2(SFCDB, JSON_PO);
            obj.BILL_TO_ADDRESS3=dvg.BILL_TO_ADDRESS3(SFCDB, JSON_PO);
            obj.BILL_TO_ADDRESS4=dvg.BILL_TO_ADDRESS4(SFCDB, JSON_PO);
            obj.BILL_TO_CITY=dvg.BILL_TO_CITY(SFCDB, JSON_PO);
            obj.BILL_TO_COMPANY_NAME=dvg.BILL_TO_COMPANY_NAME(SFCDB, JSON_PO);
            obj.BILL_TO_COUNTRY=dvg.BILL_TO_COUNTRY(SFCDB, JSON_PO);
            obj.BILL_TO_POSTAL_CODE=dvg.BILL_TO_POSTAL_CODE(SFCDB, JSON_PO);
            obj.BILL_TO_STATE=dvg.BILL_TO_STATE(SFCDB, JSON_PO);
            obj.SHIPPING_NOTE=dvg.SHIPPING_NOTE(SFCDB, JSON_PO);
            obj.SHIPPING_METHOD=dvg.SHIPPING_METHOD(SFCDB, JSON_PO);
            obj.SHIP_TO_ADDRESS1=dvg.SHIP_TO_ADDRESS1(SFCDB, JSON_PO);
            obj.SHIP_TO_ADDRESS2=dvg.SHIP_TO_ADDRESS2(SFCDB, JSON_PO);
            obj.SHIP_TO_ADDRESS3=dvg.SHIP_TO_ADDRESS3(SFCDB, JSON_PO);
            obj.SHIP_TO_ADDRESS4=dvg.SHIP_TO_ADDRESS4(SFCDB, JSON_PO);
            obj.SHIP_TO_CITY=dvg.SHIP_TO_CITY(SFCDB, JSON_PO);
            obj.SHIP_TO_COMPANY_NAME=dvg.SHIP_TO_COMPANY_NAME(SFCDB, JSON_PO);
            obj.SHIP_TO_COUNTRY=dvg.SHIP_TO_COUNTRY(SFCDB, JSON_PO);
            obj.SHIP_TO_POSTAL_CODE=dvg.SHIP_TO_POSTAL_CODE(SFCDB, JSON_PO);
            obj.SHIP_TO_STATE = dvg.SHIP_TO_STATE(SFCDB, JSON_PO);
            obj.SPECIAL_INSTRUCTION=dvg.SPECIAL_INSTRUCTION(SFCDB, JSON_PO);
            obj.INCO_TERM=dvg.INCO_TERM(SFCDB, JSON_PO);
            obj.FobCode = "";
            obj.TotalQuantityShipped = dvg.GetDN_QTY(SFCDB, DN_NO);
            obj.TotalGrossWeight = dvg.GetTotal_Gross_Weight_sum(SFCDB,DN_NO);
            obj.TotalNetWeight = dvg.GetTotal_Net_Weight_sum(SFCDB, DN_NO);
            obj.Total_Volumetric_Weight_Sum = dvg.GetTotal_Volumetric_Weight_sum(SFCDB, DN_NO, obj.SKUNO);
            obj.TotalPackages = dvg.GetTotalPallet_Qty_sum(SFCDB, DN_NO, obj.SKUNO) + "PLTS/" + dvg.GetTotalBox_qty_sum(SFCDB, DN_NO, obj.SKUNO) + "CTNS";
            obj.PCS_NT = dvg.GetPCS_NT(SFCDB, obj.SKUNO);
            obj.UOM = "EA";
            obj.Country_of_Origin = "VN";
            obj.Itemdesc = dvg.Itemdesc(SFCDB, JSON_PO);
            obj.CUSTOMER_ITEM=dvg.CUSTOMER_ITEM(SFCDB, JSON_PO);

            obj.PalletList = new List<PalletObj>();
            var  Pallet= SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == DN_NO).Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).Distinct().ToList();
            for (int i = 0; i < Pallet.Count; i++)
            {
                PackValueGroup pv = new PackValueGroup();
                obj.PalletList.Add(new PalletObj
                {
                    BoxLPNNumber = "MPO" + Pallet[i],
                    ItemNumber = obj.SKUNO,
                    Packages = dvg.GetPallet_Qty_sum(SFCDB, Pallet[i], obj.SKUNO) + "PLTS/" + dvg.GetBox_qty_sum(SFCDB, Pallet[i], obj.SKUNO) + "CTNS",
                    QuantityShipped = dvg.GetPACK_QTY(SFCDB, Pallet[i]),
                    Line = obj.Solineno,
                    UOM = "EA",
                    BoxWeight = dvg.GetPALLET_GW(SFCDB, Pallet[i], obj.SKUNO),
                    NetWeightUnit = dvg.GetPALLET_NET(SFCDB, Pallet[i], obj.SKUNO),
                    VolumetricWeight = dvg.GetTotal_Volumetric_Weight(SFCDB, DN_NO, Pallet[i], obj.SKUNO),
                    CountryOfOrigin = "VN",
                    SNList = pv.GetPalletSN(SFCDB, Pallet[i])
                });
            }
            //1 3
            //var Pallet1 = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
            //  .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == "3006000978").Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).Distinct().ToList();
            //for (int i = 0; i < Pallet1.Count; i++)
            //{
            //    PackValueGroup pv = new PackValueGroup();
            //    obj.PalletList.Add(new PalletObj
            //    {
            //        BoxLPNNumber = "MPO" + Pallet1[i],
            //        ItemNumber = obj.SKUNO,
            //        Packages = dvg.GetPallet_Qty_sum(SFCDB, Pallet[i], obj.SKUNO, "1") + "PLTS/" + dvg.GetBox_qty_sum(SFCDB, Pallet[i], obj.SKUNO, "1") + "CTNS",
            //        QuantityShipped = dvg.GetPACK_QTY(SFCDB, Pallet[i]),
            //        Line = obj.Solineno,
            //        UOM = "EA",
            //        BoxWeight = dvg.GetPALLET_GW(SFCDB, Pallet[i], obj.SKUNO),
            //        NetWeightUnit = dvg.GetPALLET_NET(SFCDB, Pallet[i], obj.SKUNO),
            //        VolumetricWeight = dvg.GetTotal_Volumetric_Weight(SFCDB, DN_NO, Pallet[i], obj.SKUNO),
            //        CountryOfOrigin = "VN",
            //        SNList = pv.GetPalletSN(SFCDB, Pallet[i])
            //    });
            //}
            //var Pallet2 = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
            // .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == "3006000978").Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).Distinct().ToList();
            //for (int i = 0; i < Pallet2.Count; i++)
            //{
            //    PackValueGroup pv = new PackValueGroup();
            //    obj.PalletList.Add(new PalletObj
            //    {
            //        BoxLPNNumber = "MPO" + Pallet2[i],
            //        ItemNumber = obj.SKUNO,
            //        Packages = dvg.GetPallet_Qty_sum(SFCDB, Pallet[i], obj.SKUNO, "1") + "PLTS/" + dvg.GetBox_qty_sum(SFCDB, Pallet[i], obj.SKUNO, "1") + "CTNS",
            //        QuantityShipped = dvg.GetPACK_QTY(SFCDB, Pallet[i]),
            //        Line = obj.Solineno,
            //        UOM = "EA",
            //        BoxWeight = dvg.GetPALLET_GW(SFCDB, Pallet[i], obj.SKUNO),
            //        NetWeightUnit = dvg.GetPALLET_NET(SFCDB, Pallet[i], obj.SKUNO),
            //        VolumetricWeight = dvg.GetTotal_Volumetric_Weight(SFCDB, DN_NO, Pallet[i], obj.SKUNO),
            //        CountryOfOrigin = "VN",
            //        SNList = pv.GetPalletSN(SFCDB, Pallet2[i])
            //    });
            //}



            return obj;
        }
    }
}
