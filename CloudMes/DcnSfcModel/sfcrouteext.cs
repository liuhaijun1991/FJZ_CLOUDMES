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
    public class sfcrouteext
    {
        public sfcrouteext()
        {
			
        }
 
       #region  sfcrouteext实体
 
	
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
	   public string eventdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool sfcevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool enabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool eventchart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal targetyieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ssnfilter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string drteventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool eventdrt {get; set;}
	
	
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
 
 