using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Config
{
  public  class LineConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addline = new APIInfo()
        {
            FunctionName = "AddLine",
            Description = "添加線體",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName= "LINE_NAME",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SECTION_ID",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "LINE_CODE",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "LINE_PCAS",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "DESCRIPTION",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>() 
            { }

        };

        private APIInfo deleteline = new APIInfo()
        {
            FunctionName = "DeleteLine",
            Description = "刪除線體",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo updateline = new APIInfo()
        {
            FunctionName = "UpdateLine",
            Description = "更新線體",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName= "LINE_NAME",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SECTION_ID",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "LINE_CODE",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "LINE_PCAS",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "DESCRIPTION",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getallline = new APIInfo()
        {
            FunctionName = "GetAllLine",
            Description = "獲取所有線體",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getlinebypcas = new APIInfo()
        {
            FunctionName = "GetLineByPCAS",
            Description = "根據區段獲取線體",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="LINE_PCAS",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo FGetLineByID = new APIInfo()
        {
            FunctionName = "GetLineByID",
            Description = "通過ID獲取線別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo FGetLineByLikeSearch = new APIInfo()
        {
            FunctionName = "GetLineByLikeSearch",
            Description = "通過模糊查詢獲取線別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="LINE_NAME",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        /// <summary>
        /// 構造函數
        /// </summary>
        public LineConfig()
        {
            this.Apis.Add(addline.FunctionName, addline);
            this.Apis.Add(deleteline.FunctionName, deleteline);
            this.Apis.Add(updateline.FunctionName, updateline);
            this.Apis.Add(getallline.FunctionName, getallline);
            this.Apis.Add(getlinebypcas.FunctionName, getlinebypcas);
            this.Apis.Add(FGetLineByID.FunctionName, FGetLineByID);
            this.Apis.Add(FGetLineByLikeSearch.FunctionName, FGetLineByLikeSearch);
        }
        /// <summary>
        /// 添加線體
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_LINE line;
            C_LINE newline;
            string LineName = Data["LINE_NAME"].ToString().Trim();
            string SectionID = Data["SECTION_ID"].ToString().Trim();
            string LineCode = Data["LINE_CODE"].ToString().Trim();
            string LinePcas = Data["LINE_PCAS"].ToString().Trim();
            string Description = Data["DESCRIPTION"].ToString().Trim();
            try
            {
                if (LineName.Length<=0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LineName");
                    return;
                }

                if (SectionID.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SectionID");
                    return;
                }
                if (LineCode.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LineCode");
                    return;
                }
                if (LinePcas.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LinePcas");
                    return;
                }
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                if (line.CheckDataExist(LineName, sfcdb))
                {
                    newline = new C_LINE();
                    newline.ID = line.GetNewID(BU,sfcdb);
                    newline.LINE_NAME = LineName;
                    newline.SECTION_ID=SectionID;
                    newline.LINE_CODE =LineCode;
                    newline.LINE_PCAS=LinePcas;
                    newline.DESCRIPTION= Description;
                    newline.EDIT_EMP=LoginUser.EMP_NO;
                    newline.EDIT_TIME=GetDBDateTime();
                    int result = line.add(newline,sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add(LineName);
                    }
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(LineName);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

        public void DeleteLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_LINE line;
            C_LINE checkLine;
            string deleteid = "";
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                bool isallOK = true;
                sfcdb.BeginTrain();
                for (int i = 0; i < ID.Count; i++)
                {
                    deleteid = ID[i].ToString();
                    checkLine = line.GetLineById(deleteid, sfcdb);
                    if (checkLine == null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000007";
                        StationReturn.MessagePara.Add("Line Id:" + deleteid);
                        isallOK = false;
                        break;
                    }
                    else
                    {
                        int result = line.deleteById(deleteid, sfcdb);
                        if (result <= 0)
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000023";
                            StationReturn.MessagePara.Add("Line Id:" + deleteid);
                            isallOK = false;
                            break;
                        }
                    }
                }
                if (isallOK)
                {
                    sfcdb.CommitTrain();                    
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    sfcdb.RollbackTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }
        /// <summary>
        /// 更新標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;           
            T_C_LINE line;
            C_LINE newline;
            string ID= Data["ID"].ToString().Trim();
            string LineName = Data["LINE_NAME"].ToString().Trim();
            string SectionID = Data["SECTION_ID"].ToString().Trim();
            string LineCode = Data["LINE_CODE"].ToString().Trim();
            string LinePcas = Data["LINE_PCAS"].ToString().Trim();
            string Description = Data["DESCRIPTION"].ToString().Trim();
            try
            {
                if (ID.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ID");
                    return;
                }
                if (LineName.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LineName");
                    return;
                }

                if (SectionID.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SectionID");
                    return;
                }
                if (LineCode.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LineCode");
                    return;
                }
                if (LinePcas.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("LinePcas");
                    return;
                }
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                newline = line.GetByLineName(LineName,sfcdb);
                if (newline != null && newline.ID != ID)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(LineName);
                }
                else
                {
                    newline = new C_LINE();
                    newline.ID =ID;
                    newline.LINE_NAME = LineName;
                    newline.SECTION_ID = SectionID;
                    newline.LINE_CODE = LineCode;
                    newline.LINE_PCAS = LinePcas;
                    newline.DESCRIPTION = Description;
                    newline.EDIT_EMP = LoginUser.EMP_NO;
                    newline.EDIT_TIME = GetDBDateTime();
                    int result = line.Update(newline, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "MES00000003";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000025";
                        StationReturn.MessagePara.Add(LineName);
                    }
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        /// <summary>
        /// 獲取所有線體
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_LINE> linelist = new List<C_LINE>();
            T_C_LINE line;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                linelist = line.GetLineData(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = linelist;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// BY區段獲取線體
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetLineByPCAS(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_LINE> linelist = new List<C_LINE>();
            T_C_LINE line;
            string LinePcas = Data["LINE_PCAS"].ToString().Trim();            
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                linelist = line.GetLinePcas(LinePcas, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES000000016";
                StationReturn.Data = linelist;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        public void GetLineByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            C_LINE resultline;
            T_C_LINE line;
            string LineId = Data["ID"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                resultline = line.GetLineById(LineId, sfcdb);
                if (resultline != null)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000026";
                    StationReturn.Data = resultline;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Line Id:" + LineId);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }
        public void GetLineByLikeSearch(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List< C_LINE> resultline;
            T_C_LINE line;
            string Linevalue = Data["LINE_NAME"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                line = new T_C_LINE(sfcdb, DBTYPE);
                resultline = line.GetLineBylike(Linevalue, sfcdb);
                if (resultline != null&&resultline.Count>0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000026";
                    StationReturn.Data = resultline;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }
    }
}
//RuRun
