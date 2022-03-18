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
    public class T_R_SILVER_ROTATION : DataObjectTable
    {
        public T_R_SILVER_ROTATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SILVER_ROTATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SILVER_ROTATION);
            TableName = "R_SILVER_ROTATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public R_SILVER_ROTATION GetRotationBySN(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_SILVER_ROTATION>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
        }

        public R_SILVER_ROTATION GetRotationBySNCheckStatus(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_SILVER_ROTATION>().Where(r => r.SN == sn && r.STATUS == "0" && r.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public R_SILVER_ROTATION GetCheckSnTestTime(string SN, OleExec DB)
        {
            R_SILVER_ROTATION R_Sn_Test_Detail = null;
            Row_R_SILVER_ROTATION Rows = (Row_R_SILVER_ROTATION)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"SELECT *
                          FROM R_SILVER_ROTATION
                         WHERE SN = '{SN}'
                           and VALID_FLAG = '1'
                           AND FIRST_ROTATION_TIME <
                               SYSDATE - (select value
                                            from r_function_control
                                           where functionname = 'SilverRotationControl'
                                             and category = 'ControlDay'
                                             and controlflag = 'Y'
                                             and functiontype = 'NOSYSTEM')
                        UNION ALL
                        SELECT *
                          FROM R_SILVER_ROTATION
                         WHERE SN = '{SN}'
                           and VALID_FLAG = '1'
                           AND TOLAL_ROTATION_TIMES >
                               (select value
                                  from r_function_control
                                 where functionname = 'SilverRotationControl'
                                   and category = 'ControlTime'
                                   and controlflag = 'Y'
                                   and functiontype = 'NOSYSTEM')";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Test_Detail = Rows.GetDataObject();
            }

            return R_Sn_Test_Detail;
        }

        public int UpdateRotationStatus(string sn, string status, string user, OleExec db)
        {
            DateTime sysdte = db.ORM.GetDate();
            return db.ORM.Updateable<R_SILVER_ROTATION>()
                .SetColumns(t => new R_SILVER_ROTATION { STATUS = status, EDIT_EMP = user, EDIT_TIME = sysdte, CHECK_OUT_EMP = user, CHECK_OUT_TIME = sysdte })
                .Where(t => t.SN == sn && t.VALID_FLAG == "1").ExecuteCommand();
        }

        public bool IsExist(OleExec SFCDB, string sn)
        {
            return SFCDB.ORM.Queryable<R_SILVER_ROTATION>().Where(r => r.SN == sn && r.VALID_FLAG == "1").Any();
        }

        public int InsterRotation(OleExec SFCDB, string skuno, string sn, string emp, string BU)
        {
            R_SILVER_ROTATION rotation = new R_SILVER_ROTATION();
            rotation.ID = this.GetNewID(BU, SFCDB);
            rotation.SKUNO = skuno;
            rotation.SN = sn;
            rotation.STATUS = "0";
            rotation.ROTATION_FLAG = 0;
            rotation.ROTATION_COUNT = 0;
            rotation.TOLAL_ROTATION_TIMES = 0;
            rotation.VALID_FLAG = "1";
            rotation.CHECK_IN_EMP = emp;
            rotation.CHECK_IN_TIME = SFCDB.ORM.GetDate();
            rotation.EDIT_EMP = emp;
            rotation.EDIT_TIME = rotation.CHECK_IN_TIME;
            return SFCDB.ORM.Insertable<R_SILVER_ROTATION>(rotation).ExecuteCommand();
        }
        public int UpdateRotation(OleExec SFCDB, R_SILVER_ROTATION rotation)
        {
            return SFCDB.ORM.Updateable<R_SILVER_ROTATION>(rotation).Where(r => r.ID == rotation.ID).ExecuteCommand();
        }
    }
    public class Row_R_SILVER_ROTATION : DataObjectBase
    {
        public Row_R_SILVER_ROTATION(DataObjectInfo info) : base(info)
        {

        }
        public R_SILVER_ROTATION GetDataObject()
        {
            R_SILVER_ROTATION DataObject = new R_SILVER_ROTATION();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.STATUS = this.STATUS;
            DataObject.ROTATION_FLAG = this.ROTATION_FLAG;
            DataObject.ROTATION_COUNT = this.ROTATION_COUNT;
            DataObject.FIRST_ROTATION_TIME = this.FIRST_ROTATION_TIME;
            DataObject.TOLAL_ROTATION_TIMES = this.TOLAL_ROTATION_TIMES;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.CHECK_IN_TIME = this.CHECK_IN_TIME;
            DataObject.CHECK_IN_EMP = this.CHECK_IN_EMP;
            DataObject.CHECK_OUT_TIME = this.CHECK_OUT_TIME;
            DataObject.CHECK_OUT_EMP = this.CHECK_OUT_EMP;
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
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public double? ROTATION_FLAG
        {
            get
            {
                return (double?)this["ROTATION_FLAG"];
            }
            set
            {
                this["ROTATION_FLAG"] = value;
            }
        }
        public double? ROTATION_COUNT
        {
            get
            {
                return (double?)this["ROTATION_COUNT"];
            }
            set
            {
                this["ROTATION_COUNT"] = value;
            }
        }
        public DateTime? FIRST_ROTATION_TIME
        {
            get
            {
                return (DateTime?)this["FIRST_ROTATION_TIME"];
            }
            set
            {
                this["FIRST_ROTATION_TIME"] = value;
            }
        }
        public double? TOLAL_ROTATION_TIMES
        {
            get
            {
                return (double?)this["TOLAL_ROTATION_TIMES"];
            }
            set
            {
                this["TOLAL_ROTATION_TIME"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public DateTime? CHECK_IN_TIME
        {
            get
            {
                return (DateTime?)this["CHECK_IN_TIME"];
            }
            set
            {
                this["CHECK_IN_TIME"] = value;
            }
        }
        public string CHECK_IN_EMP
        {
            get
            {
                return (string)this["CHECK_IN_EMP"];
            }
            set
            {
                this["CHECK_IN_EMP"] = value;
            }
        }
        public DateTime? CHECK_OUT_TIME
        {
            get
            {
                return (DateTime?)this["CHECK_OUT_TIME"];
            }
            set
            {
                this["CHECK_OUT_TIME"] = value;
            }
        }
        public string CHECK_OUT_EMP
        {
            get
            {
                return (string)this["CHECK_OUT_EMP"];
            }
            set
            {
                this["CHECK_OUT_EMP"] = value;
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
    public class R_SILVER_ROTATION
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string SN { get; set; }
        /// <summary>
        /// 0--check in  1--check out
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 0--wait rotation  1--rotationing
        /// </summary>
        public double? ROTATION_FLAG { get; set; }
        public double? ROTATION_COUNT { get; set; }
        public DateTime? FIRST_ROTATION_TIME { get; set; }
        public double? TOLAL_ROTATION_TIMES { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? CHECK_IN_TIME { get; set; }
        public string CHECK_IN_EMP { get; set; }
        public DateTime? CHECK_OUT_TIME { get; set; }
        public string CHECK_OUT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}