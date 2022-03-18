using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;

namespace MESStation.FileUpdate
{
    public class Fileuplaod : MesAPIBase
    {
        protected APIInfo _GetLabelList = new APIInfo()
        {
            FunctionName = "GetLabelList",
            Description = "GetLabelList",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _UpLoadLabelFile = new APIInfo()
        {
            FunctionName = "UpLoadLabelFile",
            Description = "UpLoadLabelFile",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "FileName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "MD5", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "LabelName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "PrintType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ArryLength", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "Bas64File", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _UpLoadFile = new APIInfo()
        {
            FunctionName = "UpLoadFile",
            Description = "UpLoadFile",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "FileName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "MD5", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "Bas64File", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileByName = new APIInfo()
        {
            FunctionName = "GetFileByName",
            Description = "GetFileByName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = "test"},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}

            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileList = new APIInfo()
        {
            FunctionName = "GetFileList",
            Description = "GetFileList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetFileUseType = new APIInfo()
        {
            FunctionName = "GetFileUseType",
            Description = "GetFileUseType",
            Parameters = new List<APIInputInfo>(),

            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _FileDownLoad = new APIInfo()
        {
            FunctionName = "FileDownLoad",
            Description = "FileDownLoad",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "STRING", DefaultValue = "LAB"}
            },

            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileHisListByName = new APIInfo()
        {
            FunctionName = "GetFileHisListByName",
            Description = "GetFileHisListByName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = "test"},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}

            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _UploadQuackLabelFile = new APIInfo()
        {
            FunctionName = "UploadQuackLabelFile",
            Description = "Upload Quack Label for BPD",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Data", InputType = "STRING", DefaultValue = ""},
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _FileDownLoadByBrowser = new APIInfo()
        {
            FunctionName = "FileDownLoadByBrowser",
            Description = "FileDownLoadByBrowser",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "STRING", DefaultValue = "LAB"}
            },

            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public APIInfo _GetStationList = new APIInfo()
        {
            FunctionName= "GetStationList",
            Description="Get all stations by skuno and version",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="SKUNO",InputType="STRING" },
                new APIInputInfo() {InputName="VERSION",InputType="STRING" }
            },
            Permissions=new List<MESPermission>()

        };

        public Fileuplaod()
        {
            Apis.Add(_UpLoadFile.FunctionName, _UpLoadFile);
            Apis.Add(_GetFileUseType.FunctionName, _GetFileUseType);
            Apis.Add(_GetFileByName.FunctionName, _GetFileByName);
            Apis.Add(_GetFileList.FunctionName, _GetFileList);
            Apis.Add(_FileDownLoad.FunctionName, _FileDownLoad);
            Apis.Add(_GetFileHisListByName.FunctionName, _GetFileHisListByName);
            Apis.Add(_UpLoadLabelFile.FunctionName, _UpLoadLabelFile);
            Apis.Add(_GetLabelList.FunctionName, _GetLabelList);
            Apis.Add(_FileDownLoadByBrowser.FunctionName, _FileDownLoadByBrowser);
        }
        public void GetLabelList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_Label> ret = TRL.GetLabelList(SFCDB);

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }


        public void UpLoadLabelFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {


            string LabelName = Data["LabelName"].ToString();
            string PrintType = Data["PrintType"].ToString();
            string ArryLength = Data["ArryLength"].ToString();
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            SFCDB.BeginTrain();
            try
            {

                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);

                Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
                RRF.ID = TRF.GetNewID(BU, SFCDB);
                RRF.NAME = Data["Name"].ToString();
                RRF.FILENAME = Data["FileName"].ToString();
                RRF.MD5 = Data["MD5"].ToString();
                RRF.USETYPE = Data["UseType"].ToString();
                RRF.STATE = "1";
                RRF.VALID = 1;
                //不使用CLOB字段
                //RRF.CLOB_FILE = ":CLOB_FILE";// Data["Bas64File"].ToString();
                RRF.BLOB_FILE = ":BLOB_FILE";
                RRF.EDIT_EMP = LoginUser.EMP_NO;
                RRF.EDIT_TIME = DateTime.Now;
                SFCDB.ThrowSqlExeception = true;
                //將同類文件改為歷史版本
                TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, SFCDB);

                string strSql = RRF.GetInsertString(this.DBTYPE);
                strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
                strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
                System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
                string B64 = Data["Bas64File"].ToString();

                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);
                p.Value = data;
                //new System.Data.OleDb.OleDbParameter(":CLOB_FILE", Data["Bas64File"].ToString()),
                SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[]
                { p
                });
                T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_Label RRL = TRL.GetLabelConfigByLabelName(LabelName, SFCDB);
                if (RRL == null)
                {
                    RRL = (Row_R_Label)TRL.NewRow();
                    RRL.ID = TRL.GetNewID(BU, SFCDB);
                    RRL.LABELNAME = LabelName;
                    RRL.R_FILE_NAME = RRF.FILENAME;
                    RRL.PRINTTYPE = PrintType;
                    RRL.EDIT_EMP = LoginUser.EMP_NO;
                    RRL.EDIT_TIME = DateTime.Now;
                    RRL.ARRYLENGTH = double.Parse(ArryLength);
                    strSql = RRL.GetInsertString(DB_TYPE_ENUM.Oracle);
                }
                else
                {
                    RRL.LABELNAME = LabelName;
                    RRL.R_FILE_NAME = RRF.FILENAME;
                    RRL.PRINTTYPE = PrintType;
                    RRL.EDIT_EMP = LoginUser.EMP_NO;
                    RRL.EDIT_TIME = DateTime.Now;
                    RRL.ARRYLENGTH = double.Parse(ArryLength);
                    strSql = RRL.GetUpdateString(DB_TYPE_ENUM.Oracle);
                }
                string strRes = SFCDB.ExecSQL(strSql);

                //SFCDB.ExecSQL(RRF.GetInsertString(this.DBTYPE));
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileHisListByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string UseType = Data["UseType"].ToString();
            string FileName = Data["Name"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_FILE> ret = TRF.GetFileHisList(FileName, UseType, SFCDB);


                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void FileDownLoad(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_FILE RRF = (Row_R_FILE)TRF.GetObjByID(ID, SFCDB);
                string filePath = ConfigurationManager.AppSettings["WebFilePath"];
                filePath += "\\" + RRF.FILENAME;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                byte[] b = (byte[])RRF["BLOB_FILE"];
                F.Write(b, 0, b.Length);
                F.Flush();
                F.Close();



                StationReturn.Data = "DOWNLOAD\\" + RRF.FILENAME;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string UseType = Data["UseType"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_FILE> ret = TRF.GetFileList(UseType, SFCDB);


                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                //R_FILE R = TRF.GetFileByName(Data["Name"].ToString(),Data["UseType"].ToString(),SFCDB );
                R_FILE R = TRF.GetFileByFileName(Data["Name"].ToString(), Data["UseType"].ToString(), SFCDB);

                StationReturn.Data = R;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileUseType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {

                List<string> ret = new List<string>();
                ret.Add("LABEL");
                ret.Add("PPL");
                ret.Add("FACK");
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {

                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            //SFCDB.ThrowSqlExeception = false;
            //this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void UpLoadFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            SFCDB.BeginTrain();
            try
            {

                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);

                Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
                RRF.ID = TRF.GetNewID(BU, SFCDB);
                RRF.NAME = Data["Name"].ToString();
                RRF.FILENAME = Data["FileName"].ToString();
                RRF.MD5 = Data["MD5"].ToString();
                RRF.USETYPE = Data["UseType"].ToString();
                RRF.STATE = "1";
                RRF.VALID = 1;
                //不使用CLOB字段
                //RRF.CLOB_FILE = ":CLOB_FILE";// Data["Bas64File"].ToString();
                RRF.BLOB_FILE = ":BLOB_FILE";
                RRF.EDIT_EMP = LoginUser.EMP_NO;
                RRF.EDIT_TIME = DateTime.Now;
                SFCDB.ThrowSqlExeception = true;
                //將同類文件改為歷史版本
                TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, SFCDB);

                string strSql = RRF.GetInsertString(this.DBTYPE);
                strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
                strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
                System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
                string B64 = Data["Bas64File"].ToString();

                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);
                p.Value = data;
                //new System.Data.OleDb.OleDbParameter(":CLOB_FILE", Data["Bas64File"].ToString()),
                SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[]
                { p
                });

                //SFCDB.ExecSQL(RRF.GetInsertString(this.DBTYPE));
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// BPD 插入 Quack Label 與 SN 的對應關係，上傳 Excel 批量插入
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UploadQuackLabelFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SN_KEYPART_DETAIL TRSKD = null;
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                TRSKD = new T_R_SN_KEYPART_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                string KeyParts = Data["KeyParts"].ToString();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(KeyParts));
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<R_SN_KEYPART_DETAIL>));
                List<R_SN_KEYPART_DETAIL> Details = serializer.ReadObject(ms) as List<R_SN_KEYPART_DETAIL>;
                SFCDB.BeginTrain();
                Details.ForEach(t =>
                {
                    t.CATEGORY = "QUACK_LABEL"; t.CATEGORY_NAME = "QUACK_LABEL";
                    t.CREATE_EMP = LoginUser.EMP_NO; t.CREATE_TIME = DateTime.Now;
                    t.EDIT_EMP = LoginUser.EMP_NO; t.EDIT_TIME = DateTime.Now;
                    t.STATION_NAME = "QUACK";
                });
                //SFCDB.ORM.Insertable<R_SN_KEYPART_DETAIL>
                int result = TRSKD.InsertKeypartDetails(Details, LoginUser.BU, SFCDB);
                SFCDB.CommitTrain();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara.Add(result.ToString());

                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
        }

        public void FileDownLoadByBrowser(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString();
            try
            {
                T_R_Label t_r_label = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_Label rowLable = (Row_R_Label)t_r_label.GetObjByID(ID, SFCDB);
                if (rowLable == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { ID }));
                }

                R_FILE file = TRF.GetFileByFileName(rowLable.R_FILE_NAME, "LABEL", SFCDB);
                if (file == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { rowLable.R_FILE_NAME }));
                }
                Row_R_FILE rowFile = (Row_R_FILE)TRF.GetObjByID(file.ID, SFCDB);
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                byte[] b = (byte[])rowFile["BLOB_FILE"];
                string content = Convert.ToBase64String(b);
                StationReturn.Data = new { FileName = file.FILENAME, Content = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetStationList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string SkuId = string.Empty;
            try
            {
                SkuId = Data["SkuId"].ToString();
                T_R_SKU_ROUTE TRSR = new T_R_SKU_ROUTE(SFCDB, DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                R_SKU_ROUTE RouteMapping = null;
                List<R_SKU_ROUTE> RouteMappings = TRSR.GetMappingBySkuId(SkuId, SFCDB);
                RouteMapping = RouteMappings.Find(t => t.DEFAULT_FLAG == "Y");
                if (RouteMapping==null)
                {
                    RouteMapping = RouteMappings.First();
                }
                List<C_ROUTE_DETAIL> RouteDetails = TCRD.GetByRouteIdOrderBySEQASC(RouteMapping.ROUTE_ID, SFCDB);
                List<string> Stations = RouteDetails.Select(t => t.STATION_NAME).ToList();
                StationReturn.Data = Stations;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }


    }
}
