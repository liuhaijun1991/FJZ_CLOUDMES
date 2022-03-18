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

namespace MESInterface.DCN
{
    class CheckSendCZDataProcess : taskBase
    {
        public OleExec SFCDB = null;
        MESPubLab.MESStation.MESStationBase station = new MESPubLab.MESStation.MESStationBase();
        public override void init()
        {
            //base.init();
            try
            {

                this._MESDB = ConfigGet("DBKey1");
                this._CZDB = ConfigGet("DBKey2");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 
        public override void Start()
        {
            OleExec oleExec = new OleExec(this._MESDB, false);
            OleExec oleExec2 = new OleExec(this._CZDB, false);

            string strSQL = "";
            oleExec.ExecSQL(strSQL);
            strSQL = $@"  SELECT *FROM r_sn WHERE sn IN(SELECT distinct Data2 FROM r_sn_log  WHERE logtype='SendCZData_CheckSN' AND Data9='N')  and VALID_FLAG='1'
                                        Union 
                                        select* from r_sn where sn in(
                                        select distinct A.SN  From r_ship_detail a,sd_to_detail b 
                                        where a.dn_no=b.vbeln and b.land1='CZ'  and a.SHIPDATE >sysdate-1)  and VALID_FLAG='1'
                                        Union 
                                        select* from r_sn where sn IN 
                                        (select distinct D.SN From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D 
                                        where a.dn_no=b.vbeln  AND A.SN=C.SN AND C.VALUE=D.SN and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' and a.SHIPDATE >sysdate-1) and VALID_FLAG='1'
                                        Union 
                                        select* from r_sn where sn in  
                                        (select distinct D.SN  From r_ship_detail a,sd_to_detail b,
                                        R_SN_KP C,R_SN D,R_SN_KP E where a.dn_no=b.vbeln AND A.SN=C.SN AND C.VALUE=E.SN AND E.VALUE=D.SN and b.land1='CZ' 
                                        AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' AND E.VALID_FLAG='1' and a.SHIPDATE >sysdate-1) and VALID_FLAG='1'";
            DataSet ds1 = oleExec.RunSelect(strSQL);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    string _sn = ds1.Tables[0].Rows[i]["SN"].ToString();
                    strSQL = "select*From mfsysproduct_temp where sysserialno='" + _sn + "'";
                    DataSet ds2 = oleExec2.RunSelect(strSQL);
                    if (ds2.Tables[0].Rows.Count == 0)
                    {
                        strSQL = "SELECT * FROM r_sn_log (NOLOCK)  WHERE logtype='SendCZData_CheckSN' AND Data2='" + _sn + "'";
                        DataSet dss = oleExec.RunSelect(strSQL);
                        if (dss.Tables[0].Rows.Count == 0)
                        {
                            T_R_SN_LOG _SN_LOG = new T_R_SN_LOG(station.SFCDB, station.DBType);
                            string SID = _SN_LOG.GetNewID(station.BU, station.SFCDB);

                            string strsqllog = "INSERT INTO r_sn_log (ID,logtype,CREATETIME,DATA1,DATA2,DATA9) VALUES ('"+ SID + "','SendCZData_CheckSN',sysdate,'','" + _sn + "','N')";
                            oleExec.ExecSQL(strsqllog);
                        }

                    }
                    else
                    {
                        strSQL = "select*From r_sn_log where data2='" + _sn + "' ";
                        DataSet ds3 = oleExec.RunSelect(strSQL);
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            strSQL = "update  r_sn_log set data9='Y' where data2='" + _sn + "' ";
                            oleExec.ExecSQL(strSQL);

                        }

                    }

                }
            }
        }
        private string _MESDB;
        private string _CZDB;
    }
}
