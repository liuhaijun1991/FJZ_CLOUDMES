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
    public class scmvendordspnote
    {
        public scmvendordspnote()
        {
			
        }
 
       #region  scmvendordspnote实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime notedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string notetype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 