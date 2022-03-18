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
    public class SFC_Bonepile_Log
    {
        public SFC_Bonepile_Log()
        {
			
        }
 
       #region  SFC_Bonepile_Log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string trantype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datatype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errmsg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 