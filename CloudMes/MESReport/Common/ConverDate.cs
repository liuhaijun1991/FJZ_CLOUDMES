using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Common
{
    public class ConverDate
    {
        /// <summary>
        /// 獲取第幾周的起始日期
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>     
        public static DateTime GetWeekStartDate(int week)
        {
            DateTime FirstDayofYear = DateTime.Parse(DateTime.Now.Year.ToString() + "-01-01 08:00:00");
            week = week - 1;
            int AddDays = week * 7;
            DateTime Temp = FirstDayofYear.Add(new TimeSpan(AddDays, 0, 0, 0));
            if (Temp.DayOfWeek == DayOfWeek.Monday)
            {
                return Temp;
            }
            else if (Temp.DayOfWeek == DayOfWeek.Tuesday)
            {
                return Temp.Add(new TimeSpan(-1, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Wednesday)
            {
                return Temp.Add(new TimeSpan(-2, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Thursday)
            {
                return Temp.Add(new TimeSpan(-3, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Friday)
            {
                return Temp.Add(new TimeSpan(-4, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Saturday)
            {
                return Temp.Add(new TimeSpan(-5, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Sunday)
            {
                return Temp.Add(new TimeSpan(-6, 0, 0, 0));
            }
            throw new Exception();

        }

        /// <summary>
        /// 獲取第幾周的起始日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public static DateTime GetWeekStartDate(string year, int week)
        {
            DateTime FirstDayofYear = DateTime.Parse(year + "-01-01 08:00:00");
            week = week - 1;
            int AddDays = week * 7;
            DateTime Temp = FirstDayofYear.Add(new TimeSpan(AddDays, 0, 0, 0));
            if (Temp.DayOfWeek == DayOfWeek.Monday)
            {
                return Temp;
            }
            else if (Temp.DayOfWeek == DayOfWeek.Tuesday)
            {
                return Temp.Add(new TimeSpan(-1, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Wednesday)
            {
                return Temp.Add(new TimeSpan(-2, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Thursday)
            {
                return Temp.Add(new TimeSpan(-3, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Friday)
            {
                return Temp.Add(new TimeSpan(-4, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Saturday)
            {
                return Temp.Add(new TimeSpan(-5, 0, 0, 0));
            }
            else if (Temp.DayOfWeek == DayOfWeek.Sunday)
            {
                return Temp.Add(new TimeSpan(-6, 0, 0, 0));
            }
            throw new Exception();

        }
         
        /// <summary>
        /// 獲取當前是第幾周，星期天為一周的開始
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekFromDate(DateTime date)
        {
            GregorianCalendar gc = new GregorianCalendar();            
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
        
    }
}
