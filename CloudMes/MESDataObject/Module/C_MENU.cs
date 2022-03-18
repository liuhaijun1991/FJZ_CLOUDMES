using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_MENU : DataObjectTable
    {
        public T_C_MENU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MENU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MENU);
            TableName = "C_MENU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool Check_MENU_NAME(string MENU_NAME, OleExec DB)
        {
            bool flag = true;
            string sql = $@" SELECT * FROM C_MENU where MENU_NAME='{MENU_NAME}' ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                flag = false;
            return flag;
        }

        public double GetmaxSORT(string PARENT_CODE, OleExec DB)
        {
            double sort = 0;
            string sql = $@" SELECT nvl(max(SORT)+10,10) FROM C_MENU where PARENT_CODE='{PARENT_CODE}' ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                sort = Convert.ToDouble(dt.Rows[0][0].ToString());
            return sort;
        }

        public bool Check_PARENT(string id, OleExec DB)
        {
            bool flag = true;
            string sql = $@" SELECT * FROM C_MENU where PARENT_CODE='{id}' ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                flag = false;
            return flag;
        }

        public Row_C_MENU getC_MenubyID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_MENU where ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_MENU ret = (Row_C_MENU)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public DataTable getC_MenubyPARENT_CODE(string PARENT_CODE, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_MENU where PARENT_CODE='{PARENT_CODE}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }

        public List<MENUS> GetMenu(string emp_l,string User_No, MESDBHelper.OleExec SFCDB)
        {
            string strSql = "";
            DataSet AllMenu = null;
            if (emp_l != "9")
            {
                #region sql1
                //strSql = $@" SELECT b.ID,
                //                       b.SYSTEM_NAME,
                //                       b.MENU_NAME,
                //                       b.PAGE_PATH,
                //                       b.PARENT_CODE,
                //                       b.SORT,
                //                       b.STYLE_NAME,
                //                       b.CLASS_NAME,
                //                       b.LANGUAGE_ID,
                //                       b.MENU_DESC
                //                FROM C_USER_PRIVILEGE a, C_menu b, c_user c
                //                WHERE     C.ID = A.USER_ID
                //                          AND A.PRIVILEGE_ID = B.ID
                //                          AND b.PARENT_CODE = '0'
                //                          AND C.EMP_NO = '{User_No}' ORDER BY SORT ";
                strSql = $@" SELECT B.ID,
                                    B.SYSTEM_NAME,
                                    B.MENU_NAME,
                                    B.PAGE_PATH,
                                    B.PARENT_CODE,
                                    B.SORT,
                                    B.STYLE_NAME,
                                    B.CLASS_NAME,
                                    B.LANGUAGE_ID,
                                    B.MENU_DESC
                            FROM C_MENU B
                            WHERE     EXISTS
                                        (SELECT 1
                                            FROM C_PRIVILEGE GH
                                            WHERE     EXISTS
                                                        (SELECT 1
                                                            FROM C_USER_PRIVILEGE A, C_USER C
                                                            WHERE     C.ID = A.USER_ID
                                                                AND C.EMP_NO = '{User_No}'
                                                                AND GH.ID = A.PRIVILEGE_ID
                                                        UNION
                                                        SELECT 1
                                                            FROM C_USER_ROLE D,
                                                                C_USER E,
                                                                C_ROLE_PRIVILEGE F
                                                            WHERE     E.ID = D.USER_ID
                                                                AND F.ROLE_ID = D.ROLE_ID
                                                                AND E.EMP_NO = '{User_No}'
                                                                AND GH.ID = F.PRIVILEGE_ID)
                                                AND GH.MENU_ID = B.ID)
                                    AND B.PARENT_CODE = '0'
                            ORDER BY SORT ";
                #endregion
            }
            else
            { 
                #region sql2
                strSql = @" select ID,
                                       SYSTEM_NAME,
                                       MENU_NAME,
                                       PAGE_PATH,
                                       PARENT_CODE,
                                       SORT,
                                       STYLE_NAME,
                                       CLASS_NAME,
                                       LANGUAGE_ID,
                                       MENU_DESC from c_menu WHERE PARENT_CODE = '0' ORDER BY SORT ";
                #endregion

                AllMenu = SFCDB.ExecSelect(@" select ID,
                                       SYSTEM_NAME,
                                       MENU_NAME,
                                       PAGE_PATH,
                                       PARENT_CODE,
                                       SORT,
                                       STYLE_NAME,
                                       CLASS_NAME,
                                       LANGUAGE_ID,
                                       MENU_DESC from c_menu  ORDER BY SORT ");
            }
            DataSet res = SFCDB.ExecSelect(strSql);
            List<MENUS> Privileges = new List<MENUS>();

            if (res.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in res.Tables[0].Rows)
                {
                    Privileges.Add(new MENUS
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        MENU_NAME = item["MENU_NAME"].ToString(),
                        PAGE_PATH = item["PAGE_PATH"].ToString(),
                        PARENT_CODE = item["PARENT_CODE"].ToString(),
                        SORT = item["SORT"].ToString(),
                        STYLE_NAME = item["STYLE_NAME"].ToString(),
                        CLASS_NAME = item["CLASS_NAME"].ToString(),
                        LANGUAGE_ID = item["LANGUAGE_ID"].ToString(),
                        MENU_DESC = item["MENU_DESC"].ToString(),
                        MENU_ITEM = getmenus(item["ID"].ToString(), User_No, emp_l, SFCDB, AllMenu)
                    });
                }

            }
            return Privileges;
        }

        public List<MENUS> getmenus(string PARENT_CODE, string User_No, string ul, MESDBHelper.OleExec SFCDB ,DataSet menuData = null)
        {
            string sql = "";
            if (ul != "9")
            {
                sql = $@" SELECT B.ID,
                                    B.SYSTEM_NAME,
                                    B.MENU_NAME,
                                    B.PAGE_PATH,
                                    B.PARENT_CODE,
                                    B.SORT,
                                    B.STYLE_NAME,
                                    B.CLASS_NAME,
                                    B.LANGUAGE_ID,
                                    B.MENU_DESC
                            FROM C_MENU B
                            WHERE     EXISTS
                                        (SELECT 1
                                            FROM C_PRIVILEGE GH
                                            WHERE     EXISTS
                                                        (SELECT 1
                                                            FROM C_USER_PRIVILEGE A, C_USER C
                                                            WHERE     C.ID = A.USER_ID
                                                                AND C.EMP_NO = '{User_No}'
                                                                AND GH.ID = A.PRIVILEGE_ID
                                                        UNION
                                                        SELECT 1
                                                            FROM C_USER_ROLE D,
                                                                C_USER E,
                                                                C_ROLE_PRIVILEGE F
                                                            WHERE     E.ID = D.USER_ID
                                                                AND F.ROLE_ID = D.ROLE_ID
                                                                AND E.EMP_NO = '{User_No}'
                                                                AND GH.ID = F.PRIVILEGE_ID)
                                                AND GH.MENU_ID = B.ID)
                                    AND B.PARENT_CODE = '{PARENT_CODE}'
                            ORDER BY SORT ";
            }
            else
            {
                sql = $@" SELECT ID,
                                   SYSTEM_NAME,
                                   MENU_NAME,
                                   PAGE_PATH,
                                   PARENT_CODE,
                                   SORT,
                                   STYLE_NAME,
                                   CLASS_NAME,
                                   LANGUAGE_ID,
                                   MENU_DESC
                              FROM c_menu
                             WHERE PARENT_CODE = '{PARENT_CODE}' ORDER BY SORT ";
            }
            DataSet res = null;

            if (menuData == null)
            {
                res = SFCDB.ExecSelect(sql);
            }
            else
            {
                var data1 = menuData.Copy();
                data1.Tables[0].Clear();
                var rows = menuData.Tables[0].Select($@"PARENT_CODE = '{PARENT_CODE}'");
                for (int i = 0; i < rows.Length; i++)
                {
                    var r = data1.Tables[0].NewRow();
                    data1.Tables[0].Rows.Add(r);
                    r["ID"] = rows[i]["ID"];
                    r["SYSTEM_NAME"] = rows[i]["SYSTEM_NAME"];
                    r["MENU_NAME"] = rows[i]["MENU_NAME"];
                    r["PAGE_PATH"] = rows[i]["PAGE_PATH"];
                    r["PARENT_CODE"] = rows[i]["PARENT_CODE"];
                    r["SORT"] = rows[i]["SORT"];
                    r["STYLE_NAME"] = rows[i]["STYLE_NAME"];
                    r["CLASS_NAME"] = rows[i]["CLASS_NAME"];
                    r["LANGUAGE_ID"] = rows[i]["LANGUAGE_ID"];
                    r["MENU_DESC"] = rows[i]["MENU_DESC"];
                }
                res = data1;
            }
            List<MENUS> pi = new List<MENUS>();
            if (res.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in res.Tables[0].Rows)
                {
                    pi.Add(new MENUS
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        MENU_NAME = item["MENU_NAME"].ToString(),
                        PAGE_PATH = item["PAGE_PATH"].ToString(),
                        PARENT_CODE = item["PARENT_CODE"].ToString(),
                        SORT = item["SORT"].ToString(),
                        STYLE_NAME = item["STYLE_NAME"].ToString(),
                        CLASS_NAME = item["CLASS_NAME"].ToString(),
                        LANGUAGE_ID = item["LANGUAGE_ID"].ToString(),
                        MENU_DESC = item["MENU_DESC"].ToString(),
                        MENU_ITEM = getmenus(item["ID"].ToString(), User_No, ul, SFCDB,menuData)
                    });
                }
            }
            return pi;
        }


        public List<MENUS> GetMenuNextID(string ColumnName, string Input, MESDBHelper.OleExec SFCDB)
        {
            string strSql = "";
           
            strSql = $@" select ID,
                                       SYSTEM_NAME,
                                       MENU_NAME,
                                       PAGE_PATH,
                                       PARENT_CODE,
                                       SORT,
                                       STYLE_NAME,
                                       CLASS_NAME,
                                       LANGUAGE_ID,
                                       MENU_DESC from c_menu WHERE {ColumnName} = '{Input}' ORDER BY SORT ";

            DataSet res = SFCDB.ExecSelect(strSql);
            List<MENUS> Privileges = new List<MENUS>();

            if (res.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in res.Tables[0].Rows)
                {
                    Privileges.Add(new MENUS
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        MENU_NAME = item["MENU_NAME"].ToString(),
                        PAGE_PATH = item["PAGE_PATH"].ToString(),
                        PARENT_CODE = item["PARENT_CODE"].ToString(),
                        SORT = item["SORT"].ToString(),
                        STYLE_NAME = item["STYLE_NAME"].ToString(),
                        CLASS_NAME = item["CLASS_NAME"].ToString(),
                        LANGUAGE_ID = item["LANGUAGE_ID"].ToString(),
                        MENU_DESC = item["MENU_DESC"].ToString(),
                        MENU_ITEM = GetMenuNextID(ColumnName, item["ID"].ToString(), SFCDB)
                    });
                }

            }
            return Privileges;
        }

        public Row_C_MENU getC_MenubyIDandPARENT(string id, string parent, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_MENU where PARENT_CODE='{parent}' and ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_MENU ret = (Row_C_MENU)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public MenuInformation getC_Menu(string id, OleExec DB)
        {
            MenuInformation m = new MenuInformation();
            string strSql = $@" SELECT * FROM C_MENU where ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                m.ID = res.Tables[0].Rows[0]["ID"].ToString();
                m.SYSTEM_NAME = res.Tables[0].Rows[0]["SYSTEM_NAME"].ToString();
                m.MENU_NAME = res.Tables[0].Rows[0]["MENU_NAME"].ToString();
                m.PAGE_PATH = res.Tables[0].Rows[0]["PAGE_PATH"].ToString();
                m.PARENT_CODE = res.Tables[0].Rows[0]["PARENT_CODE"].ToString();
                m.SORT = res.Tables[0].Rows[0]["SORT"].ToString();
                m.STYLE_NAME = res.Tables[0].Rows[0]["STYLE_NAME"].ToString();
                m.CLASS_NAME = res.Tables[0].Rows[0]["CLASS_NAME"].ToString();
                m.LANGUAGE_ID = res.Tables[0].Rows[0]["LANGUAGE_ID"].ToString();
                m.MENU_DESC = res.Tables[0].Rows[0]["MENU_DESC"].ToString();
                m.EDIT_TIME = res.Tables[0].Rows[0]["EDIT_TIME"].ToString();
                m.EDIT_EMP = res.Tables[0].Rows[0]["EDIT_EMP"].ToString();
                return m;
            }
            else
            {
                return null;
            }
        }
    }
    public class Row_C_MENU : DataObjectBase
    {
        public Row_C_MENU(DataObjectInfo info) : base(info)
        {

        }
        public C_MENU GetDataObject()
        {
            C_MENU DataObject = new C_MENU();
            DataObject.SON_CODE = this.SON_CODE;
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.MENU_NAME = this.MENU_NAME;
            DataObject.PAGE_PATH = this.PAGE_PATH;
            DataObject.PARENT_CODE = this.PARENT_CODE;
            DataObject.SORT = this.SORT;
            DataObject.STYLE_NAME = this.STYLE_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.LANGUAGE_ID = this.LANGUAGE_ID;
            DataObject.MENU_DESC = this.MENU_DESC;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string SON_CODE
        {
            get
            {
                return (string)this["SON_CODE"];
            }
            set
            {
                this["SON_CODE"] = value;
            }
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
        public string SYSTEM_NAME
        {
            get
            {
                return (string)this["SYSTEM_NAME"];
            }
            set
            {
                this["SYSTEM_NAME"] = value;
            }
        }
        public string MENU_NAME
        {
            get
            {
                return (string)this["MENU_NAME"];
            }
            set
            {
                this["MENU_NAME"] = value;
            }
        }
        public string PAGE_PATH
        {
            get
            {
                return (string)this["PAGE_PATH"];
            }
            set
            {
                this["PAGE_PATH"] = value;
            }
        }
        public string PARENT_CODE
        {
            get
            {
                return (string)this["PARENT_CODE"];
            }
            set
            {
                this["PARENT_CODE"] = value;
            }
        }
        public double? SORT
        {
            get
            {
                return (double?)this["SORT"];
            }
            set
            {
                this["SORT"] = value;
            }
        }
        public string STYLE_NAME
        {
            get
            {
                return (string)this["STYLE_NAME"];
            }
            set
            {
                this["STYLE_NAME"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string LANGUAGE_ID
        {
            get
            {
                return (string)this["LANGUAGE_ID"];
            }
            set
            {
                this["LANGUAGE_ID"] = value;
            }
        }
        public string MENU_DESC
        {
            get
            {
                return (string)this["MENU_DESC"];
            }
            set
            {
                this["MENU_DESC"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
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
        public class C_MENU
        {
            public string SON_CODE;
            public string ID;
            public string SYSTEM_NAME;
            public string MENU_NAME;
            public string PAGE_PATH;
            public string PARENT_CODE;
            public double? SORT;
            public string STYLE_NAME;
            public string CLASS_NAME;
            public string LANGUAGE_ID;
            public string MENU_DESC;
            public DateTime EDIT_TIME;
            public string EDIT_EMP;
        }
    }
    public class MENUS
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string MENU_NAME { get; set; }
        public string PAGE_PATH { get; set; }
        public string PARENT_CODE { get; set; }
        public string SORT { get; set; }
        public string STYLE_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string LANGUAGE_ID { get; set; }
        public string MENU_DESC { get; set; }
        public List<MENUS> MENU_ITEM { get; set; }
    }

    public class C_MENU
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string MENU_NAME { get; set; }
        public string PAGE_PATH { get; set; }
        public string PARENT_CODE { get; set; }
        public string SORT { get; set; }
        public string STYLE_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string LANGUAGE_ID { get; set; }
        public string MENU_DESC { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }

    }

    public class MenuInformation
    {
        public string ID{get;set;}
        public string SYSTEM_NAME{get;set;}
        public string MENU_NAME{get;set;}
        public string PAGE_PATH{get;set;}
        public string PARENT_CODE{get;set;}
        public string SORT{get;set;}
        public string STYLE_NAME{get;set;}
        public string CLASS_NAME{get;set;}
        public string LANGUAGE_ID{get;set;}
        public string MENU_DESC{get;set;}
        public string EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}