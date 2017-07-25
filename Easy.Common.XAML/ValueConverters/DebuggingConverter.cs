namespace Easy.Common.XAML.ValueConverters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A converter for helping with debugging data-binding. It allows breakpoints 
    /// to be set for the purpose of inspecting <c>XAML</c> data-binding.
    /// </summary>
    public sealed class DebuggingConverter : BaseValueConverter
    {
        /// <summary>
        /// This method does nothing except returning the <paramref name="value"/>.
        /// </summary>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value;

        /// <summary>
        /// This method does nothing except returning the <paramref name="value"/>.
        /// </summary>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}