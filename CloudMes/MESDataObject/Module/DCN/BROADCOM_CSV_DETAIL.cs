using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.DCN
{
    public class T_BROADCOM_CSV_DETAIL : DataObjectTable
    {
        public T_BROADCOM_CSV_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_BROADCOM_CSV_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_BROADCOM_CSV_DETAIL);
            TableName = "BROADCOM_CSV_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_BROADCOM_CSV_DETAIL : DataObjectBase
    {
        public Row_BROADCOM_CSV_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public BROADCOM_CSV_DETAIL GetDataObject()
        {
            BROADCOM_CSV_DETAIL DataObject = new BROADCOM_CSV_DETAIL();
            DataObject.V90 = this.V90;
            DataObject.V91 = this.V91;
            DataObject.V92 = this.V92;
            DataObject.V93 = this.V93;
            DataObject.V94 = this.V94;
            DataObject.V95 = this.V95;
            DataObject.V96 = this.V96;
            DataObject.V97 = this.V97;
            DataObject.V98 = this.V98;
            DataObject.V99 = this.V99;
            DataObject.V100 = this.V100;
            DataObject.CREATEDT = this.CREATEDT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.RECNO = this.RECNO;
            DataObject.V1 = this.V1;
            DataObject.V2 = this.V2;
            DataObject.V3 = this.V3;
            DataObject.V4 = this.V4;
            DataObject.V5 = this.V5;
            DataObject.V6 = this.V6;
            DataObject.V7 = this.V7;
            DataObject.V8 = this.V8;
            DataObject.V9 = this.V9;
            DataObject.V10 = this.V10;
            DataObject.V11 = this.V11;
            DataObject.V12 = this.V12;
            DataObject.V13 = this.V13;
            DataObject.V14 = this.V14;
            DataObject.V15 = this.V15;
            DataObject.V16 = this.V16;
            DataObject.V17 = this.V17;
            DataObject.V18 = this.V18;
            DataObject.V19 = this.V19;
            DataObject.V20 = this.V20;
            DataObject.V21 = this.V21;
            DataObject.V22 = this.V22;
            DataObject.V23 = this.V23;
            DataObject.V24 = this.V24;
            DataObject.V25 = this.V25;
            DataObject.V26 = this.V26;
            DataObject.V27 = this.V27;
            DataObject.V28 = this.V28;
            DataObject.V29 = this.V29;
            DataObject.V30 = this.V30;
            DataObject.V31 = this.V31;
            DataObject.V32 = this.V32;
            DataObject.V33 = this.V33;
            DataObject.V34 = this.V34;
            DataObject.V35 = this.V35;
            DataObject.V36 = this.V36;
            DataObject.V37 = this.V37;
            DataObject.V38 = this.V38;
            DataObject.V39 = this.V39;
            DataObject.V40 = this.V40;
            DataObject.V41 = this.V41;
            DataObject.V42 = this.V42;
            DataObject.V43 = this.V43;
            DataObject.V44 = this.V44;
            DataObject.V45 = this.V45;
            DataObject.V46 = this.V46;
            DataObject.V47 = this.V47;
            DataObject.V48 = this.V48;
            DataObject.V49 = this.V49;
            DataObject.V50 = this.V50;
            DataObject.V51 = this.V51;
            DataObject.V52 = this.V52;
            DataObject.V53 = this.V53;
            DataObject.V54 = this.V54;
            DataObject.V55 = this.V55;
            DataObject.V56 = this.V56;
            DataObject.V57 = this.V57;
            DataObject.V58 = this.V58;
            DataObject.V59 = this.V59;
            DataObject.V60 = this.V60;
            DataObject.V61 = this.V61;
            DataObject.V62 = this.V62;
            DataObject.V63 = this.V63;
            DataObject.V64 = this.V64;
            DataObject.V65 = this.V65;
            DataObject.V66 = this.V66;
            DataObject.V67 = this.V67;
            DataObject.V68 = this.V68;
            DataObject.V69 = this.V69;
            DataObject.V70 = this.V70;
            DataObject.V71 = this.V71;
            DataObject.V72 = this.V72;
            DataObject.V73 = this.V73;
            DataObject.V74 = this.V74;
            DataObject.V75 = this.V75;
            DataObject.V76 = this.V76;
            DataObject.V77 = this.V77;
            DataObject.V78 = this.V78;
            DataObject.V79 = this.V79;
            DataObject.V80 = this.V80;
            DataObject.V81 = this.V81;
            DataObject.V82 = this.V82;
            DataObject.V83 = this.V83;
            DataObject.V84 = this.V84;
            DataObject.V85 = this.V85;
            DataObject.V86 = this.V86;
            DataObject.V87 = this.V87;
            DataObject.V88 = this.V88;
            DataObject.V89 = this.V89;
            return DataObject;
        }
        public string V90
        {
            get
            {
                return (string)this["V90"];
            }
            set
            {
                this["V90"] = value;
            }
        }
        public string V91
        {
            get
            {
                return (string)this["V91"];
            }
            set
            {
                this["V91"] = value;
            }
        }
        public string V92
        {
            get
            {
                return (string)this["V92"];
            }
            set
            {
                this["V92"] = value;
            }
        }
        public string V93
        {
            get
            {
                return (string)this["V93"];
            }
            set
            {
                this["V93"] = value;
            }
        }
        public string V94
        {
            get
            {
                return (string)this["V94"];
            }
            set
            {
                this["V94"] = value;
            }
        }
        public string V95
        {
            get
            {
                return (string)this["V95"];
            }
            set
            {
                this["V95"] = value;
            }
        }
        public string V96
        {
            get
            {
                return (string)this["V96"];
            }
            set
            {
                this["V96"] = value;
            }
        }
        public string V97
        {
            get
            {
                return (string)this["V97"];
            }
            set
            {
                this["V97"] = value;
            }
        }
        public string V98
        {
            get
            {
                return (string)this["V98"];
            }
            set
            {
                this["V98"] = value;
            }
        }
        public string V99
        {
            get
            {
                return (string)this["V99"];
            }
            set
            {
                this["V99"] = value;
            }
        }
        public string V100
        {
            get
            {
                return (string)this["V100"];
            }
            set
            {
                this["V100"] = value;
            }
        }
        public DateTime? CREATEDT
        {
            get
            {
                return (DateTime?)this["CREATEDT"];
            }
            set
            {
                this["CREATEDT"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public double? RECNO
        {
            get
            {
                return (double?)this["RECNO"];
            }
            set
            {
                this["RECNO"] = value;
            }
        }
        public string V1
        {
            get
            {
                return (string)this["V1"];
            }
            set
            {
                this["V1"] = value;
            }
        }
        public string V2
        {
            get
            {
                return (string)this["V2"];
            }
            set
            {
                this["V2"] = value;
            }
        }
        public string V3
        {
            get
            {
                return (string)this["V3"];
            }
            set
            {
                this["V3"] = value;
            }
        }
        public string V4
        {
            get
            {
                return (string)this["V4"];
            }
            set
            {
                this["V4"] = value;
            }
        }
        public string V5
        {
            get
            {
                return (string)this["V5"];
            }
            set
            {
                this["V5"] = value;
            }
        }
        public string V6
        {
            get
            {
                return (string)this["V6"];
            }
            set
            {
                this["V6"] = value;
            }
        }
        public string V7
        {
            get
            {
                return (string)this["V7"];
            }
            set
            {
                this["V7"] = value;
            }
        }
        public string V8
        {
            get
            {
                return (string)this["V8"];
            }
            set
            {
                this["V8"] = value;
            }
        }
        public string V9
        {
            get
            {
                return (string)this["V9"];
            }
            set
            {
                this["V9"] = value;
            }
        }
        public string V10
        {
            get
            {
                return (string)this["V10"];
            }
            set
            {
                this["V10"] = value;
            }
        }
        public string V11
        {
            get
            {
                return (string)this["V11"];
            }
            set
            {
                this["V11"] = value;
            }
        }
        public string V12
        {
            get
            {
                return (string)this["V12"];
            }
            set
            {
                this["V12"] = value;
            }
        }
        public string V13
        {
            get
            {
                return (string)this["V13"];
            }
            set
            {
                this["V13"] = value;
            }
        }
        public string V14
        {
            get
            {
                return (string)this["V14"];
            }
            set
            {
                this["V14"] = value;
            }
        }
        public string V15
        {
            get
            {
                return (string)this["V15"];
            }
            set
            {
                this["V15"] = value;
            }
        }
        public string V16
        {
            get
            {
                return (string)this["V16"];
            }
            set
            {
                this["V16"] = value;
            }
        }
        public string V17
        {
            get
            {
                return (string)this["V17"];
            }
            set
            {
                this["V17"] = value;
            }
        }
        public string V18
        {
            get
            {
                return (string)this["V18"];
            }
            set
            {
                this["V18"] = value;
            }
        }
        public string V19
        {
            get
            {
                return (string)this["V19"];
            }
            set
            {
                this["V19"] = value;
            }
        }
        public string V20
        {
            get
            {
                return (string)this["V20"];
            }
            set
            {
                this["V20"] = value;
            }
        }
        public string V21
        {
            get
            {
                return (string)this["V21"];
            }
            set
            {
                this["V21"] = value;
            }
        }
        public string V22
        {
            get
            {
                return (string)this["V22"];
            }
            set
            {
                this["V22"] = value;
            }
        }
        public string V23
        {
            get
            {
                return (string)this["V23"];
            }
            set
            {
                this["V23"] = value;
            }
        }
        public string V24
        {
            get
            {
                return (string)this["V24"];
            }
            set
            {
                this["V24"] = value;
            }
        }
        public string V25
        {
            get
            {
                return (string)this["V25"];
            }
            set
            {
                this["V25"] = value;
            }
        }
        public string V26
        {
            get
            {
                return (string)this["V26"];
            }
            set
            {
                this["V26"] = value;
            }
        }
        public string V27
        {
            get
            {
                return (string)this["V27"];
            }
            set
            {
                this["V27"] = value;
            }
        }
        public string V28
        {
            get
            {
                return (string)this["V28"];
            }
            set
            {
                this["V28"] = value;
            }
        }
        public string V29
        {
            get
            {
                return (string)this["V29"];
            }
            set
            {
                this["V29"] = value;
            }
        }
        public string V30
        {
            get
            {
                return (string)this["V30"];
            }
            set
            {
                this["V30"] = value;
            }
        }
        public string V31
        {
            get
            {
                return (string)this["V31"];
            }
            set
            {
                this["V31"] = value;
            }
        }
        public string V32
        {
            get
            {
                return (string)this["V32"];
            }
            set
            {
                this["V32"] = value;
            }
        }
        public string V33
        {
            get
            {
                return (string)this["V33"];
            }
            set
            {
                this["V33"] = value;
            }
        }
        public string V34
        {
            get
            {
                return (string)this["V34"];
            }
            set
            {
                this["V34"] = value;
            }
        }
        public string V35
        {
            get
            {
                return (string)this["V35"];
            }
            set
            {
                this["V35"] = value;
            }
        }
        public string V36
        {
            get
            {
                return (string)this["V36"];
            }
            set
            {
                this["V36"] = value;
            }
        }
        public string V37
        {
            get
            {
                return (string)this["V37"];
            }
            set
            {
                this["V37"] = value;
            }
        }
        public string V38
        {
            get
            {
                return (string)this["V38"];
            }
            set
            {
                this["V38"] = value;
            }
        }
        public string V39
        {
            get
            {
                return (string)this["V39"];
            }
            set
            {
                this["V39"] = value;
            }
        }
        public string V40
        {
            get
            {
                return (string)this["V40"];
            }
            set
            {
                this["V40"] = value;
            }
        }
        public string V41
        {
            get
            {
                return (string)this["V41"];
            }
            set
            {
                this["V41"] = value;
            }
        }
        public string V42
        {
            get
            {
                return (string)this["V42"];
            }
            set
            {
                this["V42"] = value;
            }
        }
        public string V43
        {
            get
            {
                return (string)this["V43"];
            }
            set
            {
                this["V43"] = value;
            }
        }
        public string V44
        {
            get
            {
                return (string)this["V44"];
            }
            set
            {
                this["V44"] = value;
            }
        }
        public string V45
        {
            get
            {
                return (string)this["V45"];
            }
            set
            {
                this["V45"] = value;
            }
        }
        public string V46
        {
            get
            {
                return (string)this["V46"];
            }
            set
            {
                this["V46"] = value;
            }
        }
        public string V47
        {
            get
            {
                return (string)this["V47"];
            }
            set
            {
                this["V47"] = value;
            }
        }
        public string V48
        {
            get
            {
                return (string)this["V48"];
            }
            set
            {
                this["V48"] = value;
            }
        }
        public string V49
        {
            get
            {
                return (string)this["V49"];
            }
            set
            {
                this["V49"] = value;
            }
        }
        public string V50
        {
            get
            {
                return (string)this["V50"];
            }
            set
            {
                this["V50"] = value;
            }
        }
        public string V51
        {
            get
            {
                return (string)this["V51"];
            }
            set
            {
                this["V51"] = value;
            }
        }
        public string V52
        {
            get
            {
                return (string)this["V52"];
            }
            set
            {
                this["V52"] = value;
            }
        }
        public string V53
        {
            get
            {
                return (string)this["V53"];
            }
            set
            {
                this["V53"] = value;
            }
        }
        public string V54
        {
            get
            {
                return (string)this["V54"];
            }
            set
            {
                this["V54"] = value;
            }
        }
        public string V55
        {
            get
            {
                return (string)this["V55"];
            }
            set
            {
                this["V55"] = value;
            }
        }
        public string V56
        {
            get
            {
                return (string)this["V56"];
            }
            set
            {
                this["V56"] = value;
            }
        }
        public string V57
        {
            get
            {
                return (string)this["V57"];
            }
            set
            {
                this["V57"] = value;
            }
        }
        public string V58
        {
            get
            {
                return (string)this["V58"];
            }
            set
            {
                this["V58"] = value;
            }
        }
        public string V59
        {
            get
            {
                return (string)this["V59"];
            }
            set
            {
                this["V59"] = value;
            }
        }
        public string V60
        {
            get
            {
                return (string)this["V60"];
            }
            set
            {
                this["V60"] = value;
            }
        }
        public string V61
        {
            get
            {
                return (string)this["V61"];
            }
            set
            {
                this["V61"] = value;
            }
        }
        public string V62
        {
            get
            {
                return (string)this["V62"];
            }
            set
            {
                this["V62"] = value;
            }
        }
        public string V63
        {
            get
            {
                return (string)this["V63"];
            }
            set
            {
                this["V63"] = value;
            }
        }
        public string V64
        {
            get
            {
                return (string)this["V64"];
            }
            set
            {
                this["V64"] = value;
            }
        }
        public string V65
        {
            get
            {
                return (string)this["V65"];
            }
            set
            {
                this["V65"] = value;
            }
        }
        public string V66
        {
            get
            {
                return (string)this["V66"];
            }
            set
            {
                this["V66"] = value;
            }
        }
        public string V67
        {
            get
            {
                return (string)this["V67"];
            }
            set
            {
                this["V67"] = value;
            }
        }
        public string V68
        {
            get
            {
                return (string)this["V68"];
            }
            set
            {
                this["V68"] = value;
            }
        }
        public string V69
        {
            get
            {
                return (string)this["V69"];
            }
            set
            {
                this["V69"] = value;
            }
        }
        public string V70
        {
            get
            {
                return (string)this["V70"];
            }
            set
            {
                this["V70"] = value;
            }
        }
        public string V71
        {
            get
            {
                return (string)this["V71"];
            }
            set
            {
                this["V71"] = value;
            }
        }
        public string V72
        {
            get
            {
                return (string)this["V72"];
            }
            set
            {
                this["V72"] = value;
            }
        }
        public string V73
        {
            get
            {
                return (string)this["V73"];
            }
            set
            {
                this["V73"] = value;
            }
        }
        public string V74
        {
            get
            {
                return (string)this["V74"];
            }
            set
            {
                this["V74"] = value;
            }
        }
        public string V75
        {
            get
            {
                return (string)this["V75"];
            }
            set
            {
                this["V75"] = value;
            }
        }
        public string V76
        {
            get
            {
                return (string)this["V76"];
            }
            set
            {
                this["V76"] = value;
            }
        }
        public string V77
        {
            get
            {
                return (string)this["V77"];
            }
            set
            {
                this["V77"] = value;
            }
        }
        public string V78
        {
            get
            {
                return (string)this["V78"];
            }
            set
            {
                this["V78"] = value;
            }
        }
        public string V79
        {
            get
            {
                return (string)this["V79"];
            }
            set
            {
                this["V79"] = value;
            }
        }
        public string V80
        {
            get
            {
                return (string)this["V80"];
            }
            set
            {
                this["V80"] = value;
            }
        }
        public string V81
        {
            get
            {
                return (string)this["V81"];
            }
            set
            {
                this["V81"] = value;
            }
        }
        public string V82
        {
            get
            {
                return (string)this["V82"];
            }
            set
            {
                this["V82"] = value;
            }
        }
        public string V83
        {
            get
            {
                return (string)this["V83"];
            }
            set
            {
                this["V83"] = value;
            }
        }
        public string V84
        {
            get
            {
                return (string)this["V84"];
            }
            set
            {
                this["V84"] = value;
            }
        }
        public string V85
        {
            get
            {
                return (string)this["V85"];
            }
            set
            {
                this["V85"] = value;
            }
        }
        public string V86
        {
            get
            {
                return (string)this["V86"];
            }
            set
            {
                this["V86"] = value;
            }
        }
        public string V87
        {
            get
            {
                return (string)this["V87"];
            }
            set
            {
                this["V87"] = value;
            }
        }
        public string V88
        {
            get
            {
                return (string)this["V88"];
            }
            set
            {
                this["V88"] = value;
            }
        }
        public string V89
        {
            get
            {
                return (string)this["V89"];
            }
            set
            {
                this["V89"] = value;
            }
        }
    }
    public class BROADCOM_CSV_DETAIL
    {
        public string V90 { get; set; }
        public string V91 { get; set; }
        public string V92 { get; set; }
        public string V93 { get; set; }
        public string V94 { get; set; }
        public string V95 { get; set; }
        public string V96 { get; set; }
        public string V97 { get; set; }
        public string V98 { get; set; }
        public string V99 { get; set; }
        public string V100 { get; set; }
        public DateTime? CREATEDT { get; set; }
        public string FILENAME { get; set; }
        public double? RECNO { get; set; }
        public string V1 { get; set; }
        public string V2 { get; set; }
        public string V3 { get; set; }
        public string V4 { get; set; }
        public string V5 { get; set; }
        public string V6 { get; set; }
        public string V7 { get; set; }
        public string V8 { get; set; }
        public string V9 { get; set; }
        public string V10 { get; set; }
        public string V11 { get; set; }
        public string V12 { get; set; }
        public string V13 { get; set; }
        public string V14 { get; set; }
        public string V15 { get; set; }
        public string V16 { get; set; }
        public string V17 { get; set; }
        public string V18 { get; set; }
        public string V19 { get; set; }
        public string V20 { get; set; }
        public string V21 { get; set; }
        public string V22 { get; set; }
        public string V23 { get; set; }
        public string V24 { get; set; }
        public string V25 { get; set; }
        public string V26 { get; set; }
        public string V27 { get; set; }
        public string V28 { get; set; }
        public string V29 { get; set; }
        public string V30 { get; set; }
        public string V31 { get; set; }
        public string V32 { get; set; }
        public string V33 { get; set; }
        public string V34 { get; set; }
        public string V35 { get; set; }
        public string V36 { get; set; }
        public string V37 { get; set; }
        public string V38 { get; set; }
        public string V39 { get; set; }
        public string V40 { get; set; }
        public string V41 { get; set; }
        public string V42 { get; set; }
        public string V43 { get; set; }
        public string V44 { get; set; }
        public string V45 { get; set; }
        public string V46 { get; set; }
        public string V47 { get; set; }
        public string V48 { get; set; }
        public string V49 { get; set; }
        public string V50 { get; set; }
        public string V51 { get; set; }
        public string V52 { get; set; }
        public string V53 { get; set; }
        public string V54 { get; set; }
        public string V55 { get; set; }
        public string V56 { get; set; }
        public string V57 { get; set; }
        public string V58 { get; set; }
        public string V59 { get; set; }
        public string V60 { get; set; }
        public string V61 { get; set; }
        public string V62 { get; set; }
        public string V63 { get; set; }
        public string V64 { get; set; }
        public string V65 { get; set; }
        public string V66 { get; set; }
        public string V67 { get; set; }
        public string V68 { get; set; }
        public string V69 { get; set; }
        public string V70 { get; set; }
        public string V71 { get; set; }
        public string V72 { get; set; }
        public string V73 { get; set; }
        public string V74 { get; set; }
        public string V75 { get; set; }
        public string V76 { get; set; }
        public string V77 { get; set; }
        public string V78 { get; set; }
        public string V79 { get; set; }
        public string V80 { get; set; }
        public string V81 { get; set; }
        public string V82 { get; set; }
        public string V83 { get; set; }
        public string V84 { get; set; }
        public string V85 { get; set; }
        public string V86 { get; set; }
        public string V87 { get; set; }
        public string V88 { get; set; }
        public string V89 { get; set; }
    }
}
