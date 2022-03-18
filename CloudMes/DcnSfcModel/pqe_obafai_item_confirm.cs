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
    public class pqe_obafai_item_confirm
    {
        public pqe_obafai_item_confirm()
        {
			
        }
 
       #region  pqe_obafai_item_confirm实体
 
	
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
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tempfile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string remark {get; set;}
	
	
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
 
 