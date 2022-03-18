using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Data;

namespace MESInterface.JUNIPER
{
    public class JuniperGetAllpartMPNQty : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, apdbstr, bustr, seriesname, Series, filepath, filebackpath, remotepath;

        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                apdbstr = ConfigGet("APDB");
                bustr = ConfigGet("BU");
                seriesname = ConfigGet("SERIESNAME");
                filepath = ConfigGet("FILEPATH");
                filebackpath = ConfigGet("FILEPATHBACKPATH");
                remotepath = ConfigGet("REMOTEPATH");
                if (seriesname == "")
                {
                    throw new Exception("請設置要傳送的系列！");
                }
                string[] list = seriesname.Split(',');
                for (var i = 0; i < list.Length; i++)
                {
                    Series += $@",'{list[i]}'";
                }
                Series = Series.Substring(1);
            }
            catch (Exception)
            {
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");

                //XmlDocument doc = new XmlDocument();
                //XmlReaderSettings settings = new XmlReaderSettings();
                //settings.IgnoreComments = true;
                //XmlReader reader = XmlReader.Create(filepath, settings);
                //doc.Load(reader);
                //reader.Close();
                //XmlNode xn = doc.SelectSingleNode("ns0:MT_OnHand");
                //XmlNodeList xnl = doc.ChildNodes[1].ChildNodes;

                DataSet ds = new DataSet();
                ds.ReadXml(filepath);
                var dt = ds.Tables[0];

                dt.Columns.Add(new DataColumn() { ColumnName = "ITEM", DataType = typeof(int) });
                dt.Columns["ITEM"].SetOrdinal(0);

                dt.Columns.Add(new DataColumn() { ColumnName = "AP_NEW_ORDER_QUANTITY", DataType = typeof(string) });
                dt.Columns["AP_NEW_ORDER_QUANTITY"].SetOrdinal(5);

                dt.Columns.Add(new DataColumn() { ColumnName = "MANUFACTURER_PART_NUMBER", DataType = typeof(string) });
                dt.Columns["MANUFACTURER_PART_NUMBER"].SetOrdinal(15);

                dt.Columns.Add(new DataColumn() { ColumnName = "MANUFACTURER_NAME", DataType = typeof(string) });
                dt.Columns["MANUFACTURER_NAME"].SetOrdinal(16);

                DataTable dts = new DataTable();
                dts.TableName = "Recodes";
                dts.Columns.Add("ITEM", typeof(int));
                dts.Columns.Add("ITEM_NAME", typeof(string));
                dts.Columns.Add("ORGANIZATION_CODE", typeof(string));
                dts.Columns.Add("SR_INSTANCE_CODE", typeof(string));
                dts.Columns.Add("NEW_ORDER_QUANTITY", typeof(string));
                dts.Columns.Add("AP_NEW_ORDER_QUANTITY", typeof(string));
                dts.Columns.Add("SUBINVENTORY_CODE", typeof(string));
                dts.Columns.Add("LOT_NUMBER", typeof(string));
                dts.Columns.Add("EXPIRATION_DATE", typeof(string));
                dts.Columns.Add("DELETED_FLAG", typeof(string));
                dts.Columns.Add("CM_PART_NUMBER", typeof(string));
                dts.Columns.Add("ITEM_TYPE", typeof(string));
                dts.Columns.Add("OWNERSHIP", typeof(string));
                dts.Columns.Add("NETTABLE_FLAG", typeof(string));
                dts.Columns.Add("GROUP_CODE", typeof(string));
                dts.Columns.Add("MANUFACTURER_PART_NUMBER", typeof(string));
                dts.Columns.Add("MANUFACTURER_NAME", typeof(string));
                dts.Columns.Add("FREE_ATTR1", typeof(string));
                dts.Columns.Add("FREE_ATTR2", typeof(string));
                dts.Columns.Add("FREE_ATTR3", typeof(string));
                dts.Columns.Add("FREE_ATTR4", typeof(string));
                dts.Columns.Add("FREE_ATTR5", typeof(string));
                dts.Columns.Add("FREE_ATTR6", typeof(string));
                dts.Columns.Add("FREE_ATTR7", typeof(string));
                dts.Columns.Add("FREE_ATTR8", typeof(string));
                dts.Columns.Add("FREE_ATTR9", typeof(string));
                dts.Columns.Add("FREE_ATTR10", typeof(string));
                dts.Columns.Add("FREE_ATTR11", typeof(string));
                dts.Columns.Add("FREE_ATTR12", typeof(string));
                dts.Columns.Add("FREE_ATTR13", typeof(string));
                dts.Columns.Add("FREE_ATTR14", typeof(string));
                dts.Columns.Add("FREE_ATTR15", typeof(string));
                dts.Columns.Add("FREE_ATTR16", typeof(string));
                dts.Columns.Add("FREE_ATTR17", typeof(string));
                dts.Columns.Add("FREE_ATTR18", typeof(string));
                dts.Columns.Add("FREE_ATTR19", typeof(string));
                dts.Columns.Add("FREE_ATTR20", typeof(string));

                SqlSugarClient sfcdb = MESDBHelper.OleExec.GetSqlSugarClient(this.mesdbstr, false);
                SqlSugarClient apdb = MESDBHelper.OleExec.GetSqlSugarClient(this.apdbstr, false);
                sfcdb.Ado.Open();
                apdb.Ado.Open();
                int i = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["ITEM_NAME"].ToString() == "220-014464")
                    {
                        var DFDF = "";
                    }
                    DataRow rp = dts.NewRow();
                    rp = r;
                    int k = 0;
                    var sql = $@"select a.cust_kp_no,
                                        a.mfr_kp_no,
                                        a.mfr_code,
                                        b.mfr_name,
                                        nvl(sum(ext_qty), 0) as qty
                                    from mes4.r_tr_sn a
                                    left join mes1.c_mfr_config b
                                    on a.mfr_code = b.mfr_code
                                    where a.cust_kp_no = '{r["ITEM_NAME"].ToString().Trim()}'
                                    and a.location_flag in ('0', '1', '2')
                                    and a.work_flag in ('0', '2', '4')
                                    group by a.cust_kp_no, a.mfr_kp_no, a.mfr_code, b.mfr_name
                                    order by a.cust_kp_no, a.mfr_kp_no, a.mfr_code, b.mfr_name";
                    var dtap = apdb.Ado.GetDataTable(sql);
                    if (dtap.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtap.Rows)
                        {
                            if (k == 0)
                            {
                                rp["ITEM"] = i;
                                rp["AP_NEW_ORDER_QUANTITY"] = dr["qty"];
                                rp["MANUFACTURER_PART_NUMBER"] = dr["mfr_kp_no"];
                                rp["MANUFACTURER_NAME"] = dr["mfr_name"];
                                //dts.Rows.Add(rp);
                                dts.ImportRow(rp);
                            }
                            else
                            {
                                rp["ITEM"] = i;
                                rp["AP_NEW_ORDER_QUANTITY"] = dr["qty"];
                                rp["MANUFACTURER_PART_NUMBER"] = dr["mfr_kp_no"];
                                rp["MANUFACTURER_NAME"] = dr["mfr_name"];                                

                                rp["ITEM_NAME"] = "";
                                rp["ORGANIZATION_CODE"] = "";
                                rp["SR_INSTANCE_CODE"] = "";
                                rp["NEW_ORDER_QUANTITY"] = "";
                                rp["SUBINVENTORY_CODE"] = "";
                                rp["LOT_NUMBER"] = "";
                                rp["EXPIRATION_DATE"] = "";
                                rp["DELETED_FLAG"] = "";
                                rp["CM_PART_NUMBER"] = "";
                                rp["ITEM_TYPE"] = "";
                                rp["OWNERSHIP"] = "";
                                rp["NETTABLE_FLAG"] = "";
                                rp["GROUP_CODE"] = "";
                                rp["FREE_ATTR1"] = "";
                                rp["FREE_ATTR2"] = "";
                                rp["FREE_ATTR3"] = "";
                                rp["FREE_ATTR4"] = "";
                                rp["FREE_ATTR5"] = "";
                                rp["FREE_ATTR6"] = "";
                                rp["FREE_ATTR7"] = "";
                                rp["FREE_ATTR8"] = "";
                                rp["FREE_ATTR9"] = "";
                                rp["FREE_ATTR10"] = "";
                                rp["FREE_ATTR11"] = "";
                                rp["FREE_ATTR12"] = "";
                                rp["FREE_ATTR13"] = "";
                                rp["FREE_ATTR14"] = "";
                                rp["FREE_ATTR15"] = "";
                                rp["FREE_ATTR16"] = "";
                                rp["FREE_ATTR17"] = "";
                                rp["FREE_ATTR18"] = "";
                                rp["FREE_ATTR19"] = "";
                                rp["FREE_ATTR20"] = "";
                                dts.ImportRow(rp);
                            }
                            k++;
                            i++;
                        }
                    }
                    else
                    {
                        rp["ITEM"] = i;
                        rp["AP_NEW_ORDER_QUANTITY"] = 0;
                        rp["MANUFACTURER_PART_NUMBER"] = "";
                        rp["MANUFACTURER_NAME"] = "";
                        //dts.Rows.Add(rp);
                        dts.ImportRow(rp);
                        i++;
                    }
                }

                sfcdb.Close();
                apdb.Close();

                //var Records = Load(typeof(RecordsXmls), filepath);
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        public bool Save(Type type, string fileName, object sender)
        {

            TextWriter writer = new StreamWriter(fileName);
            XmlSerializer serializer = new XmlSerializer(type);
            serializer.Serialize(writer, sender);
            writer.Close();
            return true;
        }

        public object Load(Type type, string xml)
        {
            object result = null;
            if (File.Exists(xml))
            {
                using (StreamReader reader = new StreamReader(xml))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    result = xs.Deserialize(reader);
                }
            }
            return result;
        }

        public class RecordsXmls
        {
            [XmlElement(ElementName = "RecordsXml")]
            public RecordsXml[] Items { set; get; }
        }

        //SAP COPY來的，不懂是什麼意思
        public class RecordsXml
        {
            [XmlElement(ElementName = "ITEM_NAME")]
            public string ITEM_NAME { set; get; }

            [XmlElement(ElementName = "ORGANIZATION_CODE")]
            public string ORGANIZATION_CODE { set; get; }

            [XmlElement(ElementName = "SR_INSTANCE_CODE")]
            public string SR_INSTANCE_CODE { set; get; }

            [XmlElement(ElementName = "NEW_ORDER_QUANTITY")]
            public string NEW_ORDER_QUANTITY { set; get; }

            [XmlElement(ElementName = "SUBINVENTORY_CODE")]
            public string SUBINVENTORY_CODE { set; get; }

            [XmlElement(ElementName = "LOT_NUMBER")]
            public string LOT_NUMBER { set; get; }

            [XmlElement(ElementName = "EXPIRATION_DATE")]
            public string EXPIRATION_DATE { set; get; }

            [XmlElement(ElementName = "DELETED_FLAG")]
            public string DELETED_FLAG { set; get; }

            [XmlElement(ElementName = "CM_PART_NUMBER")]
            public string CM_PART_NUMBER { set; get; }

            [XmlElement(ElementName = "ITEM_TYPE")]
            public string ITEM_TYPE { set; get; }

            [XmlElement(ElementName = "OWNERSHIP")]
            public string OWNERSHIP { set; get; }

            [XmlElement(ElementName = "NETTABLE_FLAG")]
            public string NETTABLE_FLAG { set; get; }

            [XmlElement(ElementName = "GROUP_CODE")]
            public string GROUP_CODE { set; get; }

            [XmlElement(ElementName = "FREE_ATTR1")]
            public string FREE_ATTR1 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR2")]
            public string FREE_ATTR2 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR3")]
            public string FREE_ATTR3 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR4")]
            public string FREE_ATTR4 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR5")]
            public string FREE_ATTR5 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR6")]
            public string FREE_ATTR6 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR7")]
            public string FREE_ATTR7 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR8")]
            public string FREE_ATTR8 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR9")]
            public string FREE_ATTR9 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR10")]
            public string FREE_ATTR10 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR11")]
            public string FREE_ATTR11 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR12")]
            public string FREE_ATTR12 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR13")]
            public string FREE_ATTR13 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR14")]
            public string FREE_ATTR14 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR15")]
            public string FREE_ATTR15 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR16")]
            public string FREE_ATTR16 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR17")]
            public string FREE_ATTR17 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR18")]
            public string FREE_ATTR18 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR19")]
            public string FREE_ATTR19 { set; get; }

            [XmlElement(ElementName = "FREE_ATTR20")]
            public string FREE_ATTR20 { set; get; }
        }

    }
}
