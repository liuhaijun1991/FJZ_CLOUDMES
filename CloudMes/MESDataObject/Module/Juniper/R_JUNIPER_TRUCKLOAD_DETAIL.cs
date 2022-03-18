using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_JUNIPER_TRUCKLOAD_DETAIL : DataObjectTable
    {
        public T_R_JUNIPER_TRUCKLOAD_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JUNIPER_TRUCKLOAD_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JUNIPER_TRUCKLOAD_DETAIL);
            TableName = "R_JUNIPER_TRUCKLOAD_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool InserTODetail(string StrTONO, string StrPACKNO, string StrSkuno, string StrTrailerNum, string StrDNNO, string NewPL, string UserID, string BU, OleExec DB)
        {
            T_R_JUNIPER_TRUCKLOAD_DETAIL InsertTODet = new T_R_JUNIPER_TRUCKLOAD_DETAIL(DB, DBType);

            string StrID = InsertTODet.GetNewID(BU, DB);
            string SQL = $@"INSERT INTO  R_JUNIPER_TRUCKLOAD_DETAIL (ID,TO_NO,PACK_NO,SKUNO,TRAILER_NUM,DELIVERYNUMBER,EDIT_EMP,EDIT_TIME,NEW_PACK_NO) VALUES('{StrID}','{StrTONO}','{StrPACKNO}','{StrSkuno}','{StrTrailerNum}','{StrDNNO}','{UserID}',sysdate,'{NewPL}')";

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

        public R_JUNIPER_TRUCKLOAD_DETAIL CheckPallet(string pack_no, string BU, OleExec DB)
        {
            R_JUNIPER_TRUCKLOAD_DETAIL JNPPalletDetail = null;
            Row_R_JUNIPER_TRUCKLOAD_DETAIL Rows = (Row_R_JUNIPER_TRUCKLOAD_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@" SELECT * FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE PACK_NO='{pack_no}' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                JNPPalletDetail = Rows.GetDataObject();
            }

            return JNPPalletDetail;
        }

        public int GetNewPLQty(string to_no, string Newpallet, string BU, OleExec DB)
        {
            int qtyy = 0;
            string StrSql = string.Empty;
            DataTable dt = new DataTable();

            StrSql = $@" SELECT count(*) as PalletQty FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE TO_NO ='{to_no}' and NEW_PACK_NO='{Newpallet}' ";

            dt = DB.ExecSelect(StrSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                qtyy = Convert.ToInt32(dt.Rows[0]["PalletQty"].ToString());
                return qtyy;
            }
            else
            {
                return qtyy;
            }
        }

        public int CheckQTY(string to_no, string BU, OleExec DB)
        {
            int qtyy = 0;
            string StrSql = string.Empty;
            DataTable dt = new DataTable();

            StrSql = $@" SELECT count(*) as qtys FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE to_no='{to_no}' ";

            dt = DB.ExecSelect(StrSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                qtyy = Convert.ToInt32(dt.Rows[0]["qtys"].ToString());
                return qtyy;
            }
            else
            {
                return qtyy;
            }
        }

        public R_JUNIPER_TRUCKLOAD_DETAIL CheckTONOData(string to_no, string BU, OleExec DB)
        {
            R_JUNIPER_TRUCKLOAD_DETAIL JNPPalletDetail = null;
            Row_R_JUNIPER_TRUCKLOAD_DETAIL Rows = (Row_R_JUNIPER_TRUCKLOAD_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@" SELECT * FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE TO_NO='{to_no}' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                JNPPalletDetail = Rows.GetDataObject();
            }

            return JNPPalletDetail;
        }

        public DataTable GetOpenTOdata(string to_no, string BU, OleExec DB)
        {
            DataTable dt1 = new DataTable();
            string Strsql1 = "";

            Strsql1 = $@"SELECT TRAILER_NUM,NEW_PACK_NO FROM (
            SELECT * FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE TO_NO ='{to_no}' ORDER BY EDIT_TIME DESC)
            WHERE ROWNUM<2";

            dt1 = DB.ExecSelect(Strsql1).Tables[0];

            return dt1;
        }

        public DataTable GetOpenTOPalletListdata(string to_no, string BU, OleExec DB)
        {
            DataTable dt1 = new DataTable();
            string Strsql1 = "";

            Strsql1 = $@"SELECT NEW_PACK_NO FROM (
            SELECT * FROM R_JUNIPER_TRUCKLOAD_DETAIL WHERE TO_NO ='{to_no}' ORDER BY EDIT_TIME DESC)";

            dt1 = DB.ExecSelect(Strsql1).Tables[0];

            return dt1;
        }

        public List<object> GetOpenTOPalletListOfNewPallet(string to_no, string newPalletID, string BU, OleExec DB)
        {
            string Strsql1 = "";

            Strsql1 = $@"SELECT DISTINCT M.SALESORDERNUMBER SO#,
                            L.SALESORDERLINEITEM SOLINE,
                            K.PONO              PO#,
                            K.POLINE            POLINE,
                            A.SALESORDER        DN#,
                            A.LINEID            DNLINE,
                            A.SKUNO,
                            B.PACK_NO,
                            M.SHIPTOCOUNTRYCODE SHIPTO
                       FROM R_JUNIPER_MFPACKINGLIST A,
                            R_JUNIPER_TRUCKLOAD_DETAIL B,
                            (SELECT F.WORKORDERNO, I.PACK_NO, COUNT(F.ID) QTY
                               FROM R_SN F, R_SN_PACKING G, R_PACKING H, R_PACKING I
                              WHERE F.ID = G.SN_ID
                                AND G.PACK_ID = H.ID
                                AND H.PARENT_PACK_ID = I.ID
                                AND I.PACK_NO IN
                                    (SELECT PACK_NO
                                       FROM R_JUNIPER_TRUCKLOAD_DETAIL
                                      WHERE NEW_PACK_NO = '{newPalletID}'
                                       AND TO_NO = '{to_no}')
                              GROUP BY F.WORKORDERNO, I.PACK_NO) J,
                            O_ORDER_MAIN K,
                            O_I137_ITEM L,
                            O_I137_HEAD M
                      WHERE A.INVOICENO = B.TO_NO
                        AND A.PALLETID = B.NEW_PACK_NO
                        AND A.SKUNO = B.SKUNO
                        AND A.SALESORDER = B.DELIVERYNUMBER
                        AND A.WORKORDERNO=J.WORKORDERNO
                        AND A.QUANTITY=J.QTY
                        AND B.PACK_NO=J.PACK_NO
                        AND A.WORKORDERNO = K.PREWO
                        AND K.ITEMID = L.ID
                        AND L.TRANID = M.TRANID
                        AND A.PALLETID = '{newPalletID}'
                        AND A.INVOICENO = '{to_no}'
                        ORDER BY SO#,SOLINE";

            return DB.ORM.SqlQueryable<object>(Strsql1).ToList();
        }
    }
    public class Row_R_JUNIPER_TRUCKLOAD_DETAIL : DataObjectBase
    {
        public Row_R_JUNIPER_TRUCKLOAD_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_JUNIPER_TRUCKLOAD_DETAIL GetDataObject()
        {
            R_JUNIPER_TRUCKLOAD_DETAIL DataObject = new R_JUNIPER_TRUCKLOAD_DETAIL();
            DataObject.NEW_PACK_NO = this.NEW_PACK_NO;
            DataObject.ID = this.ID;
            DataObject.TO_NO = this.TO_NO;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TRAILER_NUM = this.TRAILER_NUM;
            DataObject.DELIVERYNUMBER = this.DELIVERYNUMBER;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string NEW_PACK_NO
        {
            get
            {
                return (string)this["NEW_PACK_NO"];
            }
            set
            {
                this["NEW_PACK_NO"] = value;
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
        public string PACK_NO
        {
            get
            {
                return (string)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
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
        public string TRAILER_NUM
        {
            get
            {
                return (string)this["TRAILER_NUM"];
            }
            set
            {
                this["TRAILER_NUM"] = value;
            }
        }
        public string DELIVERYNUMBER
        {
            get
            {
                return (string)this["DELIVERYNUMBER"];
            }
            set
            {
                this["DELIVERYNUMBER"] = value;
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
    public class R_JUNIPER_TRUCKLOAD_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string NEW_PACK_NO { get; set; }
        public string TO_NO { get; set; }
        public string PACK_NO { get; set; }
        public string SKUNO { get; set; }
        public string TRAILER_NUM { get; set; }
        public string DELIVERYNUMBER { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}