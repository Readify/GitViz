using System;
using System.Windows;
using System.Windows.Interop;
using GitViz.Logic;

namespace UI
{
    public partial class MainWindow
    {
        public double DpiHeightFactor { get; set; }

        public double DpiWidthFactor { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SetDpiFactor();
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
            var viewModel = (ViewModel)DataContext;
            if (!viewModel.IsNewRepository)
                return;
            viewModel.IsNewRepository = false;
            ResizeWindowDependingOnGraphSize();
        }

        public System.Windows.Forms.Screen GetCurrentScreen()
        {
            return System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
        }

        public void SetDpiFactor()
        {
            var mainWindowPresentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (mainWindowPresentationSource != null)
            {
                var transformToDevice = mainWindowPresentationSource.CompositionTarget.TransformToDevice;
                DpiWidthFactor = transformToDevice.M11;
                DpiHeightFactor = transformToDevice.M22;
            }
            else
            {
                DpiWidthFactor = 1;
                DpiHeightFactor = 1;
            }
        }


        private void ResizeWindowDependingOnGraphSize()
        {
            var currentScreen = GetCurrentScreen();
            Width = DpiWidthFactor * Math.Min(Math.Max(graph.ActualWidth + 80, 400), currentScreen.Bounds.Width - Left + currentScreen.Bounds.Left);
            Height = DpiWidthFactor * Math.Min(Math.Max(graph.ActualHeight + grid.RowDefinitions[0].ActualHeight + 80, 200), currentScreen.Bounds.Height - Top + currentScreen.Bounds.Top);
        }

        private void BtnResizeWindow_OnClick(object sender, RoutedEventArgs e)
        {
            ResizeWindowDependingOnGraphSize();
        }
    }
}
