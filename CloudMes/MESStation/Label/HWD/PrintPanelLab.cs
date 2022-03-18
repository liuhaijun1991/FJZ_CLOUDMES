using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Label.HWD
{
    public class PrintPanelLab : LabelBase
    {
        #region 傳入參數
        LabelInputValue inputFormat = new LabelInputValue() { Name = "Format", Type = "STRING", Value = "", StationSessionType = "Format", StationSessionKey = "1" };
        LabelInputValue inputLeft = new LabelInputValue() { Name = "Left", Type = "STRING", Value = "", StationSessionType = "Left", StationSessionKey = "1" };
        LabelInputValue inputHeight = new LabelInputValue() { Name = "Height", Type = "STRING", Value = "", StationSessionType = "Height", StationSessionKey = "1" };
        LabelInputValue inputTemperature = new LabelInputValue() { Name = "Temperature", Type = "STRING", Value = "", StationSessionType = "Temperature", StationSessionKey = "1" };
        LabelInputValue inputIsTest = new LabelInputValue() { Name = "IsTest", Type = "STRING", Value = "", StationSessionType = "IsTest", StationSessionKey = "1" };        
        #endregion

        #region 傳出參數
        LabelOutput outputFormat = new LabelOutput() { Name = "FORMAT", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };        
        LabelOutput outputMD = new LabelOutput() { Name = "MD", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputLeft = new LabelOutput() { Name = "LEFT", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputHeight = new LabelOutput() { Name = "HEIGHT", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };   
        LabelOutput outputSerialCode = new LabelOutput() { Name = "SERIAL_CODE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputSerialCodeA = new LabelOutput() { Name = "SERIAL_CODE_A", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputSerialCodeB = new LabelOutput() { Name = "SERIAL_CODE_B", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        #endregion

        public PrintPanelLab()
        {
            this.Inputs.Add(inputLeft);
            this.Inputs.Add(inputHeight);
            this.Inputs.Add(inputTemperature);
            this.Inputs.Add(inputFormat);
            this.Inputs.Add(inputIsTest);

            this.Outputs.Add(outputFormat);
            this.Outputs.Add(outputMD);
            this.Outputs.Add(outputLeft);
            this.Outputs.Add(outputHeight);
            this.Outputs.Add(outputSerialCode);
            this.Outputs.Add(outputSerialCodeA);
            this.Outputs.Add(outputSerialCodeB);
        }

        public override void MakeLabel(OleExec DB)
        {
            //base.MakeLabel(DB);
            T_C_CONTROL TCC = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            string max_sn = "", new_sn = "";
            if (inputIsTest.Value.ToString().ToUpper() == "NO")
            {
                C_CONTROL panelControl = TCC.GetControlByName("PrintPanelSN", DB);
                if (panelControl == null)
                {
                    throw new Exception("PrintPanelSN CONTROL Not Exist");
                }
                max_sn = panelControl.CONTROL_VALUE;
                long fix_value = Convert_String_To_Long(max_sn, 34);
                fix_value++;
                new_sn = Convert_Long_To_String(fix_value, 34);
                outputSerialCode.Value = new_sn;
                outputSerialCodeA.Value = new_sn + "A";
                outputSerialCodeB.Value = new_sn + "B";

                panelControl.CONTROL_VALUE = new_sn;
                DB.BeginTrain();
                int result = TCC.Update(DB, panelControl);

                //T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                //string id = t_r_mes_log.GetNewID("HWD", DB);
                //string sql = $@"insert into r_mes_log(id,program_name,class_name,function_name,log_message,edit_emp,edit_time)
                //                        values('{id}','PrintPanel','MESStation.Label.HWD.PrintPanelLab','MakeLabel','{new_sn}','SYSTEM',sysdate)";
                //DB.ExecSQL(sql);

                DB.CommitTrain();
                if (result == 0)
                {
                    throw new Exception($@"Update C_CONTROL By {panelControl.ID} Fail!");
                }
            }
            else
            {
                outputSerialCode.Value = "PN000001";
                outputSerialCodeA.Value = "PN000001A";
                outputSerialCodeB.Value = "PN000001B";
            }
            #region 模板是斑馬腳本才有用
            //設置默認原點的橫座標
            if (inputLeft.Value.ToString() == "")
            {
                outputLeft.Value = "0";
            }
            else
            {
                outputLeft.Value = inputLeft.Value.ToString();
            }
            //設置默認原點的縱座標
            if (inputHeight.Value.ToString() == "")
            {
                outputHeight.Value = "0";
            }
            else
            {
                outputHeight.Value = inputHeight.Value.ToString();
            }
            outputMD.Value = inputTemperature.Value.ToString();

            if (inputFormat.Value.ToString() != "")
            {
                outputFormat.Value = inputFormat.Value.ToString();
            }
            #endregion
        }

        private static char[] rDigits ={ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
         'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J','K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T',
          'U', 'V', 'W', 'X', 'Y', 'Z'};

        /// <summary>
        /// 将指定基数的数字的 System.String 表示形式转换为等效的 64 位有符号整数。
        /// </summary>
        /// <param name="var_value">包含数字的 System.String。</param>
        /// <param name="var_base">value 中数字的基数，它必须是[2,36]</param>
        /// <returns>等效于 value 中的数字的 64 位有符号整数。- 或 - 如果 value 为 null，则为零。</returns>
        public long Convert_String_To_Long(string var_value, int var_base)
        {
            var_value = var_value.Trim();
            if (string.IsNullOrEmpty(var_value))
            {
                return 0L;
            }

            string sDigits = new string(rDigits, 0, var_base);
            long result = 0;
            //value = reverse(value).ToUpper(); 1
            var_value = var_value.ToUpper();// 2
            for (int i = 0; i < var_value.Length; i++)
            {
                if (!sDigits.Contains(var_value[i].ToString()))
                {
                    throw new ArgumentException(string.Format("This input information  \"{0}\" is not in {1} preset system.", var_value[i], var_base));
                }
                else
                {
                    try
                    {
                        //result += (long)Math.Pow(fromBase, i) * getcharindex(rDigits, value[i]); 1
                        result += (long)Math.Pow(var_base, i) * Get_Char_Int(rDigits, var_value[var_value.Length - i - 1]);//   2
                    }
                    catch
                    {
                        throw new OverflowException("运算溢出.");
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 取得char對應的int.
        /// </summary>
        /// <param name="var_array_char"></param>
        /// <param name="var_value"></param>
        /// <returns></returns>
        public int Get_Char_Int(char[] var_array_char, char var_value)
        {
            for (int i = 0; i < var_array_char.Length; i++)
            {
                if (var_array_char[i] == var_value)
                {
                    return i;
                }
            }
            return 0;
        }

        //long转化为toBase进制
        public string Convert_Long_To_String(long var_value, int var_base)
        {
            int var_int = 0;
            long longPositive = Math.Abs(var_value);
            int radix = var_base;
            char[] outDigits = new char[63];

            for (var_int = 0; var_int <= 64; var_int++)
            {
                if (longPositive == 0) { break; }

                outDigits[outDigits.Length - var_int - 1] =
                    rDigits[longPositive % radix];
                longPositive /= radix;
            }

            return new string(outDigits, outDigits.Length - var_int, var_int);
        }
    }
}
