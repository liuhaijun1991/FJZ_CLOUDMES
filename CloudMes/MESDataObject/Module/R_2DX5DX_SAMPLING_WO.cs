using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_2DX5DX_SAMPLING_WO : DataObjectTable
    {
        public T_R_2DX5DX_SAMPLING_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX5DX_SAMPLING_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX5DX_SAMPLING_WO);
            TableName = "R_2DX5DX_SAMPLING_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 2DX/5DX/NORMAL_STATION抽測
        /// add by hgb 2019.05.28
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="WO"></param>
        /// <param name="SKUNO"></param>
        /// <param name="STATION"></param>
        /// <param name="LINE"></param>
        /// <param name="TYPE"></param>
        /// <param name="DB"></param>
        public void Sfc2dx5dxNormalStationSampling(string SN, string WO, string SKUNO, string STATION, string LINE, string TYPE, OleExec DB)
        {
            T_C_2DX5DX_SAMPLING t_c_2dx5dx_sampling = new T_C_2DX5DX_SAMPLING(DB, DB_TYPE_ENUM.Oracle);            
            if (!t_c_2dx5dx_sampling.SamplingExists(SKUNO, STATION, TYPE, DB))
            {
                return  ;//不存在配置退出
            }
            C_2DX5DX_SAMPLING c_2dx5dx_sampling = t_c_2dx5dx_sampling.LoadSampling(SKUNO, STATION, TYPE, DB);
          string id=  t_c_2dx5dx_sampling.LoadSampling(SKUNO, STATION, TYPE, DB).ID;
            T_R_SN_STATION_DETAIL r_sn_station_detail = new T_R_SN_STATION_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            if (r_sn_station_detail.CheckSNStationPass(SN, STATION,  DB).Count>1)
            {
                return  ;//第一次過站才抽，第二次退出抽測
            }
          

            string sql, result;
            DataTable dt = DB.ORM.Queryable<R_2DX5DX_SAMPLING_WO>().Where(x =>
                         x.WO == WO && x.STATION == STATION &&
                         x.LINE_NAME == LINE && x.SAMPLING_TYPE == TYPE).ToDataTable();
            

            if (dt.Rows.Count == 0)
            {

                Row_R_2DX5DX_SAMPLING_WO R = (Row_R_2DX5DX_SAMPLING_WO)NewRow();
                R.ID = MesDbBase.GetNewID(DB.ORM, "HWT", "R_2DX5DX_SAMPLING_WO");
                R.WO = WO;
                R.LINE_NAME = LINE;
                R.STATION = STATION;
                R.LOT_QTY = c_2dx5dx_sampling.LOT_QTY;
                R.SAMPLING_QTY = c_2dx5dx_sampling.SAMPLING_QTY;
                R.SAMPLING_TOTAL_SEQNO = 1;
                R.SAMPLING_SEQNO = 1;
                R.SAMPLING_FLAG = "0";
                R.SAMPLING_TIME = DateTime.Now;
                R.SAMPLING_TYPE = TYPE;

                sql = R.GetInsertString(this.DBType);
                result = DB.ExecSQL(sql);
               

                T_R_2DX5DX_SAMPLING_SN TB= null; 
                T_R_2DX5DX_SAMPLING_SN.Row_R_2DX5DX_SAMPLING_SN R2 = null;
                TB = new T_R_2DX5DX_SAMPLING_SN(DB, this.DBType);
                R2 = (T_R_2DX5DX_SAMPLING_SN.Row_R_2DX5DX_SAMPLING_SN)TB.NewRow();

                R2.ID = MesDbBase.GetNewID(DB.ORM, "HWT", "R_2DX5DX_SAMPLING_SN");
                R2.WO = WO;
                R2.LINE_NAME = LINE;
                R2.STATION = STATION;
                R2.SN = SN;
                R2.SAMPLING_TIME = DateTime.Now;
                R2.SAMPLING_TYPE = TYPE;
                R2.STATE = "";
                R2.TEST_FLAG = "0";

                sql = R2.GetInsertString(this.DBType);
                result = DB.ExecSQL(sql);
               
                throw new MESReturnMessage("OK! SAMPLING " + TYPE);
            }
            else
            {
                Row_R_2DX5DX_SAMPLING_WO R = (Row_R_2DX5DX_SAMPLING_WO)NewRow();
                R.loadData(dt.Rows[0]);
                R.SAMPLING_TOTAL_SEQNO = R.SAMPLING_TOTAL_SEQNO + 1;

                sql = R.GetUpdateString(this.DBType);
                result = DB.ExecSQL(sql);
              
                //若對LOT_QTY取余<=SAMPLING_QTY,則需要抽樣（且不能是VAR_LOT_QTY的整倍數）--
                //如LOT_QTY=24，工單數量=30,SAMPLING_QTY=2 則抽第1,2片，第25,26片
                //第1片,1除24余1 小於抽測數量2，滿足
                //第2片,2除24余2 等於抽測數量2，滿足 
                //第25片,25除24余1 小於抽測數量2，滿足
                //第26片,26除24余2 等於抽測數量2，滿足 
                //......
                if (R.SAMPLING_TOTAL_SEQNO % R.LOT_QTY <= R.SAMPLING_QTY && R.SAMPLING_TOTAL_SEQNO % R.LOT_QTY != 0)

                {

                    T_R_2DX5DX_SAMPLING_SN TB = null;
                    T_R_2DX5DX_SAMPLING_SN.Row_R_2DX5DX_SAMPLING_SN R2 = null;
                    TB = new T_R_2DX5DX_SAMPLING_SN(DB, this.DBType);
                    R2 = (T_R_2DX5DX_SAMPLING_SN.Row_R_2DX5DX_SAMPLING_SN)TB.NewRow();

                    R2.ID = MesDbBase.GetNewID(DB.ORM, "HWT", "R_2DX5DX_SAMPLING_SN");
                    R2.WO = WO;
                    R2.LINE_NAME = LINE;
                    R2.STATION = STATION;
                    R2.SN = SN;
                    R2.SAMPLING_TIME = DateTime.Now;
                    R2.SAMPLING_TYPE = TYPE;
                    R2.STATE = "";
                    R2.TEST_FLAG = "0";

                    sql = R2.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                   

                    R = (Row_R_2DX5DX_SAMPLING_WO)NewRow();
                    R.loadData(dt.Rows[0]);
                    R.SAMPLING_SEQNO = R.SAMPLING_SEQNO + 1;

                    sql = R.GetUpdateString(this.DBType);
                    result = DB.ExecSQL(sql);
                    

                    throw new MESReturnMessage("OK! SAMPLING " + TYPE);
                }
                 
            }
        }
    }
    public class Row_R_2DX5DX_SAMPLING_WO : DataObjectBase
    {
        public Row_R_2DX5DX_SAMPLING_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_2DX5DX_SAMPLING_WO GetDataObject()
        {
            R_2DX5DX_SAMPLING_WO DataObject = new R_2DX5DX_SAMPLING_WO();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.STATION = this.STATION;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.SAMPLING_QTY = this.SAMPLING_QTY;
            DataObject.SAMPLING_TOTAL_SEQNO = this.SAMPLING_TOTAL_SEQNO;
            DataObject.SAMPLING_SEQNO = this.SAMPLING_SEQNO;
            DataObject.SAMPLING_FLAG = this.SAMPLING_FLAG;
            DataObject.SAMPLING_TIME = this.SAMPLING_TIME;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string LINE_NAME
        {
            get
            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
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
        public double? LOT_QTY
        {
            get
            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
            }
        }
        public double? SAMPLING_QTY
        {
            get
            {
                return (double?)this["SAMPLING_QTY"];
            }
            set
            {
                this["SAMPLING_QTY"] = value;
            }
        }
        public double? SAMPLING_TOTAL_SEQNO
        {
            get
            {
                return (double?)this["SAMPLING_TOTAL_SEQNO"];
            }
            set
            {
                this["SAMPLING_TOTAL_SEQNO"] = value;
            }
        }
        public double? SAMPLING_SEQNO
        {
            get
            {
                return (double?)this["SAMPLING_SEQNO"];
            }
            set
            {
                this["SAMPLING_SEQNO"] = value;
            }
        }
        public string SAMPLING_FLAG
        {
            get
            {
                return (string)this["SAMPLING_FLAG"];
            }
            set
            {
                this["SAMPLING_FLAG"] = value;
            }
        }
        public DateTime? SAMPLING_TIME
        {
            get
            {
                return (DateTime?)this["SAMPLING_TIME"];
            }
            set
            {
                this["SAMPLING_TIME"] = value;
            }
        }
        public string SAMPLING_TYPE
        {
            get
            {
                return (string)this["SAMPLING_TYPE"];
            }
            set
            {
                this["SAMPLING_TYPE"] = value;
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
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
    public class R_2DX5DX_SAMPLING_WO
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string LINE_NAME { get; set; }
        public string STATION { get; set; }
        public double? LOT_QTY { get; set; }
        public double? SAMPLING_QTY { get; set; }
        public double? SAMPLING_TOTAL_SEQNO { get; set; }
        public double? SAMPLING_SEQNO { get; set; }
        public string SAMPLING_FLAG { get; set; }
        public DateTime? SAMPLING_TIME { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}