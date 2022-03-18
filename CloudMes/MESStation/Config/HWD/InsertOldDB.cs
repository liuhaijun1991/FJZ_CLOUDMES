using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.HWD
{
    public class InsertOldDB
    {
        public static int InsertSfccodelike(string skuno,string category,string codeName,string codeValue,string description,string version,string custpartno,string route, string empno)
        {
            try
            {
                if (skuno.Trim() == "")
                {
                    throw new Exception("Insert sfccodelike fail,please input skuno!");
                }
                if (category.Trim() == "")
                {
                    //throw new Exception("Insert sfccodelike fail,please input category!");
                    category = "MODEL";//2018.11.05 PE杜軍要求如果沒有配置category則默認MODEL
                }
                if (version.Trim() == "")
                {
                    version = "01";//2018.11.05 PE杜軍要求如果沒有配置version則默認01
                }
                if (codeName.Trim() == "")
                {
                    codeName = skuno;
                }
                if (codeValue.Trim() == "")
                {
                    codeValue = skuno;
                }
                if (description.Trim() == "")
                {
                    description = skuno;
                }
                if (custpartno.Trim() == "")
                {
                    custpartno = skuno;
                }
                if (route.Trim() == "")
                {
                    route = "HWD51070DBB-A";//2018.11.05 PE杜軍要求如果沒有配置路由則選擇一個默認路由
                }
                OleExec oldDB = new OleExec("HWD_OLD_SFCDB", false);
                oldDB.ThrowSqlExeception = true;
                string selectSql = $@"select * from sfccodelike where skuno='{skuno.Trim()}'";
                DataTable dtOld = oldDB.ExecSelect(selectSql).Tables[0];
                string insertSql = "";               
                if (dtOld.Rows.Count > 0)
                {
                    insertSql = $@" update sfccodelike set category='{category.Trim()}',createdate=sysdate,createby='{empno.Trim()}' where skuno='{skuno.Trim()}'";                    
                }
                else
                {
                    insertSql = $@"insert into sfccodelike
                                (category,codename,codevalue,skuno,version,custpartno,sfcroute,createby,createdate,series,ctntype,pltype,description)
                                values
                                ('{category.Trim()}',
                                '{codeName.Trim()}',
                                '{codeValue.Trim()}',
                                '{skuno.Trim()}',
                                '{version.Trim()}',
                                '{custpartno.Trim()}',
                                '{route.Trim()}', 
                                '{empno}',
                                 sysdate,
                                'HWD','C','PL','{description.Trim()}')";                    
                }                                     
                return Convert.ToInt32(oldDB.ExecSQL(insertSql));
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public static int InsertProductionConfig(OleExec APBD, string p_no, string p_version, string p_desc, string cust_pno, string edit_emp)
        {
            try
            {
                p_no = p_no.Trim();
                p_version = p_version.Trim();
                p_desc = p_desc.Trim();
                cust_pno = cust_pno.Trim();
                string selectSql = $@"select * from mes1.c_product_config where p_no='{p_no}' and p_version='{p_version}'";
                DataTable dt = APBD.ExecSelect(selectSql).Tables[0];
                string insertSql = "";
                if (dt.Rows.Count > 0)
                {
                    insertSql = $@" update  mes1.c_product_config set p_desc='{p_desc}',cust_pno='{cust_pno}',edit_emp='{edit_emp}',edit_time=sysdate where p_no='{p_no}' and p_version='{p_version}'";
                }
                else
                {
                    insertSql = $@"insert into  mes1.c_product_config(cust_code,
                                   bu_code,
                                   p_no,
                                   p_version,
                                   p_desc,
                                   p_type,
                                   sn_len,
                                   pth_flag,
                                   wo_pno,
                                   process_type,
                                   edit_time,
                                   edit_emp,
                                   weld_qty,
                                   panel_type,
                                   link_qty,
                                   process_flag,
                                   memo,
                                   data1,
                                   data2,
                                   cust_pno) values('HUAWEI',
                                   'HWD','{p_no}','{p_version}','{p_desc}','OEM','11','1','','0',sysdate,'{edit_emp}','1','0','1','','','','N','{cust_pno}') ";
                }
                return Convert.ToInt32(APBD.ExecSQL(insertSql));
            }
            catch (Exception ex)
            {
                throw ex;
                //return 0;
            }
        }
    }
}
