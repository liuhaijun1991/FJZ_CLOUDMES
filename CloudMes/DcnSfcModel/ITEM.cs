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
    public class ITEM
    {
        public ITEM()
        {
			
        }
 
       #region  ITEM实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHANGE_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHANGE_FUNCTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OLDREV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NEWREV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RELEASE_DATE {get; set;}
	
	   
	   #endregion
    }
}
 
 