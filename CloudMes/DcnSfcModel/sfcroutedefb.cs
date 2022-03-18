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
    public class sfcroutedefb
    {
        public sfcroutedefb()
        {
			
        }
 
       #region  sfcroutedefb实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short eventseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventFeature {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime addindate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string addinby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool bypassroute {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool collectcount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nexteventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prioreventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool scriptupload {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool failcodeupload {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FromEventPoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool realfailcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool nostatuscheck {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rreventpoint {get; set;}
	
	
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
	   public string NotCheckFinish {get; set;}
	
	   
	   #endregion
    }
}
 
 