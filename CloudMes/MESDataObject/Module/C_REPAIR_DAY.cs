using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_DAY : DataObjectTable
    {
        public T_C_REPAIR_DAY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_DAY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_DAY);
            TableName = "C_REPAIR_DAY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_REPAIR_DAY GetDetailBySkuno(OleExec sfcdb, string skuno)
        {
            if (string.IsNullOrEmpty(skuno))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKUNO" }));
            }
            DataTable dt = null;
            Row_C_REPAIR_DAY row_c_repair_day = null;
            string sql = $@"select * from {TableName} where skuno='{skuno.Replace("'","''")}' ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 1)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000086"));
                    }
                    row_c_repair_day = (Row_C_REPAIR_DAY) this.NewRow();
                    row_c_repair_day.loadData(dt.Rows[0]);
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
                
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return row_c_repair_day.GetDataObject();
        }

        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void InsertDB(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
        {
            string SN = DicStation["SN"].ToString();
            string PCBASN = DicStation["PCBASN"].ToString();
            string Location = DicStation["Location"].ToString();
            string NewTrackNo = DicStation["NewTrackNo"].ToString();
            string TrackNo = DicStation["TrackNo"].ToString();
            string ActionCode = DicStation["ActionCode"].ToString();
            string RootCause = DicStation["RootCause"].ToString();
            string TR_SN = DicStation["TR_SN"].ToString();
            string Component = DicStation["Component"].ToString();
            string MFR_Name = DicStation["MFR_Name"].ToString();
            string VendName = DicStation["VendName"].ToString();
            string Date_Code = DicStation["Date_Code"].ToString();
            string Lot_Code = DicStation["Lot_Code"].ToString();
            string PartDes = DicStation["PartDes"].ToString();
            string ID = DicStation["ID"].ToString();
            string Process = DicStation["Process"].ToString();
            string Description = DicStation["Description"].ToString();
            string SolderNo = DicStation["SolderNo"].ToString();
            string PackType = DicStation["PackType"].ToString();
            string NEW_KP = DicStation["NEW_KP"].ToString();
            string Old_KP = DicStation["Old_KP"].ToString();
            string ReplaceNo = DicStation["ReplaceNo"].ToString();
            string FailSysPtom = DicStation["FailSysPtom"].ToString();
            string LocationChkbox = DicStation["LocationChkbox"];
            string FailCodeID = DicStation["FailCodeID"];
            string BU = DicStation["BU"];
            string EMP = DicStation["EMP"];
            //string FailDescription = DicStation["FAILDESCRIPTION"];
            //string STATUS = DicStation["STATUS"];
            int StrSolderNo = Convert.ToInt32(DicStation["SolderNo"].ToString());

            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];
            T_R_REPAIR_ACTION TRRepairAction = new T_R_REPAIR_ACTION(DB, DB_TYPE_ENUM.Oracle);
            T_C_SECOND_USER CSeconDaryUser = new T_C_SECOND_USER(DB, DB_TYPE_ENUM.Oracle);
            List<string> ListString = new List<string>();
            T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(DB, DB_TYPE_ENUM.Oracle);
            //List<R_TESTRECORD> ListRTestRecordListStus = RTestRecord.GetListBYSNNOStationFListStatus(SN, "XRAY", ListString, DB);
            T_C_ACTION_CODE CActionCode = new T_C_ACTION_CODE(DB, DB_TYPE_ENUM.Oracle);
            C_ACTION_CODE ActionCodeInFo = CActionCode.GetByActionCode(ActionCode, DB);
            T_R_REPAIR_MAIN RRepairMain = new T_R_REPAIR_MAIN(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN RSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN_KP RSNKP = new T_R_SN_KP(DB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> ListRSNKPINFO = RSNKP.GetKPListBYSN(SN, 1, DB);
            R_SN RSNINFO = RSN.LoadSN(SN, DB);
            if (ActionCodeInFo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181107085152", new string[] { ActionCode }));
            }
            T_C_ERROR_CODE CErrorCode = new T_C_ERROR_CODE(DB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE ErrorCode = CErrorCode.GetByErrorCode(RootCause, DB);
            if (ErrorCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181106082033", new string[] { RootCause }));
            }
            if (EMP == "" || EMP == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181107085232", new string[] { EMP }));
            }
            if (RootCause == "E987" || RootCause == "E975")
            {
                ListString.Add("E987");
                ListString.Add("E975");
                List<R_REPAIR_ACTION> ListRRrpairAction = TRRepairAction.GetSNAction(SN, ListString, DB);
                if (ListRRrpairAction.Count > 0)
                {
                    List<C_SECOND_USER> ListCSecondUser = CSeconDaryUser.GetEmpStationItemSelect(EMP, "TE", DB);
                    if (ListCSecondUser.Count < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181107091030", new string[] { }));
                    }
                }
            }
            T_R_REPAIR_TRANSFER RRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> ListRRepairTransfer = RRepairTransfer.GetSNCheckInOut(SN, "0", DB);
            if (ListRRepairTransfer.Count < 0)
            {
                ListString.Clear();
                ListString.Add("AI");
                ListString.Add("AOI2");
                ListString.Add("AOI4");
                ListString.Add("VI");
                ListString.Add("5DX");
                ListString.Add("FQC");
                ListString.Add("XRAY");
                List<R_REPAIR_MAIN> ListRRepairMain = RRepairMain.GetSNBySNFailStation(SN, ListString, DB);
                if (ListRRepairMain.Count < 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105111047", new string[] { SN }));
                }
            }
            if (string.IsNullOrEmpty(Location) && (ActionCode != "A20" || ActionCode != "A21"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181106083739", new string[] { SN }));
            }
            for (int i = 0; i < Location.Length; i++)
            {
                ListString.Clear();
                ListString.Add("~");
                ListString.Add("!");
                ListString.Add("@");
                ListString.Add("#");
                ListString.Add("%");
                ListString.Add("^");
                ListString.Add("&");
                ListString.Add("*");
                ListString.Add("(");
                ListString.Add(")");
                ListString.Add("-");
                ListString.Add("=");
                ListString.Add("+");
                ListString.Add("]");
                ListString.Add("[");
                ListString.Add("{");
                ListString.Add("}");
                ListString.Add(":");
                ListString.Add(".");
                ListString.Add(",");
                ListString.Add("/");
                ListString.Add("?");
                ListString.Add(">");
                ListString.Add("<");
                if (ListString.Contains(Location.Substring(i, 1)))
                {
                    T_R_REPAIR_DATA RRepairData = new T_R_REPAIR_DATA(DB, DB_TYPE_ENUM.Oracle);
                    R_REPAIR_DATA RRepairDataINFO = new R_REPAIR_DATA();
                    RRepairDataINFO.SN = SN;
                    RRepairDataINFO.LOCATION = Location;
                    RRepairDataINFO.EDIT_EMP = EMP;
                    RRepairDataINFO.EDIT_TIME = RRepairData.GetDBDateTime(DB);
                    int RepairDateNum = RRepairData.InRepairSNLocation(RRepairDataINFO, DB);
                    if (RepairDateNum <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                    }
                }
            }
            ListString.Clear();
            ListString.Add("E516");
            ListString.Add("E517");
            ListString.Add("E518");
            ListString.Add("E519");
            ListString.Add("E520");
            ListString.Add("E521");
            ListString.Add("E522");
            if (ListString.IndexOf(RootCause) > 0 && Location != "PCB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181107093515", new string[] { RootCause }));
            }
            //獲取周別
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            string weekOfYear = Convert.ToString(gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday));
            string ProfileName = "REPAIR COMPONENT ID-" + EMP;
            T_C_SEQNO CSEQNO = new T_C_SEQNO(DB, DB_TYPE_ENUM.Oracle);
            C_SEQNO CSEQNOINFO = new C_SEQNO();
            //string NO = CSEQNO.GetLotno(ProfileName, DB);
            C_SEQNO ListCSeqNo = CSEQNO.GetSEQNAME(ProfileName, weekOfYear, DB);
            if (ListCSeqNo == null && TrackNo != "")
            {
                string CSEQNOID = CSEQNO.GetNewID(BU, DB);
                CSEQNOINFO.ID = CSEQNOID;
                CSEQNOINFO.SEQ_NAME = ProfileName;
                CSEQNOINFO.SEQ_NO = EMP.Substring(EMP.Length - 2, 2) + weekOfYear + "001";
                CSEQNOINFO.EDIT_TIME = CSEQNO.GetDBDateTime(DB);
                CSEQNOINFO.USE_TIME = CSEQNO.GetDBDateTime(DB);
                CSEQNOINFO.DIGITS = 1;
                CSEQNOINFO.BASE_CODE = "";
                CSEQNOINFO.MINIMUM = "001";
                CSEQNOINFO.MAXIMUM = "999";
                CSEQNOINFO.PREFIX = "54";
                CSEQNOINFO.SEQ_FORM = "WK";
                CSEQNOINFO.RESET = "1";
                CSEQNOINFO.EDIT_EMP = EMP;
                int SEQNum = CSEQNO.InSeqName(CSEQNOINFO, DB);
                if (SEQNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                }
            }
            else if (TrackNo != "" && ListCSeqNo != null)
            {
                ListCSeqNo.SEQ_NO = TrackNo;
                CSEQNOINFO.EDIT_EMP = EMP;
                CSEQNOINFO.EDIT_TIME = CSEQNO.GetDBDateTime(DB);
                int SEQNum = CSEQNO.UpdateSeqName(CSEQNOINFO, ProfileName, DB);
                if (SEQNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                }
            }
            //R_REPAIR_ACTION RRepairActionInFo = RRepairAction.GetSNCerLocAct(SN, Location, ActionCode, EMP, ListRTestRecordListStus[0].TEST_TIME, DB);
            if (ReplaceNo.Length > 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190502181659", new string[] { "ReplaceNo:" + ReplaceNo }));
            }
            Row_R_REPAIR_FAILCODE FailCodeRow = RepairFailcode.GetByFailCodeID(FailCodeID, DB);
            R_REPAIR_ACTION RRepairActionInFo = TRRepairAction.GetSNCerLocAct(SN, Location, ActionCode, EMP, FailCodeRow.FAIL_TIME, DB);
            if (RRepairActionInFo == null)
            {
                R_REPAIR_ACTION RRepairAction = new R_REPAIR_ACTION();
                string RRepairActionID = TRRepairAction.GetNewID(BU, DB);
                RRepairAction.ID = RRepairActionID;
                RRepairAction.R_FAILCODE_ID = "";
                RRepairAction.SN = SN;
                RRepairAction.ACTION_CODE = ActionCode;
                RRepairAction.SECTION_ID = "";
                RRepairAction.PROCESS = Process;
                RRepairAction.ITEMS_ID = ID;
                RRepairAction.ITEMS_SON_ID = "";
                RRepairAction.REASON_CODE = "";
                RRepairAction.DESCRIPTION = Description;
                RRepairAction.FAIL_LOCATION = Location;
                RRepairAction.FAIL_CODE = RootCause;
                RRepairAction.KEYPART_SN = Old_KP;
                RRepairAction.NEW_KEYPART_SN = NEW_KP;
                RRepairAction.KP_NO = Component;
                RRepairAction.TR_SN = TR_SN;
                RRepairAction.MFR_CODE = MFR_Name;
                RRepairAction.MFR_NAME = VendName;
                RRepairAction.DATE_CODE = Date_Code;
                RRepairAction.LOT_CODE = Lot_Code;
                RRepairAction.NEW_KP_NO = "";
                RRepairAction.NEW_TR_SN = "";
                RRepairAction.NEW_MFR_CODE = "";
                RRepairAction.NEW_MFR_NAME = "";
                RRepairAction.NEW_DATE_CODE = Date_Code;
                RRepairAction.NEW_LOT_CODE = Lot_Code;
                RRepairAction.REPAIR_EMP = EMP;
                RRepairAction.REPAIR_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairAction.EDIT_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairAction.EDIT_EMP = EMP;
                //RRepairActionInFo.FAIL_TIME = ListRTestRecordListStus[0].TEST_TIME;
                RRepairAction.FAIL_TIME = FailCodeRow.FAIL_TIME;
                RRepairAction.R_START_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairAction.R_FINISH_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairAction.FIXED_FLAG = "1";
                RRepairAction.REPLACED_FLAG = ReplaceNo;
                RRepairAction.SOLUTION = TrackNo;
                RRepairAction.SOLDER = "0";
                RRepairAction.PACKAGE_TYPE = PackType;
                RRepairAction.PART_DESC = PartDes;
                int RepairINSNNum = TRRepairAction.InsertRepairSNInFo(RRepairAction, DB);
                if (RepairINSNNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                }
            }
            else
            {
                RRepairActionInFo.DESCRIPTION = Description;
                RRepairActionInFo.FIXED_FLAG = "1";
                RRepairActionInFo.SOLDER = "0";
                RRepairActionInFo.SOLUTION = TrackNo;
                RRepairActionInFo.EDIT_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairActionInFo.EDIT_EMP = EMP;
                RRepairActionInFo.R_START_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairActionInFo.R_FINISH_TIME = TRRepairAction.GetDBDateTime(DB);
                RRepairActionInFo.PROCESS = Process;
                RRepairActionInFo.FAIL_LOCATION = Location;
                RRepairActionInFo.FAIL_CODE = RootCause;
                RRepairActionInFo.REPLACED_FLAG = ReplaceNo;
                RRepairActionInFo.KP_NO = Component;
                RRepairActionInFo.MFR_CODE = MFR_Name;
                RRepairActionInFo.MFR_NAME = VendName;
                RRepairActionInFo.LOT_CODE = Lot_Code;
                RRepairActionInFo.DATE_CODE = Date_Code;
                RRepairActionInFo.SOLDER = "0";
                RRepairActionInFo.PACKAGE_TYPE = PackType;
                RRepairActionInFo.PART_DESC = PartDes;
                int UpdateNum = TRRepairAction.UpdateRepairSNInFo(RRepairActionInFo, DB);
                if (UpdateNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                }
            }
            //此处的R_SN表更新放在CheckOUT中更新
            //R_SN RSNInFo = RSN.GetSNBySN(SN, DB);
            //RSNInFo.REPAIR_FAILED_FLAG = "0";
            //RSNInFo.EDIT_TIME = RSN.GetDBDateTime(DB);
            //RSNInFo.EDIT_EMP = EMP;
            //int RepairUpdateSNNum = RSN.UpdateRepairSNInFo(RSNInFo, SN, DB);
            //if (RepairUpdateSNNum < 0)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { ProfileName }));
            //}
            R_REPAIR_MAIN RRepairMainINFO = RRepairMain.GetSNBySN(SN, DB);
            T_R_REPAIR_FAILCODE RRepairFailCode = new T_R_REPAIR_FAILCODE(DB, DB_TYPE_ENUM.Oracle);
            R_REPAIR_FAILCODE RRepairFailCodeInFo = RRepairFailCode.GetRepairSN(SN, DB);
            if (RRepairMainINFO.FAIL_STATION != "AOI2" && RRepairMainINFO.FAIL_STATION != "AOI4" && RRepairMainINFO.FAIL_STATION != "VI")
            {
                T_R_REPAIR_COUNT RRepairCount = new T_R_REPAIR_COUNT(DB, DB_TYPE_ENUM.Oracle);
                R_REPAIR_COUNT RRepairCountFailINFO = RRepairCount.GetSNFailStationCodeBYCount(SN, RRepairMainINFO.FAIL_STATION, RRepairFailCodeInFo.FAIL_CODE, DB);
                if (RRepairCountFailINFO != null)
                {
                    RRepairCountFailINFO.REPAIR_COUNT = RRepairCountFailINFO.REPAIR_COUNT + 1;
                    RRepairCountFailINFO.EDIT_EMP = EMP;
                    RRepairCountFailINFO.REPAIR_CONTROL_EMP = EMP;
                    RRepairCountFailINFO.EDIT_TIME = RRepairCount.GetDBDateTime(DB);
                    RRepairCountFailINFO.REPAIR_CONTROL_TIME = RRepairCount.GetDBDateTime(DB);
                    int CountNum = RRepairCount.UpdateFailSNCountINFO(RRepairCountFailINFO, SN, RRepairMainINFO.FAIL_STATION, RRepairFailCodeInFo.FAIL_CODE, DB);
                    if (CountNum <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                    }
                }
                else
                {
                    List<R_REPAIR_MAIN> ListRRepairMain = RRepairMain.GetSNMainFailCodeCount(SN, RRepairMainINFO.FAIL_STATION, "1", RRepairFailCodeInFo.FAIL_CODE, DB);
                    if (ListRRepairMain.Count > 0)
                    {

                        R_REPAIR_COUNT INRRepairCountINFO = new R_REPAIR_COUNT();
                        string COUNTID = RRepairCount.GetNewID(BU, DB);
                        INRRepairCountINFO.ID = COUNTID;
                        INRRepairCountINFO.SN_ID = RSNINFO.ID;
                        INRRepairCountINFO.SN = SN;
                        INRRepairCountINFO.FAIL_STATION = RRepairMainINFO.FAIL_STATION;
                        INRRepairCountINFO.FAIL_CODE = RRepairFailCodeInFo.FAIL_CODE;
                        INRRepairCountINFO.REPAIR_COUNT = ListRRepairMain.Count;
                        INRRepairCountINFO.REPAIR_CONTROL_NUM = 0;
                        INRRepairCountINFO.REPAIR_CONTROL_EMP = EMP;
                        INRRepairCountINFO.REPAIR_CONTROL_TIME = Convert.ToDateTime("1900 - 01 - 01 00:00:00");
                        INRRepairCountINFO.EDIT_EMP = EMP;
                        INRRepairCountINFO.EDIT_TIME = RRepairCount.GetDBDateTime(DB); ;
                        int RepairCountNum = RRepairCount.INFailSNCountINFO(INRRepairCountINFO, DB);
                        if (RepairCountNum <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                        }
                    }
                }
            }
            //sfcrepairpcbalink
            if (PCBASN != "" && PCBASN.Length > 0)
            {
                T_R_REPAIR_PCBA_LINK RRepairPCBALink = new T_R_REPAIR_PCBA_LINK(DB, DB_TYPE_ENUM.Oracle);
                R_REPAIR_PCBA_LINK RRepairPCBALinkINFO = new R_REPAIR_PCBA_LINK();
                string PCBALINKID = RRepairPCBALink.GetNewID(BU, DB);
                RRepairPCBALinkINFO.ID = PCBALINKID;
                RRepairPCBALinkINFO.SN_ID = RSNINFO.ID;
                RRepairPCBALinkINFO.SN = SN;
                RRepairPCBALinkINFO.KP_SN = PCBASN;
                RRepairPCBALinkINFO.CREATE_DATE = RRepairPCBALink.GetDBDateTime(DB);
                RRepairPCBALinkINFO.PROCESS = Process;
                RRepairPCBALinkINFO.LOCATION = Location;
                RRepairPCBALinkINFO.ROOT_CAUSE = RootCause;
                RRepairPCBALinkINFO.DATA_CODE = Date_Code;
                RRepairPCBALinkINFO.LOT_CODE = Lot_Code;
                RRepairPCBALinkINFO.COMPONENT_CODE = Component;
                RRepairPCBALinkINFO.VENDOR_CODE = VendName;
                RRepairPCBALinkINFO.DESCRIPTION = Description;
                RRepairPCBALinkINFO.REPAIR_EMP = EMP;
                RRepairPCBALinkINFO.EDIT_EMP = EMP;
                RRepairPCBALinkINFO.EDIT_TIME = RRepairPCBALink.GetDBDateTime(DB);
                int RRepairPCBALinkNum = RRepairPCBALink.INRepairPCBALink(RRepairPCBALinkINFO, DB);
                if (RRepairPCBALinkNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { ProfileName }));
                }
            }
            //InsertRepairTable JS内容是将FailEventPoint: FailEventPoint,
            //CreateDate: createdate,ActionCode: actioncode,Process: process,Location: location,RootCause: rootcause, Component: component,
            //VendorName: vendorname,VernorCode: vendorcode,DateCode: datecode,LotCode: lotcode,Description: description,Solution: solution,
            //SolderNo: SolderNo,PackType: packtype,PartDes:partdes 这些内容形成TABLE展示到前台，暂时不写；后续有需要在添加。
            //GetNextID();NewEFox_Repair_SP GETNEXTID 通过一串的逻辑取到一个类似时间的一个返回参数但没有接收不知道干嘛用，未写。
            if (ActionCode == "A28")
            {
                //GetMaxSNSave(row.SSN);连接ALLPAIRT执行存储
                AP_DLL APDLL = new AP_DLL();
                string RES = APDLL.GetMaxSNSave(SN, DB);
                if (RES.Substring(0, 2) != "OK")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000263", new string[] { RES }));
                }
            }
        }
    }
    public class Row_C_REPAIR_DAY : DataObjectBase
    {
        public Row_C_REPAIR_DAY(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_DAY GetDataObject()
        {
            C_REPAIR_DAY DataObject = new C_REPAIR_DAY();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.REPAIR_DAY_COUNT = this.REPAIR_DAY_COUNT;
            DataObject.REPAIR_COUNT = this.REPAIR_COUNT;
            DataObject.STATION_COUNT = this.STATION_COUNT;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public double? REPAIR_DAY_COUNT
        {
            get
            {
                return (double?)this["REPAIR_DAY_COUNT"];
            }
            set
            {
                this["REPAIR_DAY_COUNT"] = value;
            }
        }
        public double? REPAIR_COUNT
        {
            get
            {
                return (double?)this["REPAIR_COUNT"];
            }
            set
            {
                this["REPAIR_COUNT"] = value;
            }
        }
        public double? STATION_COUNT
        {
            get
            {
                return (double?)this["STATION_COUNT"];
            }
            set
            {
                this["STATION_COUNT"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class C_REPAIR_DAY
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string VERSION{get;set;}
        public double? REPAIR_DAY_COUNT{get;set;}
        public double? REPAIR_COUNT{get;set;}
        public double? STATION_COUNT{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}