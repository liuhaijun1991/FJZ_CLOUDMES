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
    public class sdcontractlog
    {
        public sdcontractlog()
        {
			
        }
 
       #region  sdcontractlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime logdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contractno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventdetail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventby {get; set;}
	
	   
	   #endregion
    }
}
 
 