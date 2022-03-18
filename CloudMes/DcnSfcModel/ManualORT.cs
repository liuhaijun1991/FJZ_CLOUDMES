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
    public class ManualORT
    {
        public ManualORT()
        {
			
        }
 
       #region  ManualORT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int minhours {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int maxhours {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alerthours {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime beginautodt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime endautodt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 