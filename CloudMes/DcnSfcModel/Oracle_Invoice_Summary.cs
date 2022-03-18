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
    public class Oracle_Invoice_Summary
    {
        public Oracle_Invoice_Summary()
        {
			
        }
 
       #region  Oracle_Invoice_Summary实体
 
	
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
	   public string AmountBeforTermsDiscount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AmountAfterTermsDiscount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AmountOfInvoiceDue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AmountOfTermsDiscount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AmountOfFreightCharge {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NumberOfLineItems {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TotalWeight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitMeasurementCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FreeValume {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FreeValumeMeasurementCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FreeValumeDescription {get; set;}
	
	   
	   #endregion
    }
}
 
 