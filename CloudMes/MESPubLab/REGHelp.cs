using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab
{
    public class REGHelp
    {
        public static void setRegKey(string Key, string Value)
        {
            Microsoft.Win32.RegistryKey HKLM = Registry.CurrentUser;
            Microsoft.Win32.RegistryKey hkSoftware = HKLM.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey hkFoxconn = hkSoftware.CreateSubKey("Foxconn");
            Microsoft.Win32.RegistryKey hkMine = hkFoxconn.CreateSubKey("CloundMES");

            hkMine.SetValue(Key, Value);
        }
        public static String getRegValue(string Key)
        {
            Microsoft.Win32.RegistryKey HKLM = Registry.CurrentUser;
            Microsoft.Win32.RegistryKey hkSoftware = HKLM.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey hkFoxconn = hkSoftware.CreateSubKey("Foxconn");
            Microsoft.Win32.RegistryKey hkMine = hkFoxconn.CreateSubKey("CloundMES");

            string v;
            try
            {
                v = (string)hkMine.GetValue(Key);
                
            }
            catch
            {
                return "";
            }
            if (v == null)
            {
                v = "";
            }
            return v;
        }
    }
}
