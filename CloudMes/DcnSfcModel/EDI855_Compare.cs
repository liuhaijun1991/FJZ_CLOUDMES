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
    public class EDI855_Compare
    {
        public EDI855_Compare()
        {
			
        }
 
       #region  EDI855_Compare实体
 
	
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
	   public string TransactionNO {get; set;}
	
	
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
	   public string PONO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReleaseDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TriggerNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupplierCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int TriggedQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Cust_PartNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SIID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int OpenQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime NeedDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CDCReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CommitDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime SupResDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BrcdComment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupComment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UploadToSAP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAP_Result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UploadToSFC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_Result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UploadToSFC_EDI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string File_Name {get; set;}
	
	   
	   #endregion
    }
}
 
 