using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CARRIER_SKUNO_LINK : DataObjectTable
    {
        public T_R_CARRIER_SKUNO_LINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CARRIER_SKUNO_LINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CARRIER_SKUNO_LINK);
            TableName = "R_CARRIER_SKUNO_LINK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_CARRIER_SKUNO_LINK GET_byID(string id, OleExec DB)
        {
            List<R_CARRIER_SKUNO_LINK> Sds = DB.ORM.Queryable<R_CARRIER_SKUNO_LINK>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }

        public List<R_CARRIER_SKUNO_LINK> GetListByCarrierSkuno(string CARRIER_SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_CARRIER_SKUNO_LINK>().Where(it => it.CARRIER_SKUNO == CARRIER_SKUNO)
                .OrderBy(it => it.SKUNO)
                .ToList();
        }

        public int AddOrUpdateRCSL(string Operation, R_CARRIER_SKUNO_LINK RCSL, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<R_CARRIER_SKUNO_LINK>(RCSL).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<R_CARRIER_SKUNO_LINK>(RCSL).Where(sd => sd.ID == RCSL.ID).ExecuteCommand();
                    break;
            }
            return result;
        }

        public int DeleteById(string ID, OleExec DB)
        {
            int result = DB.ORM.Deleteable<R_CARRIER_SKUNO_LINK>().Where(it => it.ID == ID).ExecuteCommand();
            return result;
        }

        public int DeleteByCarrierSkuno(string CARRIER_SKUNO, OleExec DB)
        {
            int result = DB.ORM.Deleteable<R_CARRIER_SKUNO_LINK>().Where(it => it.CARRIER_SKUNO == CARRIER_SKUNO).ExecuteCommand();
            return result;
        }

        public bool checkExist(string CARRIER_SKUNO, string SKUNO,OleExec DB)
        {
            return DB.ORM.Queryable<R_CARRIER_SKUNO_LINK>().Where(it => it.CARRIER_SKUNO == CARRIER_SKUNO&&it.SKUNO==SKUNO)
                .Any();
        }
        public bool checkExistBySkuno(string SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_CARRIER_SKUNO_LINK>().Where(it =>it.SKUNO == SKUNO)
                .Any();
        }
    }
    public class Row_R_CARRIER_SKUNO_LINK : DataObjectBase
    {
        public Row_R_CARRIER_SKUNO_LINK(DataObjectInfo info) : base(info)
        {

        }
        public R_CARRIER_SKUNO_LINK GetDataObject()
        {
            R_CARRIER_SKUNO_LINK DataObject = new R_CARRIER_SKUNO_LINK();
            DataObject.ID = this.ID;
            DataObject.CARRIER_SKUNO = this.CARRIER_SKUNO;
            DataObject.SKUNO = this.SKUNO;


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
        public string CARRIER_SKUNO
        {
            get
            {
                return (string)this["CARRIER_SKUNO"];
            }
            set
            {
                this["CARRIER_SKUNO"] = value;
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

    }
    public class R_CARRIER_SKUNO_LINK
    {
        public string ID { get; set; }
        public string CARRIER_SKUNO { get; set; }
        public string SKUNO { get; set; }

    }
}