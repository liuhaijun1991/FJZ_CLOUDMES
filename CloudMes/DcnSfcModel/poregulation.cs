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
    public class poregulation
    {
        public poregulation()
        {
			
        }
 
       #region  poregulation实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string priority {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string subject {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string detail1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string detail2 {get; set;}
	
	
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
 
 