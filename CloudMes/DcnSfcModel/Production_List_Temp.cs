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
    public class Production_List_Temp
    {
        public Production_List_Temp()
        {
			
        }
 
       #region  Production_List_Temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DF_Site {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string S_N {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DF_Test_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime DF_Input_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Fail_Station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Failure_Symptom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Root_Cause {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Analysis {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string upload_name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Screen_Dump {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pr_an {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DF_Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments_DF {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments_DF_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments_LH {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments_LH_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Ncmr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ncmr_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RMA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rma_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QE_Approve {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PM_Approve {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Ncmr_Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Defection {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rework_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string root_cause_lh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Categoery {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FA_result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Correct_action {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Attachment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string temp_states {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LH_Failure_Symptom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Foxconn_states {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Tracking {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Tracking_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ReTest_Result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string where_to {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HK_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LH_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string owner {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string owner_lh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mail_status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Week_input {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Damage_Type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Visual_result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string visual_Attachment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rejected_reason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int TEST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Packing {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packing_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packed_ship {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string complete_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string send_mail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string chasis_sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rohs_status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string defect_location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string qe_approve_time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Pallet_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Pallet_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Remark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pm_approve_time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string df_approve_comment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime df_approve_time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastScanDt {get; set;}
	
	   
	   #endregion
    }
}
 
 