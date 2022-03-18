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
    public class Oracle_PO_Head
    {
        public Oracle_PO_Head()
        {
			
        }
 
       #region  Oracle_PO_Head实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MessageID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CreateDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoIssueDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoIssueTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoIssueTimeZone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FOBShipMethod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FOBDestCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FOBIncotermsDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FOBOriginCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FOBFreightTermsDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CurrencyCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TermsNetDays {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PaymentTermsDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CarrierName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToAddressID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToAddressID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierAddressID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToAddressID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToStreet {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToStreet {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierStreet {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToStreet {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToCity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToCity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierCity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToCity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BilltoState {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldtoState {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierState {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToState {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToZipCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToZipCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierZipCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToZipCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToCountryCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToCountryCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierCountryCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToCountryCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToLocation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierSiteName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToContactName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToContactName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierContactName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldToTelephone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToTelephone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierTelephone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SoldtoEmail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShiptoEmail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierEmail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoHeaderNotes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SendFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime GetTime {get; set;}
	
	   
	   #endregion
    }
}
 
 