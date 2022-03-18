using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class BroadcomDNManager :  MesAPIBase
    {
        protected APIInfo FGetBroadcomDNList = new APIInfo()
        {
            FunctionName = "GetBroadcomDNList",
            Description = "Get Broadcom DN List",
            Parameters = new List<APIInputInfo>()
            {
                
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetDNPackingList = new APIInfo()
        {
            FunctionName = "GetDNPackingList",
            Description = "Get DN Packing List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DN_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        
        public BroadcomDNManager()
        {
            this.Apis.Add(FGetBroadcomDNList.FunctionName, FGetBroadcomDNList);
        }

        public void GetBroadcomDNList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<R_DN_CUST_PO>().ToList();                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = list;
                StationReturn.Message = "OK";

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetDNPackingList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string dn = Data["DN_NO"].ToString().Trim();
                if (string.IsNullOrEmpty(dn))
                {
                    throw new Exception("DN_NO Is Null");
                }
                Label.DCN.PackingListLabel label = new Label.DCN.PackingListLabel();
                BroadcomPackingList list = label.GetPackingList(SFCDB, dn);
                MES_DCN.Broadcom.PackingList broadcom = new MES_DCN.Broadcom.PackingList(SFCDB.ORM);
                Spire.Xls.Workbook workboolList = broadcom.GetWorkbook("PackingList_Broadcom",list);
                //用於在服務器保存生成的PacklingList Excel
                string savePath = System.IO.Directory.GetCurrentDirectory() + @"\PackingList\";
                string saveName = $"{dn}_{SFCDB.ORM.GetDate().ToString("yyyyMMddHHmmss")}.xlsx";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                workboolList.SaveToFile($@"{savePath}{saveName}", Spire.Xls.ExcelVersion.Version2013);
                byte[] output_excel;
                using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
                {
                    downfile.LoadFromFile($@"{savePath}{saveName}");
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    downfile.SaveToStream(ms);
                    output_excel = ms.ToArray();
                }
              
                string content = Convert.ToBase64String(output_excel);

                StationReturn.Data = new { FileName = saveName, Content = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";      
                StationReturn.Message = "OK";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }        
    }
}
