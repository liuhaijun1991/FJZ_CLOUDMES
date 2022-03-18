using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Constants
{
    public enum LotConstants
    {
        /// <summary>
        /// OBA 開箱抽檢標記
        /// </summary>
        [EnumName("WAITOBA")]
        WaitOba,
        /// <summary>
        /// OBA 開箱抽檢記錄已經匹配批次標記
        /// </summary>
        [EnumName("SNSAMPLINGCOMPLETED")]
        SnSamplingCompleted,
        /// <summary>
        /// 待抽檢
        /// </summary>
        [EnumValue("0")]
        LotWaitSampling,
        /// <summary>
        /// 抽檢完成
        /// </summary>
        [EnumValue("1")]
        LotSamplingCompleted,
        /// <summary>
        /// SN抽檢Pass標記
        /// </summary>
        [EnumValue("1")]
        SnSamplingPass,
        /// <summary>
        /// SN抽檢Pass標記
        /// </summary>
        [EnumValue("0")]
        SnSamplingFail
    }

    public enum Customer
    {
        [EnumValue("NETGEAR")]
        NETGEAR,
        [EnumValue("D-LINK")]
        DLINK,
        [EnumValue("BROADCOM")]
        BROADCOM,
        [EnumValue("UFI")]
        UFI,
        [EnumValue("SE")]
        SE,
        [EnumValue("ARUBA")]
        ARUBA,
        [EnumValue("JUNIPER")]
        JUNIPER
    }

    public enum WorkType
    {
        [EnumValue("RMA")]
        RMA,
        [EnumValue("REGULAR")]
        REGULAR,
        [EnumValue("REWORK")]
        REWORK
    }

    public enum MesPlantSite
    {        
        [EnumName("FJZJNP")]
        [EnumValue("FJZ")]
        FJZ,
        [EnumName("VNJUNIPER")]
        [EnumValue("FVN")]
        FVN,
        [EnumValue("FNN")]
        FNN,
        [EnumValue("FSJ")]
        FSJ,
        [EnumValue("FCZ")]
        FCZ
    }

    public enum DcnKeyPartScantype
    {
        /// <summary>
        /// [EnumName("SW KIT P/N")]
        /// </summary>
        [EnumName("SW KIT P/N")]
        SW_KIT_PN,
        /// <summary>
        /// [EnumName("APTRSN")]
        /// </summary>
        [EnumName("APTRSN")]
        APTRSN,
        /// <summary>
        /// [EnumName("SystemSN")]
        /// </summary>
        [EnumName("SystemSN")]
        SystemSN,
        /// <summary>
        /// [EnumName("ACC S/N")]
        /// </summary>
        [EnumName("ACC S/N")]
        ACC_SN,
        /// <summary>
        /// [EnumName("ACC D/C")]
        /// </summary>
        [EnumName("ACC D/C")]
        ACC_DC,
        /// <summary>
        /// [EnumName("DC D/C")]
        /// </summary>
        [EnumName("DC D/C")]
        DC_DC,
        /// <summary>
        /// [EnumName("CUS S/N1")]
        /// </summary>
        [EnumName("CUS S/N1")]
        CUS_SN,
        /// <summary>
        /// [EnumName("PCBA S/N")]
        /// </summary>
        [EnumName("PCBA S/N")]
        PCBA_SN,
        /// <summary>
        /// [EnumName("PPM S/N")]
        /// </summary>
        [EnumName("PPM S/N")]
        PPM_SN,
        /// <summary>
        /// [EnumName("FIRMWARE P/N")]
        /// </summary>
        [EnumName("FIRMWARE P/N")]
        FIRMWARE_PN,
        /// <summary>
        /// [EnumName("PS S/N")]
        /// </summary>
        [EnumName("PS S/N")]
        PS_SN
    }
    
}
