using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    //ADD BY HGB 2019.06.20
    public class T_C_SKUNO_BASE : DataObjectTable
    {
        public T_C_SKUNO_BASE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKUNO_BASE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKUNO_BASE);
            TableName = "C_SKUNO_BASE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckExists(string var_keypartno, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_SKUNO_BASE WHERE PARTNUMBER = '{var_keypartno}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKUNO_BASE LOAD_C_SKUNO_BASE(string var_keypartno, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKUNO_BASE>().Where(t => t.PARTNUMBER == var_keypartno).ToList().FirstOrDefault();
        }

        
    }
    public class Row_C_SKUNO_BASE : DataObjectBase
    {
        public Row_C_SKUNO_BASE(DataObjectInfo info) : base(info)
        {

        }
        public C_SKUNO_BASE GetDataObject()
        {
            C_SKUNO_BASE DataObject = new C_SKUNO_BASE();
            DataObject.ID = this.ID;
            DataObject.PARTNUMBER = this.PARTNUMBER;
            DataObject.MODEL = this.MODEL;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.OUTSIDE = this.OUTSIDE;
            DataObject.GROSS_WEIGHT = this.GROSS_WEIGHT;
            DataObject.CE = this.CE;
            DataObject.UL = this.UL;
            DataObject.GS = this.GS;
            DataObject.YEARS = this.YEARS;
            DataObject.WEEE = this.WEEE;
            DataObject.CCC = this.CCC;
            DataObject.REMARK = this.REMARK;
            DataObject.CLEI_NUMBER = this.CLEI_NUMBER;
            DataObject.EDIT = this.EDIT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.CARTON_TYPE = this.CARTON_TYPE;
            DataObject.UPC = this.UPC;
            DataObject.EAN = this.EAN;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA6 = this.DATA6;
            DataObject.SKUNO = this.SKUNO;
            DataObject.DESC_CHINESE = this.DESC_CHINESE;
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
        public string PARTNUMBER
        {
            get
            {
                return (string)this["PARTNUMBER"];
            }
            set
            {
                this["PARTNUMBER"] = value;
            }
        }
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
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
        public string OUTSIDE
        {
            get
            {
                return (string)this["OUTSIDE"];
            }
            set
            {
                this["OUTSIDE"] = value;
            }
        }
        public string GROSS_WEIGHT
        {
            get
            {
                return (string)this["GROSS_WEIGHT"];
            }
            set
            {
                this["GROSS_WEIGHT"] = value;
            }
        }
        public string CE
        {
            get
            {
                return (string)this["CE"];
            }
            set
            {
                this["CE"] = value;
            }
        }
        public string UL
        {
            get
            {
                return (string)this["UL"];
            }
            set
            {
                this["UL"] = value;
            }
        }
        public string GS
        {
            get
            {
                return (string)this["GS"];
            }
            set
            {
                this["GS"] = value;
            }
        }
        public string YEARS
        {
            get
            {
                return (string)this["YEARS"];
            }
            set
            {
                this["YEARS"] = value;
            }
        }
        public string WEEE
        {
            get
            {
                return (string)this["WEEE"];
            }
            set
            {
                this["WEEE"] = value;
            }
        }
        public string CCC
        {
            get
            {
                return (string)this["CCC"];
            }
            set
            {
                this["CCC"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string CLEI_NUMBER
        {
            get
            {
                return (string)this["CLEI_NUMBER"];
            }
            set
            {
                this["CLEI_NUMBER"] = value;
            }
        }
        public string EDIT
        {
            get
            {
                return (string)this["EDIT"];
            }
            set
            {
                this["EDIT"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string CARTON_TYPE
        {
            get
            {
                return (string)this["CARTON_TYPE"];
            }
            set
            {
                this["CARTON_TYPE"] = value;
            }
        }
        public string EAN
        {
            get
            {
                return (string)this["EAN"];
            }
            set
            {
                this["EAN"] = value;
            }
        }

        public string UPC
        {
            get
            {
                return (string)this["UPC"];
            }
            set
            {
                this["UPC"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
            }
        }
        public string DATA5
        {
            get
            {
                return (string)this["DATA5"];
            }
            set
            {
                this["DATA5"] = value;
            }
        }
        public string DATA6
        {
            get
            {
                return (string)this["DATA6"];
            }
            set
            {
                this["DATA6"] = value;
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
        public string DESC_CHINESE
        {
            get
            {
                return (string)this["DESC_CHINESE"];
            }
            set
            {
                this["DESC_CHINESE"] = value;
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
    public class C_SKUNO_BASE
    {
        public string ID { get; set; }
        public string PARTNUMBER { get; set; }
        public string MODEL { get; set; }
        public string DESCRIPTION { get; set; }
        public string OUTSIDE { get; set; }
        public string GROSS_WEIGHT { get; set; }
        public string CE { get; set; }
        public string UL { get; set; }
        public string GS { get; set; }
        public string YEARS { get; set; }
        public string WEEE { get; set; }
        public string CCC { get; set; }
        public string REMARK { get; set; }
        public string CLEI_NUMBER { get; set; }
        public string EDIT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string CARTON_TYPE { get; set; }
        public string EAN { get; set; }
        public string UPC { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public string SKUNO { get; set; }
        public string DESC_CHINESE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}