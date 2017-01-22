using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebsiteGetter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WSForm());
            /*
            using (MainForm frm = new MainForm())
            {
                if (!frm.InitializeGraphics()) // Initialize Direct3D
                {
                    MessageBox.Show("Could not initialize Direct3D.  This tutorial will exit.");
                    return;
                }
                frm.Show();

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    frm.Render();
                    Application.DoEvents(); //处理当前在消息队列中的所有 Windows 消息
                }
            }
             */
        }
    }
}
