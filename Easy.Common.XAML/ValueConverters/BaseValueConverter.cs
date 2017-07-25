namespace Easy.Common.XAML.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// An abstraction for simplifying the usage of <see cref="IValueConverter"/>s.
    /// <remarks>
    /// See <see href="https://wpftutorial.net/ValueConverters.html"/>.
    /// </remarks>
    /// </summary>
    public abstract class BaseValueConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as 
        /// the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        /// <returns>The object value to set on the property where the extension is applied. </returns>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}