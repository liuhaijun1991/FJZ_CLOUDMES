using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JUNIPER_MFPACKINGLIST : DataObjectTable
    {
        public T_R_JUNIPER_MFPACKINGLIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JUNIPER_MFPACKINGLIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JUNIPER_MFPACKINGLIST);
            TableName = "R_JUNIPER_MFPACKINGLIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
  
        public bool InsertB2Bdata(string BU, string TONO, int CtnQTY, string Weight, string COO, string TrailNum, string PackNO,string Newpl,string Lasteditby, OleExec DB)
        {
            int plqtys = 0;
            string Strsql1 = "", StrDnQty="0", sql="";
            DataTable dt1 = new DataTable();
            T_R_JUNIPER_MFPACKINGLIST LOTDETAIL = new T_R_JUNIPER_MFPACKINGLIST(DB, DBType);
            string StrID = LOTDETAIL.GetNewID(BU, DB);
            //StrID = LOTDETAIL.CheckIDisExist(StrID, BU, DB);

            if (Weight == null || Weight=="")
            {
                return false;
            }
            if (COO == null || Weight == "")
            {
                return false;
            }

            Strsql1 = $@"SELECT count(SN_ID) AS UNITSQTY FROM r_sn_packing WHERE pack_id in(
            SELECT id FROM R_PACKING WHERE PARENT_PACK_ID IN(
            SELECT ID FROM R_PACKING WHERE PACK_NO='{PackNO.ToString()}'))";

            dt1 = DB.ExecSelect(Strsql1).Tables[0];

            if (dt1.Rows.Count > 0)
            {
                StrDnQty = dt1.Rows[0]["UNITSQTY"].ToString();              
            }

            if (PackNO.Replace("PL", "PLJ") == Newpl)
            {
                plqtys = 1;
            }
            else
            {
                plqtys = 0;
            }

            //string sql = $@"INSERT INTO SFCRUNTIME.R_JUNIPER_MFPACKINGLIST 
            //(ID,INVOICENO,TYPE_IU,SKUNO,GROUPID,MAINPARTNO,QUANTITY,UNIT,CARTONQTY,PALLETNO,CARTONNO,GROSSWEIGHT,CHILDCARTONNO,CHILDGROSSWEIGHT,DIMENSION,STATUS,COO,TRAILERNUMBER,PALLETID,WORKORDERNO,PO_NUMBER,SALESORDER,SHIPSET,LINEID,AMOUNT,CURRENCY,LASTEDITDT,LASTEDITBY)
            //SELECT DISTINCT * FROM (
            //SELECT '{StrID}' AS ID,'{TONO}' as invoiceno,'U' AS type_iu,B.SKUNO,B.GROUPID,'' as mainPartNo,
            //E.QTY as quantity,'PC' AS unit,E.QTY as CartonQty,F.QTY as palletno,'{CtnQTY}' as CartonNo,
            //'{Weight}' as grossweight,'0' as childCartonNo,'0' as childgrossweight,'' as dimension, 
            //'S' as status,'{COO}' AS COO,'{TrailNum}'AS TrailerNumber ,F.PACK_NO,A.WORKORDERNO,G.PONO as po_number,
            //H.DELIVERYNUMBER AS SALESORDER,'0' AS ShipSet,G.POLINE AS LineID,(G.UNITPRICE * G.Qty) as amoumt,'USD' as Currency,sysdate as lasteditdt,'{Lasteditby}' as lasteditby
            //FROM  R_WO_BASE A,R_WO_GROUPID B, R_SN C, R_SN_PACKING D,R_PACKING E,R_PACKING F,O_ORDER_MAIN G,R_I282 H
            //WHERE F.PACK_NO in('{PackNO}') and a.WORKORDERNO =b.WO and G.PREWO=A.WORKORDERNO AND G.PREASN=H.MESSAGEID
            //and  a.WORKORDERNO=c.WORKORDERNO and c.id=D.sn_id and D.pack_id=e.id and e.parent_pack_id=f.id
            //) A";

            //Improve Truck Load Two record exist PL
            //string sql = $@"INSERT INTO SFCRUNTIME.R_JUNIPER_MFPACKINGLIST 
            //(ID,INVOICENO,TYPE_IU,SKUNO,GROUPID,MAINPARTNO,QUANTITY,UNIT,CARTONQTY,PALLETNO,CARTONNO,GROSSWEIGHT,CHILDCARTONNO,CHILDGROSSWEIGHT,DIMENSION,STATUS,COO,TRAILERNUMBER,PALLETID,WORKORDERNO,PO_NUMBER,SALESORDER,SHIPSET,LINEID,AMOUNT,CURRENCY,LASTEDITDT,LASTEDITBY)
            //SELECT DISTINCT * FROM (
            //SELECT '{StrID}' AS ID,'{TONO}' as invoiceno,'U' AS type_iu, B.SKUNO,B.GROUPID,'' as mainPartNo,
            //k.quantity,'PC' AS unit, K.CartonQty ,K.palletno,'{CtnQTY}' as CartonNo,
            //'{Weight}' as grossweight,'0' as childCartonNo,'0' as childgrossweight,'' as dimension,
            //'S' as status,'{COO}' AS COO,'{TrailNum}'AS TrailerNumber, K.PACK_NO,A.WORKORDERNO,G.PONO as po_number,
            //H.DELIVERYNUMBER AS SALESORDER,'0' AS ShipSet, G.POLINE AS LineID,(G.UNITPRICE * '{StrDnQty}') as amoumt,
            //'USD' as Currency,sysdate as lasteditdt,'{Lasteditby}' as lasteditby FROM(
            //SELECT WORKORDERNO, '{StrDnQty}' as quantity, unit, SUM(CartonQty) as CartonQty, palletno, PACK_NO  FROM(
            //SELECT distinct * FROM(
            //SELECT C.WORKORDERNO, E.QTY as quantity, 'PC' AS unit, E.QTY as CartonQty, '1' as palletno, F.PACK_NO
            //FROM  R_SN C, R_SN_PACKING D, R_PACKING E, R_PACKING F
            //WHERE F.PACK_NO in ('{PackNO}')  and c.id = D.sn_id and D.pack_id = e.id and e.parent_pack_id = f.id
            //))A group by WORKORDERNO, unit, palletno, PACK_NO)K,R_WO_BASE A, R_WO_GROUPID B,O_ORDER_MAIN G, R_I282 H
            //WHERE a.WORKORDERNO = b.WO and G.PREWO = A.WORKORDERNO and K.WORKORDERNO = a.WORKORDERNO AND G.PREASN = H.MESSAGEID
            //) M";


            sql = $@"INSERT INTO SFCRUNTIME.R_JUNIPER_MFPACKINGLIST
                      (ID,
                       INVOICENO,
                       TYPE_IU,
                       SKUNO,
                       GROUPID,
                       MAINPARTNO,
                       QUANTITY,
                       UNIT,
                       CARTONQTY,
                       PALLETNO,
                       CARTONNO,
                       GROSSWEIGHT,
                       CHILDCARTONNO,
                       CHILDGROSSWEIGHT,
                       DIMENSION,
                       STATUS,
                       COO,
                       TRAILERNUMBER,
                       PALLETID,
                       WORKORDERNO,
                       PO_NUMBER,
                       SALESORDER,
                       SHIPSET,
                       LINEID,
                       AMOUNT,
                       CURRENCY,
                       LASTEDITDT,
                       LASTEDITBY)
                      SELECT K.ID,
                             K.INVOICENO,
                             K.TYPE_IU,
                             K.SKUNO,
                             K.GROUPID,
                             K.MAINPARTNO,
                             K.QUANTITY,
                             K.UNIT,
                             K.CARTONQTY,
                             K.PALLETNO,
                             K.CARTONNO,
                             K.GROSSWEIGHT,
                             K.CHILDCARTONNO,
                             K.CHILDGROSSWEIGHT,
                             K.DIMENSION,
                             K.STATUS,
                             K.COO,
                             K.TRAILERNUMBER,
                             K.PACK_NO          as PALLETID,
                             K.WORKORDERNO,
                             K.PO_NUMBER,
                             K.SALESORDER,
                             K.SHIPSET,
                             K.LINEID,
                             K.AMOUMT,
                             K.CURRENCY,
                             SYSDATE            AS LASTEDITDT,
                             K.LASTEDITBY
                        FROM (SELECT DISTINCT *
                                FROM (SELECT '{StrID}' AS ID,
                                             '{TONO}' as invoiceno,
                                             'U' AS type_iu,
                                             B.SKUNO,
                                             B.GROUPID,
                                             '' as mainPartNo,
                                             k.quantity,
                                             'PC' AS unit,
                                             K.CartonQty,
                                             K.palletno,
                                             '{CtnQTY}' as CartonNo,
                                             '{Weight}' as grossweight,
                                             '0' as childCartonNo,
                                             '0' as childgrossweight,
                                             '' as dimension,
                                             'S' as status,
                                             '{COO}' AS COO,
                                             '{TrailNum}' AS TrailerNumber,
                                             '{Newpl}' as PACK_NO,
                                             A.WORKORDERNO,
                                             G.PONO as po_number,
                                             NVL(H.DELIVERYNUMBER,HH.DELIVERYNUMBER) AS SALESORDER,
                                             '0' AS ShipSet,
                                             G.POLINE AS LineID,
                                             (G.UNITPRICE * '{StrDnQty}') as amoumt,
                                             'USD' as Currency,
                                             NVL(H.F_LASTEDITDT,HH.CREATETIME)as lasteditdt,
                                             '{Lasteditby}' as lasteditby
                                        FROM (SELECT WORKORDERNO,
                                                     '{StrDnQty}' as quantity,
                                                     unit,
                                                     floor(('{StrDnQty}' / '{CtnQTY}')) as CartonQty,
                                                     palletno,
                                                     PACK_NO
                                                FROM (SELECT distinct *
                                                        FROM (SELECT C.WORKORDERNO,
                                                                     E.QTY as quantity,
                                                                     'PC' AS unit,
                                                                     E.QTY as CartonQty,
                                                                     '{plqtys}' as palletno,
                                                                     F.PACK_NO
                                                                FROM R_SN         C,
                                                                     R_SN_PACKING D,
                                                                     R_PACKING    E,
                                                                     R_PACKING    F
                                                               WHERE F.PACK_NO in ('{PackNO}')
                                                                 and c.id = D.sn_id
                                                                 and D.pack_id = e.id
                                                                 and e.parent_pack_id = f.id)) A
                                               group by WORKORDERNO, unit, palletno, PACK_NO) K,
                                             R_WO_BASE A,
                                             R_WO_GROUPID B,
                                             O_ORDER_MAIN G
                                             LEFT JOIN
                                             R_I282 H ON G.PREASN = H.MESSAGEID
                                             LEFT JOIN
                                             R_JNP_DOA_SHIPMENTS_ACK HH ON G.PREASN = HH.ASNNUMBER
                                       WHERE a.WORKORDERNO = b.WO
                                         and G.PREWO = A.WORKORDERNO
                                         and K.WORKORDERNO = a.WORKORDERNO
                                       ORDER BY lasteditdt DESC) M
                               WHERE ROWNUM < 2) K";

            int Number = DB.ExecSqlNoReturn(sql, null);
            if (Number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ClosedTOB2BFlag(string TONO, string Lasteditby, string BU, OleExec DB)
        {
            T_R_JUNIPER_MFPACKINGLIST LOTDETAIL = new T_R_JUNIPER_MFPACKINGLIST(DB, DBType);
  
            if (TONO == null || TONO == "")
            {
                return false;
            }

            string sql = $@" UPDATE R_JUNIPER_MFPACKINGLIST SET STATUS='N' WHERE INVOICENO='{TONO}' AND STATUS='S' ";

            int Number = DB.ExecSqlNoReturn(sql, null);
            if (Number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
    }

    public class Row_R_JUNIPER_MFPACKINGLIST : DataObjectBase
    {
        public Row_R_JUNIPER_MFPACKINGLIST(DataObjectInfo info) : base(info)
        {

        }
        public R_JUNIPER_MFPACKINGLIST GetDataObject()
        {
            R_JUNIPER_MFPACKINGLIST DataObject = new R_JUNIPER_MFPACKINGLIST();
            DataObject.ID = this.ID;
            DataObject.INVOICENO = this.INVOICENO;
            DataObject.TYPE_IU = this.TYPE_IU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.GROUPID = this.GROUPID;
            DataObject.MAINPARTNO = this.MAINPARTNO;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.UNIT = this.UNIT;
            DataObject.CARTONQTY = this.CARTONQTY;
            DataObject.PALLETNO = this.PALLETNO;
            DataObject.CARTONNO = this.CARTONNO;
            DataObject.GROSSWEIGHT = this.GROSSWEIGHT;
            DataObject.CHILDCARTONNO = this.CHILDCARTONNO;
            DataObject.CHILDGROSSWEIGHT = this.CHILDGROSSWEIGHT;
            DataObject.DIMENSION = this.DIMENSION;
            DataObject.STATUS = this.STATUS;
            DataObject.COO = this.COO;
            DataObject.TRAILERNUMBER = this.TRAILERNUMBER;
            DataObject.PALLETID = this.PALLETID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PO_NUMBER = this.PO_NUMBER;
            DataObject.SALESORDER = this.SALESORDER;
            DataObject.SHIPSET = this.SHIPSET;
            DataObject.LINEID = this.LINEID;
            DataObject.AMOUNT = this.AMOUNT;
            DataObject.CURRENCY = this.CURRENCY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
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
        public string INVOICENO
        {
            get
            {
                return (string)this["INVOICENO"];
            }
            set
            {
                this["INVOICENO"] = value;
            }
        }
        public string TYPE_IU
        {
            get
            {
                return (string)this["TYPE_IU"];
            }
            set
            {
                this["TYPE_IU"] = value;
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
        public string GROUPID
        {
            get
            {
                return (string)this["GROUPID"];
            }
            set
            {
                this["GROUPID"] = value;
            }
        }
        public string MAINPARTNO
        {
            get
            {
                return (string)this["MAINPARTNO"];
            }
            set
            {
                this["MAINPARTNO"] = value;
            }
        }
        public double? QUANTITY
        {
            get
            {
                return (double?)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public string UNIT
        {
            get
            {
                return (string)this["UNIT"];
            }
            set
            {
                this["UNIT"] = value;
            }
        }
        public double? CARTONQTY
        {
            get
            {
                return (double?)this["CARTONQTY"];
            }
            set
            {
                this["CARTONQTY"] = value;
            }
        }
        public double? PALLETNO
        {
            get
            {
                return (double?)this["PALLETNO"];
            }
            set
            {
                this["PALLETNO"] = value;
            }
        }
        public double? CARTONNO
        {
            get
            {
                return (double?)this["CARTONNO"];
            }
            set
            {
                this["CARTONNO"] = value;
            }
        }
        public string GROSSWEIGHT
        {
            get
            {
                return (string)this["GROSSWEIGHT"];
            }
            set
            {
                this["GROSSWEIGHT"] = value;
            }
        }
        public double? CHILDCARTONNO
        {
            get
            {
                return (double?)this["CHILDCARTONNO"];
            }
            set
            {
                this["CHILDCARTONNO"] = value;
            }
        }
        public string CHILDGROSSWEIGHT
        {
            get
            {
                return (string)this["CHILDGROSSWEIGHT"];
            }
            set
            {
                this["CHILDGROSSWEIGHT"] = value;
            }
        }
        public string DIMENSION
        {
            get
            {
                return (string)this["DIMENSION"];
            }
            set
            {
                this["DIMENSION"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string COO
        {
            get
            {
                return (string)this["COO"];
            }
            set
            {
                this["COO"] = value;
            }
        }
        public string TRAILERNUMBER
        {
            get
            {
                return (string)this["TRAILERNUMBER"];
            }
            set
            {
                this["TRAILERNUMBER"] = value;
            }
        }
        public string PALLETID
        {
            get
            {
                return (string)this["PALLETID"];
            }
            set
            {
                this["PALLETID"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string PO_NUMBER
        {
            get
            {
                return (string)this["PO_NUMBER"];
            }
            set
            {
                this["PO_NUMBER"] = value;
            }
        }
        public string SALESORDER
        {
            get
            {
                return (string)this["SALESORDER"];
            }
            set
            {
                this["SALESORDER"] = value;
            }
        }
        public double? SHIPSET
        {
            get
            {
                return (double?)this["SHIPSET"];
            }
            set
            {
                this["SHIPSET"] = value;
            }
        }
        public string LINEID
        {
            get
            {
                return (string)this["LINEID"];
            }
            set
            {
                this["LINEID"] = value;
            }
        }
        public string AMOUNT
        {
            get
            {
                return (string)this["AMOUNT"];
            }
            set
            {
                this["AMOUNT"] = value;
            }
        }
        public string CURRENCY
        {
            get
            {
                return (string)this["CURRENCY"];
            }
            set
            {
                this["CURRENCY"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
    }
    public class R_JUNIPER_MFPACKINGLIST
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string INVOICENO { get; set; }
        public string TYPE_IU { get; set; }
        public string SKUNO { get; set; }
        public string GROUPID { get; set; }
        public string MAINPARTNO { get; set; }
        public double? QUANTITY { get; set; }
        public string UNIT { get; set; }
        public double? CARTONQTY { get; set; }
        public double? PALLETNO { get; set; }
        public double? CARTONNO { get; set; }
        public string GROSSWEIGHT { get; set; }
        public double? CHILDCARTONNO { get; set; }
        public string CHILDGROSSWEIGHT { get; set; }
        public string DIMENSION { get; set; }
        public string STATUS { get; set; }
        public string COO { get; set; }
        public string TRAILERNUMBER { get; set; }
        public string PALLETID { get; set; }
        public string WORKORDERNO { get; set; }
        public string PO_NUMBER { get; set; }
        public string SALESORDER { get; set; }
        public double? SHIPSET { get; set; }
        public string LINEID { get; set; }
        public string AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
    }
}