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
    public class SfcRepalceSN
    {
        public SfcRepalceSN()
        {
			
        }
 
       #region  SfcRepalceSN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OldSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NewSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteidtby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 