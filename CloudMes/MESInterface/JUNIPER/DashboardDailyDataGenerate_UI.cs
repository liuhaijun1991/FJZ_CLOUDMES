using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MESInterface.JUNIPER
{
    public partial class DashboardDailyDataGenerate_UI : UserControl
    {
        DashboardDailyDataGenerate generate;
        SynchronizationContext synchronization;
        public DashboardDailyDataGenerate_UI(DashboardDailyDataGenerate _generate)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            generate = _generate;
            synchronization = SynchronizationContext.Current;
        }

    }
}
