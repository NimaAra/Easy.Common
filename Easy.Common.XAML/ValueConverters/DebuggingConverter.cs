namespace Easy.Common.XAML
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// A converter for helping with debugging data-binding. It allows breakpoints 
    /// to be set for the purpose of inspecting <c>XAML</c> data-binding.
    /// </summary>
    public sealed class DebuggingConverter : IValueConverter
    {
        /// <summary>
        /// This method does nothing except launching the <see cref="Debugger"/> and returning the <paramref name="value"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        /// <summary>
        /// This method does nothing except launching the <see cref="Debugger"/> and returning the <paramref name="value"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}