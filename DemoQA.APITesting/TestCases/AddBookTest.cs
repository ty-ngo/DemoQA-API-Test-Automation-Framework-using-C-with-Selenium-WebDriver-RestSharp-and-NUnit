using Core.Extensions;
using Core.Reports;
using DemoQA.Services.Model.DataObject;
using DemoQA.Services.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace DemoQA.APITesting.TestCases
{
    [TestFixture, Category("AddBook")]
    public class AddBooksTest : BaseTest
    {
        private BookServices _bookServices;

        public AddBooksTest()
        {
            _bookServices = new BookServices(ApiClient);
        }

        [SetUp]
        public void BeforeAddBooksTest() {}

        [Test]
        [TestCase("account_01", new string[] { "9781449325862" })]
        [Category("AddBook")]
        public void AddSingleBookSuccessfully(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear book from collection before test");
            _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbns[0]);

            ReportLog.Info("3. Send request to add book to collection");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("4. Store new book to delete");
            _bookServices.StoreDataToDeleteBook(account.userId, isbns[0], UserServices.GetToken(accountKey));

            ReportLog.Info("4. Verify that status code is 201");
            response.VerifyStatusCodeCreated();

            ReportLog.Info("5. Verify schema of response");
            response.VerifySchema("TestData/Schemas/AddBooksResponseSchema.json");

            ReportLog.Info("6. Verify that book is added correctly");
            response.Data.books[0].isbn.Should().Be(isbns[0]);

            ReportLog.Info("7. Clear books from collection after test");
            _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbns[0]);
        }

        [Test]
        [TestCase("account_01", new string[] { "9781449325862", "9781449337711" })]
        [Category("AddBook")]
        public void AddMultipleBooksSuccessfully(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear books from collection before test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }

            ReportLog.Info("3. Send request to add books to collection");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("4. Verify that status code is 201");
            response.VerifyStatusCodeCreated();

            ReportLog.Info("5. Verify schema of response");
            response.VerifySchema("TestData/Schemas/AddBooksResponseSchema.json");

            ReportLog.Info("6. Verify that books are added correctly");
            var expectedBooks = BookServices.ConvertToIsbnDtoList(isbns);
            var actualBooks = response.Data.books;
            for (int i = 0; i < expectedBooks.Count; i++)
            {
                actualBooks[i].isbn.Should().Be(expectedBooks[i].isbn);
            }

            ReportLog.Info("7. Clear books from collection after test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }
        }

        [Test]
        [TestCase("account_01", new string[] { "9781449325862" })]
        [Category("AddBook")]
        public void AddBookUnsuccessfullyWhenBookAlreadyInUserCollection(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Add book to collection the first time");
            _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("3. Add book to collection the second time");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("4. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN already present in the User's Collection!"));
        }

        [Test]
        [TestCase("account_01", new string[] { "9781449325762" })]
        [Category("AddBook")]
        public void AddBooksUnsuccessfullyWithInvalidIsbn(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear books from collection before test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }

            ReportLog.Info("3. Send request to add books to collection");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("4. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("ISBN supplied is not available in Books Collection!"));
        }

        [Test]
        [TestCase("account_01", new string[] {})]
        [Category("AddBook")]
        public void AddBooksUnsuccessfullyWithoutBooks(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear books from collection before test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }

            ReportLog.Info("3. Send request to add books to collection");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), account.userId, isbns);

            ReportLog.Info("4. Verify that status code is 400");
            response.VerifyStatusCodeBadRequest();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("Collection of books required."));
        }

        [Test]
        [TestCase("account_01", new string[] { "9781449325762" })]
        [Category("AddBook")]
        public void AddBooksUnsuccessfullyWhenNotAuthorized(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear books from collection before test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }

            ReportLog.Info("3. Send request to add books to collection");
            var response = _bookServices.AddBooksToCollection("abcd", account.userId, isbns);

            ReportLog.Info("4. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User not authorized!"));
        }

        [Test]
        [TestCase("account_01", new string[] { "9781449325762" })]
        [Category("AddBook")]
        public void AddBooksUnsuccessfullyWithIncorrectUserId(string accountKey, string[] isbns)
        {
            AccountDto account = AccountData[accountKey];

            ReportLog.Info("1. Get Token");
            UserServices.StoreToken(accountKey, account);

            ReportLog.Info("2. Clear books from collection before test");
            foreach (string isbn in isbns)
            {
                _bookServices.DeleteBookFromCollection(UserServices.GetToken(accountKey), account.userId, isbn);
            }

            ReportLog.Info("3. Send request to add books to collection");
            var response = _bookServices.AddBooksToCollection(UserServices.GetToken(accountKey), "abcd", isbns);

            ReportLog.Info("4. Verify that status code is 401");
            response.VerifyStatusCodeUnauthorized();

            ReportLog.Info("5. Convert response to json");
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);

            ReportLog.Info("6. Verify message of response");
            Assert.That(result["message"].ToString(), Is.EqualTo("User Id not correct!"));
        }

        [TearDown]
        public void AfterAddBooksTest()
        {
            _bookServices.DeleteCreatedBookFromStorage();
        }
    }
}