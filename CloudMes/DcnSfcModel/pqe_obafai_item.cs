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
    public class pqe_obafai_item
    {
        public pqe_obafai_item()
        {
			
        }
 
       #region  pqe_obafai_item实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string serialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string correctiveaction {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string frequency {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string department {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime begindate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime enddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string files {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
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
 
 