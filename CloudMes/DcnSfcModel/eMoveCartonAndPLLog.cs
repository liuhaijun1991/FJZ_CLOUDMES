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
    public class eMoveCartonAndPLLog
    {
        public eMoveCartonAndPLLog()
        {
			
        }
 
       #region  eMoveCartonAndPLLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Moveobject {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Movefrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Moveto {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MachineMac {get; set;}
	
	
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
 
 