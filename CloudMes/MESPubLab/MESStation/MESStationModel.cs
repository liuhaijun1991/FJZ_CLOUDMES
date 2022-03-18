using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections;


namespace MESPubLab.MESStation
{
    //public class MESStationModel_old_test
    //{
    //    public R_Station Station;

    //  //  public List<R_Station_Input> RStationInputList = new List<R_Station_Input>();

    //    public List<StationInputListItem> InputList = new List<StationInputListItem>();
    //    //  public List<List<R_Input_Action>> InputActionList = new List<List<R_Input_Action>>();

    ////    public List<R_Station_Action> RStationActionList = new List<R_Station_Action>();

    // //   public List<R_Station_Action_Para> StationActionParaList = new List<R_Station_Action_Para>();

    //    public List<R_Station_Output> StationOutput = new List<R_Station_Output>();

    //    [JsonIgnore]
    //    [ScriptIgnore]
    //    public DB_TYPE_ENUM DBType = DB_TYPE_ENUM.Oracle;

    //    public  void Init(string DisplayName, string Line, string BU, OleExec SFCDB)
    //    {
    //        //生成R_STATION類
    //        T_R_Station T = new T_R_Station(SFCDB, DBType);
    //        Row_R_Station R = T.GetRowByDisplayName(DisplayName, SFCDB);
    //        Station= R.GetDataObject();

    //        //生成InputActionList類
    //        List<R_Station_Input> RStationInputList = new List<R_Station_Input>();
    //        T_R_Station_Input T_I = new T_R_Station_Input(SFCDB, DBType);
    //        RStationInputList = T_I.GetRowsByStationID(Station.ID, SFCDB,true);

    //        T_R_Input_Action T_A = new T_R_Input_Action(SFCDB, DBType);

    //        T_R_Station_Action S_A = new T_R_Station_Action(SFCDB, DBType);
    //        for (int i = 0; i < RStationInputList.Count; i++)
    //        {
    //            List<R_Input_Action> inputaction = new List<R_Input_Action>();
    //            inputaction=T_A.GetActionByInputID(RStationInputList[i].INPUT_ID.ToString(), SFCDB);

    //            ActionListItem aitem = new ActionListItem();
    //            for (int j = 0; j < inputaction.Count; j++)
    //            {
    //                aitem.ID = inputaction[j].ID;
    //                aitem.INPUT_ID= inputaction[j].ID;
    //                aitem.C_STATION_ACTION_ID = inputaction[j].C_STATION_ACTION_ID;
    //                aitem.SEQ_NO = inputaction[j].SEQ_NO;
    //                aitem.CONFIG_TYPE = inputaction[j].CONFIG_TYPE;
    //                aitem.CONFIG_VALUE = inputaction[j].CONFIG_VALUE;
    //                aitem.ADD_FLAG = inputaction[j].ADD_FLAG;
    //                aitem.EDIT_TIME = inputaction[j].EDIT_TIME;
    //                aitem.EDIT_EMP = inputaction[j].EDIT_EMP;
    //                T_R_Station_Action_Para T_P = new T_R_Station_Action_Para(SFCDB, DBType);
    //            //    ParaList = T_P.GetActionParaByInputActionID(inputaction[0].ID, SFCDB);
    //                aitem.ParaI = T_P.GetActionParaByInputActionID(inputaction[0].ID, SFCDB);
    //                aitem.ParaA= T_P.GetActionParaByStationActionID(inputaction[0].ID, SFCDB);//暫時有問題

    //            }

    //            List<R_Station_Action> stationaction = new List<R_Station_Action>();
    //            stationaction = (S_A.GetActionByInputID(RStationInputList[i].ID.ToString(), SFCDB));

    //            StationInputListItem item = new StationInputListItem();
    //            item.ID = RStationInputList[i].ID;
    //            item.STATION_ID= RStationInputList[i].STATION_ID;
    //            item.INPUT_ID = RStationInputList[i].INPUT_ID;
    //            item.SEQ_NO = RStationInputList[i].SEQ_NO;
    //            item.REMEMBER_LAST_INPUT = RStationInputList[i].REMEMBER_LAST_INPUT;
    //            item.SCAN_FLAG = RStationInputList[i].SCAN_FLAG;
    //            item.DISPLAY_NAME = RStationInputList[i].DISPLAY_NAME;
    //            item.EDIT_TIME = RStationInputList[i].EDIT_TIME;
    //            item.EDIT_EMP = RStationInputList[i].EDIT_EMP;
    //            //  item.ActionList = inputaction;
    //            item.ActionList = aitem;
    //            item.StationActionList = stationaction;
    //            InputList.Add(item);
    //          //  T_R_Station_Action_Para T_P = new T_R_Station_Action_Para(SFCDB, DBType);
    //          //  StationActionParaList = T_P.GetActionParaByInputActionID(inputaction[0].ID, SFCDB);
    //        }
         

    //        T_R_Station_Output R_O = new T_R_Station_Output(SFCDB, DBType);
    //        StationOutput = R_O.GetStationOutputByStationID(Station.ID, SFCDB,true);



    //    }


    //    public class StationInputListItem: R_Station_Input
    //    {
    //        public ActionListItem ActionList;
    //        public List<R_Station_Action> StationActionList;
    //    }
    //    public class ActionListItem : R_Input_Action
    //    {
    //        public List<R_Station_Action_Para> ParaI;
    //        public List<R_Station_Action_Para> ParaA;
    //    }

    
    //}
    public class MESStationModel
    {
        public Dictionary<string, object> Station = new Dictionary<string, object>();

        [JsonIgnore]
        [ScriptIgnore]
        public DB_TYPE_ENUM DBType = DB_TYPE_ENUM.Oracle;
        public Dictionary<string, object> MakeStation()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("ID", null);
            ret.Add("DISPLAY_STATION_NAME", null);
            ret.Add("STATION_NAME", null);
            ret.Add("FAIL_STATION_ID", null);
            ret.Add("FAIL_STATION_FLAG", null);
            ret.Add("EDIT_TIME", null);
            ret.Add("EDIT_EMP", null);
            ret.Add("InputList", new List<object>() );
            ret.Add("OutputList", new List<object>() );

            return ret;
        }

        public Dictionary<string, object> MakeInput()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("ID", null);
            ret.Add("STATION_ID", null);
            ret.Add("INPUT_ID", null);
            ret.Add("SEQ_NO", null);
            ret.Add("REMEMBER_LAST_INPUT", null);
            ret.Add("EDIT_TIME", null);
            ret.Add("EDIT_EMP", null);
            ret.Add("SCAN_FLAG", null);
            ret.Add("DISPLAY_NAME", null);
            ret.Add("InputActionList", new List<object>());
            ret.Add("StationActionList", new List<object>());
            ret.Add("C_Input",null);
            return ret;
        }
        public Dictionary<string, object> MakeInputAction()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("ID", null);
            ret.Add("INPUT_ID", null);
            ret.Add("C_STATION_ACTION_ID", null);
            ret.Add("SEQ_NO", null);
            ret.Add("CONFIG_TYPE", null);
            ret.Add("CONFIG_VALUE", null);
            ret.Add("ADD_FLAG", null);
            ret.Add("EDIT_TIME", null);
            ret.Add("EDIT_EMP", null);
            ret.Add("ParaSA", new List<object>());
            ret.Add("ParaSB", new List<object>());
            ret.Add("C_STATION_ACTION", new List<object>());
            return ret;
        }

        public Dictionary<string, object> MakeStationAction()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("ID", null);
            ret.Add("R_STATION_INPUT_ID", null);
            ret.Add("C_STATION_ACTION_ID", null);
            ret.Add("SEQ_NO", null);
            ret.Add("CONFIG_TYPE", null);
            ret.Add("CONFIG_VALUE", null);
            ret.Add("ADD_FLAG", null);
            ret.Add("EDIT_TIME", null);
            ret.Add("EDIT_EMP", null);
            ret.Add("ParaSA", new List<object>());
            ret.Add("ParaSB", new List<object>());
            ret.Add("C_STATION_ACTION", new List<object>());
            return ret;
        }

        public void Init(string DisplayName, OleExec SFCDB)
        {
            //生成R_STATION類
            Dictionary<string, object> ret = new Dictionary<string, object>();
            T_R_Station T = new T_R_Station(SFCDB, DBType);
            Row_R_Station R = T.GetRowByDisplayName(DisplayName, SFCDB);
            R_Station RStation= R.GetDataObject();

            ret["ID"] = RStation.ID;
            ret["DISPLAY_STATION_NAME"] = RStation.DISPLAY_STATION_NAME;
            ret["STATION_NAME"] = RStation.STATION_NAME;
            ret["FAIL_STATION_ID"] = RStation.FAIL_STATION_ID;
            ret["FAIL_STATION_FLAG"] = RStation.FAIL_STATION_FLAG;
            ret["EDIT_TIME"] = RStation.EDIT_TIME;
            ret["EDIT_EMP"] = RStation.EDIT_EMP;

            List<object> inputlist = new List<object>();

            ret["InputList"] = inputlist;
            //生成InputActionList類
            List<R_Station_Input> RStationInputList = new List<R_Station_Input>();
            T_R_Station_Input T_I = new T_R_Station_Input(SFCDB, DBType);
            RStationInputList = T_I.GetRowsByStationID(RStation.ID, SFCDB, true);

            T_R_Input_Action T_A = new T_R_Input_Action(SFCDB, DBType);

            T_R_Station_Action S_A = new T_R_Station_Action(SFCDB, DBType);
            for (int i = 0; i < RStationInputList.Count; i++)
            {
              
                Dictionary<string, object> input = new Dictionary<string, object>();

                 

                input = MakeInput();

                input["C_Input"] = SFCDB.ORM.Queryable<C_Input>().Where(t => t.ID == RStationInputList[i].INPUT_ID).First();
                input["ID"]= RStationInputList[i].ID;
                input["STATION_ID"] = RStationInputList[i].STATION_ID;
                input["INPUT_ID"] = RStationInputList[i].INPUT_ID;
                input["SEQ_NO"] = RStationInputList[i].SEQ_NO;
                input["REMEMBER_LAST_INPUT"] = RStationInputList[i].REMEMBER_LAST_INPUT;
                input["EDIT_TIME"] = RStationInputList[i].EDIT_TIME;
                input["EDIT_EMP"] = RStationInputList[i].EDIT_EMP;
                input["SCAN_FLAG"] = RStationInputList[i].SCAN_FLAG;
                input["DISPLAY_NAME"] = RStationInputList[i].DISPLAY_NAME;

                List<object> daction = new List<object>();
                List<R_Input_Action> inputaction = new List<R_Input_Action>();
                inputaction = T_A.GetActionByInputID(RStationInputList[i].INPUT_ID.ToString(), SFCDB);
                for (int j=0; j<inputaction.Count;j++) //R_Input_Action表
                {
                    Dictionary<string, object> dinputaction = new Dictionary<string, object>();

                    dinputaction["ID"]=inputaction[j].ID;
                    dinputaction["INPUT_ID"] = inputaction[j].INPUT_ID;
                    dinputaction["C_STATION_ACTION_ID"]=inputaction[j].C_STATION_ACTION_ID;
                    dinputaction["SEQ_NO"] = inputaction[j].SEQ_NO;
                    dinputaction["CONFIG_TYPE"] = inputaction[j].CONFIG_TYPE;
                    dinputaction["CONFIG_VALUE"] = inputaction[j].CONFIG_VALUE;
                    dinputaction["ADD_FLAG"] = inputaction[j].ADD_FLAG;
                    dinputaction["EDIT_TIME"] = inputaction[j].EDIT_TIME;
                    dinputaction["EDIT_EMP"] = inputaction[j].EDIT_EMP;
                    //  ret["ParaIA"] =;
                    T_R_Station_Action_Para T_P = new T_R_Station_Action_Para(SFCDB, DBType);//R_Station_Action_Para 表
                    List<R_Station_Action_Para> ParaList = T_P.GetActionParaByInputActionID(inputaction[j].ID, SFCDB);
                    dinputaction["ParaSA"] = ParaList;
                    T_C_ACTION_PARA A_P = new T_C_ACTION_PARA(SFCDB, DBType); //C_ACTION_PAR 表
                    List<C_ACTION_PARA> AParaList = A_P.QueryActionPara(RStationInputList[i].INPUT_ID, SFCDB);
                    dinputaction["ParaSB"] = AParaList;
                    T_c_station_action CS_A = new T_c_station_action(SFCDB, DBType);//R_Station_Action_Para 表
                    c_station_action ParaList3 = CS_A.GetActionByID(inputaction[j].C_STATION_ACTION_ID, SFCDB);
                    dinputaction["C_STATION_ACTION"] = ParaList3;

                    daction.Add(dinputaction);
                }
             

                List<object> dsaction = new List<object>();
                List<R_Station_Action> stationaction = new List<R_Station_Action>();//R_Station_Action 表
                stationaction = S_A.GetActionByInputID(RStationInputList[i].ID.ToString(), SFCDB);


                for (int j = 0; j < stationaction.Count; j++) //R_Station_Action 表
                {
                    Dictionary<string, object> dinputaction2 = new Dictionary<string, object>();
                    dinputaction2= MakeStationAction();
                    dinputaction2["ID"] = stationaction[j].ID;
                    dinputaction2["R_STATION_INPUT_ID"] = stationaction[j].R_STATION_INPUT_ID;
                    dinputaction2["C_STATION_ACTION_ID"] = stationaction[j].C_STATION_ACTION_ID;
                    dinputaction2["SEQ_NO"] = stationaction[j].SEQ_NO;
                    dinputaction2["CONFIG_TYPE"] = stationaction[j].CONFIG_TYPE;
                    dinputaction2["CONFIG_VALUE"] = stationaction[j].CONFIG_VALUE;
                    dinputaction2["ADD_FLAG"] = stationaction[j].ADD_FLAG;
                    dinputaction2["EDIT_TIME"] = stationaction[j].EDIT_TIME;
                    dinputaction2["EDIT_EMP"] = stationaction[j].EDIT_EMP;
                  
                    T_R_Station_Action_Para T_P2 = new T_R_Station_Action_Para(SFCDB, DBType);//R_Station_Action_Para 表
                    List<R_Station_Action_Para> ParaList2 = T_P2.GetActionParaByStationActionID(stationaction[j].ID, SFCDB);
                    dinputaction2["ParaSA"] = ParaList2;

                    T_C_ACTION_PARA A_P = new T_C_ACTION_PARA(SFCDB, DBType);//C_ACTION_PARA 表
                    List<C_ACTION_PARA> AParaList = A_P.QueryActionParaByStation(stationaction[j].C_STATION_ACTION_ID, SFCDB);
                    dinputaction2["ParaSB"] = AParaList;

                    T_c_station_action CS_A = new T_c_station_action(SFCDB, DBType);//R_Station_Action_Para 表
                    c_station_action ParaList3 = CS_A.GetActionByID(stationaction[j].C_STATION_ACTION_ID, SFCDB);
                    dinputaction2["C_STATION_ACTION"] = ParaList3;

                    dsaction.Add(dinputaction2);
                }
                input["StationActionList"] = dsaction;

                inputlist.Add(input);
            }

            ret["InputList"] = inputlist;
            T_R_Station_Output R_O = new T_R_Station_Output(SFCDB, DBType);//R_Station_Output 表
            List<R_Station_Output>  StationOutput = R_O.GetStationOutputByStationID(RStation.ID, SFCDB, true);

            ret["OutputList"]= StationOutput;

            Station = ret;
        }

        //public void CopyStation(string newDisplayName,string BU ,OleExec SFCDB)
        //{
        //    Station["ID"] = MesDbBase.GetNewID<R_Station>(SFCDB.ORM, BU);
        //    Station["DISPLAY_STATION_NAME"] = newDisplayName;
        //    List<object> inputlist = (List<object>)Station["InputList"];
        //    for (int i = 0; i < inputlist.Count; i++)
        //    {
        //        Dictionary<string, object> input = (Dictionary<string, object>)inputlist[i];
        //        input["ID"] = MesDbBase.GetNewID<R_Station_Input>(SFCDB.ORM, BU);
        //        input["STATION_ID"] = Station["ID"];
        //        List<object> inputaction = (List<object>)input["StationActionList"];
        //        for (int j = 0; j < inputaction.Count; j++)
        //        {
        //            Dictionary<string, object> dinputaction2 = (Dictionary<string, object>)inputaction[j];
        //            dinputaction2["ID"] = MesDbBase.GetNewID<R_Station_Action>(SFCDB.ORM, BU);
        //            dinputaction2["R_STATION_INPUT_ID"] = input["ID"];

        //            List<object> lParaSA = (List<object>)dinputaction2["ParaSA"];
        //            for (int k = 0; k < lParaSA.Count; k++)
        //            {
        //                Dictionary<string, object> ParaSA = (Dictionary<string, object>)lParaSA[k];
        //                ParaSA["ID"] = MesDbBase.GetNewID<R_Station_Action_Para>(SFCDB.ORM, BU);
        //                ParaSA["R_STATION_ACTION_ID"] = dinputaction2["ID"];
        //            }
        //        }

        //    }
        //    List<object> outputlist = (List<object>)Station["OutputList"];
        //    for (int i = 0; i < outputlist.Count; i++)
        //    {
        //        Dictionary<string, object> output = (Dictionary<string, object>)outputlist[j];
        //        output["ID"] = MesDbBase.GetNewID<R_Station_Output>(SFCDB.ORM, BU);
        //        output["R_STATION_ID"] = Station["ID"];
        //    }


        //}
    }
}
