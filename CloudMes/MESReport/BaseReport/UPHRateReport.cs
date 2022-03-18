using MESDBHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class UPHRateReport : ReportBase
    {
        #region 1. 定義查詢條件
        ReportInput skuInput = new ReportInput()
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput lineInput = new ReportInput()
        {
            Name = "LINE",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput eventInput = new ReportInput()
        {
            Name = "EVENTPOINT",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput fromDate = new ReportInput()
        {
            Name = "FROMDATE",
            InputType = "DateTime",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput toDate = new ReportInput()
        {
            Name = "TODATE",
            InputType = "DateTime",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        #endregion

        #region 2. 加載頁面, 初始化時間和下拉框控件
        string sqlGetLine = string.Empty;
        string sqlGetEvent = string.Empty;
        List<string> lineList;
        List<string> eventList;
        public UPHRateReport()
        {
            Inputs.Add(skuInput);
            Inputs.Add(lineInput);
            Inputs.Add(eventInput);
            Inputs.Add(fromDate);
            Inputs.Add(toDate);

            sqlGetLine = $@" select distinct productionline from sfcruntime.sfcuphratedetail_new order by 1 ";
            sqlGetEvent = $@" select distinct eventpoint from sfcruntime.sfcuphratedetail_new order by 1 ";

            Sqls.Add("SqlGetLine", sqlGetLine);
            Sqls.Add("SqlGetEvent", sqlGetEvent);
        }

        public override void Init()
        {
            lineList = GetLine();       //獲取線別
            eventList = GetEvent();     //獲取工站

            //循環將線別Add到線別下拉框中
            List<string> tempList = new List<string>();
            tempList.Add("ALL");
            foreach (string line in lineList)
            {
                tempList.Add(line);
            }
            lineInput.ValueForUse = tempList;

            //循環將工站Add到工站下拉框中
            tempList = new List<string>();
            tempList.Add("ALL");
            foreach (string eventpoint in eventList)
            {
                tempList.Add(eventpoint);
            }
            eventInput.ValueForUse = tempList;
        }

        private List<string> GetLine()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            List<string> lineList = new List<string>();
            List<string> listEvent = new List<string>();

            try
            {
                DataTable dtLine = SFCDB.RunSelect(Sqls["SqlGetLine"]).Tables[0];
                if (SFCDB != null)
                    DBPools["SFCDB"].Return(SFCDB);

                foreach (DataRow row in dtLine.Rows)
                {
                    lineList.Add(row["productionline"].ToString());
                }
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }

            return lineList;
        }

        private List<string> GetEvent()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            List<string> listEvent = new List<string>();

            try
            {
                DataTable dtEvent = SFCDB.RunSelect(Sqls["SqlGetEvent"]).Tables[0];
                if (SFCDB != null)
                    DBPools["SFCDB"].Return(SFCDB);

                foreach (DataRow row in dtEvent.Rows)
                {
                    listEvent.Add(row["eventpoint"].ToString());
                }
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }

            return listEvent;
        }
        #endregion

        #region 3. 實現Run()方法以及需要用到的其他邏輯方法
        public override void Run()
        {
            string skuno = skuInput.Value?.ToString();
            string line = lineInput.Value?.ToString();
            string eventpoint = eventInput.Value?.ToString();
            string from = fromDate.Value?.ToString().Replace("/", "-");
            string to = toDate.Value?.ToString().Replace("/", "-");
            from = from.Substring(0, from.Length - 3);
            to = to.Substring(0, to.Length - 3);

            if (string.IsNullOrEmpty(skuno))
            {
                ReportAlart alart = new ReportAlart("INPUT SKUNO!");
                Outputs.Add(alart);
                return;
            }

            try
            {
                DataTable dt = GetUphRateDetail(skuno, line, eventpoint, from, to);

                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("The input is invalid!");
                    Outputs.Add(alart);
                    return;
                }

                #region 時間點去重 重複的重 去重后的 List 為 timeList
                var tempList = new List<string>();
                var timeList = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tempList.Add(dt.Rows[i]["HOURPERIOD"].ToString());
                }
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (!timeList.Contains(tempList[i]))
                        timeList.Add(tempList[i]);
                }
                #endregion

                #region 構建需要顯示的 DataTable => showDt 把時間點作列名
                DataTable showDt = new DataTable();
                showDt.Columns.Add("Skuno");//機種
                showDt.Columns.Add("Collection point"); //採集點
                showDt.Columns.Add("Area classification");//區分類
                showDt.Columns.Add("UPM*10");

                for (int i = 0; i < timeList.Count; i++)
                {
                    showDt.Columns.Add(timeList[i].ToString());
                }
                #endregion

                #region 構建內容行 : 機種 | 工站 | 區分類 | UPM*10 | 時:分的值
                List<string> qualifyRowList = MakeContentRow("Number of good products", dt, showDt);//良品數   //查詢出的所有工站的良品數行的List集合
                List<string> unQualifyRowList = MakeContentRow("Bad number", dt, showDt);//不良數   //查詢出的所有工站的不良品數行的List集合
                List<string> dataList = new List<string>();     //將良品數行,不良數行,損失率行,良品率行拼在一起的List集合
                for (int i = 0; i < qualifyRowList.Count; i++)
                {
                    dataList.Add(qualifyRowList[i]);
                    dataList.Add(unQualifyRowList[i]);

                    string[] qualifyRowArray = qualifyRowList[i].Split(',');     //良品數行的數組
                    string[] unQualifyRowArray = unQualifyRowList[i].Split(','); //不良數行的數組

                    string qualifyRateRow = string.Empty;       //良品率行
                    string unQualifyRateRow = string.Empty;     //損失率行

                    //分別拼接良品率行與損失率行字符串
                    qualifyRateRow += qualifyRowList[i].Split(',')[0] + "," + qualifyRowList[i].Split(',')[1] + ",Yield rate," + qualifyRowList[i].Split(',')[3];
                    unQualifyRateRow += qualifyRowList[i].Split(',')[0] + "," + qualifyRowList[i].Split(',')[1] + ",Loss rate," + qualifyRowList[i].Split(',')[3];

                    for (int j = 4; j < qualifyRowList[i].Split(',').Length; j++)
                    {
                        double upm10 = Convert.ToDouble(qualifyRowList[i].Split(',')[3]);
                        int total = Convert.ToInt32(qualifyRowList[i].Split(',')[j]) + Convert.ToInt32(unQualifyRowList[i].Split(',')[j]);
                        int good = Convert.ToInt32(qualifyRowList[i].Split(',')[j]);
                        string qualifyRate = (total == 0 ? 0 : (double)good / total).ToString("0.00%");
                        string unQualifyRate = (upm10 == 0 ? 0 : (double)(upm10 - total) / upm10).ToString("0.00%");

                        qualifyRateRow += "," + qualifyRate;
                        unQualifyRateRow += "," + unQualifyRate;
                    }
                    dataList.Add(unQualifyRateRow);     //先將損失率行Add進List
                    dataList.Add(qualifyRateRow);       //再講良品率行Add進List
                }
                #endregion

                #region 根據機種路由工站順序重新排列dataList集合使之呈現最終報表的樣子 => showList
                //有些不是路由工站, 所以屏蔽這段 Edit By ZHB 2018年6月13日11:31:33
                //DataTable eventDt = GetEventSeqNo(skuno);
                //List<string> showList = new List<string>();
                //for (int i = 0; i < eventDt.Rows.Count; i++)
                //{
                //    for (int j = 0; j < dataList.Count; j++)
                //    {
                //        if (eventDt.Rows[i]["station_name"].ToString() == dataList[j].Split(',')[1])
                //            showList.Add(dataList[j]);
                //    }
                //}
                #endregion

                #region 循環 showList 填充 showDt 再顯示出來
                for (int i = 0; i < dataList.Count; i++)
                {
                    DataRow dtRow = showDt.NewRow();
                    for (int j = 0; j < showDt.Columns.Count; j++)
                    {
                        dtRow[j] = dataList[i].Split(',')[j];
                    }
                    showDt.Rows.Add(dtRow);
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(showDt, null);
                retTab.Tittle = "ZERO DEFECT UPH RATE REPORT";
                Outputs.Add(retTab);
                #endregion

            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }

        /// <summary>
        /// 獲取UphRateDetail_New表數據
        /// </summary>
        private DataTable GetUphRateDetail(string skuno, string line, string eventpoint, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            string sql = string.Empty;
            string tempSql = string.Empty;

            #region sql 前半部份
            sql = $@"select uphdate,
                            hourperiod,
                            eventpoint,
                            skuno,
                            uph_qty,
                            trunc(uph_qty / 6, 2) as UPM10,
                            sum(inputunit) as sum_input,
                            sum(outputunit) as sum_output
                       from (select a.uphdate,
                                    substr(a.hourperiod,0,2) || ':' || substr(a.min_period, 0, 2) as hourperiod,
                                    a.min_period,
                                    a.productionline,
                                    a.eventpoint,
                                    a.skuno,
                                    a.uph_qty,
                                    a.inputunit,
                                    a.outputunit
                               from sfcruntime.sfcuphratedetail_new a
                              where 1 = 1 ";
            #endregion

            #region sql 拼接部份
            if (!line.ToUpper().Equals("ALL"))
                tempSql += $@"and a.productionline = '{line}' ";
            if (!eventpoint.ToUpper().Equals("ALL"))
                tempSql += $@"and a.eventpoint = '{eventpoint}' ";
            if (skuno.Length > 0)
                tempSql += $@"and a.skuno = '{skuno}' ";

            tempSql += $@"and to_date(a.uphdate || substr(a.hourperiod, 1, 3) || substr(a.min_period, 1, 2), 'yyyy-mm-dd hh24:mi') 
                          between to_date('{fromDate}', 'yyyy-mm-dd hh24:mi') and to_date('{toDate}', 'yyyy-mm-dd hh24:mi') ";
            #endregion

            // sql 後半部份
            sql = sql + tempSql + $@" ) group by uphdate, hourperiod, eventpoint, skuno, uph_qty order by uphdate, hourperiod ";

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                dt = SFCDB.RunSelect(sql).Tables[0];
                if (SFCDB != null)
                    DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 按區分類將數據轉換成和報表行格式一樣的字符串
        /// </summary>
        private List<string> MakeContentRow(string conType, DataTable dt, DataTable showDt)
        {
            var qualifyStrList = new List<string>();    //初始化一個List<string> string數組List

            //循環數據源的行數
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool dtFlag = false;     //初始值為False
                bool strFlag = false;    //初始值為False
                string qualifyQtyString = string.Empty;
                int input = Convert.ToInt32(dt.Rows[i]["SUM_INPUT"].ToString());    //總投入數
                int output = Convert.ToInt32(dt.Rows[i]["SUM_OUTPUT"].ToString());  //良品數

                //再循環標題行的長度,即列數
                for (int j = 4; j < showDt.Columns.Count; j++)
                {
                    if (dtFlag)     //如果dtFlag為True,跳出,循環數據源的下一行
                        break;
                    if (!strFlag)   //首次循環,字符串=機種,工站,區分類,UPM*10的值
                        qualifyQtyString += dt.Rows[i]["SKUNO"].ToString() + "," + dt.Rows[i]["EVENTPOINT"].ToString() + "," + conType + "," + dt.Rows[i]["UPM10"].ToString();

                    //再循環string數組List的個數
                    for (int k = 0; k < qualifyStrList.Count; k++)
                    {
                        //如果相同的工站相同的區分類相同的UPM*10 已經Add到List中
                        if (qualifyStrList[k].Contains(qualifyQtyString))
                        {
                            var index = 0;

                            //這個循環主要是得到相同時間點的下標
                            for (int m = 0; m < showDt.Columns.Count; m++)
                            {
                                var hour = dt.Rows[i]["HOURPERIOD"].ToString().Substring(0, dt.Rows[i]["HOURPERIOD"].ToString().IndexOf(':'));
                                var minute = dt.Rows[i]["HOURPERIOD"].ToString().Substring(dt.Rows[i]["HOURPERIOD"].ToString().IndexOf(':') + 1);
                                if (minute == "60")
                                {
                                    hour = (Convert.ToInt32(hour) + 1).ToString();
                                    minute = "00";
                                }
                                if (showDt.Columns[m].ToString() == hour + ":" + minute)
                                {
                                    index = m;
                                    break;
                                }
                            }

                            //如果下標大於0,就可以把原先字符串中這個時間點的值截取出來,再替換掉
                            if (index > 0)
                            {
                                #region 複製這個時間點這個index之前的字符串
                                var tem = string.Empty;
                                for (int m = 0; m < index; m++)
                                {
                                    tem += "," + qualifyStrList[k].Split(',')[m];
                                }
                                #endregion

                                #region 替換這個時間點這個index位置的字符串
                                if (conType == "Number of good products")//良品數
                                    tem += "," + output.ToString();
                                if (conType == "Bad number")//不良數
                                    tem += "," + (input - output).ToString();
                                #endregion

                                #region 接上這個時間點這個index之後的字符串
                                for (int m = index + 1; m < qualifyStrList[k].Split(',').Length; m++)
                                {
                                    tem += "," + qualifyStrList[k].Split(',')[m];
                                }
                                #endregion

                                qualifyStrList.RemoveAt(k);     //把之前Add 到List中的字符串刪掉
                                qualifyStrList.Insert(k, tem.Substring(1));     //把重新拼接的字符串Insert到剛剛刪除字符串的那個位置
                                dtFlag = true;
                                break;      //這時就不繼續循環當前行了,直接跳出,循環List中下一個字符串
                            }
                        }
                    }

                    //若是循環到列標題與當前行的時間一樣的,就按照區分類把該時間對應的值拼接到字符串中,否則拼接0
                    if (showDt.Columns[j].ToString() == dt.Rows[i]["HOURPERIOD"].ToString())
                    {
                        if (conType == "Number of good products")
                            qualifyQtyString += "," + output.ToString();
                        if (conType == "Bad number")
                            qualifyQtyString += "," + (input - output).ToString();
                    }
                    else
                        qualifyQtyString += ",0";

                    strFlag = true;
                }

                if (!dtFlag)    //如果dtFlag為False,把拼接好的字符串Add 到List中
                    qualifyStrList.Add(qualifyQtyString);
            }

            return qualifyStrList;  //返回List
        }

        /// <summary>
        /// 獲取機種路由工站的順序,返回一個DataTable
        /// </summary>
        private DataTable GetEventSeqNo(string skuno)
        {
            DataTable dt = new DataTable();
            string sql = $@"select distinct c.station_name, c.seq_no from c_sku a, r_sku_route b, c_route_detail c
                            where a.id = b.sku_id and b.route_id = c.route_id and a.skuno = '{skuno}' order by seq_no ";

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                dt = SFCDB.RunSelect(sql).Tables[0];
                if (SFCDB != null)
                    DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
            return dt;
        }
        #endregion
    }
}
