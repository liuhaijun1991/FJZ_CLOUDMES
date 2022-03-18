using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Common
{
    public class ConvertToJson
    {
        // <copyright file="ConvertToJson.cs" company="Foxconn">
        // Copyright(c) foxconn All rights reserved
        // </copyright>
        // <author>fangguogang</author>
        // <date> 2017-11-28 </date>
        /// <summary>
        /// for data Convert To Json
        /// </summary>
        public ConvertToJson()
        {
        }
        /// <summary>
        /// Convert DataTable To Json
        /// </summary>
        /// <param name="tabel">tabel</param>
        /// <returns></returns>
        public  static object DataTableToJson(DataTable tabel)
        {
            System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in tabel.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn column in tabel.Columns)
                {
                    childRow.Add(column.ColumnName, row[column]);
                }
                parentRow.Add(childRow);
            }
            return parentRow;
        }
    }
}

