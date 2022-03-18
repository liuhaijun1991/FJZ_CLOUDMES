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
    public class sdordermaster
    {
        public sdordermaster()
        {
			
        }
 
       #region  sdordermaster实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string compcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string documenttype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime orderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custdnno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custotherrefno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string o_customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string o_customername {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string o_customermore {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string o_payterm {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcompanyname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcompanymore {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipattn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipaddr1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipaddr2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmore {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcounty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipstate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipzip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcountry {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipdayphone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipemail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmethod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool dropship {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string planname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool planorder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobcreated {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shippartial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipcomplete {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool holdship {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancelled {get; set;}
	
	
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
	   public decimal adjustamt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currencytype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ordersource {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string soldby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string gennote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string jobnote {get; set;}
	
	
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
	   public bool quote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool confirmed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime confirmdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ordertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prioritycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool dcmprocessed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime dcmdatetime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool finishedc846 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime finishedc846date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ndccode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ndccode2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ndccode3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool convertedkn14 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime convertedkn14date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime validfromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime validtodate {get; set;}
	
	   
	   #endregion
    }
}
 
 