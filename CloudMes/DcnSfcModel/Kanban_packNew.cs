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
    public class Kanban_packNew
    {
        public Kanban_packNew()
        {
			
        }
 
       #region  Kanban_packNew实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double uph {get; set;}
	
	   
	   #endregion
    }
}
 
 