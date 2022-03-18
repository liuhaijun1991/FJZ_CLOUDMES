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
    public class poprereceive
    {
        public poprereceive()
        {
			
        }
 
       #region  poprereceive实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string podnno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmethod {get; set;}
	
	
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
	   public string shipfrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipto {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool approved {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string approverorder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool called {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime calldate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool acked {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ackeddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool received {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime receivedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime closeddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal freight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal subtotal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 