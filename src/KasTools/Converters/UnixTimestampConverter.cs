using Newtonsoft.Json;

namespace KasTools.Converters;

public class UnixTimestampConverter : JsonConverter<DateTime>
{
    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        => writer.WriteValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        => DateTimeOffset.FromUnixTimeMilliseconds((long)(reader.Value ?? 0)).LocalDateTime;
}