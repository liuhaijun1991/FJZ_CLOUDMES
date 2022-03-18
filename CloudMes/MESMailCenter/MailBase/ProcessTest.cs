using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MESMailCenter
{
    class ProcessTest:Process
    {
        public override void Start()
        {
            //base.Start();
            //System.Windows.Forms.MessageBox.Show("Hello world!!");
            System.Diagnostics.Process.Start("notepad.exe");
            
        }
    }
}
