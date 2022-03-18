using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 

namespace MESDataObject.Module
{
    public class T_mfsyscserial : DataObjectTable
    {
        public T_mfsyscserial(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_mfsyscserial(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_mfsyscserial);
            TableName = "mfsyscserial".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckFNNMAC(string StrPN, string StrSN, string StrMAC, OleExec DB)
        {
            //mfsyscserial mfsyscserial = null;
            //Row_mfsyscserial Row_mfsyscserial = (Row_mfsyscserial)NewRow();
            DataTable Dt = new DataTable();          
            string StrSql = $@"select * from mfsyscserial where sysserialno in (select cserialno from mfsyscserial where sysserialno in ( select cserialno  from mfsyscserial where sysserialno in (select value from R_SN_KP where SN = '{StrSN}' and SCANTYPE = 'SystemSN')))
and custpartno = '{StrPN}' and cserialno = '{StrMAC}' UNION  select * from mfsyscserial where sysserialno in ( select cserialno  from mfsyscserial where sysserialno in (select value from R_SN_KP where SN = '{StrSN}' ))
 and cserialno = '{StrMAC}'";            
            Dt = DB.ExecSelect(StrSql).Tables[0];          
            if (Dt.Rows.Count > 0 )
            {
                //Row_mfsyscserial.loadData(Dt.Rows[0]);
                //mfsyscserial = Row_mfsyscserial.GetDataObject();
                return true;
            }

            return false;
        }

        public mfsyscserial GetFNNMAC(string StrSN, string labelname, OleExec DB)
        {
            mfsyscserial mfsyscserial = null;
            Row_mfsyscserial Row_mfsyscserial = (Row_mfsyscserial)NewRow();
            DataTable Dt = new DataTable();
            //20190307 patty very urgent to fix in order to get MAC from FNN for side label, will modifiy the code later
            string StrSql = "";
            if (labelname.Contains("SIDE_A")) //side A
            {
                StrSql = $@"select *  from mfsyscserial where sysserialno in(
select cserialno from mfsyscserial where sysserialno in (select value from R_SN_KP where  SN = '{StrSN}' and scantype = 'SystemSN' and exvalue1 like '%SMOD0%')
)and EEECODE like '%MAC%'";
            }
            else
            {
                StrSql = $@"select *  from mfsyscserial where sysserialno in(
select cserialno from mfsyscserial where sysserialno in (select value from R_SN_KP where  SN = '{StrSN}' and scantype = 'SystemSN' and exvalue1 like '%SMOD1%')
)and EEECODE like '%MAC%'";
            }

            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_mfsyscserial.loadData(Dt.Rows[0]);
                mfsyscserial = Row_mfsyscserial.GetDataObject();
            }
            else
            {
                return null;
            }

            return mfsyscserial;
        }

        public bool ChkFNNUnitSN(string FNNSN, OleExec SFCDB)
        {
            List<mfsyscserial> mfsyscserial = new List<mfsyscserial>();
            mfsyscserial = SFCDB.ORM.Queryable<mfsyscserial>().Where(t => t.SYSSERIALNO == FNNSN).ToList();
            if (mfsyscserial.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool ChkFNNCPN(int StrPNlevel, string StrSystemSN, string StrPN, int StrPNQty, OleExec DB)
        {
            //mfsyscserial mfsyscserial = null;
            Row_mfsyscserial Row_mfsyscserial = (Row_mfsyscserial)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = "";
            if (StrPNlevel.ToString() == "1")
            {
                StrSql = $@"select PARTNO, count(distinct CSERIALNO) from MFSYSCSERIAL  where SYSSERIALNO = '{StrSystemSN}' and PARTNO = '{StrPN}' group by PARTNO having count(distinct cserialno) = {StrPNQty}";
            }
            if (StrPNlevel.ToString() == "2")
            {
                StrSql = $@"select PARTNO, count(distinct CSERIALNO) from MFSYSCSERIAL where SYSSERIALNO in
(select CSERIALNO from MFSYSCSERIAL  where SYSSERIALNO = '{StrSystemSN}')  and PARTNO = '{StrPN}' group by PARTNO having count(distinct cserialno) = {StrPNQty}";
            }
            
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_mfsyscserial : DataObjectBase
    {
        public Row_mfsyscserial(DataObjectInfo info) : base(info)
        {

        }
        public mfsyscserial GetDataObject()
        {
            mfsyscserial DataObject = new mfsyscserial();
            DataObject.EEECODE = this.EEECODE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.SEQNO = this.SEQNO;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.PRODCATEGORYNAME = this.PRODCATEGORYNAME;
            DataObject.PRODTYPE = this.PRODTYPE;
            DataObject.ORIGINALCSN = this.ORIGINALCSN;
            DataObject.SCANBY = this.SCANBY;
            DataObject.SCANDT = this.SCANDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.MDSGET = this.MDSGET;
            DataObject.MPN = this.MPN;
            DataObject.OLDMPN = this.OLDMPN;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.CSERIALNO = this.CSERIALNO;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            return DataObject;
        }
        public string EEECODE
        {
            get
            {
                return (string)this["EEECODE"];
            }
            set
            {
                this["EEECODE"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string CATEGORYNAME
        {
            get
            {
                return (string)this["CATEGORYNAME"];
            }
            set
            {
                this["CATEGORYNAME"] = value;
            }
        }
        public string PRODCATEGORYNAME
        {
            get
            {
                return (string)this["PRODCATEGORYNAME"];
            }
            set
            {
                this["PRODCATEGORYNAME"] = value;
            }
        }
        public string PRODTYPE
        {
            get
            {
                return (string)this["PRODTYPE"];
            }
            set
            {
                this["PRODTYPE"] = value;
            }
        }
        public string ORIGINALCSN
        {
            get
            {
                return (string)this["ORIGINALCSN"];
            }
            set
            {
                this["ORIGINALCSN"] = value;
            }
        }
        public string SCANBY
        {
            get
            {
                return (string)this["SCANBY"];
            }
            set
            {
                this["SCANBY"] = value;
            }
        }
        public string SCANDT
        {
            get
            {
                return (string)this["SCANDT"];
            }
            set
            {
                this["SCANDT"] = value;
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
        public string MDSGET
        {
            get
            {
                return (string)this["MDSGET"];
            }
            set
            {
                this["MDSGET"] = value;
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
        public string OLDMPN
        {
            get
            {
                return (string)this["OLDMPN"];
            }
            set
            {
                this["OLDMPN"] = value;
            }
        }
        public string SYSSERIALNO
        {
            get
            {
                return (string)this["SYSSERIALNO"];
            }
            set
            {
                this["SYSSERIALNO"] = value;
            }
        }
        public string CSERIALNO
        {
            get
            {
                return (string)this["CSERIALNO"];
            }
            set
            {
                this["CSERIALNO"] = value;
            }
        }
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
            }
        }
    }
    public class mfsyscserial
    {
        public string EEECODE { get; set; }
        public string PARTNO { get; set; }
        public double? SEQNO { get; set; }
        public string CATEGORYNAME { get; set; }
        public string PRODCATEGORYNAME { get; set; }
        public string PRODTYPE { get; set; }
        public string ORIGINALCSN { get; set; }
        public string SCANBY { get; set; }
        public string SCANDT { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string MDSGET { get; set; }
        public string MPN { get; set; }
        public string OLDMPN { get; set; }
        public string SYSSERIALNO { get; set; }
        public string CSERIALNO { get; set; }
        public string EVENTPOINT { get; set; }
        public string CUSTPARTNO { get; set; }
    }

}
