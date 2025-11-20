using System.Text.Json;

namespace Imposter.UI.Extension_Methods
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _options =
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value, _options));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class
        {
            var json = session.GetString(key);
            T? ans = json == null ? null : JsonSerializer.Deserialize<T>(json, _options);
            return ans;
        }
    }
}
