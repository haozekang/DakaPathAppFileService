using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace DakaPathAppFileService.ExtendMethod
{
    public static class LongExtendMethod
    {
        /// <summary>
        /// 由10位时间戳(秒)转换为DateTime类型
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static DateTime? TimeStampToDateTime(this long timeStamp)
        {
            if (timeStamp < 0)
            {
                return null;
            }
            try
            {
                return DateTime.UnixEpoch.AddSeconds(timeStamp + 8 * 60 * 60);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 由13位时间戳(毫秒)转换为DateTime类型
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static DateTime? LongTimeStampToDateTime(this long longTimeStamp)
        {
            if (longTimeStamp < 0)
            {
                return null;
            }
            try
            {
                return DateTime.UnixEpoch.AddMilliseconds(longTimeStamp + 8 * 60 * 60 + 1000);
            }
            catch
            {
                return null;
            }
        }
    }
}
