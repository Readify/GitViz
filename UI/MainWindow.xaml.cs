using System;
using System.Windows;

namespace UI
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowPlacement();
        }

        private void WindowPlacement()
        {
            Top = SystemParameters.WorkArea.Top;
            Left = SystemParameters.WorkArea.Left;
        }

        private void BtnOpenRepository_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    SelectedPath = string.IsNullOrWhiteSpace(TxtRepositoryPath.Text)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        : TxtRepositoryPath.Text
                };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TxtRepositoryPath.Text = dialog.SelectedPath;
            }
        }

        private void Graph_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = Math.Min(Math.Max(graph.ActualWidth + 80, 400), SystemParameters.PrimaryScreenWidth - Left);
            Height = Math.Min(Math.Max(graph.ActualHeight + grid.RowDefinitions[0].ActualHeight + 80, 200), SystemParameters.PrimaryScreenHeight - Top);
        }
    }
}
