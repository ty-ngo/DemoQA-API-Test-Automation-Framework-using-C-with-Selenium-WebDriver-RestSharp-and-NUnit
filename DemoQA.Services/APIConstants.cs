namespace DemoQA.Services
{
    public class APIConstants
    {
        public const string GetUserEndPoint = "/Account/v1/User/{0}";
        public const string AddBooksEndPoint = "/BookStore/v1/Books";
        public const string DeleteBookEndPoint = "/BookStore/v1/Book";
        public const string ReplaceBookEndPoint = "/BookStore/v1/Books/{0}";
        public const string GenerateTokenEndPoint = "/Account/v1/GenerateToken";
        public const string DeleteAllBooksEndPoint = "/BookStore/v1/Books?UserId={0}";
    }
}