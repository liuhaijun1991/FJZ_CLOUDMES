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
    public class Sfc_CriticalBonepile_Report
    {
        public Sfc_CriticalBonepile_Report()
        {
			
        }
 
       #region  Sfc_CriticalBonepile_Report实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime BRC_StartDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FLH_StartDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FirstLoadDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CtoSnold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VanillaSnold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PcbaSnold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CtoSnNew {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VanillaSnNew {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PcbaSnNew {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrderNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SubSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureSymptom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BonepileDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BonepileStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BonepileCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Owner {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BRCRemark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FLHRemark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool RMA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool CriticalBonepile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HardcoreBoard {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Price {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CurrentStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool CurrentStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Scrapped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ClosedReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ClosedBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ClosedDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Uploadby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Uploaddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UploadLHby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UploadLHdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 