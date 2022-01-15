using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Service.Interface.Base
{
    [PublicAPI]
    public static class SerializerOptions
    {
        public static JsonSerializerOptions GetDefaultOptions() =>
            SetDefaultOptions(new JsonSerializerOptions());

        public static JsonSerializerOptions SetDefaultOptions(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new JsonStringEnumConverter(options.PropertyNamingPolicy));
            SetSerializerDebugOptions(options);
            return options;
        }

        [Conditional("DEBUG")]
        private static void SetSerializerDebugOptions(JsonSerializerOptions options) =>
            options.WriteIndented = true;
    }
}
