using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.Json
{
    public class JsonSave
    {
        /// <summary>
        /// 将一个对象序列化到数据库,返回参考ID
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <param name="DB"></param>
        /// <param name="BU"></param>
        /// <returns></returns>
        public static string SaveToDB(object obj, string name, string type, string user, OleExec DB, string BU, bool isoverride = false)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);

            R_JSON R = new R_JSON();
            var RO = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type).First();
            if (RO != null)
            {
                if (!isoverride)
                {
                    throw new Exception($@"{name} , {type} 已经存在");
                }
                else
                {
                    RO.BLOBDATA = byteArray;
                    RO.EDIT_EMP = user;
                    RO.EDIT_TIME = DateTime.Now;
                    DB.ORM.Updateable<R_JSON>(RO).ExecuteCommand();
                    return RO.ID;
                }
            }
            R.ID = MesDbBase.GetNewID(DB.ORM, BU, "R_JSON");
            R.NAME = name;
            R.TYPE = type;
            R.EDIT_EMP = user;
            R.EDIT_TIME = DateTime.Now;
            R.BLOBDATA = byteArray;

            DB.ORM.Insertable<R_JSON>(R).ExecuteCommand();

            return R.ID;
        }
        /// <summary>
        /// 将一个对象序列化到数据库,返回参考ID
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <param name="DB"></param>
        /// <param name="BU"></param>
        /// <returns></returns>
        public static string SaveToDB(object obj, string name, string type, string user, SqlSugar.SqlSugarClient DB, string BU, bool isoverride = false)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);

            R_JSON R = new R_JSON();
            var RO = DB.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type).First();
            if (RO != null)
            {
                if (!isoverride)
                {
                    throw new Exception($@"{name} , {type} 已经存在");
                }
                else
                {
                    RO.BLOBDATA = byteArray;
                    RO.EDIT_EMP = user;
                    RO.EDIT_TIME = DateTime.Now;
                    DB.Updateable<R_JSON>(RO).ExecuteCommand();
                    return RO.ID;
                }
            }
            R.ID = MesDbBase.GetNewID(DB, BU, "R_JSON");
            R.NAME = name;
            R.TYPE = type;
            R.EDIT_EMP = user;
            R.EDIT_TIME = DateTime.Now;
            R.BLOBDATA = byteArray;

            DB.Insertable<R_JSON>(R).ExecuteCommand();

            return R.ID;
        }

        public static string SaveToDB(object obj, string name, string type, string user, OleExec DB, string BU, string Index1, string Index2, string Index3, string Index4)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);

            R_JSON R = new R_JSON();
            var count = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type).Count();
            if (count > 0)
            {
                throw new Exception($@"{name} , {type} 已经存在");
            }
            R.ID = MesDbBase.GetNewID(DB.ORM, BU, "R_JSON");
            R.NAME = name;
            R.TYPE = type;
            R.EDIT_EMP = user;
            R.EDIT_TIME = DateTime.Now;
            R.BLOBDATA = byteArray;

            R.INDEX1 = Index1;
            R.INDEX2 = Index2;
            R.INDEX3 = Index3;
            R.INDEX4 = Index4;

            DB.ORM.Insertable<R_JSON>(R).ExecuteCommand();

            return R.ID;
        }

        public static string SaveToDB(object obj, string name, string type, string user, OleExec DB, string BU, string Index1)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);

            R_JSON R = new R_JSON();
            var count = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type && t.INDEX1 == Index1).Count();
            if (count > 0)
            {
                throw new Exception($@"{name} , {type} 已经存在");
            }
            R.ID = MesDbBase.GetNewID(DB.ORM, BU, "R_JSON");
            R.NAME = name;
            R.TYPE = type;
            R.EDIT_EMP = user;
            R.EDIT_TIME = DateTime.Now;
            R.BLOBDATA = byteArray;

            R.INDEX1 = Index1;

            DB.ORM.Insertable<R_JSON>(R).ExecuteCommand();

            return R.ID;
        }


        public static string UpdateToDB(object obj, string name, string type, string user, OleExec DB, string BU)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);
            var list
                = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON mane={name} and type= {type} 不存在");
            }

            list[0].EDIT_EMP = user;
            list[0].EDIT_TIME = DateTime.Now;
            list[0].BLOBDATA = byteArray;

            DB.ORM.Updateable<R_JSON>(list[0]).Where(t => t.ID == list[0].ID).ExecuteCommand();
            return list[0].ID;
        }

        public static string UpdateToDB(object obj, string name, string type, string SN, string user, OleExec DB, string BU)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);
            var list
                = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type && t.INDEX1 == SN).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON mane={name} and type= {type} 不存在");
            }

            list[0].EDIT_EMP = user;
            list[0].EDIT_TIME = DateTime.Now;
            list[0].BLOBDATA = byteArray;
            list[0].INDEX1 = SN;

            DB.ORM.Updateable<R_JSON>(list[0]).Where(t => t.ID == list[0].ID).ExecuteCommand();
            return list[0].ID;
        }

        public static string UpdateToDB(object obj, string ID, string user, OleExec DB, string BU)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(json);
            var list
                = DB.ORM.Queryable<R_JSON>().Where(t => t.ID == ID).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON ID={ID} 不存在");
            }

            list[0].EDIT_EMP = user;
            list[0].EDIT_TIME = DateTime.Now;
            list[0].BLOBDATA = byteArray;

            DB.ORM.Updateable<R_JSON>(list[0]).Where(t => t.ID == list[0].ID).ExecuteCommand();
            return list[0].ID;
        }

        public static string GetFromDB(string ID, OleExec DB)
        {
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.ID == ID).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON ID={ID} 没有数据");
            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);
            return str;
        }

        public static string GetFromDB(string ID, OleExec DB, ref DateTime? EditTime, ref string EMP)
        {
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.ID == ID).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON ID={ID} 没有数据");
            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);
            EditTime = list[0].EDIT_TIME;
            EMP = list[0].EDIT_EMP;
            return str;
        }

        public static string GetFromDB(string Index1, string Index2, string Index3, string Index4, OleExec DB, ref DateTime? EditTime, ref string EMP)
        {
            var Qlist = DB.ORM.Queryable<R_JSON>();
            if (Index1 != null)
            {
                Qlist = Qlist.Where(t => t.INDEX1 == Index1);
            }
            if (Index2 != null)
            {
                Qlist = Qlist.Where(t => t.INDEX2 == Index2);
            }
            if (Index3 != null)
            {
                Qlist = Qlist.Where(t => t.INDEX3 == Index3);
            }
            if (Index4 != null)
            {
                Qlist = Qlist.Where(t => t.INDEX4 == Index4);
            }

            var list = Qlist.ToList();

            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON Find 没有数据");
            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);
            EditTime = list[0].EDIT_TIME;
            EMP = list[0].EDIT_EMP;
            return str;
        }

        public static T GetFromDB<T>(string ID, OleExec DB, ref DateTime? EditTime, ref string EMP)
        {
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.ID == ID).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON ID={ID} 没有数据");
            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);
            EditTime = list[0].EDIT_TIME;
            EMP = list[0].EDIT_EMP;
            //Newtonsoft.Json.Linq.JObject Jobj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(str);
            var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            return ret;

        }
        public static T GetFromDB<T>(string name, string type, OleExec DB)
        {
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == name && t.TYPE == type).ToList();
            if (list.Count == 0)
            {
                //throw new Exception($@"NAME={name},TYPE ={type} 没有数据");
                return default(T);
            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);

            var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            return ret;
        }

        
        public static T GetFromDB<T>(string ID, OleExec DB)
        {

            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.ID == ID).ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"R_JSON ID={ID} 没有数据");

            }
            string str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);

            var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            return ret;
        }

        public static string GetJson(object obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            return json;
        }

        public static T GetFromDB<T>(string NAME, string TYPE, string index1, OleExec DB)
        {
            var list = DB.ORM.Queryable<R_JSON>().Where(t => t.NAME == NAME && t.TYPE == TYPE && t.INDEX1 == index1).ToList();
            string str = "";
            if (list.Count != 0)
            {
                str = System.Text.Encoding.Unicode.GetString(list[0].BLOBDATA);
            }

            //EditTime = list[0].EDIT_TIME;
            //EMP = list[0].EDIT_EMP;
            //Newtonsoft.Json.Linq.JObject Jobj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(str);
            var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            return ret;

        }

        public static void DeleteFromDB(string NAME, string index1, OleExec DB)
        {
            string sql = " delete R_json where name='" + NAME + "' and index1='" + index1 + "' ";
            DB.ExecSQL(sql);
        }
    }
}
