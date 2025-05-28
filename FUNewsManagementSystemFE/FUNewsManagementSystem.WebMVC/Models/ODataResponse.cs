using System.Text.Json.Serialization;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("@odata.context")]
        public string Context { get; set; }

        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        public List<T> Value { get; set; }
    }
}
