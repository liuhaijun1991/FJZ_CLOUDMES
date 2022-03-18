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
    public class TE_UUTTYPE_NAME
    {
        public TE_UUTTYPE_NAME()
        {
			
        }
 
       #region  TE_UUTTYPE_NAME实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UUTTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UUTNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double ESSTestTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double TestTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Family {get; set;}
	
	   
	   #endregion
    }
}
 
 