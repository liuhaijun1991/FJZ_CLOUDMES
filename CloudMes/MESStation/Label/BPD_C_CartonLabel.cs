using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label
{
    /// <summary>
    /// 輸入參數：
    ///          CARTON --- Carton 號
    /// 輸出參數：CARTON --- Carton 號
    ///          SKU    --- 機種名
    ///          SKUVER --- 機種版本
    ///          HHSKU  --- 鴻海料號
    ///          SKUDESC--- 機種描述
    ///          DATE   --- 包裝日期
    ///          QTY    --- 包裝數量
    ///          ROHS   --- ROHS 標識
    ///          ROHSINFO-- ROHS 標識
    /// </summary>
    public class BPD_C_CartonLabel: LabelBase
    {
        T_R_WO_BASE TRWB = null;
        T_R_PACKING TRP = null;
        T_C_SN_RULE TCSR = null;
        T_C_SKU TCS = null;
        T_C_SKU_FOXCONN_MAPPING TCSFM = null;
        LabelInputValue I_CARTON = new LabelInputValue() { Name = "CARTON", Type = "STRING", StationSessionType = "PACKNO", StationSessionKey = "1", Value = "" };

        LabelOutput O_CARTON_NO = new LabelOutput { Name = "CARTON", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Carton No" };
        LabelOutput O_SKU = new LabelOutput { Name = "SKU", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Sku number" };
        LabelOutput O_SKU_VER = new LabelOutput { Name="SKUVER", Type=LabelOutPutTypeEnum.String,Value="",Description="Sku Version"};
        LabelOutput O_HH_SKU = new LabelOutput { Name = "HHSKU", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Honghai Sku" };
        LabelOutput O_SKU_DESC = new LabelOutput { Name = "SKUDESC", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Sku description" };
        LabelOutput O_DATE = new LabelOutput { Name = "DATE", Type=LabelOutPutTypeEnum.String, Value="",Description="Packing Date,like SEP 9 2018" };
        LabelOutput O_QTY = new LabelOutput { Name = "QTY", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Quantity of product" };
        LabelOutput O_ROHS = new LabelOutput { Name = "ROHS", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Rohs Status" };
        LabelOutput O_ROHS_INFO = new LabelOutput { Name = "ROHSINFO", Type = LabelOutPutTypeEnum.String, Value = "", Description = "" };


        public BPD_C_CartonLabel()
        {
            this.Inputs.Add(I_CARTON);
            this.Outputs.Add(O_CARTON_NO);
            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_SKU_VER);
            this.Outputs.Add(O_HH_SKU);
            this.Outputs.Add(O_SKU_DESC);
            this.Outputs.Add(O_DATE);
            this.Outputs.Add(O_QTY);
            this.Outputs.Add(O_ROHS);
            this.Outputs.Add(O_ROHS_INFO);
        }

        public override void MakeLabel(OleExec DB)
        {
            R_PACKING Packing = null;
            TRWB = new T_R_WO_BASE(DB, DB_TYPE_ENUM.Oracle);
            TRP = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB,DB_TYPE_ENUM.Oracle);
            TCSR = new T_C_SN_RULE(DB, DB_TYPE_ENUM.Oracle);
            TCS = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle);

            //I_CARTON.Value = "CTN-SH22400004";
            if (I_CARTON.Value != null && I_CARTON.Value.ToString().Length > 0)
            {
                
                Packing = TRP.GetPackingByPackNo(I_CARTON.Value.ToString(), DB);
                O_CARTON_NO.Value = Packing.PACK_NO;
                R_WO_BASE Base = TRWB.GetWOByCarton(Packing.PACK_NO, DB);
                if (Base != null)
                {
                    O_SKU.Value = Base.SKUNO;
                    O_SKU_VER.Value = Base.SKU_VER==null?"???":Base.SKU_VER;
                    O_SKU_DESC.Value = Base.SKU_DESC==null?"???":Base.SKU_DESC;
                    string Month= ((DateTime)Packing.CREATE_TIME).ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                    string Date = ((DateTime)Packing.CREATE_TIME).Day.ToString();
                    string Year = ((DateTime)Packing.CREATE_TIME).Year.ToString();
                    O_DATE.Value = Month + " " + Date + " " + Year;
                    O_QTY.Value = TRP.GetQtyByCarton(Packing.PACK_NO,DB);
                    O_ROHS.Value = Base.ROHS==null?"ROHS-???":"ROHS-" + Base.ROHS.Substring(Base.ROHS.Length-1, 1);
                    SkuObject Sku = TCS.GetSkuByNameAndVersion(Base.SKUNO, Base.SKU_VER, DB);
                    string SeriesName = Sku == null ? "???" : Sku.SeriesName;
                    ///C客戶
                    if (SeriesName.StartsWith("C"))
                    {
                        O_ROHS_INFO.Value = "Y12";
                    }
                    else
                    {
                        O_ROHS_INFO.Value = "N12";
                    }
                    C_SKU_FOXCONN_MAPPING Mapping = TCSFM.GetMappingBySkuAndVer(Base.SKUNO, Base.SKU_VER, DB);
                    O_HH_SKU.Value = Mapping==null?"": Mapping.SKU_FOXCONN;
                }

            } 
        }

    }
}
