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
    public class sfcroutemaster
    {
        public sfcroutemaster()
        {
			
        }
 
       #region  sfcroutemaster实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routecategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routedesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mainskuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool defaultroute {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool released {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime releasedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CustomerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RouteProcess {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routenote {get; set;}
	
	
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
	   public bool start {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime start_date {get; set;}
	
	   
	   #endregion
    }
}
 
 