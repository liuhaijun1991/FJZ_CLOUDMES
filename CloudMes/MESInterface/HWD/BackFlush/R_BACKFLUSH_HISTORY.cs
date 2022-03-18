using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MESInterface.HWD.BackFlush
{

    public class R_BACKFLUSH_HISTORY
    {
        #region 屬性
        bool IsChange = false;
        public bool hasChange
        {
            get
            { return IsChange; }
        }
        System.String db_WORKORDERNO;
        System.String db_old_WORKORDERNO;
        bool isNull_old_WORKORDERNO = true;
        bool isNull_WORKORDERNO = true;
        public System.String WORKORDERNO
        {
            get
            {
                if (isNull_WORKORDERNO == true)
                {
                    throw new Exception("'WORKORDERNO'NullValue");
                }
                else
                {
                    return db_WORKORDERNO;
                }
            }
            set
            {
                db_WORKORDERNO = value;
                isNull_WORKORDERNO = false;
                IsChange = true;
            }
        }
        string SelectString_WORKORDERNO()
        {
            if (isNull_old_WORKORDERNO)
            {
                return " WORKORDERNO is null ";
            }
            else
            {
                return " WORKORDERNO = '" + db_old_WORKORDERNO.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_WORKORDERNO()
        {
            if (isNull_WORKORDERNO)
            {
                return " null ";
            }
            else
            {
                return " '" + db_WORKORDERNO.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_WORKORDERNO()
        {
            if (isNull_WORKORDERNO)
            {
                return " WORKORDERNO = null ";
            }
            else
            {
                return " WORKORDERNO = '" + db_WORKORDERNO.ToString().Replace("'", "") + "' ";
            }
        }
        System.String db_SKUNO;
        System.String db_old_SKUNO;
        bool isNull_old_SKUNO = true;
        bool isNull_SKUNO = true;
        public System.String SKUNO
        {
            get
            {
                if (isNull_SKUNO == true)
                {
                    throw new Exception("'SKUNO'NullValue");
                }
                else
                {
                    return db_SKUNO;
                }
            }
            set
            {
                db_SKUNO = value;
                isNull_SKUNO = false;
                IsChange = true;
            }
        }
        string SelectString_SKUNO()
        {
            if (isNull_old_SKUNO)
            {
                return " SKUNO is null ";
            }
            else
            {
                return " SKUNO = '" + db_old_SKUNO.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_SKUNO()
        {
            if (isNull_SKUNO)
            {
                return " null ";
            }
            else
            {
                return " '" + db_SKUNO.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_SKUNO()
        {
            if (isNull_SKUNO)
            {
                return " SKUNO = null ";
            }
            else
            {
                return " SKUNO = '" + db_SKUNO.ToString().Replace("'", "") + "' ";
            }
        }
        System.String db_SAP_STATION;
        System.String db_old_SAP_STATION;
        bool isNull_old_SAP_STATION = true;
        bool isNull_SAP_STATION = true;
        public System.String SAP_STATION
        {
            get
            {
                if (isNull_SAP_STATION == true)
                {
                    throw new Exception("'SAP_STATION'NullValue");
                }
                else
                {
                    return db_SAP_STATION;
                }
            }
            set
            {
                db_SAP_STATION = value;
                isNull_SAP_STATION = false;
                IsChange = true;
            }
        }
        string SelectString_SAP_STATION()
        {
            if (isNull_old_SAP_STATION)
            {
                return " SAP_STATION is null ";
            }
            else
            {
                return " SAP_STATION = '" + db_old_SAP_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_SAP_STATION()
        {
            if (isNull_SAP_STATION)
            {
                return " null ";
            }
            else
            {
                return " '" + db_SAP_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_SAP_STATION()
        {
            if (isNull_SAP_STATION)
            {
                return " SAP_STATION = null ";
            }
            else
            {
                return " SAP_STATION = '" + db_SAP_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_WORKORDER_QTY;
        System.Decimal db_old_WORKORDER_QTY;
        bool isNull_old_WORKORDER_QTY = true;
        bool isNull_WORKORDER_QTY = true;
        public System.Decimal WORKORDER_QTY
        {
            get
            {
                if (isNull_WORKORDER_QTY == true)
                {
                    throw new Exception("'WORKORDER_QTY'NullValue");
                }
                else
                {
                    return db_WORKORDER_QTY;
                }
            }
            set
            {
                db_WORKORDER_QTY = value;
                isNull_WORKORDER_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_WORKORDER_QTY()
        {
            if (isNull_old_WORKORDER_QTY)
            {
                return " WORKORDER_QTY is null ";
            }
            else
            {
                return " WORKORDER_QTY = '" + db_old_WORKORDER_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_WORKORDER_QTY()
        {
            if (isNull_WORKORDER_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_WORKORDER_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_WORKORDER_QTY()
        {
            if (isNull_WORKORDER_QTY)
            {
                return " WORKORDER_QTY = null ";
            }
            else
            {
                return " WORKORDER_QTY = '" + db_WORKORDER_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_BACKFLUSH_QTY;
        System.Decimal db_old_BACKFLUSH_QTY;
        bool isNull_old_BACKFLUSH_QTY = true;
        bool isNull_BACKFLUSH_QTY = true;
        public System.Decimal BACKFLUSH_QTY
        {
            get
            {
                if (isNull_BACKFLUSH_QTY == true)
                {
                    throw new Exception("'BACKFLUSH_QTY'NullValue");
                }
                else
                {
                    return db_BACKFLUSH_QTY;
                }
            }
            set
            {
                db_BACKFLUSH_QTY = value;
                isNull_BACKFLUSH_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_BACKFLUSH_QTY()
        {
            if (isNull_old_BACKFLUSH_QTY)
            {
                return " BACKFLUSH_QTY is null ";
            }
            else
            {
                return " BACKFLUSH_QTY = '" + db_old_BACKFLUSH_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_BACKFLUSH_QTY()
        {
            if (isNull_BACKFLUSH_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_BACKFLUSH_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_BACKFLUSH_QTY()
        {
            if (isNull_BACKFLUSH_QTY)
            {
                return " BACKFLUSH_QTY = null ";
            }
            else
            {
                return " BACKFLUSH_QTY = '" + db_BACKFLUSH_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_SFC_QTY;
        System.Decimal db_old_SFC_QTY;
        bool isNull_old_SFC_QTY = true;
        bool isNull_SFC_QTY = true;
        public System.Decimal SFC_QTY
        {
            get
            {
                if (isNull_SFC_QTY == true)
                {
                    throw new Exception("'SFC_QTY'NullValue");
                }
                else
                {
                    return db_SFC_QTY;
                }
            }
            set
            {
                db_SFC_QTY = value;
                isNull_SFC_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_SFC_QTY()
        {
            if (isNull_old_SFC_QTY)
            {
                return " SFC_QTY is null ";
            }
            else
            {
                return " SFC_QTY = '" + db_old_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_SFC_QTY()
        {
            if (isNull_SFC_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_SFC_QTY()
        {
            if (isNull_SFC_QTY)
            {
                return " SFC_QTY = null ";
            }
            else
            {
                return " SFC_QTY = '" + db_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_DIFF_QTY;
        System.Decimal db_old_DIFF_QTY;
        bool isNull_old_DIFF_QTY = true;
        bool isNull_DIFF_QTY = true;
        public System.Decimal DIFF_QTY
        {
            get
            {
                if (isNull_DIFF_QTY == true)
                {
                    throw new Exception("'DIFF_QTY'NullValue");
                }
                else
                {
                    return db_DIFF_QTY;
                }
            }
            set
            {
                db_DIFF_QTY = value;
                isNull_DIFF_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_DIFF_QTY()
        {
            if (isNull_old_DIFF_QTY)
            {
                return " DIFF_QTY is null ";
            }
            else
            {
                return " DIFF_QTY = '" + db_old_DIFF_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_DIFF_QTY()
        {
            if (isNull_DIFF_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_DIFF_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_DIFF_QTY()
        {
            if (isNull_DIFF_QTY)
            {
                return " DIFF_QTY = null ";
            }
            else
            {
                return " DIFF_QTY = '" + db_DIFF_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.String db_SFC_STATION;
        System.String db_old_SFC_STATION;
        bool isNull_old_SFC_STATION = true;
        bool isNull_SFC_STATION = true;
        public System.String SFC_STATION
        {
            get
            {
                if (isNull_SFC_STATION == true)
                {
                    throw new Exception("'SFC_STATION'NullValue");
                }
                else
                {
                    return db_SFC_STATION;
                }
            }
            set
            {
                db_SFC_STATION = value;
                isNull_SFC_STATION = false;
                IsChange = true;
            }
        }
        string SelectString_SFC_STATION()
        {
            if (isNull_old_SFC_STATION)
            {
                return " SFC_STATION is null ";
            }
            else
            {
                return " SFC_STATION = '" + db_old_SFC_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_SFC_STATION()
        {
            if (isNull_SFC_STATION)
            {
                return " null ";
            }
            else
            {
                return " '" + db_SFC_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_SFC_STATION()
        {
            if (isNull_SFC_STATION)
            {
                return " SFC_STATION = null ";
            }
            else
            {
                return " SFC_STATION = '" + db_SFC_STATION.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_HAND_QTY;
        System.Decimal db_old_HAND_QTY;
        bool isNull_old_HAND_QTY = true;
        bool isNull_HAND_QTY = true;
        public System.Decimal HAND_QTY
        {
            get
            {
                if (isNull_HAND_QTY == true)
                {
                    throw new Exception("'HAND_QTY'NullValue");
                }
                else
                {
                    return db_HAND_QTY;
                }
            }
            set
            {
                db_HAND_QTY = value;
                isNull_HAND_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_HAND_QTY()
        {
            if (isNull_old_HAND_QTY)
            {
                return " HAND_QTY is null ";
            }
            else
            {
                return " HAND_QTY = '" + db_old_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_HAND_QTY()
        {
            if (isNull_HAND_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_HAND_QTY()
        {
            if (isNull_HAND_QTY)
            {
                return " HAND_QTY = null ";
            }
            else
            {
                return " HAND_QTY = '" + db_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_LAST_SFC_QTY;
        System.Decimal db_old_LAST_SFC_QTY;
        bool isNull_old_LAST_SFC_QTY = true;
        bool isNull_LAST_SFC_QTY = true;
        public System.Decimal LAST_SFC_QTY
        {
            get
            {
                if (isNull_LAST_SFC_QTY == true)
                {
                    throw new Exception("'LAST_SFC_QTY'NullValue");
                }
                else
                {
                    return db_LAST_SFC_QTY;
                }
            }
            set
            {
                db_LAST_SFC_QTY = value;
                isNull_LAST_SFC_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_LAST_SFC_QTY()
        {
            if (isNull_old_LAST_SFC_QTY)
            {
                return " LAST_SFC_QTY is null ";
            }
            else
            {
                return " LAST_SFC_QTY = '" + db_old_LAST_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_LAST_SFC_QTY()
        {
            if (isNull_LAST_SFC_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_LAST_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_LAST_SFC_QTY()
        {
            if (isNull_LAST_SFC_QTY)
            {
                return " LAST_SFC_QTY = null ";
            }
            else
            {
                return " LAST_SFC_QTY = '" + db_LAST_SFC_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_DIFF_QTY1;
        System.Decimal db_old_DIFF_QTY1;
        bool isNull_old_DIFF_QTY1 = true;
        bool isNull_DIFF_QTY1 = true;
        public System.Decimal DIFF_QTY1
        {
            get
            {
                if (isNull_DIFF_QTY1 == true)
                {
                    throw new Exception("'DIFF_QTY1'NullValue");
                }
                else
                {
                    return db_DIFF_QTY1;
                }
            }
            set
            {
                db_DIFF_QTY1 = value;
                isNull_DIFF_QTY1 = false;
                IsChange = true;
            }
        }
        string SelectString_DIFF_QTY1()
        {
            if (isNull_old_DIFF_QTY1)
            {
                return " DIFF_QTY1 is null ";
            }
            else
            {
                return " DIFF_QTY1 = '" + db_old_DIFF_QTY1.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_DIFF_QTY1()
        {
            if (isNull_DIFF_QTY1)
            {
                return " null ";
            }
            else
            {
                return " '" + db_DIFF_QTY1.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_DIFF_QTY1()
        {
            if (isNull_DIFF_QTY1)
            {
                return " DIFF_QTY1 = null ";
            }
            else
            {
                return " DIFF_QTY1 = '" + db_DIFF_QTY1.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_DIFF_QTY2;
        System.Decimal db_old_DIFF_QTY2;
        bool isNull_old_DIFF_QTY2 = true;
        bool isNull_DIFF_QTY2 = true;
        public System.Decimal DIFF_QTY2
        {
            get
            {
                if (isNull_DIFF_QTY2 == true)
                {
                    throw new Exception("'DIFF_QTY2'NullValue");
                }
                else
                {
                    return db_DIFF_QTY2;
                }
            }
            set
            {
                db_DIFF_QTY2 = value;
                isNull_DIFF_QTY2 = false;
                IsChange = true;
            }
        }
        string SelectString_DIFF_QTY2()
        {
            if (isNull_old_DIFF_QTY2)
            {
                return " DIFF_QTY2 is null ";
            }
            else
            {
                return " DIFF_QTY2 = '" + db_old_DIFF_QTY2.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_DIFF_QTY2()
        {
            if (isNull_DIFF_QTY2)
            {
                return " null ";
            }
            else
            {
                return " '" + db_DIFF_QTY2.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_DIFF_QTY2()
        {
            if (isNull_DIFF_QTY2)
            {
                return " DIFF_QTY2 = null ";
            }
            else
            {
                return " DIFF_QTY2 = '" + db_DIFF_QTY2.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_HISTORY_HAND_QTY;
        System.Decimal db_old_HISTORY_HAND_QTY;
        bool isNull_old_HISTORY_HAND_QTY = true;
        bool isNull_HISTORY_HAND_QTY = true;
        public System.Decimal HISTORY_HAND_QTY
        {
            get
            {
                if (isNull_HISTORY_HAND_QTY == true)
                {
                    throw new Exception("'HISTORY_HAND_QTY'NullValue");
                }
                else
                {
                    return db_HISTORY_HAND_QTY;
                }
            }
            set
            {
                db_HISTORY_HAND_QTY = value;
                isNull_HISTORY_HAND_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_HISTORY_HAND_QTY()
        {
            if (isNull_old_HISTORY_HAND_QTY)
            {
                return " HISTORY_HAND_QTY is null ";
            }
            else
            {
                return " HISTORY_HAND_QTY = '" + db_old_HISTORY_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_HISTORY_HAND_QTY()
        {
            if (isNull_HISTORY_HAND_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_HISTORY_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_HISTORY_HAND_QTY()
        {
            if (isNull_HISTORY_HAND_QTY)
            {
                return " HISTORY_HAND_QTY = null ";
            }
            else
            {
                return " HISTORY_HAND_QTY = '" + db_HISTORY_HAND_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_MRB_QTY;
        System.Decimal db_old_MRB_QTY;
        bool isNull_old_MRB_QTY = true;
        bool isNull_MRB_QTY = true;
        public System.Decimal MRB_QTY
        {
            get
            {
                if (isNull_MRB_QTY == true)
                {
                    throw new Exception("'MRB_QTY'NullValue");
                }
                else
                {
                    return db_MRB_QTY;
                }
            }
            set
            {
                db_MRB_QTY = value;
                isNull_MRB_QTY = false;
                IsChange = true;
            }
        }
        string SelectString_MRB_QTY()
        {
            if (isNull_old_MRB_QTY)
            {
                return " MRB_QTY is null ";
            }
            else
            {
                return " MRB_QTY = '" + db_old_MRB_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_MRB_QTY()
        {
            if (isNull_MRB_QTY)
            {
                return " null ";
            }
            else
            {
                return " '" + db_MRB_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_MRB_QTY()
        {
            if (isNull_MRB_QTY)
            {
                return " MRB_QTY = null ";
            }
            else
            {
                return " MRB_QTY = '" + db_MRB_QTY.ToString().Replace("'", "") + "' ";
            }
        }
        System.String db_REC_TYPE;
        System.String db_old_REC_TYPE;
        bool isNull_old_REC_TYPE = true;
        bool isNull_REC_TYPE = true;
        public System.String REC_TYPE
        {
            get
            {
                if (isNull_REC_TYPE == true)
                {
                    throw new Exception("'REC_TYPE'NullValue");
                }
                else
                {
                    return db_REC_TYPE;
                }
            }
            set
            {
                db_REC_TYPE = value;
                isNull_REC_TYPE = false;
                IsChange = true;
            }
        }
        string SelectString_REC_TYPE()
        {
            if (isNull_old_REC_TYPE)
            {
                return " REC_TYPE is null ";
            }
            else
            {
                return " REC_TYPE = '" + db_old_REC_TYPE.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_REC_TYPE()
        {
            if (isNull_REC_TYPE)
            {
                return " null ";
            }
            else
            {
                return " '" + db_REC_TYPE.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_REC_TYPE()
        {
            if (isNull_REC_TYPE)
            {
                return " REC_TYPE = null ";
            }
            else
            {
                return " REC_TYPE = '" + db_REC_TYPE.ToString().Replace("'", "") + "' ";
            }
        }
        System.DateTime db_BACK_DATE;
        System.DateTime db_old_BACK_DATE;
        bool isNull_old_BACK_DATE = true;
        bool isNull_BACK_DATE = true;
        public System.DateTime BACK_DATE
        {
            get
            {
                if (isNull_BACK_DATE == true)
                {
                    throw new Exception("'BACK_DATE'NullValue");
                }
                else
                {
                    return db_BACK_DATE;
                }
            }
            set
            {
                db_BACK_DATE = value;
                isNull_BACK_DATE = false;
                IsChange = true;
            }
        }
        string SelectString_BACK_DATE()
        {
            if (isNull_old_BACK_DATE)
            {
                return " BACK_DATE is null ";
            }
            else
            {
                return " BACK_DATE = to_date('" + db_old_BACK_DATE.ToString("dd-MM-yyyy HH:mm:ss") + "','dd-mm-yyyy hh24:mi:ss') ";
            }
        }
        string InsertString_BACK_DATE()
        {
            if (isNull_BACK_DATE)
            {
                return " null ";
            }
            else
            {
                return " to_date('" + db_BACK_DATE.ToString("dd-MM-yyyy HH:mm:ss") + "','dd-mm-yyyy hh24:mi:ss') ";
            }
        }
        string UpdateString_BACK_DATE()
        {
            if (isNull_BACK_DATE)
            {
                return " BACK_DATE = null ";
            }
            else
            {
                return " BACK_DATE = to_date('" + db_BACK_DATE.ToString("dd-MM-yyyy HH:mm:ss") + "','dd-mm-yyyy hh24:mi:ss') ";
            }
        }
        System.String db_RESULT;
        System.String db_old_RESULT;
        bool isNull_old_RESULT = true;
        bool isNull_RESULT = true;
        public System.String RESULT
        {
            get
            {
                if (isNull_RESULT == true)
                {
                    throw new Exception("'RESULT'NullValue");
                }
                else
                {
                    return db_RESULT;
                }
            }
            set
            {
                db_RESULT = value;
                isNull_RESULT = false;
                IsChange = true;
            }
        }
        string SelectString_RESULT()
        {
            if (isNull_old_RESULT)
            {
                return " RESULT is null ";
            }
            else
            {
                return " RESULT = '" + db_old_RESULT.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_RESULT()
        {
            if (isNull_RESULT)
            {
                return " null ";
            }
            else
            {
                return " '" + db_RESULT.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_RESULT()
        {
            if (isNull_RESULT)
            {
                return " RESULT = null ";
            }
            else
            {
                return " RESULT = '" + db_RESULT.ToString().Replace("'", "") + "' ";
            }
        }
        System.Decimal db_TIMES;
        System.Decimal db_old_TIMES;
        bool isNull_old_TIMES = true;
        bool isNull_TIMES = true;
        public System.Decimal TIMES
        {
            get
            {
                if (isNull_TIMES == true)
                {
                    throw new Exception("'TIMES'NullValue");
                }
                else
                {
                    return db_TIMES;
                }
            }
            set
            {
                db_TIMES = value;
                isNull_TIMES = false;
                IsChange = true;
            }
        }
        string SelectString_TIMES()
        {
            if (isNull_old_TIMES)
            {
                return " TIMES is null ";
            }
            else
            {
                return " TIMES = '" + db_old_TIMES.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_TIMES()
        {
            if (isNull_TIMES)
            {
                return " null ";
            }
            else
            {
                return " '" + db_TIMES.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_TIMES()
        {
            if (isNull_TIMES)
            {
                return " TIMES = null ";
            }
            else
            {
                return " TIMES = '" + db_TIMES.ToString().Replace("'", "") + "' ";
            }
        }
        System.String db_TOSAP;
        System.String db_old_TOSAP;
        bool isNull_old_TOSAP = true;
        bool isNull_TOSAP = true;
        public System.String TOSAP
        {
            get
            {
                if (isNull_TOSAP == true)
                {
                    throw new Exception("'TOSAP'NullValue");
                }
                else
                {
                    return db_TOSAP;
                }
            }
            set
            {
                db_TOSAP = value;
                isNull_TOSAP = false;
                IsChange = true;
            }
        }
        string SelectString_TOSAP()
        {
            if (isNull_old_TOSAP)
            {
                return " TOSAP is null ";
            }
            else
            {
                return " TOSAP = '" + db_old_TOSAP.ToString().Replace("'", "") + "' ";
            }
        }
        string InsertString_TOSAP()
        {
            if (isNull_TOSAP)
            {
                return " null ";
            }
            else
            {
                return " '" + db_TOSAP.ToString().Replace("'", "") + "' ";
            }
        }
        string UpdateString_TOSAP()
        {
            if (isNull_TOSAP)
            {
                return " TOSAP = null ";
            }
            else
            {
                return " TOSAP = '" + db_TOSAP.ToString().Replace("'", "") + "' ";
            }
        }
        #endregion
        #region 基礎方法
        public string InsertToDB()
        {
            string StrSql = "";
            string cols = "";
            string values = "";
            cols += "WORKORDERNO";
            values += InsertString_WORKORDERNO();
            cols += ",SKUNO";
            values += "," + InsertString_SKUNO();
            cols += ",SAP_STATION";
            values += "," + InsertString_SAP_STATION();
            cols += ",WORKORDER_QTY";
            values += "," + InsertString_WORKORDER_QTY();
            cols += ",BACKFLUSH_QTY";
            values += "," + InsertString_BACKFLUSH_QTY();
            cols += ",SFC_QTY";
            values += "," + InsertString_SFC_QTY();
            cols += ",DIFF_QTY";
            values += "," + InsertString_DIFF_QTY();
            cols += ",SFC_STATION";
            values += "," + InsertString_SFC_STATION();
            cols += ",HAND_QTY";
            values += "," + InsertString_HAND_QTY();
            cols += ",LAST_SFC_QTY";
            values += "," + InsertString_LAST_SFC_QTY();
            cols += ",DIFF_QTY1";
            values += "," + InsertString_DIFF_QTY1();
            cols += ",DIFF_QTY2";
            values += "," + InsertString_DIFF_QTY2();
            cols += ",HISTORY_HAND_QTY";
            values += "," + InsertString_HISTORY_HAND_QTY();
            cols += ",MRB_QTY";
            values += "," + InsertString_MRB_QTY();
            cols += ",REC_TYPE";
            values += "," + InsertString_REC_TYPE();
            cols += ",BACK_DATE";
            values += "," + InsertString_BACK_DATE();
            cols += ",RESULT";
            values += "," + InsertString_RESULT();
            cols += ",TIMES";
            values += "," + InsertString_TIMES();
            cols += ",TOSAP";
            values += "," + InsertString_TOSAP();
            StrSql = string.Format("insert into {0} ({1}) values ({2})", "R_BACKFLUSH_HISTORY", cols, values);
            return StrSql;
        }
        public string delete()
        {
            string strWhere = "WORKORDERNO = '" + db_old_WORKORDERNO + "' and SKUNO = '" + db_old_SKUNO + "' and SAP_STATION = '" + db_old_SAP_STATION + "' and WORKORDER_QTY" + " =" + db_old_WORKORDER_QTY.ToString() + " and BACKFLUSH_QTY" + " =" + db_old_BACKFLUSH_QTY.ToString() + " and SFC_QTY" + " =" + db_old_SFC_QTY.ToString() + " and DIFF_QTY" + " =" + db_old_DIFF_QTY.ToString() + " and SFC_STATION = '" + db_old_SFC_STATION + "' and HAND_QTY" + " =" + db_old_HAND_QTY.ToString() + " and LAST_SFC_QTY" + " =" + db_old_LAST_SFC_QTY.ToString() + " and DIFF_QTY1" + " =" + db_old_DIFF_QTY1.ToString() + " and DIFF_QTY2" + " =" + db_old_DIFF_QTY2.ToString() + " and HISTORY_HAND_QTY" + " =" + db_old_HISTORY_HAND_QTY.ToString() + " and MRB_QTY" + " =" + db_old_MRB_QTY.ToString() + " and REC_TYPE = '" + db_old_REC_TYPE + "' and BACK_DATE" + " =to_date('" + db_old_BACK_DATE.ToString("dd-MM-yyyy HH:mm:ss") + "', 'dd-mm-yyyy hh24:mi:ss') andRESULT = '" + db_old_RESULT + "' and TIMES" + " =" + db_old_TIMES.ToString() + " and TOSAP = '" + db_old_TOSAP + "' ";
            string strSql = string.Format("delete {0} where {1}", "R_BACKFLUSH_HISTORY", strWhere);
            return strSql;
        }
        public string UpdateToDB()
        {
            string StrSql = "";
            string cols = "";
            string values = "";
            cols += SelectString_WORKORDERNO();
            values += UpdateString_WORKORDERNO();
            cols += " and " + SelectString_SKUNO();
            values += "," + UpdateString_SKUNO();
            cols += " and " + SelectString_SAP_STATION();
            values += "," + UpdateString_SAP_STATION();
            cols += " and " + SelectString_WORKORDER_QTY();
            values += "," + UpdateString_WORKORDER_QTY();
            cols += " and " + SelectString_BACKFLUSH_QTY();
            values += "," + UpdateString_BACKFLUSH_QTY();
            cols += " and " + SelectString_SFC_QTY();
            values += "," + UpdateString_SFC_QTY();
            cols += " and " + SelectString_DIFF_QTY();
            values += "," + UpdateString_DIFF_QTY();
            cols += " and " + SelectString_SFC_STATION();
            values += "," + UpdateString_SFC_STATION();
            cols += " and " + SelectString_HAND_QTY();
            values += "," + UpdateString_HAND_QTY();
            cols += " and " + SelectString_LAST_SFC_QTY();
            values += "," + UpdateString_LAST_SFC_QTY();
            cols += " and " + SelectString_DIFF_QTY1();
            values += "," + UpdateString_DIFF_QTY1();
            cols += " and " + SelectString_DIFF_QTY2();
            values += "," + UpdateString_DIFF_QTY2();
            cols += " and " + SelectString_HISTORY_HAND_QTY();
            values += "," + UpdateString_HISTORY_HAND_QTY();
            cols += " and " + SelectString_MRB_QTY();
            values += "," + UpdateString_MRB_QTY();
            cols += " and " + SelectString_REC_TYPE();
            values += "," + UpdateString_REC_TYPE();
            cols += " and " + SelectString_BACK_DATE();
            values += "," + UpdateString_BACK_DATE();
            cols += " and " + SelectString_RESULT();
            values += "," + UpdateString_RESULT();
            cols += " and " + SelectString_TIMES();
            values += "," + UpdateString_TIMES();
            cols += " and " + SelectString_TOSAP();
            values += "," + UpdateString_TOSAP();
            StrSql = string.Format("update  {0} set {1} where {2} ", "R_BACKFLUSH_HISTORY", values, cols);
            return StrSql;
        }
        public void AcceptChange()
        {
            db_old_WORKORDERNO = db_WORKORDERNO;//0
            isNull_old_WORKORDERNO = isNull_WORKORDERNO;
            db_old_SKUNO = db_SKUNO;//1
            isNull_old_SKUNO = isNull_SKUNO;
            db_old_SAP_STATION = db_SAP_STATION;//2
            isNull_old_SAP_STATION = isNull_SAP_STATION;
            db_old_WORKORDER_QTY = db_WORKORDER_QTY;//3
            isNull_old_WORKORDER_QTY = isNull_WORKORDER_QTY;
            db_old_BACKFLUSH_QTY = db_BACKFLUSH_QTY;//4
            isNull_old_BACKFLUSH_QTY = isNull_BACKFLUSH_QTY;
            db_old_SFC_QTY = db_SFC_QTY;//5
            isNull_old_SFC_QTY = isNull_SFC_QTY;
            db_old_DIFF_QTY = db_DIFF_QTY;//6
            isNull_old_DIFF_QTY = isNull_DIFF_QTY;
            db_old_SFC_STATION = db_SFC_STATION;//7
            isNull_old_SFC_STATION = isNull_SFC_STATION;
            db_old_HAND_QTY = db_HAND_QTY;//8
            isNull_old_HAND_QTY = isNull_HAND_QTY;
            db_old_LAST_SFC_QTY = db_LAST_SFC_QTY;//9
            isNull_old_LAST_SFC_QTY = isNull_LAST_SFC_QTY;
            db_old_DIFF_QTY1 = db_DIFF_QTY1;//10
            isNull_old_DIFF_QTY1 = isNull_DIFF_QTY1;
            db_old_DIFF_QTY2 = db_DIFF_QTY2;//11
            isNull_old_DIFF_QTY2 = isNull_DIFF_QTY2;
            db_old_HISTORY_HAND_QTY = db_HISTORY_HAND_QTY;//12
            isNull_old_HISTORY_HAND_QTY = isNull_HISTORY_HAND_QTY;
            db_old_MRB_QTY = db_MRB_QTY;//13
            isNull_old_MRB_QTY = isNull_MRB_QTY;
            db_old_REC_TYPE = db_REC_TYPE;//14
            isNull_old_REC_TYPE = isNull_REC_TYPE;
            db_old_BACK_DATE = db_BACK_DATE;//15
            isNull_old_BACK_DATE = isNull_BACK_DATE;
            db_old_RESULT = db_RESULT;//16
            isNull_old_RESULT = isNull_RESULT;
            db_old_TIMES = db_TIMES;//17
            isNull_old_TIMES = isNull_TIMES;
            db_old_TOSAP = db_TOSAP;//18
            isNull_old_TOSAP = isNull_TOSAP;
            IsChange = false;
        }
        #endregion
        public static R_BACKFLUSH_HISTORY DataAdapert(DataSet data, R_BACKFLUSH_HISTORY obj)
        {
            if (data.Tables[0].Rows[0]["WORKORDERNO"] != System.DBNull.Value)
            {
                obj.WORKORDERNO = (System.String)data.Tables[0].Rows[0]["WORKORDERNO"];
            }
            if (data.Tables[0].Rows[0]["SKUNO"] != System.DBNull.Value)
            {
                obj.SKUNO = (System.String)data.Tables[0].Rows[0]["SKUNO"];
            }
            if (data.Tables[0].Rows[0]["SAP_STATION"] != System.DBNull.Value)
            {
                obj.SAP_STATION = (System.String)data.Tables[0].Rows[0]["SAP_STATION"];
            }
            if (data.Tables[0].Rows[0]["WORKORDER_QTY"] != System.DBNull.Value)
            {
                obj.WORKORDER_QTY = (System.Decimal)data.Tables[0].Rows[0]["WORKORDER_QTY"];
            }
            if (data.Tables[0].Rows[0]["BACKFLUSH_QTY"] != System.DBNull.Value)
            {
                obj.BACKFLUSH_QTY = (System.Decimal)data.Tables[0].Rows[0]["BACKFLUSH_QTY"];
            }
            if (data.Tables[0].Rows[0]["SFC_QTY"] != System.DBNull.Value)
            {
                obj.SFC_QTY = (System.Decimal)data.Tables[0].Rows[0]["SFC_QTY"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY"] != System.DBNull.Value)
            {
                obj.DIFF_QTY = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY"];
            }
            if (data.Tables[0].Rows[0]["SFC_STATION"] != System.DBNull.Value)
            {
                obj.SFC_STATION = (System.String)data.Tables[0].Rows[0]["SFC_STATION"];
            }
            if (data.Tables[0].Rows[0]["HAND_QTY"] != System.DBNull.Value)
            {
                obj.HAND_QTY = (System.Decimal)data.Tables[0].Rows[0]["HAND_QTY"];
            }
            if (data.Tables[0].Rows[0]["LAST_SFC_QTY"] != System.DBNull.Value)
            {
                obj.LAST_SFC_QTY = (System.Decimal)data.Tables[0].Rows[0]["LAST_SFC_QTY"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY1"] != System.DBNull.Value)
            {
                obj.DIFF_QTY1 = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY1"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY2"] != System.DBNull.Value)
            {
                obj.DIFF_QTY2 = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY2"];
            }
            if (data.Tables[0].Rows[0]["HISTORY_HAND_QTY"] != System.DBNull.Value)
            {
                obj.HISTORY_HAND_QTY = (System.Decimal)data.Tables[0].Rows[0]["HISTORY_HAND_QTY"];
            }
            if (data.Tables[0].Rows[0]["MRB_QTY"] != System.DBNull.Value)
            {
                obj.MRB_QTY = (System.Decimal)data.Tables[0].Rows[0]["MRB_QTY"];
            }
            if (data.Tables[0].Rows[0]["REC_TYPE"] != System.DBNull.Value)
            {
                obj.REC_TYPE = (System.String)data.Tables[0].Rows[0]["REC_TYPE"];
            }
            if (data.Tables[0].Rows[0]["BACK_DATE"] != System.DBNull.Value)
            {
                obj.BACK_DATE = (System.DateTime)data.Tables[0].Rows[0]["BACK_DATE"];
            }
            if (data.Tables[0].Rows[0]["RESULT"] != System.DBNull.Value)
            {
                obj.RESULT = (System.String)data.Tables[0].Rows[0]["RESULT"];
            }
            if (data.Tables[0].Rows[0]["TIMES"] != System.DBNull.Value)
            {
                obj.TIMES = (System.Decimal)data.Tables[0].Rows[0]["TIMES"];
            }
            if (data.Tables[0].Rows[0]["TOSAP"] != System.DBNull.Value)
            {
                obj.TOSAP = (System.String)data.Tables[0].Rows[0]["TOSAP"];
            }
            obj.AcceptChange();
            return obj;
        }
        public static R_BACKFLUSH_HISTORY DataAdapert(DataSet data)
        {
            R_BACKFLUSH_HISTORY obj = new R_BACKFLUSH_HISTORY();
            if (data.Tables[0].Rows[0]["WORKORDERNO"] != System.DBNull.Value)
            {
                obj.WORKORDERNO = (System.String)data.Tables[0].Rows[0]["WORKORDERNO"];
            }
            if (data.Tables[0].Rows[0]["SKUNO"] != System.DBNull.Value)
            {
                obj.SKUNO = (System.String)data.Tables[0].Rows[0]["SKUNO"];
            }
            if (data.Tables[0].Rows[0]["SAP_STATION"] != System.DBNull.Value)
            {
                obj.SAP_STATION = (System.String)data.Tables[0].Rows[0]["SAP_STATION"];
            }
            if (data.Tables[0].Rows[0]["WORKORDER_QTY"] != System.DBNull.Value)
            {
                obj.WORKORDER_QTY = (System.Decimal)data.Tables[0].Rows[0]["WORKORDER_QTY"];
            }
            if (data.Tables[0].Rows[0]["BACKFLUSH_QTY"] != System.DBNull.Value)
            {
                obj.BACKFLUSH_QTY = (System.Decimal)data.Tables[0].Rows[0]["BACKFLUSH_QTY"];
            }
            if (data.Tables[0].Rows[0]["SFC_QTY"] != System.DBNull.Value)
            {
                obj.SFC_QTY = (System.Decimal)data.Tables[0].Rows[0]["SFC_QTY"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY"] != System.DBNull.Value)
            {
                obj.DIFF_QTY = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY"];
            }
            if (data.Tables[0].Rows[0]["SFC_STATION"] != System.DBNull.Value)
            {
                obj.SFC_STATION = (System.String)data.Tables[0].Rows[0]["SFC_STATION"];
            }
            if (data.Tables[0].Rows[0]["HAND_QTY"] != System.DBNull.Value)
            {
                obj.HAND_QTY = (System.Decimal)data.Tables[0].Rows[0]["HAND_QTY"];
            }
            if (data.Tables[0].Rows[0]["LAST_SFC_QTY"] != System.DBNull.Value)
            {
                obj.LAST_SFC_QTY = (System.Decimal)data.Tables[0].Rows[0]["LAST_SFC_QTY"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY1"] != System.DBNull.Value)
            {
                obj.DIFF_QTY1 = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY1"];
            }
            if (data.Tables[0].Rows[0]["DIFF_QTY2"] != System.DBNull.Value)
            {
                obj.DIFF_QTY2 = (System.Decimal)data.Tables[0].Rows[0]["DIFF_QTY2"];
            }
            if (data.Tables[0].Rows[0]["HISTORY_HAND_QTY"] != System.DBNull.Value)
            {
                obj.HISTORY_HAND_QTY = (System.Decimal)data.Tables[0].Rows[0]["HISTORY_HAND_QTY"];
            }
            if (data.Tables[0].Rows[0]["MRB_QTY"] != System.DBNull.Value)
            {
                obj.MRB_QTY = (System.Decimal)data.Tables[0].Rows[0]["MRB_QTY"];
            }
            if (data.Tables[0].Rows[0]["REC_TYPE"] != System.DBNull.Value)
            {
                obj.REC_TYPE = (System.String)data.Tables[0].Rows[0]["REC_TYPE"];
            }
            if (data.Tables[0].Rows[0]["BACK_DATE"] != System.DBNull.Value)
            {
                obj.BACK_DATE = (System.DateTime)data.Tables[0].Rows[0]["BACK_DATE"];
            }
            if (data.Tables[0].Rows[0]["RESULT"] != System.DBNull.Value)
            {
                obj.RESULT = (System.String)data.Tables[0].Rows[0]["RESULT"];
            }
            if (data.Tables[0].Rows[0]["TIMES"] != System.DBNull.Value)
            {
                obj.TIMES = (System.Decimal)data.Tables[0].Rows[0]["TIMES"];
            }
            if (data.Tables[0].Rows[0]["TOSAP"] != System.DBNull.Value)
            {
                obj.TOSAP = (System.String)data.Tables[0].Rows[0]["TOSAP"];
            }
            obj.AcceptChange();
            return obj;
        }
        public static List<R_BACKFLUSH_HISTORY> DataAdapert(DataSet data, bool MulValue)
        {
            R_BACKFLUSH_HISTORY obj = new R_BACKFLUSH_HISTORY();
            List<R_BACKFLUSH_HISTORY> retL = new List<R_BACKFLUSH_HISTORY>();
            for (int i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                obj = new R_BACKFLUSH_HISTORY();
                if (data.Tables[0].Rows[i]["WORKORDERNO"] != System.DBNull.Value)
                {
                    obj.WORKORDERNO = (System.String)data.Tables[0].Rows[i]["WORKORDERNO"];
                }
                if (data.Tables[0].Rows[i]["SKUNO"] != System.DBNull.Value)
                {
                    obj.SKUNO = (System.String)data.Tables[0].Rows[i]["SKUNO"];
                }
                if (data.Tables[0].Rows[i]["SAP_STATION"] != System.DBNull.Value)
                {
                    obj.SAP_STATION = (System.String)data.Tables[0].Rows[i]["SAP_STATION"];
                }
                if (data.Tables[0].Rows[i]["WORKORDER_QTY"] != System.DBNull.Value)
                {
                    obj.WORKORDER_QTY = (System.Decimal)data.Tables[0].Rows[i]["WORKORDER_QTY"];
                }
                if (data.Tables[0].Rows[i]["BACKFLUSH_QTY"] != System.DBNull.Value)
                {
                    obj.BACKFLUSH_QTY = (System.Decimal)data.Tables[0].Rows[i]["BACKFLUSH_QTY"];
                }
                if (data.Tables[0].Rows[i]["SFC_QTY"] != System.DBNull.Value)
                {
                    obj.SFC_QTY = (System.Decimal)data.Tables[0].Rows[i]["SFC_QTY"];
                }
                if (data.Tables[0].Rows[i]["DIFF_QTY"] != System.DBNull.Value)
                {
                    obj.DIFF_QTY = (System.Decimal)data.Tables[0].Rows[i]["DIFF_QTY"];
                }
                if (data.Tables[0].Rows[i]["SFC_STATION"] != System.DBNull.Value)
                {
                    obj.SFC_STATION = (System.String)data.Tables[0].Rows[i]["SFC_STATION"];
                }
                if (data.Tables[0].Rows[i]["HAND_QTY"] != System.DBNull.Value)
                {
                    obj.HAND_QTY = (System.Decimal)data.Tables[0].Rows[i]["HAND_QTY"];
                }
                if (data.Tables[0].Rows[i]["LAST_SFC_QTY"] != System.DBNull.Value)
                {
                    obj.LAST_SFC_QTY = (System.Decimal)data.Tables[0].Rows[i]["LAST_SFC_QTY"];
                }
                if (data.Tables[0].Rows[i]["DIFF_QTY1"] != System.DBNull.Value)
                {
                    obj.DIFF_QTY1 = (System.Decimal)data.Tables[0].Rows[i]["DIFF_QTY1"];
                }
                if (data.Tables[0].Rows[i]["DIFF_QTY2"] != System.DBNull.Value)
                {
                    obj.DIFF_QTY2 = (System.Decimal)data.Tables[0].Rows[i]["DIFF_QTY2"];
                }
                if (data.Tables[0].Rows[i]["HISTORY_HAND_QTY"] != System.DBNull.Value)
                {
                    obj.HISTORY_HAND_QTY = (System.Decimal)data.Tables[0].Rows[i]["HISTORY_HAND_QTY"];
                }
                if (data.Tables[0].Rows[i]["MRB_QTY"] != System.DBNull.Value)
                {
                    obj.MRB_QTY = (System.Decimal)data.Tables[0].Rows[i]["MRB_QTY"];
                }
                if (data.Tables[0].Rows[i]["REC_TYPE"] != System.DBNull.Value)
                {
                    obj.REC_TYPE = (System.String)data.Tables[0].Rows[i]["REC_TYPE"];
                }
                if (data.Tables[0].Rows[i]["BACK_DATE"] != System.DBNull.Value)
                {
                    obj.BACK_DATE = (System.DateTime)data.Tables[0].Rows[i]["BACK_DATE"];
                }
                if (data.Tables[0].Rows[i]["RESULT"] != System.DBNull.Value)
                {
                    obj.RESULT = (System.String)data.Tables[0].Rows[i]["RESULT"];
                }
                if (data.Tables[0].Rows[i]["TIMES"] != System.DBNull.Value)
                {
                    obj.TIMES = (System.Decimal)data.Tables[0].Rows[i]["TIMES"];
                }
                if (data.Tables[0].Rows[i]["TOSAP"] != System.DBNull.Value)
                {
                    obj.TOSAP = (System.String)data.Tables[0].Rows[i]["TOSAP"];
                }
                obj.AcceptChange();
                retL.Add(obj);
            }
            return retL;
        }
    }
}
