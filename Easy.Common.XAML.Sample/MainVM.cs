namespace Easy.Common.XAML.Sample
{
    using Easy.Common.XAML.Sample.Messages;
    using Easy.MessageHub;

    public sealed class MainVM : ViewModelBase
    {
        #region DataBinding

        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        #endregion

        public MainVM(IMessageHub hub) : base(hub)
        {
            Init();
        }

        private void Init()
        {
            Title = "Sample WPF Application";
            Hub.Subscribe<MessageBase>(OnUIMessage);
        }

        private void OnUIMessage(MessageBase message)
        {
            if (message.Sender == this) { return; }
            
            if (message is VMIdleState vmState)
            {
                IsBusy = vmState.IsBusy;
            }
        }
    }
}