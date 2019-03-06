using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MSClient.Views
{
    public class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            //if (AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().IsDesktop)
            //{
            //    IList tabItems = ((IList)this.FindControl<TabControl>("Sidebar").Items);
            //    tabItems.Add(new TabItem()
            //    {
            //        Header = "Dialogs",
            //        Content = new DialogsPage()
            //    });
            //    tabItems.Add(new TabItem()
            //    {
            //        Header = "Screens",
            //        Content = new ScreenPage()
            //    });

            //}
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
