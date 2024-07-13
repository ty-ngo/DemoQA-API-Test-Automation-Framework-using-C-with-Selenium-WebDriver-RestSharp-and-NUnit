
using Core.Configuration;
using Core.Reports;
using Core.ShareData;

namespace DemoQA.APITesting
{
    [SetUpFixture]
    public class TestStartup
    {
        const string AppSettings = "appsettings.json";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Console.WriteLine("One Time Setup");
            ConfigurationHelper.ReadConfiguration(AppSettings);
            DataStorage.InitData();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Console.WriteLine("One Time Tear Down");
            ExtentReportManager.GenerateReport();
        }

    }
}