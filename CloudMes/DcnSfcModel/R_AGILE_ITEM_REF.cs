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
    public class R_AGILE_ITEM_REF
    {
        public R_AGILE_ITEM_REF()
        {
			
        }
 
       #region  R_AGILE_ITEM_REF实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int BOM_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REF_DESG {get; set;}
	
	
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
 
 