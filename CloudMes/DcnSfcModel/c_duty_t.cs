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
    public class c_duty_t
    {
        public c_duty_t()
        {
			
        }
 
       #region  c_duty_t实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dutytype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dutydesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	   
	   #endregion
    }
}
 
 