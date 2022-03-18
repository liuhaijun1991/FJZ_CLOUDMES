using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ORACLE_MFASSEMBLYDATA : DataObjectTable
    {
        public T_C_ORACLE_MFASSEMBLYDATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ORACLE_MFASSEMBLYDATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ORACLE_MFASSEMBLYDATA);
            TableName = "C_ORACLE_MFASSEMBLYDATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_ORACLE_MFASSEMBLYDATA> GetLocations(string sku, string pn, OleExec SFCDB)
        {
            List<C_ORACLE_MFASSEMBLYDATA> C_ORACLE_MFASSEMBLYDATA = new List<C_ORACLE_MFASSEMBLYDATA>();
          
            if (sku.Contains("SM"))
            {
                sku = sku.Replace("SM","");
            }
            C_ORACLE_MFASSEMBLYDATA = SFCDB.ORM.Queryable<C_ORACLE_MFASSEMBLYDATA>().Where(t => t.CONFIGHEADERID == sku && t.CUSTPARTNO == pn).ToList();

            return C_ORACLE_MFASSEMBLYDATA;
        }
        public List<C_ORACLE_MFASSEMBLYDATA> _GetMappingDetail(OleExec sfcdb, string configheaderid, string custpartno, string location)
        {
            DataTable dt = null;
            Row_C_ORACLE_MFASSEMBLYDATA row_mapping = null;
            List<C_ORACLE_MFASSEMBLYDATA> mapping = new List<C_ORACLE_MFASSEMBLYDATA>();


            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where 1=1 ";
                if (!string.IsNullOrEmpty(configheaderid))
                {
                    sql += $@"and CONFIGHEADERID like '%{configheaderid.Replace("'", "''")}%' ";
                }
                if (!string.IsNullOrEmpty(custpartno))
                {
                    sql += $@"and CUSTPARTNO like '%{custpartno.Replace("'", "''")}%' ";
                }
                if (!string.IsNullOrEmpty(location))
                {
                    sql += $@"and LOCATION like '%{location.Replace("'", "''")}%' ";
                }
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_mapping = (Row_C_ORACLE_MFASSEMBLYDATA)NewRow();
                        row_mapping.loadData(dr);
                        mapping.Add(row_mapping.GetDataObject());
                    }
                    return mapping;
                }
                catch (Exception ex)
                {

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }

        }

        public List<C_ORACLE_MFASSEMBLYDATA> GetAllAssemblyMappingData(OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_ORACLE_MFASSEMBLYDATA> Ret = new List<C_ORACLE_MFASSEMBLYDATA>();
            string StrSql = $@"select * from C_ORACLE_MFASSEMBLYDATA order by CONFIGHEADERID desc ";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_ORACLE_MFASSEMBLYDATA Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }

        public C_ORACLE_MFASSEMBLYDATA GetRow(DataRow DR)
        {
            Row_C_ORACLE_MFASSEMBLYDATA Ret = (Row_C_ORACLE_MFASSEMBLYDATA)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }

        public Row_C_ORACLE_MFASSEMBLYDATA GetMappingBySKUPN(string sku, string pn, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_ORACLE_MFASSEMBLYDATA where CONFIGHEADERID='{sku}' and custpartno = '{pn}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_ORACLE_MFASSEMBLYDATA ret = (Row_C_ORACLE_MFASSEMBLYDATA)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

    }
    public class Row_C_ORACLE_MFASSEMBLYDATA : DataObjectBase
    {
        public Row_C_ORACLE_MFASSEMBLYDATA(DataObjectInfo info) : base(info)
        {

        }
        public C_ORACLE_MFASSEMBLYDATA GetDataObject()
        {
            C_ORACLE_MFASSEMBLYDATA DataObject = new C_ORACLE_MFASSEMBLYDATA();
            DataObject.ID = this.ID;
            DataObject.CONFIGHEADERID = this.CONFIGHEADERID;
            DataObject.REV = this.REV;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.QTY = this.QTY;
            DataObject.LOCATION = this.LOCATION;
            DataObject.LASTEDITTIME = this.LASTEDITTIME;
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
        public string CONFIGHEADERID
        {
            get
            {
                return (string)this["CONFIGHEADERID"];
            }
            set
            {
                this["CONFIGHEADERID"] = value;
            }
        }
        public string REV
        {
            get
            {
                return (string)this["REV"];
            }
            set
            {
                this["REV"] = value;
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
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public DateTime? LASTEDITTIME
        {
            get
            {
                return (DateTime?)this["LASTEDITTIME"];
            }
            set
            {
                this["LASTEDITTIME"] = value;
            }
        }
    }
    public class C_ORACLE_MFASSEMBLYDATA
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string CONFIGHEADERID { get; set; }
        public string REV { get; set; }
        public string CUSTPARTNO { get; set; }
        public string DESCRIPTION { get; set; }
        public string QTY { get; set; }
        public string LOCATION { get; set; }
        public DateTime? LASTEDITTIME { get; set; }
    }
}