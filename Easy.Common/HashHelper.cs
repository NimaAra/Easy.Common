namespace Easy.Common
{
    using Easy.Common.Extensions;

    /// <summary>
    /// Helper class for generating hash code.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// A relatively large prime number.
        /// </summary>
        private const int PrimeNumber = 486187739;

        /// <summary>
        /// 31 is shift and subtract hence very fast.
        /// </summary>
        private const int DefaultHashValue = 31;

        /// <summary>
        /// Gets the hash using the given parameters.
        /// </summary>
        public static int GetHashCode<T>(T param)
        {
            return param.IsDefault() ? 0 : param.GetHashCode();
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2>(T1 param1, T2 param2)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                return hash * PrimeNumber + GetHashCode(param2);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                return hash * PrimeNumber + GetHashCode(param3);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                return hash * PrimeNumber + GetHashCode(param4);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                return hash * PrimeNumber + GetHashCode(param5);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                return hash * PrimeNumber + GetHashCode(param6);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                return hash * PrimeNumber + GetHashCode(param7);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                return hash * PrimeNumber + GetHashCode(param8);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                hash = hash * PrimeNumber + GetHashCode(param8);
                return hash * PrimeNumber + GetHashCode(param9);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                hash = hash * PrimeNumber + GetHashCode(param8);
                hash = hash * PrimeNumber + GetHashCode(param9);
                return hash * PrimeNumber + GetHashCode(param10);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                hash = hash * PrimeNumber + GetHashCode(param8);
                hash = hash * PrimeNumber + GetHashCode(param9);
                hash = hash * PrimeNumber + GetHashCode(param10);
                return hash * PrimeNumber + GetHashCode(param11);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                hash = hash * PrimeNumber + GetHashCode(param8);
                hash = hash * PrimeNumber + GetHashCode(param9);
                hash = hash * PrimeNumber + GetHashCode(param10);
                hash = hash * PrimeNumber + GetHashCode(param11);
                return hash * PrimeNumber + GetHashCode(param12);
            }
        }

        /// <summary>
        /// Gets the hash using the given parameters.
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                hash = hash * PrimeNumber + GetHashCode(param1);
                hash = hash * PrimeNumber + GetHashCode(param2);
                hash = hash * PrimeNumber + GetHashCode(param3);
                hash = hash * PrimeNumber + GetHashCode(param4);
                hash = hash * PrimeNumber + GetHashCode(param5);
                hash = hash * PrimeNumber + GetHashCode(param6);
                hash = hash * PrimeNumber + GetHashCode(param7);
                hash = hash * PrimeNumber + GetHashCode(param8);
                hash = hash * PrimeNumber + GetHashCode(param9);
                hash = hash * PrimeNumber + GetHashCode(param10);
                hash = hash * PrimeNumber + GetHashCode(param11);
                hash = hash * PrimeNumber + GetHashCode(param12);
                return hash * PrimeNumber + GetHashCode(param13);
            }
        }

        /// <summary>
        /// Generates a hash code using the given <paramref name="parameters"/> 
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// <remarks>
        /// See <see href="@ http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode"/>
        /// </remarks>
        /// </summary>
        public static int GetHashCode<T>(params T[] parameters)
        {
            unchecked
            {
                var hash = DefaultHashValue;
                // ReSharper disable once LoopCanBeConvertedToQuery  [PERF]
                // ReSharper disable once ForCanBeConvertedToForeach [PERF]
                for (var i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    hash = hash * PrimeNumber + GetHashCode(param);
                }
                return hash;
            }
        }
    }
}