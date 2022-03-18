using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Data.OleDb;


namespace MESJuniper.Api
{
    public class MRRAPI : MesAPIBase
    {
        private APIInfo _GetFAISkuno = new APIInfo()
        {
            FunctionName = "GetFAISkuno",
            Description = "新增頁面獲取可選擇的機種",
            Parameters = new List<APIInputInfo>()
            {
            },

            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteOBAFAI = new APIInfo()
        {
            FunctionName = "DeleteOBAFAI",
            Description = "刪除，改狀態為9",
            Parameters = new List<APIInputInfo>()
            {
            },

            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetSkuVerBySkuno = new APIInfo()
        {

            FunctionName = "GetSkuVerBySkuno",
            Description = "新增頁面選擇機種時獲取機種版本",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Skuno" } },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _GetOBAFAIMenuList = new APIInfo()
        {

            FunctionName = "GetOBAFAIMenuList",
            Description = "FAI頁面主頁的報表",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _CheckWoExist = new APIInfo()
        {

            FunctionName = "CheckWoExist",
            Description = "輸入工站時檢查工單是否存在於系統",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _GetStationListByWo = new APIInfo()
        {

            FunctionName = "GetStationListByWo",
            Description = "新增頁面通過輸入的工單來獲得路由列表中的工站",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _GetSkuByWO = new APIInfo()
        {

            FunctionName = "GetSkuByWO",
            Description = "通過工單獲取機種",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _AddFAIByWorkorder = new APIInfo()
        {

            FunctionName = "AddFAIByWorkorder",
            Description = "新增工單類型的FAI",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _UpdateFAIByWorkorder = new APIInfo()
        {

            FunctionName = "UpdateFAIByWorkorder",
            Description = "更新工單類型的FAI",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };
        private APIInfo _NewMRRbySkuno = new APIInfo()
        {

            FunctionName = "NewMRRbySkuno",
            Description = "上傳FAI文件",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _UpdateMRRbySkuno = new APIInfo()
        {

            FunctionName = "UpdateMRRbySkuno",
            Description = "上傳FAI文件",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };

        private APIInfo _FileDownLoad = new APIInfo()
        {

            FunctionName = "FileDownLoad",
            Description = "下載FAI文件",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }


        };


        public MRRAPI()
        {
            Apis.Add(_GetFAISkuno.FunctionName, _GetFAISkuno);
            Apis.Add(_DeleteOBAFAI.FunctionName, _DeleteOBAFAI);
            Apis.Add(_GetSkuVerBySkuno.FunctionName, _GetSkuVerBySkuno);
            Apis.Add(_GetOBAFAIMenuList.FunctionName, _GetOBAFAIMenuList);
            Apis.Add(_CheckWoExist.FunctionName, _CheckWoExist);
            Apis.Add(_GetStationListByWo.FunctionName, _GetStationListByWo);
            Apis.Add(_GetSkuByWO.FunctionName, _GetSkuByWO);
            Apis.Add(_AddFAIByWorkorder.FunctionName, _AddFAIByWorkorder);
            Apis.Add(_UpdateFAIByWorkorder.FunctionName, _UpdateFAIByWorkorder);
            Apis.Add(_NewMRRbySkuno.FunctionName, _NewMRRbySkuno);
            Apis.Add(_FileDownLoad.FunctionName, _FileDownLoad);
            Apis.Add(_UpdateMRRbySkuno.FunctionName, _UpdateMRRbySkuno);
        }

        public void GetAllFAISkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_R_FAI TCSKU = new T_R_FAI(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCSKU.GetAllFAISkuno(db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void DeleteOBAFAI(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString().Trim();

            try
            {
                T_R_FAI TRFAI = new T_R_FAI(db, DB_TYPE_ENUM.Oracle);
                T_R_FILE TRF = new T_R_FILE(db, DB_TYPE_ENUM.Oracle);
                string FileName = TRFAI.GetFAIList(ID, db).FILENAME;
                TRFAI.DeleteOBAFAI(ID, db);
                TRF.SetFileDisableByFileName(FileName, "OBAFAI", db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "";
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetSkuVerBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Skuno = Data["Skuno"].ToString();
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_R_FAI TCSKUVER = new T_R_FAI(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = TCSKUVER.GetSkuVerBySkuno(Skuno, db);



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetOBAFAIMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FAI tFAIList = null;
            OleExec sfcdb = null;

            try
            {
                // var res = oleDB.ORM.Queryable<R_TEST_JUNIPER>()
                //.Where(rt => rt.SYSSERIALNO == SN)
                //.Select(rt => new { rt.SYSSERIALNO, rt.EVENTNAME, rt.STATUS, rt.TATIME })
                //.OrderBy(rt => rt.TATIME, OrderByType.Desc).ToList();


                sfcdb = this.DBPools["SFCDB"].Borrow();
                tFAIList = new T_R_FAI(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = tFAIList.GetMRRList(sfcdb);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void CheckWoExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE CheckWoFlag = null;
            OleExec sfcdb = null;
            string WORKORDERNO = Data["WORKORDERNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CheckWoFlag = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = CheckWoFlag.CheckDataExist(WORKORDERNO, sfcdb);


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetStationListByWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_ROUTE_DETAIL CheckWoFlag = null;
            OleExec sfcdb = null;
            string WORKORDERNO = Data["WORKORDERNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CheckWoFlag = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = CheckWoFlag.GetWoLStation(WORKORDERNO, sfcdb);


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetSkuByWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE CheckWoFlag = null;
            OleExec sfcdb = null;
            string WORKORDERNO = Data["WORKORDERNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CheckWoFlag = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = CheckWoFlag.GetSkunoByWO(WORKORDERNO, "", sfcdb);


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void UpdateFAIByWorkorder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_FAI Table = null;
            string FAIObject = string.Empty;
            FAIList FAI = null;
            string result = string.Empty;
            string EDIT_EMP = LoginUser.EMP_NO;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_R_FAI(sfcdb, DBTYPE);
                FAIObject = Data["FAIObject"].ToString();

                FAI = (FAIList)JsonConvert.DeserializeObject(FAIObject, typeof(FAIList));
                result = Table.UpdateFAIbyWo(FAI, EDIT_EMP, GetDBDateTime(), sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //更新成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                }
                else
                {
                    //更新失敗
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                //不是最新的數據，返回字符串無法被 Int32.Parse 方法轉換成 int,所以出現異常
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000032";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    //數據庫執行異常
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public bool CheckExistLock(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "SKU" && r.WORKORDERNO == Skuno && r.LOCK_STATUS == "1").Any();
        }

        public bool CheckExistMRRSku(string Skuno, string SkuVer, OleExec DB)
        {
            return DB.ORM.Queryable<R_FAI>().Where(r => r.SKUNO == Skuno && r.SKU_VER == SkuVer && r.PRODUCTTYPE=="PRR" && r.FAITYPE == "SKUNO").Any();
        }


        public void NewMRRbySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;


            string FAITYPE = Data["FAITYPE"].ToString();
            string PRODUCTTYPE = Data["PRODUCTTYPE"].ToString();
            string SKUNO = Data["SKUNO"].ToString();
            string SKU_VER = Data["SKU_VER"].ToString();
            string REMARK = Data["STATUS"].ToString();

            string EDITBY = LoginUser.EMP_NO;
            DateTime EDITTIME = GetDBDateTime();
            string FileName = Data["FileName"].ToString();
            string NAME = Data["Name"].ToString();
            int EXNAMENUM = 1;
            string EXNAME = FileName.Substring(FileName.LastIndexOf(".") + 1);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_FAI TRFAI = new T_R_FAI(sfcdb, DBTYPE);
                T_R_FAI_STATION TRFAIS = new T_R_FAI_STATION(sfcdb, DBTYPE);
                T_R_FAI_DETAIL TRFAID = new T_R_FAI_DETAIL(sfcdb, DBTYPE);
                T_R_SN_LOCK TRSNLOCK = new T_R_SN_LOCK(sfcdb, DBTYPE);
                T_R_FILE TRF = new T_R_FILE(sfcdb, DB_TYPE_ENUM.Oracle);

                if (!CheckExistMRRSku(SKUNO, SKU_VER, sfcdb))
                {
                    while (TRF.CheckExist(NAME, "OBAFAI", sfcdb))
                    {
                        if (NAME.EndsWith(")")
                            && int.TryParse(NAME.Substring(NAME.LastIndexOf("(") + 1, NAME.LastIndexOf(")") - NAME.LastIndexOf("(") - 1), out EXNAMENUM))
                        {
                            EXNAMENUM++;
                            NAME = NAME.Substring(0, NAME.LastIndexOf("(") + 1) + EXNAMENUM.ToString() + ")";
                            FileName = NAME + "." + EXNAME;
                        }
                        else
                        {
                            NAME = NAME + "(" + $@"{EXNAMENUM}" + ")";
                            FileName = NAME + "." + EXNAME;
                        }
                    }

                    if (!CheckExistLock(SKUNO, sfcdb))
                    {
                        Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
                        RRF.ID = TRF.GetNewID(BU, sfcdb);
                        RRF.NAME = SKUNO;
                        RRF.FILENAME = FileName;
                        RRF.USETYPE = "PRR";
                        RRF.STATE = "1";
                        RRF.VALID = 1;
                        RRF.BLOB_FILE = ":BLOB_FILE";
                        RRF.EDIT_EMP = EDITBY;
                        RRF.EDIT_TIME = DateTime.Now;
                        sfcdb.ThrowSqlExeception = true;
                        //將同類文件改為歷史版本
                        //TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, sfcdb);

                        string strSql = RRF.GetInsertString(this.DBTYPE);
                        strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
                        strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
                        System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
                        string B64 = Data["Bas64File"].ToString();

                        string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                        byte[] data = Convert.FromBase64String(b64);
                        p.Value = data;

                        Row_R_FAI RowRF = (Row_R_FAI)TRFAI.NewRow();
                        RowRF.ID = TRFAI.GetNewID(BU, sfcdb);
                        RowRF.FAITYPE = FAITYPE;
                        RowRF.PRODUCTTYPE = PRODUCTTYPE;
                        RowRF.SKUNO = SKUNO;
                        RowRF.SKU_VER = SKU_VER;
                        RowRF.STATUS = "1";
                        RowRF.REMARK = REMARK;
                        RowRF.CREATEBY = EDITBY;
                        RowRF.CREATETIME = EDITTIME;
                        RowRF.EDITBY = EDITBY;
                        RowRF.EDITTIME = EDITTIME;

                        Row_R_FAI_STATION RowRFS = (Row_R_FAI_STATION)TRFAIS.NewRow();
                        RowRFS.ID = TRFAIS.GetNewID(BU, sfcdb);
                        RowRFS.FAIID = RowRF.ID;
                        RowRFS.CREATEBY = EDITBY;
                        RowRFS.CREATETIME = EDITTIME;

                        Row_R_FAI_DETAIL RowRFD = (Row_R_FAI_DETAIL)TRFAID.NewRow();
                        RowRFD.ID = TRFAID.GetNewID(BU, sfcdb);
                        RowRFD.FAISTATIONID = RowRFS.ID;
                        RowRFD.STATUS = "1";
                        RowRFD.FILENAME = FileName;
                        RowRFD.FAITIME = EDITTIME;
                        RowRFD.CREATETIME = EDITTIME;
                        RowRFD.EDITTIME = EDITTIME;
                        RowRFD.FAIBY = EDITBY;
                        RowRFD.CREATEBY = EDITBY;
                        RowRFD.EDITBY = EDITBY;

                        Row_R_SN_LOCK RowSNLock = (Row_R_SN_LOCK)TRSNLOCK.NewRow();
                        RowSNLock.ID = TRSNLOCK.GetNewID(BU, sfcdb);
                        RowSNLock.TYPE = "SKU";
                        RowSNLock.WORKORDERNO = SKUNO;
                        RowSNLock.LOCK_STATION = "ALL";
                        RowSNLock.LOCK_STATUS = "1";
                        RowSNLock.LOCK_REASON = "PRR Checklist not released jet by JNPR. Please Contact " + EDITBY + ".";
                        RowSNLock.LOCK_EMP = EDITBY;
                        RowSNLock.LOCK_TIME = EDITTIME;



                        sfcdb.ExecSQL(RowRF.GetInsertString(DBTYPE));
                        sfcdb.ExecSQL(RowRFS.GetInsertString(DBTYPE));
                        sfcdb.ExecSQL(RowRFD.GetInsertString(DBTYPE));
                        sfcdb.ExecSQL(RowSNLock.GetInsertString(DBTYPE));
                        sfcdb.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[] { p });

                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000035";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20201SKULOCKEDALREADY";
                    }

                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20200513161845";
                }

            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        [Obsolete]
        public void InsertANDUpdateR_FILE(string NAME, string FileName, string EDITBY, Newtonsoft.Json.Linq.JObject Data)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            DateTime EDITTIME = GetDBDateTime();
            T_R_FILE TRF = new T_R_FILE(sfcdb, DB_TYPE_ENUM.Oracle);
            Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
            RRF.ID = TRF.GetNewID(BU, sfcdb);
            RRF.NAME = NAME;
            RRF.FILENAME = FileName;
            RRF.USETYPE = "PRR";
            RRF.STATE = "1";
            RRF.VALID = 1;
            RRF.BLOB_FILE = ":BLOB_FILE";
            RRF.EDIT_EMP = EDITBY;
            RRF.EDIT_TIME = DateTime.Now;
            sfcdb.ThrowSqlExeception = true;
            //將同類文件改為歷史版本
            //TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, sfcdb);

            string strSql = RRF.GetInsertString(this.DBTYPE);
            strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
            strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
            System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
            string B64 = Data["Bas64File"].ToString();

            string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(b64);
            p.Value = data;
            //UPDATE LAST REGISTER TO VALID = 0
            int RF;

            RF = sfcdb.ORM.Updateable<R_FILE>()
              .UpdateColumns(a => new R_FILE()
              {
                  VALID = 0,
              })
          .Where(a => a.NAME == NAME && a.USETYPE == "PRR").ExecuteCommand();

           sfcdb.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[] { p });

        }

        public void UpdateMRRbySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {                          
            OleExec sfcdb = null;

            sfcdb = this.DBPools["SFCDB"].Borrow();
            string SKUNO = Data["SKUNO"].ToString();
            string REMARK = Data["STATUS"].ToString();
            string ID = Data["ID"].ToString();
            string EDITBY = LoginUser.EMP_NO;
            DateTime EDITTIME = GetDBDateTime();
            string FileName = Data["FileName"].ToString();
            string NAME = Data["Name"].ToString();

            try
            {
                if (REMARK == "COMPLETE")
                {
                    
                    T_R_FAI TRFAI = new T_R_FAI(sfcdb, DBTYPE);
                    T_R_FILE TRF = new T_R_FILE(sfcdb, DB_TYPE_ENUM.Oracle);

                    string result = string.Empty;
                    int RF, RFS, RFD = 0;

                    RF = sfcdb.ORM.Updateable<R_FAI>
                      (a => new R_FAI()
                      {
                          REMARK = REMARK,
                          EDITBY = EDITBY,
                          EDITTIME = EDITTIME,
                      })
                    .Where(a => a.ID == ID).ExecuteCommand();

                    T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);
                    var test = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(a => a.TYPE == "SKU" && a.WORKORDERNO == SKUNO && a.LOCK_STATUS == "1").First();

                    Row_R_SN_LOCK rowLock = (Row_R_SN_LOCK)t_r_sn_lock.GetObjByID(test.ID, sfcdb);
                    rowLock.LOCK_STATUS = "0";
                    rowLock.UNLOCK_REASON = "PRR Completed by " + EDITBY + ".";
                    rowLock.UNLOCK_EMP = EDITBY;
                    rowLock.UNLOCK_TIME = EDITTIME;
                    sfcdb.ExecSQL(rowLock.GetUpdateString(DBTYPE));

                    InsertANDUpdateR_FILE(SKUNO, FileName, EDITBY, Data);
                }
                else
                {
                    int RFS = 0;
                    //solo actualizar FECHA Y ARCHIVO

                    RFS = sfcdb.ORM.Updateable<R_FAI>
                      (a => new R_FAI()
                      {
                          REMARK = REMARK,
                          EDITBY = EDITBY,
                          EDITTIME = EDITTIME,
                      })
                    .Where(a => a.ID == ID).ExecuteCommand();

                    InsertANDUpdateR_FILE(SKUNO, FileName, EDITBY, Data);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000035";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {

                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void FileDownLoad(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string FAIID = Data["ID"].ToString();
            string FileName = "";
            try
            {

                T_R_FAI TRFAI = new T_R_FAI(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_Label t_r_label = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                FileName = TRFAI.GetFAIList(FAIID, SFCDB).FILENAME;

                R_FILE file = TRF.GetFileByFileName(FileName, "PRR", SFCDB);
                if (file == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { FileName }));
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



    }
}
