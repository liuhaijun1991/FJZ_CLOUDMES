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
    public class mfbundlepart
    {
        public mfbundlepart()
        {
			
        }
 
       #region  mfbundlepart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string chassisno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string applepartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
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
 
 