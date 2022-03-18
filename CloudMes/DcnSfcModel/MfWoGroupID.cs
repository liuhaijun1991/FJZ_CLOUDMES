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
    public class MfWoGroupID
    {
        public MfWoGroupID()
        {
			
        }
 
       #region  MfWoGroupID实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Wo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GroupID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RuType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitPrice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitWeight {get; set;}
	
	
	   /// <summary>
	   /// 如果改Group中的料,傳資料給Force
	   /// </summary>
	   public int DwFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TranTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long dw_id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Type_IU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 