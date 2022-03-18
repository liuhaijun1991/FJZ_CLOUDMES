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
    public class sfcshipcontainer
    {
        public sfcshipcontainer()
        {
			
        }
 
       #region  sfcshipcontainer实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string containerno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime stuffingdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string weekno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string containertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short defaultpalletnbr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int containpalletnbr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool stuffinginfull {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime stuffingfulldate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string destinationport {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custshipto {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vehicleno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vessel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipweekno {get; set;}
	
	
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
 
 