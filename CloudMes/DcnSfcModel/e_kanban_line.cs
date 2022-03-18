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
    public class e_kanban_line
    {
        public e_kanban_line()
        {
			
        }
 
       #region  e_kanban_line实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
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
 
 