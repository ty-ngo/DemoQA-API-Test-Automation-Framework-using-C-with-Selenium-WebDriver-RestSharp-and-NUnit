using Newtonsoft.Json;

namespace DemoQA.Services.Model.DataObject
{
    public class IsbnDto
    {
        [JsonProperty("isbn")]
        public string isbn { get; set; }

        public IsbnDto(string isbn)
        {
            this.isbn = isbn;
        }
    }
}