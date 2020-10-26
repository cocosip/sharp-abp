using FellowOakDicom;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpAbp.Abp.FoDicom
{
    public static class FoDicomExtensions
    {
        /// <summary>
        ///  Get Date from dicom format DicomDataset
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? GetDate(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            var value = dataset.GetSingleValueOrDefault(tag, "");
            return value.ParseAsDicomDate(defaultValue);
        }

        /// <summary>
        /// Get DateTime from DicomDataset
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? GetTime(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            var value = dataset.GetSingleValueOrDefault(tag, "");
            return value.ParseAsDicomTime(defaultValue);
        }

        /// <summary>
        /// Get Age from DicomDataset
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DicomAge GetAge(this DicomDataset dataset, DicomTag tag, DicomAge defaultValue = default)
        {
            var value = dataset.GetSingleValueOrDefault(tag, "");
            return value.ParseAsDicomAge(defaultValue);
        }


        /// <summary>
        /// Use specified encode to encode value
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string GetEncodingValue(this DicomDataset dataset, DicomTag tag, string encode = "GB18030")
        {
            if (dataset.Contains(tag))
            {
                var buffer = dataset.GetValues<byte>(tag);
                if (buffer.Any())
                {
                    return buffer.ParseAsDicomString(encode);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Parse dicom string format date as DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ParseAsDicomDate(this string value, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
            }

            if (value.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(value, @"^[\d]{8}$"))
            {
                if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return d1;
                }
            }
            if (DateTime.TryParse(value, out DateTime d2))
            {
                return d2;
            }
            return defaultValue;
        }


        /// <summary>
        /// Parse dicom string format datetime as DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ParseAsDicomTime(this string value, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
            }

            if (value.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(value, @"^[\d]{6}$"))
            {
                if (DateTime.TryParseExact(value, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(value, @"^[\d]{4}$"))
            {
                if (DateTime.TryParseExact(value, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(value, @"^[\d]{6}\.[\d]{1,7}$"))
            {

                var f = "HHmmss.".PadRight(value.Length, 'f');
                if (DateTime.TryParseExact(value, f, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second, d1.Millisecond);
                }
            }

            if (DateTime.TryParse(value, out DateTime d2))
            {
                return new DateTime(1970, 1, 1, d2.Hour, d2.Minute, d2.Second);
            }
            return defaultValue;
        }

        /// <summary>
        /// Parse dicom age value as real age
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DicomAge ParseAsDicomAge(this string value, DicomAge defaultValue = default)
        {
            //042Y
            if (Regex.IsMatch(value, "^[\\d]{3}[\\S\\s]{1}$"))
            {
                var ageValue = value.Substring(0, 3);
                if (int.TryParse(ageValue, out int age))
                {
                    var p = value.Substring(3, 1);
                    DicomAgeMode mode;
                    switch (p)
                    {
                        case "D":
                        case "d":
                        case "天":
                            mode = DicomAgeMode.M;
                            break;
                        case "W":
                        case "w":
                        case "周":
                            mode = DicomAgeMode.W;
                            break;
                        case "M":
                        case "m":
                        case "月":
                            mode = DicomAgeMode.M;
                            break;
                        case "Y":
                        case "y":
                        case "年":
                        case "岁":
                        default:
                            mode = DicomAgeMode.Y;
                            break;
                    }

                    return new DicomAge(age, mode);

                }
            }

            return defaultValue;
        }


        /// <summary>
        /// Parse buffer to dicom encoding string
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ParseAsDicomString(this byte[] buffer, string encode = "GB18030")
        {
            return DicomEncoding.GetEncoding(encode).GetString(buffer);
        }

    }
}
