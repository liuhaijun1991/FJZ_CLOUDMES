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
    public class scmvendordsp
    {
        public scmvendordsp()
        {
			
        }
 
       #region  scmvendordsp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime dspdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptowhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool notified {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool commited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime commitdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string commitby {get; set;}
	
	
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
	   public bool cancelled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmreceiveno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool received {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime recvdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string receiveby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bolno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packlistno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string refno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string refno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string recvnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
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
 
 