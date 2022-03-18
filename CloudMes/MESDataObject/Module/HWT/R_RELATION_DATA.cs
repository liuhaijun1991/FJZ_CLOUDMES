using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_RELATION_DATA : DataObjectTable
    {
        public T_R_RELATION_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RELATION_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RELATION_DATA);
            TableName = "R_RELATION_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool IsExist(string SN, string CATEGORYNAME, OleExec DB)
        {
            if (CATEGORYNAME.Length > 0)
            {
                return DB.ORM.Queryable<R_RELATION_DATA>().Any(t => t.SN == SN && t.CATEGORYNAME == CATEGORYNAME);
            }
            else
            {
                return DB.ORM.Queryable<R_RELATION_DATA>().Any(t => t.SN == SN);
            }
        }

        public R_RELATION_DATA LoadData(string SN, string CATEGORYNAME, OleExec DB)
        {
            if (CATEGORYNAME.Length > 0)
            {
                return DB.ORM.Queryable<R_RELATION_DATA>().Where(t => t.SN == SN && t.CATEGORYNAME == CATEGORYNAME).ToList().FirstOrDefault();
            }
            else
            {
                return DB.ORM.Queryable<R_RELATION_DATA>().Where(t => t.SN == SN).ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// HWT ASSY類生成父項綁定關係ACTION(R_RELATION_DATA)
        /// add by hgb 2019.08.27
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        public void HWTMakeParentRelation(string SN, string SKUNO, string STATION, OleExec DB)
        {
            string value = string.Empty;
            string categoryname = string.Empty;
            string parent = string.Empty;
            string son = string.Empty;
            string Strsql = string.Empty;
            T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_SN t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN r_sn = new R_SN();
            List<C_SKU_DETAIL> c_sku_detail = t_c_sku_detail.LoadListData(SKUNO, "RELATION", "", DB);

            #region 獲取父子項配置信息
            if (c_sku_detail.Count == 0)
            {
                throw new MESReturnMessage($@"機種父項關係未設定，請確認！{SKUNO}");
            }
            else if (c_sku_detail.Count > 1)
            {
                throw new MESReturnMessage($@"機種有多筆父項關係，請確認！{SKUNO}");
            }
            value = c_sku_detail[0].VALUE.ToString();
            categoryname = c_sku_detail[0].CATEGORY_NAME.ToString();
            #endregion
            List<R_SN_KP> r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, categoryname, DB);
            #region 獲取父項和子項
            try {
            if (value == "2")//某個KP為父項
            {
                if (r_sn_kp.Count == 0)
                {
                    r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "", DB);
                    if (r_sn_kp.Count > 0)
                    {
                        parent = r_sn_kp[0].SN.ToString();
                        son = r_sn_kp[0].VALUE.ToString();
                    }
                }
                else
                {
                    parent = r_sn_kp[0].SN.ToString();
                    son = r_sn_kp[0].VALUE.ToString();
                }
            }
            else
            {
                if (r_sn_kp.Count == 0)//加這個，過ASSYP ,value 1 KP設置在ASSY也可以
                {
                    r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "", DB);
                    if (r_sn_kp.Count > 0)
                    {
                        parent = r_sn_kp[0].SN.ToString();
                        son = r_sn_kp[0].VALUE.ToString();
                    }
                    else
                    {
                        throw new MESReturnMessage($@"沒有綁定關係，請確認！{SKUNO}");
                    }
                }
                else
                {
                    parent = r_sn_kp[0].SN.ToString();
                    son = r_sn_kp[0].VALUE.ToString();
                }

            }
            }
            catch(Exception)
            {
                throw new MESReturnMessage($@"獲取父項和子項失敗{parent},{son},請確認！{SKUNO}");
            }
            #endregion

            #region 獲取SN和parent信息
            DeleteParentData(parent, DB);
            DeleteSnData(SN, DB);
            #endregion

            #region 生成父項信息
            try
            {
                Strsql = $@" INSERT INTO R_RELATION_DATA
      (PARENT, son, edit_time, edit_emp, SN,ID)
      SELECT PARENT, son, SYSDATE, '{STATION}', '{SN}','HWT'||sfc.SEQ_R_SN.NEXTVAL
        FROM (SELECT DISTINCT '{parent}' AS PARENT, SN AS son
                FROM R_SN_KP
               WHERE SN =  '{SN}' AND VALUE IS NOT NULL AND STATION LIKE 'ASSY%'
              UNION
              SELECT DISTINCT '{parent}' AS PARENT, value AS son
                FROM R_SN_KP
               WHERE SN = '{SN}' AND VALUE IS NOT NULL AND STATION LIKE 'ASSY%' )
       WHERE PARENT <> son ";
                DB.ExecSqlNoReturn(Strsql, null);
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage($@"生成父項關係失敗{ex}，請確認！{SKUNO}");
            }
            #endregion

            #region 更新父子項對應關係
            r_sn_kp = t_r_sn_kp.LoadListDataBySnAndValue(parent, son, DB);
            string var_partno = string.Empty;
            string var_categoryname = string.Empty;
            string var_eventpoint1 = string.Empty;
            if (r_sn_kp.Count > 0)
            {
                var_partno = r_sn_kp[0].PARTNO;
                var_categoryname = r_sn_kp[0].KP_NAME;
                var_eventpoint1 = r_sn_kp[0].STATION;
                try
                {
                    Strsql = $@" UPDATE R_RELATION_DATA
         SET eventpoint   = '{var_eventpoint1}',
             partno       = '{var_partno}',
             categoryname = '{var_categoryname}'
       WHERE PARENT = '{parent}'
         AND son = '{son}' ";
                    DB.ExecSqlNoReturn(Strsql, null);

                    r_sn_kp = t_r_sn_kp.LoadListDataBySnAndValue(parent, son, DB);

                    #region 調試查看用

       //             Strsql = $@" SELECT * FROM R_RELATION_DATA a          
       //WHERE PARENT = '{parent}'
       //  AND son <> '{son}' ";
       //             DataTable dt = DB.ExecSelect(Strsql).Tables[0];

       //             if (dt.Rows.Count>0)
       //             {
       //                 string testson = dt.Rows[0]["son"].ToString();
       //                 Strsql = $@" 
       //                     SELECT STATION,KP_NAME,partno
       //                   FROM r_sn_kp b
       //              WHERE   b.VALUE ='{testson}'  
       //                AND b.SN = '{parent}'
       //                     ";
       //                   dt = DB.ExecSelect(Strsql).Tables[0];
       //             }
                    #endregion

                    Strsql = $@" UPDATE R_RELATION_DATA a
         SET eventpoint  =
             (SELECT STATION
                FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{parent}'),
             categoryname =
             (SELECT KP_NAME
                 FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{parent}'),
             partno      =
             (SELECT partno
                 FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{parent}')
       WHERE PARENT = '{parent}'
         AND son <> '{son}' ";
                 //   DB.CommitTrain();
                    DB.ExecSqlNoReturn(Strsql, null);

                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage($@"更新父子項關係失敗1,{parent},{ex}，請確認！{SKUNO}");
                }
            }
            else
            {
                try
                {
                    string sonsku = string.Empty;
                r_sn = t_r_sn.LoadData(son, DB);
                sonsku = r_sn.SKUNO;
                r_sn_kp = t_r_sn_kp.LoadListDataBySnAndValue(son, parent, DB);
                var_categoryname = r_sn_kp[0].KP_NAME;
                var_eventpoint1 = r_sn_kp[0].STATION;
                
                    Strsql = $@" UPDATE R_RELATION_DATA
         SET eventpoint   = '{var_eventpoint1}',
             partno       = '{var_partno}',
             categoryname = '{var_categoryname}'
       WHERE PARENT = '{parent}'
         AND son = '{son}' ";
                    DB.ExecSqlNoReturn(Strsql, null);

                    r_sn_kp = t_r_sn_kp.LoadListDataBySnAndValue(parent, son, DB);

                    Strsql = $@" UPDATE R_RELATION_DATA a
         SET eventpoint  =
             (SELECT STATION
                FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{son}'),
             categoryname =
             (SELECT KP_NAME
                 FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{son}'),
             partno      =
             (SELECT partno
                 FROM r_sn_kp b
               WHERE a.son = b.VALUE
                 AND b.SN = '{son}')
       WHERE PARENT = '{parent}'
         AND son <> '{son}'; ";
                    DB.ExecSqlNoReturn(Strsql, null);

                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage($@"更新父項關係失敗2,{son},{ex}，請確認！{SKUNO}");
                }


                try
                {
                    Strsql = $@"SELECT *
      FROM R_RELATION_DATA a, R_RELATION_DATA b
     WHERE a.parent = '{parent}'
       AND a.son = b.SN
       AND a.SN <> b.SN ";

                    DataTable dt = DB.ExecSelect(Strsql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {

                        Strsql = $@" UPDATE R_RELATION_DATA a
         SET son =
             (SELECT PARENT
                FROM R_RELATION_DATA b
               WHERE a.son = b.son
                 AND b.parent <> '{parent}')
       WHERE a.parent = '{parent}'
         AND son IN
             (SELECT son FROM R_RELATION_DATA WHERE PARENT <> '{parent}') ";
                        DB.ExecSqlNoReturn(Strsql, null);
                    }



                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage($@"更新父項關係失敗3,{ex}，請確認！{SKUNO}");
                }


            }

            #endregion
        }

        /// <summary>
        /// 刪除SN綁定關係
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteSnData(string SN, OleExec DB)
        {
            return DB.ORM.Deleteable<R_RELATION_DATA>().Where(t => t.SN == SN).ExecuteCommand();
        }
        /// <summary>
        /// 刪除父項綁定關係
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteParentData(string PARENT, OleExec DB)
        {
            return DB.ORM.Deleteable<R_RELATION_DATA>().Where(t => t.PARENT == PARENT).ExecuteCommand();
        }

        public string GetParentSNByPCBSN(OleExec sfcdb, DB_TYPE_ENUM dbtype, R_SN objPCBSN)
        {
            string parentSN = "";
            T_C_SKU_DETAIL TCSD = new T_C_SKU_DETAIL(sfcdb, dbtype);
            T_R_WO_TYPE TRWT = new T_R_WO_TYPE(sfcdb, dbtype);     
            C_SKU_DETAIL skuDetail = TCSD.GetDetailBySkuAndCategory(sfcdb, objPCBSN.SKUNO, "RELATION");
            if (skuDetail == null)
            {
                throw new Exception(objPCBSN.SN + " " + objPCBSN.SKUNO + " Relation info not set!");
            }
            if (skuDetail.VALUE == "2")
            {
                R_RELATION_DATA relation = sfcdb.ORM.Queryable<R_RELATION_DATA>().Where(r => r.SN == objPCBSN.SN && r.CATEGORYNAME == skuDetail.CATEGORY_NAME).ToList().FirstOrDefault();
                if (relation != null)
                {
                    parentSN = relation.PARENT;
                }
                else
                {
                    if (TRWT.GetProductTypeByWO(sfcdb, objPCBSN.WORKORDERNO) == "RMA")
                    {
                        parentSN = objPCBSN.SN;
                    }
                    else
                    {
                        throw new Exception("Get Parent SN Fail!");
                    }
                }
            }
            else
            {
                parentSN = objPCBSN.SN;
            }
            return "";
        }
    }
    public class Row_R_RELATION_DATA : DataObjectBase
    {
        public Row_R_RELATION_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_RELATION_DATA GetDataObject()
        {
            R_RELATION_DATA DataObject = new R_RELATION_DATA();
            DataObject.ID = this.ID;
            DataObject.PARENT = this.PARENT;
            DataObject.SON = this.SON;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.SN = this.SN;
            DataObject.DATA1 = this.DATA1;
            DataObject.KP_VERSION = this.KP_VERSION;
            DataObject.CUSTOMER_KP_NO = this.CUSTOMER_KP_NO;
            DataObject.CUSTOMER_KP_NO_VER = this.CUSTOMER_KP_NO_VER;
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
        public string PARENT
        {
            get
            {
                return (string)this["PARENT"];
            }
            set
            {
                this["PARENT"] = value;
            }
        }
        public string SON
        {
            get
            {
                return (string)this["SON"];
            }
            set
            {
                this["SON"] = value;
            }
        }
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string CATEGORYNAME
        {
            get
            {
                return (string)this["CATEGORYNAME"];
            }
            set
            {
                this["CATEGORYNAME"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string KP_VERSION
        {
            get
            {
                return (string)this["KP_VERSION"];
            }
            set
            {
                this["KP_VERSION"] = value;
            }
        }
        public string CUSTOMER_KP_NO
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO"];
            }
            set
            {
                this["CUSTOMER_KP_NO"] = value;
            }
        }
        public string CUSTOMER_KP_NO_VER
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO_VER"];
            }
            set
            {
                this["CUSTOMER_KP_NO_VER"] = value;
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
    public class R_RELATION_DATA
    {
        public string ID { get; set; }
        public string PARENT { get; set; }
        public string SON { get; set; }
        public string EVENTPOINT { get; set; }
        public string PARTNO { get; set; }
        public string CATEGORYNAME { get; set; }
        public string SN { get; set; }
        public string DATA1 { get; set; }
        public string KP_VERSION { get; set; }
        public string CUSTOMER_KP_NO { get; set; }
        public string CUSTOMER_KP_NO_VER { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
