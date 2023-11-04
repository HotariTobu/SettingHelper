using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SettingTest.Setting
{
    public interface ISettingContainer
    {
        public IEnumerable<ISettingContainer> Containers { get; }
        public IEnumerable<string> Keys { get; }
        public string GetValue(string key);
        public bool SetValue(string key, string value);
    }

    public static class SettingExtensions
    {
        #region == IEnumerable ==

        private static readonly MethodInfo ToSettingStringMethod = typeof(SettingExtensions).GetMethod("ToSettingString", BindingFlags.Public | BindingFlags.Static);
        private static readonly MethodInfo FromSettingStringMethod = typeof(SettingExtensions).GetMethod("FromSettingString", BindingFlags.Public | BindingFlags.Static);

        public static string ToSettingString<T>(this IEnumerable<T> value)
        {
            if (ToSettingStringMethod == null)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(", ", value.Select(x => ToSettingStringMethod.Invoke(null, new object[] { x })));
            }
        }

        public static bool FromSettingString<T>(this ref IEnumerable<T> value, string str)
        {
            if (str == null)
            {
                return false;
            }
            else
            {
                ConstructorInfo constructor = typeof(T).GetConstructor(new Type[0]);
                List<T> result = new List<T>();
                return str.Split(", ").All(x =>
                {
                    T v = (T)constructor.Invoke(null);
                    object[] parameters = { v, x };
                    object re = (bool)FromSettingStringMethod.Invoke(null, parameters);
                    if (re is bool b)
                    {

                        return b;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
        }

        #endregion

        #region == bool ==

        public static string ToSettingString(this bool value) => value.ToString();
        public static bool FromSettingString(this ref bool value, string str) => bool.TryParse(str, out value);

        #endregion

        #region == byte ==

        public static string ToSettingString(this byte value) => value.ToString();
        public static bool FromSettingString(this ref byte value, string str) => byte.TryParse(str, out value);

        #endregion
    }
}