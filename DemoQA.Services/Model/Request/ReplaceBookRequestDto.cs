using Newtonsoft.Json;

namespace DemoQA.Services.Model.Request
{
    public class ReplaceBookRequestDto
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("isbn")]
        public string isbn { get; set; }

        public ReplaceBookRequestDto(string userId, string isbn)
        {
            this.userId = userId;
            this.isbn = isbn;
        }
    }
}