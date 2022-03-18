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
    public class SdShortageMaster
    {
        public SdShortageMaster()
        {
			
        }
 
       #region  SdShortageMaster实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string JobCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrderNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ParentPartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string JobDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkOpenDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkStartDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkDueDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartVersion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal LeadTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal QtyOnHand {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal SafetyQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal RequestQty {get; set;}
	
	
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
	   public string Customer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vendor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal QtyOnOrder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PoDueDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal TotalShort {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Note2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Note3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Ismaterial {get; set;}
	
	   
	   #endregion
    }
}
 
 