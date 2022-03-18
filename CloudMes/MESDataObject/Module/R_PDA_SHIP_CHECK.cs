using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class T_R_PDA_SHIP_CHECK : DataObjectTable
    {
        public T_R_PDA_SHIP_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PDA_SHIP_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PDA_SHIP_CHECK);
            TableName = "R_PDA_SHIP_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        /// <summary>
        /// 获取R_PDA_SHIP_CHECK的详细信息
        /// </summary>
        /// <param name="TRUCK_NO"></param>
        /// <param name="DB"></param>
        /// <return
        public List<R_PDA_SHIP_CHECK> GetAllSelect(OleExec DB)
        {
            string strsql = $@"SELECT * FROM  R_PDA_SHIP_CHECK where ROWNUM<100 order by EDIT_TIME desc ";
            List<R_PDA_SHIP_CHECK> result = new List<R_PDA_SHIP_CHECK>();
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_PDA_SHIP_CHECK ret = (Row_R_PDA_SHIP_CHECK)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取R_PDA_SHIP_CHECK的数据
        /// </summary>
        /// <param name="TRUCK_NO"></param>
        /// <param name="DB"></param>
        /// <return
        public List<R_PDA_SHIP_CHECK> GetSelectDetail(string TRUCK_SEQ, OleExec DB)
        {
            string strsql = $@"SELECT * FROM  R_PDA_SHIP_CHECK WHERE TRUCK_SEQ= '{TRUCK_SEQ }' ";
            List<R_PDA_SHIP_CHECK> result = new List<R_PDA_SHIP_CHECK>();
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_PDA_SHIP_CHECK ret = (Row_R_PDA_SHIP_CHECK)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取R_PDA_SHIP_CHECK的数据
        /// </summary>
        /// <param name="TRUCK_NO"></param>
        /// <param name="DB"></param>
        /// <return
        public List<R_PDA_SHIP_CHECK> GetSelect(string TRUCK_NO, string starttime, string finishtime, OleExec DB)
        {
            string strsql = "";
            if (TRUCK_NO != "")
            {
                strsql = $@"SELECT * FROM  R_PDA_SHIP_CHECK WHERE TRUCK_NO= '{TRUCK_NO }' ";
            }
            else
            {
                strsql = $@"SELECT * FROM  R_PDA_SHIP_CHECK WHERE EDIT_TIME BETWEEN TO_DATE('{starttime}','yyyy-mm-dd HH24:MI:SS') "
+ " AND TO_DATE('{finishtime}','yyyy-mm-dd HH24:MI:SS')  AND ROWNUM<10000 order by EDIT_TIME desc ";
            }

            List<R_PDA_SHIP_CHECK> result = new List<R_PDA_SHIP_CHECK>();
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_PDA_SHIP_CHECK ret = (Row_R_PDA_SHIP_CHECK)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除R_PDA_SHIP_CHECK的数据
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <return
        public int GetDelect(string ID, OleExec DB)
        {
            string strSql = $@"DELETE FROM R_PDA_SHIP_CHECK WHERE ID='{ID}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", ID) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
    }

    public class Row_R_PDA_SHIP_CHECK : DataObjectBase
    {
        public Row_R_PDA_SHIP_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public R_PDA_SHIP_CHECK GetDataObject()
        {
            R_PDA_SHIP_CHECK DataObject = new R_PDA_SHIP_CHECK();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.TRUCK_NO = this.TRUCK_NO;
            DataObject.TRUCK_SEQ = this.TRUCK_SEQ;
            DataObject.DN_NO = this.DN_NO;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.LOCATION = this.LOCATION;
            DataObject.SHIP_TIME = this.SHIP_TIME;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.PLATE_NO = this.PLATE_NO;
            DataObject.CONTAINER_NO = this.CONTAINER_NO;
            DataObject.CTN_QTY = this.CTN_QTY;
            DataObject.WHARF = this.WHARF;
            DataObject.LOADING_FLAG = this.LOADING_FLAG;
            DataObject.WHARF_TIME = this.WHARF_TIME;
            DataObject.LOAD_TIME = this.LOAD_TIME;
            DataObject.REMARK = this.REMARK;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string TRUCK_NO
        {
            get
            {
                return (string)this["TRUCK_NO"];
            }
            set
            {
                this["TRUCK_NO"] = value;
            }
        }
        public string TRUCK_SEQ
        {
            get
            {
                return (string)this["TRUCK_SEQ"];
            }
            set
            {
                this["TRUCK_SEQ"] = value;
            }
        }
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string PACK_NO
        {
            get
            {
                return (string)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public DateTime? SHIP_TIME
        {
            get
            {
                return (DateTime?)this["SHIP_TIME"];
            }
            set
            {
                this["SHIP_TIME"] = value;
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
        public string PLATE_NO
        {
            get
            {
                return (string)this["PLATE_NO"];
            }
            set
            {
                this["PLATE_NO"] = value;
            }
        }
        public string CONTAINER_NO
        {
            get
            {
                return (string)this["CONTAINER_NO"];
            }
            set
            {
                this["CONTAINER_NO"] = value;
            }
        }
        public double? CTN_QTY
        {
            get
            {
                return (double?)this["CTN_QTY"];
            }
            set
            {
                this["CTN_QTY"] = value;
            }
        }
        public string WHARF
        {
            get
            {
                return (string)this["WHARF"];
            }
            set
            {
                this["WHARF"] = value;
            }
        }
        public string LOADING_FLAG
        {
            get
            {
                return (string)this["LOADING_FLAG"];
            }
            set
            {
                this["LOADING_FLAG"] = value;
            }
        }
        public DateTime? WHARF_TIME
        {
            get
            {
                return (DateTime?)this["WHARF_TIME"];
            }
            set
            {
                this["WHARF_TIME"] = value;
            }
        }
        public DateTime? LOAD_TIME
        {
            get
            {
                return (DateTime?)this["LOAD_TIME"];
            }
            set
            {
                this["LOAD_TIME"] = value;
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
    public class R_PDA_SHIP_CHECK
    {
        public string ID { get; set; }
        public string BU { get; set; }
        public string TRUCK_NO { get; set; }
        public string TRUCK_SEQ { get; set; }
        public string DN_NO { get; set; }
        public string PACK_NO { get; set; }
        public string SKUNO { get; set; }
        public double? QTY { get; set; }
        public string LOCATION { get; set; }
        public DateTime? SHIP_TIME { get; set; }
        public string VALID_FLAG { get; set; }
        public string PLATE_NO { get; set; }
        public string CONTAINER_NO { get; set; }
        public double? CTN_QTY { get; set; }
        public string WHARF { get; set; }
        public string LOADING_FLAG { get; set; }
        public DateTime? WHARF_TIME { get; set; }
        public DateTime? LOAD_TIME { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}