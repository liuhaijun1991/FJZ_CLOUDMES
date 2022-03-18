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
    public class sdshiporderssn
    {
        public sdshiporderssn()
        {
			
        }
 
       #region  sdshiporderssn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_ORDER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int GROUPTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SYSSERIALNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRINT_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime OVERPACKTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	   
	   #endregion
    }
}
 
 