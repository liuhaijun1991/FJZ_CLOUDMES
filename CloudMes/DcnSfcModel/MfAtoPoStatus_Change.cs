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
    public class MfAtoPoStatus_Change
    {
        public MfAtoPoStatus_Change()
        {
			
        }
 
       #region  MfAtoPoStatus_Change实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int StatusID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EditTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int PoQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoissueDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PoissueTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PreWoFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitPrice {get; set;}
	
	   
	   #endregion
    }
}
 
 