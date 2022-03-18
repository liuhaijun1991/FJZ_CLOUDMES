using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCSD_NSBG_0008 : SAP_RFC_BASE
    {
        public ZCSD_NSBG_0008(string BU) : base(BU)
        {
            SetRFC_NAME("ZCSD_NSBG_0008");
        }
        public void SetValue(DataTable INA)
        {
            this.ClearValues();
            this._Tables["INA"].Clear();
            this._Tables["INA"] = INA.Copy();
            this._Tables["OUTB"].Clear();
        }

        public DataTable GetInitINA()
        {
            return this._Tables["INA"].Copy();
        }
    }
}
