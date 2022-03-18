using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.OM;
using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Base
{
    public class JuniperPoTracking
    {
        string po, poline, tranid;
        SqlSugarClient db;
        JuniperErrType juniperErrType;
        JuniperSubType juniperSubType;

        public void SetTranid(string _tranid)
        {
            tranid = _tranid;
        }

        public JuniperPoTracking(JuniperErrType _juniperErrType, JuniperSubType _juniperSubType, string _po,string _poline,string _tranid, SqlSugarClient _db)
        {
            db = _db;
            po = _po;
            tranid = _tranid;
            poline = _poline;
            juniperErrType = _juniperErrType;
            juniperSubType = _juniperSubType;
        }

        public void ReleasePoStatus<T>(string ordermainId, T fobj)
        {
            var tclassobj = typeof(T).GetMethod("GetFunctionObj");
            var currentStatus = (ENUM_O_PO_STATUS)Enum.Parse(typeof(ENUM_O_PO_STATUS), tclassobj.Invoke(fobj, null).ToString());
            db.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == ordermainId && t.VALIDFLAG == MesBool.Yes.ExtValue()).ExecuteCommand();
            db.Insertable(new O_PO_STATUS()
            {
                ID = MesDbBase.GetNewID<O_PO_STATUS>(db, Customer.JUNIPER.ExtValue()),
                STATUSID = JuniperBase.GetPoNextStatus(currentStatus).ExtValue(),
                VALIDFLAG = MesBool.Yes.ExtValue(),
                CREATETIME = DateTime.Now,
                EDITTIME = DateTime.Now,
                POID = ordermainId
            }).ExecuteCommand();
        }

        public void ReleasePoStatus<T>(string ordermainId, ENUM_O_PO_STATUS targetStatus, T fobj)
        {
            var tclassobj = typeof(T).GetMethod("GetFunctionObj");
            var currentStatus = (ENUM_O_PO_STATUS)Enum.Parse(typeof(ENUM_O_PO_STATUS), tclassobj.Invoke(fobj, null).ToString());
            db.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == ordermainId && t.STATUSID == currentStatus.ExtValue()).ExecuteCommand();
            db.Insertable(new O_PO_STATUS()
            {
                ID = MesDbBase.GetNewID<O_PO_STATUS>(db, Customer.JUNIPER.ExtValue()),
                STATUSID = targetStatus.ExtValue(),
                VALIDFLAG = MesBool.Yes.ExtValue(),
                CREATETIME = DateTime.Now,
                EDITTIME = DateTime.Now,
                POID = ordermainId
            }).ExecuteCommand();
        }

        /// <summary>
        /// order excption record,return condition
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="excption"></param>
        /// <param name="action"></param>
        /// <param name="release"></param>
        /// <returns></returns>
        public bool ExceptionProcess(bool condition,string excption,Action action=null,bool release = true)
        {
            if (condition)
            {
                JuniperBase.RecordJuniperExcption(db, excption, $@"{po}{poline}", tranid, juniperErrType, juniperSubType, false);
                action?.Invoke();
            }
            else if(release)
                JuniperBase.RecordJuniperExcption(db, excption, $@"{po}{poline}", tranid, juniperErrType, juniperSubType, true);
            return condition;
        }

        public void ReleaseFuncExcption()
        {
            JuniperBase.RecordJuniperExcption(db, string.Empty, $@"{po}{poline}", tranid, juniperErrType, juniperSubType, true);
        }

        public void ReleaseAllExcption()
        {
            JuniperBase.CloseAllExcption(db, $@"{po}{poline}");
        }
    }
}
