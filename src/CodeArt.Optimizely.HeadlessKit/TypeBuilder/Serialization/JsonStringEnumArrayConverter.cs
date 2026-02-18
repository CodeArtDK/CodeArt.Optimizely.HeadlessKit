
namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class JsonStringEnumArrayConverter<T> : JsonConverter<T[]> where T : struct, Enum
    {
        public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var enumList = new System.Collections.Generic.List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                string value = reader.GetString();
                if (Enum.TryParse(value, true, out T result))
                {
                    enumList.Add(result);
                }
                else
                {
                    throw new JsonException($"Unable to convert \"{value}\" to enum \"{typeof(T)}\".");
                }
            }

            return enumList.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var enumValue in value)
            {
                writer.WriteStringValue(enumValue.ToString());
            }
            writer.WriteEndArray();
        }
    }

}
