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
    public class ePerMission
    {
        public ePerMission()
        {
			
        }
 
       #region  ePerMission实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PERMISSIONTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PERMISSIONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string functionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool MODIFICATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 