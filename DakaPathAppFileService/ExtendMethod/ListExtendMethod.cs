using System;
using System.Collections;
using System.Collections.Generic;

namespace DakaPathAppFileService.ExtendMethod
{
    public static class ListExtendMethod
    {
        public static string Join(this IList list, string split = ",")
        {
            if (list == null)
            {
                return string.Empty;
            }
            if (list.Count <= 0)
            {
                return string.Empty;
            }
            string str = "";
            foreach (var x in list)
            {
                str += str.IsBlank() ? x : $"{split}{x}";
            }
            return str;
        }

        public static bool IsBlank(this IList list)
        {
            if (list == null)
            {
                return true;
            }
            if (list.Count <= 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsNotBlank(this IList list)
        {
            if (list == null)
            {
                return false;
            }
            if (list.Count <= 0)
            {
                return false;
            }
            return true;
        }

        public static void IsNotBlank(this IList list, Action action)
        {
            if (list == null)
            {
                return;
            }
            if (list.Count <= 0)
            {
                return;
            }
            action?.Invoke();
        }

        public static void IsNotBlank<T>(this IList<T> list, Action<T> action)
        {
            if (list == null)
            {
                return;
            }
            if (list.Count <= 0)
            {
                return;
            }
            foreach (T o in list)
            {
                action?.Invoke(o);
            }
        }
    }
}
