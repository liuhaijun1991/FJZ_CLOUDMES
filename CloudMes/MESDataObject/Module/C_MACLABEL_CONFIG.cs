using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    // ADD BY HGB 2019.07.16
    public class T_C_MACLABEL_CONFIG : DataObjectTable
    {
        public T_C_MACLABEL_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MACLABEL_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MACLABEL_CONFIG);
            TableName = "C_MACLABEL_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// ADD BY HGB 2019.07.16
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="controlValue"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_MACLABEL_CONFIG LoadData(string SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_MACLABEL_CONFIG>().Where(t => t.SKUNO == SKUNO).ToList().FirstOrDefault();
        }

        /// <summary>
        /// ADD BY HGB 2019.07.16
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="controlValue"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExist(string SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_MACLABEL_CONFIG>().Any(t => t.SKUNO == SKUNO);
        }

    }
    public class Row_C_MACLABEL_CONFIG : DataObjectBase
    {
        public Row_C_MACLABEL_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_MACLABEL_CONFIG GetDataObject()
        {
            C_MACLABEL_CONFIG DataObject = new C_MACLABEL_CONFIG();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.MACCATEGORY = this.MACCATEGORY;
            DataObject.STEP = this.STEP;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
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
        public string MACCATEGORY
        {
            get
            {
                return (string)this["MACCATEGORY"];
            }
            set
            {
                this["MACCATEGORY"] = value;
            }
        }
        public double? STEP
        {
            get
            {
                return (double?)this["STEP"];
            }
            set
            {
                this["STEP"] = value;
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
    public class C_MACLABEL_CONFIG
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string MACCATEGORY{get;set;}
        public double? STEP{get;set;}
        public string DATA1{get;set;}
        public string DATA2{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}