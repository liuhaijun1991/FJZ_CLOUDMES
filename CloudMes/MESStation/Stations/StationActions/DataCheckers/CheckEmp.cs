using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDBHelper;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckEmp
    {
        public static void InputEmpPrivchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession EMP_NOLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (EMP_NOLoadPoint == null)
            {
                EMP_NOLoadPoint = new MESStationSession() { MESDataType = "INPUTEMP", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(EMP_NOLoadPoint);
            }
            bool bPrivilege = false;
            string empNo = Input.Value.ToString();
            //T_c_user cUser = new T_c_user(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //Row_c_user rUser = cUser.getC_Userbyempno(empNo, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_c_user_role cUserRole = new T_c_user_role(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<get_c_roleid> roleList = cUserRole.GetRoleID(empNo, Station.SFCDB);
            List<string> listRoleID = new List<string>();
            foreach (var item in roleList)
            {
                listRoleID.Add(item.ROLE_ID);
            }
            T_C_ROLE_PRIVILEGE tRolePrivilege = new T_C_ROLE_PRIVILEGE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role_privilegeinfobyemp> privilegeList = new List<c_role_privilegeinfobyemp>();
            foreach (string item in listRoleID)
            {
                List<c_role_privilegeinfobyemp> tempList = tRolePrivilege.QueryRolePrivilege(item, Station.SFCDB);
                privilegeList.AddRange(tempList);
            }
            EMP_NOLoadPoint.Value = privilegeList;
            foreach (var item in privilegeList)
            {
                if (item.PRIVILEGE_NAME == Station.DisplayName)
                {
                    bPrivilege = true;
                }
            }
            if (bPrivilege)
            {
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                //throw new Exception("no privilege");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111028"));

            }
        }

        public static void LoginEmpPrivchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession EMP_LoginLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (EMP_LoginLoadPoint == null)
            {
                EMP_LoginLoadPoint = new MESStationSession() { MESDataType = "LOGINOUTEMP", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(EMP_LoginLoadPoint);
            }

            bool bPrivilege = false;
            string loginUserEmpNo = Input.Value.ToString();
            T_c_user_role cUserRole = new T_c_user_role(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<get_c_roleid> roleList = cUserRole.GetRoleID(loginUserEmpNo, Station.SFCDB);
            List<string> listRoleID = new List<string>();
            foreach (var item in roleList)
            {
                listRoleID.Add(item.ROLE_ID);
            }
            T_C_ROLE_PRIVILEGE tRolePrivilege = new T_C_ROLE_PRIVILEGE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role_privilegeinfobyemp> privilegeList = new List<c_role_privilegeinfobyemp>();
            foreach (string item in listRoleID)
            {
                List<c_role_privilegeinfobyemp> tempList = tRolePrivilege.QueryRolePrivilege(item, Station.SFCDB);
                privilegeList.AddRange(tempList);
            }
            EMP_LoginLoadPoint.Value = privilegeList;
            foreach (var item in privilegeList)
            {
                if (item.PRIVILEGE_NAME == Station.DisplayName)
                {
                    bPrivilege = true;
                }
            }
            if (bPrivilege)
            {
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new Exception("no privilege");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111028"));

            }
        }

        /// <summary>
        /// 根據工號檢查密碼是否正確
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void EmpPasswordChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionEmp == null || sessionEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionPwd = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPwd == null || sessionPwd.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowUser = t_c_user.getC_Userbyempno(sessionEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            if (rowUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionEmp.Value.ToString() }));
            }
            if (!rowUser.EMP_PASSWORD.Equals(sessionPwd.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { sessionEmp.Value.ToString() }));
            }
        }

        public static void EmpPrivCheckerByPrivilegeName(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionEmp == null || sessionEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            List<string> listPrivilege = new List<string>();
            string emp_no = sessionEmp.Value.ToString();
            bool bPrivi = false;
            T_c_user t_c_uer = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowUser = t_c_uer.getC_Userbyempno(sessionEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(Station.SFCDB, Station.DBType);
            
            if (rowUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionEmp.Value.ToString() }));
            }
            for (int i = 1; i < Paras.Count; i++)
            {
                if (Paras[i].VALUE != "")
                {
                    listPrivilege.Add(Paras[i].VALUE);
                }
            }
            if (listPrivilege.Count == 0)
            {
                //throw new MESReturnMessage("Please Input Privilege Name");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111552"));

            }
            foreach (string p in listPrivilege)
            {
                bPrivi = TCUP.CheckpPivilegeByName(Station.SFCDB, p, rowUser.EMP_NO);
                if (!bPrivi)
                {
                    //throw new MESReturnMessage("no privilege:" + p);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111756", new string[] { Paras[0].SESSION_TYPE }));
                }
            }
            Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }
        /// <summary>
        /// 檢查是否有補打權限
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void EmpPrivCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var user = Station.LoginUser.EMP_NO;
            OleExec SFCDB = Station.SFCDB;

            var station = Paras.Find(t => t.SESSION_TYPE == "STATION");
            string _station = "";

            if (station != null)
            {
                _station = station.VALUE.ToString();
            }
            var prs = SFCDB.ORM.Queryable<C_USER, C_USER_PRIVILEGE, C_PRIVILEGE>((U, UP, P) => new object[] {
                SqlSugar.JoinType.Left, U.ID == UP.USER_ID,
                SqlSugar.JoinType.Left, P.ID == UP.PRIVILEGE_ID
            })
                .Where((U, UP, P) => U.EMP_NO == user && P.PRIVILEGE_NAME == _station)
                .ToList();

            var _prs = SFCDB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE_PRIVILEGE, C_PRIVILEGE>((U, UR, RP, P) => new object[] {
                SqlSugar.JoinType.Left, U.ID == UR.USER_ID,
                SqlSugar.JoinType.Left, UR.ROLE_ID == RP.ROLE_ID,
                SqlSugar.JoinType.Left, P.ID == RP.PRIVILEGE_ID
            })
               .Where((U, UR, RP, P) => U.EMP_NO == user && P.PRIVILEGE_NAME == _station)
               .ToList();

            if (prs.Count == 0&& _prs.Count==0)
            {
               
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200318154141", new string[] { user }));
            }
         
        }


    }
}
