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
    public class MfAtoPoTracking
    {
        public MfAtoPoTracking()
        {
			
        }
 
       #region  MfAtoPoTracking实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Po {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EditTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Remark {get; set;}
	
	   
	   #endregion
    }
}
 
 