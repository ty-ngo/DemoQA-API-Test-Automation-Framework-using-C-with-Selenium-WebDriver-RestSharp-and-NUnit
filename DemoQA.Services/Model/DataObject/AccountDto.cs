using Newtonsoft.Json;

namespace DemoQA.Services.Model.DataObject
{
    public class AccountDto
    {
        
        [JsonProperty("userName")]
        public string userName { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }
    }
}