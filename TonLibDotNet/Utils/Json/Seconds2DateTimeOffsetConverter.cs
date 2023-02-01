using System.Text.Json;
using System.Text.Json.Serialization;

namespace TonLibDotNet.Utils.Json
{
    public class Seconds2DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(val);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }
    }
}
