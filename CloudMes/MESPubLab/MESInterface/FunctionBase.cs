using MESDataObject.Module.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;

namespace MESPubLab.MESInterface
{
    public class FunctionBase
    {
        public object TaskBase;
        public string bu,dbstr;
        public ServicesEnum servicesName;
        public RecordServiceLog recordServiceLog;
        public string currentRunID;
        public object functionObj;
        public FunctionBase(string _dbstr,string _bu)
        {
            dbstr = _dbstr;
            bu = _bu;            
            recordServiceLog = new RecordServiceLog(dbstr, bu);
        }

        public string ConfigGet(string key)
        {
            if (TaskBase == null)
            {
                return "";
            }
            
            Type t = TaskBase.GetType();
            if (t.BaseType.FullName != "MESInterface.taskBase")
            {
                return "";
            }
            var m = t.GetMethod("ConfigGet");
            var ret = m.Invoke(TaskBase, new object[] { key });
            return ret.ToString();
        }

        public void Run()
        {
            try
            {
                if (!ServiceConfig())
                    return;
                ChangeFunctionStatus(ServicesFunctionStatus.Start);
                try
                {
                    FunctionRun();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    ChangeFunctionStatus(ServicesFunctionStatus.End);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual void FunctionRun()
        {
        }

        bool ServiceConfig()
        {
            return recordServiceLog.RecordServiceConfig((ServicesFunctionEnum)Enum.Parse(typeof(ServicesFunctionEnum), this.GetType().Name), servicesName);
        }

        void ChangeFunctionStatus(ServicesFunctionStatus servicesFunctionStatus)
        {
            if (servicesFunctionStatus == ServicesFunctionStatus.Start && recordServiceLog.FunctionIsStart((ServicesFunctionEnum)Enum.Parse(typeof(ServicesFunctionEnum), this.GetType().Name)))
                throw new Exception("Function is NOT running!");
            currentRunID = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            recordServiceLog.RecordFunctionRunStatus((ServicesFunctionEnum)Enum.Parse(typeof(ServicesFunctionEnum), this.GetType().Name), servicesFunctionStatus);
        }

        public void SetFunctionObj(object _functionObj)
        {
            functionObj = _functionObj;
        }

        public object GetFunctionObj()
        {
            return functionObj;
        }

        protected double RoundUp(double num, int decimals)
        {
            double precision = Math.Pow(10.0, decimals);
            return RoundDown(num, decimals) + 1 / precision;
        }
        protected double RoundDown(double num, int decimals)
        {
            double precision = Math.Pow(10.0, decimals);
            return ((int)(num * precision)) / precision;
        }
    }

    public enum ServicesEnum
    {
        [EnumValue("JnpServices")] JnpServices,
    }

    public enum ServicesFunctionEnum
    {
        [EnumValue("JuniperCreateWo")] JuniperCreateWo,
        [EnumValue("JuniperPreWoGanerate")] JuniperPreWoGanerate,
        [EnumValue("JuniperGroupIdReceive")] JuniperGroupIdReceive,     
        [EnumValue("Syn137")] Syn137,
        [EnumValue("Syn140")] Syn140,
        [EnumValue("Syn285")] Syn285,        
        [EnumValue("Syn282")] Syn282,
        [EnumValue("Syn244")] Syn244,
        [EnumValue("JuniperPreUpoadBom")] JuniperPreUpoadBom,
        [EnumValue("JuniperSecUpoadBom")] JuniperSecUpoadBom,
        [EnumValue("AddNonBom")] AddNonBom,
        [EnumValue("SynAck")] SynAck,
        [EnumValue("SynCrem")] SynCrem,
        [EnumValue("JuniperSapJob")] JuniperSapJob,
        JuniperMesPoChange,
        JuniperMesPoCancel,
        JuniperAutoConfigSku,
        JuniperRecivePo,
        JuniperMesPoGanerate,
    }

    public enum ServicesFunctionStatus
    {
        Start, End
    }
}
