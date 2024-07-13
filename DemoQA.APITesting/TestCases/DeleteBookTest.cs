using Core.Extensions;
using Core.Reports;
using DemoQA.Services.Model.DataObject;
using DemoQA.Services.Model.Response;
using DemoQA.Services.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace DemoQA.APITesting.TestCases
{
    [TestFixture, Category("DeleteBook")]
    public class DeleteBookTest : BaseTest
    {
        private BookServices _bookServices;
        private UserServices _userServices;

        public DeleteBookTest()
        {
            _bookServices = new BookServices(ApiClient);
            _userServices = new UserServices(ApiClient);
        }

        [SetUp]
        public void Setup() {}

        [Test]
        [TestCase("account_01", "9781449325862")]
        [Category("DeleteBook")]
        public void DeleteBookSuccessfully(string accountKey, string isbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [isbn]);

            ReportLog.Info("3. Send request to delete book from collection");
            var response = _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);

            ReportLog.Info("4. Verify that status code is 204");
            response.VerifyStatusCodeNoContent();

            ReportLog.Info("5. Verify that User's Collection does not contain the deleted book");
            var getUserResponse = _userServices.GetDetailUser(account.userId, UserServices.GetToken(accountKey));
            foreach (BookDetailResponseDto book in getUserResponse.Data.books)
            {
                if (book.isbn == isbn)
                {
                    true.Should().Be(false);
                }
            }
        }

        [Test]
        [TestCase("account_01", "abcd")]
        [Category("DeleteBook")]
        public void DeleteBookUnsuccessfullyWhenBookNotInCollection(string accountKey, string isbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Send request to delete book from collection");
            var response = _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);

            ReportLog.Info("3. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("4. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("5. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN supplied is not available in User's Collection!"));
        }

        [Test]
        [TestCase("account_01", "")]
        [Category("DeleteBook")]
        public void DeleteBookUnsuccessfullyWhenMissingIsbn(string accountKey, string isbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Send request to delete book from collection");
            var response = _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);

            ReportLog.Info("3. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("4. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("5. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN supplied is not available in User's Collection!"));
        }

        [Test]
        [TestCase("account_01", "9781449325862")]
        [Category("DeleteBook")]
        public void DeleteBookUnsuccessfullyWhenNotAuthorized(string accountKey, string isbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [isbn]);

            ReportLog.Info("3. Send request to delete book from collection");
            var response = _bookServices.DeleteBookFromCollection("abcd", account.userId, isbn);

            ReportLog.Info("4. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("4. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("5. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User not authorized!"));
        }

        [Test]
        [TestCase("account_01", "9781449325862")]
        [Category("DeleteBook")]
        public void DeleteBookUnsuccessfullyWithIncorrectUserId(string accountKey, string isbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [isbn]);

            ReportLog.Info("3. Send request to delete book from collection");
            var response = _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), "abcd", isbn);

            ReportLog.Info("4. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("4. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("5. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User Id not correct!"));
        }

        [TearDown]
        public void TearDown() {}
    }
}