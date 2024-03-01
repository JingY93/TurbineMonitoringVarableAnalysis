using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.Forms;
using TurbineMonitoringVarableAnalysis.Forms.ImmuneForms;

namespace TurbineMonitoringVarableAnalysis
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            //Application.Run(new ElecLineDataAnalysis());
            //Application.Run(new ImmuneTraining());
            //Application.Run(new ModeCenterView());
            //Application.Run(new DataBackup());
        }
    }
}
