using System.Text.Json;
using TonLibDotNet.Utils.Json;

namespace TonLibDotNet.Utils
{
    public class TonJsonSerializer : ITonJsonSerializer
    {
        protected readonly JsonSerializerOptions jsonOptions = new()
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            WriteIndented = false,
            TypeInfoResolver = new TonTypeResolver(),
            Converters =
            {
                new Seconds2DateTimeOffsetConverter(),
            },
        };

        public string Serialize(TypeBase type)
        {
            return JsonSerializer.Serialize(type, jsonOptions);
        }

        public TypeBase? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<TypeBase>(json, jsonOptions);
        }
    }
}
