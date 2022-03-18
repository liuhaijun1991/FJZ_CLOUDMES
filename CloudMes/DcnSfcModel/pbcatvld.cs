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
    public class pbcatvld
    {
        public pbcatvld()
        {
			
        }
 
       #region  pbcatvld实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pbv_name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pbv_vald {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short pbv_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int pbv_cntr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pbv_msg {get; set;}
	
	   
	   #endregion
    }
}
 
 