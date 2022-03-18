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
    public class H_CycleCount_SN_His
    {
        public H_CycleCount_SN_His()
        {
			
        }
 
       #region  H_CycleCount_SN_His实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CARTON_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PALLET_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Container_TIME {get; set;}
	
	   
	   #endregion
    }
}
 
 