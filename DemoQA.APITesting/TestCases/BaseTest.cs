using NUnit.Framework.Interfaces;
using Core.API;
using Core.Configuration;
using Core.Reports;
using Core.ShareData;
using Core.Utilities;
using DemoQA.APITesting.Constants;
using DemoQA.Services.Services;
using DemoQA.Services.Model.DataObject;
namespace DemoQA.APITesting.TestCases
{
    [TestFixture]
    [TestFixture, Parallelizable(ParallelScope.Fixtures)]
    public class BaseTest
    {
        protected Dictionary<string, AccountDto> AccountData;
        protected static APIClient ApiClient;
        protected UserServices UserServices;

        public BaseTest()
        {
            AccountData = JsonUtils.ReadDictionaryJson<AccountDto>(FilePathConstants.AccountPath);
            ApiClient = new APIClient(ConfigurationHelper.GetConfiguration()["application:url"]);
            UserServices = new UserServices(ApiClient);

            ExtentTestManager.CreateParentTest(TestContext.CurrentContext.Test.ClassName);
        }

        [SetUp]
        public void BeforeTest()
        {
            ExtentTestManager.CreateTest(TestContext.CurrentContext.Test.Name);
            Console.WriteLine("Base Test set up");
        }

        [TearDown]
        public void TearDown()
        {
            DataStorage.ClearData(); 
            UpdateTestReport();      
            Console.WriteLine("Base Test tear down");
        }

        public void UpdateTestReport()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace) ? "" : TestContext.CurrentContext.Result.StackTrace;
            var message = TestContext.CurrentContext.Result.Message;

            switch (status)
            {
                case TestStatus.Failed:
                    ReportLog.Fail($"Test failed with message: {message}");
                    ReportLog.Fail($"Stacktrace: {stacktrace}");
                    break;
                case TestStatus.Inconclusive:
                    ReportLog.Skip($"Test inconclusive with message: {message}");
                    ReportLog.Skip($"Stacktrace: {stacktrace}");
                    break;
                case TestStatus.Skipped:
                    ReportLog.Skip($"Test skipped with message: {message}");
                    break;
                default:
                    ReportLog.Pass("Test passed");
                    break;
            }
        }
    }
}