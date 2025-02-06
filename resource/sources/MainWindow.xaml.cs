using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;



namespace EasyFileMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {

            if (DataContext is MainViewModel viewModel)
            {
                e.Cancel = true; // ウィンドウを閉じる動作をキャンセル
                viewModel.WindowCloseMangageCommand.Execute(this);

            }
        }


    }
}