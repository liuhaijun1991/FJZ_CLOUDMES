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
    public class w_hst_rma_data
    {
        public w_hst_rma_data()
        {
			
        }
 
       #region  w_hst_rma_data实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short week {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string category {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string attachment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddt {get; set;}
	
	   
	   #endregion
    }
}
 
 