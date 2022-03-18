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
    public class mfmpscapacity
    {
        public mfmpscapacity()
        {
			
        }
 
       #region  mfmpscapacity实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendardate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string effectdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dayofweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool dayoff {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool workondayoff {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short nbrofshift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short nbrofprodline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short totalworkingline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal perworklineqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal daytotalqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string defaultpartno {get; set;}
	
	
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
 
 