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
    public class R_STATIONEVENT_CONFIG
    {
        public R_STATIONEVENT_CONFIG()
        {
			
        }
 
       #region  R_STATIONEVENT_CONFIG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int C_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CONFIG_LV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SEQ {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ADDTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATAEX1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATAEX2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATAEX3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EMP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDIT_DATE {get; set;}
	
	   
	   #endregion
    }
}
 
 