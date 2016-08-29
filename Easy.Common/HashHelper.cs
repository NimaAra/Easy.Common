namespace Easy.Common
{
    using System;

    /// <summary>
    /// Helper class for generating hash code
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T>(T param)
        {
            return GetHashCodeInternal(param);
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2>(T1 param1, T2 param2)
        {
            unchecked
            {
                return GetHashCodeInternal(param1) * GetHashCodeInternal(param2);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            unchecked
            {
                return GetHashCode(param1, param2) * GetHashCodeInternal(param3);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3) * GetHashCodeInternal(param4);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4) * GetHashCodeInternal(param5);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4, param5) * GetHashCodeInternal(param6);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4, param5, param6) * GetHashCodeInternal(param7);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4, param5, param6, param7) * GetHashCodeInternal(param8);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4, param5, param6, param7, param8) * GetHashCodeInternal(param9);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        {
            unchecked
            {
                return GetHashCode(param1, param2, param3, param4, param5, param6, param7, param8, param9) * GetHashCodeInternal(param10);
            }
        }

        /// <summary>
        /// Generates a hash code using the given <paramref name="properties"/> 
        /// (credit Jon Skeet @ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode)
        /// </summary>
        public static int GetHashCode<T>(params T[] properties)
        {
            var hash = 31;

            unchecked
            {
                Array.ForEach(properties, prop =>
                {
                    if (!prop.Equals(default(T))) { hash = hash * GetHashCodeInternal(prop); }
                });

                return hash;
            }
        }

        private static int GetHashCodeInternal<T>(T param)
        {
            var hash = 31;
            const int Prime = 486187739;

            if (!param.Equals(default(T)))
            {
                unchecked
                {
                    hash = hash * Prime + param.GetHashCode();
                }
            }
            return hash;
        }
    }
}