namespace Easy.Common.XAML.ValueConverters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An abstraction for negating a <see cref="bool"/>.
    /// </summary>
    public class NegatingBooleanConverter : BaseValueConverter
    {
        /// <summary>
        /// Negates the given <see cref="bool"/> <paramref name="value"/>.
        /// </summary>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;

        /// <summary>
        /// Negates the given <see cref="bool"/> <paramref name="value"/>.
        /// </summary>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;
    }
}