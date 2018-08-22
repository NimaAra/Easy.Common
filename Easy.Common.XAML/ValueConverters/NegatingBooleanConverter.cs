namespace Easy.Common.XAML
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// An abstraction for negating a <see cref="bool"/>.
    /// </summary>
    public sealed class NegatingBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Negates the given <see cref="bool"/> <paramref name="value"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;

        /// <summary>
        /// Negates the given <see cref="bool"/> <paramref name="value"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;
    }
}