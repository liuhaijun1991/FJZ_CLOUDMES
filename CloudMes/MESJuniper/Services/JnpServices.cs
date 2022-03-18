using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MESInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MESJuniper.Services
{
    partial class JnpServices : ServiceBase
    {
        public JnpServices()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MesLog.Info($@"JnpServices Start :{DateTime.Now}");
            servicesetup();
        }

        void servicesetup()
        {
            using (var db = OleExec.GetSqlSugarClient("JNPODB", false))
            {
                var servicesobj = db.Queryable<C_SERVICE_CONTROL>().Where(t => t.SERVERNAME == ServicesEnum.JnpServices.ToString()).ToList();
                foreach (var item in servicesobj)
                {
                    Timer timer = new Timer();
                    timer.Interval = Convert.ToDouble(item.TIMESPAN);
                    timer.Elapsed += (sender, args) => servicesetup(sender, item);
                    timer.Start();
                }
            }
        }

        void servicesetup(object sender, C_SERVICE_CONTROL item)
        {
            Assembly assembly = Assembly.Load(item.CLASSNAME.Split('.')[0]);
            var APIType = assembly.GetType(item.CLASSNAME);
            object API_CLASS = assembly.CreateInstance(item.CLASSNAME);
            var f = APIType.GetMethod(item.FUNCTIONNAME);
            f.Invoke(API_CLASS,null);
        }

        protected override void OnStop()
        {
            MesLog.Info($@"JnpServices Stop :{DateTime.Now}");
        }
    }
}
