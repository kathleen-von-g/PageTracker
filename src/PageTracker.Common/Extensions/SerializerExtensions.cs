using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PageTracker.Common.Extensions
{
    public static class SerializerExtensions
    {
        public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string Serialize<T>(this T? value)
        {
            if (value == null)
                return string.Empty;

            return JsonSerializer.Serialize(value, DefaultOptions);
        }
    }
}
