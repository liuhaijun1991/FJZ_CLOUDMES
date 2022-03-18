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
    public class sfcbrcdspccorrective
    {
        public sfcbrcdspccorrective()
        {
			
        }
 
       #region  sfcbrcdspccorrective实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currenteventscandt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currenteventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string problem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int timedelayed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool delayed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string corrective {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string delaycase {get; set;}
	
	
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
	   public string correctiveby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime correctivedt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string approvedby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime approveddt {get; set;}
	
	   
	   #endregion
    }
}
 
 