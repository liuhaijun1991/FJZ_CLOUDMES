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
    public class sdCoverageItem
    {
        public sdCoverageItem()
        {
			
        }
 
       #region  sdCoverageItem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MRP_version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DB {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LeadTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ReqDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal FinishQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal OnHandQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal WipQty {get; set;}
	
	
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
 
 