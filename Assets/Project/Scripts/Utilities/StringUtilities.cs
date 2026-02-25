using System;
using UnityEngine;

namespace BigProject.Utilities
{
    public static class StringUtilities
    {
        /// <returns>Field name in enum.</returns>
        public static string GetEnumValueName<T>(object value) => Enum.GetName(typeof(T), value);
    }
}
