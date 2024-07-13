using Core.API;
using RestSharp;
using DemoQA.Services.Model.Response;
using DemoQA.Services.Model.Request;
using DemoQA.Services.Model.DataObject;
using Core.ShareData;

namespace DemoQA.Services.Services
{
    public class BookServices
    {
        private readonly APIClient _client;

        public BookServices(APIClient client)
        {
            _client = client;
        }

        public static List<IsbnDto> ConvertToIsbnDtoList(string[] isbns)
        {
            List<IsbnDto> isbnDtoList = new List<IsbnDto>();
            foreach (string isbn in isbns)
            {
                isbnDtoList.Add(new IsbnDto(isbn));
            }
            return isbnDtoList;
        }

        public RestResponse<AddBooksResponseDto> AddBooksToCollection(string token, string userId, string[] isbns)
        {
            List<IsbnDto> bookList = ConvertToIsbnDtoList(isbns);
            AddBooksRequestDto requestBody = new AddBooksRequestDto(userId, bookList);

            return _client.CreateRequest(APIConstants.AddBooksEndPoint)
                    .AddHeader("accept", "application/json")
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddHeaderBearerToken(token)
                    .AddBody(requestBody)
                    .ExecutePost<AddBooksResponseDto>();
        }

        // public async Task<RestResponse<AddBookResponseDto>> AddBookToCollectionSuccess(AddBookToCollectionDtoReq bookDtoReq, string token)
        // {
        //     var response = await _client.CreateRequest(APIConstants.AddBooksEndPoint)
        //         .AddHeader("accept", "application/json")
        //         .AddHeaderBearerToken(token)
        //         .AddBody(bookDtoReq)
        //         .ExecutePostAsync<AddBookResponseDto>();
        //     return response;
        // }

        // public RestResponse<AddBookResponseDto> AddBookToCollectionSuccess(AddBookToCollectionDtoReq bookDtoReq, string token)
        // {
        //     return _client.CreateRequest(APIConstants.AddBooksEndPoint)
        //         .AddHeader("accept", "application/json")
        //         .AddHeaderBearerToken(token)
        //         .AddBody(bookDtoReq)
        //         .ExecutePost<AddBookResponseDto>();
        // }

        // public RestResponse<AddBookResponseDto> AddBookToCollectionSuccess(AddBookRequestDto requestBody, string token)
        // {
        //     return _client.CreateRequest(APIConstants.AddBooksEndPoint)
        //         .AddHeader("accept", "application/json")
        //         .AddHeaderBearerToken(token)
        //         .AddBody(requestBody)
        //         .ExecutePost<AddBookResponseDto>();
        // }

        // public async Task<RestResponse<AddBookResponseDto>> AddBookToCollectionSuccess(string userId, string isbn, string token)
        // {
        //     var bookReq = new AddBookToCollectionDtoReq()
        //     {
        //         UserId = userId,
        //         CollectionOfIsbns = new List<CollectionOfIsbn>()
        //     {
        //         new CollectionOfIsbn { Isbn = isbn  }
        //     }
        //     };
        //     return await AddBookToCollectionSuccess(bookReq, token);
        // }

        // public RestResponse<AddBookResponseDto> AddBookToCollectionSuccess(string userId, string isbn, string token)
        // {
        //     var bookReq = new AddBookToCollectionDtoReq()
        //     {
        //         UserId = userId,
        //         CollectionOfIsbns = new List<CollectionOfIsbn>()
        //     {
        //         new CollectionOfIsbn { Isbn = isbn  }
        //     }
        //     };
        //     return AddBookToCollectionSuccess(bookReq, token);
        // }

        // public RestResponse<AddBookResponseDto> AddBookToCollectionSuccess(string userId, string isbn, string token)
        // {
        //     var requestBody = new AddBookRequestDto()
        //     {
        //         userId = userId,
        //         collectionOfIsbns = new List<IsbnDto>()
        //     {
        //         new IsbnDto { isbn = isbn  }
        //     }
        //     };
        //     return AddBookToCollectionSuccess(requestBody, token);
        // }

        public RestResponse<ReplaceBookResponseDto> ReplaceBookInCollection(string token, string userId, string oldIsbn, string newIsbn)
        {
            var replaceBooksRequestBody = new ReplaceBookRequestDto(userId, newIsbn);

            return _client.CreateRequest(string.Format(APIConstants.ReplaceBookEndPoint, oldIsbn))
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddHeaderBearerToken(token)
                    .AddBody(replaceBooksRequestBody)
                    .ExecutePut<ReplaceBookResponseDto>();
        }

        public RestResponse<DeleteBookResponseDto> DeleteBookFromCollection(string token, string userId, string isbn)
        {
            var deleteBooksRequestBody = new DeleteBookRequestDto(isbn, userId);

            return _client.CreateRequest(APIConstants.DeleteBookEndPoint)
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddHeaderBearerToken(token)
                    .AddBody(deleteBooksRequestBody)
                    .ExecuteDelete<DeleteBookResponseDto>();
        }

        public RestResponse DeleteAllBooksFromCollection(string token, string userId)
        {
            return _client.CreateRequest(string.Format(APIConstants.DeleteAllBooksEndPoint, userId))
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddHeaderBearerToken(token)
                    .ExecuteDelete();
        }

        public void StoreDataToDeleteBook(string userId, string isbn, string token)
        {
            DataStorage.SetData("hasCreatedBook", true);
            DataStorage.SetData("userId", userId);
            DataStorage.SetData("isbn", isbn);
            DataStorage.SetData("token", token);
        }

        public void DeleteCreatedBookFromStorage()
        {
            if (DataStorage.GetData("hasCreatedBook") != null)
            {
                if ((bool)DataStorage.GetData("hasCreatedBook") == true)
                {
                    DeleteBookFromCollection(
                        (string)DataStorage.GetData("token"),
                        (string)DataStorage.GetData("userId"),
                        (string)DataStorage.GetData("isbn")
                    );
                }
            }
        }
    }
}