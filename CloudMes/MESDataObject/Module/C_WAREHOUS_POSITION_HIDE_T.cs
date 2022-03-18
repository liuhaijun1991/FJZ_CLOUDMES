using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    class T_C_WAREHOUS_POSITION_HIDE_T
    {
    }
    public class C_WAREHOUS_POSITION_HIDE_T
    {
        public string WH_ID { get; set; }
         public double? ROW_POSITION { get; set; }
        public double? COL_POSITION { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
   
    }

    public class Row_C_WAREHOUS_POSITION_HIDE_T : DataObjectBase
    {
        public Row_C_WAREHOUS_POSITION_HIDE_T(DataObjectInfo info) : base(info)
        {

        }
        public C_WAREHOUS_POSITION_HIDE_T GetDataObject()
        {
            C_WAREHOUS_POSITION_HIDE_T DataObject = new C_WAREHOUS_POSITION_HIDE_T();
            DataObject.WH_ID = this.WH_ID;
            DataObject.ROW_POSITION = this.ROW_POSITION;
            DataObject.COL_POSITION = this.COL_POSITION;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            
            return DataObject;
        }
        public string WH_ID
        {
            get
            {
                return (string)this["WH_ID"];
            }
            set
            {
                this["WH_ID"] = value;
            }
        }
       
        public double? ROW_POSITION
        {
            get
            {
                return (double?)this["ROW_POSITION"];
            }
            set
            {
                this["ROW_POSITION"] = value;
            }
        }
        public double? COL_POSITION
        {
            get
            {
                return (double?)this["COL_POSITION"];
            }
            set
            {
                this["COL_POSITION"] = value;
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
}
