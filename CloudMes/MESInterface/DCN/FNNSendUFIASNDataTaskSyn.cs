using DcnSfcModel;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESInterface.DCN
{
    public class FNNSendUFIASNDataTaskSyn: taskBase
    {
        private bool IsRuning = false;
        private string mesdbstr, custdbstr, plantstr, taskNum;
        
        public override void init()
        {
            try
            {
                //apdbstr = ConfigGet("APDB");
                mesdbstr = ConfigGet("MESDB");
                custdbstr = ConfigGet("CUSTDB");
                plantstr = ConfigGet("PLANT");               
                taskNum = ConfigGet("TASK_NUM");
                //csvFilePath = $@"{filepath}";
                Output.UI = new FNNSendUFIASNDataUI(this);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      

        public override void Start()
        {
            try
            {
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false, SqlSugar.DbType.SqlServer);
                DateTime collectDate = SFCDB.GetDate().AddDays(-1);
                SendData(collectDate);
                SFCDB.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        
        public void SendData(DateTime collectDate)
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes> listresult = new Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes>();
            try
            {
                MesLog.Info("Start");
                if (plantstr == "")
                {
                    throw new Exception("Please setting PLANT");
                }                
                Run(collectDate, taskNum);
            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                try
                {
                    foreach (var r in listresult)
                    {
                        string log = $@"{r.Key} send ";
                        if (r.Value.IsSuccess)
                        {
                            log += $@"success";
                        }
                        else
                        {
                            log += $@"fail;Error message:{r.Value.ErrorMessage}";
                        }
                        MesLog.Info(log);
                    }
                }
                catch (Exception)
                {
                }
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        public void Run(DateTime collectedDate, string taskNum)
        {
            try
            {
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false, SqlSugar.DbType.SqlServer);
                SqlSugarClient CUSTDB = OleExec.GetSqlSugarClient(custdbstr, false, SqlSugar.DbType.SqlServer);
                string collectedDateStr = collectedDate.ToString("yyyy-MM-dd");
                //string collectedDateStr = "2021-11-16";
                DNCancel(SFCDB,CUSTDB,collectedDateStr);
                DataTable ASNData = CollectASNData(SFCDB, collectedDateStr);
                MakeASNData(SFCDB, ASNData);
                InsertUfiAsnDB(SFCDB, CUSTDB);
                WriteLog(SFCDB);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Class:Run FNNSendUFIASNData Error Msg: {ex.Message}");
            }
        }
        public DataTable CollectASNData(SqlSugarClient SFCDB, string collectedDateStr)
        {

            //SFCDB.DbFirst.Where()
            
            try
            {
                return SFCDB.Ado.GetDataTable($@"  SELECT A.DN_NO,
                                                                C.TOPASSPN PNO,
                                                                C.TOPASSSN SYSTEM_SNO,
                                                                rtrim(ltrim(ISNULL(CASE WHEN G.VSSN IS NULL OR G.VSSN = '' THEN  E.CSERIALNO ELSE G.VSSN END,''))) VANILLA_SNO,
                                                                rtrim(ltrim(ISNULL(CASE WHEN G.WSN IS NULL OR G.WSN = '' THEN F.CSERIALNO ELSE G.WSN END, ''))) MB_PCBA_SNO,
                                                                A.PO_NO UFI_PO_NO,
                                                                C.SHIPDATE SHIP_DATE,
                                                                B.TO_NO,
                                                                rtrim(ltrim(ISNULL(G.WWNTB0, ''))) SSD_VENDOR,
                                                                rtrim(ltrim(ISNULL(G.MAC, ''))) MAC_ADDR,
                                                                D.CARTON_NO CARTON_ID,
                                                                D.LOCATION PALLET_ID,
                                                                'N' SendFlag,
                                                                GETDATE() Create_Time,
                                                                'interface' Edit_Emp,
                                                                GETDATE() Edit_time
                                                        FROM R_DN_BROCADE A(NOLOCK)
                                                        INNER JOIN R_TO_DETAIL B(NOLOCK)
                                                            ON A.DN_NO = B.DN_NO
                                                            AND B.DN_CUSTOMER = 'BUS005'
                                                        INNER JOIN ASSHIPPED C(NOLOCK)
                                                            ON A.SHIP_ORDER = C.SHIPORDERNO
                                                            AND C.TOPASSSN NOT LIKE '[R~*#]%'
                                                            AND CONVERT(VARCHAR(10), C.SHIPDATE, 121) = '{collectedDateStr}'
                                                        INNER JOIN ADD_WO_INFO D(NOLOCK)
                                                            ON C.TOPASSSN = D.SYSSERIALNO
                                                        LEFT JOIN MFSYSCSERIAL E(NOLOCK)
                                                            ON C.TOPASSSN = E.SYSSERIALNO
                                                            AND E.EEECODE = 'PPM-1'
                                                        LEFT JOIN MFSYSCSERIAL F(NOLOCK)
                                                            ON E.CSERIALNO = F.SYSSERIALNO
                                                            AND F.EEECODE = 'PCBA-1'
                                                        LEFT JOIN WWN_DATASHARING G(NOLOCK)
                                                            ON C.TOPASSSN = G.CSSN
                                                            AND G.MAC <> ''
                                                        WHERE 1 = 1 ");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running Collect ASNData Data Fail;Fail Msg:{ex.Message}");
            }
        }

        public void WriteLog(SqlSugarClient SFCDB)
        {
            try
            {
                SqlSugarClient LogDB = SFCDB;
                AppLog log = new AppLog();
                log.AppName = "FNNSendUFIASNData";
                log.MsgType = "Running Log";
                log.Message = "Complete";
                log.Time = LogDB.GetDate();
                log.AddValue = "interface"; 
                LogDB.Insertable<AppLog>(log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Wirte AppLog error;Error Msg:{ex.Message}");
            }
        }

        public void MakeASNData(SqlSugarClient SFCDB, DataTable ASNData)
        {
            if (ASNData != null)
            {
                foreach (DataRow asn in ASNData.Rows)
                {
                    try
                    {
                        SqlSugarClient sfcdb = SFCDB;
                        r_ufi_asndata rua = new r_ufi_asndata();
                        rua.DN_NO = asn["DN_NO"].ToString();
                        rua.PNO = asn["PNO"].ToString();
                        rua.System_SNO = asn["System_SNO"].ToString();
                        rua.Vanilla_SNO = asn["Vanilla_SNO"].ToString();
                        rua.MB_PCBA_SNO = asn["MB_PCBA_SNO"].ToString();
                        rua.UFI_PO_NO = asn["UFI_PO_NO"].ToString();
                        rua.SHIP_DATE = Convert.ToDateTime(asn["SHIP_DATE"]);
                        rua.TO_NO = asn["TO_NO"].ToString();
                        rua.SSD_Vendor = asn["SSD_Vendor"].ToString();
                        rua.MAC_Addr = asn["MAC_Addr"].ToString();
                        rua.Carton_ID = asn["Carton_ID"].ToString(); 
                        rua.Pallet_ID = asn["Pallet_ID"].ToString();
                        rua.SendFlag = "N";
                        rua.Create_Time = sfcdb.GetDate();
                        rua.Edit_Emp= "interface";
                        rua.Edit_Time = sfcdb.GetDate();
                        sfcdb.Insertable<r_ufi_asndata>(rua).ExecuteCommand();

                    }
                    catch (Exception ex)
                    {
                        throw new Exception($@"MakeASNData error, Msg: {ex.Message}");
                    }
                }
            }

        }

        public void DNCancel(SqlSugarClient SFCDB, SqlSugarClient CUSTDB, string collectedDateStr)
        {
            var checkrs = SFCDB.Ado.GetDataTable($@"select logevent from sdeventlog a where a.orderno = 'RETRUN_DN' and exists (select * from R_TO_DETAIL b where a.logevent = b.DN_NO and b.DN_CUSTOMER = 'BUS005') and  convert(varchar(10),logdate,121)='{collectedDateStr}' " );
            if(checkrs.Rows.Count!=0)
            {
                foreach (DataRow rs in checkrs.Rows)
                {
                    try                    
                    {
                        _ = CUSTDB.Ado.ExecuteCommand($@" update shipment set status='N' where DN_NO='{rs["logevent"]}' and Status='Y'  ");
                    }
                    catch (Exception ex)
                    { 
                        throw new Exception($@"update custdb error, Msg: {ex.Message}");
                    }
                }    
            }
        }

        public void InsertUfiAsnDB(SqlSugarClient SFCDB, SqlSugarClient CUSTDB)
        {
            var waitsend = SFCDB.Ado.GetDataTable($@" select DN_NO,PNO,System_SNO,Vanilla_SNO,MB_PCBA_SNO,UFI_PO_NO,SHIP_DATE,TO_NO,SSD_Vendor,MAC_Addr,Carton_ID,Pallet_ID from r_ufi_asndata where SendFlag='N' ");
            if(waitsend.Rows.Count!=0)
            {
                foreach (DataRow ws in waitsend.Rows)
                {
                    try
                    {
                        var isSend = CUSTDB.Ado.GetDataTable($@"select top 1 1 from shipment(nolock) where status='Y' and DN_NO='{ws["DN_NO"]}'and System_SNO= '{ws["System_SNO"]}' ");
                        if (isSend.Rows.Count==0)
                        {
                            _ = CUSTDB.Ado.ExecuteCommand($@" insert into shipment(UPLOAD_TIME,DN_NO,PNO,System_SNO,Vanilia_SNO,MB_PCBA_SNO,UFI_PO_NO,SHIP_DATE,TO_NO,SSD_Vendor,MAC_Addr,Carton_ID,Pallet_ID,Status)
                                                                values(getdate(),'{ws["DN_NO"]}','{ws["PNO"]}','{ws["System_SNO"]}','{ws["Vanilla_SNO"]}','{ws["MB_PCBA_SNO"]}','{ws["UFI_PO_NO"]}','{ws["SHIP_DATE"]}','{ws["TO_NO"]}','{ws["SSD_Vendor"]}','{ws["MAC_Addr"]}','{ws["Carton_ID"]}','{ws["Pallet_ID"]}' ,'Y')  ");

                            _ = SFCDB.Ado.ExecuteCommand($@" update r_ufi_asndata set SendFlag='Y',Edit_Time=GETDATE(),Message='Send Successfully' where DN_NO='{ws["DN_NO"]}' and System_SNO='{ws["System_SNO"]}' and SendFlag='N'");
                        }
                        else
                        {
                            _ = SFCDB.Ado.ExecuteCommand($@" update r_ufi_asndata set SendFlag='Y',Message='Already Exists' where DN_NO='{ws["DN_NO"]}' and System_SNO='{ws["System_SNO"]}' and SendFlag='N'");
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = SFCDB.Ado.ExecuteCommand($@" update r_ufi_asndata set Message='{ex.Message}' where DN_NO='{ws["DN_NO"]}' and System_SNO='{ws["System_SNO"]}' and SendFlag='N'");
                        throw new Exception($@"insert custdb error, Msg: {ex.Message}");
                    }
                }
            }
        }

    }
}
