using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class SkinPreview : Form
    {
        public string skin = "";
        public SkinPreview(string skinname)
        {
            InitializeComponent();
            skin = skinname;
        }

        private void SkinPreview_Load(object sender, EventArgs e)
        {
            DirectoryInfo di = (new DirectoryInfo(Application.StartupPath)).Parent.Parent;
            skinEngine1.SkinFile = di.FullName + "\\skins\\" + skin;
        }
    }
}
