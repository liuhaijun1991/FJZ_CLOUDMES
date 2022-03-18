using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using MESDataObject.Module;

namespace MESInterface
{
   public class SAPMappingBase
    {
        List<C_SAP_STATION_MAP> _Mapping;
        public SAPMappingBase(string skuno, OleExec sfcdb)
        {
            T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new T_C_SAP_STATION_MAP(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            _Mapping = TC_SAP_STATION_MAP.GetSAPStationMapBySkuOrderBySAPCodeASC(skuno, sfcdb);
            if (_Mapping == null || _Mapping.Count <= 0)
            {
                throw new Exception(skuno+ " 沒有配置SAP Mapping");
            }
        }
        public string GetSFCStation(string SAPCode)
        {
            string sfcstation = "";
            foreach (C_SAP_STATION_MAP ma in _Mapping)
            {
                if (ma.SAP_STATION_CODE == SAPCode)
                {
                    sfcstation = ma.STATION_NAME;
                    break;
                }
            }
            if (sfcstation.Length <= 0)
            {
                throw new Exception(SAPCode+"沒有對應的SFCStation");
            }
            return sfcstation;
        }
        public string GetLastSAPStationCode()
        {
            return _Mapping[_Mapping.Count - 1].SAP_STATION_CODE;
        }
    }
}
