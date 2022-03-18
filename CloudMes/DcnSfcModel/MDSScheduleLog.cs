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
    public class MDSScheduleLog
    {
        public MDSScheduleLog()
        {
			
        }
 
       #region  MDSScheduleLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datapoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime previousgetdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime currentgetdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 