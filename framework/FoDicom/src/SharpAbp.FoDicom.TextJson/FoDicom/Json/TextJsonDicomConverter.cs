using FellowOakDicom;
using FellowOakDicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SharpAbp.FoDicom.Json
{
    /// <summary>
    /// Converts a DicomDataset object to and from JSON using the NewtonSoft Json.NET library
    /// </summary>
    public class TextJsonDicomConverter : JsonConverter<DicomDataset>
    {
        private readonly bool _writeTagsAsKeywords;
        private readonly static Encoding _jsonTextEncoding = Encoding.UTF8;

        /// <summary>
        /// Initialize the JsonDicomConverter.
        /// </summary>
        /// <param name="writeTagsAsKeywords">Whether to write the json keys as DICOM keywords instead of tags. This makes the json non-compliant to DICOM JSON.</param>
        public TextJsonDicomConverter(bool writeTagsAsKeywords = false)
        {
            _writeTagsAsKeywords = writeTagsAsKeywords;
        }

        #region JsonConverter overrides
        public override DicomDataset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadJsonDataset(ref reader);
        }

        private DicomDataset ReadJsonDataset(ref Utf8JsonReader reader)
        {
            var dataset = new DicomDataset();
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();

                    DicomTag tag = ParseTag(propertyName);
                    var item = ReadJsonDicomItem(tag, ref reader);
                    dataset.Add(item);
                }

            }


            foreach (var item in dataset)
            {
                if (item.Tag.IsPrivate && ((item.Tag.Element & 0xff00) != 0))
                {
                    var privateCreatorTag = new DicomTag(item.Tag.Group, (ushort)(item.Tag.Element >> 8));

                    if (dataset.Contains(privateCreatorTag))
                    {
                        item.Tag.PrivateCreator = new DicomPrivateCreator(dataset.GetSingleValue<string>(privateCreatorTag));
                    }
                }
            }

            return dataset;
        }


        public override void Write(Utf8JsonWriter writer, DicomDataset value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var dataset = (DicomDataset)value;

            writer.WriteStartObject();
            foreach (var item in dataset)
            {
                if (((uint)item.Tag & 0xffff) == 0)
                {
                    // Group length (gggg,0000) attributes shall not be included in a DICOM JSON Model object.
                    continue;
                }

                // Unknown or masked tags cannot be written as keywords
                var unknown = item.Tag.DictionaryEntry == null
                              || string.IsNullOrWhiteSpace(item.Tag.DictionaryEntry.Keyword)
                              ||
                              (item.Tag.DictionaryEntry.MaskTag != null &&
                               item.Tag.DictionaryEntry.MaskTag.Mask != 0xffffffff);
                if (_writeTagsAsKeywords && !unknown)
                {
                    writer.WritePropertyName(item.Tag.DictionaryEntry.Keyword);
                }
                else
                {
                    writer.WritePropertyName(item.Tag.Group.ToString("X4") + item.Tag.Element.ToString("X4"));
                }

                WriteJsonDicomItem(writer, item, options);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DicomDataset).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }
        #endregion

        /// <summary>
        /// Create an instance of a IBulkDataUriByteBuffer. Override this method to use a different IBulkDataUriByteBuffer implementation in applications.
        /// </summary>
        /// <param name="bulkDataUri">The URI of a bulk data element as defined in <see cref="!:http://dicom.nema.org/medical/dicom/current/output/chtml/part19/chapter_A.html#table_A.1.5-2">Table A.1.5-2 in PS3.19</see>.</param>
        /// <returns>An instance of a Bulk URI Byte buffer.</returns>
        protected virtual IBulkDataUriByteBuffer CreateBulkDataUriByteBuffer(string bulkDataUri)
        {
            return new BulkDataUriByteBuffer(bulkDataUri);
        }

        #region Utilities

        internal static DicomTag ParseTag(string tagstr)
        {
            if (Regex.IsMatch(tagstr, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                var group = Convert.ToUInt16(tagstr.Substring(0, 4), 16);
                var element = Convert.ToUInt16(tagstr.Substring(4), 16);
                var tag = new DicomTag(group, element);
                return tag;
            }

            return DicomDictionary.Default[tagstr];
        }

        private static DicomItem CreateDicomItem(DicomTag tag, string vr, object data)
        {
            DicomItem item;
            switch (vr)
            {
                case "AE":
                    item = new DicomApplicationEntity(tag, (string[])data);
                    break;
                case "AS":
                    item = new DicomAgeString(tag, (string[])data);
                    break;
                case "AT":
                    item = new DicomAttributeTag(tag, ((string[])data).Select(ParseTag).ToArray());
                    break;
                case "CS":
                    item = new DicomCodeString(tag, (string[])data);
                    break;
                case "DA":
                    item = new DicomDate(tag, (string[])data);
                    break;
                case "DS":
                    if (data is IByteBuffer dataBufferDS)
                    {
                        item = new DicomDecimalString(tag, dataBufferDS);
                    }
                    else
                    {
                        item = new DicomDecimalString(tag, (string[])data);
                    }
                    break;
                case "DT":
                    item = new DicomDateTime(tag, (string[])data);
                    break;
                case "FD":
                    if (data is IByteBuffer dataBufferFD)
                    {
                        item = new DicomFloatingPointDouble(tag, dataBufferFD);
                    }
                    else
                    {
                        item = new DicomFloatingPointDouble(tag, (double[])data);
                    }
                    break;
                case "FL":
                    if (data is IByteBuffer dataBufferFL)
                    {
                        item = new DicomFloatingPointSingle(tag, dataBufferFL);
                    }
                    else
                    {
                        item = new DicomFloatingPointSingle(tag, (float[])data);
                    }
                    break;
                case "IS":
                    if (data is IByteBuffer dataBufferIS)
                    {
                        item = new DicomIntegerString(tag, dataBufferIS);
                    }
                    else
                    {
                        item = new DicomIntegerString(tag, (int[])data);
                    }
                    break;
                case "LO":
                    item = new DicomLongString(tag, (string[])data);
                    break;
                case "LT":
                    if (data is IByteBuffer dataBufferLT)
                    {
                        item = new DicomLongText(tag, _jsonTextEncoding, dataBufferLT);
                    }
                    else
                    {
                        item = new DicomLongText(tag, _jsonTextEncoding, data.AsStringArray().SingleOrEmpty());
                    }
                    break;
                case "OB":
                    item = new DicomOtherByte(tag, (IByteBuffer)data);
                    break;
                case "OD":
                    item = new DicomOtherDouble(tag, (IByteBuffer)data);
                    break;
                case "OF":
                    item = new DicomOtherFloat(tag, (IByteBuffer)data);
                    break;
                case "OL":
                    item = new DicomOtherLong(tag, (IByteBuffer)data);
                    break;
                case "OW":
                    item = new DicomOtherWord(tag, (IByteBuffer)data);
                    break;
                case "OV":
                    item = new DicomOtherVeryLong(tag, (IByteBuffer)data);
                    break;
                case "PN":
                    item = new DicomPersonName(tag, (string[])data);
                    break;
                case "SH":
                    item = new DicomShortString(tag, (string[])data);
                    break;
                case "SL":
                    if (data is IByteBuffer dataBufferSL)
                    {
                        item = new DicomSignedLong(tag, dataBufferSL);
                    }
                    else
                    {
                        item = new DicomSignedLong(tag, (int[])data);
                    }
                    break;
                case "SQ":
                    item = new DicomSequence(tag, ((DicomDataset[])data));
                    break;
                case "SS":
                    if (data is IByteBuffer dataBufferSS)
                    {
                        item = new DicomSignedShort(tag, dataBufferSS);
                    }
                    else
                    {
                        item = new DicomSignedShort(tag, (short[])data);
                    }
                    break;
                case "ST":
                    if (data is IByteBuffer dataBufferST)
                    {
                        item = new DicomShortText(tag, _jsonTextEncoding, dataBufferST);
                    }
                    else
                    {
                        item = new DicomShortText(tag, _jsonTextEncoding, data.AsStringArray().FirstOrEmpty());
                    }
                    break;
                case "SV":
                    if (data is IByteBuffer dataBufferSV)
                    {
                        item = new DicomSignedVeryLong(tag, dataBufferSV);
                    }
                    else
                    {
                        item = new DicomSignedVeryLong(tag, (long[])data);
                    }
                    break;
                case "TM":
                    item = new DicomTime(tag, (string[])data);
                    break;
                case "UC":
                    if (data is IByteBuffer dataBufferUC)
                    {
                        item = new DicomUnlimitedCharacters(tag, _jsonTextEncoding, dataBufferUC);
                    }
                    else
                    {
                        item = new DicomUnlimitedCharacters(tag, _jsonTextEncoding, data.AsStringArray().SingleOrDefault());
                    }
                    break;
                case "UI":
                    item = new DicomUniqueIdentifier(tag, (string[])data);
                    break;
                case "UL":
                    if (data is IByteBuffer dataBufferUL)
                    {
                        item = new DicomUnsignedLong(tag, dataBufferUL);
                    }
                    else
                    {
                        item = new DicomUnsignedLong(tag, (uint[])data);
                    }
                    break;
                case "UN":
                    item = new DicomUnknown(tag, (IByteBuffer)data);
                    break;
                case "UR":
                    item = new DicomUniversalResource(tag, data.AsStringArray().SingleOrEmpty());
                    break;
                case "US":
                    if (data is IByteBuffer dataBufferUS)
                    {
                        item = new DicomUnsignedShort(tag, dataBufferUS);
                    }
                    else
                    {
                        item = new DicomUnsignedShort(tag, (ushort[])data);
                    }
                    break;
                case "UT":
                    if (data is IByteBuffer dataBufferUT)
                    {
                        item = new DicomUnlimitedText(tag, _jsonTextEncoding, dataBufferUT);
                    }
                    else
                    {
                        item = new DicomUnlimitedText(tag, _jsonTextEncoding, data.AsStringArray().SingleOrEmpty());
                    }
                    break;
                case "UV":
                    if (data is IByteBuffer dataBufferUV)
                    {
                        item = new DicomUnsignedVeryLong(tag, dataBufferUV);
                    }
                    else
                    {
                        item = new DicomUnsignedVeryLong(tag, (ulong[])data);
                    }
                    break;
                default:
                    throw new NotSupportedException("Unsupported value representation");
            }
            return item;
        }

        #endregion

        #region WriteJson helpers

        private void WriteJsonDicomItem(Utf8JsonWriter writer, DicomItem item, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("vr");
            writer.WriteStringValue(item.ValueRepresentation.Code);

            switch (item.ValueRepresentation.Code)
            {
                case "PN":
                    WriteJsonPersonName(writer, (DicomPersonName)item);
                    break;
                case "SQ":
                    WriteJsonSequence(writer, (DicomSequence)item, options);
                    break;
                case "OB":
                case "OD":
                case "OF":
                case "OL":
                case "OV":
                case "OW":
                case "UN":
                    WriteJsonOther(writer, (DicomElement)item);
                    break;
                case "FL":
                    WriteJsonElement<float>(writer, (DicomElement)item);
                    break;
                case "FD":
                    WriteJsonElement<double>(writer, (DicomElement)item);
                    break;
                case "IS":
                case "SL":
                    WriteJsonElement<int>(writer, (DicomElement)item);
                    break;
                case "SS":
                    WriteJsonElement<short>(writer, (DicomElement)item);
                    break;
                case "SV":
                    WriteJsonElement<long>(writer, (DicomElement)item);
                    break;
                case "UL":
                    WriteJsonElement<uint>(writer, (DicomElement)item);
                    break;
                case "US":
                    WriteJsonElement<ushort>(writer, (DicomElement)item);
                    break;
                case "UV":
                    WriteJsonElement<ulong>(writer, (DicomElement)item);
                    break;
                case "DS":
                    WriteJsonDecimalString(writer, (DicomElement)item);
                    break;
                case "AT":
                    WriteJsonAttributeTag(writer, (DicomElement)item);
                    break;
                default:
                    WriteJsonElement<string>(writer, (DicomElement)item);
                    break;
            }
            writer.WriteEndObject();
        }

        private static void WriteJsonDecimalString(Utf8JsonWriter writer, DicomElement elem)
        {
            if (elem.Count != 0)
            {
                writer.WritePropertyName("Value");
                writer.WriteStartArray();
                foreach (var val in elem.Get<string[]>())
                {
                    if (string.IsNullOrEmpty(val))
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        var fix = FixDecimalString(val);
                        if (ulong.TryParse(fix, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong xulong))
                        {
                            writer.WriteNumberValue(xulong);
                        }
                        else if (long.TryParse(fix, NumberStyles.Integer, CultureInfo.InvariantCulture, out long xlong))
                        {
                            writer.WriteNumberValue(xlong);
                        }
                        else if (decimal.TryParse(fix, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal xdecimal))
                        {
                            writer.WriteNumberValue(xdecimal);
                        }
                        else if (double.TryParse(fix, NumberStyles.Float, CultureInfo.InvariantCulture, out double xdouble))
                        {
                            writer.WriteNumberValue(xdouble);
                        }
                        else
                        {
                            throw new FormatException($"Cannot write dicom number {val} to json");
                        }
                    }
                }
                writer.WriteEndArray();
            }
        }

        private static bool IsValidJsonNumber(string val)
        {
            try
            {
                DicomValidation.ValidateDS(val);
                return true;
            }
            catch (DicomValidationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Fix-up a Dicom DS number for use with json.
        /// Rationale: There is a requirement that DS numbers shall be written as json numbers in part 18.F json, but the
        /// requirements on DS allows values that are not json numbers. This method "fixes" them to conform to json numbers.
        /// </summary>
        /// <param name="val">A valid DS value</param>
        /// <returns>A json number equivalent to the supplied DS value</returns>
        private static string FixDecimalString(string val)
        {
            if (IsValidJsonNumber(val))
            {
                return val;
            }

            if (string.IsNullOrWhiteSpace(val)) { return null; }

            val = val.Trim();

            var negative = false;
            // Strip leading superfluous plus signs
            if (val[0] == '+')
            {
                val = val.Substring(1);
            }
            else if (val[0] == '-')
            {
                // Temporarily remove negation sign for zero-stripping later
                negative = true;
                val = val.Substring(1);
            }

            // Strip leading superfluous zeros
            if (val.Length > 1 && val[0] == '0' && val[1] != '.')
            {
                int i = 0;
                while (i < val.Length - 1 && val[i] == '0' && val[i + 1] != '.')
                {
                    i++;
                }

                val = val.Substring(i);
            }

            // Re-add negation sign
            if (negative) { val = "-" + val; }

            if (IsValidJsonNumber(val))
            {
                return val;
            }

            throw new ArgumentException("Failed converting DS value to json");
        }

        private static void WriteJsonElement<T>(Utf8JsonWriter writer, DicomElement elem)
        {
            if (elem.Count != 0)
            {
                writer.WritePropertyName("Value");
                writer.WriteStartArray();
                foreach (var val in elem.Get<T[]>())
                {
                    if (val == null || (typeof(T) == typeof(string) && val.Equals("")))
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        if (typeof(T) == typeof(int))
                        {
                            writer.WriteNumberValue((int)Convert.ChangeType(val, typeof(int)));
                        }
                        else if (typeof(T) == typeof(short))
                        {
                            writer.WriteNumberValue((short)Convert.ChangeType(val, typeof(short)));
                        }
                        else if (typeof(T) == typeof(long))
                        {
                            writer.WriteNumberValue((long)Convert.ChangeType(val, typeof(long)));
                        }
                        else if (typeof(T) == typeof(uint))
                        {
                            writer.WriteNumberValue((uint)Convert.ChangeType(val, typeof(uint)));
                        }
                        else if (typeof(T) == typeof(ushort))
                        {
                            writer.WriteNumberValue((ushort)Convert.ChangeType(val, typeof(ushort)));
                        }
                        else if (typeof(T) == typeof(ulong))
                        {
                            writer.WriteNumberValue((ulong)Convert.ChangeType(val, typeof(ulong)));
                        }
                        else if (typeof(T) == typeof(double))
                        {
                            writer.WriteNumberValue((double)Convert.ChangeType(val, typeof(double)));
                        }
                        else if (typeof(T) == typeof(float))
                        {
                            writer.WriteNumberValue((float)Convert.ChangeType(val, typeof(float)));
                        }
                        else if (typeof(T) == typeof(string))
                        {
                            writer.WriteStringValue(val.ToString());
                        }
                        else
                        {
                            //默认写入
                            writer.WriteStringValue(val.ToString());
                        }
                    }
                }
                writer.WriteEndArray();
            }
        }

        private static void WriteJsonAttributeTag(Utf8JsonWriter writer, DicomElement elem)
        {
            if (elem.Count != 0)
            {
                writer.WritePropertyName("Value");
                writer.WriteStartArray();
                foreach (var val in elem.Get<DicomTag[]>())
                {
                    if (val == null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStringValue(((uint)val).ToString("X8"));
                    }
                }
                writer.WriteEndArray();
            }
        }

        private static void WriteJsonOther(Utf8JsonWriter writer, DicomElement elem)
        {
            if (elem.Buffer is IBulkDataUriByteBuffer buffer)
            {
                writer.WritePropertyName("BulkDataURI");
                writer.WriteStringValue(buffer.BulkDataUri);
            }
            else if (elem.Count != 0)
            {
                writer.WritePropertyName("InlineBinary");
                writer.WriteBase64StringValue(elem.Buffer.Data);
                //writer.WriteBase64StringValue(Convert.ToBase64String(elem.Buffer.Data));
            }
        }

        private void WriteJsonSequence(Utf8JsonWriter writer, DicomSequence seq, JsonSerializerOptions options)
        {
            if (seq.Items.Count != 0)
            {
                writer.WritePropertyName("Value");
                writer.WriteStartArray();

                foreach (var child in seq.Items)
                {
                    Write(writer, child, options);
                }

                writer.WriteEndArray();
            }
        }

        private static void WriteJsonPersonName(Utf8JsonWriter writer, DicomPersonName pn)
        {
            if (pn.Count != 0)
            {
                writer.WritePropertyName("Value");
                writer.WriteStartArray();

                foreach (var val in pn.Get<string[]>())
                {
                    if (string.IsNullOrEmpty(val))
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("Alphabetic");
                        writer.WriteStringValue(val);
                        writer.WriteEndObject();
                    }
                }

                writer.WriteEndArray();
            }
        }

        #endregion

        #region ReadJson helpers

        private DicomItem ReadJsonDicomItem(DicomTag tag, ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            var propertyName = reader.GetString();
            if (propertyName != "vr")
            {
                throw new JsonException("Malformed DICOM json");
            }

            //读取vr的值
            reader.Read();
            string vr = reader.GetString();

            object data;

            bool hasReadEndObject = false;
            switch (vr)
            {
                case "OB":
                case "OD":
                case "OF":
                case "OL":
                case "OW":
                case "OV":
                case "UN":
                    data = ReadJsonOX(ref reader, ref hasReadEndObject);
                    break;
                case "SQ":
                    data = ReadJsonSequence(ref reader, ref hasReadEndObject);
                    break;
                case "PN":
                    data = ReadJsonPersonName(ref reader, ref hasReadEndObject);
                    break;
                case "FL":
                    data = ReadJsonMultiNumber<float>(ref reader, ref hasReadEndObject);
                    break;
                case "FD":
                    data = ReadJsonMultiNumber<double>(ref reader, ref hasReadEndObject);
                    break;
                case "IS":
                    data = ReadJsonMultiNumber<int>(ref reader, ref hasReadEndObject);
                    break;
                case "SL":
                    data = ReadJsonMultiNumber<int>(ref reader, ref hasReadEndObject);
                    break;
                case "SS":
                    data = ReadJsonMultiNumber<short>(ref reader, ref hasReadEndObject);
                    break;
                case "SV":
                    data = ReadJsonMultiNumber<long>(ref reader, ref hasReadEndObject);
                    break;
                case "UL":
                    data = ReadJsonMultiNumber<uint>(ref reader, ref hasReadEndObject);
                    break;
                case "US":
                    data = ReadJsonMultiNumber<ushort>(ref reader, ref hasReadEndObject);
                    break;
                case "UV":
                    data = ReadJsonMultiNumber<ulong>(ref reader, ref hasReadEndObject);
                    break;
                case "DS":
                    data = ReadJsonMultiString(ref reader, ref hasReadEndObject);
                    break;
                default:
                    data = ReadJsonMultiString(ref reader, ref hasReadEndObject);
                    break;
            }

            DicomItem item = CreateDicomItem(tag, vr, data);
            if (!hasReadEndObject)
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException();
                }
            }
            return item;
        }

        private object ReadJsonMultiString(ref Utf8JsonReader reader, ref bool hasReadEndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                hasReadEndObject = true;
                return new string[0];
            }
            //
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (propertyName == "Value")
            {
                return ReadJsonMultiStringValue(ref reader);
            }
            else if (propertyName == "BulkDataURI")
            {
                return ReadJsonBulkDataUri(ref reader);
            }
            else
            {
                return new string[0];
            }
        }

        private static string[] ReadJsonMultiStringValue(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                reader.Skip();
            }
            var childStrings = new List<string>();
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return childStrings.ToArray();
                }
                else if (reader.TokenType == JsonTokenType.Null)
                {
                    childStrings.Add(null);
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    childStrings.Add(reader.GetString());
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    childStrings.Add(reader.GetDecimal().ToString());
                }
            }
            var data = childStrings.ToArray();
            return data;
        }

        private object ReadJsonMultiNumber<T>(ref Utf8JsonReader reader, ref bool hasReadEndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                hasReadEndObject = true;
                return new T[0];
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (propertyName == "Value")
            {
                return ReadJsonMultiNumberValue<T>(ref reader);
            }
            else if (propertyName == "BulkDataURI")
            {
                return ReadJsonBulkDataUri(ref reader);
            }
            else
            {
                return new T[0];
            }
        }

        private static T[] ReadJsonMultiNumberValue<T>(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                return new T[0];
            }
            var childValues = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return childValues.ToArray();
                }

                if (reader.TokenType != JsonTokenType.Number)
                {
                    throw new JsonException("ReadJsonMultiNumberValue TokenType is not Number.");
                }

                if (typeof(T) == typeof(int))
                {
                    childValues.Add((T)Convert.ChangeType(reader.GetInt32(), typeof(T)));
                }
                else if (typeof(T) == typeof(double))
                {
                    childValues.Add((T)Convert.ChangeType(reader.GetDouble(), typeof(T)));
                }
                else
                {
                    childValues.Add((T)Convert.ChangeType(reader.GetDecimal(), typeof(T)));
                }
            }

            throw new JsonException();
        }

        private string[] ReadJsonPersonName(ref Utf8JsonReader reader, ref bool hasReadEndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                hasReadEndObject = true;
                return new string[0];
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            var propertyName = reader.GetString();
            if (propertyName == "Value")
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException();
                }

                var childStrings = new List<string>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        return childStrings.ToArray();
                    }
                    else if (reader.TokenType == JsonTokenType.Null)
                    {
                        childStrings.Add(null);
                    }
                    else
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            var alphabeticName = reader.GetString();
                            if (alphabeticName == "Alphabetic")
                            {
                                reader.Read();

                                childStrings.Add(reader.GetString());
                            }
                        }

                    }
                }

                throw new JsonException();
            }
            else
            {
                return new string[0];
            }

        }

        private DicomDataset[] ReadJsonSequence(ref Utf8JsonReader reader, ref bool hasReadEndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                hasReadEndObject = true;
                return new DicomDataset[0];
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            var propertyName = reader.GetString();
            if (propertyName == "Value")
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException();
                }
                var childItems = new List<DicomDataset>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        return childItems.ToArray();
                    }
                    else
                    {
                        childItems.Add(ReadJsonDataset(ref reader));
                    }
                }
                throw new JsonException();
            }
            else
            {
                return new DicomDataset[0];
            }
        }

        private IByteBuffer ReadJsonOX(ref Utf8JsonReader reader, ref bool hasReadEndObject)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                hasReadEndObject = true;
                return EmptyBuffer.Value;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            var propertyName = reader.GetString();
            if (propertyName == "InlineBinary")
            {
                return ReadJsonInlineBinary(ref reader);
            }
            else if (propertyName == "BulkDataURI")
            {
                return ReadJsonBulkDataUri(ref reader);
            }
            return EmptyBuffer.Value;
        }

        private static IByteBuffer ReadJsonInlineBinary(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Malformed DICOM json");
            }
            var data = new MemoryByteBuffer(reader.GetBytesFromBase64());
            // var data = new MemoryByteBuffer(Convert.FromBase64String(token.Value<string>()));
            return data;

        }

        private IBulkDataUriByteBuffer ReadJsonBulkDataUri(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Malformed DICOM json");
            }

            var data = CreateBulkDataUriByteBuffer(reader.GetString());
            return data;
        }


        #endregion
    }


    internal static class JsonDicomConverterExtensions
    {

        public static string[] AsStringArray(this object data) => (string[])data;

        public static string FirstOrEmpty(this string[] array) => array.Length > 0 ? array[0] : string.Empty;

        public static string SingleOrEmpty(this string[] array) => array.Length > 0 ? array.Single() : string.Empty;

    }
}
