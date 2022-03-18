using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Constants
{
    public class PublicConstants
    {

        public enum MesWeekDay
        {
            [EnumValue("1")]
            Monday,
            [EnumValue("2")]
            Tuesday,
            [EnumValue("3")]
            Wednesday,
            [EnumValue("4")]
            Thursday,
            [EnumValue("5")]
            Friday,
            [EnumValue("6")]
            Saturday,
            [EnumValue("7")]
            Sunday,
        }

        public enum MesBool
        {
            /// <summary>
            /// EnumName("N")
            /// EnumValue("0")
            /// </summary>
            [EnumName("N")]
            [EnumValue("0")]
            No,
            /// <summary>
            /// EnumName("Y")
            /// EnumValue("1")
            /// </summary>
            [EnumName("Y")]
            [EnumValue("1")]
            Yes
        }

        public enum MesSysUser
        {
            /// <summary>
            /// EnumName("Sys")
            /// EnumValue("0")
            /// </summary>
            [EnumValue("Sys")]
            Sys
        }
    }
}
