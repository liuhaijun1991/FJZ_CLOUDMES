using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MOVE_LIST : DataObjectTable
    {
        public T_R_MOVE_LIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MOVE_LIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MOVE_LIST);
            TableName = "R_MOVE_LIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int SaveMoveList(OleExec sfcdb,string BU,string moveValue,string type,string fromLocation,string toLocation,string user)
        {                    
            R_MOVE_LIST moveList = new R_MOVE_LIST();
            moveList.ID = GetNewID(BU, sfcdb);
            moveList.MOVE_ID = moveValue;
            moveList.FROM_LOCATION = fromLocation;
            moveList.TO_LOCATION = toLocation;
            moveList.PACK_TYPE = type;
            moveList.MOVE_EMP = user;
            moveList.MOVE_DATE = GetDBDateTime(sfcdb);
            return sfcdb.ORM.Insertable<R_MOVE_LIST>(moveList).ExecuteCommand();
        }
    }
    public class Row_R_MOVE_LIST : DataObjectBase
    {
        public Row_R_MOVE_LIST(DataObjectInfo info) : base(info)
        {

        }
        public R_MOVE_LIST GetDataObject()
        {
            R_MOVE_LIST DataObject = new R_MOVE_LIST();
            DataObject.MOVE_DATE = this.MOVE_DATE;
            DataObject.MOVE_EMP = this.MOVE_EMP;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.TO_LOCATION = this.TO_LOCATION;
            DataObject.FROM_LOCATION = this.FROM_LOCATION;
            DataObject.MOVE_ID = this.MOVE_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public DateTime? MOVE_DATE
        {
            get
            {
                return (DateTime?)this["MOVE_DATE"];
            }
            set
            {
                this["MOVE_DATE"] = value;
            }
        }
        public string MOVE_EMP
        {
            get
            {
                return (string)this["MOVE_EMP"];
            }
            set
            {
                this["MOVE_EMP"] = value;
            }
        }
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public string TO_LOCATION
        {
            get
            {
                return (string)this["TO_LOCATION"];
            }
            set
            {
                this["TO_LOCATION"] = value;
            }
        }
        public string FROM_LOCATION
        {
            get
            {
                return (string)this["FROM_LOCATION"];
            }
            set
            {
                this["FROM_LOCATION"] = value;
            }
        }
        public string MOVE_ID
        {
            get
            {
                return (string)this["MOVE_ID"];
            }
            set
            {
                this["MOVE_ID"] = value;
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
    }
    public class R_MOVE_LIST
    {
        public DateTime? MOVE_DATE{get;set;}
        public string MOVE_EMP{get;set;}
        public string PACK_TYPE{get;set;}
        public string TO_LOCATION{get;set;}
        public string FROM_LOCATION{get;set;}
        public string MOVE_ID{get;set;}
        public string ID{get;set;}
    }
}