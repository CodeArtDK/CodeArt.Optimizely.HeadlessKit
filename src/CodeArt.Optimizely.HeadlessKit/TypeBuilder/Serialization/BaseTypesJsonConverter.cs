using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Serialization
{
    public class BaseTypesJsonConverter : JsonConverter<BaseTypes?>
    {
        public override BaseTypes? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();
            if (value.StartsWith("_", StringComparison.Ordinal))
            {
                value = value[1..];
            }

            if (string.Equals(value, "component", StringComparison.OrdinalIgnoreCase))
            {
                return BaseTypes.Block;
            }

            return Enum.TryParse<BaseTypes>(value, true, out var result) ? result : null;
        }

        public override void Write(Utf8JsonWriter writer, BaseTypes? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var apiValue = value.Value switch
            {
                BaseTypes.Block => "_component",
                BaseTypes.Element => "_component",
                _ => $"_{value.Value.ToString().ToLowerInvariant()}"
            };
            writer.WriteStringValue(apiValue);
        }
    }
}
