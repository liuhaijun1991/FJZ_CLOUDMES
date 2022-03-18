using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_SN_KP : DataObjectTable
    {
        public T_R_QUACK_SN_KP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_SN_KP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_SN_KP);
            TableName = "R_QUACK_SN_KP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_KP> GetBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN_KP>().Where(x => x.QSN == SN).ToList();
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_KP> GetByLOT(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN_KP>().Where(x => x.SN == SN).ToList();
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="LOT"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_KP> GetByLotSN(string SN, string QSN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN_KP>().Where(x => x.SN == SN && x.QSN == QSN).ToList();
        }
		
		public int InsertRowToRQuackSnKp(R_QUACK_SN_KP RQuackSnKp, OleExec db)
        {
            return db.ORM.Insertable(RQuackSnKp).ExecuteCommand();
        }

        public R_QUACK_SN_KP GetRQuackSnKpByQsn(string Qsn,OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_SN_KP>().Where(t => t.QSN == Qsn).ToList().FirstOrDefault();
        }

        public int UpdateRowByQSn(R_QUACK_SN_KP RQuackSnKp, string Qsn, OleExec db)
        {
            return db.ORM.Updateable(RQuackSnKp).Where(t => t.QSN == Qsn).ExecuteCommand();
        }
    }
    public class Row_R_QUACK_SN_KP : DataObjectBase
    {
        public Row_R_QUACK_SN_KP(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_SN_KP GetDataObject()
        {
            R_QUACK_SN_KP DataObject = new R_QUACK_SN_KP();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.QSN = this.QSN;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.MPN = this.MPN;
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
        public string QSN
        {
            get
            {
                return (string)this["QSN"];
            }
            set
            {
                this["QSN"] = value;
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
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
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
    public class R_QUACK_SN_KP
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string QSN { get; set; }
        public string PARTNO { get; set; }
        public string CATEGORY { get; set; }
        public string MPN { get; set; }
        public double? ITEMSEQ { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}