using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.JUNIPER
{
    public partial class ICTDataUploadUI : UserControl
    {
        ICTDataUpload task;
        public ICTDataUploadUI()
        {
            InitializeComponent();
        }

        public ICTDataUploadUI(ICTDataUpload U)
        {
            task = U;
            InitializeComponent();
            
        }

        private void ICTDataUploadUI_Load(object sender, EventArgs e)
        {
            label1.Text = "URL:"+ task.url + "\r\nUSER:"+task.user+ "\r\nPWD:" + task.pwd+"\r\nPATH:"+task.path+"\r\n";
        }

    }
}
