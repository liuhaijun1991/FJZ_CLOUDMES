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
    public class sfcsyscount
    {
        public sfcsyscount()
        {
			
        }
 
       #region  sfcsyscount实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string postdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
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
	   public bool processcounted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string processperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool passcounted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string passperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool failcounted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failperiod {get; set;}
	
	
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
	   public string stationname {get; set;}
	
	   
	   #endregion
    }
}
 
 