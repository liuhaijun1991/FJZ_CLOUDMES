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
    public class sfcControlRunDetail
    {
        public sfcControlRunDetail()
        {
			
        }
 
       #region  sfcControlRunDetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SysserialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ControlRunNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDt {get; set;}
	
	   
	   #endregion
    }
}
 
 