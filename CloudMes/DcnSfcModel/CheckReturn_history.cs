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
    public class CheckReturn_history
    {
        public CheckReturn_history()
        {
			
        }
 
       #region  CheckReturn_history实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TableNamed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sendQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int receiveqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int dismatchqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptocountry {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 