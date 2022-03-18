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
    public class Oracle_Invoice_Detail
    {
        public Oracle_Invoice_Detail()
        {
			
        }
 
       #region  Oracle_Invoice_Detail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InvoiceNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InvoiceLineNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InvoiceLineQuantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitMeasurementCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitPrice {get; set;}
	
	
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
	   public string PartNumberDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TotalTaxAmount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TaxPercentage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PONumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string POLineNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PackingSlipNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ScheduleNumber {get; set;}
	
	   
	   #endregion
    }
}
 
 