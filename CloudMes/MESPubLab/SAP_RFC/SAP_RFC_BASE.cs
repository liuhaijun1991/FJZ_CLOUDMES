using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAP.Middleware.Connector;
using System.Threading.Tasks;
using MESDBHelper;
using System.Configuration;

namespace MESPubLab.SAP_RFC
{
    public class SAP_RFC_BASE
    {
        protected RfcConfigParameters RfcPara = new RfcConfigParameters();
        protected RfcDestination RfcDest;
        protected IRfcFunction Fun;
        protected DataTable RFCMDT;
        protected RfcRepository RfcRep;
        bool debug = ConfigurationManager.AppSettings["sapdebug"].ToUpper().Equals("TRUE")?true:false;

        protected string RFC_NAME;

        static int ConnCount = 0;

        protected Dictionary<string, DataTable> _Tables = new Dictionary<string, DataTable>();

        public bool CallRFCShowFrom = false;
        /// <summary>
        /// 獲取當前的RFC名稱
        /// </summary>
        public string RFCName
        {
            get
            {
                return RFC_NAME;
            }
        }
        /// <summary>
        /// 獲取當前RFC的輸入輸出參數

        public SapParameterObj getSapParameobj() {
            return new SapParameterObj()
            {
                LOGKEY = (Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmssfff")) + new Random().Next(1000)).ToString(),
                INOUT = Newtonsoft.Json.JsonConvert.SerializeObject(new object[] { this.PARAS, _Tables }, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }),
                CREATETIME = DateTime.Now
            };
        }

        /// </summary>
        public DataTable PARAS
        {
            get
            {
                return RFCMDT;
            }
        }
        /// <summary>
        /// 初始化RFC對象,連接參數從配置文件中取得
        /// </summary>
        public SAP_RFC_BASE(string BU)
        {
            RfcPara.Clear();
            RfcPara = new RfcConfigParameters();
            //從App.Config中取得BU對應的SAP連接方式
            if (!debug)
            {
                var LOGINTYPE = System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LOGINTYPE"];

                if (LOGINTYPE != null && LOGINTYPE == "SERVERGROUP")
                {
                    RfcPara.Add(RfcConfigParameters.MessageServerHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.SystemNumber, 
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_System"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language, "EN");
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost, "10.62.205.75");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);
                    RfcPara.Add(RfcConfigParameters.LogonGroup,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);
                    RfcPara.Add(RfcConfigParameters.SystemID, System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemID"]);
                }
                else if (BU.Substring(0, 3) == "MBD")
                {
                    //SystemNumber=10
                    //RfcPara.Add(RfcConfigParameters.SystemNumber,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_SystemNumber"]);
                    //SystemID=CNP:SAP實例ID
                    RfcPara.Add(RfcConfigParameters.SystemID,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_System"]);
                    //System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_System"]);
                    //GROUPNAME=CNSBG_800
                    RfcPara.Add(RfcConfigParameters.LogonGroup,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);
                    //Client=800
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    //LANGUAGE=EN
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    //USERID=NSGBG
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    //PASSWORD=MESEDICU
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    //正式庫MessageServer = 10.134.108.111
                    //RfcPara.Add(RfcConfigParameters.GatewayHost,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.MessageServerHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_MessageServerHost"]);
                    //測式庫AppServerHost = 10.134.108.152
                    //RfcPara.Add(RfcConfigParameters.AppServerHost,
                    //     System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_AppServerHost"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                }
                else if (BU.Equals("BPD"))
                {

                    RfcPara.Add(RfcConfigParameters.SystemNumber,
                    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemNumber"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.MessageServerHost,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.GatewayHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);

                }
                else if (BU.Equals("DCN"))
                {
                    RfcPara.Add(RfcConfigParameters.SystemNumber,
                    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemNumber"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    RfcPara.Add(RfcConfigParameters.GatewayHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);
                    RfcPara.Add(RfcConfigParameters.LogonGroup,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);

                }
                else if (BU.Equals("VERTIV") || BU.Equals("HWD"))
                {
                    RfcPara.Add(RfcConfigParameters.SystemNumber, //"01");
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemNumber"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.MessageServerHost,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.GatewayHost,
                        "10.134.108.122");
                    RfcPara.Add(RfcConfigParameters.LogonGroup,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);
                }
                else if ( BU.Equals("FJZTESTJNP") || BU.Equals("FJZ"))
                {
                    RfcPara.Add(RfcConfigParameters.AppServerHost, System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_AppServerHost"]);
                    //RfcPara.Add(RfcConfigParameters.MessageServerHost,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.SystemNumber,
                    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemNumber"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);
                    //RfcPara.Add(RfcConfigParameters.LogonGroup,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);
                }
                else if (BU.Equals("FJZJNP"))
                {
                    //RfcPara.Add(RfcConfigParameters.AppServerHost, "10.62.205.75");
                    RfcPara.Add(RfcConfigParameters.MessageServerHost,"10.62.205.75");
                    RfcPara.Add(RfcConfigParameters.SystemNumber,"00"  );
                    RfcPara.Add(RfcConfigParameters.User,"NSGBG");
                    RfcPara.Add(RfcConfigParameters.Password, "12345678");
                    RfcPara.Add(RfcConfigParameters.Client,"878");
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,"EN");
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost, "10.62.205.75");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);
                    RfcPara.Add(RfcConfigParameters.LogonGroup,"CNSBG_868");
                    RfcPara.Add(RfcConfigParameters.SystemID,"KPD");
                }
                else if (BU.Equals("FJZJNPTEST")) //TO TEST FJZ SAP BY DAN
                {
                    RfcPara.Add(RfcConfigParameters.AppServerHost, "10.62.205.85");
                    //RfcPara.Add(RfcConfigParameters.MessageServerHost,"10.62.205.85");
                    RfcPara.Add(RfcConfigParameters.SystemNumber, "00");
                    RfcPara.Add(RfcConfigParameters.User, "NSGBG");
                    RfcPara.Add(RfcConfigParameters.Password, "12345678");
                    RfcPara.Add(RfcConfigParameters.Client, "878");
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language, "EN");
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost, "10.62.205.75");
                    //RfcPara.Add(RfcConfigParameters.GatewayHost,
                    //    System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);
                    //RfcPara.Add(RfcConfigParameters.LogonGroup,"");
                    RfcPara.Add(RfcConfigParameters.SystemID, "KQA");
                }
                else if (BU.Equals("FNNTEST")) //TO TEST FNN SAP BY EDEN
                {

                    RfcPara.Add(RfcConfigParameters.AppServerHost,
                        "10.134.108.152");
                    RfcPara.Add(RfcConfigParameters.SystemNumber, "00");
                    RfcPara.Add(RfcConfigParameters.User, "NSGBG");
                    //"NSGBG");
                    RfcPara.Add(RfcConfigParameters.Password,
                        "MESEDICU");
                    RfcPara.Add(RfcConfigParameters.Client,
                        "800");
                    RfcPara.Add(RfcConfigParameters.Language,
                        "EN");
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.MaxPoolSize, "10");
                }
                else
                {
                    RfcPara.Add(RfcConfigParameters.SystemNumber, //"01");
                         System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_SystemNumber"]);
                    RfcPara.Add(RfcConfigParameters.User,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_User"]);
                    RfcPara.Add(RfcConfigParameters.Password,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Password"]);
                    RfcPara.Add(RfcConfigParameters.Client,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Client"]);
                    RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                    RfcPara.Add(RfcConfigParameters.Language,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_Language"]);
                    RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                    RfcPara.Add(RfcConfigParameters.PeakConnectionsLimit, "10");
                    //RfcPara.Add(RfcConfigParameters.MessageServerHost,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.GatewayHost,
                        "10.134.108.122");
                    RfcPara.Add(RfcConfigParameters.LogonGroup,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_LogonGroup"]);
                }    
            }
            else
            {                

                RfcPara.Add(RfcConfigParameters.AppServerHost,
                    "10.134.108.152");
                RfcPara.Add(RfcConfigParameters.SystemNumber, "00");
                RfcPara.Add(RfcConfigParameters.User, "NSGBG");
                //"NSGBG");
                RfcPara.Add(RfcConfigParameters.Password,
                    "MESEDICU");
                RfcPara.Add(RfcConfigParameters.Client,
                    "800");
                RfcPara.Add(RfcConfigParameters.Language,
                    "EN");
                RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                RfcPara.Add(RfcConfigParameters.MaxPoolSize, "10");
            }
            //8300:代表繁體
            RfcPara.Add(RfcConfigParameters.Codepage, "8300");
            //初始化SAP連接對象RfcDest
            RfcDest = RfcDestinationManager.GetDestination(RfcPara);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RFC_Name"></param>
        public void SetRFC_NAME(string RFC_Name)
        {
            RFC_NAME = RFC_Name;
            try
            {
                //RfcDest.
                RfcRep = RfcDest.Repository;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            _Tables.Clear();
            Fun = RfcRep.CreateFunction(RFC_NAME);
            RFCMDT = new DataTable();
            RFCMDT.Columns.Add("NAME");
            RFCMDT.Columns.Add("TYPE");
            RFCMDT.Columns.Add("VALUE");
            RFCMDT.Columns.Add("Documentation");
            for (int i = 0; i < Fun.Metadata.ParameterCount; i++)
            {
                DataRow dr = RFCMDT.NewRow();
                dr["NAME"] = Fun.Metadata[i].Name;
                dr["TYPE"] = Fun.Metadata[i].DataType;
                dr["VALUE"] = Fun.Metadata[i].DefaultValue;
                dr["Documentation"] = Fun.Metadata[i].Documentation;
                RFCMDT.Rows.Add(dr);
                if (dr["TYPE"].ToString() == "TABLE")
                {

                    DataTable DT = new DataTable(dr["NAME"].ToString());
                    _Tables.Add(dr["NAME"].ToString(), DT);
                    IRfcTable rfctable = Fun.Metadata[i].ValueMetadataAsTableMetadata.CreateTable();
                    ChangeRfcTableToDataTable(rfctable, dr["NAME"].ToString());
                }
                if (dr["TYPE"].ToString() == "STRUCTURE")
                {
                    DataTable DT = new DataTable(dr["NAME"].ToString());
                    _Tables.Add(dr["NAME"].ToString(), DT);
                    IRfcStructure rfcstruct = Fun.Metadata[i].ValueMetadataAsStructureMetadata.CreateStructure();
                    ChangeRfcStructureToDataTable(rfcstruct, dr["NAME"].ToString());
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="KEY"></param>
        /// <param name="Value"></param>
        public void SetValue(string KEY, string Value)
        {
            for (int i = 0; i < RFCMDT.Rows.Count; i++)
            {
                if (RFCMDT.Rows[i]["NAME"].ToString() == KEY)
                {
                    RFCMDT.Rows[i]["VALUE"] = Value;
                    return;
                }
            }
            throw new Exception("沒有這個參數!");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="KEY"></param>
        /// <returns></returns>
        public string GetValue(string KEY)
        {
            for (int i = 0; i < RFCMDT.Rows.Count; i++)
            {
                if (RFCMDT.Rows[i]["NAME"].ToString() == KEY)
                {
                    return RFCMDT.Rows[i]["VALUE"].ToString();
                }
            }
            throw new Exception("沒有這個參數!");
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearValues()
        {
            foreach (DataRow dr in RFCMDT.Rows)
            {
                try
                {
                    dr["VALUE"] = "";
                }
                catch
                { }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CallRFC(Action action=null)
        {
            if (!CallRFCShowFrom)
            {
                foreach (DataRow dr in RFCMDT.Rows)
                {
                     if (dr["TYPE"].ToString() != "TABLE" && dr["VALUE"].ToString() != "")
                    {
                        if (dr["TYPE"].ToString() == "DATE")
                        {
                            if (dr["VALUE"].ToString().Trim() != "0000-00-00")
                            {
                                Fun.SetValue(dr["NAME"].ToString(), DateTime.Parse(dr["VALUE"].ToString()));
                            }
                        }
                        else
                        {
                            Fun.SetValue(dr["NAME"].ToString(), dr["VALUE"].ToString());
                        }
                    }
                    else
                    {
                        if (dr["TYPE"].ToString() == "TABLE")
                        {
                            IRfcTable _RFCTable = Fun.Metadata[dr["NAME"].ToString()].ValueMetadataAsTableMetadata.CreateTable();
                            DataTable dt = _Tables[dr["NAME"].ToString()];
                            foreach (DataRow tdr in dt.Rows)
                            {
                                _RFCTable.Append();
                                foreach (DataColumn tdc in dt.Columns)
                                {
                                    if (_RFCTable.Metadata.LineType[tdc.ColumnName].ToString() == "DATE")
                                    {
                                        _RFCTable.SetValue(tdc.ColumnName, DateTime.Parse(tdr[tdc.ColumnName].ToString()));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            _RFCTable.SetValue(tdc.ColumnName, tdr[tdc.ColumnName].ToString());
                                        }
                                        catch(Exception ex) 
                                        {
                                            var t = ex.Message;
                                        }
                                    }
                                }
                            }
                            Fun.SetValue(dr["NAME"].ToString(), _RFCTable);
                        }
                        #region TYPE = STRUCTURE
                        if (dr["TYPE"].ToString() == "STRUCTURE")
                        {
                            IRfcStructure _RFCStruct = Fun.Metadata[dr["NAME"].ToString()].ValueMetadataAsStructureMetadata.CreateStructure();
                            DataTable dt = _Tables[dr["NAME"].ToString()];
                            foreach (DataRow tdr in dt.Rows)
                            {
                                foreach (DataColumn tdc in dt.Columns)
                                {
                                    if (_RFCStruct.Metadata[tdc.ColumnName].DataType.ToString() == "DATE")
                                    {
                                        _RFCStruct.SetValue(tdc.ColumnName, DateTime.Parse(tdr[tdc.ColumnName].ToString()));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            _RFCStruct.SetValue(tdc.ColumnName, tdr[tdc.ColumnName].ToString());
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                            Fun.SetValue(dr["NAME"].ToString(), _RFCStruct);
                        }
                        #endregion
                    }
                }
                Fun.Invoke(RfcDest);
                foreach (DataRow dr in RFCMDT.Rows)
                {
                    if (dr["TYPE"].ToString() == "TABLE")
                    {
                        //dr["VALUE"] = Fun[dr["NAME"].ToString()].ToString();
                        dr["VALUE"] = Fun[dr["NAME"].ToString()].ToString();
                        IRfcTable t = Fun[dr["NAME"].ToString()].GetTable();
                        ChangeRfcTableToDataTable(t, dr["NAME"].ToString());
                        //t.Metadata.

                    }
                    else if (dr["TYPE"].ToString() == "STRUCTURE")
                    {
                        dr["VALUE"] = Fun[dr["NAME"].ToString()].ToString();
                        IRfcStructure s = Fun[dr["NAME"].ToString()].GetStructure();
                        ChangeRfcStructureToDataTable(s, dr["NAME"].ToString());
                    }
                    else
                    {
                        try
                        {
                            dr["VALUE"] = Fun[dr["NAME"].ToString()].GetString();
                        }
                        catch (Exception ee)
                        {
                            dr["VALUE"] = ee.Message;
                        }
                        // dr["VALUE"] = Fun[dr["NAME"].ToString()].ToString();
                    }
                }
            }
            else
            {
                //this.CallRFCShowFrom = false;
                ////frmRFC_Moniter f = new frmRFC_Moniter(this);
                ////f.CallOnly(true);
                ////DialogResult dl = f.ShowDialog();
                //this.CallRFCShowFrom = true;
                //if (dl == DialogResult.OK)
                //{

                //}
                //else
                //{
                //    throw new Exception("用戶取消拋帳!");
                //}

            }
            action?.Invoke();
        }

        DataTable ChangeRfcStructureToDataTable(IRfcStructure rs, string name)
        {
            int colCount = rs.Metadata.FieldCount;
            DataTable dt = _Tables[name];
            dt.Clear();
            dt.Columns.Clear();
            for (int i = 0; i < colCount; i++)
            {
                dt.Columns.Add(rs.Metadata[i].Name);
            }

            DataRow dr = dt.NewRow();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                try
                {
                    dr[dt.Columns[j].ColumnName] = rs[dt.Columns[j].ColumnName].GetString();
                }
                catch
                {
                    dr[dt.Columns[j].ColumnName] = rs[dt.Columns[j].ColumnName].GetString();
                }
            }
            dt.Rows.Add(dr);

            return dt;
        }


        DataTable ChangeRfcTableToDataTable(IRfcTable rt, string name)
        {
            //System.Windows.Forms.MessageBox.Show(rt[0].Metadata.ContainerType.ToString());
            //;
            int colCount = rt.Metadata.LineType.FieldCount;
            DataTable dt = _Tables[name];
            dt.Clear();
            dt.Columns.Clear();
            for (int i = 0; i < colCount; i++)
            {
                dt.Columns.Add(rt.Metadata[i].Name);
            }

            int rowCount = rt.RowCount;
            for (int i = 0; i < rowCount; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    try
                    {
                        dr[dt.Columns[j].ColumnName] = rt[i][dt.Columns[j].ColumnName].GetString();
                    }
                    catch
                    {
                        dr[dt.Columns[j].ColumnName] = rt[i][dt.Columns[j].ColumnName].GetString();
                    }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ValueName"></param>
        /// <returns></returns>
        public DataTable GetTableValue(string ValueName)
        {
            return _Tables[ValueName];
        }     

    }

    public class SapParameterObj
    {
        public string LOGKEY { get; set; }
        public string INOUT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}
