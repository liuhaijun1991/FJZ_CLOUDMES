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
    public class Shipment_toCzech_shipment
    {
        public Shipment_toCzech_shipment()
        {
			
        }
 
       #region  Shipment_toCzech_shipment实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cust {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string To_number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dn_number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string itemno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Deliverdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string palletNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string serialNo {get; set;}
	
	   
	   #endregion
    }
}
 
 