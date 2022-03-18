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
    public class Kanban_LINE
    {
        public Kanban_LINE()
        {
			
        }
 
       #region  Kanban_LINE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFCLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string APLine {get; set;}
	
	   
	   #endregion
    }
}
 
 