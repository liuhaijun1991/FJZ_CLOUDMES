using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Reflection;
using MESDataObject;
using System.Data;
using MESPubLab.MESStation;

namespace MESStation.Packing
{
    public class PackingBase
    {
        public static List<C_PACKING> GetPackingConfigBySKU(string SkuNO , OleExec DB)
        {
            List<C_PACKING> ret = new List<C_PACKING>();
            T_C_PACKING TCP = new T_C_PACKING(DB, DB_TYPE_ENUM.Oracle);
            ret = TCP.GetPackingBySku(SkuNO, DB);
            return ret;

        }

        public static Row_R_PACKING GetNewPacking(C_PACKING config, string line, string Station, string IP,string BU ,  string User,OleExec DB)
        {
            T_R_PACKING TRP = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            Row_R_PACKING RRP = (Row_R_PACKING)TRP.NewRow();
            RRP.PACK_NO = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(config.SN_RULE, DB);
            RRP.PACK_TYPE = config.PACK_TYPE;
            RRP.PARENT_PACK_ID = "";
            RRP.SKUNO = config.SKUNO;
            RRP.MAX_QTY = config.MAX_QTY;
            RRP.QTY = 0;
            RRP.CLOSED_FLAG = "0";
            RRP.LINE = line;
            RRP.STATION = Station;
            RRP.ID = TRP.GetNewID(BU, DB);
            RRP.IP = IP;
            string result = DB.ExecSQL(RRP.GetInsertString(DB_TYPE_ENUM.Oracle));
            int r = 0;
            bool bInt = Int32.TryParse(result, out r);
            if (!bInt)
            {
                throw new Exception(result);
            }
            if (r == 0)
            {
                throw new Exception($@"Insert R_SN_PACKING Fail!{RRP.PACK_NO },{config.PACK_TYPE}");
            }
            RRP.AcceptChange();
            return RRP;

        }

        public static Row_R_PACKING GetNewPackNo(C_PACKING config,string packNo, string line, string Station, string IP, string BU, string User, OleExec DB)
        {
            T_R_PACKING TRP = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            Row_R_PACKING RRP = (Row_R_PACKING)TRP.NewRow();
            RRP.PACK_NO = packNo;
            RRP.PACK_TYPE = config.PACK_TYPE;
            RRP.PARENT_PACK_ID = "";
            RRP.SKUNO = config.SKUNO;
            RRP.MAX_QTY = config.MAX_QTY;
            RRP.QTY = 0;
            RRP.CLOSED_FLAG = "0";
            RRP.LINE = line;
            RRP.STATION = Station;
            RRP.ID = TRP.GetNewID(BU, DB);
            RRP.IP = IP;
            string result= DB.ExecSQL(RRP.GetInsertString(DB_TYPE_ENUM.Oracle));
            int r = 0;
            bool bInt = Int32.TryParse(result, out r);
            if (!bInt)
            {
                throw new Exception(result);
            }
            if (r == 0)
            {
                throw new Exception($@"Insert R_SN_PACKING Fail!{packNo},{config.PACK_TYPE}");
            }
            RRP.AcceptChange();
            return RRP;

        }

    }

    public class CartionBase
    {
        public Row_R_PACKING DATA;
        public CartionBase(Row_R_PACKING data)
        {
            DATA = data;
            if (data.PACK_TYPE != "CARTON")
            {
                throw new Exception("PackType is not CARTON");
            }
        }
        public int GetCount(OleExec DB)
        {
            string strSQL = $@"select count(1) from R_SN_PACKING where PACK_ID = '{DATA.ID}'";
            return int.Parse( DB.ExecSelectOneValue(strSQL).ToString());
        }
        public List<Row_R_SN_PACKING> GetList(OleExec DB)
        {
            List<Row_R_SN_PACKING> ret = null;
            T_R_SN_PACKING TRSP = new T_R_SN_PACKING(DB, DB_TYPE_ENUM.Oracle);
            ret = TRSP.GetPackItem(DATA.ID ,DB);
            return ret;
        }
        public void Add(LogicObject.SN SN,string BU ,string user,OleExec DB)
        {
            if (SN.SkuNo != DATA.SKUNO)
            {
                throw new Exception($@"SN.SKUNO ={SN.SkuNo} PACK.SKUNO={DATA.SKUNO}");
            }
            T_R_SN_PACKING TRSP = new T_R_SN_PACKING(DB, DB_TYPE_ENUM.Oracle);
            if (DATA.MAX_QTY <= GetCount(DB))
            {
                throw new Exception($@"{DATA.PACK_NO} is Full");
            }
            Row_R_SN_PACKING RRSP = TRSP.GetDataBySNID(SN.ID, DB);
            if (RRSP != null)
            {
                throw new Exception($@"{SN.SerialNo} is Packed");
            }
            RRSP = (Row_R_SN_PACKING)TRSP.NewRow();
            RRSP.ID = TRSP.GetNewID(BU, DB);
            RRSP.PACK_ID = DATA.ID;
            RRSP.SN_ID = SN.ID;
            RRSP.EDIT_EMP = user;
            RRSP.EDIT_TIME = DateTime.Now;
            string result = DB.ExecSQL(RRSP.GetInsertString(DB_TYPE_ENUM.Oracle));
            int r = 0;
            bool bInt = Int32.TryParse(result, out r);
            if (!bInt)
            {
                throw new Exception(result);
            }
            if (r == 0)
            {
                throw new Exception($@"Insert R_SN_PACKING Fail!{SN.SerialNo}");
            }
            RRSP.AcceptChange();

            DATA.QTY = GetCount(DB);
            DATA.EDIT_TIME = DateTime.Now;
            DATA.EDIT_EMP = user;
            result = DB.ExecSQL(DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            if (r == 0)
            {
                throw new Exception($@"Update R_PACKING Fail!{SN.SerialNo}");
            }
            Int32.TryParse(result, out r);
            if (!bInt)
            {
                throw new Exception(result);
            }
            DATA.AcceptChange();
        }
        public override string ToString()
        {
            return  $@"{DATA.PACK_NO}({DATA.QTY}/{DATA.MAX_QTY})" ;
        }
    }

    public class PalletBase
    {
        public override string ToString()
        {
            return $@"{DATA.PACK_NO}({DATA.QTY}/{DATA.MAX_QTY})";
        }
        public Row_R_PACKING DATA;
        public PalletBase(Row_R_PACKING data)
        {
            DATA = data;
            if (data.PACK_TYPE != "PALLET")
            {
                throw new Exception("PackType is not PALLET");
            }
        }

        public PalletBase(string PLNO , OleExec DB)
        {
            T_R_PACKING TRP = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            Row_R_PACKING data = TRP.GetRPackingByPackNo(DB, PLNO);
            if (data.PACK_TYPE != "PALLET")
            {
                throw new Exception($@"{PLNO} in not Pallet");
            }
            DATA = data;
        }

        public List<string> GetSNList(OleExec DB)
        {
            //           List<string> ret = new List<string>();
            //           string strSql = $@"select SN
            // from r_sn s
            //where id in
            //      (select sn_id
            //         from R_SN_PACKING
            //        where pack_id in
            //              (select ID
            //                 from R_packing
            //                where parent_pack_id = '{DATA.ID}'))";
            //           DataSet res = DB.RunSelect(strSql);
            //           for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //           {
            //               ret.Add(res.Tables[0].Rows[i]["SN"].ToString());
            //           }

            //           return ret;

            return DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING>((s, p, sp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID)
                .Where((s, p, sp) => p.PARENT_PACK_ID == DATA.ID).Select((s, p, sp) => s.SN).ToList();
        }
 

        public List<string> GetKeyPartList(OleExec DB)
        {
            return DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_KP>((s, p, sp, kp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID && s.ID == kp.R_SN_ID)
                                                .Where((s, p, sp, kp) => p.PARENT_PACK_ID == DATA.ID).Select((s, p, sp, kp) => kp.VALUE).ToList();
        }


        public List<string> GetScanTypeList(string scanType, OleExec DB)
        {
            //var tt = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_KP, R_SN_KP>((s, p, sp, kp, kkp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID && s.ID == kp.R_SN_ID && kp.VALUE == kkp.SN)
            //.Where((s, p, sp, kp, kkp) => p.PARENT_PACK_ID == DATA.ID && kkp.SCANTYPE == scanType).Select((s, p, sp, kp, kkp) => kkp.VALUE).ToList();
            var tt = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_KP, R_SN_KP>((s, p, sp, kp, kkp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID && s.ID == kp.R_SN_ID && kp.VALUE == kkp.SN)
                 .Where((s, p, sp, kp, kkp) => p.PARENT_PACK_ID == DATA.ID && kkp.SCANTYPE == scanType).Select((s, p, sp, kp, kkp) => new { s, p, sp, kp, kkp }).ToList().Select(t => t.kkp.VALUE).ToList();
            return tt;
        }

        public int GetCount(OleExec DB)
        {
            string strSQL = $@"select count(1) from R_PACKING where PARENT_PACK_ID = '{DATA.ID}'";
            return int.Parse( DB.ExecSelectOneValue(strSQL).ToString());
        }

        public int GetSnCount(OleExec DB)
        {
            string strSQL = $@" select count(1) from R_SN_PACKING A,R_PACKING B WHERE A.PACK_ID=B.ID AND B.PARENT_PACK_ID='{DATA.ID}' ";
            return int.Parse(DB.ExecSelectOneValue(strSQL).ToString());
        }

        public void Add(CartionBase Cartion, string BU, string user, OleExec DB)
        {
            if (Cartion.DATA.SKUNO != DATA.SKUNO)
            {
                throw new Exception("Cartion.DATA.SKUNO != Pallet.SKUNO");
            }
            if (GetCount(DB) >= DATA.MAX_QTY)
            {
                throw new Exception($@"{DATA.PACK_NO} is Full");
            }

            if (Cartion.DATA.PARENT_PACK_ID != null && Cartion.DATA.PARENT_PACK_ID != "")
            {
                throw new Exception($@"{Cartion.DATA.PACK_NO} is packed");
            }
            Cartion.DATA.PARENT_PACK_ID = DATA.ID;
            Cartion.DATA.EDIT_EMP = user;
            Cartion.DATA.EDIT_TIME = DateTime.Now;
            string result = DB.ExecSQL(Cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            int r = 0;
            bool bInt = Int32.TryParse(result, out r);
            if (!bInt)
            {
                throw new Exception(result);
            }
            if (r == 0)
            {
                throw new Exception($@"Update R_SN_PACKING Fail!{Cartion.DATA.PACK_NO}");
            }
            Cartion.DATA.AcceptChange();
            DATA.EDIT_EMP = user;
            DATA.EDIT_TIME = DateTime.Now;
            DATA.QTY = GetCount(DB);
            result = DB.ExecSQL(DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            if (!bInt)
            {
                throw new Exception(result);
            }
            if (r == 0)
            {
                throw new Exception($@"Update R_SN_PACKING Fail!{Cartion.DATA.PACK_NO}");
            }
            DATA.AcceptChange();


        }

    }

}
