using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;


namespace MESReport.BaseReport
{ 
  public class R_EMS_TRANSMIT_RECEIVER_Report : ReportBase
    {
        #region
        ReportInput doc_noInput = new ReportInput()
        {
            Name = "DOC",
            InputType = "TXT",
            Value = "DOC_NO",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput buyer_remarkInput = new ReportInput()
        {
            Name = "buyer",
            InputType = "TXT",
            Value = "buyer_remark",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput shipment_numInput = new ReportInput()
        {
            Name = "shipment",
            InputType = "TXT",
            Value = "shipment_num",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput asn_line_numInput = new ReportInput()
        {
            Name = "asn_line",
            InputType = "TXT",
            Value = "asn_line_num",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput typeInput = new ReportInput()
        {
            Name = "TYPE",
            InputType = "Select",
            // Value = "查詢最後運行時間",
            Value = "QueryLastRunTime",
            Enable = true,
            SendChangeEvent = false,
            //ValueForUse = new string[] { "查詢最後運行時間", "查詢buyer_remark", "修改buyer_remark", "查詢待獲取EMS收貨數據", "按時間獲取EMS收貨數據", "篩選需要重傳的數據", "修改為2重傳", "修改為0重傳" }
            ValueForUse = new string[] { "QueryLastRunTime", "QueryBuyerRemark", "ModifyBuyerRemark", "QueryEMSReceiptDataToBeObtained", "GetEMSReceivingDataBytime", "FilterDataToBeRetransmitted", "ModifiedToTwoRetransmission", "ModifiedToZeroRetransmission" }
        };
       
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            //Value = "2018-01-01",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput toDate = new ReportInput()
        {
            Name = "To",
            InputType = "DateTime",
            //Value = "2018-03-01",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        #endregion
        public R_EMS_TRANSMIT_RECEIVER_Report()
        {
            Inputs.Add(typeInput);
            Inputs.Add(doc_noInput);
            Inputs.Add(buyer_remarkInput);            
            Inputs.Add(fromDate);
            Inputs.Add(toDate);
            Inputs.Add(shipment_numInput);
            Inputs.Add(asn_line_numInput);
        }

        public override void Run()
        { 
            string start = string.Empty;
            string end = string.Empty;
            if (fromDate.Value != null && fromDate.Value.ToString() != "")
            {
                try
                {
                    start = Convert.ToDateTime(fromDate.Value.ToString()).ToString("yyyyMMddHHmmss");
                }
                catch (Exception)
                {
                   // throw new Exception("日期格式不正確！");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154813"));
                }

            }
            if (toDate.Value != null && toDate.Value.ToString() != "")
            {
                try
                {
                    end = Convert.ToDateTime(toDate.Value.ToString()).ToString("yyyyMMddHHmmss");
                }
                catch (Exception)
                {
                    //throw new Exception("日期格式不正確！");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154813"));
                }

            }
            string type = typeInput.Value.ToString();
            string baseSQL = string.Empty;
            DataTable dt=new DataTable();
            // if (type== "查詢最後運行時間"
            if (type == "QueryLastRunTime")
            {
                //baseSQL = $@" SELECT REMARK AS 上次運行時間, '10分鐘' as 運行頻率
                //                   FROM MES4.R_EMS_TRANSMIT_RECEIVE
                //                  WHERE DOC_NO = 'LAST_RUN_TIME'
                //                    AND STATUS = '1'
                //                    AND ASN_LINE_ID = 'LAST_RUN_TIME'";

                baseSQL = $@" SELECT REMARK AS LastRunTime, 'TenMinutes' as OperatingFrequency
                                   FROM MES4.R_EMS_TRANSMIT_RECEIVE
                                  WHERE DOC_NO = 'LAST_RUN_TIME'
                                    AND STATUS = '1'
                                    AND ASN_LINE_ID = 'LAST_RUN_TIME'";
                dt = getdtfromsql(baseSQL);

            }
            // else if (type == "查詢buyer_remark") QueryBuyerRemark
            else if (type == "QueryBuyerRemark")
            {
                baseSQL = $@" SELECT DOC_NO, BUYER_REMARK
                                  FROM MES4.R_GRn
                                 where doc_no = '{doc_noInput.Value.ToString()}'";
                dt = getdtfromsql(baseSQL);
            }
            //else if (type == "修改buyer_remark")

            else if (type == "ModifyBuyerRemark")
            {
                baseSQL = $@"    UPDATE MES4.R_GRn
                          SET BUYER_REMARK = '{buyer_remarkInput.Value.ToString()}'
                        where doc_no = '{doc_noInput.Value.ToString()}'";
                execSql(baseSQL);

                baseSQL = $@" SELECT DOC_NO, BUYER_REMARK
                                  FROM MES4.R_GRn
                                 where doc_no = '{doc_noInput.Value.ToString()}'";
                dt = getdtfromsql(baseSQL);
            }
            //else if (type == "查詢待獲取EMS收貨數據")
            else if (type == "QueryEMSReceiptDataToBeObtained")
            {
                baseSQL = $@"SELECT remark  , sp_message ,DOC_NO, STATUS, ASN_LINE_ID,create_time
                              FROM MES4.R_EMS_TRANSMIT_RECEIVE
                             where DOC_NO = 'SET_RUNTIME_FROM_TO'
                               and STATUS = '1'
                               and ASN_LINE_ID = 'SET_RUNTIME_FROM_TO'";
                dt = getdtfromsql(baseSQL);
            }
            // else if (type == "按時間獲取EMS收貨數據")
            else if (type == "GetEMSReceivingDataBytime")
            {
                baseSQL = $@"  
                 insert into MES4.R_EMS_TRANSMIT_RECEIVE
                   (remark,
                    sp_message,
                    DOC_NO,
                    STATUS,
                    ASN_LINE_ID,
                    create_time)
                 values
                   ('{start}',
                    '{end}',
                    'SET_RUNTIME_FROM_TO',
                    1,
                    'SET_RUNTIME_FROM_TO',
                    SYSDATE)                     
                    ";
                execSql(baseSQL);
                baseSQL = $@"SELECT remark  , sp_message ,DOC_NO, STATUS, ASN_LINE_ID,create_time
                              FROM MES4.R_EMS_TRANSMIT_RECEIVE
                             where DOC_NO = 'SET_RUNTIME_FROM_TO'
                               and STATUS = '1'
                               and ASN_LINE_ID = 'SET_RUNTIME_FROM_TO'";
                dt = getdtfromsql(baseSQL);
            }
            // else if (type == "篩選需要重傳的數據") 
            else if (type == "FilterDataToBeRetransmitted")
            {
                baseSQL = $@"
                            select *
                              from mes4.r_ems_transmit_receive
                             WHERE shipment_num = '{shipment_numInput.Value.ToString()}'
                               and asn_line_num = '{asn_line_numInput.Value.ToString()}' ";
                dt = getdtfromsql(baseSQL);
            }
            //else if (type == "修改為2重傳") 
            else if (type == "ModifiedToTwoRetransmission")
            {
                baseSQL = $@"  
                              update mes4.r_ems_transmit_receive
                                 set status = 2
                               WHERE shipment_num = '{shipment_numInput.Value.ToString()}'
                                 and asn_line_num = '{asn_line_numInput.Value.ToString()}'
                    ";
                execSql(baseSQL);
                baseSQL = $@"
                            select *
                              from mes4.r_ems_transmit_receive
                             WHERE shipment_num = '{shipment_numInput.Value.ToString()}'
                               and asn_line_num = '{asn_line_numInput.Value.ToString()}' ";
                dt = getdtfromsql(baseSQL);

            }
            // else if (type == "修改為0重傳") ModifiedToZeroRetransmission
            else if (type == "ModifiedToZeroRetransmission")
            {
                baseSQL = $@"  
                              update mes4.r_ems_transmit_receive
                                 set status = 0,QUANTITY_RECEIVED='', QUANTITY_REJECT='', DOC_NO=''
                               WHERE shipment_num = '{shipment_numInput.Value.ToString()}'
                                 and asn_line_num = '{asn_line_numInput.Value.ToString()}'
                    ";
                execSql(baseSQL);
                baseSQL = $@"
                            select *
                              from mes4.r_ems_transmit_receive
                             WHERE shipment_num = '{shipment_numInput.Value.ToString()}'
                               and asn_line_num = '{asn_line_numInput.Value.ToString()}' ";
                dt = getdtfromsql(baseSQL);

            }



            ReportTable retTab = new ReportTable();
            retTab.LoadData(dt, null);
            retTab.Tittle = "SMTFQC BY SN REPORT";
            Outputs.Add(retTab);
        }
        private DataTable getdtfromsql(string sql) {
            OleExec apdb = DBPools["APDB"].Borrow();
            DataTable dt = apdb.RunSelect(sql).Tables[0];
            if (apdb != null)
            {
                this.DBPools["APDB"].Return(apdb);
            }
            return dt;
        }

        private void execSql(string sql)
        {
            OleExec apdb = DBPools["APDB"].Borrow();
             apdb.ExecSQL(sql);
            if (apdb != null)
            {
                this.DBPools["APDB"].Return(apdb);
            }
             
        }
    }
}
