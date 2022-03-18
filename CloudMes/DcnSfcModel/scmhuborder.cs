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
    public class scmhuborder
    {
        public scmhuborder()
        {
			
        }
 
       #region  scmhuborder实体
 
	
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
	   public DateTime orderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipfrom {get; set;}
	
	
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
	   public string etddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etdtime {get; set;}
	
	
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
	   public string processby {get; set;}
	
	
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
	   public string podnno {get; set;}
	
	
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
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
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
	   public string shipto {get; set;}
	
	   
	   #endregion
    }
}
 
 