using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESPubLab.MesBase
{
    public static class MesConst
    {
        public static string MesNullStr = "";
    }

    public enum MES_CONST_DAY_RANGE
    {
        /// <summary>
        /// "00:00:00"
        /// </summary>
        [EnumValue("00:00:00")]
        CONST_DAY_BEGIN,
        /// <summary>
        /// "23:59:59"
        /// </summary>
        [EnumValue("23:59:59")]
        CONST_DAY_END
    }

    public enum MES_CONST_DATETIME_FORMAT
    {
        /// <summary>
        /// [EnumValue("yyyyMMddHHmmss")]
        /// </summary>
        [EnumValue("yyyyMMddHHmmss")]
        DEFAULT,
        /// <summary>
        /// [EnumValue("yyyy-MM-dd")]
        /// </summary>
        [EnumValue("yyyy-MM-dd")]
        YMD_A,
        /// <summary>
        /// [EnumValue("yyyy/MM/dd")]
        /// </summary>
        [EnumValue("yyyy/MM/dd")]
        YMD_B,
        /// <summary>
        /// [EnumValue("MM-dd-yyyy")]
        /// </summary>
        [EnumValue("MM-dd-yyyy")]
        MDY_A,
        /// <summary>
        /// [EnumValue("MM/dd/yyyy")]
        /// </summary>
        [EnumValue("MM/dd/yyyy")]
        MDY_B,
        /// <summary>
        /// [EnumValue("yyyy-MM-dd HH:mm:ss")]
        /// </summary>
        [EnumValue("yyyy-MM-dd HH:mm:ss")]
        Normal,
        /// <summary>
        /// [EnumValue("yyyyMMddHHmmss")]
        /// </summary>
        [EnumValue("yyyyMMddHHmmssfff")]
        yyyyMMddHHmmssfff,

    }

    public enum MES_CONST_SAVE_TYPE
    {
        /// <summary>
        /// [EnumValue("MM-dd-yyyy")]
        /// </summary>
        [EnumValue("0")]
        SaveMesDb,
        /// <summary>
        /// [EnumValue("MM/dd/yyyy")]
        /// </summary>
        [EnumValue("1")]
        SaveLocal
    }

    public enum MES_CONST_SYSTEM
    {
        /// <summary>
        /// [EnumValue("MM-dd-yyyy")]
        /// </summary>
        [EnumValue("System")]
        System
    }

}
