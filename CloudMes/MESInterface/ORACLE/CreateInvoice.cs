using HWDNNSFCBase;
using MESInterface.Common;
using MESPubLab.SAP_RFC;
using System;
using System.Data;

namespace MESInterface.ORACLE
{
    public class CreateInvoice : taskBase
    {
        public string BU = "";
        public bool IsRuning = false;
        OleExec SFCDB = null;
        OleExec LOGDB = null;
        SAP_RFC_BASE RFC;
        Log log;
        public override void init()
        {
            try
            {
                BU = ConfigGet("BU");
                SFCDB = new OleExec(ConfigGet("DB"), false);
                LOGDB = new OleExec(ConfigGet("DB"), false);
                log = new Log(LOGDB);
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
                CreateInvoiceData();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                IsRuning = false;
                throw ex;
            }
            finally
            {
                SFCDB.FreeMe();
                LOGDB.FreeMe();
            }
        }

        public void CreateInvoiceData()
        {
            log.UpdateStatus("OracleCreateInvoice", "START");
            RFC = new SAP_RFC_BASE(BU);
            RFC.SetRFC_NAME("ZRFC_NSBG_SD015");
            string sql = @"SELECT DN_NO 
                                  FROM   R_DN_LINK rdb 
                                  WHERE  rdb.PO_NO IN (SELECT ponumber 
                                                        FROM   dbo.Oracle_PO_Head)
                                          AND rdb.DN_NO NOT IN (SELECT DN_NO 
                                                             FROM   r_dn_invoice) AND rdb.WORKTIME >'2019-04-01'";
            DataSet dns = SFCDB.RunSelect(sql);

            for (int i = 0; i < dns.Tables[0].Rows.Count; i++)
            {
                string DN = dns.Tables[0].Rows[i]["DN_NO"].ToString();
                if (DN.Trim() == "")
                {
                    continue;
                }
                RFC.SetValue("I_VBELN", DN);
                RFC.CallRFC();
                if (RFC.GetValue("O_FLAG") == "0")
                {
                    SFCDB.BeginTrain();

                    DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
                    string PO = O_HEAD.Rows[0]["BSTKD"].ToString();

                    string FileName = "FTX_Oracle_810_" + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".txt";
                    try
                    {
                        sql = "INSERT INTO R_DN_Invoice \n"
                               + "( \n"
                               + "	DN_NO, \n"
                               + "	Invoice_NO, \n"
                               + "	PO_NO, \n"
                               + "	send_Date \n"
                               + ") \n"
                               + "VALUES \n"
                               + "( \n"
                               + "	'" + DN + "'/* DN_NO	*/, \n"
                               + "	'" + O_HEAD.Rows[0]["VBELN"].ToString() + "'/* Invoice_NO	*/, \n"
                               + "	'" + PO + "'/* PO_NO	*/, \n"
                               + "	GETDATE()/* send_Date	*/ \n"
                               + ")";
                        int p = Int32.Parse(SFCDB.ExecSQL(sql));
                        if (!AutoRunFlag(RFC, SFCDB, DN) || !CheckPrice(RFC, SFCDB, DN))
                        {
                            SFCDB.CommitTrain();
                            continue;
                        }
                        insertOracle_Invoice_Head(RFC, SFCDB, FileName, DN);
                        insertOracle_Invoice_Detail(RFC, SFCDB, FileName, DN);
                        insertOracle_Invoice_Summary(RFC, SFCDB, FileName, DN);
                        //verification
                        verificationData(SFCDB, FileName, DN, PO);
                        log.WriterLog("OracleCreateInvoice", DN, O_HEAD.Rows[0]["VBELN"].ToString(), PO, "", "", "S", "N");
                        SFCDB.CommitTrain();
                    }
                    catch (Exception ee)
                    {
                        SFCDB.RollbackTrain();
                        sql = $@" SELECT * FROM dbo.ServiceLog WHERE data1='{DN}' AND FunctionType='OracleCreateInvoice' AND data6='F' AND data9='Y' ";
                        DataSet dnv = SFCDB.RunSelect(sql);
                        if (dnv.Tables[0].Rows.Count == 0)
                        {
                            log.WriterLog("OracleCreateInvoice", DN, O_HEAD.Rows[0]["VBELN"].ToString(), PO, ee.Message.Replace("'", ""), "", "F", "N");
                        }
                    }
                }
            }
            log.UpdateStatus("OracleCreateInvoice", "END");
        }

        /// <summary>
        /// 1.没有EDI850的PO的DN不会做自动Invoice;
        /// 2.SO没有POLINE信息的不做自动Invoice;
        /// 不自动Run的记录LOG并添加到R_DN_Invoice列表;
        /// </summary>
        bool AutoRunFlag(SAP_RFC_BASE RFC, OleExec sfcdb, string DN)
        {
            DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
            DataTable O_ITEM = RFC.GetTableValue("O_ITEM");
            string PO = O_HEAD.Rows[0]["BSTKD"].ToString();
            try
            {
                string strSql = $@"  SELECT * FROM R_DN_LINK WHERE PO_NO='{PO}' AND DN_NO='{DN}' ";
                DataTable DT = sfcdb.RunSelect(strSql).Tables[0];
                if (DT.Rows.Count > 0)
                {
                    return true;
                }

                log.WriterLog("OracleCreateInvoice", "NoRun", DN, "", "", PO, "F", "N");
                return false;
            }
            catch
            {
                log.WriterLog("OracleCreateInvoice", "NoRun", DN, "", "", PO, "F", "N");
                return false;
            }
        }

        /// <summary>
        /// CheckPrice sapPrice<>OraclePrice=> return false
        /// </summary>
        /// <param name="RFC"></param>
        /// <param name="sfcdb"></param>
        /// <param name="DN"></param>
        /// <returns></returns>
        bool CheckPrice(SAP_RFC_BASE RFC, OleExec sfcdb, string DN)
        {
            decimal sapPrice = 0, OraclePrice = 0;
            string PO, SoNo, Description;
            DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
            DataTable O_ITEM = RFC.GetTableValue("O_ITEM");
            sapPrice = decimal.Parse(O_ITEM.Rows[0]["U_PRICE"].ToString());
            PO = O_HEAD.Rows[0]["BSTKD"].ToString();
            SoNo = O_ITEM.Rows[0]["AUBEL"].ToString();
            Description = O_ITEM.Rows[0]["ARKTX"].ToString();
            try
            {
                string strSql = $@"SELECT * FROM R_ORACLE_MFPRESETWOHEAD A
                                     WHERE EXISTS (SELECT TOP 1 1 FROM R_DN_LINK B
                                            WHERE A.PO = B.orderno
                                               AND A.POLine = B.orderlineno
                                               AND B.DN_NO = '{DN}')";
                DataTable DT = sfcdb.RunSelect(strSql).Tables[0];
                OraclePrice = decimal.Parse(DT.Rows[0]["TotUnitprice"].ToString());
                if (sapPrice == OraclePrice)
                {
                    return true;
                }

                log.WriterLog("OracleCreateInvoice", "PriceErr", DN, PO, $@"OraclePrice:{OraclePrice}", $@"SapPrice:{sapPrice}", DT.Rows[0]["Skuno"].ToString(), SoNo, Description, "N");
                return false;
            }
            catch
            {
                log.WriterLog("OracleCreateInvoice", "PriceErr", DN, PO, $@"OraclePrice:{OraclePrice}", $@"SapPrice:{sapPrice}", "", SoNo, Description, "N");
                return false;
            }
        }

        /// <summary>
        /// verificationData
        /// </summary>
        void verificationData(OleExec sfcdb, string FileName, string dnno, string po)
        {
            int failnum = 0;
            //verification_Oracle_Invoice_Head
            string strOracleInvoiceHead = $@" select * from Oracle_Invoice_Head where FileName='{FileName}' ";
            DataTable OracleInvoiceHeaddt = sfcdb.RunSelect(strOracleInvoiceHead).Tables[0];
            //string verificationHead = @"MessageID,CreateDate,Plant,InvoiceNumber,InvoiceIssueDate,PoNumber,PoIssueDate,CurrencyCode,TermsNetDays,PaymentTermsDesc,SupplierName,SupplierAddressID,SupplierStreet,SupplierCity,SupplierState,SupplierZipCode,SupplierCountryCode,SupplierContactName,SupplierTelephone,SupplierEmail";
            //SupplierState暫不驗證,TermsNetDays暫不驗證;
            string verificationHead = @"MessageID,CreateDate,Plant,InvoiceNumber,InvoiceIssueDate,PoNumber,PoIssueDate,CurrencyCode,PaymentTermsDesc,SupplierName,SupplierAddressID,SupplierStreet,SupplierCity,SupplierZipCode,SupplierCountryCode,SupplierContactName,SupplierTelephone,SupplierEmail";
            string[] headList = verificationHead.Split(',');
            foreach (var item in headList)
            {
                failnum = verification(OracleInvoiceHeaddt, item, "Oracle_Invoice_Head", dnno, po) ? failnum : failnum + 1;
            }

            //verification_Oracle_Invoice_Detail
            string strOracleInvoiceDetail = $@" select * from Oracle_Invoice_Detail where FileName='{FileName}' ";
            DataTable OracleInvoiceDetaildt = sfcdb.RunSelect(strOracleInvoiceDetail).Tables[0];
            string verificationDetail = "InvoiceNumber,InvoiceLineNumber,InvoiceLineQuantity,UnitMeasurementCode,UnitPrice,BuyerPartNumber,PartNumberDescription,TotalTaxAmount,TaxPercentage,PONumber,POLineNumber,ScheduleNumber";
            string[] detailList = verificationDetail.Split(',');
            foreach (var item in detailList)
            {
                failnum = verification(OracleInvoiceDetaildt, item, "Oracle_Invoice_Detail", dnno, po) ? failnum : failnum + 1;
            }

            //verification_Oracle_Invoice_Head
            string strOracleInvoiceSummary = $@" select * from Oracle_Invoice_Summary where FileName='{FileName}' ";
            DataTable OracleInvoiceSummarydt = sfcdb.RunSelect(strOracleInvoiceSummary).Tables[0];
            string verificationSummary = "InvoiceNumber,AmountBeforTermsDiscount,NumberOfLineItems";
            string[] summaryList = verificationSummary.Split(',');
            foreach (var item in summaryList)
            {
                failnum = verification(OracleInvoiceSummarydt, item, "Oracle_Invoice_Summary", dnno, po) ? failnum : failnum + 1;
            }

            if (failnum > 0)
            {
                throw new Exception("verification fail!");
            }
        }

        bool verification(DataTable dt, string coumtuns, string type, string dnno, string po)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][coumtuns] == null || dt.Rows[i][coumtuns].ToString().Trim() == "")
                {
                    log.WriterLog("OracleCreateInvoice", "verification", type, coumtuns, dnno, po, "F", "", "", "N");
                    return false;
                }
            }
            return true;
        }


        int ZeroPost(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '0')
                {
                    return i;
                }
            }
            return 0;
        }

        void insertOracle_Invoice_Head(SAP_RFC_BASE RFC, OleExec sfcdb, string FileName, string DN)
        {
            DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
            string PO = O_HEAD.Rows[0]["BSTKD"].ToString();
            string sql = "SELECT * FROM dbo.Oracle_PO_Head WHERE PoNumber='" + PO + "'";
            DataSet res850 = sfcdb.RunSelect(sql);
            DataTable dt850 = res850.Tables[0];
            DataRow dr850 = dt850.Rows[0];
            sql = "SELECT ISNULL(MAX(MESSAGEID),'100000001') FROM dbo.Oracle_Invoice_Head";
            int msgId = Int32.Parse(sfcdb.RunSelectOneValue(sql).ToString()) + 1;
            DataRow dr = res850.Tables[0].Rows[0];

            string SupplierContactName = dr850["SupplierContactName"].ToString() == "" ? "Lincoln Leung" : dr850["SupplierContactName"].ToString();
            string SupplierTelephone = dr850["SupplierTelephone"].ToString() == "" ? "(281)378-2775" : dr850["SupplierTelephone"].ToString();
            string SupplierEmail = dr850["SupplierEmail"].ToString() == "" ? "Lincoln.Leung@foxconn.com" : dr850["SupplierEmail"].ToString();
            sql = "INSERT INTO Oracle_Invoice_Head \n"
           + "( \n"
           + "	[FileName], \n"
           + "	MessageID, \n"
           + "	CreateDate, \n"
           + "	Plant, \n"
           + "	InvoiceNumber, \n"
           + "	InvoiceIssueDate, \n"
           + "	InvoiceIssueTime, \n"
           + "	InvoiceIssueTimeZone, \n"
           + "	PoNumber, \n"
           + "	PoIssueDate, \n"
           + "	CurrencyCode, \n"
           + "	DiscountPercentage, \n"
           + "	DiscountDaysDue, \n"
           + "	TermsNetDueDays, \n"
           + "	TermsNetDays, \n"
           + "	PaymentTermsDesc, \n"
           + "	BillToName, \n"
           + "	SoldToName, \n"
           + "	SupplierName, \n"
           + "	ShipToName, \n"
           + "	BillToAddressID, \n"
           + "	SoldToAddressID, \n"
           + "	SupplierAddressID, \n"
           + "	ShipToAddressID, \n"
           + "	BillToStreet, \n"
           + "	SoldToStreet, \n"
           + "	SupplierStreet, \n"
           + "	ShipToStreet, \n"
           + "	BillToCity, \n"
           + "	SoldToCity, \n"
           + "	SupplierCity, \n"
           + "	ShipToCity, \n"
           + "	BilltoState, \n"
           + "	SoldtoState, \n"
           + "	SupplierState, \n"
           + "	ShipToState, \n"
           + "	BillToZipCode, \n"
           + "	SoldToZipCode, \n"
           + "	SupplierZipCode, \n"
           + "	ShipToZipCode, \n"
           + "	BillToCountryCode, \n"
           + "	SoldToCountryCode, \n"
           + "	SupplierCountryCode, \n"
           + "	ShipToCountryCode, \n"
           + "	SupplierContactName, \n"
           + "	SupplierTelephone, \n"
           + "	SupplierEmail, \n"
           + "	sendFlag \n"
           + ") \n"
           + "VALUES \n"
           + "( \n"
           + "	'" + FileName + "', \n"
           + "	'" + msgId.ToString() + "', \n"
           + "	'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', \n"
           + "	'" + dr["Plant"].ToString() + "', \n"
           + "	'" + O_HEAD.Rows[0]["VBELN"].ToString() + "', \n"
           + "	'" + O_HEAD.Rows[0]["FKDAT"].ToString().Replace("-", "") + "', \n"
           + "	'" + DateTime.Now.ToString("HHmmss") + "', \n"
           + "	'GM', \n"
           + "	'" + PO + "', \n"
           + "	'" + dr850["PoIssueDate"].ToString() + "', \n"
           + "	'" + dr850["CurrencyCode"].ToString() + "', \n"
           + "	'', \n"
           + "	'', \n"
           + "	'', \n"
           + "	'" + dr850["TermsNetDays"].ToString() + "', \n"
           + "	'" + dr850["PaymentTermsDesc"].ToString() + "', \n"
           + "	'" + dr850["BillToName"].ToString().Replace("'", "''") + "', \n"
           + "	'" + dr850["SoldToName"].ToString().Replace("'", "''") + "', \n"
           + "	'" + dr850["SupplierName"].ToString().Replace("'", "''") + "', \n"
           + "	'" + dr850["ShipToName"].ToString().Replace("'", "''") + "', \n"
           + "	'" + dr850["BillToAddressID"].ToString() + "', \n"
           + "	'" + dr850["SoldToAddressID"].ToString() + "', \n"
           + "	'" + dr850["SupplierAddressID"].ToString() + "', \n"
           + "	'" + dr850["ShipToAddressID"].ToString() + "', \n"
           + "	'" + dr850["BillToStreet"].ToString().Replace("'", "''") + "', \n"
           + "	N'" + dr850["SoldToStreet"].ToString().Replace("'", "''") + "', \n"
           + "	N'" + dr850["SupplierStreet"].ToString().Replace("'", "''") + "', \n"
           + "	N'" + dr850["ShipToStreet"].ToString().Replace("'", "''") + "', \n"
           + "	N'" + dr850["BillToCity"].ToString() + "', \n"
           + "	N'" + dr850["SoldToCity"].ToString() + "', \n"
           + "	N'" + dr850["SupplierCity"].ToString() + "', \n"
           + "	N'" + dr850["ShipToCity"].ToString() + "', \n"
           + "	'" + dr850["BilltoState"].ToString() + "', \n"
           + "	'" + dr850["SoldtoState"].ToString() + "', \n"
           + "	'" + dr850["SupplierState"].ToString() + "', \n"
           + "	'" + dr850["ShipToState"].ToString() + "', \n"
           + "	'" + dr850["BillToZipCode"].ToString() + "', \n"
           + "	'" + dr850["SoldToZipCode"].ToString() + "', \n"
           + "	'" + dr850["SupplierZipCode"].ToString() + "', \n"
           + "	'" + dr850["ShipToZipCode"].ToString() + "', \n"
           + "	'" + dr850["BillToCountryCode"].ToString() + "', \n"
           + "	'" + dr850["SoldToCountryCode"].ToString() + "', \n"
           + "	'" + dr850["SupplierCountryCode"].ToString() + "', \n"
           + "	'" + dr850["ShipToCountryCode"].ToString() + "', \n"
           + "	N'" + SupplierContactName.Replace("'", "''") + "',  \n"
           + "	'" + SupplierTelephone + "',  \n"
           + "	'" + SupplierEmail + "', \n"
           + "	'0' \n"
           + ")";
            string res = sfcdb.ExecSQL(sql);
            if (res.IndexOf("SQL") > -1)
            {
                log.WriterLog("OracleCreateInvoice", DN, O_HEAD.Rows[0]["VBELN"].ToString(), "insertOracle_Invoice_Head", res.Substring(0, 199), "E", "", "");
                throw new Exception(res);
            }
        }

        void insertOracle_Invoice_Detail(SAP_RFC_BASE RFC, OleExec sfcdb, string FileName, string DN)
        {
            DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
            DataTable O_ITEM = RFC.GetTableValue("O_ITEM");
            string PO = O_HEAD.Rows[0]["BSTKD"].ToString();


            for (int i = 0; i < O_ITEM.Rows.Count; i++)
            {
                DataRow dr = O_ITEM.Rows[i];
                string InvoiceLineNo = dr["POSNR"].ToString();
                int z = ZeroPost(InvoiceLineNo);
                InvoiceLineNo = InvoiceLineNo.Substring(z);
                string VendorPartNumber = dr["MATNR"].ToString();
                if (VendorPartNumber.IndexOf('+') > 0)
                {
                    VendorPartNumber = "'" + VendorPartNumber + "'";
                }
                else
                {
                    VendorPartNumber = "null";
                }
                string strSql = $@"SELECT * FROM R_ORACLE_MFPRESETWOHEAD A
                                     WHERE EXISTS (SELECT TOP 1 1 FROM R_DN_LINK B
                                            WHERE A.PO = B.orderno
                                               AND A.POLine = B.orderlineno
                                               AND B.DN_NO = '{DN}')";
                DataTable DT = sfcdb.RunSelect(strSql).Tables[0];
                string PoLine = "";
                string schedule = "1";
                string ls= DT.Rows[0]["POLine"].ToString();
                if (ls.Contains("."))
                {
                    string[] temp = ls.Split('.');
                    PoLine = temp[0];
                    schedule = temp[1];
                }
                else
                {
                    PoLine = ls;
                }
                //string PoLine = Int32.Parse(O_ITEM.Rows[0]["POSEX"].ToString()).ToString();
                string sql = "INSERT INTO Oracle_Invoice_Detail \n"
                       + "( \n"
                       + "	[FileName], \n"
                       + "	InvoiceNumber, \n"
                       + "	InvoiceLineNumber, \n"
                       + "	InvoiceLineQuantity, \n"
                       + "	UnitMeasurementCode, \n"
                       + "	UnitPrice, \n"
                       + "	BuyerPartNumber, \n"
                       + "	VendorPartNumber, \n"
                       + "	PartNumberDescription, \n"
                       + "	TotalTaxAmount, \n"   //需精确到小数点后两位
                       + "	TaxPercentage, \n" //需精确到小数点后两位
                       + "	PONumber, \n"
                       + "	POLineNumber, \n"
                       + "	PackingSlipNumber, \n"
                       + "	ScheduleNumber \n"
                       + ") \n"
                       + "VALUES \n"
                       + "( \n"
                       + "	'" + FileName + "', \n"
                       + "	'" + O_HEAD.Rows[0]["VBELN"].ToString() + "', \n"
                       + "	'" + InvoiceLineNo + "', \n"
                       + "	'" + dr["FKIMG"].ToString() + "', \n"
                       + "	'EA', \n"
                       + "	'" + dr["U_PRICE"].ToString() + "', \n"
                       + "	'" + dr["MATNR"].ToString() + "', \n"
                       + "	" + VendorPartNumber + ", \n"
                       + "	'" + dr["ARKTX"].ToString() + "', \n"
                       + "	'0', \n"
                       + "	'0', \n"
                       + "	'" + PO + "', \n"
                       + "	'" + PoLine + "', \n"
                       + "	'" + PO + "." + PoLine + "', \n"
                       + "	'"+ schedule + "' \n"
                       + ")";
                sql = sql.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                string res = sfcdb.ExecSQL(sql);
                if (res.IndexOf("SQL") > -1)
                {
                    log.WriterLog("OracleCreateInvoice", DN, O_HEAD.Rows[0]["VBELN"].ToString(), "insertOracle_Invoice_Detail", res.Substring(0, 199), "E", "", "");
                    throw new Exception(res);
                }
            }

        }

        void insertOracle_Invoice_Summary(SAP_RFC_BASE RFC, OleExec sfcdb, string FileName, string DN)
        {
            DataTable O_HEAD = RFC.GetTableValue("O_HEAD");
            string PO = O_HEAD.Rows[0]["BSTKD"].ToString();
            DataTable O_TOTAL = RFC.GetTableValue("O_TOTAL");
            DataRow dr = O_TOTAL.Rows[0];
            string AmountBeforTermsDiscount =
                 ((Math.Round((double.Parse(dr["T_AMOUNT"].ToString())), 2)) * 100).ToString();
            string AmountAfterTermsDiscount =
                 ((Math.Round((double.Parse(dr["T_TOTAL"].ToString())), 2)) * 100).ToString();

            string sql = "INSERT INTO Oracle_Invoice_Summary \n"
           + "  ( \n"
            + "	    [FileName], \n"
           + "  	InvoiceNumber, \n"
           + "  	AmountBeforTermsDiscount, \n"
           + "  	AmountAfterTermsDiscount, \n"
           + "  	AmountOfInvoiceDue, \n"
           + "  	AmountOfTermsDiscount, \n"
           + "  	AmountOfFreightCharge, \n"
           + "  	NumberOfLineItems, \n"
           + "  	TotalWeight, \n"
           + "  	UnitMeasurementCode, \n"
           + "  	FreeValume, \n"
           + "  	FreeValumeMeasurementCode, \n"
           + "  	FreeValumeDescription \n"
           + "  ) \n"
           + "  VALUES \n"
           + "  ( \n"
            + "	'" + FileName + "'/* [FileName]	*/, \n"
           + "	'" + O_HEAD.Rows[0]["VBELN"].ToString() + "'/* InvoiceNumber	*/, \n"
           + "  	'" + AmountBeforTermsDiscount + "'/* AmountBeforTermsDiscount	*/, \n"
           + "  	'" + AmountAfterTermsDiscount + "'/* AmountAfterTermsDiscount	*/, \n"
           + "  	null /* AmountOfInvoiceDue	*/, \n"
           + "  	null /* AmountOfTermsDiscount	*/, \n"
           + "  	null/* AmountOfFreightCharge	*/, \n"
           + "  	'1' /* NumberOfLineItems	*/, \n"
           + "  	null/* TotalWeight	*/, \n"
           + "  	'EA'/* UnitMeasurementCode	*/, \n"
           + "  	null/* FreeValume	*/, \n"
           + "  	''/* FreeValumeMeasurementCode	*/, \n"
           + "  	null/* FreeValumeDescription	*/ \n"
           + "  )";
            string res = sfcdb.ExecSQL(sql);
            if (res.IndexOf("SQL") > -1)
            {
                log.WriterLog("OracleCreateInvoice", DN, O_HEAD.Rows[0]["VBELN"].ToString(), "insertOracle_Invoice_Summary", res.Substring(0, 199), "E", "", "");
                throw new Exception(res);
            }
        }
    }
}

