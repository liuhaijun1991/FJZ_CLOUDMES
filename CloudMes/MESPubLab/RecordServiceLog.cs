using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MESInterface;

namespace MESPubLab
{
    public class RecordServiceLog
    {
        public string dbStr, Bu;

        public RecordServiceLog(string _dbStr, string _Bu)
        {
            dbStr = _dbStr;
            Bu = _Bu;
        }

        public void Record(R_SERVICE_LOG log)
        {
            using (var db = OleExec.GetSqlSugarClient(dbStr, true))
            {
                log.ID = MesDbBase.GetNewID(dbStr, Bu, "R_SERVICE_LOG");
                log.CURRENTEDITTIME = DateTime.Now;
                log.SOURCECODE = GetCurrentMethodFullName();
                log.MAILFLAG = "N";
                db.Insertable(log).ExecuteCommand();
            }
        }

        /// <summary>
        /// RecordFunctionRunStatus
        /// </summary>
        /// <param name="servicesFunction"></param>
        /// <param name="servicesFunctionStatus"></param>
        public bool RecordServiceConfig(ServicesFunctionEnum servicesFunction, ServicesEnum services)
        {
            using (var db = OleExec.GetSqlSugarClient(dbStr, false))
            {
                string functiontype = servicesFunction.ToString().ToUpper();
                var functionService = db.Queryable<C_SERVICE_CONTROL>().Where(t => t.SERVERNAME == functiontype).ToList().FirstOrDefault();
                if (functionService == null)
                {
                    functionService = new C_SERVICE_CONTROL()
                    {
                        ID = MesDbBase.GetNewID<R_SERVICE_LOG>(db, Bu),
                        SERVERNAME = services.ToString().ToUpper(),
                        SERVERFUNCTION = servicesFunction.ToString().ToUpper(),
                        RUNSTATUS = ServicesFunctionStatus.Start.ToString().ToUpper(),
                        TIMESPAN = 300000.ToString(),
                        CLASSNAME = GetCurrentMethodFullName(),
                        CURRENTIP = PublicHelp.GetInternalIP().ToString(),
                        RUNTIME = DateTime.Now
                    };
                    db.Insertable(functionService).ExecuteCommand();
                    return true;
                }
                else if (functionService.CURRENTIP != PublicHelp.GetInternalIP().ToString())
                    throw new Exception($@"Service: {functiontype} is Running on {functionService.CURRENTIP}!");
                else if (functionService.RUNSTATUS == ServicesFunctionStatus.Start.ToString().ToUpper())
                {
                    functionService.RUNTIME = DateTime.Now;
                    db.Updateable(functionService).ExecuteCommand();
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// RecordFunctionRunStatus
        /// </summary>
        /// <param name="servicesFunction"></param>
        /// <param name="servicesFunctionStatus"></param>
        public void RecordFunctionRunStatus(ServicesFunctionEnum servicesFunction, ServicesFunctionStatus servicesFunctionStatus)
        {
            using (var db = OleExec.GetSqlSugarClient(dbStr, false))
            {
                string functiontype = servicesFunction.ToString().ToUpper(),
                       functionStatus = servicesFunctionStatus.ToString().ToUpper();
                var functionService = db.Queryable<R_SERVICE_LOG>().Where(t => t.FUNCTIONTYPE == functiontype & t.DATA1 == "RUNSTATUS" & t.DATA2 == functionStatus).ToList();
                if (functionService.Count > 0)
                    db.Updateable<R_SERVICE_LOG>().SetColumns(t => new R_SERVICE_LOG() { LASTEDITTIME = functionService.FirstOrDefault().CURRENTEDITTIME, CURRENTEDITTIME = DateTime.Now })
                        .Where(t => t.FUNCTIONTYPE == functiontype & t.DATA1 == "RUNSTATUS" & t.DATA2 == functionStatus).ExecuteCommand();
                else
                    db.Insertable(new R_SERVICE_LOG() {
                        ID = MesDbBase.GetNewID<R_SERVICE_LOG>(db, Bu),
                        FUNCTIONTYPE = servicesFunction.ToString().ToUpper(),
                        SOURCECODE = GetCurrentMethodFullName(),
                        CURRENTEDITTIME =DateTime.Now,
                        DATA1 = "RUNSTATUS",
                        DATA2 = servicesFunctionStatus.ToString().ToUpper(),
                        MAILFLAG = "Y"
                    }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 函数是否在运行状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool FunctionIsStart(ServicesFunctionEnum type)
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(dbStr, false))
            {
                string startType = ServicesFunctionStatus.Start.ToString().ToUpper(),
                    endType = ServicesFunctionStatus.End.ToString().ToUpper(),
                    functiontype = type.ToString().ToUpper();
                var startLog = db.Queryable<R_SERVICE_LOG>().Where(t => t.FUNCTIONTYPE == functiontype & t.DATA1 == "RUNSTATUS" & t.DATA2 == startType).ToList();
                var endLog = db.Queryable<R_SERVICE_LOG>().Where(t => t.FUNCTIONTYPE == functiontype & t.DATA1 == "RUNSTATUS" & t.DATA2 == endType).ToList();
                if (startLog.Count == 0)
                    return false;
                else if (endLog.Count == 0)
                    return true;
                else if (startLog.FirstOrDefault().CURRENTEDITTIME > endLog.FirstOrDefault().CURRENTEDITTIME)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 取當前執行的FunctionFullName--add by Eden
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentMethodFullName()
        {
            try
            {
                StackTrace st = new StackTrace();
                int maxFrames = st.GetFrames().Length;
                StackFrame sf;
                string methodName, className;
                Type classType;
                sf = st.GetFrame(2);
                classType = sf.GetMethod().DeclaringType;
                className = classType.ToString();
                methodName = sf.GetMethod().Name;
                return className + "." + methodName;
            }
            catch (Exception)
            {
                return "获取方法名失败";
            }
        }
    }

}
