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
    public class sfcPCBALotDetail
    {
        public sfcPCBALotDetail()
        {
			
        }
 
       #region  sfcPCBALotDetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SysSerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LotNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Pass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDt {get; set;}
	
	   
	   #endregion
    }
}
 
 