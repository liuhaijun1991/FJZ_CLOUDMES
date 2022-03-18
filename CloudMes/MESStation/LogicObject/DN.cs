using MESDataObject;
using MESDataObject.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MESStation.LogicObject
{
    public class DN
    {
        R_DN_STATUS _DNBase { get; set; }

        [JsonIgnore]
        [ScriptIgnore]
        public DB_TYPE_ENUM _DBType { get; set; }

        public R_DN_STATUS DNBase
        {
            get { return _DNBase; }
            private set { _DNBase = value; }
        }

        #region R_DN_STATUS 映射屬性
        public string DNNo { get { return _DNBase.DN_NO; } }
        public string DNLine { get { return _DNBase.DN_LINE; } }
        public string PONo { get { return _DNBase.PO_NO; } }
        public string POLine { get { return _DNBase.PO_LINE; } }
        public string SONo { get { return _DNBase.SO_NO; } }
        public string SkuNo { get { return _DNBase.SKUNO; } }
        public double? Qty { get { return _DNBase.QTY; } }
        public string GTtype { get { return _DNBase.GTTYPE; } }
        public string GTFlag { get { return _DNBase.GT_FLAG; } }
        public DateTime? GTDate { get { return _DNBase.GTDATE; } }
        public string DNFlag { get { return _DNBase.DN_FLAG; } }
        public string DNPlant { get { return _DNBase.DN_PLANT; } }
        public DateTime? CreateTime { get { return _DNBase.CREATETIME; } }
        public DateTime? EditTime { get { return _DNBase.EDITTIME; } }
        public string GTEvent { get { return _DNBase.GTEVENT; } }
        public string ID { get { return _DNBase.ID; } }
        #endregion


    }
}
