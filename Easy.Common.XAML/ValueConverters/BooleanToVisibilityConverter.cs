namespace Easy.Common.XAML.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// An abstraction for converting between <see cref="bool"/> and <see cref="Visibility"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : BaseValueConverter
    {
        /// <summary>
        /// Gets or set the <see cref="TrueValue"/>.
        /// </summary>
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        
        /// <summary>
        /// Gets or set the <see cref="FalseValue"/>.
        /// </summary>
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// Converts the given <paramref name="value"/>.
        /// </summary>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) { return null; }
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/>.
        /// </summary>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue)) { return true; }
            if (Equals(value, FalseValue)) { return false; }
            return null;
        }
    }
}