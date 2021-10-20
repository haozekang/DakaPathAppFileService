using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace DakaPathAppFileService.ExtendMethod
{
    public static class DateTimeExtendMethod
    {
        [Pure]
        public static string GetString(this DateTime? time)
        {
            if (time == null)
            {
                return string.Empty;
            }
            return time.Value.ToString("yyyy/MM/dd HH:mm:ss");
        }

        [Pure]
        public static string GetString(this DateTime? time, string format)
        {
            if (time == null)
            {
                return string.Empty;
            }
            if (format.IsBlank())
            {
                return GetString(time);
            }
            try
            {
                return time.Value.ToString(format);
            }
            catch
            {
                return GetString(time);
            }
        }

        [Pure]
        public static string GetDate(this DateTime? time)
        {
            if (time == null)
            {
                return string.Empty;
            }
            return GetString(time, "yyyy/MM/dd");
        }

        [Pure]
        public static string GetDate(this DateTime? time, string format)
        {
            if (time == null)
            {
                return string.Empty;
            }
            if (format.IsBlank())
            {
                return GetDate(time.Value.Date);
            }
            try
            {
                return time.Value.Date.ToString(format);
            }
            catch
            {
                return GetDate(time.Value.Date);
            }
        }

        [Pure]
        public static string GetTime(this DateTime? time)
        {
            if (time == null)
            {
                return string.Empty;
            }
            return GetString(time, "HH:mm:ss");
        }

        [Pure]
        public static string GetTime(this TimeSpan? time)
        {
            if (time == null)
            {
                return string.Empty;
            }
            return time.Value.ToString(@"hh\:mm\:ss");
        }

        [Pure]
        public static string GetTime(this DateTime? time, string format)
        {
            if (time == null)
            {
                return string.Empty;
            }
            if (format.IsBlank())
            {
                return GetTime(time.Value.TimeOfDay);
            }
            try
            {
                return time.Value.ToString(format);
            }
            catch
            {
                return GetTime(time);
            }
        }

        [Pure]
        public static bool IsBlank(this DateTime? time)
        {
            if (time.HasValue)
            {
                return false;
            }
            return true;
        }

        [Pure]
        public static void IsBlank(this DateTime? time, Action action)
        {
            if (time.HasValue)
            {
                return;
            }
            action?.Invoke();
        }

        [Pure]
        public static bool IsNotBlank(this DateTime? time)
        {
            return !IsBlank(time);
        }

        [Pure]
        public static void IsNotBlank(this DateTime? time, Action action)
        {
            if (IsBlank(time))
            {
                return;
            }
            action?.Invoke();
        }
    }
}
