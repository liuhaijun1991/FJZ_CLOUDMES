using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.ARUBA
{
    public class R_SNR
    {
    }

    /// <summary>
    /// Header Record/Trailer Record
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_HEAD")]
    public class R_SNR_HEAD
    {
        public string RId { get; set; }
        public string Data_Type { get; set; }
        public string Key_Type { get; set; }
        public string Key_Value { get; set; }
        public DateTime? Start_Time { get; set; }
        public DateTime? End_Time { get; set; }
        public string File_Name { get; set; }
        public string File_Local_Path { get; set; }
        public string File_Remote_Path { get; set; }
        public int? Seq { get; set; }
        public string Header_Record { get; set; }
        public string Trailer_Record { get; set; }
        public string Get { get; set; }
        public DateTime? Get_Time { get; set; }
        public string Conver { get; set; }
        public DateTime? Conver_Time { get; set; }
        public string Send { get; set; }
        public DateTime? Send_Time { get; set; }
        public string Cancel { get; set; }
    }

    /// <summary>
    /// Bundle Parent Record Descriptor's Module
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_BP")]
    public class R_SNR_BP
    {
        public string RId { get; set; }
        public string Record_Type_Id { get; set; }
        public string Serial_Number { get; set; }
        public string Product_Number { get; set; }
        public string Bundle_Description { get; set; }
        public int? Record_Origin_Id { get; set; }
        public string Sub_FA_Origin_Id { get; set; }
        public string Localization_Hardware_Option { get; set; }
        public string Warranty_Option { get; set; }
        public string Other_Options { get; set; }
        public string Ship_Date { get; set; }
        public string Asset_Tag { get; set; }
        public string Future { get; set; }
        public int? Num_Products_In_Bundle { get; set; }
    }

    /// <summary>
    /// Bundle CHILD Record Descriptor's Module
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_BC")]
    public class R_SNR_BC
    {
        public string RId { get; set; }
        public string Record_Type_Id { get; set; }
        public string Serial_Number { get; set; }
        public string Product_Number { get; set; }
        public string Localization_Option { get; set; }
        public string Parent_Serial_Number { get; set; }
    }

    /// <summary>
    /// Refurbished Products
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_RP")]
    public class R_SNR_RP
    {
        public string RId { get; set; }
        public string Record_Type_Id { get; set; }
        public string New_Serial_Number { get; set; }
        public string New_Product_Number { get; set; }
        public string New_Localization_Option { get; set; }
        public string Old_Serial_Number { get; set; }
        public string Old_Product_Number { get; set; }
        public string Date { get; set; }
        public string Sub_FA_Origin_Id { get; set; }
    }

    /// <summary>
    /// Product Configuration Record(Base Unit Product Descriptor's Module)
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_PC")]
    public class R_SNR_PC
    {
        public string RId { get; set; }
        public string Record_Type_Id { get; set; }
        public string Serial_Number { get; set; }
        public string Product_Number { get; set; }
        public int? Record_Origin_Id { get; set; }
        public string Sub_FA_Origin_Id { get; set; }
        public string Localization_Hardware_Option { get; set; }
        public string Warranty_Option { get; set; }
        public string Other_Options { get; set; }
        public string Date { get; set; }
        public string Asset_Tag { get; set; }
        public string Future { get; set; }
        public string Test_Result { get; set; }
        public int? Nb_Sub_Modules { get; set; }
    }

    /// <summary>
    /// Product Configuration Record(Hardware Commodity Descriptor's Module)
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_HC")]
    public class R_SNR_HC
    {
        public string RId { get; set; }
        public string PC_Serial_Number { get; set; }
        public string Module_Type_Id { get; set; }
        public string Generic_Category { get; set; }
        public string HP_Component_Part_Number { get; set; }
        public string Supplier_Part_Number { get; set; }
        public string Serial_Number { get; set; }
        public string CT_Serial_Number_Or_Date_Code { get; set; }
        public string Hardware_Revision { get; set; }
        public string Firmware_Revision { get; set; }
        public string Supplier_Name { get; set; }
        public string E_T_Status { get; set; }
        public string Type_Of_Operation { get; set; }
        public int? Quantity { get; set; }
        public string Parent_Product { get; set; }
        public string Family { get; set; }
        public string Part_Category_Or_Commodity_Code { get; set; }
        public string Description { get; set; }
        public string Eatra_Info { get; set; }
    }

    /// <summary>
    /// Product Configuration Record(Virtual Part Data)
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_VP")]
    public class R_SNR_VP
    {
        public string RId { get; set; }
        public string PC_Serial_Number { get; set; }
        public string Module_Type_Id { get; set; }
        public string Generic_Category { get; set; }
        public string HP_Component_Part_Number { get; set; }
        public string Supplier_Part_Number { get; set; }
        public string Serial_Number { get; set; }
        public string Date_Code { get; set; }
        public string Hardware_Revision { get; set; }
        public string Firmware_Revision { get; set; }
        public string Supplier_Name { get; set; }
        public string E_T_Status { get; set; }
        public string Type_Of_Operation { get; set; }
        public int? Quantity { get; set; }
        public string Parent_Product { get; set; }
        public string Family { get; set; }
        public string Part_Category_Or_Commodity_Code { get; set; }
        public string Description { get; set; }
        public string Eatra_Info { get; set; }
    }

    /// <summary>
    /// Product Configuration Record(Royalty Tracking Flex Build Descriptor's Module)
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_RT")]
    public class R_SNR_RT
    {
        public string RId { get; set; }
        public string PC_Serial_Number { get; set; }
        public string Module_Type_Id { get; set; }
        public string Generic_Category { get; set; }
        public string IMG_Descriptor { get; set; }
        public string Operation { get; set; }
        public int? Quantity { get; set; }
        public string Dependent_On_IMG_Descriptor { get; set; }
    }

    /// <summary>
    /// Product Configuration Record(Shopping Basket Descriptor's Module)
    /// </summary>
    [SqlSugar.SugarTable("dbo.R_SNR_SB")]
    public class R_SNR_SB
    {
        public string RId { get; set; }
        public string PC_Serial_Number { get; set; }
        public string Module_Type_Id { get; set; }
        public string V_Product_Number { get; set; }
        public string Option_Localization { get; set; }
        public int? Quantity { get; set; }
        public string Description { get; set; }
        public string Feature_Code { get; set; }
        public string Feature_Value { get; set; }
        public string Other_Options { get; set; }
        public string Type_Of_Operation { get; set; }
        public string Future { get; set; }
    }

    [SqlSugar.SugarTable("dbo.SFCCODELIKE_EXTEND")]
    public class SFCCODELIKE_EXTEND
    {
        public string SKUNO { get; set; }
        public string CTYPE { get; set; }
        public string VALUE1 { get; set; }
        public string VALUE2 { get; set; }
        public string VALUE3 { get; set; }
        public string VALUE4 { get; set; }
        public string VALUE5 { get; set; }
        public string INPUT_EMP { get; set; }
        public DateTime? INPUT_DATE { get; set; }
    }

    /// <summary>
    /// PO,SHIP_TO_ADDRESS
    /// </summary>
    public class R_SNR_SHIP_TO_ADDRESS
    {
        public string Serial_Number { get; set; }
        public string PO { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
