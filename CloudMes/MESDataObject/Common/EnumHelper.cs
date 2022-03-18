using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MESDataObject.Common
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 已过时,请用.Ext<T>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumName(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes =
                (EnumNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }

        /// <summary>
        /// 已过时,请用.Ext<T>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumValue(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }
            string description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes =
                (EnumValueAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumValueAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }
        /// <summary>
        /// 取EnumValue 值
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string ExtValue(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
            string description = enumValue.ToString();
            var attributes = (EnumValueAttribute[])memInfo[0].GetCustomAttributes(typeof(EnumValueAttribute), false);
            if (attributes != null && attributes.Length > 0)            
                description = attributes[0].Description;            
            else
                throw new Exception("enum EnumValue is null!");
            return description;
        }
        /// <summary>
        /// 取EnumName 值
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string ExtName(this Enum enumName)
        {
            Type type = enumName.GetType();
            MemberInfo[] memInfo = type.GetMember(enumName.ToString());
            string description = enumName.ToString();
            var attributes = (EnumNameAttribute[])memInfo[0].GetCustomAttributes(typeof(EnumNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
                description = attributes[0].Description;
            else
                throw new Exception("enum EnumName is null!");
            return description;
        }

        /// <summary>
        /// 取T的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T Ext<T>(this Enum enumValue) where T : Attribute
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public sealed class EnumNameAttribute : Attribute
        {
            private string description;
            public string Description { get { return description; } }

            public EnumNameAttribute(string description)
                : base()
            {
                this.description = description;
            }
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public sealed class EnumValueAttribute : Attribute
        {
            private string description;
            public string Description { get { return description; } }

            public EnumValueAttribute(string description)
                : base()
            {
                this.description = description;
            }
        }

    }


}
