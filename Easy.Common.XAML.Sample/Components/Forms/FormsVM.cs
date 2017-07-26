namespace Easy.Common.XAML.Sample.Components.Forms
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Easy.Common.XAML.Commands;

    public sealed class FormsVM : BindableBase
    {
        #region DataBinding

        private bool _isIdle;
        public bool IsIdle
        {
            get => _isIdle;
            set
            {
                SetField(ref _isIdle, value);
                SetField(ref _isBusy, !_isIdle, nameof(IsBusy));
            }
        }

        private bool _isBusy;
        public bool IsBusy => _isBusy;

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

        public FormsVM()
        {
            Count = 1;
            SelectedDate = DateTime.Now;
            IsIdle = true;
        }

        private void ClickImpl()
        {
            Count++;
        }

        private async void DelayImpl()
        {
            IsIdle = false;
            await Task.Delay(3000);
            IsIdle = true;
        }
    }
}