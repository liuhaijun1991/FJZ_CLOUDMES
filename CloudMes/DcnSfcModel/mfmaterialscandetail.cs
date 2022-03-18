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
    public class mfmaterialscandetail
    {
        public mfmaterialscandetail()
        {
			
        }
 
       #region  mfmaterialscandetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string kp_sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lastevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandatetime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scandby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkout_wo {get; set;}
	
	   
	   #endregion
    }
}
 
 