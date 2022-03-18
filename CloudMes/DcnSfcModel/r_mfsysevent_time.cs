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
    public class r_mfsysevent_time
    {
        public r_mfsysevent_time()
        {
			
        }
 
       #region  r_mfsysevent_time实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasttime {get; set;}
	
	   
	   #endregion
    }
}
 
 