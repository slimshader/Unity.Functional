using System;

namespace Bravasoft.Functional
{
    public static class Check
    {
        public static class CheckImpl<T>
        {
            public static bool IsNull(T value) =>
                typeof(T).IsValueType
                ? false
                : (object)value == null;
        }

        public static bool IsNull<T>(T value) => CheckImpl<T>.IsNull(value);

        public static T AssureNotNull<T>(T value, string name)
        {
            if (IsNull(value))
                throw new ArgumentNullException(name);

            return value;
        }
    }
}
