namespace Easy.Common.EasyComparer
{
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using Easy.Common.Extensions;

    /// <summary>
    /// An abstraction for representing the difference between objects.
    /// </summary>
    public sealed class Variance
    {
        internal Variance(PropertyInfo property, object leftValue, object rightValue)
        {
            Property = property;
            LeftValue = leftValue;
            RightValue = rightValue;

            Varies = VariesImpl();
        }

        private bool VariesImpl()
        {
            var isSequence = Property.PropertyType.IsSequence(out SequenceType _);

            if (LeftValue is null) { return RightValue != null; }
            if (RightValue is null) { return true; }

            if (!isSequence) { return !Equals(LeftValue, RightValue); }

            var left = ((IEnumerable)LeftValue).Cast<object>();
            var right = ((IEnumerable)RightValue).Cast<object>();
            return !left.SequenceEqual(right);
        }

        /// <summary>
        /// Gets the property to which <see cref="LeftValue"/> and <see cref="RightValue"/>
        /// have been retrieved.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object LeftValue { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object RightValue { get; }

        /// <summary>
        /// Gets the flag indicating whether the <see cref="LeftValue"/> 
        /// and <see cref="RightValue"/> are different.
        /// </summary>
        public bool Varies { get; }
    }
}