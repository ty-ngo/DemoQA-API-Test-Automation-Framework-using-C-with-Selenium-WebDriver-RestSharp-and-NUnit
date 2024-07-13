using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class GenerateTokenResponseDto
    {
        [JsonProperty("token")]
        public string token { get; set; }

        [JsonProperty("expires")]
        public DateTime expires { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("result")]
        public string result { get; set; }
    }
}