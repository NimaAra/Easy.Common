namespace Easy.Common.XAML
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// An abstraction for converting between <see cref="bool"/> and <see cref="Style"/>.
    /// </summary>
    public sealed class BooleanToStyleConverter : IValueConverter
    {
        /// <summary>
        /// The key for the <see cref="Style"/> to be returned if the value is <c>True</c>.
        /// </summary>
        public string TrueStyleKey { get; set; }

        /// <summary>
        /// The key for the <see cref="Style"/> to be returned if the value is <c>False</c>.
        /// </summary>
        public string FalseStyleKey { get; set; }

        /// <summary>
        /// Converts the given <paramref name="value"/> to a <see cref="Style"/> specified 
        /// by <see cref="TrueStyleKey"/> and <see cref="FalseStyleKey"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? GetStyle(TrueStyleKey) : GetStyle(FalseStyleKey);
        }

        /// <summary>
        /// This back conversion is not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Style GetStyle(string key) => (Style)Application.Current.Resources[key];
    }
}