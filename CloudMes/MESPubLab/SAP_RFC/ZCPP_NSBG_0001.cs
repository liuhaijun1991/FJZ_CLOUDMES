

using MESDataObject.Module.Juniper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0001 : SAP_RFC_BASE
    {

        public ZCPP_NSBG_0001(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0001");
        }

        public void SetValue(string Plant, string BaseDay, string Ec05Type, string Identifier, string RohsStatus, string Org, string Bu, DataTable DTIn)
        {
            this.ClearValues();
            this.SetValue("BASEDAY", BaseDay);
            this.SetValue("EC05TYPE", Ec05Type);

            this._Tables["PLANT"].Clear();
            DataRow dr = this._Tables["PLANT"].NewRow();
            dr["WERKS"] = Plant;
            this._Tables["PLANT"].Rows.Add(dr);

            this._Tables["ZTABIN"].Clear();
            for (int i = 0; i < DTIn.Rows.Count; i++)
            {
                dr = this._Tables["ZTABIN"].NewRow();
                dr["IDENTIFIER"] = Identifier;
                dr["ROHS_STATUS"] = RohsStatus;
                dr["ORG"] = Org;
                dr["BU"] = Bu;
                dr["PRODUCT"] = DTIn.Rows[i]["SKUNO"].ToString();
                string seq = string.Empty;
                for (int j = 1; j < DTIn.Columns.Count; j++)
                {
                    if (j.ToString().Length == 1)
                        seq = "00" + j.ToString();
                    else if (j.ToString().Length == 2)
                        seq = "0" + j.ToString();
                    else
                        seq = j.ToString();

                    dr["DAY" + seq] = DTIn.Rows[i][j].ToString();
                }
                this._Tables["ZTABIN"].Rows.Add(dr);
            }
            this._Tables["ZRETURN"].Clear();
        }

        public void SetValue(string BaseDay, string Ec05Type, string Plant, DataTable ZTABIN)
        {
            this.ClearValues();
            this.SetValue("BASEDAY", BaseDay);
            if(Ec05Type!="")
                this.SetValue("EC05TYPE", Ec05Type);
            //this.SetValue("PLANT", Plant);

            this._Tables["PLANT"].Clear();
            DataRow dr = this._Tables["PLANT"].NewRow();
            dr["WERKS"] = Plant;
            this._Tables["PLANT"].Rows.Add(dr);

            this._Tables["ZTABIN"].Clear();
            this._Tables["ZTABIN"] = ZTABIN.Copy();
            this._Tables["ZRETURN"].Clear();
        }

        public DataTable GetInitTableZTABIN()
        {
            return this._Tables["ZTABIN"].Copy();
        }


    }
}

