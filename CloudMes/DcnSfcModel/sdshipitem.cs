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
    public class sdshipitem
    {
        public sdshipitem()
        {
			
        }
 
       #region  sdshipitem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
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
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string itemtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string changetype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string originalpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal packqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal uweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal uniprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal pricediff {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodcategoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal originalqty {get; set;}
	
	
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
 
 