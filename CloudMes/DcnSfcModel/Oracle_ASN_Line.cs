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
    public class Oracle_ASN_Line
    {
        public Oracle_ASN_Line()
        {
			
        }
 
       #region  Oracle_ASN_Line实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ToNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoLineNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ScheduleNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BuyerPartNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VendorPartNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CountryCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ShippedQuantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNumberDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PackingSlipNumber {get; set;}
	
	   
	   #endregion
    }
}
 
 