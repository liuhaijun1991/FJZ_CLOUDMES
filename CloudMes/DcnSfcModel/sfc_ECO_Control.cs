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
    public class sfc_ECO_Control
    {
        public sfc_ECO_Control()
        {
			
        }
 
       #region  sfc_ECO_Control实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ECO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ECO_info {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ECO_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ECO_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PEEsop {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Esop {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PEDeviation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Deviation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PELabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Label {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PEBom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Bom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PE_choose {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Esop_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Esop_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_choose {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KeyPart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KeyPart_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime KeyPart_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Material {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MaterialPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int StockQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int POQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Material_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Material_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_Stencil {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_file {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AOItest {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DXtest {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PE_PTH {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PE_PTH_file {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PE_PTH_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PE_PTH_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRESS_FIT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ICTtest {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TE_Test {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TE_Test_file {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TE_Test_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TE_Test_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PC_Wo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PC_Wo_file {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PC_Wo_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PC_Wo_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QE_FAI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QE_FAI_file {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QE_FAI_Emp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime QE_FAI_Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Data1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Data2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Data3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Data4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATUS {get; set;}
	
	
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
 
 