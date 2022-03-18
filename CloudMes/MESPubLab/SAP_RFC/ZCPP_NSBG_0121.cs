using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    /// IN_TAB   TABLE
    /// POSDT,TSORT,TSORU,TSORL,WERKS,EXTWG,TSORNUM,MATNR,HWDMAT,MAKTX,REQTY,REQDT,MEINS,HWPONU,FOXDES,HWDDES,REMARK
    /// 
    /// OUT_TAB  TABLE
    /// POSDT,TSORT,TSORU,TSORL,WERKS,EXTWG,TSORNUM,MATNR,HWDMAT,MAKTX,REQTY,REQDT,MEINS,HWPONU,FOXDES,HWDDES,REMARK,DELFLAG,FLAG,MESS
    /// </summary>
    public class ZCPP_NSBG_0121 : SAP_RFC_BASE
    {        
        public ZCPP_NSBG_0121(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0121");
        }
        public void SetValue(DataTable in_table)
        {
            ClearValues();
            try
            {
                _Tables["OUT_TAB"].Clear();
                _Tables["IN_TAB"].Clear();
            }
            catch{}
            DataRow dr ;
            try
            {
                foreach (DataRow row in in_table.Rows)
                {
                    dr = _Tables["IN_TAB"].NewRow();
                    dr["POSDT"] = row["POSDT"];
                    dr["TSORT"] = row["TSORT"];
                    dr["TSORU"] = row["TSORU"];
                    dr["TSORL"] = row["TSORL"];
                    dr["WERKS"] = row["WERKS"];
                    dr["EXTWG"] = row["EXTWG"];
                    dr["TSORNUM"] = row["TSORNUM"];
                    dr["MATNR"] = row["MATNR"];
                    dr["HWDMAT"] = row["HWDMAT"];
                    dr["MAKTX"] = row["MAKTX"];
                    dr["REQTY"] = row["REQTY"];
                    dr["REQDT"] = row["REQDT"];
                    dr["MEINS"] = row["MEINS"];
                    dr["HWPONU"] = row["HWPONU"];
                    dr["FOXDES"] = row["FOXDES"];
                    dr["HWDDES"] = row["HWDDES"];
                    dr["REMARK"] = row["REMARK"];
                    _Tables["IN_TAB"].Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
