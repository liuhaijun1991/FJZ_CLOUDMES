using MESDataObject;
using MESDataObject.Module;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using MESPubLab;
using MESDBHelper;
using MESPubLab.Common;


namespace MESInterface.DCN
{
    public class SendBrcmData : taskBase
    {
        public bool IsRuning = false;
        public MESDBHelper.OleExec SFCDB = null;
       
        public override void init()
        {

            try
            {
                SFCDB = new OleExec( ConfigGet("DB"),false);
             
            }
            catch (Exception)
            {
            }

        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("正在執行,請稍後再試");
            }
            try
            {
                CustData_Brcm CustData_Brcm = new CustData_Brcm(SFCDB);
                DataTable dt = CustData_Brcm.GetDnList();
             

                if (dt.Rows.Count> 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CustData_Brcm.CreateFile(dt.Rows[i][0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SFCDB.FreeMe();
            }

            IsRuning = true;
            IsRuning = false;
        }



        class CustData_Brcm
        {
            public string FolderName = Environment.CurrentDirectory + "\\File\\BRCM_ASN";
            OleExec SFCDB;

            public string RecodeDateTime;
            public string Skuno;
            public string DnQty;
            public string So;
            public string SoLIne;
      

            public CustData_Brcm(OleExec _SFCDB)
            {
                SFCDB = _SFCDB;
            }

            /// <summary>
            /// 取需要生成文件的列表,待寫
            /// </summary>
            /// <returns></returns>
            public DataTable GetDnList()
            {
                DataTable dt = new DataTable();
                
              
                DataSet ds = SFCDB.RunSelect($@"select a.dn_no From r_dn_cust_po a, r_ship_detail b where a.dn_no=b.dn_no and a.ext_key1 is null ");
                dt = ds.Tables[0];
                return dt;
            }

            /// <summary>
            /// 生成格式為|的字符串;
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public string GetData(DataTable dt)
            {
                string strFileData = "";
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i == dt.Columns.Count - 1)
                            strFileData += dr[i].ToString().Trim() + "\r\n";
                        else
                            strFileData += dr[i].ToString().Trim() + "|";
                    }
                }
                strFileData += "CTL|" + dt.Rows.Count;
                return strFileData;
            }
          
            public void UpdateStatus(string DNNO)
            {
                //Update The DN Status      edit by taylor
                string sql = "update r_dn_cust_po set ext_key1='OK' WHERE dn_no='" + DNNO + "'";
                 SFCDB.ExecSQL(sql);

            }

            public void UpdateSendStatus(string DNNO)
            {
              
                string sql = "update r_brcm_data_head set sendtime=sysdate WHERE dnno='" + DNNO + "' ";
                 SFCDB.ExecSQL(sql);
            }

            /// <summary>
            /// TEST: 5043018917
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            public FileAttr GetFile_CustData(string TYPE, string DNNO)
            {
                string strsql = $@" SELECT distinct c.V2 FROM r_dn_cust_po a, broadcom_csv_detail c  WHERE a.DN_NO='{DNNO}' AND a.CUST_PO_NO=c.V10 ";
                DataSet dss = SFCDB.RunSelect(strsql);
                string CM_CODE = dss.Tables[0].Rows[0][0].ToString();

                DataSet ShipDs = new DataSet();
                DataSet DataDs = new DataSet();
                switch (TYPE)
                {
                    case "SHIP":
                        InsertSHIP_DATA(DNNO, CM_CODE, SFCDB);
                        ShipDs = SFCDB.RunSelect($@"SELECT DISTINCT Record_Type,CM_Code,Record_Creation_Date ,Transaction_Type
                                                                                ,Shipment_ID ,Item,UOM ,Department_Code ,Completion_Date,Shipment_Date
                                                                                ,Shipment_Number ,Delivery_Date,Delivery_Note_Number ,Ship_To_Address_Code
                                                                                 ,Ship_Method ,Order_Number,Line_Number,PO_Number,PO_Line_Number,PO_Shipment_Number
                                                                                 ,PO_Release_Number ,Quantity_Completed ,Quantity_Shipped ,Waybill_Number,Packing_Slip_Number
                                                                                 ,Bill_Of_Lading,Lot_Number,Lot_Quantity,COO ,LPN ,Manufacture_Date,CAT
                                                                                 ,BIN,Custom_PN,REV,Vendor ,Comment1 ,Test_Program_Revision ,Number_of_WIPC_records
                                                                                ,Reserved_Columns ,ShipToSite,ShipToDept,Vendor_Part_Number,Vendor_Lot_Number
                                                                                 ,NC_Reason_Code ,BATCH_NOTE,SUBSTRATE_ID,Seal_Date ,Good_die_qty
                                                                                ,Die_Part_Number,Date_Code,LPN_Lot_attributes,Outer_LPN_Number ,Outer_LPN_Flag
                                                                                ,Wafer_Batch_Number,Country_of_Diffusion FROM R_BRCM_DATA_HEAD a,R_BRCM_SHIP_DATA b 
                                                                                where a.dnno='{DNNO}' and a.filename=b.filename and filetype='SHIP' 
                                                                                ORDER BY transaction_type ASC");

                         DataDs = SFCDB.RunSelect($@"SELECT filename FROM R_BRCM_DATA_HEAD WHERE dnno = '{DNNO}' AND filetype = 'SHIP'");

                        break;
                    
                    case "WIP":
                        InsertWIP(DNNO, CM_CODE, SFCDB);
                       
                        ShipDs = SFCDB.RunSelect($@"SELECT * FROM R_BRCM_DATA_HEAD a,R_BRCM_PICK2SHIP_DATA b 
                                                                                          where a.dnno='{DNNO}' and a.filename=b.filename and filetype='WIP'");

                        DataDs = SFCDB.RunSelect($@"SELECT filename FROM R_BRCM_DATA_HEAD WHERE dnno='{DNNO}' AND filetype='WIP'");

                        break;
                    case "P2SHPCFM":
                        InsertP2SHPCFM(DNNO, CM_CODE, SFCDB);
                        ShipDs = SFCDB.RunSelect($@"SELECT DISTINCT Record_Type ,CM_Code,Delivery_Name,Sales_Order_Number
                                                                                        ,Sales_Order_Line_Number ,Ship_Method ,Incoterm ,Shipment_Date
                                                                                        ,Waybill_Number ,Shipping_LPN ,Box_Code,Box_Weight ,Item
                                                                                        ,Lot_Number,Shipped_Quantity,Lot_LPN,Serial_Number,Customer_Serial_Number
                                                                                        ,Department_Code ,Comment1,Reserved_Columns1,Reserved_Columns2,Reserved_Columns3
                                                                                        ,Reserved_Columns4,Reserved_Columns5,Reserved_Columns6,Reserved_Columns7
                                                                                        ,Reserved_Columns8,Reserved_Columns9 ,Reserved_Columns10,Reserved_Columns11
                                                                                        ,Reserved_Columns12,Reserved_Columns13,Reserved_Columns14,Reserved_Columns15
                                                                                        ,Reserved_Columns16,Reserved_Columns17 ,Reserved_Columns18,Reserved_Columns19
                                                                                        ,Reserved_Columns20 FROM R_BRCM_DATA_HEAD a,R_BRCM_PICK2SHIP_DATA b 
                                                                                        where a.dnno='{DNNO}' and a.filename=b.filename and filetype='P2SHPCFM' ");

                        DataDs = SFCDB.RunSelect($@"SELECT filename FROM R_BRCM_DATA_HEAD WHERE dnno = '{DNNO}' AND filetype = 'P2SHPCFM'");
                        break;
                    default:
                        break;
                }
                FileAttr fa = new FileAttr();
                fa.FileName = DataDs.Tables[0].Rows[0][0].ToString();
                fa.StrFileData = GetData(ShipDs.Tables[0]);
                return fa;
            }
            public void InsertSHIP_DATA(string DNNO, string CM_CODE,OleExec oleExec)
            {
                MESPubLab.MESStation.MESStationBase station = new MESPubLab.MESStation.MESStationBase();
               T_R_BRCM_SHIP_DATA SHIP_DATA = new T_R_BRCM_SHIP_DATA(SFCDB, station.DBType);
                T_R_BRCM_DATA_HEAD DATA_HEAD = new T_R_BRCM_DATA_HEAD(SFCDB, station.DBType);
                string HEADER_ID = SHIP_DATA.GetNewID(station.BU, SFCDB);                
                string DATA_ID = DATA_HEAD.GetNewID(station.BU, SFCDB);
                RecodeDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                string strsql = $@"select*From R_BRCM_DATA_HEAD where  DNNO='{DNNO}' ";
                DataSet ds = oleExec.RunSelect(strsql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strsql = $@" select b.RECORD_CREATION_DATE  From R_BRCM_DATA_HEAD a,R_BRCM_SHIP_DATA b where a.filename=b.filename and  a.DNNO='{DNNO}'";
                    ds = oleExec.RunSelect(strsql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        RecodeDateTime = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
               string FileName = "VN_XXAT_SHIP_" + CM_CODE + "_" + RecodeDateTime + ".dat";
                string SqlDeteleship= $@"delete R_BRCM_SHIP_DATA where FileName='{FileName}'  ";
                oleExec.ExecSQL(SqlDeteleship);
                string SqlDetelehead = $@"delete R_BRCM_DATA_HEAD where Filetype = 'SHIP' and FileName = '{FileName}'";
                oleExec.ExecSQL(SqlDetelehead);

                string StrSqlDn = $@"select*From r_dn_cust_po where DN_NO='{DNNO}'";
                DataSet dsDn = oleExec.RunSelect(StrSqlDn);
                if (dsDn.Tables[0].Rows.Count > 0)
                {
                     Skuno = dsDn.Tables[0].Rows[0]["DN_SKUNO"].ToString();
                    DnQty = dsDn.Tables[0].Rows[0]["DN_QTY"].ToString();
                    So = dsDn.Tables[0].Rows[0]["CUST_PO_NO"].ToString();
                    SoLIne = dsDn.Tables[0].Rows[0]["CUST_PO_LINE_NO"].ToString();
                }
                string StrSqlSHIP_DATA = $@"INSERT INTO R_BRCM_SHIP_DATA (id, record_type,cm_code, 
                                                  Record_Creation_Date, Transaction_Type,  Item,  UOM, Department_Code,  Completion_Date,
                                                  Shipment_Date, Shipment_Number, Quantity_Completed, Quantity_Shipped, Packing_Slip_Number,  ShipToSite, CreateTime , FileName)
                                                  VALUES ('{HEADER_ID}','SHIP','{CM_CODE}','{RecodeDateTime}', 'HEADER', '{Skuno}', 'EA','SAN',       
                                                  '{RecodeDateTime}','{RecodeDateTime}','FXV'||'{DNNO}','{DnQty}','{DnQty}','FXV'||'{DNNO}','M', sysdate, '{FileName}')";
                oleExec.ExecSQL(StrSqlSHIP_DATA);

                string StrsqlDetail = $@"   select substr(sn,0,8) as lot ,count(1) lotqty From r_ship_detail where dn_no='{DNNO}' GROUP BY substr(sn,0,8) ";
                DataSet Detail = oleExec.RunSelect(StrsqlDetail);
                if (Detail.Tables[0].Rows.Count > 0)
                {
                    
                    for (int i = 0; i < Detail.Tables[0].Rows.Count; i++)
                    {
                        string LOT_ID = SHIP_DATA.GetNewID(station.BU, SFCDB);
                        string StrLot = Detail.Tables[0].Rows[i]["lot"].ToString();
                        string LotQty= Detail.Tables[0].Rows[i]["lotqty"].ToString();
                        string Manufacture_Date= DateTime.Now.ToString("yyyyMMdd")+"000000";

                        string Ship_Lot = $@"INSERT INTO R_BRCM_SHIP_DATA (id,record_type, cm_code,  Record_Creation_Date, Transaction_Type, 
                                                                    Item,  Department_Code,Shipment_Number,  Lot_Number,Lot_Quantity, coo, Manufacture_Date, CreateTime,FileName)
                                                                    VALUES ('{LOT_ID}','SHIP','{CM_CODE}','{RecodeDateTime}','LOT','{Skuno}',  'SAN','FXV'||'{DNNO}','{StrLot}'||'VN','{LotQty}','VN','{Manufacture_Date}', 
                                                                    sysdate,'{FileName}' )  ";
                        oleExec.ExecSQL(Ship_Lot);
                    }
                }
                string Data_Head = $@"INSERT INTO R_BRCM_DATA_HEAD (ID,DNNO,SONO,SOLINENO,FILENAME,FILETYPE,CREATETIME,FLAG)
					VALUES('{DATA_ID}','{DNNO}','{So}','{SoLIne}','{FileName}','SHIP',sysdate,0)";
                oleExec.ExecSQL(Data_Head);
            }
            public void InsertP2SHPCFM(string DNNO, string CM_CODE, OleExec oleExec)
            {
                string Incoterm = "";
                string Shiptomethod = "";
                string DeliveryPrefix = "";
                string Box_Code = "";
                string BOXWEIGHT = "";
                if (CM_CODE == "B_FXCN_CH_BD")
                {
                    DeliveryPrefix = "MBT";
                }
                 if(CM_CODE == "B_FXCN_US_BD")

                {
                    DeliveryPrefix = "MBN";
                }
                if (CM_CODE == "B_FXVN_US_BD")

                {
                    DeliveryPrefix = "MBO";
                }
                MESPubLab.MESStation.MESStationBase station = new MESPubLab.MESStation.MESStationBase();
                T_R_BRCM_DATA_HEAD DATA_HEAD = new T_R_BRCM_DATA_HEAD(SFCDB, station.DBType);
                T_R_BRCM_PICK2SHIP_DATA P2SHIP_DATA = new T_R_BRCM_PICK2SHIP_DATA(SFCDB, station.DBType);
                string DATA_ID = DATA_HEAD.GetNewID(station.BU, SFCDB);
                
                string StrSqlCheck = $@" select*from R_BRCM_DATA_HEAD where Filetype='SHIP' AND DNNO='{DNNO}'";
                DataSet dscheck = oleExec.RunSelect(StrSqlCheck);
                if (dscheck.Tables[0].Rows.Count == 0)
                {
                    InsertSHIP_DATA(DNNO, CM_CODE, SFCDB);
                }
                string StrSqlCheckWip = $@" select*from R_BRCM_DATA_HEAD where Filetype='WIP' AND DNNO='{DNNO}'";
                DataSet dscheckWip = oleExec.RunSelect(StrSqlCheckWip);
                if (dscheckWip.Tables[0].Rows.Count == 0)
                {
                    InsertWIP(DNNO, CM_CODE, SFCDB);
                }
                RecodeDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string strsql = $@"select*From R_BRCM_DATA_HEAD where  DNNO='{DNNO}' ";
                DataSet ds = oleExec.RunSelect(strsql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strsql = $@" select b.RECORD_CREATION_DATE  From R_BRCM_DATA_HEAD a,R_BRCM_SHIP_DATA b where a.filename=b.filename and  a.DNNO='{DNNO}'";
                    ds = oleExec.RunSelect(strsql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        RecodeDateTime = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
                string FileName = "VN_XXAT_P2SHPCFM_" + CM_CODE + "_" + RecodeDateTime + ".dat";
                string StrSqlDn = $@"select*From r_dn_cust_po where DN_NO='{DNNO}'";
                DataSet dsDn = oleExec.RunSelect(StrSqlDn);
                if (dsDn.Tables[0].Rows.Count > 0)
                {
                    Skuno = dsDn.Tables[0].Rows[0]["DN_SKUNO"].ToString();
                    DnQty = dsDn.Tables[0].Rows[0]["DN_QTY"].ToString();
                    So = dsDn.Tables[0].Rows[0]["CUST_PO_NO"].ToString();
                    SoLIne = dsDn.Tables[0].Rows[0]["CUST_PO_LINE_NO"].ToString();
                }
                string StrSqlPacksize = $@"select*from c_packing_size where skuno='{Skuno}'";
                DataSet DsSize = SFCDB.RunSelect(StrSqlPacksize);
                if (DsSize.Tables[0].Rows.Count > 0)
                {
                    Box_Code = DsSize.Tables[0].Rows[0]["CARTON_SIZE"].ToString();
                }
                string StrSqlPw= $@"select*from SFCBASE.C_PACKOUT_WEIGHT  where PN='{Skuno}'";
                DataSet DsPw = SFCDB.RunSelect(StrSqlPw);
                if (DsPw.Tables[0].Rows.Count > 0)
                {
                    BOXWEIGHT = DsPw.Tables[0].Rows[0]["BOX_GW"].ToString();
                }

                string strsqlcsv = $@" SELECT c.V36,c.V35  FROM r_dn_cust_po a,  broadcom_csv_detail c 
                                                       WHERE a.DN_NO='{DNNO}'AND a.CUST_PO_NO=c.V10 order by c.recno desc";
                DataSet dscsv = SFCDB.RunSelect(strsqlcsv);
                if(dscsv.Tables[0].Rows.Count > 0)
                {
                     Incoterm = dscsv.Tables[0].Rows[0][0].ToString();
                    Shiptomethod = dscsv.Tables[0].Rows[0][1].ToString();
                }
      
                string SqlDeteleship= $@"delete R_BRCM_PICK2SHIP_DATA where FileName='{FileName}'  ";
                oleExec.ExecSQL(SqlDeteleship);
                string SqlDetelehead = $@"delete R_BRCM_DATA_HEAD where Filetype = 'P2SHPCFM' and FileName = '{FileName}'";
                oleExec.ExecSQL(SqlDetelehead);

                string StrsqlDetail = $@" SELECT distinct a.sn,platte.pack_no,CASE WHEN C.value='' OR C.value IS NULL THEN 'NA' 
                                    ELSE C.value END PPID_SN from r_ship_detail a,r_sn_packing e,r_packing f,r_packing platte, r_sn b 
                                    LEFT JOIN r_sn_kp C ON B.SN=C.SN AND C.KP_NAME='PPID S/N' where a.sn=b.SN and platte.id=f.parent_pack_id
                                    and b.valid_flag =1 and b.id=e.sn_id and e.pack_id=f.id and a.DN_NO='{DNNO}' ";
                DataSet Detail = oleExec.RunSelect(StrsqlDetail);
                if (Detail.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < Detail.Tables[0].Rows.Count; i++)
                    {
                        String P2SHIP_DATA_ID = P2SHIP_DATA.GetNewID(station.BU, SFCDB);
                        string StrSn= Detail.Tables[0].Rows[i]["sn"].ToString();
                        string Pack_no = Detail.Tables[0].Rows[i]["pack_no"].ToString();
                        string Psn = Detail.Tables[0].Rows[i]["PPID_SN"].ToString();


                        string PICK2SHIP = $@"  INSERT INTO R_BRCM_PICK2SHIP_DATA ( ID,Record_Type,
                                                                  CM_Code, Delivery_Name,Sales_Order_Number,Sales_Order_Line_Number,Ship_Method,Incoterm,
                                                                  Shipment_Date,Shipping_LPN, Box_Code,Box_Weight,Item,Lot_Number,Shipped_Quantity,
                                                                  Lot_LPN,Serial_Number,Customer_Serial_Number,Department_Code,CreateTime, FileName)
                                                                  VALUES('{P2SHIP_DATA_ID}', 'P2SHPCFM', '{CM_CODE}','FXV'||'{DNNO}','{So}','{SoLIne}',
                                                                   '{Shiptomethod}', '{Incoterm}','{RecodeDateTime}','{DeliveryPrefix}'||'{Pack_no}','{Box_Code}','{BOXWEIGHT}',
                                                                    '{Skuno}',substr('{StrSn}',0,8)||'VN','1','NA','{StrSn}','{Psn}','SAN',sysdate,'{FileName}')   ";
                        oleExec.ExecSQL(PICK2SHIP);
                    }
                }
                string Data_Head = $@"INSERT INTO R_BRCM_DATA_HEAD (ID,DNNO,SONO,SOLINENO,FILENAME,FILETYPE,CREATETIME,FLAG)
					VALUES('{DATA_ID}','{DNNO}','{So}','{SoLIne}','{FileName}','P2SHPCFM',sysdate,0)";
                oleExec.ExecSQL(Data_Head);

            }
            public void InsertWIP(string DNNO, string CM_CODE,OleExec oleExec)
            {
                MESPubLab.MESStation.MESStationBase station = new MESPubLab.MESStation.MESStationBase();
                T_R_BRCM_DATA_HEAD DATA_HEAD = new T_R_BRCM_DATA_HEAD(SFCDB, station.DBType);
                string DATA_ID = DATA_HEAD.GetNewID(station.BU, SFCDB);
                string StrSqlCheck = $@" select*from R_BRCM_DATA_HEAD where Filetype='SHIP' AND DNNO='{DNNO}'";
                DataSet dscheck = oleExec.RunSelect(StrSqlCheck);
                if (dscheck.Tables[0].Rows.Count == 0)
                {
                    InsertSHIP_DATA(DNNO, CM_CODE, SFCDB);
                }

                RecodeDateTime = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
                string strsql = $@"select*From R_BRCM_DATA_HEAD where  DNNO='{DNNO}' ";
                DataSet ds = oleExec.RunSelect(strsql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strsql = $@" select b.RECORD_CREATION_DATE  From R_BRCM_DATA_HEAD a,R_BRCM_SHIP_DATA b where a.filename=b.filename and  a.DNNO='{DNNO}'";
                    ds = oleExec.RunSelect(strsql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        RecodeDateTime = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
              string FileName = "VN_XXAT_WIPC_" + CM_CODE + "_" + RecodeDateTime + ".dat";
                string StrSqlDn = $@"select*From r_dn_cust_po where DN_NO='{DNNO}'";
                DataSet dsDn = oleExec.RunSelect(StrSqlDn);
                if (dsDn.Tables[0].Rows.Count > 0)
                {
                    Skuno = dsDn.Tables[0].Rows[0]["DN_SKUNO"].ToString();
                    DnQty = dsDn.Tables[0].Rows[0]["DN_QTY"].ToString();
                }
                string StrSqlDetele = $@" delete R_BRCM_DATA_HEAD where Filetype='WIP' and FileName='{FileName}'";
                oleExec.ExecSQL(StrSqlDetele);

                string Data_Head = $@"INSERT INTO R_BRCM_DATA_HEAD (ID,DNNO,FILENAME,FILETYPE,CREATETIME,FLAG)
					VALUES('{DATA_ID}','{DNNO}','{FileName}','WIP',sysdate,0)";
                oleExec.ExecSQL(Data_Head);


            }
            /// <summary>
            /// 創建SHIP,P2SHPCFM,WIP,ASBUILT文件
            /// </summary>
            /// <param name="DNNO"></param>
            /// <returns></returns>
            public bool CreateFile(string DNNO)
            {
                MESInterface.Common.FileHelp fileHelp = new MESInterface.Common.FileHelp();
                List<FileAttr> FileAttrList = new List<FileAttr>();
                try
                {
                    SFCDB.ThrowSqlExeception = true;
                    SFCDB.BeginTrain();
                    FileAttrList.Add(GetFile_CustData("SHIP", DNNO));
                    FileAttrList.Add(GetFile_CustData("WIP", DNNO));
                    FileAttrList.Add(GetFile_CustData("P2SHPCFM", DNNO));
                   

                    foreach (var item in FileAttrList)
                    {
                        fileHelp.CreateFile(FolderName, item.FileName, item.StrFileData);

                        //把生成的文件上传至FTP  2017年11月15日15:25:18 by Taylor
                        string sFileDstPath = $@"{FolderName}/{ item.FileName}";        //需要上传的文件的路径
                        string FTP_FolderName = "/VN/ASN/"; //FSJ
                        //string FTP_FolderName = "FTX\\ASN"; //HST
                        string ftpServerIP = "ftp://10.132.48.74";
                        string ftpUserName = "Broadcom_SFC";
                        //string ftpUserName = "Broadcom_SFC_Test";
                        string ftpPwd = "Broadcom!";
                        //string ftpPwd = "test";
                        FTPHelp ftpHelp = new FTPHelp($@"{ftpServerIP}{FTP_FolderName}", ftpUserName, ftpPwd);

                        ftpHelp.Upload(sFileDstPath);



                    }
                    //File生成完畢則更新DN狀態:
                    UpdateStatus(DNNO);

                    UpdateSendStatus(DNNO);
                    SFCDB.CommitTrain();
                }
                catch (Exception e)
                {
                    SFCDB.RollbackTrain();
                    fileHelp.WriteContentToLogTxt("DN:" + DNNO + "生成ASN文件錯誤,錯誤信息:" + e.Message);
                    RollBackCreateFile(FileAttrList);
                }
                return true;
            }

            void RollBackCreateFile(List<FileAttr> FileAttrList)
            {
                MESInterface.Common.FileHelp fileHelp = new MESInterface.Common.FileHelp();
                foreach (var item in FileAttrList)
                {
                    try
                    {
                        fileHelp.DeleteFile(FolderName + "\\" + item.FileName);
                    }
                    catch (Exception e)
                    {
                        fileHelp.WriteContentToLogTxt("回退操作=>文件:" + item.FileName + "刪除失敗,錯誤信息:" + e.Message);
                    }
                }

            }
        }

        class FileAttr
        {
            public string FileName;
            public string StrFileData;
     
        }
    }
}
