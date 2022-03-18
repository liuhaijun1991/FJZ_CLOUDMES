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
    public class EDI856_FromQhub
    {
        public EDI856_FromQhub()
        {
			
        }
 
       #region  EDI856_FromQhub实体
 
	
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
	   public string DeliveryNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime DeliveryDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LineNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string suppliercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BrcdSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDIDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ICNNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ST02 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string uploadtosfc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MDNNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime MDNDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MDNMsg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string File_Name {get; set;}
	
	   
	   #endregion
    }
}
 
 