using MESDataObject.Common;
using MESDataObject.Module.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESJuniper.Base
{
    public class JnpConst
    {
        public static string[] JnpWaitCrtdWoInSap = new string[] {
            ENUM_O_PO_STATUS.AddNonBom.ExtValue(),
            ENUM_O_PO_STATUS.OnePreUploadBom.ExtValue(),
            ENUM_O_PO_STATUS.ReceiveGroupId.ExtValue(),
            ENUM_O_PO_STATUS.SecPreUploadBom.ExtValue(),
            ENUM_O_PO_STATUS.ValidationI137.ExtValue(),
            ENUM_O_PO_STATUS.CreateWo.ExtValue(),
            ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue()
        };

        public static string[] JnpNotProducStatus = new string[] {
            ENUM_O_PO_STATUS.AddNonBom.ExtValue(),
            ENUM_O_PO_STATUS.CreateWo.ExtValue(),
            ENUM_O_PO_STATUS.DownloadWo.ExtValue(),
            ENUM_O_PO_STATUS.OnePreUploadBom.ExtValue(),
            ENUM_O_PO_STATUS.ReceiveGroupId.ExtValue(),
            ENUM_O_PO_STATUS.SecPreUploadBom.ExtValue(),
            ENUM_O_PO_STATUS.ValidationI137.ExtValue(),
            ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue()
        };

        public static string[] JnpProductingStatus = new string[] {
            //ENUM_O_PO_STATUS.DownloadWo.ExtValue(),
            ENUM_O_PO_STATUS.Production.ExtValue(),
            ENUM_O_PO_STATUS.CBS.ExtValue(),
            ENUM_O_PO_STATUS.PreAsn.ExtValue(),
            ENUM_O_PO_STATUS.WaitDismantle.ExtValue()
        };
    }

    public enum ENUM_JNP_SITE
    {
        [EnumValue("FJZ")]
        FJZ,
        [EnumValue("VNJUNIPER")]
        FVN,
    }
}
