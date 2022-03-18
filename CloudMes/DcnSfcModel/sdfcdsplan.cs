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
    public class sdfcdsplan
    {
        public sdfcdsplan()
        {
			
        }
 
       #region  sdfcdsplan实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string planname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custshiptono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarday {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custweekname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custweekofyear {get; set;}
	
	
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
	   public string fromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string todate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobconverted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool soconverted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal planqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal orderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal jobqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal actualqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note2 {get; set;}
	
	
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
 
 