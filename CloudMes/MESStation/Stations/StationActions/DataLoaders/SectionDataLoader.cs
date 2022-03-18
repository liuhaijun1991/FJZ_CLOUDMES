using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class SectionDataLoader
    {
        /// <summary>
        /// 從輸入加載C_SECTION信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,C_SECTION保存的位置</param>
        public static void LoadSectionFromInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string SectionInput = "";
            MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == "StrSavePoint" && t.SessionKey == "1");
            if (strInput == null)
            {

            }
            else
            {
                SectionInput = strInput.Value.ToString();
            }
         
            string strSql = $@"SELECT * FROM C_SECTION WHERE SECTION_NAME = '{SectionInput.Replace("'", "''")}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RootCause", SectionInput) };
            DataTable res = Station.SFCDB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count <= 0)
            {
                Station.AddMessage("MES00000007", new string[] { "Section", SectionInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession Section = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (Section == null)
                {
                    Section = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(Section);
                }
                Section.Value = SectionInput;
            }
            
        }
    }
}
