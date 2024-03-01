using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace TurbineMonitoringVarableAnalysis.CommonCode.ArtificalImmune
{
    public class IAController
    {
        public ArrayList AntibodyArray; //用于存储抗体的arrayList
        private int antibodyNumber; // 抗体种群数量
        private int cloneNumber; // k克隆操作中，每个抗体被克隆的次数
        private int maxGens; //  最大进化代数
        private double mutationFactor; //变异率
        private double removeThreshold;//从抗体种群中删除较差抗体的阈值
        private double diversity;//为保证抗体多样性而增加当前种群的比例
        private double clonalSelectionThreshold;//克隆选择平均误差阈值
        private double lowerBoundary; //寻优的最低边界
        private double upperBoundary; //寻优的最高边界

        public int mode_dimension;//抗体的维数
        public DataTable trainingdata;//用于训练的数据表

        /// <summary>
        /// 该构造函数适用于目标很清晰的函数求解问题，
        /// </summary>
        /// <param name="dimension">抗体的维数，也就是目标函数的参数个数</param>
        public IAController(int dimension)
        {
            //所有参数初始化
            AntibodyArray = new ArrayList();
            antibodyNumber = 50;
            cloneNumber = 30;
            maxGens = 600;
            mutationFactor = 80;
            removeThreshold = 0.2;
            clonalSelectionThreshold = 0.01;
            diversity = 0.5;
            lowerBoundary = 0.0;
            upperBoundary = 1.0;
            mode_dimension = dimension;

            #region 读取配置参数
            ConfReadAndModify conf = new ConfReadAndModify();
            string para1 = conf.getConfigSetting("antibodyNumber");
            string para2 = conf.getConfigSetting("cloneNumber");
            string para3 = conf.getConfigSetting("maxGens");
            string para4 = conf.getConfigSetting("mutationFactor");
            string para5 = conf.getConfigSetting("removeThreshold");
            string para6 = conf.getConfigSetting("clonalSelectionThreshold");
            string para7 = conf.getConfigSetting("diversity");
            string para8 = conf.getConfigSetting("lowerBoundary");
            string para9 = conf.getConfigSetting("upperBoundary");
            if (!para1.Equals(""))
            {
                antibodyNumber = int.Parse(para1);
            }
            if (!para2.Equals(""))
            {
                cloneNumber = int.Parse(para2);
            }
            if (!para3.Equals(""))
            {
                maxGens = int.Parse(para3);
            }
            if (!para4.Equals(""))
            {
                mutationFactor = int.Parse(para4);
            }
            if (!para5.Equals(""))
            {
                removeThreshold = double.Parse(para5);
            }
            if (!para6.Equals(""))
            {
                clonalSelectionThreshold = double.Parse(para6);
            }
            if (!para7.Equals(""))
            {
                diversity = double.Parse(para7);
            }
            if (!para8.Equals(""))
            {
                lowerBoundary = double.Parse(para8);
            }
            if (!para9.Equals(""))
            {
                upperBoundary = double.Parse(para9);
            }
            #endregion
        }

        /// <summary>
        /// 该构造函数适用于类似服役质量模式中心训练的任务
        /// </summary>
        /// <param name="dimension">数据维数，也就是参数个数，抗体的维数</param>
        /// <param name="dt">用于训练的数据集合，行为时间，列为数据</param>
        public IAController(int dimension, DataTable dt)
        {
            //所有参数初始化
            AntibodyArray = new ArrayList();
            antibodyNumber = 50;
            cloneNumber =30;
            maxGens = 600;
            mutationFactor = 80;
            removeThreshold = 0.2;
            clonalSelectionThreshold = 0.01;
            diversity = 0.5;
            lowerBoundary = 0.0;
            upperBoundary = 1.0;

            mode_dimension = dimension;
            trainingdata = dt;

            #region 读取配置参数
            ConfReadAndModify conf = new ConfReadAndModify();
            string para1 = conf.getConfigSetting("antibodyNumber");
            string para2 = conf.getConfigSetting("cloneNumber");
            string para3 = conf.getConfigSetting("maxGens");
            string para4 = conf.getConfigSetting("mutationFactor");
            string para5 = conf.getConfigSetting("removeThreshold");
            string para6 = conf.getConfigSetting("clonalSelectionThreshold");
            string para7 = conf.getConfigSetting("diversity");
            string para8 = conf.getConfigSetting("lowerBoundary");
            string para9 = conf.getConfigSetting("upperBoundary");
            if (!para1.Equals(""))
            {
                antibodyNumber = int.Parse(para1);
            }
            if (!para2.Equals(""))
            {
                cloneNumber = int.Parse(para2);
            }
            if (!para3.Equals(""))
            {
                maxGens = int.Parse(para3);
            }
            if (!para4.Equals(""))
            {
                mutationFactor = int.Parse(para4);
            }
            if (!para5.Equals(""))
            {
                removeThreshold = double.Parse(para5);
            }
            if (!para6.Equals(""))
            {
                clonalSelectionThreshold = double.Parse(para6);
            }
            if (!para7.Equals(""))
            {
                diversity = double.Parse(para7);
            }
            if (!para8.Equals(""))
            {
                lowerBoundary = double.Parse(para8);
            }
            if (!para9.Equals(""))
            {
                upperBoundary = double.Parse(para9);
            }
            #endregion
        }

        /// <summary>
        /// 优化过程
        /// </summary>
        /// <param name="genLabel"></param>
        public void GoOptimise()
        {
            //生成初始抗体种群
            CreateAntibodies(antibodyNumber);

            //进化过程，逐代进化
            for (int g = 0; g < maxGens; g++)
            {
                //克隆选择
                ClonalSelection();

                //计算亲和力
                CalcAffinityInteractions();

                //产生新的抗体，增加多样性，新生成的数量是抗体种群数量乘以比例
                CreateAntibodies((int)(Math.Round(AntibodyArray.Count * diversity)));
            }
        }

        /// <summary>
        /// 生产抗体
        /// </summary>
        /// <param name="numOfCells">种群数量</param>
        public void CreateAntibodies(int numOfCells)
        {
            Antibody c;
            for (int i = 0; i < numOfCells; i++)
            {
                c = new Antibody(mutationFactor, lowerBoundary, upperBoundary,mode_dimension,trainingdata);
                c.evaluate();
                AntibodyArray.Add(c);
            }
        }

        /// <summary>
        /// 克隆选择函数
        /// </summary>
        private void ClonalSelection()
        {
            Boolean goahead;
            double averageFitnessBefore = 0.0; //克隆选择之前的抗体种群平均适应度
            double averageFitnessAfter; //克隆选择之后的抗体种群平均适应度
            double fitnessSum; //系统中所有抗体适应度的和

            do
            {
                Evaluatecells();
                CloneCells();
                fitnessSum = 0;

                for (int i = 0; i < AntibodyArray.Count; i++)
                {
                    Antibody c = (Antibody)AntibodyArray[i];
                    fitnessSum = fitnessSum + c.Fitness;
                }

                averageFitnessAfter = fitnessSum / AntibodyArray.Count;

                if ((averageFitnessAfter - averageFitnessBefore) < clonalSelectionThreshold)
                {
                    goahead = false;
                }
                else
                {
                    goahead = true;
                    averageFitnessBefore = averageFitnessAfter;
                }
            } while (goahead);

        }

        /// <summary>
        /// 抗体评估函数
        /// </summary>
        private void Evaluatecells()
        {
            double lowest = 0;
            double highest = 0;
            Boolean starting = true;

            foreach (Antibody c in this.AntibodyArray)
            {
                c.evaluate(); 

                if (starting)
                {
                    lowest = c.Fitness;
                    highest = c.Fitness;
                    starting = false;
                }
                else
                {
                    if (c.Fitness > highest)
                    {
                        highest = c.Fitness;
                    }

                    if (c.Fitness < lowest) 
                    {
                        lowest = c.Fitness; 
                    }
                }
            }

            foreach (Antibody c in this.AntibodyArray)
            {
                c.calcNormalisedFitness(lowest, highest);
            }
        }


        /// <summary>
        /// 克隆函数
        /// </summary>
        private void CloneCells()
        {
            Antibody currentAntibody;

            Antibody[] clones = new Antibody[cloneNumber]; //用于存放克隆的抗体数组

            int best;

            for (int i = 0; i < AntibodyArray.Count; i++)
            {
                currentAntibody = (Antibody)AntibodyArray[i];
                best = 0;

                for (int j = 0; j < cloneNumber; j++)
                {
                    clones[j] = (Antibody)currentAntibody.Clone();

                    clones[j].Mutate(); //变异
                    clones[j].evaluate();// 评估

                    if (clones[j].Fitness > clones[best].Fitness)
                    {
                        best = j;
                    }
                }

                //将好的抗体掺入抗体库，删除不好的抗体
                if (clones[best].Fitness > currentAntibody.Fitness)
                {
                    AntibodyArray.Insert(i, clones[best]);
                    AntibodyArray.RemoveAt(i + 1);
                }
            }
        }

        /// <summary>
        /// 计算所有抗体之间的亲和度
        /// </summary>
        private void CalcAffinityInteractions()
        {
            //初始化亲和度数组，形式为一个方阵
            double[,] affinities = new double[AntibodyArray.Count, AntibodyArray.Count];

            //通过计算所有抗体之间的亲和度，初始化亲和度数组
            for (int i = 0; i < AntibodyArray.Count; i++)
            {
                for (int j = 0; j < AntibodyArray.Count; j++)
                {
                    if (i > j)
                    {
                        affinities[i, j] = 0;
                    }
                    else
                    {
                        Antibody ci = (Antibody)AntibodyArray[i];
                        Antibody cj = (Antibody)AntibodyArray[j];
                        affinities[i, j] = ci.findAffinity(cj);
                    }
                }
            }
            Antibody[] tempAntibodyArray = new Antibody[AntibodyArray.Count];

            for (int k = 0; k < AntibodyArray.Count; k++)
            {
                tempAntibodyArray[k] = (Antibody)AntibodyArray[k];
            }

            for (int i = 0; i < tempAntibodyArray.Length; i++)
            {
                for (int j = 0; j < tempAntibodyArray.Length; j++)
                {
                    if ((i < j) && (affinities[i, j] < removeThreshold))
                    {
                        if (tempAntibodyArray[i].Fitness < tempAntibodyArray[j].Fitness)
                        {
                            AntibodyArray.Remove(tempAntibodyArray[i]);
                        }
                        else
                        {
                            AntibodyArray.Remove(tempAntibodyArray[j]);
                        }
                    }
                }
            } 
        }
    }
}
