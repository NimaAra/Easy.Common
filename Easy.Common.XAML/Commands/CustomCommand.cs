namespace Easy.Common.XAML.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// An abstraction for handling custom commands.
    /// </summary>
    public sealed class CustomCommand : ICommand
    {
        private readonly Action<object> _actionWithParam;

        /// <summary>
        /// Creates an instance of the <see cref="CustomCommand"/>.
        /// </summary>
        public CustomCommand(Action action)
        {
            _actionWithParam = _ => action();
        }

        /// <summary>
        /// Creates an instance of the <see cref="CustomCommand"/>.
        /// </summary>
        public CustomCommand(Action<object> actionWithParam)
        {
            _actionWithParam = actionWithParam;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to <see langword="null" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to <see langword="null" />.
        /// </param>
        public void Execute(object parameter) => _actionWithParam(parameter);

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}