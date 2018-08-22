namespace Easy.Common.XAML.Sample.Messages
{
    public sealed class VMIdleState : MessageBase
    {
        /// <summary>
        /// Gets the flag indicating whether the view-model is busy or not.
        /// </summary>
        public bool IsBusy { get; }

        public VMIdleState(ViewModelBase viewModel, bool isBusy) : base(viewModel)
        {
            IsBusy = isBusy;
        }
    }
}