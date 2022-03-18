using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CARRIER_LINK : DataObjectTable
    {
        public T_R_CARRIER_LINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CARRIER_LINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CARRIER_LINK);
            TableName = "R_CARRIER_LINK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_CARRIER_LINK GET_byID(string id, OleExec DB)
        {
            List<R_CARRIER_LINK> Sds = DB.ORM.Queryable<R_CARRIER_LINK>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }

        public bool CheckLink(string SN, DateTime DBTIME,int ValidDays,OleExec DB)
        {
            DateTime CompareDate = DBTIME.AddDays(-ValidDays);
            return DB.ORM.Queryable<R_CARRIER_LINK>()
                .Where(it => it.SN == SN && it.EDITTIME>=CompareDate).Any();

        }
        public int GetCarrierCount(string CARRIERNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_CARRIER_LINK>().Where(cl => cl.CARRIERNO == CARRIERNO).Count();
        }

        public List<R_CARRIER_LINK> GetDataList(OleExec DB)
        {
            return DB.ORM
                    .Queryable<R_CARRIER_LINK>()
                    .Select(it => new R_CARRIER_LINK
                     {
                        SN=it.SN,
                        CARRIERNO=it.CARRIERNO,
                        EDITTIME=it.EDITTIME,
                        EDITBY=it.EDITBY })
                    .OrderBy(it => it.EDITTIME, SqlSugar.OrderByType.Desc)
                    .ToList();
        }
        public int AddOrUpdateRCL(string Operation, R_CARRIER_LINK RCL, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<R_CARRIER_LINK>(RCL).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<R_CARRIER_LINK>(RCL).Where(sd => sd.ID == RCL.ID).ExecuteCommand();
                    break;
            }
            return result;
        }
    }
    public class Row_R_CARRIER_LINK : DataObjectBase
    {
        public Row_R_CARRIER_LINK(DataObjectInfo info) : base(info)
        {

        }
        public R_CARRIER_LINK GetDataObject()
        {
            R_CARRIER_LINK DataObject = new R_CARRIER_LINK();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.CARRIERNO = this.CARRIERNO;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;

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

        public string CARRIERNO
        {
            get
            {
                return (string)this["CARRIERNO"];
            }
            set
            {
                this["CARRIERNO"] = value;
            }
        }

        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }


    }
    public class R_CARRIER_LINK
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string CARRIERNO { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }


    }
}