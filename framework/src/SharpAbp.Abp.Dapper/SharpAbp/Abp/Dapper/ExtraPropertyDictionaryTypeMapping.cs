using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Volo.Abp.Data;

namespace SharpAbp.Abp.Dapper
{
    public class ExtraPropertyDictionaryTypeMapping : DapperTypeMapping<ExtraPropertyDictionary>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => true;

        public override bool CanParse(object? value) => true;

        public override ExtraPropertyDictionary Parse(object value)
        {
            if (value == null || value is DBNull)
            {
                return new ExtraPropertyDictionary();
            }

            if (value is ExtraPropertyDictionary dictionary)
            {
                return dictionary;
            }

            var json = value as string ?? value.ToString();
            if (string.IsNullOrWhiteSpace(json) || json == "{}")
            {
                return new ExtraPropertyDictionary();
            }

            var jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (jsonDictionary == null)
            {
                return new ExtraPropertyDictionary();
            }

            var extraProperties = new ExtraPropertyDictionary();
            foreach (var item in jsonDictionary)
            {
                extraProperties[item.Key] = NormalizeJsonElement(item.Value);
            }

            return extraProperties;
        }

        public override object ParseNull(Type destinationType)
        {
            return new ExtraPropertyDictionary();
        }

        public override void SetValue(IDbDataParameter parameter, ExtraPropertyDictionary value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonSerializer.Serialize(value);
        }

        public override void SetNullValue(IDbDataParameter parameter)
        {
            parameter.DbType = DbType.String;
            parameter.Value = JsonSerializer.Serialize(new ExtraPropertyDictionary());
        }

        private static object? NormalizeJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    if (element.TryGetDateTime(out var dateTime))
                    {
                        return dateTime;
                    }

                    if (element.TryGetGuid(out var guid))
                    {
                        return guid;
                    }

                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt32(out var int32Value))
                    {
                        return int32Value;
                    }

                    if (element.TryGetInt64(out var int64Value))
                    {
                        return int64Value;
                    }

                    if (element.TryGetDecimal(out var decimalValue))
                    {
                        return decimalValue;
                    }

                    return element.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var arrayItem in element.EnumerateArray())
                    {
                        list.Add(NormalizeJsonElement(arrayItem));
                    }

                    return list;

                case JsonValueKind.Object:
                    var dictionary = new Dictionary<string, object?>();
                    foreach (var property in element.EnumerateObject())
                    {
                        dictionary[property.Name] = NormalizeJsonElement(property.Value);
                    }

                    return dictionary;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }
    }
}
