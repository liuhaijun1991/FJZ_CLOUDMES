using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using SqlSugar;
using MESPubLab.Common;
using System.Configuration;

namespace MESStation.Config
{
    public class OnlineCardConfig : MesAPIBase
    {
        protected APIInfo FGetOpenCardList = new APIInfo()
        {
            FunctionName = "GetOpenCardList",
            Description = "GetOpenCardList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddCard = new APIInfo()
        {
            FunctionName = "AddCard",
            Description = "AddCard",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetCardNo = new APIInfo()
        {
            FunctionName = "GetCardNo",
            Description = "GetCardNo",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSingelCardByCardNo = new APIInfo()
        {
            FunctionName = "GetSingelCardByCardNo",
            Description = "GetSingelCardByCardNo",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditCard = new APIInfo()
        {
            FunctionName = "EditCard",
            Description = "EditCard",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        public OnlineCardConfig()
        {
            this.Apis.Add(FGetOpenCardList.FunctionName, FGetOpenCardList);
            this.Apis.Add(FAddCard.FunctionName, FAddCard);
            this.Apis.Add(FGetCardNo.FunctionName, FGetCardNo);
            this.Apis.Add(FGetSingelCardByCardNo.FunctionName, FGetSingelCardByCardNo);
            this.Apis.Add(FEditCard.FunctionName, FEditCard);
        }


        public void GetOpenCardList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = SFCDB.ORM.Queryable<R_ONLINECAR>().ToList();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(res.Count);
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetCardNo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = SFCDB.ORM.Ado.GetScalar($@"select COUNT(1) NUMS from r_onlinecar where to_char(createtime,'YYYY-MM-DD')  =to_char(SYSDATE,'YYYY-MM-DD')").ToString().PadLeft(3,'0');
                res = $@"B{DateTime.Now.ToString("yyyyMMdd")}{res}";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Pass";
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void GetSingelCardByCardNo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var cardNo = Data["CardNo"].ToString();
                var res = SFCDB.ORM.Queryable<R_ONLINECAR>().Where(t=>t.CARNO == cardNo).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Pass";
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void AddCard(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var model = Data["Model"].ToObject<R_ONLINECAR>();
                if (SFCDB.ORM.Queryable<R_ONLINECAR>().Any(t => t.CARNO == model.CARNO))
                    throw new Exception("CardNo is Exists,pls refresh!");
                model.ID = MesDbBase.GetNewID<R_ONLINECAR>(SFCDB.ORM, this.BU);
                model.CREATETIME = DateTime.Now;
                model.CREATEBY = LoginUser.EMP_NO;
                model.EDITTIME= DateTime.Now;
                model.EDITBY = LoginUser.EMP_NO;
                model.ESTATUS = 1;
                model.OKTORELEASE = 0;
                model.CURRENTDEP = "PQE";
                var res = SFCDB.ORM.Insertable(model).ExecuteCommand();

                #region  send mail
                //sendmail(model, "IT", SFCDB);
                sendmail(model,"PQE,IPQC",SFCDB);
                #endregion

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        void sendmail(R_ONLINECAR model, string department, OleExec SFCDB)
        {
            var departs = department.Split(',');
            var maillist = SFCDB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE>((cu, cur, cr) => cu.ID == cur.USER_ID && cur.ROLE_ID == cr.ID).Where((cu, cur, cr) => departs.Contains(cr.ROLE_NAME))
                 .GroupBy((cu, cur, cr) => cu.MAIL_ADDRESS).Select((cu, cur, cr) => cu.MAIL_ADDRESS).ToList();
            if (maillist.Count == 0)
                return;
            var res = new MESPubLab.Common.MesMail().SendNotes(new MesMail.MailObj()
            {
                MailTitle = $@"Attention!Online CAR issue!Please Analysis immediately!-{DateTime.Now.ToString()}",
                MailBody = $@"Dear {department},<br><br>
                            CAR No:   { model.CARNO}<br><br> 
                            CAR Type:   {(OnlineCardType)Convert.ToInt32(model.CARTYPE)}<br><br>
                            CAR Title:   {model.CARTITLE}<br><br>
                            Sku Name:   {model.SKUNAME} <br><br>
                            SkuNo:   {model.SKUNO}  <br><br>
                            Total Product Qty:   {model.SKUQTY} <br><br>
                            Total Fail Qty:   {model.FAILQTY} <br><br>
                            Failure Description:<br>
                               {model.FAILTEXT}<br><br>
                            has been send  by {model.FAILPNAME}, Need you Analysis immediately,\n please Click:{ConfigurationManager.AppSettings["RunServerAddress"]}<br><br> this mail sent by system ,need not response.Any question please connect IT MES <br><br>",
                MailTo = maillist.ToArray()
            });
            if (res.ToString() != "OK")
                throw new Exception("sendmail Err!");
        }

        void checkpermissions(string[] department, OleExec SFCDB)
        {
            var departmentres = SFCDB.ORM.Queryable<C_USER>().Where(t=> department.Contains(t.DPT_NAME) ).Any();
            if (departmentres)
                return;
            var res = SFCDB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE>((cu, cur, cr) => cu.ID == cur.USER_ID && cur.ROLE_ID == cr.ID).Where((cu, cur, cr) => cu.EMP_NO == LoginUser.EMP_NO && department.Contains(cr.ROLE_NAME) ).Select((cu, cur, cr) => cu).Any();
            if (!res)
                throw new Exception("You don't have access!");

        }

        /// <summary>
        /// PQE,IPQC,QC,QA-LEADER
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void EditCard(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var model = Data["Model"].ToObject<R_ONLINECAR>();
                var maildepartment = string.Empty;
                switch (model.ESTATUS)
                {
                    case 1:
                        checkpermissions(new string[] { "PQE" }, SFCDB);
                        if (model.OKTORELEASE == 0)
                        {
                            model.ESTATUS = 6;
                            model.ISCANCEL = Convert.ToInt32(true).ToString();
                            maildepartment = "PQE,IPQC";
                        }
                        else
                        {
                            maildepartment = model.FAILDEPART;
                            model.ESTATUS++;
                        }
                        model.CONFIRMDATE = DateTime.Now;
                        break;
                    case 2:
                        maildepartment = model.ROOTDEPART;
                        model.ESTATUS++;
                        break;
                    case 3:
                        maildepartment = checkAddDept(model);
                        if(string.IsNullOrEmpty(maildepartment))
                        {
                            maildepartment = "QC";
                            model.ESTATUS++;
                        }
                        break;
                    case 4:
                        maildepartment = "QA-LEADER";
                        model.ESTATUS++;
                        break;
                    case 5:
                        if (model.PROMIT != "1")
                        {
                            SFCDB.ORM.Insertable(new R_ONLINECAR()
                            {
                                ID = MesDbBase.GetNewID<R_ONLINECAR>(SFCDB.ORM, this.BU),
                                CARNO = new Func<string>(() =>
                                {
                                    var tempcarno = SFCDB.ORM.Ado.GetScalar($@"select COUNT(1) NUMS from r_onlinecar where to_char(createtime,'YYYY-MM-DD')  =to_char(SYSDATE,'YYYY-MM-DD')").ToString().PadLeft(3, '0');
                                    return $@"B{DateTime.Now.ToString("yyyyMMdd")}{tempcarno}";
                                })(),
                                CARTYPE = model.CARTYPE,
                                SKUNAME = model.SKUNAME,
                                OKTORELEASE=0,
                                SKUNO = model.SKUNO,
                                PRODUCTIONLINE = model.PRODUCTIONLINE,
                                STATION = model.STATION,
                                FINDTIME = model.FINDTIME,
                                TOTIME = model.TOTIME,
                                SKUQTY = model.SKUQTY,
                                FAILQTY = model.FAILQTY,
                                FAILTEXT = $@"{model.FAILTEXT} Reject CAR NO : {model.CARNO} {model.REMARK}",
                                ATTACHFILE1 = model.ATTACHFILE1,
                                FAILPNAME = model.FAILPNAME,
                                BUNAME = model.BUNAME,
                                ISSUEDATE = DateTime.Now,
                                LASTEDITDT = DateTime.Now,
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                CREATEBY = "SYS",
                                EDITBY = "SYS",
                                ESTATUS=1
                            }).ExecuteCommand();
                            maildepartment = "PQE,IPQC";
                        }
                        model.ESTATUS++;
                        break;
                    case 6:
                        model.ESTATUS++;
                        break;
                    default:
                        model.ESTATUS++;
                        break;
                }
                if(!string.IsNullOrEmpty(maildepartment))
                    sendmail(model, maildepartment, SFCDB);
                model.EDITTIME = DateTime.Now;
                model.EDITBY = LoginUser.EMP_NO;
                model.CURRENTDEP = maildepartment;
                var res = SFCDB.ORM.Updateable(model).ExecuteCommand();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// 返回莫填写部门集合,用逗号分开
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string checkAddDept(R_ONLINECAR model)
        {
            var res = string.Empty;
            if (!String.IsNullOrEmpty(model.AUDITDEPART1) && String.IsNullOrEmpty(model.AUDITNAME1))
                res += $@"{ model.AUDITDEPART1},";
            if (!String.IsNullOrEmpty(model.AUDITDEPART2) && String.IsNullOrEmpty(model.AUDITNAME2))
                res += $@"{ model.AUDITDEPART2},";
            if (!String.IsNullOrEmpty(model.AUDITDEPART3) && String.IsNullOrEmpty(model.AUDITNAME3))
                res += $@"{ model.AUDITDEPART3},";
            if (!String.IsNullOrEmpty(model.AUDITDEPART4) && String.IsNullOrEmpty(model.AUDITNAME4))
                res += $@"{ model.AUDITDEPART4},";
            if (!String.IsNullOrEmpty(model.AUDITDEPART5) && String.IsNullOrEmpty(model.AUDITNAME5))
                res += $@"{ model.AUDITDEPART5},";
            return res;
        }
    }
    public enum OnlineCardType
    {
        Under_FPY_Goal=1,
        Over_DPPM_Goal=2,
        F_3pcs_same_failed_out_of_20_pcs=4,
        OBA_reject=5,
        Machine_Failed=6,
        Reject_by_QA_Supervisor=7,
        Customer_Complain=9,
        CQA_reject=10,
        Other=11
    }
}
