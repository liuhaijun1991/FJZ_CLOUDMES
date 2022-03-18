using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.SNMaker;
using MESPubLab.Common;
using SqlSugar;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json.Linq;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Module.DCN;
using MESJuniper.Base;

namespace MESJuniper.SendData
{
    public class JuniperDOAShipment
    {
        SqlSugarClient _db;
        string _bu;
        string _user;
        string FileNamePreFix = "FGSHIP_FC";
        public JuniperDOAShipment(SqlSugarClient db, string bu, string emp_no)
        {
            _db = db;
            _bu = bu;
            _user = emp_no;
        }

        /// <summary>
        /// User上传客户回复的数据
        /// </summary>
        /// <param name="data"></param>
        public void UploadReply(JArray data)
        {
            DateTime dt = DateTime.Now;
            List<R_JNP_DOA_SHIPMENTS_ACK> acks = new List<R_JNP_DOA_SHIPMENTS_ACK>();
            for (int i = 0; i < data.Count; i++)
            {
                var ack = new R_JNP_DOA_SHIPMENTS_ACK()
                {
                    ID = MesDbBase.GetNewID<R_JNP_DOA_SHIPMENTS_ACK>(_db, _bu),
                    FILE_PO = data[i]["File PO"].ToString().Trim(),
                    FILE_PO_LINE = data[i]["File PO Line"].ToString().Trim(),
                    FILE_QTY = int.Parse(data[i]["File Qty"].ToString().Trim()),
                    MODEL = data[i]["Model"].ToString().Trim(),
                    SERIAL = data[i]["Serial"].ToString().Trim(),
                    DELIVERYNUMBER = (data[i]["Delivery Number"] != null ? data[i]["Delivery Number"].ToString().Trim() : ""),
                    DNLINE = (data[i]["DN Line"] != null ? data[i]["DN Line"].ToString().Trim() : ""),
                    EQUIPMENT = data[i]["Equipment"].ToString().Trim(),
                    USER_STATUS = data[i]["User Status"].ToString().Trim(),
                    IB_DELIVERY = data[i]["IB Delivery"].ToString().Trim(),
                    MESSAGE_CODE = data[i]["Message Code"].ToString().Trim(),
                    MESSAGE_TEXT = data[i]["Message Text"].ToString().Trim(),
                    CREATEBY = _user,
                    CREATETIME = dt
                };
                acks.Add(ack);
            }
            var records = acks.Select(t => t.FILE_PO + t.FILE_PO_LINE).ToList();
            var shipments = _db.Queryable<R_JNP_DOA_SHIPMENTS_ACK, R_JNP_DOA_SHIPMENTS, O_ORDER_MAIN>((A, S, O) => new object[] {
                JoinType.Left, S.ASNNUMBER == A.ASNNUMBER,
                JoinType.Left,S.ASNNUMBER==O.PREASN
            })
                .Where((A, S, O) => records.Contains(O.UPOID))
                .ToList();
            if (shipments.Count == 0)
            {
                Analyze(acks, dt);
            }
            else
            {
                throw new Exception("The current shipment status is not available, please check the uploaded data.");
            }
        }

        /// <summary>
        /// 分析上传的数据，是成功还是失败，同一个Shipment的PO一个失败全部失败
        /// </summary>
        /// <param name="acks"></param>
        /// <param name="dt"></param>
        private void Analyze(List<R_JNP_DOA_SHIPMENTS_ACK> acks, DateTime dt)
        {
            var failRecord = acks.FindAll(t => t.MESSAGE_CODE != null && t.MESSAGE_CODE != "").Select(t => t.FILE_PO + t.FILE_PO_LINE).ToList();
            if (failRecord.Count > 0)
            {
                #region fail record
                var failOrders = _db.Queryable<O_ORDER_MAIN>().Where(t => failRecord.Contains(t.UPOID)).ToList();
                if (failOrders.Count == 0)
                {
                    throw new Exception("PO&Line [" + failRecord[0] + "]  could not be found,please make sure the PO & Line is correct!");
                }
                var failAsnnumbers = failOrders.Select(t => t.PREASN).Distinct().ToList();
                #endregion

                #region correct record
                var correctRecord = acks.FindAll(t => t.MESSAGE_CODE == null || t.MESSAGE_CODE == "").Select(t => t.FILE_PO + t.FILE_PO_LINE).ToList();
                var correctOrders = _db.Queryable<O_ORDER_MAIN>().Where(t => correctRecord.Contains(t.UPOID) && !failAsnnumbers.Contains(t.PREASN)).ToList();
                if (correctOrders.Count == 0)
                {
                    throw new Exception("PO&Line [" + correctOrders[0] + "]  could not be found,please make sure the PO & Line is correct!");
                }
                var correctAsnnumbers = correctOrders.Select(t => t.PREASN).Distinct().ToList();
                #endregion

                try
                {
                    #region update fail status
                    var failOrdersForUpdate = _db.Queryable<O_ORDER_MAIN>().Where(t => failAsnnumbers.Contains(t.PREASN)).ToList();
                    _db.Ado.BeginTran();
                    for (int i = 0; i < failOrdersForUpdate.Count; i++)
                    {
                        _db.Updateable<O_PO_STATUS>()
                            .SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue(), EDITTIME = dt, EDITBY = _user })
                            .Where(t => t.POID == failOrdersForUpdate[i].ID)
                            .ExecuteCommand();
                        var cpostatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, _bu),
                            STATUSID = ENUM_O_PO_STATUS.DOAShipment.ExtValue(),
                            VALIDFLAG = MesBool.Yes.ExtValue(),
                            CREATETIME = dt,
                            CREATEBY = _user,
                            EDITTIME = dt,
                            EDITBY = _user,
                            POID = failOrdersForUpdate[i].ID
                        };
                        acks.ForEach(a =>
                        {
                            if (a.FILE_PO == failOrdersForUpdate[i].PONO && a.FILE_PO_LINE == failOrdersForUpdate[i].POLINE)
                            {
                                a.ASNNUMBER = failOrdersForUpdate[i].PREASN;
                            }
                        });
                        _db.Insertable(cpostatus).ExecuteCommand();
                        failOrdersForUpdate[i].PREASN = "0";
                        _db.Updateable(failOrdersForUpdate[i]).ExecuteCommand();
                    }
                    #endregion

                    #region update correct status
                    var correctOrdersForUpdate = _db.Queryable<O_ORDER_MAIN>().Where(t => correctAsnnumbers.Contains(t.PREASN)).ToList();
                    _db.Ado.BeginTran();
                    for (int i = 0; i < correctOrdersForUpdate.Count; i++)
                    {
                        acks.ForEach(a =>
                        {
                            if (a.FILE_PO == correctOrdersForUpdate[i].PONO && a.FILE_PO_LINE == correctOrdersForUpdate[i].POLINE)
                            {
                                a.ASNNUMBER = correctOrdersForUpdate[i].PREASN;
                            }
                        });
                    }
                    #endregion

                    _db.Insertable(acks).ExecuteCommand();
                    _db.Ado.CommitTran();
                }
                catch (Exception)
                {
                    _db.Ado.RollbackTran();
                    throw;
                }
            }
            else
            {
                var records = acks.Select(t => t.FILE_PO + t.FILE_PO_LINE).ToList();
                var orders = _db.Queryable<O_ORDER_MAIN>().Where(t => records.Contains(t.UPOID)).ToList();
                if (orders.Count == 0)
                {
                    throw new Exception("PO&Line [" + orders[0] + "]  could not be found,please make sure the PO & Line is correct!");
                }
                _db.Ado.BeginTran();
                try
                {
                    for (int i = 0; i < orders.Count; i++)
                    {
                        _db.Updateable<O_PO_STATUS>()
                            .SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue(), EDITTIME = dt, EDITBY = _user })
                            .Where(t => t.POID == orders[i].ID)
                            .ExecuteCommand();
                        var cpostatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, _bu),
                            STATUSID = ENUM_O_PO_STATUS.PrintLabelAndList.ExtValue(),
                            VALIDFLAG = MesBool.Yes.ExtValue(),
                            CREATETIME = dt,
                            CREATEBY = _user,
                            EDITTIME = dt,
                            EDITBY = _user,
                            POID = orders[i].ID
                        };
                        _db.Insertable(cpostatus).ExecuteCommand();
                        acks.ForEach(a =>
                        {
                            if (a.FILE_PO == orders[i].PONO && a.FILE_PO_LINE == orders[i].POLINE)
                            {
                                a.ASNNUMBER = orders[i].PREASN;
                            }
                        });
                    }
                    _db.Insertable(acks).ExecuteCommand();
                    _db.Ado.CommitTran();
                }
                catch (Exception)
                {
                    _db.Ado.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// Send DOA Shipment File,Auto Build Shipment Data & Shipment File In The Background 
        /// </summary>
        /// <param name="polist">po+poline</param>
        /// <param name="transport">transport type</param>
        /// <param name="notSend">only build data,not send data</param>
        public void SendShipment(string[] polist, string transport, bool notsend = false)
        {
            var shipmentConfig = _db.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDOAShipment" && r.USERCONTROL == "N" && r.FUNCTIONTYPE == "SYSTEM").ToList();
            if (shipmentConfig.Count == 0 || shipmentConfig.Count < 3)
            {
                throw new Exception("Juniper DOA Shiment missing setting!");
            }
            var pathConfig = shipmentConfig.Find(t => t.CATEGORY == "FILEPATH");
            if (pathConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing file path setting!");
            }
            var serverConfig = shipmentConfig.Find(t => t.CATEGORY == "SERVER");
            if (serverConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing mail server setting!");
            }
            var emailConfig = shipmentConfig.Find(t => t.CATEGORY == "SENDFROM");
            if (emailConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing email account setting!");
            }
            var mailList = shipmentConfig.Find(t => t.CATEGORY == "SENDTO");
            if (emailConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing email account setting!");
            }

            BuilShipmentData(polist, transport);
            if (!notsend)
            {
                MakeShipmentFile(polist, pathConfig.VALUE);
                SendShipmentFile(serverConfig.VALUE, serverConfig.EXTVAL, emailConfig.VALUE, emailConfig.EXTVAL, mailList.VALUE, pathConfig.VALUE, polist);
            }
        }

        /// <summary>
        /// Create File & Send File,Not Buil Data.
        /// </summary>
        /// <param name="polist"></param>
        public void SendShipment(string[] polist)
        {
            var shipmentConfig = _db.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDOAShipment" && r.USERCONTROL == "N" && r.FUNCTIONTYPE == "SYSTEM").ToList();
            if (shipmentConfig.Count == 0 || shipmentConfig.Count < 3)
            {
                throw new Exception("Juniper DOA Shiment missing setting!");
            }
            var pathConfig = shipmentConfig.Find(t => t.CATEGORY == "FILEPATH");
            if (pathConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing file path setting!");
            }
            var serverConfig = shipmentConfig.Find(t => t.CATEGORY == "SERVER");
            if (serverConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing mail server setting!");
            }
            var emailConfig = shipmentConfig.Find(t => t.CATEGORY == "SENDFROM");
            if (emailConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing email account setting!");
            }
            var mailList = shipmentConfig.Find(t => t.CATEGORY == "SENDTO");
            if (emailConfig == null)
            {
                throw new Exception("Juniper DOA Shiment missing email account setting!");
            }
            MakeShipmentFile(polist, pathConfig.VALUE);
            SendShipmentFile(serverConfig.VALUE, serverConfig.EXTVAL, emailConfig.VALUE, emailConfig.EXTVAL, mailList.VALUE, pathConfig.VALUE, polist);
        }

        /// <summary>
        /// Build shipment data & Insert into table
        /// </summary>
        /// <param name="polist"></param>
        /// <param name="transport"></param>
        private void BuilShipmentData(string[] polist, string transport)
        {
            var asnnumber = _db.Queryable<O_ORDER_MAIN>().Where(t => polist.Contains(t.UPOID) && t.FINALASN != "0").Select(t => t.PREASN).Distinct().ToList();
            if (asnnumber.Count > 1)
            {
                var shipments_ack = _db.Queryable<R_JNP_DOA_SHIPMENTS_ACK>().Where(t => asnnumber.Contains(t.ASNNUMBER)).Select(t => t.ASNNUMBER).Distinct().ToList();
                var shipments = _db.Queryable<R_JNP_DOA_SHIPMENTS>().Where(t => asnnumber.Contains(t.ASNNUMBER) && shipments_ack.Contains(t.ASNNUMBER) && t.SEND_FLAG == 1).ToList();
                if (shipments.Count > 0)
                {
                    throw new Exception("Some PO that you chose,the shipment file has been send out,can't sent shipment file again before custer reply");
                }
                else if (shipments.Count == 0)
                {
                    throw new Exception("Some PO that you chose,the shipment file has been send out,and got the correct reply,please call IT to fix it!");
                }
            }

            var plant = "";
            var sql = "";
            var upoid = polist[0].ToString();
            var firstorder = _db.Queryable<O_ORDER_MAIN, I137_I, I137_H>((m, ii, ih) => m.ITEMID == ii.ID && ii.TRANID == ih.TRANID)
                .Where((m, ii, ih) => m.UPOID == upoid)
                .Select((m, ii, ih) => new { m.PONO, m.POLINE, ih.COMPLETEDELIVERY })
                .ToList()
                .FirstOrDefault();
            var oOrderMainList = new List<O_ORDER_MAIN>();
            if (firstorder.COMPLETEDELIVERY == "X")
                oOrderMainList = JuniperASNObj.GetPoList(firstorder.PONO, firstorder.POLINE, _db);
            else
                oOrderMainList = JuniperASNObj.GetPoList(polist, _db);
            if (oOrderMainList.Count <= 0)
            {
                throw new Exception("Get PO error(O_ORDER_MAIN)");
            }

            //PO Plant Check
            int soQty = 0;
            bool firstPlant = true;
            foreach (var r in oOrderMainList)
            {
                if (firstPlant == true)
                {
                    plant = r.PLANT;
                    firstPlant = false;
                }
                else
                {
                    if (plant != r.PLANT) throw new Exception("Multiple plant in SO, Please check!");
                }
                soQty = soQty + int.Parse(Double.Parse(r.QTY).ToString());
            }
            if (plant != "FVN" && plant != "FJZ")
            {
                throw new Exception($@"Plant error ,Plant:{plant}!");
            }
            if (plant == "FVN")
            {
                FileNamePreFix += "VIETNAM_";
            }
            else if (plant == "FJZ")
            {
                FileNamePreFix += "JUAREZ_";
            }
            if (soQty == 0)
            {
                throw new Exception("Workorder Qty error, please check!");
            }

            var nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
            var fileid = nowTime.ToString("MMddyyyyHHmmss");
            while (_db.Queryable<R_JNP_DOA_SHIPMENTS>().Where(t => t.FILE_NAME.Contains(fileid)).Any())
            {
                nowTime = Convert.ToDateTime(_db.Ado.GetDataTable("select systimestamp from dual").Rows[0][0]);
                fileid = nowTime.ToString("yyyyMMddHHmmss");
            }
            //FGSHIP_FCJUAREZ_XXXXXXXXXXXXX or FGSHIP_FCVIETNAM_XXXXXXXXXXXXX
            var FileName = FileNamePreFix + fileid + ".xlsx";
            List<R_JNP_DOA_SHIPMENTS> shipmentList = new List<R_JNP_DOA_SHIPMENTS>();
            foreach (var r in oOrderMainList)
            {
                var IsUseSN = true;
                var strSql = $@"select * from o_agile_attr a where  a.item_number='{r.PID}' and a.actived=1";
                var agailattr = _db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == r.PID && t.ACTIVED == "1").First();
                if (agailattr != null)
                {
                    if (agailattr.SERIALIZATION.ToUpper() == "NO")
                    {
                        IsUseSN = false;
                    }
                }

                if (IsUseSN)
                {
                    var snList = _db.Queryable<R_SN, O_ORDER_MAIN>((S, O) => S.WORKORDERNO == O.PREWO)
                        .Where((S, O) => O.PONO == r.PONO && O.POLINE == r.POLINE)
                        .Select((S, O) => S.SN)
                        .ToList();

                    for (int v = 0; v < snList.Count; v++)
                    {
                        MESJuniper.OrderManagement.JuniperOmBase.JuniperI054AckCheck(snList[v], _db);
                        MESJuniper.OrderManagement.JuniperOmBase.JuniperI244Check(snList[v], _db);

                        R_JNP_DOA_SHIPMENTS shipment = new R_JNP_DOA_SHIPMENTS();
                        shipment.FILE_NAME = FileName;
                        shipment.PART_NUMBER = r.CUSTPID;
                        shipment.SERIAL_NUMBER = snList[v];
                        shipment.PO_NUMBER = r.PONO;
                        shipment.PO_LINE_NO = r.POLINE;
                        shipment.MEANS_OF_TRANSPORT = transport;
                        shipment.SHIPPED_QTY = 1;
                        shipment.FILE_FLAG = 0;
                        shipment.SEND_FLAG = 0;
                        shipmentList.Add(shipment);
                    }
                }
                else
                {
                    R_JNP_DOA_SHIPMENTS shipment = new R_JNP_DOA_SHIPMENTS();
                    shipment.FILE_NAME = FileName;
                    shipment.PART_NUMBER = r.CUSTPID;
                    shipment.PO_NUMBER = r.PONO;
                    shipment.PO_LINE_NO = r.POLINE;
                    shipment.MEANS_OF_TRANSPORT = transport;
                    shipment.SHIPPED_QTY = double.Parse(r.QTY);
                    shipment.FILE_FLAG = 0;
                    shipment.SEND_FLAG = 0;
                    shipmentList.Add(shipment);
                }
            }

            var asnNumber = SNmaker.GetNextSN(plant == "FVN" ? JuniperASNObj.AsnRule.PreShip.ExtName() : JuniperASNObj.AsnRule.PreShipFJZ.ExtName(), _db);
            while (_db.Queryable<R_I139>().Where(t => t.ASNNUMBER == asnNumber).Any())
            {
                asnNumber = SNmaker.GetNextSN(plant == "FVN" ? JuniperASNObj.AsnRule.PreShip.ExtName() : JuniperASNObj.AsnRule.PreShipFJZ.ExtName(), _db);
            }

            try
            {
                _db.Ado.BeginTran();
                foreach (var r in shipmentList)
                {
                    r.ID = MesDbBase.GetNewID<R_JNP_DOA_SHIPMENTS>(_db, _bu);
                    r.CREATETIME = nowTime;
                    r.CREATEBY = _user;
                    r.ASNNUMBER = asnNumber;
                }
                _db.Insertable(shipmentList).ExecuteCommand();

                foreach (var v in oOrderMainList)
                {
                    sql = $@"UPDATE O_ORDER_MAIN SET PREASN = '{asnNumber}', PREASNTIME = SYSDATE WHERE ID = '{v.ID}'";
                    _db.Ado.ExecuteCommand(sql);
                }

                var strpolist = "";
                polist.ToList().ForEach(t => { strpolist += t.ToString(); });

                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(_db, _bu),
                    FUNCTION_NAME = "BuilShipmentData",
                    CLASS_NAME = "MESJuniper.SendData.JuniperDOAShipment",
                    PROGRAM_NAME = "JuniperDOAShipment",
                    DATA1 = FileNamePreFix,
                    DATA2 = asnNumber,
                    LOG_MESSAGE = strpolist,
                    EDIT_TIME = nowTime,
                    EDIT_EMP = _user
                };
                _db.Insertable(log).ExecuteCommand();
                _db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Build Shipment File & Update Shipment data status
        /// </summary>
        /// <param name="polist"></param>
        /// <param name="FilePath"></param>
        private void MakeShipmentFile(string[] polist, string FilePath)
        {
            DateTime dt = DateTime.Now;
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            var shipmentdata = _db.Queryable<R_JNP_DOA_SHIPMENTS, O_ORDER_MAIN>((S, O) => S.ASNNUMBER == O.PREASN)
                .Where((S, O) => S.FILE_FLAG == 0 && S.SEND_FLAG == 0 && polist.Contains(O.UPOID))
                .Select((S, O) => S)
                .ToList();
            var files = shipmentdata.Select(t => t.FILE_NAME).Distinct().ToList();
            _db.Ado.BeginTran();
            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string sqlString = string.Format("SELECT T.PART_NUMBER        AS Part_Number,\n" +
                    "       T.SERIAL_NUMBER      AS Serial_Number,\n" +
                    "       T.CARTON_ID          AS \"Carton ID\",\n" +
                    "       T.MIST_CLAIM_CODE    AS MIST_CLAIM_CODE,\n" +
                    "       T.ETH_MAC            AS ETH_MAC,\n" +
                    "       T.MIST_PALLET_ID     AS MIST_Pallet_ID,\n" +
                    "       T.INVOICE_NO         AS \"Invoice No\",\n" +
                    "       T.MFG_DATE           AS MFG_DATE,\n" +
                    "       T.HW_REVISION        AS HW_Revision,\n" +
                    "       T.PO_NUMBER          AS PO_Number,\n" +
                    "       T.PO_LINE_NO         AS PO_Line_No,\n" +
                    "       T.SHIPPED_QTY        AS Shipped_Qty,\n" +
                    "       T.COO                AS COO,\n" +
                    "       T.BUILD_SITE         AS Build_Site,\n" +
                    "       T.STATUS             AS Status,\n" +
                    "       T.ROHS2              AS ROHS2,\n" +
                    "       T.MEANS_OF_TRANSPORT AS \"Means Of Transport\"\n" +
                    "  FROM R_JNP_DOA_SHIPMENTS T\n" +
                    " WHERE T.FILE_NAME = '{0}'", files[i]);
                    var data = _db.Ado.GetDataTable(sqlString);
                    MESPubLab.Common.ExcelHelp.ExportExcelToLoacl(data, FilePath + files[i], true);
                    var filedt = _db.Queryable<R_JNP_DOA_SHIPMENTS>().Where(t => t.FILE_NAME == files[i]).ToList();
                    for (int x = 0; x < filedt.Count; x++)
                    {
                        filedt[x].FILE_FLAG = 1;
                        filedt[x].FILE_TIME = dt;
                    }
                    _db.Updateable(filedt).ExecuteCommand();
                }
                _db.Ado.CommitTran();
            }
            catch (Exception)
            {
                _db.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// Send Shipment File 
        /// </summary>
        /// <param name="serverType">SMTP.......</param>
        /// <param name="server">Server IP Or another email server info</param>
        /// <param name="From">Email Send From</param>
        /// <param name="To">Send Mail To Mail List</param>
        /// <param name="filePath">Attachment File Path,Does not include file name</param>
        /// <param name="polist"></param>
        private void SendShipmentFile(string serverType, string server, string From,string PWD, string To, string filePath, string[] polist)
        {
            DateTime dt = DateTime.Now;
            var shipmentdata = _db.Queryable<R_JNP_DOA_SHIPMENTS, O_ORDER_MAIN>((S, O) => S.ASNNUMBER == O.PREASN)
                .Where((S, O) => S.FILE_FLAG == 1 && S.SEND_FLAG == 0 && polist.Contains(O.UPOID))
                .Select((S, O) => S)
                .ToList();
            var files = shipmentdata.Select(t => t.FILE_NAME).Distinct().ToList();

            _db.Ado.BeginTran();
            try
            {
                switch (serverType)
                {
                    case "SMTP":
                        MailMessage Email = new MailMessage();
                        for (int i = 0; i < files.Count; i++)
                        {
                            Attachment Attach = new Attachment(filePath + files[i]);
                            Email.Attachments.Add(Attach);
                        }
                        if (Email.Attachments.Count > 0)
                        {
                            Email.From = new MailAddress(From);
                            Email.To.Add(To);
                            Email.Subject = "FCJURAZE DOA Shipment File";
                            Email.IsBodyHtml = true;
                            Email.Body = "";
                            Email.Priority = MailPriority.Normal;
                            SmtpClient client = new SmtpClient(server);
                            client.Send(Email);
                        }
                        else
                        {
                            throw new Exception("Fail to get attachment!");
                        }
                        break;
                    default:
                        //"If hava another way to send email, please define it";
                        break;
                }
                for (int i = 0; i < shipmentdata.Count; i++)
                {
                    shipmentdata[i].SEND_FLAG = 1;
                    shipmentdata[i].SEND_TIME = dt;

                    var poid = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == shipmentdata[i].PO_NUMBER && t.POLINE == shipmentdata[i].PO_LINE_NO).Select(t => t.ID).First();
                    var sql = $@"update o_po_status set validflag = '0',edittime = sysdate where poid = '{poid}' and validflag = '1'";
                    _db.Ado.ExecuteCommand(sql);

                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, _bu),
                        POID = poid,
                        STATUSID = "12",
                        VALIDFLAG = "1",
                        CREATETIME = dt,
                        EDITTIME = dt,
                        CREATEBY = _user,
                    };
                    _db.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
                }
                _db.Updateable(shipmentdata).ExecuteCommand();

                _db.Ado.CommitTran();
            }
            catch (Exception e)
            {
                _db.Ado.RollbackTran();
                var strpolist = "";
                polist.ToList().ForEach(t => { strpolist += t.ToString(); });
                for (int i = 0; i < files.Count; i++)
                {
                    R_MES_LOG log = new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_MES_LOG>(_db, _bu),
                        FUNCTION_NAME = "BuilShipmentData",
                        CLASS_NAME = "MESJuniper.SendData.JuniperDOAShipment",
                        PROGRAM_NAME = "JuniperDOAShipment",
                        DATA1 = files[i],
                        LOG_MESSAGE = e.Message,
                        EDIT_TIME = dt,
                        EDIT_EMP = _user
                    };
                    _db.Insertable(log).ExecuteCommand();
                }
                throw;
            }
        }

    }
}
