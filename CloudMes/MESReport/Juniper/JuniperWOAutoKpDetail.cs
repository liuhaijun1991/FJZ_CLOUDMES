using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESPubLab.Json;
using MESDataObject.Module.Juniper;
using MESDataObject.Module;

namespace MESReport.Juniper
{

    //查詢工單AutoKP信息
    public class JuniperWOAutoKpDetail : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };

        public JuniperWOAutoKpDetail()
        {
            Inputs.Add(WO);
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (WO.Value == null)
                {
                    throw new Exception("WO Can not be null");
                }

                string wo = WO.Value.ToString();

                DataTable linkTable = new DataTable();

               var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(wo, "JuniperAutoKPConfig", SFCDB);


                if (kpl != null)
                {
                    linkTable.Columns.Add("WO", typeof(string));
                    linkTable.Columns.Add("PN", typeof(string));
                    linkTable.Columns.Add("PN_SERIALIZATION", typeof(string));
                    linkTable.Columns.Add("CUST_PN", typeof(string));
                    linkTable.Columns.Add("PN_7XX", typeof(string));
                    linkTable.Columns.Add("SN_RULE", typeof(string));
                    linkTable.Columns.Add("QTY", typeof(string));
                    linkTable.Columns.Add("TYPE", typeof(string));
                    linkTable.Columns.Add("REV", typeof(string));
                    linkTable.Columns.Add("CLEI_CODE", typeof(string));
                    linkTable.Columns.Add("CHAS_SN", typeof(string));

                    foreach (JuniperAutoKpConfig v in kpl)
                    {
                        DataRow r = linkTable.NewRow();
                        r["WO"] = wo;
                        r["PN"] = v.PN;
                        r["PN_SERIALIZATION"] = v.PN_SERIALIZATION;
                        r["CUST_PN"] = v.CUST_PN;
                        r["PN_7XX"] = v.PN_7XX;
                        r["SN_RULE"] = v.SN_RULE;
                        r["QTY"] = v.QTY;
                        r["TYPE"] = v.TYPE;
                        r["REV"] = v.REV;
                        r["CLEI_CODE"] = v.CLEI_CODE;
                        r["CHAS_SN"] = v.CHAS_SN;

                        linkTable.Rows.Add(r);
                    }
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(linkTable, null);
                reportTable.Tittle = "WoKpList";
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public override void DownFile()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable linkTable = new DataTable("JuniperAutoKPConfig");
                List<JuniperAutoKpConfig> kpl = new List<JuniperAutoKpConfig>();
                List<List<JuniperAutoKpConfig>> kpLL = new List<List<JuniperAutoKpConfig>>();
                List<JuniperAutoKpConfig_Extend> kpLExt = new List<JuniperAutoKpConfig_Extend>();
                
                linkTable.Columns.Add("WO");
                linkTable.Columns.Add("PN");
                linkTable.Columns.Add("PN_SERIALIZATION");
                linkTable.Columns.Add("CUST_PN");
                linkTable.Columns.Add("PN_7XX");
                linkTable.Columns.Add("SN_RULE");
                linkTable.Columns.Add("QTY");
                linkTable.Columns.Add("TYPE");
                linkTable.Columns.Add("REV");
                linkTable.Columns.Add("CLEI_CODE");
                linkTable.Columns.Add("CHAS_SN");


                kpLExt = GetAllJuniperAutoKpConfig("JuniperAutoKPConfig", SFCDB);

                if (kpLL != null)
                {
                    foreach (JuniperAutoKpConfig_Extend model in kpLExt)
                    {
                        foreach (JuniperAutoKpConfig v in model.AutoKpConfig)
                        {
                            DataRow r = linkTable.NewRow();
                            r["WO"] = model.WorkOrderNo;
                            r["PN"] = v.PN;
                            r["PN_SERIALIZATION"] = v.PN_SERIALIZATION;
                            r["CUST_PN"] = v.CUST_PN;
                            r["PN_7XX"] = v.PN_7XX;
                            r["SN_RULE"] = v.SN_RULE;
                            r["QTY"] = v.QTY;
                            r["TYPE"] = v.TYPE;
                            r["REV"] = v.REV;
                            r["CLEI_CODE"] = v.CLEI_CODE;
                            r["CHAS_SN"] = v.CHAS_SN;
                            linkTable.Rows.Add(r);
                        }

                    }
                }

                string fileName = "WOAutoKPConfig_Massive" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                MESPubLab.Common.ExcelHelp.ExportExcelToLoacl(linkTable, fileName, true);

            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public List<JuniperAutoKpConfig_Extend> GetAllJuniperAutoKpConfig(string type, OleExec DB)
        {
            List<JuniperAutoKpConfig_Extend> kpExtList = new List<JuniperAutoKpConfig_Extend>();
            List<JuniperAutoKpConfig> kpList = new List<JuniperAutoKpConfig>();

            //var list = DB.ORM.Queryable<R_JSON>().Where(t => t.TYPE == type && t.BLOBDATA != null && (t.NAME == "006A00034649" || t.NAME == "006A00000001")).ToList();
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.TYPE == type && t.BLOBDATA != null).ToList();

            if (list.Count == 0)
            {
                return kpExtList;
            }

            foreach (R_JSON item in list)
            {
                JuniperAutoKpConfig_Extend kpExtModel = new JuniperAutoKpConfig_Extend();

                if (string.IsNullOrEmpty(item.NAME))
                    break;

                kpExtModel.WorkOrderNo = item.NAME.Trim();

                if (item.BLOBDATA == null)
                    break;

                string str = System.Text.Encoding.Unicode.GetString(item.BLOBDATA);
                if (string.IsNullOrEmpty(str))
                    break;

                kpExtModel.AutoKpConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JuniperAutoKpConfig>>(str);

                kpExtList.Add(kpExtModel);
            }
            //var kplist = listR;
            return kpExtList;
        }

    }


}