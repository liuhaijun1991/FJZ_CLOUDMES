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
    public class R_mrb_err
    {
        public R_mrb_err()
        {
			
        }
 
       #region  R_mrb_err实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string start_station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string err {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandt {get; set;}
	
	   
	   #endregion
    }
}
 
 