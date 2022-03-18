using MESDataObject;
using MESDataObject.Module;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using MESPubLab;
using MESDBHelper;
using System.IO;

namespace MESInterface.DCN
{
    class SendCZDataProccess : taskBase
    {
        public OleExec SFCDB = null;
        MESPubLab.MESStation.MESStationBase station = new MESPubLab.MESStation.MESStationBase();
        public string Series = "";

        public override void init()
        {
            //base.init();
            try
            {

                this._DBName1 = ConfigGet("DBKey1");
                this._DBName2 = ConfigGet("DBKey2");
                string series = ConfigGet("SERIES");
                if (series == "")
                {
                    throw new Exception("請設置要傳送的系列！");
                }
                string[] list = series.Split(',');
                for (var i = 0; i < list.Length; i++)
                {
                    Series += $@",'{list[i]}'";
                }
                Series = Series.Substring(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void WriteLog(string EVENT_TYPE, string MESSAGE, string RUNTIME_ID, OleExec oleExec)
        {
            string str = this.GetNewID("VNDCN", oleExec, "R_PROCCESS_EVENT");
            string[] textArray1 = new string[] { "insert into R_PROCCESS_EVENT (ID,PROCCESS_NAME,EVENT_TYPE,MESSAGE,EVENT_LV,RUNTIME_ID,IP,STATE,EDIT_EMP,EDIT_DATE)\r\n   VALUES ('", str, "','VNTrainCZData','", EVENT_TYPE, "','", MESSAGE, "','','", RUNTIME_ID, "','','','SYSTEM',SYSDATE) " };
            string str2 = string.Concat(textArray1);
            oleExec.ExecSQL(str2);
        }
        public string GetNewID(string BU, OleExec DB, string TableName)
        {
            OleDbParameter[] parameterArray = new OleDbParameter[] { new OleDbParameter(":IN_BU", OleDbType.VarChar, 300), new OleDbParameter(":IN_TYPE", OleDbType.VarChar, 300), new OleDbParameter(":OUT_RES", OleDbType.VarChar, 500) };
            parameterArray[0].Value = BU;
            parameterArray[1].Value = TableName;
            parameterArray[2].Direction = ParameterDirection.Output;
            DB.ExecProcedureNoReturn("SFC.GET_ID", parameterArray);
            string str = parameterArray[2].Value.ToString();
            if (str.StartsWith("ERR"))
            {
                throw new Exception("獲取表'" + TableName + "' ID 異常! " + str);
            }
            return str;
        }

        public void InsertProduct(string Serial, string shiporderid, OleExec oleExec)
        {
            string delete_sql = $@"delete mfsysproduct where SYSSERIALNO='{Serial}' ";
            oleExec.ExecSQL(delete_sql);

            string strsql = $@"select*From mfsysproduct where SYSSERIALNO='{Serial}' and RESEAT=0";
            DataSet ds = oleExec.ExecSelect(strsql);
            if (ds.Tables[0].Rows.Count == 0)
            {
                string SQL = $@"   insert into mfsysproduct
                  select 'PRO'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}',a.sn,'99',a.plant,a.route_id,e.sku_series ,a.workorderno,a.skuno,a.skuno,a.workorderno,a.start_time,'','','','','99',
                  e.production_type,'DEFAULT',e.START_STATION,'','','','','',a.SHIPPED_FLAG,a.shipdate,  case when exists(select*From r_sn_packing c,r_packing d 
                  where a.id=c.sn_id and c.pack_id=d.id )then (select dd.PACK_NO From r_sn_packing cc,r_packing dd 
                  where a.id=cc.sn_id and cc.pack_id=dd.id) else '' end  as PACK_NO,'','',e.wo_type,'','','','','','0','0',a.edit_emp,a.edit_time,'',''
                  from r_sn a ,r_wo_base e  where  a.valid_flag='1'   and a.workorderno=e.workorderno and a.sn ='{Serial}'";
                oleExec.ExecSQL(SQL);
            }

        }
        public void InsertComponent(string Serial, string shiporderid, OleExec oleExec)
        {
            string delete_sql = $@"delete MFSYSCOMPONENT where SYSSERIALNO='{Serial}' ";
            oleExec.ExecSQL(delete_sql);

            string strsql = $@"select*From MFSYSCOMPONENT where SYSSERIALNO='{Serial}' and noreplacepart=0";
            DataSet ds = oleExec.ExecSelect(strsql);
            if (ds.Tables[0].Rows.Count == 0)
            {
                string SQL = $@"  insert into MFSYSCOMPONENT
                         select 'COM'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', aa.sn,aa.PARTNO,aa.revlv,'0',aa.PARTS,aa.PARTNO,'1',aa.PARTNO,'1','1',
                         aa.parts,aa.station,'','','','',aa.kp_name,aa.scantype,aa.auart,'0',  '0','','0',aa.edit_emp,sysdate From (select distinct
                         a.SN,c.revlv,c.parts,a.PARTNO,a.station,a.kp_name, a.scantype,substr(c.auart, 0, 2) auart,a.edit_emp
                         From r_sn_kp A, R_sn b, R_WO_ITEM c where a.sn = '{Serial}' and a.sn = b.sn and a.SCANTYPE<>'AUTOAP'
                         and b.workorderno = c.aufnr and a.partno = c.matnr and a.valid_flag = 1) aa";
                oleExec.ExecSelect(SQL);
            }
        }
        public void InsertCserial(string Serial, string shiporderid, OleExec oleExec)
        {
            string delete_sql = $@"delete mfsyscserial where SYSSERIALNO='{Serial}' ";
            oleExec.ExecSQL(delete_sql);

            string strsql = $@"select*From mfsyscserial where SYSSERIALNO='{Serial}' and ( prodtype is null or prodtype='' ) ";
            DataSet ds = oleExec.ExecSelect(strsql);
            if (ds.Tables[0].Rows.Count == 0)
            {
                string SQL = $@"insert into mfsyscserial
                                                    select 'CSE'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', sn,value,
                                                    station,partno,CASE WHEN SCANTYPE='AUTOAP'THEN LOCATION  WHEN KP_NAME='PS' THEN 'POWER-'||seqno
                                                    else  KP_NAME||'-'||seqno END  as eeecode,partno,detailseq,scantype,'CSERIALNO', '',value,edit_emp,edit_time,edit_emp,edit_time,'1',mpn,'' 
                                                    from (select a.*,row_number() over(partition by a.partno order by id) seqno
                                                    from r_sn_kp a where a.sn='{Serial}'  and a.valid_flag=1 )";
                oleExec.ExecSelect(SQL);
            }
        }

        public override void Start()
        {
            SentOneByOne();
            return;

            //CheckSent();
            //return;
            //string resent_sn = ConfigGet("RESENT");
            //if (!string.IsNullOrEmpty(resent_sn))
            //{
            //    ReSent();
            //    return;
            //}
            OleExec oleExec = new OleExec(this._DBName1, false);
            HWDNNSFCBase.OleExec oleExec2 = new HWDNNSFCBase.OleExec(this._DBName2, false);
            string str = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.WriteLog("START", "OK", str, oleExec);
            oleExec2.RunStoredprocedure("Truncate_TempToNormal_CZ", new OleDbParameter[]
            {
                new OleDbParameter("@trantype", "TRUNCATE_TEMP")
            });

            //DUS4545R0HV
            //取需要传的SN
            string strSQL = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ'  and a.SHIPDATE >sysdate-30
                             and a.sn not in (select sysserialno From mfsysproduct where Reseat=1)
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D where a.dn_no=b.vbeln  AND A.SN=C.SN AND C.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' and a.SHIPDATE >sysdate-30
                             and D.sn not in (select sysserialno From mfsysproduct  where Reseat=1) 
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D,R_SN_KP E where a.dn_no=b.vbeln  
                             AND A.SN=C.SN AND C.VALUE=E.SN AND E.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' AND E.VALID_FLAG='1' and a.SHIPDATE >sysdate-30
                             and D.sn not in (select sysserialno From mfsysproduct  where Reseat=1) 
                             union
                             SELECT distinct Data2 SN,data1 shiporderid FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N'  ";

            DataSet Dssn = oleExec.RunSelect(strSQL);
            if (Dssn.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < Dssn.Tables[0].Rows.Count; i++)
                {
                    string _sn = Dssn.Tables[0].Rows[i]["SN"].ToString();
                    string shiporderid = Dssn.Tables[0].Rows[i]["shiporderid"].ToString();
                    InsertProduct(_sn, shiporderid, oleExec);
                    InsertComponent(_sn, shiporderid, oleExec);
                    InsertCserial(_sn, shiporderid, oleExec);
                }
            }

            DBTableP dbtableP = new DBTableP("mfsysproduct_cz", oleExec2);
            dbtableP.myDBTYPE = 0;
            dbtableP.analyse();
            DBTableP dbtableP2 = new DBTableP("mfsyscomponent_cz", oleExec2);
            dbtableP2.myDBTYPE = 0;
            dbtableP2.analyse();
            DBTableP dbtableP3 = new DBTableP("mfsyscserial_cz", oleExec2);
            dbtableP3.myDBTYPE = 0;
            dbtableP3.analyse();
            DBTableP dbtableP4 = new DBTableP("wwn_datasharing_TEMP", oleExec2);
            dbtableP4.myDBTYPE = 0;
            dbtableP4.analyse();
            DBTableP dbtableP5 = new DBTableP("wwn_datasharing_cz", oleExec2);
            dbtableP5.myDBTYPE = 0;
            dbtableP5.analyse();
            strSQL = $@"select sysserialno,seqno,factoryid,routeid,customerid,workorderno,skuno,custpartno,eeecode,custssn,''firmware,
                                        software,servicetag,enetid,prioritycode,productfamily,productlevel,productcolor,productlangulage,
                                        shipcountry,productdesc,''orderno,''compcode,shipped,shipdate,location,whid,areaid,workordertype,packageno,systemstage,unitcost,lineseqno,reseatpre,reseat,reseatTag,lasteditby,lasteditdt,coo 
                                        from mfsysproduct where Reseat=0  ";
            DataSet dataSet = oleExec.RunSelect(strSQL);
            dbtableP.InsertData(dataSet.Tables[0]);
            strSQL = $@"select sysserialno,partno,version,seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,
                                        eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,
                                       substr(originalqty, 0,6) AS orig,unitcost,replacegroup,noreplacepart,lasteditby,lasteditdt 
                                        from mfsyscomponent  where noreplacepart=0 and  sysserialno in(select sysserialno from mfsysproduct where Reseat=0  )";
            dataSet = oleExec.RunSelect(strSQL);
            dbtableP2.InsertData(dataSet.Tables[0]);
            strSQL = $@"select sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,
                                        prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial 
                                        where ( prodtype is null or prodtype='' ) and sysserialno in (select sysserialno from mfsysproduct  where Reseat=0 )";
            dataSet = oleExec.RunSelect(strSQL);
            dbtableP3.InsertData(dataSet.Tables[0]);
            try
            {
                oleExec2.RunStoredprocedure("Z_TempToNormal", new OleDbParameter[0]);
            }
            catch (Exception ex)
            {
                this.WriteLog("ERR", ex.Message.Replace("'", ""), str, oleExec);
            }
            try
            {
                string strcomsql = "update  mfsyscomponent set noreplacepart=1  where sysserialno in (select sysserialno from mfsysproduct where reseat=0)";
                oleExec.ExecSQL(strcomsql);
                string strcsesql = $@" update  mfsyscserial  set prodtype='OK'  where sysserialno in (select sysserialno from mfsysproduct  
                                                                where  workorderno not like '000031%' and workorderno not like '000034%' and  reseat=0)";
                oleExec.ExecSQL(strcsesql);
                string strprosql = "update  mfsysproduct    set reseat=1  where  reseat=0";
                oleExec.ExecSQL(strprosql);

            }
            catch (Exception ex2)
            {

            }
            strSQL = "truncate table wwn_datasharing_cz";
            oleExec2.ExecSQL(strSQL);

            #region CZ要求Wedge、Odin、Skybolt產品在wwn表的格式與NN一致(VN綁60，NN直接綁40)
            //strSQL = $@"
            //                    select * from wwn_datasharing where cssn in  (
            //                    SELECT distinct Data2  FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N'   
            //                    union  
            //                    select distinct A.SN  From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ'  and a.SHIPDATE >sysdate-15)
            //                    union
            //                    select * from wwn_datasharing where vssn in  (
            //                    SELECT distinct Data2  FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N'   
            //                    union  
            //                    select distinct A.SN  From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ'  and a.SHIPDATE >sysdate-15)
            //                    union  
            //                    select * from wwn_datasharing where wsn in  (
            //                    SELECT distinct Data2  FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N'   
            //                    union  
            //                    select distinct A.SN  From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ'  and a.SHIPDATE >sysdate-15 )";
            if (Series == "")
            {
                throw new Exception("獲取系列失敗！");
            }
            strSQL = $@"
                select ID,
                       WSN,
                       SKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          CSSN
                         else
                          VSSN
                       END VSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          CSKU
                         else
                          VSKU
                       END VSKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSSN
                       END CSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSKU
                       END CSKU,
                       MAC,
                       WWN,
                       MAC_BLOCK_SIZE,
                       WWN_BLOCK_SIZE,
                       LASTEDITBY,
                       LASTEDITDT,
                       MACTB0,
                       MACTB1,
                       MACTB2,
                       MACTB3,
                       MACTB4,
                       WWNTB0,
                       WWNTB1,
                       WWNTB2,
                       WWNTB3,
                       WWNTB4
                  from (select *
                          from wwn_datasharing
                         where cssn in (SELECT distinct Data2
                                          FROM r_sn_log
                                         WHERE LOGTYPE = 'SendCZData_CheckSN'
                                           AND Data9 = 'N'
                                        union
                                        select distinct A.SN
                                          From r_ship_detail a, sd_to_detail b
                                         where a.dn_no = b.vbeln
                                           and b.land1 = 'CZ'
                                           and a.SHIPDATE > sysdate - 15)
                        union
                        select *
                          from wwn_datasharing
                         where vssn in (SELECT distinct Data2
                                          FROM r_sn_log
                                         WHERE LOGTYPE = 'SendCZData_CheckSN'
                                           AND Data9 = 'N'
                                        union
                                        select distinct A.SN
                                          From r_ship_detail a, sd_to_detail b
                                         where a.dn_no = b.vbeln
                                           and b.land1 = 'CZ'
                                           and a.SHIPDATE > sysdate - 15)
                        union
                        select *
                          from wwn_datasharing
                         where wsn in (SELECT distinct Data2 FROM r_sn_log
                                        WHERE LOGTYPE = 'SendCZData_CheckSN'
                                          AND Data9 = 'N'
                                       union
                                       select distinct A.SN
                                         From r_ship_detail a, sd_to_detail b
                                        where a.dn_no = b.vbeln
                                          and b.land1 = 'CZ'
                                          and a.SHIPDATE > sysdate - 15))";
            #endregion

            dataSet = oleExec.RunSelect(strSQL);
            dbtableP5.InsertData(dataSet.Tables[0]);
            strSQL = $@" select WSN, SKU, VSSN, VSKU, CSSN, CSKU, MAC, WWN, MAC_block_size, WWN_block_size, lasteditby,
                                    lasteditdt, MACTB0, MACTB1, MACTB2, MACTB3, MACTB4, WWNTB0, WWNTB1, WWNTB2, WWNTB3, WWNTB4,getdate()
                                    from WWN_Datasharing_cz a(nolock) where not exists(select top 1 1 from WWN_Datasharing_temp b(nolock) where a.wsn=b.wsn)";
            dataSet = oleExec2.RunSelect(strSQL);
            dbtableP4.InsertData(dataSet.Tables[0]);
            strSQL = $@"update a set a.sku=b.sku,a.dt_cr=getdate()
                                    from wwn_datasharing_temp as  a,wwn_datasharing_cz as b  where a.wsn=b.wsn and a.sku<>b.sku and a.wsn not like 'FS04%'";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a set a.vsku=b.vsku,a.dt_cr=getdate() from wwn_datasharing_temp as a,wwn_datasharing_cz as b 
                                        where a.wsn=b.wsn and a.vssn=b.vssn and a.vsku<>b.vsku and a.wsn not like 'FS04%' ";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a set a.vssn=b.vssn,a.vsku=b.vsku,a.mac=b.mac,a.wwn=b.wwn,a.dt_cr=getdate()
                                        from wwn_datasharing_temp as a,wwn_datasharing_cz as b  where a.wsn=b.wsn and (a.vssn<>b.vssn)
                                        and a.wsn not like 'FS04%'  and a.vssn not like 'UB06%' and a.vssn not like 'UE06%'  and a.vssn not like 'AGE06%'";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a set a.cssn=b.cssn,a.csku=b.csku,a.mac=b.mac,a.wwn=b.wwn,a.dt_cr=getdate() 
                                        from wwn_datasharing_temp as a,wwn_datasharing_cz as b where a.wsn=b.wsn and (a.cssn<>b.cssn) and b.cssn<>'N/A'  and a.wsn not like 'FS04%'";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a set a.csku=b.csku,a.dt_cr=getdate() from wwn_datasharing_temp as a,wwn_datasharing_cz as b  where a.wsn=b.wsn and a.cssn=b.cssn and a.csku<>b.csku  and a.wsn not like 'FS04%'";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a  set  a.mac=b.mac ,a.dt_cr=getdate() from  wwn_datasharing_temp as  a , wwn_datasharing_cz as b  where a.wsn=b.wsn and (a.mac<>b.mac and b.mac<>'' and  Len(b.mac)=12) and a.wsn not like 'FS04%'";
            oleExec2.ExecSQL(strSQL);
            strSQL = $@"update a  set  a.wwn=b.wwn ,a.dt_cr=getdate() from  wwn_datasharing_temp as  a , wwn_datasharing_cz as b where a.wsn=b.wsn and (a.wwn<>b.wwn and b.wwn<>'' and Len(b.wwn)=16) and a.wsn not like 'FS04%'";
            oleExec2.ExecSQL(strSQL);

            this.WriteLog("END", "OK", str, oleExec);
        }

        public void ReSent()
        {
            try
            {
                string resent_data_excle = System.IO.Directory.GetCurrentDirectory() + "\\CZ_RESENT_DATA\\WAIT_RESNT\\ResentCZ.xlsx";
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(resent_data_excle);
                string sql_sn = "";
                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"ResentCZ.xlsx No Data Or Not Exists!");
                }
                if (dt.Rows[0][0].ToString().ToUpper().Trim() != "SN")
                {
                    //throw new Exception($@"ResentCZ No SN Clomun!");
                }
                for (var r = 0; r < dt.Rows.Count; r++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[r][0].ToString()))
                    {
                        sql_sn += $@",'{dt.Rows[r][0].ToString().ToUpper()}'";
                    }
                }
                sql_sn = sql_sn.Substring(1);

                OleExec oleExec = new OleExec(this._DBName1, false);
                HWDNNSFCBase.OleExec oleExec2 = new HWDNNSFCBase.OleExec(this._DBName2, false);
                string str = DateTime.Now.ToString("yyyyMMddHHmmss");
                this.WriteLog("Begin Resent", "OK", str, oleExec);
                oleExec2.RunStoredprocedure("Truncate_TempToNormal_CZ", new OleDbParameter[]
                {
                new OleDbParameter("@trantype", "TRUNCATE_TEMP")
                });
                #region 先在CZ的中轉庫中刪掉,不用TRUNCATE TABLE
                //string delete_cz = $@" delete  mfsyscomponent_cz where sysserialno in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);
                //delete_cz = $@" delete  mfsysproduct_cz where sysserialno in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);
                //delete_cz = $@" delete  mfsyscserial_cz where sysserialno in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);
                #endregion

                string strSQL = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ' 
                             and a.sn in ({sql_sn})
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D where a.dn_no=b.vbeln  AND A.SN=C.SN AND C.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' and a.sn in ( {sql_sn} ) 
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D,R_SN_KP E where a.dn_no=b.vbeln  
                             AND A.SN=C.SN AND C.VALUE=E.SN AND E.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' AND E.VALID_FLAG='1'
                             and a.sn in ( {sql_sn} ) 
                             union
                             SELECT distinct Data2 SN,data1 shiporderid FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N' ";


                DataSet Dssn = oleExec.RunSelect(strSQL);
                if (Dssn.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Dssn.Tables[0].Rows.Count; i++)
                    {
                        string _sn = Dssn.Tables[0].Rows[i]["SN"].ToString();
                        string shiporderid = Dssn.Tables[0].Rows[i]["shiporderid"].ToString();
                        InsertProduct(_sn, shiporderid, oleExec);
                        InsertComponent(_sn, shiporderid, oleExec);
                        InsertCserial(_sn, shiporderid, oleExec);
                    }
                }


                DBTableP dbtableP = new DBTableP("mfsysproduct_cz", oleExec2);
                dbtableP.myDBTYPE = 0;
                dbtableP.analyse();
                DBTableP dbtableP2 = new DBTableP("mfsyscomponent_cz", oleExec2);
                dbtableP2.myDBTYPE = 0;
                dbtableP2.analyse();
                DBTableP dbtableP3 = new DBTableP("mfsyscserial_cz", oleExec2);
                dbtableP3.myDBTYPE = 0;
                dbtableP3.analyse();
                DBTableP dbtableP4 = new DBTableP("wwn_datasharing_TEMP", oleExec2);
                dbtableP4.myDBTYPE = 0;
                dbtableP4.analyse();
                DBTableP dbtableP5 = new DBTableP("wwn_datasharing_cz", oleExec2);
                dbtableP5.myDBTYPE = 0;
                dbtableP5.analyse();
                strSQL = $@"select distinct sysserialno,seqno,factoryid,routeid,customerid,workorderno,skuno,custpartno,eeecode,custssn,''firmware,
                                        software,servicetag,enetid,prioritycode,productfamily,productlevel,productcolor,productlangulage,
                                        shipcountry,productdesc,''orderno,''compcode,shipped,shipdate,location,whid,areaid,workordertype,packageno,systemstage,unitcost,lineseqno,reseatpre,reseat,reseatTag,lasteditby,lasteditdt,coo 
                                        from mfsysproduct where Reseat=0 ";
                DataSet dataSet = oleExec.RunSelect(strSQL);
                dbtableP.InsertData(dataSet.Tables[0]);
                strSQL = $@"select distinct sysserialno,partno,version,seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,
                                        eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,
                                       substr(originalqty, 0,6) AS orig,unitcost,replacegroup,noreplacepart,lasteditby,lasteditdt 
                                        from mfsyscomponent  where noreplacepart=0 and  sysserialno in(select sysserialno from mfsysproduct where Reseat=0 )";
                dataSet = oleExec.RunSelect(strSQL);
                dbtableP2.InsertData(dataSet.Tables[0]);
                strSQL = $@"select distinct sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,
                                        prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial 
                                        where ( prodtype is null or prodtype='' ) and sysserialno in (select sysserialno from mfsysproduct  where Reseat=0 )";
                dataSet = oleExec.RunSelect(strSQL);
                dbtableP3.InsertData(dataSet.Tables[0]);
                try
                {
                    oleExec2.RunStoredprocedure("Z_TempToNormal", new OleDbParameter[0]);
                    //                    #region 按SN 刪除
                    //                    delete_cz = $@" delete from mfsyscserial_temp where sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(delete_cz);
                    //                    delete_cz = $@" delete from mfsyscomponent_temp where sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(delete_cz);
                    //                    delete_cz = $@" delete from mfsysproduct_temp where sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(delete_cz);

                    //                    string insert_cz_temp = $@"insert into mfsyscserial_temp (sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,  
                    //prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN)
                    //select sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,  
                    //prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial_cz a(nolock) where not exists(select * from mfsyscserial_temp b(nolock) where
                    //a.sysserialno=b.sysserialno and a.cserialno=b.cserialno and a.eeecode=b.eeecode) and  sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(insert_cz_temp);

                    //                    insert_cz_temp = $@"insert into mfsyscomponent_temp (sysserialno,partno,[version],seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,
                    //installedqty,eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,originalqty,unitcost,
                    //replacegroup,noreplacepart,lasteditby,lasteditdt)
                    //select sysserialno,partno,[version],seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,eeecode,
                    //cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,originalqty,unitcost,replacegroup,
                    //noreplacepart,lasteditby,lasteditdt
                    //from mfsyscomponent_cz a (nolock) where not exists(select * from mfsyscomponent_temp  b(nolock)
                    //where a.sysserialno=b.sysserialno and a.partno=b.partno) and sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(insert_cz_temp);

                    //                    insert_cz_temp = $@"insert into mfsysproduct_temp
                    //select * from mfsysproduct_cz a(nolock) where not exists(select * from mfsysproduct_temp  b(nolock) where 
                    //a.sysserialno=b.sysserialno) and sysserialno in ({sql_sn})";
                    //                    oleExec2.ExecSQL(insert_cz_temp);
                    //                    #endregion
                }
                catch (Exception ex)
                {
                    this.WriteLog("ERR", ex.Message.Replace("'", ""), str, oleExec);
                }
                try
                {
                    string strcomsql = "update  mfsyscomponent set noreplacepart=1  where sysserialno in (select sysserialno from mfsysproduct where reseat=0)";
                    oleExec.ExecSQL(strcomsql);
                    string strcsesql = $@" update  mfsyscserial  set prodtype='OK'  where sysserialno in (select sysserialno from mfsysproduct  
                                                                where  workorderno not like '000031%' and workorderno not like '000034%' and  reseat=0)";
                    oleExec.ExecSQL(strcsesql);
                    string strprosql = "update  mfsysproduct    set reseat=1  where  reseat=0";
                    oleExec.ExecSQL(strprosql);

                }
                catch (Exception ex2)
                {

                }
                strSQL = "truncate table wwn_datasharing_cz";
                oleExec2.ExecSQL(strSQL);
                //#region 先在CZ的中轉庫中刪掉,不用TRUNCATE TABLE        
                //delete_cz = $@" delete  WWN_Datasharing_cz where VSSN in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);
                //delete_cz = $@" delete  WWN_Datasharing_cz where CSSN in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);
                //delete_cz = $@" delete  WWN_Datasharing_cz where WSN in ({sql_sn})";
                //oleExec2.ExecSQL(delete_cz);                
                //#endregion

                #region CZ要求Wedge、Odin、Skybolt產品在wwn表的格式與NN一致(VN綁60，NN直接綁40)            
                if (Series == "")
                {
                    throw new Exception("獲取系列失敗！");
                }
                strSQL = $@"
                select ID,
                       WSN,
                       SKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          CSSN
                         else
                          VSSN
                       END VSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          CSKU
                         else
                          VSKU
                       END VSKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSSN
                       END CSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ({Series}))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSKU
                       END CSKU,
                       MAC,
                       WWN,
                       MAC_BLOCK_SIZE,
                       WWN_BLOCK_SIZE,
                       LASTEDITBY,
                       LASTEDITDT,
                       MACTB0,
                       MACTB1,
                       MACTB2,
                       MACTB3,
                       MACTB4,
                       WWNTB0,
                       WWNTB1,
                       WWNTB2,
                       WWNTB3,
                       WWNTB4
                  from (select *
                          from wwn_datasharing
                         where cssn in ({sql_sn})
                        union
                        select *
                          from wwn_datasharing
                         where vssn in ({sql_sn})
                        union
                        select *
                          from wwn_datasharing
                         where wsn in ({sql_sn}))";
                #endregion

                dataSet = oleExec.RunSelect(strSQL);
                dbtableP5.InsertData(dataSet.Tables[0]);
                strSQL = $@" select WSN, SKU, VSSN, VSKU, CSSN, CSKU, MAC, WWN, MAC_block_size, WWN_block_size, lasteditby,
                                    lasteditdt, MACTB0, MACTB1, MACTB2, MACTB3, MACTB4, WWNTB0, WWNTB1, WWNTB2, WWNTB3, WWNTB4,getdate()
                                    from WWN_Datasharing_cz a(nolock) where not exists(select top 1 1 from WWN_Datasharing_temp b(nolock) where a.wsn=b.wsn)";
                dataSet = oleExec2.RunSelect(strSQL);
                dbtableP4.InsertData(dataSet.Tables[0]);
                strSQL = $@"update a set a.sku=b.sku,a.dt_cr=getdate()
                                    from wwn_datasharing_temp as  a,wwn_datasharing_cz as b  where a.wsn=b.wsn and a.sku<>b.sku and a.wsn not like 'FS04%'";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a set a.vsku=b.vsku,a.dt_cr=getdate() from wwn_datasharing_temp as a,wwn_datasharing_cz as b 
                                        where a.wsn=b.wsn and a.vssn=b.vssn and a.vsku<>b.vsku and a.wsn not like 'FS04%' ";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a set a.vssn=b.vssn,a.vsku=b.vsku,a.mac=b.mac,a.wwn=b.wwn,a.dt_cr=getdate()
                                        from wwn_datasharing_temp as a,wwn_datasharing_cz as b  where a.wsn=b.wsn and (a.vssn<>b.vssn)
                                        and a.wsn not like 'FS04%'  and a.vssn not like 'UB06%' and a.vssn not like 'UE06%'  and a.vssn not like 'AGE06%'";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a set a.cssn=b.cssn,a.csku=b.csku,a.mac=b.mac,a.wwn=b.wwn,a.dt_cr=getdate() 
                                        from wwn_datasharing_temp as a,wwn_datasharing_cz as b where a.wsn=b.wsn and (a.cssn<>b.cssn) and b.cssn<>'N/A'  and a.wsn not like 'FS04%'";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a set a.csku=b.csku,a.dt_cr=getdate() from wwn_datasharing_temp as a,wwn_datasharing_cz as b  where a.wsn=b.wsn and a.cssn=b.cssn and a.csku<>b.csku  and a.wsn not like 'FS04%'";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a  set  a.mac=b.mac ,a.dt_cr=getdate() from  wwn_datasharing_temp as  a , wwn_datasharing_cz as b  where a.wsn=b.wsn and (a.mac<>b.mac and b.mac<>'' and  Len(b.mac)=12) and a.wsn not like 'FS04%'";
                oleExec2.ExecSQL(strSQL);
                strSQL = $@"update a  set  a.wwn=b.wwn ,a.dt_cr=getdate() from  wwn_datasharing_temp as  a , wwn_datasharing_cz as b where a.wsn=b.wsn and (a.wwn<>b.wwn and b.wwn<>'' and Len(b.wwn)=16) and a.wsn not like 'FS04%'";
                oleExec2.ExecSQL(strSQL);

                this.WriteLog("END Resent", "OK", str, oleExec);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 一個一個傳，不寫到CZ表
        /// </summary>
        public void SentOneByOne()
        {
            OleExec MESDB_LOG = new OleExec(this._DBName1, false);
            OleExec MESDB = new OleExec(this._DBName1, false);
            HWDNNSFCBase.OleExec CZDB = new HWDNNSFCBase.OleExec(this._DBName2, false);
            string str = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.WriteLog("START", "OK", str, MESDB);
            //取需要传的SN
            string resent_sn = ConfigGet("RESENT_FLAG");
            string strSQL = "";
            if (resent_sn.ToUpper()=="FALSE")
            {
                strSQL = $@"select *
                     from (select distinct A.SN SN, a.id shiporderid, a.shipdate
                             From r_ship_detail a, sd_to_detail b
                            where a.dn_no = b.vbeln
                              and b.land1 = 'CZ'
                              and a.SHIPDATE > sysdate - 30
                              and not exists (select sysserialno
                                     From mfsysproduct m
                                    where m.Reseat = 1
                                      and m.sysserialno = a.sn)
                              and not exists
                            (select *
                                     from R_MES_LOG c
                                    where c.PROGRAM_NAME = 'SendCZData'
                                      and c.class_name = 'MESInterface.DCN.SendCZDataProccess'
                                      and c.function_name = 'SentOneByOne'
                                      and a.sn = c.data1)
                            order by a.shipdate asc)
                    where rownum < 100";
            }
            else
            {
                strSQL = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ' 	
                                and exists (select * from r_sn_log g where g.LOGTYPE = 'SendCZData' and g.flag='N' and g.sn=a.sn) ";
            }

            DataSet Dssn = MESDB.RunSelect(strSQL);
            MESDB.ThrowSqlExeception = true;
            CZDB.ThrowSqlExeception = true;
            if (Dssn.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < Dssn.Tables[0].Rows.Count; i++)
                {                   
                    MESDB.BeginTrain();                    
                    CZDB.BeginTrain();
                    string _sn = Dssn.Tables[0].Rows[i]["SN"].ToString();
                    string shiporderid = Dssn.Tables[0].Rows[i]["shiporderid"].ToString();
                    string result = "";
                    string sql = "";
                    try
                    {
                        //1.本階
                        sql = $@" select * from R_SN_KP a where a.sn='{_sn}' and a.valid_flag='1' ";
                        DataTable kp_table_1 = MESDB.ORM.Ado.GetDataTable(sql);
                        if(kp_table_1.Rows.Count>0)
                        {
                            result = InsertMES(_sn, shiporderid, MESDB);
                            if (result != "OK")
                            {
                                throw new Exception(result);
                            }
                            result = InsertCZ(_sn, CZDB, MESDB);
                            if (result != "OK")
                            {
                                throw new Exception(result);
                            }
                            UpdateMES(_sn, MESDB);
                        }
                        
                        //2.2階KP
                        sql = $@"select a.value from R_SN_KP a where a.sn='{_sn}' and a.valid_flag='1'	
                                and exists (select * from r_sn b where a.value=b.sn and b.valid_flag='1')";
                        DataTable kp_table_2 = MESDB.ORM.Ado.GetDataTable(sql);
                        string sn_kp_2 = "";
                        if (kp_table_2.Rows.Count > 0)
                        {
                            foreach (DataRow kp_1 in kp_table_2.Rows)
                            {
                                sn_kp_2 = kp_1["VALUE"].ToString();
                                result = InsertMES(sn_kp_2, shiporderid, MESDB);
                                if (result != "OK")
                                {
                                    throw new Exception(result);
                                }
                                result = InsertCZ(sn_kp_2, CZDB, MESDB);
                                if (result != "OK")
                                {
                                    throw new Exception(result);
                                }
                                UpdateMES(sn_kp_2, MESDB);
                            }
                        }

                        //3.3階KP
                        sql = $@"select n.value from R_SN_KP m,R_SN_KP n where m.sn='{_sn}' and m.valid_flag='1'	
                                    and m.value=n.sn and n.valid_flag='1' 
                                    and  exists (select * from r_sn sn where n.value=sn.sn and sn.valid_flag='1')";
                        DataTable kp_table_3 = MESDB.ORM.Ado.GetDataTable(sql);
                        string sn_kp_3 = "";
                        if (kp_table_3.Rows.Count > 0)
                        {
                            foreach (DataRow kp_3 in kp_table_3.Rows)
                            {
                                sn_kp_3 = kp_3["VALUE"].ToString();
                                result = InsertMES(sn_kp_3, shiporderid, MESDB);
                                if (result != "OK")
                                {
                                    throw new Exception(result);
                                }
                                result = InsertCZ(sn_kp_3, CZDB, MESDB);
                                if (result != "OK")
                                {
                                    throw new Exception(result);
                                }
                                UpdateMES(sn_kp_3, MESDB);
                            }
                        }

                        //4.wwwn
                        if (Series == "")
                        {
                            throw new Exception("獲取系列失敗！");
                        }
                        string wwn_sql = $@"
                                select ID,
                                       WSN,
                                       SKU,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          CSSN
                                         else
                                          VSSN
                                       END VSSN,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          CSKU
                                         else
                                          VSKU
                                       END VSKU,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          'N/A'
                                         else
                                          CSSN
                                       END CSSN,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          'N/A'
                                         else
                                          CSKU
                                       END CSKU,
                                       MAC,
                                       WWN,
                                       MAC_BLOCK_SIZE,
                                       WWN_BLOCK_SIZE,
                                       LASTEDITBY,
                                       LASTEDITDT,
                                       MACTB0,
                                       MACTB1,
                                       MACTB2,
                                       MACTB3,
                                       MACTB4,
                                       WWNTB0,
                                       WWNTB1,
                                       WWNTB2,
                                       WWNTB3,
                                       WWNTB4
                                  from (select *
                                          from wwn_datasharing
                                         where cssn='{_sn}'
                                        union
                                        select *
                                          from wwn_datasharing
                                         where vssn ='{_sn}'
                                        union
                                        select *
                                          from wwn_datasharing
                                         where wsn ='{_sn}')";
                        wwn_sql = $@"select WSN, SKU, VSSN, VSKU, CSSN, CSKU, MAC, WWN, MAC_block_size, WWN_block_size, lasteditby,
                                    lasteditdt, MACTB0, MACTB1, MACTB2, MACTB3, MACTB4, WWNTB0, WWNTB1, WWNTB2, WWNTB3, WWNTB4,sysdate
                                    from ({wwn_sql})";
                        string insert_wwn = "";
                        try
                        {
                            DataTable wwn_table = MESDB.ORM.Ado.GetDataTable(wwn_sql);
                            if (wwn_table.Rows.Count > 0)
                            {
                                string delete_wwn = $@"delete from wwn_datasharing_TEMP where cssn = '{_sn}'";
                                CZDB.ExecSQL(delete_wwn);

                                delete_wwn = $@"delete from wwn_datasharing_TEMP where vssn = '{_sn}'";
                                CZDB.ExecSQL(delete_wwn);

                                delete_wwn = $@"delete from wwn_datasharing_TEMP where wsn = '{_sn}'";
                                CZDB.ExecSQL(delete_wwn);

                                DBTableP dbtable_wwn = new DBTableP("wwn_datasharing_TEMP", CZDB);
                                dbtable_wwn.myDBTYPE = 0;
                                dbtable_wwn.analyse();

                                foreach (DataRow row in wwn_table.Rows)
                                {
                                    insert_wwn = dbtable_wwn.GetInsertSql(row);
                                    CZDB.ExecSQL(insert_wwn);
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($@"SN:{_sn};SQL:{insert_wwn};Error:{ex.Message}");
                        }

                        string update_log = $@" update r_sn_log  set flag='Y' where LOGTYPE = 'SendCZData' and flag='N' and sn='{_sn}'";
                        MESDB.ORM.Ado.ExecuteCommand(update_log);

                        MESDB.CommitTrain();
                        CZDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        string fail_path = System.IO.Directory.GetCurrentDirectory() + "\\SendCZDataFail\\";
                        if (!Directory.Exists(fail_path))
                        {
                            Directory.CreateDirectory(fail_path);
                        }
                        string fail_file = System.IO.Directory.GetCurrentDirectory() + "\\SendCZDataFail\\SN_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                        using (FileStream fs = new FileStream(fail_file, FileMode.Append, FileAccess.Write))
                        {
                            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
                            sw.Flush();
                            sw.BaseStream.Seek(0, SeekOrigin.Current);
                            sw.WriteLine($@"Shipped SN:{_sn};Error:{ex.Message}");
                        }
                        try
                        {
                            MESDB.RollbackTrain();
                            CZDB.RollbackTrain();
                        }
                        catch (Exception)
                        {
                        }
                       
                        try
                        {
                            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(MESDB_LOG, DB_TYPE_ENUM.Oracle);
                            R_MES_LOG log = new R_MES_LOG();
                            log.ID = t_r_mes_log.GetNewID("VNDCN", MESDB_LOG);
                            log.PROGRAM_NAME = "SendCZData";
                            log.CLASS_NAME = "MESInterface.DCN.SendCZDataProccess";
                            log.FUNCTION_NAME = "SentOneByOne";
                            log.LOG_MESSAGE = "Send CZ Data Fail";
                            log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                            log.DATA1 = _sn;
                            log.MAILFLAG = "N";
                            log.EDIT_EMP = "Interface";
                            log.EDIT_TIME = MESDB_LOG.ORM.GetDate();
                            MESDB_LOG.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();                            
                        }
                        catch (Exception)
                        {
                        }                        
                    }
                }
            }
            this.WriteLog("END", "OK", str, MESDB);
        }
        public string InsertMES(string Serial, string shiporderid, OleExec oleExec)
        {
            string delete_sql = "";
            string insert_sql = "";
            string result = "OK";
            try
            {
                //mfsysproduct
                delete_sql = $@"delete mfsysproduct where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);
                insert_sql = $@"   insert into mfsysproduct
                  select 'PRO'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}',a.sn,'99',a.plant,a.route_id,e.sku_series ,a.workorderno,a.skuno,a.skuno,a.workorderno,a.start_time,'','','','','99',
                  e.production_type,'DEFAULT',e.START_STATION,'','','','','',a.SHIPPED_FLAG,a.shipdate,  case when exists(select*From r_sn_packing c,r_packing d 
                  where a.id=c.sn_id and c.pack_id=d.id )then (select dd.PACK_NO From r_sn_packing cc,r_packing dd 
                  where a.id=cc.sn_id and cc.pack_id=dd.id) else '' end  as PACK_NO,'','',e.wo_type,'','','','','','0','0',a.edit_emp,a.edit_time,'',''
                  from r_sn a ,r_wo_base e  where  a.valid_flag='1'   and a.workorderno=e.workorderno and a.sn ='{Serial}'";
                oleExec.ExecSQL(insert_sql);

                //MFSYSCOMPONENT
                delete_sql = $@"delete MFSYSCOMPONENT where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);
                insert_sql = $@"  insert into MFSYSCOMPONENT
                         select 'COM'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', aa.sn,aa.PARTNO,aa.revlv,'0',aa.PARTS,aa.PARTNO,'1',aa.PARTNO,'1','1',
                         aa.parts,aa.station,'','','','',aa.scantype,aa.kp_name,aa.auart,'0',  '0','','0','SYSTEM',sysdate From (select distinct
                         a.SN,c.revlv,c.parts,a.PARTNO,a.station, a.scantype,a.kp_name,substr(c.auart, 0, 2) auart
                         From r_sn_kp A, R_sn b, R_WO_ITEM c where a.sn = '{Serial}' and a.sn = b.sn and a.SCANTYPE<>'AUTOAP'
                         and b.workorderno = c.aufnr and a.partno = c.matnr and a.valid_flag = 1 and b.valid_flag='1' and a.edit_time>b.START_TIME ) aa";
                oleExec.ExecSelect(insert_sql);

                //mfsyscserial
                delete_sql = $@"delete mfsyscserial where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);

                //insert_sql = $@"insert into mfsyscserial
                //                                    select 'CSE'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', sn,value,
                //                                    station,partno,CASE WHEN SCANTYPE in( 'AUTOAP','APTRSN') THEN LOCATION WHEN SCANTYPE='PN' and KP_NAME='PCBA' THEN KP_NAME || '-' || location  WHEN KP_NAME='PS' THEN 'POWER-'||seqno
                //                                    else  KP_NAME||'-'||seqno END  as eeecode,partno,detailseq,scantype,'CSERIALNO', '',value,edit_emp,to_char(edit_time,'yyyy/mm/dd hh24:mi:ss') as SCANDT,edit_emp,edit_time,'1',mpn,'' 
                //                                    from (select a.*,row_number() over(partition by a.partno order by a.id) seqno
                //                                    from r_sn_kp a,r_sn b where a.sn='{Serial}'  AND A.SN=B.SN AND a.R_SN_ID=b.id  and a.valid_flag=1 AND b.valid_flag=1 )";

                insert_sql = $@"insert into mfsyscserial
                                                    select 'CSE'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', sn,value,
                                                    station,partno,     CASE
                                                     WHEN SCANTYPE in ('AUTOAP', 'APTRSN') THEN
                                                      LOCATION
                                                     WHEN SCANTYPE = 'PN' and KP_NAME = 'PCBA' THEN
                                                      KP_NAME || '-' || location
                                                     WHEN KP_NAME = 'PS' THEN
                                                      'POWER-' || seqno
                                                     else
                                                      case when length(KP_NAME)>14 then substr(KP_NAME,0,13) || '-' || seqno else
						                        KP_NAME || '-' || seqno end
                                END as eeecode,partno,detailseq,scantype,'CSERIALNO', '',value,edit_emp,to_char(edit_time,'yyyy/mm/dd hh24:mi:ss') as SCANDT,edit_emp,edit_time,'1',mpn,'' 
                                                    from (select a.*,row_number() over(partition by a.partno order by a.id) seqno
                                                    from r_sn_kp a,r_sn b where a.sn='{Serial}'  AND A.SN=B.SN AND a.R_SN_ID=b.id  and a.valid_flag=1 AND b.valid_flag=1 )";


                oleExec.ExecSelect(insert_sql);
            }
            catch (Exception ex)
            {
                result = $@"SN:{Serial};SQL:{insert_sql};Error:{ex.Message}";
            }
            return result;
        }

        public void UpdateMES(string sn, OleExec oleExec)
        {
            string update_sql = $@"update mfsysproduct set Reseat=1 where Reseat=0 and sysserialno='{sn}' ";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);

            update_sql = $@"update mfsyscomponent set noreplacepart=1 where noreplacepart=0 and  sysserialno='{sn}'";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);

            update_sql = $@"update mfsyscserial set prodtype='OK' where ( prodtype is null or prodtype='' ) and sysserialno='{sn}' ";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);
        }
        public string InsertCZ(string sn, HWDNNSFCBase.OleExec cz_db, OleExec mes_db)
        {
            string result = "OK";
            string select_sql = "";
            string insert_sql = "";
            string delete_sql = "";
            DataTable data_table = null;
            try
            {
                //mfsysproduct_temp
                delete_sql = $@"delete mfsysproduct_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                DBTableP dbtable_product = new DBTableP("mfsysproduct_temp", cz_db);
                dbtable_product.myDBTYPE = 0;
                dbtable_product.analyse();
                select_sql = $@"select sysserialno,seqno,factoryid,routeid,customerid,workorderno,skuno,custpartno,eeecode,custssn,''firmware,
                                        software,servicetag,enetid,prioritycode,productfamily,productlevel,productcolor,productlangulage,
                                        shipcountry,productdesc,''orderno,''compcode,shipped,shipdate,location,whid,areaid,workordertype,packageno,systemstage,unitcost,lineseqno,reseatpre,reseat,reseatTag,lasteditby,lasteditdt,coo 
                                        from mfsysproduct where Reseat=0 and sysserialno='{sn}'  ";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_product.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }

                //mfsyscomponent_temp
                delete_sql = $@"delete mfsyscomponent_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                DBTableP dbtable_component = new DBTableP("mfsyscomponent_temp", cz_db);
                dbtable_component.myDBTYPE = 0;
                dbtable_component.analyse();
                select_sql = $@"select sysserialno,partno,version,seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,
                                        eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,
                                       substr(originalqty, 0,6) AS orig,unitcost,replacegroup,noreplacepart,lasteditby,lasteditdt 
                                        from mfsyscomponent  where noreplacepart=0 and  sysserialno='{sn}'";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_component.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }
                //mfsyscserial_temp
                delete_sql = $@"delete mfsyscserial_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                DBTableP dbtable_cserial = new DBTableP("mfsyscserial_temp", cz_db);
                dbtable_cserial.myDBTYPE = 0;
                dbtable_cserial.analyse();
                select_sql = $@"select sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,
                                        prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial 
                                        where ( prodtype is null or prodtype='' ) and sysserialno ='{sn}'";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_cserial.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }
            }
            catch (Exception ex)
            {
                result = $@"SN:{sn};SQL:{insert_sql};Error:{ex.Message}";
            }
            return result;
        }
      
        public void CheckSent()
        {
            string resent_data_excle = System.IO.Directory.GetCurrentDirectory() + "\\CZ_RESENT_DATA\\WAIT_RESNT\\SN_VN.XLSX";
            DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(resent_data_excle);
            OleExec oleExec = new OleExec(this._DBName1, false);
            string sql_sn = "";
            string sn = "";
            if (dt.Rows.Count == 0)
            {
                throw new Exception($@"ResentCZ.xlsx No Data Or Not Exists!");
            }
            if (dt.Rows[0][0].ToString().ToUpper().Trim() != "SN")
            {
                //throw new Exception($@"ResentCZ No SN Clomun!");
            }
            HWDNNSFCBase.OleExec oleExec2 = new HWDNNSFCBase.OleExec(this._DBName2, false);
            bool exists = true;
            for (var r = 0; r < dt.Rows.Count; r++)
            {
                exists = true;
                if (string.IsNullOrEmpty(dt.Rows[r][0].ToString()))
                {
                    continue;
                }
                sn = dt.Rows[r][0].ToString();
                sql_sn = $@"select* from mfsysproduct_temp where sysserialno='{sn}'";
                DataTable table = oleExec2.RunSelect(sql_sn).Tables[0];
                if (table.Rows.Count == 0)
                {
                    exists = false;
                }
                else
                {
                    sql_sn = $@"select* from mfsyscomponent_temp where sysserialno='{sn}'";
                    table = oleExec2.RunSelect(sql_sn).Tables[0];
                    if (table.Rows.Count == 0)
                    {
                        exists = false;
                    }
                    else
                    {
                        sql_sn = $@"select* from mfsyscserial_temp where sysserialno='{sn}'";
                        table = oleExec2.RunSelect(sql_sn).Tables[0];
                        if (table.Rows.Count == 0)
                        {
                            exists = false;
                        }
                        else
                        {
                            sql_sn = $@"select* from WWN_Datasharing_temp where (vssn='{sn}' or cssn='{sn}')";
                            table = oleExec2.RunSelect(sql_sn).Tables[0];
                            if (table.Rows.Count == 0)
                            {
                                exists = false;
                            }                            
                        }
                    }
                }

                if (exists)
                {
                    //sql_sn = $@"
                    //         select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D where a.dn_no=b.vbeln  AND A.SN=C.SN AND C.VALUE=D.SN
                    //         and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' and a.sn='{sn}' 
                    //         UNION
                    //         select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D,R_SN_KP E where a.dn_no=b.vbeln  
                    //         AND A.SN=C.SN AND C.VALUE=E.SN AND E.VALUE=D.SN
                    //         and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' AND E.VALID_FLAG='1'
                    //         and a.sn='{sn}'  ";
                    //table = oleExec.RunSelect(sql_sn).Tables[0];
                    //if (table.Rows.Count > 0)
                    //{
                    //    for (var i = 0; i < table.Rows.Count; i++)
                    //    {
                    //        sql_sn = $@"select* from mfsysproduct_temp where sysserialno='{table.Rows[i]["SN"].ToString()}'";
                    //        DataTable tba = oleExec2.RunSelect(sql_sn).Tables[0];
                    //        if (tba.Rows.Count == 0)
                    //        {
                    //            exists = false;
                    //        }
                    //        else
                    //        {
                    //            sql_sn = $@"select* from mfsyscomponent_temp where sysserialno='{table.Rows[i]["SN"].ToString()}'";
                    //            tba = oleExec2.RunSelect(sql_sn).Tables[0];
                    //            if (tba.Rows.Count == 0)
                    //            {
                    //                exists = false;
                    //            }
                    //            else
                    //            {
                    //                sql_sn = $@"select* from mfsyscserial_temp where sysserialno='{table.Rows[i]["SN"].ToString()}'";
                    //                tba = oleExec2.RunSelect(sql_sn).Tables[0];
                    //                if (tba.Rows.Count == 0)
                    //                {
                    //                    exists = false;
                    //                }
                    //            }
                    //        }
                    //    }                        
                    //}
                }
                else
                {
                    RecodeLocalLog(sn);
                }
            }

        }
        private void RecodeLocalLog(string msg)
        {
            string logPath = System.IO.Directory.GetCurrentDirectory() + "\\SN\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string logFile = System.IO.Directory.GetCurrentDirectory() + "\\SN\\SN_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
            //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Current);
            if (msg == "")
            {
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine(msg);
            }
            sw.Flush();
            sw.Close();
        }
        private string _name;

        // Token: 0x04000085 RID: 133
        private string _strSql;

        // Token: 0x04000086 RID: 134

        // Token: 0x04000087 RID: 135
        private string[] _mailList;

        // Token: 0x04000088 RID: 136
        private string[] _mailTittle;

        // Token: 0x04000089 RID: 137
        private bool useExcelFile;

        // Token: 0x0400008A RID: 138
        public string _TimeSpan;

        // Token: 0x0400008B RID: 139
        private string _DBName1;

        // Token: 0x0400008C RID: 140
        private string _DBName2;

        // Token: 0x0400008D RID: 141
        private string _strExecSQL = "";
    }
}
