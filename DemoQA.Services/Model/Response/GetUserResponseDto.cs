using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class GetUserResponseDto
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("userName")]
        public string userName { get; set; }

        [JsonProperty("books")]
        public List<BookDetailResponseDto> books { get; set; }
    }
}