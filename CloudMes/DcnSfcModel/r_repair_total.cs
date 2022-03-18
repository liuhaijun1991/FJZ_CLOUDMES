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
    public class r_repair_total
    {
        public r_repair_total()
        {
			
        }
 
       #region  r_repair_total实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int scrap {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int B89Y {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int B89N {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int B78M {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int B79M {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int InLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int realTotal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lastCheckInOut {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CheckInOut {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime executeDt {get; set;}
	
	   
	   #endregion
    }
}
 
 