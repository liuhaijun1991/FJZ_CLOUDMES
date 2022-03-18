using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using Newtonsoft.Json;

namespace MESDataObject.Module
{
    public class T_R_SN_KP_MV : DataObjectTable
    {
        public T_R_SN_KP_MV(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_KP_MV(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_KP_MV);
            TableName = "R_SN_KP_MV".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void CopyKPSNBySnId(string snId, string[] stationName, string user, OleExec DB)
        {
            List<R_SN_KP> RKP = new List<R_SN_KP>();

            RKP = DB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == snId && stationName.Contains(t.STATION) && t.VALUE != " ").OrderBy(t => t.ITEMSEQ).OrderBy(t => t.SCANSEQ).OrderBy(t => t.DETAILSEQ).ToList();
            string ss = JsonConvert.SerializeObject(RKP);
            List<R_SN_KP_MV> kk = JsonConvert.DeserializeObject<List<R_SN_KP_MV>>(ss);
            DB.ORM.Insertable<R_SN_KP_MV>(kk).ExecuteCommand();
           // DB.ORM.Insertable<R_SN_KP_MV>(RKP).ExecuteCommand();

        }
    }
    public class Row_R_SN_KP_MV : DataObjectBase
    {
        public Row_R_SN_KP_MV(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_KP_MV GetDataObject()
        {
            R_SN_KP_MV DataObject = new R_SN_KP_MV();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.VALUE = this.VALUE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.KP_NAME = this.KP_NAME;
            DataObject.MPN = this.MPN;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.ITEMSEQ = this.ITEMSEQ;
            DataObject.SCANSEQ = this.SCANSEQ;
            DataObject.DETAILSEQ = this.DETAILSEQ;
            DataObject.STATION = this.STATION;
            DataObject.REGEX = this.REGEX;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EXKEY1 = this.EXKEY1;
            DataObject.EXVALUE1 = this.EXVALUE1;
            DataObject.EXKEY2 = this.EXKEY2;
            DataObject.EXVALUE2 = this.EXVALUE2;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
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
        public string KP_NAME
        {
            get
            {
                return (string)this["KP_NAME"];
            }
            set
            {
                this["KP_NAME"] = value;
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
        public string SCANTYPE
        {
            get
            {
                return (string)this["SCANTYPE"];
            }
            set
            {
                this["SCANTYPE"] = value;
            }
        }
        public double? ITEMSEQ
        {
            get
            {
                return (double?)this["ITEMSEQ"];
            }
            set
            {
                this["ITEMSEQ"] = value;
            }
        }
        public double? SCANSEQ
        {
            get
            {
                return (double?)this["SCANSEQ"];
            }
            set
            {
                this["SCANSEQ"] = value;
            }
        }
        public double? DETAILSEQ
        {
            get
            {
                return (double?)this["DETAILSEQ"];
            }
            set
            {
                this["DETAILSEQ"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string REGEX
        {
            get
            {
                return (string)this["REGEX"];
            }
            set
            {
                this["REGEX"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string EXKEY1
        {
            get
            {
                return (string)this["EXKEY1"];
            }
            set
            {
                this["EXKEY1"] = value;
            }
        }
        public string EXVALUE1
        {
            get
            {
                return (string)this["EXVALUE1"];
            }
            set
            {
                this["EXVALUE1"] = value;
            }
        }
        public string EXKEY2
        {
            get
            {
                return (string)this["EXKEY2"];
            }
            set
            {
                this["EXKEY2"] = value;
            }
        }
        public string EXVALUE2
        {
            get
            {
                return (string)this["EXVALUE2"];
            }
            set
            {
                this["EXVALUE2"] = value;
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
    public class R_SN_KP_MV
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string VALUE { get; set; }
        public string PARTNO { get; set; }
        public string KP_NAME { get; set; }
        public string MPN { get; set; }
        public string SCANTYPE { get; set; }
        public double? ITEMSEQ { get; set; }
        public double? SCANSEQ { get; set; }
        public double? DETAILSEQ { get; set; }
        public string STATION { get; set; }
        public string REGEX { get; set; }
        public double? VALID_FLAG { get; set; }
        public string EXKEY1 { get; set; }
        public string EXVALUE1 { get; set; }
        public string EXKEY2 { get; set; }
        public string EXVALUE2 { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}