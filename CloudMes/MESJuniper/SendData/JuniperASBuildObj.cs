using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MESJuniper.SendData
{
    public class JuniperASBuildObj
    {
        private SqlSugarClient _db = null;
        private string BuildSite = "";
        private string Factory = "";

        public JuniperASBuildObj(SqlSugarClient db, string _Factory, string _BuildSite)
        {
            _db = db;
            Factory = _Factory;
            BuildSite = _BuildSite;
        }

        #region Captrue AS-Build Data
        /// <summary>
        /// Generate AS-Build Data
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<R_I054> GenerateAS_Build_Data(List<R_SN> list)
        {
            var dt = DateTime.Now;
            var wolist = list.Select(t => t.WORKORDERNO).Distinct().ToList();
            var skulist = list.Select(t => t.SKUNO).Distinct().ToList();

            bool HasChild = false;
            int unisCount = 0;
            var res = new List<R_I054>();
            for (int x = 0; x < wolist.Count; x++)
            {
                string TransID = "";
                var datalist = list.FindAll(t => t.WORKORDERNO == wolist[x]);
                var order = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == wolist[x]).First();
                var series = _db.Queryable<C_SKU, C_SERIES>((sku, ser) => sku.C_SERIES_ID == ser.ID)
                    .Where((sku, ser) => sku.SKUNO == order.PID)
                    .Select((sku, ser) => ser)
                    .ToList()
                    .FirstOrDefault();
                var i137_Item = _db.Queryable<O_I137_ITEM>().Where(t => t.ID == order.ITEMID).First();
                var i137_Head = _db.Queryable<O_I137_HEAD>().Where(t => t.TRANID == i137_Item.TRANID).First();
                List<R_I054> tempI054 = new List<R_I054>();
                var agilelist = GetAgileList(order.PREWO);
                //判断有没有KP，带KP的记录最多10个产品一个Message，不带KP的记录最多25个产品一个Message
                if (agilelist.Count > 0)
                {
                    HasChild = true;
                }
                else
                {
                    HasChild = false;
                }

                for (int i = 0; i < datalist.Count; i++)
                {
                    if (TransID == "")
                    {
                        TransID = GetTranID();
                        unisCount = 0;
                    }
                    if (HasChild && unisCount >= 10)
                    {
                        TransID = GetTranID();
                        unisCount = 0;
                    }
                    else if (!HasChild && unisCount >= 25)
                    {
                        TransID = GetTranID();
                        unisCount = 0;
                    }

                    var agile = GetAgileRecord(order.PID);

                    #region Try To Get CLIE_CODE
                    var kp = _db.Queryable<R_SN_KP>()
                        .Where(t => t.SN == datalist[i].SN && t.VALUE == datalist[i].SN && t.KP_NAME.StartsWith("AutoKP") && t.PARTNO == order.PID)
                        .First();
                    if (kp == null)
                    {
                        //when AutoKP no record,skip this unit and write log, retry next time;
                        WriteLog.WriteIntoMESLog(
                            _db, 
                            Factory, 
                            "MESInterface", 
                            "MESInterface.JUNIPER.I054CaptrueProcess",
                            "GenerateAS_Build_Data", 
                            "", 
                            "",
                            datalist[i].SN, 
                            datalist[i].WORKORDERNO, 
                            "", "N", "interface", "N");
                        continue;
                    }
                    #endregion

                    #region Try To Get MAC Address
                    var MAC_ADDRESS = "";
                    var mac_kp = _db.Queryable<R_SN_KP>().Where(t => t.KP_NAME == "MAC" && t.SN == datalist[i].SN && t.VALID_FLAG == 1).First();
                    if (mac_kp != null)
                    {
                        MAC_ADDRESS = mac_kp.VALUE;
                    }
                    #endregion

                    var i054 = new R_I054()
                    {
                        ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                        F_PLANT = i137_Item.F_PLANT,
                        FILENAME = "",
                        MESSAGEID = "",
                        CREATIONDATETIME = dt,
                        CREATEDBY = "Foxconn",
                        LOGICALSYSTEM = "ECP",
                        SENDER = "Foxconn",
                        SALESORDERNUMBER = series != null && series.SERIES_NAME.StartsWith("JNP-ODM") ? int.Parse(i137_Head.SALESORDERNUMBER).ToString() : "",
                        SALESORDERLINENUMBER = series != null && series.SERIES_NAME.StartsWith("JNP-ODM") ? int.Parse(i137_Item.SALESORDERLINEITEM).ToString() : "",
                        PNTYPE = "Parent",
                        PARENTMODEL = agile.CUSTPARTNO,
                        PARENTSN = datalist[i].SN,
                        CHILDMATERIAL = "",
                        SN = "",
                        REVISION = "",
                        QTY = "1",
                        COO = (kp != null ? kp.LOCATION : ""),
                        CLEICODE = (kp != null ? kp.EXVALUE2 : ""),
                        MACADDRESS = MAC_ADDRESS,
                        BUILTSITE = BuildSite,
                        BUILDDATE = dt,
                        SOFTWAREVERSION = "",
                        SUBASSEMBLYNUMBER = (kp != null ? kp.MPN : ""),
                        SUBASSEMBLYREVISION = (kp != null ? kp.EXVALUE1 : ""),//这里是版本号
                        IBMSERIALNUMBER = "",
                        //FUTURE1 = (agile != null ? (agile.ROHS_COMPLIANCE.EndsWith("- YES") ? agile.ROHS_COMPLIANCE.Replace("- YES", "").Trim() : "") : ""),
                        //ODM模式agile.ROHS_COMPLIANCE時導致程序報錯，影響其他工單創建I054數據，這裡改一下
                        FUTURE1 = (agile != null ? (agile.ROHS_COMPLIANCE != null ? (agile.ROHS_COMPLIANCE.EndsWith("- YES") ? agile.ROHS_COMPLIANCE.Replace("- YES", "").Trim() : "") : "") : ""),
                        FUTURE2 = "",
                        FUTURE3 = "",
                        FUTURE4 = "",
                        FUTURE5 = "",
                        TRANID = TransID,
                        CREATETIME = dt
                    };

                    #region Validation
                    try
                    {
                        Validation(i054, agile);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message.ToString();
                        var pg = "MESInterface";
                        var cls = "MESInterface.JUNIPER.I054CaptrueProcess";
                        var func = "GenerateAS_Build_Data";
                        var sn = datalist[i].SN;
                        var wo = datalist[i].WORKORDERNO;
                        var r = _db.Queryable<R_MES_LOG>().Where(t => t.LOG_MESSAGE == msg && t.FUNCTION_NAME == func && t.DATA1 == sn && t.DATA2 == wo && t.MAILFLAG == "N").ToList();
                        if (r.Count == 0)
                        {
                            WriteLog.WriteIntoMESLog(_db, Factory, pg, cls, func, msg, "", sn, wo, "", "", "interface", "N");
                        }
                        continue;
                    }
                    #endregion

                    tempI054.Add(i054);

                    #region Get Property
                    CaptrueProperty(tempI054, i054, agile, datalist[i], i137_Head, i137_Item, order, dt, TransID, series);
                    #endregion

                    #region Get Child
                    try
                    {
                        CaptrueChild(tempI054, datalist[i], i137_Head, i137_Item, order, agile, dt, TransID, series);
                        unisCount++;
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(_db, Factory, "MESInterface", "MESInterface.JUNIPER.I054CaptrueProcess", "GenerateAS_Build_Data", ex.Message.ToString(), "", datalist[i].SN, datalist[i].WORKORDERNO, "", "", "interface", "N");
                        tempI054 = new List<R_I054>();
                        continue;
                    }
                    #endregion
                }
                if (tempI054.Count > 0)
                {
                    res.AddRange(tempI054);
                }
            }
            return res;
        }

        /// <summary>
        /// Captrue Child
        /// </summary>
        /// <param name="i054"></param>
        /// <param name="sn"></param>
        /// <param name="i137H"></param>
        /// <param name="i137I"></param>
        /// <param name="pwh"></param>
        /// <param name="dt"></param>
        /// <param name="TransID"></param>
        private void CaptrueChild(List<R_I054> i054, R_SN sn, O_I137_HEAD i137H, O_I137_ITEM i137I, O_ORDER_MAIN order, O_AGILE_ATTR agile, DateTime dt, string TransID, C_SERIES series)
        {
            #region Try To Get Child
            var kplist = _db.Queryable<R_SN_KP>()
                        .Where(t => t.SN == sn.SN && t.KP_NAME.StartsWith("AutoKP") && (t.SCANTYPE == "SN" || t.SCANTYPE == "NOValue"))
                        .ToList();
            var agilelist = GetAgileList(order.PREWO);
            for (int i = 0; i < agilelist.Count; i++)
            {
                var childs = kplist.FindAll(t => t.PARTNO == agilelist[i].ITEM_NUMBER).ToList();
                for (int x = 0; x < childs.Count; x++)
                {
                    if (childs[x].VALUE == "N/A")
                    {
                        var temp = i054.Find(t => t.PARENTSN == sn.SN && t.CHILDMATERIAL == agilelist[i].CUSTPARTNO);
                        if (temp != null)
                        {
                            temp.QTY = (int.Parse(temp.QTY) + 1).ToString();
                            continue;
                        }
                    }
                    var i137d = _db.Queryable<O_I137_DETAIL>()
                        .Where(t => t.TRANID == i137I.TRANID && t.ITEM == i137I.ITEM && t.COMPONENTID == agilelist[i].CUSTPARTNO)
                        .ToList()
                        .FirstOrDefault();
                    var i054_child = new R_I054()
                    {
                        ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                        F_PLANT = i137I.F_PLANT,
                        FILENAME = "",
                        MESSAGEID = "",
                        CREATIONDATETIME = dt,
                        CREATEDBY = "Foxconn",
                        LOGICALSYSTEM = "ECP",
                        SENDER = "Foxconn",
                        SALESORDERNUMBER = series != null && series.SERIES_NAME.StartsWith("JNP-ODM") ? int.Parse(i137H.SALESORDERNUMBER).ToString() : "",
                        SALESORDERLINENUMBER = series != null && series.SERIES_NAME.StartsWith("JNP-ODM") && i137d != null && !childs[x].KP_NAME.EndsWith("_HB") ? int.Parse(i137d.COMSALESORDERLINEITEM).ToString() : "",
                        PARENTMODEL = agile.CUSTPARTNO,
                        PNTYPE = "Child",
                        PARENTSN = sn.SN,
                        CHILDMATERIAL = agilelist[i].CUSTPARTNO,
                        SN = childs[x].VALUE == "N/A" ? "" : childs[x].VALUE,
                        REVISION = "",
                        QTY = "1",
                        COO = childs[x].LOCATION,
                        CLEICODE = childs[x].EXVALUE2,
                        MACADDRESS = "",
                        BUILTSITE = BuildSite,
                        BUILDDATE = dt,
                        SOFTWAREVERSION = "",
                        SUBASSEMBLYNUMBER = childs[x].MPN,// GetSubAssemblyNumber(order.PREWO, agilelist[i].ITEM_NUMBER),
                        SUBASSEMBLYREVISION = childs[x].EXVALUE1,//GetSubAssemblyRevision(order.PREWO, agilelist[i].ITEM_NUMBER),
                        IBMSERIALNUMBER = "",
                        FUTURE1 = (agilelist[i].ROHS_COMPLIANCE != null && agilelist[i].ROHS_COMPLIANCE.EndsWith("- YES") ? agilelist[i].ROHS_COMPLIANCE.Replace("- YES", "").Trim() : ""),
                        FUTURE2 = "",
                        FUTURE3 = "",
                        FUTURE4 = "",
                        FUTURE5 = "",
                        TRANID = TransID,
                        CREATETIME = dt
                    };
                    Validation(i054_child, agilelist[i]);
                    i054.Add(i054_child);
                    CaptrueProperty(i054, i054_child, agilelist[i], sn, i137H, i137I, order, dt, TransID, series);
                }
            }
            #endregion
        }

        /// <summary>
        /// Captrue Propertry
        /// </summary>
        /// <param name="i054"></param>
        /// <param name="sn"></param>
        /// <param name="i137H"></param>
        /// <param name="i137I"></param>
        /// <param name="pwh"></param>
        /// <param name="dt"></param>
        /// <param name="TransID"></param>
        private void CaptrueProperty(List<R_I054> list, R_I054 i054, O_AGILE_ATTR agile, R_SN sn, O_I137_HEAD i137H, O_I137_ITEM i137I, O_ORDER_MAIN order, DateTime dt, string TransID, C_SERIES series)
        {
            #region Captrue Software
            var sw = _db.Queryable<R_JUNIPER_SW>().Where(t => t.PARENTPN == order.CUSTPID && (t.MODELPN == i054.CHILDMATERIAL || t.MODELPN == i054.PARENTMODEL)).First();
            if (sw != null)
            {
                i054.SOFTWAREVERSION = sw.SWVERSION;
                var i054_Property_SWTYPE = new R_I054()
                {
                    ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                    F_PLANT = i137I.F_PLANT,
                    FILENAME = "",
                    MESSAGEID = "",
                    CREATIONDATETIME = dt,
                    CREATEDBY = "Foxconn",
                    LOGICALSYSTEM = "ECP",
                    SENDER = "Foxconn",
                    SALESORDERNUMBER = series != null && series.SERIES_NAME.StartsWith("JNP-ODM") ? int.Parse(i137H.SALESORDERNUMBER).ToString() : "",
                    SALESORDERLINENUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137I.SALESORDERLINEITEM,
                    PARENTMODEL = sw.MODELPN,
                    PNTYPE = "Property",
                    PARENTSN = (i054.SN == null || i054.SN == "" ? i054.PARENTSN : i054.SN),
                    CHILDMATERIAL = "SoftwareType",
                    SN = sw.SWTYPE,
                    REVISION = "",
                    QTY = "1",
                    COO = "",
                    CLEICODE = "",
                    MACADDRESS = "",
                    BUILTSITE = BuildSite,
                    BUILDDATE = dt,
                    SOFTWAREVERSION = "",
                    SUBASSEMBLYNUMBER = "",
                    SUBASSEMBLYREVISION = "",
                    IBMSERIALNUMBER = "",
                    FUTURE1 = "",
                    FUTURE2 = "",
                    FUTURE3 = "",
                    FUTURE4 = "",
                    FUTURE5 = "",
                    TRANID = TransID,
                    CREATETIME = dt
                };
                var i054_Property_SWPN = new R_I054()
                {
                    ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                    F_PLANT = i137I.F_PLANT,
                    FILENAME = "",
                    MESSAGEID = "",
                    CREATIONDATETIME = dt,
                    CREATEDBY = "Foxconn",
                    LOGICALSYSTEM = "ECP",
                    SENDER = "Foxconn",
                    SALESORDERNUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137H.SALESORDERNUMBER,
                    SALESORDERLINENUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137I.SALESORDERLINEITEM,
                    PARENTMODEL = sw.MODELPN,
                    PNTYPE = "Property",
                    PARENTSN = (i054.SN == null || i054.SN == "" ? i054.PARENTSN : i054.SN),
                    CHILDMATERIAL = "SoftwareImagePartNumber",
                    SN = sw.SWPN,
                    REVISION = "",
                    QTY = "1",
                    COO = "",
                    CLEICODE = "",
                    MACADDRESS = "",
                    BUILTSITE = BuildSite,
                    BUILDDATE = dt,
                    SOFTWAREVERSION = "",
                    SUBASSEMBLYNUMBER = "",
                    SUBASSEMBLYREVISION = "",
                    IBMSERIALNUMBER = "",
                    FUTURE1 = "",
                    FUTURE2 = "",
                    FUTURE3 = "",
                    FUTURE4 = "",
                    FUTURE5 = "",
                    TRANID = TransID,
                    CREATETIME = dt
                };
                list.Add(i054_Property_SWTYPE);
                list.Add(i054_Property_SWPN);
            }
            #endregion
            #region Captrue CHAS SysSerOfChassis
            if (i054.PARENTMODEL.Contains("CHAS"))
            {
                var sysSer = _db.Queryable<R_SN_KP>().Where(t => t.SN == sn.SN && t.PARTNO == agile.ITEM_NUMBER && t.SCANTYPE == "SysSerOfChassis").First();
                if (sysSer != null)
                {
                    var SysSerOfChassis = new R_I054()
                    {
                        ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                        F_PLANT = i137I.F_PLANT,
                        FILENAME = "",
                        MESSAGEID = "",
                        CREATIONDATETIME = dt,
                        CREATEDBY = "Foxconn",
                        LOGICALSYSTEM = "ECP",
                        SENDER = "Foxconn",
                        SALESORDERNUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137H.SALESORDERNUMBER,
                        SALESORDERLINENUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137I.SALESORDERLINEITEM,
                        PARENTMODEL = i054.PARENTMODEL,
                        PNTYPE = "Property",
                        PARENTSN = sn.SN,
                        CHILDMATERIAL = "SysSerOfChassis",
                        SN = sysSer.VALUE,
                        REVISION = "",
                        QTY = "1",
                        COO = "",
                        CLEICODE = "",
                        MACADDRESS = "",
                        BUILTSITE = BuildSite,
                        BUILDDATE = dt,
                        SOFTWAREVERSION = "",
                        SUBASSEMBLYNUMBER = "",
                        SUBASSEMBLYREVISION = "",
                        IBMSERIALNUMBER = "",
                        FUTURE1 = "",
                        FUTURE2 = "",
                        FUTURE3 = "",
                        FUTURE4 = "",
                        FUTURE5 = "",
                        TRANID = TransID,
                        CREATETIME = dt
                    };
                    list.Add(SysSerOfChassis);
                }
            }
            else if (i054.CHILDMATERIAL.Contains("CHAS"))
            {
                var sysSer = _db.Queryable<R_SN_KP>().Where(t => t.SN == sn.SN && t.PARTNO == agile.ITEM_NUMBER && t.SCANTYPE == "SysSerOfChassis").First();
                if (sysSer != null)
                {
                    var SysSerOfChassis = new R_I054()
                    {
                        ID = MESDataObject.MesDbBase.GetNewID<R_I054>(_db, "JUNIPER"),
                        F_PLANT = i137I.F_PLANT,
                        FILENAME = "",
                        MESSAGEID = "",
                        CREATIONDATETIME = dt,
                        CREATEDBY = "Foxconn",
                        LOGICALSYSTEM = "ECP",
                        SENDER = "Foxconn",
                        SALESORDERNUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137H.SALESORDERNUMBER,
                        SALESORDERLINENUMBER = "",//order.USERITEMTYPE == "BNDL" ? "" : i137I.SALESORDERLINEITEM,
                        PARENTMODEL = i054.CHILDMATERIAL,
                        PNTYPE = "Property",
                        PARENTSN = (i054.SN == null || i054.SN == "" ? i054.PARENTSN : i054.SN),
                        CHILDMATERIAL = "SysSerOfChassis",
                        SN = sysSer.VALUE,
                        REVISION = "",
                        QTY = "1",
                        COO = "",
                        CLEICODE = "",
                        MACADDRESS = "",
                        BUILTSITE = BuildSite,
                        BUILDDATE = dt,
                        SOFTWAREVERSION = "",
                        SUBASSEMBLYNUMBER = "",
                        SUBASSEMBLYREVISION = "",
                        IBMSERIALNUMBER = "",
                        FUTURE1 = "",
                        FUTURE2 = "",
                        FUTURE3 = "",
                        FUTURE4 = "",
                        FUTURE5 = "",
                        TRANID = TransID,
                        CREATETIME = dt
                    };
                    list.Add(SysSerOfChassis);
                }
            }
            #endregion
        }
        #endregion

        #region Data Validation
        private void Validation(R_I054 data, O_AGILE_ATTR agile)
        {
            if (agile.SERIALIZATION == "Yes" && agile.SERIAL_NUMBER_MASK != null)
            {
                var mask = agile.SERIAL_NUMBER_MASK.Substring(agile.SERIAL_NUMBER_MASK.IndexOf('|') + 1).Replace(" (-BB/R/S)", "");
                if (data.PNTYPE == "Parent")
                {
                    ValidationInvalidChar(data.PARENTSN);
                    ValidationSNMask(data.PARENTSN, mask);
                }
                else
                {
                    ValidationInvalidChar(data.SN);
                    ValidationSNMask(data.SN, mask);
                }
            }
            //if (agile.CLEI_CODE != null && agile.CLEI_CODE != data.CLEICODE)
            //{
            //    throw new Exception("CLEI Is Provided In Agile,Please Scan In Key Part");
            //}
            if (agile.ROHS_COMPLIANCE != null && agile.ROHS_COMPLIANCE.EndsWith("- YES"))
            {
                var agileRoHs = agile.ROHS_COMPLIANCE.Replace("- YES", "").Trim();
                ValidationROHS(data.FUTURE1, agileRoHs);
            }
            if (data.CHILDMATERIAL == "SoftwareType")
            {
                ValidationStandardSWType(data.SN);
            }
            if (data.CHILDMATERIAL == "SoftwareImagePartNumber")
            {
                ValidationStandardSWPN(data.SN);
            }
            if (data.COO != null && data.COO != "")
            {
                ValidationCOO(data.COO);
            }
        }

        /// <summary>
        /// Validation SN Mask
        /// Serial number must comply with the serial number mask in Agile (if applicable).  
        /// # > Denotes one numeric character(0-9)
        /// * > Denotes one alphabetic character(a-z,A-Z)
        /// @ > Denotes one alpha-numeric character(0-9,a-z,A-Z)
        /// ! > Denotes one optional numneric character(0-9)
        /// ''> Characters specified within these quotes denote actual alphabetic or numberic characters that are expected in the serial number
        /// MM> Denotes a valid month(a valid month is a current or past month)
        /// yyyy> Denotes a valid year(a valid year is a current or past year)
        /// </summary>
        /// <param name="sn">Serial NO</param>
        /// <param name="mask">SN Mask</param>
        private void ValidationSNMask(string sn, string mask)
        {
            var sns = sn.ToArray();
            var masks = mask.ToArray();
            var RMasks = mask.Replace("'", "");

            if (sns.Length != RMasks.Length)
            {
                throw new Exception(sn + " length does not match " + mask);
            }
            var isConstant = false;
            var n = 0;
            for (int i = 0; i < masks.Length; i++)
            {
                if (!isConstant && (masks[i].ToString().ToUpper() == "Y" || masks[i].ToString().ToUpper() == "M"))//年月另外验证
                {
                    n++;
                    continue;
                }
                if (!isConstant && masks[i].ToString() == "'")//引号表示内容为常量
                {
                    isConstant = true;
                    continue;
                }
                if (isConstant && masks[i].ToString() == "'")//常量结束
                {
                    isConstant = false;
                    continue;
                }
                if (!isConstant)
                {
                    Regex reg;
                    Match ma;
                    switch (masks[i])
                    {
                        case '#':
                            reg = new Regex("^[0-9]+$");
                            ma = reg.Match(sns[n].ToString());
                            if (!ma.Success)
                            {
                                throw new Exception("The " + n + " position of " + sn + " dose not match " + mask);
                            }
                            break;
                        case '*':
                            reg = new Regex("^[a-zA-Z]+$");
                            ma = reg.Match(sns[n].ToString());
                            if (!ma.Success)
                            {
                                throw new Exception("The " + n + " position of " + sn + " dose not match " + mask);
                            }
                            break;
                        case '@':
                            reg = new Regex("^[a-zA-Z0-9]+$");
                            ma = reg.Match(sns[n].ToString());
                            if (!ma.Success)
                            {
                                throw new Exception("The " + n + " position of " + sn + " dose not match " + mask);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (isConstant && !sns[n].Equals(mask[i]))
                {
                    throw new Exception("The " + n + " position of " + sn + " dose not match " + mask);
                }
                n++;
            }

            #region 验证年月
            int yIndex = RMasks.IndexOf("yyyy");
            int mIndex = RMasks.IndexOf("MM");

            if (yIndex >= 0 && mIndex >= 0)
            {
                int year = 0;
                int month = 0;
                try
                {
                    year = int.Parse(sn.Substring(yIndex, 4));
                }
                catch
                {
                    throw new Exception("Years of " + sn + " from " + yIndex + " to " + (yIndex + 4) + " are invalid");
                }
                var currentYear = DateTime.Now.Year;
                if (currentYear < year)
                {
                    throw new Exception("The year must be current year or past year");
                }

                try
                {
                    month = int.Parse(sn.Substring(mIndex, 2));
                }
                catch
                {
                    throw new Exception("Months of " + sn + " from " + mIndex + " to " + (mIndex + 2) + " are invalid");
                }
                var dt = new DateTime(year, month, 1);
                if (DateTime.Now < dt)
                {
                    throw new Exception("The month must be current month or past month");
                }
            }
            #endregion
        }

        /// <summary>
        /// Validation Invalid Character;
        /// No invalid character in serial number;
        /// </summary>
        /// <param name="sn">Serial NO</param>
        private void ValidationInvalidChar(string sn)
        {
            var sns = sn.ToArray();
            for (int i = 0; i < sns.Length; i++)
            {
                if (!Char.IsNumber(sns[i]) && !Char.IsLetter(sns[i]))
                {
                    throw new Exception(sn + " contains invalid characters [" + sns[i].ToString() + "]");
                }
            }
        }

        /// <summary>
        /// Validation Standard SW - only 13 of SW Types are allowed; 
        /// otherwise returning an error messag
        /// </summary>
        /// <param name="SWType">SW Type</param>
        private void ValidationStandardSWType(string SWType)
        {
            var swType = new string[] {
                "JUNOS",
                "JUNOS-BB",
                "JUNOS-64",
                "JUNOS-64-BB",
                "JUNOS-EVO-64",
                "JUNOS-LTD",
                "JUNOS-LTD-BB",
                "JUNOS-LTD-64",
                "JUNOS-LTD-64-BB",
                "JUNOS-WW",
                "JUNOS-WW-BB",
                "JUNOS-WW-64",
                "JUNOS-WW-64-BB"
            };
            if (!swType.Contains(SWType))
            {
                throw new Exception(SWType + " Not In Standard SW Type List");
            }
        }

        /// <summary>
        /// Validation Standard SW PN
        /// SW PN must be with prefix 8XX (see appendix possible values in I054); 
        /// </summary>
        /// <param name="SWPN"></param>
        private void ValidationStandardSWPN(string SWPN)
        {
            if (SWPN.StartsWith("8"))
            {
                throw new Exception("Standard SW PN [" + SWPN + "] must be with prefix 8XX");
            }
        }

        /// <summary>
        /// Validation COO;
        /// COO shall be within the list of COO provided by Juniper.  
        /// See appendix "COO List"
        /// </summary>
        /// <param name="coo"></param>
        private void ValidationCOO(string coo)
        {
            var cooList = _db.Queryable<R_COO_MAP>().Where(t => t.CODE == coo).ToList();
            if (cooList.Count == 0)
            {
                throw new Exception(coo + " not in the list of COO provided by Juniper.");
            }
        }

        /// <summary>
        /// Validation RoHS
        /// Correct RoHS Value "2011/65/EU" must be provided when it is RoHS compliant per Agile, eg "2011/65/EU - YES"
        /// </summary>
        /// <param name="RoHS"></param>
        /// <param name="agile"></param>
        private void ValidationROHS(string RoHS, string ROHS_COMPLIANCE)
        {
            if (RoHS == null || RoHS == "")
            {
                throw new Exception("RoHS Value must be provided when it is RoHS compliant per Agile!");
            }
            if (RoHS != ROHS_COMPLIANCE)
            {
                throw new Exception("ROHS " + RoHS + " doesn't match the ROHS value of Agile.");
            }
        }
        #endregion

        public string GetTranID()
        {
            return Factory + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
        public O_AGILE_ATTR GetAgileRecord(string partno)
        {
            return _db.Queryable<O_AGILE_ATTR>()
                    .Where(t => t.ITEM_NUMBER == partno && t.PLANT == Factory)
                    .OrderBy(t => t.DATE_CREATED, SqlSugar.OrderByType.Desc)
                    .First();
        }
        public List<O_AGILE_ATTR> GetAgileList(string wo)
        {
            var res = _db.Ado.SqlQuery<O_AGILE_ATTR>(@"
                    SELECT *
                      FROM (SELECT a.*,
                                   ROW_NUMBER() OVER(PARTITION BY item_number ORDER BY DATE_CREATED DESC) numbs
                              FROM o_agile_attr a
                             WHERE item_number IN (SELECT pn FROM r_sap_hb WHERE wo = @wo)
                               AND PLANT = @Factory)
                     WHERE numbs = 1
                    UNION
                    SELECT *
                      FROM (SELECT a.*,
                                   ROW_NUMBER() OVER(PARTITION BY item_number ORDER BY DATE_CREATED DESC) numbs
                              FROM o_agile_attr a
                             WHERE CUSTOMER_PART_NUMBER IN (                               
                                                   SELECT componentid
                                                     FROM o_i137_detail
                                                    WHERE (tranid, item) IN
                                                          (SELECT tranid, item
                                                             FROM o_i137_item
                                                            WHERE id = (SELECT itemid
                                                                          FROM o_order_main
                                                                         WHERE prewo = @wo))
                               
                                                   )
                               and PLANT = @Factory)
                     WHERE numbs = 1
            ", new { wo, Factory })
            .ToList();
            return res;
        }
        
    }
}
