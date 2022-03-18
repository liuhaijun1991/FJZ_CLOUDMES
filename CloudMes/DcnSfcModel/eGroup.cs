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
    public class eGroup
    {
        public eGroup()
        {
			
        }
 
       #region  eGroup实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GROUPNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GROUPTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
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
 
 