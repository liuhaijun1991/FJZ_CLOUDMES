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
    public class sfcpackoutweight
    {
        public sfcpackoutweight()
        {
			
        }
 
       #region  sfcpackoutweight实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double BOX_NT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double BOX_GW {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double PCS_NT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double PCS_GW {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double P_NULLWG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double P_GW {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PCS_B {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PCS_P {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal BOX_P {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDT {get; set;}
	
	   
	   #endregion
    }
}
 
 