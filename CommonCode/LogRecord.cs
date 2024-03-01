using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TurbineMonitoringVarableAnalysis.CommonCode
{
    public static class LogRecord
    {
        #region 写日志
        public static void WriteLog(string content)
        {
            FileInfo fi = new FileInfo(new DirectoryInfo(Application.StartupPath.ToString()).Parent.Parent.FullName + "\\Files\\Log\\LogFile.txt");
            FileStream fs = new FileStream(fi.FullName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "#" + content.Replace("\r\n", "   "));
            sw.Close();
            fs.Close();
        }
        #endregion
    }
}
