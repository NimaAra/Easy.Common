namespace Easy.Common.XAML.Sample
{
    public sealed class MainVM : BindableBase
    {
        #region DataBinding

        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        #endregion

        public MainVM()
        {
            Title = "Sample WPF Application";
        }
    }
}