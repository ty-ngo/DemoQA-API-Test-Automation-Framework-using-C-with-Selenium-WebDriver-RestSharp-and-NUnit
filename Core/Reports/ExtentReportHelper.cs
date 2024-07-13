using System;
using System.IO;
using System.Text.RegularExpressions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace Core.Reports
{
    public class ExtentReportHelper
    {
        static AventStack.ExtentReports.ExtentReports ExtentManager;

        [ThreadStatic]
        public static ExtentTest ExtentTest;
        [ThreadStatic]
        public static ExtentTest Node;

        public static void InitializeReport(string reportPath, string hostName, string environment, string browser)
        {
            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(reportPath);
            ExtentManager = new AventStack.ExtentReports.ExtentReports();
            ExtentManager.AttachReporter(htmlReporter);
            ExtentManager.AddSystemInfo("Host Name", hostName);
            ExtentManager.AddSystemInfo("Environment", environment);
            ExtentManager.AddSystemInfo("Browser", browser);
            Console.WriteLine("Initialize report");
        }

        public static void Flush()
        {
            Console.WriteLine("before flush");
            ExtentManager.Flush();
            Console.WriteLine("after flush");
        }

        public static void CreateTest(string name)
        {
            ExtentTest = ExtentManager.CreateTest(name);
            Console.WriteLine("create test");
        }

        public static void CreateNode(string name)
        {
            Node = ExtentTest.CreateNode(name);
            Console.WriteLine("create node");
        }

        public static void LogTestStep(string step)
        {
            Node.Info(step);
        }

        public static void CreateTestResult(TestStatus status, string stacktrace, string className, string testName, IWebDriver driver = null)
        {
            Status logstatus;
            try
            {
                testName = SanitizeFileName(testName);

                switch (status)
                {
                    case TestStatus.Failed:
                        // logstatus = Status.Fail;
                        // var fileLocation = CaptureScreenshot(driver, className, testName);
                        // // Node.Fail($"#Test Name: {testName} #Status: {logstatus} {stacktrace}");
                        // // Node.AddScreenCaptureFromPath(fileLocation.Replace(Directory.GetCurrentDirectory(), "."));
                        // var mediaEntity = CaptureScreenShotAndAttachToExtendReport( fileLocation);
                        // Node.Fail($"#Test Name:  {testName}  #Status:  {logstatus} {stacktrace}", mediaEntity);
                        // Node.Fail("#Screenshot Below: " + Node.AddScreenCaptureFromPath(fileLocation));
                        // break;

                        logstatus = Status.Fail;
                        string fileLocation = null;
                        if (driver != null)
                        {
                            fileLocation = CaptureScreenshot(driver, className, testName);
                            var mediaEntity = CaptureScreenShotAndAttachToExtendReport(fileLocation);
                            Node.Fail($"#Test Name:  {testName}  #Status:  {logstatus} {stacktrace}", mediaEntity);
                            Node.Fail("#Screenshot Below: " + Node.AddScreenCaptureFromPath(fileLocation));
                        }
                        else
                        {
                            Node.Fail($"#Test Name:  {testName}  #Status:  {logstatus} {stacktrace}");
                        }
                        break;
                    case TestStatus.Inconclusive:
                        logstatus = Status.Warning;
                        Node.Log(logstatus, $"#Test Name: {testName} #Status: {logstatus}");
                        break;
                    case TestStatus.Skipped:
                        logstatus = Status.Skip;
                        Node.Skip($"#Test Name: {testName} #Status: {logstatus}");
                        break;
                    default:
                        logstatus = Status.Pass;
                        Node.Log(logstatus, $"#Test Name: {testName} #Status: {logstatus}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CreateTestResult: " + ex.Message);
            }
        }

        public static string CaptureScreenshot(IWebDriver driver, string className, string testName)
        {
            try
            {
                ITakesScreenshot ts = (ITakesScreenshot)driver;
                OpenQA.Selenium.Screenshot screenshot = ts.GetScreenshot();
                var screenshotDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshot.Folder", className);
                testName = SanitizeFileName(testName);
                var fileName = $"Screenshot_{testName}_{DateTime.Now:yyyyMMdd_HHmmssff}.png";
                Directory.CreateDirectory(screenshotDirectory);
                var fileLocation = Path.Combine(screenshotDirectory, fileName);
                screenshot.SaveAsFile(fileLocation);
                return fileLocation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CaptureScreenshot: " + ex.Message);
                throw;
            }
        }
        
        public static MediaEntityModelProvider CaptureScreenShotAndAttachToExtendReport( String filePath)
        {
            return MediaEntityBuilder.CreateScreenCaptureFromPath(filePath).Build();
        }

        private static string SanitizeFileName(string fileName)
        {
            fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9_\-]", "_");

            const int maxLength = 100;
            if (fileName.Length > maxLength)
            {
                fileName = fileName.Substring(0, maxLength);
            }

            return fileName;
        }
    }
}