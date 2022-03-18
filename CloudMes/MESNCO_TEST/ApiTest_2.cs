using MESNCO;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;

namespace MESNCO_TEST
{
    public partial class ApiTest_2 : Form
    {
        IStationObj I = new StationObj();
        public ApiTest_2()
        {
            InitializeComponent();
        }

        private void ApiTest_2_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            tbLoginMsg.Text = I.SetConnPara(tbMESServic.Text, tbEmp.Text, tbPwd.Text);
        }

        void CallTest(object args)
        {
            I.CallTest("1256");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ThreadPool.SetMinThreads(120, 120);
            ThreadPool.SetMaxThreads(500, 500);
            for (int i = 0; i < 5000; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(CallTest), null);
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoTimes));
            }

            

            return;
            string function = cbbFunction.SelectedItem == null ? "" : cbbFunction.SelectedItem.ToString();
            string input = tbApiInput.Text;
            string result = "";
            if (string.IsNullOrEmpty(function))
            {
                MessageBox.Show("Please select a function name");
                return;
            }
            try
            {                
                switch (function)
                {
                    case "UpdateRotationDetail":
                        result = I.UpdateRotationDetail(input);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }                  
            tbApiOutput.Text = result;
        }
        private void btnPost_Click(object sender, EventArgs e)
        {
            //GetWebserviceResult("http://10.120.246.102:805/oracleSmtloading.asmx", "OracleAutoSmtLoading", new string[] { "wo", "trsn", "sn", "line", "user", "barcode" });
            //return;
            Post();           
        }       

        private void Post()
        {
            string url = tbUrl.Text;          
            string input = tbInputData.Text;
            string result = "";           
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("URL is null!");
                return;
            }            
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("input data is null!");
                return;
            }            
            try
            {   
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($@"{url}");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "text/html,application/xhtml+xml,*/*";                
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                httpWebRequest.ContentLength = bytes.Length;
                using (Stream reqSteam = httpWebRequest.GetRequestStream())
                {
                    reqSteam.Write(bytes, 0, bytes.Length);
                    reqSteam.Flush();
                }
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = sr.ReadToEnd();
                }
            }
            catch (WebException webex)
            {
                var res = (HttpWebResponse)webex.Response;
                StreamReader sr_ex = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                result = sr_ex.ReadToEnd();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            tbOutputData.Text = result;
        }
              

        private void TaskTest()
        {
            List<Task> listTasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {              
                TaskFactory factory = new TaskFactory();
                Task task = factory.StartNew(() =>
                {                 
                    I.GetSNStationKPList("JN127077FAFB");
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                    try
                    {
                        I.GetSNStationKPList("JN127077FAFB");
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                listTasks.Add(task);
            }
            Task.WaitAll(listTasks.ToArray());
            Console.WriteLine("Task End");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 1000; i++)
            {
                //Thread T = new Thread(new ThreadStart(DoTimes));
                //T.Start();
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoTimes));
               
            }
        }
        static object syncObj ;
        void DoTimes()
        {
            syncObj = 0;
            Post1();
        }
        void DoTimes(object o)
        {
            syncObj = 0;
            Post1();
        }

        private void Post1()
        {
            int a;
            lock (syncObj)
            {
                a = ((int)syncObj);
                a++;
                syncObj = a;
            }
            string url = tbUrl.Text;
            string input = tbInputData.Text;
            string result = "";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("URL is null!");
                return;
            }
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("input data is null!");
                return;
            }
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($@"{url}");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "text/html,application/xhtml+xml,*/*";
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.Timeout = 999999999;
                using (Stream reqSteam = httpWebRequest.GetRequestStream())
                {
                    reqSteam.Write(bytes, 0, bytes.Length);
                    reqSteam.Flush();
                }
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = sr.ReadToEnd();
                }
            }
            catch (WebException webex)
            {
                try
                {
                    var res = (HttpWebResponse)webex.Response;
                    StreamReader sr_ex = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    result = sr_ex.ReadToEnd();
                }
                catch (Exception eee)
                {
                    result = a.ToString()+"\r\n"+eee.Message;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            if (result.IndexOf("{\"DUMMY\":\"X\"}") < 0)
            {
                MessageBox.Show(result);
            }

            //tbOutputData.Text = result;
        }
    
        private object GetWebserviceResult(string url,string functionName,object [] args)
        {
            //string strUrl = "http://10.120.246.102:805/oracleSmtloading.asmx";
            //string[] objs = new string[] { "wo", "trsn", "sn", "line", "user", "barcode" };
            //用动态调用web服务的方法獲取結果
            //object result1 = DynamicRefWebservice.InvokeWebService(strUrl, "HelloWorld", null);
            //object result = DynamicRefWebservice.InvokeWebService(url, "OracleAutoSmtLoading", objs);
            return DynamicRefWebservice.InvokeWebService(url, functionName, args);
        }
    }

    public class DynamicRefWebservice
    {
        private static ParameterInfo[] pi;

        /// < summary>
        /// 动态调用web服务
        /// < /summary>
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="methodname">方法名< /param>
        /// < param name="args">参数< /param>
        /// < returns>< /returns>
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return InvokeWebService(url, null, methodname, args);
        }

        /// < summary>
        /// 动态调用web服务
        /// < /summary>
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="classname">类名< /param>
        /// < param name="methodname">方法名< /param>
        /// < param name="args">参数< /param>
        /// < returns>< /returns>
        public static object InvokeWebService(
            string url,
            string classname,
            string methodname,
            object[] args)
        {
            string @namespace = "RefWebservice.DynamicRefWS";

            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(url);
            }

            try
            {
                //MessageBox.Show("获取WSDL url:" + url);
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider ccp = new CSharpCodeProvider();

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //MessageBox.Show("编译代理类");
                //编译代理类
                CompilerResults cr = ccp.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //MessageBox.Show("生成代理实例 ");
                //生成代理实例
                Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                //MessageBox.Show("獲取并調用實例方法 methodname:" + methodname);
                //獲取并調用實例方法
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                pi = mi.GetParameters();
                //MessageBox.Show("invoke :" + args);
                return mi.Invoke(obj, args);
            }
            catch (System.Reflection.TargetInvocationException tiex)
            {
                string msg = tiex.Message + "**" + tiex.Source + "**" + tiex.StackTrace + "**" + tiex.TargetSite;

                object objException;
                objException = msg + ",Declare Parameter Count:" + pi.Length +
                    ",Now Parameter Count:" + args.Length;
                for (int i = 0; i < pi.Length; i++)
                {
                    objException += ",Type:" + pi[i].ParameterType.FullName + ",Name:" + pi[i].Name;
                }

                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                object objException;
                objException = ex.Message + ",Declare Parameter Count:" + pi.Length +
                    ",Now Parameter Count:" + args.Length;
                for (int i = 0; i < pi.Length; i++)
                {
                    objException += ",Type:" + pi[i].ParameterType.FullName + ",Name:" + pi[i].Name;
                }
                throw new Exception(objException.ToString());
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}
