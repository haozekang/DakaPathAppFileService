using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DakaPathAppFileService.ExtendMethod
{
    public static class GuidExtendMethod
    {
        public static bool IsBlank(this Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public static bool IsBlank(this Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public static void IsBlank(this Guid guid, Action action, Action uaction = null)
        {
            if (guid != Guid.Empty)
            {
                uaction?.Invoke();
                return;
            }
            action?.Invoke();
        }

        public static void IsBlank(this Guid? guid, Action action, Action uaction = null)
        {
            if (guid != null && guid != Guid.Empty)
            {
                uaction?.Invoke();
                return;
            }
            action?.Invoke();
        }

        public static bool IsNotBlank(this Guid guid)
        {
            return !IsBlank(guid);
        }

        public static bool IsNotBlank(this Guid? guid)
        {
            return !IsBlank(guid);
        }

        public static void IsNotBlank(this Guid guid, Action action, Action uaction = null)
        {
            if (IsBlank(guid))
            {
                uaction?.Invoke();
                return;
            }
            action?.Invoke();
        }

        public static void IsNotBlank(this Guid? guid, Action action, Action uaction = null)
        {
            if (IsBlank(guid))
            {
                uaction?.Invoke();
                return;
            }
            action?.Invoke();
        }
    }
}
