using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Module.ARUBA;
using MESDataObject.Module.DCN.ARUBA;

namespace MESInterface.DCN
{
    public class ArubaB2BSyn : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, b2bdbstr, bustr;
        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                b2bdbstr = ConfigGet("LHB2BDB");
                bustr = ConfigGet("BU");

            }
            catch (Exception)
            {
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                Syn850();
                Syn860();
                Anal860();
                Syn824();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        void Syn850()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(this.b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_HPE_EDI_850>().Where(t => t.F_LASTEDIT_DT > DateTime.Now.AddDays(-5)).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<HPE_EDI_850>().Where(t => filterIDs.Contains(t.F_ID)).ToList();
                        foreach (var item in waitsynlist)
                        {
                            if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                continue;
                            var targetobj = ObjectDataHelper.Mapper<HPE_EDI_850, B2B_HPE_EDI_850>(item);
                            targetobj.F_LINE_PRICE = Convert.ToDouble(item.F_LINE_PRICE);
                            targetobj.ID = MesDbBase.GetNewID<HPE_EDI_850>(mesdb, bustr);
                            targetobj.EDIT_TIME = DateTime.Now;
                            targetobj.FLAG = "0";

                            var orderobj = ObjectDataHelper.Mapper<HPE_EDI_ORDER, HPE_EDI_850>(targetobj);
                            orderobj.ID = MesDbBase.GetNewID<HPE_EDI_ORDER>(mesdb, bustr);
                            orderobj.EDITYPE = "850";
                            orderobj.SOURCEID = targetobj.ID;

                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                var cumain = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.UPOID == $@"{targetobj.F_PO}{targetobj.F_LINE}").ToList().FirstOrDefault();

                                var poid = string.Empty;
                                if (cumain == null)
                                {
                                    var ordermain = new O_ORDER_MAIN()
                                    {
                                        ID = MesDbBase.GetNewID<O_ORDER_MAIN>(mesdb, bustr),
                                        UPOID = $@"{targetobj.F_PO}{targetobj.F_LINE}",
                                        PONO = targetobj.F_PO,
                                        POLINE = targetobj.F_LINE,
                                        QTY = targetobj.F_LINE_QTY,
                                        UNITPRICE = targetobj.F_LINE_PRICE.ToString(),
                                        PID = targetobj.F_PN,
                                        //CUSTPID = targetobj.F_PN,
                                        DELIVERY = targetobj.F_SCH_DR_DATE,
                                        COMPLETED = ENUM_O_ORDER_MAIN.COMPLETED_NO.ExtValue(),
                                        CLOSED = ENUM_O_ORDER_MAIN.CLOSED_NO.ExtValue(),
                                        CANCEL = ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue(),
                                        PREASN = ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue(),
                                        FINALASN = ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue(),
                                        CUSTOMER = Customer.ARUBA.ExtValue(),
                                        VERSION = "0",
                                        POTYPE = targetobj.F_PO_TYPE,
                                        CREATETIME = DateTime.Now,
                                        EDITTIME = DateTime.Now,
                                        ITEMID = orderobj.ID,
                                        ORIGINALID = string.Empty,
                                        POCREATETIME = targetobj.F_PO_DATE,
                                        PLANT = targetobj.F_SITE,
                                        LASTCHANGETIME = targetobj.F_PO_DATE.ToString("yyyy-MM-dd HH:mm:ss")
                                    };
                                    poid = ordermain.ID;
                                    mesdb.Insertable(ordermain).ExecuteCommand();
                                }
                                else
                                {
                                    poid = cumain.ID;
                                    cumain.ITEMID = orderobj.ID;
                                    mesdb.Updateable(cumain).ExecuteCommand();

                                }
                                if (exsist.FindAll(t => t.F_ID == item.F_ID).Count == 0)
                                { 
                                    mesdb.Insertable(targetobj).ExecuteCommand();
                                    mesdb.Insertable(orderobj).ExecuteCommand();
                                }
                                
                                #region PO status
                                var cpostatus = new O_PO_STATUS()
                                {
                                    ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.ARUBA.ExtValue()),
                                    STATUSID = ENUM_ARUBA_PO_STATUS.WaitCommit.ExtValue(),
                                    VALIDFLAG = MesBool.Yes.ExtValue(),
                                    CREATETIME = DateTime.Now,
                                    EDITTIME = DateTime.Now,
                                    POID = poid
                                };
                                mesdb.Insertable(cpostatus).ExecuteCommand();
                                #endregion
                            });
                            if (!dbres.IsSuccess)
                                throw new Exception(dbres.ErrorMessage);
                        }
                    }
            }
        }

        void Syn850_old()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(this.b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_HPE_EDI_850>().Where(t => t.F_LASTEDIT_DT > DateTime.Now.AddDays(-15)).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<HPE_EDI_850>().Where(t => filterIDs.Contains(t.F_ID)).ToList();
                        foreach (var item in waitsynlist)
                        {
                            if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                continue;
                            var targetobj = ObjectDataHelper.Mapper<HPE_EDI_850, B2B_HPE_EDI_850>(item);
                            targetobj.F_LINE_PRICE = Convert.ToDouble(item.F_LINE_PRICE);
                            targetobj.ID = MesDbBase.GetNewID<HPE_EDI_850>(mesdb, bustr);
                            targetobj.EDIT_TIME = DateTime.Now;
                            targetobj.FLAG = "0";

                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                mesdb.Insertable(new R_ORDER_MAIN()
                                {
                                    ID = MesDbBase.GetNewID<HPE_EDI_850>(mesdb, bustr),
                                    CREATETIME = DateTime.Now,
                                    DELIVERYDATE = targetobj.F_SCH_DR_DATE,
                                    PO = targetobj.F_PO,
                                    POLINE = targetobj.F_LINE,
                                    POQTY = Convert.ToInt32(targetobj.F_LINE_QTY),
                                    STATUSID = ENUM_ORDER_STATUS.WaitPoCommit.Ext<EnumValueAttribute>().Description,
                                    CUSTOMER = Customer.ARUBA.Ext<EnumValueAttribute>().Description,
                                    FINISHED = ENUM_R_ORDER_MAIN.Finished_N.Ext<EnumValueAttribute>().Description,
                                    PREWOFLAG = ENUM_R_ORDER_MAIN.PreWo_N.Ext<EnumValueAttribute>().Description,
                                    UNITPRICE = targetobj.F_LINE_PRICE,
                                    SKUNO = targetobj.F_PN,
                                    POID = targetobj.F_ID
                                }).ExecuteCommand();
                                mesdb.Insertable(targetobj).ExecuteCommand();
                            });
                            if (!dbres.IsSuccess)
                                throw new Exception(dbres.ErrorMessage);
                        }
                    }
            }


        }
        
        void Syn860()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(this.b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_HPE_EDI_860>().Where(t => t.F_LASTEDIT_DT > DateTime.Now.AddDays(-5))
                    .OrderBy(t => t.F_PO, OrderByType.Desc).OrderBy(t => t.F_LINE, OrderByType.Asc).OrderBy(t => t.F_LASTEDIT_DT, OrderByType.Desc).OrderBy(t => t.F_ID, OrderByType.Desc).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<HPE_EDI_860>().Where(t => filterIDs.Contains(t.F_ID)).ToList();
                        foreach (var item in waitsynlist)
                        {
                            if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                continue;
                            var targetobj = ObjectDataHelper.Mapper<HPE_EDI_860, B2B_HPE_EDI_860>(item);
                            targetobj.ID = MesDbBase.GetNewID<HPE_EDI_860>(mesdb, bustr);
                            targetobj.CREATETIME = DateTime.Now;
                            targetobj.FLAG = aruba860.init.ExtValue();
                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                mesdb.Insertable(targetobj).ExecuteCommand();
                                var orderobj = ObjectDataHelper.Mapper<HPE_EDI_ORDER, HPE_EDI_860>(targetobj);
                                orderobj.ID = MesDbBase.GetNewID<HPE_EDI_ORDER>(mesdb, bustr);
                                orderobj.EDITYPE = "860";
                                orderobj.EDIT_TIME = targetobj.CREATETIME;
                                orderobj.SOURCEID = targetobj.ID;
                                mesdb.Insertable(orderobj).ExecuteCommand();
                            });
                            if (!dbres.IsSuccess)
                                throw new Exception(dbres.ErrorMessage);
                        }
                    }
            }
        }

        void Anal860()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
            {
                var waitsynlist = mesdb.Queryable<HPE_EDI_ORDER>().Where(t => t.FLAG == aruba860.init.ExtValue() && t.EDITYPE =="860")
                    .OrderBy(t => t.F_PO, OrderByType.Desc).OrderBy(t => t.F_LINE, OrderByType.Asc).OrderBy(t => t.F_LASTEDIT_DT, OrderByType.Desc).OrderBy(t => t.F_ID, OrderByType.Desc).ToList();
                foreach (var item in waitsynlist)
                {
                    var main = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == item.F_PO && t.POLINE == item.F_LINE && t.CUSTOMER == Customer.ARUBA.ExtValue()).ToList().FirstOrDefault();
                    var h860list = mesdb.Queryable<HPE_EDI_860>().Where(t => t.F_PO == item.F_PO && t.F_LINE == item.F_LINE).ToList();
                    var last860 = mesdb.Queryable<HPE_EDI_860>().Where(t => t.F_PO == item.F_PO && t.F_LINE == item.F_LINE).OrderBy(t => t.F_PO, OrderByType.Desc).OrderBy(t => t.F_LINE, OrderByType.Asc)
                        .OrderBy(t => t.F_ID, OrderByType.Desc).ToList().FirstOrDefault();

                    var dbres = mesdb.Ado.UseTran(() =>
                    {
                        if (main == null)
                        {
                            item.FLAG = aruba860.finish.ExtValue();
                            mesdb.Updateable(item).ExecuteCommand();
                        }
                        else if (int.Parse(item.F_ID.ToString()) < int.Parse(last860.F_ID))
                        {
                            item.FLAG = aruba860.skip.ExtValue();
                            mesdb.Updateable(item).ExecuteCommand();
                        }
                        else
                        {
                            item.FLAG = aruba860.waitmail.ExtValue();
                            main.DELIVERY = item.F_SCH_DR_DATE;
                            main.QTY = item.F_LINE_QTY;
                            main.UNITPRICE = item.F_LINE_PRICE.ToString();
                            main.VERSION = h860list.Count().ToString();
                            main.ORIGINALITEMID = main.ITEMID;
                            main.ITEMID = item.ID;
                            main.EDITTIME = DateTime.Now;
                            #region PO status
                            mesdb.Updateable<O_PO_STATUS>().SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue() }).Where(t => t.POID == main.ID && t.VALIDFLAG==MesBool.Yes.ExtValue()).ExecuteCommand();
                            var cpostatus = new O_PO_STATUS()
                            {
                                ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.ARUBA.ExtValue()),
                                STATUSID = ENUM_ARUBA_PO_STATUS.WaitCommit.ExtValue(),
                                VALIDFLAG = MesBool.Yes.ExtValue(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                POID = main.ID
                            };

                            if (item.F_PC_CODE == aruba860status.cancel.ExtValue())
                            {
                                cpostatus.STATUSID = ENUM_ARUBA_PO_STATUS.Cancel.ExtValue();
                                main.CANCEL = MesBool.Yes.ExtValue();
                                main.CANCELTIME = item.F_LASTEDIT_DT;
                            }
                            mesdb.Insertable(cpostatus).ExecuteCommand();
                            mesdb.Updateable(item).ExecuteCommand();
                            mesdb.Updateable(main).ExecuteCommand();
                            #endregion
                        }
                    });
                    if (!dbres.IsSuccess)
                        throw new Exception(dbres.ErrorMessage);
                }
            }
        }
        
        void Syn824()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(this.b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_HPE_EDI_824>()//.Where(t => t.F_LASTEDITDT > DateTime.Now.AddDays(-3))
                    .OrderBy(t => t.F_ID, OrderByType.Desc).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<HPE_EDI_824>().Where(t => IMesDbEx.OracleContain(t.F_ID, filterIDs)).ToList();
                        foreach (var item in waitsynlist)
                        {
                            if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                continue;
                            var targetobj = ObjectDataHelper.Mapper<HPE_EDI_824, B2B_HPE_EDI_824>(item);
                            targetobj.ID = MesDbBase.GetNewID<HPE_EDI_824>(mesdb, bustr);
                            targetobj.CREATETIME = DateTime.Now;
                            mesdb.Insertable(targetobj).ExecuteCommand();
                        }
                    }
            }
        }



    }
}
