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
    public class CheckQty
    {
        public CheckQty()
        {
			
        }
 
       #region  CheckQty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TableNamed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RowQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reserved {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string data {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string index_size {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string unused {get; set;}
	
	   
	   #endregion
    }
}
 
 