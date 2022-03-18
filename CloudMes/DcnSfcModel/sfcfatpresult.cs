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
    public class sfcfatpresult
    {
        public sfcfatpresult()
        {
			
        }
 
       #region  sfcfatpresult实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime auditdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalaudit {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalauditfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal passrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal fatpqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string audityear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditweek {get; set;}
	
	
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
 
 