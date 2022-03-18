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
    public class mfdailylog
    {
        public mfdailylog()
        {
			
        }
 
       #region  mfdailylog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool loginprocess {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool logprocessed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prelogname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkprelog {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short workdays {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short workmaxdays {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool autolognotallow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalqty {get; set;}
	
	
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
 
 