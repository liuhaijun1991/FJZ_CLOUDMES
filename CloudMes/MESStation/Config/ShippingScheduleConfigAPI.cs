
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    public class ShippingScheduleConfigAPI : MesAPIBase
    {
        protected APIInfo FGetSAPSOList = new APIInfo()
        {
            FunctionName = "GetSAPSOList",
            Description = "获取SAP上下载的SOl列表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Status", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Page", InputType = "string", DefaultValue = "1" },
                new APIInputInfo() {InputName = "Pagesize", InputType = "string", DefaultValue = "50" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSAPSO = new APIInfo()
        {
            FunctionName = "GetSAPSO",
            Description = "获取SAP上下载的SO",
            Parameters = new List<APIInputInfo>()
            {
               new APIInputInfo() {InputName = "SO_NO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDownloadSAPSO = new APIInfo()
        {
            FunctionName = "DownloadSAPSO",
            Description = "从SAP上下载的SO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DownSoDate", InputType = "string", DefaultValue = "2020-06-10" },
                new APIInputInfo() {InputName = "DownSoPlant", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FConvertSAPSO = new APIInfo()
        {
            FunctionName = "ConvertSAPSO",
            Description = "ConvertSAPSO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_DATE", InputType = "string", DefaultValue = "2020-06-10" },
                new APIInputInfo() {InputName = "END_DATE", InputType = "string", DefaultValue = "2020-09-10" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateBroadcomPO = new APIInfo()
        {
            FunctionName = "UpdateBroadcomPO",
            Description = "UpdateBroadcomPO",
            Parameters = new List<APIInputInfo>()
            {
                
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetCustPOList = new APIInfo()
        {
            FunctionName = "GetCustPOList",
            Description = "GetCustPOList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "STATUS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DATE_FROM", InputType = "string", DefaultValue = "2020-06-10" },
                new APIInputInfo() {InputName = "DATE_TO", InputType = "string", DefaultValue = "2020-09-10" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetCustPOListByPO = new APIInfo()
        {
            FunctionName = "GetCustPOListByPO",
            Description = "GetCustPOListByPO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PO_NO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSOListBySkuno = new APIInfo()
        {
            FunctionName = "GetSOListBySkuno",
            Description = "GetSOListBySkuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCreateDNBySOPO = new APIInfo()
        {
            FunctionName = "CreateDNBySOPO",
            Description = "CreateDNBySOPO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SO_ITEN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO_ITEM", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };



        public ShippingScheduleConfigAPI()
        {
            this.Apis.Add(FGetSAPSOList.FunctionName, FGetSAPSOList);
            this.Apis.Add(FGetSAPSO.FunctionName, FGetSAPSO);
            this.Apis.Add(FDownloadSAPSO.FunctionName, FDownloadSAPSO);
            this.Apis.Add(FConvertSAPSO.FunctionName, FConvertSAPSO);
            this.Apis.Add(FUpdateBroadcomPO.FunctionName, FUpdateBroadcomPO);
            this.Apis.Add(FGetCustPOList.FunctionName, FGetCustPOList);
            this.Apis.Add(FGetCustPOListByPO.FunctionName, FGetCustPOListByPO);
            this.Apis.Add(FGetSOListBySkuno.FunctionName, FGetSOListBySkuno);
            this.Apis.Add(FCreateDNBySOPO.FunctionName, FCreateDNBySOPO);
        }
        public void CreateDNBySOPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string SO_NO = Data["SO_NO"].ToString().Trim();
                string SO_ITEN = Data["SO_ITEN"].ToString().Trim();
                int QTY = int.Parse( Data["QTY"].ToString().Trim());
                string PO_NO = Data["PO_NO"].ToString().Trim();
                string PO_ITEM = Data["PO_ITEM"].ToString().Trim();

                var so= sfcdb.ORM.Queryable<R_SO_DETAIL>().Where(t => t.SO_NO == SO_NO && t.LINE_SEQ == SO_ITEN).First();
                var po = sfcdb.ORM.Queryable<R_CUST_PO_DETAIL>().Where(t => t.CUST_PO_NO == PO_NO && t.LINE_NO == PO_ITEM).First();
                ShippingScheduleConfig.CreateDNBySOLine(BU, "VN06", so, QTY, "B75F", DateTime.Now, po, sfcdb,LoginUser.EMP_NO);

                //var ret = ShippingScheduleConfig.GetSOListBySkuno(sfcdb, SKUNO);
                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Data = ret;
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

        public void GetSOListBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string SKUNO = Data["SKUNO"].ToString().Trim();
                var ret = ShippingScheduleConfig.GetSOListBySkuno(sfcdb, SKUNO);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;
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

        public void GetCustPOListByPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string PO_NO = Data["PO_NO"].ToString().Trim();
                var ret = ShippingScheduleConfig.GetCustPOList(sfcdb, PO_NO);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;
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
        public void GetCustPOList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string STATUS = Data["STATUS"].ToString().Trim();
                string DATE_FROM = Data["DATE_FROM"].ToString().Trim();
                string DATE_TO = Data["DATE_TO"].ToString().Trim();
                var ret = ShippingScheduleConfig.GetCustPOList(sfcdb, DateTime.Parse(DATE_FROM), DateTime.Parse(DATE_TO), STATUS);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;
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

        public void UpdateBroadcomPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                ShippingScheduleConfig.UPDATE_Broadcom_PO(sfcdb, BU, LoginUser.EMP_NO);
                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Data = new { ListData, TotalNumber = ListData.Count };
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

        public void ConvertSAPSO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            sfcdb.BeginTrain();
            try
            {
                string SO_NO = Data["SO_NO"].ToString().Trim();
                string START_DATE = Data["START_DATE"].ToString().Trim();
                string END_DATE = Data["END_DATE"].ToString().Trim();

                var query = sfcdb.ORM.Queryable<R_SO_FILE>().Where(t => t.VBELN == SO_NO);
                var ListData = query.ToList();

                for (int i = 0; i < ListData.Count; i++)
                {
                    if (ListData[i].STATUS != R_SO_FILE_Status.WaitToConvert)
                    {
                        continue;
                    }
                    var SO = sfcdb.ORM.Queryable<R_SO>().Where(t => t.SO_NO == ListData[i].VBELN).First();
                    var isNewSO = false;
                    if (SO == null)
                    {
                        SO = new R_SO() { ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_SO")
                            , SO_NO = ListData[i].VBELN, BILL_TO_CODE = ListData[i].LAND1, ENABLE_DATE_FROM = DateTime.Parse(START_DATE),
                            ENABLE_DATE_TO = DateTime.Parse(END_DATE), STATUS = R_SO_Status.OPEN,
                             EDIT_DATE = DateTime.Now, EDIT_EMP = LoginUser.EMP_NO
                        };
                        isNewSO = true;
                        sfcdb.ORM.Insertable(SO).ExecuteCommand();
                    }
                    var SO_D = sfcdb.ORM.Queryable<R_SO_DETAIL>().Where(t => t.R_SO_ID == SO.ID && t.LINE_SEQ == ListData[i].POSNR).First();
                    if (SO_D == null)
                    {
                        SO_D = new R_SO_DETAIL() { ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_SO_DETAIL"), SO_NO = SO.SO_NO, LINE_SEQ = ListData[i].POSNR,
                            DN_QTY = 0, R_SO_ID = SO.ID , SKUNO = ListData[i].MATNR, QTY = double.Parse(ListData[i].KWMENG)
                        };
                        sfcdb.ORM.Insertable(SO_D).ExecuteCommand();
                        ListData[i].STATUS = R_SO_FILE_Status.WaitToDN;
                        ListData[i].EDIT_DATE = DateTime.Now;
                        ListData[i].EDIT_EMP = LoginUser.EMP_NO;
                        sfcdb.ORM.Updateable(ListData[i]).Where(t => t.ID == ListData[i].ID).ExecuteCommand();
                    }

                    sfcdb.CommitTrain();
                }


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = new { ListData, TotalNumber = ListData.Count };
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DownloadSAPSO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {

                string DownSoDate = Data["DownSoDate"].ToString().Trim();
                string DownSoPlant = Data["DownSoPlant"].ToString().Trim();
                MESPubLab.SAP_RFC.ZRFC_NSG_SD_0007B RFC = new MESPubLab.SAP_RFC.ZRFC_NSG_SD_0007B(this.BU);
                //RFC.SetValue(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), "VUGS", "ALL");
                RFC.SetValue(DownSoDate, DownSoPlant, "ALL");
                RFC.CallRFC();

                List<object> ret = new List<object>();
                for (int i = 0; i < RFC.SO_TAB.Rows.Count; i++)
                {
                    var row = RFC.SO_TAB.Rows[i];
                    var ex = sfcdb.ORM.Queryable<R_SO_FILE>().Where(t => t.VBELN == row["VBELN"].ToString().Trim() && t.POSNR == row["POSNR"].ToString()).First();
                    bool isNew = false;
                    if (ex == null)
                    {
                        ex = new R_SO_FILE();
                        ex.ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_SO_FILE");
                        ex.STATUS = R_SO_FILE_Status.WaitToConvert;
                        isNew = true;
                    }
                    ex.VBELN = row["VBELN"].ToString().Trim();
                    ex.POSNR = row["POSNR"].ToString().Trim();
                    ex.MATNR = row["MATNR"].ToString().Trim();
                    ex.BSTNK = row["BSTNK"].ToString().Trim();
                    ex.KWMENG = row["KWMENG"].ToString().Trim();
                    ex.CMPRE = row["CMPRE"].ToString().Trim();
                    ex.KUNNR = row["KUNNR"].ToString().Trim();
                    ex.KUNNV = row["KUNNV"].ToString().Trim();
                    ex.ARKTX = row["ARKTX"].ToString().Trim();
                    ex.NAME = row["NAME"].ToString().Trim();
                    ex.LAND1 = row["LAND1"].ToString().Trim();
                    ex.NETPR = row["NETPR"].ToString().Trim();
                    ex.EDIT_EMP = this.LoginUser.EMP_NO;
                    ex.EDIT_DATE = DateTime.Now;
                    if (isNew)
                    {
                        sfcdb.ORM.Insertable<R_SO_FILE>(ex).ExecuteCommand();
                    }
                    else
                    {
                        sfcdb.ORM.Updateable<R_SO_FILE>(ex).Where(t => t.ID == ex.ID).ExecuteCommand();
                    }
                    ret.Add(ex);

                }
                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;
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


        public void GetSAPSO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string SO_NO = Data["SO_NO"].ToString().Trim();
               
                var query = sfcdb.ORM.Queryable<R_SO_FILE>().Where(t=>t.VBELN == SO_NO);

                var ListData = query.ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = new { ListData, TotalNumber = ListData.Count };
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

        public void GetSAPSOList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();

            int TotalNumber = 0;
            try
            {
                string Page = Data["Page"].ToString().Trim();
                string Pagesize = Data["Pagesize"].ToString().Trim();
                var Status = Data["Status"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var query = sfcdb.ORM.Queryable<R_SO_FILE>().OrderBy(r => r.EDIT_DATE, SqlSugar.OrderByType.Desc).Take(1000);

                if (Status.Length > 0)
                {
                    query = query.In(t => t.STATUS, Status);
                }
                //var ListData = query.ToPageList(int.Parse(Page), int.Parse(Pagesize), ref TotalNumber);
                var ListData = query.ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = new { ListData, TotalNumber };
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
    }
}
