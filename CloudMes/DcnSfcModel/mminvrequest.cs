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
    public class mminvrequest
    {
        public mminvrequest()
        {
			
        }
 
       #region  mminvrequest实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string invrequestno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string compcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime requestdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string requestby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfromaddr {get; set;}
	
	
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
	   public DateTime processdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancelled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool internalxfer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptowhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoaddr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool recvprocessed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime recvprocessdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool recvclosed {get; set;}
	
	
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
	   public string shipnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string recvnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lastprocessno {get; set;}
	
	
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
 
 