using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using NUnit.Framework;

namespace XPathOptimizer.Test
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestExec()
        {
            var exe = "XPathOptimizer.Main.exe";

            var p = "/body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]";
            var xml = @"XmlFile.xml";

            var proc = Process.Start(new ProcessStartInfo(exe, string.Format("/p {0} /x {1}", p, xml))
            {
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            });

            proc.WaitForExit();

            Trace.WriteLine("Output: " + proc.StandardOutput.ReadToEnd());
            Trace.WriteLine("Error: " + proc.StandardError.ReadToEnd());
        }

        [TestMethod]
        public void TestMain()
        {
            var p = "/body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]";
            var xml = @"XmlFile.xml";
            
            XPathOptimizer.Main.Program.Main("-p=" + p, "-x=" + xml);
        }

        [TestMethod]
        public void TestHelp()
        {
            XPathOptimizer.Main.Program.Main("-h");
        }
    }
}