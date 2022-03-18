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
    public class sfctaskcount
    {
        public sfctaskcount()
        {
			
        }
 
       #region  sfctaskcount实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string postdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string producttype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stationname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiftperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int buildqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int failqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int passqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	   
	   #endregion
    }
}
 
 