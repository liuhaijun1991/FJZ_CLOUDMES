using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.Json;
using MESPubLab.MesBase;
using System;

namespace MESPubLab.SAP_RFC
{
    public class MesSapHelp
    {
        public static void SapLog(string PrimaryKey, SapParameterObj sapobj, SqlSugar.SqlSugarClient DB, MES_CONST_SAVE_TYPE saveType=MES_CONST_SAVE_TYPE.SaveLocal)
        {
            using (var _db = OleExec.GetSqlSugarClient(DB.CurrentConnectionConfig.ConnectionString))
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(sapobj, Newtonsoft.Json.Formatting.Indented,
                             new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                _db.Insertable(new R_SAP_LOG()
                {
                    ID = MesDbBase.GetNewID<R_SAP_LOG>(_db, ""),
                    LOGKEY = sapobj.LOGKEY,
                    CID = PrimaryKey,
                    OUTID = new Func<string>(() => {
                        if (saveType == MES_CONST_SAVE_TYPE.SaveMesDb)
                            return JsonSave.SaveToDB(sapobj, sapobj.LOGKEY, "SAP", "", DB, "");
                        else {
                            MesLog.Info($@"SapKey:{sapobj.LOGKEY} \r\n {json}");
                            return MesConst.MesNullStr;
                        }
                    })(), 
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();
            }
        }
    }
}
