using System;
using System.Windows.Forms;

namespace Ella_Rose_Assignment
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // These two lines are important for Windows Forms applications
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
