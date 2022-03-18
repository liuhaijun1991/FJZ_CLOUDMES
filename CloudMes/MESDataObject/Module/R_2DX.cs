using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_2DX : DataObjectTable
    {
        public T_R_2DX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX);
            TableName = "R_2DX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 查詢SN 是否存在
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_2DX GetSNByR2DX(string SN, OleExec DB)
        {
            string sql = $@"SELECT * FROM R_2DX WHERE SN='{SN}'";
            DataSet ds = DB.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_2DX rowPacking = (Row_R_2DX)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public R_2DX GetSNByR2DXStation(string SN,string Station, OleExec DB)
        {
            string sql = $@"SELECT * FROM R_2DX WHERE SN='{SN}' and station='{Station}' and STATUS='PASS'";
            DataSet ds = DB.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_2DX rowPacking = (Row_R_2DX)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// WZW 把前一LOT Fail的SN抓出来报错
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="Line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_2DX> IsFailSN(string StationName, string Line, OleExec DB)
        {
            DataTable dt = new DataTable();
            List<R_2DX> Listr2DX = new List<R_2DX>();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
//                string StrSQL = $@"select * from R_2DX a
//inner join  ( select sn,max(edit_time) as edit_time from R_2DX where sn in   
//(select sn from R_2DX_LOT_DETAIL  where lot_no   in
//(select lot_no from R_2DX_LOT_STATUS  where  station = '{StationName}' and line = '{Line}' and lotpass_flag = '0'))group by sn)b  
//on a.sn = b.sn and a.edit_time = b.edit_time and a.status='FAIL'";
                string StrSQL = $@"select * from R_2DX a
inner join  ( select sn,max(edit_time) as edit_time from R_2DX where sn in   
(select sn from R_2DX_LOT_DETAIL  where lot_id   in
(select ID from R_2DX_LOT_STATUS  where  station = '{StationName}' and line = '{Line}' and lotpass_flag = '0'))group by sn)b  
on a.sn = b.sn and a.edit_time = b.edit_time and a.status='FAIL'";
                dt = DB.ExecSelect(StrSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Row_R_2DX RowRLotStation = (Row_R_2DX)NewRow();
                    RowRLotStation.loadData(dt.Rows[0]);
                    Listr2DX.Add(RowRLotStation.GetDataObject());

                }
                return Listr2DX;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// WZW 生成LOT號
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void InR_2DX(string BU, string SN, string WO, string SKUNO, string LINE, string LOCATION, string STATION, string STATUS, string MISALIGNMENT, double VOID, string SHORT_FLAG, string OTHER_CAUSE_FLAG, string SCANBY, string REMARK, string Emp, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_2DX Table_R_2DX = new T_R_2DX(DB, DBType);
                    Row_R_2DX Row_R_2DX = (Row_R_2DX)NewRow();
                    string LotID = "";
                    LotID = Table_R_2DX.GetNewID(BU, DB);
                    Row_R_2DX.ID = LotID;
                    Row_R_2DX.SN = SN;
                    Row_R_2DX.WO = WO;
                    Row_R_2DX.SKUNO = SKUNO;
                    Row_R_2DX.BU = BU;
                    Row_R_2DX.LINE = LINE;
                    Row_R_2DX.LOCATION = LOCATION;
                    Row_R_2DX.STATION = STATION;
                    Row_R_2DX.STATUS = STATUS;
                    Row_R_2DX.MISALIGNMENT = MISALIGNMENT;
                    Row_R_2DX.SHORT_FLAG = SHORT_FLAG;
                    Row_R_2DX.VOID = VOID.ToString();
                    Row_R_2DX.OTHER_CAUSE_FLAG = OTHER_CAUSE_FLAG;
                    Row_R_2DX.SCANBY = SCANBY;
                    Row_R_2DX.SCANDATE = GetDBDateTime(DB);
                    Row_R_2DX.REMARK = REMARK;
                    Row_R_2DX.EDIT_EMP = Emp;
                    Row_R_2DX.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_2DX.GetInsertString(DBType));
                }
                catch (Exception)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_R_2DX : DataObjectBase
    {
        public Row_R_2DX(DataObjectInfo info) : base(info)
        {

        }
        public R_2DX GetDataObject()
        {
            R_2DX DataObject = new R_2DX();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.BU = this.BU;
            DataObject.LINE = this.LINE;
            DataObject.LOCATION = this.LOCATION;
            DataObject.STATION = this.STATION;
            DataObject.STATUS = this.STATUS;
            DataObject.MISALIGNMENT = this.MISALIGNMENT;
            DataObject.VOID = this.VOID;
            DataObject.OTHER_CAUSE_FLAG = this.OTHER_CAUSE_FLAG;
            DataObject.SHORT_FLAG = this.SHORT_FLAG;
            DataObject.SCANBY = this.SCANBY;
            DataObject.SCANDATE = this.SCANDATE;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
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
        public string MISALIGNMENT
        {
            get
            {
                return (string)this["MISALIGNMENT"];
            }
            set
            {
                this["MISALIGNMENT"] = value;
            }
        }
        public string VOID
        {
            get
            {
                return (string)this["VOID"];
            }
            set
            {
                this["VOID"] = value;
            }
        }
        public string OTHER_CAUSE_FLAG
        {
            get
            {
                return (string)this["OTHER_CAUSE_FLAG"];
            }
            set
            {
                this["OTHER_CAUSE_FLAG"] = value;
            }
        }
        public string SHORT_FLAG
        {
            get
            {
                return (string)this["SHORT_FLAG"];
            }
            set
            {
                this["SHORT_FLAG"] = value;
            }
        }
        public string SCANBY
        {
            get
            {
                return (string)this["SCANBY"];
            }
            set
            {
                this["SCANBY"] = value;
            }
        }
        public DateTime? SCANDATE
        {
            get
            {
                return (DateTime?)this["SCANDATE"];
            }
            set
            {
                this["SCANDATE"] = value;
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
    public class R_2DX
    {
        public string ID;
        public string SN;
        public string WO;
        public string SKUNO;
        public string BU;
        public string LINE;
        public string LOCATION;
        public string STATION;
        public string STATUS;
        public string MISALIGNMENT;
        public string VOID;
        public string OTHER_CAUSE_FLAG;
        public string SHORT_FLAG;
        public string SCANBY;
        public DateTime? SCANDATE;
        public string REMARK;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}