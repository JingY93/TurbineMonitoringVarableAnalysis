using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.OleDb;

namespace TurbineMonitoringVarableAnalysis.CommonCode
{
    public class ExcelHelper
    {
        /// <summary>
        /// OleDb读取xls/xlsx
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static DataTable GetExcelToDataTableByOleDb(string filepath, string sheetname)
        {
            string strCon = "";
            if (filepath.IndexOf(".xlsx") != -1)
                strCon = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            else if (filepath.IndexOf(".xls") != -1)
                strCon = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";

            //string strCom = " SELECT * FROM [" + sheetname + "] where time is not null and convert(datetime, time,120) >=convert(datetime, '2016-06-01 00:00:00') and convert(datetime, time,120) <=convert(datetime, '2016-06-02 23:59:59')";
            string strCom = " SELECT * FROM [" + sheetname + "] ";
            //OleDbConnection conn = new OleDbConnection(strCon);
            //conn.Open();
            //DataTable sheetNames = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" }); 
            //string strCom = " SELECT * FROM [" + sheetname + "] ";
            DataTable dt_temp = new DataTable();
            using (OleDbConnection myConn = new OleDbConnection(strCon))
            using (OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn))
            {
                myConn.Open();
                myCommand.Fill(dt_temp);
            }

            return dt_temp;
        }

        public static string exportDataTableToExcel(DataTable dataTable, string fileName, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application excel;

            Microsoft.Office.Interop.Excel._Workbook workBook;

            Microsoft.Office.Interop.Excel._Worksheet workSheet;

            object misValue = System.Reflection.Missing.Value;

            excel = new Microsoft.Office.Interop.Excel.Application();

            workBook = excel.Workbooks.Add(misValue);

            workSheet = (Microsoft.Office.Interop.Excel._Worksheet)workBook.ActiveSheet;

            int rowIndex = 1;

            int colIndex = 0;

            //取得标题  
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;

                excel.Cells[1, colIndex] = col.ColumnName;
            }

            //取得表格中的数据  
            foreach (DataRow row in dataTable.Rows)
            {
                rowIndex++;

                colIndex = 0;

                foreach (DataColumn col in dataTable.Columns)
                {
                    colIndex++;

                    excel.Cells[rowIndex, colIndex] =

                         row[col.ColumnName].ToString().Trim();

                    //设置表格内容居中对齐  
                    //workSheet.get_Range(excel.Cells[rowIndex, colIndex],

                    //  excel.Cells[rowIndex, colIndex]).HorizontalAlignment =

                    //  Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                }
            }

            excel.Visible = false;

            string saveFile = filePath + fileName + ".csv";

            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);//嘿嘿，这样不好，偷偷把原来的删掉了，暂时这样写，项目中不可以
            }

            workBook.SaveAs(saveFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue,

                misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,

                misValue, misValue, misValue, misValue, misValue);

            dataTable = null;

            workBook.Close(true, misValue, misValue);

            excel.Quit();

            PublicMethod.Kill(excel);//调用kill当前excel进程  

            releaseObject(workSheet);

            releaseObject(workBook);

            releaseObject(excel);

            if (!File.Exists(saveFile))
            {
                return null;
            }
            return saveFile;
        }
        /// <summary>
        /// filepath csv文件路径
        /// num 从第几行开始抓取数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static DataTable OpenCSV(string filePath, int num)//从csv读取数据返回table  
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//  
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("TurbineID");
            dt.Columns.Add(dc1);
            DataColumn dc2 = new DataColumn("AnalysisVar");
            dt.Columns.Add(dc2);
            DataColumn dc3 = new DataColumn("ConditionVar");
            dt.Columns.Add(dc3);
            DataColumn dc4 = new DataColumn("ConditionRange");
            dt.Columns.Add(dc4);
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录  
            string strLine = "";
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            //逐行读取CSV中的数据  
            int rows = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                rows++;
                if (rows > num)
                {
                    aryLine = strLine.Split(',');
                    if (aryLine.Length < 4)//至少需要两列
                    {
                        break;
                    }
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < 4; j++)
                    {
                        dr[j] = aryLine[j].Replace('"', ' ').Trim();
                    }
                    dt.Rows.Add(dr);
                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        /// <summary>
        /// filepath csv文件路径
        /// num 从第几行开始抓取数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static DataTable OpenCSV2(string filePath, int num)//从csv读取数据返回table  
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//  
            DataTable dt = new DataTable();

            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录  
            string strLine = "";
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            //逐行读取CSV中的数据  
            int rows = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (rows==0)//第一行，先把表结构建立起来
                {
                     aryLine = strLine.Split(',');
                     for (int i = 0; i < aryLine.Length; i++)
                     {
                         DataColumn dc = new DataColumn();
                         if (i==0)
                         {
                             dc.DataType=typeof(int);
                         }
                         dc.Caption = aryLine[i];
                         dc.ColumnName = aryLine[i]; ;
                         dt.Columns.Add(dc);
                     }

                }
                rows++;
                if (rows > num)
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < aryLine.Length; j++)
                    {
                        dr[j] = aryLine[j].Replace('"', ' ').Trim();
                    }
                    dt.Rows.Add(dr);
                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        /// <summary>
        /// filepath csv文件路径
        /// num 从第几行开始抓取数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static DataTable OpenCSV3(string filePath, int num)//从csv读取数据返回table  
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//  
            DataTable dt = new DataTable();

            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录  
            string strLine = "";
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            //逐行读取CSV中的数据  
            int rows = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (rows == 0)//第一行，先把表结构建立起来
                {
                    aryLine = strLine.Split(',');
                    for (int i = 0; i < aryLine.Length; i++)
                    {
                        DataColumn dc = new DataColumn();
                        dc.Caption = aryLine[i];
                        dc.ColumnName = aryLine[i]; ;
                        dt.Columns.Add(dc);
                    }

                }
                rows++;
                if (rows > num)
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < aryLine.Length; j++)
                    {
                        dr[j] = aryLine[j].Replace('"', ' ').Trim();
                    }
                    dt.Rows.Add(dr);
                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型  
        /// <param name="FILE_NAME">文件路径</param>  
        /// <returns>文件的编码类型</returns>  

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型  
        /// <param name="fs">文件流</param>  
        /// <returns>文件的编码类型</returns>  
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM  
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式  
        /// <param name="data"></param>  
        /// <returns></returns>  
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数  
            byte curByte; //当前分析的字节.  
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前  
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　  
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1  
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        /// <summary>
        /// 释放COM组件对象
        /// </summary>
        /// <param name="obj"></param>
        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// 关闭进程的内部类
        /// </summary>
        public class PublicMethod
        {
            [DllImport("User32.dll", CharSet = CharSet.Auto)]

            public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

            public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
            {
                //如果外层没有try catch方法这个地方需要抛异常。
                IntPtr t = new IntPtr(excel.Hwnd);

                int k = 0;

                GetWindowThreadProcessId(t, out k);

                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);

                p.Kill();
            }

        }

        public static string SaveCSV(DataTable dt, string fileName, string filePath)
        {
            string saveFile = filePath + "\\" + fileName + ".csv";
            FileStream fs = new FileStream(saveFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString().Replace(",",";");
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            return saveFile;
        }

        public static string AppendCSV(DataTable dt, string filePath)
        {
            string saveFile = filePath;
            FileStream fs = new FileStream(saveFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            ////写出列名称
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    data += dt.Columns[i].ColumnName.ToString();
            //    if (i < dt.Columns.Count - 1)
            //    {
            //        data += ",";
            //    }
            //}
            //sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            return saveFile;
        }

    }
}
