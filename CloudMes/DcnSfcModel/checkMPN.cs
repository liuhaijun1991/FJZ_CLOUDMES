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
    public class checkMPN
    {
        public checkMPN()
        {
			
        }
 
       #region  checkMPN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PCBA_SKU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CUST_PART_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 