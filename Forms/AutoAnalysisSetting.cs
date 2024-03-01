using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class AutoAnalysisSetting : Form
    {
        public AutoAnalysisSetting()
        {
            InitializeComponent();
            BindConfiguration();
        }
        public void BindConfiguration() 
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string autoanalysis = conf.getConfigSetting("AutoAnalysis");
            if (autoanalysis.Equals("true"))
            {
                this.radioButton1.Checked=true;
            }
            else
            {
                this.radioButton1.Checked = false;
            }
            string AutoAnalysisTime = "2";//默认2点钟自动分析
            if (!conf.getConfigSetting("AutoAnalysisTime").Equals(""))
            {
                AutoAnalysisTime = conf.getConfigSetting("AutoAnalysisTime");
            }
            this.comboBox1.SelectedIndex = int.Parse(AutoAnalysisTime);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            if (this.radioButton1.Checked)
            {
                conf.modifyConfigSetting("AutoAnalysis", "true");
            }
            else
            {
                conf.modifyConfigSetting("AutoAnalysis", "false");
            }
         
            conf.modifyConfigSetting("AutoAnalysisTime", this.comboBox1.SelectedItem.ToString());
            MessageBox.Show("自动分析配置成功，立即生效！", "操作信息提示", MessageBoxButtons.OK);
        } 
    }
}
