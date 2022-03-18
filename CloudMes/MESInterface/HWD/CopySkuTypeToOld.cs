using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESPubLab;

namespace MESInterface.HWD
{
    public class CopySkuTypeToOld : taskBase
    {
        private OleExec newSFCDB = null;
        private OleExec oldSFCDB = null;
        string ip;

        public string updateDate = "";
        public override void init()
        {
            //base.init();
            try
            {

                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                Output.UI = new CopySkuTypeToOld_UI(this);
            }
            catch (Exception ex)
            {
                throw new Exception("Init CopySkuTypeToOld Fail" + ex.Message);
            }
        }

        public override void Start()
        {
            string oldSql = "";
            string newSql = "";
            string runSql = "";
            string codeName = "";
            string codeValue = "";
            string description = "";
            string route = "";
            string model = "";
            string version = "01";
            string custpartno = "";
            DataTable dtNew = new DataTable();
            DataTable dtOld = new DataTable();
            DataTable dtRoute = new DataTable();

            try
            {
                newSFCDB = new OleExec("HWDMES", false);
                oldSFCDB = new OleExec("HWD_OLD_SFCDB", false);
                newSFCDB.ThrowSqlExeception = true;
                oldSFCDB.ThrowSqlExeception = true;

                if (updateDate != "")
                {
                    newSql = $@"select * from c_sku where to_date(to_char(edit_time,'yyyy/mm/dd'),'yyyy/mm/dd') =to_date('{updateDate}','yyyy/mm/dd')";
                }
                else
                {
                    newSql = "select * from c_sku where edit_time>sysdate-3";
                }
                dtNew = newSFCDB.ExecSelect(newSql).Tables[0];
                foreach (DataRow row in dtNew.Rows)
                {
                    if (row["SKUNO"].ToString() != "")
                    {
                        try
                        {
                            runSql = $@"select c.* from r_sku_route a,c_sku b,c_route c where a.sku_id=b.id and c.id=a.route_id and b.skuno='{row["SKUNO"].ToString()}'";
                            dtRoute = newSFCDB.ExecSelect(runSql).Tables[0];
                            dtRoute = newSFCDB.ExecSelect(runSql).Tables[0];
                            if (dtRoute.Rows.Count == 0)
                            {
                                //throw new Exception(row["SKUNO"].ToString() + " can't setting route!");
                                route = "HWD51070DBB-A";//2018.11.05 PE杜軍要求如果沒有配置路由則選擇一個默認路由
                            }
                            else
                            {
                                route = dtRoute.Rows[0]["ROUTE_NAME"].ToString();
                            }

                            oldSql = $@"select * from sfccodelike where skuno='{row["SKUNO"].ToString()}'";
                            dtOld = oldSFCDB.ExecSelect(oldSql).Tables[0];
                            runSql = "";
                            if (dtOld.Rows.Count > 0)
                            {
                                if (row["SKU_TYPE"].ToString() != "" && dtOld.Rows[0]["CATEGORY"].ToString() != row["SKU_TYPE"].ToString())
                                {
                                    runSql = $@" update sfccodelike set category='{row["SKU_TYPE"].ToString()}' where skuno='{row["SKUNO"].ToString()}'";
                                }                                
                            }
                            else
                            {
                                if (row["SKU_NAME"].ToString() != "")
                                {
                                    codeName = row["SKU_NAME"].ToString();
                                    codeValue = row["SKU_NAME"].ToString();
                                }
                                else
                                {
                                    codeName = row["SKUNO"].ToString();
                                    codeValue = row["SKUNO"].ToString();
                                }

                                if (row["DESCRIPTION"].ToString() != "")
                                {
                                    description = row["DESCRIPTION"].ToString();
                                }
                                else
                                {
                                    description = row["SKUNO"].ToString();
                                }
                                if (row["VERSION"].ToString() == "")
                                {
                                    //throw new Exception(row["SKUNO"].ToString() + " the version is null!");
                                    version = "01";
                                }
                                else
                                {
                                    version = row["VERSION"].ToString();
                                }

                                if (row["SKU_TYPE"].ToString() != "")
                                {

                                    model = row["SKU_TYPE"].ToString();                
                                }
                                else
                                {
                                    model = "MODEL";
                                }
                                if (row["CUST_PARTNO"].ToString() != "")
                                {

                                    custpartno = row["CUST_PARTNO"].ToString();
                                }
                                else
                                {
                                    custpartno = row["SKUNO"].ToString();
                                }
                                
                                runSql = $@"insert into sfccodelike
                                (category,codename,codevalue,skuno,version,custpartno,sfcroute,createby,createdate,series,ctntype,pltype,description)
                                values
                                ('{model}',
                                '{codeName}',
                                '{codeValue}',
                                '{row["SKUNO"].ToString()}',
                                '{version}',
                                '{custpartno}',
                                '{route}', 
                                '{row["EDIT_EMP"].ToString()}',
                                 sysdate,
                                'HWD','C','PL','{description}')";
                            }
                            if (runSql != "")
                            {
                                oldSFCDB.ExecSQL(runSql);
                                oldSFCDB.CommitTrain();
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog.WriteIntoMESLog(newSFCDB, "HWD", "MESInterface", "MESInterface.HWD.CopySkuTypeToOld", "CopySkuTypeToOld", ip + ";" + row["SKUNO"].ToString() + ";" + ex.Message.ToString(), "", "interface");
                            newSFCDB.CommitTrain();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Start CopySkuTypeToOld Fail" + ex.Message);                
            }
        }
    }
}
