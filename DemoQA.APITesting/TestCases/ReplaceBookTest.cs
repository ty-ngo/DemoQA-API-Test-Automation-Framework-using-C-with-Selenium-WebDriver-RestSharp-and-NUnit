using Core.Extensions;
using Core.Reports;
using DemoQA.APITesting.Constants;
using DemoQA.Services.Model.DataObject;
using DemoQA.Services.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace DemoQA.APITesting.TestCases
{
    [TestFixture, Category("ReplaceBook")]
    public class ReplaceBookTest : BaseTest
    {
        private BookServices _bookServices;

        public ReplaceBookTest()
        {
            _bookServices = new BookServices(ApiClient);
        }

        [SetUp]
        public void Setup() {}

        [Test]
        [TestCase("account_01", "9781449325862", "9781449337711")]
        [Category("ReplaceBook")]
        public void ReplaceBookSuccessfully(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);

            ReportLog.Info("4. Store new book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, expectedNewIsbn);

            ReportLog.Info("5. Verify that status code is 200");
            response.VerifyStatusCodeOk();

            ReportLog.Info("6. Verify that new book is the same as expected");
            var actualNewIsbn = response.Data.books[0].isbn;
            actualNewIsbn.Should().Be(expectedNewIsbn);

            ReportLog.Info("7. Verify schema of the response");
            response.VerifySchema(FilePathConstants.ReplaceBookResponseSchemaFilePath);

        }

        [Test]
        [TestCase("account_01", "9781449325862", "9781449337711")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenOldIsbnNotInCollection(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Delete old book from collection");
            _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("3. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);

            ReportLog.Info("4. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN supplied is not available in User's Collection!"));
        }

        [Test]
        [TestCase("account_01", "9781449325862", "abcd")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenNewIsbnNotInStore(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("4. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);

            ReportLog.Info("5. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("6. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("7. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN supplied is not available in Books Collection!"));

        }

        [Test]
        [TestCase("account_01", "9781449325862", "9781449337711")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenNotAuthorized(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("4. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection("abcd", account.userId, oldIsbn, expectedNewIsbn);
            
            ReportLog.Info("5. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("6. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("7. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User not authorized!"));
        }

        [Test]
        [TestCase("account_01", "9781449325862", "9781449337711")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWithIncorrectUserId(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("4. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), "abcd", oldIsbn, expectedNewIsbn);
            
            ReportLog.Info("5. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("6. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("7. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User Id not correct!"));

        }

        [Test]
        [TestCase("account_01", "9781449325862", "")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenMissingNewIsbn(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("4. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);
            
            ReportLog.Info("5. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("6. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("7. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("Request Body is Invalid!"));

        }

        [Test]
        [TestCase("account_01", "9781449325862", "9781449337711")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenNewIsbnAlreadyInUserCollection(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Add new book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [expectedNewIsbn]);

            ReportLog.Info("4. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("5. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);
            
            ReportLog.Info("6. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("7. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("8. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN already present in the User's Collection!"));

        }

        [Test]
        [TestCase("account_01", "9781449325862", "9781449325862")]
        [Category("ReplaceBook")]
        public void ReplaceBookUnsuccessfullyWhenNewIsbnIsTheSameAsOldIsbn(string accountKey, string oldIsbn, string expectedNewIsbn)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add old book to collection before test");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, [oldIsbn]);

            ReportLog.Info("3. Store book to delete");
            _bookServices.StoreDataToDeleteBook(UserServices.GetToken(accountKey), account.userId, oldIsbn);

            ReportLog.Info("4. Send request to replace book in collection");
            var response = _bookServices.ReplaceBookInCollection(UserServices.GetToken(accountKey), account.userId, oldIsbn, expectedNewIsbn);
            
            ReportLog.Info("5. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("6. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("7. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN already present in the User's Collection!"));

        }

        [TearDown]
        public void TearDown() {}
    }
}