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
    public class KanbanWip_his
    {
        public KanbanWip_his()
        {
			
        }
 
       #region  KanbanWip_his实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string StationName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 