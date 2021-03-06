﻿using AccessBattleAI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBattleTests
{
    [TestClass]
    public class NeuralNetworkTest
    {
        #region Init and Cleanup
        [TestInitialize]
        public void Init()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
        }
        #endregion

        CultureInfo _culture = CultureInfo.InvariantCulture;

        [TestMethod]
        public void CheckOutputs()
        {
            var net = new NeuralNetwork(3, 4, 2, 0);

            net.ComputeOutputs(false);
            PrintNet(net); // Should be 0.5,0.5 at the beginning

            for (int i = 0; i < 10; ++i)
            {
                net.Mutate(0.00001);
                net.ComputeOutputs(false);
                PrintNet(net);
            }
        }

        // Output can be found in the results of MS Test Explorer when selecting the result.
        // There should be a link called 'Output' at the bottom.
        void PrintNet(NeuralNetwork net)
        {
            Console.WriteLine("Neural Network Output: " + net.Outputs[0].ToString(_culture) + ", " + net.Outputs[1].ToString(_culture));
        }

        [TestMethod]
        public void SaveFileTest()
        {
            var net = new NeuralNetwork(3, 2, 2, 1);
            net.Mutate(0.00001);

            Assert.IsTrue(net.SaveAsFile("tmpNet.txt"));
            var net2 = NeuralNetwork.ReadFromFile("tmpNet.txt");
            System.IO.File.Delete("tmpNet.txt");

            Assert.IsNotNull(net2);
        }
    }
}
