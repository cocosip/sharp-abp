using FellowOakDicom;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpAbp.Abp.FoDicom
{
    public class DicomTagConverter : JsonConverter<DicomTag>
    {
        public DicomTagConverter()
        {

        }

        public override DicomTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var s = reader.GetString();

            return DicomTag.Parse(s);
        }

        public override void Write(Utf8JsonWriter writer, DicomTag value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteStringValue(string.Empty);
                return;
            }
            writer.WriteStringValue(value.ToString());
        }
    }
}
