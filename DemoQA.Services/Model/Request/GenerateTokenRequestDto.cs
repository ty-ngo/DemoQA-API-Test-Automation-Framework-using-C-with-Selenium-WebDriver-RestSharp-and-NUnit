using Newtonsoft.Json;

namespace DemoQA.Services.Model.Request
{
    public class GenerateTokenRequestDto
    {
        [JsonProperty("userName")]
        public string username { get; set; }
        
        [JsonProperty("password")]
        public string password { get; set; }

        public GenerateTokenRequestDto(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}