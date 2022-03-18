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
    public class Oracle_ASN_Head
    {
        public Oracle_ASN_Head()
        {
			
        }
 
       #region  Oracle_ASN_Head实体
 
	
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
	   public DateTime MappingDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ASNNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ToNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShippedDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShippedTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShippedTimeZone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DeliveryDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DeliveryTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DeliveryTimeZone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal NetWeightValue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal TareWeightValue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitMeasurementCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LadingQuantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PackingSlipNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierAddressName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShiptoAddressName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShiptoAddressID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SendFlag {get; set;}
	
	   
	   #endregion
    }
}
 
 