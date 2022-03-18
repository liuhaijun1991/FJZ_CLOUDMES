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
    public class R_BRCM_SHIP_DATA
    {
        public R_BRCM_SHIP_DATA()
        {
			
        }
 
       #region  R_BRCM_SHIP_DATA实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Record_Type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CM_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Record_Creation_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Transaction_Type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shipment_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Item {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UOM {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Department_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Completion_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shipment_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shipment_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Delivery_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Delivery_Note_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Ship_To_Address_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Ship_Method {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Order_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Line_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Line_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Shipment_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Release_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Quantity_Completed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Quantity_Shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Waybill_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Packing_Slip_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Bill_Of_Lading {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lot_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Lot_Quantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Manufacture_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CAT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BIN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Custom_PN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vendor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Test_Program_Revision {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Number_of_WIPC_records {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Reserved_Columns {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToSite {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToDept {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vendor_Part_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vendor_Lot_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NC_Reason_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BATCH_NOTE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SUBSTRATE_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Seal_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Good_die_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Die_Part_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Date_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LPN_Lot_attributes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Outer_LPN_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Outer_LPN_Flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Wafer_Batch_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Country_of_Diffusion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CreateTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	   
	   #endregion
    }
}
 
 