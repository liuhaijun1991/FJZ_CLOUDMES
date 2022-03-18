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
    public class EXECSQLLOG
    {
        public EXECSQLLOG()
        {
			
        }
 
       #region  EXECSQLLOG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TEMPSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SQLSTR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime DTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 