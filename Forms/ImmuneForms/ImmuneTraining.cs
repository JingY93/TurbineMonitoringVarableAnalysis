using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode.ArtificalImmune;
using TurbineMonitoringVarableAnalysis.CommonCode;

namespace TurbineMonitoringVarableAnalysis.Forms.ImmuneForms
{
    public partial class ImmuneTraining : Form
    {
        public ImmuneTraining()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            //获取训练数据
            string filepath = this.textBox2.Text;
            DataTable dt_temp = ExcelHelper.OpenCSV3(filepath,1);
            //训练数据案列标准化
            DataTable dt_normalization = MYSQLHelper.NormilizationDataTable(dt_temp);

            //开始对数据进行训练，打开这个注释，需要两个动作，注释下一行，同时，在Antibody类中，的evaluate函数进行相应的修改
            //IAController ia = new IAController(dt_normalization.Columns.Count, dt_normalization);
            IAController ia = new IAController(2);
            ia.GoOptimise();

            string output = "抗体：";
            int antibody_num = 1;
            foreach (Antibody c in ia.AntibodyArray)
            {
                output = "抗体：【" + antibody_num + " 】   ";
                antibody_num++;
                double[] xyvals = c.XYvalues;
                for (int i = 0; i < xyvals.Length; i++)
                {
                    output = output + xyvals[i].ToString("F4") + "  ";
                }
                output = output + " fitness = " + c.Fitness.ToString("F4") + "\r\n\r\n";

                textBox1.AppendText(output);
            }
        }
    }
}
