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
    public class sdshiporder2
    {
        public sdshiporder2()
        {
			
        }
 
       #region  sdshiporder2实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string saleslistno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string invoiceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool LinesFinish {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool dropship {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custotherrefno {get; set;}
	
	
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
	   public bool processed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ooinv {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool packed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool invoiced {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipped {get; set;}
	
	
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
	   public int boxnbr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool finishedc856b {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime finishedc856bdate {get; set;}
	
	
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
	   public string totalpackages {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EINno {get; set;}
	
	   
	   #endregion
    }
}
 
 