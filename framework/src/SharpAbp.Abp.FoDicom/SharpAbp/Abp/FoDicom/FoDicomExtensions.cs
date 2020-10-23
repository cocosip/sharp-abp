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
            var dateValue = dataset.GetSingleValueOrDefault(tag, "");
            return dateValue.ParseAsDicomDate(defaultValue);
        }

        /// <summary>
        /// Parse dicom string format date as DateTime
        /// </summary>
        /// <param name="stringDateValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ParseAsDicomDate(this string stringDateValue, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
            }

            if (stringDateValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(stringDateValue, @"^[\d]{8}$"))
            {
                if (DateTime.TryParseExact(stringDateValue, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return d1;
                }
            }
            if (DateTime.TryParse(stringDateValue, out DateTime d2))
            {
                return d2;
            }
            return defaultValue;
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
            var stringTimeValue = dataset.GetSingleValueOrDefault(tag, "");
            return stringTimeValue.ParseAsDicomTime(defaultValue);
        }

        /// <summary>
        /// Parse dicom string format datetime as DateTime
        /// </summary>
        /// <param name="stringTimeValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ParseAsDicomTime(this string stringTimeValue, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
            }

            if (stringTimeValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(stringTimeValue, @"^[\d]{6}$"))
            {
                if (DateTime.TryParseExact(stringTimeValue, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(stringTimeValue, @"^[\d]{4}$"))
            {
                if (DateTime.TryParseExact(stringTimeValue, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(stringTimeValue, @"^[\d]{6}\.[\d]{1,7}$"))
            {

                var f = "HHmmss.".PadRight(stringTimeValue.Length, 'f');
                if (DateTime.TryParseExact(stringTimeValue, f, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }

            if (DateTime.TryParse(stringTimeValue, out DateTime d2))
            {
                return new DateTime(1970, 1, 1, d2.Hour, d2.Minute, d2.Second);
            }
            return defaultValue;
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
