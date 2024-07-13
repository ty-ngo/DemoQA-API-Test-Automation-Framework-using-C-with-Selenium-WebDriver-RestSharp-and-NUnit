using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class ReplaceBookResponseDto
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("books")]
        public List<BookDetailResponseDto> books { get; set; }
    }
}