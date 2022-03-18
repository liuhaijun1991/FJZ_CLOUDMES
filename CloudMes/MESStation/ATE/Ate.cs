using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.ATE
{
    public class Ate : MesAPIBase
    {
        protected APIInfo FUploadTestRecord = new APIInfo()
        {
            FunctionName = "UploadTestRecord",
            Description = "UploadTestRecord",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TEGROUP", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TESTATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MESSTATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DEVICE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STARTTIME", InputType = "DateTime", DefaultValue = "" },
                new APIInputInfo() {InputName = "ENDTIME", InputType = "DateTime", DefaultValue = "" },
                new APIInputInfo() {InputName = "TESTINFO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DETAILTABLE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TEST_EMP", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSNKP = new APIInfo()
        {
            FunctionName = "GetSNKP",
            Description = "GetSNKP",
            Parameters = new List<APIInputInfo>()

            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
               
            },
            Permissions = new List<MESPermission>() { }
        };

        public Ate()
        {
            this.Apis.Add(FUploadTestRecord.FunctionName, FUploadTestRecord);
            this.Apis.Add(FGetSNKP.FunctionName, FGetSNKP);
        }
        public void TEMP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
            }
            catch
            { }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        GetSNkpReturn _GetSNKP(string _SN, OleExec SFCDB , GetSNkpReturn ret,int Deep,int DeepMax)
        {
            
            SN sn = new SN();
            try
            {
                ret.SNBase = sn.LoadSN(_SN, SFCDB);
                //ret.SNBase = sn.baseSN;
                var kps = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == ret.SNBase.ID).ToList();
                for (int i = 0; i < kps.Count; i++)
                {
                    GetSNkpReturn temp = new GetSNkpReturn();
                    temp.KP = kps[i];
                    ret.SubKPS.Add(temp);
                    if (kps[i].VALUE == null || kps[i].SCANTYPE.IndexOf("PN")>=0)
                    {
                        continue;
                    }
                    if (Deep <= DeepMax)
                    {
                        try
                        {
                            _GetSNKP(kps[i].VALUE, SFCDB, temp, Deep + 1, DeepMax);
                        }
                        catch
                        { }
                    }
                }
            }
            catch(Exception)
            {
                //throw ee;
            }
            return ret;

        }

        public void GetSNKP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            
            try
            {
                var SN = Data["SN"].ToString();
                StationReturn.Data = _GetSNKP(SN, SFCDB, new GetSNkpReturn(), 1, 5);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch(Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);

            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void UploadTestRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var SN = Data["SN"].ToString();
                var TEGROUP = Data["TEGROUP"].ToString();
                var TESTATION = Data["TESTATION"].ToString();
                var MESSTATION = Data["MESSTATION"].ToString();
                var DEVICE = Data["DEVICE"].ToString();
                var STARTTIME = Data["STARTTIME"].ToString();
                var ENDTIME = Data["ENDTIME"].ToString();
                var TESTINFO = Data["TESTINFO"].ToString();
                var DETAILTABLE = Data["DETAILTABLE"].ToString();
                var TEST_EMP = Data["TEST_EMP"].ToString();

                R_TEST_RECORD R = new R_TEST_RECORD();
                R.ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD");
                LogicObject.SN sn = new LogicObject.SN();
                try
                {
                    sn.LoadSN(SN, SFCDB);
                    R.R_SN_ID = sn.baseSN.ID;
                }
                catch
                {

                }
                R.SN = SN;
                R.STARTTIME = DateTime.Parse(STARTTIME);
                R.ENDTIME = DateTime.Parse(ENDTIME);
                R.TEGROUP = TEGROUP;
                R.TESTATION = TESTATION;
                R.MESSTATION = MESSTATION;
                R.DEVICE = DEVICE;
                R.TESTINFO = TESTINFO;
                R.DETAILTABLE = DETAILTABLE;
                if (TEST_EMP != "")
                {
                    R.EDIT_EMP = TEST_EMP;
                }
                else
                {
                    R.EDIT_EMP = LoginUser.EMP_NO;
                }
                R.EDIT_TIME = DateTime.Now;

                SFCDB.ORM.Insertable<R_TEST_RECORD>(R).ExecuteCommand();

            }
            catch
            {

            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
    public class GetSNkpReturn
    {
        public R_SN SNBase;
        public R_SN_KP KP;
        public List<GetSNkpReturn> SubKPS = new List<GetSNkpReturn>();
    }
}
