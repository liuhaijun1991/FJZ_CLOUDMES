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
    public class R_AGILE_ITEM_HEAD
    {
        public R_AGILE_ITEM_HEAD()
        {
			
        }
 
       #region  R_AGILE_ITEM_HEAD实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PARENT_ITEMNUM {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REV_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDIT_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EDIT_EMP {get; set;}
	
	   
	   #endregion
    }
}
 
 