using FellowOakDicom;
using System;
using System.Globalization;
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
            var dateStringValue = dataset.GetSingleValueOrDefault(tag, "");
            if (dateStringValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }
            //将字符串转换成时间
            if (Regex.IsMatch(dateStringValue, @"^[\d]{8}$"))
            {
                if (DateTime.TryParseExact(dateStringValue, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
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
            var timeStringValue = dataset.GetSingleValueOrDefault(tag, "");
            if (timeStringValue.IsNullOrWhiteSpace())
            {
                return defaultValue;
            }

            if (Regex.IsMatch(timeStringValue, @"^[\d]{6}$"))
            {
                if (DateTime.TryParseExact(timeStringValue, "HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return d1;
                }
            }
            else if (Regex.IsMatch(timeStringValue, @"^[\d]{4}$"))
            {
                if (DateTime.TryParseExact(timeStringValue, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime d1))
                {
                    return d1;
                }
            }

            if (DateTime.TryParse(timeStringValue, out DateTime d2))
            {
                return d2;
            }
            return defaultValue;
        }

    }
}
