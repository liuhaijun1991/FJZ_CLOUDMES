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
    public class eRMA_Bonepile_Pareto_Log
    {
        public eRMA_Bonepile_Pareto_Log()
        {
			
        }
 
       #region  eRMA_Bonepile_Pareto_Log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TranType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string YR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WKOrMO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FirstDateOfWork {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrMsg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 