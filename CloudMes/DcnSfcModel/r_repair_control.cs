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
    public class r_repair_control
    {
        public r_repair_control()
        {
			
        }
 
       #region  r_repair_control实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string remark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string control_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string confirmby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime confirmdt {get; set;}
	
	   
	   #endregion
    }
}
 
 