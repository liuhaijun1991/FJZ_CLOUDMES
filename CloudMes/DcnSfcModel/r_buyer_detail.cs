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
    public class r_buyer_detail
    {
        public r_buyer_detail()
        {
			
        }
 
       #region  r_buyer_detail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int weight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime date {get; set;}
	
	   
	   #endregion
    }
}
 
 