using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_JUNIPER_TRUCKLOAD_TO : DataObjectTable
    {
        public static Dictionary<string, List<C_CODE_MAPPING>> _CodeMapping = new Dictionary<string, List<C_CODE_MAPPING>>();

        public T_R_JUNIPER_TRUCKLOAD_TO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JUNIPER_TRUCKLOAD_TO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JUNIPER_TRUCKLOAD_TO);
            TableName = "R_JUNIPER_TRUCKLOAD_TO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        //Add by Winster FJZ Juniper GenerateTONO For Truck Load
        public bool GenerateTONO(string UserID, string BU, OleExec DB)
        {
            T_R_JUNIPER_TRUCKLOAD_TO InsertTOno = new T_R_JUNIPER_TRUCKLOAD_TO(DB, DBType);
            T_C_CONTROL PLANTcontrol = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            C_CONTROL RowUS = null;
            string StrTONO = GetNextTO("TRUCK_LOADTO_TONO", DB.ORM);

            RowUS = PLANTcontrol.GetControlByName("TRUCKLOAD_PLANT", DB);
            string CheckUS = RowUS.CONTROL_VALUE.ToString().Trim();

            if (CheckUS==null || CheckUS =="")
            {
                CheckUS = "MBGA";
            }

            string StrID = InsertTOno.GetNewID(BU, DB);
            string SQL = $@"INSERT INTO  R_JUNIPER_TRUCKLOAD_TO (ID,TO_NO,PLANT,QTY,BU,CLOSED,EDIT_EMP,EDIT_TIME) VALUES('{StrID}','{StrTONO}','{CheckUS}','0','FJZ','0','{UserID}',sysdate)";

            int Number = DB.ExecSqlNoReturn(SQL, null);
            if (Number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public R_JUNIPER_TRUCKLOAD_TO CheckTOqty(string TONO, string BU, OleExec DB)
        {
            T_R_JUNIPER_TRUCKLOAD_TO CheckTO = new T_R_JUNIPER_TRUCKLOAD_TO(DB, DBType);
            string sql = $@" SELECT DISTINCT QTY FROM R_JUNIPER_TRUCKLOAD_TO WHERE TO_NO='{TONO}' ";
            DataSet ds = DB.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_JUNIPER_TRUCKLOAD_TO rowtoqty = (Row_R_JUNIPER_TRUCKLOAD_TO)this.NewRow();
                rowtoqty.loadData(ds.Tables[0].Rows[0]);
                return rowtoqty.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public R_JUNIPER_TRUCKLOAD_TO GetDetailData(string To_no, string BU, OleExec DB)
        {
            R_JUNIPER_TRUCKLOAD_TO JNPPalletDetail = null;
            Row_R_JUNIPER_TRUCKLOAD_TO Rows = (Row_R_JUNIPER_TRUCKLOAD_TO)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@" SELECT * FROM R_JUNIPER_TRUCKLOAD_TO WHERE TO_NO='{To_no}' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                JNPPalletDetail = Rows.GetDataObject();
            }

            return JNPPalletDetail;
        }

        public bool UpdateTONO(string TYPE, string TONO, string UserID, string BU, OleExec DB)
        {
            T_R_JUNIPER_TRUCKLOAD_TO UpdateTOno = new T_R_JUNIPER_TRUCKLOAD_TO(DB, DBType);
            string flag = "1";

            if (TYPE == "OPEN")
            {
                flag = "0";
            }
            else if (TYPE == "CLOSED")
            {
                flag = "1";
            }

            string SQL = $@"UPDATE  R_JUNIPER_TRUCKLOAD_TO SET CLOSED='{flag}',EDIT_EMP='{UserID}',EDIT_TIME=SYSDATE WHERE TO_NO='{TONO}'";
            int Number = DB.ExecSqlNoReturn(SQL, null);
            if (Number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateTOQTY(string StrTONO, int StrTOQty, string UserID, string BU, OleExec DB)
        {
            T_R_JUNIPER_TRUCKLOAD_TO UpdateTOqtys = new T_R_JUNIPER_TRUCKLOAD_TO(DB, DBType);

            string StrID = UpdateTOqtys.GetNewID(BU, DB);
            string SQL = $@"UPDATE  R_JUNIPER_TRUCKLOAD_TO SET QTY='{StrTOQty}',EDIT_EMP='{UserID}',EDIT_TIME=SYSDATE WHERE TO_NO='{StrTONO}'";

            int Number = DB.ExecSqlNoReturn(SQL, null);
            if (Number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Row_R_JUNIPER_TRUCKLOAD_TO GetByTONo(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_JUNIPER_TRUCKLOAD_TO row = (Row_R_JUNIPER_TRUCKLOAD_TO)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@" SELECT * FROM (
                SELECT * FROM R_JUNIPER_TRUCKLOAD_TO WHERE CLOSED='0' AND ROWNUM<2
                )A ORDER BY EDIT_TIME DESC";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_JUNIPER_TRUCKLOAD_TO GetOpenTONo(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_JUNIPER_TRUCKLOAD_TO row = (Row_R_JUNIPER_TRUCKLOAD_TO)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@" SELECT * FROM (
                SELECT * FROM R_JUNIPER_TRUCKLOAD_TO WHERE closed='0' ORDER BY 2 DESC
                ) WHERE rownum<2 ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public static string GetNextTO(string RuleName, SqlSugar.SqlSugarClient db)
        {
            C_SN_RULE root = null;
            List<C_SN_RULE_DETAIL> detail = null;
            DateTime sysdate = db.GetDate();
            root = db.Queryable<C_SN_RULE>().Where(t => t.NAME == RuleName).ToList().FirstOrDefault();
            detail = db.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == root.ID).OrderBy(t => t.SEQ).ToList();

            string SN = "";
            bool ResetFlag = false;
            //¹M¾ú¨C­Ó³W«h¡A®Ú¾Ú C_SN_RULE_DETAIL ªí¤¤ªº SEQ_NO ¶i¦æ¹M¾ú
            for (int i = 0; i < detail.Count; i++)
            {
                //­º¥ýÂê¦í³o±ø detail °O¿ý
                detail[i].LockMe(db);
                //¦pªG°t¸mªº¬O PREFIX¡A«hª½±µ²K¥[¨ì SN ÅÜ¶q¦Z
                if (detail[i].INPUTTYPE == "PREFIX")
                {
                    SN += detail[i].CURVALUE;
                }
                //¦pªG°t¸mªº¬O¦~¤ë¤é®æ¦¡
                else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                {
                    //®Ú¾Ú¥N½XÃþ«¬¦b C_CODE_MAPPING ªí¤¤§ä¨ì¹ïÀ³ªº¹ï¶H¶°¦X
                    string codeType = detail[i].CODETYPE;
                    List<C_CODE_MAPPING> CodeMapping = null;
                    if (_CodeMapping.ContainsKey(codeType))
                    {
                        CodeMapping = _CodeMapping[codeType];
                    }
                    else
                    {
                        //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                        //CodeMapping = TCCM.GetDataByName(codeType, DB);
                        CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                        if (CodeMapping != null)
                        {
                            _CodeMapping.Add(codeType, CodeMapping);
                        }
                    }
                    //¦pªG¬O¦~¤ë¤é®æ¦¡¡A´Nªð¦^ 4¦ìªº¦~ ©ÎªÌ 2¦ìªº¤ë ©ÎªÌ 2¦ìªº¤é ©ÎªÌ 2¦ìªº©P
                    string VALUE = null;
                    switch (detail[i].INPUTTYPE)
                    {
                        case "YYYY":
                            //VALUE = DateTime.Now.Year.ToString();
                            //§ï¬°¨ú¼Æ¾Ú®w®É¶¡ 2020.12.24 fgg
                            VALUE = sysdate.Year.ToString();
                            break;
                        case "MM":
                            //VALUE = DateTime.Now.Month.ToString();
                            //§ï¬°¨ú¼Æ¾Ú®w®É¶¡ 2020.12.24 fgg
                            VALUE = sysdate.Month.ToString();
                            break;
                        case "DD":
                            //VALUE = DateTime.Now.Day.ToString();
                            //§ï¬°¨ú¼Æ¾Ú®w®É¶¡ 2020.12.24 fgg
                            VALUE = sysdate.Day.ToString();
                            break;
                        case "WW":
                            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                            //VALUE = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                            //§ï¬°¨ú¼Æ¾Ú®w®É¶¡ 2020.12.24 fgg
                            VALUE = gc.GetWeekOfYear(sysdate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                            break;

                    }

                    //³o¬qªº¥Øªº¬O¬°¤F¯à­«¸m¬y¤ô¸¹¡A·í¦~¡B¤ë¡B¤é¡B©Pµ¥µo¥ÍÅÜ¤Æ®É¡A®Ú¾Ú VALUE Àò¨ú¨ìªº CodeMapping ¹ï¶Hªº CODEVALUE »P ·í«eªº
                    //C_SN_RULE_DETAIL ¹ï¶Hªº CURVALUE ­È¤£¤@¼Ë¡A´N·|¶i¤J¨ì¸Ì­±ªº§PÂ_¤¤¡AµM«á¹Á¸ÕÀò¨ú C_SN_RULE_DETAIL ¹ï¶Hªº RESETSN_FLAG ¬O§_
                    //¬° 1¡A¦pªG¬° 1¡A«h±N ResetFlag ³]¸m¬°true¡Aµ¥¤U¦¸¶i¤J¨ì¬y¤ô½X­pºâ®É¡A±N C_SN_RULE_DETAIL ¹ï¶Hªº VALUE10 ³]¸m¬°ªì©l­È RESETVALUE
                    C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);
                    if (detail[i].CURVALUE != TAG.CODEVALUE)
                    {
                        detail[i].CURVALUE = TAG.CODEVALUE;
                        if (detail[i].RESETSN_FLAG == 1)
                        {
                            ResetFlag = true;
                        }
                    }
                    SN += detail[i].CURVALUE;
                }

                //¦pªG°t¸mªº¬O SN¡A¨º»ò´NÀò¨ú¨ì·í«e C_SN_RULE_DETAIL ¹ï¶Hªº Value10 ÄÝ©Ê­È¡A¥[ 1 ¤§«á±N 10 ¶i¨îªº¼ÆÂà´«¦¨ N ¶i¨îªº¼Æ
                //¡]N ªº¤j¤p¥Ñ CodeMapping ¹ï¶H­Ó¼Æ¨Ó´£¨Ñ¡^¡A³Ì«á±NÂà´«¦Zªºµ²ªG¸É¹s¦Z«÷±µ¨ì SN ÅÜ¶q«á­±
                else if (detail[i].INPUTTYPE == "SN")
                {
                    //­«¸m¬y¤ô¸¹
                    if (ResetFlag)
                    {
                        detail[i].VALUE10 = detail[i].RESETVALUE;
                    }
                    string codeType = detail[i].CODETYPE;
                    List<C_CODE_MAPPING> CodeMapping = null;
                    if (_CodeMapping.ContainsKey(codeType))
                    {
                        CodeMapping = _CodeMapping[codeType];
                    }
                    else
                    {
                        //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                        //CodeMapping = TCCM.GetDataByName(codeType, DB);
                        CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                        if (CodeMapping != null)
                        {
                            _CodeMapping.Add(codeType, CodeMapping);
                        }
                    }
                    int curValue = int.Parse(detail[i].VALUE10);
                    curValue++;
                    detail[i].VALUE10 = curValue.ToString();
                    int T = CodeMapping.Count;
                    string sn = "";

                    while (curValue / T != 0)
                    {
                        int R = curValue % T;
                        sn = CodeMapping[R].CODEVALUE + sn;
                        curValue = curValue / T;
                    }
                    sn = CodeMapping[curValue].CODEVALUE + sn;
                    if (sn.Length < detail[i].CURVALUE.Length)
                    {
                        for (int k = 0; detail[i].CURVALUE.Length != sn.Length; k++)
                        {
                            sn = "0" + sn;
                        }
                    }
                    if (sn.Length > detail[i].CURVALUE.Length)
                    {
                        var ex = detail.Find(t => t.CHECK_FLAG == detail[i].SEQ && t.INPUTTYPE == "SN_EX");
                        if (ex != null)
                        {
                            sn = sn.Substring(1);
                            detail[i].VALUE10 = "0";
                            MakeEx_SN(detail, ex, db);
                        }
                        else
                        {

                            throw new Exception("¥Í¦¨ªºSN¶W¹L³Ì¤j­È!");
                        }
                    }

                    detail[i].CURVALUE = sn;
                    SN += detail[i].CURVALUE;
                }
                if (detail[i].INPUTTYPE == "SN_EX")
                {
                    continue;
                }
                int T1 = 0;
                detail[i].EDIT_TIME = DateTime.Now;
                //string ret = DB.ExecSQL(detail[i].GetUpdateString(DB_TYPE_ENUM.Oracle));
                string ret = db.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                if (!Int32.TryParse(ret, out T1))
                {
                    throw new Exception("§ó·s§Ç¦C­È¥X¿ù!" + ret);
                }
            }
            SN = "";
            for (int i = 0; i < detail.Count; i++)
            {
                SN += detail[i].CURVALUE;
            }

            int T2 = 0;
            root.CURVALUE = SN;
            //string ret1 = DB.ExecSQL(root.GetUpdateString(DB_TYPE_ENUM.Oracle));
            string ret1 = db.Updateable<C_SN_RULE>(root).Where(t => t.ID == root.ID).ExecuteCommand().ToString();
            if (!Int32.TryParse(ret1, out T2))
            {
                throw new Exception("§ó·s§Ç¦C­È¥X¿ù!" + ret1);
            }
            return SN;
        }

        static void MakeEx_SN(List<C_SN_RULE_DETAIL> ruls, C_SN_RULE_DETAIL SN_EX, SqlSugar.SqlSugarClient db)
        {
            ////­«¸m¬y¤ô¸¹
            //if (ResetFlag)
            //{
            //    detail[i].VALUE10 = detail[i].RESETVALUE;
            //}
            string codeType = SN_EX.CODETYPE;
            List<C_CODE_MAPPING> CodeMapping = null;
            if (_CodeMapping.ContainsKey(codeType))
            {
                CodeMapping = _CodeMapping[codeType];
            }
            else
            {
                //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                //CodeMapping = TCCM.GetDataByName(codeType, DB);
                CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                if (CodeMapping != null)
                {
                    _CodeMapping.Add(codeType, CodeMapping);
                }
            }
            int curValue = int.Parse(SN_EX.VALUE10);
            curValue++;
            SN_EX.VALUE10 = curValue.ToString();
            int T = CodeMapping.Count;
            string sn = "";

            while (curValue / T != 0)
            {
                int R = curValue % T;
                sn = CodeMapping[R].CODEVALUE + sn;
                curValue = curValue / T;
            }
            sn = CodeMapping[curValue].CODEVALUE + sn;
            if (sn.Length < SN_EX.CURVALUE.Length)
            {
                for (int k = 0; SN_EX.CURVALUE.Length != sn.Length; k++)
                {
                    sn = CodeMapping[0].CODEVALUE + sn;
                }
            }
            if (sn.Length > SN_EX.CURVALUE.Length)
            {
                var ex = ruls.Find(t => t.CHECK_FLAG == SN_EX.SEQ && t.INPUTTYPE == "SN_EX");
                if (ex != null)
                {
                    sn = sn.Substring(1);
                    MakeEx_SN(ruls, ex, db);
                }
                else
                {
                    throw new Exception("¥Í¦¨ªºSN¶W¹L³Ì¤j­È!");
                }
            }

            SN_EX.CURVALUE = sn;
            db.Updateable(SN_EX).Where(t => t.ID == SN_EX.ID).ExecuteCommand();
            //SN += detail[i].CURVALUE;
        }

    }
    public class Row_R_JUNIPER_TRUCKLOAD_TO : DataObjectBase
    {
        public Row_R_JUNIPER_TRUCKLOAD_TO(DataObjectInfo info) : base(info)
        {

        }
        public R_JUNIPER_TRUCKLOAD_TO GetDataObject()
        {
            R_JUNIPER_TRUCKLOAD_TO DataObject = new R_JUNIPER_TRUCKLOAD_TO();
            DataObject.ID = this.ID;
            DataObject.TO_NO = this.TO_NO;
            DataObject.PLANT = this.PLANT;
            DataObject.QTY = this.QTY;
            DataObject.BU = this.BU;
            DataObject.CLOSED = this.CLOSED;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string CLOSED
        {
            get
            {
                return (string)this["CLOSED"];
            }
            set
            {
                this["CLOSED"] = value;
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
    }
    public class R_JUNIPER_TRUCKLOAD_TO
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string TO_NO { get; set; }
        public string PLANT { get; set; }
        public double? QTY { get; set; }
        public string TRAILER_NUM { get; set; }
        public string BU { get; set; }
        /// <summary>
        /// Old version only 0\1\2\3 status,
        /// new version define for new Truck-Load Function at 2022-3-1 16:33:09
        /// 0>Open Status
        /// 1>Close,Pending Call Truck-Load GT(F3FF to CAFG)
        /// 2>Finish Truck Load GT,Pending SHIPOUT GT(CAFG to FGTG)
        /// 3>Finish Shipout GT,Pending PGI.
        /// 4>Completed
        /// </summary>
        public string CLOSED { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}