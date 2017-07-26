namespace Easy.Common.XAML.Sample
{
    using Easy.Common.XAML.Sample.Components.Forms;

    public sealed class VMLocator
    {
        public MainVM MainVM => new MainVM();
        public FormsVM FormsVM => new FormsVM();
    }
}