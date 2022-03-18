using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;


namespace MESMailCenter
{
    public class HostInfo
    {
        public static string HostName
        {
            get
            {
                return Dns.GetHostName();
            }
        }
        public static IPAddress[] IP
        {
            get
            {
                return Dns.GetHostAddresses(HostName);
            }
        }
        public static List< string> Mac
        {
            get
            {
                List<string> Ret = new List<string>(); 
                //System.Management.ManagementObjectSearcher query = 
                //    new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
                //System.Management.ManagementObjectCollection res = query.Get();
                System.Management.ManagementClass MOC = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
                System.Management.ManagementObjectCollection MC = MOC.GetInstances();
                foreach (System.Management.ManagementObject item in MC)
                {
                    try
                    {
                        if ((bool )item["IPEnable"] == true)
                        {
                            Ret.Add(item["MacAddress"].ToString());
                        }
                    }
                    catch
                    { }
                }
                return Ret;

                //System.Management.ManagementClass MC = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");

                
            }
        }
    }
}
