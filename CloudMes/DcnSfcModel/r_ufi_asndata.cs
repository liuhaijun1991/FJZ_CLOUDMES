using SqlSugar;
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
	public class r_ufi_asndata
	{
		public r_ufi_asndata()
		{

		}

		#region  r_ufi_asndata实体

		//[SugarColumn(IsPrimaryKey = true)]
		public string DN_NO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string PNO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string System_SNO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string Vanilla_SNO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string MB_PCBA_SNO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string UFI_PO_NO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public DateTime SHIP_DATE { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string TO_NO { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string SSD_Vendor { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string MAC_Addr { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string Carton_ID { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string Pallet_ID { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string SendFlag { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public DateTime Create_Time { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Edit_Emp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime Edit_Time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_SN_0 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_SN_1 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_SN_2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_SN_3 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_PN_0 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_PN_1 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_PN_2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PSU_PN_3 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_0 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_1 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_3 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_4 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_5 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_6 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_SN_7 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_0 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_1 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_3 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_4 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_5 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_6 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Fan_PN_7 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Message { get; set; }

		#endregion
	}
}

