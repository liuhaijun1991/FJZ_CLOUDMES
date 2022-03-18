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
using MESStation.LogicObject;

namespace MESStation.Label
{

    public class BPD_SILOADING : LabelBase
    {
       
        T_R_SN TRS = null;
        T_R_SN_KEYPART_DETAIL TRSK= null;
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", StationSessionType = "SN", StationSessionKey = "1", Value = "" };
        LabelInputValue I_WO = new LabelInputValue() { Name = "WO", Type = "STRING", StationSessionType = "WO", StationSessionKey = "1", Value = "" };

        LabelOutput O_SN = new LabelOutput { Name = "SN", Type = LabelOutPutTypeEnum.String, Value = "", Description = "SYSSERIALNO" };
        LabelOutput O_SKU = new LabelOutput { Name = "SKU", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Sku Number" };
        LabelOutput O_SKU_VER = new LabelOutput { Name = "SKU_VER", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Sku Version" };
        LabelOutput O_KEYPART_SN = new LabelOutput { Name = "KEYPART_SN", Type = LabelOutPutTypeEnum.String, Value = "", Description = "Keypart SN" };


        public BPD_SILOADING()
        {
            this.Inputs.Add(I_SN);
            this.Inputs.Add(I_WO);
            this.Outputs.Add(O_SN);
            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_SKU_VER);
            this.Outputs.Add(O_KEYPART_SN);
        }
        /// <summary>
        /// BPD 打印调用
        /// </summary>
        /// <param name="DB"></param>
        public override void MakeLabel(OleExec DB)
        {
            R_SN Sn = null;                    
            TRS = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            TRSK = new T_R_SN_KEYPART_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            if (I_SN.Value != null && I_SN.Value.ToString().Length > 0)
            {
                string strSn = I_SN.Value.ToString();
                O_SN.Value = strSn;
                //如果是 FOX 的话，加载的还是 NWG 的 SN 的记录，针对的是同时要打印 NWG 和 FOX的条码
                Sn = TRS.GetSN(strSn.Replace("FOX","NWG"), DB);
                
                string WO = I_WO.Value.ToString();
                WorkOrder Base = new WorkOrder();
                Base.Init(WO, DB);
                R_SN_KEYPART_DETAIL SK = TRSK.GetKeypartSN(Sn.SN, DB);
                if (Base != null)
                {
                    O_SKU.Value = Base.SkuNO;
                    O_SKU_VER.Value = Base.SKU_VER;
                }
                if (SK != null)
                {
                    O_KEYPART_SN.Value = SK.KEYPART_SN;
                }
            }
        }

    }
}
