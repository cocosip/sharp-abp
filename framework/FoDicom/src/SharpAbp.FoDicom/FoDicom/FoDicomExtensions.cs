using FellowOakDicom;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpAbp.FoDicom
{
    /// <summary>扩展
    /// </summary>
    public static class FoDicomExtensions
    {

        /// <summary>获取日期类型时间
        /// </summary>
        public static DateTime? GetDate(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1900, 1, 1);
            }
            var dateStringValue = dataset.GetSingleValueOrDefault(tag, "");
            if (dateStringValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }
            //将字符串转换成时间
            if (Regex.IsMatch(dateStringValue, @"^[\d]{8}$"))
            {
                if (DateTime.TryParseExact(dateStringValue, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return d1;
                }
            }
            if (DateTime.TryParse(dateStringValue, out DateTime d2))
            {
                return d2;
            }
            return defaultValue;
        }


        /// <summary>获取时间类型
        /// </summary>
        public static DateTime? GetTime(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1900, 1, 1);
            }
            var timeStringValue = dataset.GetSingleValueOrDefault(tag, "");
            if (timeStringValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(timeStringValue, @"^[\d]{6}$"))
            {
                if (DateTime.TryParseExact(timeStringValue, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1900, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(timeStringValue, @"^[\d]{4}$"))
            {
                if (DateTime.TryParseExact(timeStringValue, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1900, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(timeStringValue, @"^[\d]{6}\.[\d]{1,7}$"))
            {

                var f = "HHmmss.".PadRight(timeStringValue.Length, 'f');
                if (DateTime.TryParseExact(timeStringValue, f, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1900, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }

            if (DateTime.TryParse(timeStringValue, out DateTime d2))
            {
                return new DateTime(1900, 1, 1, d2.Hour, d2.Minute, d2.Second);
            }
            return defaultValue;
        }

        /// <summary>获取中文字符串
        /// </summary>
        public static string GetChineseString(this DicomDataset dataset, DicomTag tag, string encode = "utf-8", string defaultValue = "")
        {
            if (!dataset.Contains(tag))
            {
                return defaultValue;
            }
            var data = dataset.GetValues<byte>(tag);
            return Encoding.GetEncoding(encode).GetString(data);
        }
    }
}
