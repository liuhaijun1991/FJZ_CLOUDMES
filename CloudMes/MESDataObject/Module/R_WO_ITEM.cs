using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_ITEM : DataObjectTable
    {
        public T_R_WO_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_ITEM);
            TableName = "R_WO_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckWoItemByWo(string Workorderno, string PartNo, bool Download_Auto, string ColumnName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;
            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_ITEM where AUFNR='{Workorderno}' and MATNR='{ PartNo }' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }

            }
            else
            {
                StrSql = $@"select * from R_WO_ITEM where AUFNR='{Workorderno}' and MATNR='{ PartNo }'";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DB.ExecSQL($@" delete from H_WO_ITEM where AUFNR = '{Workorderno}' and MATNR='{ PartNo }'");
                    StrSql = $@"insert into H_WO_ITEM(ID,{ ColumnName }) ";
                    StrSql = StrSql + $@" select* from R_WO_ITEM where AUFNR = '{Workorderno}'  and MATNR='{ PartNo }'";
                    StrReturnMsg = DB.ExecSQL(StrSql);
                    int.TryParse(StrReturnMsg, out n);
                    if (n > 0)
                    {
                        StrSql = $@" delete from R_WO_ITEM where AUFNR = '{Workorderno}' and MATNR='{ PartNo }'";
                        StrReturnMsg = DB.ExecSQL(StrSql);

                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }

                }
            }
            return CheckFlag;
        }

        public List<R_WO_ITEM> GetKpSetInSapList(string Workorderno, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_ITEM, R_WO_HEADER, C_KP_LIST, C_KP_List_Item>
               ((a, b, c, d)
               => a.MATNR == d.KP_PARTNO
               && d.LIST_ID == c.ID
               && c.SKUNO == b.MATNR
               && b.AUFNR == a.AUFNR)
               .Where((a, b, c, d) => a.AUFNR == Workorderno && c.FLAG == "1")
               .Select((a,b,c,d)=>new R_WO_ITEM{
                   ID=a.ID,
                   AUFNR=a.AUFNR,
                   POSNR=a.POSNR,
                   MATNR=a.MATNR,
                   PARTS=a.PARTS,
                   KDMAT=a.KDMAT,
                   BDMNG=a.BDMNG,
                   MEINS=a.MEINS,
                   REVLV=a.REVLV,
                   BAUGR=a.BAUGR,
                   REPNO=a.REPNO,
                   REPPARTNO=a.REPPARTNO,
                   AUART=a.AUART,
                   AENAM=a.AENAM,
                   AEDAT=a.AEDAT,
                   MAKTX=a.MAKTX,
                   MATKL=a.MATKL,
                   WGBEZ=a.WGBEZ,
                   ALPOS=a.ALPOS,
                   ABLAD=a.ABLAD,
                   MVGR3=a.MVGR3,
                   RGEKZ=a.RGEKZ,
                   LGORT=a.LGORT,
                   ENMNG=a.ENMNG,
                   DUMPS=a.DUMPS,
                   BISMT=a.BISMT,
                   XLOEK=a.XLOEK,
                   SHKZG=a.SHKZG,
                   CHARG=a.CHARG,
                   RSPOS=a.RSPOS,
                   VORNR=a.VORNR
               })
               .ToList();
        }
        public bool CheckWoItemByWo(string Workorderno, string PartNo, bool Download_Auto, OleExec DB)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;
            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_ITEM where AUFNR='{Workorderno}' and MATNR='{ PartNo }' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_ITEM where AUFNR='{Workorderno}' and MATNR='{ PartNo }'";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DB.ExecSQL($@" delete from H_WO_ITEM where AUFNR = '{Workorderno}' and MATNR='{ PartNo }'");
                    StrSql = $@"insert into H_WO_ITEM  select* from R_WO_ITEM where AUFNR = '{Workorderno}'  and MATNR='{ PartNo }'";
                    StrReturnMsg = DB.ExecSQL(StrSql);
                    int.TryParse(StrReturnMsg, out n);
                    if (n > 0)
                    {
                        StrSql = $@" delete from R_WO_ITEM where AUFNR = '{Workorderno}' and MATNR='{ PartNo }'";
                        StrReturnMsg = DB.ExecSQL(StrSql);
                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }
                }
            }
            return CheckFlag;
        }

        public string EditWoItem(string EditSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string ReturnMsg = DB.ExecSQL(EditSql);

            return ReturnMsg;
        }
       
        /// <summary>
        /// add by ZGJ 2018-03-16
        /// 判斷是否有配置該機種不需要經過TS101檢測
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="PartNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckExist(string WorkOrder, string PartNo, OleExec DB)
        {
            bool isExist = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT * FROM R_WO_ITEM WHERE AUFNR='{WorkOrder}' AND MATNR='{PartNo}'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                isExist = true;
            }
            return isExist;
        }

        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="Partno"></param>
        /// <param name="Seqno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_ITEM> GetListByWOPARTSEQ(string WO, List<string> Partno, string Seqno, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_ITEM>().Where(t => t.AUFNR == WO && Partno.Contains(t.REPPARTNO) && t.POSNR != Seqno).ToList();
        }
    }
    public class Row_R_WO_ITEM : DataObjectBase
    {
        public Row_R_WO_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_ITEM GetDataObject()
        {
            R_WO_ITEM DataObject = new R_WO_ITEM();
            DataObject.BDMNG = this.BDMNG;
            DataObject.MEINS = this.MEINS;
            DataObject.REVLV = this.REVLV;
            DataObject.BAUGR = this.BAUGR;
            DataObject.REPNO = this.REPNO;
            DataObject.REPPARTNO = this.REPPARTNO;
            DataObject.AUART = this.AUART;
            DataObject.AENAM = this.AENAM;
            DataObject.AEDAT = this.AEDAT;
            DataObject.MAKTX = this.MAKTX;
            DataObject.MATKL = this.MATKL;
            DataObject.WGBEZ = this.WGBEZ;
            DataObject.ALPOS = this.ALPOS;
            DataObject.ABLAD = this.ABLAD;
            DataObject.MVGR3 = this.MVGR3;
            DataObject.RGEKZ = this.RGEKZ;
            DataObject.LGORT = this.LGORT;
            DataObject.ENMNG = this.ENMNG;
            DataObject.DUMPS = this.DUMPS;
            DataObject.BISMT = this.BISMT;
            DataObject.XLOEK = this.XLOEK;
            DataObject.SHKZG = this.SHKZG;
            DataObject.CHARG = this.CHARG;
            DataObject.RSPOS = this.RSPOS;
            DataObject.VORNR = this.VORNR;
            DataObject.ID = this.ID;
            DataObject.AUFNR = this.AUFNR;
            DataObject.POSNR = this.POSNR;
            DataObject.MATNR = this.MATNR;
            DataObject.PARTS = this.PARTS;
            DataObject.KDMAT = this.KDMAT;
            return DataObject;
        }
        public string BDMNG
        {
            get
            {
                return (string)this["BDMNG"];
            }
            set
            {
                this["BDMNG"] = value;
            }
        }
        public string MEINS
        {
            get
            {
                return (string)this["MEINS"];
            }
            set
            {
                this["MEINS"] = value;
            }
        }
        public string REVLV
        {
            get
            {
                return (string)this["REVLV"];
            }
            set
            {
                this["REVLV"] = value;
            }
        }
        public string BAUGR
        {
            get
            {
                return (string)this["BAUGR"];
            }
            set
            {
                this["BAUGR"] = value;
            }
        }
        public string REPNO
        {
            get
            {
                return (string)this["REPNO"];
            }
            set
            {
                this["REPNO"] = value;
            }
        }
        public string REPPARTNO
        {
            get
            {
                return (string)this["REPPARTNO"];
            }
            set
            {
                this["REPPARTNO"] = value;
            }
        }
        public string AUART
        {
            get
            {
                return (string)this["AUART"];
            }
            set
            {
                this["AUART"] = value;
            }
        }
        public string AENAM
        {
            get
            {
                return (string)this["AENAM"];
            }
            set
            {
                this["AENAM"] = value;
            }
        }
        public string AEDAT
        {
            get
            {
                return (string)this["AEDAT"];
            }
            set
            {
                this["AEDAT"] = value;
            }
        }
        public string MAKTX
        {
            get
            {
                return (string)this["MAKTX"];
            }
            set
            {
                this["MAKTX"] = value;
            }
        }
        public string MATKL
        {
            get
            {
                return (string)this["MATKL"];
            }
            set
            {
                this["MATKL"] = value;
            }
        }
        public string WGBEZ
        {
            get
            {
                return (string)this["WGBEZ"];
            }
            set
            {
                this["WGBEZ"] = value;
            }
        }
        public string ALPOS
        {
            get
            {
                return (string)this["ALPOS"];
            }
            set
            {
                this["ALPOS"] = value;
            }
        }
        public string ABLAD
        {
            get
            {
                return (string)this["ABLAD"];
            }
            set
            {
                this["ABLAD"] = value;
            }
        }
        public string MVGR3
        {
            get
            {
                return (string)this["MVGR3"];
            }
            set
            {
                this["MVGR3"] = value;
            }
        }
        public string RGEKZ
        {
            get
            {
                return (string)this["RGEKZ"];
            }
            set
            {
                this["RGEKZ"] = value;
            }
        }
        public string LGORT
        {
            get
            {
                return (string)this["LGORT"];
            }
            set
            {
                this["LGORT"] = value;
            }
        }
        public string ENMNG
        {
            get
            {
                return (string)this["ENMNG"];
            }
            set
            {
                this["ENMNG"] = value;
            }
        }
        public string DUMPS
        {
            get
            {
                return (string)this["DUMPS"];
            }
            set
            {
                this["DUMPS"] = value;
            }
        }
        public string BISMT
        {
            get
            {
                return (string)this["BISMT"];
            }
            set
            {
                this["BISMT"] = value;
            }
        }
        public string XLOEK
        {
            get
            {
                return (string)this["XLOEK"];
            }
            set
            {
                this["XLOEK"] = value;
            }
        }
        public string SHKZG
        {
            get
            {
                return (string)this["SHKZG"];
            }
            set
            {
                this["SHKZG"] = value;
            }
        }
        public string CHARG
        {
            get
            {
                return (string)this["CHARG"];
            }
            set
            {
                this["CHARG"] = value;
            }
        }
        public string RSPOS
        {
            get
            {
                return (string)this["RSPOS"];
            }
            set
            {
                this["RSPOS"] = value;
            }
        }
        public string VORNR
        {
            get
            {
                return (string)this["VORNR"];
            }
            set
            {
                this["VORNR"] = value;
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
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
            }
        }
        public string POSNR
        {
            get
            {
                return (string)this["POSNR"];
            }
            set
            {
                this["POSNR"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string PARTS
        {
            get
            {
                return (string)this["PARTS"];
            }
            set
            {
                this["PARTS"] = value;
            }
        }
        public string KDMAT
        {
            get
            {
                return (string)this["KDMAT"];
            }
            set
            {
                this["KDMAT"] = value;
            }
        }
    }
    public class R_WO_ITEM
    {
        public string BDMNG{get;set;}
        public string MEINS{get;set;}
        public string REVLV{get;set;}
        public string BAUGR{get;set;}
        public string REPNO{get;set;}
        public string REPPARTNO{get;set;}
        public string AUART{get;set;}
        public string AENAM{get;set;}
        public string AEDAT{get;set;}
        public string MAKTX{get;set;}
        public string MATKL{get;set;}
        public string WGBEZ{get;set;}
        public string ALPOS{get;set;}
        public string ABLAD{get;set;}
        public string MVGR3{get;set;}
        public string RGEKZ{get;set;}
        public string LGORT{get;set;}
        public string ENMNG{get;set;}
        public string DUMPS{get;set;}
        public string BISMT{get;set;}
        public string XLOEK{get;set;}
        public string SHKZG{get;set;}
        public string CHARG{get;set;}
        public string RSPOS{get;set;}
        public string VORNR{get;set;}
        public string ID{get;set;}
        public string AUFNR{get;set;}
        public string POSNR{get;set;}
        public string MATNR{get;set;}
        public string PARTS{get;set;}
        public string KDMAT{get;set;}
    }
}