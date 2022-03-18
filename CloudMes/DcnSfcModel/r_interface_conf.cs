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
    public class r_interface_conf
    {
        public r_interface_conf()
        {
			
        }
 
       #region  r_interface_conf实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string trantype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string val {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool stopped {get; set;}
	
	
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
 
 