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
    public class sdshipinvoice
    {
        public sdshipinvoice()
        {
			
        }
 
       #region  sdshipinvoice实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string invoiceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime invoicedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string paymentduedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string invoiceby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string compcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customername {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customermore {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string addr1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string addr2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string city {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string state {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string zip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string country {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string phone1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fax {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contact1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string payterm {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmethod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string priceterm {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currencytype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool invconfirmed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string invconfirmdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancelled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool payclear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal subtotal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal taxrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal taxamount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipcharge {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal adjustment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal paid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal balancedue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool converted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime convertdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bolno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool convertedi {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime edidate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 