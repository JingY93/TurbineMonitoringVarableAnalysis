using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TurbineMonitoringVarableAnalysis.CommonCode;
using System.IO;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class ShowElectrLineData : Form
    {
        public string direcorypath = "";
        public string datadate = "";
        public string location = "";
        public string total_rows_count = "";
        public double upper_limitation = 0.0;
        public double lower_limitation = 0.0;

        public ShowElectrLineData(string path,string date,string location_temp,string rows_count,string datavalue)
        {
            InitializeComponent();
            InitChart1();
            InitChart2();

            direcorypath = path;
            datadate = date;
            location = location_temp;
            total_rows_count = rows_count;

            this.textBox1.Text = location_temp;
            this.textBox2.Text = date;
            this.textBox3.Text = datavalue;

            List<DataTable> datas= SearchDataByDatatimeAndLocation();

            if (double.Parse(datavalue)>upper_limitation||double.Parse(datavalue)<lower_limitation)//异常数据
            {
                this.label11.Text = "该点异常"; ;
            }
            else
            {
                this.label11.Text = "该点正常"; ;
            }
            DrawChart1(datas[0]);

            List<double> manmin_chart1 = GetMaxMinData(ConvertDataTableToList(datas[0],3));

            this.chart1.ChartAreas["ChartArea1"].AxisY.Minimum = manmin_chart1[0]*0.95;
            this.chart1.ChartAreas["ChartArea1"].AxisY.Maximum = manmin_chart1[manmin_chart1.Count - 1] * 1.05;

            DrawChart2(datas[1]);

            List<double> manmin_chart2 = GetMaxMinData(ConvertDataTableToList(datas[1], 3));

            this.chart2.ChartAreas["ChartArea1"].AxisY.Minimum = manmin_chart2[0] * 0.95;
            this.chart2.ChartAreas["ChartArea1"].AxisY.Maximum = manmin_chart2[manmin_chart2.Count - 1] * 1.05;

        }
        
        public void DrawChart1(DataTable dt)
        {
            this.chart1.Series[0].Points.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //this.chart1.Series[0].Points.AddXY(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString());
                this.chart1.Series[0].Points.AddXY(dt.Rows[i][0].ToString(), dt.Rows[i][2].ToString());
                if (dt.Rows[i][0].ToString().Equals(location))
                {
                    this.chart1.Series[0].Points[i].MarkerColor = Color.Black;
                    this.chart1.Series[0].Points[i].MarkerSize= 10;
                }
            }
        }

        public void DrawChart2(DataTable dt)
        {
            this.chart2.Series[0].Points.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //this.chart1.Series[0].Points.AddXY(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString());
                this.chart2.Series[0].Points.AddXY(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString());
                if (dt.Rows[i][1].ToString().Equals(datadate))
                {
                    if (double.Parse(dt.Rows[i][2].ToString())>upper_limitation||double.Parse(dt.Rows[i][2].ToString())<lower_limitation)
                    {
                        this.chart2.Series[0].Points[i].MarkerColor = Color.Red;
                    }
                    else
                    {
                        this.chart2.Series[0].Points[i].MarkerColor = Color.Green;
                    }
                    this.chart2.Series[0].Points[i].MarkerSize = 10;
                }
            }
        }

        public void InitChart1()
        {
            this.chart1.Series.Clear();
            Series series = new Series();
            series.Color = Color.Blue;
            series.ChartType = SeriesChartType.Spline;
            series.IsVisibleInLegend = false;
            series.BorderWidth = 2;
            series.MarkerSize = 1;
            series.MarkerStyle = MarkerStyle.Circle;
            series.IsValueShownAsLabel = false;

            //chart1.ChartAreas["ChartArea1"].Position.X = 1;
            //chart1.ChartAreas["ChartArea1"].Position.Y = 15;
            //chart1.ChartAreas["ChartArea1"].Position.Height = 97;
            //chart1.ChartAreas["ChartArea1"].Position.Width = 97;

            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.X = 5;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Y = 3;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Width = 95;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Height = 99;

            chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.Black;
            chart1.ChartAreas["ChartArea1"].AxisY.LineWidth = 1;

            chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.Black;
            chart1.ChartAreas["ChartArea1"].AxisX.LineWidth = 1;

            chart1.Visible = true;
            chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.IsEndLabelVisible = true;
            chart1.ChartAreas["ChartArea1"].AxisX.Enabled = AxisEnabled.True;
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = true;

            chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;

            this.chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            this.chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            this.chart1.Series.Add(series);
        }

        public void InitChart2()
        {
            this.chart2.Series.Clear();
            Series series = new Series();
            series.Color = Color.Blue;
            series.ChartType = SeriesChartType.Spline;
            series.IsVisibleInLegend = false;
            series.BorderWidth = 2;
            series.MarkerSize = 1;
            series.MarkerStyle = MarkerStyle.Circle;
            series.IsValueShownAsLabel = false;

            //chart1.ChartAreas["ChartArea1"].Position.X = 1;
            //chart1.ChartAreas["ChartArea1"].Position.Y = 15;
            //chart1.ChartAreas["ChartArea1"].Position.Height = 97;
            //chart1.ChartAreas["ChartArea1"].Position.Width = 97;

            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.X = 5;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Y = 3;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Width = 95;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Height = 99;

            chart2.ChartAreas["ChartArea1"].AxisY.LineColor = Color.Black;
            chart2.ChartAreas["ChartArea1"].AxisY.LineWidth = 1;

            chart2.ChartAreas["ChartArea1"].AxisX.LineColor = Color.Black;
            chart2.ChartAreas["ChartArea1"].AxisX.LineWidth = 1;

            chart2.Visible = true;
            chart2.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            chart2.ChartAreas["ChartArea1"].AxisX.LabelStyle.IsEndLabelVisible = true;
            chart2.ChartAreas["ChartArea1"].AxisX.Enabled = AxisEnabled.True;
            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = true;
            

            chart2.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = true;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;

            this.chart2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            this.chart2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            this.chart2.Series.Add(series);
        }

        /// <summary>
        /// 根据位置和时刻混合查询数据
        /// 找到同一个位置前后各defaultpoints个数据，共2*defaultpoints+1个数据
        /// 找到同一个时刻前后各defaultpoints个数据，共2*defaultpoints+1个数据
        /// </summary>
        /// <param name="direcorypath">数据文件地址</param>
        /// <param name="datadate">要查询的时间</param>
        /// <param name="location">要查询的位置</param>
        /// <returns></returns>
        public List<DataTable> SearchDataByDatatimeAndLocation()
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            int defaultpoints = 5;
            string defaultpoints_temp = conf.getConfigSetting("Default_Points_Line");
            if (!defaultpoints_temp.Equals(""))
            {
                defaultpoints = int.Parse(defaultpoints_temp);
            }
            List<DataTable> list_result = new List<DataTable>();
            string target_file = "";//用于存储找到的目标数据文件
            int target_file_num = 0;
            DirectoryInfo di = new DirectoryInfo(direcorypath);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件

            file_combined = OrderFileByNum(file_combined);
            //用于存储每一个数据文件的起止行
            List<int> start_num = new List<int>();
            List<int> end_num = new List<int>();

            #region 获取每一个数据文件的起止行
            int total_data_rows = int.Parse(total_rows_count);
            int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

            int start_row = 0;
            int end_row = 0;

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                {
                    start_row = i * file_data_rows;
                    end_row = (i + 1) * file_data_rows - 1;
                }
                else
                {
                    start_row = i * file_data_rows;
                    end_row = total_data_rows - 1;
                }
                start_num.Add(start_row);
                end_num.Add(end_row);
            }
            #endregion

            #region 判断位置点之前，之后数据的位置
            int start_location = int.Parse(location) - defaultpoints;
            if (start_location<0)
            {
                start_location = 0;
            }
            int end_location = int.Parse(location) + defaultpoints;
            //int.Parse(total_rows_count) - 1;
            if (end_location>int.Parse(total_rows_count) - 1)
            {
                end_location = int.Parse(total_rows_count) - 1;
            }
            #endregion

            this.textBox4.Text = start_location.ToString();
            this.textBox5.Text = end_location.ToString();


            #region 开始进行同一个位置纵向位置数据的获取
            DataTable dt_result_location = new DataTable();

            DataColumn datalocation = new DataColumn();
            datalocation.Caption = "位置";
            datalocation.ColumnName = "位置";

            DataColumn datatime = new DataColumn();
            datatime.Caption = "时间";
            datatime.ColumnName = "时间";

            DataColumn datavalue = new DataColumn();
            datavalue.Caption = "监测值";
            datavalue.ColumnName = "监测值";

            dt_result_location.Columns.Add(datalocation);
            dt_result_location.Columns.Add(datatime);
            dt_result_location.Columns.Add(datavalue);

            bool need_combin = false;

            #region 首先判断从start_location到end_location这些数据是在几个数据文件中

            for (int i = 0; i < start_num.Count; i++)
            {
                if (start_location >= start_num[i] && end_location <= end_num[i])//说明这些数据是在一个数据文件中，是最简单的情形
                {
                    target_file = file_combined[i].FullName;
                    target_file_num = i;
                    break;
                }

                if (start_location <= end_num[i] && end_location > end_num[i])//说明是在两个文件中，需要拼接
                {
                    need_combin = true;
                    target_file = file_combined[i].FullName;
                    target_file_num = i;
                    break;
                }
            }
            #endregion

            #region 开始获取数据

            if (need_combin)//需要合并，则首先是目标文件从start_location,然后是目标文件的下一个文件
            {
                DataTable data_dt_former = ExcelHelper.OpenCSV2(target_file, 1);//获取数据前半部分
                DataTable dt_temp = data_dt_former.Clone();

                DataRow[] dtRow_former = data_dt_former.Select("location>= '" + start_location + "' and  location <='" + end_num[target_file_num] + "' ");//根据查询条件，筛选出所有满足条件的行
                foreach (DataRow item in dtRow_former)//把满足条件的所有行数据填入数据表
                {
                    dt_temp.ImportRow(item);
                }

                DataTable data_dt_later = ExcelHelper.OpenCSV2(file_combined[target_file_num+1].FullName, 1);//获取数据前半部分

                DataRow[] dtRow_later = data_dt_later.Select("location >=' " + start_num[target_file_num + 1] + "'  and location <=' " + end_location + "' ");//根据查询条件，筛选出所有满足条件的行
                foreach (DataRow item in dtRow_later)//把满足条件的所有行数据填入数据表
                {
                    dt_temp.ImportRow(item);
                }

                for (int k = 0; k < dt_temp.Rows.Count; k++)
                {
                    for (int i = 1; i < dt_temp.Columns.Count; i++)
                    {
                        if (dt_temp.Columns[i].ColumnName.Equals(datadate))
                        {
                            List<string> list = new List<string>();
                            list.Add(dt_temp.Rows[k][0].ToString());//location信息
                            list.Add(dt_temp.Columns[i].ColumnName);
                            list.Add(dt_temp.Rows[k][i].ToString());//value信息

                            DataRow dr = dt_result_location.NewRow();
                            dr.ItemArray = list.ToArray();
                            dt_result_location.Rows.Add(dr);
                        }
                    }
                }
            }
            else
            {
                DataTable data_dt = ExcelHelper.OpenCSV2(target_file, 1);
                data_dt.Columns[0].DataType=typeof(int);

                DataRow[] dtRow = data_dt.Select("location >= '" + start_location + "' and location <= '" + end_location + "' ");//根据查询条件，筛选出所有满足条件的行

                DataTable dt_temp = data_dt.Clone();
                foreach (DataRow item in dtRow)//把满足条件的所有行数据填入数据表
                {
                    dt_temp.ImportRow(item);
                }
                for (int k = 0; k < dt_temp.Rows.Count; k++)
                {
                    for (int i = 1; i < dt_temp.Columns.Count; i++)
                    {
                        if (dt_temp.Columns[i].ColumnName.Equals(datadate))
                        {
                            List<string> list = new List<string>();
                            list.Add(dt_temp.Rows[k][0].ToString());//location信息
                            list.Add(dt_temp.Columns[i].ColumnName);
                            list.Add(dt_temp.Rows[k][i].ToString());//value信息

                            DataRow dr = dt_result_location.NewRow();
                            dr.ItemArray = list.ToArray();
                            dt_result_location.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion
  
            #endregion

            list_result.Add(dt_result_location);

            #region 开始同一位置，某一时刻前后横向数据的获取
            DataTable dt_result_date = new DataTable();

            DataColumn datalocation2 = new DataColumn();
            datalocation2.Caption = "位置";
            datalocation2.ColumnName = "位置";

            DataColumn datatime2 = new DataColumn();
            datatime2.Caption = "时间";
            datatime2.ColumnName = "时间";

            DataColumn datavalue2 = new DataColumn();
            datavalue2.Caption = "监测值";
            datavalue2.ColumnName = "监测值";

            dt_result_date.Columns.Add(datalocation2);
            dt_result_date.Columns.Add(datatime2);
            dt_result_date.Columns.Add(datavalue2);

            #region 判断要检索的位置位于第几个文件
            int int_location2 = int.Parse(location);
            for (int i = 0; i < start_num.Count; i++)
            {
                if (int_location2 >= start_num[i] && int_location2 <= end_num[i])
                {
                    target_file = file_combined[i].FullName;
                    break;
                }
            }
            #endregion
            DataTable data_dt2 = ExcelHelper.OpenCSV2(target_file, 1);

            DataRow[] dtRow2 = data_dt2.Select("location='" + location + "' ");//根据查询条件，筛选出所有满足条件的行

            DataTable dt_temp2 = data_dt2.Clone();
            foreach (DataRow item in dtRow2)//把满足条件的所有行数据填入数据表
            {
                dt_temp2.ImportRow(item);
            }

            int date_index = 0;
            int start_cols = 1;
            int end_cols = dt_temp2.Columns.Count; ;

            #region 获取数据的起止列
            for (int i = 1; i < dt_temp2.Columns.Count; i++)
            {
                if (dt_temp2.Columns[i].ColumnName.Equals(datadate))
                {
                    date_index = i;
                    start_cols = date_index - defaultpoints;
                    if (start_cols < 1)
                    {
                        start_cols = 1;
                    }
                    end_cols = date_index + defaultpoints;
                    if (end_cols >= dt_temp2.Columns.Count)
                    {
                        end_cols = dt_temp2.Columns.Count;
                    }
                    break;
                }
            }
            #endregion

            this.textBox6.Text = dt_temp2.Columns[start_cols].ColumnName;
            this.textBox7.Text = dt_temp2.Columns[end_cols].ColumnName;

            List<double> list_value_for_result = new List<double>();//用于存储从开始列到查询列（即过去default_points的数据），用于计算上线限值和IQR值来判断目标点是否异常

            for (int i = start_cols; i < end_cols; i++)
            {
                if (i<date_index)
                {
                    list_value_for_result.Add(double.Parse(dt_temp2.Rows[0][i].ToString()));
                }
                DataRow dr = dt_result_date.NewRow();

                List<string> list = new List<string>();
                list.Add(location);
                list.Add(dt_temp2.Columns[i].ColumnName);
                list.Add(dt_temp2.Rows[0][i].ToString());

                dr.ItemArray=list.ToArray();
                dt_result_date.Rows.Add(dr);
            }

            List<double> compare_resule = OrderDataByValue(list_value_for_result);

            this.textBox8.Text = compare_resule[1].ToString();
            this.textBox9.Text = compare_resule[0].ToString();
           
            lower_limitation = compare_resule[0];
            upper_limitation = compare_resule[1];

            #endregion

            list_result.Add(dt_result_date);
            return list_result;
        }

        /// <summary>
        /// 按照合并文件的序号对文件进行排序
        /// 目的是保证在数据拼接的过程中是按照序号先后顺序拼接的
        /// </summary>
        /// <param name="Original_files"></param>
        /// <returns></returns>
        public FileInfo[] OrderFileByNum(FileInfo[] Original_files)
        {
            //DateTime[] list_file_date = new DateTime[Original_files.Length];
            int[] list_file_num = new int[Original_files.Length];
            for (int i = 0; i < Original_files.Length; i++)
            {
                //20230112212900_CobinedData_2021-04-30-16-12-42_2021-05-24-20-33-15_Temperature_1
                string file_name = Original_files[i].Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[5].Split(new char[] { '.' })[0];//得到序号：1
                list_file_num[i] = int.Parse(file_name);
            }
            for (int i = 0; i < list_file_num.Length - 1; i++)
            {
                for (int j = 0; j < list_file_num.Length - 1 - i; j++)
                {
                    if (list_file_num[j] > list_file_num[j + 1])
                    {
                        int temp_num = list_file_num[j];
                        list_file_num[j] = list_file_num[j + 1];
                        list_file_num[j + 1] = temp_num;

                        FileInfo temp_fi = Original_files[j];
                        Original_files[j] = Original_files[j + 1];
                        Original_files[j + 1] = temp_fi;
                    }
                }
            }
            return Original_files;
        }

        public List<double> GetMaxMinData(List<double> data) 
        {
            for (int i = 0; i < data.Count - 1; i++)
            {
                for (int j = 0; j < data.Count - 1 - i; j++)
                {
                    if (data[j] > data[j + 1])
                    {
                        double temp_num = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp_num;
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// 将DataTable指定列进行大小排序
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column_num"></param>
        /// <returns></returns>
        public List<double> ConvertDataTableToList(DataTable dt, int column_num) 
        {
            List<double> result = new List<double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result.Add(double.Parse(dt.Rows[i][column_num-1].ToString()));
            }
            return result;
        }

        public List<double> OrderDataByValue(List<double> data)
        {
            List<double> list = new List<double>();

            //冒泡排序
            for (int i = 0; i < data.Count-1; i++)
            {
                for (int j = 0; j <  data.Count-1-i; j++)
                {
                    if (data[j] > data[j + 1])
                    {
                        double temp_data = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp_data;
                    }
                }
            }
            ConfReadAndModify conf = new ConfReadAndModify();
            double IQR_Factor = 1.5;
            string IQR_Factor_conf=conf.getConfigSetting("IQR_Factor").Trim();
            if (!IQR_Factor_conf.Equals(""))
            {
                IQR_Factor = double.Parse(IQR_Factor_conf);
            }
            int q1 = int.Parse(Math.Floor(0.25*data.Count).ToString());//  1/4位
            int q3 = int.Parse(Math.Floor(0.75 * data.Count).ToString());//  3/4位
            double iqr = data[q3]-data[q1];
            list.Add(data[q1] - IQR_Factor * iqr); //下限值
            list.Add(data[q3] + IQR_Factor * iqr);//上限值
            return list;
        }
    }
}
