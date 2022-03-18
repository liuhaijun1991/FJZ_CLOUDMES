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
    public class BOM
    {
        public BOM()
        {
			
        }
 
       #region  BOM实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHANGE_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PARENT_ITEM_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_REV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QUANTITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FIND_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REF_DES {get; set;}
	
	   
	   #endregion
    }
}
 
 