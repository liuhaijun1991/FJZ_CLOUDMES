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
    public class EDI861Summary
    {
        public EDI861Summary()
        {
			
        }
 
       #region  EDI861Summary实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Cnt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Site {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TriggerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReceivedDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipTo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Supplier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Customer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PalletID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VendorPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BuyerPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ReceivedQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ICNNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 