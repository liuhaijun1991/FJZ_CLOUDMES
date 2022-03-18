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
    public class sfcstationaction
    {
        public sfcstationaction()
        {
			
        }
 
       #region  sfcstationaction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string macaddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string actionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool enabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short actioncount {get; set;}
	
	
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
 
 