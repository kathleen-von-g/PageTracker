using System.Text.Json;

namespace PageTracker.Common.Extensions
{
    public static class SerializerExtensions
    {
        public static string Serialize<T>(this T? value)
        {
            if (value == null)
                return string.Empty;

            return JsonSerializer.Serialize(value);
        }
    }
}
