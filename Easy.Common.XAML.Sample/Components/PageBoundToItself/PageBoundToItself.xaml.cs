namespace Easy.Common.XAML.Sample.Components.PageBoundToItself
{
    /// <summary>
    /// Interaction logic for PageBoundToItself.xaml
    /// </summary>
    public partial class PageBoundToItself
    {
        #region DataBinding

        public string Message => "Some Message";

        #endregion
        
        public PageBoundToItself()
        {
            InitializeComponent();
        }
    }
}
