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
    public class Sfc_RmaBonepile_Report
    {
        public Sfc_RmaBonepile_Report()
        {
			
        }
 
       #region  Sfc_RmaBonepile_Report实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReceivedDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LotNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RmaReqDate {get; set;}
	
	
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
	   public string PcbaSn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PcbaPn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PcbaLoadDate {get; set;}
	
	
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
	   public string DF_FailStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DF_Site {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureSymptom {get; set;}
	
	
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
	   public string Valuable {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Remark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rma_VI_Result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VI_Failure_Symptom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VI_Failure_Location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime VI_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VI_Inspector_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RE_PN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DebugDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RepairAction {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RepairLocation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Fail_Comp_DC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RepairDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DebugID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Rma {get; set;}
	
	
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
	   public string UploadBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UploadDate {get; set;}
	
	
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
	   public int RmaTimes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FunctionRmaTimes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CosmeticRmaTimes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ComponentID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAOWNER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAResult {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FAEDITDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ANAEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ANAEDITDT {get; set;}
	
	   
	   #endregion
    }
}
 
 