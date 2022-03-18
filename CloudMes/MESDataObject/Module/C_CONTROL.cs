using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_CONTROL : DataObjectTable
    {
        public T_C_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CONTROL);
            TableName = "C_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_CONTROL GetControlByName(string controlName,OleExec db)
        {
            List<C_CONTROL> Cs = db.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == controlName).ToList();
            if (Cs.Count > 0)
            {
                return Cs.First();
            }
            else
            {
                return null;
            }
        }

        public List<C_CONTROL> GetControlList(string controlName, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == controlName).ToList();
        }
        public List<C_CONTROL> GetControlList(string controlName,string controlValue, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == controlName)
                .WhereIF(!string.IsNullOrEmpty(controlValue), t => t.CONTROL_VALUE.Contains(controlValue)).ToList();
        }

        public bool ValueIsExist(string controlName, string controlValue, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Any(t => t.CONTROL_NAME == controlName && t.CONTROL_VALUE == controlValue);
        }
        public bool existControlValue(string controlName, string controlValue,string controlLevel, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == controlName && t.CONTROL_VALUE == controlValue && t.CONTROL_LEVEL== controlLevel).Any();
        }

        public int Update(OleExec DB, C_CONTROL controlObject)
        {
            return DB.ORM.Updateable<C_CONTROL>(controlObject).Where(r => r.ID == controlObject.ID).ExecuteCommand();
        }

        /// <summary>
        /// 檢查CONTROL RUN工單是否單獨包裝
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckSoloPack(string Wo, string Carton, string Pallet, OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
                    FROM R_WO_BASE
                   WHERE WORKORDERNO <> '{Wo}'
                     AND WORKORDERNO IN
                         (SELECT WORKORDERNO
                            FROM R_SN
                           WHERE ID IN
                                 (SELECT SN_ID
                                    FROM R_SN_PACKING
                                   WHERE PACK_ID IN
                                         (SELECT ID
                                            FROM R_PACKING
                                           WHERE PACK_TYPE = 'CARTON'
                                             AND (PARENT_PACK_ID IN
                                                 (SELECT ID
                                                     FROM R_PACKING
                                                    WHERE PACK_TYPE = 'PALLET'
                                                      AND PACK_NO =
                                                          '{Pallet}') OR
                                                 PACK_ID IN
                                                 (SELECT ID
                                                     FROM R_PACKING
                                                    WHERE PACK_TYPE = 'CARTON'
                                                      AND PACK_NO = 
                                                          '{Carton}')))))
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812161642", new string[] { Wo });
                //errMsg=Wo+",CONTROL RUN工單要求單獨包裝"
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 檢查ASSY,assyp,特殊工單綁定管控
        /// 子工單(CONTROL_VALUE)只能和父工單(CONTROL_LEVEL)綁定
        /// SPECIFY_WO
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckSpecifyWo(string fatherWO,string kpwo, OleExec DB)
        {
            string sql = string.Empty;            
            string dtwo = string.Empty;
            string sonwo = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
               SELECT B.CONTROL_VALUE,B.CONTROL_LEVEL
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'SPECIFY_WO'
               AND B.CONTROL_VALUE = '{kpwo}' 
               AND B.CONTROL_LEVEL = '{fatherWO}'              
                 "; 
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count ==0)
            {
                sql = $@" 
               SELECT B.CONTROL_VALUE,B.CONTROL_LEVEL
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'SPECIFY_WO'
               AND B.CONTROL_VALUE = '{fatherWO}'                   
                 ";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    dtwo = dt.Rows[0]["CONTROL_LEVEL"].ToString();
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812161846", new string[] { fatherWO, dtwo });
                    throw new MESReturnMessage(errMsg);
                }

                sql = $@" 
               SELECT B.CONTROL_VALUE,B.CONTROL_LEVEL
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'SPECIFY_WO'
              AND B.CONTROL_LEVEL = '{fatherWO}'
                 ";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    sonwo = dt.Rows[0]["CONTROL_VALUE"].ToString();
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812162201", new string[] { fatherWO, sonwo });
                    throw new MESReturnMessage(errMsg);
                }

                sql = $@" 
               SELECT B.CONTROL_VALUE,B.CONTROL_LEVEL
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'SPECIFY_WO'
               AND B.CONTROL_VALUE = '{kpwo}'                   
                 ";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    dtwo = dt.Rows[0]["CONTROL_LEVEL"].ToString();
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812161846", new string[] { kpwo, dtwo });
                    throw new MESReturnMessage(errMsg);
                }

                sql = $@" 
               SELECT B.CONTROL_VALUE,B.CONTROL_LEVEL
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'SPECIFY_WO'
              AND B.CONTROL_LEVEL = '{kpwo}'
                 ";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    sonwo = dt.Rows[0]["CONTROL_VALUE"].ToString();
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812162201", new string[] { kpwo, sonwo });
                    throw new MESReturnMessage(errMsg);
                    //throw new MESReturnMessage($@"PE配置工單{kpwo}只能綁定子工單{sonwo},請找PE確認");
                }
            }

        }


        /// <summary>
        /// 檢查卡通，棧板是否混裝了 CONTROL RUN工單 
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckExistsControlWo( string Carton, string Pallet, OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
     FROM R_WO_BASE A
    WHERE EXISTS
    (SELECT 1
             FROM C_CONTROL B
            WHERE B.CONTROL_NAME = 'NO_MIXED'
              AND B.CONTROL_VALUE = A.WORKORDERNO)
      AND A.WORKORDERNO IN
          (SELECT WORKORDERNO
             FROM R_SN
            WHERE ID IN
                  (SELECT SN_ID
                     FROM R_SN_PACKING
                    WHERE PACK_ID IN
                          (SELECT ID
                             FROM R_PACKING
                            WHERE PACK_TYPE = 'CARTON'
                              AND (PARENT_PACK_ID IN
                                  (SELECT ID
                                      FROM R_PACKING
                                     WHERE PACK_TYPE = 'PALLET'
                                       AND PACK_NO = '{Pallet}') OR
                                  PACK_ID IN
                                  (SELECT ID
                                      FROM R_PACKING
                                     WHERE PACK_TYPE = 'CARTON'
                                       AND PACK_NO = '{Carton}')))))
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812162358", new string[] { Carton, Pallet });
                throw new MESReturnMessage(errMsg);
                //throw new MESReturnMessage($@"CONTROL RUN 卡通{Carton}&棧板{Pallet}不能裝入非CONTROL RUN產品"); ;
            }
        }

        public C_CONTROL getnamefromcontrol(string routename, OleExec DB)
        {
            string strSql = $@"select * from c_control where control_name='FAI_CONTROL_ROUTE' and CONTROL_TYPE=:routename";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":routename", OleDbType.VarChar, 240) };
            paramet[0].Value = routename;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }

        }

        public C_CONTROL GetRouteInControlbyWO(string routeid, OleExec DB)
        {
            string strSql = $@"select * from c_control where control_value in(
                               select route_name from c_route where id=:routeid)";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":routeid", OleDbType.VarChar, 240) };
            paramet[0].Value = routeid;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 檢查檢查扣板在整機是否有綁定過
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckBoundedRecord(string SN,  OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                select * from sfcruntime.r_sn_keypart_detail WHERE KEYPART_SN ='{SN}' and VALID=0
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //throw new MESReturnMessage("扣板在整機有綁定過,不能單獨包裝出貨,請聯系TE﹗");
                throw new MESReturnMessage("contect TE﹗");
            }
        }

        public int AddNewControl(string bu, string controlName, string controlValue, string controlType, string controlLevel, string controlDesc, string emp,OleExec DB)
        {
            C_CONTROL control = new C_CONTROL();
            control.ID = this.GetNewID(bu, DB);
            control.CONTROL_NAME = controlName;
            control.CONTROL_VALUE = controlValue;
            control.CONTROL_TYPE = controlType;
            control.CONTROL_LEVEL = controlLevel;
            control.CONTROL_DESC = controlDesc;
            control.EDIT_EMP = emp;
            control.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(control).ExecuteCommand();
        }

        public int DeleteControlByID(string id, OleExec DB)
        {
            return DB.ORM.Deleteable<C_CONTROL>().With(SqlSugar.SqlWith.RowLock).Where(c => c.ID == id).ExecuteCommand();
        }

        /// <summary>
        /// WZW 通過control_name取C_CONTROL表的control_value列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_CONTROL GetControlNAMEDESC(string controlName, string controlDESC, OleExec db)
        {
            string strSql = $@" select * from c_control where control_name in ('{controlName}') and control_DESC in ('{controlDESC}')";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_CONTROL Control = new C_CONTROL();
            Row_C_CONTROL RowCCcontrol = (Row_C_CONTROL)NewRow();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    RowCCcontrol.loadData(dr);
                    Control = RowCCcontrol.GetDataObject();
                }
            }
            else
            {
                Control = null;
            }
            return Control;
        }
        /// <summary>
        /// <summary>
        /// 通過control_name取C_CONTROL表的control_value列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetControlListByName(string controlName, OleExec db)
        {
            string strSql = $@" select * from c_control where control_name='{controlName}'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<string> result = new List<string>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                    ret.loadData(dr);
                    result.Add(ret.CONTROL_VALUE);
                }

            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 通過control_name取C_CONTROL表的control_value列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetControlLevelListByNameAndType(string controlName, string controlType, OleExec db)
        {
            string strSql = $@" select * from c_control where control_name in ('{controlName}') and control_type in ('{controlType}')";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<string> result = new List<string>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                    ret.loadData(dr);
                    result.Add(ret.CONTROL_LEVEL);
                }

            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// wzw 通過control_name controlType取C_CONTROL表的control_value列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetControlListByNameAndType(string controlName, string controlType, OleExec db)
        {
            string strSql = $@" select NVL(CONTROL_VALUE,'') as CONTROL_VALUE from c_control where control_name = ('{controlName}') and control_type = ('{controlType}')";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<string> result = new List<string>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    //Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                    //ret.loadData(dr);
                    result.Add(dr["CONTROL_VALUE"].ToString());
                }
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 通過control_name，control_value取C_CONTROL表的列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="controlValue"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_CONTROL GetControlByNameAndValue(string controlName, string controlValue, OleExec db)
        {
            string strSql = $@" select * from c_control where trim(control_name)='{controlName}' and trim(control_value)='{controlValue}' ";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_CONTROL result = new C_CONTROL();
            if (table.Rows.Count > 0)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        public List<C_CONTROL> GetListByCONTROL(string ControlName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_CONTROL> ListCCONTROL = new List<C_CONTROL>();
            sql = $@"SELECT * FROM C_CONTROL WHERE CONTROL_NAME='QUACK_RECEIVE_EMP'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)NewRow();
                ret.loadData(item);
                ListCCONTROL.Add(ret.GetDataObject());
            }
            return ListCCONTROL;
        }

        /// <summary>
        /// 工治具check管控線體方法:CONTROL_LEVEL='1'有效,control_type='CHECKLINE'管控類型
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetCheckLine(OleExec db)
        {
            string strSql = $@" select distinct  CONTROL_VALUE from c_control where control_type='CHECKLINE' and CONTROL_NAME='CHECKLINE_APCHECK' and CONTROL_LEVEL='1' ";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<string> result = new List<string>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    result.Add(dr["CONTROL_VALUE"].ToString());
                }
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="ControlType"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_CONTROL> GetListByNameType(string ControlName, List<string> ControlType, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == ControlName && ControlType.Contains(t.CONTROL_TYPE)).ToList();
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="ControlType"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_CONTROL> GetListByNameValue(string ControlName, string CONTROL_VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == ControlName && CONTROL_VALUE.Contains(t.CONTROL_VALUE)).ToList();
        }
        public List<C_CONTROL> GetListNameValueByNV(string ControlName, string CONTROL_VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == ControlName && t.CONTROL_VALUE == CONTROL_VALUE).ToList();
        }
        public List<C_CONTROL> GetListByValue(string CONTROL_VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_VALUE == CONTROL_VALUE).Take(0).ToList();
        }
        public List<string> GetListByType(string CONTROLTYPE, string CONTROLNAME, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_TYPE.Contains(CONTROLTYPE) && t.CONTROL_NAME == CONTROLNAME).ToList().Select(t => t.CONTROL_VALUE).ToList();
            //List<string> ListString = new List<string>();
            //List<C_CONTROL> ListCControl = DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME.Contains(CONTROLNAME)).ToList();
            //for (int i = 0; i < ListCControl.Count; i++)
            //{
            //    ListString.Add(ListCControl[i].CONTROL_VALUE);
            //}
            //return ListString;
        }
        public List<C_CONTROL> GetListByRepairType(string CONTROLTYPE, string CONTROLDESC, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_TYPE == CONTROLTYPE && t.CONTROL_DESC == CONTROLDESC).ToList();
        }

        /// <summary>
        /// 查詢該變更工號的配置信息
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_CONTROL> GetbyEditCon(string Editby, OleExec DB)
        {
            List<C_CONTROL> ConDet = new List<C_CONTROL>();
            string sql = string.Empty;
            DataTable dt = new DataTable("GetbyEditCon");
            Row_C_CONTROL ConRow = (Row_C_CONTROL)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM C_CONTROL where EDIT_EMP='{Editby}' ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    ConRow.loadData(dr);
                    ConDet.Add(ConRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return ConDet;
        }

        /// <summary>
        /// Update變更權限所屬工號,A工號配置的sku信息轉移到B工號
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="Editchage"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpConEdit(string Editby, string Editchage, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (Editby.Length > 0)
                {
                    sql = $@" update C_CONTROL set EDIT_EMP='{Editchage}' where EDIT_EMP='{Editby}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        public C_CONTROL GetNameValueTypeBySKU(string CONTROLName, string CONTROLValue, string CONTROLTYPE, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == CONTROLName && t.CONTROL_TYPE == CONTROLTYPE && t.CONTROL_VALUE == CONTROLValue).ToList().FirstOrDefault();
        }

        public int UpdateControlValue(string value, OleExec DB)
        {
            string strSql = $@" update c_control set control_value=:control_value where CONTROL_NAME='BACKFLUSH'";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":control_value", value),
            };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }

        public List<string> GetControlUpperValueListByName(string ControlName, OleExec db)
        {
            string strSql = $@"select NVL(CONTROL_VALUE,'') as CONTROL_VALUE from c_control where control_name = ('{ControlName}') ";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<string> result = new List<string>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    //Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                    //ret.loadData(dr);
                    result.Add(dr["CONTROL_VALUE"].ToString().ToUpper());
                }
            }
            return result;
        }

        public bool NeedCheckSMTFAI(OleExec DB)
        {
            bool NeedCheck = false;
            string SqlStr = $@"select * from C_CONTROL where CONTROL_NAME='CHECKLIST' and CONTROL_VALUE='1' ";
            DataTable res = DB.ExecuteDataTable(SqlStr, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                NeedCheck = true;
            }
            return NeedCheck;
        }
        public List<C_CONTROL> GetTypeNameBYLevel(string Name, string Type, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_CONTROL> ListCCONTROL = new List<C_CONTROL>();
            sql = $@"SELECT * FROM C_CONTROL WHERE CONTROL_NAME = '{Name}' AND CONTROL_TYPE = '{Type}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)NewRow();
                ret.loadData(item);
                ListCCONTROL.Add(ret.GetDataObject());
            }
            return ListCCONTROL;
        }
        public List<string> GetCONTROLBYCONTROL(string CONTROLNAME, string CONTROLDESC, string CONTROLTYPE, string CONTROLLEVEL, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == CONTROLNAME && t.CONTROL_DESC == CONTROLDESC && t.CONTROL_TYPE == CONTROLTYPE && t.CONTROL_LEVEL == CONTROLLEVEL).Select(t => t.CONTROL_VALUE).ToList();
        }
        public List<string> GetBUBYPO(string CONTROLNAME, string CONTROLDESC, string SKUNO, OleExec DB)
        {
            string sql = string.Empty;
            //string Str = null;
            DataTable dt = new DataTable();
            //List<C_CONTROL> ListCCONTROL = new List<C_CONTROL>();
            List<string> ListCCONTROLString = new List<string>();
            sql = $@"SELECT * FROM  C_CONTROL WHERE CONTROL_NAME='{CONTROLNAME}' AND CONTROL_DESC='{CONTROLDESC}' AND CONTROL_TYPE IN (SELECT BU FROM C_SKU WHERE SKUNO='{SKUNO}')";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)NewRow();
                ret.loadData(item);
                ListCCONTROLString.Add(ret.GetDataObject().CONTROL_VALUE);
            }
            //if (dt.Rows.Count > 0)
            //{
            //    Str = dt.Rows[0]["CONTROL_VALUE"].ToString();
            //}
            return ListCCONTROLString;
        }
        public List<C_CONTROL> GetSNBYSNStationDetailControl(string SN, string CURRENTSTATION, OleExec DB)
        {
            string StrSql = string.Empty;
            StrSql = $@"SELECT B.* FROM R_SN_STATION_DETAIL A, C_CONTROL B WHERE A.SN = '{SN}' AND A.STATION_NAME = '{CURRENTSTATION}'   
  AND B.CONTROL_NAME='SKUPACKINGRATE' AND B.CONTROL_VALUE=SUBSTR(A.SKUNO,1,LENGTH(A.SKUNO)-3)  ";
            List<C_CONTROL> ListC_CONTROL = new List<C_CONTROL>();
            C_CONTROL C_CONTROL = null;
            Row_C_CONTROL Rows = (Row_C_CONTROL)this.NewRow();
            DataTable Dt = new DataTable();
            Dt = DB.ExecSelect(StrSql).Tables[0];
            foreach (DataRow r in Dt.Rows)
            {
                Rows.loadData(r);
                C_CONTROL = Rows.GetDataObject();
                ListC_CONTROL.Add(C_CONTROL);
            }
            return ListC_CONTROL;
        }
        public int UpdateControlType(C_CONTROL CControl, OleExec DB)
        {
            return DB.ORM.Updateable<C_CONTROL>(CControl).Where(t => t.CONTROL_NAME == CControl.CONTROL_NAME && t.CONTROL_VALUE == CControl.CONTROL_VALUE).ExecuteCommand();
        }
        /// <summary>
        /// Get Control Object
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="controlName"></param>
        /// <param name="controlType"></param>
        /// <param name="controlValue"></param>
        /// <param name="controlLevel"></param>
        /// <returns></returns>
        public C_CONTROL GetControlObject(OleExec sfcdb, string controlName, string controlType, string controlValue = null, string controlLevel = null)
        {
            return sfcdb.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == controlName && r.CONTROL_TYPE == controlType)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(controlValue), r => r.CONTROL_VALUE == controlValue)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(controlLevel), r => r.CONTROL_LEVEL == controlLevel)
                .ToList().FirstOrDefault();
        }

        public bool BackflushControl(OleExec sfcdb)
        {
            bool isMonthly = false;
            string[] times;
            string sql = string.Empty;          
            C_CONTROL control = GetControlByName("BACKFLUSH", sfcdb);
            if (control != null )
            {
                times = control.CONTROL_VALUE.Split(new char[] { '~' });

                sql = $@"select 1 from dual where sysdate between to_date('{times[0]}' ,'yyyy-mm-dd hh24:mi:ss') and to_date('{times[1]}' ,'yyyy-mm-dd hh24:mi:ss')";
                DataSet temp = sfcdb.RunSelect(sql);
                if (temp.Tables[0].Rows.Count > 0)
                {
                    isMonthly = true;
                }
            }
            return isMonthly;
        }


        public List<C_REWORK_SKU_MAPPING> GetReworkSkuMappings(string SearchValue,OleExec DB)
        {
            List<C_REWORK_SKU_MAPPING> mappings = new List<C_REWORK_SKU_MAPPING>();
            List<C_CONTROL> controls = DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "SKUNO_CAN_REWORK_MAPPING").WhereIF(SearchValue.Length>0,$@"instr(CONTROL_VALUE,'{SearchValue}')>0").ToList();
            foreach (C_CONTROL control in controls)
            {
                string[] skus = control.CONTROL_VALUE.Split(':');
                if (skus.Length == 2)
                {
                    mappings.Add(new C_REWORK_SKU_MAPPING()
                    {
                        ID = control.ID,
                        SKU1=skus[0],
                        SKU2=skus[1],
                        EDIT_EMP=control.EDIT_EMP,
                        EDIT_TIME=control.EDIT_TIME
                    });
                }
                
            }
            return mappings;
        }

        public int DeleteReworkSkuMappingByIds(string Ids, OleExec DB)
        {
            int result = 0;
            List<string> idArray = Ids.Trim('\'').Split(',').ToList();
            foreach (string id in idArray)
            {
                result += DB.ORM.Deleteable<C_CONTROL>().Where(t => t.CONTROL_NAME == "SKUNO_CAN_REWORK_MAPPING" && t.ID == id).ExecuteCommand();
            }
            return result;
        }

        public int SaveReworkSkuMapping(string Sku1, string Sku2, string Bu,string Emp, OleExec DB)
        {

            return DB.ORM.Insertable(new C_CONTROL()
            {
                ID = this.GetNewID(Bu, DB),
                CONTROL_NAME = "SKUNO_CAN_REWORK_MAPPING",
                CONTROL_VALUE = string.Concat(Sku1,":",Sku2),
                CONTROL_TYPE = "SKU",
                CONTROL_LEVEL = "0",
                EDIT_EMP = Emp,
                EDIT_TIME=GetDBDateTime(DB)
            }
            ).ExecuteCommand();
        }

        public int UpdateReworkSkuMappingById(string Id, string Sku1, string Sku2, string Emp, OleExec DB)
        {
            int result = 0;
            Id = Id.Trim('\'');
            C_CONTROL control = DB.ORM.Queryable<C_CONTROL>().Where(t => t.ID == Id).ToList().FirstOrDefault();
            if (control != null)
            {
                control.CONTROL_VALUE= string.Concat(Sku1, ":", Sku2);
                control.EDIT_EMP = Emp;
                control.EDIT_TIME = GetDBDateTime(DB);
                result = DB.ORM.Updateable<C_CONTROL>(control).Where(t => t.ID == Id).ExecuteCommand();
                
            }
            return result;
        }
    }
    public class Row_C_CONTROL : DataObjectBase
    {
        public Row_C_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public C_CONTROL GetDataObject()
        {
            C_CONTROL DataObject = new C_CONTROL();
            DataObject.ID = this.ID;
            DataObject.CONTROL_NAME = this.CONTROL_NAME;
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_LEVEL = this.CONTROL_LEVEL;
            DataObject.CONTROL_DESC = this.CONTROL_DESC;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string CONTROL_NAME
        {
            get
            {
                return (string)this["CONTROL_NAME"];
            }
            set
            {
                this["CONTROL_NAME"] = value;
            }
        }
        public string CONTROL_VALUE
        {
            get
            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_LEVEL
        {
            get
            {
                return (string)this["CONTROL_LEVEL"];
            }
            set
            {
                this["CONTROL_LEVEL"] = value;
            }
        }
        public string CONTROL_DESC
        {
            get
            {
                return (string)this["CONTROL_DESC"];
            }
            set
            {
                this["CONTROL_DESC"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class C_CONTROL
    {
        public string ID{get;set;}
        public string CONTROL_NAME{get;set;}
        public string CONTROL_VALUE{get;set;}
        public string CONTROL_TYPE{get;set;}
        public string CONTROL_LEVEL{get;set;}
        public string CONTROL_DESC{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }

    public class C_REWORK_SKU_MAPPING
    {
        public string ID { get; set; }
        public string SKU1 { get; set; }
        public string SKU2 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}