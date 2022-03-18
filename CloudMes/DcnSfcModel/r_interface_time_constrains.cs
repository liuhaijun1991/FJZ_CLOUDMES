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
    public class r_interface_time_constrains
    {
        public r_interface_time_constrains()
        {
			
        }
 
       #region  r_interface_time_constrains实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TaskName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dtFrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dtTo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 