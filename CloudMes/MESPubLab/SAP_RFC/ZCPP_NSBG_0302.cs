using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    /// RFC: ZCPP_NSBG_0302

//PLANT ：廠別
//RELFG ：空 （X - 代表 Create 工單時自動 Release 工單）
//CREATEWO ：空 （ X- 代表 Upload 工單）

//ZWO_HEADER ： Header table（放需要開工單的 PID 和數量）
//ZWO_ITEM ：Item table （放其他 PID 和數量 ）
//ZWO_HIDBOM ：放 Hidden BOM PID，數量不用放

    /// </summary>
    public class ZCPP_NSBG_0302 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0302(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0302");
        }

        public void SetValue(string PLANT, string RELFG, string CREATEWO, DataTable ZWO_HEADER,DataTable ZWO_ITEM,DataTable ZWO_HIDBOM)
        {
            ClearValues();
            try
            {
                _Tables["ZWO_HEADER"].Clear();
                _Tables["ZWO_ITEM"].Clear();
                _Tables["ZWO_HIDBOM"].Clear();
                _Tables["RETURN"].Clear();
                _Tables["PODETAIL"].Clear();
                _Tables["MINI_LIST"].Clear();
                _Tables["BOM_LIST"].Clear();
            }
            catch { }
            DataRow dr;
            try
            {

                this.SetValue("PLANT", PLANT);
                this.SetValue("RELFG", RELFG);
                this.SetValue("CREATEWO", CREATEWO);
                //AUFNR ：工單號碼
                //WERKS ：廠別
                //AUART ：工單類型
                //MATNR ： PID
                //GAMNG ： Qty
                //ABLAD ： 客戶 PO 號碼
                //EXPLD ：是否展 BOM （ Y - 需要展 BOM ，N - 不展 BOM）
                foreach (DataRow row in ZWO_HEADER.Rows)
                {
                    dr = _Tables["ZWO_HEADER"].NewRow();
                    dr["AUFNR"] = row["WO"];
                    dr["WERKS"] = row["PLANT"];
                    dr["AUART"] = row["WOTYPE"];
                    dr["MATNR"] = row["PID"];
                    dr["GAMNG"] = row["QTY"];
                    dr["ABLAD"] = row["PO"];
                    dr["EXPLD"] = row["EXBOM"];
                    _Tables["ZWO_HEADER"].Rows.Add(dr);
                }

                //AUFNR ：工單號碼
                //IDNRK ：PID
                //MENGE ： Qty
                foreach (DataRow row in ZWO_ITEM.Rows)
                {
                    dr = _Tables["ZWO_ITEM"].NewRow();
                    dr["AUFNR"] = row["WO"];
                    dr["IDNRK"] = row["PN"];
                    dr["MENGE"] = row["QTY"];
                    _Tables["ZWO_ITEM"].Rows.Add(dr);
                }
                
                //AUFNR ：工單號碼
                //IDNRK ：PID
                //MENGE ： Qty
                foreach (DataRow row in ZWO_HIDBOM.Rows)
                {
                    dr = _Tables["ZWO_HIDBOM"].NewRow();
                    dr["AUFNR"] = row["WO"];
                    dr["IDNRK"] = row["HB"];
                    dr["MENGE"] = row["QTY"];
                    _Tables["ZWO_HIDBOM"].Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GET_NEW_ZWO_HEADER()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("WO");
            dt.Columns.Add("PLANT");
            dt.Columns.Add("WOTYPE");
            dt.Columns.Add("PID");
            dt.Columns.Add("QTY");
            dt.Columns.Add("PO");
            dt.Columns.Add("EXBOM");
            return dt;
        }

        public DataTable GET_NEW_ZWO_ITEM()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("WO");
            dt.Columns.Add("PN");
            dt.Columns.Add("QTY");
            return dt;
        }

        public DataTable GET_NEW_ZWO_HIDBOM()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("WO");
            dt.Columns.Add("HB");
            dt.Columns.Add("QTY");
            return dt;
        }
    }
}
