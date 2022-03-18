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
    public class CSV_HEAD
    {
        public CSV_HEAD()
        {
			
        }
 
       #region  CSV_HEAD实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILENAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATEDT {get; set;}
	
	   
	   #endregion
    }
}
 
 