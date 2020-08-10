using FellowOakDicom;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpAbp.Abp.FoDicom
{
    public static class FoDicomExtensions
    {
        /// <summary>
        /// 获取日期类型时间
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? GetDate(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
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

        /// <summary>
        /// 获取时间类型
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="tag"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? GetTime(this DicomDataset dataset, DicomTag tag, DateTime? defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = new DateTime(1970, 1, 1);
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
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(timeStringValue, @"^[\d]{4}$"))
            {
                if (DateTime.TryParseExact(timeStringValue, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }
            else if (Regex.IsMatch(timeStringValue, @"^[\d]{6}\.[\d]{1,7}$"))
            {

                var f = "HHmmss.".PadRight(timeStringValue.Length, 'f');
                if (DateTime.TryParseExact(timeStringValue, f, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return new DateTime(1970, 1, 1, d1.Hour, d1.Minute, d1.Second);
                }
            }

            if (DateTime.TryParse(timeStringValue, out DateTime d2))
            {
                return new DateTime(1970, 1, 1, d2.Hour, d2.Minute, d2.Second);
            }
            return defaultValue;
        }


        /// <summary>
        /// 使用指定的编码格式对二进制数据进行解码
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
                var value = DicomEncoding.GetEncoding(encode).GetString(buffer);
                return value;
            }
            return "";
        }

    }
}
