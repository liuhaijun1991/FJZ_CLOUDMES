using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 
namespace DcnSfcModel
{
    /// <summary>
    /// 功能: 实体类 ()
    /// 创建人：Eden     
    /// 创建日期：2019/12/31    
    /// </summary>
    [Serializable]
    public class adCalendar
    {
        public adCalendar()
        {
			
        }
 
       #region  adCalendar实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendardate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dayofweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendaryear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string monthname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string weekofyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string weekofmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string weekname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool isholiday {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string holidayname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 