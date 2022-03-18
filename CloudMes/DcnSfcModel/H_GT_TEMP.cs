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
    public class H_GT_TEMP
    {
        public H_GT_TEMP()
        {
			
        }
 
       #region  H_GT_TEMP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string From_Storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string To_Storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rec_Type {get; set;}
	
	   
	   #endregion
    }
}
 
 