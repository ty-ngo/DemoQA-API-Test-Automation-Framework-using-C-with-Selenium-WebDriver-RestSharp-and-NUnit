using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class DeleteBookResponseDto
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("isbn")]
        public string isbn { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }
    }
}