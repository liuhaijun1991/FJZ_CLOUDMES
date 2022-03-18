using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject;
using System.Data;

namespace MESStation.Config
{
    class RFacilityDetail : MesAPIBase
    {
       
        protected APIInfo FDeleteFacilityDetail = new APIInfo()
        {
            FunctionName = "DeleteFacilityDetail",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadFacilityData = new APIInfo()
        {
            FunctionName = "UploadFacilityData",
            Description = "UploadFacilityData",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewFacilityData = new APIInfo()
        {
            FunctionName = "NewFacilityData",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditFacilityData = new APIInfo()
        {
            FunctionName = "EditFacilityData",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetFacilityList = new APIInfo()
        {
            FunctionName = "GetFacilityList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };


        public RFacilityDetail()
        {
            this.Apis.Add(FDeleteFacilityDetail.FunctionName, FDeleteFacilityDetail);
            this.Apis.Add(FUploadFacilityData.FunctionName, FUploadFacilityData);
            this.Apis.Add(FNewFacilityData.FunctionName, FNewFacilityData);
            this.Apis.Add(FEditFacilityData.FunctionName, FEditFacilityData);
            this.Apis.Add(FGetFacilityList.FunctionName, FGetFacilityList);

        }
        public void DeleteFacilityDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<r_facility>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = "刪除成功";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void NewFacilityData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string FACILITY_NAME = Data["FACILITY_NAME"].ToString().Trim();
            string FACILITY_NO = Data["FACILITY_NO"].ToString().Trim();
            string SN = Data["SN"].ToString();
            string CUSTOMER_NAME = Data["CUSTOMER_NAME"].ToString();
            string MT_PERIOD = Data["MT_PERIOD"].ToString();
            string MT_EMP = Data["MT_EMP"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_r_facility osku = new T_r_facility(sfcdb, DB_TYPE_ENUM.Oracle);
                bool checkExist = sfcdb.ORM.Queryable<r_facility>()
                    .Where(o => o.FACILITY_NAME == FACILITY_NAME && o.FACILITY_NO == FACILITY_NO && o.SN == SN).Any();
                if (checkExist)
                {
                    StationReturn.Message = "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                sfcdb.ORM.Insertable<r_facility>(new r_facility()
                {
                    ID = osku.GetNewID(BU, sfcdb),
                    FACILITY_NAME = FACILITY_NAME,
                    FACILITY_NO = FACILITY_NO,
                    SN = SN,
                    STATUS= "OPEN",
                    CUSTOMER_NAME = CUSTOMER_NAME,
                    REGIST_DATE = DateTime.Now,
                    MT_PERIOD = MT_PERIOD,
                    MT_EMP= MT_EMP,
                    MT_COUNT="1",
                    NEXT_MT_DATE= DateTime.Now.AddDays(Convert.ToDouble(MT_PERIOD)),
                    EDIT_EMP= this.LoginUser.EMP_NO,
                    EDIT_DATE = DateTime.Now
                }).ExecuteCommand();

                StationReturn.Message = "新增成功";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void EditFacilityData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string FACILITY_NAME = Data["FACILITY_NAME"].ToString().Trim();
            string FACILITY_NO = Data["FACILITY_NO"].ToString().Trim();
            string SN = Data["SN"].ToString();
            string CUSTOMER_NAME = Data["CUSTOMER_NAME"].ToString();
            string MT_PERIOD = Data["MT_PERIOD"].ToString();
            string MT_EMP = Data["MT_EMP"].ToString();
            string MT_DATE = Data["MT_DATE"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<r_facility>()
                    .Where(o =>  o.ID == ID&& MT_DATE!="").Any();

                if (checkExist)
                {
                    StationReturn.Message = "修改後的配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                if (sfcdb.ORM.Queryable<r_facility>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<r_facility>().SetColumns(o => new r_facility()
                    {
                        MT_DATE = Convert.ToDateTime(MT_DATE),
                        STATUS="CLOSED",
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_DATE = DateTime.Now
                   
                    }).Where(o => o.ID == ID).ExecuteCommand();
                    var getcount = sfcdb.ORM.Queryable<r_facility>().Where(o => o.ID == ID).ToList();
                    int MT_COUNT;
                    MT_COUNT = Convert.ToInt32(getcount[0].MT_COUNT);

                    T_r_facility osku = new T_r_facility(sfcdb, DB_TYPE_ENUM.Oracle);
                    sfcdb.ORM.Insertable<r_facility>(new r_facility()
                    {
                        ID = osku.GetNewID(BU, sfcdb),
                        FACILITY_NAME = FACILITY_NAME,
                        FACILITY_NO = FACILITY_NO,
                        SN = SN,
                        STATUS = "OPEN",
                        CUSTOMER_NAME = CUSTOMER_NAME,
                        REGIST_DATE = getcount[0].REGIST_DATE,
                        MT_PERIOD = MT_PERIOD,
                        MT_EMP = MT_EMP,
                        MT_COUNT = Convert.ToString(MT_COUNT+1),
                        NEXT_MT_DATE = Convert.ToDateTime(MT_DATE).AddDays(Convert.ToDouble(MT_PERIOD)),
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_DATE = DateTime.Now
                    }).ExecuteCommand();

                }
                else
                {
                    StationReturn.Message = "執行異常，找不到ID。";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = "修改成功";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void UploadSkuPackageData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    //throw new Exception("Please Input Excel Data");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429	", new string[] { "Excel Data" }));
                }
                if (Data["FileName"] == null)
                {
                    //throw new Exception("Please Input FileName");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429	", new string[] { "FileName" }));
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    //throw new Exception($@"上傳的Excel中沒有內容!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111744"));

                }




                string result = "";

                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string FACILITY_NAME = Data["FACILITY_NAME"].ToString().Trim();
                    string FACILITY_NO = Data["FACILITY_NO"].ToString().Trim();
                    string SN = Data["SN"].ToString();
                    string CUSTOMER_NAME = Data["CUSTOMER_NAME"].ToString();
                    string MT_PERIOD = Data["MT_PERIOD"].ToString();
                    string MT_EMP = Data["MT_EMP"].ToString();

                    try
                    {
                        SFCDB = this.DBPools["SFCDB"].Borrow();
                        T_r_facility osku = new T_r_facility(SFCDB, DB_TYPE_ENUM.Oracle);
                        bool checkExist = SFCDB.ORM.Queryable<r_facility>()
                            .Where(o => o.FACILITY_NAME == FACILITY_NAME && o.FACILITY_NO == FACILITY_NO && o.SN == SN).Any();
                        if (checkExist)
                        {
                            StationReturn.Message = "新增配置已存在";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }

                        SFCDB.ORM.Insertable<r_facility>(new r_facility()
                        {
                            ID = osku.GetNewID(BU, SFCDB),
                            FACILITY_NAME = FACILITY_NAME,
                            FACILITY_NO = FACILITY_NO,
                            SN = SN,
                            STATUS = "",
                            CUSTOMER_NAME = CUSTOMER_NAME,
                            REGIST_DATE = DateTime.Now,
                            MT_PERIOD = MT_PERIOD,
                            MT_EMP = MT_EMP,
                            MT_COUNT = "1",
                            NEXT_MT_DATE = DateTime.Now.AddDays(Convert.ToDouble(MT_PERIOD)),
                            EDIT_EMP = this.LoginUser.EMP_NO,
                            EDIT_DATE = DateTime.Now
                        }).ExecuteCommand();

                        StationReturn.Message = "新增成功";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    catch (Exception ex)
                    {
                        result += FACILITY_NO + "," + ex.Message + ";";
                    }

                }


                if (result == "")
                {
                    result = "All Upload OK ! ";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    result = "Upload Fail:" + result;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                #endregion

                StationReturn.Message = result;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }
        public void GetFacilityList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<r_facility>().OrderBy(o => o.EDIT_DATE, SqlSugar.OrderByType.Desc).ToList();
                StationReturn.Data = MenuData;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }


    }
}
