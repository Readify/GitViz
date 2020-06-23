using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var wnd = new MainWindow();
            wnd.Show();
            if (e.Args.Length == 1)
                wnd.TxtRepositoryPath.Text = e.Args[0];
        }

    }
}
