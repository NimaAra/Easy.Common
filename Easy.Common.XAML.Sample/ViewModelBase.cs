namespace Easy.Common.XAML.Sample
{
    using Easy.Common.XAML.Sample.Messages;
    using Easy.MessageHub;

    public abstract class ViewModelBase : BindableBase
    {
        protected IMessageHub Hub { get; }

        protected ViewModelBase(IMessageHub hub)
        {
            Hub = hub;
        }

        #region DataBinding

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetField(ref _isBusy, value);
                PublishIdleState(IsBusy);
            }
        }

        #endregion

        private void PublishIdleState(bool isBusy)
        {
            Hub.Publish(new VMIdleState(this, isBusy));
        }
    }
}