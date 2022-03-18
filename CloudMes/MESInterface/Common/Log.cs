using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWDNNSFCBase;
using System.Data;
using System.Data.OleDb;

namespace MESInterface.Common
{
    public class Log
    {
        OleExec _db;
        public Log(OleExec SFCDB)
        {
            _db = SFCDB;
        }
        
        public void WriterLog(string LogType,string data1, string data2, string data3, string data4,string data5,string data6,string data9)
        {
            string strSql = " INSERT INTO servicelog(FUNCTIONTYPE,CURRENTEDITTIME,DATA1,DATA2,DATA3,DATA4,DATA5,DATA6,DATA9) VALUES ('" + LogType + "',GETDATE(),'" + data1 + "','" + data2 + "','" + data3 + "','" + data4 + "','" + data5 + "','" + data6 + "','" + data9 + "')   ";
            _db.ExecSQL(strSql);
        }

        public void WriterLog(string LogType, string data1, string data2, string data3, string data4, string data5, string data6, string data7, string data8, string data9)
        {
            string strSql = " INSERT INTO servicelog(FUNCTIONTYPE,CURRENTEDITTIME,DATA1,DATA2,DATA3,DATA4,DATA5,DATA6,DATA7,DATA8,DATA9) VALUES ('" + LogType + "',GETDATE(),'" + data1 + "','" + data2 + "','" + data3 + "','" + data4 + "','" + data5 + "','" + data6 + "','" + data7 + "','" + data8 + "','" + data9 + "')   ";
            _db.ExecSQL(strSql);
        }

        public void UpdateStatus(string LogType,string StartOrEnd)
        {
            string strSql = "  UPDATE servicelog SET currentedittime=GETDATE(),lastedittime=currentedittime WHERE functiontype='" + LogType + "' and data1='RunStatus' and data2='" + StartOrEnd + "' ";
            _db.ExecSQL(strSql);
        }
    }
}
