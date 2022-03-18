using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MESStation.Label.Public
{
    class CustSnValueGroup: LabelValueGroup
    {
        public CustSnValueGroup()
        {
            ConfigGroup = "CustSnValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN1", Description = "GetCustsnSSN1", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN2", Description = "GetCustsnSSN2", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN3", Description = "GetCustsnSSN3", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN4", Description = "GetCustsnSSN4", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN5", Description = "GetCustsnSSN5", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN6", Description = "GetCustsnSSN6", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN7", Description = "GetCustsnSSN7", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN8", Description = "GetCustsnSSN8", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN9", Description = "GetCustsnSSN9", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN10", Description = "GetCustsnSSN10", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN11", Description = "GetCustsnSSN11", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN12", Description = "GetCustsnSSN12", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN13", Description = "GetCustsnSSN13", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN14", Description = "GetCustsnSSN14", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN15", Description = "GetCustsnSSN15", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN16", Description = "GetCustsnSSN16", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN17", Description = "GetCustsnSSN17", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN18", Description = "GetCustsnSSN18", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN19", Description = "GetCustsnSSN19", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN20", Description = "GetCustsnSSN20", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN21", Description = "GetCustsnSSN21", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnSSN22", Description = "GetCustsnSSN22", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC1", Description = "GetCustsnMAC1", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC2", Description = "GetCustsnMAC2", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC3", Description = "GetCustsnMAC3", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC4", Description = "GetCustsnMAC4", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC5", Description = "GetCustsnMAC5", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC6", Description = "GetCustsnMAC6", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC7", Description = "GetCustsnMAC7", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC8", Description = "GetCustsnMAC8", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC9", Description = "GetCustsnMAC9", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC10", Description = "GetCustsnMAC10", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC11", Description = "GetCustsnMAC11", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC12", Description = "GetCustsnMAC12", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC13", Description = "GetCustsnMAC13", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC14", Description = "GetCustsnMAC14", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC15", Description = "GetCustsnMAC15", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC16", Description = "GetCustsnMAC16", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC17", Description = "GetCustsnMAC17", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC18", Description = "GetCustsnMAC18", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC19", Description = "GetCustsnMAC19", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustsnMAC20", Description = "GetCustsnMAC20", Paras = new List<string>() { "SN" } });
        }

        public string GetCustsnSSN1(OleExec SFCDB, string SN)
        {

            var SSN1 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN1)
                .First();
            return SSN1;
        }

        public string GetCustsnSSN2(OleExec SFCDB, string SN)
        {

           var SSN2 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN2)
                .First();
            return SSN2;
        }

        public string GetCustsnSSN3(OleExec SFCDB, string SN)
        {

           var SSN3 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN3)
                .First();
            return SSN3;
        }

        public string GetCustsnSSN4(OleExec SFCDB, string SN)
        {

           var SSN4 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN4)
                .First();
            return SSN4 ;
        }

        public string GetCustsnSSN5(OleExec SFCDB, string SN)
        {

           var SSN5 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN5)
                .First();
            return SSN5;
        }

        public string GetCustsnSSN6(OleExec SFCDB, string SN)
        {

            var SSN6 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN6)
                .First();
            return SSN6;
        }

        public string GetCustsnSSN7(OleExec SFCDB, string SN)
        {

           var SSN7 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN7)
                .First();
            return SSN7;
        }

        public string GetCustsnSSN8(OleExec SFCDB, string SN)
        {

            var SSN8 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN8)
                .First();
            return SSN8;
        }

        public string GetCustsnSSN9(OleExec SFCDB, string SN)
        {

            var SSN9 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN9)
                .First();
            return SSN9;
        }

        public string GetCustsnSSN10(OleExec SFCDB, string SN)
        {

            var SSN10 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN10)
                .First();
            return SSN10;
        }

        public string GetCustsnSSN11(OleExec SFCDB, string SN)
        {

            var SSN11 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN11)
                .First();
            return SSN11;
        }

        public string GetCustsnSSN12(OleExec SFCDB, string SN)
        {

            var SSN12 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN12)
                .First();
            return SSN12;
        }

        public string GetCustsnSSN13(OleExec SFCDB, string SN)
        {

            var SSN13 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN13)
                .First();
            return SSN13;
        }

        public string GetCustsnSSN14(OleExec SFCDB, string SN)
        {

            var SSN14 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN14)
                .First();
            return SSN14;
        }

        public string GetCustsnSSN15(OleExec SFCDB, string SN)
        {

            var SSN15 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN15)
                .First();
            return SSN15;
        }

        public string GetCustsnSSN16(OleExec SFCDB, string SN)
        {

            var SSN16 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN16)
                .First();
            return SSN16;
        }

        public string GetCustsnSSN17(OleExec SFCDB, string SN)
        {

            var SSN17 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN17)
                .First();
            return SSN17;
        }

        public string GetCustsnSSN18(OleExec SFCDB, string SN)
        {

            var SSN18 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN18)
                .First();
            return SSN18;
        }

        public string GetCustsnSSN19(OleExec SFCDB, string SN)
        {

           var SSN19 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN19)
                .First();
            return SSN19;
        }

        public string GetCustsnSSN20(OleExec SFCDB, string SN)
        {

            var SSN20 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN20)
                .First();
            return SSN20;
        }

        public string GetCustsnSSN21(OleExec SFCDB, string SN)
        {

            var SSN21 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN21)
                .First();
            return SSN21;
        }

        public string GetCustsnSSN22(OleExec SFCDB, string SN)
        {

            var SSN22 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.SSN22)
                .First();
            return SSN22;
        }

        public string GetCustsnMAC1(OleExec SFCDB, string SN)
        {

            var MAC1 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC1)
                .First();
            return MAC1;
        }

        public string GetCustsnMAC2(OleExec SFCDB, string SN)
        {

           var MAC2 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC2)
                .First();
            return MAC2;
        }

        public string GetCustsnMAC3(OleExec SFCDB, string SN)
        {

            var MAC3 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC3)
                .First();
            return MAC3;
        }

        public string GetCustsnMAC4(OleExec SFCDB, string SN)
        {

            var MAC4 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC4)
                .First();
            return MAC4;
        }

        public string GetCustsnMAC5(OleExec SFCDB, string SN)
        {

            var MAC5 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC5)
                .First();
            return MAC5;
        }

        public string GetCustsnMAC6(OleExec SFCDB, string SN)
        {

           var MAC6 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC6)
                .First();
            return MAC6;
        }

        public string GetCustsnMAC7(OleExec SFCDB, string SN)
        {

            var MAC7 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC7)
                .First();
            return MAC7;
        }

        public string GetCustsnMAC8(OleExec SFCDB, string SN)
        {

            var MAC8 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC8)
                .First();
            return MAC8;
        }

        public string GetCustsnMAC9(OleExec SFCDB, string SN)
        {

            var MAC9 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC9)
                .First();
            return MAC9;
        }

        public string GetCustsnMAC10(OleExec SFCDB, string SN)
        {

            var MAC10 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC10)
                .First();
            return MAC10;
        }

        public string GetCustsnMAC11(OleExec SFCDB, string SN)
        {

            var MAC11 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC11)
                .First();
            return MAC11;
        }

        public string GetCustsnMAC12(OleExec SFCDB, string SN)
        {

            var MAC12 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC12)
                .First();
            return MAC12;
        }

        public string GetCustsnMAC13(OleExec SFCDB, string SN)
        {

            var MAC13 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC13)
                .First();
            return MAC13;
        }

        public string GetCustsnMAC14(OleExec SFCDB, string SN)
        {

           var MAC14 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC14)
                .First();
            return MAC14;
        }

        public string GetCustsnMAC15(OleExec SFCDB, string SN)
        {

            var MAC15 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC15)
                .First();
            return MAC15;
        }

        public string GetCustsnMAC16(OleExec SFCDB, string SN)
        {

           var MAC16 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC16)
                .First();
            return MAC16;
        }

        public string GetCustsnMAC17(OleExec SFCDB, string SN)
        {

            var MAC17 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC17)
                .First();
            return MAC17;
        }

        public string GetCustsnMAC18(OleExec SFCDB, string SN)
        {

           var MAC18 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC18)
                .First();
            return MAC18;
        }

        public string GetCustsnMAC19(OleExec SFCDB, string SN)
        {

            var MAC19 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC19)
                .First();
            return MAC19;
        }

        public string GetCustsnMAC20(OleExec SFCDB, string SN)
        {

            var MAC20 = SFCDB.ORM.Queryable<R_CUSTSN_T>()
                .Where(t => t.SERIAL_NUMBER == SN)
                .Select(t => t.MAC20)
                .First();
            return MAC20;
        }
    }
}
