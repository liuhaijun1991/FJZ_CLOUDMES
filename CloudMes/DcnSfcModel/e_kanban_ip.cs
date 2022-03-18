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
    public class e_kanban_ip
    {
        public e_kanban_ip()
        {
			
        }
 
       #region  e_kanban_ip实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lineSeqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int eventseqno {get; set;}
	
	   
	   #endregion
    }
}
 
 