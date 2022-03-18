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
    public class MFATOBOMDETAIL
    {
        public MFATOBOMDETAIL()
        {
			
        }
 
       #region  MFATOBOMDETAIL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Top_PID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	   
	   #endregion
    }
}
 
 