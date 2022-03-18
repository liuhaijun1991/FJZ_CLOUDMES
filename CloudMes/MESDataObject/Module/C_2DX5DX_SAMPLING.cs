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
    public class T_C_2DX5DX_SAMPLING : DataObjectTable
    {
        public T_C_2DX5DX_SAMPLING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_2DX5DX_SAMPLING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_2DX5DX_SAMPLING);
            TableName = "C_2DX5DX_SAMPLING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取配置表信息
        /// ADD BY HGB
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="STATION"></param>
        /// <param name="TYPE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SamplingExists(string SKUNO, string STATION, string TYPE, OleExec DB)
        {
            return DB.ORM.Queryable<C_2DX5DX_SAMPLING>().Where(t => t.SKUNO == SKUNO && t.STATION == STATION && t.SAMPLING_TYPE== TYPE).Any();
        }

        /// <summary>
        /// 獲取配置表信息
        /// ADD BY HGB
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="STATION"></param>
        /// <param name="TYPE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_2DX5DX_SAMPLING LoadSampling(string SKUNO, string STATION, string TYPE, OleExec DB)
        {
            var aa = DB.ORM.Queryable<C_2DX5DX_SAMPLING>().Where(t => t.SKUNO == SKUNO && t.STATION == STATION && t.SAMPLING_TYPE == TYPE).ToList();
            return DB.ORM.Queryable<C_2DX5DX_SAMPLING>().Where(t => t.SKUNO == SKUNO && t.STATION == STATION && t.SAMPLING_TYPE == TYPE).ToList().FirstOrDefault();

            
        }

    }
    public class Row_C_2DX5DX_SAMPLING : DataObjectBase
    {
        public Row_C_2DX5DX_SAMPLING(DataObjectInfo info) : base(info)
        {

        }
        public C_2DX5DX_SAMPLING GetDataObject()
        {
            C_2DX5DX_SAMPLING DataObject = new C_2DX5DX_SAMPLING();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.SAMPLING_QTY = this.SAMPLING_QTY;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.UPH = this.UPH;
            DataObject.REMARK = this.REMARK;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CREATE_EMP = this.CREATE_EMP;
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
        public string UPH
        {
            get
            {
                return (string)this["UPH"];
            }
            set
            {
                this["UPH"] = value;
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
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
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
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
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
    public class C_2DX5DX_SAMPLING
    {
        public string ID { get; set; }
      
        public string SKUNO { get; set; }
        public string STATION { get; set; }
        public double? LOT_QTY { get; set; }
        public double? SAMPLING_QTY { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string UPH { get; set; }
        public string REMARK { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}