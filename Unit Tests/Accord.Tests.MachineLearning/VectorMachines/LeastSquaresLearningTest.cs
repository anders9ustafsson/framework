﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using Accord.Statistics.Analysis;

    [TestFixture]
    public class LeastSquaresLearningTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [Test]
        public void LearnTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);

            // Create the Least Squares Support Vector Machine teacher
            LeastSquaresLearning learn = new LeastSquaresLearning(machine, inputs, xor);
            learn.Complexity = 10;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void LearnTest2()
        {
            double[][] inputs = yinyang.Submatrix(null, 0, 1).ToJagged();
            int[] outputs = yinyang.GetColumn(2).ToInt32();

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(3), inputs[0].Length);

            // Create the Least Squares Support Vector Machine teacher
            LeastSquaresLearning learn = new LeastSquaresLearning(machine, inputs, outputs);
            learn.Complexity = 1 / 0.1;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(outputs[i]), System.Math.Sign(output[i]));
        }


        [Test]
        [Ignore("Large")] // Usually takes 6 mins
        public void LargeLearningTest1()
        {
            // Create large input vectors

            int rows = 1000;
            int dimension = 10000;

            double[][] inputs = new double[rows][];
            int[] outputs = new int[rows];

            Random rnd = new Random();

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new double[dimension];

                if (i > rows / 2)
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble();
                    outputs[i] = -1;
                }
                else
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble() * 4.21 + 5;
                    outputs[i] = +1;
                }
            }

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Polynomial(2), dimension);

            LeastSquaresLearning smo = new LeastSquaresLearning(svm, inputs, outputs);


            double error = smo.Run();

            Assert.AreEqual(0, error);
        }

        [Test]
        public void LeastSquaresConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or =
            {
                0,
                0,
                0,
                +1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            var machine = new SupportVectorMachine(inputs[0].Length);

            var learn = new LeastSquaresLearning(machine, inputs, or);

            double error = learn.Run();
            Assert.AreEqual(0, error);

            {
                int[] iout = new int[inputs.Length];
                machine.ToMulticlass().Decide(inputs, iout);
                for (int i = 0; i < iout.Length; i++)
                    Assert.AreEqual(or[i], iout[i]);
            }
            {
                double[] dout = new double[inputs.Length];
                machine.ToMulticlass().Decide(inputs, dout);
                for (int i = 0; i < dout.Length; i++)
                    Assert.AreEqual(or[i], dout[i]);
            }
            {
                bool[] bout = new bool[inputs.Length];
                machine.Decide(inputs, bout);
                Assert.IsFalse(bout[0]);
                Assert.IsFalse(bout[1]);
                Assert.IsFalse(bout[2]);
                Assert.IsTrue(bout[3]);
            }
            {
                int[][] iiout = Jagged.Create<int>(inputs.Length, 2);
                machine.ToMulticlass().Decide(inputs, iiout);
                for (int i = 0; i < iiout.Length; i++)
                {
                    Assert.AreEqual(or[i], iiout[i][0]);
                    Assert.AreEqual(or[i], iiout[i][1] == 1 ? 0 : 1);
                }
            }
            {
                bool[][] bbout = Jagged.Create<bool>(inputs.Length, 2);
                machine.ToMulticlass().Decide(inputs, bbout);
                for (int i = 0; i < bbout.Length; i++)
                {
                    Assert.AreEqual(or[i], bbout[i][0] ? 1 : 0);
                    Assert.AreEqual(or[i], bbout[i][1] ? 0 : 1);
                }
            }
        }



        public static double[,] yinyang =
        {
            #region Yin Yang
            { -0.876847428, 1.996318824, -1 },
            { -0.748759325, 1.997248514, -1 },
            { -0.635574695, 1.978046579, -1 },
            { -0.513769071, 1.973224777, -1 },
            { -0.382577547, 1.955077224, -1 },
            { -0.275144211, 1.923813789, -1 },
            { -0.156802752, 1.949219695, -1 },
            { -0.046002059, 1.895342542, -1 },
            { 0.084152257, 1.873104082, -1 },
            { 0.192063131, 1.868157532, -1 },
            { 0.238547032, 1.811664165, -1 },
            { 0.381412694, 1.830869925, -1 },
            { 0.431182119, 1.755312479, -1 },
            { 0.562589082, 1.725444806, -1 },
            { 0.553294269, 1.689047886, -1 },
            { 0.730976261, 1.610522064, -1 },
            { 0.722164981, 1.633112952, -1 },
            { 0.861069302, 1.562450197, -1 },
            { 0.825107945, 1.435846225, -1 },
            { 0.825261132, 1.456391196, -1 },
            { 0.948721626, 1.393367552, -1 },
            { 1.001705278, 1.275768447, -1 },
            { 0.966788667, 1.321375233, -1 },
            { 1.030828944, 1.228437023, -1 },
            { 1.083195636, 1.143011589, -1 },
            { 0.920876422, 1.037854388, -1 },
            { 0.994518277, 1.064971023, -1 },
            { 0.954169422, 0.938084211, -1 },
            { 0.903586083, 0.985255341, -1 },
            { 0.877869854, 0.729143525, -1 },
            { 0.866594018, 0.75025734, -1 },
            { 0.757278389, 0.638917822, -1 },
            { 0.655489515, 0.670717406, -1 },
            { 0.687639626, 0.511655563, -1 },
            { 0.656365078, 0.638542346, -1 },
            { 0.491775914, 0.401874802, -1 },
            { 0.35504489, 0.38963967, -1 },
            { 0.275616568, 0.182958126, -1 },
            { 0.338471037, 0.102347682, -1 },
            { 0.103918095, 0.152960961, -1 },
            { 0.238473941, -0.070899965, -1 },
            { -0.00657754, 0.168107931, -1 },
            { -0.091307058, -0.032174399, -1 },
            { -0.290772034, -0.345025689, -1 },
            { -0.287555253, -0.397984323, -1 },
            { -0.363424618, -0.365636808, -1 },
            { -0.544071691, -0.512970644, -1 },
            { -0.7098968, -0.54654921, -1 },
            { -1.007857216, -0.811837224, -1 },
            { -0.932787122, -0.687973276, -1 },
            { -0.123987649, -1.547976483, 1 },
            { -0.247236701, -1.546629461, 1 },
            { -0.369357682, -1.533968755, 1 },
            { -0.497892178, -1.525597952, 1 },
            { -0.606998699, -1.518386229, 1 },
            { -0.751556976, -1.46427032, 1 },
            { -0.858848619, -1.464142289, 1 },
            { -0.957834238, -1.454165888, 1 },
            { -1.061602698, -1.444783216, 1 },
            { -1.169634343, -1.426033507, 1 },
            { -1.272115895, -1.408678817, 1 },
            { -1.380383293, -1.345651442, 1 },
            { -1.480866574, -1.279955202, 1 },
            { -1.548927664, -1.223262541, 1 },
            { -1.597886819, -1.227115936, 1 },
            { -1.686711497, -1.141898276, 1 },
            { -1.812689051, -1.14805053, 1 },
            { -1.809841336, -1.083347602, 1 },
            { -1.938850711, -1.019723742, 1 },
            { -1.974552679, -0.970515422, 1 },
            { -1.953184359, -0.88363121, 1 },
            { -1.98749965, -0.861879772, 1 },
            { -2.04215554, -0.797813815, 1 },
            { -1.984185734, -0.826986835, 1 },
            { -2.063307605, -0.749495213, 1 },
            { -1.964274134, -0.653639779, 1 },
            { -2.020258155, -0.530431615, 1 },
            { -1.946081996, -0.514425683, 1 },
            { -1.934356006, -0.435380423, 1 },
            { -1.827017658, -0.425058004, 1 },
            { -1.788385889, -0.312443513, 1 },
            { -1.800874033, -0.237312969, 1 },
            { -1.784225126, 0.013987951, 1 },
            { -1.682828321, -0.063911465, 1 },
            { -1.754042471, -0.075520653, 1 },
            { -1.5680733, 0.110795036, 1 },
            { -1.438333268, 0.170230561, 1 },
            { -1.356614661, 0.163613841, 1 },
            { -1.336362397, 0.334537756, 1 },
            { -1.296677607, 0.316006907, 1 },
            { -1.109908857, 0.474036646, 1 },
            { -0.845929174, 0.485303884, 1 },
            { -0.855794711, 0.395603118, 1 },
            { -0.68479255, 0.671166245, 1 },
            { -0.514222252, 0.652065554, 1 },
            { -0.387612557, 0.700858902, 1 },
            { -0.51939719, 1.025735335, 1 },
            { -0.228760025, 0.93490314, 1 },
            { -0.293782477, 1.008861678, 1 },
            { 0.013431012, 1.082021525, 1 },
            #endregion
        };
    }
}
