using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;
using MESDBHelper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data;

namespace MESStation.LogicObject
{
    public class SKU
    {
        #region C_SKU 映射属性
        public string SkuId
        {
            get
            {
                return SkuBase.ID;
            }
        }

        public string Bu
        {
            get
            {
                return SkuBase.BU;
            }
        }

        public string SkuNo
        {
            get
            {
                return SkuBase.SKUNO;
            }
        }

        public string Version
        {
            get
            {
                return SkuBase.VERSION;
            }
        }

        public string SkuName
        {
            get
            {
                return SkuBase.SKU_NAME;
            }
        }

        public string CSeriesId
        {
            get
            {
                return SkuBase.C_SERIES_ID;
            }
        }

        public string CustPartNo
        {
            get
            {
                return SkuBase.CUST_PARTNO;
            }
        }

        public string CustSkuCode
        {
            get
            {
                return SkuBase.CUST_SKU_CODE;
            }
        }

        public string SnRule
        {
            get
            {
                return SkuBase.SN_RULE;
            }
        }

        public string Description
        {
            get
            {
                return SkuBase.DESCRIPTION;
            }
        }

        public string LastEditUser
        {
            get
            {
                return SkuBase.EDIT_EMP;
            }
        }

        public DateTime LastEditTime
        {
            get
            {
                return SkuBase.EDIT_TIME;
            }
        }

        #endregion

        #region C_PACKING 映射属性
        public string CPackingId
        {
            get
            {
                return Packing.ID;
            }
        }

        public string CPackingSkuNo
        {
            get
            {
                return Packing.SKUNO;
            }
        }

        public string CPackingPackType
        {
            get
            {
                return Packing.PACK_TYPE;
            }
        }

        public string CPackingTransportType
        {
            get
            {
                return Packing.TRANSPORT_TYPE;
            }
        }

        public string CPackingInsidePackType
        {
            get
            {
                return Packing.INSIDE_PACK_TYPE;
            }
        }

        public double? CPackingMaxQty
        {
            get
            {
                return Packing.MAX_QTY;
            }
        }

        public string CPackingDescription
        {
            get
            {
                return Packing.DESCRIPTION;
            }
        }

        public DateTime? CPackingEditTime
        {
            get
            {
                return Packing.EDIT_TIME;
            }
        }

        public string CPackingEditEmp
        {
            get
            {
                return Packing.EDIT_EMP;
            }
        }
        #endregion


        C_SKU _SkuBase { get; set; }
        public C_SKU SkuBase
        {
            get
            {
                return _SkuBase; 
            }
            private set
            {
                _SkuBase = value;
            }
        }

        public List<Route> SkuRoutes;

        public List<string> ProcessingWo; //線上工單

        public C_PACKING Packing=new C_PACKING(); //包裝參數

        public Dictionary<string, string> LabelPaths;

        List<C_SAP_STATION_MAP> _SapStationMaps;
        public List<C_SAP_STATION_MAP> SapStationMaps
        {
            get
            {
                return _SapStationMaps;
            }
            private set
            {
                this._SapStationMaps = value;
            }
        } //拋賬

        [JsonIgnore]
        [ScriptIgnore]
        public DB_TYPE_ENUM _DBType { get; set; } 

        /// <summary>
        /// 根據機種和版本加載機種信息
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="SkuVersion"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        public SKU Init(string SkuNo,string SkuVersion,OleExec DB,DB_TYPE_ENUM DBType)
        {
            MESDataObject.Module.SkuObject SkuObject = new MESDataObject.Module.SkuObject();
            T_C_SKU table_sku = new T_C_SKU(DB, DBType);
            SkuObject = table_sku.GetSkuByNameAndVersion(SkuNo, SkuVersion, DB);
            if (SkuObject != null)
            {
                this.SkuBase = SkuObject.SkuBase;
                this._DBType = DBType;
                this.ProcessingWo = new List<string>();
                this.LabelPaths = new Dictionary<string, string>();
                this.SkuRoutes = new List<Route>();
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000172", new string[] { SkuNo,SkuVersion}));
            }
            return this;
        }

        public SKU Init(string SkuNo, OleExec DB, DB_TYPE_ENUM DBType)
        {
            MESDataObject.Module.SkuObject SkuObject = new MESDataObject.Module.SkuObject();
            T_C_SKU table_sku = new T_C_SKU(DB, DBType);
            SkuObject = table_sku.GetSkuBySkuno(SkuNo, DB);
            this.SkuBase = SkuObject.SkuBase;
            this._DBType = DBType;
            this.ProcessingWo = new List<string>();
            this.LabelPaths = new Dictionary<string, string>();
            this.SkuRoutes = new List<Route>();
            LoadSapStationMap(DB);
            return this;
        }

        public SKU InitBySn(string Sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            MESDataObject.Module.SkuObject SkuObject = new MESDataObject.Module.SkuObject();
            T_C_SKU table_sku = new T_C_SKU(DB, DBType);
            SkuObject = table_sku.GetSkuBySn(Sn, DB);
            this.SkuBase = SkuObject.SkuBase;
            this._DBType = DBType;
            this.ProcessingWo = new List<string>();
            this.LabelPaths = new Dictionary<string, string>();
            this.SkuRoutes = new List<Route>();
            return this;
        }

        public SKU MaxVersionSkuInitBySn(string Sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            MESDataObject.Module.SkuObject SkuObject = new MESDataObject.Module.SkuObject();
            T_C_SKU table_sku = new T_C_SKU(DB, DBType);
            SkuObject = table_sku.GetMaxVersionSkuBySn(Sn, DB);
            this.SkuBase = SkuObject.SkuBase;
            this._DBType = DBType;
            this.ProcessingWo = new List<string>();
            this.LabelPaths = new Dictionary<string, string>();
            this.SkuRoutes = new List<Route>();
            return this;
        }

        /// <summary>
        /// 加載對應的路由信息
        /// </summary>
        /// <param name="DB"></param>
        public SKU LoadRoute(OleExec DB)
        {
            T_R_SKU_ROUTE table = null;
            List<R_SKU_ROUTE> mappings = new List<R_SKU_ROUTE>();

            if (!this.SkuBase.ID.Equals(""))
            {
                if (this._DBType.Equals(DB_TYPE_ENUM.Oracle))
                {
                    table = new T_R_SKU_ROUTE(DB, this._DBType);
                    if (this.SkuBase.ID != null && !string.IsNullOrEmpty(this.SkuBase.ID))
                    {
                        mappings = table.GetMappingBySkuId(this.SkuBase.ID, DB);
                        foreach (R_SKU_ROUTE mapping in mappings)
                        {
                            this.SkuRoutes.Add(new Route(mapping.ROUTE_ID, GetRouteType.ROUTEID, DB, this._DBType));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage("Please call this function by an instance of SKU class");
                    }
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { _DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
                return this;
            }
            else
            {
                throw new MESReturnMessage("Please ensure the C_SKU property is not null before using other methods.");
            }
            
        }

        /// <summary>
        /// 根據工單號加載機種信息
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        public SKU LoadSkuByWorkOrder(string WorkOrder, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string SkuNo = string.Empty;//根據工單獲取到機種
            string SkuVersion = string.Empty;
            WorkOrder wo = new WorkOrder();
            wo.Init(WorkOrder, DB, DBType);
            return Init(wo.SkuNO,wo.SKU_VER,DB,DBType);
        }

        public SKU LoadSkuBySkuno(string skuno, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string SkuNo = string.Empty;//根據工單獲取到機種
            string SkuVersion = string.Empty;
            return Init(skuno, DB, DBType);
        }

        /// <summary>
        /// 根據 SN 加載機種信息
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        public SKU LoadSkuBySN(string SN, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string WorkOrder = string.Empty;//根據 SN 獲取工单
            LogicObject.SN sn = new LogicObject.SN(SN, DB, DBType);
            return LoadSkuByWorkOrder(sn.WorkorderNo,DB,DBType);
        }

        /// <summary>
        /// 加載對應的拋賬信息
        /// </summary>
        /// <param name="DB"></param>
        public SKU LoadSapStationMap(OleExec DB)
        {
            if (!this.SkuBase.ID.Equals(""))
            {
                T_C_SAP_STATION_MAP table = new T_C_SAP_STATION_MAP(DB, this._DBType);
                this.SapStationMaps = table.GetSAPStationMapBySku(this.SkuBase.SKUNO, DB);
                return this;
            }
            else
            {
                throw new MESReturnMessage("Please ensure the C_SKU property is not null before using other methods.");
            }
        }

        /// <summary>
        /// 加載對應的在制工單
        /// </summary>
        /// <param name="DB"></param>
        public SKU LoadProcessingWo(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (!this.SkuBase.ID.Equals(""))
            {
                if (this._DBType.Equals(DB_TYPE_ENUM.Oracle))
                {
                    sql = $@"SELECT DISTINCT WO FROM R_WO_BASE WHERE SKUNO='{this.SkuBase.SKUNO}' AND SKU_VER='{this.SkuBase.VERSION}' AND CLOSED_FLAG='2'";
                    dt = DB.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!(dr["WO"] is System.DBNull || string.IsNullOrEmpty(dr["WO"].ToString())))
                        {
                            this.ProcessingWo.Add(dr["WO"].ToString());
                        }
                    }
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this._DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
                return this;
            }
            else
            {
                throw new MESReturnMessage("Please ensure the C_SKU property is not null before using other methods.");
            }
        }

        /// <summary>
        /// 加載對應的包裝規格
        /// </summary>
        /// <param name="PackType"></param>
        /// <param name="DB"></param>
        public SKU LoadPacking(string PackType, OleExec DB)
        {
            if (!this.SkuBase.ID.Equals(""))
            {
                T_C_PACKING table = new T_C_PACKING(DB, this._DBType);
                this.Packing = table.GetPackingBySkuAndType(this.SkuBase.SKUNO, PackType, DB);
                return this;
            }
            else
            {
                throw new MESReturnMessage("Please ensure the C_SKU property is not null before using other methods.");
            }
        }

        /// <summary>
        /// 重新載入機種信息
        /// </summary>
        /// <param name="DB"></param>
        public SKU ReLoad(OleExec DB)
        {
            if (!this.SkuBase.ID.Equals(""))
            {
                return this.Init(this.SkuBase.SKUNO, this.SkuBase.VERSION, DB, this._DBType).LoadPacking("", DB)
                            .LoadProcessingWo(DB).LoadRoute(DB).LoadSapStationMap(DB).ReLoad(DB);
            }
            else
            {
                throw new MESReturnMessage("Please ensure the C_SKU property is not null before using other methods.");
            }
        }

        public override string ToString()
        {
            if (this.SkuBase != null)
            {
                return this.SkuBase.SKUNO;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 檢查是否是MRA料號
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="skuno"></param>
        /// <returns></returns>
        public bool IsRMASkuno(OleExec sfcdb, string skuno)
        {
            bool IsRMA = false;
            T_C_CONTROL TCC = new T_C_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
            List<C_CONTROL> listControl = TCC.GetControlList("RMA_SKUNO_FORMAT", sfcdb);
            foreach (C_CONTROL c in listControl)
            {
                if (c.CONTROL_TYPE.ToUpper() == "START" && skuno.StartsWith(c.CONTROL_VALUE))
                {
                    IsRMA = true;
                    break;
                }
                else if(c.CONTROL_TYPE.ToUpper() == "END" && skuno.EndsWith(c.CONTROL_VALUE))
                {
                    IsRMA = true;
                    break;
                }
            }
            return IsRMA;
        }
    }
}
