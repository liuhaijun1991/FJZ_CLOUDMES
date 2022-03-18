using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MESDBHelper;
using SAP.Middleware.Connector;

namespace MESStation.Interface.SAPRFC
{
    public class SAP_RFC_BASE
    {
        protected RfcConfigParameters RfcPara = new RfcConfigParameters();
        protected RfcDestination RfcDest;
        protected IRfcFunction Fun;
        protected DataTable RFCMDT;
        protected RfcRepository RfcRep;

        protected string RFC_NAME;
        public Dictionary<string, DataTable> ReturnDatatableByName = new Dictionary<string, DataTable>(); //add by LLF 2017-12-10
        public Dictionary<int, DataTable> ReturnDatatableByIndex = new Dictionary<int, DataTable>(); //add by LLF 2017-12-10

        static int ConnCount = 500000;

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
            if (true)
            {
                if (BU.Substring(0, 3) == "MBD")
                {
                    //SystemNumber=10
                    //RfcPara.Add(RfcConfigParameters.SystemNumber,
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU + "_SAP_SystemNumber"]);
                    //SystemID=CNP:SAP實例ID
                    RfcPara.Add(RfcConfigParameters.SystemID,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_System"]);
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
                    RfcPara.Add(RfcConfigParameters.MessageServerHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_MessageServerHost"]);
                    RfcPara.Add(RfcConfigParameters.GatewayHost,
                        System.Configuration.ConfigurationManager.AppSettings[BU + "_SAP_GateWayHost"]);


                }
                else
                {
                    //RfcPara.Add(RfcConfigParameters.AppServerHost, 
                    //    System.Configuration.ConfigurationSettings.AppSettings[BU+"_SAP_AppServerHost"]);
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
                }
            }
            else
            {
                //RfcPara.Add(RfcConfigParameters.AppServerHost,
                //     "10.134.108.152");
                //RfcPara.Add(RfcConfigParameters.SystemNumber, "00");
                //RfcPara.Add(RfcConfigParameters.User, "NSGBG");
                ////"NSGBG");
                //RfcPara.Add(RfcConfigParameters.Password,
                //    "MESEDICU");
                //RfcPara.Add(RfcConfigParameters.Client,
                //    "800");
                //RfcPara.Add(RfcConfigParameters.Language,
                //    "EN");
                //RfcPara.Add(RfcConfigParameters.Name, "CON" + (ConnCount++).ToString());
                //RfcPara.Add(RfcConfigParameters.PoolSize, "5");
                //RfcPara.Add(RfcConfigParameters.MaxPoolSize, "10");
            }
            //RfcConfigParameters.LogonGroup

            RfcPara.Add(RfcConfigParameters.Codepage, "8300");

            RfcDest = RfcDestinationManager.GetDestination(RfcPara);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RFC_Name"></param>
        public void SetRFC_NAME(string RFC_Name, string WO)
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
                    ChangeRfcTableToDataTable(rfctable, dr["NAME"].ToString(), WO);
                }

            }
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
            throw new Exception("There is no such parameter!");

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
            throw new Exception("There is no such parameter!");
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearValues()
        {
            foreach (DataRow dr in RFCMDT.Rows)
            {
                dr["VALUE"] = "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CallRFC()
        {
            int i = 0;
            if (!CallRFCShowFrom)
            {
                foreach (DataRow dr in RFCMDT.Rows)
                {
                    if (dr["TYPE"].ToString() != "TABLE" && dr["VALUE"].ToString() != "")
                    {
                        if (dr["TYPE"].ToString() == "DATE")
                        {
                            Fun.SetValue(dr["NAME"].ToString(), DateTime.Parse(dr["VALUE"].ToString()));
                            //Fun.SetValue(dr["NAME"].ToString(), dr["VALUE"].ToString());
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
                                        catch
                                        { }
                                    }
                                }
                            }
                            Fun.SetValue(dr["NAME"].ToString(), _RFCTable);
                        }
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
                        DataTable dt = ChangeRfcTableToDataTable(t, dr["NAME"].ToString(), "");
                        //t.Metadata.
                        ReturnDatatableByName.Add(dr["NAME"].ToString(), dt);
                        ReturnDatatableByIndex.Add(i, dt);
                        i = i + 1;
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
                    }
                }
            }
            else
            {
            }
        }

        DataTable ChangeRfcTableToDataTable(IRfcTable RFC_Table, string Name, string WO)
        {
            //System.Windows.Forms.MessageBox.Show(rt[0].Metadata.ContainerType.ToString());
            //;
            int colCount = RFC_Table.Metadata.LineType.FieldCount;
            DataTable dt = _Tables[Name];
            dt.Clear();
            dt.Columns.Clear();
            for (int i = 0; i < colCount; i++)
            {
                dt.Columns.Add(RFC_Table.Metadata[i].Name);
            }

            if (Name == "ITAB" && WO.Length > 0)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        dr[j] = WO;
                    }
                    else
                    {
                        dr[j] = "";
                    }
                }
                dt.Rows.Add(dr);
            }
            else
            {
                int rowCount = RFC_Table.RowCount;
                for (int i = 0; i < rowCount; i++)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        try
                        {
                            dr[dt.Columns[j].ColumnName] = RFC_Table[i][dt.Columns[j].ColumnName].GetString();
                        }
                        catch
                        {
                            dr[dt.Columns[j].ColumnName] = RFC_Table[i][dt.Columns[j].ColumnName].GetString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }

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
}

