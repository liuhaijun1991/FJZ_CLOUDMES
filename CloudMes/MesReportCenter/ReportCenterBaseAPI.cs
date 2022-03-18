using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;


namespace MesReportCenter
{
    class ReportCenterBaseAPI:MesAPIBase
    {
        protected APIInfo _AddData = new APIInfo()
        {
            FunctionName = "AddData",
            Description = "AddData",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="ID", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="CONFIGTYPE", InputType="string", DefaultValue="REPORT"},
                new APIInputInfo(){ InputName="KEY", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="DATATYPE", InputType="string", DefaultValue="json"},
                new APIInputInfo(){ InputName="PARENTKEY", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="DATA", InputType="string", DefaultValue=""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetReportList = new APIInfo()
        {
            FunctionName = "GetReportList",
            Description = "GetReportList",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetDataByID = new APIInfo()
        {
            FunctionName = "GetDataByID",
            Description = "GetDataByID",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="ID", InputType="string", DefaultValue=""},
                
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };


        protected APIInfo _GetDataLinkList = new APIInfo()
        {
            FunctionName = "GetDataLinkList",
            Description = "GetDataLinkList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="CONFIGTYPE", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="KEY", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="DATATYPE", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="PARENTKEY", InputType="string", DefaultValue=""},
                new APIInputInfo(){ InputName="DATA", InputType="string", DefaultValue=""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetDataSourceList = new APIInfo()
        {
            FunctionName = "GetDataSourceList",
            Description = "GetDataSourceList",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetDataLink = new APIInfo()
        {
            FunctionName = "GetDataLink",
            Description = "GetDataLink",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Key", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetDataSource = new APIInfo()
        {
            FunctionName = "GetDataSource",
            Description = "GetDataSource",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Key", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _RunDataSource = new APIInfo()
        {
            FunctionName = "RunDataSource",
            Description = "RunDataSource",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DataLinkKey", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "DataSourceKey", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ParaValues", InputType = "STRING", DefaultValue = ""},
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public ReportCenterBaseAPI()
        {
            Apis.Add(_AddData.FunctionName, _AddData);
            Apis.Add(_GetReportList.FunctionName, _GetReportList);
            Apis.Add(_GetDataByID.FunctionName, _GetDataByID);
            Apis.Add(_GetDataLinkList.FunctionName, _GetDataLinkList);
            Apis.Add(_GetDataSourceList.FunctionName, _GetDataSourceList);
            Apis.Add(_GetDataLink.FunctionName, _GetDataLink);
            Apis.Add(_GetDataSource.FunctionName, _GetDataSource);
            Apis.Add(_RunDataSource.FunctionName, _RunDataSource);
        }
        public void AddData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ID = "";
                try
                {
                    ID = Data["ID"].ToString().Trim();
                }
                catch
                { }
                string CONFIGTYPE = Data["CONFIGTYPE"].ToString();
                string KEY = Data["KEY"].ToString();
                string DATATYPE = Data["DATATYPE"].ToString();
                string PARENTKEY = Data["PARENTKEY"].ToString();
                string DATA = Data["DATA"].ToString();
                int c = sfcdb.ORM.Queryable<R_REPORTCENTER>().Where(t => t.KEY == KEY && t.CONFIGTYPE == CONFIGTYPE).Count();
                if (ID == "")
                {
                    if (c > 0)
                    {
                        throw new Exception($@"KEY:{KEY},TYPE:{CONFIGTYPE} 已经存在！");
                    }
                }
                else
                {
                    if (c == 0)
                    {
                        throw new Exception($@"KEY:{KEY},TYPE:{CONFIGTYPE} ID:{ID} 不存在！");
                    }
                    sfcdb.ORM.Deleteable<R_REPORTCENTER>().Where(t => t.ID == ID).ExecuteCommand();
                }

                
                R_REPORTCENTER r = new R_REPORTCENTER()
                {
                    CONFIGTYPE = CONFIGTYPE,
                    DATATYPE = DATATYPE,
                    DATA = DATA,
                    KEY = KEY,
                    PARENTKEY = PARENTKEY,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                };
                if (ID == null || ID == "")
                {
                    ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_REPORTCENTER");
                }
                r.ID = ID;
                T_R_REPORTCENTER TRR = new T_R_REPORTCENTER(sfcdb, DBTYPE);
                Row_R_REPORTCENTER RRR =(Row_R_REPORTCENTER) TRR.NewRow();
                RRR.ID = r.ID;
                RRR.CONFIGTYPE = r.CONFIGTYPE;
                RRR.DATATYPE = r.DATATYPE;
                RRR.DATA = ":BLOB_DATA";
                RRR.KEY = r.KEY;
                RRR.PARENTKEY = r.PARENTKEY;
                RRR.EDIT_EMP = r.EDIT_EMP;
                RRR.EDIT_TIME = r.EDIT_TIME;

                string strsql = RRR.GetInsertString(DBTYPE);
                strsql = strsql.Replace("':BLOB_DATA'", ":BLOB_DATA");
                var datas = System.Text.Encoding.UTF8.GetBytes(DATA);
                sfcdb.ExecSqlNoReturn(strsql, new System.Data.OleDb.OleDbParameter[]
                { new System.Data.OleDb.OleDbParameter(":BLOB_DATA",datas)
                });


                //sfcdb.ORM.Insertable<R_REPORTCENTER>(r).ExecuteCommand();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void GetReportList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strSql = "select r.id,r.configtype,r.key,r.datatype,r.parentkey from R_REPORTCENTER r where r.configtype='REPORT'";
                var res = sfcdb.RunSelect(strSql);
                List<R_REPORTCENTER> ret = new List<R_REPORTCENTER>();
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    R_REPORTCENTER r = new R_REPORTCENTER();
                    r.ID = res.Tables[0].Rows[i]["ID"].ToString();
                    r.CONFIGTYPE = res.Tables[0].Rows[i]["CONFIGTYPE"].ToString();
                    r.KEY = res.Tables[0].Rows[i]["KEY"].ToString();
                    r.DATATYPE = res.Tables[0].Rows[i]["DATATYPE"].ToString();
                    try
                    {
                        r.PARENTKEY = res.Tables[0].Rows[i][""].ToString();
                    }
                    catch
                    { }
                    ret.Add(r);
                }
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void GetDataByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ID = Data["ID"].ToString();
                string strSql= $@"select r.id,r.configtype,r.key,r.datatype,r.parentkey,r.data from R_REPORTCENTER r where r.configtype='REPORT' and r.id='{ID}'";
                var res = sfcdb.RunSelect(strSql);
                R_REPORTCENTER r = new R_REPORTCENTER();
                r.ID = res.Tables[0].Rows[0]["ID"].ToString();
                r.CONFIGTYPE = res.Tables[0].Rows[0]["CONFIGTYPE"].ToString();
                r.KEY = res.Tables[0].Rows[0]["KEY"].ToString();
                r.DATATYPE = res.Tables[0].Rows[0]["DATATYPE"].ToString();
                try
                {
                    r.PARENTKEY = res.Tables[0].Rows[0][""].ToString();
                }
                catch
                { }
                byte[] data = (byte[])res.Tables[0].Rows[0]["DATA"];
                r.DATA = System.Text.Encoding.UTF8.GetString(data);
                StationReturn.Data = r;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void TEMP2(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();



                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void TEMP3(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();



                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void TEMP4(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }


    }
}
