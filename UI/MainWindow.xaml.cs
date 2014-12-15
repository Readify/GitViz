using System;
using System.Windows;

namespace UI
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
