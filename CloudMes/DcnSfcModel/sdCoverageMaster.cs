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
    public class sdCoverageMaster
    {
        public sdCoverageMaster()
        {
			
        }
 
       #region  sdCoverageMaster实体
 
	
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
	   public string ReportType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SeqNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CustPartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal LeadTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal CurrQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal SafetyQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ByerCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal IQCQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PerQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal WipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PoQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PoDueDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MrpDesc {get; set;}
	
	
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
 
 