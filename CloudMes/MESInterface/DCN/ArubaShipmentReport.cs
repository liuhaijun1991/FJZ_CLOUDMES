using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.ARUBA;
using MESDataObject.Module.DCN;
using MESDataObject.Module.DCN.ARUBA;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESInterface.DCN
{
    public class ArubaShipmentReport : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr;
        #region B2B sftp
        string CONST_SFTPHost = "10.132.48.74";
        string CONST_SFTPPort = "8022";
        string CONST_SFTPLogin = "hpe";
        string CONST_SFTPPassword = "0s02QtFZ";
        string CONST_SFTPPath = "ArubaReports\\PROD";
        #endregion
        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");                
            }
            catch (Exception)
            {
            }
        }
        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                BuildShipmentReport();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }
        private void BuildShipmentReport()
        {
            System.Threading.Thread.Sleep(2000);
            using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
            {
                try
                {
                    var shippedList = mesdb.Queryable<HPE_SHIP_DATA, R_DN_STATUS>((h, d) => h.F_TO_DN == d.DN_NO && h.F_TO_DN_LINE == d.DN_LINE)
                        .Where((h, d) => d.DN_FLAG != "0").Select((h, d) => h).ToList();
                    var res = new List<ArubaShipmentFomat>();
                    foreach (var shippedItem in shippedList)
                    {
                        //var tartgetdate = mesdb.Queryable<O_ORDER_MAIN, HPE_EDI_ORDER>((m, o) => new object[] { JoinType.Left, m.ITEMID == o.ID })
                        //    .Where((m, o) => m.CLOSED == MesBool.No.ExtValue() && m.CUSTOMER == Customer.ARUBA.ExtValue() && m.CANCEL == MesBool.No.ExtValue())
                        //    .Select((m, o) => new { m, o }).ToList();
                        var tartgetdate = mesdb.Queryable<O_ORDER_MAIN, HPE_EDI_ORDER>((m, o) => new object[] { JoinType.Left, m.ITEMID == o.ID })
                            .Where((m, o) => m.PONO==shippedItem.F_PO_NO && m.POLINE==shippedItem.F_PO_LINE_NO)
                            .Select((m, o) => new { m, o }).ToList();
                        foreach (var item in tartgetdate)
                        {
                            try
                            {
                                var ArubaPONo = item.o.F_PO_COMMENT;
                                var Region = new Func<string>(() =>
                                {
                                    var notes = item.o.F_N1_DA;
                                    var regionlist = mesdb.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "ARUBA_REGION" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList();
                                    foreach (var regionconfig in regionlist)
                                    {
                                        if (notes.IndexOf(regionconfig.EXTVAL) > 0)
                                            return regionconfig.VALUE;
                                    }
                                    return "";
                                    //throw new Exception($@"Miss ARUBA REGION :{notes}!");
                                })();
                                var TradePOtoSupplier = item.o.F_PO;
                                var OrderReceivedDate = item.o.F_PO_DATE.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                //var ProcessDay = DateTime.Now.Subtract(Convert.ToDateTime(OrderReceivedDate)).Days.ToString();
                                var ArubaSKUNo = item.m.PID;
                                var CMSKUNo = item.m.PID;
                                //var Quantity = item.m.QTY;                               
                                var e856 = mesdb.Queryable<HPE_EDI_856>().Where(t => t.F_PO_NO == item.m.PONO && t.F_PO_LINE_NO == item.m.POLINE).ToList().FirstOrDefault();
                                //var WeightTotal = new Func<string>(() =>
                                //{
                                //    var packagedata = mesdb.Queryable<R_SAP_PACKAGE>().Where(t => t.PN == item.m.PID).ToList().FirstOrDefault();
                                //    if (packagedata != null)
                                //        return (Convert.ToDouble(packagedata.PCS_NT) * Convert.ToDouble(item.m.QTY)).ToString("f3");
                                //    else
                                //        return null;
                                //})();
                                //var Status = e856 != null ? "Released" : "Backlog";
                                var Shipvia = item.o.F_SHIP_MODE;
                                var ArubarequestETA = item.o.F_SHIP_DATE?.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());

                                var e855s = mesdb.Queryable<HPE_EDI_855>().Where(t => t.F_PO == item.m.PONO && t.F_LINE == item.m.POLINE).ToList();
                                var first855 = e855s.Count > 0 ? e855s.OrderBy(t => t.EDIT_TIME).ToList().FirstOrDefault() : null;
                                var last855 = e855s.Count > 1 ? e855s.OrderByDescending(t => t.EDIT_TIME).ToList().FirstOrDefault() : null;
                                var lastsec855 = e855s.Count > 1 ? e855s.OrderByDescending(t => t.EDIT_TIME).ToList().Skip(1).FirstOrDefault() : null;
                                //string ShippingScheduleDate = string.Empty;
                                //string CurrentCommitETA = string.Empty;
                                //string LastCommitETA = string.Empty;
                                string L1stConfirmDate = string.Empty;
                                if (e855s.Count == 1)
                                {
                                    //ShippingScheduleDate = Convert.ToDateTime(first855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                    //CurrentCommitETA = Convert.ToDateTime(first855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                    //LastCommitETA = "";
                                    L1stConfirmDate = Convert.ToDateTime(first855.EDIT_TIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                }
                                else if (e855s.Count > 1)
                                {
                                    //ShippingScheduleDate = Convert.ToDateTime(last855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                    //CurrentCommitETA = Convert.ToDateTime(last855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                    //LastCommitETA = Convert.ToDateTime(lastsec855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                    L1stConfirmDate = Convert.ToDateTime(first855.EDIT_TIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                }


                                var c = new ArubaShipmentFomat()
                                {
                                    ArubaPONo = ArubaPONo,
                                    Region = Region,
                                    TradePOtoSupplier = TradePOtoSupplier,
                                    ArubaSKUNo = ArubaSKUNo,
                                    CMSKUNo = CMSKUNo,
                                    Shipvia = Shipvia.Replace("AF", "Air"),
                                    Quantity = shippedItem.F_PO_LINE_QTY,
                                    ShippedDate = ((DateTime)shippedItem.F_TO_SHIPDATE).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()),
                                    TrackingNo = shippedItem.F_TO_TRAILERNO,
                                    OrderReceivedDate = OrderReceivedDate,
                                    ArubarequestETA = ArubarequestETA,
                                    L1stConfirmDate = L1stConfirmDate
                                };
                                res.Add(c);
                            }
                            catch (Exception e)
                            {
                                //throw e;
                                MesLog.Info($@"Get PO[{item.o.F_PO_COMMENT}] SKUNO]{item.m.PID}] Fail;Error:{e.Message}");
                            }
                        }
                    }

                    
                    var fullfilename = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Aruba\\";
                    var filename = $@"ArubaShipmentReport{DateTime.Now.AddDays(-1).ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.csv";
                    if (!System.IO.Directory.Exists(fullfilename))
                        System.IO.Directory.CreateDirectory(fullfilename);
                    if (res.Count > 0)
                    {
                        ExcelHelp.ExportCsv(res, $@"{fullfilename}{filename}");
                        #region send to custermor      
                        SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword);
                        sftpHelp.Put($@"{fullfilename}{filename}", $@"{CONST_SFTPPath}\\{filename}");
                    }
                }
                catch (Exception e)
                {
                    mesdb.Insertable(new R_SERVICE_LOG()
                    {
                        ID = MesDbBase.GetNewID<C_PACKING>(mesdb, Customer.ARUBA.ExtValue()),
                        FUNCTIONTYPE = "ArubaOpoReport",
                        CURRENTEDITTIME = DateTime.Now,
                        DATA1 = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()),
                        DATA8 = e.Message.ToString().Length > 250 ? e.Message.ToString() : e.Message.ToString().Substring(0, 250),
                        MAILFLAG = MesBool.No.ExtValue()
                    }).ExecuteCommand();
                }
                #endregion
            }
        }
    }

    public class ArubaShipmentFomat
    {
        public string ArubaPONo { get; set; }
        public string Region { get; set; }
        public string TradePOtoSupplier { get; set; }
        public string ArubaSKUNo { get; set; }
        public string CMSKUNo { get; set; }
        public string Shipvia { get; set; }
        public string Quantity { get; set; }
        public string ShippedDate { get; set; }
        public string TrackingNo { get; set; }
        public string OrderReceivedDate { get; set; }
        public string ArubarequestETA { get; set; }
        public string L1stConfirmDate { get; set; }


        //public string ProcessDay { get; set; }
        //public string WeightTotal { get; set; }
        //public string Status { get; set; }            

        //public string ShippingScheduleDate { get; set; }
        //public string CurrentCommitETA { get; set; }
        //public string LastCommitETA { get; set; }

    }
}
