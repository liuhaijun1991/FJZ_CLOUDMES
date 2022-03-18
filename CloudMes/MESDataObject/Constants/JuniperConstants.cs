using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Constants
{
    public enum JuniperErrType
    {
        [EnumValue("I137")]
        I137,
        [EnumValue("I138")]
        I138,
        [EnumValue("I054")]
        I054,
        [EnumValue("I139")]
        I139,
        [EnumValue("I282")]
        I282,
        [EnumValue("I244")]
        I244,
        [EnumValue("I140")]
        I140,
        [EnumValue("I285")]
        I285,
        [EnumValue("ACK")]
        ACK,
    }

    public enum JuniperSubType
    {
        [EnumValue("I137DataValid")]
        I137DataValid,
        [EnumValue("NewOrderProcess")]
        NewOrderProcess,
        [EnumValue("SysB2bI137")]
        SysB2bI137,
        [EnumValue("ChangeOrderProcess")]
        ChangeOrderProcess,
        [EnumValue("UploadI138")]
        UploadI138,
        [EnumValue("JuniperPreWoGanerate")]
        JuniperPreWoGanerate,
        [EnumValue("JuniperOnePreUpoadBom")]
        JuniperOnePreUpoadBom,
        [EnumValue("JuniperSecPreUpoadBom")]
        JuniperSecPreUpoadBom,
        [EnumValue("AddNonBom")]
        AddNonBom,
        [EnumValue("GroupIdReceive")]
        GroupIdReceive,
        [EnumValue("CreateWo")]
        CreateWo,
        [EnumValue("I054Ack")]
        I054Ack,
        [EnumValue("IAck")]
        IAck,
        [EnumValue("I285")]
        I285,
        [EnumValue("TecoSapWoWithCancelJob")]
        TecoSapWoWithCancelJob,
        [EnumValue("TecoSapWoWithChangeJob")]
        TecoSapWoWithChangeJob,
        [EnumValue("ChangeCrsdJob")]
        ChangeCrsdJob,
    }

    public enum JuniperErrStatus
    {
        /// <summary>
        /// EnumValue("2")
        /// </summary>
        [EnumValue("2")]
        Close,
        /// <summary>
        /// EnumValue("0")
        /// </summary>
        [EnumValue("0")]
        Open,
        /// <summary>
        /// EnumValue("1")
        /// </summary>
        [EnumValue("1")]
        Ongoing
    }
}
