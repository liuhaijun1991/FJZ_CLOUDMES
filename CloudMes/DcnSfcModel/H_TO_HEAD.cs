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
    public class H_TO_HEAD
    {
        public H_TO_HEAD()
        {
			
        }
 
       #region  H_TO_HEAD实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PLAN_STARTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PLAN_ENDTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime REAL_STARTTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime REAL_ENDTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TO_CREATETIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TO_LASTUPDATEIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTAINER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VEHICLE_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EXTENAL_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ABNORMITY_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DROP_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ship_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ship_country {get; set;}
	
	   
	   #endregion
    }
}
 
 