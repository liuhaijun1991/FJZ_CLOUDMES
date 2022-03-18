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
    public class T_R_FUNCTION_CONTROL_EX : DataObjectTable
    {
        public T_R_FUNCTION_CONTROL_EX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FUNCTION_CONTROL_EX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FUNCTION_CONTROL_EX);
            TableName = "R_FUNCTION_CONTROL_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_F_CONTROL_EX GET_byID(string id, OleExec DB)
        {
            List<R_F_CONTROL_EX> Sds = DB.ORM.Queryable<R_F_CONTROL_EX>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }

        public R_F_CONTROL_EX ByDETAIL_ID_SEQ(string DetailId,double? SEQ, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL_EX>().Where(sd => sd.DETAIL_ID == DetailId && sd.SEQ_NO==SEQ).First();
        }
        public int AddOrUpdateRFCEX(string Operation, R_F_CONTROL_EX RFCEX, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<R_F_CONTROL_EX>(RFCEX).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<R_F_CONTROL_EX>(RFCEX).Where(sd => sd.ID == RFCEX.ID).ExecuteCommand();
                    break;
            }
            return result;
        }
    }
    public class Row_R_FUNCTION_CONTROL_EX : DataObjectBase
    {
        public Row_R_FUNCTION_CONTROL_EX(DataObjectInfo info) : base(info)
        {

        }
        public R_F_CONTROL_EX GetDataObject()
        {
            R_F_CONTROL_EX DataObject = new R_F_CONTROL_EX();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
            DataObject.DETAIL_ID = this.DETAIL_ID;

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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
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
        public string DETAIL_ID
        {
            get
            {
                return (string)this["DETAIL_ID"];
            }
            set
            {
                this["DETAIL_ID"] = value;
            }
        }
      
    }

    [SugarTable("R_FUNCTION_CONTROL_EX")]
    public class R_F_CONTROL_EX
    {
        public string ID { get; set; }
        public double? SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string DETAIL_ID { get; set; }


    }
}