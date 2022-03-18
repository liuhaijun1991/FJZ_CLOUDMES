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
    public class sdshipsku
    {
        public sdshipsku()
        {
			
        }
 
       #region  sdshipsku实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderlineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
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
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool originalsku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool SendASN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime SendASNDate {get; set;}
	
	
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
	   public string BrocadePO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string polineno {get; set;}
	
	
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
	   public decimal uweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FromInvCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ToInvCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoloc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmentno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipmentdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipmentfinish {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool lineFinish {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string waybillno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int doccontrol {get; set;}
	
	
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
	   public bool Shippable {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Parentline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal TotalPackage {get; set;}
	
	   
	   #endregion
    }
}
 
 