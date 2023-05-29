using System;

namespace Bravasoft.Functional
{
    public static class Check
    {
        private static class CheckImpl<T>
        {
            public static bool IsNull(T value) =>
                typeof(T).IsValueType
                ? false
                : (object)value == null;
        }

        /// <summary>
        /// Checks whether the provided value is null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is null; otherwise, false.</returns>
        public static bool IsNull<T>(T value) => CheckImpl<T>.IsNull(value);

        /// <summary>
        /// Ensures that the provided value is not null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the value (used in the exception message).</param>
        /// <returns>The non-null value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
        public static T AssureNotNull<T>(T value, string name)
        {
            if (IsNull(value))
                throw new ArgumentNullException(name);

            return value;
        }

    }
}
