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
    public class webuserlist
    {
        public webuserlist()
        {
			
        }
 
       #region  webuserlist实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PASSWORD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USERNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USERLEVEL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USERROLE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DEPARTMENTNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMPANYNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MAIL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CREATEBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATEDATETIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 