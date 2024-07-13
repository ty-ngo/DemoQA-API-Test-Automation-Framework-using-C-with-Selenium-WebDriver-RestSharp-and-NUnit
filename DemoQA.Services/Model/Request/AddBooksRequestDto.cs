using DemoQA.Services.Model.DataObject;
using Newtonsoft.Json;

namespace DemoQA.Services.Model.Request
{
    public class AddBooksRequestDto
    {
        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("collectionOfIsbns")]
        public List<IsbnDto> collectionOfIsbns { get; set; }

        public AddBooksRequestDto(string userId, List<IsbnDto> collectionOfIsbns)
        {
            this.userId = userId;;
            this.collectionOfIsbns = new List<IsbnDto>(collectionOfIsbns);
        }
    }
}