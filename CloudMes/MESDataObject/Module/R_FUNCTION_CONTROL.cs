using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_FUNCTION_CONTROL : DataObjectTable
    {
        public T_R_FUNCTION_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FUNCTION_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FUNCTION_CONTROL);
            TableName = "R_FUNCTION_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_F_CONTROL> GetSystemListbyFuncCate (string FUNCTIONNAME,string CATEGORY, OleExec DB) {

            return DB.ORM.Queryable<R_F_CONTROL>().Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY== CATEGORY && it.CONTROLFLAG =="Y" && it.FUNCTIONTYPE=="SYSTEM").ToList();
        }
        public bool CheckSystemFunctionExist(string FUNCTIONNAME, string CATEGORY, OleExec DB)
        {
            
            return DB.ORM.Queryable<R_F_CONTROL>()
                 .Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY == CATEGORY && it.FUNCTIONTYPE == "SYSTEM" && it.CONTROLFLAG == "Y")
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                 .Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY == CATEGORY && it.VALUE == VALUE && it.FUNCTIONTYPE == "NOSYSTEM" && it.CONTROLFLAG == "Y")
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1,OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                 .Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY == CATEGORY &&it.VALUE== VALUE &&it.EXTVAL== EXTVAL1 && it.FUNCTIONTYPE == "NOSYSTEM" && it.CONTROLFLAG == "Y")
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, string EXTVAL2,OleExec DB)
        {
            
            return DB.ORM.Queryable<R_F_CONTROL,R_F_CONTROL_EX>((a,b)=>a.ID==b.DETAIL_ID)
                 .Where((a,b) => a.FUNCTIONNAME == FUNCTIONNAME 
                 && a.CATEGORY == CATEGORY 
                 && a.VALUE == VALUE 
                 && a.EXTVAL == EXTVAL1 
                 && a.FUNCTIONTYPE == "NOSYSTEM" 
                 && a.CONTROLFLAG == "Y"
                 && b.VALUE== EXTVAL2 && b.SEQ_NO==1)
                 .Select((a)=>a.ID)
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, string EXTVAL2, string EXTVAL3, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX, R_F_CONTROL_EX>
                ((a,b,c) => a.ID == b.DETAIL_ID &&a.ID==c.DETAIL_ID)
                 .Where((a,b,c) => a.FUNCTIONNAME == FUNCTIONNAME
                 && a.CATEGORY == CATEGORY
                 && a.VALUE == VALUE
                 && a.EXTVAL == EXTVAL1
                 && a.FUNCTIONTYPE == "NOSYSTEM"
                 && a.CONTROLFLAG == "Y"
                 && b.VALUE == EXTVAL2 && b.SEQ_NO == 1
                 && c.VALUE == EXTVAL3 && c.SEQ_NO == 2)
                 .Select((a) => a.ID)
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, string EXTVAL2, string EXTVAL3, string EXTVAL4, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX>
                ((a,b,c,d) => a.ID == b.DETAIL_ID && a.ID == c.DETAIL_ID&&a.ID==d.DETAIL_ID)
                 .Where((a,b,c,d) => a.FUNCTIONNAME == FUNCTIONNAME
                 && a.CATEGORY == CATEGORY
                 && a.VALUE == VALUE
                 && a.EXTVAL == EXTVAL1
                 && a.FUNCTIONTYPE == "NOSYSTEM"
                 && a.CONTROLFLAG == "Y"
                 && b.VALUE == EXTVAL2 && b.SEQ_NO == 1
                 && c.VALUE == EXTVAL3 && c.SEQ_NO == 2
                 && d.VALUE == EXTVAL4 && d.SEQ_NO == 3)
                 .Select((a) => a.ID)
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, string EXTVAL2, string EXTVAL3, string EXTVAL4, string EXTVAL5, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX>
                ((a, b, c, d,e) => a.ID == b.DETAIL_ID && a.ID == c.DETAIL_ID && a.ID == d.DETAIL_ID&&a.ID==e.DETAIL_ID)
                 .Where((a, b, c, d,e) => a.FUNCTIONNAME == FUNCTIONNAME
                 && a.CATEGORY == CATEGORY
                 && a.VALUE == VALUE
                 && a.EXTVAL == EXTVAL1
                 && a.FUNCTIONTYPE == "NOSYSTEM"
                 && a.CONTROLFLAG == "Y"
                 && b.VALUE == EXTVAL2 && b.SEQ_NO == 1
                 && c.VALUE == EXTVAL3 && c.SEQ_NO == 2
                 && d.VALUE == EXTVAL4 && d.SEQ_NO == 3
                 && e.VALUE == EXTVAL5 && e.SEQ_NO == 4)
                 .Select((a) => a.ID)
                 .Any();
        }
        public bool CheckUserFunctionExist(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, string EXTVAL2, string EXTVAL3, string EXTVAL4, string EXTVAL5, string EXTVAL6, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX, R_F_CONTROL_EX>
                ((a, b, c, d, e,f) => a.ID == b.DETAIL_ID && a.ID == c.DETAIL_ID && a.ID == d.DETAIL_ID && a.ID == e.DETAIL_ID&&a.ID==f.DETAIL_ID)
                 .Where((a, b, c, d, e,f) => a.FUNCTIONNAME == FUNCTIONNAME
                 && a.CATEGORY == CATEGORY
                 && a.VALUE == VALUE
                 && a.EXTVAL == EXTVAL1
                 && a.FUNCTIONTYPE == "NOSYSTEM"
                 && a.CONTROLFLAG == "Y"
                 && b.VALUE == EXTVAL2 && b.SEQ_NO == 1
                 && c.VALUE == EXTVAL3 && c.SEQ_NO == 2
                 && d.VALUE == EXTVAL4 && d.SEQ_NO == 3
                 && e.VALUE == EXTVAL5 && e.SEQ_NO == 4
                 && f.VALUE == EXTVAL6 && f.SEQ_NO == 5)
                 .Select((a) => a.ID)
                 .Any();
        }
        public List<R_F_CONTROL> GetListByFcv(string FUNCTIONNAME,OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL>().Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CONTROLFLAG == "Y" && it.FUNCTIONTYPE == "NOSYSTEM").ToList();
        }
        public List<R_F_CONTROL> GetListByFcv(string FUNCTIONNAME, string CATEGORY, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL>().Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY == CATEGORY  && it.CONTROLFLAG == "Y" && it.FUNCTIONTYPE == "NOSYSTEM").ToList();
        }
        public List<R_F_CONTROL> GetListByFcv(string FUNCTIONNAME, string CATEGORY,string VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL>().Where(it => it.FUNCTIONNAME == FUNCTIONNAME && it.CATEGORY == CATEGORY && it.VALUE == VALUE && it.CONTROLFLAG=="Y" && it.FUNCTIONTYPE == "NOSYSTEM").ToList();
        }
        public List<R_F_CONTROL> GetListByFcv(string FUNCTIONNAME, string CATEGORY, string VALUE,string EXTVAL1,OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL>().Where(it => it.FUNCTIONNAME == FUNCTIONNAME && 
                   it.CATEGORY == CATEGORY && it.VALUE == VALUE && it.EXTVAL==EXTVAL1 && it.CONTROLFLAG == "Y" && it.FUNCTIONTYPE == "NOSYSTEM").ToList();
        }
        public List<R_FUNCTION_CONTROL_NewList> Get1ExListbyVarValue(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX>
                 ((A, B) => A.ID == B.DETAIL_ID
                       && A.FUNCTIONNAME == FUNCTIONNAME
                       && A.CATEGORY == CATEGORY
                       && A.VALUE == VALUE
                       && A.EXTVAL == EXTVAL1
                       && A.CONTROLFLAG == "Y"
                       && A.FUNCTIONTYPE == "NOSYSTEM"
                       && B.SEQ_NO == 1)
                 .Select((A, B) => new R_FUNCTION_CONTROL_NewList
                 {
                     ID = A.ID,
                     FUNCTIONNAME = A.FUNCTIONNAME,
                     CATEGORY = A.CATEGORY,
                     VALUE = A.VALUE,
                     EXTVAL1 = A.EXTVAL,
                     EXTVAL2 = B.VALUE,
                     CREATETIME = A.CREATETIME,
                     CREATEBY = A.CREATEBY,
                     EDITTIME = A.EDITTIME,
                     EDITBY = A.EDITBY
                 }
                 )
                 .ToList();
        }
        public List<R_FUNCTION_CONTROL_NewList> Get2ExListbyVarValue(string FUNCTIONNAME, string CATEGORY, string VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX>
                ((A, B) => A.ID == B.DETAIL_ID
                      && A.FUNCTIONNAME == FUNCTIONNAME
                      && A.CATEGORY == CATEGORY
                      && A.VALUE == VALUE
                      && A.CONTROLFLAG == "Y"
                      && A.FUNCTIONTYPE == "NOSYSTEM"
                      && B.SEQ_NO == 1)
                .Select((A, B) => new R_FUNCTION_CONTROL_NewList
                {   ID=A.ID,
                    FUNCTIONNAME = A.FUNCTIONNAME,
                    CATEGORY = A.CATEGORY,
                    VALUE = A.VALUE,
                    EXTVAL1 = A.EXTVAL,
                    EXTVAL2 = B.VALUE,
                    CREATETIME = A.CREATETIME,
                    CREATEBY = A.CREATEBY,
                    EDITTIME = A.EDITTIME,
                    EDITBY = A.EDITBY
                }
                )
                .ToList();
        }
        public List<R_FUNCTION_CONTROL_NewList> Get2ExListbyVarValue(string FUNCTIONNAME, string CATEGORY, string VALUE, string EXTVAL1,string EXTVAL2, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL,R_F_CONTROL_EX>
                ((A,B)=> A.ID == B.DETAIL_ID
                      && A.FUNCTIONNAME == FUNCTIONNAME 
                      && A.CATEGORY == CATEGORY 
                      && A.VALUE == VALUE 
                      && A.EXTVAL == EXTVAL1 
                      && A.CONTROLFLAG == "Y" 
                      && A.FUNCTIONTYPE == "NOSYSTEM"
                      && B.VALUE == EXTVAL2
                      && B.SEQ_NO==1)
                .Select((A,B)=>new R_FUNCTION_CONTROL_NewList
                {
                    ID = A.ID,
                    FUNCTIONNAME =A.FUNCTIONNAME,
                    CATEGORY=A.CATEGORY,
                    VALUE=A.VALUE,
                    EXTVAL1=A.EXTVAL,
                    EXTVAL2=B.VALUE,
                    CREATETIME=A.CREATETIME,
                    CREATEBY=A.CREATEBY,
                    EDITTIME=A.EDITTIME,
                    EDITBY=A.EDITBY
                }
                )
                .ToList();
        }
        public List<R_FUNCTION_CONTROL_NewList> Get3ExListbyVarValue(string FUNCTIONNAME, string CATEGORY, string VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<R_F_CONTROL, R_F_CONTROL_EX, R_F_CONTROL_EX>
                 ((A,B,C) => A.ID == B.DETAIL_ID
                       && A.ID == C.DETAIL_ID
                       && B.SEQ_NO == 1
                       && C.SEQ_NO == 2
                       && A.FUNCTIONNAME == FUNCTIONNAME
                       && A.CATEGORY == CATEGORY
                       && A.VALUE == VALUE
                       && A.CONTROLFLAG == "Y"
                       && A.FUNCTIONTYPE == "NOSYSTEM"
                       )
                 .Select((A,B,C) => new R_FUNCTION_CONTROL_NewList
                 {
                     ID = A.ID,
                     FUNCTIONNAME = A.FUNCTIONNAME,
                     CATEGORY = A.CATEGORY,
                     VALUE = A.VALUE,
                     EXTVAL1 = A.EXTVAL,
                     EXTVAL2 = B.VALUE,
                     EXTVAL3 = C.VALUE,
                     CREATETIME = A.CREATETIME,
                     CREATEBY = A.CREATEBY,
                     EDITTIME = A.EDITTIME,
                     EDITBY = A.EDITBY
                 }
                 )
                 .ToList();
        }
        public object GetSystemConfigMenuList(OleExec DB)
        {
            DataTable Dt = new DataTable();

            string strsql = $@"select a.id,a.functionname,a.functiondec,a.category,a.categorydec,a.usercontrol,a.value,a.extval EXTVAL1,
                                b.value EXTVAL2,c.value EXTVAL3,d.value EXTVAL4,e.value EXTVAL5,f.value EXTVAL6,
                                a.editby,a.edittime
                                from R_FUNCTION_CONTROL a
                                left join R_FUNCTION_CONTROL_EX b on a.id=b.DETAIL_ID and b.seq_no=1
                                left join R_FUNCTION_CONTROL_EX c on a.id=c.detail_id and c.seq_no=2
                                left join R_FUNCTION_CONTROL_EX d on a.id=d.detail_id and d.seq_no=3
                                left join R_FUNCTION_CONTROL_EX e on a.id=e.detail_id and e.seq_no=4
                                left join R_FUNCTION_CONTROL_EX f on a.id=f.detail_id and f.seq_no=5
                                where a.controlflag='Y'
                                and a.functiontype='SYSTEM'
                                ORDER BY a.EDITTIME desc";

            DataTable dt = null;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(strsql).Tables[0];

                var rows = dt.AsEnumerable().Select(entity => new
                {
                    ID = entity["ID"].ToString(),
                    FUNCTIONNAME = entity["FUNCTIONNAME"].ToString(),
                    FUNCTIONDEC = entity["FUNCTIONDEC"].ToString(),
                    CATEGORY = entity["CATEGORY"].ToString(),
                    CATEGORYDEC = entity["CATEGORYDEC"].ToString(),
                    USERCONTROL = entity["USERCONTROL"].ToString(),
                    VALUE = entity["VALUE"].ToString(),
                    EXTVAL1 = entity["EXTVAL1"].ToString(),
                    EXTVAL2 = entity["EXTVAL2"].ToString(),
                    EXTVAL3 = entity["EXTVAL3"].ToString(),
                    EXTVAL4 = entity["EXTVAL4"].ToString(),
                    EXTVAL5 = entity["EXTVAL5"].ToString(),
                    EXTVAL6 = entity["EXTVAL6"].ToString(),
                    EDITBY = entity["EDITBY"].ToString(),
                    EDITTIME = entity["EDITTIME"].ToString()

                });

                return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }
        public object GetUserConfigMenuList(OleExec DB)
        {
            DataTable Dt = new DataTable();

            string strsql = $@"select a.id,a.functionname,a.functiondec,a.category,a.categorydec,a.value,a.extval EXTVAL1,
                                b.value EXTVAL2,c.value EXTVAL3,d.value EXTVAL4,e.value EXTVAL5,f.value EXTVAL6,
                                a.editby,a.edittime
                                from R_FUNCTION_CONTROL a
                                left join R_FUNCTION_CONTROL_EX b on a.id=b.DETAIL_ID and b.seq_no=1
                                left join R_FUNCTION_CONTROL_EX c on a.id=c.detail_id and c.seq_no=2
                                left join R_FUNCTION_CONTROL_EX d on a.id=d.detail_id and d.seq_no=3
                                left join R_FUNCTION_CONTROL_EX e on a.id=e.detail_id and e.seq_no=4
                                left join R_FUNCTION_CONTROL_EX f on a.id=f.detail_id and f.seq_no=5
                                where a.controlflag='Y'
                                and a.functiontype='NOSYSTEM'
                                ORDER BY a.EDITTIME desc";

            DataTable dt = null;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(strsql).Tables[0];

                var rows = dt.AsEnumerable().Select(entity => new
                {
                    ID = entity["ID"].ToString(),
                    FUNCTIONNAME = entity["FUNCTIONNAME"].ToString(),
                    CATEGORY = entity["CATEGORY"].ToString(),
                    VALUE = entity["VALUE"].ToString(),
                    EXTVAL1 = entity["EXTVAL1"].ToString(),
                    EXTVAL2 = entity["EXTVAL2"].ToString(),
                    EXTVAL3 = entity["EXTVAL3"].ToString(),
                    EXTVAL4 = entity["EXTVAL4"].ToString(),
                    EXTVAL5 = entity["EXTVAL5"].ToString(),
                    EXTVAL6 = entity["EXTVAL6"].ToString(),
                    EDITBY = entity["EDITBY"].ToString(),
                    EDITTIME = entity["EDITTIME"].ToString()

                });

                return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }
        public List<string> GetUserFUNCTIONNAME(OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                .Where(it => it.FUNCTIONTYPE == "SYSTEM"&&it.CONTROLFLAG=="Y" && it.USERCONTROL=="Y")
                 .GroupBy(it => it.FUNCTIONNAME)
                 .OrderBy(it => it.FUNCTIONNAME, SqlSugar.OrderByType.Asc)
                 .Select(it => it.FUNCTIONNAME)
                 .With(SqlSugar.SqlWith.NoLock)
                .ToList();
        }
        public List<string> GetAllFUNCTIONNAME(OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                .Where(it => it.FUNCTIONTYPE == "SYSTEM" && it.CONTROLFLAG == "Y")
                 .GroupBy(it => it.FUNCTIONNAME)
                 .OrderBy(it => it.FUNCTIONNAME, SqlSugar.OrderByType.Asc)
                 .Select(it => it.FUNCTIONNAME)
                 .With(SqlSugar.SqlWith.NoLock)
                .ToList();
        }
        public object GetSettingValueByFUNCTIONNAME(string FUNCTIONNAME, string CATEGORY, OleExec DB)
        {

            DataTable Dt = new DataTable();

            string strsql = $@"SELECT A.ID,A.VALUE,A.EXTVAL EXTVAL1 ,B.VALUE EXTVAL2,C.VALUE EXTVAL3,D.VALUE EXTVAL4,E.VALUE EXTVAL5,F.VALUE EXTVAL6
                                FROM R_FUNCTION_CONTROL A 
                                LEFT JOIN R_FUNCTION_CONTROL_EX B on A.id=b.DETAIL_ID and B.SEQ_NO=1
                                LEFT JOIN R_FUNCTION_CONTROL_EX C on A.id=c.DETAIL_ID and C.SEQ_NO=2
                                LEFT JOIN R_FUNCTION_CONTROL_EX D on A.id=d.DETAIL_ID and D.SEQ_NO=3
                                LEFT JOIN R_FUNCTION_CONTROL_EX E on A.id=e.DETAIL_ID and E.SEQ_NO=4
                                LEFT JOIN R_FUNCTION_CONTROL_EX F on A.id=f.DETAIL_ID and F.SEQ_NO=5
                                WHERE A.FUNCTIONTYPE='SYSTEM'
                                AND A.CONTROLFLAG='Y' 
                                AND A.FUNCTIONNAME='{FUNCTIONNAME}' 
                                AND A.CATEGORY='{CATEGORY}'";

            DataTable dt = null;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(strsql).Tables[0];

                var rows = dt.AsEnumerable().Select(entity => new
                {
                    ID = entity["ID"].ToString(),
                    VALUE = entity["VALUE"].ToString(),
                    EXTVAL1 = entity["EXTVAL1"].ToString(),
                    EXTVAL2 = entity["EXTVAL2"].ToString(),
                    EXTVAL3 = entity["EXTVAL3"].ToString(),
                    EXTVAL4 = entity["EXTVAL4"].ToString(),
                    EXTVAL5 = entity["EXTVAL5"].ToString(),
                    EXTVAL6 = entity["EXTVAL6"].ToString(),
                });

                return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }
        public List<string> GetUserCATEGORY(string FUNCTIONNAME,OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                .Where(it => it.FUNCTIONTYPE == "SYSTEM" && it.CONTROLFLAG == "Y" && it.USERCONTROL == "Y" &&it.FUNCTIONNAME== FUNCTIONNAME)
                 .GroupBy(it => it.CATEGORY)
                 .OrderBy(it => it.CATEGORY, SqlSugar.OrderByType.Asc)
                 .Select(it => it.CATEGORY)
                 .With(SqlSugar.SqlWith.NoLock)
                .ToList();
        }
        public List<string> GetAllCATEGORY(string FUNCTIONNAME, OleExec DB)
        {

            return DB.ORM.Queryable<R_F_CONTROL>()
                .Where(it => it.FUNCTIONTYPE == "SYSTEM" && it.CONTROLFLAG == "Y" && it.FUNCTIONNAME == FUNCTIONNAME)
                 .GroupBy(it => it.CATEGORY)
                 .OrderBy(it => it.CATEGORY, SqlSugar.OrderByType.Asc)
                 .Select(it => it.CATEGORY)
                 .With(SqlSugar.SqlWith.NoLock)
                .ToList();
        }
        public int DeleteFunctionControl(string RFCId, string EDIT_EMP, DateTime EDIT_TIME, OleExec DB)
        {
            return DB.ORM.Updateable<R_F_CONTROL>().UpdateColumns(sd => new R_F_CONTROL { CONTROLFLAG = "N", EDITBY = EDIT_EMP, EDITTIME = EDIT_TIME }).Where(sd => sd.ID == RFCId).ExecuteCommand();
        }
        public R_F_CONTROL GET_byID(string id, OleExec DB)
        {
            List<R_F_CONTROL> Sds = DB.ORM.Queryable<R_F_CONTROL>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }
        public int AddOrUpdateRFC(string Operation, R_F_CONTROL RFC, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<R_F_CONTROL>(RFC).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<R_F_CONTROL>(RFC).Where(sd => sd.ID == RFC.ID).ExecuteCommand();
                    break;
            }
            return result;
        }
        public R_F_CONTROL GetControl(OleExec DB, string functionname, string category,string value)
        {
            return DB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == functionname && r.CATEGORY == category && r.VALUE == value && r.CONTROLFLAG == "Y").ToList().FirstOrDefault();
        }
    }
    public class Row_R_FUNCTION_CONTROL : DataObjectBase
    {
        public Row_R_FUNCTION_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public R_F_CONTROL GetDataObject()
        {
            R_F_CONTROL DataObject = new R_F_CONTROL();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.FUNCTIONDEC = this.FUNCTIONDEC;
            DataObject.CATEGORYDEC = this.CATEGORYDEC;
            DataObject.VALUE = this.VALUE;
            DataObject.EXTVAL = this.EXTVAL;
            DataObject.CONTROLFLAG = this.CONTROLFLAG;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.FUNCTIONTYPE = this.FUNCTIONTYPE;
            DataObject.USERCONTROL = this.USERCONTROL;
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
        public string FUNCTIONNAME
        {
            get
            {
                return (string)this["FUNCTIONNAME"];
            }
            set
            {
                this["FUNCTIONNAME"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string FUNCTIONDEC
        {
            get
            {
                return (string)this["FUNCTIONDEC"];
            }
            set
            {
                this["FUNCTIONDEC"] = value;
            }
        }
        public string CATEGORYDEC
        {
            get
            {
                return (string)this["CATEGORYDEC"];
            }
            set
            {
                this["CATEGORYDEC"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string EXTVAL
        {
            get
            {
                return (string)this["EXTVAL"];
            }
            set
            {
                this["EXTVAL"] = value;
            }
        }
        public string CONTROLFLAG
        {
            get
            {
                return (string)this["CONTROLFLAG"];
            }
            set
            {
                this["CONTROLFLAG"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public string FUNCTIONTYPE
        {
            get
            {
                return (string)this["FUNCTIONTYPE"];
            }
            set
            {
                this["FUNCTIONTYPE"] = value;
            }
        }
        public string USERCONTROL
        {
            get
            {
                return (string)this["USERCONTROL"];
            }
            set
            {
                this["USERCONTROL"] = value;
            }
        }
    }
    [SugarTable("R_FUNCTION_CONTROL")]
    public class R_F_CONTROL
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string CATEGORY { get; set; }
        public string FUNCTIONDEC { get; set; }
        public string CATEGORYDEC { get; set; }
        public string VALUE { get; set; }
        public string EXTVAL { get; set; }
        public string CONTROLFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public string FUNCTIONTYPE { get; set; }
        public string USERCONTROL { get; set; }


    }
    public class R_FUNCTION_CONTROL_NewList
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string CATEGORY { get; set; }
        public string VALUE { get; set; }
        public string EXTVAL1 { get; set; }
        public string EXTVAL2 { get; set; }
        public string EXTVAL3 { get; set; }
        public string EXTVAL4 { get; set; }
        public string EXTVAL5 { get; set; }
        public string EXTVAL6 { get; set; }
        public string CONTROLFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public string FUNCTIONTYPE { get; set; }
        public string USERCONTROL { get; set; }

    }

    public enum ENUM_R_F_CONTROL
    {
        /// <summary>
        /// [EnumValue("NOSYSTEM")]
        /// </summary>
        [EnumValue("NOSYSTEM")]
        FUNCTIONTYPE_NOSYSTEM,

    }


}