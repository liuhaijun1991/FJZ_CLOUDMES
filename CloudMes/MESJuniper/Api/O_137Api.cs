using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESPubLab;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject;
using MESDataObject.Module.OM;

namespace MESJuniper.Api
{
    public class O_137Api : MesAPIBase
    {
        protected APIInfo FGetCooMenuList = new APIInfo()
        {
            FunctionName = "GetCooMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetCartonLabelMenuList = new APIInfo()
        {
            FunctionName = "GetCartonLabelMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetModelSubPnMenuList = new APIInfo()
        {
            FunctionName = "GetModelSubPnMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSkuPackageMenuList = new APIInfo()
        {
            FunctionName = "GetSkuPackageMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FGetPnHbMapMenuList = new APIInfo()
        {
            FunctionName = "GetPnHbMapMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditCoo = new APIInfo()
        {
            FunctionName = "EditCoo",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditModelSubPn = new APIInfo()
        {
            FunctionName = "EditModelSubPn",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditCartonLabel = new APIInfo()
        {
            FunctionName = "EditCartonLabel",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditSkuPackage = new APIInfo()
        {
            FunctionName = "EditSkuPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditPnHbMap = new APIInfo()
        {
            FunctionName = "EditPnHbMap",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewCoo = new APIInfo()
        {
            FunctionName = "NewCoo",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewModelSubPn = new APIInfo()
        {
            FunctionName = "NewModelSubPn",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewCartonLabel = new APIInfo()
        {
            FunctionName = "NewCartonLabel",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewSkuPackage = new APIInfo()
        {
            FunctionName = "NewSkuPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewPnHbMap = new APIInfo()
        {
            FunctionName = "NewPnHbMap",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCoo = new APIInfo()
        {
            FunctionName = "DeleteCoo",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteModelSubPn = new APIInfo()
        {
            FunctionName = "DeleteModelSubPn",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCartonLabel = new APIInfo()
        {
            FunctionName = "DeleteCartonLabel",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteSkuPackage = new APIInfo()
        {
            FunctionName = "DeleteSkuPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeletePnHbMap = new APIInfo()
        {
            FunctionName = "DeletePnHbMap",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadSkuPackageData = new APIInfo()
        {
            FunctionName = "UploadSkuPackageData",
            Description = "UploadSkuPackageData",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadModelPnMapData = new APIInfo()
        {
            FunctionName = "UploadModelPnMapData",
            Description = "UploadModelPnMapData",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetWHSPackageMenuList = new APIInfo()
        {
            FunctionName = "GetWHSPackageMenuList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteWHSPackage = new APIInfo()
        {
            FunctionName = "DeleteWHSPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUploadWHSPackageData = new APIInfo()
        {
            FunctionName = "UploadWHSPackageData",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FNewWHSPackage = new APIInfo()
        {
            FunctionName = "NewWHSPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditWHSPackage = new APIInfo()
        {
            FunctionName = "EditWHSPackage",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        public O_137Api() {
            this.Apis.Add(FGetCooMenuList.FunctionName, FGetCooMenuList);
            this.Apis.Add(FGetCartonLabelMenuList.FunctionName, FGetCartonLabelMenuList);
            this.Apis.Add(FGetModelSubPnMenuList.FunctionName, FGetModelSubPnMenuList);
            this.Apis.Add(FGetSkuPackageMenuList.FunctionName, FGetSkuPackageMenuList);
            this.Apis.Add(FGetPnHbMapMenuList.FunctionName, FGetPnHbMapMenuList);

            this.Apis.Add(FEditCoo.FunctionName, FEditCoo);
            this.Apis.Add(FEditCartonLabel.FunctionName, FEditCartonLabel);
            this.Apis.Add(FEditModelSubPn.FunctionName, FEditModelSubPn);
            this.Apis.Add(FEditSkuPackage.FunctionName, FEditSkuPackage);
            this.Apis.Add(FEditPnHbMap.FunctionName, FEditPnHbMap);


            this.Apis.Add(FNewCoo.FunctionName, FNewCoo);
            this.Apis.Add(FNewCartonLabel.FunctionName, FNewCartonLabel);
            this.Apis.Add(FNewModelSubPn.FunctionName, FNewModelSubPn);
            this.Apis.Add(FNewSkuPackage.FunctionName, FNewSkuPackage);
            this.Apis.Add(FNewPnHbMap.FunctionName, FNewPnHbMap);


            this.Apis.Add(FDeleteCoo.FunctionName, FDeleteCoo);
            this.Apis.Add(FDeleteCartonLabel.FunctionName, FDeleteCartonLabel);
            this.Apis.Add(FDeleteModelSubPn.FunctionName, FDeleteModelSubPn);
            this.Apis.Add(FDeleteSkuPackage.FunctionName, FDeleteSkuPackage);
            this.Apis.Add(FDeletePnHbMap.FunctionName, FDeletePnHbMap);

            this.Apis.Add(FUploadSkuPackageData.FunctionName, FUploadSkuPackageData);
            this.Apis.Add(FUploadModelPnMapData.FunctionName, FUploadModelPnMapData);
            this.Apis.Add(FGetWHSPackageMenuList.FunctionName, FGetWHSPackageMenuList);
            this.Apis.Add(FDeleteWHSPackage.FunctionName, FDeleteWHSPackage);
            this.Apis.Add(FUploadWHSPackageData.FunctionName, FUploadWHSPackageData);

            this.Apis.Add(FNewWHSPackage.FunctionName, FNewWHSPackage);
            this.Apis.Add(FEditWHSPackage.FunctionName, FEditWHSPackage);

        }

        public void GetCooMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<O_137_COO_LABEL>().OrderBy(o => o.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
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

        public void GetCartonLabelMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<O_137CARTON_LABEL>().OrderBy(o => o.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
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

        public void GetModelSubPnMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<R_MODELSUBPN_MAP>().OrderBy(o => o.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
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
        public void UploadSkuPackageData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
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
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160849"));
                }




                string result = "";
         
                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string SKUNO = dt.Rows[i]["SKUNO"].ToString().Trim();
                    string SCENARIO = dt.Rows[i]["SCENARIO"].ToString().Trim();
                    string TYPE = dt.Rows[i]["TYPE"].ToString();
                    string USAGE = dt.Rows[i]["USAGE"].ToString();
                    string FROMN = dt.Rows[i]["FROMN"].ToString();
                    string TON = dt.Rows[i]["TON"].ToString();
                    string PARTNO = dt.Rows[i]["PARTNO"].ToString();
                    string CREATETIME = dt.Rows[i]["CREATETIME"].ToString();


                    try
                    {
                        
                        T_O_SKU_PACKAGE osku = new T_O_SKU_PACKAGE(SFCDB, DB_TYPE_ENUM.Oracle);
                        bool checkExist = SFCDB.ORM.Queryable<O_SKU_PACKAGE>()
                            .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();
                        if (checkExist)
                        {
                            
                            throw new Exception($@"機種新增配置已存在!");
                        }

                        int re = SFCDB.ORM.Insertable<O_SKU_PACKAGE>(new O_SKU_PACKAGE()
                        {
                            ID = osku.GetNewID(BU, SFCDB),
                            SKUNO = SKUNO,
                            SCENARIO = SCENARIO,
                            TYPE = TYPE,
                            USAGE = USAGE,
                            FROMN = FROMN,
                            TON = TON,
                            PARTNO = PARTNO,
                            CREATETIME = DateTime.Now
                        }).ExecuteCommand();

                        if (re == 0)
                        {
                            throw new Exception("Save Fail!");
                        }

                       
                    }
                    catch (Exception ex)
                    {
                        result += SKUNO + "," + ex.Message + ";";
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
        public void UploadModelPnMapData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
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
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160849"));
                }




                string result = "";

                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string CUSTPN = dt.Rows[i]["CUSTPN"].ToString().Trim();
                    string PARTNO = dt.Rows[i]["PARTNO"].ToString().Trim();
                    string SUBPARTNO = dt.Rows[i]["SUBPARTNO"].ToString();
                    string SUBPNREV = dt.Rows[i]["SUBPNREV"].ToString();
                    string CREATETIME = dt.Rows[i]["CREATETIME"].ToString();
                    string ISBASEPID = dt.Rows[i]["FLAG"].ToString();


                    try
                    {
                        if (SFCDB == null)
                        {
                            SFCDB = this.DBPools["SFCDB"].Borrow();
                        }
                        T_R_MODELSUBPN_MAP trm = new T_R_MODELSUBPN_MAP(SFCDB, DB_TYPE_ENUM.Oracle);

                        bool checkExist = SFCDB.ORM.Queryable<R_MODELSUBPN_MAP>().Where(r => r.CUSTPN == CUSTPN && r.PARTNO == PARTNO && r.SUBPNREV == SUBPNREV).Any();

                        if (checkExist)
                        {
                            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }

                        SFCDB.ORM.Insertable<R_MODELSUBPN_MAP>(new R_MODELSUBPN_MAP()
                        {
                            ID = trm.GetNewID(BU, SFCDB),
                            CUSTPN = CUSTPN,
                            PARTNO = PARTNO,
                            SUBPARTNO = SUBPARTNO,
                            SUBPNREV = SUBPNREV,
                            FLAG= ISBASEPID,
                            CREATETIME = DateTime.Now
                        }).ExecuteCommand();

                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    catch (Exception ex)
                    {
                        result += CUSTPN + "," + ex.Message + ";";
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

        public void GetSkuPackageMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<O_SKU_PACKAGE>().OrderBy(o => o.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
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
        public void GetPnHbMapMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<R_PN_HB_MAP>().OrderBy(o => o.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
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

        public void EditCoo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string COOVALUE = Data["COOVALUE"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string DESCRIPTIONS = Data["DESCRIPTIONS"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<O_137_COO_LABEL>().Where(o => o.COOVALUE == COOVALUE && o.PARTNO == PARTNO).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                if (sfcdb.ORM.Queryable<O_137_COO_LABEL>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<O_137_COO_LABEL>().SetColumns(o=>new O_137_COO_LABEL() {
                        COOVALUE= COOVALUE,
                        PARTNO= PARTNO,
                        DESCRIPTIONS= DESCRIPTIONS
                    }).Where(o => o.ID == ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message =MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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
        public void EditCartonLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SPECVAL = Data["SPECVAL"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string strQTY = Data["QTY"].ToString().Trim();
            string DESCRIPTIONS = Data["DESCRIPTIONS"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<O_137CARTON_LABEL>().Where(o => o.SKUNO == SKUNO && o.SPECVAL == SPECVAL && o.PARTNO == PARTNO).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                var QTY = Convert.ToInt16(strQTY);

                if (sfcdb.ORM.Queryable<O_137CARTON_LABEL>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<O_137CARTON_LABEL>().SetColumns(o => new O_137CARTON_LABEL()
                    {
                        SKUNO = SKUNO,
                        SPECVAL= SPECVAL,
                        PARTNO = PARTNO,
                        QTY= QTY.ToString(),
                        DESCRIPTIONS = DESCRIPTIONS
                    }).Where(o=>o.ID==ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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

        public void EditModelSubPn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string CUSTPN = Data["CUSTPN"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string SUBPARTNO = Data["SUBPARTNO"].ToString().Trim();
            string SUBPNREV = Data["SUBPNREV"].ToString().Trim();
            string FLAG = Data["FLAG"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<R_MODELSUBPN_MAP>().Where(r => r.CUSTPN == CUSTPN && r.PARTNO == PARTNO && r.SUBPNREV == SUBPNREV && r.FLAG == FLAG).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }


                if (sfcdb.ORM.Queryable<R_MODELSUBPN_MAP>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<R_MODELSUBPN_MAP>().SetColumns(o => new R_MODELSUBPN_MAP()
                    {
                        CUSTPN = CUSTPN,
                        PARTNO = PARTNO,
                        SUBPARTNO = SUBPARTNO,
                        SUBPNREV = SUBPNREV,
                        FLAG= FLAG
                    }).Where(o => o.ID == ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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

        public void EditSkuPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SCENARIO = Data["SCENARIO"].ToString().Trim();
            string TYPE = Data["TYPE"].ToString();
            string USAGE = Data["USAGE"].ToString();
            string FROMN = Data["FROMN"].ToString();
            string TON = Data["TON"].ToString();
            string PARTNO = Data["PARTNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<O_SKU_PACKAGE>()
                    .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                if (sfcdb.ORM.Queryable<O_SKU_PACKAGE>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<O_SKU_PACKAGE>().SetColumns(o => new O_SKU_PACKAGE()
                    {
                        SKUNO = SKUNO,
                        SCENARIO = SCENARIO,
                        TYPE = TYPE,
                        USAGE = USAGE,
                        FROMN = FROMN,
                        TON = TON,
                        PARTNO = PARTNO
                    }).Where(o => o.ID == ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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

        public void EditPnHbMap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string CUSTPN = Data["CUSTPN"].ToString().Trim();
            string HBPN = Data["HBPN"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();


                bool checkExist = sfcdb.ORM.Queryable<R_PN_HB_MAP>()
                    .Where(r => r.CUSTPN == CUSTPN && r.HBPN == HBPN).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                if (sfcdb.ORM.Queryable<R_PN_HB_MAP>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<R_PN_HB_MAP>().SetColumns(o => new R_PN_HB_MAP()
                    {
                        CUSTPN = CUSTPN,
                        HBPN = HBPN
                    }).Where(o => o.ID == ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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

        public void NewCoo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string COOVALUE = Data["COOVALUE"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string DESCRIPTIONS = Data["DESCRIPTIONS"].ToString();
          
          
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_O_137_COO_LABEL to137coo = new T_O_137_COO_LABEL(sfcdb, DB_TYPE_ENUM.Oracle);

                bool checkExist = sfcdb.ORM.Queryable<O_137_COO_LABEL>().Where(o => o.COOVALUE == COOVALUE && o.PARTNO == PARTNO).Any();
                //var id = MesDbBase.GetNewID<O_SKU_PACKAGE>(db,this.bu);
                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                    sfcdb.ORM.Insertable<O_137_COO_LABEL>(new O_137_COO_LABEL()
                    {
                        ID= to137coo.GetNewID(BU, sfcdb),
                        COOVALUE= COOVALUE,
                        PARTNO= PARTNO,
                        DESCRIPTIONS= DESCRIPTIONS,
                        CREATETIME=DateTime.Now
                    }).ExecuteCommand();
              
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
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

        public void NewCartonLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SPECVAL = Data["SPECVAL"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string strQTY = Data["QTY"].ToString().Trim();
            string DESCRIPTIONS = Data["DESCRIPTIONS"].ToString();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_O_137CARTON_LABEL to137ct = new T_O_137CARTON_LABEL(sfcdb, DB_TYPE_ENUM.Oracle);

                bool checkExist = sfcdb.ORM.Queryable<O_137CARTON_LABEL>().Where(o => o.SKUNO == SKUNO&& o.SPECVAL == SPECVAL && o.PARTNO == PARTNO).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                var QTY = Convert.ToInt16(strQTY);

                sfcdb.ORM.Insertable<O_137CARTON_LABEL>(new O_137CARTON_LABEL()
                {
                    ID = to137ct.GetNewID(BU, sfcdb),
                    SKUNO = SKUNO,
                    SPECVAL= SPECVAL,
                    PARTNO = PARTNO,
                    QTY=QTY.ToString(),
                    DESCRIPTIONS = DESCRIPTIONS,
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
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

        public void NewModelSubPn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string CUSTPN = Data["CUSTPN"].ToString().Trim();
            string PARTNO = Data["PARTNO"].ToString().Trim();
            string SUBPARTNO = Data["SUBPARTNO"].ToString().Trim();
            string SUBPNREV = Data["SUBPNREV"].ToString().Trim();
            string FLAG = Data["FLAG"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_MODELSUBPN_MAP trm = new T_R_MODELSUBPN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);

                bool checkExist = sfcdb.ORM.Queryable<R_MODELSUBPN_MAP>().Where(r => r.CUSTPN == CUSTPN && r.PARTNO == PARTNO && r.SUBPNREV == SUBPNREV).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                sfcdb.ORM.Insertable<R_MODELSUBPN_MAP>(new R_MODELSUBPN_MAP()
                {
                    ID = trm.GetNewID(BU, sfcdb),
                    CUSTPN = CUSTPN,
                    PARTNO = PARTNO,
                    SUBPARTNO = SUBPARTNO,
                    SUBPNREV = SUBPNREV,
                    CREATETIME = DateTime.Now,
                    FLAG= FLAG
                }).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
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

        public void NewSkuPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SCENARIO = Data["SCENARIO"].ToString().Trim();
            string TYPE = Data["TYPE"].ToString();
            string USAGE = Data["USAGE"].ToString();
            string FROMN = Data["FROMN"].ToString();
            string TON = Data["TON"].ToString();
            string PARTNO = Data["PARTNO"].ToString();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_O_SKU_PACKAGE osku = new T_O_SKU_PACKAGE(sfcdb, DB_TYPE_ENUM.Oracle);
                bool checkExist = sfcdb.ORM.Queryable<O_SKU_PACKAGE>()
                    .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();
                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                sfcdb.ORM.Insertable<O_SKU_PACKAGE>(new O_SKU_PACKAGE()
                {
                    ID = osku.GetNewID(BU, sfcdb),
                    SKUNO = SKUNO,
                    SCENARIO = SCENARIO,
                    TYPE = TYPE,
                    USAGE = USAGE,
                    FROMN = FROMN,
                    TON = TON,
                    PARTNO = PARTNO,
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
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

        public void NewPnHbMap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string CUSTPN = Data["CUSTPN"].ToString().Trim();
            string HBPN = Data["HBPN"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_PN_HB_MAP trpn = new T_R_PN_HB_MAP(sfcdb, DB_TYPE_ENUM.Oracle);

                bool checkExist = sfcdb.ORM.Queryable<R_PN_HB_MAP>()
                                .Where(r => r.CUSTPN == CUSTPN && r.HBPN == HBPN).Any();
                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161037");// "新增配置已存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                sfcdb.ORM.Insertable<R_PN_HB_MAP>(new R_PN_HB_MAP()
                {
                    ID = trpn.GetNewID(BU, sfcdb),
                    CUSTPN = CUSTPN,
                    HBPN = HBPN,
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");  //"新增成功";
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

        public void DeleteCoo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();
           

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<O_137_COO_LABEL>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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

        public void DeleteCartonLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<O_137CARTON_LABEL>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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
        public void DeleteModelSubPn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<R_MODELSUBPN_MAP>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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

        public void DeleteSkuPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<O_SKU_PACKAGE>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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

        public void DeletePnHbMap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<R_PN_HB_MAP>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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

        public void GetWHSPackageMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<O_WHS_PACKAGE>().OrderBy(o => new { o.SKUNO, o.PARTNO, o.CREATETIME, SqlSugar.OrderByType.Desc }).ToList();
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

        public void DeleteWHSPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<O_WHS_PACKAGE>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功
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

        public void UploadWHSPackageData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
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
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160849"));
                }

                string result = "";

                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string SKUNO = dt.Rows[i]["SKUNO"].ToString().Trim();
                    string SCENARIO = dt.Rows[i]["SCENARIO"].ToString().Trim();
                    string TYPE = dt.Rows[i]["TYPE"].ToString();
                    string USAGE = dt.Rows[i]["USAGE"].ToString();
                    string PARTNO = dt.Rows[i]["PARTNO"].ToString();
                    string CREATEEMP = dt.Rows[i]["CREATEEMP"].ToString();
                    string CREATETIME = dt.Rows[i]["CREATETIME"].ToString();

                    try
                    {

                        T_O_WHS_PACKAGE osku = new T_O_WHS_PACKAGE(SFCDB, DB_TYPE_ENUM.Oracle);
                        bool checkExist = SFCDB.ORM.Queryable<O_WHS_PACKAGE>()
                            .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();
                        if (checkExist)
                        {
                            throw new Exception($@"SKUNO {SKUNO} PARTNO {PARTNO} has exists, Please check!");
                        }

                        int re = SFCDB.ORM.Insertable<O_WHS_PACKAGE>(new O_WHS_PACKAGE()
                        {
                            ID = osku.GetNewID(BU, SFCDB),
                            SKUNO = SKUNO,
                            SCENARIO = SCENARIO,
                            TYPE = TYPE,
                            USAGE = USAGE,
                            PARTNO = PARTNO,
                            CREATEEMP = this.LoginUser.EMP_NO,
                            CREATETIME = DateTime.Now
                        }).ExecuteCommand();

                        if (re == 0)
                        {
                            throw new Exception("Save Fail!");
                        }

                    }
                    catch (Exception ex)
                    {
                        result += SKUNO + "," + ex.Message + ";";
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

        public void NewWHSPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SCENARIO = Data["SCENARIO"].ToString().Trim();
            string TYPE = Data["TYPE"].ToString();
            string USAGE = Data["USAGE"].ToString();
            string PARTNO = Data["PARTNO"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_O_WHS_PACKAGE osku = new T_O_WHS_PACKAGE(sfcdb, DB_TYPE_ENUM.Oracle);
                bool checkExist = sfcdb.ORM.Queryable<O_WHS_PACKAGE>()
                    .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();
                if (checkExist)
                {
                    StationReturn.Message = "This SKUNO already exists, Please check!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                sfcdb.ORM.Insertable<O_WHS_PACKAGE>(new O_WHS_PACKAGE()
                {
                    ID = osku.GetNewID(BU, sfcdb),
                    SKUNO = SKUNO,
                    SCENARIO = SCENARIO,
                    TYPE = TYPE,
                    USAGE = USAGE,
                    PARTNO = PARTNO,
                    CREATEEMP = this.LoginUser.EMP_NO,
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                StationReturn.Message = "Add Success";
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

        public void EditWHSPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString();
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string SCENARIO = Data["SCENARIO"].ToString().Trim();
            string TYPE = Data["TYPE"].ToString();
            string USAGE = Data["USAGE"].ToString();
            string PARTNO = Data["PARTNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                bool checkExist = sfcdb.ORM.Queryable<O_WHS_PACKAGE>()
                    .Where(o => o.SKUNO == SKUNO && o.SCENARIO == SCENARIO && o.TYPE == TYPE && o.PARTNO == PARTNO).Any();

                if (checkExist)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160936");// "修改後的配置已存在";;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }

                if (sfcdb.ORM.Queryable<O_WHS_PACKAGE>().Where(o => o.ID == ID).Any())
                {
                    sfcdb.ORM.Updateable<O_WHS_PACKAGE>().SetColumns(o => new O_WHS_PACKAGE()
                    {
                        SKUNO = SKUNO,
                        SCENARIO = SCENARIO,
                        TYPE = TYPE,
                        USAGE = USAGE,
                        CREATEEMP = this.LoginUser.EMP_NO,
                        PARTNO = PARTNO
                    }).Where(o => o.ID == ID).ExecuteCommand();
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161014");// "執行異常，找不到ID!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功
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

    }
}
