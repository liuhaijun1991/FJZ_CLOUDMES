using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESInterface.DCN
{
    public class SQLFilter
    {
        public string TAG;
        public SQLFilterType FilterType;
        public List<SQLPara> Para;
        public SQLFilter(string tag, SQLFilterType filterType, List<SQLPara> para)
        {
            TAG = tag;
            FilterType = filterType;
            Para = para;
        }
        public SQLFilter(string tag, SQLFilterType filterType)
        {
            TAG = tag;
            FilterType = filterType;
            Para = new List<SQLPara>();
        }
        public string ToString()
        {
            //strRet = "";
            if (this.FilterType == SQLFilterType.IN)
            {
                return this.SQLFilterTypeIN();
            }
            else if (this.FilterType == SQLFilterType.BETWEEN)
            {
                return this.SQLFilterTypeBETWEEN();
            }
            else if (this.FilterType == SQLFilterType.EQUAL)
            {
                return this.SQLFilterTypeEQUAL();
            }
            else if (this.FilterType == SQLFilterType.GREAT)
            {
                return this.SQLFilterTypeGREAT();
            }
            else if (this.FilterType == SQLFilterType.GREAT_OR_EQUAL)
            {
                return this.SQLFilterTypeGREAT_OR_EQUAL();
            }
            else if (this.FilterType == SQLFilterType.IN)
            {
                return this.SQLFilterTypeIN();
            }
            else if (this.FilterType == SQLFilterType.LESS)
            {
                return this.SQLFilterTypeLESS();
            }
            else if (this.FilterType == SQLFilterType.LESS_OR_EQUAL)
            {
                return this.SQLFilterTypeLESS_OR_EQUAL();
            }
            else if (this.FilterType == SQLFilterType.NOT_BETWEEN)
            {
                return this.SQLFilterTypeNOT_BETWEEN();
            }
            else if (this.FilterType == SQLFilterType.NOT_EQUAL)
            {
                return this.SQLFilterTypeNOT_EQUAL();
            }
            else if (this.FilterType == SQLFilterType.NOT_IN)
            {
                return this.SQLFilterTypeNOT_IN();
            }
            else
            {
                return "NOT Math Type";
            }
        }
        string getParaList()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Para.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                sb.Append(Para[i].ToString());
            }
            return sb.ToString();
        }
        string SQLFilterTypeIN()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " IN ( ");
            sb.Append(this.getParaList());
            sb.Append(" )");
            return sb.ToString();
        }
        string SQLFilterTypeNOT_IN()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + "NOT IN ( ");
            sb.Append(this.getParaList());
            sb.Append(" )");
            return sb.ToString();
        }
        string SQLFilterTypeEQUAL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " = ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeGREAT()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " > ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeLESS()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " < ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeGREAT_OR_EQUAL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " >= ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeLESS_OR_EQUAL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " <= ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeNOT_EQUAL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " <> ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeBETWEEN()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " BETWEEN ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" AND ");
            sb.Append(this.Para[1].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
        string SQLFilterTypeNOT_BETWEEN()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " + TAG + " NOT BETWEEN ");
            sb.Append(this.Para[0].ToString());
            sb.Append(" AND ");
            sb.Append(this.Para[1].ToString());
            sb.Append(" ");
            return sb.ToString();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum SQLFilterType
    {
        /// <summary>
        /// IN 包含
        /// </summary>
        IN,
        /// <summary>
        /// 等於 =
        /// </summary>
        EQUAL,
        /// <summary>
        /// 大於 >
        /// </summary>
        GREAT,
        /// <summary>
        /// 小於 
        /// </summary>
        LESS,
        /// <summary>
        ///  大於或等於
        /// </summary>
        GREAT_OR_EQUAL,
        /// <summary>
        ///  小於或等於
        /// </summary>
        LESS_OR_EQUAL,
        /// <summary>
        /// 不等於
        /// </summary>
        NOT_EQUAL,
        /// <summary>
        /// Between 
        /// </summary>
        BETWEEN,
        /// <summary>
        /// not between
        /// </summary>
        NOT_BETWEEN,
        /// <summary>
        /// 不在列表內 not in
        /// </summary>
        NOT_IN
    }
}