using Newtonsoft.Json;

namespace DemoQA.Services.Model.Request
{
    public class DeleteBookRequestDto
    {
        [JsonProperty("isbn")]
        public string isbn { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }

        public DeleteBookRequestDto(string isbn, string userId)
        {
            this.isbn = isbn;
            this.userId = userId;
        }
    }
}