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
    public class pbcatfmt
    {
        public pbcatfmt()
        {
			
        }
 
       #region  pbcatfmt实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pbf_name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pbf_frmt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short pbf_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int pbf_cntr {get; set;}
	
	   
	   #endregion
    }
}
 
 