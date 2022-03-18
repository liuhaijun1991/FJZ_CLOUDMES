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
    public class sfcfatpstatus
    {
        public sfcfatpstatus()
        {
			
        }
 
       #region  sfcfatpstatus实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string macaddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int passqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int failqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool bypalletcompleted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failtype {get; set;}
	
	
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
 
 