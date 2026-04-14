using System.Windows;

namespace ExampleWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        { }

        protected override void OnStartup(StartupEventArgs e)
        {
            var locator = new RFBCodeWorks.Mvvm.DialogServices.DialogService();
            base.OnStartup(e);
        }

    }
}
