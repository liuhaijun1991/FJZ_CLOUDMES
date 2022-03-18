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
    public class r_sample_sn
    {
        public r_sample_sn()
        {
			
        }
 
       #region  r_sample_sn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sampletype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sample_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 