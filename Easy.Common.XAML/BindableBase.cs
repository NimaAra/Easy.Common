namespace Easy.Common.XAML
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A reusable implementation of <see cref="INotifyPropertyChanged"/> using <c>C# 6</c>.
    /// <remarks>
    /// See <see href="https://stackoverflow.com/a/35582811/1226568"/>.
    /// </remarks>
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies clients that a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <c>PropertyChanged</c> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the value of the given <paramref name="field"/> to the given <paramref name="value"/>.
        /// </summary>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }

            field = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}