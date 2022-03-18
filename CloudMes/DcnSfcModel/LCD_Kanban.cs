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
    public class LCD_Kanban
    {
        public LCD_Kanban()
        {
			
        }
 
       #region  LCD_Kanban实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Work_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Span_Time {get; set;}
	
	   
	   #endregion
    }
}
 
 