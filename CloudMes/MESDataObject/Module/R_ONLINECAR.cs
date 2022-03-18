using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ONLINECAR : DataObjectTable
    {
        public T_R_ONLINECAR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ONLINECAR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ONLINECAR);
            TableName = "R_ONLINECAR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ONLINECAR : DataObjectBase
    {
        public Row_R_ONLINECAR(DataObjectInfo info) : base(info)
        {

        }
        public R_ONLINECAR GetDataObject()
        {
            R_ONLINECAR DataObject = new R_ONLINECAR();
            DataObject.PRODUCTIONLINE = this.PRODUCTIONLINE;
            DataObject.STATION = this.STATION;
            DataObject.FINDTIME = this.FINDTIME;
            DataObject.TOTIME = this.TOTIME;
            DataObject.SKUQTY = this.SKUQTY;
            DataObject.FAILQTY = this.FAILQTY;
            DataObject.FAILTEXT = this.FAILTEXT;
            DataObject.ATTACHFILE1 = this.ATTACHFILE1;
            DataObject.FAILPNAME = this.FAILPNAME;
            DataObject.ISSUEDATE = this.ISSUEDATE;
            DataObject.OKTORELEASE = this.OKTORELEASE;
            DataObject.FAILDEPART = this.FAILDEPART;
            DataObject.CONFIRMNAME = this.CONFIRMNAME;
            DataObject.CONFIRMDATE = this.CONFIRMDATE;
            DataObject.ROOTDEPART = this.ROOTDEPART;
            DataObject.ROOTTEXT = this.ROOTTEXT;
            DataObject.PATEXT = this.PATEXT;
            DataObject.ATTACHFILE2 = this.ATTACHFILE2;
            DataObject.ROOTPNAME = this.ROOTPNAME;
            DataObject.ISCANCEL = this.ISCANCEL;
            DataObject.FADATE = this.FADATE;
            DataObject.AUDITDEPART1 = this.AUDITDEPART1;
            DataObject.AUDITDEPART2 = this.AUDITDEPART2;
            DataObject.AUDITDEPART3 = this.AUDITDEPART3;
            DataObject.AUDITDEPART4 = this.AUDITDEPART4;
            DataObject.AUDITDEPART5 = this.AUDITDEPART5;
            DataObject.MODIPART1 = this.MODIPART1;
            DataObject.MODIPART2 = this.MODIPART2;
            DataObject.MODIPART3 = this.MODIPART3;
            DataObject.MODIPART4 = this.MODIPART4;
            DataObject.MODIPART5 = this.MODIPART5;
            DataObject.YCOMPLETEDDATE1 = this.YCOMPLETEDDATE1;
            DataObject.YCOMPLETEDDATE2 = this.YCOMPLETEDDATE2;
            DataObject.YCOMPLETEDDATE3 = this.YCOMPLETEDDATE3;
            DataObject.YCOMPLETEDDATE4 = this.YCOMPLETEDDATE4;
            DataObject.YCOMPLETEDDATE5 = this.YCOMPLETEDDATE5;
            DataObject.COMPLETEDDATE1 = this.COMPLETEDDATE1;
            DataObject.COMPLETEDATE2 = this.COMPLETEDATE2;
            DataObject.COMPLETEDATE3 = this.COMPLETEDATE3;
            DataObject.COMPLETEDATE4 = this.COMPLETEDATE4;
            DataObject.COMPLETEDATE5 = this.COMPLETEDATE5;
            DataObject.AUDITNAME1 = this.AUDITNAME1;
            DataObject.AUDITNAME2 = this.AUDITNAME2;
            DataObject.AUDITNAME3 = this.AUDITNAME3;
            DataObject.AUDITNAME4 = this.AUDITNAME4;
            DataObject.AUDITNAME5 = this.AUDITNAME5;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.RETURNREASON = this.RETURNREASON;
            DataObject.QCFAILPRODPROCESS = this.QCFAILPRODPROCESS;
            DataObject.ATTACHFILE3 = this.ATTACHFILE3;
            DataObject.QCPREPAREDBY = this.QCPREPAREDBY;
            DataObject.QCPREPAREDDATE = this.QCPREPAREDDATE;
            DataObject.PROMIT = this.PROMIT;
            DataObject.PROMITDATE = this.PROMITDATE;
            DataObject.REMARK = this.REMARK;
            DataObject.QAMANAGER = this.QAMANAGER;
            DataObject.ESTATUS = this.ESTATUS;
            DataObject.ID = this.ID;
            DataObject.CARNO = this.CARNO;
            DataObject.CARTYPE = this.CARTYPE;
            DataObject.CARTITLE = this.CARTITLE;
            DataObject.BUNAME = this.BUNAME;
            DataObject.SKUNAME = this.SKUNAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CURRENTDEP = this.CURRENTDEP;
            return DataObject;
        }
        public string PRODUCTIONLINE
        {
            get
            {
                return (string)this["PRODUCTIONLINE"];
            }
            set
            {
                this["PRODUCTIONLINE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public DateTime? FINDTIME
        {
            get
            {
                return (DateTime?)this["FINDTIME"];
            }
            set
            {
                this["FINDTIME"] = value;
            }
        }
        public DateTime? TOTIME
        {
            get
            {
                return (DateTime?)this["TOTIME"];
            }
            set
            {
                this["TOTIME"] = value;
            }
        }
        public double? SKUQTY
        {
            get
            {
                return (double?)this["SKUQTY"];
            }
            set
            {
                this["SKUQTY"] = value;
            }
        }
        public double? FAILQTY
        {
            get
            {
                return (double?)this["FAILQTY"];
            }
            set
            {
                this["FAILQTY"] = value;
            }
        }
        public string FAILTEXT
        {
            get
            {
                return (string)this["FAILTEXT"];
            }
            set
            {
                this["FAILTEXT"] = value;
            }
        }
        public string ATTACHFILE1
        {
            get
            {
                return (string)this["ATTACHFILE1"];
            }
            set
            {
                this["ATTACHFILE1"] = value;
            }
        }
        public string FAILPNAME
        {
            get
            {
                return (string)this["FAILPNAME"];
            }
            set
            {
                this["FAILPNAME"] = value;
            }
        }
        public DateTime? ISSUEDATE
        {
            get
            {
                return (DateTime?)this["ISSUEDATE"];
            }
            set
            {
                this["ISSUEDATE"] = value;
            }
        }
        public double? OKTORELEASE
        {
            get
            {
                return (double?)this["OKTORELEASE"];
            }
            set
            {
                this["OKTORELEASE"] = value;
            }
        }
        public string FAILDEPART
        {
            get
            {
                return (string)this["FAILDEPART"];
            }
            set
            {
                this["FAILDEPART"] = value;
            }
        }
        public string CONFIRMNAME
        {
            get
            {
                return (string)this["CONFIRMNAME"];
            }
            set
            {
                this["CONFIRMNAME"] = value;
            }
        }
        public DateTime? CONFIRMDATE
        {
            get
            {
                return (DateTime?)this["CONFIRMDATE"];
            }
            set
            {
                this["CONFIRMDATE"] = value;
            }
        }
        public string ROOTDEPART
        {
            get
            {
                return (string)this["ROOTDEPART"];
            }
            set
            {
                this["ROOTDEPART"] = value;
            }
        }
        public string ROOTTEXT
        {
            get
            {
                return (string)this["ROOTTEXT"];
            }
            set
            {
                this["ROOTTEXT"] = value;
            }
        }
        public string PATEXT
        {
            get
            {
                return (string)this["PATEXT"];
            }
            set
            {
                this["PATEXT"] = value;
            }
        }
        public string ATTACHFILE2
        {
            get
            {
                return (string)this["ATTACHFILE2"];
            }
            set
            {
                this["ATTACHFILE2"] = value;
            }
        }
        public string ROOTPNAME
        {
            get
            {
                return (string)this["ROOTPNAME"];
            }
            set
            {
                this["ROOTPNAME"] = value;
            }
        }
        public string ISCANCEL
        {
            get
            {
                return (string)this["ISCANCEL"];
            }
            set
            {
                this["ISCANCEL"] = value;
            }
        }
        public DateTime? FADATE
        {
            get
            {
                return (DateTime?)this["FADATE"];
            }
            set
            {
                this["FADATE"] = value;
            }
        }
        public string AUDITDEPART1
        {
            get
            {
                return (string)this["AUDITDEPART1"];
            }
            set
            {
                this["AUDITDEPART1"] = value;
            }
        }
        public string AUDITDEPART2
        {
            get
            {
                return (string)this["AUDITDEPART2"];
            }
            set
            {
                this["AUDITDEPART2"] = value;
            }
        }
        public string AUDITDEPART3
        {
            get
            {
                return (string)this["AUDITDEPART3"];
            }
            set
            {
                this["AUDITDEPART3"] = value;
            }
        }
        public string AUDITDEPART4
        {
            get
            {
                return (string)this["AUDITDEPART4"];
            }
            set
            {
                this["AUDITDEPART4"] = value;
            }
        }
        public string AUDITDEPART5
        {
            get
            {
                return (string)this["AUDITDEPART5"];
            }
            set
            {
                this["AUDITDEPART5"] = value;
            }
        }
        public string MODIPART1
        {
            get
            {
                return (string)this["MODIPART1"];
            }
            set
            {
                this["MODIPART1"] = value;
            }
        }
        public string MODIPART2
        {
            get
            {
                return (string)this["MODIPART2"];
            }
            set
            {
                this["MODIPART2"] = value;
            }
        }
        public string MODIPART3
        {
            get
            {
                return (string)this["MODIPART3"];
            }
            set
            {
                this["MODIPART3"] = value;
            }
        }
        public string MODIPART4
        {
            get
            {
                return (string)this["MODIPART4"];
            }
            set
            {
                this["MODIPART4"] = value;
            }
        }
        public string MODIPART5
        {
            get
            {
                return (string)this["MODIPART5"];
            }
            set
            {
                this["MODIPART5"] = value;
            }
        }
        public DateTime? YCOMPLETEDDATE1
        {
            get
            {
                return (DateTime?)this["YCOMPLETEDDATE1"];
            }
            set
            {
                this["YCOMPLETEDDATE1"] = value;
            }
        }
        public DateTime? YCOMPLETEDDATE2
        {
            get
            {
                return (DateTime?)this["YCOMPLETEDDATE2"];
            }
            set
            {
                this["YCOMPLETEDDATE2"] = value;
            }
        }
        public DateTime? YCOMPLETEDDATE3
        {
            get
            {
                return (DateTime?)this["YCOMPLETEDDATE3"];
            }
            set
            {
                this["YCOMPLETEDDATE3"] = value;
            }
        }
        public DateTime? YCOMPLETEDDATE4
        {
            get
            {
                return (DateTime?)this["YCOMPLETEDDATE4"];
            }
            set
            {
                this["YCOMPLETEDDATE4"] = value;
            }
        }
        public DateTime? YCOMPLETEDDATE5
        {
            get
            {
                return (DateTime?)this["YCOMPLETEDDATE5"];
            }
            set
            {
                this["YCOMPLETEDDATE5"] = value;
            }
        }
        public DateTime? COMPLETEDDATE1
        {
            get
            {
                return (DateTime?)this["COMPLETEDDATE1"];
            }
            set
            {
                this["COMPLETEDDATE1"] = value;
            }
        }
        public DateTime? COMPLETEDATE2
        {
            get
            {
                return (DateTime?)this["COMPLETEDATE2"];
            }
            set
            {
                this["COMPLETEDATE2"] = value;
            }
        }
        public DateTime? COMPLETEDATE3
        {
            get
            {
                return (DateTime?)this["COMPLETEDATE3"];
            }
            set
            {
                this["COMPLETEDATE3"] = value;
            }
        }
        public DateTime? COMPLETEDATE4
        {
            get
            {
                return (DateTime?)this["COMPLETEDATE4"];
            }
            set
            {
                this["COMPLETEDATE4"] = value;
            }
        }
        public DateTime? COMPLETEDATE5
        {
            get
            {
                return (DateTime?)this["COMPLETEDATE5"];
            }
            set
            {
                this["COMPLETEDATE5"] = value;
            }
        }
        public string AUDITNAME1
        {
            get
            {
                return (string)this["AUDITNAME1"];
            }
            set
            {
                this["AUDITNAME1"] = value;
            }
        }
        public string AUDITNAME2
        {
            get
            {
                return (string)this["AUDITNAME2"];
            }
            set
            {
                this["AUDITNAME2"] = value;
            }
        }
        public string AUDITNAME3
        {
            get
            {
                return (string)this["AUDITNAME3"];
            }
            set
            {
                this["AUDITNAME3"] = value;
            }
        }
        public string AUDITNAME4
        {
            get
            {
                return (string)this["AUDITNAME4"];
            }
            set
            {
                this["AUDITNAME4"] = value;
            }
        }
        public string AUDITNAME5
        {
            get
            {
                return (string)this["AUDITNAME5"];
            }
            set
            {
                this["AUDITNAME5"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string RETURNREASON
        {
            get
            {
                return (string)this["RETURNREASON"];
            }
            set
            {
                this["RETURNREASON"] = value;
            }
        }
        public string QCFAILPRODPROCESS
        {
            get
            {
                return (string)this["QCFAILPRODPROCESS"];
            }
            set
            {
                this["QCFAILPRODPROCESS"] = value;
            }
        }
        public string ATTACHFILE3
        {
            get
            {
                return (string)this["ATTACHFILE3"];
            }
            set
            {
                this["ATTACHFILE3"] = value;
            }
        }
        public string QCPREPAREDBY
        {
            get
            {
                return (string)this["QCPREPAREDBY"];
            }
            set
            {
                this["QCPREPAREDBY"] = value;
            }
        }
        public DateTime? QCPREPAREDDATE
        {
            get
            {
                return (DateTime?)this["QCPREPAREDDATE"];
            }
            set
            {
                this["QCPREPAREDDATE"] = value;
            }
        }
        public string PROMIT
        {
            get
            {
                return (string)this["PROMIT"];
            }
            set
            {
                this["PROMIT"] = value;
            }
        }
        public DateTime? PROMITDATE
        {
            get
            {
                return (DateTime?)this["PROMITDATE"];
            }
            set
            {
                this["PROMITDATE"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string QAMANAGER
        {
            get
            {
                return (string)this["QAMANAGER"];
            }
            set
            {
                this["QAMANAGER"] = value;
            }
        }
        public double? ESTATUS
        {
            get
            {
                return (double?)this["ESTATUS"];
            }
            set
            {
                this["ESTATUS"] = value;
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
        public string CARNO
        {
            get
            {
                return (string)this["CARNO"];
            }
            set
            {
                this["CARNO"] = value;
            }
        }
        public string CARTYPE
        {
            get
            {
                return (string)this["CARTYPE"];
            }
            set
            {
                this["CARTYPE"] = value;
            }
        }
        public string CARTITLE
        {
            get
            {
                return (string)this["CARTITLE"];
            }
            set
            {
                this["CARTITLE"] = value;
            }
        }
        public string BUNAME
        {
            get
            {
                return (string)this["BUNAME"];
            }
            set
            {
                this["BUNAME"] = value;
            }
        }
        public string SKUNAME
        {
            get
            {
                return (string)this["SKUNAME"];
            }
            set
            {
                this["SKUNAME"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public string CURRENTDEP
        {
            get
            {
                return (string)this["CURRENTDEP"];
            }
            set
            {
                this["CURRENTDEP"] = value;
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
    }
    public class R_ONLINECAR
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string CARNO { get; set; }
        public string CARTYPE { get; set; }
        public string CARTITLE { get; set; }
        public string BUNAME { get; set; }
        public string SKUNAME { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PRODUCTIONLINE { get; set; }
        public string STATION { get; set; }
        public DateTime? FINDTIME { get; set; }
        public DateTime? TOTIME { get; set; }
        public double? SKUQTY { get; set; }
        public double? FAILQTY { get; set; }
        public string FAILTEXT { get; set; }
        public string ATTACHFILE1 { get; set; }
        public string FAILPNAME { get; set; }
        public DateTime? ISSUEDATE { get; set; }
        public double? OKTORELEASE { get; set; }
        public string FAILDEPART { get; set; }
        public string CONFIRMNAME { get; set; }
        public DateTime? CONFIRMDATE { get; set; }
        public string ROOTDEPART { get; set; }
        public string ROOTTEXT { get; set; }
        public string PATEXT { get; set; }
        public string ATTACHFILE2 { get; set; }
        public string ROOTPNAME { get; set; }
        public string ISCANCEL { get; set; }
        public DateTime? FADATE { get; set; }
        public string AUDITDEPART1 { get; set; }
        public string AUDITDEPART2 { get; set; }
        public string AUDITDEPART3 { get; set; }
        public string AUDITDEPART4 { get; set; }
        public string AUDITDEPART5 { get; set; }
        public string MODIPART1 { get; set; }
        public string MODIPART2 { get; set; }
        public string MODIPART3 { get; set; }
        public string MODIPART4 { get; set; }
        public string MODIPART5 { get; set; }
        public DateTime? YCOMPLETEDDATE1 { get; set; }
        public DateTime? YCOMPLETEDDATE2 { get; set; }
        public DateTime? YCOMPLETEDDATE3 { get; set; }
        public DateTime? YCOMPLETEDDATE4 { get; set; }
        public DateTime? YCOMPLETEDDATE5 { get; set; }
        public DateTime? COMPLETEDDATE1 { get; set; }
        public DateTime? COMPLETEDATE2 { get; set; }
        public DateTime? COMPLETEDATE3 { get; set; }
        public DateTime? COMPLETEDATE4 { get; set; }
        public DateTime? COMPLETEDATE5 { get; set; }
        public string AUDITNAME1 { get; set; }
        public string AUDITNAME2 { get; set; }
        public string AUDITNAME3 { get; set; }
        public string AUDITNAME4 { get; set; }
        public string AUDITNAME5 { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string RETURNREASON { get; set; }
        public string QCFAILPRODPROCESS { get; set; }
        public string ATTACHFILE3 { get; set; }
        public string QCPREPAREDBY { get; set; }
        public DateTime? QCPREPAREDDATE { get; set; }
        public string PROMIT { get; set; }
        public DateTime? PROMITDATE { get; set; }
        public string REMARK { get; set; }
        public string QAMANAGER { get; set; }
        public double? ESTATUS { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public string CURRENTDEP { get; set; }        
    }
}