using DemoQA.Services.Model.DataObject;
using Newtonsoft.Json;

namespace DemoQA.Services.Model.Response
{
    public class AddBooksResponseDto
    {
        [JsonProperty("books")]
        public List<IsbnDto> books { get; set; }
    }
}