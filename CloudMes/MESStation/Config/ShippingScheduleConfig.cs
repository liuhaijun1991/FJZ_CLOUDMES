using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.DCN;
using MESStation.Config.DCN;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.Json;
using SqlSugar;
using MESPubLab.SAP_RFC;
using System.Globalization;

namespace MESStation.Config
{
    public class ShippingScheduleConfig
    {
        public static void UPDATE_Broadcom_PO(OleExec SFCDB,string BU,string EMP_NO)
        {
            var CSVHs = SFCDB.ORM.Queryable<BROADCOM_CSV_HEAD>().Where(t => t.STATUS == "0").ToList();
            var keys = BroadcomCustPOLine.CsvMapping.Keys.ToArray();
            var T = typeof(BROADCOM_CSV_DETAIL);
            for (int i = 0; i < CSVHs.Count; i++)
            {
                var detail = SFCDB.ORM.Queryable<BROADCOM_CSV_DETAIL>().Where(t => t.FILENAME == CSVHs[i].FILENAME && t.RECNO >= 20)
                    .OrderBy(t => t.RECNO).ToList();
                //验证首行是否为表头
                BROADCOM_CSV_DETAIL HRow = detail[0];
                if (HRow.RECNO != 20)
                {
                    continue;
                }
                try
                {
                    for (int j = 0; j < keys.Count(); j++)
                    {
                        if (keys[j] == "V29")
                        {
                            continue;
                        }
                        var pname = BroadcomCustPOLine.CsvMapping[keys[j]];
                        var hvalue = T.GetProperty(keys[j]).GetValue(HRow).ToString();
                        if (hvalue != pname)
                        {
                            throw new Exception("CSV Format Err");
                        }
                    }
                }
                catch
                {
                    continue;
                }
                //表头验证完成
                for (int j = 1; j < detail.Count; j++)
                {
                    BroadcomCustPOLine POline = new BroadcomCustPOLine(detail[j]);
                    
                    var C_PO= SFCDB.ORM.Queryable<R_CUST_PO>().Where(t => t.CUST_PO_NO == POline.SALES_ORDER_NUMBER).First();
                    if (C_PO == null)
                    {
                        C_PO = new R_CUST_PO()
                        {
                            ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_CUST_PO"),
                            CUST_PO_NO = POline.SALES_ORDER_NUMBER,
                            BILL_TO_CODE = POline.BILL_TO_COUNTRY,
                            //PO_FILE_TYPE = "MESStation.Config.DCN.BroadcomCustPOLine",
                            //PO_FILE_DESC = "JSON",
                            STATUS = "OPEN",
                            EDIT_DATE = DateTime.Now,
                            EDIT_EMP = EMP_NO
                        };
                        SFCDB.ORM.Insertable(C_PO).ExecuteCommand();
                    }

                    var EXPOLine = JsonSave.GetFromDB<BroadcomCustPOLine>(POline.SALES_ORDER_NUMBER + "." + POline.SALES_ORDER_LINE, "BroadcomCustPOLine", SFCDB);
                    
                    bool isNewLine = false;
                    string jsonid = "";
                    if (EXPOLine != null && double.Parse(EXPOLine.VERSION) < double.Parse(POline.VERSION))
                    {
                        jsonid = JsonSave.SaveToDB(POline, POline.SALES_ORDER_NUMBER + "." + POline.SALES_ORDER_LINE, "BroadcomCustPOLine", EMP_NO, SFCDB, BU, true);
                    }
                    else if(EXPOLine == null)
                    {
                        isNewLine = true;
                        jsonid = JsonSave.SaveToDB(POline, POline.SALES_ORDER_NUMBER + "." + POline.SALES_ORDER_LINE, "BroadcomCustPOLine", EMP_NO, SFCDB, BU, true);
                    }
                    
                   
                    R_CUST_PO_DETAIL PO_DETAIL = new R_CUST_PO_DETAIL()
                    {
                        //ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_CUST_PO_DETAIL"),
                        R_CUST_PO_ID = C_PO.ID,
                        CUST_PO_NO = POline.SALES_ORDER_NUMBER,
                        CUST_SKUNO = POline.PART_NUMBER,
                        DN_QTY = 0,
                        QTY = double.Parse(POline.ORDERED_QUANTITY),
                        PO_FILE_TYPE = "MESStation.Config.DCN.BroadcomCustPOLine",
                        PO_FILE_DESC = "JSON",
                        PO_FILE_ID = jsonid,
                        EDIT_DATE = DateTime.Now,
                        EDIT_EMP = EMP_NO,
                        LINE_NO = POline.SALES_ORDER_LINE,
                        NEED_BY_DATE = DateTime.Parse(DateTime.ParseExact(POline.SCHEDULE_SHIP_DATE, "dd-MMM-yy", CultureInfo.CreateSpecificCulture("en-US")).ToString()),
                        STATUS = "OPEN",
                        SHIPED_QTY = 0
                    };
                    var sku = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.CUST_PARTNO == PO_DETAIL.CUST_SKUNO).First();
                    if (sku != null)
                    {
                        PO_DETAIL.SKUNO = sku.SKUNO;
                    }

                    isNewLine = !SFCDB.ORM.Queryable<R_CUST_PO_DETAIL>().Where(t => t.CUST_PO_NO == POline.SALES_ORDER_NUMBER && t.LINE_NO == POline.SALES_ORDER_LINE).Any();

                    if (isNewLine == true)
                    {
                        PO_DETAIL.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_CUST_PO_DETAIL");
                    }
                    else
                    {
                        var polo =SFCDB.ORM.Queryable<R_CUST_PO_DETAIL>().Where(t => t.CUST_PO_NO == POline.SALES_ORDER_NUMBER && t.LINE_NO == POline.SALES_ORDER_LINE).First();
                        if (polo != null)
                        {
                            PO_DETAIL.ID = polo.ID;
                            PO_DETAIL.DN_QTY = polo.DN_QTY;
                            PO_DETAIL.PO_FILE_ID = polo.PO_FILE_ID;
                            PO_DETAIL.STATUS = polo.STATUS;
                            PO_DETAIL.SHIPED_QTY = polo.SHIPED_QTY;
                            //PO_DETAIL.SKUNO = polo.SKUNO;
                            isNewLine = false;
                        }
                    }
                    if (isNewLine)
                    {
                        SFCDB.ORM.Insertable(PO_DETAIL).ExecuteCommand();
                    }
                    else
                    {
                        SFCDB.ORM.Updateable(PO_DETAIL).Where(t=>t.ID == PO_DETAIL.ID).ExecuteCommand();
                    }

                }
                CSVHs[i].STATUS = "1";
                SFCDB.ORM.Updateable(CSVHs[i]).Where(t => t.FILENAME == CSVHs[i].FILENAME).ExecuteCommand();
            }

        }

        public static List<R_CUST_PO_DETAIL> GetCustPOList(OleExec SFCDB,DateTime from, DateTime To ,string Status)
        {
            var query = SFCDB.ORM.Queryable<R_CUST_PO, R_CUST_PO_DETAIL>((t, d) => new object[] { JoinType.Inner, t.CUST_PO_NO == d.CUST_PO_NO })
                .Where((t,d)=>d.NEED_BY_DATE>from && d.NEED_BY_DATE<=To);
            if (Status != null && Status.Trim() != "")
            {
                query = query.Where((t, d) => t.STATUS == Status);
            }
            var ret = query.OrderBy((t, d) => d.NEED_BY_DATE).Select((t, d) => d).ToList();

            return ret;
        }

        public static List<R_CUST_PO_DETAIL> GetCustPOList(OleExec SFCDB, string PO)
        {
            var query = SFCDB.ORM.Queryable<R_CUST_PO, R_CUST_PO_DETAIL>((t, d) => new object[] { JoinType.Inner, t.CUST_PO_NO == d.CUST_PO_NO })
                .Where((t, d) => d.CUST_PO_NO == PO);
            
            var ret = query.OrderBy((t, d) => d.NEED_BY_DATE).Select((t, d) => d).ToList();

            return ret;
        }

        public static List<R_SO_DETAIL> GetSOListBySkuno(OleExec SFCDB, string SKUNO)
        {
            var query = SFCDB.ORM.Queryable<R_SO, R_SO_DETAIL>((s, d) => new object[] { JoinType.Inner, s.SO_NO == d.SO_NO })
                .Where((s, d) => s.ENABLE_DATE_FROM <= DateTime.Now && s.ENABLE_DATE_TO >= DateTime.Now && d.SKUNO == SKUNO && d.QTY > d.DN_QTY);
            var ret = query.Select((s, d) => d).ToList();

            return ret;
        }

       

        public static void CreateDNBySOLine(string BU ,string SHIPPOINT, R_SO_DETAIL SOD, int QTY, string LOCATION,
            DateTime Date,R_CUST_PO_DETAIL CPD ,OleExec SFCDB,string EMP_NO)
        {
            ZRFC_NSG_SD_0005B RFC = new ZRFC_NSG_SD_0005B(BU);
            RFC.SetValue(SHIPPOINT, SOD.SO_NO, SOD.LINE_SEQ, QTY, LOCATION, Date);
            RFC.CallRFC();
            if (RFC.O_FLAG == "1")
            {
                throw new Exception(RFC.O_MESSAGE);
            }
            SFCDB.ORM.Insertable<R_DN_STATUS>(new R_DN_STATUS()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_DN_STATUS"),
                DN_NO = RFC.O_VBELN,
                DN_LINE = "000010",
                PO_NO = CPD.CUST_PO_NO,
                PO_LINE = CPD.LINE_NO,
                SO_NO = SOD.SO_NO,
                SKUNO = SOD.SKUNO,
                QTY = QTY,
                GTTYPE = "NPI",
                GT_FLAG = "0",
                DN_FLAG = "0",
                DN_PLANT = SHIPPOINT,
                GTEVENT = "10",
                CREATETIME = System.DateTime.Now,
                EDITTIME = System.DateTime.Now
            }).ExecuteCommand();

            R_DN_CUST_PO DP = new R_DN_CUST_PO()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_DN_CUST_PO"),
                CUST_PO_LINE_NO = CPD.LINE_NO,
                CUST_PO_NO = CPD.CUST_PO_NO,
                CUST_SKUNO = SOD.SKUNO,
                DN_LINE_NO = "000010",
                DN_NO = RFC.O_VBELN,
                DN_QTY = QTY,
                DN_SKUNO = SOD.SKUNO,
                EDIT_DATE = DateTime.Now,
                EDIT_EMP = EMP_NO,
                PO_QTY = CPD.QTY
            };
            SFCDB.ORM.Insertable(DP).ExecuteCommand();

            R_DN_SO DS = new R_DN_SO()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_DN_SO"),
                DN_NO = RFC.O_VBELN,
                DN_LINE = "000010",
                DN_QTY = QTY,
                DN_SKUNO = SOD.SKUNO,
                SO_NO = SOD.SO_NO,
                SO_LINE_SEQ = SOD.LINE_SEQ,
                SO_LINE_QTY = SOD.QTY,
                EDIT_DATE = DateTime.Now,
                EDIT_EMP = EMP_NO
            };
            SFCDB.ORM.Insertable(DS).ExecuteCommand();

            SOD.DN_QTY += QTY;
            SFCDB.ORM.Updateable<R_SO_DETAIL>(SOD).Where(t => t.ID == SOD.ID).ExecuteCommand();
            CPD.DN_QTY += QTY;
            SFCDB.ORM.Updateable<R_CUST_PO_DETAIL>(CPD).Where(t => t.ID == CPD.ID).ExecuteCommand();
        }
    }


}
