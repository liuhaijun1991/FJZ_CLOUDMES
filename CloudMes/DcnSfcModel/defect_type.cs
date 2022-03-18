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
    public class defect_type
    {
        public defect_type()
        {
			
        }
 
       #region  defect_type实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type_id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parent_id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type_name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte level_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte has_next {get; set;}
	
	   
	   #endregion
    }
}
 
 