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
    public class sdordersku
    {
        public sdordersku()
        {
			
        }
 
       #region  sdordersku实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string polineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BillToCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skudesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool originalsku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal orderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal jobqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitshipcharge {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal subtotal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string suborderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime etddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime etadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal dcmqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal uweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool converted846 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime converted846date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string plantcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string storagelocation {get; set;}
	
	
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
 
 