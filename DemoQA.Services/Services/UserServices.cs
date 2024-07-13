
using Core.API;
using Core.ShareData;
using DemoQA.Services.Model.Request;
using DemoQA.Services.Model.Response;
using RestSharp;
using Core.Extensions;
using Newtonsoft.Json;
using DemoQA.Services.Model.DataObject;

namespace DemoQA.Services.Services
{
    public class UserServices
    {
        private readonly APIClient _client;

        public UserServices(APIClient client)
        {
            _client = client;
        }

        public RestResponse<GetUserResponseDto> GetDetailUser(string UserId, string token)
        {
            return _client.CreateRequest(String.Format(APIConstants.GetUserEndPoint, UserId))
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddHeaderBearerToken(token)
                    .ExecuteGet<GetUserResponseDto>();
        }

        public RestResponse<GenerateTokenResponseDto> GenerateToken(string username, string password)
        {
            GenerateTokenRequestDto tokenRequestBody = new GenerateTokenRequestDto(username, password);

            return _client.CreateRequest(APIConstants.GenerateTokenEndPoint)
                    .AddHeader("accept", ContentType.Json)
                    .AddHeader("Content-Type", ContentType.Json)
                    .AddBody(tokenRequestBody)
                    .ExecutePost<GenerateTokenResponseDto>();
        }

        public void StoreToken(string accountKey, AccountDto account)
        {
            if (DataStorage.GetData(accountKey) is null)
            {
                var response = GenerateToken(account.userName, account.password);
                response.VerifyStatusCodeOk();

                var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
                DataStorage.SetData(accountKey, result["token"]);
            }
        }

        public string GetToken(string accountKey)
        {
            if (DataStorage.GetData(accountKey) is null)
            {
                throw new Exception("Token is not stored with account " + accountKey);
            }

            return DataStorage.GetData(accountKey).ToString();
        }
    }
}