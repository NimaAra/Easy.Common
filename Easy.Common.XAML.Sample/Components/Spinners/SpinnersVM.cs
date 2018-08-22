namespace Easy.Common.XAML.Sample.Components.Spinners
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Easy.MessageHub;

    public sealed class SpinnersVM : ViewModelBase
    {
        #region DataBinding

        private int _count;
        public int Count
        {
            get => _count;
            set => SetField(ref _count, value);
        }
        
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetField(ref _selectedDate, value);
        }

        public ICommand IncrementCommand => new CustomCommand(ClickImpl);
        public ICommand DelayCommand => new CustomCommand(DelayImpl);

        #endregion

        public SpinnersVM(IMessageHub hub) : base(hub)
        {
            Init();
        }

        private void Init()
        {
            Count = 1;
            SelectedDate = DateTime.Now;
        }

        private void ClickImpl()
        {
            Count++;
        }

        private async void DelayImpl()
        {
            IsBusy = true;
            await Task.Delay(3000);
            IsBusy = false;
        }
    }
}