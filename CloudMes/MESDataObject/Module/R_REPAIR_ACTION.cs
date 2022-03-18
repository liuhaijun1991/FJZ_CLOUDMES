using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_ACTION : DataObjectTable
    {
        public T_R_REPAIR_ACTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_ACTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_ACTION);
            TableName = "r_repair_action".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable SelectRepairActionBySN(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select re.sn,re.action_code,re.section_id,re.process,item.item_name,items.items_son,re.reason_code,re.description,re.fail_location,re.fail_code,
                                re.keypart_sn,re.new_keypart_sn,re.kp_no,re.tr_sn,re.mfr_code,re.mfr_name,re.date_code,re.lot_code,re.new_kp_no,
                                re.new_tr_sn,re.new_mfr_code,re.new_mfr_name,re.new_date_code,re.new_lot_code,re.repair_emp,re.edit_time,re.edit_emp
                                 from r_repair_action re,c_repair_items item,c_repair_items_son items  where 1=1 and re.items_id=item.id and re.items_son_id=items.id and sn='{sn}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;

        }

        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetBySNActionCode(string sn, OleExec DB)
        {
            bool res = false;
            string strSQL = $@"SELECT * FROM R_REPAIR_ACTION WHERE SN='{sn}' AND ACTION_CODE IN (SELECT CONTROL_VALUE FROM C_CONTROL WHERE CONTROL_NAME='CHECK_ICT_RETEST_CODE')";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            //int Num = DB.ExecSqlNoReturn(strSQL, null);
            //if (Num > 0)
            //{
            //    res = true;
            //}
            return res;
        }
        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckChar(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
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
            string FailSysPtom = DicStation["FailSysPtom"];
            string LocationChkbox = DicStation["LocationChkbox"];
            string FailCodeID = DicStation["FailCodeID"];
            string BU = DicStation["BU"];
            string EMP = DicStation["EMP"];
            int StrSolderNo = Convert.ToInt32(DicStation["SolderNo"].ToString());


            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];

            List<string> ListString = new List<string>();
            List<string> ListString1 = new List<string>();
            T_C_CONTROL CControl = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            if ((Location.Trim().Length > 0 && Date_Code.Trim().Length <= 0) || Lot_Code.Trim().Length <= 0 || Component.Trim().Length <= 0 || VendName.Trim().Length <= 0 || MFR_Name.Trim().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103090207", new string[] { }));
            }
            if (ActionCode == "A23")
            {
                // REPAIR+ACTIONCODEIN(NOT IN)+_+ERRORCODEIN(NOT IN)   PS:REPAIR_ACTIONCODEIN_ERRORCODEIN
                ListString = CControl.GetListByType("A23", "REPAIR_ACTIONCODEIN_ERRORCODEIN", DB);
                if (ListString.Contains(RootCause)/*RootCause == "E718"*/)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103090357", new string[] { }));
                }
                ListString.Clear();
            }
            //string RootCauseValue = "E202,E205,E206,E214,E215,E216,E221,E222,E223,E224,E408,E711,E712,E713,E714,E722,E739,E740,E804,E805,E808,E901,E915,E916,E917,E918,E920";
            if (ActionCode == "A12")
            {
                ListString = CControl.GetListByType("A12", "REPAIR_ACTIONCODEIN_ERRORCODEIN", DB);
                if (ListString.Contains(RootCause)/*!RootCauseValue.Contains(RootCause)*/)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
                }
                ListString.Clear();
            }
            if (ActionCode == "A30")
            {
                ListString = CControl.GetListByType("A30", "REPAIR_ACTIONCODEIN_ERRORCODENOTIN", DB);
                //string ACA = "E206,E304,E305";
                if (!ListString.Contains(RootCause))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
                }
                ListString.Clear();
        }
            //ListString.Add("E202");
            //ListString.Add("E205");
            //ListString.Add("E206");
            //ListString.Add("E214");
            //ListString.Add("E215");
            //ListString.Add("E216");
            //ListString.Add("E221");
            //ListString.Add("E222");
            //ListString.Add("E223");
            //ListString.Add("E224");
            //ListString.Add("E408");
            //ListString.Add("E711");
            //ListString.Add("E712");
            //ListString.Add("E713");
            //ListString.Add("E714");
            //ListString.Add("E722");
            //ListString.Add("E739");
            //ListString.Add("E740");
            //ListString.Add("E804");
            //ListString.Add("E805");
            //ListString.Add("E808");
            //ListString.Add("E901");
            //ListString.Add("E920");
            ListString = CControl.GetListByType("A12", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause) && ActionCode != "A12")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            //ListString.Add("E302");
            //ListString.Add("E304");
            //ListString.Add("E305");
            //ListString.Add("E307");
            ListString = CControl.GetListByType("A11", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause) && ActionCode != "A11")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            //ListString.Add("E906");
            //ListString.Add("NDF");
            //ListString.Add("NPF");
            //ListString.Add("E806");
            //ListString.Add("E807");
            //ListString.Add("E808");
            //ListString.Add("E809");
            //ListString.Add("E810");
            //ListString.Add("E811");
            //ListString.Add("E812");
            //ListString.Add("E813");
            //ListString.Add("E814");
            //ListString.Add("E741");
            //ListString.Add("E742");
            //ListString.Add("E743");
            //ListString.Add("E744");
            //ListString.Add("E976");
            //ListString.Add("E981");
            //ListString.Add("E982");
            //ListString.Add("E983");
            //ListString.Add("E984");
            //ListString.Add("E985");
            //ListString.Add("E986");
            //ListString.Add("E987");
            //ListString1.Add("A21");
            //ListString1.Add("A19");
            ListString = CControl.GetListByType("A21,A19", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (!ListString1.Contains(ActionCode) && ListString.Contains(RootCause))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString1.Clear();
            //ListString.Add("E906");
            //ListString.Add("NDF");
            //ListString.Add("NPF");
            //ListString.Add("E806");
            //ListString.Add("E807");
            //ListString.Add("E808");
            //ListString.Add("E809");
            //ListString.Add("E810");
            //ListString.Add("E811");
            //ListString.Add("E812");
            //ListString.Add("E813");
            //ListString.Add("E814");
            //ListString.Add("E741");
            //ListString.Add("E742");
            //ListString.Add("E743");
            //ListString.Add("E744");
            //ListString.Add("E904");
            //ListString.Add("E956");
            //ListString.Add("E957");
            //ListString.Add("E958");
            //ListString.Add("E955");
            //ListString.Add("E975");
            //ListString.Add("E976");
            //ListString.Add("E977");
            //ListString.Add("E981");
            //ListString.Add("E982");
            //ListString.Add("E983");
            //ListString.Add("E984");
            //ListString.Add("E985");
            //ListString.Add("E986");
            //ListString.Add("E987");
            //ListString1.Add("A21");
            //ListString1.Add("A19");
            ListString = CControl.GetListByType("A21,A19", "REPAIR_ACTIONCODEIN_ERRORCODENOTIN", DB);
            if (ListString1.Contains(ActionCode) && !ListString.Contains(RootCause))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
    }
            ListString.Clear();
            ListString = CControl.GetListByType("A13", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ActionCode != "A13")
            {
                if (RootCause == "E204")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103090357", new string[] { }));
                }
            }
            ListString.Clear();
            //ListString1.Clear();
            //ListString.Add("E973");
            //ListString.Add("E974");
            //ListString.Add("E975");
            //ListString.Add("E976");
            //ListString.Add("E977");
            //ListString.Add("E970");
            ListString = CControl.GetListByType("A21", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause) && ActionCode != "A21")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString = CControl.GetListByType("A21", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause)/*RootCause == "E971"*/ && ActionCode != "A24")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString = CControl.GetListByType("A23", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause)/*RootCause == "E972" */&& ActionCode != "A23")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString1.Clear();
            //ListString.Add("E516");
            //ListString.Add("E517");
            //ListString.Add("E518");
            //ListString.Add("E519");
            //ListString.Add("E520");
            //ListString.Add("E521");
            //ListString.Add("E522");
            //ListString.Add("E523");
            //ListString.Add("E907");
            //ListString.Add("E908");
            //ListString.Add("E909");
            //ListString.Add("E910");
            //ListString.Add("E911");
            //ListString.Add("E912");
            //ListString.Add("E913");
            //ListString.Add("E914");
            //ListString.Add("E903");
            //ListString.Add("E905");
            //ListString1.Add("A26");
            //ListString1.Add("A28");
            //ListString1.Add("A29");
            ListString = CControl.GetListByType("A23", "REPAIR_ACTIONCODEIN_ERRORCODENOTIN", DB);
            if (ListString1.Contains(ActionCode) && !ListString.Contains(RootCause))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString1.Clear();
            //ListString.Add("E516");
            //ListString.Add("E517");
            //ListString.Add("E518");
            //ListString.Add("E519");
            //ListString.Add("E520");
            //ListString.Add("E521");
            //ListString.Add("E522");
            //ListString.Add("E523");
            //ListString.Add("E907");
            //ListString.Add("E908");
            //ListString.Add("E909");
            //ListString.Add("E910");
            //ListString.Add("E911");
            //ListString.Add("E912");
            //ListString.Add("E913");
            //ListString.Add("E914");
            //ListString1.Add("A26");
            //ListString1.Add("A28");
            //ListString1.Add("A29");
            ListString = CControl.GetListByType("A23", "REPAIR_ACTIONCODENOTIN_ERRORCODEIN", DB);
            if (!ListString1.Contains(ActionCode) && ListString.Contains(RootCause))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103092423", new string[] { }));
            }
            ListString.Clear();
            ListString1.Clear();
            //ListString.Add("E516");
            //ListString.Add("E517");
            //ListString.Add("E518");
            //ListString.Add("E519");
            //ListString.Add("E520");
            //ListString.Add("E521");
            //ListString.Add("E522");
            ListString = CControl.GetListByType("A23", "REPAIR_LOCATIONNOTIN_ERRORCODEIN", DB);
            if (ListString.Contains(RootCause) && Location != "PCB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103112540", new string[] { RootCause }));
            }
            T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            List<R_TESTRECORD> ListRTestRecordSNTestDesc = RTestRecord.GetListBYSNDesc(SN, DB);
            string FailEventPoint = ListRTestRecordSNTestDesc[0].STATION_NAME;
            if (ListRTestRecordSNTestDesc.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180929202405", new string[] { }));
            }
            if (FailEventPoint.IndexOf("AOI") > 0)
            {
                ListString.Clear();
                ListString.Add("E206");
                ListString.Add("E215");
                ListString.Add("E216");
                if (ListString.Contains(RootCause))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103112639", new string[] { RootCause }));
                }
            }
            if (Location.Length > 0)
            {
                if (Location.Length < 2 || Location.Length > 15)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103135904", new string[] { }));
                }
                else
                {
                    T_R_QUACK_SN TOACSII = new T_R_QUACK_SN(DB, DB_TYPE_ENUM.Oracle);
                    int ASC = TOACSII.Asc(Location.Substring(0, 1));
                    if (ASC < 65 || ASC > 90)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103164321", new string[] { }));
                    }
                    if (2 <= Location.Length)
                    {
                        for (int i = 2; i < Location.Length; i++)
                        {
                            int locAsc = TOACSII.Asc(Location.Substring(i, 1));
                            if (locAsc < 48 || (locAsc > 57 && locAsc < 65) || (locAsc > 90 && locAsc != 95))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103164424", new string[] { }));
                            }
                        }
                    }
                    if (Location.Substring(0, 3) == "FOC" && Location.Substring(0, 3) == "XOC")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103164424", new string[] { }));
                    }
                    if (Description.Length > 0)
                    {
                        if (Description.Length < 2 || Description.Length > 60)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103164525", new string[] { Description }));
                        }
                        else
                        {
                            if (1 < Description.Length)
                            {
                                for (int i = 1; i < Description.Length; i++)
                                {
                                    int descAsc = TOACSII.Asc(Description.Substring(i, 1));
                                    if (descAsc > 125)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103165211", new string[] { Description }));
                                    }
                                }
                            }
                            if (Component.Length > 0)
                            {
                                if (Component.Length < 6)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103165543", new string[] { Component }));
                                }
                                if (Component.Substring(0, 1) != "C")
                                {
                                    int ComAsc = TOACSII.Asc(Component.Substring(0, 1));
                                    if (ComAsc < 48 || ComAsc > 57)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103165828", new string[] { }));
                                    }
                                    int ComUnm = Component.IndexOf("-") + 1;
                                    if (ComUnm < 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103172412", new string[] { Component }));
                                    }
                                    if (Component.Substring(ComUnm, Component.Length - ComUnm) == "0")
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103172412", new string[] { Component }));
                                    }
                                    else
                                    {
                                        if (1 <= Component.Length)
                                        {
                                            for (int i = 0; i < Component.Length; i++)
                                            {
                                                int ComAsc1 = TOACSII.Asc(Component.Substring(i, 1));
                                                if ((ComAsc1 < 48 && ComAsc1 != 45) || ComAsc1 > 57)
                                                {
                                                    if (i != Component.Length)
                                                    {
                                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103172457", new string[] { Component }));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (PackType.Length > 0)
                            {
                                if (1 <= PackType.Length)
                                {
                                    for (int i = 0; i < PackType.Length; i++)
                                    {
                                        int PacktypeAsc = TOACSII.Asc(PackType.Substring(i, 1));
                                        if (PacktypeAsc < 65 || PacktypeAsc > 90)
                                        {
                                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103173227", new string[] { PackType }));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if ((RootCause == "E304" || RootCause == "E304") && Description.Length == 0 && StrSolderNo > 4)
    {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103174229", new string[] { }));
            }
            else
        {
                if (ActionCode == "A12")
                {
                    if (ReplaceNo.Length == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105092058", new string[] { }));
                    }
                    else if (ReplaceNo.Substring(0, 1) != "T")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105092204", new string[] { }));
                    }
                    else if (NewTrackNo.Length == 0 && ("E711,E712,E741,E742,E746,E747,E206,E214,E215,E216,E202,E204,E205,E213".IndexOf(RootCause) == -1))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105092409", new string[] { }));
                    }
                    else
                    {
                        T_R_REPAIR_FAILCODE RRepairFailCode = new T_R_REPAIR_FAILCODE(DB, DB_TYPE_ENUM.Oracle);
                        RRepairFailCode.NsgrokRepairSave(DicStation, DicDB, ref DicRef);
                    }
                }
                else
                {
                    if (FailSysPtom == "on")
                    {
                        T_R_REPAIR_OFFLINE RRepairOffline = new T_R_REPAIR_OFFLINE(DB, DB_TYPE_ENUM.Oracle);
                        RRepairOffline.AutoRepairSave(DicStation, DicDB, ref DicRef);
                    }
                    else
                    {
                        T_R_REPAIR_TRANSFER RRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
                        RRepairTransfer.common(DicStation, DicDB, ref DicRef);
                    }
                }
            }
            //return "OK";
        }
        public List<R_REPAIR_ACTION> GetSNAction(string SN, List<string> Action, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_ACTION>().Where(t => t.SN == SN && Action.Contains(t.ACTION_CODE)).OrderBy(t => t.ACTION_CODE, SqlSugar.OrderByType.Desc).ToList();
        }
        public R_REPAIR_ACTION GetSNCerLocAct(string SN, string Location, string ActionCode, string EMP, DateTime? Createdate, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_ACTION>().Where(t => t.SN == SN && t.FAIL_LOCATION == Location && t.ACTION_CODE == ActionCode && t.EDIT_EMP == EMP && t.FAIL_TIME == Createdate).ToList().FirstOrDefault();
        }
        public List<R_REPAIR_ACTION> GetCountSN(string SN, string FailCode, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_ACTION>().Where(t => t.SN == SN && t.FAIL_CODE == FailCode).ToList();
        }
        public int InsertRepairSNInFo(R_REPAIR_ACTION InRepairInFo, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_ACTION>(InRepairInFo).ExecuteCommand();
        }
        public int GetSNBYCount(string SN, OleExec DB)
        {
            string StrSql = $@"SELECT COUNT(*) AS CON FROM (
SELECT SN,ACTION_CODE FROM R_REPAIR_ACTION  WHERE  ACTION_CODE='A30'  AND SN='{SN}'   
UNION ALL    
SELECT SN,ACTION_CODE FROM R_REPAIR_OFFLINE  WHERE  ACTION_CODE='A30' AND  SN='{SN}')";
            int Count = 0;
            DataTable Dt = DB.ExecSelect(StrSql).Tables[0];
            Count = int.Parse(Dt.Rows[0]["CON"].ToString());
            return Count;
        }
        public int InsertRepairAction(object InsertSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                result = DB.ExecSqlNoReturn(InsertSql.ToString(), null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
        public int UpdateRepairSNInFo(R_REPAIR_ACTION UPDATERepairInFo, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_ACTION>(UPDATERepairInFo).Where(t => t.ID == UPDATERepairInFo.ID).ExecuteCommand();
        }

        public R_REPAIR_ACTION GetActionByFailCodeID(OleExec DB,string failCodeID)
        {
            return DB.ORM.Queryable<R_REPAIR_ACTION>().Where(r => r.R_FAILCODE_ID == failCodeID).ToList().FirstOrDefault();
        }

        public int ReplaceSnRepairAction(string new_sn,string old_sn, OleExec SFCDB)
        {
            return SFCDB.ORM.Updateable<R_REPAIR_ACTION>().UpdateColumns(r => new R_REPAIR_ACTION { SN = new_sn })
                .Where(r => r.SN == old_sn).ExecuteCommand();
        }
    }
    public class Row_R_REPAIR_ACTION : DataObjectBase
    {
        public Row_R_REPAIR_ACTION(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_ACTION GetDataObject()
        {
            R_REPAIR_ACTION DataObject = new R_REPAIR_ACTION();
            DataObject.SOLDER = this.SOLDER;
            DataObject.PACKAGE_TYPE = this.PACKAGE_TYPE;
            DataObject.PART_DESC = this.PART_DESC;
            DataObject.ITEMS_SON_ID = this.ITEMS_SON_ID;
            DataObject.REASON_CODE = this.REASON_CODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.KEYPART_SN = this.KEYPART_SN;
            DataObject.NEW_KEYPART_SN = this.NEW_KEYPART_SN;
            DataObject.KP_NO = this.KP_NO;
            DataObject.TR_SN = this.TR_SN;
            DataObject.MFR_CODE = this.MFR_CODE;
            DataObject.MFR_NAME = this.MFR_NAME;
            DataObject.DATE_CODE = this.DATE_CODE;
            DataObject.LOT_CODE = this.LOT_CODE;
            DataObject.NEW_KP_NO = this.NEW_KP_NO;
            DataObject.NEW_TR_SN = this.NEW_TR_SN;
            DataObject.NEW_MFR_CODE = this.NEW_MFR_CODE;
            DataObject.NEW_MFR_NAME = this.NEW_MFR_NAME;
            DataObject.NEW_DATE_CODE = this.NEW_DATE_CODE;
            DataObject.NEW_LOT_CODE = this.NEW_LOT_CODE;
            DataObject.REPAIR_EMP = this.REPAIR_EMP;
            DataObject.REPAIR_TIME = this.REPAIR_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.FAIL_TIME = this.FAIL_TIME;
            DataObject.R_START_TIME = this.REPAIR_START_TIME;
            DataObject.R_FINISH_TIME = this.REPAIR_COMPLETE_TIME;
            DataObject.FIXED_FLAG = this.FIXED_FLAG;
            DataObject.REPLACED_FLAG = this.REPLACED_FLAG;
            DataObject.SOLUTION = this.SOLUTION;
            DataObject.ID = this.ID;
            DataObject.R_FAILCODE_ID = this.REPAIR_FAILCODE_ID;
            DataObject.SN = this.SN;
            DataObject.ACTION_CODE = this.ACTION_CODE;
            DataObject.SECTION_ID = this.SECTION_ID;
            DataObject.PROCESS = this.PROCESS;
            DataObject.ITEMS_ID = this.ITEMS_ID;
            DataObject.COMPOMENTID = this.COMPOMENTID;//Add By ZHB 20200805
            DataObject.MPN = this.MPN;
            DataObject.NEW_MPN = this.NEW_MPN;
            return DataObject;
        }
        public string SOLDER
        {
            get
            {
                return (string)this["SOLDER"];
            }
            set
            {
                this["SOLDER"] = value;
            }
        }
        public string PACKAGE_TYPE
        {
            get
            {
                return (string)this["PACKAGE_TYPE"];
            }
            set
            {
                this["PACKAGE_TYPE"] = value;
            }
        }
        public string PART_DESC
        {
            get
            {
                return (string)this["PART_DESC"];
            }
            set
            {
                this["PART_DESC"] = value;
            }
        }
        public string ITEMS_SON_ID
        {
            get
            {
                return (string)this["ITEMS_SON_ID"];
            }
            set
            {
                this["ITEMS_SON_ID"] = value;
            }
        }
        public string REASON_CODE
        {
            get
            {
                return (string)this["REASON_CODE"];
            }
            set
            {
                this["REASON_CODE"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return (string)this["FAIL_LOCATION"];
            }
            set
            {
                this["FAIL_LOCATION"] = value;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public string KEYPART_SN
        {
            get
            {
                return (string)this["KEYPART_SN"];
            }
            set
            {
                this["KEYPART_SN"] = value;
            }
        }
        public string NEW_KEYPART_SN
        {
            get
            {
                return (string)this["NEW_KEYPART_SN"];
            }
            set
            {
                this["NEW_KEYPART_SN"] = value;
            }
        }
        public string KP_NO
        {
            get
            {
                return (string)this["KP_NO"];
            }
            set
            {
                this["KP_NO"] = value;
            }
        }
        public string TR_SN
        {
            get
            {
                return (string)this["TR_SN"];
            }
            set
            {
                this["TR_SN"] = value;
            }
        }
        public string MFR_CODE
        {
            get
            {
                return (string)this["MFR_CODE"];
            }
            set
            {
                this["MFR_CODE"] = value;
            }
        }
        public string MFR_NAME
        {
            get
            {
                return (string)this["MFR_NAME"];
            }
            set
            {
                this["MFR_NAME"] = value;
            }
        }
        public string DATE_CODE
        {
            get
            {
                return (string)this["DATE_CODE"];
            }
            set
            {
                this["DATE_CODE"] = value;
            }
        }
        public string LOT_CODE
        {
            get
            {
                return (string)this["LOT_CODE"];
            }
            set
            {
                this["LOT_CODE"] = value;
            }
        }
        public string NEW_KP_NO
        {
            get
            {
                return (string)this["NEW_KP_NO"];
            }
            set
            {
                this["NEW_KP_NO"] = value;
            }
        }
        public string NEW_TR_SN
        {
            get
            {
                return (string)this["NEW_TR_SN"];
            }
            set
            {
                this["NEW_TR_SN"] = value;
            }
        }
        public string NEW_MFR_CODE
        {
            get
            {
                return (string)this["NEW_MFR_CODE"];
            }
            set
            {
                this["NEW_MFR_CODE"] = value;
            }
        }
        public string NEW_MFR_NAME
        {
            get
            {
                return (string)this["NEW_MFR_NAME"];
            }
            set
            {
                this["NEW_MFR_NAME"] = value;
            }
        }
        public string NEW_DATE_CODE
        {
            get
            {
                return (string)this["NEW_DATE_CODE"];
            }
            set
            {
                this["NEW_DATE_CODE"] = value;
            }
        }
        public string NEW_LOT_CODE
        {
            get
            {
                return (string)this["NEW_LOT_CODE"];
            }
            set
            {
                this["NEW_LOT_CODE"] = value;
            }
        }
        public string REPAIR_EMP
        {
            get
            {
                return (string)this["REPAIR_EMP"];
            }
            set
            {
                this["REPAIR_EMP"] = value;
            }
        }
        public DateTime? REPAIR_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_TIME"];
            }
            set
            {
                this["REPAIR_TIME"] = value;
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
        public DateTime? FAIL_TIME
        {
            get
            {
                return (DateTime?)this["FAIL_TIME"];
            }
            set
            {
                this["FAIL_TIME"] = value;
            }
        }
        public DateTime? REPAIR_START_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_START_TIME"];
            }
            set
            {
                this["REPAIR_START_TIME"] = value;
            }
        }
        public DateTime? REPAIR_COMPLETE_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_COMPLETE_TIME"];
            }
            set
            {
                this["REPAIR_COMPLETE_TIME"] = value;
            }
        }
        public string FIXED_FLAG
        {
            get
            {
                return (string)this["FIXED_FLAG"];
            }
            set
            {
                this["FIXED_FLAG"] = value;
            }
        }
        public string REPLACED_FLAG
        {
            get
            {
                return (string)this["REPLACED_FLAG"];
            }
            set
            {
                this["REPLACED_FLAG"] = value;
            }
        }
        public string SOLUTION
        {
            get
            {
                return (string)this["SOLUTION"];
            }
            set
            {
                this["SOLUTION"] = value;
            }
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
        public string REPAIR_FAILCODE_ID
        {
            get
            {
                return (string)this["REPAIR_FAILCODE_ID"];
            }
            set
            {
                this["REPAIR_FAILCODE_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string ACTION_CODE
        {
            get
            {
                return (string)this["ACTION_CODE"];
            }
            set
            {
                this["ACTION_CODE"] = value;
            }
        }
        public string SECTION_ID
        {
            get
            {
                return (string)this["SECTION_ID"];
            }
            set
            {
                this["SECTION_ID"] = value;
            }
        }
        public string PROCESS
        {
            get
            {
                return (string)this["PROCESS"];
            }
            set
            {
                this["PROCESS"] = value;
            }
        }
        public string ITEMS_ID
        {
            get
            {
                return (string)this["ITEMS_ID"];
            }
            set
            {
                this["ITEMS_ID"] = value;
            }
        }
        public string COMPOMENTID
        {
            get
            {
                return (string)this["COMPOMENTID"];
            }
            set
            {
                this["COMPOMENTID"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string NEW_MPN
        {
            get
            {
                return (string)this["NEW_MPN"];
            }
            set
            {
                this["NEW_MPN"] = value;
            }
        }
    }
    public class R_REPAIR_ACTION
    {
        public string SOLDER { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PART_DESC { get; set; }
        public string ITEMS_SON_ID { get; set; }
        public string REASON_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string FAIL_LOCATION { get; set; }
        public string FAIL_CODE { get; set; }
        public string KEYPART_SN { get; set; }
        public string NEW_KEYPART_SN { get; set; }
        public string KP_NO { get; set; }
        public string TR_SN { get; set; }
        public string MFR_CODE { get; set; }
        public string MFR_NAME { get; set; }
        public string DATE_CODE { get; set; }
        public string LOT_CODE { get; set; }
        public string NEW_KP_NO { get; set; }
        public string NEW_TR_SN { get; set; }
        public string NEW_MFR_CODE { get; set; }
        public string NEW_MFR_NAME { get; set; }
        public string NEW_DATE_CODE { get; set; }
        public string NEW_LOT_CODE { get; set; }
        public string REPAIR_EMP { get; set; }
        public DateTime? REPAIR_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? FAIL_TIME { get; set; }
        [SugarColumn(ColumnName = "REPAIR_START_TIME")]
        public DateTime? R_START_TIME { get; set; }
        [SugarColumn(ColumnName = "REPAIR_COMPLETE_TIME")]
        public DateTime? R_FINISH_TIME { get; set; }
        public string FIXED_FLAG { get; set; }
        public string REPLACED_FLAG { get; set; }
        public string SOLUTION { get; set; }
        public string ID { get; set; }
        [SugarColumn(ColumnName = "REPAIR_FAILCODE_ID")]
        public string R_FAILCODE_ID { get; set; }
        public string SN { get; set; }
        public string ACTION_CODE { get; set; }
        public string SECTION_ID { get; set; }
        public string PROCESS { get; set; }
        public string ITEMS_ID { get; set; }
        public string COMPOMENTID { get; set; } //Add By ZHB 20200805
        public string MPN { get; set; }
        public string NEW_MPN { get; set; } //Add By ZHB 20200805

    }
}