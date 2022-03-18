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
    public class PP_OperationMapping
    {
        public PP_OperationMapping()
        {
			
        }
 
       #region  PP_OperationMapping实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAP_Operation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_Operation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_Opt_Desc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAP_Opt_Desc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDt {get; set;}
	
	   
	   #endregion
    }
}
 
 