using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;

namespace TurbineMonitoringVarableAnalysis.Forms.ImmuneForms
{
    public partial class ParameterConfiguration : Form
    {
        public ParameterConfiguration()
        {
            InitializeComponent();
            BindDefaultConfiguration();
        }

        public void BindDefaultConfiguration()
        {
            ConfReadAndModify conf = new ConfReadAndModify();

            this.textBox1.Text = conf.getConfigSetting("antibodyNumber");
            this.textBox2.Text = conf.getConfigSetting("cloneNumber");
            this.textBox3.Text = conf.getConfigSetting("maxGens");
            this.textBox4.Text = conf.getConfigSetting("mutationFactor");
            this.textBox5.Text = conf.getConfigSetting("removeThreshold");
            this.textBox6.Text = conf.getConfigSetting("clonalSelectionThreshold");
            this.textBox7.Text = conf.getConfigSetting("diversity");
            this.textBox8.Text = conf.getConfigSetting("lowerBoundary");
            this.textBox9.Text = conf.getConfigSetting("upperBoundary");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals("")||this.textBox2.Text.Trim().Equals("")||this.textBox3.Text.Trim().Equals("")||this.textBox4.Text.Trim().Equals("")||this.textBox5.Text.Trim().Equals("")||this.textBox6.Text.Trim().Equals("")||this.textBox7.Text.Trim().Equals("")||this.textBox8.Text.Trim().Equals("")||this.textBox9.Text.Trim().Equals(""))
            {
                MessageBox.Show("参数信息不能为空！","操作信息提示",MessageBoxButtons.OK);
            }
            else
            {
                ConfReadAndModify conf = new ConfReadAndModify();
                bool success1 = conf.modifyConfigSetting("antibodyNumber", this.textBox1.Text.Trim());
                bool success2 = conf.modifyConfigSetting("cloneNumber", this.textBox2.Text.Trim());
                bool success3 = conf.modifyConfigSetting("maxGens", this.textBox3.Text.Trim());
                bool success4 = conf.modifyConfigSetting("mutationFactor", this.textBox4.Text.Trim());
                bool success5 = conf.modifyConfigSetting("removeThreshold", this.textBox5.Text.Trim());
                bool success6 = conf.modifyConfigSetting("clonalSelectionThreshold", this.textBox6.Text.Trim());
                bool success7 = conf.modifyConfigSetting("diversity", this.textBox7.Text.Trim());
                bool success8 = conf.modifyConfigSetting("lowerBoundary", this.textBox8.Text.Trim());
                bool success9 = conf.modifyConfigSetting("upperBoundary", this.textBox9.Text.Trim());

                if (success1&&success2&&success3&&success4&&success5&&success6&&success7&&success8&&success9)
                {
                    MessageBox.Show("模型参数信息配置成功，立即生效！","操作信息提示",MessageBoxButtons.OK);
                }
            }
        }
    }
}
