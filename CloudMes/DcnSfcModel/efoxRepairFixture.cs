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
    public class efoxRepairFixture
    {
        public efoxRepairFixture()
        {
			
        }
 
       #region  efoxRepairFixture实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHASSISNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SLOTNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SLOTDEBUGTIMES {get; set;}
	
	   
	   #endregion
    }
}
 
 