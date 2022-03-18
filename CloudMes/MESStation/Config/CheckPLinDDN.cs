using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    class CheckPLinDDN : MesAPIBase 
    {
        private APIInfo addCheckPalletinDN = new APIInfo()
        {
            FunctionName = "AddCheckPalletinDN",
            Description = "AddCheckPalletinDN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="dn",InputType="string",DefaultValue=""},
                 new APIInputInfo() { InputName="pallet",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo checkSnInPallet = new APIInfo()
        {
            FunctionName = "CheckSnInPallet",
            Description = "CheckSnInPallet",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="dn",InputType="string",DefaultValue=""},
                 new APIInputInfo() { InputName="pallet",InputType="string",DefaultValue=""},
                  new APIInputInfo() { InputName="sn",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo getListDNandPL = new APIInfo()
        {
            FunctionName = "GetListDNandPL",
            Description = "GetListDNandPL",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="dn",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo resetPLinDN = new APIInfo()
        {
            FunctionName = "ResetPLinDN",
            Description = "resetPLinDN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="dn",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        public CheckPLinDDN()
        {
            this.Apis.Add(getListDNandPL.FunctionName, getListDNandPL);
            this.Apis.Add(addCheckPalletinDN.FunctionName, addCheckPalletinDN);
            this.Apis.Add(resetPLinDN.FunctionName, resetPLinDN);
            this.Apis.Add(checkSnInPallet.FunctionName, checkSnInPallet);

        }
        public void GetListDNandPL(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string dn_no = Data["DN_NO"].ToString();
                string sqlcheck, sql;
                sqlcheck = $@"  select * from r_sn where sn IN (
                                SELECT sn FROM SFCRUNTIME.R_SHIP_DETAIL WHERE DN_NO = '{dn_no}')  AND NEXT_STATION='SHIPFINISH'";
                DataTable dt = sfcdb.RunSelect(sqlcheck).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"This is {dn_no} not next_station SHIPFINISH";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                sql = $@" SELECT * FROM  (SELECT rsd.DN_NO ,
				                        RP1.PACK_NO PALLET,
                                        rp1.PACK_NO PALLETCHECK,
				                        'OK' OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2,
		                                r_sn_packing rsp,
		                                r_sn   rn,
		                                r_ship_detail rsd,
                                        R_MES_LOG LOG
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND
                                       rsd.SN= rn.SN          AND
                                       rsd.DN_NO ='{dn_no}' AND
                                       RN.VALID_FLAG = 1 AND
                                         LOG.DATA2=RP1.PACK_NO AND LOG.PROGRAM_NAME='CHECKPALLETINDN' AND LOG.DATA1= RSD.DN_NO
                                         UNION
                                       SELECT DISTINCT rsd.DN_NO, RP1.PACK_NO PALLET, '' PALLETCHECK,'' OK
                                  FROM R_PACKING           RP1,
                                       R_PACKING RP2,
                                       R_SN                RN,
                                       R_SN_PACKING RSP,
                                       R_SHIP_DETAIL rsd
                                 WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND
                                       rsd.SN= rn.SN          AND
                                       rsd.DN_NO ='{dn_no}' AND
                                     rsd.DN_NO not in (select DATA1 from R_MES_LOG rssd where rssd.DATA1 = rsd.DN_NO AND rssd.DATA2=RP1.PACK_NO  and RSSD.PROGRAM_NAME = 'CHECKPALLETINDN')) a ORDER BY a.PALLET";
                DataTable dt1 = sfcdb.RunSelect(sql).Tables[0];
                if (dt1.Rows.Count != 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000026";
                    StationReturn.Message = "獲取成功";
                    StationReturn.Data = dt1;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"This is {dn_no} not exist";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }

            }
            catch (Exception e)
            {
                throw e ;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void AddCheckPalletinDN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string dn_no = Data["DN_NO"].ToString();
                string pallet = Data["pallet"].ToString();
                string sqlcheck;
                sqlcheck = $@"SELECT * FROM R_SHIP_DETAIL WHERE sn IN (
                                     SELECT sn FROM r_sn WHERE id IN (
                                     SELECT SN_ID FROM r_sn_packing WHERE PACK_ID IN (
                                     SELECT ID FROM r_packing WHERE PARENT_PACK_ID IN (
                                     SELECT ID FROM r_packing WHERE PACK_NO='{pallet}')))) AND DN_NO ='{dn_no}'";
                DataTable dt = sfcdb.RunSelect(sqlcheck).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"This is {pallet} not in {dn_no}";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                else
                {
                    R_MES_LOG log = new R_MES_LOG();
                    T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                    log.ID = TRML.GetNewID(BU, sfcdb);
                    log.PROGRAM_NAME = "CHECKPALLETINDN";
                    log.CLASS_NAME = "MESStation.Config.CheckPLinDDN";
                    log.FUNCTION_NAME = "AddCheckPalletinDN";
                    log.DATA1 = dn_no;
                    log.DATA2 = pallet;
                    log.EDIT_EMP = LoginUser.EMP_NO;
                    log.EDIT_TIME = TRML.GetDBDateTime(sfcdb);
                    TRML.InsertMESLogOld(log, sfcdb);
                    string StrCount = "0";
                    StationReturn.Data = StrCount;
                    StationReturn.Message = "OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void CheckSnInPallet(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                string dn_no = Data["DN_NO"].ToString();
                string pallet = Data["pallet"].ToString();
                string sn = Data["sn"].ToString();
                string sqlcheck;
                sqlcheck = $@"SELECT * FROM r_sn a , r_sn_packing b, r_packing c, r_packing d WHERE a.ID=b.SN_ID AND  b.PACK_ID=c.ID AND c.PARENT_PACK_ID=d.ID AND a.SN='{sn}' AND d.PACK_NO='{pallet}'";
                DataTable dt = sfcdb.RunSelect(sqlcheck).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"This is {sn} not in {pallet}";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                else
                {
                    string StrCount = "0";
                    StationReturn.Data = StrCount;
                    StationReturn.Message = "OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void ResetPLinDN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string dn_no = Data["DN_NO"].ToString();
                string sqlcheck;
                sqlcheck = $@"	delete R_MES_LOG WHERE DATA1='{dn_no}' AND  PROGRAM_NAME ='CHECKPALLETINDN'";
                int a = sfcdb.ExecSqlNoReturn(sqlcheck,null);
                StationReturn.Status = StationReturnStatusValue.Pass;
               
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
