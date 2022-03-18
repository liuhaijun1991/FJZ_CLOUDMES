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
    public class R_TESTED_COUNT
    {
        public R_TESTED_COUNT()
        {
			
        }
 
       #region  R_TESTED_COUNT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int counter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool sapflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 