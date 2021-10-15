using System.Text.Encodings.Web;
using System.Text.Json;

namespace PersonnelInformationMerger.Core.Helpers
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string jsonContent)
        {

            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<T>(jsonContent, serializerOptions);

        }
        
        public static string Serialize<T>(T source)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(source, options);

            return json;
        }
    }
}