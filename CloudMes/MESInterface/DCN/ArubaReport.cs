using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
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
    public class ArubaReport : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, bustr;
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
                bustr = ConfigGet("BU");

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
                BuildOpoReport();
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

        class BuildOpoFomat
        {
            public string ArubaPONo { get; set; }
            public string Region { get; set; }
            public string TradePOtoSupplier { get; set; }
            public string OrderReceivedDate { get; set; }
            public string ProcessDay { get; set; }
            public string ArubaSKUNo { get; set; }
            public string CMSKUNo { get; set; }
            public string Quantity { get; set; }
            public string WeightTotal { get; set; }
            public string Status { get; set; }
            public string Shipvia { get; set; }
            public string ArubarequestETA { get; set; }
            public string ShippingScheduleDate { get; set; }
            public string CurrentCommitETA { get; set; }
            public string LastCommitETA { get; set; }
            public string L1stConfirmDate { get; set; }
        }

        DataTable BuildOpoFomatDt()
        {
            var dt = new DataTable();
            dt.Columns.Add("ArubaPONo");
            dt.Columns.Add("Region");
            dt.Columns.Add("TradePOtoSupplier");
            dt.Columns.Add("OrderReceivedDate");
            dt.Columns.Add("ProcessDay");
            dt.Columns.Add("ArubaSKUNo");
            dt.Columns.Add("CMSKUNo");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("WeightTotal");
            dt.Columns.Add("Status");
            dt.Columns.Add("Shipvia");
            dt.Columns.Add("ArubarequestETA");
            dt.Columns.Add("ShippingScheduleDate");
            dt.Columns.Add("CurrentCommitETA");
            dt.Columns.Add("LastCommitETA");
            dt.Columns.Add("L1stConfirmDate");
            dt.TableName = "OpoReport";
            return dt;
        }

        void BuildOpoReport()
        {
            System.Threading.Thread.Sleep(2000);
            using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
            {
                try
                {
                    var tartgetdate = mesdb.Queryable<O_ORDER_MAIN, HPE_EDI_ORDER>((m,o) => new object[] { JoinType.Left, m.ITEMID == o.ID })
                    .Where((m, o) => m.CLOSED == MesBool.No.ExtValue() && m.CUSTOMER == Customer.ARUBA.ExtValue() && m.CANCEL == MesBool.No.ExtValue())
                    .Select((m, o) => new { m, o.ID }).ToList();
                    //var res = BuildOpoFomatDt();
                    var res = new List<BuildOpoFomat>();
                    foreach (var item in tartgetdate)
                    {
                        try
                        {
                            var sourcedata = mesdb.Queryable<HPE_EDI_ORDER>().Where(t=>t.ID == item.m.ITEMID).Select(t=> new { t.F_PO_COMMENT, t.F_N1_DA ,t.F_PO,t.F_PO_DATE,t.F_SHIP_MODE,t.F_SHIP_DATE }).ToList().FirstOrDefault();
                            //var dr = res.NewRow();
                            var ArubaPONo = sourcedata.F_PO_COMMENT;
                            var Region = new Func<string>(() =>
                            {
                                var notes = sourcedata.F_N1_DA;
                                var regionlist = mesdb.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "ARUBA_REGION" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList();
                                foreach (var regionconfig in regionlist)
                                {
                                    if (notes.IndexOf(regionconfig.EXTVAL) > 0)
                                        return regionconfig.VALUE;
                                }
                                return "";
                                //throw new Exception($@"Miss ARUBA REGION :{notes}!");
                            })();
                            var TradePOtoSupplier = sourcedata.F_PO;
                            var OrderReceivedDate = sourcedata.F_PO_DATE.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                            var ProcessDay = DateTime.Now.Subtract(Convert.ToDateTime(OrderReceivedDate)).Days.ToString();
                            var ArubaSKUNo = item.m.PID;
                            var CMSKUNo = item.m.PID;
                            var Quantity = item.m.QTY;
                            var e856 = mesdb.Queryable<HPE_EDI_856>().Where(t => t.F_PO_NO == item.m.PONO && t.F_PO_LINE_NO == item.m.POLINE).ToList().FirstOrDefault();
                            var WeightTotal = new Func<string>(() =>
                            {
                                var packagedata = mesdb.Queryable<R_SAP_PACKAGE>().Where(t => t.PN == item.m.PID).ToList().FirstOrDefault();
                                if (packagedata != null)
                                    return (Convert.ToDouble(packagedata.PCS_NT) * Convert.ToDouble(item.m.QTY)).ToString("f3");
                                else
                                    return null;
                            })();
                            var Status = e856 != null ? "Released" : "Backlog";
                            var Shipvia = sourcedata.F_SHIP_MODE;
                            var ArubarequestETA = sourcedata.F_SHIP_DATE?.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());

                            var e855s = mesdb.Queryable<HPE_EDI_855>().Where(t => t.F_PO == item.m.PONO && t.F_LINE == item.m.POLINE).ToList();
                            var first855 = e855s.Count > 0 ? e855s.OrderBy(t => t.EDIT_TIME).ToList().FirstOrDefault() : null;
                            var last855 = e855s.Count > 1 ? e855s.OrderByDescending(t => t.EDIT_TIME).ToList().FirstOrDefault() : null;
                            var lastsec855 = e855s.Count > 1 ? e855s.OrderByDescending(t => t.EDIT_TIME).ToList().Skip(1).FirstOrDefault() : null;
                            string ShippingScheduleDate = string.Empty, CurrentCommitETA = string.Empty, LastCommitETA = string.Empty, L1stConfirmDate = string.Empty;
                            if (e855s.Count == 1)
                            {
                                ShippingScheduleDate = Convert.ToDateTime(first855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                CurrentCommitETA = Convert.ToDateTime(first855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                LastCommitETA = "";
                                L1stConfirmDate = Convert.ToDateTime(first855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                            }
                            else if (e855s.Count > 1)
                            {
                                ShippingScheduleDate = Convert.ToDateTime(last855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                CurrentCommitETA = Convert.ToDateTime(last855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                LastCommitETA = Convert.ToDateTime(lastsec855.F_ACK_EDD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                                L1stConfirmDate = Convert.ToDateTime(first855.F_ACK_ESD).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue());
                            }
                            var c = new BuildOpoFomat() {
                                ArubaPONo = ArubaPONo,
                                Region = Region,
                                TradePOtoSupplier = TradePOtoSupplier,
                                OrderReceivedDate = OrderReceivedDate,
                                ProcessDay = ProcessDay,
                                ArubaSKUNo = ArubaSKUNo,
                                CMSKUNo = CMSKUNo,
                                Quantity = Quantity,
                                WeightTotal = WeightTotal,
                                Status = Status,
                                Shipvia = Shipvia.Replace("AF","Air"),
                                ArubarequestETA = ArubarequestETA,
                                ShippingScheduleDate = ShippingScheduleDate,
                                CurrentCommitETA = CurrentCommitETA,
                                LastCommitETA = LastCommitETA,
                                L1stConfirmDate = L1stConfirmDate
                            };
                            res.Add(c);
                        }
                        catch (Exception e)
                        {
                            //throw e;
                        }
                    }
                    var fullfilename = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Aruba\\";
                    var filename = $@"OpoReport{DateTime.Now.AddDays(-1).ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.csv";
                    if (!System.IO.Directory.Exists(fullfilename))
                        System.IO.Directory.CreateDirectory(fullfilename);
                    if (res.Count > 0)
                    {
                        res = res.Distinct().ToList();
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
}
