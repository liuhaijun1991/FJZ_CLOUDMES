using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_SCRAP : DataObjectTable
    {
        public T_R_QUACK_SCRAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_SCRAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_SCRAP);
            TableName = "R_QUACK_SCRAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW SN查询
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SCRAP> GetBYSN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SCRAP>().Where(t => t.SN == SN).ToList();
        }
        /// <summary>
        /// WZW SN查询ORDER BY DTODT 返回第一笔记录
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_QUACK_SCRAP GetBYSNFirstOrDefault(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SCRAP>().Where(t => t.SN == SN).OrderBy(t => t.DTO_DATE, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }
    }
    public class Row_R_QUACK_SCRAP : DataObjectBase
    {
        public Row_R_QUACK_SCRAP(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_SCRAP GetDataObject()
        {
            R_QUACK_SCRAP DataObject = new R_QUACK_SCRAP();
            DataObject.ID = this.ID;
            DataObject.OLD_QSN = this.OLD_QSN;
            DataObject.SN = this.SN;
            DataObject.NEW_QSN = this.NEW_QSN;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.SCRAP_CAUSE = this.SCRAP_CAUSE;
            DataObject.RFROM_ID = this.RFROM_ID;
            DataObject.RFROM_DATE = this.RFROM_DATE;
            DataObject.RTO_ID = this.RTO_ID;
            DataObject.RTO_DATE = this.RTO_DATE;
            DataObject.DFROM_ID = this.DFROM_ID;
            DataObject.DFROM_DATE = this.DFROM_DATE;
            DataObject.DTO_ID = this.DTO_ID;
            DataObject.DTO_DATE = this.DTO_DATE;
            DataObject.ISCOST_FLAG = this.ISCOST_FLAG;
            DataObject.SCRAP_COMPLETED_FLAG = this.SCRAP_COMPLETED_FLAG;
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
        public string OLD_QSN
        {
            get
            {
                return (string)this["OLD_QSN"];
            }
            set
            {
                this["OLD_QSN"] = value;
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
        public string NEW_QSN
        {
            get
            {
                return (string)this["NEW_QSN"];
            }
            set
            {
                this["NEW_QSN"] = value;
            }
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string SCRAP_CAUSE
        {
            get
            {
                return (string)this["SCRAP_CAUSE"];
            }
            set
            {
                this["SCRAP_CAUSE"] = value;
            }
        }
        public string RFROM_ID
        {
            get
            {
                return (string)this["RFROM_ID"];
            }
            set
            {
                this["RFROM_ID"] = value;
            }
        }
        public DateTime? RFROM_DATE
        {
            get
            {
                return (DateTime?)this["RFROM_DATE"];
            }
            set
            {
                this["RFROM_DATE"] = value;
            }
        }
        public string RTO_ID
        {
            get
            {
                return (string)this["RTO_ID"];
            }
            set
            {
                this["RTO_ID"] = value;
            }
        }
        public DateTime? RTO_DATE
        {
            get
            {
                return (DateTime?)this["RTO_DATE"];
            }
            set
            {
                this["RTO_DATE"] = value;
            }
        }
        public string DFROM_ID
        {
            get
            {
                return (string)this["DFROM_ID"];
            }
            set
            {
                this["DFROM_ID"] = value;
            }
        }
        public DateTime? DFROM_DATE
        {
            get
            {
                return (DateTime?)this["DFROM_DATE"];
            }
            set
            {
                this["DFROM_DATE"] = value;
            }
        }
        public string DTO_ID
        {
            get
            {
                return (string)this["DTO_ID"];
            }
            set
            {
                this["DTO_ID"] = value;
            }
        }
        public DateTime? DTO_DATE
        {
            get
            {
                return (DateTime?)this["DTO_DATE"];
            }
            set
            {
                this["DTO_DATE"] = value;
            }
        }
        public string ISCOST_FLAG
        {
            get
            {
                return (string)this["ISCOST_FLAG"];
            }
            set
            {
                this["ISCOST_FLAG"] = value;
            }
        }
        public string SCRAP_COMPLETED_FLAG
        {
            get
            {
                return (string)this["SCRAP_COMPLETED_FLAG"];
            }
            set
            {
                this["SCRAP_COMPLETED_FLAG"] = value;
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
    public class R_QUACK_SCRAP
    {
        public string ID { get; set; }
        public string OLD_QSN { get; set; }
        public string SN { get; set; }
        public string NEW_QSN { get; set; }
        public string LOT_NO { get; set; }
        public string SCRAP_CAUSE { get; set; }
        public string RFROM_ID { get; set; }
        public DateTime? RFROM_DATE { get; set; }
        public string RTO_ID { get; set; }
        public DateTime? RTO_DATE { get; set; }
        public string DFROM_ID { get; set; }
        public DateTime? DFROM_DATE { get; set; }
        public string DTO_ID { get; set; }
        public DateTime? DTO_DATE { get; set; }
        public string ISCOST_FLAG { get; set; }
        public string SCRAP_COMPLETED_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}