using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter.Class
{
    public class DBTable
    {
        public List<DBCol> Cols = new List<DBCol>();
        public string DBTableName;

        public static DBTable GetTestTable_R_SN()
        {
            DBTable t1 = new DBTable();
            t1.DBTableName = "R_SN";
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "ID" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "SN" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "SKUNO" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "WORKORDERNO" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "PLANT" });

            return t1;
        }

        public static DBTable GetTestTable_R_WO_BASE()
        {
            DBTable t1 = new DBTable();
            t1.DBTableName = "R_WO_BASE";
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "ID" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "WORKORDERNO" });
            t1.Cols.Add(new DBCol() { DataType = typeof(string), Name = "SKUNO" });
            t1.Cols.Add(new DBCol() { DataType = typeof(int), Name = "WORKORDERN_QTY" });
            t1.Cols.Add(new DBCol() { DataType = typeof(int), Name = "INPUT_QTY" });

            return t1;
        }


    }


}
