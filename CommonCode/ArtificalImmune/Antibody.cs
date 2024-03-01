using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TurbineMonitoringVarableAnalysis.CommonCode.ArtificalImmune
{
    /// <summary>
    /// 每一个抗体都是一个潜在的问题的姐，就是一个潜在的模式中心
    /// 抗体具有如下的属性
    /// 1. 一个随机产生的[x,y]
    /// 2. 适应度值
    /// 3. 每个值的上下限
    /// 4. 亲和度的变异率
    /// 具有如下能力
    /// 评估各自的适应度值
    /// 获取同其他抗体的亲和度（就是抗体之间的欧式距离值）
    /// 克隆能力
    /// 变异能力
    /// </summary>
    
    public class Antibody : System.ICloneable
    {

        private double[] xyvalues;
        public double[] XYvalues
        {
            get { return xyvalues; }
        }

        private double fitness;
        public double Fitness
        {
            get { return fitness; }
        }

        private double normalisedFitness;

        private double mutationfactor;
        private double lowerboundary;
        private double upperboundary;
        private static Random rand = new Random(1);

        public DataTable trainingdata;


        /// <summary>
        /// 抗体构造函数
        /// </summary>
        /// <param name="mutation">变异率</param>
        /// <param name="lowerlimit">下限</param>
        /// <param name="upperlimit">上限</param>
        public Antibody(double mutation, double lowerlimit, double upperlimit,int mode_dimension,DataTable dt)
        {
            this.mutationfactor = mutation;
            this.lowerboundary = lowerlimit;
            this.upperboundary = upperlimit;
            this.trainingdata = dt;
            xyvalues = new double[mode_dimension];

            for (int i = 0; i < xyvalues.Length; i++)
            {
                xyvalues[i] = lowerboundary + (upperboundary - lowerboundary) * rand.NextDouble();
            }
        }

        /// <summary>
        /// 抗体构造函数2
        /// </summary>
        /// <param name="copyAntibody">复制的抗体</param>
        public Antibody(Antibody copyAntibody)
        {
            this.mutationfactor = copyAntibody.mutationfactor;
            this.lowerboundary = copyAntibody.lowerboundary;
            this.upperboundary = copyAntibody.upperboundary;
            this.xyvalues = copyAntibody.xyvalues;
        }


        public object Clone()
        {
            Antibody myAntibody = this.MemberwiseClone() as Antibody;
            myAntibody.xyvalues = (double[])xyvalues.Clone();
            return myAntibody;
        }

        /// <summary>
        /// 抗体变异
        /// </summary>
        public void Mutate()
        {
            //mutationfactor = 100 则 1/100 =0.01
            double affinityMutator;
            affinityMutator = (1.0 / mutationfactor) * Math.Exp(-1 * normalisedFitness);

            for (int i = 0; i < xyvalues.Length; i++)
            {
                 xyvalues[i] = xyvalues[i] + affinityMutator * rand.NextDouble();
                 if (xyvalues[i] > upperboundary)
                 {
                      xyvalues[i] = upperboundary;
                 }

                 if (xyvalues[i] < lowerboundary)
                 {
                      xyvalues[i] = lowerboundary;
                 }
            } 
        }


        /// <summary>
        /// 计算和目标抗体之间的亲和度
        /// </summary>
        /// <param name="c">目标抗体</param>
        /// <returns>亲和度</returns>
        public double findAffinity(Antibody c)
        {
            double affinity = 0.0;

            double[] cXYvalues = c.XYvalues;
            double sum_temp = 0.0;
            for (int i = 0; i < cXYvalues.Length; i++)
            {
                double temp=Math.Pow((xyvalues[i] - cXYvalues[i]), 2);
                sum_temp = sum_temp + temp;
            }

            affinity = Math.Sqrt(sum_temp);

            return affinity;
        }

        /// <summary>
        /// 亲和度值得归一化
        /// </summary>
        /// <param name="lowfit">所有抗体的最小亲和度</param>
        /// <param name="highfit">所有抗体的最大亲和度</param>
        public void calcNormalisedFitness(double lowfit, double highfit)
        {
            normalisedFitness = (fitness - lowfit) / (highfit - lowfit);
        }

        /// <summary>
        /// 抗体评价
        /// </summary>
        public void evaluate()
        {
            FitnessFunction ff = new FitnessFunction(trainingdata);
            //数据训练用
            //fitness = ff.evaluateFunction_dt(xyvalues);

            //一般问题求解用
            fitness = ff.evaluateFunction(xyvalues);
        }
    }
}
