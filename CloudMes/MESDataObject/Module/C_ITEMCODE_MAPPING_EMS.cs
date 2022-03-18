using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Text.RegularExpressions;

namespace MESDataObject.Module
{
    /// <summary>
    /// ADD BY HGB 2019.06.20
    /// </summary>
    public class T_C_ITEMCODE_MAPPING_EMS : DataObjectTable
    {
        public T_C_ITEMCODE_MAPPING_EMS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ITEMCODE_MAPPING_EMS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ITEMCODE_MAPPING_EMS);
            TableName = "C_ITEMCODE_MAPPING_EMS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="var_keypartno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckKPExists(string var_keypartno, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM c_itemcode_mapping_ems WHERE fox_item_code = '{var_keypartno}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_ITEMCODE_MAPPING_EMS LoadPartNoFromTb(string var_keypartno, OleExec DB)
        { 
            return DB.ORM.Queryable<C_ITEMCODE_MAPPING_EMS>().Where(t => t.FOX_ITEM_CODE == var_keypartno).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 傳入SKUNO，獲取華為料號,PARTNO,(ap.get_customer_partno('KP_NO', var_skuno))
        /// ADD BY HGB 2016.06.20
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public string Get_Customer_Partno(string var_type, string var_keypartno, OleExec DB)
        {
            string var_return_data = string.Empty;
            try
            {
                string var_barcode_2d_flag="N",var_version="N",
                    var_newrule="N",
                    var_temp="";
                if (var_keypartno.IndexOf("[)>") >= 0)

                {
                    var_barcode_2d_flag = "Y";
                }
                else
                {
                    var_barcode_2d_flag = "N";
                }

                if ((var_keypartno.IndexOf("1P") >= 0 && var_keypartno.IndexOf("18VLEHWT") >= 0) || (var_keypartno.Substring(0, 1) == "S" && var_keypartno.Length >= 12 && var_keypartno.IndexOf("1P") >= 0))
                {
                    var_newrule = "Y";
                }
                else
                {
                    var_newrule = "N";
                }                

                //if (var_type == "CONVERT_BOXSN" && var_barcode_2d_flag=="Y")
                //{
                //    string AE="ABCDE",FZ = "FGHIJKLMNOPQRSTUVWXYZ",_59="56789";

                //    var_temp = var_keypartno.Substring(5, 1).ToString();
                //    if (AE.IndexOf(var_temp) >= 0)//A到E之間的直接返回
                //    {
                //        var_return_data = var_keypartno;
                //    }
                //    else if (FZ.IndexOf(var_temp) >= 0 || _59.IndexOf(var_temp) >= 0)//F~Z之間或者5~9之間的報錯
                //    {
                //        var_return_data = "NG,Over Range,must be 0-4 or A-E";
                //    }
                //    else
                //    {
                //        switch (var_temp)
                //        {
                //            case "0":
                //                var_temp = "A";
                //                break;
                //            case "1":
                //                var_temp = "B";
                //                break;
                //            case "2":
                //                var_temp = "C";
                //                break;
                //            case "3":
                //                var_temp = "D";
                //                break;
                //            case "4":
                //                var_temp = "E";
                //                break;
                //        }
                //        var_return_data = var_keypartno.Substring(0, 5) + var_temp + var_keypartno.Substring(6, 6);
                //    }
                //}

                if (var_type == "SKUTYPE")
                {
                    string subtype = var_keypartno.Substring(0, 1);
                    if (subtype == "T" || subtype == "Q" || subtype == "R" || subtype == "N")
                    {
                        return "NEW";
                    }
                    else if (subtype == "F" || subtype == "C" || subtype == "D" || subtype == "G" || subtype == "L")
                    {
                        return "NEW";
                    }
                    else
                    {
                        return "OLD";
                    }
                }

                if (var_type == "KP_NO")
                {
                    T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(DB, DB_TYPE_ENUM.Oracle);
                    if (t_c_itemcode_mapping_ems.CheckKPExists(var_keypartno, DB))
                    {
                        var_return_data = t_c_itemcode_mapping_ems.LoadPartNoFromTb(var_keypartno, DB).HW_ITEM_CODE;

                        return var_return_data;
                    }

                    string subtype = var_keypartno.Substring(0, 1);
                    if (subtype == "T" || subtype == "Q" || subtype == "R" || subtype == "N")
                    {
                        var_version = "Y";//啟用版本的--
                    }
                    else if (subtype == "F" || subtype == "C" || subtype == "D" || subtype == "G" || subtype == "L")
                    {
                        var_version = "Y";//長料號，但沒有啟用版本的--
                    }

                    if (var_version == "N")
                    {
                        if (var_keypartno.Substring(0, 1) == "U")
                        {
                            if (var_keypartno.Length == 9)
                            {
                                var_return_data = var_keypartno;
                            }
                            else if (var_keypartno.Length == 11)
                            {
                                var_return_data = var_keypartno.Substring(2, 9);
                            }
                            else if (var_keypartno.Length == 18)
                            {
                                var_return_data = var_keypartno.Substring(0, 9);
                            }
                            else if (var_keypartno.Length == 19)
                            {
                                var_return_data = var_keypartno.Substring(0, 9);
                            }
                            else
                            {
                                var_return_data = var_keypartno.Substring(2, var_keypartno.Length - 2);
                            }
                        }
                        else if (var_keypartno.Substring(0, 1) == "P")
                        {
                            if (var_keypartno.Length == 9)
                            {
                                var_return_data = var_keypartno.Substring(1, var_keypartno.Length - 2);
                            }
                        }
                        else//這里都是舊的料號規則
                        {
                            if (var_keypartno.Length <= 8)
                            {
                                var_return_data = var_keypartno;
                            }
                            else if ((var_keypartno.Substring(0, 1) == "W" || var_keypartno.Substring(0, 1) == "A" || var_keypartno.Substring(0, 1) == "B") && (var_keypartno.Length == 9 || var_keypartno.Length == 10))
                            {
                                var_return_data = var_keypartno.Substring(var_keypartno.Length - 8, 8);
                            }
                            else if ((var_keypartno.Substring(0, 1) == "W" || var_keypartno.Substring(0, 1) == "A" || var_keypartno.Substring(0, 1) == "B") && (var_keypartno.Length > 10))
                            {

                                if (var_keypartno.IndexOf("-") > 9)
                                {
                                    //--'WF03010ALD-BC10' --    '-' 的位置大於9 --
                                    var_return_data = var_keypartno.Substring(0, var_keypartno.IndexOf("-"));
                                    var_return_data = var_return_data.Substring(var_return_data.Length - 8, 8);
                                }
                                else if (var_keypartno.IndexOf("-") > 0 && var_keypartno.IndexOf("-") < 4)
                                {
                                    //'W-DP2-03020LVF'--    '-''的位置小於4 --
                                    var_return_data = var_keypartno.Substring(var_keypartno.Length - 8, 8);

                                }
                                else if (var_keypartno.IndexOf("-") < 0 && var_keypartno.Length == 11)
                                {
                                    //W02113455A1--
                                    var_return_data = var_keypartno.Substring(1, 8);
                                }
                                else if (var_keypartno.IndexOf("-") < 0 && (var_keypartno.Length == 12 || var_keypartno.Length == 13))
                                {
                                    var_return_data = var_keypartno.Substring(2, 8);
                                }
                                else
                                {
                                    var_return_data = var_keypartno.Substring(var_keypartno.Length - 8, 8);
                                }

                            }
                            else if ((var_keypartno.Substring(0, 1) != "W" || var_keypartno.Substring(0, 1) != "A" || var_keypartno.Substring(0, 1) != "B") && (var_keypartno.Length == 9 || var_keypartno.Length == 10))
                            {
                                var_return_data = var_keypartno;
                            }
                            else if ((var_keypartno.Substring(0, 1) != "W" || var_keypartno.Substring(0, 1) != "A" || var_keypartno.Substring(0, 1) != "B") && (var_keypartno.Length == 9 || var_keypartno.Length > 10))
                            {
                                var_return_data = var_keypartno;
                            }
                            else
                            {
                                var_return_data = var_keypartno;
                            }
                        }
                    }
                    else
                    {

                        if (var_keypartno.Substring(0, 1) == "T" || var_keypartno.Substring(0, 1) == "Q" || var_keypartno.Substring(0, 1) == "R" || var_keypartno.Substring(0, 1) == "N")
                        {
                            //啟用版本的--
                            if (var_keypartno.Substring(0, 2) == "RQ")
                            {
                                var_return_data = var_keypartno.Substring(2, 8);
                            }
                            else
                            {
                                if (var_keypartno.Length - 1 >= 8)
                                {
                                    var_return_data = var_keypartno.Substring(1, 8);
                                }
                                else
                                {
                                    var_return_data = var_keypartno.Substring(1, var_keypartno.Length - 1);
                                }
                            }
                        }
                        else if (var_keypartno.Substring(0, 1) == "F" || var_keypartno.Substring(0, 1) == "C" || var_keypartno.Substring(0, 1) == "D" || var_keypartno.Substring(0, 1) == "G" || var_keypartno.Substring(0, 1) == "L")
                        {
                            //長料號，但沒有啟用版本的--
                            var_return_data = var_keypartno.Substring(1, var_keypartno.Length - 1);
                        }
                        else
                        {
                            var_return_data = var_keypartno.Substring(1, var_keypartno.Length - 1);
                        }

                    }

                    //System.Text.RegularExpressions.Regex reg ;
                    //if (var_type == "CONVERT_PCBSN")
                    //{                        
                    //    var_temp = var_keypartno.Substring(5, 1);
                    //    reg = new Regex("[^0-4]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //        var_part_no = var_keypartno;
                    //    }

                    //    reg = new Regex("[^5-9]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //    var_part_no= "ERROR,Over Range,must be 0-4 or A-E";
                    //    }
                    //    reg = new Regex("[^F-Z]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //        var_part_no = "ERROR,Over Range,must be 0-4 or A-E";
                    //    }
                    //    if (var_temp=="A")
                    //    {
                    //        var_temp = "0";
                    //    }
                    //    if (var_temp == "B")
                    //    {
                    //        var_temp = "1";
                    //    }
                    //    if (var_temp == "C")
                    //    {
                    //        var_temp = "2";
                    //    }
                    //    if (var_temp == "D")
                    //    {
                    //        var_temp = "3";
                    //    }
                    //    if (var_temp == "E")
                    //    {
                    //        var_temp = "4";
                    //    }
                    //    var_part_no = var_keypartno.Substring(0,5)+ var_temp+ var_keypartno.Substring(6, 6);
                    //}

                    //if (var_type == "CONVERT_BOXSN")
                    //{
                    //    string var_temp = string.Empty;
                    //    var_temp = var_keypartno.Substring(5, 1);
                    //    reg = new Regex("[^A-E]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //        var_part_no = var_keypartno;
                    //    }

                    //    reg = new Regex("[^F-Z]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //        var_part_no = "ERROR,Over Range,must be 0-4 or A-E";
                    //    }
                    //    reg = new Regex("[^5-9]");
                    //    if (reg.IsMatch(var_temp))
                    //    {
                    //        var_part_no = "ERROR,Over Range,must be 0-4 or A-E";
                    //    }
                    //    if (var_temp == "0")
                    //    {
                    //        var_temp = "A";
                    //    }
                    //    if (var_temp == "1")
                    //    {
                    //        var_temp = "B";
                    //    }
                    //    if (var_temp == "2")
                    //    {
                    //        var_temp = "C";
                    //    }
                    //    if (var_temp == "3")
                    //    {
                    //        var_temp = "D";
                    //    }
                    //    if (var_temp == "4")
                    //    {
                    //        var_temp = "E";
                    //    }
                    //    var_part_no = var_keypartno.Substring(0, 5) + var_temp + var_keypartno.Substring(6, 6);
                    //}
                }

                System.Text.RegularExpressions.Regex reg;
                if (var_type == "CONVERT_PCBSN" && var_barcode_2d_flag == "Y")
                {
                    var_temp = var_keypartno.Substring(5, 1);
                    reg = new Regex("[^0-4]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = var_keypartno;
                    }

                    reg = new Regex("[^5-9]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = "ERROR,Over Range,must be 0-4 or A-E";
                    }
                    reg = new Regex("[^F-Z]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = "ERROR,Over Range,must be 0-4 or A-E";
                    }
                    if (var_temp == "A")
                    {
                        var_temp = "0";
                    }
                    if (var_temp == "B")
                    {
                        var_temp = "1";
                    }
                    if (var_temp == "C")
                    {
                        var_temp = "2";
                    }
                    if (var_temp == "D")
                    {
                        var_temp = "3";
                    }
                    if (var_temp == "E")
                    {
                        var_temp = "4";
                    }
                    var_return_data = var_keypartno.Substring(0, 5) + var_temp + var_keypartno.Substring(6, 6);
                }

                if (var_type == "CONVERT_BOXSN" && var_barcode_2d_flag == "Y")
                {                    
                    var_temp = var_keypartno.Substring(5, 1);
                    reg = new Regex("[^A-E]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = var_keypartno;
                    }

                    reg = new Regex("[^F-Z]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = "ERROR,Over Range,must be 0-4 or A-E";
                    }
                    reg = new Regex("[^5-9]");
                    if (reg.IsMatch(var_temp))
                    {
                        var_return_data = "ERROR,Over Range,must be 0-4 or A-E";
                    }
                    if (var_temp == "0")
                    {
                        var_temp = "A";
                    }
                    if (var_temp == "1")
                    {
                        var_temp = "B";
                    }
                    if (var_temp == "2")
                    {
                        var_temp = "C";
                    }
                    if (var_temp == "3")
                    {
                        var_temp = "D";
                    }
                    if (var_temp == "4")
                    {
                        var_temp = "E";
                    }
                    var_return_data = var_keypartno.Substring(0, 5) + var_temp + var_keypartno.Substring(6, 6);
                }
                #region
                //T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(DB, DB_TYPE_ENUM.Oracle);
                //if (t_c_itemcode_mapping_ems.CheckKPExists(var_keypartno, DB))
                //{
                //    var_part_no = t_c_itemcode_mapping_ems.LoadPartNoFromTb(var_keypartno, DB).HW_ITEM_CODE;

                //    return var_part_no;
                //}

                //if (var_barcode_2d_flag == "N")
                //{
                //    if (var_type == "KP_NO")
                //    {
                //        if (var_keypartno.Substring(0, 1) == "U")
                //        {
                //            if (var_keypartno.Length == 9)
                //            {
                //                var_part_no = var_keypartno;
                //            }
                //            else if (var_keypartno.Length == 11)
                //            {
                //                var_part_no = var_keypartno.Substring(2, 9);
                //            }
                //            else if (var_keypartno.Length == 18)
                //            {
                //                var_part_no = var_keypartno.Substring(0, 9);
                //            }
                //            else if (var_keypartno.Length == 19)
                //            {
                //                var_part_no = var_keypartno.Substring(0, 9);
                //            }
                //            else
                //            {
                //                var_part_no = var_keypartno.Substring(2, var_keypartno.Length - 2);
                //            }



                //        }
                //        else if (var_keypartno.Substring(0, 1) == "P")
                //        {
                //            if (var_keypartno.Length == 9)
                //            {
                //                var_part_no = var_keypartno.Substring(1, var_keypartno.Length - 2);
                //            }
                //        }
                //        else//這里都是舊的料號規則
                //        {
                //            if (var_keypartno.Length <= 8)
                //            {
                //                var_part_no = var_keypartno;
                //            }
                //            else if ((var_keypartno.Substring(0, 1) == "W" || var_keypartno.Substring(0, 1) == "A" || var_keypartno.Substring(0, 1) == "B") && (var_keypartno.Length == 9 || var_keypartno.Length == 10))
                //            {
                //                var_part_no = var_keypartno.Substring(var_keypartno.Length - 8, 8);
                //            }
                //            else if ((var_keypartno.Substring(0, 1) == "W" || var_keypartno.Substring(0, 1) == "A" || var_keypartno.Substring(0, 1) == "B") && (var_keypartno.Length > 10))
                //            {

                //                if (var_keypartno.IndexOf("-") > 9)
                //                {
                //                    //--'WF03010ALD-BC10' --    '-' 的位置大於9 --
                //                    var_part_no = var_keypartno.Substring(0, var_keypartno.IndexOf("-"));
                //                    var_part_no = var_part_no.Substring(var_part_no.Length - 8, 8);
                //                }
                //                else if (var_keypartno.IndexOf("-") > 0 && var_keypartno.IndexOf("-") < 4)
                //                {
                //                    //'W-DP2-03020LVF'--    '-''的位置小於4 --
                //                    var_part_no = var_keypartno.Substring(var_keypartno.Length - 8, 8);

                //                }
                //                else if (var_keypartno.IndexOf("-") < 0 && var_keypartno.Length == 11)
                //                {
                //                    //W02113455A1--
                //                    var_part_no = var_keypartno.Substring(1, 8);
                //                }
                //                else if (var_keypartno.IndexOf("-") < 0 && (var_keypartno.Length == 12 || var_keypartno.Length == 13))
                //                {
                //                    var_part_no = var_keypartno.Substring(2, 8);
                //                }
                //                else
                //                {
                //                    var_part_no = var_keypartno.Substring(var_keypartno.Length - 8, 8);
                //                }

                //            }
                //            else if ((var_keypartno.Substring(0, 1) != "W" || var_keypartno.Substring(0, 1) != "A" || var_keypartno.Substring(0, 1) != "B") && (var_keypartno.Length == 9 || var_keypartno.Length == 10))
                //            {
                //                var_part_no = var_keypartno;
                //            }
                //            else if ((var_keypartno.Substring(0, 1) != "W" || var_keypartno.Substring(0, 1) != "A" || var_keypartno.Substring(0, 1) != "B") && (var_keypartno.Length == 9 || var_keypartno.Length > 10))
                //            {
                //                var_part_no = var_keypartno;
                //            }
                //            else
                //            {
                //                var_part_no = var_keypartno;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    if (var_type == "KP_NO")
                //    {
                //        if (var_keypartno.Substring(0, 1) == "T" || var_keypartno.Substring(0, 1) == "Q" || var_keypartno.Substring(0, 1) == "R" || var_keypartno.Substring(0, 1) == "N")
                //        {
                //            //啟用版本的--
                //            if (var_keypartno.Substring(0, 2) == "RQ")
                //            {
                //                var_part_no = var_keypartno.Substring(2, 8);
                //            }
                //            else
                //            {
                //                if (var_keypartno.Length - 1 >= 8)
                //                {
                //                    var_part_no = var_keypartno.Substring(1, 8);
                //                }
                //                else
                //                {
                //                    var_part_no = var_keypartno.Substring(1, var_keypartno.Length - 1);
                //                }
                //            }
                //        }
                //        else if (var_keypartno.Substring(0, 1) == "F" || var_keypartno.Substring(0, 1) == "C" || var_keypartno.Substring(0, 1) == "D" || var_keypartno.Substring(0, 1) == "G" || var_keypartno.Substring(0, 1) == "L")
                //        {
                //            //長料號，但沒有啟用版本的--
                //            var_part_no = var_keypartno.Substring(1, var_keypartno.Length - 1);
                //        }
                //        else
                //        {
                //            var_part_no = var_keypartno.Substring(1, var_keypartno.Length - 1);
                //        }
                //    }
                //}
                #endregion


                return var_return_data;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }


        }

    }
    public class Row_C_ITEMCODE_MAPPING_EMS : DataObjectBase
    {
        public Row_C_ITEMCODE_MAPPING_EMS(DataObjectInfo info) : base(info)
        {

        }
        public C_ITEMCODE_MAPPING_EMS GetDataObject()
        {
            C_ITEMCODE_MAPPING_EMS DataObject = new C_ITEMCODE_MAPPING_EMS();
            DataObject.ID = this.ID;
            DataObject.FOX_ITEM_CODE = this.FOX_ITEM_CODE;
            DataObject.FOX_VERSION = this.FOX_VERSION;
            DataObject.HW_ITEM_CODE = this.HW_ITEM_CODE;
            DataObject.ITEM_REMARK = this.ITEM_REMARK;
            DataObject.HW_VERSION = this.HW_VERSION;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.CREATE_ACCOUNT = this.CREATE_ACCOUNT;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string FOX_ITEM_CODE
        {
            get
            {
                return (string)this["FOX_ITEM_CODE"];
            }
            set
            {
                this["FOX_ITEM_CODE"] = value;
            }
        }
        public string FOX_VERSION
        {
            get
            {
                return (string)this["FOX_VERSION"];
            }
            set
            {
                this["FOX_VERSION"] = value;
            }
        }
        public string HW_ITEM_CODE
        {
            get
            {
                return (string)this["HW_ITEM_CODE"];
            }
            set
            {
                this["HW_ITEM_CODE"] = value;
            }
        }
        public string ITEM_REMARK
        {
            get
            {
                return (string)this["ITEM_REMARK"];
            }
            set
            {
                this["ITEM_REMARK"] = value;
            }
        }
        public string HW_VERSION
        {
            get
            {
                return (string)this["HW_VERSION"];
            }
            set
            {
                this["HW_VERSION"] = value;
            }
        }
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string CREATE_ACCOUNT
        {
            get
            {
                return (string)this["CREATE_ACCOUNT"];
            }
            set
            {
                this["CREATE_ACCOUNT"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class C_ITEMCODE_MAPPING_EMS
    {
        public string ID { get; set; }
        public string FOX_ITEM_CODE { get; set; }
        public string FOX_VERSION { get; set; }
        public string HW_ITEM_CODE { get; set; }
        public string ITEM_REMARK { get; set; }
        public string HW_VERSION { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string CREATE_ACCOUNT { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}