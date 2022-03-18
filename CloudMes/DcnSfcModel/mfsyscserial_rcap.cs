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
    public class mfsyscserial_rcap
    {
        public mfsyscserial_rcap()
        {
			
        }
 
       #region  mfsyscserial_rcap实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string kp_sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandt {get; set;}
	
	   
	   #endregion
    }
}
 
 