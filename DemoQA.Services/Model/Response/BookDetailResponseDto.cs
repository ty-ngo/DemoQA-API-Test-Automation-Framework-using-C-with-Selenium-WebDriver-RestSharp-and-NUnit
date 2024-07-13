using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class BookDetailResponseDto
    {
        [JsonProperty("isbn")]
        public string isbn { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("subTitle")]
        public string subTitle { get; set; }

        [JsonProperty("author")]
        public string author { get; set; }

        [JsonProperty("publish_date")]
        public DateTime publish_date { get; set; }

        [JsonProperty("publisher")]
        public string publisher { get; set; }

        [JsonProperty("pages")]
        public int pages { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("website")]
        public string website { get; set; }
    }
}