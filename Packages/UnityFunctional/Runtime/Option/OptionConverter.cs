using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bravasoft.Functional.Json
{
    public sealed class OptionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var valueType = typeToConvert.GetGenericArguments()[0];
            var converterType = typeof(OptionConverter<>).MakeGenericType(valueType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }

    public sealed class OptionConverter<T> : JsonConverter<Option<T>>
    {
        public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return Option<T>.None;
            }

            var value = JsonSerializer.Deserialize<T>(ref reader, options);

            return Option<T>.Some(value);
        }

        public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions options)
        {
            var (isSome, value) = option;

            if (isSome)
            {
                JsonSerializer.Serialize(writer, value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    public static class JsonConfigurationExtensions
    {
        public static JsonSerializerOptions AddOptionConverter(this JsonSerializerOptions options)
        {
            options.Converters.Add(new OptionConverterFactory());
            return options;
        }
    }

}