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
    public class sdbroorderchglog
    {
        public sdbroorderchglog()
        {
			
        }
 
       #region  sdbroorderchglog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string chgfield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fielddesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string originalvalue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentvalue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime changedate {get; set;}
	
	
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
	   public string refno3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool realeased {get; set;}
	
	
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
 
 