using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter
{
    public class DataSourceBase
    {
        public List<DataSourcePara> para = new List<DataSourcePara>()
        { };
        public string Name = "";
        /// <summary>
        /// 数据类型，取值如下：
        /// DBTABLE,URLTABLE
        /// </summary>
        public string DataSourceType = "";


    }
    
    public class DataSourcePara
    {
        public string Name = "";
        /// <summary>
        /// 数据类型，取值如下：
        /// STRING , DATETIME , OBJECT
        /// </summary>
        public string DataType = "";

        public string Value = "";
        /// <summary>
        /// 说明
        /// </summary>
        public string Description = "";
    }
}
