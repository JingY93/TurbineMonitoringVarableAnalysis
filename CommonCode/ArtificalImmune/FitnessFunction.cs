using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TurbineMonitoringVarableAnalysis.CommonCode.ArtificalImmune
{
    /// <summary>
    /// 适应度函数
    /// </summary>
    public class FitnessFunction
    {
        public DataTable TrainningData;
        public FitnessFunction(DataTable dt) 
        {
            TrainningData = dt;
        }

        public FitnessFunction()
        {
        }  

        /// <summary>
        /// 一个用于测试的适应度函数
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double evaluateFunction(double[] values)
        {
            double x = values[0];
            double y = values[1];
            double n = 9;
            double fitness;

            fitness = Math.Pow(15 * x * y * (1 - x) * (1 - y) * Math.Sin(n * Math.PI * x) * Math.Sin(n * Math.PI * y), 2);
            return fitness;
        }

        /// <summary>
        /// 基于训练数据评价抗体适应度，其基本思想为：
        /// 抗体到所有训练数据的距离应该最小
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double evaluateFunction_dt(double[] values)
        {
            double fitness = 0.0 ;

            if (TrainningData!=null&&TrainningData.Rows.Count>0)
            {
                for (int i = 0; i < TrainningData.Rows.Count; i++)
                {
                    double fitness_temp = 0.0;
                    for (int j = 0; j < TrainningData.Columns.Count; j++)
                    {
                        fitness_temp += Math.Pow(values[j] - double.Parse(TrainningData.Rows[i][j].ToString()), 2);
                    }
                    fitness_temp = Math.Sqrt(fitness_temp);
                    fitness_temp=1/(1+fitness_temp);
                    fitness = fitness + fitness_temp;
                }
            }
            
            return fitness;
        }
    }
}
