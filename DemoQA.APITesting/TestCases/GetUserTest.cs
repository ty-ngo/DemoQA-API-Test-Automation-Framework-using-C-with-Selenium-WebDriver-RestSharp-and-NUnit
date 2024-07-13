using Core.Extensions;
using Core.Reports;
using DemoQA.APITesting.Constants;
using DemoQA.Services.Model.DataObject;
using DemoQA.Services.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace DemoQA.APITesting.TestCases
{
    [TestFixture, Category("GetUser")]
    public class GetUserTest : BaseTest
    {
        private BookServices _bookServices;
        public GetUserTest()
        {
            _bookServices = new BookServices(ApiClient);
        }

        [SetUp]
        public void Setup() {}

        [Test]
        [TestCase("account_01", new string[] { "9781449325862", "9781449337711" })]
        [Category("GetUser1")]
        public void GetUserSuccessfully(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Delete all books from collection before test");
            _bookServices.DeleteAllBooksFromCollection(UserServices.GetToken(accountKey), account.userId);

            ReportLog.Info("2. Add books to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("2. Send request to get user");
            var response = UserServices.GetDetailUser(account.userId, UserServices.GetToken(accountKey));

            ReportLog.Info("3. Verify status code is 200");
            response.VerifyStatusCodeOk();

            ReportLog.Info("4. Verify userName is correct");
            response.Data.userName.Should().Be(account.userName);

            ReportLog.Info("5. Verify userId is correct");
            response.Data.userId.Should().Be(account.userId);

            ReportLog.Info("6. Verify books are correct");
            response.Data.books.Count.Should().Be(isbns.Length);

            foreach (var book in response.Data.books)
            {
                if (!isbns.Contains(book.isbn))
                {
                    Assert.True(false);
                }
            }

            ReportLog.Info("7. Verify schema of the response");
            response.VerifySchema(FilePathConstants.GetUserResponseSchemaFilePath);

            ReportLog.Info("8. Delete all books from collection after test");
            _bookServices.DeleteAllBooksFromCollection(UserServices.GetToken(accountKey), account.userId);
        }

        [Test]
        [TestCase("account_01")]
        [Category("GetUser")]
        public void GetUserUnsuccessfullyWhenNotAuthorized(string accountKey)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Send request to get user");
            var response = UserServices.GetDetailUser(account.userId, "abcd");

            ReportLog.Info("2. Verify status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("3. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("4. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User not authorized!"));
        }

        [Test]
        [TestCase("account_01")]
        [Category("GetUser")]
        public void GetUserUnsuccessfullyWithInvalidUserId(string accountKey)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Send request to get user");
            var response = UserServices.GetDetailUser("abcd", UserServices.GetToken(accountKey));

            ReportLog.Info("3. Verify status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("4. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("5. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User not found!"));
        }

        [TearDown]
        public void TearDown() {}
    }
}